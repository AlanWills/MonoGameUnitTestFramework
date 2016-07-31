using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace UnitTestFramework
{
    /// <summary>
    /// This class does not do explicit testing, but is rather a schema for generating repetitive unit tests.
    /// A virtual function defines the test outline which will be generated for a user defined set of inputs.
    /// </summary>
    public abstract class AutogenUnitTest : UnitTest
    {
        // We only want this class to see the internals of the TestData class.
        // The only public interface should be the BuildTest and Valid/InvalidParams.
        #region Test Data

        public class TestData
        {
            internal string MethodName { get; private set; }
            internal List<object[]> ValidParameters { get; private set; }
            internal List<object[]> InvalidParameters { get; private set; }
            internal Type TestAttribute { get; private set; }
            internal string AttributeParamsString { get; private set; }

            internal TestData(string methodName, Type testAttributeType, params object[] attributeParams)
            {
                MethodName = methodName;
                ValidParameters = new List<object[]>();
                InvalidParameters = new List<object[]>();
                TestAttribute = testAttributeType;
                AttributeParamsString = attributeParams.CreateParameterString();
            }

            public TestData ValidParams(params object[] validParameters)
            {
                ValidParameters.Add(validParameters);

                return this;
            }

            public TestData InvalidParams(params object[] validParameters)
            {
                InvalidParameters.Add(validParameters);

                return this;
            }
        }

        #endregion

        #region Properties and Fields

        /// <summary>
        /// The destination file path for the C# file that will be generated when this class autogenerates it's tests
        /// </summary>
        private string fullDestinationFilePath;
        protected string FullDestinationFilePath
        {
            get { return fullDestinationFilePath; }
            set
            {
                fullDestinationFilePath = Path.Combine(AutoGenDirectory, value);
            }
        }

        /// <summary>
        /// A shortcut to allow us to easily change the indentation of our file.
        /// </summary>
        protected string CurrentIndent { get; set; }

        public static string AutoGenDirectory { get; set; }

        private StreamWriter Writer { get; set; }

        /// <summary>
        /// A list of all of the test data we have created for the autogen test we will write
        /// </summary>
        private List<TestData> AutogenTestData { get; set; }

        /// <summary>
        /// A list of 'using' assemblies we will print at the top of the autogen unit test
        /// </summary>
        private List<string> RequiredAssemblies { get; set; }

        #endregion

        public AutogenUnitTest()
        {
            AutogenTestData = new List<TestData>();
            RequiredAssemblies = new List<string>()
            {
                "_2DEngine",
                "UnitTestFramework"
            };
        }

        #region Virtual Functions

        /// <summary>
        /// This function is where you specify the tests you wish to write.
        /// Use the Build function, any inputs you wish and as many times as you like.
        /// The base class will take care of the generation of the rest of the class code, you just need to specify data for your tests.
        /// </summary>
        protected virtual void BuildTests() { }

        #endregion

        #region Code Generation Functions


        /// <summary>
        /// This is the function responsible for creating the string unit test using the valid and invalid parameters for each test data.
        /// Iterates through the test data we have built and writes them into a test.
        /// </summary>
        public void WriteTests()
        {
            foreach (TestData testData in AutogenTestData)
            {
                foreach (object[] parameters in testData.ValidParameters)
                {
                    WriteTest(testData, parameters, true);
                }

                foreach (object[] parameters in testData.InvalidParameters)
                {
                    WriteTest(testData, parameters, false);
                }
            }
        }

        /// <summary>
        /// Writes an individual test using the test data and specific parameters array
        /// </summary>
        /// <param name="testData"></param>
        /// <param name="parameters"></param>
        private void WriteTest(TestData testData, object[] parameters, bool shouldPass)
        {
            string parametersString = "[Parameters(" + shouldPass.ToString().ToLower();
            if (parameters.Length > 0)
            {
                parametersString += ", ";
            }

            string parameterStringForFunctionName = "";
            for (int i = 0; i < parameters.Length; i++)
            {
                parameterStringForFunctionName += parameters[i].ToString();
                parametersString += parameters[i].CreateParameterString();

                if (i < parameters.Length - 1)
                {
                    parameterStringForFunctionName += "_";
                    parametersString += ", ";
                }
            }

            parameterStringForFunctionName = parameterStringForFunctionName.Replace("\\", "_");
            parameterStringForFunctionName = parameterStringForFunctionName.Replace(" ", "");
            parameterStringForFunctionName = parameterStringForFunctionName.Replace("'", "");
            parameterStringForFunctionName = parameterStringForFunctionName.Replace(".xml", "");

            parameterStringForFunctionName += "_" + (string)testData.TestAttribute.GetField("Description").GetValue(null);
            if (!shouldPass)
            {
                parameterStringForFunctionName += "_Fail";
            }

            string attributeParamsString = testData.AttributeParamsString;
            if (!string.IsNullOrEmpty(attributeParamsString))
            {
                attributeParamsString = ", " + attributeParamsString;
            }

            WriteLine("[" + testData.TestAttribute.Name + "(\"" + testData.MethodName + "\"" + attributeParamsString + ")]");
            WriteLine(parametersString + ")]");
            WriteLine("public void Test_" + testData.MethodName + "_" + parameterStringForFunctionName + "() { }");
            WriteLine("");
        }

        /// <summary>
        /// Builder pattern for specifying TestData.
        /// The only public interface onto autogen test data to make the process as clear and simple as possible
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="testName"></param>
        /// <returns></returns>
        protected TestData BuildTest<T>(string testName, params object[] attributeParameters) where T : TestPassIf
        {
            // Need to check that enough attribute parameters have been passed for the attribute T
            Type[] constructorTypes = new Type[attributeParameters.Length + 1];
            constructorTypes[0] = typeof(string);   // Method name argument

            for (int i = 0; i < attributeParameters.Length; i++)
            {
                constructorTypes[i + 1] = attributeParameters[i].GetType();

                // Add required assemblies or types
                Type attrAsType = attributeParameters[i] as Type;
                if (attrAsType != null)
                {
                    string assembly = attrAsType.FullName.Replace(attrAsType.Name, "");
                    assembly = assembly.Remove(assembly.Length - 1);   // Remove the "." at the end

                    if (!RequiredAssemblies.Exists(x => x == assembly))
                    {
                        // Add if required - if already added no need to repeat
                        RequiredAssemblies.Add(assembly);
                    }
                }
            }

            Debug.Assert(typeof(T).GetConstructor(constructorTypes) != null);
            TestData testData = new TestData(testName, typeof(T), attributeParameters);
            AutogenTestData.Add(testData);

            return testData;
        }

        public void WriteLine(string text)
        {
            Writer.WriteLine(CurrentIndent + text);
        }

        /// <summary>
        /// Call this function to generate the code for your test class.
        /// </summary>
        public void Generate()
        {
            // Create our file stream - overwrite any existing files if they exist
            using (Writer = new StreamWriter(FullDestinationFilePath, false))
            {
                // Build tests before doing ANY writing because we may need to add extra assemblies etc.
                BuildTests();

                // TODO: Hard coded namespace - not good
                foreach (string assembly in RequiredAssemblies)
                {
                    WriteLine("using " + assembly + ";");
                }

                WriteLine("");
                WriteLine("namespace UnitTestGameProject");
                WriteLine("{");

                CurrentIndent += "\t";

                WriteLine("/// <summary>");
                WriteLine("/// Auto-generated unit tests for class " + TestClassAttr.TestingClass.Name);
                WriteLine("/// Do not edit by hand as changes will not be preserved between regenerations");
                WriteLine("/// <summary>");
                WriteLine("[TestClassForType(typeof(" + TestClassAttr.TestingClass.Name + "))]");
                WriteLine("public class Test" + TestClassAttr.TestingClass.Name + "Autogen : UnitTest");
                WriteLine("{");

                CurrentIndent += "\t";

                WriteTests();

                CurrentIndent = "\t";

                WriteLine("}");

                CurrentIndent = "";

                WriteLine("}");
            }
        }

        #endregion
    }
}
