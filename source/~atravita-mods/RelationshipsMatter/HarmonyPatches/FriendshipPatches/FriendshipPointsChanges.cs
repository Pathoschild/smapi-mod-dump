/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

using AtraBase.Toolkit.Extensions;

using HarmonyLib;

namespace RelationshipsMatter.HarmonyPatches.FriendshipPatches;

/// <summary>
/// Harmony patches to affect point changes for friendship.
/// </summary>
[HarmonyPatch(typeof(Farmer))]
internal static class FriendshipPointsChanges
{
    [HarmonyPrefix]
    [HarmonyPatch(nameof(Farmer.changeFriendship))]
    private static void PrefixChangeFriendship(ref int amount)
    {
        if (amount > 0)
        {
            amount = (amount * ModEntry.Config.FriendshipGainFactor).RandomRoundProportional();
        }
        else
        {
            amount = (amount * ModEntry.Config.FriendshipLossFactor).RandomRoundProportional();
        }
    }
}
