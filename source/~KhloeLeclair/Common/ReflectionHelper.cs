/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/KhloeLeclair/StardewMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

using StardewModdingAPI;

namespace Leclair.Stardew.Common;

internal static class ReflectionHelper {

	private static string MakeAccessorName(string prefix, MemberInfo field) {
		string assembly = Assembly.GetExecutingAssembly().GetName().Name ?? "Unknown";
		return $"{assembly}_{prefix}_{field.DeclaringType?.Name}_{field.Name}";
	}

	#region GetWhatIsPatchingMe

#if HARMONY

	public static void UnpatchMyStuff(Mod mod, HarmonyLib.Harmony harmony, Func<string, bool> shouldUnpatch) {

		Assembly assembly = mod.GetType().Assembly;

		foreach (var method in HarmonyLib.PatchProcessor.GetAllPatchedMethods()) {
			if (method.DeclaringType?.Assembly != assembly)
				continue;

			var patches = HarmonyLib.PatchProcessor.GetPatchInfo(method);
			foreach (string owner in patches.Owners) {
				if (shouldUnpatch(owner)) {
					harmony.Unpatch(method, HarmonyLib.HarmonyPatchType.All, owner);
				}
			}
		}
	}

	public static void UnpatchMe(Mod mod, HarmonyLib.Harmony harmony, Func<string, bool> shouldUnpatch) {

		UnpatchMyStuff(mod, harmony, shouldUnpatch);

		var patches = GetPatchedMethodsInAssembly(mod.GetType().Assembly);

		Dictionary<string, List<string>> ByOwner = new();

		foreach (var entry in patches) {
			foreach (string owner in entry.Value.Owners) {
				if (shouldUnpatch(owner)) {
					string name = ToTargetString(entry.Key, false) ?? entry.Key.Name;
					if (!ByOwner.TryGetValue(owner, out var list)) {
						list = [];
						ByOwner[owner] = list;
					}

					list.Add(name);
					harmony.Unpatch(entry.Key, HarmonyLib.HarmonyPatchType.All, owner);
				}
			}
		}

		foreach (var entry in ByOwner) {
			var info = mod.Helper.ModRegistry.Get(entry.Key);
			string name = info is null ? entry.Key : $"{info.Manifest.Name} ({entry.Key})";
			string list = string.Join("\n  ", entry.Value);

			mod.Monitor.Log($"Forcibly removed Harmony patches from {name}:\n  {list}", LogLevel.Warn);
		}

	}
#endif

	public static StringBuilder? WhatPatchesMe(Mod mod, string? indentation = null, bool brief = false) {
#if HARMONY
		indentation ??= string.Empty;

		var patches = GetPatchedMethodsInAssembly(mod.GetType().Assembly);

		StringBuilder builder = new();

		foreach (var entry in patches) {
			string name = ToTargetString(entry.Key) ?? entry.Key.Name;
			builder.AppendLine($"{indentation}{name}");

			Dictionary<string, List<string>> ByOwner = new();

			foreach (var patch in entry.Value.Prefixes) {
				if (!ByOwner.TryGetValue(patch.owner, out var list)) {
					list = [];
					ByOwner[patch.owner] = list;
				}

				if (brief)
					list.Add($"prefix({patch.priority})");
				else
					list.Add($"prefix: {ToTargetString(patch.PatchMethod)}, priority: {patch.priority}");
			}

			foreach (var patch in entry.Value.Transpilers) {
				if (!ByOwner.TryGetValue(patch.owner, out var list)) {
					list = [];
					ByOwner[patch.owner] = list;
				}

				if (brief)
					list.Add($"transpiler({patch.priority})");
				else
					list.Add($"transpiler: {ToTargetString(patch.PatchMethod)}, priority: {patch.priority}");
			}

			foreach (var patch in entry.Value.Postfixes) {
				if (!ByOwner.TryGetValue(patch.owner, out var list)) {
					list = [];
					ByOwner[patch.owner] = list;
				}

				if (brief)
					list.Add($"postfix({patch.priority})");
				else
					list.Add($"postfix: {ToTargetString(patch.PatchMethod)}, priority: {patch.priority}");
			}

			foreach (var patch in entry.Value.Finalizers) {
				if (!ByOwner.TryGetValue(patch.owner, out var list)) {
					list = [];
					ByOwner[patch.owner] = list;
				}

				if (brief)
					list.Add($"finalizer({patch.priority})");
				else
					list.Add($"finalizer: {ToTargetString(patch.PatchMethod)}, priority: {patch.priority}");
			}

			foreach (var pair in ByOwner) {
				if (brief)
					builder.AppendLine($"{indentation}- {pair.Key} : {string.Join(", ", pair.Value)}");
				else {
					builder.AppendLine($"{indentation}- {pair.Key}");
					foreach (string le in pair.Value) {
						builder.AppendLine($"{indentation}  - {le}");
					}
				}
			}

			builder.AppendLine("");
		}

		return builder;
#else
		return null;
#endif
	}

#if HARMONY

	/// <summary>
	/// Generate a string for targeting a member of a type.
	/// </summary>
	/// <param name="member">The member we want a string for.</param>
	/// <param name="replaceMenuWithHash">When true, if a type name starts with
	/// <c>StardewValley.Menus.</c> it will be replaced with a <c>#</c> for
	/// the sake of brevity.</param>
	/// <param name="includeTypes">Whether or not a type list should be
	/// included. By default, this is null and a list will be included only
	/// if the member is a constructor or method AND there is potential
	/// ambiguity in selecting the correct method.</param>
	/// <param name="fullTypes">Whether or not the type list should include
	/// the full types, or just the minimum necessary to uniquely match
	/// the method.</param>
	public static string? ToTargetString(this MemberInfo member, bool replaceMenuWithHash = true, bool? includeTypes = null, bool fullTypes = true) {
		string? type = member.DeclaringType?.FullName;
		if (type is null)
			return null;

		if (type.StartsWith("StardewValley.Menus.") && replaceMenuWithHash)
			type = $"#{type[20..]}";

		bool[]? differingTypes = null;
		int min_types = 0;

		if (!includeTypes.HasValue || !fullTypes) {
			IEnumerable<MethodBase>? methods = null;

			if (member is ConstructorInfo) {
				var ctors = member.DeclaringType is not null ?
					HarmonyLib.AccessTools.GetDeclaredConstructors(member.DeclaringType)
					: null;

				if (!includeTypes.HasValue)
					includeTypes = ctors is null || ctors.Count > 1 || ctors[0] != member;

				if (ctors is not null && ctors.Count > 1)
					methods = ctors;

			} else if (member is MethodBase) {
				var meths = member.DeclaringType is not null ?
					HarmonyLib.AccessTools.GetDeclaredMethods(member.DeclaringType).Where(x => x.Name.Equals(member.Name)).ToArray()
					: null;

				if (!includeTypes.HasValue)
					includeTypes = meths is null || meths.Length > 1 || meths[0] != member;

				if (meths is not null && meths.Length > 1)
					methods = meths;

			} else if (!includeTypes.HasValue)
				includeTypes = false;

			// If we have methods, work on the types.
			if (methods is not null && member is MethodBase meth) {
				var parameters = meth.GetParameters();
				differingTypes = new bool[parameters.Length];

				// TODO: This.
			}
		}

		string? argumentList;

		if (includeTypes.Value) {
			string[] args;

			if (member is MethodBase method) {
				var parms = method.GetParameters();
				args = new string[parms.Length];

				for (int i = 0; i < parms.Length; i++)
					args[i] = parms[i].ParameterType?.Name ?? string.Empty;

			} else if (member is PropertyInfo prop)
				args = [prop.PropertyType?.Name ?? string.Empty];

			else if (member is FieldInfo field)
				args = [field.FieldType?.Name ?? string.Empty];

			else
				args = [];

			argumentList = $"({string.Join(',', args)})";
		} else
			argumentList = string.Empty;

		string? assembly = member.DeclaringType?.Assembly.GetName().Name;
		if (assembly is null || assembly == "Stardew Valley")
			assembly = string.Empty;
		else
			assembly = $"{assembly}!";

		string memberName = member.Name;
		/*if (member is ConstructorInfo) {
			memberName = "";
			if (string.IsNullOrEmpty(argumentList))
				argumentList = "()";
		}*/

		return $"{assembly}{type}:{memberName}{argumentList}";
	}

	public static Dictionary<MethodBase, HarmonyLib.Patches> GetPatchedMethodsInAssembly(Assembly? assembly = null) {
		assembly ??= Assembly.GetExecutingAssembly();
		Dictionary<MethodBase, HarmonyLib.Patches> result = [];

		foreach (var method in HarmonyLib.PatchProcessor.GetAllPatchedMethods()) {
			if (method.DeclaringType is null || method.DeclaringType.Assembly != assembly)
				continue;

			result[method] = HarmonyLib.PatchProcessor.GetPatchInfo(method);
		}

		return result;
	}

#endif

	#endregion

	#region Fields

	private static readonly Dictionary<FieldInfo, Delegate> FieldGetters = new();
	private static readonly Dictionary<FieldInfo, Delegate> FieldSetters = new();

	#region Static Fields

	/// <summary>
	/// Use <see cref="System.Reflection.Emit"/> to create a dynamic method
	/// for reading the value of a static field. This is much more efficient
	/// than calling <see cref="FieldInfo.GetValue(object?)"/>.
	/// </summary>
	/// <typeparam name="TValue">The return type of the field.</typeparam>
	/// <param name="field">The <see cref="FieldInfo"/> to access.</param>
	/// <returns>A function for reading the value.</returns>
	/// <exception cref="ArgumentNullException">If the field is null</exception>
	/// <exception cref="ArgumentException">If the field is not static</exception>
	/// <exception cref="InvalidCastException">If the provided <typeparamref name="TValue"/> is not the field's type</exception>
	internal static Func<TValue> CreateGetter<TValue>(this FieldInfo field) {
		if (field is null)
			throw new ArgumentNullException(nameof(field));
		if (!field.FieldType.IsAssignableTo(typeof(TValue)))
			throw new InvalidCastException($"{typeof(TValue)} is not assignable from field type {field.FieldType}");
		if (!field.IsStatic)
			throw new ArgumentException("field is not static");

		if (!FieldGetters.TryGetValue(field, out var getter)) {
			DynamicMethod dm = new(MakeAccessorName("Get", field), typeof(TValue), null, true);

			var generator = dm.GetILGenerator();
			generator.Emit(OpCodes.Ldsfld, field);
			generator.Emit(OpCodes.Ret);

			getter = dm.CreateDelegate(typeof(Func<TValue>));
			FieldGetters[field] = getter;
		}

		return (Func<TValue>) getter;
	}

	/// <summary>
	/// Use <see cref="System.Reflection.Emit"/> to create a dynamic method
	/// for writing a value to a static field. This is much more efficient
	/// than calling <see cref="FieldInfo.SetValue(object?, object?)"/>.
	/// </summary>
	/// <typeparam name="TValue">The type of the field.</typeparam>
	/// <param name="field">The <see cref="FieldInfo"/> to access.</param>
	/// <returns>A function for writing the value.</returns>
	/// <exception cref="ArgumentNullException">If either argument is null</exception>
	/// <exception cref="ArgumentException">If the field is not static</exception>
	/// <exception cref="InvalidCastException">If the provided <typeparamref name="TValue"/> is not the field's type</exception>
	internal static Action<TValue> CreateSetter<TValue>(this FieldInfo field) {
		if (field is null)
			throw new ArgumentNullException(nameof(field));
		if (!field.FieldType.IsAssignableTo(typeof(TValue)))
			throw new InvalidCastException($"{typeof(TValue)} is not assignable from field type {field.FieldType}");
		if (!field.IsStatic)
			throw new ArgumentException("field is not static");

		if (!FieldSetters.TryGetValue(field, out var setter)) {
			DynamicMethod dm = new(MakeAccessorName("Set", field), null, [typeof(TValue)], true);

			var generator = dm.GetILGenerator();
			generator.Emit(OpCodes.Ldarg_0);
			generator.Emit(OpCodes.Stsfld, field);
			generator.Emit(OpCodes.Ret);

			setter = dm.CreateDelegate(typeof(Action<TValue>));
			FieldSetters[field] = setter;
		}

		return (Action<TValue>) setter;
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
		if (field is null || field.DeclaringType is null)
			throw new ArgumentNullException(nameof(field));
		if (typeof(TOwner) != typeof(object) && !field.DeclaringType.IsAssignableFrom(typeof(TOwner)))
			throw new InvalidCastException($"{typeof(TOwner)} is not assignable to declaring type {field.DeclaringType}");
		if (!field.FieldType.IsAssignableTo(typeof(TValue)))
			throw new InvalidCastException($"{typeof(TValue)} is not assignable from field type {field.FieldType}");
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
		if (field is null || field.DeclaringType is null)
			throw new ArgumentNullException(nameof(field));
		if (typeof(TOwner) != typeof(object) && !field.DeclaringType.IsAssignableFrom(typeof(TOwner)))
			throw new InvalidCastException($"{typeof(TOwner)} is not assignable to declaring type {field.DeclaringType}");
		if (!field.FieldType.IsAssignableTo(typeof(TValue)))
			throw new InvalidCastException($"{typeof(TValue)} is not assignable from field type {field.FieldType}");
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

	#region Methods

	internal static Func<TOwner, TResult> CreateGenericFunc<TOwner, TResult>(this MethodInfo method, params Type[] typeArguments) {
		var generic = method.MakeGenericMethod(typeArguments);
		return generic.CreateFunc<TOwner, TResult>();
	}

	internal static Func<TOwner, T1, TResult> CreateGenericFunc<TOwner, T1, TResult>(this MethodInfo method, params Type[] typeArguments) {
		var generic = method.MakeGenericMethod(typeArguments);
		return generic.CreateFunc<TOwner, T1, TResult>();
	}

	private static readonly Dictionary<MethodInfo, Delegate> MethodCallers = new();

	internal static Delegate CreateFuncInner(this MethodInfo method, Type ownerType, Type resultType, params Type[] types) {
		if (method is null || method.DeclaringType is null)
			throw new ArgumentNullException(nameof(method));
		if (!resultType.IsAssignableFrom(method.ReturnType))
			throw new InvalidCastException($"{resultType} is not assignable from return type {method.ReturnType}");
		if (ownerType != typeof(object) && !method.DeclaringType.IsAssignableFrom(ownerType))
			throw new InvalidCastException($"{ownerType} is not assignable to declaring type {method.DeclaringType}");
		if (method.IsStatic)
			throw new ArgumentException("method is static");

		var parms = method.GetParameters();
		if (parms.Length != types.Length)
			throw new ArgumentException("incorrect parameter count");

		for (int i = 0; i < types.Length; i++) {
			if (!parms[i].ParameterType.IsValueType && types[i] == typeof(object))
				continue;
			if (!parms[i].ParameterType.IsAssignableFrom(types[i]))
				throw new ArgumentException($"Parameter type mismatch at index {i}. Expected: {parms[i].ParameterType}, Actual: {types[i]}");
		}

		if (!MethodCallers.TryGetValue(method, out var caller)) {
			Type[] finalTypes = [ownerType, .. types];
			var delegateType = Expression.GetFuncType([.. finalTypes, resultType]);

			DynamicMethod dm = new(MakeAccessorName("Call", method), resultType, finalTypes, true);

			var generator = dm.GetILGenerator();

			if (ownerType.IsValueType)
				generator.Emit(OpCodes.Ldarga_S, (byte) 0);
			else
				generator.Emit(OpCodes.Ldarg_0);

			for (byte i = 1; i <= parms.Length; i++)
				generator.Emit(OpCodes.Ldarg_S, i);

			generator.Emit(OpCodes.Call, method);
			generator.Emit(OpCodes.Ret);

			caller = dm.CreateDelegate(delegateType);
			MethodCallers[method] = caller;
		}

		return caller;
	}


	internal static Func<TOwner, TResult> CreateFunc<TOwner, TResult>(this MethodInfo method) {
		return (Func<TOwner, TResult>) CreateFuncInner(method, typeof(TOwner), typeof(TResult));
	}

	internal static Func<TOwner, TArg1, TResult> CreateFunc<TOwner, TArg1, TResult>(this MethodInfo method) {
		return (Func<TOwner, TArg1, TResult>) CreateFuncInner(method, typeof(TOwner), typeof(TResult), typeof(TArg1));
	}

	internal static Func<TOwner, TArg1, TArg2, TResult> CreateFunc<TOwner, TArg1, TArg2, TResult>(this MethodInfo method) {
		return (Func<TOwner, TArg1, TArg2, TResult>) CreateFuncInner(method, typeof(TOwner), typeof(TResult), typeof(TArg1), typeof(TArg2));
	}

	internal static Delegate CreateActionInner(this MethodInfo method, Type ownerType, params Type[] types) {
		if (method is null || method.DeclaringType is null)
			throw new ArgumentNullException(nameof(method));
		if (ownerType != typeof(object) && !method.DeclaringType.IsAssignableFrom(ownerType))
			throw new InvalidCastException($"{ownerType} is not same as parent type {method.ReflectedType}");
		if (method.IsStatic)
			throw new ArgumentException("method is static");

		var parms = method.GetParameters();
		if (parms.Length != types.Length)
			throw new ArgumentException("incorrect parameter count");

		for (int i = 0; i < types.Length; i++) {
			if (!parms[i].ParameterType.IsValueType && types[i] == typeof(object))
				continue;
			if (!parms[i].ParameterType.IsAssignableFrom(types[i]))
				throw new ArgumentException($"Parameter type mismatch at index {i}. Expected: {parms[i].ParameterType}, Actual: {types[i]}");
		}

		if (true || !MethodCallers.TryGetValue(method, out var caller)) {
			Type[] finalTypes = [ownerType, .. types];
			var delegateType = Expression.GetActionType(finalTypes);

			DynamicMethod dm = new(MakeAccessorName("Call", method), null, finalTypes, true);

			var generator = dm.GetILGenerator();

			if (ownerType.IsValueType)
				generator.Emit(OpCodes.Ldarga_S, (byte) 0);
			else
				generator.Emit(OpCodes.Ldarg_0);

			for (byte i = 1; i <= parms.Length; i++)
				generator.Emit(OpCodes.Ldarg_S, i);

			generator.Emit(OpCodes.Call, method);
			generator.Emit(OpCodes.Ret);

			caller = dm.CreateDelegate(delegateType);
			MethodCallers[method] = caller;
		}

		return caller;
	}


	internal static Action<TOwner> CreateAction<TOwner>(this MethodInfo method) {
		return (Action<TOwner>) CreateActionInner(method, typeof(TOwner));
	}

	internal static Action<TOwner, TArg1> CreateAction<TOwner, TArg1>(this MethodInfo method) {
		return (Action<TOwner, TArg1>) CreateActionInner(method, typeof(TOwner), typeof(TArg1));
	}

	internal static Action<TOwner, TArg1, TArg2> CreateAction<TOwner, TArg1, TArg2>(this MethodInfo method) {
		return (Action<TOwner, TArg1, TArg2>) CreateActionInner(method, typeof(TOwner), typeof(TArg1), typeof(TArg2));
	}

	internal static Action<TOwner, TArg1, TArg2, TArg3> CreateAction<TOwner, TArg1, TArg2, TArg3>(this MethodInfo method) {
		return (Action<TOwner, TArg1, TArg2, TArg3>) CreateActionInner(method, typeof(TOwner), typeof(TArg1), typeof(TArg2), typeof(TArg3));
	}

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
