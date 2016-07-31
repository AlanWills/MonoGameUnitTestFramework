using System;
using System.Collections.Generic;

namespace UnitTestFramework
{
    public class TestData
    {
        public string MethodName { get; private set; }
        public List<object[]> ValidParameters { get; private set; }
        public List<object[]> InvalidParameters { get; private set; }
        public Type TestAttribute { get; private set; }
        public string AttributeParamsString { get; private set; }

        public TestData(string methodName, Type testAttributeType, params object[] attributeParams)
        {
            MethodName = methodName;
            ValidParameters = new List<object[]>();
            InvalidParameters = new List<object[]>();
            TestAttribute = testAttributeType;
            AttributeParamsString = attributeParams.CreateParameterString();
        }

        public TestData ValidParams(params object[] validParameters)
        {
            ValidParameters.Add(validParameters);

            return this;
        }

        public TestData InvalidParams(params object[] validParameters)
        {
            InvalidParameters.Add(validParameters);

            return this;
        }
    }
}
