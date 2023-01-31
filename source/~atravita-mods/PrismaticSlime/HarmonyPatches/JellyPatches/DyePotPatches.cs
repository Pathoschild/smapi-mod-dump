/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

using AtraShared.Utils.Extensions;

using HarmonyLib;

using StardewValley.Menus;

// TODO - color in the little slots somehow and make them not interactable.

namespace PrismaticSlime.HarmonyPatches.JellyPatches;

/// <summary>
/// Patches against the dye pots.
/// </summary>
[HarmonyPatch(typeof(DyeMenu))]
internal static class DyePotPatches
{
    internal const string ModData = "atravita.PrismaticSlime.DyeMenu";

    [HarmonyPatch(nameof(DyeMenu.CanDye))]
    private static void Postfix(ref bool __result)
    {
        if (!__result)
        {
            if (Game1.player.modData.GetInt(ModData) is > 0)
            {
                __result = true;
            }
        }
    }
}
