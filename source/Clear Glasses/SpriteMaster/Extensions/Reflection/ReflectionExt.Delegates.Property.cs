/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aurpine/Stardew-SpriteMaster
**
*************************************************/

using FastExpressionCompiler.LightExpression;
using System;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace SpriteMaster.Extensions.Reflection;

internal static partial class ReflectionExt {
	#region GetPropertyMeow (instance)
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static Action<TObject, TMember>? GetPropertySetter<TObject, TMember>(this Type type, string name) => GetPropertySetter<TObject, TMember>(type, type.GetProperty(name, InstanceFlags));

	internal static Action<TObject, TMember>? GetPropertySetter<TObject, TMember>(this Type type, PropertyInfo? property) {
		if (property is null) {
			return null;
		}

		var objExp = Expression.Parameter(type, "object");
		var valueExp = Expression.Parameter(property.PropertyType, "value");
		var memberExp = Expression.Property(objExp, property);
		var assignExp = Expression.Assign(memberExp, valueExp);
		return Expression.Lambda<Action<TObject, TMember>>(assignExp, objExp, valueExp).CompileFast();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static Func<TObject, TMember>? GetPropertyGetter<TObject, TMember>(this Type type, string name) => GetPropertyGetter<TObject, TMember>(type, type.GetProperty(name, InstanceFlags));

	internal static Func<TObject, TMember>? GetPropertyGetter<TObject, TMember>(this Type type, PropertyInfo? property) {
		if (property is null) {
			return null;
		}

		var objExp = Expression.Parameter(type, "object");
		Expression memberExp = Expression.Property(objExp, property);
		return Expression.Lambda<Func<TObject, TMember>>(memberExp, objExp).CompileFast();
	}
	#endregion

	#region GetPropertyMeow (instance, bound)
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static Action<TMember>? GetPropertySetter<TObject, TMember>(this Type type, string name, TObject target) => GetPropertySetter<TObject, TMember>(type, type.GetProperty(name, InstanceFlags), target);

	internal static Action<TMember>? GetPropertySetter<TObject, TMember>(this Type type, PropertyInfo? property, TObject target) {
		if (property is null) {
			return null;
		}

		var objExp = Expression.Constant(target);
		var valueExp = Expression.Parameter(property.PropertyType, "value");
		var memberExp = Expression.Property(objExp, property);
		var assignExp = Expression.Assign(memberExp, valueExp);
		return Expression.Lambda<Action<TMember>>(assignExp, valueExp).CompileFast();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static Func<TMember>? GetPropertyGetter<TObject, TMember>(this Type type, string name, TObject target) => GetPropertyGetter<TObject, TMember>(type, type.GetProperty(name, InstanceFlags), target);

	internal static Func<TMember>? GetPropertyGetter<TObject, TMember>(this Type type, PropertyInfo? property, TObject target) {
		if (property is null) {
			return null;
		}

		var objExp = Expression.Constant(target);
		Expression memberExp = Expression.Property(objExp, property);
		return Expression.Lambda<Func<TMember>>(memberExp).CompileFast();
	}
	#endregion

	#region GetPropertyMeow (static)
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static Action<TMember>? GetPropertySetter<TMember>(this Type type, string name) => GetPropertySetter<TMember>(type, type.GetProperty(name, StaticFlags));

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static Action<TMember>? GetPropertySetter<TMember>(this Type type, PropertyInfo? property) =>
		GetPropertySetter<TMember>(property);

	internal static Action<TMember>? GetPropertySetter<TMember>(this PropertyInfo? property) {
		if (property is null) {
			return null;
		}

		var valueExp = Expression.Parameter(typeof(TMember), "value");
		var convertedValueExp = Expression.Convert(valueExp, property.PropertyType);

		var memberExp = Expression.Property(property);
		Expression assignExp = Expression.Assign(memberExp, convertedValueExp);
		if (!property.PropertyType.IsClass) {
			assignExp = Expression.Convert(assignExp, typeof(object));
		}
		return Expression.Lambda<Action<TMember>>(assignExp, valueExp).CompileFast();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static Action<TMember>? GetSetter<TMember>(this PropertyInfo? property) =>
		GetPropertySetter<TMember>(property);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static Func<TMember>? GetPropertyGetter<TMember>(this Type type, string name) => GetPropertyGetter<TMember>(type, type.GetProperty(name, StaticFlags));

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static Func<TMember>? GetPropertyGetter<TMember>(this Type type, PropertyInfo? property) =>
		GetPropertyGetter<TMember>(property);

	internal static Func<TMember>? GetPropertyGetter<TMember>(this PropertyInfo? property) {
		if (property is null) {
			return null;
		}

		Expression memberExp = Expression.Property(property);
		if (!property.PropertyType.IsClass) {
			memberExp = Expression.Convert(memberExp, typeof(object));
		}
		return Expression.Lambda<Func<TMember>>(memberExp).CompileFast();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static Func<TMember>? GetGetter<TMember>(this PropertyInfo? property) =>
		GetPropertyGetter<TMember>(property);
	#endregion
}
