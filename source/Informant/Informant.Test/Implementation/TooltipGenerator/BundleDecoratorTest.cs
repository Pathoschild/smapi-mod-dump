/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/slothsoft/stardew-informant
**
*************************************************/

using System;
using System.Linq;
using Netcode;
using NUnit.Framework;
using Slothsoft.Informant.Implementation.Decorator;
using StardewTests.Harness.Game;
using StardewValley.Network;

namespace InformantTest.Implementation.TooltipGenerator; 

[TestFixture]
public class BundleDecoratorTest {
    
    // FIXME: When the IModContentHelper is implemented, this might work, too:
    //
    // private BundleDecorator _classUnderTest = CreateClassUnderTest();
    //
    // private static BundleDecorator CreateClassUnderTest() {
    //     return new BundleDecorator(new TestModHelper(TestUtils.ModFolder));
    // }
    //
    // [SetUp]
    // public void SetUp() {
    //     _classUnderTest = CreateClassUnderTest();
    // }
    //
    // [Test]
    // public void Id() {
    //     Assert.NotNull(_classUnderTest.Id);
    // }
    //
    // [Test]
    // public void DisplayName() {
    //     Assert.NotNull(_classUnderTest.DisplayName);
    // }
    //
    // [Test]
    // public void Description() {
    //     Assert.NotNull(_classUnderTest.Description);
    // }
    //
    // [Test]
    // public void HasDecoration() {
    //     Assert.IsTrue(_classUnderTest.HasDecoration(null));
    // }
    
    
    // See https://stardewvalleywiki.com/Modding:Bundles
    
    [Test]
    [TestCase("Pantry/0", "Spring Crops/O 465 20/24 1 0 188 1 0 190 1 0 192 1 0/0", new[] {false, false, false, false}, new[] { 24, 188, 190, 192 })] 
    [TestCase("Pantry/1", "Summer Crops/O 621 1/256 1 0 260 1 0 258 1 0 254 1 0/3", new[] {false, true, true, false}, new[] { 256, 254 })] 
    [TestCase("Pantry/2", "Fall Crops/BO 10 1/270 1 0 272 1 0 276 1 0 280 1 0/2", new[] {true, true, true, true}, new int[0])] 
    public void GetNeededItems(string bundleTitle, string bundleData, bool[] bundlesCompleted, int[] expectedNeededItems) {
        var worldState = new TestWorldState {
            BundleData = {
                [bundleTitle] = bundleData
            },
            Bundles = {
                [Convert.ToInt32(bundleTitle.Split('/')[1])] = bundlesCompleted
            }
        };

        Game1.netWorldState = new NetRoot<IWorldState>(worldState);
        
        Assert.AreEqual(expectedNeededItems, BundleDecorator.GetNeededItems(null, true).ToArray());
        
        // since the game is not set up correctly, decorateLockedBundles=false will return an empty array
        Assert.AreEqual(Array.Empty<int>(), BundleDecorator.GetNeededItems(null, false).ToArray());
    }
}