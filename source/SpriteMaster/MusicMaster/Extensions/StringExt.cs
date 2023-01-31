/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

using Pastel;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;

namespace MusicMaster.Extensions;

internal static class StringExt {
	#region General
	[MethodImpl(Runtime.MethodImpl.Inline)]
	[return: NotNullIfNotNull("str")]
	internal static string? Intern(this string? str) => str is null ? null : string.Intern(str);

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

	#endregion General

	#region Equality

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static bool EqualsInvariantInsensitive(this string str1, string str2) => str1.Equals(str2, System.StringComparison.InvariantCultureIgnoreCase);

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static bool EqualsOrdinal(this string str1, string str2) => str1.Equals(str2, System.StringComparison.Ordinal);

	#endregion Equality

	#region Experimental Extensions

	internal static class Reflection {

	}
	
	#endregion
}
