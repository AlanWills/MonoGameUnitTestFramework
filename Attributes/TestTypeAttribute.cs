using System;

namespace UnitTestFramework
{
    public class TestType : Attribute
    {
        #region Properties and Fields

        public Type TestingType { get; private set; }
        public Type MockTestingType { get; private set; }

        public static string Name = "TestType";

        #endregion

        public TestType(Type testingType) :
            this(testingType, testingType)
        {
        }

        public TestType(Type testingType, Type mockTestingType)
        {
            TestingType = testingType;
            MockTestingType = mockTestingType;
        }
    }
}
