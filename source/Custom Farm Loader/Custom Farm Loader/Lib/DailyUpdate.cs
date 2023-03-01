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
    public class DailyUpdate
    {
        private static Mod Mod;
        private static IMonitor Monitor;
        private static IModHelper Helper;

        public DailyUpdateType Type;
        public string Name;
        public SpringObjectID ItemID;
        public float Chance = 1;
        public int Attempts = int.MaxValue;
        public List<string> Items = new List<string>();
        public Filter Filter = new Filter();
        public Area Area = new Area();
        public List<BackgroundType> BackgroundTypes = new List<BackgroundType>();
        public Vector2 Position = new Vector2(0, 0);
        public GameLocation Location = null;

        public static void Initialize(Mod mod)
        {
            Mod = mod;
            Monitor = mod.Monitor;
            Helper = mod.Helper;
        }

        public static List<DailyUpdate> parseDailyUpdateJsonArray(JProperty dailyUpdateArray, IManifest manifest)
        {
            List<DailyUpdate> ret = new List<DailyUpdate>();
            int i = 0;

            foreach (JObject obj in dailyUpdateArray.First())
                ret.Add(parseDailyUpdateJObject(obj, i++, manifest));

            return ret;
        }

        private static DailyUpdate parseDailyUpdateJObject(JObject obj, int i, IManifest manifest)
        {
            DailyUpdate dailyUpdate = new DailyUpdate();
            dailyUpdate.Filter.Manifest = manifest;
            string name = "";

            try {
                foreach (JProperty property in obj.Properties()) {
                    if (property.Value.Type == JTokenType.Null)
                        continue;

                    name = property.Name;
                    string value = property.Value.ToString();

                    switch (name.ToLower()) {
                        case "type":
                            dailyUpdate.Type = UtilityMisc.parseEnum<DailyUpdateType>(value);
                            break;
                        case "name":
                            dailyUpdate.Name = name;
                            dailyUpdate.ItemID = UtilityMisc.parseEnum<SpringObjectID>(value);
                            break;
                        case "chance":
                            dailyUpdate.Chance = float.Parse(value);
                            break;
                        case "attempts":
                            dailyUpdate.Attempts = int.Parse(value);
                            break;
                        case "backgrounds":
                            dailyUpdate.BackgroundTypes = UtilityMisc.parseEnumArray<BackgroundType>(property);
                            break;
                        case "items":
                            dailyUpdate.Items = ItemObject.MapNameToParentsheetindex(UtilityMisc.parseStringArray(property));
                            break;
                        default:
                            if (dailyUpdate.Filter.parseAttribute(property))
                                break;
                            if (dailyUpdate.Area.parseAttribute(property))
                                break;
                            Monitor.Log("Unknown DailyUpdate Attribute", LogLevel.Error);
                            throw new ArgumentException($"Unknown DailyUpdate Attribute", name);
                    }
                }
            } catch (Exception ex) {
                Monitor.Log($"At DailyUpdates[{i}] -> '{name}'", LogLevel.Error);
                Monitor.Log(ex.Message, LogLevel.Trace);
                throw;
            }

            if (dailyUpdate.Type == DailyUpdateType.None)
                throw new ArgumentException("Missing DailyUpdate Type", "Type");

            dailyUpdate.Attempts = dailyUpdate.Attempts == 0 ? 1 : dailyUpdate.Attempts;

            return dailyUpdate;
        }

        public List<Vector2> validTiles()
        {
            List<Vector2> validTiles = new List<Vector2>();

            if (Area.Include.Count == 0)
                addValidTilesFromRectangle(new Rectangle(0, 0, int.MaxValue, int.MaxValue), validTiles);
            else
                foreach (Rectangle rect in Area.Include)
                    addValidTilesFromRectangle(rect, validTiles, true);

            return validTiles;
        }

        private void addValidTilesFromRectangle(Rectangle rect, List<Vector2> validTiles, bool checkExists = false)
        {
            var tiles = Location.Map.GetLayer("Back").Tiles.Array;

            int maxX = rect.X + rect.Width > tiles.GetLength(0) ? tiles.GetLength(0) : rect.X + rect.Width;
            int maxY = rect.Y + rect.Height > tiles.GetLength(1) ? tiles.GetLength(1) : rect.Y + rect.Height;

            for (int x = rect.X; x < maxX; x++)
                for (int y = rect.Y; y < maxY; y++) {
                    Vector2 v = new Vector2(x, y);

                    if (checkExists && validTiles.Exists(el => el.X == x && el.Y == y))
                        continue;

                    if (tiles[x, y] == null || !Area.isTileIncluded(v))
                        continue;

                    if (isValidTile(v))
                        validTiles.Add(v);
                }
        }

        private static List<string> test = new List<string>();
        public bool isValidTile(Vector2 v)
        {
            if (!Location.isTileLocationTotallyClearAndPlaceable(v))
                return false;

            if (Location.getTileIndexAt((int)v.X, (int)v.Y, "AlwaysFront") != -1)
                return false;

            string x = Location.doesTileHavePropertyNoNull((int)v.X, (int)v.Y, "BeachSpawn", "Back");
            if (!test.Exists(e => e == x))
                test.Add(x);

            if (BackgroundTypes.Contains(BackgroundType.Beach) && Location.doesTileHavePropertyNoNull((int)v.X, (int)v.Y, "BeachSpawn", "Back") != "")
                return true;

            if (!BackgroundTypes.Contains(BackgroundType.Any) && BackgroundTypes.Count != 0)
                return BackgroundTypes.Exists(bg => Location.doesTileHavePropertyNoNull((int)v.X, (int)v.Y, "Type", "Back").Equals(bg.ToString()));

            return true;
        }
    }
}
