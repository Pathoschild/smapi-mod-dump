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
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace SpriteMaster.Types;

static class TypeExt {
	[Pure, MethodImpl(Runtime.MethodImpl.Hot)]
	internal static Type<T> GetTypeT<T>(this T _) => Type<T>.This;

	[Pure, MethodImpl(Runtime.MethodImpl.Hot)]
	internal static Type<T> From<T>() => Type<T>.This;
}

sealed class Type<T> {
	internal static readonly Type<T> This = new();
	internal static readonly Type UnderlyingType = typeof(T);

	public static implicit operator Type(Type<T> type) => Type<T>.UnderlyingType;
	public static implicit operator Type<T>(Type type) => Type<T>.This;

	// TODO : implements equals, comparable, ==, !=, hashcode, etc

	public Guid GUID => typeof(T).GUID;

	public Module Module => typeof(T).Module;

	public Assembly Assembly => typeof(T).Assembly;

	public string? FullName => typeof(T).FullName;

	public string? Namespace => typeof(T).Namespace;

	public string? AssemblyQualifiedName => typeof(T).AssemblyQualifiedName;

	public Type? BaseType => typeof(T).BaseType;

	public Type UnderlyingSystemType => typeof(T).UnderlyingSystemType;

	public string Name => typeof(T).Name;

	[MethodImpl(Runtime.MethodImpl.Hot)]
	private Type() { }

	[Pure, MethodImpl(Runtime.MethodImpl.Hot)]
	public ConstructorInfo[] GetConstructors(BindingFlags bindingAttr) => typeof(T).GetConstructors(bindingAttr);

	[Pure, MethodImpl(Runtime.MethodImpl.Hot)]
	public object[] GetCustomAttributes(bool inherit) => typeof(T).GetCustomAttributes(inherit);

	[Pure, MethodImpl(Runtime.MethodImpl.Hot)]
	public object[] GetCustomAttributes(Type attributeType, bool inherit) => typeof(T).GetCustomAttributes(attributeType, inherit);

	[Pure, MethodImpl(Runtime.MethodImpl.Hot)]
	public Type? GetElementType() => typeof(T).GetElementType();

	[Pure, MethodImpl(Runtime.MethodImpl.Hot)]
	public EventInfo? GetEvent(string name, BindingFlags bindingAttr) => typeof(T).GetEvent(name, bindingAttr);

	[Pure, MethodImpl(Runtime.MethodImpl.Hot)]
	public EventInfo[] GetEvents(BindingFlags bindingAttr) => typeof(T).GetEvents(bindingAttr);

	[Pure, MethodImpl(Runtime.MethodImpl.Hot)]
	public FieldInfo? GetField(string name, BindingFlags bindingAttr) => typeof(T).GetField(name, bindingAttr);

	[Pure, MethodImpl(Runtime.MethodImpl.Hot)]
	public FieldInfo[] GetFields(BindingFlags bindingAttr) => typeof(T).GetFields(bindingAttr);

	[Pure, MethodImpl(Runtime.MethodImpl.Hot)]
	public Type? GetInterface(string name, bool ignoreCase) => typeof(T).GetInterface(name, ignoreCase);

	[Pure, MethodImpl(Runtime.MethodImpl.Hot)]
	public Type[] GetInterfaces() => typeof(T).GetInterfaces();

	[Pure, MethodImpl(Runtime.MethodImpl.Hot)]
	public MemberInfo[] GetMembers(BindingFlags bindingAttr) => typeof(T).GetMembers(bindingAttr);

	[Pure, MethodImpl(Runtime.MethodImpl.Hot)]
	public MethodInfo[] GetMethods(BindingFlags bindingAttr) => typeof(T).GetMethods(bindingAttr);

	[Pure, MethodImpl(Runtime.MethodImpl.Hot)]
	public Type? GetNestedType(string name, BindingFlags bindingAttr) => typeof(T).GetNestedType(name, bindingAttr);

	[Pure, MethodImpl(Runtime.MethodImpl.Hot)]
	public Type[] GetNestedTypes(BindingFlags bindingAttr) => typeof(T).GetNestedTypes(bindingAttr);

	[Pure, MethodImpl(Runtime.MethodImpl.Hot)]
	public PropertyInfo[] GetProperties(BindingFlags bindingAttr) => typeof(T).GetProperties(bindingAttr);

	[Pure, MethodImpl(Runtime.MethodImpl.Hot)]
	public object? InvokeMember(string name, BindingFlags invokeAttr, Binder? binder, object? target, object?[]? args, ParameterModifier[]? modifiers, CultureInfo? culture, string[]? namedParameters) =>
		typeof(T).InvokeMember(name, invokeAttr, binder, target, args, modifiers, culture, namedParameters);

	[Pure, MethodImpl(Runtime.MethodImpl.Hot)]
	public bool IsDefined(Type attributeType, bool inherit) => typeof(T).IsDefined(attributeType, inherit);
}
