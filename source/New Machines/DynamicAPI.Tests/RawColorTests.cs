using Igorious.StardewValley.DynamicAPI.Data.Supporting;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Igorious.StardewValley.DynamicAPI.Tests
{
    [TestClass]
    public class RawColorTests
    {
        [TestMethod]
        public void CheckHexConversion()
        {
            var raw = RawColor.FromHex("12AB34");
            Assert.AreEqual(18, raw.R);
            Assert.AreEqual(171, raw.G);
            Assert.AreEqual(52, raw.B);

            var hex = raw.ToHex();
            Assert.AreEqual("12AB34", hex);
        }

        [TestMethod]
        public void CheckHsbConversion()
        {
            var c1 = new RawColor(18, 171, 52);
            double h, s, l;
            c1.ToHSL(out h, out s, out l);
            var c2 = RawColor.FromHSL(h, s, l);
            Assert.AreEqual(c1.R, c2.R, 1);
            Assert.AreEqual(c1.G, c2.G, 1);
            Assert.AreEqual(c1.B, c2.B, 1);
        }
    }
}
