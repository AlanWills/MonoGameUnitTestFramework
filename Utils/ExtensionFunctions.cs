using System;
using System.Collections;
using System.Collections.Generic;

namespace UnitTestFramework
{
    internal static class ExtensionFunctions
    {
        public static string CreateParameterString(this object inputParameter)
        {
            string objToString = inputParameter.ToString();

            if (inputParameter is string)
            {
                objToString = "\"" + objToString + "\"";
                if (objToString.Contains("\\"))
                {
                    objToString = "@" + objToString;
                }
            }
            else if (inputParameter is Type)
            {
                string typeName = (inputParameter as Type).Name;
                if (typeName == "Single")
                {
                    typeName = "float";
                }

                objToString = "typeof(" + typeName + ")";
            }
            else if (inputParameter is bool)
            {
                objToString = objToString.ToLower();
            }
            else if (inputParameter is IList &&
                     inputParameter.GetType().GetGenericTypeDefinition().IsAssignableFrom(typeof(List<>)))
            {
                // Yeah this needs to be optimized but for now it is the easiest thing
                // Definitely want to avoid allocating another array for the list
                IList list = inputParameter as IList;
                object[] objects = new object[list.Count];
                list.CopyTo(objects, 0);

                objToString = "new List<object>() { " + objects.CreateParameterString() + " }";
            }

            return objToString;
        }

        public static string CreateParameterString(this object[] inputParameters)
        {
            string paramString = "";
            for (int i = 0; i < inputParameters.Length; i++)
            {
                if (inputParameters[i] == null)
                {
                    paramString += "null";
                }
                else
                {
                    paramString += inputParameters[i].CreateParameterString();
                }

                if (i < inputParameters.Length - 1)
                {
                    paramString += ", ";
                }
            }

            return paramString;
        }

        public static bool CheckListValuesEqual(this List<object> actual, List<object> expected)
        {
            if (actual.Count != expected.Count) { return false; }

            foreach (object obj in actual)
            {
                if (!expected.Exists(x => x.))
            }
        }
    }
}
