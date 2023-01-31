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
using Microsoft.Xna.Framework.Graphics;
using System.IO;

namespace Custom_Farm_Loader.Lib
{
    public class UtilityMisc
    {

        public static bool isInArea(Vector2 areaBegin, Vector2 areaEnd, Vector2 point)
        {
            return areaBegin.X <= point.X && point.X <= areaEnd.X
                && areaBegin.Y <= point.Y && point.Y <= areaEnd.Y;
        }

        public static bool isInArea(Point areaBegin, Point areaEnd, Point point)
        {
            return areaBegin.X <= point.X && point.X <= areaEnd.X
                && areaBegin.Y <= point.Y && point.Y <= areaEnd.Y;
        }

        public static IEnumerable<SomeType> PickSomeInRandomOrder<SomeType>(
            IEnumerable<SomeType> someTypes,
            int maxCount)
        {
            Dictionary<double, SomeType> randomSortTable = new Dictionary<double, SomeType>();

            foreach (SomeType someType in someTypes)
                randomSortTable[Game1.random.NextDouble()] = someType;

            return randomSortTable.OrderBy(KVP => KVP.Key).Take(maxCount).Select(KVP => KVP.Value);
        }

        public static Texture2D createSubTexture(Texture2D src, Rectangle rect)
        {
            Texture2D tex = new Texture2D(Game1.graphics.GraphicsDevice, rect.Width, rect.Height);
            int count = rect.Width * rect.Height;
            Color[] data = new Color[count];
            src.GetData(0, rect, data, 0, count);
            tex.SetData(data);
            return tex;
        }

        public static T parseEnum<T>(string value) where T : IConvertible
        {
            string enumComparableString = value.Replace(" ", "_");

            return (T)Convert.ChangeType(Enum.Parse(typeof(T), enumComparableString, true), typeof(T));
        }

        public static List<T> parseEnumArray<T>(JProperty property) where T : IConvertible
        {
            var items = new List<T>();

            foreach (JValue obj in property.First())
                items.Add(parseEnum<T>(obj.Value.ToString()));

            return items;
        }

        public static IEnumerable<T[]> Filter<T>(T[,] source, Func<T[], bool> predicate)
        {
            for (int i = 0; i < source.GetLength(0); ++i)
            {
                T[] values = new T[source.GetLength(1)];
                for (int j = 0; j < values.Length; ++j)
                {
                    values[j] = source[i, j];
                }
                if (predicate(values))
                {
                    yield return values;
                }
            }
        }

        public static Point parsePoint(string text)
        {
            return new Point(int.Parse(text.Split(",")[0]), int.Parse(text.Split(",")[1]));
        }

        public static Vector2 parseVector2(string text)
        {
            return new Vector2(int.Parse(text.Split(",")[0]), int.Parse(text.Split(",")[1]));
        }

        public static List<string> parseStringArray(JProperty property, bool toLower = false)
        {

            List<string> items = new List<string>();

            foreach (JValue obj in property.First()) {
                var val = obj.Value.ToString();
                if (toLower)
                    val = val.ToLower();
                items.Add(val);
            }


            return items;
        }

        public static string getSeasonString(int seasonNumber)
        {
            return seasonNumber switch {
                0 => "spring",
                1 => "summer",
                2 => "fall",
                3 or _ => "winter"
            };
        }

        public static string getRelativeModDirectory(string modId)
        {
            var helper = ModEntry.Mod.Helper;
            var mod = helper.ModRegistry.GetAll().ToList().Find(el => el.Manifest.UniqueID == modId);
            string directoryPath = (string)HarmonyLib.AccessTools.GetDeclaredFields(mod.GetType()).Find(e => e.Name.Contains("DirectoryPath")).GetValue(mod);
            string relativeModDirectory = Path.GetRelativePath(helper.DirectoryPath + Path.DirectorySeparatorChar, directoryPath + Path.DirectorySeparatorChar);
            return relativeModDirectory;
        }
    }
}
