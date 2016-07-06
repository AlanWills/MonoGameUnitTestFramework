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
        private TestClass TestClassAttr { get { return GetType().GetCustomAttribute<TestClass>(); } }

        #endregion

        public UnitTest()
        {
            FailedTestsInfo = new List<string>();

            DebugUtils.AssertNotNull(TestClassAttr);
        }

        #region Utility Functions

        /// <summary>
        /// Runs all the unit tests within this class (they must be marked with our unit test attribute).
        /// </summary>
        public void Run()
        {
            // Clear our output strings
            FailedTestsInfo.Clear();

            // Run our OnTestClassStart function because we are beginning our test suite
            OnTestClassStart();

            // The unit test methods must be public to be discovered here
            foreach (MethodInfo method in GetType().GetMethods())
            {
                // Get the test attribute for this method
                Test unitTest = method.GetCustomAttribute<Test>();
                if (unitTest == null)
                {
                    // This method does not have the test attribute, so move on
                    continue;
                }

                // Find the valid inputs and invalid inputs attributes
                // The former is required, the latter is optional
                // The type is also inspected and invalid inputs are generated using standard values
                // e.g. for float we would try -1, 0, 1, Infinity and Negative Infinity
                // If however any of these are marked as valid inputs we would just ignore them

                // Have extension functions for each type to generate a list of invalid inputs for our unit test

                // Run the test for each valid input and if it fails we log an error
                {
                    TestValidInputs validInputs = method.GetCustomAttribute<TestValidInputs>();
                    DebugUtils.AssertNotNull(validInputs);

                    foreach (object validInput in validInputs)
                    {
                        // Loop over each unit test, call the before test function, run the test and call the after test function
                        OnTestStart();

                        if (!unitTest.Invoke(TestClassAttr, validInput))
                        {
                            // Logs the failure of the unit test
                            FailedTestsInfo.Add(unitTest.RegisterFailure(TestClassAttr.TestingClass.Name));
                            TotalFailedTests++;
                        }

                        TotalRunTests++;
                        OnTestEnd();
                    }
                }

                // Run the test for each invvalid input and if it succeeds we log an error
                {
                    TestInvalidInputs invalidInputs = method.GetCustomAttribute<TestInvalidInputs>();
                    if (invalidInputs == null)
                    {
                        invalidInputs = new TestInvalidInputs();
                    }

                    foreach (object invalidInput in invalidInputs)
                    {
                        // Loop over each unit test, call the before test function, run the test and call the after test function
                        OnTestStart();

                        if (unitTest.Invoke(TestClassAttr, invalidInput))
                        {
                            // Logs the success of the unit test - this is actually a failure as it should not succeed
                            FailedTestsInfo.Add(unitTest.RegisterFailure(TestClassAttr.TestingClass.Name));
                            TotalFailedTests++;
                        }

                        TotalRunTests++;
                        OnTestEnd();
                    }
                }
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

        public static bool CheckReferencesAreEqual(object expected, object actual)
        {
            return expected == actual;
        }

        #endregion
    }
}