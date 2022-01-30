/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using static SpriteMaster.Runtime;

namespace SpriteMaster.Extensions;

static class Collections {
	#region IsBlank
	[MethodImpl(MethodImpl.Hot)]
	internal static bool IsBlank<T>(this IEnumerable<T>? enumerable) => enumerable is null || !enumerable.Any();

	[MethodImpl(MethodImpl.Hot)]
	internal static bool IsBlank<T>(this ICollection<T>? collection) => collection is null || !collection.Any();

	[MethodImpl(MethodImpl.Hot)]
	internal static bool IsBlank<T>(this IList<T>? list) => list is null || list.Count == 0;

	[MethodImpl(MethodImpl.Hot)]
	internal static bool IsBlank<T>(this List<T>? list) => list is null || list.Count == 0;

	[MethodImpl(MethodImpl.Hot)]
	internal static bool IsBlank<T>(this T[]? array) => array is null || array.Length == 0;
	#endregion

	#region IsEmpty
	[MethodImpl(MethodImpl.Hot)]
	internal static bool IsEmpty<T>(this IEnumerable<T> enumerable) => !enumerable.Any();

	[MethodImpl(MethodImpl.Hot)]
	internal static bool IsEmpty<T>(this ICollection<T> collection) => !collection.Any();

	[MethodImpl(MethodImpl.Hot)]
	internal static bool IsEmpty<T>(this IList<T> list) => list.Count == 0;

	[MethodImpl(MethodImpl.Hot)]
	internal static bool IsEmpty<T>(this List<T> list) => list.Count == 0;

	[MethodImpl(MethodImpl.Hot)]
	internal static bool IsEmpty<T>(this T[] array) => array.Length == 0;
	#endregion

	#region Blanked
	[MethodImpl(MethodImpl.Hot)]
	internal static T? Blanked<T>(this T? enumerable) where T : class?, IEnumerable<T> => enumerable.IsBlank() ? null : enumerable;

	[MethodImpl(MethodImpl.Hot)]
	internal static T[]? Blanked<T>(this T[]? array) => array.IsBlank() ? null : array;
	#endregion

	[MethodImpl(MethodImpl.Hot)]
	internal static V? GetOrAddDefault<K, V>(this Dictionary<K, V> dictionary, K key, Func<V> defaultGetter) where K : notnull {
		if (dictionary.TryGetValue(key, out V? value)) {
			return value;
		}
		var newValue = defaultGetter.Invoke();
		dictionary.Add(key, newValue);
		return newValue;
	}

	#region List Conversions
	/// <summary>
	/// Returns a new List that is constructed from the array.
	/// </summary>
	[MethodImpl(MethodImpl.Hot)]
	internal static List<T> ToList<T>(this T[] array) => new List<T>(array);

	// TODO : define me for .NET and .NETfx
	private static class ListReflectImpl<T> {
		internal static readonly Action<List<T>, T[]>? ListSetItems = typeof(List<T>).GetFieldSetter<List<T>, T[]>("_items");
		internal static readonly Action<List<T>, int>? ListSetSize = typeof(List<T>).GetFieldSetter<List<T>, int>("_size");
	}

	/// <summary>
	/// Returns a new List that contains the same elements as the array.
	/// <para>Warning: it is not safe to use the array afterwards - the List now owns it.</para>
	/// </summary>
	[MethodImpl(MethodImpl.Hot)]
	internal static List<T> BeList<T>(this T[] array) {
		if (ListReflectImpl<T>.ListSetItems is null || ListReflectImpl<T>.ListSetSize is null) {
			return array.ToList();
		}

		var newList = new List<T>();
		ListReflectImpl<T>.ListSetItems(newList, array);
		ListReflectImpl<T>.ListSetSize(newList, array.Length);
		return newList;
	}
	#endregion

	#region Ranges
	internal static class ArrayExt {
		internal static int[] Range(int start, int count, int change = 1) {
			var result = new int[count];
			for (int i = 0; count > 0; --count) {
				result[i++] = start;
				start += change;
			}
			return result;
		}

		internal static long[] Range(long start, long count, long change = 1) {
			var result = new long[count];
			for (int i = 0; count > 0; --count) {
				result[i++] = start;
				start += change;
			}
			return result;
		}
	}
	#endregion
}
