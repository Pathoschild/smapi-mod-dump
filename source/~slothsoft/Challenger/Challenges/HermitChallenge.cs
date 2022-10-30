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
using System.Collections.Generic;
using Slothsoft.Challenger.Api;
using Slothsoft.Challenger.Goals;
using Slothsoft.Challenger.Models;
using Slothsoft.Challenger.Restrictions;

namespace Slothsoft.Challenger.Challenges;

public class HermitChallenge : BaseChallenge {
    public HermitChallenge(IModHelper modHelper) : base(modHelper, "hermit") {
    }

    protected override IRestriction[] CreateRestrictions(IModHelper modHelper, Difficulty difficulty) {
        return new IRestriction[] {
            new PreventWarping(modHelper.Translation.Get("HermitChallenge.CanOnlyLeaveOnSunday"), new Dictionary<PreventWarping.WarpDirection, Func<bool>>{
                { new PreventWarping.WarpDirection(LocationName.Farm, LocationName.Backwoods), IsNotSunday},
                { new PreventWarping.WarpDirection(LocationName.Farm, LocationName.Forest), IsNotSunday},         
                { new PreventWarping.WarpDirection(LocationName.Farm, LocationName.BusStop), IsNotSunday},
            }),
        };
    }

    private static bool IsNotSunday() {
        return Game1.dayOfMonth % 7 != 0;
    }
    
    protected override IGoal CreateGoal(IModHelper modHelper) {
        return new CommunityCenterOrPerfectionGoal(modHelper);
    }
    
    public override MagicalReplacement GetMagicalReplacement(Difficulty difficulty) {
        return difficulty == Difficulty.Easy ? MagicalReplacement.SeedMaker : MagicalReplacement.Default;
    }
}