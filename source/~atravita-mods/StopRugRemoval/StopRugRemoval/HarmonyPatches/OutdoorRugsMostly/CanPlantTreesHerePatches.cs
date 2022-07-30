/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

using System.Reflection;
using AtraBase.Toolkit.Reflection;
using HarmonyLib;
using StardewValley.Objects;

namespace StopRugRemoval.HarmonyPatches.OutdoorRugsMostly;

/// <summary>
/// I think this prevents the cursor from turning green when trying to place a tree on a rug.
/// </summary>
[HarmonyPatch]
internal static class CanPlantTreesHerePatches
{
    /// <summary>
    /// Defines the methods for which to patch.
    /// </summary>
    /// <returns>Methods to patch.</returns>
    [UsedImplicitly]
    internal static IEnumerable<MethodBase> TargetMethods()
    {
        foreach (Type type in typeof(GameLocation).GetAssignableTypes(publiconly: true, includeAbstract: false))
        {
            if (type.DeclaredInstanceMethodNamedOrNull(nameof(GameLocation.CanPlantTreesHere), new Type[] { typeof(int), typeof(int), typeof(int) }) is MethodBase method
                && method.DeclaringType == type)
            {
                yield return method;
            }
        }
    }

    /// <summary>
    /// Prefix to prevent planting trees on rugs.
    /// </summary>
    /// <param name="__instance">Game location.</param>
    /// <param name="tile_x">Tile X.</param>
    /// <param name="tile_y">Tile Y.</param>
    /// <param name="__result">Result to replace the original with.</param>
    /// <returns>True to continue to original, false to skip.</returns>
    [SuppressMessage("StyleCop", "SA1313", Justification = "Style prefered by Harmony")]
    internal static bool Prefix(GameLocation __instance, int tile_x, int tile_y, ref bool __result)
    {
        try
        {
            int xpos = (tile_x * 64) + 32;
            int ypos = (tile_y * 64) + 32;
            foreach (Furniture f in __instance.furniture)
            {
                if (f.getBoundingBox(f.TileLocation).Contains(xpos, ypos))
                {
                    __result = false;
                    return false;
                }
            }
        }
        catch (Exception ex)
        {
            ModEntry.ModMonitor.Log($"Encountered error in prefix on GameLocation.CanPlantTrees Here\n\n{ex}", LogLevel.Error);
        }
        return true;
    }
}