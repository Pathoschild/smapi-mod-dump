/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

using LinqFasterer;
using SpriteMaster.Extensions;
using SpriteMaster.Extensions.Reflection;
using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace SpriteMaster.Types.Reflection;
internal abstract class VariableInfo : MemberInfo, ILongHash {
	protected abstract MemberInfo Value { get; }

	internal abstract VariableAccessor<TObject, TResult> GetAccessor<TObject, TResult>();

	internal abstract VariableStaticAccessor<TResult> GetAccessor<TResult>(object target);

	internal abstract VariableStaticAccessor<TResult> GetStaticAccessor<TResult>();

	public abstract VariableAttributes Attributes { get; }

	#region MemberInfo

	internal virtual FieldInfo? Field => null;
	internal virtual PropertyInfo? Property => null;
	internal MemberInfo Member => Value;

	public sealed override object[] GetCustomAttributes(bool inherit) =>
		Value.GetCustomAttributes(inherit);

	public sealed override object[] GetCustomAttributes(Type attributeType, bool inherit) =>
		Value.GetCustomAttributes(attributeType, inherit);

	public sealed override bool IsDefined(Type attributeType, bool inherit) =>
		Value.IsDefined(attributeType, inherit);

	public sealed override Type? DeclaringType => Value.DeclaringType;
	public sealed override string Name => Value.Name;
	public sealed override Type? ReflectedType => Value.ReflectedType;

	#endregion

	#region FieldInfo/PropertyInfo

	public virtual FieldAttributes? FieldAttributes => null;
	public virtual PropertyAttributes? PropertyAttributes => null;
	public abstract Type DataType { get; }
	public abstract bool IsInitOnly { get; }
	public abstract bool IsLiteral { get; }
	public abstract bool IsNotSerialized { get; }
	public abstract bool IsPinvokeImpl { get; }
	public abstract bool IsSpecialName { get; }
	public abstract bool IsStatic { get; }

	public abstract bool IsAssembly { get; }
	public abstract bool IsFamily { get; }
	public abstract bool IsFamilyAndAssembly { get; }
	public abstract bool IsFamilyOrAssembly { get; }
	public abstract bool IsPrivate { get; }
	public abstract bool IsPublic { get; }

	public abstract bool IsSecurityCritical { get; }
	public abstract bool IsSecuritySafeCritical { get; }
	public abstract bool IsSecurityTransparent { get; }

	public abstract RuntimeFieldHandle? FieldHandle { get; }

	public abstract override bool Equals(object? obj);
	public abstract override int GetHashCode();
	public abstract ulong GetLongHashCode();

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator ==(VariableInfo? left, VariableInfo? right) {
		if (right is null) {
			// return true/false not the test result https://github.com/dotnet/runtime/issues/4207
			return (left is null) ? true : false;
		}

		// Try fast reference equality and opposite null check prior to calling the slower virtual Equals
		if ((object?)left == (object)right) {
			return true;
		}

		return left?.Equals(right) ?? false;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator !=(VariableInfo? left, VariableInfo? right) => !(left == right);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public abstract object? GetValue(object? obj);

	[DebuggerHidden, DebuggerStepThrough, MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void SetValue(object? obj, object? value) => SetValue(obj, value, BindingFlags.Default, Type.DefaultBinder, null);
	public abstract void SetValue(object? obj, object? value, BindingFlags invokeAttr, Binder? binder, CultureInfo? culture);

	[CLSCompliant(false)]
	public virtual void SetValueDirect(TypedReference obj, object value) => ThrowHelper.ThrowNotSupportedException(ThrowHelper.Strings.AbstractNonCls);
	[CLSCompliant(false)]
	public virtual object? GetValueDirect(TypedReference obj) => ThrowHelper.ThrowNotSupportedException<object?>(ThrowHelper.Strings.AbstractNonCls);

	public virtual object? GetConstantValue() => ThrowHelper.ThrowNotSupportedException<object?>(ThrowHelper.Strings.AbstractNonCls);
	public virtual object? GetRawConstantValue() => ThrowHelper.ThrowNotSupportedException<object?>(ThrowHelper.Strings.AbstractNonCls);

	public virtual Type[] GetOptionalCustomModifiers() => ThrowHelper.ThrowNotImplementedException<Type[]>(ThrowHelper.Strings.ByDesign);
	public virtual Type[] GetRequiredCustomModifiers() => ThrowHelper.ThrowNotImplementedException<Type[]>(ThrowHelper.Strings.ByDesign);

	#endregion

	#region PropertyInfo
	public abstract ParameterInfo[] GetIndexParameters();
	public abstract bool CanRead { get; }
	public abstract bool CanWrite { get; }

	public MethodInfo[] GetAccessors() => GetAccessors(nonPublic: false);
	public abstract MethodInfo[] GetAccessors(bool nonPublic);

	public virtual MethodInfo? GetMethod => GetGetMethod(nonPublic: true);
	public MethodInfo? GetGetMethod() => GetGetMethod(nonPublic: false);
	public abstract MethodInfo? GetGetMethod(bool nonPublic);

	public virtual MethodInfo? SetMethod => GetSetMethod(nonPublic: true);
	public MethodInfo? GetSetMethod() => GetSetMethod(nonPublic: false);
	public abstract MethodInfo? GetSetMethod(bool nonPublic);
	#endregion

	internal static VariableInfo From(FieldInfo field) =>
		new FieldVariableInfo(field);

	internal static VariableInfo From(PropertyInfo property) =>
		new PropertyVariableInfo(property);

	[return: NotNullIfNotNull("member")]
	internal static VariableInfo? From(MemberInfo? member) {
		if (member is null) {
			return null;
		}
		return member switch {
			FieldInfo field => From(field),
			PropertyInfo property => From(property),
			_ => ThrowHelper.ThrowArgumentException<VariableInfo>(
				$"Member '{member}' ({member.GetType().GetTypeName()}) is not {typeof(FieldInfo).GetTypeName()} or {typeof(PropertyInfo).GetTypeName()}",
				nameof(member)
			)
		};
	}
}

internal sealed class FieldVariableInfo : VariableInfo {
	protected override MemberInfo Value => Field;
	internal override FieldInfo Field { get; }

	[Obsolete("Field does not have Property")]
	private static new PropertyInfo? Property => null;

	internal override VariableAccessor<TObject, TResult> GetAccessor<TObject, TResult>() {
		return new(
			this,
			typeof(TObject).GetFieldGetter<TObject, TResult>(Field),
			typeof(TObject).GetFieldSetter<TObject, TResult>(Field)
		);
	}

	internal override VariableStaticAccessor<TResult> GetAccessor<TResult>(object target) {
		if (Field.DeclaringType is not { } type) {
			return ThrowHelper.ThrowInvalidOperationException<VariableStaticAccessor<TResult>>($"Field '{Field}' lacks a declaring type");
		}

		return new(
			this,
			type.GetFieldGetter<object, TResult>(Field, target),
			type.GetFieldSetter<object, TResult>(Field, target)
		);
	}

	internal override VariableStaticAccessor<TResult> GetStaticAccessor<TResult>() {
		return new(
			this,
			Field.GetGetter<TResult>(),
			Field.GetSetter<TResult>()
		);
	}

	public override VariableAttributes Attributes { get; }

	[NotNull]
	public override FieldAttributes? FieldAttributes => Field.Attributes;

	private readonly Lazy<MethodInfo> AccessorGet;
	private readonly Lazy<MethodInfo> AccessorSet;
	private readonly Lazy<MethodInfo[]> Accessors;

	public override MemberTypes MemberType => MemberTypes.Field;

	internal FieldVariableInfo(FieldInfo field) {
		Field = field;

		AccessorGet = new(Field.IsStatic ? new Func<object?>(GetValueStatic).Method : new Func<object?, object?>(GetValue).Method);
		AccessorSet = new(Field.IsStatic ? new Action<object?>(SetValueStatic).Method : new Action<object?, object?>(SetValue).Method);

		Accessors = new(new[] {
			AccessorGet.Value,
			AccessorSet.Value,
		});

		Attributes = VariableAttributesExt.GetVariableAttributes(null, null, Field.Attributes);
	}

	internal FieldVariableInfo(MemberInfo member) : this(member switch {
		FieldInfo field => field,
		_ => ThrowHelper.ThrowArgumentException<FieldInfo>($"Member '{member}' ({member.GetType().GetTypeName()}) is not {typeof(FieldInfo).GetTypeName()}", nameof(member))
	}) { }

	#region Implementation

	public override Type DataType => Field.FieldType;
	public override bool IsInitOnly => Field.IsInitOnly;
	public override bool IsLiteral => Field.IsLiteral;
	public override bool IsNotSerialized => Field.IsNotSerialized;
	public override bool IsPinvokeImpl => Field.IsPinvokeImpl;
	public override bool IsSpecialName => Field.IsSpecialName;
	public override bool IsStatic => Field.IsStatic;
	public override bool IsAssembly => Field.IsAssembly;
	public override bool IsFamily => Field.IsFamily;
	public override bool IsFamilyAndAssembly => Field.IsFamilyAndAssembly;
	public override bool IsFamilyOrAssembly => Field.IsFamilyOrAssembly;
	public override bool IsPrivate => Field.IsPrivate;
	public override bool IsPublic => Field.IsPublic;
	public override bool IsSecurityCritical => Field.IsSecurityCritical;
	public override bool IsSecuritySafeCritical => Field.IsSecuritySafeCritical;
	public override bool IsSecurityTransparent => Field.IsSecurityTransparent;
	[NotNull]
	public override RuntimeFieldHandle? FieldHandle => Field.FieldHandle;

	public override bool Equals(object? obj) {
		return obj switch {
			FieldVariableInfo fieldVarInfo => Field.Equals(fieldVarInfo.Field),
			FieldInfo fieldInfo => Field.Equals(fieldInfo),
			_ => false
		};
	}

	public override int GetHashCode() => Field.GetHashCode();

	public override ulong GetLongHashCode() => Field.GetLongHashCode();

	private object? GetValueStatic() => Field.GetValue(null);

	public override object? GetValue(object? obj) => Field.GetValue(obj);

	private void SetValueStatic(object? value) => Field.SetValue(null, value);

	public override void SetValue(object? obj, object? value, BindingFlags invokeAttr, Binder? binder, CultureInfo? culture) =>
		Field.SetValue(obj, value, invokeAttr, binder, culture);

	public override ParameterInfo[] GetIndexParameters() => Array.Empty<ParameterInfo>();

	public override bool CanRead => true;
	public override bool CanWrite => true;

	public override MethodInfo[] GetAccessors(bool nonPublic) => Accessors.Value;

	public override MethodInfo GetGetMethod(bool nonPublic) => AccessorGet.Value;

	public override MethodInfo GetSetMethod(bool nonPublic) => AccessorSet.Value;

	[CLSCompliant(false)]
	public override void SetValueDirect(TypedReference obj, object value) => Field.SetValueDirect(obj, value);

	[CLSCompliant(false)]
	public override object? GetValueDirect(TypedReference obj) => Field.GetValueDirect(obj);

	public override object? GetConstantValue() => Field.GetRawConstantValue();

	public override object? GetRawConstantValue() => Field.GetRawConstantValue();
	public override Type[] GetOptionalCustomModifiers() => Field.GetOptionalCustomModifiers();
	public override Type[] GetRequiredCustomModifiers() => Field.GetRequiredCustomModifiers();

	#endregion
}

internal sealed class PropertyVariableInfo : VariableInfo {
	protected override MemberInfo Value => Property;
	internal override PropertyInfo Property { get; }

	[Obsolete("Property does not have Field")]
	private static new FieldInfo? Field => null;

	internal override VariableAccessor<TObject, TResult> GetAccessor<TObject, TResult>() {
		return new(
			this,
			typeof(TObject).GetPropertyGetter<TObject, TResult>(Property),
			typeof(TObject).GetPropertySetter<TObject, TResult>(Property)
		);
	}

	internal override VariableStaticAccessor<TResult> GetAccessor<TResult>(object target) {
		if (Property.DeclaringType is not { } type) {
			return ThrowHelper.ThrowInvalidOperationException<VariableStaticAccessor<TResult>>($"Property '{Property}' lacks a declaring type");
		}

		return new(
			this,
			type.GetPropertyGetter<object, TResult>(Property, target),
			type.GetPropertySetter<object, TResult>(Property, target)
		);
	}

	internal override VariableStaticAccessor<TResult> GetStaticAccessor<TResult>() {
		return new(
			this,
			Property.GetGetter<TResult>(),
			Property.GetSetter<TResult>()
		);
	}

	private readonly Lazy<VariableAttributes> AttributesLazy;
	public override VariableAttributes Attributes => AttributesLazy.Value;
	public override MemberTypes MemberType => MemberTypes.Property;

	internal readonly Lazy<bool> IsLiteralLazy;

	[NotNull]
	public override PropertyAttributes? PropertyAttributes => Property.Attributes;

	public override FieldAttributes? FieldAttributes => BackingField.Value?.Attributes;

	internal PropertyVariableInfo(PropertyInfo property) {
		Property = property;

		IsLiteralLazy = new(
			() => {
				try {
					_ = GetConstantValue();
					return true;
				}
				catch {
					return false;
				}
			});

		ReferenceMethodInfo = new(() => GetAccessors(false).FirstOrDefaultF() ?? GetAccessors(true).FirstOrDefaultF());

		BackingField = new(GetBackingField);

		AttributesLazy = new(() => VariableAttributesExt.GetVariableAttributes(this, property.Attributes, null));
	}

	internal PropertyVariableInfo(MemberInfo member) : this(member switch {
		PropertyInfo property => property,
		_ => ThrowHelper.ThrowArgumentException<PropertyInfo>($"Member '{member}' ({member.GetType().GetTypeName()}) is not {typeof(PropertyInfo).GetTypeName()}", nameof(member))
	}) { }

	#region Implementation

	internal Lazy<MethodInfo?> ReferenceMethodInfo;
	internal Lazy<FieldInfo?> BackingField;

	// https://stackoverflow.com/a/14210097/5055153
	private FieldInfo? GetBackingField() {
		static string GetBackingFieldName(string propertyName) {
			return $"<{propertyName}>k__BackingField";
		}

		return Property.DeclaringType?.GetField(
			GetBackingFieldName(Property.Name),
			BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.NonPublic
		);
	}

	public override Type DataType => Property.PropertyType;
	public override bool IsInitOnly => !CanWrite;
	public override bool IsLiteral => IsLiteralLazy.Value;
	public override bool IsNotSerialized => Property.HasAttribute<NonSerializedAttribute>();
	public override bool IsPinvokeImpl => false;
	public override bool IsSpecialName => Property.IsSpecialName;
	public override bool IsStatic => ReferenceMethodInfo.Value?.IsStatic ?? false;
	public override bool IsAssembly => ReferenceMethodInfo.Value?.IsAssembly ?? false;
	public override bool IsFamily => ReferenceMethodInfo.Value?.IsFamily ?? false;
	public override bool IsFamilyAndAssembly => ReferenceMethodInfo.Value?.IsFamilyAndAssembly ?? false;
	public override bool IsFamilyOrAssembly => ReferenceMethodInfo.Value?.IsFamilyOrAssembly ?? false;
	public override bool IsPrivate => ReferenceMethodInfo.Value?.IsPrivate ?? false;
	public override bool IsPublic => ReferenceMethodInfo.Value?.IsPublic ?? false;
	public override bool IsSecurityCritical => ReferenceMethodInfo.Value?.IsSecurityCritical ?? false;
	public override bool IsSecuritySafeCritical => ReferenceMethodInfo.Value?.IsSecuritySafeCritical ?? false;
	public override bool IsSecurityTransparent => ReferenceMethodInfo.Value?.IsSecurityTransparent ?? false;
	public override RuntimeFieldHandle? FieldHandle => GetBackingField()?.FieldHandle;
	public override bool Equals(object? obj) {
		return obj switch {
			PropertyVariableInfo propertyVarInfo => Property.Equals(propertyVarInfo.Property),
			PropertyInfo propertyInfo => Property.Equals(propertyInfo),
			_ => false
		};
	}

	public override int GetHashCode() => Property.GetHashCode();

	public override ulong GetLongHashCode() => Property.GetLongHashCode();

	private object? GetValueStatic() => Property.GetValue(null);

	public override object? GetValue(object? obj) => Property.GetValue(obj);

	private void SetValueStatic(object? value) => Property.SetValue(null, value);

	[DebuggerStepThrough]
	[DebuggerHidden]
	public void SetValue(object? obj, object? value, object?[]? index) =>
		this.SetValue(obj, value, BindingFlags.Default, (Binder?)null, index, (CultureInfo?)null);

	public void SetValue(object? obj, object? value, BindingFlags invokeAttr, Binder? binder, object?[]? index, CultureInfo? culture) =>
		Property.SetValue(obj, value, invokeAttr, binder, index, culture);

	public override void SetValue(object? obj, object? value, BindingFlags invokeAttr, Binder? binder, CultureInfo? culture) =>
		Property.SetValue(obj, value, invokeAttr, binder, null, culture);

	public override ParameterInfo[] GetIndexParameters() => Array.Empty<ParameterInfo>();

	public override bool CanRead => true;
	public override bool CanWrite => true;

	public override MethodInfo[] GetAccessors(bool nonPublic) => Property.GetAccessors(nonPublic);

	public override MethodInfo? GetGetMethod(bool nonPublic) => Property.GetGetMethod(nonPublic);

	public override MethodInfo? GetSetMethod(bool nonPublic) => Property.GetSetMethod(nonPublic);

	[CLSCompliant(false)]
	public override void SetValueDirect(TypedReference obj, object value) => Property.SetValue(TypedReference.ToObject(obj), value);

	[CLSCompliant(false)]
	public override object? GetValueDirect(TypedReference obj) => Property.GetValue(TypedReference.ToObject(obj));

	public override object? GetConstantValue() => Property.GetRawConstantValue();

	public override object? GetRawConstantValue() => Property.GetRawConstantValue();
	public override Type[] GetOptionalCustomModifiers() => Property.GetOptionalCustomModifiers();
	public override Type[] GetRequiredCustomModifiers() => Property.GetRequiredCustomModifiers();

	#endregion
}
