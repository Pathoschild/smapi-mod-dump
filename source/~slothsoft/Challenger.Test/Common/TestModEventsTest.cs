/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/slothsoft/stardew-challenger
**
*************************************************/

using NUnit.Framework;

namespace ChallengerTest.Common; 

public class TestModEventsTest {
    
    private TestModEvents _classUnderTest = new();

    [SetUp]
    public void SetUp() {
        _classUnderTest = new();
    }

    [Test]
    public void Content() {
        Assert.NotNull(_classUnderTest.Content);
    }
    
    [Test]
    public void Display() {
        Assert.NotNull(_classUnderTest.Display);
    }
    
    [Test]
    public void GameLoop() {
        Assert.NotNull(_classUnderTest.GameLoop);
    }
    
    [Test]
    public void Input() {
        Assert.NotNull(_classUnderTest.Input);
    }
    
    [Test]
    public void Multiplayer() {
        Assert.NotNull(_classUnderTest.Multiplayer);
    }
    
    [Test]
    public void Player() {
        Assert.NotNull(_classUnderTest.Player);
    }
    
    [Test]
    public void World() {
        Assert.NotNull(_classUnderTest.World);
    }
    
    [Test]
    public void Specialized() {
        Assert.NotNull(_classUnderTest.Specialized);
    }
}