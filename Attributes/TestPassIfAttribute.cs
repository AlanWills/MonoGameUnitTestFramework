using System;
using System.Collections.Generic;
using System.Reflection;

namespace UnitTestFramework
{
    public abstract class TestPassIf : Attribute
    {
        #region Properties and Fields

        private string FunctionName { get; set; }
        protected Delegate CheckFunc { private get; set; }
        private List<object> ParametersForCheckFunction { get; set; }

        #endregion

        public TestPassIf(string functionName, Delegate functionToApply, params object[] parametersForCheckFunction)
        {
            FunctionName = functionName;
            CheckFunc = functionToApply;

            ParametersForCheckFunction = new List<object>(parametersForCheckFunction.Length + 1);
            ParametersForCheckFunction.AddRange(parametersForCheckFunction);
        }

        public bool Invoke(TestClassForType testClass, object[] inputs)
        {
            MethodInfo funcToTest = testClass.TestingClass.GetMethod(FunctionName);
            DebugUtils.AssertNotNull(funcToTest);

            if (funcToTest.IsGenericMethod)
            {
                funcToTest = funcToTest.MakeGenericMethod(ParametersForCheckFunction[0] as Type);
            }

            // Perform the function we are testing and obtain the output
            object result = funcToTest.Invoke(null, inputs);
            ParametersForCheckFunction.Add(result);

            // Run our check function on the output to see if the function has performed as we expected
            return (bool)CheckFunc.DynamicInvoke(ParametersForCheckFunction.ToArray());
        }

        public virtual string RegisterFailure(string testClassName)
        {
            return "The function " + FunctionName + " in class " + testClassName + " failed";
        }
    }
}
