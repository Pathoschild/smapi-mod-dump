using Igorious.StardewValley.DynamicAPI.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Igorious.StardewValley.DynamicAPI.Tests.InformationParsing
{
    [TestClass]
    public sealed class BundlesInformationTests
    {
        [TestMethod]
        public void BundleParsing1()
        {
            TestParsing("Spring Crops/O 465 20/24 1 0 188 1 0 190 1 0 192 1 0/0");
        }  

        [TestMethod]
        public void BundleParsing2()
        {
            TestParsing("Artisan/BO 12 1/432 1 0 428 1 0 426 1 0 424 1 0 340 1 0 344 1 0 613 1 0 634 1 0 635 1 0 636 1 0 637 1 0 638 1 0/1/6");
        }  
        
        private static void TestParsing(string value)
        {
            var bundleInformation = BundleInformation.Parse(value);
            Assert.AreEqual(value, bundleInformation.ToString());
        } 
    }
}
