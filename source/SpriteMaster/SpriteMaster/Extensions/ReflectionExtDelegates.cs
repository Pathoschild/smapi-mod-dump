/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

using FastExpressionCompiler.LightExpression;
using System;
using System.Reflection;

namespace SpriteMaster.Extensions;

static partial class ReflectionExt {
	private const BindingFlags InstanceFlags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.FlattenHierarchy;
	private const BindingFlags StaticFlags = BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.FlattenHierarchy;

	#region GetFieldMeow (instance)
	internal static Action<T, U>? GetFieldSetter<T, U>(this Type type, string name) => GetFieldSetter<T, U>(type, type.GetField(name, InstanceFlags));

	internal static Action<T, U>? GetFieldSetter<T, U>(this Type type, FieldInfo? field) {
		if (field is null) {
			return null;
		}

		var objExp = Expression.Parameter(type, "object");
		var valueExp = Expression.Parameter(typeof(U), "value");
		var convertedValueExp = Expression.Convert(valueExp, field.FieldType);

		var memberExp = Expression.Field(objExp, field);
		Expression assignExp = Expression.Assign(memberExp, convertedValueExp);
		if (!field.FieldType.IsClass) {
			assignExp = Expression.Convert(assignExp, typeof(object));
		}
		return Expression.Lambda<Action<T, U>>(assignExp, objExp, valueExp).CompileFast();
	}

	internal static Func<T, U>? GetFieldGetter<T, U>(this Type type, string name) => GetFieldGetter<T, U>(type, type.GetField(name, InstanceFlags));

	internal static Func<T, U>? GetFieldGetter<T, U>(this Type type, FieldInfo? field) {
		if (field is null) {
			return null;
		}

		var objExp = Expression.Parameter(type, "object");
		Expression memberExp = Expression.Field(objExp, field);
		if (!field.FieldType.IsClass) {
			memberExp = Expression.Convert(memberExp, typeof(object));
		}
		return Expression.Lambda<Func<T, U>>(memberExp, objExp).CompileFast();
	}
	#endregion

	#region GetFieldMeow (static)
	internal static Action<U>? GetFieldSetter<U>(this Type type, string name) => GetFieldSetter<U>(type, type.GetField(name, StaticFlags));

	internal static Action<U>? GetFieldSetter<U>(this Type type, FieldInfo? field) {
		if (field is null) {
			return null;
		}

		var valueExp = Expression.Parameter(typeof(U), "value");
		var convertedValueExp = Expression.Convert(valueExp, field.FieldType);

		var memberExp = Expression.Field(field);
		Expression assignExp = Expression.Assign(memberExp, convertedValueExp);
		if (!field.FieldType.IsClass) {
			assignExp = Expression.Convert(assignExp, typeof(object));
		}
		return Expression.Lambda<Action<U>>(assignExp, valueExp).CompileFast();
	}

	internal static Func<U>? GetFieldGetter<U>(this Type type, string name) => GetFieldGetter<U>(type, type.GetField(name, StaticFlags));

	internal static Func<U>? GetFieldGetter<U>(this Type type, FieldInfo? field) {
		if (field is null) {
			return null;
		}

		Expression memberExp = Expression.Field(field);
		if (!field.FieldType.IsClass) {
			memberExp = Expression.Convert(memberExp, typeof(object));
		}
		return Expression.Lambda<Func<U>>(memberExp).CompileFast();
	}
	#endregion

	#region GetPropertyMeow (instance)
	internal static Action<T, U>? GetPropertySetter<T, U>(this Type type, string name) => GetPropertySetter<T, U>(type, type.GetProperty(name, InstanceFlags));

	internal static Action<T, U>? GetPropertySetter<T, U>(this Type type, PropertyInfo? property) {
		if (property is null) {
			return null;
		}

		var objExp = Expression.Parameter(type, "object");
		var valueExp = Expression.Parameter(property.PropertyType, "value");
		var memberExp = Expression.Property(objExp, property);
		var assignExp = Expression.Assign(memberExp, valueExp);
		return Expression.Lambda<Action<T, U>>(assignExp, objExp, valueExp).CompileFast();
	}

	internal static Func<T, U>? GetPropertyGetter<T, U>(this Type type, string name) => GetPropertyGetter<T, U>(type, type.GetProperty(name, InstanceFlags));

	internal static Func<T, U>? GetPropertyGetter<T, U>(this Type type, PropertyInfo? property) {
		if (property is null) {
			return null;
		}

		var objExp = Expression.Parameter(type, "object");
		Expression memberExp = Expression.Property(objExp, property);
		if (!property.PropertyType.IsClass) {
			memberExp = Expression.Convert(memberExp, typeof(object));
		}
		return Expression.Lambda<Func<T, U>>(memberExp, objExp).CompileFast();
	}
	#endregion

	#region GetMemberMeow (instance)
	internal static Action<T, U>? GetMemberSetter<T, U>(this Type type, string name) => GetMemberSetter<T, U>(type, type.GetPropertyOrField(name, InstanceFlags));

	internal static Action<T, U>? GetMemberSetter<T, U>(this Type type, MemberInfo? member) {
		switch (member) {
			case FieldInfo field:
				return GetFieldSetter<T, U>(type, field);
			case PropertyInfo property:
				return GetPropertySetter<T, U>(type, property);
			default:
				return null;
		}
	}

	internal static Func<T, U>? GetMemberGetter<T, U>(this Type type, string name) => GetMemberGetter<T, U>(type, type.GetPropertyOrField(name, InstanceFlags));

	internal static Func<T, U>? GetMemberGetter<T, U>(this Type type, MemberInfo? member) {
		switch (member) {
			case FieldInfo field:
				return GetFieldGetter<T, U>(type, field);
			case PropertyInfo property:
				return GetPropertyGetter<T, U>(type, property);
			default:
				return null;
		}
	}
	#endregion
}
