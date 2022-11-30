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
using System.Collections.Generic;
using NUnit.Framework;
using Slothsoft.Informant.Implementation.Common;
using Slothsoft.Informant.Implementation.TooltipGenerator;
using StardewTests.Harness;
using StardewValley.Buildings;
using StardewValley.TerrainFeatures;

namespace InformantTest.Implementation.TooltipGenerator; 

[TestFixture]
public class FruitTreeTooltipGeneratorTest {

    private FruitTreeTooltipGenerator _classUnderTest = CreateClassUnderTest();

    private static FruitTreeTooltipGenerator CreateClassUnderTest() {
        return new FruitTreeTooltipGenerator(new TestModHelper(TestUtils.ModFolder));
    }

    [OneTimeSetUp]
    public void SetUpOnce() {
        Game1.objectInformation ??= new Dictionary<int, string>();
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
    public void HasTooltip_True() {
        Assert.IsTrue(_classUnderTest.HasTooltip(new FruitTree()));
    }
    
    [Test]
    public void HasTooltip_False() {
        Assert.IsFalse(_classUnderTest.HasTooltip(new Grass()));
    }

    [Test]
    [TestCase(5,Seasons.Spring, 0, 3, Seasons.Spring, "Ready to harvest")] 
    [TestCase(10,Seasons.Summer, 0, 0, Seasons.Summer, "1 day left")]
    [TestCase(10,Seasons.Spring, 10, 0, Seasons.Spring, "10 days left")] 
    [TestCase(10,Seasons.Spring, 10, 0, "unsupported season", "")] 
    public void Generate(int dayOfMonth, string season, int daysUntilMature, int fruitsOnTree, string fruitSeason, string expectedString) {
        Game1.dayOfMonth = dayOfMonth;
        Game1.currentSeason = season;
        
        var fruitTree = new FruitTree {
            daysUntilMature = { daysUntilMature },
            fruitsOnTree = { fruitsOnTree },
            fruitSeason = { fruitSeason }
        };
        Assert.AreEqual($"???\n{expectedString}", _classUnderTest.Generate(fruitTree).Text);
    }
    
    [Test]
    [TestCase(10,Seasons.Spring, 10, 0, Seasons.Spring, 10)] 
    [TestCase(10,Seasons.Spring, 10, 0, Seasons.Summer, 18)]
    [TestCase(10,Seasons.Spring, 10, 0, Seasons.Fall, 46)]
    [TestCase(10,Seasons.Spring, 18, 0, Seasons.Spring, 102)]
    [TestCase(10,Seasons.Summer, 18, 0, Seasons.Summer, 102)]
    [TestCase(10,Seasons.Fall, 18, 0, Seasons.Fall, 102)]
    [TestCase(10,Seasons.Summer, 0, 0, Seasons.Summer, 1)]
    [TestCase(10,Seasons.Fall, 0, 1, Seasons.Fall, 0)]
    [TestCase(10,Seasons.Fall, 0, 3, Seasons.Fall, 0)]
    [TestCase(5,Seasons.Spring, 0, 1, Seasons.Summer, 0)]
    [TestCase(5,Seasons.Spring, 0, 0, Seasons.Summer, 23)]
    [TestCase(5,Seasons.Spring, 0, 0, "unsupported season", -1)]
    public void CalculateDaysLeft(int dayOfMonth, string season, int daysUntilMature, int fruitsOnTree, string fruitSeason, int expectedDaysLeft) {
        Game1.dayOfMonth = dayOfMonth;
        Game1.currentSeason = season;
        
        var fruitTree = new FruitTree {
            daysUntilMature = { daysUntilMature },
            fruitsOnTree = { fruitsOnTree },
            fruitSeason = { fruitSeason }
        };
        Assert.AreEqual(expectedDaysLeft, _classUnderTest.CalculateDaysLeft(fruitTree));
    }
    
    [Test]
    [TestCase(10,Seasons.Spring, 10, 0, Seasons.Summer, 10)]
    [TestCase(10,Seasons.Spring, 18, 0, Seasons.Spring, 18)]
    [TestCase(10,Seasons.Fall, 0, 1, Seasons.Fall, 0)]
    [TestCase(5,Seasons.Spring, 0, 0, "unsupported season", 1)] // we don't check the season
    public void CalculateDaysLeftInGreenhouse(int dayOfMonth, string season, int daysUntilMature, int fruitsOnTree, string fruitSeason, int expectedDaysLeft) {
        Game1.dayOfMonth = dayOfMonth;
        Game1.currentSeason = season;
        
        var fruitTree = new FruitTree {
            currentLocation = new Farm {
                isGreenhouse = { true },
            },
            daysUntilMature = { daysUntilMature },
            fruitsOnTree = { fruitsOnTree },
            fruitSeason = { fruitSeason },
        };
        Assert.AreEqual(expectedDaysLeft, _classUnderTest.CalculateDaysLeft(fruitTree));
    }
    
    [Test]
    [TestCase(10,Seasons.Spring, 10, 0, Seasons.Summer, 10)]
    [TestCase(10,Seasons.Spring, 18, 0, Seasons.Spring, 18)]
    [TestCase(10,Seasons.Fall, 0, 1, Seasons.Fall, 0)]
    [TestCase(5,Seasons.Spring, 0, 0, "unsupported season", 1)] // we don't check the season
    public void CalculateDaysLeftOnIsland(int dayOfMonth, string season, int daysUntilMature, int fruitsOnTree, string fruitSeason, int expectedDaysLeft) {
        Game1.dayOfMonth = dayOfMonth;
        Game1.currentSeason = season;
        
        var fruitTree = new FruitTree {
            currentLocation = new Farm {
                name = { "IslandWest" },
            },
            daysUntilMature = { daysUntilMature },
            fruitsOnTree = { fruitsOnTree },
            fruitSeason = { fruitSeason },
        };
        Assert.AreEqual(expectedDaysLeft, _classUnderTest.CalculateDaysLeft(fruitTree));
    }
}