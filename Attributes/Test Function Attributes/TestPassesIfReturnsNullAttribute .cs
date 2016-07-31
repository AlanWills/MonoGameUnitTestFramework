﻿using System;

namespace UnitTestFramework
{
    /// <summary>
    /// The test attribute which corresponds to a unit test for checking an object is null
    /// </summary>
    public class ReturnsNull : TestPassIf
    {
        public static string Description = "IsNull";

        public ReturnsNull(string functionName) :
            base(functionName, (Func<object, bool>)UnitTest.CheckIsNull)
        {
            
        }
    }
}