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
using Microsoft.Xna.Framework;
using StardewValley.Locations;
using StardewValley.Objects;

namespace StopRugRemoval;

/// <summary>
/// Class to hold patches for the Furniture class, to allow me to place rugs under other furniture
/// And to prevent me from removing rugs when I'm not supposed to....
/// </summary>
[HarmonyPatch(typeof(Furniture))]
internal class FurniturePatches
{
    [SuppressMessage("StyleCop", "SA1313", Justification = "Style prefered by Harmony")]
    [HarmonyPostfix]
    [HarmonyPatch(nameof(Furniture.canBeRemoved))]
    private static void PostfixCanBeRemoved(Furniture __instance, ref Farmer __0, ref bool __result)
    {
        try
        {
            if (!ModEntry.Config.Enabled)
            { // mod disabled
                return;
            }
            if (!__result)
            { // can't be removed already
                return;
            }
            if (!__instance.furniture_type.Value.Equals(Furniture.rug))
            { // only want to deal with rugs
                return;
            }
            GameLocation currentLocation = __0.currentLocation; // get location of farmer
            if (currentLocation is null)
            {
                return;
            }

            Rectangle bounds = __instance.boundingBox.Value;
            ModEntry.ModMonitor.Log($"Checking rug: {bounds.X / 64f}, {bounds.Y / 64f}, W/H {bounds.Width / 64f}/{bounds.Height / 64f}");

            for (int x = 0; x < bounds.Width / 64; x++)
            {
                for (int y = 0; y < bounds.Height / 64; y++)
                {
                    if (!currentLocation.isTileLocationTotallyClearAndPlaceable(x + (bounds.X / 64), y + (bounds.Y / 64)))
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

    [SuppressMessage("StyleCop", "SA1313", Justification = "Style prefered by Harmony")]
    private static bool PrefixCanBePlacedHere(Furniture __instance, GameLocation __0, Vector2 tile, ref bool __result)
    {
        try
        {
            if (!ModEntry.Config.Enabled || !ModEntry.Config.CanPlaceRugsUnder)
            {
                return true;
            }
            if (__instance.furniture_type.Value.Equals(Furniture.rug))
            {
                return true;
            }
            if (__instance.placementRestriction != 0)
            { // someone requested a custom placement restriction, respect that.
                return true;
            }
            Rectangle bounds = __instance.boundingBox.Value;
            bool okaytoplace = true;
            for (int x = 0; x < bounds.Width / 64; x++)
            {
                for (int y = 0; y < bounds.Height / 64; y++)
                {
                    // check for large terrain+terrain, refuse placement.
                    // check for is placeable everywhere, and if the thing that's blocking placement is an
                    // another furniture item, I'm still okay to place.
                }
            }
        }
        catch (Exception ex)
        {
            ModEntry.ModMonitor.Log($"Ran into errors in PrefixCanBePlacedHere for {__instance.Name} at {__0.NameOrUniqueName} ({tile.X}, {tile.Y})\n\n{ex}", LogLevel.Error);
        }
        return true;
    }
}

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
            if (!ModEntry.Config.Enabled || !ModEntry.Config.CanPlaceRugsOutside)
            { // mod disabled
                return;
            }
            if (__instance is MineShaft || __instance is VolcanoDungeon)
            { // do not want to affect mines
                return;
            }
            if (__result)
            { // can already be placed
                return;
            }
            if (__0.placementRestriction != 0)
            { // someone requested a custom placement restriction, respect that.
                return;
            }
            if (__0.furniture_type.Value.Equals(Furniture.rug))
            {// Let me place rug
                __result = true;
                return;
            }
        }
        catch (Exception ex)
        {
            ModEntry.ModMonitor.Log($"Failed in attempting to place rug outside in PostfixCanPlaceFurnitureHere.\n{ex}", LogLevel.Error);
        }
    }
}