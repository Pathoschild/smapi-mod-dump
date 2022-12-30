/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/delixx/stardew-valley-custom-farm-loader
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Custom_Farm_Loader.Lib.Enums;
using Microsoft.Xna.Framework;
using StardewValley;
using Newtonsoft.Json.Linq;
using StardewModdingAPI;

namespace Custom_Farm_Loader.Lib
{
    //The start furniture will be created in:
    //GameLoopInjections._FarmHouse.loadStartFurniture

    public class Furniture
    {
        private static Mod Mod;
        private static IMonitor Monitor;
        private static IModHelper Helper;

        private static Dictionary<int, string> CachedFurnitureData;
        public static List<string> TvIds = new List<string>() { "1466", "1468", "1680", "2326" };
        public static List<string> BedIds = new List<string>() { "2048", "2052", "2058", "2064", "2070", "2176", "2180", "2186", "2192", "2496", "2502", "2508", "2514" };

        public string ID = "0";
        private string Name = ""; //Makes debugging easier
        public FurnitureType Type = FurnitureType.Furniture;
        public int Rotations;
        public Vector2 Position = new Vector2(0, 0);
        public Furniture heldObject = null;
        public List<ItemObject> Items = new List<ItemObject>();

        public static void Initialize(Mod mod)
        {
            Mod = mod;
            Monitor = mod.Monitor;
            Helper = mod.Helper;

            CachedFurnitureData = Helper.GameContent.Load<Dictionary<int, string>>("Data\\Furniture");
        }

        public static List<Furniture> parseFurnitureJsonArray(JProperty furnitureArray)
        {
            List<Furniture> ret = new List<Furniture>();

            foreach (JObject obj in furnitureArray.First())
                ret.Add(parseFurnitureJObject(obj));

            return ret;
        }

        private static Furniture parseFurnitureJObject(JObject obj)
        {
            Furniture furniture = new Furniture();

            string name = "";
            try {
                foreach (JProperty property in obj.Properties()) {
                    if (property.Value.Type == JTokenType.Null)
                        continue;

                    name = property.Name;
                    string value = property.Value.ToString();

                    switch (name.ToLower()) {
                        case "id":
                            furniture.Name = value;
                            furniture.ID = MapNameToParentsheetindex(value);
                            break;
                        case "rotations":
                            furniture.Rotations = int.Parse(value);
                            break;
                        case "type":
                            furniture.Type = UtilityMisc.parseEnum<FurnitureType>(value);
                            break;
                        case "position":
                            furniture.Position = new Vector2(int.Parse(value.Split(",")[0]), int.Parse(value.Split(",")[1]));
                            break;
                        case "heldobject":
                            furniture.heldObject = parseHeldObject((JObject)property.First());
                            break;
                        case "items":
                            furniture.Items = parseFurnitureItems(property);
                            break;
                    }
                }
            } catch (Exception ex) {
                Monitor.Log($"At StartFurniture -> '{name}'", LogLevel.Error);
                Monitor.Log(ex.Message, LogLevel.Trace);
                throw;
            }

            return furniture;
        }

        private static Furniture parseHeldObject(JObject obj)
        {
            Furniture furniture = new Furniture();

            string name = "";
                foreach (JProperty property in obj.Properties()) {
                    name = property.Name;
                    string value = property.Value.ToString();

                    switch (name.ToLower()) {
                        case "id":
                            furniture.ID = MapNameToParentsheetindex(value);
                            break;
                        case "rotations":
                            furniture.Rotations = int.Parse(value);
                            break;
                    }
                }

            return furniture;
        }

        private static List<ItemObject> parseFurnitureItems(JProperty itemArray)
        {
            List<ItemObject> items = new List<ItemObject>();

            foreach (JObject obj in itemArray.First()) {
                string id = "0";
                int amount = 1;
                int quality = 0;

                foreach (JProperty property in obj.Properties()) {
                    string name = property.Name;
                    string value = property.Value.ToString();


                    switch (name.ToLower()) {
                        case "id":
                            id = ItemObject.MapNameToParentsheetindex(value);
                            break;
                        case "amount":
                            amount = int.Parse(value);
                            break;
                        case "quality":
                            switch (value.ToLower()) {
                                case "silver":
                                    quality = 1; break;
                                case "gold":
                                    quality = 2; break;
                                case "iridium":
                                    quality = 3; break;
                            }
                            break;
                    }
                }

                items.Add(new ItemObject() { Id = id, Amount = amount, Quality = quality });
            }

            return items;
        }

        public static string MapNameToParentsheetindex(string name)
        {
            var comparableName = name.ToLower().Replace("_", " ").Replace("'", "");
            string duplicateFurnitureID = MapDuplicateFurniture(comparableName);

            if (duplicateFurnitureID != "")
                return duplicateFurnitureID;

            var match = CachedFurnitureData.FirstOrDefault(fur => fur.Value.ToLower().Replace("'", "").StartsWith(comparableName + "/"));

            if (match.Value != null)
                return match.Key.ToString();

            return name;
        }

        public static string MapParentsheetindexToName(int id)
        {
            var match = CachedFurnitureData.FirstOrDefault(fur => fur.Key == id);

            if (match.Value != null)
                return match.Value.Split("/").First();

            return "";
        }

        private static string MapDuplicateFurniture(string name)
        {
            //Some furniture in data\\furniture has the same name.
            //Instead of always placing the first one in the list we allow the user to append a number after the name
            //eg. allowed: "House Plant 2", "cloud_decal_3", "Small Junimo plush", "Ceiling leaves 1"
            //"Small Junimo Plush" and "Small Junimo Plush 1" would return the same furniture parentsheetindex

            if (name.StartsWith("house plant")) {
                if (int.TryParse(name.Split("house plant").Last(), out int nr))
                    return (1375 + nr).ToString();

                return "1376";
            }

            if (name.StartsWith("cloud decal")) {
                if (int.TryParse(name.Split("cloud decal").Last(), out int nr))
                    if (nr == 2)
                        return "1692";

                return "1687";
            }

            if (name.StartsWith("small junimo plush")) {
                if (int.TryParse(name.Split("small junimo plush").Last(), out int nr))
                    return (1759 + nr).ToString();

                return "1760";
            }

            if (name.StartsWith("ceiling leaves")) {
                if (int.TryParse(name.Split("ceiling leaves").Last(), out int nr))
                    return (1816 + nr).ToString();

                return "1817";
            }

            if (name.StartsWith("jungle decal")) {
                if (int.TryParse(name.Split("jungle decal").Last(), out int nr))
                    return (2626 + nr).ToString();

                return "2727";
            }

            if (name.StartsWith("floor divider r"))
                return name.Split("floor divider r").Last().Trim() switch {
                    "2" => "2639",
                    "3" => "2641",
                    "4" => "2643",
                    "5" => "2645",
                    "6" => "2647",
                    "7" => "2649",
                    "8" => "2651",
                    _ => "2637",
                };


            if (name.StartsWith("floor divider l"))
                return name.Split("floor divider l").Last().Trim() switch {
                    "2" => "2640",
                    "3" => "2642",
                    "4" => "2644",
                    "5" => "2646",
                    "6" => "2648",
                    "7" => "2650",
                    "8" => "2652",
                    _ => "2638",
                };


            if (name.StartsWith("wall sconce"))
                return name.Split("wall sconce").Last().Trim() switch {
                    "2" => "2736",
                    "3" => "2738",
                    "4" => "2740",
                    "5" => "2748",
                    "6" => "2750",
                    "7" => "2812",
                    _ => "2734",
                };


            return "";
        }

    }


}
