/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Tlitookilakin/AeroCore
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewValley;
using System;
using System.Collections.Generic;
using DColor = System.Drawing.Color;
using System.Linq;
using System.Text;
using System.Globalization;
using StardewModdingAPI.Utilities;
using StardewModdingAPI;
using SObject = StardewValley.Object;
using StardewValley.Objects;
using StardewValley.Tools;
using StardewValley.TerrainFeatures;

namespace AeroCore.Utils
{
    public static class Strings
    {
        internal static string[] ObjectPrefixes = 
            { "(O)", "(BC)", "(F)", "(B)", "(FL)", "(WP)", "(W)", "(H)", "(S)", "(P)"}; //tool creation not supported

        public static bool ToPoint(this string[] strings, out Point point, int offset = 0)
        {
            if (offset + 1 >= strings.Length)
            {
                point = new();
                return false;
            }
            return ToPoint(strings[offset], strings[offset + 1], out point);
        }
        public static bool ToPoint(string x, string y, out Point point)
        {
            if (int.TryParse(x, out int xx) && int.TryParse(y, out int yy))
            {
                point = new(xx, yy);
                return true;
            }
            point = new();
            return false;
        }
        public static bool ToVector2(this string[] strings, out Vector2 vec, int offset = 0)
        {
            if (offset + 1 >= strings.Length)
            {
                vec = new();
                return false;
            }
            return ToVector2(strings[offset], strings[offset + 1], out vec);
        }
        public static bool ToVector2(string x, string y, out Vector2 vec)
        {
            if (float.TryParse(x, out float xx) && float.TryParse(y, out float yy))
            {
                vec = new(xx, yy);
                return true;
            }
            vec = new();
            return false;
        }
        public static bool ToRect(this string[] strings, out Rectangle rect, int offset = 0)
        {
            if (offset + 3 >= strings.Length)
            {
                rect = new();
                return false;
            }
            return ToRect(strings[offset], strings[offset + 1], strings[offset + 2], strings[offset + 3], out rect);
        }
        public static bool ToRect(string x, string y, string w, string h, out Rectangle rect)
        {
            if (int.TryParse(x, out int xx) && 
                int.TryParse(y, out int yy) && 
                int.TryParse(w, out int ww) && 
                int.TryParse(h, out int hh))
            {
                rect = new(xx, yy, ww, hh);
                return true;
            }
            rect = new();
            return false;
        }
        public static bool FromCorners(this string[] strings, out Rectangle rect, int offset = 0)
        {
            if (offset + 3 >= strings.Length)
            {
                rect = new();
                return false;
            }
            return FromCorners(strings[offset], strings[offset + 1], strings[offset + 2], strings[offset + 3], out rect);
        }
        public static bool FromCorners(string x1, string y1, string x2, string y2, out Rectangle rect)
        {
            if (int.TryParse(x1, out int ax) &&
                int.TryParse(y1, out int ay) &&
                int.TryParse(x2, out int bx) &&
                int.TryParse(y2, out int by))
            {
                rect = new(Math.Min(ax, bx), Math.Min(ax, bx), Math.Abs(ax - bx + 1), Math.Abs(ay - by + 1));
                return true;
            }
            rect = new();
            return false;
        }
        public static string GetChunk(this string str, char delim, int which)
        {
            int i = 0;
            int n = 0;
            int z = 0;
            while(i < str.Length)
            {
                if (str[i] == delim)
                {
                    if (n == which)
                        return str[z..i];
                    n++;
                    z = i + 1;
                }
                i++;
            }
            if (n == which)
                return str[z..i];
            return "";
        }
        public static string ContentsToString(this IEnumerable<object> iter, string separator = ", ")
        {
            StringBuilder sb = new();
            foreach (object item in iter)
            {
                sb.Append(item.ToString());
                sb.Append(separator);
            }
            return sb.ToString();
        }
        public static string WithoutPath(this IAssetName name, string path)
        {
            if (!name.StartsWith(path, false))
                return null;

            if (name.IsEquivalentTo(path))
                return string.Empty;

            int count = PathUtilities.GetSegments(path).Length;
            return string.Join(PathUtilities.PreferredAssetSeparator, PathUtilities.GetSegments(name.ToString())[count..]);
        }
        public static bool TryGetItem(this string str, out Item item, Color? color = null)
		{
            int i = 0;
			str = str.Trim();
            item = ModEntry.DGA?.SpawnDGAItem(str, color) as Item;
            var isDGAItem = item is not null;
            // JA does not use namespaced ids, and internal id format may change in 1.6
			// so removing direct access for now
            if (item is null)
            {
                while (i < ObjectPrefixes.Length)
                    if (str.StartsWith(ObjectPrefixes[i]))
                        break;
                    else
                        i++;
                int clip = 0;
                if (i >= ObjectPrefixes.Length)
                    i = 0;
                else
                    clip = ObjectPrefixes[i].Length;
                if (!int.TryParse(str[clip..], out int id))
                    return false;
                item = i switch
                {
                    0 => new SObject(id, 1),
                    1 => new SObject(Vector2.Zero, id),
                    2 => new Furniture(id, Vector2.Zero),
                    3 => new Boots(id),
                    4 => new Wallpaper(id, true),
                    5 => new Wallpaper(id, false),
                    6 => new MeleeWeapon(id),
                    7 => new Hat(id),
                    8 or 9 => new Clothing(id),
                    _ => null
                };
            }
            if (item is null)
                return false;
			item.modData["tlitoo.aerocore.preservedID"] = str;
            if (isDGAItem)
                item.modData["tlitoo.aerocore.IsDGAItem"] = "T";
			return true;
        }
        public static bool TryGetFruitTree(this string id, out FruitTree tree, int stage = -1)
        {
            tree = null;
            int ind;
            if ((ind = ModEntry.JA?.GetFruitTreeId(id) ?? -1) == -1 && !int.TryParse(id, out ind))
                return false;
            tree = stage >= 0 ? new(ind, stage) : new(ind);
            return true;
        }
        public static bool TryGetCrop(this string id, int x, int y, out Crop crop)
        {
            crop = null;
            int ind;
            if ((ind = ModEntry.JA?.GetCropId(id) ?? -1) == -1 && !int.TryParse(id, out ind))
                return false;
            crop = new(ind, x, y);
            return true;
        }
        public static int GetDeterministicHashCode(this string value)
        {
			int count = value.Length;
			int hash1 = 352654597;
			int hash2 = hash1;
			int i;
			for (i = 0; i < count; i++)
			{
				int c = value[i];
				hash1 = ((hash1 << 5) + hash1) ^ c;
				if (++i >= count)
				{
					break;
				}
				c = value[i];
				hash2 = ((hash2 << 5) + hash2) ^ c;
			}
			return hash1 + hash2 * 1566083941;
		}
        /// <returns>A copy of the string with all whitespace stripped</returns>
        public static string Collapse(this string str)
        {
            var s = str.AsSpan();
            var r = new Span<char>(new char[s.Length]);
            int len = 0;
            int last = 0;
            for (int i = 0; i < s.Length; i++)
            {
                if (!char.IsWhiteSpace(s[i]))
                    continue;

                if (i - last <= 1)
                {
                    last = i + 1;
                    continue;
                }

                s[last..i].CopyTo(r[len..]);
                len += i - last;
                last = i + 1;
            }
            if (last < s.Length)
            {
                s[last..].CopyTo(r[len..]);
                len += s.Length - last;
            }
            return new string(r[..len]);
        }
        public static IList<string> SafeSplit(this ReadOnlySpan<char> s, char delim, bool RemoveEmpty = false)
        {
            List<string> result = new();
            bool dquote = false;
            bool squote = false;
            bool escaped = false;
            char c;
            int last = 0;
            var prev = new char[s.Length];
            int skip = 0;
            int skipped = 0;
            for(int i = 0; i < s.Length; i++)
            {
                if (escaped)
                {
                    escaped = false;
                    continue;
                }
                c = s[i];
                switch (c)
                {
                    case '"':
                        if (!squote)
                        {
                            dquote = !dquote;
                            s[skip..i].CopyTo(prev.AsSpan(skip - skipped));
                            skipped++;
                            skip = i + 1;
                        }
                        break;
                    case '\'':
                        if (!dquote)
                        {
                            squote = !squote;
                            s[skip..i].CopyTo(prev.AsSpan(skip - skipped));
                            skipped++;
                            skip = i + 1;
                        }
                        break;
                    case '\\':
                        escaped = true;
                        s[skip..i].CopyTo(prev.AsSpan(skip - skipped));
                        skipped++;
                        skip = i + 1;
                        break;
                    default:
                        if (c == delim && !dquote && !squote)
                        {
                            s[skip..i].CopyTo(prev.AsSpan(skip - skipped));
                            if (!RemoveEmpty || i - last - skipped > 0)
                                result.Add(new string(prev[last..(i - skipped)]));
                            last = i + 1;
                            skip = last;
                            skipped = 0;
                        }
                        break;
                }
            }
            s[skip..].CopyTo(prev.AsSpan(skip - skipped));
            if (s.Length - last - skipped > 0)
                result.Add(new string(prev[last..^skipped]));
            return result;
        }
        public static IList<string> SafeSplit(this string s, char delim, bool RemoveEmpty = false) => s.AsSpan().SafeSplit(delim, RemoveEmpty);

        /// <summary>Parses a color from a string. Valid formats: #rgb #rgba #rrggbb #rrggbbaa r,g,b r,g,b,a</summary>
        /// <param name="str">The string to parse from</param>
        /// <param name="color">The color parsed, if successful</param>
        /// <returns>Whether or not a color could be parsed from the string</returns>
        public static bool TryParseColor(this string str, out Color color)
        {
            color = Color.Transparent;

            if (str is null || str.Length == 0)
                return false;

            DColor c = DColor.FromName(str);
            if (c.ToArgb() != 0)
            {
                color = new(c.R, c.G, c.B, c.A);
                return true;
            }

            ReadOnlySpan<char> s = str.AsSpan();
            if (s[0] == '#')
            {
                if (s.Length <= 3)
                    return false;

                if (s.Length > 6)
                {
                    if (int.TryParse(s[1..3], NumberStyles.HexNumber, null, out int r) &&
                        int.TryParse(s[3..5], NumberStyles.HexNumber, null, out int g) &&
                        int.TryParse(s[5..7], NumberStyles.HexNumber, null, out int b))
                    {
                        if (s.Length > 8 && int.TryParse(s[7..9], NumberStyles.HexNumber, null, out int a))
                            color = new(r, g, b, a);
                        else
                            color = new(r, g, b);
                        return true;
                    }
                } else
                {
                    if (int.TryParse($"{s[1]}{s[1]}", NumberStyles.HexNumber, null, out int r) &&
                        int.TryParse($"{s[2]}{s[2]}", NumberStyles.HexNumber, null, out int g) &&
                        int.TryParse($"{s[3]}{s[3]}", NumberStyles.HexNumber, null, out int b))
                    {
                        if (s.Length > 4 && int.TryParse($"{s[4]}{s[4]}", NumberStyles.HexNumber, null, out int a))
                            color = new(r, g, b, a);
                        else
                            color = new(r, g, b);
                        return true;
                    }
                }
            }
            else
            {
                string[] vals = str.Split(',', StringSplitOptions.RemoveEmptyEntries);
                if (vals.Length > 2)
                {
                    if(int.TryParse(vals[0], out int r) && 
                        int.TryParse(vals[1], out int g) && 
                        int.TryParse(vals[2], out int b))
                    {
                        if(vals.Length > 3 && int.TryParse(vals[3], out int a))
                            color = new Color(r, g, b, a);
                        else
                            color = new Color(r, g, b);
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
