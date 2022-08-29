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
    public class StringJoinTest {
        private readonly JObject testData;

        public StringJoinTest() {
            testData = ReadTestData("string-join-test.json");
        }

        [TestMethod]
        public void TestStringJoin() {
            LogCollector log = new();
            IJsonProcessor processor = new JsonProcessorImpl(log.Log);
            new StringJoin().AddTo(processor);
            JToken expectedResults = testData["expected"];
            AssertCleanTransform("string join full-form", processor, log, testData["full-form"], expectedResults);
            AssertCleanTransform("string join shorthand", processor, log, testData["shorthand"], expectedResults);
        }

        [TestMethod]
        public void TestStringJoinDelimiter() {
            LogCollector log = new();
            IJsonProcessor processor = new JsonProcessorImpl(log.Log);
            new StringJoin().AddTo(processor);
            JToken expectedResults = testData["delimiter expected"];
            AssertCleanTransform("string join delimiter", processor, log, testData["delimiter"], expectedResults);
        }

        [TestMethod]
        [DataRow("missing strings", "[0]: missing required \"strings\" property for string-join transformer")]
        [DataRow("bad delimiter", "[0].delimiter: string-join delimiter must be a string, but it is a Integer")]
        [DataRow("bad strings", "[0].strings: string-join needs an array, but got a Integer")]
        [DataRow("bad strings shorthand", "[0].$string-join: string-join needs an array, but got a Integer")]
        public void TestStringJoinErrors(string dataName, string expectedError) {
            LogCollector log = new();
            IJsonProcessor processor = new JsonProcessorImpl(log.Log);
            new StringJoin().AddTo(processor);
            JToken data = testData[dataName].DeepClone();
            bool result = AssertNoChanges("string join error", processor, data);
            Assert.IsFalse(result, "transformer should return false");
            Assert.AreEqual(expectedError + "\n", log.ToString());
        }

        [TestMethod]
        public void TestBadString() {
            LogCollector log = new();
            IJsonProcessor processor = new JsonProcessorImpl(log.Log);
            new StringJoin().AddTo(processor);
            JToken data = testData["bad string"].DeepClone();
            bool result = processor.Transform(data);
            Assert.AreEqual("[0].$string-join[1]: ignoring non-string in string-join\n", log.ToString());
            AssertTreesEqual("string join bad string", testData["bad string results"], data);
            Assert.IsFalse(result, "transformer should return false");
        }

    }


}

