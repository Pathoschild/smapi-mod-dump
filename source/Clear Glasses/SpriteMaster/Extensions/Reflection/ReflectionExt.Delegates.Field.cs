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
	#region GetFieldMeow (instance)
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static Action<TObject, TMember>? GetFieldSetter<TObject, TMember>(this Type type, string name) => GetFieldSetter<TObject, TMember>(type, type.GetField(name, InstanceFlags));

	internal static Action<TObject, TMember>? GetFieldSetter<TObject, TMember>(this Type type, FieldInfo? field) {
		if (field is null) {
			return null;
		}

		var objExp = Expression.Parameter(type, "object");
		var valueExp = Expression.Parameter(typeof(TMember), "value");
		var convertedValueExp = Expression.Convert(valueExp, field.FieldType);

		var memberExp = Expression.Field(objExp, field);
		Expression assignExp = Expression.Assign(memberExp, convertedValueExp);
		if (!field.FieldType.IsClass) {
			assignExp = Expression.Convert(assignExp, typeof(object));
		}
		return Expression.Lambda<Action<TObject, TMember>>(assignExp, objExp, valueExp).CompileFast();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static Func<TObject, TMember>? GetFieldGetter<TObject, TMember>(this Type type, string name) => GetFieldGetter<TObject, TMember>(type, type.GetField(name, InstanceFlags));

	internal static Func<TObject, TMember>? GetFieldGetter<TObject, TMember>(this Type type, FieldInfo? field) {
		if (field is null) {
			return null;
		}

		var objExp = Expression.Parameter(type, "object");
		Expression memberExp = Expression.Field(objExp, field);
		return Expression.Lambda<Func<TObject, TMember>>(memberExp, objExp).CompileFast();
	}
	#endregion

	#region GetFieldMeow (instance, bound)
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static Action<TMember>? GetFieldSetter<TObject, TMember>(this Type type, string name, TObject target) => GetFieldSetter<TObject, TMember>(type, type.GetField(name, InstanceFlags), target);

	internal static Action<TMember>? GetFieldSetter<TObject, TMember>(this Type type, FieldInfo? field, TObject target) {
		if (field is null) {
			return null;
		}

		var objExp = Expression.Constant(target);
		var valueExp = Expression.Parameter(typeof(TMember), "value");
		var convertedValueExp = Expression.Convert(valueExp, field.FieldType);

		var memberExp = Expression.Field(objExp, field);
		Expression assignExp = Expression.Assign(memberExp, convertedValueExp);
		if (!field.FieldType.IsClass) {
			assignExp = Expression.Convert(assignExp, typeof(object));
		}
		return Expression.Lambda<Action<TMember>>(assignExp, valueExp).CompileFast();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static Func<TMember>? GetFieldGetter<TObject, TMember>(this Type type, string name, TObject target) => GetFieldGetter<TObject, TMember>(type, type.GetField(name, InstanceFlags), target);

	internal static Func<TMember>? GetFieldGetter<TObject, TMember>(this Type type, FieldInfo? field, TObject target) {
		if (field is null) {
			return null;
		}

		var objExp = Expression.Constant(target);
		Expression memberExp = Expression.Field(objExp, field);
		return Expression.Lambda<Func<TMember>>(memberExp).CompileFast();
	}
	#endregion

	#region GetFieldMeow (static)
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static Action<TMember>? GetFieldSetter<TMember>(this Type type, string name) => GetFieldSetter<TMember>(type, type.GetField(name, StaticFlags));

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static Action<TMember>? GetFieldSetter<TMember>(this Type type, FieldInfo? field) =>
		GetFieldSetter<TMember>(field);

	internal static Action<TMember>? GetFieldSetter<TMember>(this FieldInfo? field) {
		if (field is null) {
			return null;
		}

		var valueExp = Expression.Parameter(typeof(TMember), "value");
		var convertedValueExp = Expression.Convert(valueExp, field.FieldType);

		var memberExp = Expression.Field(field);
		Expression assignExp = Expression.Assign(memberExp, convertedValueExp);
		if (!field.FieldType.IsClass) {
			assignExp = Expression.Convert(assignExp, typeof(object));
		}
		return Expression.Lambda<Action<TMember>>(assignExp, valueExp).CompileFast();
	}

	internal static Action<TMember>? GetSetter<TMember>(this FieldInfo? field) =>
		GetFieldSetter<TMember>(field);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static Func<TMember>? GetFieldGetter<TMember>(this Type type, string name) => GetFieldGetter<TMember>(type, type.GetField(name, StaticFlags));

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static Func<TMember>? GetFieldGetter<TMember>(this Type type, FieldInfo? field) =>
		GetFieldGetter<TMember>(field);

	internal static Func<TMember>? GetFieldGetter<TMember>(this FieldInfo? field) {
		if (field is null) {
			return null;
		}

		Expression memberExp = Expression.Field(field);
		if (!field.FieldType.IsClass) {
			memberExp = Expression.Convert(memberExp, typeof(object));
		}
		return Expression.Lambda<Func<TMember>>(memberExp).CompileFast();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static Func<TMember>? GetGetter<TMember>(this FieldInfo? field) =>
		GetFieldGetter<TMember>(field);
	#endregion
}
