using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

namespace UnitTestFramework
{
    /// <summary>
    /// The base class for every unit test.
    /// </summary>
    public abstract class UnitTest
    {
        #region Properties and Fields

        /// <summary>
        /// An int to mark the number of tests that have failed
        /// </summary>
        public int TotalFailedTests { get; private set; }

        /// <summary>
        /// An int to mark the total number of discovered tests using reflection for the unit test class
        /// </summary>
        public int TotalRunTests { get; private set; }

        private TestType testClassAttr = null;
        protected TestType TestClassAttr
        {
            get
            {
                if (testClassAttr == null)
                {
                    testClassAttr = GetType().GetCustomAttribute<TestType>();
                }

                return testClassAttr;
            }
        }

        /// <summary>
        /// The test attribute used to store information on the class we will be testing in this Unit Test
        /// </summary>
        protected Type TestClass { get { return TestClassAttr.TestingType; } }

        private MethodInfo ClassInitialize { get; set; }
        private MethodInfo ClassCleanup { get; set; }
        private MethodInfo TestInitialize { get; set; }
        private MethodInfo TestCleanup { get; set; }

        #endregion

        public UnitTest()
        {
            // This is going to need cleaning up so we do checking to make sure there is only one of each attribute and one function does not have two attributes etc.
            // Also do checks on no parameters and returning void
            foreach (MethodInfo method in GetType().GetMethods())
            {
                if (method.GetCustomAttribute<ClassInitializeAttribute>() != null)
                {
                    ClassInitialize = method;
                }
                else if (method.GetCustomAttribute<ClassCleanupAttribute>() != null)
                {
                    ClassCleanup = method;
                }
                else if (method.GetCustomAttribute<TestInitializeAttribute>() != null)
                {
                    TestInitialize = method;
                }
                else if (method.GetCustomAttribute<TestCleanupAttribute>() != null)
                {
                    TestCleanup = method;
                }
            }
        }

        #region Utility Functions

        /// <summary>
        /// Runs all the unit tests within this class (they must be marked with our unit test attribute).
        /// </summary>
        public List<string> Run()
        {
            List<string> output = new List<string>();

            // Run our OnTestClassStart function because we are beginning our test suite
            OnTestClassStart();

            // The unit test methods must be public to be discovered here
            foreach (MethodInfo method in GetType().GetMethods())
            {
                object classWeAreTestingInstance = null;
                if (!method.IsStatic)
                {
                    // Create an instance for acting on when we invoke our function
                    // Currently we do not support 
                    classWeAreTestingInstance = Activator.CreateInstance(TestClassAttr.MockTestingType);
                }

                // Get the test attribute for this method
                TestMethodAttribute microsoftTest = method.GetCustomAttribute<TestMethodAttribute>();
                if (microsoftTest == null && method.GetCustomAttribute<FunctionName>() == null)
                {
                    // This method is not a unit test
                    continue;
                }

                // Call the before test function, run the test and call the after test function
                OnTestStart();

                // Turn off asserts when we are about to start our tests
                Trace.Listeners.Clear();
                string resultString = method.Name + " passed";

                if (microsoftTest != null)
                {
                    // This is a method using the standard microsoft unit testing framework
                    try
                    {
                        method.Invoke(this, null);
                    }
                    catch
                    {
                        // Logs the failure of the unit test
                        resultString = "The function " + method.Name + " in class " + GetType().Name + " failed";
                        TotalFailedTests++;
                    }
                }
                // Run the test using our attributes and if it fails we log an error
                else
                {
                    FunctionName functionName = method.GetCustomAttribute<FunctionName>();
                    TemplateParameters templateParameters = method.GetCustomAttribute<TemplateParameters>();
                    FunctionParameters functionParameters = method.GetCustomAttribute<FunctionParameters>();
                    TestPassIf testCheckFunc = method.GetCustomAttribute<TestPassIf>();
                    bool shouldPass = method.GetCustomAttribute<ShouldPass>() != null;

                    DebugUtils.AssertNotNull(functionName);
                    DebugUtils.AssertNotNull(functionParameters);

                    MethodInfo funcToTest = TestClass.GetMethod(functionName.FuncName);
                    DebugUtils.AssertNotNull(funcToTest);

                    if (funcToTest.IsGenericMethod)
                    {
                        // If the method we are testing is generic, we will have used the templated parameter set builder, so we can now extract that attribute from the method
                        funcToTest = funcToTest.MakeGenericMethod(templateParameters.Params);
                    }

                    // Perform the function we are testing and obtain the output
                    // The 'classWeAreTestingInstance' will be null if the function is static
                    object result = funcToTest.Invoke(classWeAreTestingInstance, functionParameters.Params);
                    testCheckFunc.ParametersForCheckFunction.Add(result);

                    // Run our check function on the output to see if the function has performed as we expected
                    bool resultOfTest = (bool)testCheckFunc.CheckFunc.DynamicInvoke(testCheckFunc.ParametersForCheckFunction.ToArray());

                    // If our test result is the opposite to whether the parameters are valid or not, then this test has returned the opposite result to what it should have done, so it has failed
                    if (resultOfTest != shouldPass)
                    {
                        // Logs the failure of the unit test
                        resultString = testCheckFunc.RegisterFailure(TestClass.Name) + " with parameters " + functionParameters.ParamsString;
                        TotalFailedTests++;
                    }
                }

                output.Add(resultString);

                // Immediately turn asserts back on as soon as the test has finished
                // We want to still catch problems in the OnTestBegin & OnTestEnd functions
                Trace.Refresh();

                TotalRunTests++;
                OnTestEnd();
            }

            // Run our OnTestClassEnd function because we have finished running all the tests on this class
            OnTestClassEnd();

            return output;
        }

        #endregion

        #region Virtual Functions

        /// <summary>
        /// A function called when our test class begins running.
        /// Called before any test is run.
        /// Invokes the function on this type which has been marked with the microsoft unit testing framework ClassInitialize attribute.
        /// </summary>
        private void OnTestClassStart()
        {
            if (ClassInitialize != null)
            {
                ClassInitialize.Invoke(this, new object[] { null });
            }
        }

        /// <summary>
        /// A function called when our test class has finished running.
        /// Called after every test has run.
        /// Invokes the function on this type which has been marked with the microsoft unit testing framework ClassCleanUp attribute.
        /// </summary>
        private void OnTestClassEnd()
        {
            if (ClassCleanup != null)
            {
                ClassCleanup.Invoke(this, null);
            }
        }

        /// <summary>
        /// A function called before every test is run.
        /// Invokes the function on this type which has been marked with the microsoft unit testing framework TestInitialize attribute.
        /// </summary>
        private void OnTestStart()
        {
            if (TestInitialize != null)
            {
                TestInitialize.Invoke(this, null);
            }
        }

        /// <summary>
        /// A function called after every test is run.
        /// Invokes the function on this type which has been marked with the microsoft unit testing framework TestCleanUp attribute.
        /// </summary>
        private void OnTestEnd()
        {
            if (TestCleanup != null)
            {
                TestCleanup.Invoke(this, null);
            }
        }

        #endregion

        #region Test Functions

        public static bool CheckIsNotNull(object objectToTest)
        {
            return objectToTest != null;
        }

        public static bool CheckIsNull(object objectToTest)
        {
            return objectToTest == null;
        }

        public static bool CheckIsType(Type expectedType, object objectToTest)
        {
            return CheckIsNotNull(objectToTest) && objectToTest.GetType() == expectedType;
        }

        public static bool CheckReferencesAreEqual(object expected, object actual)
        {
            return expected == actual;
        }

        public static bool CheckInstanceFunctionCallSuccess(object objectToActOn, string functionName, params object[] functionParameters)
        {
            MethodInfo method = objectToActOn.GetType().GetMethod(functionName);
            DebugUtils.AssertNotNull(method);

            try
            {
                method.Invoke(objectToActOn, functionParameters);
            }
            catch
            {
                return false;
            }

            return true;
        }

        public static bool CheckStaticFunctionCallSuccess(Type typeWithFunctionToTest, string functionName, params object[] functionParameters)
        {
            MethodInfo method = typeWithFunctionToTest.GetMethod(functionName);
            DebugUtils.AssertNotNull(method);

            try
            {
                method.Invoke(null, functionParameters);
            }
            catch
            {
                return false;
            }

            return true;
        }

        #endregion
    }
}