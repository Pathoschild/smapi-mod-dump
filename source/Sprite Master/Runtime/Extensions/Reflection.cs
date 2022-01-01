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

static class Reflection {
	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static int TypeSize<T>(this T obj) => Marshal.SizeOf((typeof(T) is Type) ? typeof(T) : obj.GetType());

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static int Size(this Type type) => Marshal.SizeOf(type);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static T AddRef<T>(this T type) where T : Type => (T)type.MakeByRefType();

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static T RemoveRef<T>(this T type) where T : Type => (T)(type.IsByRef ? type.GetElementType() : type);

	internal static string GetFullName(this MethodBase method) => $"{method.DeclaringType.Name}::{method.Name}";

	internal static string GetCurrentMethodName() => MethodBase.GetCurrentMethod().GetFullName();

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static T GetValue<T>(this FieldInfo field, object instance) => (T)field.GetValue(instance);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static bool HasAttribute<T>(this MemberInfo member) where T : Attribute => member.GetCustomAttribute<T>() is not null;

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static bool GetAttribute<T>(this MemberInfo member, out T attribute) where T : Attribute {
		attribute = member.GetCustomAttribute<T>();
		return attribute is not null;
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static bool TryGetField(this Type type, string name, out FieldInfo field, BindingFlags bindingAttr = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic) {
		field = type.GetField(name, bindingAttr);
		return field is not null;
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static bool TryGetProperty(this Type type, string name, out PropertyInfo property, BindingFlags bindingAttr = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic) {
		property = type.GetProperty(name, bindingAttr);
		return property is not null;
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static bool TryGetMethod(this Type type, string name, out MethodInfo method, BindingFlags bindingAttr = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic) {
		method = type.GetMethod(name, bindingAttr);
		return method is not null;
	}

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static object GetField(this object obj, string name) => obj?.GetType().GetField(
		name,
		BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.FlattenHierarchy
	)?.GetValue(obj);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static object GetProperty(this object obj, string name) => obj?.GetType().GetProperty(
		name,
		BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.FlattenHierarchy
	)?.GetValue(obj);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static T CreateInstance<T>(this Type _) => Activator.CreateInstance<T>();

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static T CreateInstance<T>(this Type _, params object[] parameters) => (T)Activator.CreateInstance(typeof(T), parameters);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static T CreateInstance<T>(this Type<T> _) => Activator.CreateInstance<T>();

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static T CreateInstance<T>(this Type<T> _, params object[] parameters) => (T)Activator.CreateInstance(typeof(T), parameters);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static T CreateInstance<T>() => Activator.CreateInstance<T>();

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static T CreateInstance<T>(params object[] parameters) => (T)Activator.CreateInstance(typeof(T), parameters);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static T Invoke<T>(this MethodInfo method, object obj) => (T)method.Invoke(obj, Arrays<object>.Empty);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static T Invoke<T>(this MethodInfo method, object obj, params object[] args) => (T)method.Invoke(obj, args);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static T InvokeMethod<T>(this object obj, MethodInfo method) => (T)method.Invoke(obj, Arrays<object>.Empty);

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal static T InvokeMethod<T>(this object obj, MethodInfo method, params object[] args) => (T)method.Invoke(obj, args);
}
