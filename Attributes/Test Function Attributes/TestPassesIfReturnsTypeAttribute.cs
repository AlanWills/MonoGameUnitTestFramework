using System;

namespace UnitTestFramework
{
    /// <summary>
    /// The attribute representing testing that a method returns an object of the inputted type which is also not null
    /// </summary>
    public class ReturnsType : TestPassIf
    {
        public static string Description = "IsType";

        public ReturnsType(string functionName, Type expectedType) :
            base(functionName, (Func<Type, object, bool>)UnitTest.CheckIsType, expectedType)
        {
        }
    }
}
