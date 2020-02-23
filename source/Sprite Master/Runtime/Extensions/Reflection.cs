using SpriteMaster.Types;
using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace SpriteMaster.Extensions {
	public static class Reflection {
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int TypeSize<T> (this T obj) {
			if (typeof(T) is Type) {
				return Marshal.SizeOf(typeof(T));
			}
			return Marshal.SizeOf(obj.GetType());
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int Size (this Type type) {
			return Marshal.SizeOf(type);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static T AddRef<T> (this T type) where T : Type {
			return (T)type.MakeByRefType();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static T RemoveRef<T> (this T type) where T : Type {
			return (T)(type.IsByRef ? type.GetElementType() : type);
		}

		public static string GetFullName (this MethodBase method) {
			return method.DeclaringType.Name + "::" + method.Name;
		}

		public static string GetCurrentMethodName () {
			return MethodBase.GetCurrentMethod().GetFullName();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static T GetValue<T> (this FieldInfo field, object instance) {
			var result = field.GetValue(instance);
			return (T)result;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool GetAttribute<T> (this MemberInfo member, out T attribute) where T : Attribute {
			attribute = member.GetCustomAttribute<T>();
			return attribute != null;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool TryGetField (this Type type, string name, out FieldInfo field, BindingFlags bindingAttr = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic) {
			field = type.GetField(name, bindingAttr);
			return (field != null);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool TryGetProperty (this Type type, string name, out PropertyInfo property, BindingFlags bindingAttr = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic) {
			property = type.GetProperty(name, bindingAttr);
			return (property != null);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool TryGetMethod (this Type type, string name, out MethodInfo method, BindingFlags bindingAttr = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic) {
			method = type.GetMethod(name, bindingAttr);
			return (method != null);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static object GetField (this object obj, string name) {
			return obj?.GetType().GetField(
				name,
				BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.FlattenHierarchy
			)?.GetValue(obj);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static object GetProperty (this object obj, string name) {
			return obj?.GetType().GetProperty(
				name,
				BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.FlattenHierarchy
			)?.GetValue(obj);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static T CreateInstance<T> (this Type _) {
			return Activator.CreateInstance<T>();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static T CreateInstance<T> (this Type _, params object[] parameters) {
			return (T)Activator.CreateInstance(typeof(T), parameters);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static T CreateInstance<T> (this Type<T> _) {
			return Activator.CreateInstance<T>();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static T CreateInstance<T> (this Type<T> _, params object[] parameters) {
			return (T)Activator.CreateInstance(typeof(T), parameters);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static T CreateInstance<T> () {
			return Activator.CreateInstance<T>();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static T CreateInstance<T> (params object[] parameters) {
			return (T)Activator.CreateInstance(typeof(T), parameters);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static T Invoke<T> (this MethodInfo method, object obj) {
			return (T)method.Invoke(obj, Arrays<object>.Empty);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static T Invoke<T> (this MethodInfo method, object obj, params object[] args) {
			return (T)method.Invoke(obj, args);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static T InvokeMethod<T> (this object obj, MethodInfo method) {
			return (T)method.Invoke(obj, Arrays<object>.Empty);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static T InvokeMethod<T> (this object obj, MethodInfo method, params object[] args) {
			return (T)method.Invoke(obj, args);
		}
	}
}
