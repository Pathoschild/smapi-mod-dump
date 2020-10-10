/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Igorious/StardevValleyNewMachinesMod
**
*************************************************/

using System.Collections.Generic;
using System.Linq;
using Igorious.StardewValley.DynamicAPI.Constants;
using Igorious.StardewValley.DynamicAPI.Data.Supporting;
using Igorious.StardewValley.DynamicAPI.Extensions;
using Igorious.StardewValley.DynamicAPI.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace Igorious.StardewValley.DynamicAPI.Tests.Json
{
    [TestClass]
    public class JsonIdConverterTests
    {
        private static JsonSerializerSettings JsonSettings
        {
            get
            {
                var jsonSettings = new JsonSerializerSettings { DefaultValueHandling = DefaultValueHandling.Ignore };
                jsonSettings.Converters.AddDefaults();
                return jsonSettings;
            }
        }

        private class Mock1
        {
            public DynamicID<ItemID> ID { get; set; }
        }

        private class Mock2
        {
            public DynamicID<CraftableID> ID { get; set; }
        }

        [TestMethod]
        public void SerializeItemID()
        {
            var item1 = new Mock1 { ID = ItemID.Acorn };
            var json = item1.ToJson();
            Assert.IsTrue(json.Contains("Acorn"));

            var item2 = JsonConvert.DeserializeObject<Mock1>(json, JsonSettings);
            Assert.AreEqual(item1.ID, item2.ID);
        }

        [TestMethod]
        public void SerializeIntAsItemID()
        {
            var item1 = new Mock1 { ID = -2 };
            var json = item1.ToJson();
            Assert.IsTrue(json.Contains("-2"));

            var item2 = JsonConvert.DeserializeObject<Mock1>(json, JsonSettings);
            Assert.AreEqual(item1.ID, item2.ID);
        }

        [TestMethod]
        public void SerializeCraftableID()
        {
            var item1 = new Mock2 { ID = CraftableID.BeeHouse };
            var json = item1.ToJson();
            Assert.IsTrue(json.Contains("BeeHouse"));

            var item2 = JsonConvert.DeserializeObject<Mock2>(json, JsonSettings);
            Assert.AreEqual(item1.ID, item2.ID);
        }

        [TestMethod]
        public void SerializeIntAsCraftableID()
        {
            var item1 = new Mock2 { ID = -3 };
            var json = item1.ToJson();
            Assert.IsTrue(json.Contains("-3"));

            var item2 = JsonConvert.DeserializeObject<Mock2>(json, JsonSettings);
            Assert.AreEqual(item1.ID, item2.ID);
        }

        [TestMethod]
        public void SerializeDictionary()
        {
            var item1 = new Dictionary<DynamicID<ItemID>, DynamicID<CraftableID>>
            {
                { ItemID.Acorn, -2 },
                { 1000, CraftableID.BeeHouse },
            };
            var json = item1.ToJson();
            Assert.IsTrue(json.Contains("Acorn"));
            Assert.IsTrue(json.Contains("BeeHouse"));
            Assert.IsTrue(json.Contains("-2"));
            Assert.IsTrue(json.Contains("1000"));

            var item2 = JsonConvert.DeserializeObject<Dictionary<DynamicID<ItemID>, DynamicID<CraftableID>>>(json, JsonSettings);
            Assert.AreEqual(item1.Keys.First(), item2.Keys.First());
            Assert.AreEqual(item1.Keys.Last(), item2.Keys.Last());
            Assert.AreEqual(item1.Values.First(), item2.Values.First());
            Assert.AreEqual(item1.Values.Last(), item2.Values.Last());
        }

        [TestMethod]
        public void DoubleConverterSmokeTest()
        {
            var c = new JsonDynamicIdConverter<ItemID, CategoryID>();
        }
    }
}
