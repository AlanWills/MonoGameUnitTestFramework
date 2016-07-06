using System;
using System.Reflection;

namespace UnitTestFramework
{
    public abstract class Test : Attribute
    {
        #region Properties and Fields

        private string FunctionName { get; set; }
        protected Delegate CheckFunc { private get; set; }

        #endregion

        public Test(string functionName, Delegate functionToApply)
        {
            FunctionName = functionName;
            CheckFunc = functionToApply;
        }

        public bool Invoke(TestClass testClass, object input)
        {
            MethodInfo testFunc = testClass.TestingClass.GetMethod(FunctionName);
            DebugUtils.AssertNotNull(testFunc);

            // Perform the function we are testing and  obtain the output
            object result = testFunc.Invoke(null, new object[] { input });
            
            // Run our check function on the output to see if the function has performed as we expected
            return (bool)CheckFunc.DynamicInvoke(result);
        }

        public virtual string RegisterFailure(string testClassName)
        {
            return "The function " + FunctionName + " in class " + testClassName + " failed";
        }
    }
}
