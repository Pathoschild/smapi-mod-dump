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
using CropGrowthAdjustments.Types;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.TerrainFeatures;

namespace CropGrowthAdjustments
{
    public class Utility
    {
        public static int GetItemIdByName(string itemName, IModHelper helper)
        {
            var objectData = helper.GameContent.Load<Dictionary<int, string>>("Data/ObjectInformation");

            foreach (var objectEntry in objectData)
            {
                if (CompareTwoStringsCaseAndSpaceIndependently(objectEntry.Value.Split('/')[0], itemName))
                {
                    return objectEntry.Key;
                }
            }

            return -1;
        }

        public static string[] GetCropDataForProduceItemId(int produceId, IModHelper helper)
        {
            var cropData = helper.GameContent.Load<Dictionary<int, string>>("Data/Crops");
            
            foreach (var itemId in cropData.Keys)
            {
                var itemData = cropData[itemId];
                var fields = itemData.Split('/');
                
                if(int.Parse(fields[3]) != produceId) continue;

                return fields;
            }

            return null;
        }

        public static bool JsonAssetsHasCropsLoaded(IJsonAssetsApi jsonAssetsApi)
        {
            if (jsonAssetsApi == null) return false;

            return jsonAssetsApi.GetAllCropIds().Count != 0;
        }

        public static bool CompareTwoStringsCaseAndSpaceIndependently(string first, string second)
        {
            return RemoveWhitespaceInString(first.ToLower()) == RemoveWhitespaceInString(second.ToLower());
        }
        
        public static string RemoveWhitespaceInString(string str) {
            return string.Join("", str.Split(default(string[]), StringSplitOptions.RemoveEmptyEntries));
        }

        public static bool IsCropInAnyOfSpecifiedLocations(List<string> locations, GameLocation environment)
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

        public static void ChangeSpritesToSpecial(HoeDirt hoeDirt, GameLocation location, Vector2 tileLocation)
        {
            foreach (var contentPack in ModEntry.ContentPackManager.ContentPacks)
            {
                foreach (var adjustment in contentPack.CropAdjustments)
                {
                    // skip if this crop is not the desired one.
                    if (hoeDirt.crop.indexOfHarvest.Value != adjustment.CropProduceItemId) continue;
                    // skip if there are no special sprites specified for this crop
                    if (adjustment.SpecialSpritesForSeasons == null) continue;

                    // reset the crop sprite to the default sprite at first; this will then be overriden by
                    // the special sprites if any of them is applied
                    hoeDirt.crop.rowInSpriteSheet.Value = adjustment.OriginalRowInSpriteSheet;

                    foreach (var specialSprite in adjustment.SpecialSpritesForSeasons)
                    {
                        // skip if the crop is planted in any of the ignored locations.
                        if (IsCropInAnyOfSpecifiedLocations(specialSprite.GetLocationsToIgnore(), location)) continue;
                        // skip if this special sprite is over the limit of 51.
                        if (specialSprite.RowInSpriteSheet == -1) continue;
                        // skip if this special sprite should not be applied in this season.
                        if (!CompareTwoStringsCaseAndSpaceIndependently(specialSprite.Season, Game1.currentSeason)) continue;
                        
                        // set the crop sprite to the special sprite
                        hoeDirt.crop.rowInSpriteSheet.Value = specialSprite.RowInSpriteSheet;
                        // update crop draw math
                        hoeDirt.crop.updateDrawMath(tileLocation);
                    }
                }
            }
        }
    }
}