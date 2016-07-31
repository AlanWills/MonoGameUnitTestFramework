using System;

namespace UnitTestFramework
{
    public class Parameters : Attribute
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

        public bool ShouldBeValid { get; private set; }

        #endregion

        public Parameters(bool shouldBeValid, params object[] parameters)
        {
            ShouldBeValid = shouldBeValid;
            Params = parameters;
        }
    }
}
