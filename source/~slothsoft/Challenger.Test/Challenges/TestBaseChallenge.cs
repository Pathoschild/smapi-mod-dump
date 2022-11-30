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
using ChallengerTest.Goals;
using ChallengerTest.Restrictions;
using Slothsoft.Challenger.Api;
using Slothsoft.Challenger.Challenges;
using Slothsoft.Challenger.Models;

namespace ChallengerTest.Challenges;

public class TestBaseChallenge : BaseChallenge {

    public string DisplayName { get; init; } = "Test Base Challenge";
    public string DisplayText { get; init; } = "Test Base Challenge Description";
    public IRestriction[] Restrictions { get; init; } = { new TestRestriction() };
    public TestBaseGoal Goal { get; init; } = new();
    protected override ChallengeInfo ChallengeInfo { get; } = new();

    public TestBaseChallenge(IModHelper modHelper, string id = "test-base-challenge") : base(modHelper, id) {
    }
    
    public override string GetDisplayText(Difficulty difficulty) {
        return DisplayText;
    }

    protected override IRestriction[] CreateRestrictions(IModHelper modHelper, Difficulty difficulty) {
        return Restrictions;
    }
    
    protected override IGoal CreateGoal(IModHelper modHelper) {
        return Goal;
    }
}