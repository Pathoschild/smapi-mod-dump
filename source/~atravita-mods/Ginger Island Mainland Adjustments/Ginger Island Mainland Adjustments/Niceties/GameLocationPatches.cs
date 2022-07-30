/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley.Locations;

namespace GingerIslandMainlandAdjustments.Niceties;

/// <summary>
/// Holds patches against GameLocation to prevent trampling of objects on IslandWest.
/// </summary>
[HarmonyPatch(typeof(GameLocation))]
internal class GameLocationPatches
{
    /// <summary>
    /// Prefix to prevent trampling.
    /// </summary>
    /// <param name="__instance">Gamelocation.</param>
    /// <returns>True to continue to original function, false to skip original function.</returns>
    [HarmonyPrefix]
    [HarmonyPatch(nameof(GameLocation.characterTrampleTile), new Type[] { typeof(Vector2) })]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1313:Parameter names should begin with lower-case letter", Justification = "Harmony convention")]
    private static bool PrefixCharacterTrample(GameLocation __instance)
    {
        try
        {
            if (__instance is IslandWest)
            {
                return false;
            }
        }
        catch (Exception ex)
        {
            Globals.ModMonitor.Log($"Crashed while trying to prevent trampling at {__instance.NameOrUniqueName},\n\n{ex}", LogLevel.Error);
        }
        return true;
    }

    /// <summary>
    /// Prefix to prevent characters from destroying things.
    /// </summary>
    /// <param name="__instance">GameLocation.</param>
    /// <returns>True to continue to original function, false to skip original function.</returns>
    [HarmonyPrefix]
    [HarmonyPatch(nameof(GameLocation.characterDestroyObjectWithinRectangle), new Type[] { typeof(Rectangle), typeof(bool) })]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1313:Parameter names should begin with lower-case letter", Justification = "Harmony convention")]
    private static bool PrefixCharacterDestroy(GameLocation __instance)
    {
        try
        {
            if (__instance is IslandWest)
            {
                return false;
            }
        }
        catch (Exception ex)
        {
            Globals.ModMonitor.Log($"Crashed while trying to prevent trampling at {__instance.NameOrUniqueName},\n\n{ex}", LogLevel.Error);
        }
        return true;
    }
}