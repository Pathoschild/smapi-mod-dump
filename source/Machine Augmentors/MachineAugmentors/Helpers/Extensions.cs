/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Videogamers0/SDV-MachineAugmentors
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

namespace MachineAugmentors.Helpers
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

        //Taken from: https://stackoverflow.com/questions/521146/c-sharp-split-string-but-keep-split-chars-separators
        public static IEnumerable<string> SplitAndKeepDelimiter(this string s, char[] delims)
        {
            int start = 0, index;

            while ((index = s.IndexOfAny(delims, start)) != -1)
            {
                if (index - start > 0)
                    yield return s.Substring(start, index - start);
                yield return s.Substring(index, 1);
                start = index + 1;
            }

            if (start < s.Length)
            {
                yield return s.Substring(start);
            }
        }
    }
}
