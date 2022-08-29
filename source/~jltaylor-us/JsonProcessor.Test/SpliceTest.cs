/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/jltaylor-us/StardewJsonProcessor
**
*************************************************/

// // Copyright 2022 Jamie Taylor
using JsonProcessor.Framework;
using Newtonsoft.Json.Linq;
using JsonProcessor.Framework.Transformers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using static JsonProcessor.Test.Utils;

namespace JsonProcessor.Test {
    [TestClass]
    public class SpliceTest {
        private readonly JObject testData;
        public SpliceTest() {
            testData = ReadTestData("splice-test.json");
        }
        [TestMethod]
        public void TestSpliceArray() {
            LogCollector log = new();
            IJsonProcessor processor = new JsonProcessorImpl(log.Log);
            new Splice().AddTo(processor);
            JToken xformData = testData["splice array"].DeepClone();
            JToken shorthandData = testData["splice shorthand array"].DeepClone();
            JToken expectedResults = testData["splice array results"];
            Assert.IsTrue(processor.Transform(xformData));
            Assert.IsTrue(processor.Transform(shorthandData));
            AssertTreesEqual("splice array", expectedResults, xformData);
            AssertTreesEqual("splice array shorthand", expectedResults, shorthandData);
            Assert.IsTrue(log.IsEmpty);
        }

        [TestMethod]
        public void TestSpliceObject() {
            LogCollector log = new();
            IJsonProcessor processor = new JsonProcessorImpl(log.Log);
            new Splice().AddTo(processor);
            JToken xformData = testData["splice object"].DeepClone();
            JToken shorthandData = testData["splice shorthand object"].DeepClone();
            JToken expectedResults = testData["splice object results"];
            Assert.IsTrue(processor.Transform(xformData));
            Assert.AreEqual("", log.ToString(), "unexpected error log from splice object");
            Assert.IsTrue(processor.Transform(shorthandData));
            Assert.AreEqual("", log.ToString(), "unexpected error log from splice shorthand object");
            AssertTreesEqual("splice object", expectedResults, xformData);
            AssertTreesEqual("splice object shorthand", expectedResults, shorthandData);
        }

        [TestMethod]
        [DataRow("splice error 1", "[0].$splice: can't splice content of type Object into a parent of type Array")]
        [DataRow("splice error 2", "[0].content: can't splice content of type Object into a parent of type Array")]
        [DataRow("splice error 3", "foo.$splice: can't splice content of type Array into a parent of type Object")]
        [DataRow("splice error 4", "$splice: can't splice content of type Array into a parent of type Object")]
        [DataRow("splice error top level", ": The root node can't be a transformer")]
        public void TestSpliceErrors(string dataName, string expectedError) {
            LogCollector log = new();
            IJsonProcessor processor = new JsonProcessorImpl(log.Log);
            new Splice().AddTo(processor);
            JToken data = testData[dataName].DeepClone();
            bool result = AssertNoChanges("splice error", processor, data);
            Assert.IsFalse(result, "transformer should return false");
            Assert.AreEqual(expectedError + "\n", log.ToString());
        }

        [TestMethod]
        public void TestDuplicateKey() {
            LogCollector log = new();
            IJsonProcessor processor = new JsonProcessorImpl(log.Log);
            new Splice().AddTo(processor);
            JToken data = testData["duplicate key"].DeepClone();
            JToken expected = testData["duplicate key results"];
            bool result = processor.Transform(data);
            Assert.AreEqual("$splice.foo: parent already has a value for this key; skipping\n", log.ToString());
            Assert.IsFalse(result, "processor should have returned false when splicing encountered duplicate key");
            AssertTreesEqual("splice duplicate key", data, expected);
            log.Clear();
            data = testData["duplicate key long-form"].DeepClone();
            result = processor.Transform(data);
            Assert.AreEqual("abc.content.foo: parent already has a value for this key; skipping\n", log.ToString());
            Assert.IsFalse(result, "processor should have returned false when splicing encountered duplicate key");
            AssertTreesEqual("splice duplicate key", data, expected);

        }
    }
}
