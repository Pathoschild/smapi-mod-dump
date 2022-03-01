/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StopRugRemoval
**
*************************************************/

using HarmonyLib;
using StardewValley.Locations;
using StardewValley.Objects;

namespace StopRugRemoval;

/// <summary>
/// Handles applying and removing NoSpawn from each rug's tile.
/// </summary>
internal class OutdoorRugs
{

}

#if DEBUG // not yet finished implementing....
/// <summary>
/// Patches on GameLocation to allow me to place rugs anywhere.
/// </summary>
[HarmonyPatch(typeof(GameLocation))]
internal class GameLocationPatches
{
    [SuppressMessage("StyleCop", "SA1313", Justification = "Style prefered by Harmony")]
    [HarmonyPostfix]
    [HarmonyPatch(nameof(GameLocation.CanPlaceThisFurnitureHere))]
    private static void PostfixCanPlaceFurnitureHere(GameLocation __instance, Furniture __0, ref bool __result)
    {
        try
        {
            if (__result // can already be placed
                || __0.placementRestriction != 0 // someone requested a custom placement restriction, respect that.
                || !ModEntry.Config.Enabled || !ModEntry.Config.CanPlaceRugsOutside // mod disabled
                || __instance is MineShaft || __instance is VolcanoDungeon // do not want to affect mines
                || !__0.furniture_type.Value.Equals(Furniture.rug) // only want to affect rugs
                )
            {
                return;
            }
            __result = true;
        }
        catch (Exception ex)
        {
            ModEntry.ModMonitor.Log($"Failed in attempting to place rug outside in PostfixCanPlaceFurnitureHere.\n{ex}", LogLevel.Error);
        }
    }
}

#endif