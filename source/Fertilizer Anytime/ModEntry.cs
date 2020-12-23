/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/stoobinator/Stardew-Valley-Fertilizer-Anytime
**
*************************************************/

using System;
using Harmony;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.TerrainFeatures;

/// <summary>
/// Mod to allow basic and quality fertilizer to be applied when crops are
/// already growing
/// </summary>
namespace FertilizerAnytime
{
    public class ModEntry : Mod
    {
        #region Constants
        /// <summary>
        /// Item index that corresponds to basic fertilizer
        /// </summary>
        private const int BasicFertilizer = 368;

        /// <summary>
        /// Item index that corresponds to quality fertilizer
        /// </summary>
        private const int QualityFertilizer = 369;

        /// <summary>
        /// Item index that corresponds to deluxe fertilizer
        /// </summary>
        private const int DeluxeFertilizer = 919;
        #endregion

        #region Fields
        /// <summary>
        /// The monitor for outputting messages
        /// </summary>
        private static new IMonitor Monitor;

        /// <summary>
        /// Whether or not the user is currently clicking
        /// </summary>
        private bool Clicking = false;
        #endregion

        #region Events and patch
        /// <summary>
        /// The mod entry point, called after the mod is first loaded
        /// </summary>
        /// <param name="helper">Provides simplified APIs for writing mods</param>
        public override void Entry(IModHelper helper)
        {
            Monitor = base.Monitor;

            // Change the logic for the cursor so it shows as green instead of red
            var harmony = HarmonyInstance.Create(ModManifest.UniqueID);
            harmony.Patch(
                original: AccessTools.Method(typeof(GameLocation), nameof(GameLocation.isTileOccupiedForPlacement)),
                postfix: new HarmonyMethod(typeof(ModEntry), nameof(ModEntry.isTileOccupiedForPlacement_Postfix))
            );

            helper.Events.Input.ButtonPressed += ButtonPressed;
            helper.Events.Input.ButtonReleased += ButtonReleased;
            helper.Events.GameLoop.UpdateTicking += UpdateTicking;
        }

        /// <summary>
        /// Shows the cursor as green when hovering over a tile that can be
        /// fertilized using this mod
        /// </summary>
        /// <param name="__instance"></param>
        /// <param name="tileLocation"></param>
        /// <param name="toPlace"></param>
        /// <param name="__result"></param>
        public static void isTileOccupiedForPlacement_Postfix(GameLocation __instance, Vector2 tileLocation, StardewValley.Object toPlace, ref bool __result)
        {
            try
            {
                // Replicated logic from the original method
                __instance.objects.TryGetValue(tileLocation, out StardewValley.Object o);
                Rectangle tileLocationRect = new Rectangle((int)tileLocation.X * 64, (int)tileLocation.Y * 64, 64, 64);
                for (int i = 0; i < __instance.characters.Count; i++)
                {
                    if (__instance.characters[i] != null && __instance.characters[i].GetBoundingBox().Intersects(tileLocationRect))
                    {
                        return;
                    }
                }
                if (__instance.isTileOccupiedByFarmer(tileLocation) != null && (toPlace == null || !toPlace.isPassable()))
                {
                    return;
                }
                if (__instance.largeTerrainFeatures != null)
                {
                    foreach (LargeTerrainFeature largeTerrainFeature in __instance.largeTerrainFeatures)
                    {
                        if (largeTerrainFeature.getBoundingBox().Intersects(tileLocationRect))
                        {
                            return;
                        }
                    }
                }

                // New logic - show the cursor as green if fertilizer can be placed there
                if (toPlace is Item
                    && IsValidTileAndItem(__instance, tileLocation, toPlace, out _))
                {
                    __result = false;
                }
            }
            catch (Exception ex)
            {
                Monitor.Log($"Failed in {nameof(isTileOccupiedForPlacement_Postfix)}:\n{ex}", LogLevel.Error);
            }
        }

        /// <summary>
        /// Note that the user is pressing a button that would apply fertilizer
        /// </summary>
        /// <param name="sender">The event sender</param>
        /// <param name="e">The event arguments</param>
        private void ButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (e.Button.IsActionButton() || e.Button.IsUseToolButton())
            {
                Clicking = true;
            }
        }

        /// <summary>
        /// Note that the user released a button that would apply fertilizer
        /// </summary>
        /// <param name="sender">The event sender</param>
        /// <param name="e">The event arguments</param>
        private void ButtonReleased(object sender, ButtonReleasedEventArgs e)
        {
            if (e.Button.IsActionButton() || e.Button.IsUseToolButton())
            {
                Clicking = false;
            }
        }

        /// <summary>
        /// Apply fertilizer to crops that have started growing but aren't
        /// yet harvestable if the user is clicking
        /// </summary>
        /// <param name="sender">The event sender</param>
        /// <param name="e">The event arguments</param>
        private void UpdateTicking(object sender, UpdateTickingEventArgs e)
        {
            if (Game1.hasLoadedGame && Clicking)
            {
                GameLocation location = Game1.currentLocation;
                Farmer player = Game1.player;
                ICursorPosition cursor = Helper.Input.GetCursorPosition();
                Vector2 tileToFertilize = cursor.GrabTile;

                if (location.isCropAtTile((int)tileToFertilize.X, (int)tileToFertilize.Y)
                    && IsValidTileAndItem(location, tileToFertilize, player.CurrentItem, out HoeDirt dirt))
                {
                    location.playSound("dirtyHit");
                    dirt.fertilizer.Value = player.CurrentItem.ParentSheetIndex;
                    player.removeItemsFromInventory(player.CurrentItem.ParentSheetIndex, 1);
                }
            }
        }
        #endregion

        #region Helpers
        /// <summary>
        /// Checks if conditions are right in the tile and that the item is
        /// fertilizer
        /// </summary>
        /// <param name="location">The game location where the action is happening</param>
        /// <param name="tile">The tile the action is happening to</param>
        /// <param name="item">The item that is being applied to the tile</param>
        /// <param name="outDirt">The dirt object contained in the tile</param>
        /// <returns></returns>
        private static bool IsValidTileAndItem(GameLocation location, Vector2 tile, Item item, out HoeDirt outDirt)
        {
            if (item != null
                && IsItemFertilizer(item)
                && location.terrainFeatures.ContainsKey(tile)
                && location.terrainFeatures[tile] is HoeDirt dirt
                && dirt.fertilizer.Value == 0
                && !IsHarvestable(dirt.crop))
            {
                outDirt = dirt;
                return true;
            }
            outDirt = null;
            return false;
        }

        /// <summary>
        /// Checks whether an item is one of the three affected fertilizers
        /// </summary>
        /// <param name="item">Item currently held by the player</param>
        /// <returns></returns>
        private static bool IsItemFertilizer(Item item)
        {
            return item.ParentSheetIndex == BasicFertilizer
                || item.ParentSheetIndex == QualityFertilizer
                || item.ParentSheetIndex == DeluxeFertilizer;
        }

        /// <summary>
        /// Check if a crop can be harvested
        /// </summary>
        /// <param name="crop">The crop to check</param>
        /// <returns>True if the crop can be harvested, false otherwise</returns>
        private static bool IsHarvestable(Crop crop)
        {
            return crop != null
                && crop.currentPhase.Value >= crop.phaseDays.Count - 1
                && (!crop.fullyGrown.Value || crop.dayOfCurrentPhase.Value <= 0);
        }
        #endregion
    }
}