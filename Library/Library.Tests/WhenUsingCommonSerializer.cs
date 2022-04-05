using System;
using System.IO;
using Mnk.Library.Common;
using Mnk.Library.Common.AutoUpdate;
using Mnk.Library.Common.SaveLoad;
using Mnk.Library.Common.UI.Model;
using NUnit.Framework;

namespace Mnk.Library.Tests
{
    [TestFixture]
    class WhenUsingCommonSerializer
    {
        class TestData : PairData
        {
            public double DefaultTest { get; set; }

            public TestData()
            {
                DefaultTest = 123;
            }
        }
        private readonly string OutputPath = "test.json";

        [SetUp]
        public void SetUp()
        {
            File.Delete(OutputPath);
        }

        [Test]
        public void ShouldSerializeDefaultsTest()
        {
            var expected = new TestData { Key = "1", Value = "2" };
            {
                var old = new PairData { Key = expected.Key, Value = expected.Value };
                var serializer = new AnySerializer(OutputPath, old.GetType());
                Assert.IsTrue(serializer.Save(old));
            }
            {
                var serializer = new AnySerializer(OutputPath, expected.GetType());
                var actual = serializer.Load() as TestData;
                Assert.IsNotNull(actual);
                Assert.AreEqual(expected.Key, actual.Key);
                Assert.AreEqual(expected.Value, actual.Value);
                // New fields should be initialized with default values.
                Assert.AreEqual(expected.DefaultTest, actual.DefaultTest);
            }
        }

        class DateTest
        {
            public DateTime Time { get; set; }
        }

        [Test]
        public void ShouldSerializeDateTimeTest()
        {
            var expected = new DateTest { Time = new DateTime(1, 1, 1) };
            File.WriteAllText(OutputPath, "{\"Time\":\"\\/Date(-62135596800000-0000)\\/\"}");
            var serializer = new AnySerializer(OutputPath, expected.GetType());
            var actual = serializer.Load() as DateTest;
            Assert.IsNotNull(actual);
            Assert.AreEqual(expected.Time, actual.Time);
        }

        public class UpdateTest
        {
            public UpdateInterval Interval { get; set; }
            public DateTime Last { get; set; }
            public bool ShowChangeLog { get; set; }
            public long LastChangeLogPosition { get; set; }
            public UpdateTest()
            {
                Interval = UpdateInterval.Startup;
                ShowChangeLog = true;
                LastChangeLogPosition = 0;

            }
        }

        [Test]
        public void ShouldSerializeComplexTypesTest()
        {
            var expected = new UpdateTest { Interval = UpdateInterval.Daily, Last = DateTime.Today };
            {
                var serializer = new AnySerializer(OutputPath, expected.GetType());
                Assert.IsTrue(serializer.Save(expected));
            }
            {
                var serializer = new AnySerializer(OutputPath, expected.GetType());
                var actual = serializer.Load() as UpdateTest;
                Assert.IsNotNull(actual);
                Assert.AreEqual(expected.Interval, actual.Interval);
                Assert.AreEqual(expected.Last, actual.Last);
                Assert.AreEqual(expected.ShowChangeLog, actual.ShowChangeLog);
                Assert.AreEqual(expected.LastChangeLogPosition, actual.LastChangeLogPosition);
            }
        }
    }
}
