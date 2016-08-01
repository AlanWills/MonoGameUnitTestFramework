using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UnitTestFramework;
using System.Collections.Generic;

namespace FrameworkUnitTests
{
    [TestClass]
    public class TestCreateParameterStringSingleObject
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

        [TestMethod]
        public void Test_CreateParameterString_NumberList()
        {
            List<float> floatList = new List<float>() { 10, 5, 0, -5, -10 };
            Assert.AreEqual("new List<object>() { 10, 5, 0, -5, -10 }", floatList.CreateParameterString());
        }

        [TestMethod]
        public void Test_CreateParameterString_NumberListAsObject()
        {
            object floatList = new List<float>() { 10, 5, 0, -5, -10 };
            Assert.AreEqual("new List<object>() { 10, 5, 0, -5, -10 }", floatList.CreateParameterString());
        }

        [TestMethod]
        public void Test_CreateParameterString_BoolList()
        {
            List<bool> floatList = new List<bool>() { true, false, false, true };
            Assert.AreEqual("new List<object>() { true, false, false, true }", floatList.CreateParameterString());
        }

        [TestMethod]
        public void Test_CreateParameterString_BoolListAsObject()
        {
            object floatList = new List<bool>() { true, false, false, true };
            Assert.AreEqual("new List<object>() { true, false, false, true }", floatList.CreateParameterString());
        }

        [TestMethod]
        public void Test_CreateParameterString_StringList()
        {
            List<string> floatList = new List<string>() { "", "hello", "\"" };
            Assert.AreEqual("new List<object>() { \"\", \"hello\", \"\"\" }", floatList.CreateParameterString());
        }

        [TestMethod]
        public void Test_CreateParameterString_StringListAsObject()
        {
            object floatList = new List<string>() { "", "hello", "\"" };
            Assert.AreEqual("new List<object>() { \"\", \"hello\", \"\"\" }", floatList.CreateParameterString());
        }

        [TestMethod]
        public void Test_CreateParameterString_TypeList()
        {
            List<Type> floatList = new List<Type>() { typeof(float), typeof(MockClass) };
            Assert.AreEqual("new List<object>() { typeof(float), typeof(MockClass) }", floatList.CreateParameterString());
        }

        [TestMethod]
        public void Test_CreateParameterString_TypeListAsObject()
        {
            object floatList = new List<Type>() { typeof(float), typeof(MockClass) };
            Assert.AreEqual("new List<object>() { typeof(float), typeof(MockClass) }", floatList.CreateParameterString());
        }
    }
}
