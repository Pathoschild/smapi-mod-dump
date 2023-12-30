/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/gzhynko/stardew-mods
**
*************************************************/

using System;
using System.Collections.Generic;
using StardewModdingAPI;
using StardewValley;

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