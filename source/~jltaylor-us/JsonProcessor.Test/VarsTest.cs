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
    public class VarsTest {
        private readonly JObject testData;
        private readonly IJsonProcessor processor;
        private readonly LogCollector log;

        public VarsTest() {
            testData = ReadTestData("vars-test.json");
            log = new();
            processor = new JsonProcessorImpl(log.Log);
            new Define().AddTo(processor);
            new Let().AddTo(processor);
            new Var().AddTo(processor);
        }

        [TestMethod]
        public void TestUnbound() {
            Assert.IsTrue(AssertNoChanges("unbound", processor, testData["unbound"]));
        }

        [TestMethod]
        public void TestDefines() {
            AssertCleanTransform("test defines", processor, log, testData["defines"], testData["defines results"]);
            AssertCleanTransform("test defines", processor, log, testData["defines long-form"], testData["defines results"]);
            AssertCleanTransform("test defines", processor, log, testData["defines in obj"], testData["defines in obj results"]);
        }

        [TestMethod]
        [DataRow("define not obj", "[0].$define: must be an object")]
        [DataRow("var not string", "[0].$var: must be a string")]
        [DataRow("let missing bindings", "[0]: missing required \"bindings\" property for let transformer")]
        [DataRow("let missing body", "[0]: missing required \"body\" property for let transformer")]
        [DataRow("let bad bindings", "[0].bindings: must be an object")]
        public void TestErrors(string dataName, string expectedError) {
            JToken data = testData[dataName];
            bool result = AssertNoChanges("define error", processor, data);
            Assert.IsFalse(result, "transformer should return false");
            Assert.AreEqual(expectedError + "\n", log.ToString());
        }

        //[TestMethod]
        //// no tests of this shape?
        //public void TestBadDefinesWithReplacement(string dataName, string expectedError) {
        //    JToken data = testData[dataName].DeepClone();
        //    bool result = processor.Transform(data);
        //    Assert.AreEqual(expectedError + "\n", log.ToString());
        //    AssertTreesEqual(dataName, testData[dataName + " results"], data);
        //    Assert.IsFalse(result, "transformer should return false");
        //}

        [TestMethod]
        [DataRow("var simple")]
        [DataRow("var 2step")]
        [DataRow("var self-rec")]
        [DataRow("var co-rec")]
        [DataRow("let simple shadow")]
        [DataRow("let as dynamic scope")]
        public void TestVarClean(string dataName) {
            AssertCleanTransform("test var", processor, log, testData[dataName], testData[dataName + " results"]);
        }
    }
}

