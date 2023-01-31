/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

using Microsoft.Toolkit.HighPerformance;
using MusicMaster.Extensions.Reflection;
using System;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace MusicMaster.Types.Reflection;

// Clones from PropertyAttributes and FieldAttributes
[Flags]
internal enum VariableAttributes {
	None = 0x0000,

	#region PropertyAttributes

	#endregion

	#region FieldAttributes

	#region Member Access Mask
	FieldAccessMask = 0x0007,
	PrivateScope = 0x0000,
	Private = 0x0001,
	FamANDAssem = 0x0002,
	Assembly = 0x0003,
	Family = 0x0004,
	FamORAssem = 0x0005,
	Public = 0x0006,
	#endregion

	// field contract attributes.
	Static = 0x0010,        // Defined on type, else per instance.
	InitOnly = 0x0020,     // Field may only be initialized, not written to after init.
	Literal = 0x0040,        // Value is compile time constant.
	NotSerialized = 0x0080,        // Field does not have to be serialized when type is remoted.


	// interop attributes
	PinvokeImpl = 0x2000,        // Implementation is forwarded through pinvoke.

	HasFieldMarshal = 0x1000,     // Field has marshalling information.
	HasFieldRVA = 0x0100,     // Field has RVA.

	#endregion

	#region Common Attributes

	SpecialName = 0x0200,
	RTSpecialName = 0x0400,
	HasDefault = 0x8000,

	#endregion
}

internal static class VariableAttributesExt {
	private static readonly ConditionalWeakTable<PropertyInfo, Box<VariableAttributes>> VariableAttributesCache = new();

	internal static VariableAttributes GetVariableAttributes(PropertyVariableInfo? propertyInfo, PropertyAttributes? propertyAttributes, FieldAttributes? fieldAttributes) {
		var property = propertyInfo?.Property;

		if (property is not null) {
			if (VariableAttributesCache.TryGetValue(property, out var boxedAttributes)) {
				return boxedAttributes;
			}
		}

		VariableAttributes result = VariableAttributes.None;

		MethodAttributes? methodAttributes = null;

		if (fieldAttributes.HasValue) {
			result |= (VariableAttributes)(fieldAttributes.Value & FieldAttributes.FieldAccessMask);
		}
		else if (propertyInfo is not null) {
			if (propertyInfo.ReferenceMethodInfo.Value is { } method) {
				methodAttributes = method.Attributes;
				result |= (VariableAttributes)(methodAttributes.Value & MethodAttributes.MemberAccessMask);
			}
			else if (propertyInfo.BackingField.Value is {} field) {
				fieldAttributes = field.Attributes;
				result |= (VariableAttributes)(fieldAttributes.Value & FieldAttributes.FieldAccessMask);
			}
		}

		void CopyAttribute(PropertyAttributes propertyAttribute, FieldAttributes fieldAttribute, VariableAttributes variableAttribute) {
			if (propertyAttributes.HasValue) {
				if (propertyAttributes.Value.HasFlag(propertyAttribute)) {
					result |= variableAttribute;
				}
			}
			else if (fieldAttributes.HasValue) {
				if (fieldAttributes.Value.HasFlag(fieldAttribute)) {
					result |= variableAttribute;
				}
			}
		}

		void CopyFieldAttribute(FieldAttributes fieldAttribute, VariableAttributes variableAttribute) {
			if (fieldAttributes?.HasFlag(fieldAttribute) ?? false) {
				result |= variableAttribute;
			}
		}

		void CopyMethodAttribute(MethodAttributes methodAttribute, VariableAttributes variableAttribute) {
			if (methodAttributes?.HasFlag(methodAttribute) ?? false) {
				result |= variableAttribute;
			}
		}

		CopyAttribute(PropertyAttributes.SpecialName, FieldAttributes.SpecialName, VariableAttributes.SpecialName);
		CopyAttribute(PropertyAttributes.RTSpecialName, FieldAttributes.RTSpecialName, VariableAttributes.RTSpecialName);
		CopyAttribute(PropertyAttributes.HasDefault, FieldAttributes.HasDefault, VariableAttributes.HasDefault);

		if (fieldAttributes.HasValue) {
			CopyFieldAttribute(FieldAttributes.Static, VariableAttributes.Static);
			CopyFieldAttribute(FieldAttributes.InitOnly, VariableAttributes.InitOnly);
			CopyFieldAttribute(FieldAttributes.Literal, VariableAttributes.Literal);
			CopyFieldAttribute(FieldAttributes.NotSerialized, VariableAttributes.NotSerialized);
			CopyFieldAttribute(FieldAttributes.PinvokeImpl, VariableAttributes.PinvokeImpl);
			CopyFieldAttribute(FieldAttributes.HasFieldMarshal, VariableAttributes.HasFieldMarshal);
			CopyFieldAttribute(FieldAttributes.HasFieldRVA, VariableAttributes.HasFieldRVA);
		}
		else if (methodAttributes.HasValue) {
			CopyMethodAttribute(MethodAttributes.Static, VariableAttributes.Static);
			CopyMethodAttribute(MethodAttributes.PinvokeImpl, VariableAttributes.PinvokeImpl);
		}

		if (propertyInfo is not null && property is not null) {
			if (!result.HasFlag(VariableAttributes.NotSerialized) && property.HasAttribute<NonSerializedAttribute>()) {
				result |= VariableAttributes.NotSerialized;
			}

			if (!result.HasFlag(VariableAttributes.Literal) && propertyInfo.IsLiteralLazy.Value) {
				result |= VariableAttributes.Literal;
			}

			if (!result.HasFlag(VariableAttributes.InitOnly) && !propertyInfo.CanWrite) {
				result |= VariableAttributes.InitOnly;
			}

			VariableAttributesCache.Add(property, result);
		}

		return result;
	}
}
