﻿using System;

namespace UnitTestFramework
{
    public static class ExtensionFunctions
    {
        public static string CreateParameterString(this object inputParameter)
        {
            string objToString = inputParameter.ToString();

            if (inputParameter is string)
            {
                objToString = "@\"" + objToString + "\"";
            }
            else if (inputParameter is Type)
            {
                objToString = "typeof(" + (inputParameter as Type).Name + ")";
            }

            return objToString;
        }

        public static string CreateParameterString(this object[] inputParameters)
        {
            string paramString = "";
            for (int i = 0; i < inputParameters.Length; i++)
            {
                paramString += inputParameters[i].CreateParameterString();
                if (i < inputParameters.Length - 1)
                {
                    paramString += ", ";
                }
            }

            return paramString;
        }
    }
}