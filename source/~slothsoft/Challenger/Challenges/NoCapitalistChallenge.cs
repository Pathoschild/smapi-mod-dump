/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/slothsoft/stardew-challenger
**
*************************************************/

using Slothsoft.Challenger.Api;
using Slothsoft.Challenger.Goals;
using Slothsoft.Challenger.Common;
using Slothsoft.Challenger.Restrictions;
using StardewModdingAPI.Events;

namespace Slothsoft.Challenger.Challenges;

public class NoCapitalistChallenge : BaseChallenge {
    public NoCapitalistChallenge(IModHelper modHelper) : base(modHelper, "no-capitalist") {
    }

    protected override IRestriction[] CreateRestrictions(IModHelper modHelper, Difficulty difficulty) {
        if (difficulty == Difficulty.Hard) {
            return new IRestriction[] {
                new CannotBuyFromShop(modHelper, ShopIds.Pierre, ShopIds.Clint, ShopIds.JoJo),
            };
        }
        return new IRestriction[] {
            new CannotBuyFromShop(modHelper, ShopIds.Pierre, ShopIds.JoJo),
        };
    }

    public override MagicalReplacement GetMagicalReplacement(Difficulty difficulty) {
        return difficulty == Difficulty.Easy ? MagicalReplacement.SeedMaker : MagicalReplacement.Default;
    }

    protected override IGoal CreateGoal(IModHelper modHelper) {
        return new CommunityCenterGoal(modHelper);
    }
    
    public override void Start(Difficulty difficulty) {
        base.Start(difficulty);
        ModHelper.Events.GameLoop.TimeChanged += OnTimeChanged;
    }

    private void OnTimeChanged(object? sender, TimeChangedEventArgs e) {
        // this challenge has no clear events that change for it to be completed, so update periodically
        ProgressChangedInvoked();
    }

    public override void Stop() {
        ModHelper.Events.GameLoop.TimeChanged -= OnTimeChanged;
        base.Stop();
    }
}