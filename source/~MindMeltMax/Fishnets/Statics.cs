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
using StardewValley.GameData.Locations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Fishnets
{
    internal static class Statics
    {
        internal static List<string> ExcludedFish;

        internal static Object GetRandomFishForLocation(string bait, Farmer who, string locationName) //Actually going to try to explain this code, to make sure I still understand later
        {
            Dictionary<string, LocationData> locationData = ModEntry.IHelper.GameContent.Load<Dictionary<string, LocationData>>("Data\\Locations");

            string itemId = ""; //QualifiedItemId of the fish to return
            bool flag2 = bait == "908"; //If magic bait, run magic code
            string locationKey = locationName;
            if (locationKey == "BeachNightMarket") //BeachNightMarket == Beach as far as locationData is concerned
                locationKey = "Beach";

            if (locationData.ContainsKey(locationKey))
            {
                GameLocation location = Game1.getLocationFromName(locationKey);

                List<string> keys = new(locationData[locationKey].Fish.SelectMany(x => x.Id.Split('|')));
                Dictionary<string, string> fishData = ModEntry.IHelper.GameContent.Load<Dictionary<string, string>>("Data\\Fish");
                Utility.Shuffle(Game1.random, keys); //Randomize the fish ids
                for (int i = 0; i < keys.Count; ++i)
                {
                    bool flag3 = false; //If weather matches, allow catch
                    if (ItemRegistry.GetTypeDefinition($"{keys[i].Split(')')[0]})") != ItemRegistry.GetObjectTypeDefinition()) //Filter out non-object keys
                        continue;
                    if (!fishData.TryGetValue(UnQualifyItemId(keys[i]), out string fishStr))
                    {
                        ModEntry.IMonitor.Log($"Tried to catch fish with id {keys[i]}, but it could not be found in the fish data");
                        continue;
                    }
                    string cond = locationData[locationKey].Fish.First(x => x.Id.Contains(keys[i])).Condition;
                    if (!string.IsNullOrWhiteSpace(cond) && !GameStateQuery.CheckConditions(cond, location, who))
                    {
                        ModEntry.IMonitor.Log($"Conditions for catching fish with id {keys[i]} failed");
                        continue;
                    }
                    string[] fish = fishStr.Split('/');
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
                            itemId = keys[i];
                            break;
                        }
                    }
                }
            }
            int stackSize = bait == "774" && Game1.random.NextDouble() <= .15 ? 2 : 1; //If using wild bait, get small chance for double catch
            if (string.IsNullOrWhiteSpace(itemId)) //If no catch made, catch trash
            {
                itemId = $"(O){Game1.random.Next(167, 173)}";
                stackSize = 1;
            }
            if (Game1.random.NextDouble() <= .15 && Game1.player.team.SpecialOrderRuleActive("DROP_QI_BEANS")) //If has qi crop challenge active, get small chance to get qi beans instead
                itemId = "(O)890";
            Object obj = (Object)ItemRegistry.Create(itemId, stackSize); //Create fish from id and return
            return obj;
        }

        internal static bool CanCatchThisFish(string itemId, string locationName)
        {
            Dictionary<string, LocationData> locationData = Game1.content.Load<Dictionary<string, LocationData>>("Data\\Locations");

            string locationKey = locationName;
            if (locationKey == "BeachNightMarket")
                locationKey = "Beach";
            if (!locationData.TryGetValue(locationKey, out LocationData data))
                return false;

            foreach (var fish in data.Fish)
                if (fish.ItemId == itemId)
                    return true;
            return false;
        }

        internal static Object? GetObjectFromSerializable(FishNetSerializable serializable)
        {
            Object? o = null;
            string id = serializable.ObjectId;
            if (serializable.IsJAObject)
            {
                id = ModEntry.IJsonAssetsApi.GetObjectId(serializable.ObjectName);
                if (string.IsNullOrWhiteSpace(id))
                    id = serializable.ObjectId;
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
                return o;
            }
            o = (Object?)ItemRegistry.Create(id, serializable.ObjectStack, serializable.ObjectQuality, true);
            return o;
        }

        private static string UnQualifyItemId(string id) => ItemRegistry.IsQualifiedItemId(id) ? id.Split(')')[1] : id;
    }
}
