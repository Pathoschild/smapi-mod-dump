/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

using HarmonyLib;
using LinqFasterer;
using Microsoft.Xna.Framework;
using Pastel;
using SpriteMaster.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace SpriteMaster.Harmonize;

static class Harmonize {
	private const BindingFlags InstanceFlags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;
	private const BindingFlags StaticFlags = BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public;

	internal const string Constructor = ".ctor";

	internal enum Fixation {
		Prefix,
		Postfix,
		Finalizer,
		Transpile,
		Reverse
	}

	internal enum Generic {
		None,
		Struct,
		Class
	}

	internal enum Platform {
		All = 0,
		Windows,
		Linux,
		Macintosh,
		Unix,
		XNA,
		MonoGame
	}

	internal enum PriorityLevel : int {
		Last = int.MinValue,
		Lowest = Priority.Last,
		VeryLow = Priority.VeryLow,
		Low = Priority.Low,
		BelowAverage = Priority.LowerThanNormal,
		Average = Priority.Normal,
		AboveAverage = Priority.HigherThanNormal,
		High = Priority.High,
		VeryHigh = Priority.VeryHigh,
		Highest = Priority.First,
		First = int.MaxValue
	}

	internal static readonly Type[] StructTypes = new[] {
		typeof(Color),
		typeof(char),
		typeof(byte),
		typeof(sbyte),
		typeof(short),
		typeof(ushort),
		typeof(int),
		typeof(uint),
		typeof(long),
		typeof(ulong),
		typeof(float),
		typeof(double),
		typeof(Vector2),
		typeof(Vector3),
		typeof(Vector4),
		typeof(System.Drawing.Color)
	};

	private static string GetMethodName(MethodInfo method, HarmonizeAttribute attribute) => attribute.Name ?? method.Name.Split('`', 2)[0];
	private static string GetFullMethodName(Type type, MethodInfo? method, HarmonizeAttribute attribute) => (attribute.Name is not null) ? $"{type.FullName}.{attribute.Name}" : (method?.GetFullName() ?? "[null]");

	private static void ApplyPatch(Harmony @this, Type type, MethodInfo? method, HarmonizeAttribute attribute) {
		try {
			if (!attribute.CheckPlatform()) {
				return;
			}

			if (method is null) {
				throw new ArgumentNullException(nameof(method));
			}

			string methodName = GetMethodName(method, attribute);
			Debug.Trace($"Patching Method {GetFullMethodName(type, method, attribute)} ({method.GetFullName()})");

			var instanceType = attribute.Type;
			if (instanceType is null) {
				var instancePar = method.GetParameters().WhereF(p => p.Name == "__instance");
				Contracts.AssertTrue(instancePar.Count != 0, $"Type not specified for method {method.GetFullName()}, but no __instance argument present");
				instanceType = instancePar[0].ParameterType.RemoveRef();
			}

			switch (attribute.GenericType) {
				case Generic.None:
					Patch(
						@this,
						instanceType,
						methodName,
						pre: (attribute.PatchFixation == Fixation.Prefix) ? method : null,
						post: (attribute.PatchFixation == Fixation.Postfix) ? method : null,
						finalizer: (attribute.PatchFixation == Fixation.Finalizer) ? method : null,
						trans: (attribute.PatchFixation == Fixation.Transpile) ? method : null,
						reverse: (attribute.PatchFixation == Fixation.Reverse) ? method : null,
						instanceMethod: attribute.Instance
					);
					break;
				case Generic.Struct:
					foreach (var structType in StructTypes) {
						//Debug.Trace($"\tGeneric Type: {structType.FullName}");
						Patch(
							@this,
							instanceType,
							structType,
							methodName,
							pre: (attribute.PatchFixation == Fixation.Prefix) ? method : null,
							post: (attribute.PatchFixation == Fixation.Postfix) ? method : null,
							finalizer: (attribute.PatchFixation == Fixation.Finalizer) ? method : null,
							trans: (attribute.PatchFixation == Fixation.Transpile) ? method : null,
							reverse: (attribute.PatchFixation == Fixation.Reverse) ? method : null,
							instanceMethod: attribute.Instance
						);
					}
					break;
				case Generic.Class:
					Patch(
						@this,
						instanceType,
						typeof(object),
						methodName,
						pre: (attribute.PatchFixation == Fixation.Prefix) ? method : null,
						post: (attribute.PatchFixation == Fixation.Postfix) ? method : null,
						finalizer: (attribute.PatchFixation == Fixation.Finalizer) ? method : null,
						trans: (attribute.PatchFixation == Fixation.Transpile) ? method : null,
						reverse: (attribute.PatchFixation == Fixation.Reverse) ? method : null,
						instanceMethod: attribute.Instance
					);
					break;
				default:
					throw new NotImplementedException($"Unknown Generic Enum: {attribute.GenericType}");
			}
		}
		catch (Exception ex) {
			Debug.ConditionalError(attribute.Critical, $"Exception Patching Method {GetFullMethodName(type, method, attribute)} ({(method?.GetFullName() ?? "[null]")})", ex);
		}
	}

	private static void ApplyPatches(Harmony @this, Type type, MethodInfo method, IEnumerable<HarmonizeAttribute> attributes) {
		try {
			if (attributes.IsBlank()) {
				return;
			}

			foreach (var attribute in attributes) {
				ApplyPatch(@this, type, method, attribute);
			}
		}
		catch (Exception ex) {
			Debug.Warning($"Exception Patching Method {method.GetFullName()}", ex);
		}
	}

	internal static void ApplyPatches(this Harmony @this) {
		Contracts.AssertNotNull(@this);
		Debug.Trace("Applying Patches");
		var assembly = typeof(Harmonize).Assembly;
		foreach (var type in assembly.GetTypes()) {
			var typeAttributes = type.GetCustomAttributes<HarmonizeFinalizeCatcherFixedAttribute>();
			foreach (var attribute in typeAttributes) {
				ApplyPatch(
					@this: @this,
					type: type,
					method: attribute.MethodInfo,
					attribute: attribute
				);
			}

			foreach (var method in type.GetMethods(StaticFlags)) {
				ApplyPatches(
					@this: @this,
					type: type,
					method: method,
					attributes: method.GetCustomAttributes<HarmonizeAttribute>()
				);
			}
		}
		Debug.Trace("Done Applying Patches");
	}

	private static Type[] GetArguments(this MethodInfo method) {
		var filteredParameters = method.GetParameters().WhereF(t => t.Name is null || !t.Name.StartsWith("__") || t.Name.StartsWith("__unnamed"));
		return filteredParameters.SelectF(p => p.ParameterType).ToArrayF();
	}

	internal static IList<MethodInfo> GetMethods(this Type type, string name, BindingFlags bindingFlags) {
		return type.GetMethods(bindingFlags).WhereF(t => t.Name == name);
	}

	internal static IList<MethodInfo> GetStaticMethods(this Type type, string name) {
		return type.GetMethods(name, StaticFlags);
	}

	internal static MethodInfo? GetStaticMethod(this Type type, string name) {
		return type.GetMethod(name, StaticFlags);
	}

	internal static IList<MethodInfo> GetInstanceMethods(this Type type, string name) {
		return type.GetMethods(name, InstanceFlags);
	}

	internal static MethodInfo? GetInstanceMethod(this Type type, string name) {
		return type.GetMethod(name, InstanceFlags);
	}

	internal static IList<MethodInfo> GetMethods<T>(string name, BindingFlags bindingFlags) {
		return typeof(T).GetMethods(name, bindingFlags);
	}

	internal static IList<MethodInfo> GetStaticMethods<T>(string name) {
		return typeof(T).GetStaticMethods(name);
	}

	internal static IList<MethodInfo> GetInstanceMethods<T>(string name) {
		return typeof(T).GetInstanceMethods(name);
	}

	private static MethodBase? GetPatchMethod(Type type, string name, MethodInfo method, bool instance, bool isFinalizer, bool transpile = false) {
		bool constructor = (name == Constructor);
		var flags = instance ? InstanceFlags : StaticFlags;

		if (transpile) {
			if (constructor) {
				// TODO fix me
				return type.GetConstructor(Type.EmptyTypes);
			}
			else {
				return type.GetInstanceMethod(name);
			}
		}

		var bindingFlags = instance ? InstanceFlags : StaticFlags;

		var methodParameters = method.GetArguments();

		int numGenericArguments = 0;
		bool isGeneric = method.IsGenericMethod;
		if (isGeneric) {
			numGenericArguments = method.GetGenericArguments().Length;
		}

		if (isFinalizer && !methodParameters.IsEmpty()) {
			// Remove the last (exception) argument for Harmony finalizers
			if (methodParameters.LastF().RemoveRef().IsAssignableTo(typeof(Exception))) {
				Array.Resize(ref methodParameters, methodParameters.Length - 1);
			}
		}

		MethodBase? GetMethod(string name) => constructor ?
			type!.GetConstructor(
				bindingAttr: bindingFlags,
				binder: null,
				types: methodParameters,
				modifiers: null
			) :
			isGeneric ?
				type!.GetMethod(
					name: name,
					genericParameterCount: numGenericArguments,
					bindingAttr: bindingFlags,
					binder: null,
					types: methodParameters,
					modifiers: null
				) :
				type!.GetMethod(
					name: name,
					bindingAttr: bindingFlags,
					binder: null,
					types: methodParameters,
					modifiers: null
				);
		MethodBase? typeMethod = null;
		Exception? exception = null;
		try {
			typeMethod = GetMethod(name);
		}
		catch (Exception ex) {
			exception = ex;
		}

		if (typeMethod is null || exception is not null) {
			if (name[0] == '~') {
				name = "Finalize";
				typeMethod = GetMethod(name);
				if (typeMethod is null) {
					// This shouldn't be possible, as Object has a finalizer.
					return null;
				}
				if (typeMethod.DeclaringType != type) {
					if (typeMethod.DeclaringType == typeof(Object)) {
						return null;
					}
					else {
						if (typeMethod.DeclaringType is not null) {
							type = typeMethod.DeclaringType;
							typeMethod = GetMethod(name);
						}
					}
				}
			}
			else if (exception is not null) {
				throw exception;
			}
		}

		if (typeMethod is null) {
			MethodBase[] typeMethods = constructor ?
				type.GetConstructors(flags) :
				type.GetMethods(name, flags).ToArrayF();

			foreach (var testMethod in typeMethods) {
				// Compare the parameters. Ignore references.
				var testParameters = testMethod.GetParameters();
				if (testParameters.Length != methodParameters.Length) {
					continue;
				}

				if (isGeneric) {
					if (!testMethod.IsGenericMethod) {
						continue;
					}
					if (testMethod.GetGenericArguments().Length != numGenericArguments) {
						continue;
					}
				}

				bool found = true;
				for (int i = 0; i < testParameters.Length; ++i) {
					var testParameter = testParameters[i].ParameterType.RemoveRef();
					var testParameterRef = testParameter.AddRef();
					var testBaseParameter = testParameter.IsArray ? testParameter.GetElementType()! : testParameter;
					var methodParameter = methodParameters[i].RemoveRef();
					var methodParameterRef = methodParameter.AddRef();
					var baseParameter = methodParameter.IsArray ? methodParameter.GetElementType()! : methodParameter;
					if (
						!(testParameter.IsPointer && methodParameter.IsPointer) &&
						!testParameterRef.Equals(methodParameterRef) &&
						!(
							(testBaseParameter.IsGenericType && baseParameter.IsGenericType) ||
							(testBaseParameter.IsGenericParameter && baseParameter.IsGenericParameter)
						) &&
						!methodParameter.Equals(typeof(object)) &&
						!(
							testParameter.IsArray &&
							methodParameter.IsArray &&
							baseParameter.Equals(typeof(object)))
						) {
						found = false;
						break;
					}
				}
				if (found) {
					typeMethod = testMethod;
					break;
				}
			}

			if (typeMethod is null) {
				Debug.Warning($"Failed to patch {type.Name.Pastel(DrawingColor.LightYellow)}.{name.Pastel(DrawingColor.LightYellow)}");
				return null;
			}
		}
		return typeMethod;
	}

	internal static int GetPriority(MethodInfo method, int defaultPriority) {
		try {
			if (method.GetCustomAttribute<HarmonyPriority>() is HarmonyPriority priorityAttribute) {
				return priorityAttribute.info.priority;
			}
		}
		catch { }
		return defaultPriority;
	}

	private static void Patch(
		this Harmony instance,
		Type type,
		string name,
		MethodInfo? pre = null,
		MethodInfo? post = null,
		MethodInfo? finalizer = null,
		MethodInfo? trans = null,
		MethodInfo? reverse = null,
		int priority = Priority.Last,
		bool instanceMethod = true
	) {
		[return: NotNullIfNotNull("methodInfo")]
		HarmonyMethod? MakeHarmonyMethod(MethodInfo? methodInfo) => (methodInfo is null) ? null : new(methodInfo) { priority = GetPriority(methodInfo, priority) };

		var referenceMethod = pre ?? post ?? finalizer;
		if (referenceMethod is not null) {
			var typeMethod = GetPatchMethod(
				type,
				name,
				referenceMethod,
				instance: instanceMethod,
				isFinalizer: referenceMethod == finalizer
			) ?? throw new ArgumentException($"Method '{name}' in type '{type.FullName}' could not be found");
			instance.Patch(
				original: typeMethod,
				prefix: MakeHarmonyMethod(pre),
				postfix: MakeHarmonyMethod(post),
				finalizer: MakeHarmonyMethod(finalizer)
			);
		}
		if (trans is not null) {
			var typeMethod = GetPatchMethod(
				type,
				name,
				referenceMethod ?? trans,
				transpile: true,
				instance: instanceMethod,
				isFinalizer: referenceMethod == finalizer
			) ?? throw new ArgumentException($"Method '{name}' in type '{type.FullName}' could not be found");
			instance.Patch(
				original: typeMethod,
				transpiler: MakeHarmonyMethod(trans)
			);
		}
		if (reverse is not null) {
			priority = Priority.Normal;

			var typeMethod = GetPatchMethod(
				type,
				name,
				referenceMethod ?? reverse,
				instance: instanceMethod,
				isFinalizer: referenceMethod == finalizer
			) ?? throw new ArgumentException($"Method '{name}' in type '{type.FullName}' could not be found");
			instance.CreateReversePatcher(
				original: typeMethod,
				standin: MakeHarmonyMethod(reverse)
			).Patch(HarmonyReversePatchType.Original);
		}
	}

	internal static void Patch(
		this Harmony instance,
		Type type,
		Type genericType,
		string name,
		MethodInfo? pre = null,
		MethodInfo? post = null,
		MethodInfo? finalizer = null,
		MethodInfo? trans = null,
		MethodInfo? reverse = null,
		int priority = Priority.Last,
		bool instanceMethod = true
	) {
		[return: NotNullIfNotNull("methodInfo")]
		HarmonyMethod? MakeHarmonyMethod(MethodInfo? methodInfo) => (methodInfo is null) ? null : new(methodInfo.MakeGenericMethod(genericType)) { priority = GetPriority(methodInfo, priority) };

		var referenceMethod = pre ?? post ?? finalizer;
		if (referenceMethod is not null) {
			var typeMethod = GetPatchMethod(
				type,
				name,
				referenceMethod,
				instance: instanceMethod,
				isFinalizer: referenceMethod == finalizer
			) ?? throw new ArgumentException($"Method '{name}' in type '{type.FullName}' could not be found");
			var typeMethodInfo = (MethodInfo)typeMethod;
			instance.Patch(
				original: typeMethodInfo.MakeGenericMethod(genericType),
				prefix: MakeHarmonyMethod(pre),
				postfix: MakeHarmonyMethod(post),
				finalizer: MakeHarmonyMethod(finalizer)
			);
		}
		if (trans is not null) {
			var typeMethod = GetPatchMethod(
				type,
				name,
				referenceMethod ?? trans,
				transpile: true,
				instance: instanceMethod,
				isFinalizer: referenceMethod == finalizer
			) ?? throw new ArgumentException($"Method '{name}' in type '{type.FullName}' could not be found");
			var typeMethodInfo = (MethodInfo)typeMethod;
			instance.Patch(
				original: typeMethodInfo.MakeGenericMethod(genericType),
				transpiler: MakeHarmonyMethod(trans)
			);
		}
		if (reverse is not null) {
			priority = Priority.Normal;

			var typeMethod = GetPatchMethod(
				type,
				name,
				referenceMethod ?? reverse,
				instance: instanceMethod,
				isFinalizer: referenceMethod == finalizer
			) ?? throw new ArgumentException($"Method '{name}' in type '{type.FullName}' could not be found");
			var typeMethodInfo = (MethodInfo)typeMethod;
			instance.CreateReversePatcher(
				original: typeMethodInfo.MakeGenericMethod(genericType),
				standin: MakeHarmonyMethod(reverse)
			).Patch(HarmonyReversePatchType.Original);
		}
	}
}
