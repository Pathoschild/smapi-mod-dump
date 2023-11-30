/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/gzhynko/StardewMods
**
*************************************************/

using System;
using System.Collections.Generic;
using CropGrowthAdjustments.Patching;
using CropGrowthAdjustments.Types;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;

namespace CropGrowthAdjustments
{
    /// <summary> The mod entry class loaded by SMAPI. </summary>
    public class ModEntry : Mod
    {
        public static ContentPackManager ContentPackManager = new ContentPackManager();
        public static IMonitor ModMonitor;

        private IJsonAssetsApi _jsonAssetsApi;
        
        #region Public methods

        /// <summary> The mod entry point, called after the mod is first loaded. </summary>
        /// <param name="helper"> Provides simplified APIs for writing mods. </param>
        public override void Entry(IModHelper helper)
        {
            helper.Events.Content.AssetRequested += OnAssetRequested;
            helper.Events.GameLoop.GameLaunched += OnGameLaunched;
            
            ContentPackManager.InitializeContentPacks(helper, Monitor);

            ModMonitor = Monitor;
        }

        private void OnAssetRequested(object sender, AssetRequestedEventArgs e)
        {
            if (e.NameWithoutLocale.IsEquivalentTo("TileSheets/crops"))
            {
                e.Edit(EditCropTilesheet);
            }
            else if (e.NameWithoutLocale.IsEquivalentTo("Data/Crops"))
            {
                e.Edit(EditCropData);
            }
        }
        
        #endregion
        #region Private methods

        private void EditCropTilesheet(IAssetData asset)
        {
            var editor = asset.AsImage();

            // determine the total number of sprites that need to be loaded
            var totalNewSpritesCount = 0;
            foreach (var adjustments in ContentPackManager.ContentPacks)
            {
                foreach (var cropAdjustment in adjustments.CropAdjustments)
                {
                    if(cropAdjustment.SpecialSpritesForSeasons == null) continue;

                    foreach (var _ in cropAdjustment.SpecialSpritesForSeasons)
                    {
                        totalNewSpritesCount++;
                    }
                }
            }

            // expand the spritesheet to the bottom to fit the special sprites.
            int minHeight = (int) (768 + Math.Ceiling((decimal) (totalNewSpritesCount / 2f)) * 32);
            editor.ExtendImage(0, minHeight);
            
            // 48 is the first row unused by the game.
            var currentRowInSpriteSheet = 48;
            
            // loop thru all loaded content packs
            foreach (var adjustments in ContentPackManager.ContentPacks)
            {
                // loop thru all crop adjustments within this pack
                foreach (var cropAdjustment in adjustments.CropAdjustments)
                {
                    if (cropAdjustment.SpecialSpritesForSeasons == null) continue;
                    
                    // load and add special sprites for this crop adjustment
                    foreach (var specialSprite in cropAdjustment.SpecialSpritesForSeasons)
                    {
                        // Abort adding special sprites if JsonAssets has custom crops loaded and the limit of 100 rows is reached.
                        if (Utility.JsonAssetsHasCropsLoaded(_jsonAssetsApi) && currentRowInSpriteSheet >= 99)
                        {
                            Monitor.Log($"Special sprites for {adjustments.ContentPack.Manifest.Name} (season: {specialSprite.Season}) cannot be fully loaded as the limit of 100 rows (set by JsonAssets) in the crops tilesheet is reached. The content pack will continue to work correctly, though you won't see the special sprites.", LogLevel.Warn);
                            return;
                        }
                        
                        Texture2D sourceImage;
                        
                        try
                        {
                            sourceImage = adjustments.ContentPack.ModContent.Load<Texture2D>(specialSprite.Sprites);
                        }
                        catch (Exception e)
                        {
                            Monitor.Log($"{adjustments.ContentPack.Manifest.Name} - Could not load special sprites for {cropAdjustment.CropProduceName} (season: {specialSprite.Season}), the plant might be invisible: {e}.", LogLevel.Error);
                            continue;
                        }

                        int xCoordinate = currentRowInSpriteSheet % 2 == 0 ? 0 : 128;
                        int yCoordinate = (int) (Math.Floor(currentRowInSpriteSheet / 2f) * 32);
                        
                        editor.PatchImage(sourceImage, targetArea: new Rectangle(xCoordinate, yCoordinate, 128, 32));

                        specialSprite.RowInSpriteSheet = currentRowInSpriteSheet;
                        currentRowInSpriteSheet++;
                    }
                }
            }
        }
        
        private void EditCropData(IAssetData asset)
        {
            IDictionary<int, string> cropData = asset.AsDictionary<int, string>().Data;
            
            foreach (var adjustments in ContentPackManager.ContentPacks)
            {
                foreach (var cropAdjustment in adjustments.CropAdjustments)
                {
                    var cropProduceItemId = cropAdjustment.CropProduceItemId;
                    if (cropProduceItemId == -1) continue;
                    
                    foreach (var itemId in cropData.Keys)
                    {
                        var itemData = cropData[itemId];
                        var fields = itemData.Split('/');
                        
                        if(int.Parse(fields[3]) != cropProduceItemId) continue;
                        
                        fields[1] = cropAdjustment.GetSeasonsToGrowIn().Join(delimiter: " ");
                        cropData[itemId] = string.Join("/", fields);
                        
                        break;
                    }
                }
            }
        }
        
        private void ApplyHarmonyPatches()
        {
            var harmony = new Harmony("GZhynko.CropGrowthAdjustments");
            
            harmony.Patch(
                AccessTools.Method(typeof(HoeDirt), nameof(HoeDirt.dayUpdate)),
                new HarmonyMethod(typeof(HarmonyPatches), nameof(HarmonyPatches.HoeDirtDayUpdate))
            );
            
            harmony.Patch(
                AccessTools.Method(typeof(HoeDirt), nameof(HoeDirt.plant)),
                postfix: new HarmonyMethod(typeof(HarmonyPatches), nameof(HarmonyPatches.HoeDirtPlant))
            );
            
            harmony.Patch(
                AccessTools.Method(typeof(IndoorPot), nameof(IndoorPot.DayUpdate)), 
                new HarmonyMethod(typeof(HarmonyPatches), nameof(HarmonyPatches.IndoorPotDayUpdate))
            );

            harmony.Patch(
                AccessTools.Method(typeof(Crop), nameof(Crop.newDay)),
                new HarmonyMethod(typeof(HarmonyPatches), nameof(HarmonyPatches.CropNewDay))
            );
        }

        /// <summary> Raised after the game is launched, right before the first update tick. This happens once per game session (unrelated to loading saves). All mods are loaded and initialised at this point, so this is a good time to set up mod integrations. </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            ApplyHarmonyPatches();
            
            var jsonAssetsApi = Helper.ModRegistry.GetApi<IJsonAssetsApi>("spacechase0.JsonAssets");
            _jsonAssetsApi = jsonAssetsApi;

            if (jsonAssetsApi != null)
            {
                jsonAssetsApi.IdsAssigned += (o, args) => OnJsonAssetsIdsAssigned(jsonAssetsApi);
            }
            else
            {
                ContentPackManager.AssignCropProduceItemIds(Helper, null);
                ContentPackManager.AssignCropOriginalRowsInSpritesheet(Helper);
            }
        }

        private void OnJsonAssetsIdsAssigned(IJsonAssetsApi jsonAssetsApi)
        {
            ContentPackManager.AssignCropProduceItemIds(Helper, jsonAssetsApi);
            Helper.GameContent.InvalidateCache("Data/Crops");
            
            ContentPackManager.AssignCropOriginalRowsInSpritesheet(Helper);
        }

        #endregion
    }
}
