using System;

namespace MonoGameUnitTestFramework
{
    /// <summary>
    /// The attribute representing testing that a method returns an object of the inputted type which is also not null
    /// </summary>
    public class ReturnsType : TestPassIf
    {
        public static string Description = "IsType";

        public ReturnsType(Type expectedType) :
            base((Func<object, Type, bool>)UnitTest.CheckIsType, expectedType)
        {
        }
    }
}
