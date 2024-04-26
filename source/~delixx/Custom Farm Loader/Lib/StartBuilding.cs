/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/delixx/stardew-valley/custom-farm-loader
**
*************************************************/

using Newtonsoft.Json.Linq;
using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Custom_Farm_Loader.Lib
{
    public class StartBuilding
    {
        private static Mod Mod;
        private static IMonitor Monitor;
        private static IModHelper Helper;

        public string Type = "";
        public Area Area = new Area();
        public Dictionary<string, string> Animals = new();
        public static void Initialize(Mod mod)
        {
            Mod = mod;
            Monitor = mod.Monitor;
            Helper = mod.Helper;
        }

        public static List<StartBuilding> parseJsonArray(JProperty arr)
        {
            List<StartBuilding> ret = new List<StartBuilding>();

            foreach (JObject obj in arr.First())
                ret.Add(parseJObject(obj));

            return ret;
        }

        private static StartBuilding parseJObject(JObject obj)
        {
            StartBuilding building = new StartBuilding();

            string name = "";
            try {
                foreach (JProperty property in obj.Properties()) {
                    if (property.Value.Type == JTokenType.Null)
                        continue;

                    name = property.Name;
                    string value = property.Value.ToString();

                    switch (name.ToLower()) {
                        case "type":
                            building.Type = value; break;
                        case "animals":
                            building.Animals = parseAnimals(property);
                            break;
                        default:
                            if (building.Area.parseAttribute(property))
                                break;
                            throw new ArgumentException($"Unknown StartBuildings Attribute", name);
                    }
                }
            } catch (Exception ex) {
                Monitor.Log($"At StartBuildings -> '{name}'", LogLevel.Error);
                Monitor.Log(ex.Message, LogLevel.Trace);
                throw;
            }

            return building;
        }

        private static Dictionary<string, string> parseAnimals(JProperty arr)
        {
            Dictionary<string, string> ret = new();

            foreach (JObject obj in arr.Children()) {
                foreach (JProperty property in obj.Properties()) {
                    if (property.Value.Type == JTokenType.Null)
                        continue;

                    string name = property.Name;
                    string value = property.Value.ToString();
                    ret.Add(name, value);
                }
            }

            return ret;
        }
    }
}
