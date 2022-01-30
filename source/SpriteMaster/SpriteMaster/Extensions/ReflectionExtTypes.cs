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
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace SpriteMaster.Extensions;

static partial class ReflectionExt {
	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static int TypeSize<T>(this T obj) where T : struct => Marshal.SizeOf((typeof(T) is Type) ? typeof(T) : obj.GetType());

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static int Size(this Type type) => Marshal.SizeOf(type);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static T AddRef<T>(this T type) where T : Type => (type.MakeByRefType() as T)!;

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static T RemoveRef<T>(this T type) where T : Type => ((type.IsByRef ? type.GetElementType() : type) as T)!;

	internal static string GetFullName(this MethodBase method) => method.DeclaringType is null ? method.Name : $"{method.DeclaringType.Name}::{method.Name}";

	internal static string? GetCurrentMethodName() => MethodBase.GetCurrentMethod()?.GetFullName();

	[MethodImpl(Runtime.MethodImpl.Hot)]
	[Obsolete("Non-performant: is uncached/non-delegate")]
	internal static T CreateInstance<T>(this Type _) => Activator.CreateInstance<T>();

	[MethodImpl(Runtime.MethodImpl.Hot)]
	[Obsolete("Non-performant: is uncached/non-delegate")]
	internal static T? CreateInstance<T>(this Type _, params object[] parameters) => (T?)Activator.CreateInstance(typeof(T), parameters);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	[Obsolete("Non-performant: is uncached/non-delegate")]
	internal static T CreateInstance<T>(this Type<T> _) => Activator.CreateInstance<T>();

	[MethodImpl(Runtime.MethodImpl.Hot)]
	[Obsolete("Non-performant: is uncached/non-delegate")]
	internal static T? CreateInstance<T>(this Type<T> _, params object[] parameters) => (T?)Activator.CreateInstance(typeof(T), parameters);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	[Obsolete("Non-performant: is uncached/non-delegate")]
	internal static T CreateInstance<T>() => Activator.CreateInstance<T>();

	[MethodImpl(Runtime.MethodImpl.Hot)]
	[Obsolete("Non-performant: is uncached/non-delegate")]
	internal static T? CreateInstance<T>(params object[] parameters) => (T?)Activator.CreateInstance(typeof(T), parameters);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	[Obsolete("Non-performant: is uncached/non-delegate")]
	internal static T? Invoke<T>(this MethodInfo method, object obj) => (T?)method.Invoke(obj, Arrays<object>.Empty);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	[Obsolete("Non-performant: is uncached/non-delegate")]
	internal static T? Invoke<T>(this MethodInfo method, object obj, params object[] args) => (T?)method.Invoke(obj, args);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	[Obsolete("Non-performant: is uncached/non-delegate")]
	internal static T? InvokeMethod<T>(this object obj, MethodInfo method) => (T?)method.Invoke(obj, Arrays<object>.Empty);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	[Obsolete("Non-performant: is uncached/non-delegate")]
	internal static T? InvokeMethod<T>(this object obj, MethodInfo method, params object[] args) => (T?)method.Invoke(obj, args);
}
