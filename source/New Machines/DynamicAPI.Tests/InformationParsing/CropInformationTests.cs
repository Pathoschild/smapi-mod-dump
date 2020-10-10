/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Igorious/StardevValleyNewMachinesMod
**
*************************************************/

using Igorious.StardewValley.DynamicAPI.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Igorious.StardewValley.DynamicAPI.Tests.InformationParsing
{
    [TestClass]
    public sealed class CropInformationTests
    {
        [TestMethod]
        public void ParseSimpleCrop()
        {
            TestParsing("2 4 6 6 6/fall/32/417/-1/0/false/false/false");
        }

        [TestMethod]
        public void ParseManySeasonsCrop()
        {
            TestParsing("2 7 7 7 5/spring summer fall/24/454/7/0/false/false/false");
        }

        [TestMethod]
        public void ParseCropWithHarvestParameters()
        {
            TestParsing("1 1 1 2 1/spring/3/192/-1/0/true 1 2 8 .2/false/false");
        }

        [TestMethod]
        public void ParseFlower()
        {
            TestParsing("1 2 3 2/summer/29/593/-1/0/false/false/true 0 208 255 99 255 210 255 212 0 255 144 122 255 0 238 206 91 255");
        } 
        
        private static void TestParsing(string value)
        {
            var cropInformation = CropInformation.Parse(value);
            Assert.AreEqual(value, cropInformation.ToString());
        } 
    }
}
