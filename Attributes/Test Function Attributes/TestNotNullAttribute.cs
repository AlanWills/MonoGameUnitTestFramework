using System;

namespace UnitTestFramework
{
    /// <summary>
    /// The test attribute which corresponds to a unit test for checking an object is not null
    /// </summary>
    public class TestNotNull : Test
    {
        public TestNotNull(string functionName) :
            base(functionName, (Func<object, bool>)UnitTest.CheckIsNotNull)
        {
            
        }
    }
}
