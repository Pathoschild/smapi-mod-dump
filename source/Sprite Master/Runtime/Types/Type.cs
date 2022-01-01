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

static class TypeTExtensions {
	[Pure, MethodImpl(Runtime.MethodImpl.Hot)]
	internal static Type<T> GetTypeT<T>(this T _) => Type<T>.This;
}

sealed class Type<T> : Type {
	internal static readonly Type<T> This = new();
	internal static readonly Type UnderlyingType = typeof(T);

	// TODO : implements equals, comparable, ==, !=, hashcode, etc

	public override Guid GUID => typeof(T).GUID;

	public override Module Module => typeof(T).Module;

	public override Assembly Assembly => typeof(T).Assembly;

	public override string FullName => typeof(T).FullName;

	public override string Namespace => typeof(T).Namespace;

	public override string AssemblyQualifiedName => typeof(T).AssemblyQualifiedName;

	public override Type BaseType => typeof(T).BaseType;

	public override Type UnderlyingSystemType => typeof(T).UnderlyingSystemType;

	public override string Name => typeof(T).Name;

	[MethodImpl(Runtime.MethodImpl.Hot)]
	internal Type() : base() { }

	[Pure, MethodImpl(Runtime.MethodImpl.Hot)]
	public override ConstructorInfo[] GetConstructors(BindingFlags bindingAttr) => typeof(T).GetConstructors(bindingAttr);

	[Pure, MethodImpl(Runtime.MethodImpl.Hot)]
	public override object[] GetCustomAttributes(bool inherit) => typeof(T).GetCustomAttributes(inherit);

	[Pure, MethodImpl(Runtime.MethodImpl.Hot)]
	public override object[] GetCustomAttributes(Type attributeType, bool inherit) => typeof(T).GetCustomAttributes(attributeType, inherit);

	[Pure, MethodImpl(Runtime.MethodImpl.Hot)]
	public override Type GetElementType() => typeof(T).GetElementType();

	[Pure, MethodImpl(Runtime.MethodImpl.Hot)]
	public override EventInfo GetEvent(string name, BindingFlags bindingAttr) => typeof(T).GetEvent(name, bindingAttr);

	[Pure, MethodImpl(Runtime.MethodImpl.Hot)]
	public override EventInfo[] GetEvents(BindingFlags bindingAttr) => typeof(T).GetEvents(bindingAttr);

	[Pure, MethodImpl(Runtime.MethodImpl.Hot)]
	public override FieldInfo GetField(string name, BindingFlags bindingAttr) => typeof(T).GetField(name, bindingAttr);

	[Pure, MethodImpl(Runtime.MethodImpl.Hot)]
	public override FieldInfo[] GetFields(BindingFlags bindingAttr) => typeof(T).GetFields(bindingAttr);

	[Pure, MethodImpl(Runtime.MethodImpl.Hot)]
	public override Type GetInterface(string name, bool ignoreCase) => typeof(T).GetInterface(name, ignoreCase);

	[Pure, MethodImpl(Runtime.MethodImpl.Hot)]
	public override Type[] GetInterfaces() => typeof(T).GetInterfaces();

	[Pure, MethodImpl(Runtime.MethodImpl.Hot)]
	public override MemberInfo[] GetMembers(BindingFlags bindingAttr) => typeof(T).GetMembers(bindingAttr);

	[Pure, MethodImpl(Runtime.MethodImpl.Hot)]
	public override MethodInfo[] GetMethods(BindingFlags bindingAttr) => typeof(T).GetMethods(bindingAttr);

	[Pure, MethodImpl(Runtime.MethodImpl.Hot)]
	public override Type GetNestedType(string name, BindingFlags bindingAttr) => typeof(T).GetNestedType(name, bindingAttr);

	[Pure, MethodImpl(Runtime.MethodImpl.Hot)]
	public override Type[] GetNestedTypes(BindingFlags bindingAttr) => typeof(T).GetNestedTypes(bindingAttr);

	[Pure, MethodImpl(Runtime.MethodImpl.Hot)]
	public override PropertyInfo[] GetProperties(BindingFlags bindingAttr) => typeof(T).GetProperties(bindingAttr);

	[Pure, MethodImpl(Runtime.MethodImpl.Hot)]
	public override object InvokeMember(string name, BindingFlags invokeAttr, Binder binder, object target, object[] args, ParameterModifier[] modifiers, CultureInfo culture, string[] namedParameters) =>
		typeof(T).InvokeMember(name, invokeAttr, binder, target, args, modifiers, culture, namedParameters);

	[Pure, MethodImpl(Runtime.MethodImpl.Hot)]
	public override bool IsDefined(Type attributeType, bool inherit) => typeof(T).IsDefined(attributeType, inherit);

	[MethodImpl(Runtime.MethodImpl.Cold)]
	private static F GetPrivateMethod<F>(string name) where F : Delegate => typeof(T).GetMethod(name, BindingFlags.Instance | BindingFlags.NonPublic).CreateDelegate<F>(typeof(T));

	private delegate TypeAttributes GetAttributeFlagsDelegate();
	private static readonly Lazy<GetAttributeFlagsDelegate> GetAttributeFlagsImplRefl = new(() => GetPrivateMethod<GetAttributeFlagsDelegate>("GetAttributeFlagsImpl"));
	[Pure, MethodImpl(Runtime.MethodImpl.Hot)]
	protected override TypeAttributes GetAttributeFlagsImpl() => GetAttributeFlagsImplRefl.Value();

	private delegate ConstructorInfo GetConstructorDelegate(BindingFlags _0, Binder _1, CallingConventions _2, Type[] _3, ParameterModifier[] _4);
	private static readonly Lazy<GetConstructorDelegate> GetConstructorImplRefl = new(() => GetPrivateMethod<GetConstructorDelegate>("GetConstructorImpl"));
	[Pure, MethodImpl(Runtime.MethodImpl.Hot)]
	protected override ConstructorInfo GetConstructorImpl(BindingFlags bindingAttr, Binder binder, CallingConventions callConvention, Type[] types, ParameterModifier[] modifiers) =>
		GetConstructorImplRefl.Value(bindingAttr, binder, callConvention, types, modifiers);

	private delegate MethodInfo GetMethodDelegate(string _0, BindingFlags _1, Binder _2, CallingConventions _3, Type[] _4, ParameterModifier[] _5);
	private static readonly Lazy<GetMethodDelegate> GetMethodImplRefl = new(() => GetPrivateMethod<GetMethodDelegate>("GetMethodImpl"));
	[Pure, MethodImpl(Runtime.MethodImpl.Hot)]
	protected override MethodInfo GetMethodImpl(string name, BindingFlags bindingAttr, Binder binder, CallingConventions callConvention, Type[] types, ParameterModifier[] modifiers) =>
		GetMethodImplRefl.Value(name, bindingAttr, binder, callConvention, types, modifiers);

	private delegate PropertyInfo GetPropertyDelegate(string _0, BindingFlags _1, Binder _2, Type _3, Type[] _4, ParameterModifier[] _5);
	private static readonly Lazy<GetPropertyDelegate> GetPropertyImplRefl = new(() => GetPrivateMethod<GetPropertyDelegate>("GetPropertyImpl"));
	[Pure, MethodImpl(Runtime.MethodImpl.Hot)]
	protected override PropertyInfo GetPropertyImpl(string name, BindingFlags bindingAttr, Binder binder, Type returnType, Type[] types, ParameterModifier[] modifiers) =>
		GetPropertyImplRefl.Value(name, bindingAttr, binder, returnType, types, modifiers);

	private delegate bool BooleanGetter();

	private static readonly Lazy<BooleanGetter> HasElementTypeImplRefl = new(() => GetPrivateMethod<BooleanGetter>("HasElementTypeImpl"));
	[Pure, MethodImpl(Runtime.MethodImpl.Hot)]
	protected override bool HasElementTypeImpl() => HasElementTypeImplRefl.Value();

	private static readonly Lazy<BooleanGetter> IsArrayImplRefl = new(() => GetPrivateMethod<BooleanGetter>("IsArrayImpl"));
	[Pure, MethodImpl(Runtime.MethodImpl.Hot)]
	protected override bool IsArrayImpl() => IsArrayImplRefl.Value();

	private static readonly Lazy<BooleanGetter> IsByRefImplRefl = new(() => GetPrivateMethod<BooleanGetter>("IsByRefImpl"));
	[Pure, MethodImpl(Runtime.MethodImpl.Hot)]
	protected override bool IsByRefImpl() => IsByRefImplRefl.Value();

	private static readonly Lazy<BooleanGetter> IsCOMObjectImplRefl = new(() => GetPrivateMethod<BooleanGetter>("IsCOMObjectImpl"));
	[Pure, MethodImpl(Runtime.MethodImpl.Hot)]
	protected override bool IsCOMObjectImpl() => IsCOMObjectImplRefl.Value();

	private static readonly Lazy<BooleanGetter> IsPointerImplRefl = new(() => GetPrivateMethod<BooleanGetter>("IsPointerImpl"));
	[Pure, MethodImpl(Runtime.MethodImpl.Hot)]
	protected override bool IsPointerImpl() => IsPointerImplRefl.Value();

	private static readonly Lazy<BooleanGetter> IsPrimitiveImplRefl = new(() => GetPrivateMethod<BooleanGetter>("IsPrimitiveImpl"));
	[Pure, MethodImpl(Runtime.MethodImpl.Hot)]
	protected override bool IsPrimitiveImpl() => IsPrimitiveImplRefl.Value();
}
