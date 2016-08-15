using System;

namespace MonoGameUnitTestFramework
{
    /// <summary>
    /// Stores the name of the function we are testing
    /// </summary>
    public class FunctionName : Attribute
    {
        #region Properties and Fields

        public string FuncName { get; private set; }

        public static string Name = "FunctionName";

        #endregion

        public FunctionName(string functionName)
        {
            FuncName = functionName;
        }
    }
}