using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mnk.Library.Common;

namespace Mnk.Library.Tests
{
    [TestClass]
    public class CommonOpsTestFixture
    {
        [DataTestMethod]
        [DataRow("", "")]
        [DataRow("a", "a")]
        [DataRow("\r", "\\r")]
        [DataRow("\r\n", "\\r\\n")]
        [DataRow("\\", "\\\\")]
        [DataRow("\\\\", "\\\\\\\\")]
        [DataRow("\\\r", "\\\\\\r")]
        public void Should_encode_string(string value, string expected)
        {
            Assert.AreEqual(expected, CommonEncoders.EncodeString(value));
        }

        [DataTestMethod]
        [DataRow("", "")]
        [DataRow("a", "a")]
        [DataRow("\\\\", "\\")]
        [DataRow("\\r\\n", "\r\n")]
        [DataRow("\\", "\\")]
        [DataRow("\\\\r", "\\r")]
        [DataRow("test\\", "test\\")]
        [DataRow("\"test\\\"", "\"test\"")]
        public void Should_decode_string(string value, string expected)
        {
            Assert.AreEqual(expected, CommonEncoders.DecodeString(value));
        }

        [DataTestMethod]
        [DataRow("", "")]
        [DataRow("a", "a")]
        [DataRow("  ", " ")]
        [DataRow("\r\n", " ")]
        [DataRow("\r\n       ", " ")]
        [DataRow("a\r\nb      c", "a b c")]
        public void Should_minimize_string(string value, string expected)
        {
            Assert.AreEqual(expected, CommonEncoders.Minimize(value));
        }
    }
}
