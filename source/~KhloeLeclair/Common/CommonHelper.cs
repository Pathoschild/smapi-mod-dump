/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/KhloeLeclair/StardewMods
**
*************************************************/

using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Microsoft.Xna.Framework;

using StardewValley.Menus;


namespace Leclair.Stardew.Common {
	public static class CommonHelper {

		#region Color Parsing

		public static readonly Regex HEX_REGEX = new(@"^#([0-9a-f]{3,4}|(?:[0-9a-f]{2}){3,4})$", RegexOptions.IgnoreCase);
		public static readonly Regex RGB_REGEX = new(@"^\s*rgba?\s*\(\s*(\d+%?)\s*,\s*(\d+%?)\s*,\s*(\d+%?)(?:\s*,\s*(\d+%|[\d.]+))?\s*\)\s*$", RegexOptions.IgnoreCase);

		private static int HydrateRGBValue(string value) {
			if (value.EndsWith('%'))
				return (int) Math.Floor(Convert.ToInt32(value[0..^1]) / 100f * 255);

			return Convert.ToInt32(value);
		}

		public static Color? ParseColor(string input) {
			if (string.IsNullOrEmpty(input))
				return null;

			int r = -1;
			int g = -1;
			int b = -1;
			int a = 255;

			// CSS hex format is:
			//   #RGB
			//   #RGBA
			//   #RRGGBB
			//   #RRGGBBAA
			// ColorTranslator.FromHtml does this wrong, so let's do it ourselves.
			var match = HEX_REGEX.Match(input);
			if (match.Success) {
				string value = match.Groups[1].Value;
				if (value.Length == 3 || value.Length == 4) {
					r = Convert.ToInt32(value[0..1], 16) * 17;
					g = Convert.ToInt32(value[1..2], 16) * 17;
					b = Convert.ToInt32(value[2..3], 16) * 17;

					if (value.Length == 4)
						a = Convert.ToInt32(value[3..4], 16) * 17;
				}

				if (value.Length == 6 || value.Length == 8) {
					r = Convert.ToInt32(value[0..2], 16);
					g = Convert.ToInt32(value[2..4], 16);
					b = Convert.ToInt32(value[4..6], 16);

					if (value.Length == 8)
						a = Convert.ToInt32(value[6..8], 16);
				}
			}

			// CSS rgb format
			match = RGB_REGEX.Match(input);
			if (match.Success) {
				r = HydrateRGBValue(match.Groups[1].Value);
				g = HydrateRGBValue(match.Groups[2].Value);
				b = HydrateRGBValue(match.Groups[3].Value);

				if (match.Groups[4].Success) {
					string value = match.Groups[4].Value;
					if (value.EndsWith('%'))
						a = HydrateRGBValue(value);
					else {
						float fval = Convert.ToSingle(value);
						if (fval >= 0)
							a = (int) Math.Floor(255 * fval);
					}
				}
			}

			if (r != -1 && g != -1 && b != -1) {
				if (r >= 0 && r <= 255 && g >= 0 && g <= 255 && b >= 0 && b <= 255 && a >= 0 && a <= 255)
					return new Color(r, g, b, a);
				return null;
			}

			// Fall back on ColorTranslator for handling color names so we don't need
			// to include our own lookup table.
			System.Drawing.Color color;
			try {
				color = System.Drawing.ColorTranslator.FromHtml(input);
			} catch (Exception) {
				return null;
			}

			if (color == System.Drawing.Color.Empty)
				return null;

			return new Color(color.R, color.G, color.B, color.A);
		}

		#endregion

		public static T Clamp<T>(T value, T min, T max) where T : IComparable<T> {
			if (value.CompareTo(min) < 0) return min;
			if (value.CompareTo(max) > 0) return max;
			return value;
		}

		public static T Cycle<T>(T current, int direction = 1) {
			var values = Enum.GetValues(typeof(T)).Cast<T>().ToArray();

			int idx = -1;

			for (int i = 0; i < values.Length; i++) {
				if (current == null || current.Equals(values[i])) {
					idx = i + direction;
					break;
				}
			}

			if (idx < 0)
				return values.Last();

			if (idx >= values.Length)
				return values[0];

			return values[idx];
		}

		public static IEnumerable<T> GetValues<T>() {
			return Enum.GetValues(typeof(T)).Cast<T>();
		}

		public static void YeetMenu(IClickableMenu menu) {
			if (menu == null) return;

			MethodInfo CleanupMethod = menu.GetType().GetMethod("cleanupBeforeExit", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			menu.behaviorBeforeCleanup?.Invoke(menu);

			if (CleanupMethod != null && CleanupMethod.GetParameters().Length == 0)
				CleanupMethod.Invoke(menu, null);

			if (menu.exitFunction != null) {
				IClickableMenu.onExit exitFunction = menu.exitFunction;
				menu.exitFunction = null;
				exitFunction();
			}
		}

	}
}
