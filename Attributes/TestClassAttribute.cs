using System;

namespace UnitTestFramework
{
    public class TestClass : Attribute
    {
        #region Properties and Fields

        public Type TestingClass { get; private set; }

        #endregion

        public TestClass(Type testingClass)
        {
            TestingClass = testingClass;
        }
    }
}
