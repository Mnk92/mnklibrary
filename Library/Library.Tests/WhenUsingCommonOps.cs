﻿using Mnk.Library.Common;
using NUnit.Framework;

namespace Mnk.Library.Tests
{
    [TestFixture]
    class WhenUsingCommonOps
    {
        [Test]
        [TestCase("", "")]
        [TestCase("a", "a")]
        [TestCase("\r", "\\r")]
        [TestCase("\r\n", "\\r\\n")]
        [TestCase("\\", "\\\\")]
        [TestCase("\\\\", "\\\\\\\\")]
        [TestCase("\\\r", "\\\\\\r")]
        public void Should_encode_string(string value, string expected)
        {
            Assert.AreEqual(expected, CommonEncoders.EncodeString(value));
        }

        [Test]
        [TestCase("", "")]
        [TestCase("a", "a")]
        [TestCase("\\\\", "\\")]
        [TestCase("\\r\\n", "\r\n")]
        [TestCase("\\", "\\")]
        [TestCase("\\\\r", "\\r")]
        [TestCase("test\\", "test\\")]
        [TestCase("\"test\\\"", "\"test\"")]
        public void Should_decode_string(string value, string expected)
        {
            Assert.AreEqual(expected, CommonEncoders.DecodeString(value));
        }

        [Test]
        [TestCase("", "")]
        [TestCase("a", "a")]
        [TestCase("  ", " ")]
        [TestCase("\r\n", " ")]
        [TestCase("\r\n       ", " ")]
        [TestCase("a\r\nb      c", "a b c")]
        public void Should_minimize_string(string value, string expected)
        {
            Assert.AreEqual(expected, CommonEncoders.Minimize(value));
        }

    }
}