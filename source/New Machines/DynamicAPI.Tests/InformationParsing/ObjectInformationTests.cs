using Igorious.StardewValley.DynamicAPI.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Igorious.StardewValley.DynamicAPI.Tests.InformationParsing
{
    [TestClass]
    public sealed class ObjectInformationTests
    {
        [TestMethod]
        public void ParseMineral()
        {
            TestParsing("Emerald/250/-300/Minerals -2/A precious stone with a brilliant green color.");
        }

        [TestMethod]
        public void ParseBasic()
        {
            TestParsing("Sap/2/-1/Basic -81/A fluid obtained from trees.");
        }

        [TestMethod]
        public void ParseCraftable()
        {
            TestParsing("Torch/5/-300/Crafting/Provides a modest amount of light.");
        }

        [TestMethod]
        public void ParseArch()
        {
            TestParsing("Rare Disc/300/-300/Arch/A heavy black disc studded with peculiar red stones. When you hold it, you're overwhelmed with a feeling of dread./UndergroundMine .01/Decor 1 29");
        }

        [TestMethod]
        public void ParseFish()
        {
            TestParsing("Carp/30/5/Fish -4/A common pond fish./Day Night^Spring Summer Fall");
        }

        [TestMethod]
        public void ParseCooking()
        {
            TestParsing("Omelet/125/40/Cooking -7/It's super fluffy./food/0 0 0 0 0 0 0 0 0 0 0/0");
        }

        [TestMethod]
        public void ParseSeeds()
        {
            TestParsing("Tulip Bulb/10/-300/Seeds -74/Plant in spring. Takes 6 days to produce a colorful flower. Assorted colors.");
        }

        [TestMethod]
        public void ParseRing()
        {
            TestParsing("Magnet Ring/Increases your radius for collecting items./200/Ring");
        }

        private static void TestParsing(string value)
        {
            var objectInformation = ItemInformation.Parse(value);
            Assert.AreEqual(value, objectInformation.ToString());
        } 
    }
}
