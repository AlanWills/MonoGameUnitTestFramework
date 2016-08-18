using System;

namespace MonoGameUnitTestFramework
{
    public class FunctionParameters : Attribute
    {
        #region Properties and Fields

        public object[] Params { get; private set; }

        public string ParamsString
        {
            get
            {
                string total = "";
                foreach (object obj in Params)
                {
                    total += obj.CreateParameterString();
                    total += " ";
                }

                return total;
            }
        }

        public static string Name = "FunctionParameters";

        #endregion

        public FunctionParameters(params object[] parameters)
        {
            Params = parameters;
        }
    }
}
