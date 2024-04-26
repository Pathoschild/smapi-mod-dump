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
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.GameData.Crops;
using StardewValley.GameData.Objects;
using StardewValley.TerrainFeatures;

namespace CropGrowthAdjustments
{
    public class Utility
    {
        public static string GetItemIdByName(string itemName, IModHelper helper)
        {
            var objectData = helper.GameContent.Load<Dictionary<string, ObjectData>>("Data/Objects");

            foreach (var objectEntry in objectData)
            {
                if (CompareTwoStringsCaseAndSpaceIndependently(objectEntry.Value.Name, itemName))
                {
                    return objectEntry.Key;
                }
            }

            return "-1";
        }

        public static CropData GetCropDataForProduceItemId(string produceId, IModHelper helper)
        {
            var cropData = helper.GameContent.Load<Dictionary<string, CropData>>("Data/Crops");
            
            foreach (var itemId in cropData.Keys)
            {
                var itemData = cropData[itemId];

                if(ItemRegistry.QualifyItemId(itemData.HarvestItemId) != ItemRegistry.QualifyItemId(produceId)) continue;

                return itemData;
            }

            return null;
        }

        public static bool CompareTwoStringsCaseAndSpaceIndependently(string first, string second)
        {
            return RemoveWhitespaceInString(first.ToLower()) == RemoveWhitespaceInString(second.ToLower());
        }
        
        public static string RemoveWhitespaceInString(string str) {
            return string.Join("", str.Split(default(string[]), StringSplitOptions.RemoveEmptyEntries));
        }

        public static bool IsInAnyOfSpecifiedLocations(List<string> locations, GameLocation environment)
        {
            foreach (var location in locations)
            {
                switch (RemoveWhitespaceInString(location.ToLower()))
                {
                    case "indoors":
                        if (!environment.IsOutdoors) return true;
                        break;
                    default:
                        if (CompareTwoStringsCaseAndSpaceIndependently(location, environment.Name)) return true;
                        break;
                }
            }

            return false;
        }

        public static List<Texture2D> GetListOfSpecialTextures()
        {
            var result = new List<Texture2D>();
            foreach (var contentPack in ModEntry.ContentPackManager.ContentPacks)
            {
                foreach (var adjustment in contentPack.CropAdjustments)
                {
                    foreach (var specialSprites in adjustment.SpecialSpritesForSeasons)
                    {
                        result.Add(specialSprites.SpritesTexture);
                    }
                }
            }

            return result;
        }
        
        public static void ChangeSpritesToSpecial(HoeDirt hoeDirt)
        {
            var location = hoeDirt.Location;
            
            foreach (var contentPack in ModEntry.ContentPackManager.ContentPacks)
            {
                foreach (var adjustment in contentPack.CropAdjustments)
                {
                    // skip if this crop is not the desired one.
                    if (hoeDirt.crop.indexOfHarvest.Value != adjustment.CropProduceItemId) continue;
                    // skip if there are no special sprites specified for this crop
                    if (adjustment.SpecialSpritesForSeasons == null) continue;
                    if (adjustment.InitialTexture == ModEntry.DefaultCropSpritesheetName) continue;
                    
                    // reset the crop sprite to the default sprite at first; this will then be overriden by
                    // the special sprites if any of them is applied
                    hoeDirt.crop.overrideTexturePath.Value = adjustment.InitialTexture;
                    ModEntry.ModHelper.Reflection.GetField<Texture2D>(hoeDirt.crop, "_drawnTexture").SetValue(null);

                    foreach (var specialSprite in adjustment.SpecialSpritesForSeasons)
                    {
                        // skip if the crop is planted in any of the ignored locations.
                        if (IsInAnyOfSpecifiedLocations(specialSprite.GetLocationsToIgnore(), location)) continue;
                        // skip if this special sprite should not be applied in this season.
                        if (specialSprite.GetSeason() != Game1.season)
                            continue;

                        // set the crop sprite to the special sprite
                        hoeDirt.crop.overrideTexturePath.Value = specialSprite.SpritesTexture.Name;
                        ModEntry.ModHelper.Reflection.GetField<Texture2D>(hoeDirt.crop, "_drawnTexture").SetValue(null);

                        // update crop draw math
                        hoeDirt.crop.updateDrawMath(hoeDirt.crop.tilePosition);
                    }
                }
            }
        }
    }
}