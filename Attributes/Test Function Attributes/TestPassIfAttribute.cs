using System;
using System.Collections.Generic;

namespace MonoGameUnitTestFramework
{
    public abstract class TestPassIf : Attribute
    {
        #region Properties and Fields

        public Delegate CheckFunc { get; private set; }
        public List<object> ParametersForCheckFunction { get; private set; }

        // If set, the class instance created when we run the tests will be passed to the CheckFunc
        // as a parameter after the result of the test function
        public bool RequiresClassInstance { get; protected set; }

        #endregion

        public TestPassIf(Delegate functionToApply, params object[] parametersForCheckFunction)
        {
            CheckFunc = functionToApply;
            RequiresClassInstance = false;

            ParametersForCheckFunction = new List<object>(parametersForCheckFunction.Length + 1);
            ParametersForCheckFunction.AddRange(parametersForCheckFunction);
        }
    }
}
