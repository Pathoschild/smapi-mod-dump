using Igorious.StardewValley.DynamicAPI.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Igorious.StardewValley.DynamicAPI.Tests.InformationParsing
{
    [TestClass]
    public sealed class TreeInformationTests
    {
        [TestMethod]
        public void ParseTree()
        {
            TestParsing("2/summer/635/1234");
        }  
        
        private static void TestParsing(string value)
        {
            var treeInformation = TreeInformation.Parse(value);
            Assert.AreEqual(value, treeInformation.ToString());
        } 
    }
}
