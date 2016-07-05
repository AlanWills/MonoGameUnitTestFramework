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
        public int FailedTests { get; private set; }

        #endregion

        public UnitTest()
        {
            FailedTests = 0;
            FailedTestsInfo = new List<string>();
        }

        #region Utility Functions

        /// <summary>
        /// Runs all the unit tests within this class (they must be marked with our unit test attribute).
        /// </summary>
        public void Run()
        {
            // Clear our output strings
            FailedTestsInfo.Clear();

            MethodInfo[] methods = GetType().GetMethods();

            List<MethodInfo> unitTests = new List<MethodInfo>();

            // Loop through all the methods on the class and find all of the methods marked with our unit test attribute
            foreach (MethodInfo method in methods)
            {
                Test testAttr = method.GetCustomAttribute(typeof(Test)) as Test;
                if (testAttr != null)
                {
                    // Check we do not accept any parameters
                    Debug.Assert(method.GetParameters().Length == 0);

                    // Add to our list of unit tests
                    unitTests.Add(method);
                }
            }

            // If our class has unit tests
            if (unitTests.Count > 0)
            {
                // Run our OnTestClassStart function because we are beginning our test suite
                OnTestClassStart();

                // Loop over each unit test, call the before test function, run the test and call the after test function
                foreach (MethodInfo method in unitTests)
                {
                    OnTestStart();

                    method.Invoke(this, null);

                    Test testAttr = method.GetCustomAttribute(typeof(Test)) as Test;
                    if (!testAttr.Invoke(this))
                    {
                        // Change this so it uses the attr's information like the function name and calling class
                        RegisterFailure();
                    }

                    OnTestEnd();
                }

                // Run our OnTestClassEnd function because we have finished running all the tests on this class
                OnTestClassEnd();
            }
            else
            {
                Debug.Fail("No unit tests on registered unit test class: " + GetType().Name);
            }
        }

        /// <summary>
        /// Adds the name of the failed test and the file and line it failed in.
        /// Increments our failed unit test number.
        /// </summary>
        private void RegisterFailure()
        {
            StackFrame callStack = new StackTrace(true).GetFrame(1);

            string[] splitStrings = callStack.GetFileName().Split('\\');
            Debug.Assert(splitStrings.Length > 0);

            FailedTestsInfo.Add(callStack.GetMethod() + " failed " + splitStrings[splitStrings.Length - 1] + " at line number " + callStack.GetFileLineNumber().ToString());

            FailedTests++;
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