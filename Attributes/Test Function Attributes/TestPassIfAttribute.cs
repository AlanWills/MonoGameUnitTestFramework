using System;
using System.Collections.Generic;

namespace UnitTestFramework
{
    public abstract class TestPassIf : Attribute
    {
        #region Properties and Fields

        public Delegate CheckFunc { get; set; }
        public List<object> ParametersForCheckFunction { get; private set; }

        #endregion

        public TestPassIf(Delegate functionToApply, params object[] parametersForCheckFunction)
        {
            CheckFunc = functionToApply;

            ParametersForCheckFunction = new List<object>(parametersForCheckFunction.Length + 1);
            ParametersForCheckFunction.AddRange(parametersForCheckFunction);
        }

        public virtual string RegisterFailure(string testClassName)
        {
            //return "The function " + FunctionName + " in class " + testClassName + " failed";
            return "";
        }
    }
}
