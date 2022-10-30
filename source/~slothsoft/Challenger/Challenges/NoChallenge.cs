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
using Slothsoft.Challenger.Api;
using Slothsoft.Challenger.Goals;

namespace Slothsoft.Challenger.Challenges;

public class NoChallenge : BaseChallenge {
    public const string ChallengeId = "none";

    public NoChallenge(IModHelper modHelper) : base(modHelper, ChallengeId) {
    }
    
    public override string GetDisplayText(Difficulty difficulty) {
        return ModHelper.Translation.Get("NoChallenge.DisplayText");
    }

    protected override IRestriction[] CreateRestrictions(IModHelper modHelper, Difficulty difficulty) {
        return Array.Empty<IRestriction>();
    }
    
    protected override IGoal CreateGoal(IModHelper modHelper) {
        return new NoGoal(modHelper);
    }
}