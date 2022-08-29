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
    public class RealWorldTest {
        private readonly IJsonProcessor processor;
        private readonly LogCollector log;

        public RealWorldTest() {
            log = new();
            processor = new JsonProcessorImpl(log.Log);
            JsonProcessorAPI.AddDefaultProcessors(processor);
        }

        [TestMethod]
        public void TestAccessory() {
            JObject orig = ReadTestData("accessory-orig.json");
            AssertCleanTransform("test accessory.json", processor, log, ReadTestData("accessory.json"), orig);
            AssertCleanTransform("test accessory2.json", processor, log, ReadTestData("accessory2.json"), orig);
        }
    }
}

