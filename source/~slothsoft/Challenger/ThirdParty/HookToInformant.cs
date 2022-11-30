/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/slothsoft/stardew-challenger
**
*************************************************/

using Microsoft.Xna.Framework.Graphics;
using Slothsoft.Challenger.Challenges;
using Slothsoft.Challenger.Goals;

namespace Slothsoft.Challenger.ThirdParty; 

internal static class HookToInformant {
    
    private static Texture2D? _bundle;
    
    public static void Apply(ChallengerMod challengerMod) {
        // get Generic Mod Config Menu's API (if it's installed)
        var helper = challengerMod.Helper;
        var informant = helper.ModRegistry.GetApi<IInformant>("Slothsoft.Informant");
        if (informant is null)
            return;

        _bundle ??= helper.ModContent.Load<Texture2D>("assets/challenge_decorator.png");
        informant.AddItemDecorator(
            "challenger-decorator",
            helper.Translation.Get("HookToInformant.DisplayName"),
            helper.Translation.Get("HookToInformant.Description"),
            FetchDecoratorIfNecessary
        );
    }

    static Texture2D? FetchDecoratorIfNecessary(Item item) {
        // figure out if the item is used in the challenge
        var api = ChallengerMod.Instance.GetApi();
        if (api == null) {
            return null;
        }
        
        // only one goal currently has items that need to be decorated
        var goal = (api.ActiveChallenge as BaseChallenge)?.GetGoal() as EarnMoneyGoal;
        if (goal == null) {
            return null;
        }
        return goal.IsCountingAllowed(item) ? _bundle : null;
    }
}