using System;
using System.Reflection;

namespace UnitTestFramework
{
    public abstract class TestPassIf : Attribute
    {
        #region Properties and Fields

        private string FunctionName { get; set; }
        protected Delegate CheckFunc { private get; set; }
        private object[] ParametersToCheckFunction { get; set; }

        #endregion

        public TestPassIf(string functionName, Delegate functionToApply, params object[] parametersToCheckFunc)
        {
            FunctionName = functionName;
            CheckFunc = functionToApply;
            ParametersToCheckFunction = parametersToCheckFunc;
        }

        public bool Invoke(TestClassForType testClass, object[] inputs)
        {
            MethodInfo testFunc = testClass.TestingClass.GetMethod(FunctionName);
            DebugUtils.AssertNotNull(testFunc);

            // Perform the function we are testing and  obtain the output
            object result = testFunc.Invoke(null, inputs);

            // Run our check function on the output to see if the function has performed as we expected
            if (ParametersToCheckFunction == null || ParametersToCheckFunction.Length == 0)
            {
                return (bool)CheckFunc.DynamicInvoke(result);
            }
            else
            {
                return (bool)CheckFunc.DynamicInvoke(result, ParametersToCheckFunction);
            }
        }

        public virtual string RegisterFailure(string testClassName)
        {
            return "The function " + FunctionName + " in class " + testClassName + " failed";
        }
    }
}
