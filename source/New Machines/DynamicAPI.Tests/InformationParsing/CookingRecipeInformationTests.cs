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
    public sealed class CookingRecipeInformationTests
    {
        [TestMethod]
        public void ParseSkillRecipe()
        {
            TestParsing("131 2 210 1/38 2/242/s Fishing 3");
        }  

        [TestMethod]
        public void ParseFriendshipRecipe()
        {
            TestParsing("256 1 284 1/1 8/200/f Caroline 7");
        }  
        
        [TestMethod]
        public void ParseDefaultRecipe()
        {
            TestParsing("-5 1/10 10/194/default");
        }  

        [TestMethod]
        public void ParseTvRecipe()
        {
            TestParsing("270 1/5 4/229/l 13");
        } 

        private static void TestParsing(string value)
        {
            var cookingRecipeInformation = CookingRecipeInformation.Parse(value);
            Assert.AreEqual(value, cookingRecipeInformation.ToString());
        } 
    }
}
