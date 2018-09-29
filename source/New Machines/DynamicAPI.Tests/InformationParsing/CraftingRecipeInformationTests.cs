using Igorious.StardewValley.DynamicAPI.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Igorious.StardewValley.DynamicAPI.Tests.InformationParsing
{
    [TestClass]
    public sealed class CraftingRecipeInformationTests
    {
        [TestMethod]
        public void ParseRecipeWithoutSkill()
        {
            TestParsing("388 50/Home/130/true/null");
        }  

        [TestMethod]
        public void ParseRecipeWithSkill()
        {
            TestParsing("766 50 709 20 336 1/Home/19/true/Farming 8");
        }  
        
        [TestMethod]
        public void ParseDefaultRecipe()
        {
            TestParsing("388 1 92 2/Field/93/false/l 0");
        }  

        private static void TestParsing(string value)
        {
            var craftingRecipeInformation = CraftingRecipeInformation.Parse(value);
            Assert.AreEqual(value, craftingRecipeInformation.ToString());
        } 
    }
}
