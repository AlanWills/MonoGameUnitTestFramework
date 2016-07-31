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
        /// A list of all the failed tests with info
        /// </summary>
        public List<string> FailedTestsInfo { get; private set; }

        /// <summary>
        /// An int to mark the number of tests that have failed
        /// </summary>
        public int TotalFailedTests { get; private set; }

        /// <summary>
        /// An int to mark the total number of discovered tests using reflection for the unit test class
        /// </summary>
        public int TotalRunTests { get; private set; }

        /// <summary>
        /// The test attribute used to store information on the class we will be testing in this Unit Test
        /// </summary>
        protected TestClassForType TestClassAttr { get { return GetType().GetCustomAttribute<TestClassForType>(); } }

        #endregion

        public UnitTest()
        {
            FailedTestsInfo = new List<string>();
        }

        #region Utility Functions

        /// <summary>
        /// Runs all the unit tests within this class (they must be marked with our unit test attribute).
        /// </summary>
        public void Run()
        {
            DebugUtils.AssertNotNull(TestClassAttr);

            // Clear our output strings
            FailedTestsInfo.Clear();

            // Run our OnTestClassStart function because we are beginning our test suite
            OnTestClassStart();

            // The unit test methods must be public to be discovered here
            foreach (MethodInfo method in GetType().GetMethods())
            {
                // Get the test attribute for this method
                TestPassIf unitTest = method.GetCustomAttribute<TestPassIf>();
                TestMethodAttribute microsoftTest = method.GetCustomAttribute<TestMethodAttribute>();

                if (unitTest == null && microsoftTest == null)
                {
                    // This method is not a unit test
                    continue;
                }

                // Call the before test function, run the test and call the after test function
                OnTestStart();

                // Turn off asserts when we are about to start our tests
                Trace.Listeners.Clear();

                if (unitTest == null)
                {
                    // This is a method using the standard microsoft unit testing framework
                    try
                    {
                        method.Invoke(this, null);
                    }
                    catch
                    {
                        // Logs the failure of the unit test
                        FailedTestsInfo.Add("The function " + method.Name + " in class " + GetType().Name + " failed");
                        TotalFailedTests++;
                    }
                }
                // Run the test for the parameter(s) and if it fails we log an error, using our custom unit testing implementation
                else
                {
                    FunctionParameters parameters = method.GetCustomAttribute<FunctionParameters>();
                    DebugUtils.AssertNotNull(parameters);

                    bool shouldPass = method.GetCustomAttribute<ShouldPass>() != null;

                    // If our test result is the opposite to whether the parameters are valid or not, then this test has returned the opposite result to what it should have done, so it has failed
                    if (unitTest.Invoke(TestClassAttr, parameters.Params) != shouldPass)
                    {
                        // Logs the failure of the unit test
                        FailedTestsInfo.Add(unitTest.RegisterFailure(TestClassAttr.TestingClass.Name) + " with parameters " + parameters.ParamsString);
                        TotalFailedTests++;
                    }
                }

                // Immediately turn asserts back on as soon as the test has finished
                // We want to still catch problems in the OnTestBegin & OnTestEnd functions
                Trace.Refresh();

                TotalRunTests++;
                OnTestEnd();
            }

            // Run our OnTestClassEnd function because we have finished running all the tests on this class
            OnTestClassEnd();
        }

        #endregion

        #region Virtual Functions

        /// <summary>
        /// A function called when our test class begins running.
        /// Called before any test is run.
        /// </summary>
        protected virtual void OnTestClassStart() { }

        /// <summary>
        /// A function called when our test class has finished running.
        /// Called after every test has run.
        /// </summary>
        protected virtual void OnTestClassEnd() { }

        /// <summary>
        /// A function called before every test is run.
        /// </summary>
        protected virtual void OnTestStart() { }

        /// <summary>
        /// A function called after every test is run.
        /// </summary>
        protected virtual void OnTestEnd() { }

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

        #endregion
    }
}