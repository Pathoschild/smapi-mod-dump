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
    public class ForEachTest {
        private readonly JObject testData;
        private readonly IJsonProcessor processor;
        private readonly LogCollector log;
        public ForEachTest() {
            testData = ReadTestData("for-each-test.json");
            log = new();
            processor = new JsonProcessorImpl(log.Log);
            new Var().AddTo(processor);
            new ForEach().AddTo(processor);
        }

        [TestMethod]
        [DataRow("missing var", "[0]: missing required \"var\" property for for-each transformer")]
        [DataRow("missing in", "[0]: missing required \"in\" property for for-each transformer")]
        [DataRow("missing yield", "[0]: missing required \"yield\" property for for-each transformer")]
        [DataRow("bad in", "[0].in: must be an array or object")]
        [DataRow("array with bad var", "[0].var: must be a string or array containing one string")]
        [DataRow("object with bad var", "[0].var: must be an array containing two strings")]
        [DataRow("0 var array", "[0].var: must be a string or array containing one string")]
        [DataRow("2 var array", "[0].var: must be a string or array containing one string")]
        [DataRow("non-string var array", "[0].var: must be a string or array containing one string")]
        [DataRow("0 var object", "[0].var: must be an array containing two strings")]
        [DataRow("1 var object", "[0].var: must be an array containing two strings")]
        [DataRow("3 var object", "[0].var: must be an array containing two strings")]
        [DataRow("non-string var 1 object", "[0].var: must be an array containing two strings")]
        [DataRow("non-string var 2 object", "[0].var: must be an array containing two strings")]
        public void TestErrors(string dataName, string expectedError) {
            JToken data = testData[dataName];
            bool result = AssertNoChanges("define error", processor, data);
            Assert.IsFalse(result, "transformer should return false");
            Assert.AreEqual(expectedError + "\n", log.ToString());
        }

        [TestMethod]
        [DataRow("empty")]
        [DataRow("empty [var]")]
        [DataRow("empty obj")]
        public void TestEmpty(string dataName) {
            AssertCleanTransform("test for-each", processor, log, testData[dataName], testData["empty results"]);
        }

        [TestMethod]
        [DataRow("array identity")]
        [DataRow("object explode")]
        public void TestClean(string dataName) {
            AssertCleanTransform("test for-each", processor, log, testData[dataName], testData[dataName + " results"]);
        }

    }
}

