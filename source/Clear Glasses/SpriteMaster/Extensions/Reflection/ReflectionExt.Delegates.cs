/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aurpine/Stardew-SpriteMaster
**
*************************************************/

using System;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace SpriteMaster.Extensions.Reflection;

internal static partial class ReflectionExt {
	private const BindingFlags InstanceFlags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.FlattenHierarchy;
	private const BindingFlags StaticFlags = BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.FlattenHierarchy;

	private const BindingFlags ShallowInstanceFlags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;
	private const BindingFlags ShallowStaticFlags = BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public;

	#region GetMemberMeow (instance)
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static Action<TObject, TMember>? GetMemberSetter<TObject, TMember>(this Type type, string name) => GetMemberSetter<TObject, TMember>(type, type.GetPropertyOrField(name, InstanceFlags));

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static Action<TObject, TMember>? GetMemberSetter<TObject, TMember>(this Type type, MemberInfo? member) {
		switch (member) {
			case FieldInfo field:
				return GetFieldSetter<TObject, TMember>(type, field);
			case PropertyInfo property:
				return GetPropertySetter<TObject, TMember>(type, property);
			default:
				return null;
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static Func<TObject, TMember>? GetMemberGetter<TObject, TMember>(this Type type, string name) => GetMemberGetter<TObject, TMember>(type, type.GetPropertyOrField(name, InstanceFlags));

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static Func<TObject, TMember>? GetMemberGetter<TObject, TMember>(this Type type, MemberInfo? member) {
		switch (member) {
			case FieldInfo field:
				return GetFieldGetter<TObject, TMember>(type, field);
			case PropertyInfo property:
				return GetPropertyGetter<TObject, TMember>(type, property);
			default:
				return null;
		}
	}
	#endregion

	#region GetMemberMeow (instance, bound)
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static Action<TMember>? GetMemberSetter<TObject, TMember>(this Type type, string name, TObject target) => GetMemberSetter<TObject, TMember>(type, type.GetPropertyOrField(name, InstanceFlags), target);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static Action<TMember>? GetMemberSetter<TObject, TMember>(this Type type, MemberInfo? member, TObject target) {
		switch (member) {
			case FieldInfo field:
				return GetFieldSetter<TObject, TMember>(type, field, target);
			case PropertyInfo property:
				return GetPropertySetter<TObject, TMember>(type, property, target);
			default:
				return null;
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static Func<TMember>? GetMemberGetter<TObject, TMember>(this Type type, string name, TObject target) => GetMemberGetter<TObject, TMember>(type, type.GetPropertyOrField(name, InstanceFlags), target);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static Func<TMember>? GetMemberGetter<TObject, TMember>(this Type type, MemberInfo? member, TObject target) {
		switch (member) {
			case FieldInfo field:
				return GetFieldGetter<TObject, TMember>(type, field, target);
			case PropertyInfo property:
				return GetPropertyGetter<TObject, TMember>(type, property, target);
			default:
				return null;
		}
	}
	#endregion

	#region GetMemberMeow (static)
	internal static Action<TMember>? GetMemberSetter<TMember>(this Type type, string name) => GetMemberSetter<TMember>(type, type.GetPropertyOrField(name, InstanceFlags));

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static Action<TMember>? GetMemberSetter<TMember>(this Type type, MemberInfo? member) =>
		GetMemberSetter<TMember>(member);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static Action<TMember>? GetMemberSetter<TMember>(this MemberInfo? member) {
		switch (member) {
			case FieldInfo field:
				return GetFieldSetter<TMember>(field);
			case PropertyInfo property:
				return GetPropertySetter<TMember>(property);
			default:
				return null;
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static Func<TMember>? GetMemberGetter<TMember>(this Type type, string name) => GetMemberGetter<TMember>(type, type.GetPropertyOrField(name, InstanceFlags));

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static Func<TMember>? GetMemberGetter<TMember>(this Type type, MemberInfo? member) =>
		GetMemberGetter<TMember>(member);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static Func<TMember>? GetMemberGetter<TMember>(this MemberInfo? member) {
		switch (member) {
			case FieldInfo field:
				return GetFieldGetter<TMember>(field);
			case PropertyInfo property:
				return GetPropertyGetter<TMember>(property);
			default:
				return null;
		}
	}
	#endregion
}
