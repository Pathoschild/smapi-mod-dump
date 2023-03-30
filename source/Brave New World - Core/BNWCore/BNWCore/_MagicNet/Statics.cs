/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/DiogoAlbano/StardewValleyMods
**
*************************************************/

using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using Object = StardewValley.Object;

namespace BNWCore
{
    internal static class Statics
    {
        internal readonly static List<string> ExcludedFish = new List<string>()
        {
            "Crimsonfish",
            "Angler",
            "Legend",
            "Mutant Carp",
            "Glacierfish",
            "Son of Crimsonfish",
            "Ms. Angler",
            "Legend II",
            "Radioactive Carp",
            "Glacierfish Jr."
        };
        internal static Object GetRandomFishForLocation(int bait, Farmer who, string locationName)
        {
            Dictionary<string, string> locationData = Game1.content.Load<Dictionary<string, string>>("Data\\Locations");
            int parentSheetIndex = -1;
            bool flag1 = false;
            bool flag2 = bait == 908;
            string locationKey = locationName;
            if (locationKey == "BeachNightMarket")
                locationKey = "Beach";
            if (locationData.ContainsKey(locationKey))
            {
                GameLocation location = Game1.getLocationFromName(locationKey);
                string[] arr1 = locationData[locationKey].Split('/')[4 + Utility.getSeasonNumber(Game1.currentSeason)].Split(' ');
                if (flag2)
                {
                    List<string> strings = new();
                    for (int i = 0; i < 4; ++i)
                        if (locationData[locationKey].Split('/')[4 + i].Split(' ').Length > 1)
                            strings.AddRange(locationData[locationKey].Split('/')[4 + i].Split(' '));
                    arr1 = strings.ToArray();
                }
                Dictionary<string, string> data = new();
                if (arr1.Length > 1)
                    for (int i = 0; i < arr1.Length; i += 2)
                        data[arr1[i]] = arr1[i + 1];
                string[] keys = data.Keys.ToArray();
                Dictionary<int, string> fishData = Game1.content.Load<Dictionary<int, string>>("Data\\Fish");
                Utility.Shuffle(Game1.random, keys);
                for (int i = 0; i < keys.Length; ++i)
                {
                    bool flag3 = false;
                    string[] fish = fishData[Convert.ToInt32(keys[i])].Split('/');
                    if (ExcludedFish.Contains(fish[0])) continue;
                    if (fish[7] != "both")
                    {
                        if (fish[7] == "rainy" && !Game1.IsRainingHere(location))
                            flag3 = true;
                        else if (fish[7] == "sunny" && Game1.IsRainingHere(location))
                            flag3 = true;
                    }
                    if (flag2) 
                        flag3 = false;
                    if (who.FishingLevel < Convert.ToInt32(fish[12])) 
                        flag3 = true;
                    if (!flag3)
                    {
                        double chance1 = Convert.ToDouble(fish[10]);
                        double chance2 = chance1 + who.FishingLevel / 50.0d;
                        double chance3 = Math.Min(chance2, 0.899999976158142);
                        if (Game1.random.NextDouble() <= chance3)
                        {
                            parentSheetIndex = Convert.ToInt32(keys[i]);
                            break;
                        }
                    }
                }
            }
            int stackSize = bait == 774 && Game1.random.NextDouble() <= .15 ? 2 : 1;
            if (parentSheetIndex == -1)
            {
                parentSheetIndex = Game1.random.Next(167, 173);
                stackSize = 1;
            }
            if (Game1.random.NextDouble() <= .15 && Game1.player.team.SpecialOrderRuleActive("DROP_QI_BEANS"))
                parentSheetIndex = 890;
            Object obj = new(parentSheetIndex, stackSize);
            if (flag1)
                obj.scale.X = 1f;
            return obj;
        }
        internal static bool CanCatchThisFish(int index, string locationName)
        {
            Dictionary<string, string> locationData = Game1.content.Load<Dictionary<string, string>>("Data\\Locations");

            string locationKey = locationName;
            if (locationKey == "BeachNightMarket")
                locationKey = "Beach";
            if (locationData.ContainsKey(locationKey))
            {
                GameLocation location = Game1.getLocationFromName(locationKey);
                string[] arr1 = locationData[locationKey].Split('/')[4 + Utility.getSeasonNumber(Game1.currentSeason)].Split(' ');
                Dictionary<string, string> data = new();
                if (arr1.Length > 1)
                    for (int i = 0; i < arr1.Length; i += 2)
                        data[arr1[i]] = arr1[i + 1];
                string[] keys = data.Keys.ToArray();
                return keys.Any(x => x == $"{index}");
            }
            return false;
        }
    }
}
