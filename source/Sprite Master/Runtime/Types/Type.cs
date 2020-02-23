using SpriteMaster.Extensions;
using System;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace SpriteMaster.Types {
	public static class TypeTExtensions {
		public static Type<T> GetTypeT<T> (this T _) {
			return Type<T>.This;
		}
	}

	public sealed class Type<T> : Type {
		public static readonly Type<T> This = new Type<T>();
		public static readonly Type UnderlyingType = typeof(T);

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

		[Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override ConstructorInfo[] GetConstructors (BindingFlags bindingAttr) {
			return typeof(T).GetConstructors(bindingAttr);
		}

		[Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override object[] GetCustomAttributes (bool inherit) {
			return typeof(T).GetCustomAttributes(inherit);
		}

		[Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override object[] GetCustomAttributes (Type attributeType, bool inherit) {
			return typeof(T).GetCustomAttributes(attributeType, inherit);
		}

		[Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override Type GetElementType () {
			return typeof(T).GetElementType();
		}

		[Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override EventInfo GetEvent (string name, BindingFlags bindingAttr) {
			return typeof(T).GetEvent(name, bindingAttr);
		}

		[Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override EventInfo[] GetEvents (BindingFlags bindingAttr) {
			return typeof(T).GetEvents(bindingAttr);
		}

		[Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override FieldInfo GetField (string name, BindingFlags bindingAttr) {
			return typeof(T).GetField(name, bindingAttr);
		}

		[Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override FieldInfo[] GetFields (BindingFlags bindingAttr) {
			return typeof(T).GetFields(bindingAttr);
		}

		[Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override Type GetInterface (string name, bool ignoreCase) {
			return typeof(T).GetInterface(name, ignoreCase);
		}

		[Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override Type[] GetInterfaces () {
			return typeof(T).GetInterfaces();
		}

		[Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override MemberInfo[] GetMembers (BindingFlags bindingAttr) {
			return typeof(T).GetMembers(bindingAttr);
		}

		[Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override MethodInfo[] GetMethods (BindingFlags bindingAttr) {
			return typeof(T).GetMethods(bindingAttr);
		}

		[Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override Type GetNestedType (string name, BindingFlags bindingAttr) {
			return typeof(T).GetNestedType(name, bindingAttr);
		}

		[Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override Type[] GetNestedTypes (BindingFlags bindingAttr) {
			return typeof(T).GetNestedTypes(bindingAttr);
		}

		[Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override PropertyInfo[] GetProperties (BindingFlags bindingAttr) {
			return typeof(T).GetProperties(bindingAttr);
		}

		[Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override object InvokeMember (string name, BindingFlags invokeAttr, Binder binder, object target, object[] args, ParameterModifier[] modifiers, CultureInfo culture, string[] namedParameters) {
			return typeof(T).InvokeMember(name, invokeAttr, binder, target, args, modifiers, culture, namedParameters);
		}

		[Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override bool IsDefined (Type attributeType, bool inherit) {
			return typeof(T).IsDefined(attributeType, inherit);
		}

		private static readonly Lazy<MethodInfo> GetAttributeFlagsImplRefl = new Lazy<MethodInfo>(() => typeof(T).GetMethod("GetAttributeFlagsImpl", BindingFlags.Instance | BindingFlags.NonPublic));
		[Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected override TypeAttributes GetAttributeFlagsImpl () {
			return GetAttributeFlagsImplRefl.Value.Invoke<TypeAttributes>(typeof(T));
		}

		private static readonly Lazy<MethodInfo> GetConstructorImplRefl = new Lazy<MethodInfo>(() => typeof(T).GetMethod("GetConstructorImpl", BindingFlags.Instance | BindingFlags.NonPublic));
		[Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected override ConstructorInfo GetConstructorImpl (BindingFlags bindingAttr, Binder binder, CallingConventions callConvention, Type[] types, ParameterModifier[] modifiers) {
			return GetConstructorImplRefl.Value.Invoke<ConstructorInfo>(typeof(T), bindingAttr, binder, callConvention, types, modifiers);
		}

		private static readonly Lazy<MethodInfo> GetMethodImplRefl = new Lazy<MethodInfo>(() => typeof(T).GetMethod("GetMethodImpl", BindingFlags.Instance | BindingFlags.NonPublic));
		[Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected override MethodInfo GetMethodImpl (string name, BindingFlags bindingAttr, Binder binder, CallingConventions callConvention, Type[] types, ParameterModifier[] modifiers) {
			return GetMethodImplRefl.Value.Invoke<MethodInfo>(typeof(T), name, bindingAttr, binder, callConvention, types, modifiers);
		}

		private static readonly Lazy<MethodInfo> GetPropertyImplRefl = new Lazy<MethodInfo>(() => typeof(T).GetMethod("GetPropertyImpl", BindingFlags.Instance | BindingFlags.NonPublic));
		[Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected override PropertyInfo GetPropertyImpl (string name, BindingFlags bindingAttr, Binder binder, Type returnType, Type[] types, ParameterModifier[] modifiers) {
			return GetPropertyImplRefl.Value.Invoke<PropertyInfo>(typeof(T), name, bindingAttr, binder, returnType, types, modifiers);
		}

		private static readonly Lazy<MethodInfo> HasElementTypeImplRefl = new Lazy<MethodInfo>(() => typeof(T).GetMethod("HasElementTypeImpl", BindingFlags.Instance | BindingFlags.NonPublic));
		[Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected override bool HasElementTypeImpl () {
			return HasElementTypeImplRefl.Value.Invoke<bool>(typeof(T));
		}

		private static readonly Lazy<MethodInfo> IsArrayImplRefl = new Lazy<MethodInfo>(() => typeof(T).GetMethod("IsArrayImpl", BindingFlags.Instance | BindingFlags.NonPublic));
		[Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected override bool IsArrayImpl () {
			return IsArrayImplRefl.Value.Invoke<bool>(typeof(T));
		}

		private static readonly Lazy<MethodInfo> IsByRefImplRefl = new Lazy<MethodInfo>(() => typeof(T).GetMethod("IsByRefImpl", BindingFlags.Instance | BindingFlags.NonPublic));
		[Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected override bool IsByRefImpl () {
			return IsByRefImplRefl.Value.Invoke<bool>(typeof(T));
		}

		private static readonly Lazy<MethodInfo> IsCOMObjectImplRefl = new Lazy<MethodInfo>(() => typeof(T).GetMethod("IsCOMObjectImpl", BindingFlags.Instance | BindingFlags.NonPublic));
		[Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected override bool IsCOMObjectImpl () {
			return IsCOMObjectImplRefl.Value.Invoke<bool>(typeof(T));
		}

		private static readonly Lazy<MethodInfo> IsPointerImplRefl = new Lazy<MethodInfo>(() => typeof(T).GetMethod("IsPointerImpl", BindingFlags.Instance | BindingFlags.NonPublic));
		[Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected override bool IsPointerImpl () {
			return IsPointerImplRefl.Value.Invoke<bool>(typeof(T));
		}

		private static readonly Lazy<MethodInfo> IsPrimitiveImplRefl = new Lazy<MethodInfo>(() => typeof(T).GetMethod("IsPrimitiveImpl", BindingFlags.Instance | BindingFlags.NonPublic));
		[Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected override bool IsPrimitiveImpl () {
			return IsPrimitiveImplRefl.Value.Invoke<bool>(typeof(T));
		}
	}
}
