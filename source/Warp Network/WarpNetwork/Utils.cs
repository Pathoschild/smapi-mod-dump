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
using StardewValley;
using System;
using System.Collections.Generic;
using System.Text;
using xTile;

namespace WarpNetwork
{
    class Utils
    {
        private static IModHelper Helper;
        private static IMonitor Monitor;
        private static readonly Dictionary<string, int> FarmTypeMap = new Dictionary<string, int>()
        {
            {"farm", 0},
            {"farm_fishing", 1},
            {"farm_foraging", 2},
            {"farm_mining", 3},
            {"farm_combat", 4},
            {"farm_fourcorners", 5},
            {"farm_island", 6}
        };

        internal static void Init(IModHelper helper, IMonitor monitor)
        {
            Helper = helper;
            Monitor = monitor;
        }
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
        public static int GetFarmType(string filename)
        {
            if(filename is null)
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
            if(!map.Properties.ContainsKey(property))
            {
                return new Point(default_x, default_y);
            }
            string prop = map.Properties[property];
            string[] args = prop.Split(' ');
            if(args.Length < 2)
            {
                return new Point(default_x, default_y);
            }
            return new Point(Int32.Parse(args[0]), Int32.Parse(args[1]));
        }
        public static Dictionary<string, WarpLocation> GetWarpLocations()
        {
            Dictionary<string, WarpLocation> data = Helper.Content.Load<Dictionary<string, WarpLocation>>(ModEntry.pathLocData, ContentSource.GameContent);
            return new Dictionary<string, WarpLocation>(data, StringComparer.OrdinalIgnoreCase);
        }
        public static Dictionary<string, WarpItem> GetWarpItems()
        {
            return Helper.Content.Load<Dictionary<string, WarpItem>>(ModEntry.pathItemData, ContentSource.GameContent);
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
                } else
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
            if(str.Length == 0)
            {
                Monitor.Log("Could not parse color from string: '" + str + "'.", LogLevel.Warn);
                return Color.Transparent;
            }
            if(str[0] == '#')
            {
                if(str.Length <= 6)
                {
                    Monitor.Log("Could not parse color from string: '" + str + "'.", LogLevel.Warn);
                    return Color.Transparent;
                }
                int r = Convert.ToInt32(str.Substring(1, 2), 16);
                int g = Convert.ToInt32(str.Substring(3, 2), 16);
                int b = Convert.ToInt32(str.Substring(5, 2), 16);
                if(str.Length > 8)
                {
                    int a = Convert.ToInt32(str.Substring(7, 2), 16);
                    return new Color(r, g, b, a);
                }
                return new Color(r, g, b);
            } else
            {
                string[] vals = str.Replace(" ", "").Split(',');
                if(vals.Length > 2)
                {
                    if(vals.Length > 3)
                    {
                        return new Color(Int32.Parse(vals[0]), Int32.Parse(vals[1]), Int32.Parse(vals[2]), Int32.Parse(vals[3]));
                    }
                    return new Color(Int32.Parse(vals[0]), Int32.Parse(vals[1]), Int32.Parse(vals[2]));
                }
                Monitor.Log("Could not parse color from string: '" + str + "'.", LogLevel.Warn);
                return Color.Transparent;
            }
        }
        public static bool IsFestivalAtLocation(string Location)
        {
            return !(Location is null) && Game1.weatherIcon == 1 && Game1.whereIsTodaysFest.ToLower() == Location.ToLower();
        }
        public static bool IsFestivalReady()
        {
            if(Game1.weatherIcon != 1)
            {
                return false;
            }
            return Convert.ToInt32(Helper.Content.Load<Dictionary<string, string>>("Data/Festivals/" + Game1.currentSeason + Game1.dayOfMonth, ContentSource.GameContent)["conditions"].Split('/')[1].Split(' ')[0]) <= Game1.timeOfDay;
        }
        public static bool LocationExists(string name)
        {
            return WarpHandler.CustomLocs.ContainsKey(name) || GetWarpLocations().ContainsKey(name);
        }
        public static bool IsLocationEnabled(string name)
        {
            Dictionary<string, WarpLocation> dests = GetWarpLocations();
            return WarpHandler.CustomLocs.ContainsKey(name) ? WarpHandler.CustomLocs[name].GetEnabled(name) : dests.ContainsKey(name) && dests[name].Enabled;
        }
        public static string IterableToString(IEnumerable<object> iter)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("[");
            foreach (object item in iter)
            {
                sb.Append(item.ToString());
                sb.Append(", ");
            }
            sb.Append("]");
            return sb.ToString();
        }
    }
}
