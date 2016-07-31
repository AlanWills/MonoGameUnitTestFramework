using System;

namespace UnitTestFramework
{
    public class TemplateParameters : Attribute
    {
        #region Properties and Fields

        public Type[] Params { get; private set; }

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

        public static string Name = "TemplateParameters";

        #endregion

        public TemplateParameters(params Type[] parameters)
        {
            Params = parameters;
        }
    }
}
