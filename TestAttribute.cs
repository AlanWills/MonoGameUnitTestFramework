using System;
using System.Reflection;

namespace UnitTestFramework
{
    public class Test : Attribute
    {
        #region Properties and Fields

        private MethodInfo TestFuncInfo { get; set; }
        private object[] ValidInputs { get; set; }
        protected Delegate FuncToApply { private get; set; }

        #endregion

        public Test(Type callingClass, string functionName, params object[] inputs)
        {
            TestFuncInfo = callingClass.GetMethod(functionName);
            DebugUtils.AssertNotNull(TestFuncInfo);

            ValidInputs = inputs;
            FuncToApply = (Func<object, bool>)UnitTest.CheckIsNotNull;
        }

        public bool Invoke(UnitTest test)
        {
            foreach (object input in ValidInputs)
            {
                object result = TestFuncInfo.Invoke(null, new object[] { input });
                if (!(bool)FuncToApply.DynamicInvoke(result))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
