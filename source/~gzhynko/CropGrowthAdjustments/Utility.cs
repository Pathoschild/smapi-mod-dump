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
using StardewModdingAPI;
using StardewValley;
using StardewValley.GameData.Crops;
using StardewValley.GameData.Objects;

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
                
                if(itemData.HarvestItemId != produceId) continue;

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
    }
}