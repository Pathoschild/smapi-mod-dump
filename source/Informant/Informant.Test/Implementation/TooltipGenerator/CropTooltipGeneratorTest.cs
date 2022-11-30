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
using NUnit.Framework;
using Slothsoft.Informant.Implementation.TooltipGenerator;
using StardewTests.Harness;

namespace InformantTest.Implementation.TooltipGenerator; 

[TestFixture]
public class CropTooltipGeneratorTest {
    
    private static readonly IModHelper ModHelper = new TestModHelper(TestUtils.ModFolder);
    private CropTooltipGenerator _classUnderTest = CreateClassUnderTest();

    private static CropTooltipGenerator CreateClassUnderTest() {
        return new CropTooltipGenerator(ModHelper);
    }

    [SetUp]
    public void SetUp() {
        _classUnderTest = CreateClassUnderTest();
    }

    [Test]
    public void Id() {
        Assert.NotNull(_classUnderTest.Id);
    }
    
    [Test]
    public void DisplayName() {
        Assert.NotNull(_classUnderTest.DisplayName);
    }
    
    [Test]
    public void Description() {
        Assert.NotNull(_classUnderTest.Description);
    }
    
    [Test]
    [TestCase(1,2, 0, new[] { 1, 2, 3, 4, 99999 }, false, "7 days left")] 
    [TestCase(3,3, 0, new[] { 1, 2, 3, 4, 99999 }, false, "1 day left")] 
    [TestCase(3,4, 0, new[] { 1, 2, 3, 4, 99999 }, false, "Ready to harvest")] 
    [TestCase(1,2, 0, new[] { 1, 2, 3, 4, 99999 }, true, "Dead")] 
    public void CalculateDaysLeftString(int currentPhase, int dayOfCurrentPhase, int regrowAfterHarvest, int[] cropPhaseDays, bool dead, string expectedString) {
        var crop = new Crop {
            currentPhase = { currentPhase },
            dayOfCurrentPhase = { dayOfCurrentPhase },
            regrowAfterHarvest = { regrowAfterHarvest },
            dead = { dead },
        };
        crop.phaseDays.AddRange(cropPhaseDays);
        Assert.AreEqual(expectedString, CropTooltipGenerator.CalculateDaysLeftString(ModHelper, crop));
    }
    
    [Test]
    [TestCase(0,0, 0, new[] { 1, 2, 3, 4, 99999 }, 10)] 
    [TestCase(0,1, 0, new[] { 1, 2, 3, 4, 99999 }, 9)] 
    [TestCase(1,2, 0, new[] { 1, 2, 3, 4, 99999 }, 7)] 
    [TestCase(3,3, 0, new[] { 1, 2, 3, 4, 99999 }, 1)] 
    [TestCase(3,4, 0, new[] { 1, 2, 3, 4, 99999 }, 0)] 
    [TestCase(4,4, 4, new[] { 1, 2, 3, 4, 99999 }, 4)] 
    [TestCase(4,0, 4, new[] { 1, 2, 3, 4, 99999 }, 0)] 
    // Amaranth:  current = 4 | day = 0 | days = 1, 2, 2, 2, 99999 | result => 0
    // Fairy Rose:  current = 4 | day = 1 | days = 1, 4, 4, 3, 99999 | result => 0
    // Cranberry:  current = 5 | day = 4 | days = 1, 2, 1, 1, 2, 99999 | result => ???
    // Ancient Fruit: current = 5 | day = 4 | days = 1 5 5 6 4 99999 | result => 4
    // Blueberry (harvested): current = 5 | day = 4 | days = 1 3 3 4 2 99999 | regrowAfterHarvest = 4 | result => 4
    // Blueberry (harvested): current = 5 | day = 0 | days = 1 3 3 4 2 99999 | regrowAfterHarvest = 4 | result => 0
    [TestCase(4,0, 0, new[] { 1, 2, 2, 2, 99999 }, 0)] 
    [TestCase(4,1, 0, new[] { 1, 4, 4, 3, 99999 }, 0)]
    [TestCase(5,4, 0, new[] { 1, 2, 1, 1, 2, 99999 }, 0)]
    [TestCase(5,4, 7, new[] { 1, 5, 5, 6, 4, 99999 },  4)]
    [TestCase(5,4, 4, new[] { 1, 3, 3, 4, 2, 99999 }, 4)]
    [TestCase(5,0, 4, new[] { 1, 3, 3, 4, 2, 99999 }, 0)]
    public void CalculateDaysLeft(int currentPhase, int dayOfCurrentPhase, int regrowAfterHarvest, int[] cropPhaseDays, int expectedDaysLeft) {
        var crop = new Crop {
            currentPhase = { currentPhase },
            dayOfCurrentPhase = { dayOfCurrentPhase },
            regrowAfterHarvest = { regrowAfterHarvest },
        };
        crop.phaseDays.AddRange(cropPhaseDays);
        Assert.AreEqual(expectedDaysLeft, CropTooltipGenerator.CalculateDaysLeft(crop));
    }
}