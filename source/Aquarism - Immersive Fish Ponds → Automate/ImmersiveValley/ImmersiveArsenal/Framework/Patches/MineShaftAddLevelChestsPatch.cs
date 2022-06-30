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
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Objects;

#endregion using directives

[UsedImplicitly]
internal sealed class MineShaftAddLevelChestsPatch : Common.Harmony.HarmonyPatch
{
    /// <summary>Construct an instance.</summary>
    internal MineShaftAddLevelChestsPatch()
    {
        Target = RequireMethod<MineShaft>("addLevelChests");
    }

    #region harmony patches

    /// <summary>Add custom Qi Challenge reward.</summary>
    [HarmonyPostfix]
    private static void MineShaftAddLevelChestsPostfix(MineShaft __instance)
    {
        if (__instance.mineLevel != 170 || Game1.player.hasOrWillReceiveMail("QiChallengeComplete") ||
            !Game1.player.hasQuest(ModEntry.QiChallengeFinalQuestId)) return;

        var chestSpot = new Vector2(9f, 9f);
        __instance.overlayObjects[chestSpot] =
            new Chest(0, new() { new StardewValley.Object(Constants.GALAXY_SOUL_INDEX_I, 1) }, chestSpot)
            {
                Tint = Color.White
            };
    }

    #endregion harmony patches
}