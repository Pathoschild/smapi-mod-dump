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
using Microsoft.Xna.Framework;
using StardewValley.BellsAndWhistles;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;

namespace StopRugRemoval.HarmonyPatches;

/// <summary>
/// Class to hold patches for the Furniture class, to allow me to place rugs under other furniture
/// And to prevent me from removing rugs when I'm not supposed to....
/// </summary>
[HarmonyPatch(typeof(Furniture))]
internal class FurniturePatches
{
    private static int ticks;

    [SuppressMessage("StyleCop", "SA1313", Justification = "Style prefered by Harmony")]
    [HarmonyPostfix]
    [HarmonyPatch(nameof(Furniture.canBeRemoved))]
    private static void PostfixCanBeRemoved(Furniture __instance, ref Farmer __0, ref bool __result)
    {
        try
        {
            if (!ModEntry.Config.Enabled || !ModEntry.Config.PreventRugRemoval
                || !__result
                || !__instance.furniture_type.Value.Equals(Furniture.rug)
                || __0.currentLocation is not GameLocation currentLocation)
            {
                return;
            }

            Rectangle bounds = __instance.boundingBox.Value;
            int tileX = bounds.X / 64;
            int tileY = bounds.Y / 64;
            ModEntry.ModMonitor.DebugOnlyLog($"Checking rug: {bounds.X / 64f}, {bounds.Y / 64f}, W/H {bounds.Width / 64f}/{bounds.Height / 64f}", LogLevel.Debug);

            for (int x = 0; x < bounds.Width / 64; x++)
            {
                for (int y = 0; y < bounds.Height / 64; y++)
                {
                    if (!currentLocation.isTileLocationTotallyClearAndPlaceable(x + tileX, y + tileY))
                    {
                        Game1.showRedMessage(I18n.RugRemovalMessage());
                        __result = false;
                        return;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            ModEntry.ModMonitor.Log($"Ran into issues with postfix for Furniture::CanBeRemoved for {__instance.Name}\n\n{ex}", LogLevel.Error);
        }
    }

#if DEBUG
    [HarmonyPrefix]
    [HarmonyPatch(nameof(Furniture.canBePlacedHere))]
    [SuppressMessage("StyleCop", "SA1313", Justification = "Style prefered by Harmony")]
    private static bool PrefixCanBePlacedHere(Furniture __instance, GameLocation l, Vector2 tile, ref bool __result)
    {
        try
        {
            if (!ModEntry.Config.Enabled || !ModEntry.Config.CanPlaceRugsUnder
                || !__instance.furniture_type.Value.Equals(Furniture.rug)
                || __instance.placementRestriction != 0 // someone requested a custom placement restriction, respect that.
                || l.CanPlaceThisFurnitureHere(__instance)
                /*|| __instance.GetAdditionalFurniturePlacementStatus(l, (int)tile.X * 64, (int)tile.Y * 64) != 0*/)
            {
                return true;
            }
            Rectangle bounds = __instance.boundingBox.Value;
            int tileX = (int)tile.X;
            int tileY = (int)tile.Y;
            for (int x = 0; x < bounds.Width / 64; x++)
            {
                for (int y = 0; y < bounds.Height / 64; y++)
                {
                    Vector2 currentTile = new(tileX + x, tileY + y);
                    if ((l.terrainFeatures.TryGetValue(currentTile, out TerrainFeature possibletree) && possibletree is Tree)
                        || l.isTerrainFeatureAt(tileX + x, tileY + y))
                    {
                        __result = false;
                        return false;
                    }
                }
            }
            __result = true;
            return false;
        }
        catch (Exception ex)
        {
            ModEntry.ModMonitor.Log($"Ran into errors in PrefixCanBePlacedHere for {__instance.Name} at {l.NameOrUniqueName} ({tile.X}, {tile.Y})\n\n{ex}", LogLevel.Error);
        }
        return true;
    }
#endif

#warning - remember DGA?

    /// <summary>
    /// Prefix to prevent objects from accidentally being removed from tables.
    /// </summary>
    /// <param name="__instance">The table.</param>
    /// <param name="who">The farmer who clicked.</param>
    /// <param name="__result">The result of the original function.</param>
    /// <returns>Return true to continue to the original function, false otherwise.</returns>
    [HarmonyPrefix]
    [HarmonyPriority(Priority.First)]
    [HarmonyPatch(nameof(Furniture.clicked))]
    [SuppressMessage("StyleCop", "SA1313", Justification = "Style prefered by Harmony")]
    private static bool PrefixClicked(Furniture __instance, Farmer who, ref bool __result)
    {
        try
        {
            if (!ModEntry.Config.Enabled)
            {
                return true;
            }
            if ((__instance.furniture_type.Value == Furniture.table
                    || __instance.furniture_type.Value == Furniture.longTable
                    || __instance is StorageFurniture)
                && ModEntry.Config.PreventRemovalFromTable
                && !ModEntry.Config.FurniturePlacementKey.IsDown())
            {
                if (Game1.ticks > ticks + 60)
                {
                    Game1.showRedMessage(I18n.TableRemovalMessage(keybind: ModEntry.Config.FurniturePlacementKey));
                    ticks = Game1.ticks;
                }
                __result = false;
                return false;
            }
            else if (__instance.ParentSheetIndex == 1971 && who.currentLocation is GameLocation loc)
            {
                // clicked on a butterfly hutch!
                Vector2 v = new(Game1.random.Next(-2, 4), Game1.random.Next(-1, 1));
                loc.instantiateCrittersList();
                loc.addCritter(new Butterfly(__instance.TileLocation + v).setStayInbounds(stayInbounds: true));
            }
            return true;
        }
        catch (Exception ex)
        {
            ModEntry.ModMonitor.Log($"Ran into errors preventing removal of item from table for {who.Name}\n\n{ex}", LogLevel.Error);
            return true;
        }
    }

    [HarmonyPrefix]
    [HarmonyPatch(nameof(Furniture.DoesTileHaveProperty))]
    [SuppressMessage("StyleCop", "SA1313", Justification = "Style prefered by Harmony")]
    private static bool PrefixDoesTileHaveProperty(Furniture __instance, int tile_x, int tile_y, string property_name, string layer_name, ref string property_value, ref bool __result)
    {
        try
        {
            if (__instance.getBoundingBox(__instance.TileLocation).Contains((tile_x * 64) + 32, (tile_y * 64) + 32)
                && layer_name.Equals("Back", StringComparison.OrdinalIgnoreCase)
                && property_name.Equals("NoSpawn", StringComparison.OrdinalIgnoreCase))
            {
                property_value = "All";
                __result = true;
                return false;
            }
        }
        catch (Exception ex)
        {
            ModEntry.ModMonitor.Log($"Failed in preventing spawning on rugs at {tile_x} {tile_y}\n\n{ex}", LogLevel.Error);
        }
        return true;
    }
}