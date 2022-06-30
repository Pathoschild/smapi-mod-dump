/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tlitookilakin/WarpNetwork
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Text;
using WarpNetwork.api;
using WarpNetwork.models;
using xTile;

namespace WarpNetwork
{
    static class Utils
    {
        public static Dictionary<string, IWarpNetHandler> CustomLocs = new(StringComparer.OrdinalIgnoreCase);
        private static readonly string[] VanillaMapNames =
        {
            "Farm","Farm_Fishing","Farm_Foraging","Farm_Mining","Farm_Combat","Farm_FourCorners","Farm_Island"
        };
        private static readonly Dictionary<string, int> FarmTypeMap = new()
        {
            { "farm", 0 },
            { "farm_fishing", 1 },
            { "farm_foraging", 2 },
            { "farm_mining", 3 },
            { "farm_combat", 4 },
            { "farm_fourcorners", 5 },
            { "farm_island", 6 }
        };
        public static Point GetActualFarmPoint(int default_x, int default_y)
        {
            return GetActualFarmPoint(Game1.getFarm().Map, default_x, default_y);
        }
        public static Point GetActualFarmPoint(Map map, int default_x, int default_y, string filename = null)
        {
            int x = default_x;
            int y = default_y;
            switch (GetFarmType(filename))
            {
                //four corners
                case 5:
                    x = 48;
                    y = 39;
                    break;
                //beach
                case 6:
                    x = 82;
                    y = 29;
                    break;
            }
            return GetMapPropertyPosition(map, "WarpTotemEntry", x, y);
        }
        public static string GetFarmMapPath()
        {
            if (Game1.whichFarm < 0)
            {
                ModEntry.monitor.Log("Something is wrong! Game1.whichfarm does not contain a valid value!", LogLevel.Warn);
                return "";

            }
            else if (Game1.whichFarm < 7)
            {
                return VanillaMapNames[Game1.whichFarm];

            }
            else if (Game1.whichModFarm == null)
            {
                ModEntry.monitor.Log("Something is wrong! Custom farm indicated, but Game1.whichModFarm is null!", LogLevel.Warn);
                return "";
            }
            else
            {
                return Game1.whichModFarm.MapName;
            }
        }
        public static int GetFarmType(string filename)
        {
            if (filename is null)
            {
                return Game1.whichFarm;
            }
            if (FarmTypeMap.ContainsKey(filename))
            {
                return FarmTypeMap[filename];
            }
            return 0;
        }
        public static Point GetMapPropertyPosition(Map map, string property, int default_x, int default_y)
        {
            if (!map.Properties.ContainsKey(property))
            {
                return new Point(default_x, default_y);
            }
            string prop = map.Properties[property];
            string[] args = prop.Split(' ');
            if (args.Length < 2)
            {
                return new Point(default_x, default_y);
            }
            return new Point(int.Parse(args[0]), int.Parse(args[1]));
        }
        public static Dictionary<string, WarpLocation> GetWarpLocations()
        {
            Dictionary<string, WarpLocation> data = ModEntry.helper.GameContent.Load<Dictionary<string, WarpLocation>>(ModEntry.pathLocData);
            Dictionary<string, WarpLocation> ret = new(data, StringComparer.OrdinalIgnoreCase);
            foreach ((string key, IWarpNetHandler value) in CustomLocs)
            {
                if (ret.ContainsKey(key))
                {
                    ModEntry.monitor.Log("Overwriting destination '" + key + "' with custom handler", LogLevel.Debug);
                    ret[key] = new CustomWarpLocation(value);
                }
                else
                {
                    ret.Add(key, new CustomWarpLocation(value));
                }
            }
            return ret;
        }
        public static Dictionary<string, WarpItem> GetWarpItems()
        {
            return ModEntry.helper.GameContent.Load<Dictionary<string, WarpItem>>(ModEntry.pathItemData);
        }
        public static T ParseEnum<T>(string str)
        {
            Type type = typeof(T);
            try
            {
                T val = (T)Enum.Parse(type, str);
                if (Enum.IsDefined(type, val))
                {
                    return val;
                }
                else
                {
                    return default;
                }
            }
            catch (ArgumentException)
            {
                return default;
            }
        }
        public static Color ParseColor(string str)
        {
            if (str.Length == 0)
            {
                ModEntry.monitor.Log("Could not parse color from string: '" + str + "'.", LogLevel.Warn);
                return Color.Transparent;
            }
            if (str[0] == '#')
            {
                if (str.Length <= 6)
                {
                    ModEntry.monitor.Log("Could not parse color from string: '" + str + "'.", LogLevel.Warn);
                    return Color.Transparent;
                }
                int r = Convert.ToInt32(str.Substring(1, 2), 16);
                int g = Convert.ToInt32(str.Substring(3, 2), 16);
                int b = Convert.ToInt32(str.Substring(5, 2), 16);
                if (str.Length > 8)
                {
                    int a = Convert.ToInt32(str.Substring(7, 2), 16);
                    return new Color(r, g, b, a);
                }
                return new Color(r, g, b);
            }
            else
            {
                string[] vals = str.Replace(" ", "").Split(',');
                if (vals.Length > 2)
                {
                    if (vals.Length > 3)
                    {
                        return new Color(int.Parse(vals[0]), int.Parse(vals[1]), int.Parse(vals[2]), int.Parse(vals[3]));
                    }
                    return new Color(int.Parse(vals[0]), int.Parse(vals[1]), int.Parse(vals[2]));
                }
                ModEntry.monitor.Log("Could not parse color from string: '" + str + "'.", LogLevel.Warn);
                return Color.Transparent;
            }
        }
        public static bool IsFestivalAtLocation(string Location)
        {
            return !(Location is null) && Game1.weatherIcon == 1 && Game1.whereIsTodaysFest.ToLower() == Location.ToLower();
        }
        public static bool IsFestivalReady()
        {
            if (Game1.weatherIcon != 1)
            {
                return false;
            }
            return Convert.ToInt32(ModEntry.helper.GameContent.Load<Dictionary<string, string>>("Data/Festivals/" + Game1.currentSeason + Game1.dayOfMonth)["conditions"].Split('/')[1].Split(' ')[0]) <= Game1.timeOfDay;
        }
        public static bool LocationExists(string name)
        {
            return GetWarpLocations().ContainsKey(name);
        }
        public static bool IsLocationEnabled(string name)
        {
            Dictionary<string, WarpLocation> dests = GetWarpLocations();
            return dests.ContainsKey(name) && dests[name].Enabled;
        }
        public static string IterableToString(IEnumerable<object> iter)
        {
            StringBuilder sb = new();
            sb.Append("[");
            foreach (object item in iter)
            {
                sb.Append(item.ToString());
                sb.Append(", ");
            }
            sb.Append("]");
            return sb.ToString();
        }
        public static void reduceItemCount(Farmer who, Item what, int count)
        {
            if (what == null)
                return;
            what.Stack -= count;
            if (what != null && what.Stack <= 0)
            {
                if (who != null)
                {
                    if (what == who.CurrentItem)
                    {
                        who.showNotCarrying();
                    }
                    who.removeItemFromInventory(what);
                }
            }
        }
        //Used to get DGA item #
        public static int GetDeterministicHashCode(string str)
        {
            unchecked
            {
                int hash1 = (5381 << 16) + 5381;
                int hash2 = hash1;

                for (int i = 0; i < str.Length; i += 2)
                {
                    hash1 = ((hash1 << 5) + hash1) ^ str[i];
                    if (i == str.Length - 1)
                        break;
                    hash2 = ((hash2 << 5) + hash2) ^ str[i + 1];
                }

                return hash1 + (hash2 * 1566083941);
            }
        }
        public static string WithoutPath(this string path, string prefix)
            => PathUtilities.GetSegments(path, PathUtilities.GetSegments(prefix).Length + 1)[^1];
    }
}
