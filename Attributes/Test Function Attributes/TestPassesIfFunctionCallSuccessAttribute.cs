using System;

namespace UnitTestFramework
{
    // Only supports instance functions for now
    public class FunctionSuccess : TestPassIf
    {
        public FunctionSuccess(string functionName) :
            base((Func<object, string, bool>)UnitTest.CheckInstanceFunctionCallSuccess)
        {
            RequiresClassInstance = true;
            ParametersForCheckFunction.Insert(0, functionName);
        }

        //public FunctionSuccess(string functionName, params object[] parametersForFunction) :
        //    base((Func<object, string, object[], bool>)UnitTest.CheckInstanceFunctionCallSuccess, parametersForFunction)
        //{
        //    ParametersForCheckFunction.Insert(0, functionName);
        //}
    }
}
