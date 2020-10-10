/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Igorious/StardevValleyNewMachinesMod
**
*************************************************/

using Igorious.StardewValley.DynamicAPI.Utils;
using Igorious.StardewValley.NewMachinesMod.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Igorious.StardewValley.DynamicAPI.Tests
{
    [TestClass]
    public class ExpressionCompilerTests
    {
        [TestMethod]
        public void InvokeIntExpression()
        {
            var f = ExpressionCompiler.CompileExpression<PriceExpression>("100");
            var result = f(1, 350);
            Assert.AreEqual(100, result);
        }

        [TestMethod]
        public void InvokeArithmeticExpression()
        {
            var f = ExpressionCompiler.CompileExpression<CountExpression>("1 + q + p / 200");
            var result = f(350, 1, 0.3, 0.6);
            Assert.AreEqual(3, result);
        }

        [TestMethod]
        public void InvokeConditionalExpression()
        {
            var f = ExpressionCompiler.CompileExpression<QualityExpression>("(r1 > 0.9)? 2 : (r2 > 0.2)? 1 : 0");
            var result = f(350, 1, 0.3, 0.6);
            Assert.AreEqual(1, result);
        }
    }
}
