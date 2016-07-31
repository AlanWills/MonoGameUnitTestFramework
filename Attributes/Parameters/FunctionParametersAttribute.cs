using System;

namespace UnitTestFramework
{
    public class FunctionParameters : Attribute
    {
        public static string Name = "FunctionParameters";

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

        #endregion

        public FunctionParameters(params object[] parameters)
        {
            Params = parameters;
        }
    }
}
