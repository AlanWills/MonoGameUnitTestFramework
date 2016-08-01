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
            internal string FunctionName { get; private set; }
            internal List<List<object>> ValidParameters { get; private set; }
            internal List<List<object>> InvalidParameters { get; private set; }
            internal Type TestAttribute { get; private set; }
            internal string AttributeParamsString { get; private set; }

            internal TestData(string functionName, Type testAttributeType, params object[] attributeParams)
            {
                FunctionName = functionName;
                ValidParameters = new List<List<object>>();
                InvalidParameters = new List<List<object>>();
                TestAttribute = testAttributeType;
                AttributeParamsString = attributeParams.CreateParameterString();
            }

            #region Parameter Sets Builder Functions

            // All these functions work in the same way
            // They take a variadic number of parameters for the function we are invoking
            // Then, either null or the type of the template function (if using generic version) is added to the end
            // That way, we can push a type to use as the generic argument for any function we are testing which is generic
            // If we do not need a generic type argument, we push null here and it will be ignored when writing the test
            // If we do, it is written as an extra parameter to the attribute's constructor and will be dealt with there

            public TestData ValidParams(params object[] validParameters)
            {
                List<object> parameters = new List<object>(validParameters);
                parameters.Add(null);
                ValidParameters.Add(parameters);

                return this;
            }

            public TestData ValidParams<T>(params object[] validParameters)
            {
                List<object> parameters = new List<object>(validParameters);
                parameters.Add(typeof(T));
                ValidParameters.Add(parameters);

                return this;
            }

            public TestData InvalidParams(params object[] invalidParameters)
            {
                List<object> parameters = new List<object>(invalidParameters);
                parameters.Add(null);
                InvalidParameters.Add(parameters);

                return this;
            }

            public TestData InvalidParams<T>(params object[] invalidParameters)
            {
                List<object> parameters = new List<object>(invalidParameters);
                parameters.Add(typeof(T));
                InvalidParameters.Add(parameters);

                return this;
            }

            #endregion
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

        private static int testCounter = 1;

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
                foreach (List<object> parameters in testData.ValidParameters)
                {
                    WriteTest(testData, parameters, true);
                }

                foreach (List<object> parameters in testData.InvalidParameters)
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
        private void WriteTest(TestData testData, List<object> parameters, bool shouldPass)
        {
            string templateParameters = "";
            Type genericType = parameters[parameters.Count - 1] as Type;
            parameters.RemoveAt(parameters.Count - 1);

            if (genericType != null)
            {
                templateParameters += "[" + TemplateParameters.Name + "(typeof(" + genericType.Name + "))]";
            }

            string functionParameters = "[" + FunctionParameters.Name + "(";
            string parameterStringForFunctionName = "";
            for (int i = 0; i < parameters.Count; i++)
            {
                object ithParam = parameters[i];

                parameterStringForFunctionName += ithParam.ToString();
                functionParameters += ithParam != null ? ithParam.CreateParameterString() : "null";

                if (i < parameters.Count - 1)
                {
                    parameterStringForFunctionName += "_";
                    functionParameters += ", ";
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

            WriteLine("[" + FunctionName.Name + "(\"" + testData.FunctionName + "\")]");
            WriteLine("[" + testData.TestAttribute.Name + "(" + testData.AttributeParamsString + ")]");

            if (!string.IsNullOrEmpty(templateParameters))
            {
                WriteLine(templateParameters);
            }

            WriteLine(functionParameters + ")]");
            // Yeah this is not working out
            //WriteLine("public void Test_" + testData.MethodName + "_" + parameterStringForFunctionName + "() { }");

            string passFailAttrString = shouldPass ? ShouldPass.Name : ShouldFail.Name;

            WriteLine("[" + passFailAttrString + "]");
            WriteLine("public void Test_" + testCounter + "() { }");
            WriteLine("");

            testCounter++;
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
            // So need to create an array for all the types of the inputted parameters for the attribute
            Type[] constructorTypes = new Type[attributeParameters.Length];

            for (int i = 0; i < attributeParameters.Length; i++)
            {
                constructorTypes[i] = attributeParameters[i].GetType();

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
                WriteLine("/// Auto-generated unit tests for class " + TestClassAttr.TestingType.Name);
                WriteLine("/// Do not edit by hand as changes will not be preserved between regenerations");
                WriteLine("/// <summary>");
                WriteLine("[" + TestType.Name + "(typeof(" + TestClassAttr.TestingType.Name + "))]");
                WriteLine("public class Test" + TestClassAttr.TestingType.Name + "Autogen : UnitTest");
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
