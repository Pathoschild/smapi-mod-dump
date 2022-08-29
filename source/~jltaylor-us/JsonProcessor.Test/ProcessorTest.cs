/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/jltaylor-us/StardewJsonProcessor
**
*************************************************/

using Microsoft.VisualStudio.TestTools.UnitTesting;

using JsonProcessor.Framework;
using Newtonsoft.Json.Linq;
using JsonProcessor.Framework.Transformers;

using static JsonProcessor.Test.Utils;

namespace JsonProcessor.Test {
    [TestClass]
    public class ProcessorTest {
        private readonly JObject testData;
        public ProcessorTest() {
            testData = ReadTestData("processor-test.json");
        }

        [TestMethod]
        public void TestNoTransformers() {
            LogCollector log = new();
            Assert.IsTrue(log.IsEmpty);
            JsonProcessorImpl processor = new(log.Log);
            AssertNoChanges("no transformers", processor, testData);
        }

        [TestMethod]
        public void TestAddRemove() {
            LogCollector log = new();
            IJsonProcessor processor = new JsonProcessorImpl(log.Log);
            JToken data = testData["test transformer"];
            AssertNoChanges("before adding", processor, data);
            log.Clear();
            processor.AddTransformer("test", (proc, obj) => { obj.Replace(new JValue(42)); return true; });
            AssertCleanTransform("test transformer", processor, log, data, testData["test transformer results"]);
            processor.RemoveTransformer("test");
            AssertNoChanges("after remove", processor, data);
        }

        [TestMethod]
        public void TestShorthandAddRemove() {
            LogCollector log = new();
            IJsonProcessor processor = new JsonProcessorImpl(log.Log);
            JToken data = testData["test transformer shorthand"];
            AssertNoChanges("before adding", processor, data);
            log.Clear();
            processor.AddShorthandTransformer("test", "arg", (proc, obj, val) => { obj.Replace(val); return true; });
            AssertCleanTransform("test shorthand transformer", processor, log, data, testData["test transformer shorthand results"]);
            processor.RemoveTransformer("test");
            AssertNoChanges("after remove", processor, data);
        }
        [TestMethod]
        public void TestPropertyAddRemove() {
            LogCollector log = new();
            IJsonProcessor processor = new JsonProcessorImpl(log.Log);
            JToken data = testData["test property transformer"];
            AssertNoChanges("before adding", processor, data);
            log.Clear();
            processor.AddPropertyTransformer("test", (proc, prop) => {
                if (prop.Value.Type == JTokenType.String) {
                    prop.Replace(new JProperty(prop.Value.Value<string>(), new JValue(42)));
                } else {
                    prop.Replace(new JProperty("oops", prop.Value));
                }
                return true;
            });
            AssertCleanTransform("test property transformer", processor, log, data, testData["test property transformer results"]);
            processor.RemoveTransformer("test");
            AssertNoChanges("after remove", processor, data);
        }

        [TestMethod]
        public void TestUnknown() {
            LogCollector log = new();
            IJsonProcessor processor = new JsonProcessorImpl(log.Log);
            Assert.IsFalse(AssertNoChanges("unknown processor", processor, testData["unknown"]));
            Assert.AreEqual("[0]: don't have any transformer named unknown transformer\n", log.ToString());
            log.Clear();
            Assert.IsTrue(AssertNoChanges("unknown shorthand", processor, testData["unknown shorthand"]));
            Assert.IsTrue(log.IsEmpty);
        }

        [TestMethod]
        public void TestNoTransformersAtRoot() {
            LogCollector log = new();
            IJsonProcessor processor = new JsonProcessorImpl(log.Log);
            processor.AddShorthandTransformer("test", "arg", (proc, obj, val) => { obj.Replace(val); return true; });
            Assert.IsFalse(AssertNoChanges("transformer at root", processor, testData["transformer at root"]));
            Assert.AreEqual(": The root node can't be a transformer\n", log.ToString());
            log.Clear();
            Assert.IsFalse(AssertNoChanges("transformer at root", processor, testData["shorthand at root"]));
            Assert.AreEqual(": The root node can't be a transformer\n", log.ToString());
        }

    }
}