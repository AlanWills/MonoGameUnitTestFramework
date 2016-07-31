using System;

namespace UnitTestFramework
{
    public class TestClassForType : Attribute
    {
        #region Properties and Fields

        public Type TestingClass { get; private set; }

        #endregion

        public TestClassForType(Type testingClass)
        {
            TestingClass = testingClass;
        }
    }
}
