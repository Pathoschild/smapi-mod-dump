/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/delixx/stardew-valley-custom-farm-loader
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json.Linq;
using StardewModdingAPI;
using StardewValley;
using StardewValley.GameData;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Custom_Farm_Loader.Lib
{
    public class CustomFarm
    {
        private static bool LoadedCachedCustomFarms = false;
        public static List<CustomFarm> CachedCustomFarms = new List<CustomFarm>();
        public static List<ModFarmType> CachedModFarmTypes = new List<ModFarmType>();

        private static Mod Mod;
        private static IMonitor Monitor;
        private static IModHelper Helper;
        private static Texture2D MissingMapIcon;

        private static readonly List<string> VanillaTypes = new List<string> { "standard", "riverland", "forest", "hills", "wilderness", "four corners", "beach" };
        private static CustomFarm CurrentCustomFarm = null;
        private static string CurrentCustomFarmId = "";

        private bool IsCFLMap = false;
        private ModFarmType ModFarmType; //Only used for non CFL maps

        public string UniqueModID = "";
        public string ID = "";

        public string Name = "Unknown Farm";
        public string Description = "<none>";
        public List<KeyValuePair<string, string>> TranslatedDescriptions = new List<KeyValuePair<string, string>>();

        public Texture2D Icon;
        public Texture2D WorldMapOverlay;
        public string Preview = "";
        private string IconValue = "";
        private string WorldMapOverlayValue = "";

        public string Author = "Unknown";
        public int MaxPlayers = 0;
        public string MapFile = "";

        public List<KeyValuePair<string, Point>> Warps = new List<KeyValuePair<string, Point>>();
        public List<KeyValuePair<string, Point>> Locations = new List<KeyValuePair<string, Point>>();

        public List<Furniture> StartFurniture = new List<Furniture>();
        public List<DailyUpdate> DailyUpdates = new List<DailyUpdate>();
        public List<FishingRule> FishingRules = new List<FishingRule>();
        public List<Bridge> Bridges = new List<Bridge>();
        public FarmProperties Properties = new FarmProperties();


        public string ContentPackDirectory = ""; // The mod directory of the map we're loading
        public string RelativeContentPackPath = ""; // The mod folder of the map we're loading as a relative path to CFL
        public string RelativeMapDirectoryPath = ""; // The map directory of the map we're loading relative its mod folder
        public CustomFarm() { }

        public CustomFarm(ModFarmType modFarmType)
        {
            ModFarmType = modFarmType;
        }

        public static void Initialize(Mod mod)
        {
            Mod = mod;
            Monitor = mod.Monitor;
            Helper = mod.Helper;

            MissingMapIcon = ModEntry._Helper.ModContent.Load<Texture2D>("assets/MissingMapIcon.png");
            getAll(); //Cache all maps on gameload to decrease load later
        }

        public static CustomFarm parseMapJson(string mapJson, string contentPackDirectory, IManifest manifest)
        {
            JObject o;
            CustomFarm customFarm = new CustomFarm();
            customFarm.ContentPackDirectory = contentPackDirectory.Split(Path.DirectorySeparatorChar).Last();
            string relativeMapJsonPath = mapJson.Substring(contentPackDirectory.Count() + 1);
            customFarm.RelativeMapDirectoryPath = relativeMapJsonPath.Substring(0, (relativeMapJsonPath.Count() - relativeMapJsonPath.Split(Path.DirectorySeparatorChar).Last().Count() - 1));
            customFarm.RelativeContentPackPath = Path.GetRelativePath(Helper.DirectoryPath + Path.DirectorySeparatorChar, contentPackDirectory + Path.DirectorySeparatorChar);

            customFarm.UniqueModID = manifest.UniqueID;
            customFarm.Author = manifest.Author;
            customFarm.IsCFLMap = true;

            try {
                o = JObject.Parse(File.ReadAllText(mapJson));
            } catch (Exception ex) {
                Monitor.Log($"Failed to parse cfl_map.json in path:\n{customFarm.ContentPackDirectory}{Path.DirectorySeparatorChar}{relativeMapJsonPath}", LogLevel.Error);
                Monitor.Log(ex.Message, LogLevel.Error);
                return null;
            }

            string name = "";
            try {
                JEnumerable<JToken> childNodes = o.Children();

                foreach (JProperty n in childNodes) {
                    if (n.Value.Type == JTokenType.Null)
                        continue;

                    name = n.Name;
                    string value = n.Value.ToString();

                    switch (name.ToLower()) {
                        case "name":
                            customFarm.Name = value;
                            break;
                        case "author":
                            customFarm.Author = value;
                            break;
                        case "description":
                            customFarm.Description = value;
                            break;
                        case "mapfile":
                            customFarm.MapFile = value;
                            break;
                        case "maxplayers":
                            customFarm.MaxPlayers = int.Parse(value);
                            break;
                        case "icon":
                            customFarm.IconValue = value;
                            break;
                        case "preview":
                            if (value != "")
                                customFarm.Preview = $"{customFarm.RelativeContentPackPath}{customFarm.RelativeMapDirectoryPath}\\{value}";
                            break;
                        case "worldmapoverlay":
                            customFarm.WorldMapOverlayValue = value;
                            customFarm.WorldMapOverlay = customFarm.loadWorldMapTexture();
                            break;
                        case "warps":
                            customFarm.Warps = parseWarps(n);
                            break;
                        case "locations":
                            customFarm.Locations = parseLocations(n);
                            break;
                        case "startfurniture":
                            customFarm.StartFurniture = Furniture.parseFurnitureJsonArray(n);
                            break;
                        case "dailyupdates":
                            customFarm.DailyUpdates = DailyUpdate.parseDailyUpdateJsonArray(n);
                            break;
                        case "fishingrules":
                            customFarm.FishingRules = FishingRule.parseFishingRuleJsonArray(n);
                            break;
                        case "bridges":
                            customFarm.Bridges = Bridge.parseBridgeJObject(n);
                            break;
                        case "properties":
                            customFarm.Properties = FarmProperties.parseJObject(n);
                            break;
                        case "descriptiontranslations":
                            customFarm.TranslatedDescriptions = parseTranslatedDescriptionJObject(n);
                            break;
                        default:
                            Monitor.Log("Unknown cfl_map.json Attribute", LogLevel.Error);
                            throw new ArgumentException($"Unknown cfl_map.json Attribute", name);
                    }
                }

                customFarm.ID = $"{customFarm.UniqueModID}/{customFarm.Name}";

                return customFarm;
            } catch (Exception ex) {
                Monitor.Log($"Failed to parse cfl_map.json {(name != "" ? $"at '{name}' " : "")}in path:\n{customFarm.ContentPackDirectory}{Path.DirectorySeparatorChar}{relativeMapJsonPath}", LogLevel.Error);
                Monitor.Log(ex.Message, LogLevel.Error);
            }


            return null;
        }

        private static List<KeyValuePair<string, Point>> parseWarps(JProperty jArray)
        {
            List<KeyValuePair<string, Point>> ret = new List<KeyValuePair<string, Point>>();
            string name = "";
            string value = "";

            try {
                foreach (JProperty jProperty in jArray.First()) {
                    if (jProperty.Value.Type == JTokenType.Null)
                        continue;

                    name = jProperty.Name;
                    value = jProperty.Value.ToString();

                    if (value == "")
                        continue;

                    switch (name.ToLower().Replace(" ", "")) {
                        case "busstop" or "forest" or "backwoods" or "farmcave" or "warptotem":
                            ret.Add(new KeyValuePair<string, Point>(name, UtilityMisc.parsePoint(value)));
                            break;
                        default:
                            Monitor.Log("Unknown Warps Attribute", LogLevel.Error);
                            throw new ArgumentException($"Unknown Warps Attribute", name);
                    }
                }
            } catch (Exception ex) {
                Monitor.Log($"At Warps -> '{name}'", LogLevel.Error);
                Monitor.Log(ex.Message, LogLevel.Trace);
                throw;
            }

            return ret;
        }

        private static List<KeyValuePair<string, Point>> parseLocations(JProperty jArray)
        {
            List<KeyValuePair<string, Point>> ret = new List<KeyValuePair<string, Point>>();
            string name = "";
            string value = "";

            try {
                foreach (JProperty jProperty in jArray.First()) {
                    if (jProperty.Value.Type == JTokenType.Null)
                        continue;

                    name = jProperty.Name;
                    value = jProperty.Value.ToString();

                    if (value == "")
                        continue;

                    switch (name.ToLower().Replace(" ", "")) {
                        case "farmhouse" or "greenhouse" or "spousearea" or "mailbox" or "shippingbin" or "grandpashrine":
                            ret.Add(new KeyValuePair<string, Point>(name, UtilityMisc.parsePoint(value)));
                            break;
                        default:
                            Monitor.Log("Unknown Locations Attribute", LogLevel.Error);
                            throw new ArgumentException($"Unknown Locations Attribute", name);
                    }
                }
            } catch (Exception ex) {
                Monitor.Log($"At Locations -> '{name}'", LogLevel.Error);
                Monitor.Log(ex.Message, LogLevel.Trace);
                throw;
            }

            return ret;
        }

        private static List<KeyValuePair<string, string>> parseTranslatedDescriptionJObject(JProperty jObject)
        {
            List<KeyValuePair<string, string>> ret = new List<KeyValuePair<string, string>>();

            foreach (JProperty jProperty in jObject.First()) {
                if (jProperty.Value.Type == JTokenType.Null)
                    continue;

                ret.Add(new KeyValuePair<string, string>(jProperty.Name, (string)jProperty.Value));
            }

            return ret;
        }

        public void reloadTextures()
        {
            if (!IsCFLMap)
                return;

            //We load the icon and world map textures late to give recolor mods time to patch them.
            //The world map is additionally being reloaded on daystart to allow for seasonal world maps
            if (Icon == null)
                Icon = Helper.GameContent.Load<Texture2D>(asModFarmType().IconTexture);

            Helper.GameContent.InvalidateCache(asModFarmType().WorldMapTexture);
            WorldMapOverlay = Helper.GameContent.Load<Texture2D>(asModFarmType().WorldMapTexture);
        }

        public Texture2D loadIconTexture()
        {
            string path = RelativeContentPackPath + RelativeMapDirectoryPath;

            if (IconValue == "") return MissingMapIcon;

            if (Game1.mouseCursors == null)
                Game1.mouseCursors = Game1.content.Load<Texture2D>("LooseSprites\\Cursors");

            switch (IconValue.ToLower()) {
                case "standard":
                    return UtilityMisc.createSubTexture(Game1.mouseCursors, new Rectangle(2, 324, 18, 20));
                case "riverland":
                    return UtilityMisc.createSubTexture(Game1.mouseCursors, new Rectangle(24, 324, 19, 20));
                case "forest":
                    return UtilityMisc.createSubTexture(Game1.mouseCursors, new Rectangle(46, 324, 18, 20));
                case "hills":
                    return UtilityMisc.createSubTexture(Game1.mouseCursors, new Rectangle(68, 324, 18, 20));
                case "wilderness":
                    return UtilityMisc.createSubTexture(Game1.mouseCursors, new Rectangle(90, 324, 18, 20));
                case "four corners":
                    return UtilityMisc.createSubTexture(Game1.mouseCursors, new Rectangle(2, 345, 18, 20));
                case "beach":
                    return UtilityMisc.createSubTexture(Game1.mouseCursors, new Rectangle(24, 345, 18, 20));
            }

            try {
                return Helper.ModContent.Load<Texture2D>($"{path}\\{IconValue}");

            } catch (Exception ex) {
                Monitor.LogOnce($"Unable to load the map icon in:\n{path}\\{IconValue}", LogLevel.Warn);
            }
            return MissingMapIcon;
        }

        public Texture2D loadWorldMapTexture()
        {
            string path = RelativeContentPackPath + RelativeMapDirectoryPath;
            string worldMapFile = WorldMapOverlayValue;

            if (worldMapFile == "") return null;

            //Texture2D map = Game1.content.Load<Texture2D>("LooseSprites\\map");
            Texture2D map = Helper.GameContent.Load<Texture2D>("LooseSprites\\map");
            switch (worldMapFile.ToLower()) {
                case "standard":
                    return UtilityMisc.createSubTexture(map, new Rectangle(0, 43, 131, 61));
                case "riverland":
                    return UtilityMisc.createSubTexture(map, new Rectangle(0, 180, 131, 61));
                case "forest":
                    return UtilityMisc.createSubTexture(map, new Rectangle(131, 180, 131, 61));
                case "hills":
                    return UtilityMisc.createSubTexture(map, new Rectangle(0, 241, 131, 61));
                case "wilderness":
                    return UtilityMisc.createSubTexture(map, new Rectangle(131, 241, 131, 61));
                case "four corners":
                    return UtilityMisc.createSubTexture(map, new Rectangle(0, 302, 131, 61));
                case "beach":
                    return UtilityMisc.createSubTexture(map, new Rectangle(131, 302, 131, 61));
            }

            try {
                return Helper.ModContent.Load<Texture2D>($"{path}\\{worldMapFile}");

            } catch (Exception ex) {
                Monitor.LogOnce($"Unable to load the world map texture in:\n{path}\\{worldMapFile}", LogLevel.Warn);
            }

            WorldMapOverlayValue = "";
            return null;
        }

        public static CustomFarm getCurrentCustomFarm()
        {
            if (CurrentCustomFarmId != Game1.whichModFarm.ID) {
                CurrentCustomFarmId = Game1.whichModFarm.ID;
                CurrentCustomFarm = get(Game1.whichModFarm.ID);
            }

            return CurrentCustomFarm;
        }

        public ModFarmType asModFarmType()
        {
            if (!IsCFLMap)
                return ModFarmType;

            return new ModFarmType() {
                ID = $"{this.UniqueModID}/{this.Name}",
                MapName = $"CFL_Map/{this.ID}",
                TooltipStringPath = $"Strings/UI:CFL_Description/{this.ID}",
                IconTexture = $"CFL_Icon/{this.ID}",
                WorldMapTexture = $"CFL_WorldMap/{this.ID}",
                ModData = new Dictionary<string, string>()
            };
        }

        public static CustomFarm get(string id, bool isCFLMap = true)
        {
            if (!LoadedCachedCustomFarms) getAll();

            IEnumerable<CustomFarm> maps = CachedCustomFarms.FindAll(el => el.ID == id && el.IsCFLMap == isCFLMap);

            if (maps.Count() > 1) {
                Monitor.Log($"(!) Multiple Custom Farms with the same ID found: '{id}'", LogLevel.Error);
                Monitor.Log($"Please make sure Custom Farm IDs are unique", LogLevel.Error);

                foreach (CustomFarm map in maps) {
                    Monitor.Log($"{map.ContentPackDirectory}\\{map.RelativeMapDirectoryPath}", LogLevel.Warn);
                }
            } else if (maps.Count() == 1)
                return maps.First();
            else
                Monitor.LogOnce($"Failed to load Custom Farm with the id '{id}'", LogLevel.Trace);
            return null;
        }

        public static List<CustomFarm> getAll()
        {
            if (CachedCustomFarms.Count > 0)
                return CachedCustomFarms;

            var mods = Helper.ModRegistry.GetAll();

            CachedCustomFarms = new List<CustomFarm>();
            CachedModFarmTypes = new List<ModFarmType>();

            foreach (IModInfo mod in mods) {
                string directoryPath = (string)HarmonyLib.AccessTools.GetDeclaredFields(mod.GetType()).Find(e => e.Name.Contains("DirectoryPath")).GetValue(mod);

                CachedCustomFarms.AddRange(getAllFromModFolder(directoryPath, mod.Manifest));
            }

            CachedCustomFarms = CachedCustomFarms.OrderBy(o => o.Name).ToList();
            LoadedCachedCustomFarms = true;

            return CachedCustomFarms;
        }

        public static List<ModFarmType> getAllAsModFarmType()
        {
            if (CachedModFarmTypes.Count() > 0)
                return CachedModFarmTypes;

            getAll().ForEach(el => CachedModFarmTypes.Add(el.asModFarmType()));

            return CachedModFarmTypes;
        }

        public static List<CustomFarm> getAllFromModFolder(string modFolder, IManifest manifest)
        {
            List<CustomFarm> ret = new List<CustomFarm>();
            IEnumerable<string> maps = Directory.EnumerateFiles(modFolder, "cfl_map.json", new EnumerationOptions() { RecurseSubdirectories = true }).AsParallel();

            foreach (string mapJson in maps) {
                CustomFarm customFarm = parseMapJson(mapJson, modFolder, manifest);
                if (customFarm != null)
                    ret.Add(customFarm);
            }

            return ret;
        }

        public static bool IsCFLMapSelected()
        {
            return Game1.whichFarm == 7
                && getCurrentCustomFarm() != null
                && getCurrentCustomFarm().IsCFLMap;
        }

        public string getLocalizedDescription()
        {
            var description = Game1.content.LoadString(asModFarmType().TooltipStringPath);

            if (IsCFLMap)
                return description;
            else
                return description.Substring(description.IndexOf('_') + 1);

        }
    }
}
