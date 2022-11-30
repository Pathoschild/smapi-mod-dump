/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/slothsoft/stardew-challenger
**
*************************************************/

using System;
using System.Reflection;
using ChallengerTest.Common;
using ChallengerTest.Restrictions;
using Moq;
using Netcode;
using NUnit.Framework;
using Slothsoft.Challenger;
using Slothsoft.Challenger.Api;
using StardewValley.Network;

namespace ChallengerTest.Challenges;

internal class BaseChallengeTest {
    private TestModHelper _modHelper = new();
    private TestBaseChallenge _classUnderTest = new(new TestModHelper());
    
    [SetUp]
    public void Setup() {
        _modHelper = new();
        _classUnderTest = new(_modHelper);

        Game1.content = new LocalizedContentManager(Mock.Of<IServiceProvider>(), "S:\\Spiele\\Steam\\steamapps\\common\\Stardew Valley\\Content");
        Game1.netWorldState = new NetRoot<IWorldState>(new NetWorldState());
    }

    [Test]
    public void GetProgress_ForUncompleted() {
        _classUnderTest.Goal.Progress = "Progress 50/100";
        var progress = _classUnderTest.GetProgress(Difficulty.Easy);
        Assert.AreEqual("Progress 50/100", progress);
    }
 
    [Test]
    public void Start() {
        var restrictionApplied = false;
        ((TestRestriction) _classUnderTest.Restrictions[0]).OnApply = _ => restrictionApplied = true;
        
        var goalStarted = false;
        _classUnderTest.Goal.OnStart = _ => goalStarted = true;
        
        _classUnderTest.Start(Difficulty.Easy);

        Assert.IsTrue(restrictionApplied, "Restriction should have been applied!");
        Assert.IsTrue(goalStarted, "Goal should have been started!");
    }
    
    [Test]
    public void Stop() {
        var restrictionRemoved = false;
        ((TestRestriction) _classUnderTest.Restrictions[0]).OnRemove = _ => restrictionRemoved = true;
        
        var goalStopped = false;
        _classUnderTest.Goal.OnStop = _ => goalStopped = true;
        
        _classUnderTest.Start(Difficulty.Easy);
        _classUnderTest.Stop();

        Assert.IsTrue(restrictionRemoved, "Restriction should have been removed!");
        Assert.IsTrue(goalStopped, "Goal should have been stopped!");
    }
}