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
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

using static SpriteMaster.Runtime;

namespace SpriteMaster.Extensions;

static class Collections {
	[MethodImpl(MethodImpl.Hot)]
	internal static V GetOrAddDefault<K, V>(this Dictionary<K, V> dictionary, K key, Func<V> defaultGetter) {
		if (dictionary.TryGetValue(key, out V value)) {
			return value;
		}
		var newValue = defaultGetter.Invoke();
		dictionary.Add(key, newValue);
		return newValue;
	}

	/// <summary>
	/// Returns a new List that is constructed from the array.
	/// </summary>
	[MethodImpl(MethodImpl.Hot)]
	internal static List<T> ToList<T>(this T[] array) => new List<T>(array);

	// TODO : define me for .NET and .NETfx
	private static class ListReflectImpl<T> {
		internal static readonly Action<List<T>, T[]> ListSetItems = GetCompiledLambda<T[]>("_items");
		internal static readonly Action<List<T>, int> ListSetSize = GetCompiledLambda<int>("_size");

		static Action<List<T>, U> GetCompiledLambda<U>(string name) {
			var field = typeof(List<T>).GetField(name, BindingFlags.Instance | BindingFlags.NonPublic);
			var objExp = Expression.Parameter(typeof(List<T>), "object");
			var valueExp = Expression.Parameter(field.FieldType, "value");
			var fieldExp = Expression.Field(objExp, field);
			var assignExp = Expression.Assign(fieldExp, valueExp);
			return Expression.Lambda<Action<List<T>, U>>(assignExp, tailCall: true, objExp, valueExp).Compile();
		}
	}

	/// <summary>
	/// Returns a new List that contains the same elements as the array.
	/// <para>Warning: it is not safe to use the array afterwards - the List now owns it.</para>
	/// </summary>
	[MethodImpl(MethodImpl.Hot)]
	internal static List<T> BeList<T>(this T[] array) {
		var newList = new List<T>();
		ListReflectImpl<T>.ListSetItems(newList, array);
		ListReflectImpl<T>.ListSetSize(newList, array.Length);
		return newList;
	}

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
}
