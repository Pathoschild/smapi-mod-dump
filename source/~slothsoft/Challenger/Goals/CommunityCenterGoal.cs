/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/slothsoft/stardew-challenger
**
*************************************************/

using System.Linq;
using Slothsoft.Challenger.Api;
using Slothsoft.Challenger.Challenges;

namespace Slothsoft.Challenger.Goals;

public class CommunityCenterGoal : IGoal {

    private readonly IModHelper _modHelper;
    
    public CommunityCenterGoal(IModHelper modHelper) {
        _modHelper = modHelper;
    }

    public string GetDisplayName(Difficulty difficulty) {
        return _modHelper.Translation.Get(GetType().Name);
    }
    
    public bool IsCompleted(Difficulty difficulty) {
        var (finished, all) = GetProgressAsInts();
        return finished >= all;
    }
    
    public void Start() {
        // we don't need to start this
    }

    public void Stop() {
        // we don't need to stop this
    }

    public bool WasStarted() {
        return true;
    }

    public string GetProgress(Difficulty difficulty) {
        var (finished, all) = GetProgressAsInts();
        return $"{(100 * finished / all):0} / 100%";
    }
    
    private (int, int) GetProgressAsInts() {
        int finished, all;
        
        if (Game1.player.hasCompletedCommunityCenter()) {
            // For some reason this shows as 98/100
            finished = 1;
            all = 1;
        } else if (Game1.player.hasOrWillReceiveMail("JojaMember")) {
            // Player is contributing to JojaMart
            var mailsReceived = GetMailsForCommunityCenter();
            finished = mailsReceived.Count(b => b);
            all = mailsReceived.Length;
        } else {
            // The "normal" community center
            finished = Game1.netWorldState.Value.Bundles.Values.SelectMany(b => b).Count(v => v);
            all = Game1.netWorldState.Value.Bundles.Values.SelectMany(b => b).Count();
        }
        return (finished, all);
    }

    private static bool[] GetMailsForCommunityCenter() {
        // See Game1.player.hasCompletedCommunityCenter()
        return new[] {
            Game1.player.mailReceived.Contains("ccBoilerRoom"),
            Game1.player.mailReceived.Contains("ccCraftsRoom"),
            Game1.player.mailReceived.Contains("ccPantry"),
            Game1.player.mailReceived.Contains("ccFishTank"),
            Game1.player.mailReceived.Contains("ccVault"),
        };
    }
}