/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/smapi-mods
**
*************************************************/

namespace DaLion.Stardew.Arsenal.Framework.Patches;

#region using directives

using HarmonyLib;
using JetBrains.Annotations;
using StardewValley;
using StardewValley.Locations;

#endregion using directives

[UsedImplicitly]
internal sealed class MineShaftCheckForQiChallengeCompletionPatch : Common.Harmony.HarmonyPatch
{
    /// <summary>Construct an instance.</summary>
    internal MineShaftCheckForQiChallengeCompletionPatch()
    {
        Target = RequireMethod<MineShaft>(nameof(MineShaft.CheckForQiChallengeCompletion));
    }

    #region harmony patches

    /// <summary>Add custom quest.</summary>
    [HarmonyPrefix]
    private static bool MineShaftCheckForQiChallengeCompletionPrefix()
    {
        if (!ModEntry.Config.TrulyLegendaryGalaxySword) return true; // run original logic

        if (Game1.player.deepestMineLevel >= 145 && Game1.player.hasQuest(20) &&
            !Game1.player.hasOrWillReceiveMail("QiChallengeFirst"))
        {
            Game1.player.completeQuest(20);
            Game1.addMailForTomorrow("QiChallengeFirst");
        }
        else if (Game1.player.deepestMineLevel >= 170 && Game1.player.hasQuest(ModEntry.QiChallengeFinalQuestId) &&
                 !Game1.player.hasOrWillReceiveMail("QiChallengeComplete"))
        {
            Game1.player.completeQuest(ModEntry.QiChallengeFinalQuestId);
            Game1.addMailForTomorrow("QiChallengeComplete");
        }

        return false; // don't run original logic
    }

    #endregion harmony patches
}