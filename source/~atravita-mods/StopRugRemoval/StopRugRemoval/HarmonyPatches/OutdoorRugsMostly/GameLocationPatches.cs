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
using StardewValley.Objects;

namespace StopRugRemoval.HarmonyPatches.OutdoorRugsMostly;

/// <summary>
/// Patches on GameLocation to allow me to place rugs anywhere.
/// </summary>
[HarmonyPatch(typeof(GameLocation))]
internal class GameLocationPatches
{
    [HarmonyPostfix]
    [HarmonyPatch(nameof(GameLocation.CanPlaceThisFurnitureHere))]
    [SuppressMessage("StyleCop", "SA1313", Justification = "Style prefered by Harmony")]
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

    [HarmonyPrefix]
    [HarmonyPatch(nameof(GameLocation.makeHoeDirt))]
    [SuppressMessage("StyleCop", "SA1313", Justification = "Style prefered by Harmony")]
    private static bool PrefixMakeHoeDirt(GameLocation __instance, Vector2 tileLocation, bool ignoreChecks = false)
    {
        try
        {
            if (ignoreChecks || !ModEntry.Config.PreventPlantingOnRugs)
            {
                return true;
            }

            int posX = ((int)tileLocation.X * 64) + 32;
            int posY = ((int)tileLocation.Y * 64) + 32;
            foreach (Furniture f in __instance.furniture)
            {
                if (f.furniture_type.Value == Furniture.rug && f.getBoundingBox(f.TileLocation).Contains(posX, posY))
                {
                    return false;
                }
            }
        }
        catch (Exception ex)
        {
            ModEntry.ModMonitor.Log($"Encountered error trying to prevent hoeing on rugs.\n\n{ex}", LogLevel.Error);
        }
        return true;
    }

    [HarmonyPrefix]
    [HarmonyPatch(nameof(GameLocation.doesTileHavePropertyNoNull))]
    [SuppressMessage("StyleCop", "SA1313", Justification = "Style prefered by Harmony")]
    private static bool PrefixDoesTileHavePropertyNoNull(GameLocation __instance, int xTile, int yTile, string propertyName, string layerName, ref string __result)
    {
        try
        {
            if (propertyName.Equals("NoSpawn", StringComparison.OrdinalIgnoreCase) && layerName.Equals("Back", StringComparison.OrdinalIgnoreCase))
            {
                foreach (Furniture f in __instance.furniture)
                {
                    if (f.furniture_type.Value == Furniture.rug && f.getBoundingBox(f.TileLocation).Contains((xTile * 64) + 32, (yTile * 64) + 32))
                    {
                        __result = "All";
                        return false;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            ModEntry.ModMonitor.Log($"Error encountered trying to prevent grass growth\n\n{ex}", LogLevel.Error);
        }
        return true;
    }
}
