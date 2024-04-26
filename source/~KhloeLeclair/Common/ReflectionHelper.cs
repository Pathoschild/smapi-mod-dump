/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/KhloeLeclair/StardewMods
**
*************************************************/

using System.Collections.Generic;
using System.Reflection.Emit;
using System.Reflection;
using System;
using Sickhead.Engine.Util;

namespace Leclair.Stardew.Common;

internal static class ReflectionHelper {

	private static string MakeAccessorName(string prefix, MemberInfo field) {
		string assembly = Assembly.GetExecutingAssembly().GetName().Name ?? "Unknown";
		return $"{assembly}_{prefix}_{field.DeclaringType?.Name}_{field.Name}";
	}

	#region Fields

	private static readonly Dictionary<FieldInfo, Delegate> FieldGetters = new();
	private static readonly Dictionary<FieldInfo, Delegate> FieldSetters = new();

	#region Static Fields

	/// <summary>
	/// Use <see cref="System.Reflection.Emit"/> to create a dynamic method
	/// for reading the value of a static field. This is much more efficient
	/// than calling <see cref="FieldInfo.GetValue(object?)"/>.
	/// </summary>
	/// <typeparam name="T">The return type of the field.</typeparam>
	/// <param name="field">The <see cref="FieldInfo"/> to access.</param>
	/// <returns>A function for reading the value.</returns>
	/// <exception cref="ArgumentNullException">If the field is null</exception>
	/// <exception cref="ArgumentException">If the field is not static</exception>
	/// <exception cref="InvalidCastException">If the provided <typeparamref name="T"/> is not the field's type</exception>
	internal static Func<T> CreateGetter<T>(this FieldInfo field) {
		if (field is null)
			throw new ArgumentNullException(nameof(field));
		if (typeof(T) != field.FieldType)
			throw new InvalidCastException($"{typeof(T)} is not same as field type {field.FieldType}");
		if (!field.IsStatic)
			throw new ArgumentException("field is not static");

		if (!FieldGetters.TryGetValue(field, out var getter)) {
			DynamicMethod dm = new(MakeAccessorName("Get", field), typeof(T), null, true);

			var generator = dm.GetILGenerator();
			generator.Emit(OpCodes.Ldsfld, field);
			generator.Emit(OpCodes.Ret);

			getter = dm.CreateDelegate(typeof(Func<T>));
			FieldGetters[field] = getter;
		}

		return (Func<T>) getter;
	}

	/// <summary>
	/// Use <see cref="System.Reflection.Emit"/> to create a dynamic method
	/// for writing a value to a static field. This is much more efficient
	/// than calling <see cref="FieldInfo.SetValue(object?, object?)"/>.
	/// </summary>
	/// <typeparam name="T">The type of the field.</typeparam>
	/// <param name="field">The <see cref="FieldInfo"/> to access.</param>
	/// <returns>A function for writing the value.</returns>
	/// <exception cref="ArgumentNullException">If either argument is null</exception>
	/// <exception cref="ArgumentException">If the field is not static</exception>
	/// <exception cref="InvalidCastException">If the provided <typeparamref name="T"/> is not the field's type</exception>
	internal static Action<T> CreateSetter<T>(this FieldInfo field) {
		if (field is null)
			throw new ArgumentNullException(nameof(field));
		if (typeof(T) != field.FieldType)
			throw new InvalidCastException($"{typeof(T)} is not same as field type {field.FieldType}");
		if (!field.IsStatic)
			throw new ArgumentException("field is not static");

		if (!FieldSetters.TryGetValue(field, out var setter)) {
			DynamicMethod dm = new(MakeAccessorName("Set", field), null, [typeof(T)], true);

			var generator = dm.GetILGenerator();
			generator.Emit(OpCodes.Ldarg_0);
			generator.Emit(OpCodes.Stsfld, field);
			generator.Emit(OpCodes.Ret);

			setter = dm.CreateDelegate(typeof(Action<T>));
			FieldSetters[field] = setter;
		}

		return (Action<T>) setter;
	}

	#endregion

	#region Instance Fields

	/// <summary>
	/// Use <see cref="System.Reflection.Emit"/> to create a dynamic method
	/// for reading the value of a field on an object. This is much more
	/// efficient than calling <see cref="FieldInfo.GetValue(object?)"/>.
	/// </summary>
	/// <typeparam name="TOwner">The type of object that owns the field.</typeparam>
	/// <typeparam name="TValue">The type of the field.</typeparam>
	/// <param name="field">The <see cref="FieldInfo"/> to access.</param>
	/// <returns>A function for reading the value.</returns>
	/// <exception cref="ArgumentNullException">If the field is null</exception>
	/// <exception cref="ArgumentException">If the field is not static</exception>
	/// <exception cref="InvalidCastException">If the provided <typeparamref name="TOwner"/>
	/// or <typeparamref name="TValue"/> do not match the field.</exception>
	internal static Func<TOwner, TValue> CreateGetter<TOwner, TValue>(this FieldInfo field) {
		if (field is null)
			throw new ArgumentNullException(nameof(field));
		if (typeof(TOwner) != field.DeclaringType)
			throw new InvalidCastException($"{typeof(TOwner)} is not same as declaring type {field.DeclaringType}");
		if (typeof(TValue) != field.FieldType)
			throw new InvalidCastException($"{typeof(TValue)} is not same as field type {field.FieldType}");
		if (field.IsStatic)
			throw new ArgumentException("field is static");

		if (!FieldGetters.TryGetValue(field, out var getter)) {
			DynamicMethod dm = new(MakeAccessorName("Get", field), typeof(TValue), [typeof(TOwner)], true);

			var generator = dm.GetILGenerator();
			generator.Emit(OpCodes.Ldarg_0);
			generator.Emit(OpCodes.Ldfld, field);
			generator.Emit(OpCodes.Ret);

			getter = dm.CreateDelegate(typeof(Func<TOwner, TValue>));
			FieldGetters[field] = getter;
		}

		return (Func<TOwner, TValue>) getter;
	}

	/// <summary>
	/// Use <see cref="System.Reflection.Emit"/> to create a dynamic method
	/// for writing a value to a field on an object. This is much more efficient
	/// than calling <see cref="FieldInfo.SetValue(object?, object?)"/>.
	/// </summary>
	/// <typeparam name="TOwner">The type of object that owns the field.</typeparam>
	/// <typeparam name="TValue">The type of the field.</typeparam>
	/// <param name="field">The <see cref="FieldInfo"/> to access.</param>
	/// <returns>A function for setting the value.</returns>
	/// <exception cref="ArgumentNullException"> If the field is null</exception>
	/// <exception cref="ArgumentException">If the field is not static</exception>
	/// <exception cref="InvalidCastException">If the provided <typeparamref name="TOwner"/>
	/// or <typeparamref name="TValue"/> do not match the field.</exception>
	internal static Action<TOwner, TValue> CreateSetter<TOwner, TValue>(this FieldInfo field) {
		if (field is null)
			throw new ArgumentNullException(nameof(field));
		if (typeof(TOwner) != field.DeclaringType)
			throw new InvalidCastException($"{typeof(TOwner)} is not same as declaring type {field.DeclaringType}");
		if (typeof(TValue) != field.FieldType)
			throw new InvalidCastException($"{typeof(TValue)} is not same as field type {field.FieldType}");
		if (field.IsStatic)
			throw new ArgumentException("field is static");

		if (!FieldSetters.TryGetValue(field, out var setter)) {
			DynamicMethod dm = new(MakeAccessorName("Set", field), null, [typeof(TOwner), typeof(TValue)], true);

			var generator = dm.GetILGenerator();
			generator.Emit(OpCodes.Ldarg_0);
			generator.Emit(OpCodes.Ldarg_1);
			generator.Emit(OpCodes.Stfld, field);
			generator.Emit(OpCodes.Ret);

			setter = dm.CreateDelegate(typeof(Action<TOwner, TValue>));
			FieldSetters[field] = setter;
		}

		return (Action<TOwner, TValue>) setter;
	}

	#endregion

	#endregion

	#region Properties

	private static readonly Dictionary<PropertyInfo, Delegate> PropertyGetters = new();
	private static readonly Dictionary<PropertyInfo, Delegate> PropertySetters = new();

	#region Static Properties

	/// <summary>
	/// Use <see cref="System.Reflection.Emit"/> to create a dynamic method
	/// for reading the value from a static property. This is much more efficient
	/// than calling <see cref="PropertyInfo.GetValue(object?)"/>.
	/// </summary>
	/// <typeparam name="TValue">The return type of the property.</typeparam>
	/// <param name="property">The <see cref="PropertyInfo"/> to access.</param>
	/// <returns>A function for reading the value.</returns>
	/// <exception cref="ArgumentNullException">If the property is null</exception>
	/// <exception cref="ArgumentException">If the property is not static</exception>
	/// <exception cref="InvalidCastException">If the provided <typeparamref name="TValue"/> is not the property's type</exception>
	internal static Func<TValue> CreateGetter<TValue>(this PropertyInfo property) {
		if (property is null)
			throw new ArgumentNullException(nameof(property));
		if (typeof(TValue) != property.PropertyType)
			throw new InvalidCastException($"{typeof(TValue)} is not same as property type {property.PropertyType}");

		if (!PropertyGetters.TryGetValue(property, out var getter)) {
			var getMethod = property.GetGetMethod(nonPublic: true) ?? throw new ArgumentNullException("property has no getter");
			if (!getMethod.IsStatic)
				throw new ArgumentException("property is not static");

			DynamicMethod dm = new(MakeAccessorName("Get", property), typeof(TValue), null, true);

			var generator = dm.GetILGenerator();
			generator.Emit(OpCodes.Call, getMethod);
			generator.Emit(OpCodes.Ret);

			getter = dm.CreateDelegate(typeof(Func<TValue>));
			PropertyGetters[property] = getter;
		}

		return (Func<TValue>) getter;
	}

	/// <summary>
	/// Use <see cref="System.Reflection.Emit"/> to create a dynamic method
	/// for writing a value to a static property. This is much more efficient
	/// than calling <see cref="PropertyInfo.SetValue(object?, object?)"/>.
	/// </summary>
	/// <typeparam name="TValue">The type of the property.</typeparam>
	/// <param name="property">The <see cref="PropertyInfo"/> to access.</param>
	/// <returns>A function for writing the value.</returns>
	/// <exception cref="ArgumentNullException">If the property is null</exception>
	/// <exception cref="ArgumentException">If the property is not static</exception>
	/// <exception cref="InvalidCastException">If the provided <typeparamref name="TValue"/> is not the property's type</exception>
	internal static Action<TValue> CreateSetter<TValue>(this PropertyInfo property) {
		if (property is null)
			throw new ArgumentNullException(nameof(property));
		if (typeof(TValue) != property.PropertyType)
			throw new InvalidCastException($"{typeof(TValue)} is not same as property type {property.PropertyType}");

		if (!PropertySetters.TryGetValue(property, out var setter)) {
			var setMethod = property.GetSetMethod(nonPublic: true) ?? throw new ArgumentNullException("property has no setter");
			if (!setMethod.IsStatic)
				throw new ArgumentException("property is not static");

			DynamicMethod dm = new(MakeAccessorName("Set", property), typeof(TValue), null, true);

			var generator = dm.GetILGenerator();
			generator.Emit(OpCodes.Ldarg_0);
			generator.Emit(OpCodes.Call, setMethod);
			generator.Emit(OpCodes.Ret);

			setter = dm.CreateDelegate(typeof(Action<TValue>));
			PropertyGetters[property] = setter;
		}

		return (Action<TValue>) setter;
	}

	#endregion

	#region Instance Properties

	/// <summary>
	/// Use <see cref="System.Reflection.Emit"/> to create a dynamic method
	/// for reading the value from an instance property. This is much more
	/// efficient than calling <see cref="PropertyInfo.GetValue(object?)"/>.
	/// </summary>
	/// <typeparam name="TOwner">The type of object that owns the property.</typeparam>
	/// <typeparam name="TValue">The type of the property.</typeparam>
	/// <param name="property">The <see cref="PropertyInfo"/> to access.</param>
	/// <returns>A function for reading the value.</returns>
	/// <exception cref="ArgumentNullException">If the property is null</exception>
	/// <exception cref="ArgumentException">If the property is not static</exception>
	/// <exception cref="InvalidCastException">If the provided <typeparamref name="T"/> is not the property's type</exception>
	internal static Func<TOwner, TValue> CreateGetter<TOwner, TValue>(this PropertyInfo property) {
		if (property is null)
			throw new ArgumentNullException(nameof(property));
		if (typeof(TValue) != property.PropertyType)
			throw new InvalidCastException($"{typeof(TValue)} is not same as property type {property.PropertyType}");
		if (typeof(TOwner) != property.DeclaringType)
			throw new InvalidCastException($"{typeof(TOwner)} is not the same as declaring type {property.DeclaringType}");

		if (!PropertyGetters.TryGetValue(property, out var getter)) {
			var getMethod = property.GetGetMethod(nonPublic: true) ?? throw new ArgumentNullException("property has no getter");
			if (getMethod.IsStatic)
				throw new ArgumentException("property is static");

			DynamicMethod dm = new(MakeAccessorName("Get", property), typeof(TValue), [typeof(TOwner)], true);

			var generator = dm.GetILGenerator();
			generator.Emit(OpCodes.Ldarg_0);
			generator.Emit(OpCodes.Call, getMethod);
			generator.Emit(OpCodes.Ret);

			getter = dm.CreateDelegate(typeof(Func<TOwner, TValue>));
			PropertyGetters[property] = getter;
		}

		return (Func<TOwner, TValue>) getter;
	}

	/// <summary>
	/// Use <see cref="System.Reflection.Emit"/> to create a dynamic method
	/// for writing a value to an instance property. This is much more efficient
	/// than calling <see cref="PropertyInfo.SetValue(object?, object?)"/>.
	/// </summary>
	/// <typeparam name="TOwner">The type of object that owns the property.</typeparam>
	/// <typeparam name="TValue">The type of the property.</typeparam>
	/// <param name="property">The <see cref="PropertyInfo"/> to access.</param>
	/// <returns>A function for writing the value.</returns>
	/// <exception cref="ArgumentNullException">If the property is null</exception>
	/// <exception cref="ArgumentException">If the property is not static</exception>
	/// <exception cref="InvalidCastException">If the provided <typeparamref name="T"/> is not the property's type</exception>
	internal static Action<TOwner, TValue> CreateSetter<TOwner, TValue>(this PropertyInfo property) {
		if (property is null)
			throw new ArgumentNullException(nameof(property));
		if (typeof(TValue) != property.PropertyType)
			throw new InvalidCastException($"{typeof(TValue)} is not same as property type {property.PropertyType}");
		if (typeof(TOwner) != property.DeclaringType)
			throw new InvalidCastException($"{typeof(TOwner)} is not the same as declaring type {property.DeclaringType}");

		if (!PropertySetters.TryGetValue(property, out var setter)) {
			var setMethod = property.GetSetMethod(nonPublic: true) ?? throw new ArgumentNullException("property has no setter");
			if (setMethod.IsStatic)
				throw new ArgumentException("property is static");

			DynamicMethod dm = new(MakeAccessorName("Set", property), null, [typeof(TOwner), typeof(TValue)], true);

			var generator = dm.GetILGenerator();
			generator.Emit(OpCodes.Ldarg_0);
			generator.Emit(OpCodes.Ldarg_1);
			generator.Emit(OpCodes.Call, setMethod);
			generator.Emit(OpCodes.Ret);

			setter = dm.CreateDelegate(typeof(Action<TOwner, TValue>));
			PropertyGetters[property] = setter;
		}

		return (Action<TOwner, TValue>) setter;
	}

	#endregion

	#endregion

}
