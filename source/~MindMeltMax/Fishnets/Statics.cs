/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MindMeltMax/Stardew-Valley-Mods
**
*************************************************/

using Fishnets.Data;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fishnets
{
    internal static class Statics
    {
        internal readonly static List<string> ExcludedFish = new()
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

        internal static Object GetRandomFishForLocation(int bait, Farmer who, string locationName) //Actually going to try to explain this code, to make sure I still understand later
        {
            Dictionary<string, string> locationData = Game1.content.Load<Dictionary<string, string>>("Data\\Locations");

            int parentSheetIndex = -1; //Index of the fish to return
            bool flag2 = bait == 908; //If magic bait, run magic code
            string locationKey = locationName;
            if (locationKey == "BeachNightMarket") //BeachNightMarket == Beach as far as locationData is concerned
                locationKey = "Beach";

            if (locationData.ContainsKey(locationKey))
            {
                GameLocation location = Game1.getLocationFromName(locationKey);

                string[] arr1 = locationData[locationKey].Split('/')[4 + Utility.getSeasonNumber(Game1.currentSeason)].Split(' '); //Get an array of fish id + zone id (see https://stardewvalleywiki.com/Modding:Location_data)
                if (flag2) //If magic bait, ignore season
                {
                    List<string> strings = new();
                    for (int i = 0; i < 4; ++i)
                        if (locationData[locationKey].Split('/')[4 + i].Split(' ').Length > 1)
                            strings.AddRange(locationData[locationKey].Split('/')[4 + i].Split(' '));
                    arr1 = strings.ToArray();
                }
                Dictionary<string, string> data = new();
                if (arr1.Length > 1) //Create dictionary separating fish id and zone id
                    for (int i = 0; i < arr1.Length; i += 2)
                        data[arr1[i]] = arr1[i + 1];
                string[] keys = data.Keys.ToArray();
                Dictionary<int, string> fishData = Game1.content.Load<Dictionary<int, string>>("Data\\Fish");
                Utility.Shuffle(Game1.random, keys); //Randomize the fish ids
                for (int i = 0; i < keys.Length; ++i)
                {
                    bool flag3 = false; //If weather matches, allow catch
                    string[] fish = fishData[Convert.ToInt32(keys[i])].Split('/');
                    if (ExcludedFish.Contains(fish[0])) //If the fish's name appears in the list of excluded fish, move on
                        continue; 
                    if (fish[7] != "both") //If fish can't be caught during both rain and sunshine...
                    {
                        if (fish[7] == "rainy" && !Game1.IsRainingHere(location)) //If it can be caught during rain and it's not raining, don't allow catch
                            flag3 = true;
                        else if (fish[7] == "sunny" && Game1.IsRainingHere(location)) //If it can be caught while it's clear and it's raining, don't allow catch
                            flag3 = true;
                    }
                    if (flag2) //If using magic bait, ignore weather
                        flag3 = false;
                    if (who.FishingLevel < Convert.ToInt32(fish[12])) //If the player's fishing level is below the fishes minimum catch level, don't allow catch, regardless of bait
                        flag3 = true;

                    if (!flag3) //If catch is allowed
                    {
                        double chance1 = Convert.ToDouble(fish[10]); //Get fish catch chance
                        double chance2 = chance1 + who.FishingLevel / 50.0d; //Add chance increase for player's fishing level
                        double chance3 = Math.Min(chance2, 0.899999976158142); //Get chance below 0.9
                        if (Game1.random.NextDouble() <= chance3) //If success catch fish at current index
                        {
                            parentSheetIndex = Convert.ToInt32(keys[i]);
                            break;
                        }
                    }
                }
            }
            int stackSize = bait == 774 && Game1.random.NextDouble() <= .15 ? 2 : 1; //If using wild bait, get small chance for double catch
            if (parentSheetIndex == -1) //If no catch made, catch trash
            {
                parentSheetIndex = Game1.random.Next(167, 173);
                stackSize = 1;
            }
            if (Game1.random.NextDouble() <= .15 && Game1.player.team.SpecialOrderRuleActive("DROP_QI_BEANS")) //If has qi crop challenge active, get small chance to get qi beans instead
                parentSheetIndex = 890;
            Object obj = new(parentSheetIndex, stackSize); //Create fish from id and return
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

        internal static Object? GetObjectFromSerializable(FishNetSerializable serializable)
        {
            Object? o = null;
            if (serializable.IsJAObject)
            {
                int id = ModEntry.IJsonAssetsApi.GetObjectId(serializable.ObjectName);
                if (id != -1)
                    o = new(id, serializable.ObjectStack) { Quality = serializable.ObjectQuality };
            }
            else if (serializable.IsDGAObject)
            {
                object spawned = ModEntry.IDynamicGameAssetsApi.SpawnDGAItem(serializable.ObjectName);
                if (spawned is Object dgaObject)
                {
                    o = dgaObject;
                    o.Stack = serializable.ObjectStack;
                    o.Quality = serializable.ObjectQuality;
                }
            }
            else if (serializable.ObjectId >= 0)
                o = new(serializable.ObjectId, serializable.ObjectStack) { Quality = serializable.ObjectQuality };
            return o;
        }
    }
}
