/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/KhloeLeclair/StardewMods
**
*************************************************/

#nullable enable

using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Reflection;

using Leclair.Stardew.Common.Types;

using Microsoft.Xna.Framework;

using StardewValley;
using StardewValley.Locations;
using StardewValley.Menus;

using SColor = System.Drawing.Color;

namespace Leclair.Stardew.Common;

public static class CommonHelper {

	#region Equality

	internal static bool ShallowEquals<TValue>(this TValue[]? input, TValue[]? other, IEqualityComparer<TValue>? comparer = null) {
		if (input == other) return true;
		if ((input == null) || (other == null)) return false;
		if (input.Rank != other.Rank) return false;
		if (input.LongLength != other.LongLength) return false;

		comparer ??= EqualityComparer<TValue>.Default;

		for (int i = 0; i < input.Length; i++) {
			if (!comparer.Equals(input[i], other[i]))
				return false;
		}

		return true;
	}

	internal static bool ShallowEquals<TValue>(this IList<TValue> input, IList<TValue> other, IEqualityComparer<TValue>? comparer = null) {
		if (input == other) return true;
		if ((input == null) || (other == null)) return false;
		if (input.Count != other.Count) return false;

		comparer ??= input is ValueEqualityList<TValue> vel
			? vel.Comparer
			: EqualityComparer<TValue>.Default;

		for (int i = 0; i < input.Count; i++) {
			if (!comparer.Equals(input[i], other[i]))
				return false;
		}

		return true;
	}

	internal static bool ShallowEquals<TKey, TValue>(this Dictionary<TKey, TValue> input, Dictionary<TKey, TValue> other, IEqualityComparer<TValue>? comparer = null) where TKey : notnull {
		if (input == other) return true;
		if ((input == null) || (other == null)) return false;
		if (input.Count != other.Count) return false;

		comparer ??= input is ValueEqualityDictionary<TKey, TValue> ved
			? ved.ValueComparer
			: EqualityComparer<TValue>.Default;

		foreach (var entry in input) {
			if (!other.TryGetValue(entry.Key, out TValue? value))
				return false;
			if (!comparer.Equals(entry.Value, value))
				return false;
		}

		return true;
	}

	#endregion

	#region Color Parsing

	#region Color Names

	/// <summary>
	/// CSS4 colors, and JojaBlue from the base game.
	/// </summary>
	private static readonly Dictionary<string, Color> ExtraColors = new(StringComparer.OrdinalIgnoreCase) {
		{ "rebeccapurple", new Color(102, 51, 153, 255) },
		{ "jojablue", new Color(52, 50, 122) }
	};

	/// <summary>
	/// A dictionary mapping color names to XNA Colors. Names are handled
	/// in a case-insensitive way, and lazy loaded.
	/// </summary>
	private static readonly Lazy<Dictionary<string, Color>> NamedColors = new(LoadNamedColors);

	private static void AddColor(Dictionary<string, Color> dict, string name, Color color) {
		if (!dict.ContainsKey(name))
			dict[name] = color;

		if (name.Contains("Gray")) {
			name = name.Replace("Gray", "Grey");
			if (!dict.ContainsKey(name))
				dict[name] = color;
		}
	}

	private static Dictionary<string, Color> LoadNamedColors() {
		Dictionary<string, Color> result = new(StringComparer.OrdinalIgnoreCase);

		// Load every available color name from XNA Color.
		foreach (PropertyInfo prop in typeof(Color).GetProperties(BindingFlags.Static | BindingFlags.Public)) {
			string name = prop.Name;
			if (prop.PropertyType != typeof(Color))
				continue;

			Color? color;
			try {
				color = prop.GetValue(null) as Color?;
			} catch {
				continue;
			}

			if (color.HasValue)
				AddColor(result, name, color.Value);
		}

		// Also load every available color name from System.Drawing.Color.
		foreach (PropertyInfo prop in typeof(SColor).GetProperties(BindingFlags.Static | BindingFlags.Public)) {
			string name = prop.Name;
			if (prop.PropertyType != typeof(SColor))
				continue;

			SColor? color;
			try {
				color = prop.GetValue(null) as SColor?;
			} catch {
				continue;
			}

			if (color.HasValue)
				AddColor(result, name, new Color(
					color.Value.R,
					color.Value.G,
					color.Value.B,
					color.Value.A
				));
		}

		foreach (var entry in ExtraColors) {
			AddColor(result, entry.Key, entry.Value);
		}

		return result;
	}

	#endregion

	public static Color PremultiplyAlpha(this Color input) {
		if (input.A == 0)
			return Color.Transparent;
		if (input.A == 255)
			return input;

		float alpha = input.A / 255f;

		return new Color(
			input.R * alpha,
			input.G * alpha,
			input.B * alpha,
			input.A
		);
	}

	public static string ToHex(this Color input) {
		return string.Format("#{0:X02}{1:X02}{2:X02}{3:X02}", input.R, input.G, input.B, input.A);
	}

	public static Color? ParseColor(string? input) {
		if (TryParseColor(input, out Color? result))
			return result.Value;

		return null;
	}

	public static bool TryParseColor(string? input, [NotNullWhen(true)] out Color? result) {
		bool premultiply = !string.IsNullOrEmpty(input) && input.StartsWith("premultiply:");
		if (premultiply)
			input = input![12..];

		if (TryParseColorImpl(input, out result)) {
			if (premultiply)
				result = PremultiplyAlpha(result.Value);
			return true;
		}

		return false;
	}

	private static bool TryParseColorImpl(string? input, [NotNullWhen(true)] out Color? result) {
		if (!string.IsNullOrEmpty(input)) {
			// Raw Format (Old)
			if (char.IsDigit(input[0]))
				return ParseRawColor(input, out result);

			// Hex Format
			if (input[0] == '#')
				return ParseHexColor(input, out result);

			// Test for /rgba?\(/i
			if (
				input.Length > 4 &&
				(input[0] == 'r' || input[0] == 'R') &&
				(input[1] == 'g' || input[1] == 'G') &&
				(input[2] == 'b' || input[2] == 'B')
			) {
				// Step 2. Check for either '(' or 'a('

				if (input[3] == '(')
					return ParseRgbColor(input, 4, out result);

				if (
					input.Length > 5 &&
					(input[3] == 'a' || input[3] == 'A') &&
					input[4] == '('
				)
					return ParseRgbColor(input, 5, out result);
			}

			// Test for /hsla?\(/i
			if (
				input.Length > 4 &&
				(input[0] == 'h' || input[0] == 'H') &&
				(input[1] == 's' || input[1] == 'S') &&
				(input[2] == 'l' || input[2] == 'L')
			) {
				// Step 2. Check for either '(' or 'a('
				if (input[3] == '(')
					return ParseHslColor(input, 4, out result);

				if (
					input.Length > 5 &&
					(input[3] == 'a' || input[3] == 'A') &&
					input[4] == '('
				)
					return ParseHslColor(input, 5, out result);
			}

			// Named Colors
			if (NamedColors.Value.TryGetValue(input, out Color color)) {
				result = color;
				return true;
			}
		}

		result = null;
		return false;
	}

	#region Raw Color

	/// <summary>
	/// Parse a color in the raw format used by SMAPI. This consists of
	/// comma-separated integer values for R, G, B, and A in the range
	/// of 0 to 255, inclusive.
	/// </summary>
	/// <param name="input">The string to parse</param>
	/// <param name="result">The resulting color</param>
	/// <returns>Whether or not the string was valid and a color was
	/// successfully parsed</returns>
	public static bool ParseRawColor(string input, [NotNullWhen(true)] out Color? result) {
		// We need to find two or three comma separators.
		int s1 = input.IndexOf(',');
		int s2 = s1 == -1 ? -1 : input.IndexOf(',', s1 + 1);
		int s3 = s2 == -1 ? -1 : input.IndexOf(',', s2 + 1);

		// If we don't find enough commas, we can't parse a color.
		if (s2 == -1) {
			result = null;
			return false;
		}

		// Now parse integers from the spans between: the start of the line,
		// the commas, and the end of the line. Use AsSpan rather than
		// Substring or just string slicing to avoid allocations.
		int r = int.Parse(input.AsSpan(0, s1));
		int g = int.Parse(input.AsSpan(s1 + 1, s2 - s1 - 1));
		int b = int.Parse(s3 == -1 ? input.AsSpan(s2 + 1) : input.AsSpan(s2 + 1, s3 - s2 - 1));
		int a = s3 == -1 ? 255 : int.Parse(input.AsSpan(s3 + 1));

		// If we got here, no exceptions were thrown from bad input. Build
		// a Color and return it. It's probably okay, and if not Color
		// should handle validation.
		result = new Color(r, g, b, a);
		return true;
	}

	#endregion

	#region CSS Hex Color

	/// <summary>
	/// Parse a color in the CSS hex format. This consists of a hash sign
	/// followed by 3, 4, 6, or 8 hexadecimal characters.
	/// <see href="https://www.w3.org/TR/css-color-4/#hex-notation" />
	/// </summary>
	/// <param name="input">The string to parse</param>
	/// <param name="result">The resulting color</param>
	/// <returns>Whether or not the string was valid and a color was
	/// successfully parsed</returns>
	public static bool ParseHexColor(string input, [NotNullWhen(true)] out Color? result) {
		int r, g, b, a;

		if (input.Length == 4) {
			r = int.Parse(input.AsSpan(1, 1), NumberStyles.HexNumber) * 17;
			g = int.Parse(input.AsSpan(2, 1), NumberStyles.HexNumber) * 17;
			b = int.Parse(input.AsSpan(3, 1), NumberStyles.HexNumber) * 17;
			a = 255;
		} else if (input.Length == 5) {
			r = int.Parse(input.AsSpan(1, 1), NumberStyles.HexNumber) * 17;
			g = int.Parse(input.AsSpan(2, 1), NumberStyles.HexNumber) * 17;
			b = int.Parse(input.AsSpan(3, 1), NumberStyles.HexNumber) * 17;
			a = int.Parse(input.AsSpan(4, 1), NumberStyles.HexNumber) * 17;
		} else if (input.Length == 7) {
			r = int.Parse(input.AsSpan(1, 2), NumberStyles.HexNumber);
			g = int.Parse(input.AsSpan(3, 2), NumberStyles.HexNumber);
			b = int.Parse(input.AsSpan(5, 2), NumberStyles.HexNumber);
			a = 255;
		} else if (input.Length == 9) {
			r = int.Parse(input.AsSpan(1, 2), NumberStyles.HexNumber);
			g = int.Parse(input.AsSpan(3, 2), NumberStyles.HexNumber);
			b = int.Parse(input.AsSpan(5, 2), NumberStyles.HexNumber);
			a = int.Parse(input.AsSpan(7, 2), NumberStyles.HexNumber);
		} else {
			result = null;
			return false;
		}

		result = new Color(r, g, b, a);
		return true;
	}

	#endregion

	#region RGB() and HSL() Helpers

	private static int HydrateRgbValue(ReadOnlySpan<char> input) {
		int idx = input.LastIndexOf('%');
		if (idx == -1)
			return int.Parse(input);

		return (int) (float.Parse(input[..idx]) * 2.55f);
	}

	private static double HydratePercentage(ReadOnlySpan<char> input) {
		int idx = input.LastIndexOf('%');
		if (idx == -1)
			return double.Parse(input);

		return double.Parse(input[..idx]) / 100.0;
	}

	private static double HydrateRotation(ReadOnlySpan<char> input) {
		int mode = 0;
		int idx = input.LastIndexOf("turn");
		if (idx != -1)
			mode = 1;
		else {
			idx = input.LastIndexOf("rad");
			if (idx != -1)
				mode = 2;
			else {
				idx = input.LastIndexOf("grad");
				if (idx != -1)
					mode = 3;
				else
					idx = input.LastIndexOf("deg");
			}
		}

		double value = double.Parse(idx == -1 ? input : input[..idx]);

		if (mode == 1)
			value *= 360.0;
		if (mode == 2)
			value *= (180.0 / Math.PI);
		if (mode == 3)
			value *= 0.9;

		return value;
	}

	#endregion

	#region CSS RGB() Color

	/// <summary>
	/// Parse a color in the CSS rgb() format. This method does not fully
	/// conform to CSS Color 4, as it does not allow using spaces to
	/// separate values. Commas are required.
	/// </summary>
	/// <param name="input">The string to parse</param>
	/// <param name="result">The resulting color</param>
	/// <returns>Whether or not the string was valid and a color was
	/// successfully parsed</returns>
	private static bool ParseRgbColor(string input, int offset, [NotNullWhen(true)] out Color? result) {
		int end = input.IndexOf(')', offset);
		if (end == -1) {
			result = null;
			return false;
		}

		// This doesn't strictly conform to CSS Colors 4, but we aren't going
		// to support space-separated values. Only commas.
		int s1 = input.IndexOf(',', offset);
		int s2 = s1 == -1 ? -1 : input.IndexOf(',', s1 + 1);
		int s3 = s2 == -1 ? -1 : input.IndexOf(',', s2 + 1);

		// If we don't find enough commas, we can't parse a color.
		if (s2 == -1) {
			result = null;
			return false;
		}

		int r = HydrateRgbValue(input.AsSpan(offset, s1 - offset));
		int g = HydrateRgbValue(input.AsSpan(s1 + 1, s2 - s1 - 1));
		int b = HydrateRgbValue(input.AsSpan(s2 + 1, (s3 == -1 ? end : s3) - s2 - 1));
		int a = s3 == -1 ? 255 : (int) (HydratePercentage(input.AsSpan(s3 + 1, end - s3 - 1)) * 255);

		result = new Color(r, g, b, a);
		return true;
	}

	#endregion

	#region CSS HSL() Color

	/// <summary>
	/// Convert a color in the HSL color space to a color in the RGB color
	/// space. This method is implemented using the algoritm described in
	/// the CSS3 color documentation at <see href="https://www.w3.org/TR/css-color-3/#hsl-color" />
	/// </summary>
	/// <param name="h">The hue, a number between 0 (inclusive) and 360 (exclusive)</param>
	/// <param name="s">The saturation, a percentage between 0 and 1 inclusive</param>
	/// <param name="l">The luminance, a percentage between 0 and 1 inclusive</param>
	/// <param name="a">The alpha, and int between 0 and 255 inclusive</param>
	public static Color FromHSL(double h, double s, double l, int a = 255) {
		// Normalize the range of hue to be 0..360 without clamping
		h %= 360;
		if (h < 0)
			h += 360;

		return new Color(
			(int) Math.Round(255 * HueToRgb(h, s, l, 0)),
			(int) Math.Round(255 * HueToRgb(h, s, l, 8)),
			(int) Math.Round(255 * HueToRgb(h, s, l, 4)),
			a
		);
	}

	private static double HueToRgb(double h, double s, double l, int n) {
		double k = (n + h / 30) % 12;
		double a = s * Math.Min(l, 1 - l);
		return l - a * Math.Max(-1, Math.Min(Math.Min(k - 3, 9 - k), 1));
	}

	/// <summary>
	/// Parse a color in the CSS hsl() format. This method does not fully
	/// conform to CSS Color 4, as it does not allow using spaces to
	/// separate values. Commas are required.
	/// </summary>
	/// <param name="input">The string to parse</param>
	/// <param name="result">The resulting color</param>
	/// <returns>Whether or not the string was valid and a color was
	/// successfully parsed</returns>
	private static bool ParseHslColor(string str, int offset, [NotNullWhen(true)] out Color? result) {
		int end = str.IndexOf(')');
		if (end == -1) {
			result = null;
			return false;
		}

		// This doesn't strictly conform to CSS Colors 4, but we aren't going
		// to support space-separated values. Only commas.
		int s1 = str.IndexOf(',', offset);
		int s2 = s1 == -1 ? -1 : str.IndexOf(',', s1 + 1);
		int s3 = s2 == -1 ? -1 : str.IndexOf(',', s2 + 1);

		// If we don't find enough commas, we can't parse a color.
		if (s2 == -1) {
			result = null;
			return false;
		}

		double h = HydrateRotation(str.AsSpan(offset, s1 - offset));
		double s = HydratePercentage(str.AsSpan(s1 + 1, s2 - s1 - 1));
		double l = HydratePercentage(s3 == -1 ? str.AsSpan(s2 + 1, end - s2 - 1) : str.AsSpan(s1 + 1, s3 - s2 - 1));
		int a = s3 == -1 ? 255 : (int) (HydratePercentage(str.AsSpan(s3 + 1, end - s3 - 1)) * 255);

		result = FromHSL(h, s, l, a);
		return true;
	}

	#endregion

	#endregion

	#region Colors

	internal static int PackColor(this Color color) {
		return (color.R << 16) + (color.G << 8) + color.B;
	}

	internal static Color UnpackColor(int color) {
		return new Color(
			color >> 16 & 0xFF,
			color >> 8 & 0xFF,
			color & 0xFF
		);
	}

	#endregion

	#region Enums

	/// <summary>
	/// Return the next value in an enum, after the given value. Loop around
	/// if we reach the end of the enum.
	/// </summary>
	/// <typeparam name="T">The enum type.</typeparam>
	/// <param name="current">The current value</param>
	/// <param name="direction">The number of steps to move. Can be negative.</param>
	/// <returns>The next value.</returns>
	public static T Cycle<T>(T current, int direction = 1, T[]? skip = null) {
		var values = Enum.GetValues(typeof(T)).Cast<T>().ToArray();

		int idx = -1;

		for (int i = 0; i < values.Length; i++) {
			if (current == null || current.Equals(values[i])) {
				idx = i + direction;
				break;
			}
		}

		T result;

		if (idx < 0)
			result = values.Last();
		else if (idx >= values.Length)
			result = values[0];
		else
			result = values[idx];

		if (skip is not null && skip.Contains(result))
			return Cycle(result, direction, skip);

		return result;
	}

	public static IEnumerable<TEnum> GetValues<TEnum>() where TEnum : struct, Enum {
		return Enum.GetValues<TEnum>();
		//return Enum.GetValues(typeof(T)).Cast<T>();
	}

	#endregion

	#region Strings

	/// <summary>
	/// A more efficient version of the game's <see cref="ArgUtility.SplitQuoteAware(string, char, StringSplitOptions, bool)"/> method.
	/// </summary>
	/// <param name="input"></param>
	/// <param name="delimiter"></param>
	/// <param name="splitOptions"></param>
	/// <param name="keepQuotesAndEscapes"></param>
	/// <returns></returns>
	internal static string[] SplitQuoteAware(string input, char delimiter, StringSplitOptions splitOptions = StringSplitOptions.None, bool keepQuotesAndEscapes = false) {
		if (string.IsNullOrEmpty(input))
			return [];

		if (!input.Contains('"'))
			return input.Split(delimiter, splitOptions);

		bool trimEntries = splitOptions.HasFlag(StringSplitOptions.TrimEntries);
		bool removeEmpty = splitOptions.HasFlag(StringSplitOptions.RemoveEmptyEntries);

		var span = input.AsSpan();
		if (trimEntries)
			span = span.Trim();

		List<string> values = [];

		bool inQuotes = false;
		bool escapeNext = false;

		ValueStringBuilder vsb = new(stackalloc char[256]);

		foreach (char c in span) {
			if (escapeNext)
				escapeNext = false;
			else if (c == '\\') {
				escapeNext = true;
				if (!keepQuotesAndEscapes)
					continue;

			} else if (c == '"') {
				inQuotes = !inQuotes;
				if (!keepQuotesAndEscapes)
					continue;
			} else if (c == delimiter && !inQuotes) {
				var segment = vsb.AsSpan();
				if (trimEntries)
					segment = segment.Trim();
				if (!(removeEmpty && segment.IsEmpty))
					values.Add(segment.ToString());
				vsb.Clear();
				continue;
			}

			vsb.Append(c);
		}

		if (vsb.Length > 0) {
			var segment = vsb.AsSpan();
			if (trimEntries)
				segment = segment.Trim();
			if (!(removeEmpty && segment.IsEmpty))
				values.Add(segment.ToString());
		}

		return values.ToArray();
	}


	internal static int IndexOfWhitespace(this string text, int startIndex = 0) {
		int i = startIndex;
		while (i < text.Length) {
			if (char.IsWhiteSpace(text[i]))
				return i;
			i++;
		}
		return -1;
	}

	internal static int LastIndexOfWhitespace(this string text, int startIndex = -1) {
		int i = startIndex < 0 ? text.Length - 1 : startIndex;
		while (i >= 0) {
			if (char.IsWhiteSpace(text[i]))
				return i;
			i--;
		}
		return -1;
	}

	internal static int IndexOfWhitespace(this ReadOnlySpan<char> text, int startIndex = 0) {
		int i = startIndex;
		while (i < text.Length) {
			if (char.IsWhiteSpace(text[i]))
				return i;
			i++;
		}
		return -1;
	}

	internal static int LastIndexOfWhitespace(this ReadOnlySpan<char> text, int startIndex = -1) {
		int i = startIndex < 0 ? text.Length - 1 : startIndex;
		while (i >= 0) {
			if (char.IsWhiteSpace(text[i]))
				return i;
			i--;
		}
		return -1;
	}

	#endregion

	internal static IEnumerable<NPC> EnumerateCharacters(IEnumerable<GameLocation>? locations = null, bool includeEventActors = false, bool? dateable = null) {
		foreach (var location in locations ?? EnumerateLocations(true, true)) {
			foreach (NPC npc in location.characters) {
				if ((includeEventActors || !npc.EventActor) && (!dateable.HasValue || dateable.Value == npc.datable.Value))
					yield return npc;
			}
		}
	}

	internal static IEnumerable<GameLocation> EnumerateLocations(bool includeInteriors = true, bool includeGenerated = false) {

		GameLocation current = Game1.currentLocation;
		string? currentName = current?.NameOrUniqueName;

		foreach (var rawLocation in Game1.locations) {
			GameLocation loc = (rawLocation.NameOrUniqueName == currentName && current != null)
				? current
				: rawLocation;

			yield return loc;

			if (!includeInteriors)
				continue;

			foreach (var building in loc.buildings) {
				if (building.GetIndoorsType() == StardewValley.Buildings.IndoorsType.Instanced) {
					var indoors = building.GetIndoors();
					if (indoors != null)
						yield return indoors;
				}
			}
		}

		if (!includeGenerated)
			yield break;

		foreach (var rawLocation in MineShaft.activeMines) {
			GameLocation loc = (rawLocation.NameOrUniqueName == currentName && current != null)
				? current
				: rawLocation;

			yield return loc;
		}

		foreach (var rawLocation in VolcanoDungeon.activeLevels) {
			GameLocation loc = (rawLocation.NameOrUniqueName == currentName && current != null)
				? current
				: rawLocation;

			yield return loc;
		}
	}

	internal static Vector2 GetNearestPoint(this Rectangle rectangle, Vector2 position) {
		float minX = rectangle.X;
		float maxX = minX + rectangle.Width;
		float minY = rectangle.Y;
		float maxY = minY + rectangle.Height;

		return new Vector2(
			position.X < minX
				? minX
				: position.X > maxX
					? maxX
					: position.X,
			position.Y < minY
				? minY
				: position.Y > maxY
					? maxY
					: position.Y
		);
	}

	internal static xTile.Dimensions.Location ToLocation(this Vector2 pos) {
		return new((int) pos.X, (int) pos.Y);
	}

	internal static Point ToPoint(this Vector2 pos) {
		return new((int) pos.X, (int) pos.Y);
	}

	private static Action<IClickableMenu?>? ActiveMenuSetter;

	[MemberNotNull(nameof(ActiveMenuSetter))]
	private static void LoadActiveMenuSetter() {
		if (ActiveMenuSetter == null) {
			var field = typeof(Game1).GetField("_activeClickableMenu", BindingFlags.Static | BindingFlags.NonPublic)
				?? throw new ArgumentNullException("Could not get _activeClickableMenu field");

			ActiveMenuSetter = field.CreateSetter<IClickableMenu?>();
		}
	}


	/// <summary>
	/// Call <see cref="IClickableMenu.exitThisMenu(bool)"/> without actually
	/// modifying <see cref="Game1.activeClickableMenu"/>. This allows us to
	/// ensure that any pending <see cref="Game1.nextClickableMenu"/> entries
	/// aren't made active when we're replacing a menu with our own.
	/// </summary>
	/// <param name="menu">The menu being removed.</param>
	public static void YeetMenu(this IClickableMenu menu) {
		if (menu == null) return;

		IClickableMenu? active = Game1.activeClickableMenu;

		bool is_active = menu == active;
		bool is_game_menu_active = !is_active && active is GameMenu gm && gm.GetCurrentPage() == menu;

		if (is_active || is_game_menu_active) {
			LoadActiveMenuSetter();
			ActiveMenuSetter(null);
		}

		menu.exitThisMenu(playSound: false);

		if (is_game_menu_active)
			ActiveMenuSetter!(active);
	}

}
