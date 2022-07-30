/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace SpriteMaster.Extensions;

/// <summary>
/// Enumerator Extensions
/// </summary>
internal static class EnumExt {
	/// <summary>
	/// Return an array name and value pairs representing the enum
	/// </summary>
	/// <typeparam name="TEnum">Enumerator Type</typeparam>
	/// <returns>Array of name-value pairs</returns>
	[Pure, MustUseReturnValue]
	internal static KeyValuePair<string, TEnum>[] Get<TEnum>() where TEnum : struct, Enum {
		var names = Enum.GetNames(typeof(TEnum));
		var result = new KeyValuePair<string, TEnum>[names.Length];
		for (int i = 0; i < names.Length; ++i) {
			result[i] = new(names[i], Enum.Parse<TEnum>(names[i]));
		}

		return result;
	}

	/// <summary>
	/// Return an array name and value pairs representing the enum
	/// </summary>
	/// <param name="type">Enumerator Type</param>
	/// <returns>Array of name-value pairs</returns>
	[Pure, MustUseReturnValue]
	internal static KeyValuePair<string, int>[] Get(Type type) {
		var names = Enum.GetNames(type);
		var result = new KeyValuePair<string, int>[names.Length];
		for (int i = 0; i < names.Length; ++i) {
			result[i] = new(names[i], (int)Enum.Parse(type, names[i]));
		}

		return result;
	}

	[Pure, MustUseReturnValue, MethodImpl(Runtime.MethodImpl.Inline)]
	internal static Type GetUnderlyingType<T>() where T : Enum => Enum.GetUnderlyingType(typeof(T));

	[Pure, MustUseReturnValue, MethodImpl(Runtime.MethodImpl.Inline)]
	internal static T? Parse<T>(Type? enumType, string value) where T : unmanaged {
		if (enumType is null) {
			return null;
		}

		if (Enum.TryParse(enumType, value, out var result)) {
			return (T)result!;
		}

		return null;
	}
}
