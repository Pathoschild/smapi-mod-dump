/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/mjSurber/FarmHouseRedone
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xTile;
using Newtonsoft.Json.Linq;
using StardewModdingAPI;

namespace FarmHouseRedone
{
    public class LevelNUpgrade
    {
        public string name;
        public int level;
        public string description;
        public Dictionary<int, int> cost;
        public int moneyCost;
        public Map map;
        public Map marriageMap;

        public LevelNUpgrade(JObject upgrade)
        {
            try
            {
                name = upgrade["Name"].ToString();
                level = Convert.ToInt32(upgrade["Level"]);
                description = upgrade["Description"].ToString();
                cost = new Dictionary<int, int>();
                moneyCost = 0;
                foreach(KeyValuePair<string, JToken> item in upgrade["Cost"] as JObject)
                {
                    if (item.Key.ToLower() == "g")
                    {
                        moneyCost = Convert.ToInt32(item.Value);
                        continue;
                    }
                    int id = ObjectIDHelper.getID(item.Key);
                    if (id == -1)
                    {
                        Logger.Log("Couldn't find the ID for an object named \"" + item.Key + "\"!", StardewModdingAPI.LogLevel.Error);
                        continue;
                    }
                    cost[id] = Convert.ToInt32(item.Value);
                }
                map = findMapByName(upgrade["Map"].ToString());
                if (upgrade.ContainsKey("Marriage"))
                    marriageMap = findMapByName(upgrade["Marriage"].ToString());
                else
                    marriageMap = findMapByName(upgrade["Map"].ToString() + "_marriage");
            }
            catch (KeyNotFoundException e)
            {
                Logger.Log("A farmhouse upgrade definition was missing a required field!", LogLevel.Error);
                throw e;
            }
        }

        internal static Map findMapByName(string name)
        {
            Map map = null;
            //Try loading from the game content.  This is where Content Patcher patches will place them.
            try
            {
                map = FarmHouseStates.loader.Load<Map>("Maps/" + name, ContentSource.GameContent);
                Logger.Log("Found a map by the name '" + ("Maps/" + name) + "'!");
            }
            catch (Microsoft.Xna.Framework.Content.ContentLoadException)
            {
                //No map found.
            }
            //Next try loading from the mod directory, for the default cellar map
            try
            {
                map = FarmHouseStates.loader.Load<Map>("assets/maps/" + name + ".tbin", ContentSource.ModFolder);
                Logger.Log("Found packaged default map by the name '" + ("assets/maps/" + name) + "'");
            }
            catch (Microsoft.Xna.Framework.Content.ContentLoadException e)
            {
                Logger.Log("No farmhouse map found by the name " + name + " within assets/maps/");
                Logger.Log(e.Message + e.StackTrace);
            }
            if (map == null)
            {
                Logger.Log("No farmhouse map could be found by the name '" + name + "'!");
            }
            return map;
        }

        internal string priceListToString()
        {
            string outString = "";
            foreach(int id in cost.Keys)
            {
                outString += id + " x " + cost[id] + ", ";
            }
            outString += moneyCost + "g";
            return outString;
        }

        public override string ToString()
        {
            return "Level " + level + " upgrade:\nDescription: " + description + "\nCost: " + priceListToString() + "\nMap: " + map.Id + " Marriage: " + marriageMap.Id;
        }
    }
}
