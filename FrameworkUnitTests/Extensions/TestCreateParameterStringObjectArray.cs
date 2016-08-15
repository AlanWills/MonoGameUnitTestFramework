using Microsoft.VisualStudio.TestTools.UnitTesting;
using MonoGameUnitTestFramework;

namespace FrameworkUnitTests
{
    [TestClass]
    public class TestCreateParameterStringObjectArray
    {
        [TestMethod]
        public void Test_CreateParameterString_NumberArray()
        {
            object[] numberArray = new object[] { 10, 5, 0, -5, -10 };
            Assert.AreEqual("10, 5, 0, -5, -10", numberArray.CreateParameterString());
        }

        [TestMethod]
        public void Test_CreateParameterString_BoolArray()
        {
            object[] boolArray = new object[] { true, false, false, true };
            Assert.AreEqual("true, false, false, true", boolArray.CreateParameterString());
        }

        [TestMethod]
        public void Test_CreateParameterString_StringArray()
        {
            object[] stringArray = new object[] { "", "test", "\'" };
            Assert.AreEqual("\"\", \"test\", \"\'\"", stringArray.CreateParameterString());
        }

        [TestMethod]
        public void Test_CreateParameterString_TypeArray()
        {
            object[] typeArray = new object[] { typeof(float), typeof(MockClass) };
            Assert.AreEqual("typeof(float), typeof(MockClass)", typeArray.CreateParameterString());
        }

        [TestMethod]
        public void Test_CreateParameterString_NullArray()
        {
            object[] nullArray = new object[] { null, null };
            Assert.AreEqual("null, null", nullArray.CreateParameterString());
        }

        [TestMethod]
        public void Test_CreateParameterString_MixedArray()
        {
            object[] mixedArray = new object[] { 10, true, "test", typeof(MockClass), null };
            Assert.AreEqual("10, true, \"test\", typeof(MockClass), null", mixedArray.CreateParameterString());
        }
    }
}
