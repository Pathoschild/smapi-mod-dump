/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Videogamers0/SDV-ItemBags
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ItemBags.Helpers
{
    public static class Extensions
    {
        //Taken from: https://stackoverflow.com/questions/45426266/get-description-attributes-from-a-flagged-enum
        /// <summary>Returns the description attribute of the Enum value</summary>
        public static string GetDescription(this Enum value)
        {
            Type type = value.GetType();
            string name = Enum.GetName(type, value);
            if (name != null)
            {
                System.Reflection.FieldInfo field = type.GetField(name);
                if (field != null)
                {
                    DescriptionAttribute attr = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;
                    if (attr != null)
                    {
                        return attr.Description;
                    }
                }
            }
            return null;
        }

        public static Rectangle GetOffseted(this Rectangle value, Point Offset)
        {
            return new Rectangle(value.X + Offset.X, value.Y + Offset.Y, value.Width, value.Height);
        }

        public static Point AsPoint(this Vector2 value)
        {
            return new Point((int)value.X, (int)value.Y);
        }

        /// <summary>Intended to be used with <see cref="ICursorPosition.ScreenPixels"/></summary>
        /// <param name="value">The cursor position as a Point, that will be multiplied by the Game's zoomLevel if on Android.</param>
        public static Point AsAndroidCompatibleCursorPoint(this Vector2 value)
        {
            if (Constants.TargetPlatform == GamePlatform.Android)
            {
                return new Point((int)(Game1.options.zoomLevel * value.X), (int)(Game1.options.zoomLevel * value.Y));
            }
            else
            {
                return new Point((int)value.X, (int)value.Y);
            }
        }

        public static double SquaredDistanceBetweenCenters(this Rectangle value, Rectangle other)
        {
            Point c1 = value.Center;
            Point c2 = other.Center;

            double XDifference = c1.X - c2.X;
            double YDifference = c1.Y - c2.Y;

            return XDifference * XDifference + YDifference * YDifference;
        }

        public static bool IsLegacyCursorPosition { get { return !Constants.ApiVersion.IsNewerThan("3.8.1"); } }

        /// <summary>Returns <see cref="ICursorPosition.ScreenPixels"/>, always adjusted for UI Scaling. SMAPI version 3.8.1 and earlier used to always do this, but changes were made to SMAPI 3.8.2+.</summary>
        public static Vector2 LegacyScreenPixels(this ICursorPosition value)
        {
            if (IsLegacyCursorPosition)
                return value.ScreenPixels;
            else
                return Utility.ModifyCoordinatesForUIScale(value.ScreenPixels);
        }

        #region Split List
        //Taken from: https://stackoverflow.com/questions/3514740/how-to-split-an-array-into-a-group-of-n-elements-each
        private static IEnumerable<TList> Split<TList, T>(this TList value, int countOfEachPart) where TList : IEnumerable<T>
        {
            int cnt = value.Count() / countOfEachPart;
            List<IEnumerable<T>> result = new List<IEnumerable<T>>();
            for (int i = 0; i <= cnt; i++)
            {
                IEnumerable<T> newPart = value.Skip(i * countOfEachPart).Take(countOfEachPart).ToArray();
                if (newPart.Any())
                    result.Add(newPart);
                else
                    break;
            }

            return result.Cast<TList>();
        }

        public static IEnumerable<IDictionary<TKey, TValue>> Split<TKey, TValue>(this IDictionary<TKey, TValue> value, int countOfEachPart)
        {
            IEnumerable<Dictionary<TKey, TValue>> result = value.ToArray()
                                                                .Split(countOfEachPart)
                                                                .Select(p => p.ToDictionary(k => k.Key, v => v.Value));
            return result;
        }

        public static IEnumerable<IList<T>> Split<T>(this IList<T> value, int countOfEachPart)
        {
            return value.Split<IList<T>, T>(countOfEachPart);
        }

        public static IEnumerable<T[]> Split<T>(this T[] value, int countOfEachPart)
        {
            return value.Split<T[], T>(countOfEachPart);
        }

        public static IEnumerable<IEnumerable<T>> Split<T>(this IEnumerable<T> value, int countOfEachPart)
        {
            return value.Split<IEnumerable<T>, T>(countOfEachPart);
        }
        #endregion Split List
    }
}
