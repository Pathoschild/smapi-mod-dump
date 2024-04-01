/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aurpine/Stardew-SpriteMaster
**
*************************************************/

using Pastel;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;

namespace SpriteMaster.Extensions;

internal static class StringExt {
	#region General
	[MethodImpl(Runtime.MethodImpl.Inline)]
	[return: NotNullIfNotNull("str")]
	internal static string? Intern(this string? str) => str is null ? null : string.Intern(str);

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static string ToString<T>(this T? obj, in DrawingColor color) => (obj?.ToString() ?? "[null]").Pastel(color);

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static bool IsEmpty(this string str) => str.Length == 0;

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static bool IsBlank([NotNullWhen(false)] this string? str) => string.IsNullOrEmpty(str);

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static bool IsWhiteBlank([NotNullWhen(false)] this string? str) => string.IsNullOrEmpty(str?.Trim());

	internal static string CapitalizeFirstLetter(this string str) {
		if (str.Length == 0 || char.IsUpper(str[0])) {
			return str;
		}

		var charSpan = new char[1] { char.ToUpperInvariant(str[0]) }.AsReadOnlySpan();

		return string.Concat(charSpan, str.AsSpan(1));
	}

	internal static string Reverse(this string str) {
		str.AssertNotNull();

		int strLength = str.Length;
		int strEnd = strLength - 1;
		int strLengthHalf = strLength >> 1;

		var strSpan = str.AsSpan().ToSpanUnsafe();
		for (int i = 0; i < strLengthHalf; ++i) {
			int endIndex = strEnd - i;
			var temp = strSpan[endIndex];
			strSpan[endIndex] = strSpan[i];
			strSpan[i] = temp;
		}

		return str;
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static string Reversed(this string str) {
		str.AssertNotNull();
		return new string(str).Reverse();
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static string Enquote(this string str, char quote = '\'') {
		if (str.Length >= 2 && str[0] == quote && str[^1] == quote) {
			return str;
		}
		return $"{quote}{str}{quote}";
	}

	private static readonly char[] NewlineChars = { '\n', '\r' };
	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static string[] Lines(this string str, bool removeEmpty = false) {
		var strings = str.Split(NewlineChars, removeEmpty ? StringSplitOptions.RemoveEmptyEntries : StringSplitOptions.None);
		return strings;
	}

	#region Case Sensitivity Comparison Conversion

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static StringComparison AsCaseInsensitive(this StringComparison comparisonType) => comparisonType switch {
		StringComparison.CurrentCulture => StringComparison.CurrentCultureIgnoreCase,
		StringComparison.InvariantCulture => StringComparison.InvariantCultureIgnoreCase,
		StringComparison.Ordinal => StringComparison.OrdinalIgnoreCase,
		_ => comparisonType
	};

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static StringComparison AsCaseSensitive(this StringComparison comparisonType) => comparisonType switch {
		StringComparison.CurrentCultureIgnoreCase => StringComparison.CurrentCulture,
		StringComparison.InvariantCultureIgnoreCase => StringComparison.InvariantCulture,
		StringComparison.OrdinalIgnoreCase => StringComparison.Ordinal,
		_ => comparisonType
	};

	#endregion

	#region RemoveFromEnd

	[MethodImpl(Runtime.MethodImpl.Inline)]
	private static string RemoveFromEndInternal(this string value, string remove, StringComparison comparisonType, out bool removed) {
		int expectedIndex = value.Length - remove.Length;
		if (remove.Length != 0 && expectedIndex >= 0 && value.LastIndexOf(remove, comparisonType) == expectedIndex) {
			removed = true;
			return value[..expectedIndex];
		}

		removed = false;
		return value;
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static string RemoveFromEnd(this string value, string remove, StringComparison comparisonType = StringComparison.Ordinal) =>
		RemoveFromEndInternal(value, remove, comparisonType, out _);

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static string RemoveFromEndInsensitive(this string value, string remove, StringComparison comparisonType = StringComparison.OrdinalIgnoreCase) =>
		RemoveFromEndInternal(value, remove, comparisonType.AsCaseInsensitive(), out _);

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static bool TryRemoveFromEnd(this string value, string remove, StringComparison comparisonType, out string result) {
		result = RemoveFromEndInternal(value, remove, comparisonType, out var removed);
		return removed;
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static bool TryRemoveFromEndInsensitive(string value, string remove, StringComparison comparisonType, out string result) =>
		TryRemoveFromEnd(value, remove, comparisonType.AsCaseInsensitive(), out result);

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static bool TryRemoveFromEnd(this string value, string remove, out string result) =>
		TryRemoveFromEnd(value, remove, StringComparison.Ordinal, out result);

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static bool TryRemoveFromEndInsensitive(this string value, string remove, out string result) =>
		TryRemoveFromEndInsensitive(value, remove, StringComparison.OrdinalIgnoreCase, out result);

	#endregion

	#region RemoveFromStart

	[MethodImpl(Runtime.MethodImpl.Inline)]
	private static string RemoveFromStartInternal(this string value, string remove, StringComparison comparisonType, out bool removed) {
		if (remove.Length != 0 && remove.Length <= value.Length && value.StartsWith(remove, comparisonType)) {
			removed = true;
			return value[remove.Length..];
		}

		removed = false;
		return value;
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static string RemoveFromStart(this string value, string remove, StringComparison comparisonType = StringComparison.Ordinal) =>
		RemoveFromStartInternal(value, remove, comparisonType, out _);

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static string RemoveFromStartInsensitive(this string value, string remove, StringComparison comparisonType = StringComparison.OrdinalIgnoreCase) =>
		RemoveFromStartInternal(value, remove, comparisonType.AsCaseInsensitive(), out _);

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static bool TryRemoveFromStart(this string value, string remove, StringComparison comparisonType, out string result) {
		result = RemoveFromStartInternal(value, remove, comparisonType, out var removed);
		return removed;
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static bool TryRemoveFromStartInsensitive(string value, string remove, StringComparison comparisonType, out string result) =>
		TryRemoveFromStart(value, remove, comparisonType.AsCaseInsensitive(), out result);

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static bool TryRemoveFromStart(this string value, string remove, out string result) =>
		TryRemoveFromStart(value, remove, StringComparison.Ordinal, out result);

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static bool TryRemoveFromStartInsensitive(this string value, string remove, out string result) =>
		TryRemoveFromStartInsensitive(value, remove, StringComparison.OrdinalIgnoreCase, out result);

	#endregion

	#endregion General

	#region Equality

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static bool EqualsInvariantInsensitive(this string str1, string str2) => str1.Equals(str2, System.StringComparison.InvariantCultureIgnoreCase);

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static bool EqualsOrdinal(this string str1, string str2) => str1.Equals(str2, System.StringComparison.Ordinal);

	#endregion Equality

	#region Color

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static string Colorized(this string str, DrawingColor color) => str.Pastel(color);

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static string Colorized(this string str, DrawingColor foregroundColor, DrawingColor backgroundColor) => str.Pastel(foregroundColor).PastelBg(backgroundColor);

	#endregion Color

	#region Experimental Extensions

	internal static class Reflection {

	}
	
	#endregion
}
