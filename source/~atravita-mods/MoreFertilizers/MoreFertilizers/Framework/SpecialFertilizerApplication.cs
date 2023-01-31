/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

using AtraCore.Utilities;
using AtraShared.Utils;
using AtraShared.Utils.Extensions;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewModdingAPI.Events;
using StardewValley.Buildings;
using StardewValley.Locations;

namespace MoreFertilizers.Framework;

/// <summary>
/// Handles applying special fertilizers.
/// </summary>
[HarmonyPatch(typeof(Utility))]
internal static class SpecialFertilizerApplication
{
    private const int PLACEMENTRADIUS = 2;

    private static readonly CanPlaceHandler PlaceHandler = new();

    /// <summary>
    /// Handles applying a fertilizer from an input button press.
    /// </summary>
    /// <param name="e">Button press event arguments.</param>
    /// <param name="helper">SMAPI's input helper.</param>
    internal static void ApplyFertilizer(ButtonPressedEventArgs e, IInputHelper helper)
    {
        if (Game1.player.ActiveObject?.bigCraftable?.Value != false || Game1.player.ActiveObject.GetType() != typeof(SObject))
        {
            return;
        }

        SObject obj = Game1.player.ActiveObject;

        Vector2 placementtile = Utility.withinRadiusOfPlayer(((int)e.Cursor.Tile.X * 64) + 32, ((int)e.Cursor.Tile.Y * 64) + 32, PLACEMENTRADIUS, Game1.player)
                                    ? e.Cursor.Tile
                                    : e.Cursor.GrabTile;

        // HACK move the tile further from the player if they're controller.
        if ((obj.ParentSheetIndex == ModEntry.FishFoodID || obj.ParentSheetIndex == ModEntry.DeluxeFishFoodID) &&
            !Game1.currentLocation.isWaterTile((int)placementtile.X, (int)placementtile.Y))
        {
            placementtile = Game1.player.FacingDirection switch
            {
                Game1.up => Game1.player.getTileLocation() - new Vector2(0, 3),
                Game1.down => Game1.player.getTileLocation() + new Vector2(0, 3),
                Game1.left => Game1.player.getTileLocation() - new Vector2(3, 0),
                _ => Game1.player.getTileLocation() + new Vector2(3, 0)
            };
        }

        ModEntry.ModMonitor.DebugOnlyLog($"Checking tile {placementtile}");

        if (!PlaceHandler.CanPlaceFertilizer(obj, Game1.currentLocation, placementtile, true))
        {
            return;
        }

        // Handle the graphics for the special case of tossing the fish food fertilizer.
        if (obj.ParentSheetIndex == ModEntry.FishFoodID || obj.ParentSheetIndex == ModEntry.DeluxeFishFoodID || obj.ParentSheetIndex == ModEntry.DomesticatedFishFoodID)
        {
            Vector2 placementpixel = (placementtile * 64f) + new Vector2(32f, 32f);
            if (obj.ParentSheetIndex == ModEntry.DomesticatedFishFoodID && Game1.currentLocation is BuildableGameLocation loc)
            {
                foreach (Building b in loc.buildings)
                {
                    if (b is FishPond fishPond && b.occupiesTile(placementtile))
                    {
                        placementpixel = fishPond.GetCenterTile() * 64f;
                        break;
                    }
                }
            }
            Game1.player.FaceFarmerTowardsPosition(placementpixel);
            Game1.playSound("throwDownITem");

            Multiplayer? mp = MultiplayerHelpers.GetMultiplayer();
            float time = obj.ParabolicThrowItem(Game1.player.Position - new Vector2(0, 128), placementpixel, mp, Game1.currentLocation);

            GameLocationUtils.DrawWaterSplash(Game1.currentLocation, placementpixel, mp, (int)time);

            DelayedAction.playSoundAfterDelay("waterSlosh", (int)time, Game1.player.currentLocation);
            if (obj.ParentSheetIndex != ModEntry.DomesticatedFishFoodID)
            {
                DelayedAction.functionAfterDelay(
                    static () => Game1.currentLocation.waterColor.Value = ModEntry.Config.WaterOverlayColor,
                    (int)time);
            }
        }

        // The actual placement.
        if (PlaceHandler.TryPlaceFertilizer(obj, Game1.currentLocation, placementtile))
        {
            Game1.player.reduceActiveItemByOne();
            helper.Suppress(e.Button);
            return;
        }
    }

    [HarmonyPrefix]
    [HarmonyPriority(Priority.VeryHigh)]
    [HarmonyPatch(nameof(Utility.playerCanPlaceItemHere))]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1313:Parameter names should begin with lower-case letter", Justification = "Harmony Convention")]
    private static bool PrefixPlayerCanPlaceItemHere(GameLocation location, Item item, int x, int y, Farmer f, ref bool __result)
    {
        if (item.GetType() != typeof(SObject))
        {
            return true;
        }

        try
        {
            Vector2 tile = new(x / 64, y / 64);
            if (item is SObject obj && PlaceHandler.CanPlaceFertilizer(obj, location, tile) &&
                Utility.withinRadiusOfPlayer(x, y, PLACEMENTRADIUS, f))
            {
                __result = true;
                return false;
            }
            else if (item is SObject fert && !fert.bigCraftable.Value && fert.Category == SObject.fertilizerCategory
                && ModEntry.SpecialFertilizerIDs.Contains(fert.ParentSheetIndex))
            {
                __result = false;
                return false;
            }
        }
        catch (Exception ex)
        {
            ModEntry.ModMonitor.Log($"Attempt to prefix Utility.playerCanPlaceItemHere has failed:\n\n{ex}", LogLevel.Error);
        }
        return true;
    }
}