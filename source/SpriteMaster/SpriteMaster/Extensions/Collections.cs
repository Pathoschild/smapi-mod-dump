/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

using Microsoft.Toolkit.HighPerformance;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
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
	// TODO : define me for .NET and .NETfx
	private static class ListReflectImpl<T> {
		private const string ItemsFieldName = "_items";
		private const string SizeFieldName = "_size";

		internal static readonly Action<List<T>, T[]>? SetItems = typeof(List<T>).GetFieldSetter<List<T>, T[]>(ItemsFieldName);
		internal static readonly Func<List<T>, T[]>? GetItems = typeof(List<T>).GetFieldGetter<List<T>, T[]>(ItemsFieldName);
		internal static readonly Action<List<T>, int>? SetSize = typeof(List<T>).GetFieldSetter<List<T>, int>(SizeFieldName);
		internal static readonly Func<List<T>, int>? GetSize = typeof(List<T>).GetFieldGetter<List<T>, int>(SizeFieldName);

		[MemberNotNullWhen(true, nameof(SetItems), nameof(SetSize))]
		// ReSharper disable once StaticMemberInGenericType
		internal static bool SetEnabled { get; } =
			SetItems is not null &&
			SetSize is not null;

		[MemberNotNullWhen(true, nameof(GetItems), nameof(GetSize))]
		// ReSharper disable once StaticMemberInGenericType
		internal static bool GetEnabled { get; } =
			GetItems is not null &&
			GetSize is not null;

		[MemberNotNullWhen(true, nameof(GetItems), nameof(GetSize), nameof(SetItems), nameof(SetSize))]
		// ReSharper disable once StaticMemberInGenericType
		internal static bool Enabled { get; } =
			SetEnabled &&
			GetEnabled;
	}

	/// <summary>
	/// Returns a new List that is constructed from the array.
	/// </summary>
	internal static List<T> ToList<T>(this T[] array) {
		if (ListReflectImpl<T>.SetEnabled) {
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

	/// <summary>
	/// Returns a new List that contains the same elements as the array.
	/// <para>Warning: it is not safe to use the array afterwards - the List now owns it.</para>
	/// </summary>
	internal static List<T> BeList<T>(this T[] array) {
		if (!ListReflectImpl<T>.SetEnabled) {
			return array.ToList();
		}


		// I would use RuntimeHelpers.GetUninitializedObject,
		// but it would be slower to manually initialize everything in the object
		var newList = new List<T>();
		ListReflectImpl<T>.SetItems(newList, array);
		ListReflectImpl<T>.SetSize(newList, array.Length);
		return newList;
	}

	/// <summary>
	/// Returns a <seealso cref="ReadOnlySpan{T}"/> representing the <paramref name="list"/>'s contents, and clears the <paramref name="list"/>.
	/// </summary>
	/// <typeparam name="T">Element type</typeparam>
	/// <param name="list">List to fetch/clear</param>
	/// <returns><seealso cref="ReadOnlySpan{T}"/> representing the <paramref name="list"/>'s contents</returns>
	internal static ReadOnlySpan<T> ExchangeClear<T>(this List<T> list) {
		T[] result;
		if (!ListReflectImpl<T>.Enabled) {
			result = list.ToArray();
			list.Clear();
			return result;
		}

		result = ListReflectImpl<T>.GetItems(list);
		int count = list.Count;
		ListReflectImpl<T>.SetItems(list, Array.Empty<T>());
		ListReflectImpl<T>.SetSize(list, 0);
		return result.AsReadOnlySpan(0, count);
	}

	/// <summary>
	/// Returns a <seealso cref="ReadOnlySpan{T}"/> representing the <paramref name="list"/>'s contents, and clears the <paramref name="list"/>.
	/// <para>
	/// The <paramref name="list"/> is <see langword="lock"/>ed during any operations on it.
	/// </para>
	/// </summary>
	/// <typeparam name="T">Element type</typeparam>
	/// <param name="list">List to fetch/clear</param>
	/// <returns><seealso cref="ReadOnlySpan{T}"/> representing the <paramref name="list"/>'s contents</returns>
	internal static ReadOnlySpan<T> ExchangeClearLocked<T>(this List<T> list) {
		T[] result;
		if (!ListReflectImpl<T>.Enabled) {
			lock (list) {
				if (list.Count == 0) {
					return default;
				}
				result = list.ToArray();
				list.Clear();
			}

			return result;
		}

		int count;
		lock (list) {
			count = list.Count;
			if (count == 0) {
				return default;
			}
			result = ListReflectImpl<T>.GetItems(list);
			ListReflectImpl<T>.SetItems(list, Array.Empty<T>());
			ListReflectImpl<T>.SetSize(list, 0);
		}

		return result.AsReadOnlySpan(0, count);
	}
	#endregion

	#region TryAt

	[DoesNotReturn]
	[MethodImpl(MethodImplOptions.NoInlining)]
	private static T ThrowIndexLessThanZeroException<T, U>(string name, int value, out U? item) =>
		throw new ArgumentOutOfRangeException(name, $"{value} is less than zero");

	internal static bool TryAt<T>(this List<T> list, int index, out T? item) {
		if (index < 0) {
			return ThrowIndexLessThanZeroException<bool, T>(nameof(index), index, out item);
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
			return ThrowIndexLessThanZeroException<bool, T>(nameof(index), index, out item); ;
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
			return ThrowIndexLessThanZeroException<bool, T>(nameof(index), index, out item);
		}
		if (index >= array.Length) {
			item = default(T);
			return false;
		}

		item = array[index];
		return true;
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
