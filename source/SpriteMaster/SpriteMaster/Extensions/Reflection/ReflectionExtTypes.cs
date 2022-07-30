/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

using SpriteMaster.Types;
using System;
using System.Collections.Concurrent;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace SpriteMaster.Extensions.Reflection;

internal static partial class ReflectionExt {
	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static int TypeSize<T>(this T obj) where T : struct {
		Marshal.SizeOf<T>().AssertEqual(Unsafe.SizeOf<T>());
		return Unsafe.SizeOf<T>();
	}


	private static readonly ConcurrentDictionary<Type, int> MarshalSizeCache = new();

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static int Size(this Type type) => MarshalSizeCache.GetOrAdd(type, Marshal.SizeOf);

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static T AddRef<T>(this T type) where T : Type => (type.MakeByRefType() as T)!;

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static T RemoveRef<T>(this T type) where T : Type => ((type.IsByRef ? type.GetElementType() : type) as T)!;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static string GetFullName(this MethodBase method) => method.DeclaringType is null ? method.Name : $"{method.DeclaringType.Name}::{method.Name}";

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static string? GetCurrentMethodName() => MethodBase.GetCurrentMethod()?.GetFullName();

	[MethodImpl(Runtime.MethodImpl.Inline)]
	[Obsolete("Non-performant: is uncached/non-delegate")]
	internal static T CreateInstance<T>(this Type _) => Activator.CreateInstance<T>();

	[MethodImpl(Runtime.MethodImpl.Inline)]
	[Obsolete("Non-performant: is uncached/non-delegate")]
	internal static T? CreateInstance<T>(this Type _, params object[] parameters) => (T?)Activator.CreateInstance(typeof(T), parameters);

	[MethodImpl(Runtime.MethodImpl.Inline)]
	[Obsolete("Non-performant: is uncached/non-delegate")]
	internal static T CreateInstance<T>() => Activator.CreateInstance<T>();

	[MethodImpl(Runtime.MethodImpl.Inline)]
	[Obsolete("Non-performant: is uncached/non-delegate")]
	internal static T? CreateInstance<T>(params object[] parameters) => (T?)Activator.CreateInstance(typeof(T), parameters);

	[MethodImpl(Runtime.MethodImpl.Inline)]
	[Obsolete("Non-performant: is uncached/non-delegate")]
	internal static T? Invoke<T>(this MethodInfo method, object obj) => (T?)method.Invoke(obj, Arrays<object>.Empty);

	[MethodImpl(Runtime.MethodImpl.Inline)]
	[Obsolete("Non-performant: is uncached/non-delegate")]
	internal static T? Invoke<T>(this MethodInfo method, object obj, params object[] args) => (T?)method.Invoke(obj, args);

	[MethodImpl(Runtime.MethodImpl.Inline)]
	[Obsolete("Non-performant: is uncached/non-delegate")]
	internal static T? InvokeMethod<T>(this object obj, MethodInfo method) => (T?)method.Invoke(obj, Arrays<object>.Empty);

	[MethodImpl(Runtime.MethodImpl.Inline)]
	[Obsolete("Non-performant: is uncached/non-delegate")]
	internal static T? InvokeMethod<T>(this object obj, MethodInfo method, params object[] args) => (T?)method.Invoke(obj, args);

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static Type? GetTypeExt(string typeName) {
		if (Type.GetType(typeName) is { } localType) {
			return localType;
		}

		foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies()) {
			if (assembly.GetType(typeName) is { } type) {
				return type;
			}
		}

		return null;
	}
}
