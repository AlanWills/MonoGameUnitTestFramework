using System;

namespace UnitTestFramework
{
    /// <summary>
    /// The test attribute which corresponds to a unit test for checking a function returns an inputted object.
    /// For strings and primitives it will not perform a reference check, but rather that their values are equal.
    /// For lists it will also not perform a reference check, but rather that their contents are equal.
    /// </summary>
    public class ReturnsValue : TestPassIf
    {
        public static string Description = "ReturnsObject";

        public ReturnsValue(object expectedObject) :
            base((Func<object, object, bool>)UnitTest.CheckValue, expectedObject)
        {
            
        }
    }
}
