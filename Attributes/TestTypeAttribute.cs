using System;

namespace UnitTestFramework
{
    public class TestType : Attribute
    {
        #region Properties and Fields

        public Type TestingType { get; private set; }

        public static string Name = "TestType";

        #endregion

        public TestType(Type testingType)
        {
            TestingType = testingType;
        }
    }
}
