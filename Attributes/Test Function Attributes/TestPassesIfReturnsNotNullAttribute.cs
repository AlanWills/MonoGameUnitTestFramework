﻿using System;

namespace UnitTestFramework
{
    /// <summary>
    /// The test attribute which corresponds to a unit test for checking an object is not null
    /// </summary>
    public class ReturnsNotNull : TestPassIf
    {
        public static string Description = "IsNotNull";

        public ReturnsNotNull(string functionName) :
            base(functionName, (Func<object, bool>)UnitTest.CheckIsNotNull)
        {
            
        }
    }
}