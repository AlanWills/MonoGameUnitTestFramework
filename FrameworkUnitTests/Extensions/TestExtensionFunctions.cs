using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UnitTestFramework;

namespace FrameworkUnitTests
{
    [TestClass]
    public class TestExtensionFunctions
    {
        [TestMethod]
        public void Test_CreateParameterString_PositiveNumber()
        {
            float number = 5;
            Assert.AreEqual("5", number.CreateParameterString());
        }

        [TestMethod]
        public void Test_CreateParameterString_NegativeNumber()
        {
            float number = -5;
            Assert.AreEqual("-5", number.CreateParameterString());
        }

        [TestMethod]
        public void Test_CreateParameterString_True()
        {
            bool boolValue = true;
            Assert.AreEqual("true", boolValue.CreateParameterString());
        }

        [TestMethod]
        public void Test_CreateParameterString_False()
        {
            bool boolValue = false;
            Assert.AreEqual("false", boolValue.CreateParameterString());
        }

        [TestMethod]
        public void Test_CreateParameterString_BoolAsObject()
        {
            object boolAsObject = false;
            Assert.AreEqual("false", boolAsObject.CreateParameterString());
        }

        [TestMethod]
        public void Test_CreateParameterString_EmptyString()
        {
            string emptyString = "";
            Assert.AreEqual("\"\"", emptyString.CreateParameterString());
        }

        [TestMethod]
        public void Test_CreateParameterString_NonEmptyString()
        {
            string nonEmptyString = "Parameter";
            Assert.AreEqual("\"Parameter\"", nonEmptyString.CreateParameterString());
        }

        [TestMethod]
        public void Test_CreateParameterString_NonEmptyStringAsObject()
        {
            object nonEmptyStringAsObject = "Parameter";
            Assert.AreEqual("\"Parameter\"", nonEmptyStringAsObject.CreateParameterString());
        }

        [TestMethod]
        public void Test_CreateParameterString_Path()
        {
            string path = "Test\\Path";
            Assert.AreEqual("@\"Test\\Path\"", path.CreateParameterString());
        }

        [TestMethod]
        public void Test_CreateParameterString_Type()
        {
            Type mockClassType = new MockClass().GetType();
            Assert.AreEqual("typeof(MockClass)", mockClassType.CreateParameterString());
        }

        [TestMethod]
        public void Test_CreateParameterString_TypeAsObject()
        {
            object mockClassTypeAsObject = new MockClass().GetType();
            Assert.AreEqual("typeof(MockClass)", mockClassTypeAsObject.CreateParameterString());
        }
    }
}
