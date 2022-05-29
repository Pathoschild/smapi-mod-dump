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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using static SpriteMaster.Runtime;

namespace SpriteMaster.Extensions;

internal static class Collections {
	#region IsBlank
	[MethodImpl(MethodImpl.Inline)]
	internal static bool IsBlank<T>(this IEnumerable<T>? enumerable) => enumerable is null || !enumerable.Any();

	[MethodImpl(MethodImpl.Inline)]
	internal static bool IsBlank<T>(this ICollection<T>? collection) => collection is null || collection.Count == 0;

	[MethodImpl(MethodImpl.Inline)]
	internal static bool IsBlank<T>(this IList<T>? list) => list is null || list.Count == 0;

	[MethodImpl(MethodImpl.Inline)]
	internal static bool IsBlank<T>(this List<T>? list) => list is null || list.Count == 0;

	[MethodImpl(MethodImpl.Inline)]
	internal static bool IsBlank<T>(this T[]? array) => array is null || array.Length == 0;
	#endregion

	#region IsEmpty
	[MethodImpl(MethodImpl.Inline)]
	internal static bool IsEmpty<T>(this IEnumerable<T> enumerable) => !enumerable.Any();

	[MethodImpl(MethodImpl.Inline)]
	internal static bool IsEmpty<T>(this ICollection<T> collection) => !collection.Any();

	[MethodImpl(MethodImpl.Inline)]
	internal static bool IsEmpty<T>(this IList<T> list) => list.Count == 0;

	[MethodImpl(MethodImpl.Inline)]
	internal static bool IsEmpty<T>(this List<T> list) => list.Count == 0;

	[MethodImpl(MethodImpl.Inline)]
	internal static bool IsEmpty<T>(this T[] array) => array.Length == 0;
	#endregion

	#region Blanked
	[MethodImpl(MethodImpl.Inline)]
	internal static T? Blanked<T>(this T? enumerable) where T : class, IEnumerable<T> => enumerable.IsBlank() ? null : enumerable;

	[MethodImpl(MethodImpl.Inline)]
	internal static T[]? Blanked<T>(this T[]? array) => array.IsBlank() ? null : array;
	#endregion

	[MethodImpl(MethodImpl.Inline)]
	internal static V GetOrAddDefault<K, V>(this Dictionary<K, V> dictionary, K key, Func<V> defaultGetter) where K : notnull {
		if (dictionary.TryGetValue(key, out var value)) {
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
	internal static List<T> ToList<T>(this T[] array) {
		if (ListReflectImpl<T>.ListSetItems is null || ListReflectImpl<T>.ListSetSize is null) {
			var newList = new List<T>(array.Length);
			foreach (var item in array) {
				newList.Add(item);
			}
			return newList;
		}
		var newArray = GC.AllocateUninitializedArray<T>(array.Length);
		array.CopyTo(newArray, 0);
		return newArray.BeList();
	}

	// TODO : define me for .NET and .NETfx
	private static class ListReflectImpl<T> {
		internal static readonly Action<List<T>, T[]>? ListSetItems = typeof(List<T>).GetFieldSetter<List<T>, T[]>("_items");
		internal static readonly Action<List<T>, int>? ListSetSize = typeof(List<T>).GetFieldSetter<List<T>, int>("_size");
	}

	/// <summary>
	/// Returns a new List that contains the same elements as the array.
	/// <para>Warning: it is not safe to use the array afterwards - the List now owns it.</para>
	/// </summary>
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

	#region TryAt
	internal static bool TryAt<T>(this List<T> list, int index, out T? item) {
		if (index < 0) {
			throw new ArgumentOutOfRangeException(nameof(index), $"{index} is less than zero");
		}
		if (index >= list.Count) {
			item = default(T);
			return false;
		}

		item = list[index];
		return true;
	}

	internal static bool TryAt<T>(this IReadOnlyList<T> list, int index, out T? item) {
		if (index < 0) {
			throw new ArgumentOutOfRangeException(nameof(index), $"{index} is less than zero");
		}
		if (index >= list.Count) {
			item = default(T);
			return false;
		}

		item = list[index];
		return true;
	}

	internal static bool TryAt<T>(this T[] array, int index, out T? item) {
		if (index < 0) {
			throw new ArgumentOutOfRangeException(nameof(index), $"{index} is less than zero");
		}
		if (index >= array.Length) {
			item = default(T);
			return false;
		}

		item = array[index];
		return true;
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

	#region AddRange

	internal static int AddRange<T>(this HashSet<T> set, IEnumerable<T> collection) {
		int added = 0;
		foreach (T item in collection) {
			if (set.Add(item)) {
				++added;
			}
		}
		return added;
	}

	internal static int AddRange<T>(this HashSet<T> set, ICollection<T> collection) {
		int added = 0;
		set.EnsureCapacity(set.Count + collection.Count);
		foreach (T item in collection) {
			if (set.Add(item)) {
				++added;
			}
		}
		return added;
	}

	internal static int AddRange<T>(this HashSet<T> set, IList<T> list) {
		int added = 0;
		set.EnsureCapacity(set.Count + list.Count);
		for (int i = 0; i < list.Count; ++i) {
			T item = list[i];
			if (set.Add(item)) {
				++added;
			}
		}
		return added;
	}

	internal static int AddRange<T>(this HashSet<T> set, T[] array) {
		int added = 0;
		set.EnsureCapacity(set.Count + array.Length);
		foreach (T item in array) {
			if (set.Add(item)) {
				++added;
			}
		}
		return added;
	}

	internal static void AddRange<T>(this System.Collections.ObjectModel.Collection<T> collection, params T[] array) {
		foreach (var item in array) {
			collection.Add(item);
		}
	}

	#endregion

	#region Select
	/*
	internal static U[] Select2<T, U>(IList<T> list, Func<T, U> selector) {
		var result = GC.AllocateUninitializedArray<U>(list.Count);

		for (int i = 0; i < list.Count; ++i) {
			result[i] = selector(list[i]);
		}

		return result;
	}
	*/
	#endregion

	#region ToList
	internal static List<T> ToList<T>(this ConcurrentBag<T> bag) {
		return bag.ToArray().BeList();
	}
	#endregion

	#region Clone / CopyTo
	internal static T[] Clone<T>(this T[] array) => (T[])array.Clone();

	internal static T[]? CopyTo<T>(this T[]? source, T[]? destination) {
		if (source is null) {
			return null;
		}

		if (destination is null || destination.Length != source.Length) {
			destination = GC.AllocateUninitializedArray<T>(source.Length);
		}

		source.CopyTo(destination, 0);
		return destination;
	}
	#endregion
}
