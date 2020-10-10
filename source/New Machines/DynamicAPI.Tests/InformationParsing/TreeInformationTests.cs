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
