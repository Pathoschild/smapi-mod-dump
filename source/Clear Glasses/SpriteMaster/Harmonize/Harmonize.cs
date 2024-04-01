/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aurpine/Stardew-SpriteMaster
**
*************************************************/

using HarmonyLib;
using LinqFasterer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pastel;
using SpriteMaster.Extensions;
using SpriteMaster.Extensions.Reflection;
using SpriteMaster.Types;
using SpriteMaster.Types.Fixed;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Threading.Tasks;

namespace SpriteMaster.Harmonize;

internal static class Harmonize {
	private const BindingFlags InstanceFlags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;
	private const BindingFlags StaticFlags = BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public;

	internal const string Constructor = ".ctor";

	internal enum Fixation {
		Prefix,
		Postfix,
		Finalizer,
		Transpile,
		Reverse,
		ReversePatched
	}

	internal enum Generic {
		None,
		Struct,
		GfxIndex,
		GfxVertex,
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

	internal enum PriorityLevel {
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

	internal static readonly Type[] StructTypes = {
		typeof(XColor),
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
		typeof(XVector2),
		typeof(Vector3),
		typeof(Vector4),
		typeof(DrawingColor),
		typeof(Color8),
		typeof(Color16),
		typeof(Fixed8),
		typeof(Fixed16),
		typeof(VertexPosition),
		typeof(VertexPositionColor),
		typeof(VertexPositionColorTexture),
		typeof(VertexPositionNormalTexture),
		typeof(VertexPositionTexture)
	};

	internal static readonly Type[] IndexTypes = {
		typeof(short),
		typeof(ushort),
		typeof(int),
		typeof(uint)
	};

	internal static readonly Type[] VertexTypes = {
		typeof(VertexPosition),
		typeof(VertexPositionColor),
		typeof(VertexPositionColorTexture),
		typeof(VertexPositionNormalTexture),
		typeof(VertexPositionTexture)
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

			if (attribute.Enabled is { } enablement) {
				try {
					object? fieldValue;
					bool constant = false;

					if (enablement.Type.GetField(
								enablement.Member, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static
							) is { } field) {
						constant = field.Attributes.HasFlag(FieldAttributes.Literal);
						fieldValue = field.GetValue(null);
					}
					else if (enablement.Type.GetProperty(
										enablement.Member, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static
									) is { } property) {
						constant = !property.CanWrite;
						fieldValue = property.GetValue(null);
					}
					else {
						throw new Exception($"Could not access member {enablement.Type.FullName}.{enablement.Member}");
					}

					if (constant) {
						var enabled = fieldValue switch {
							bool v => v,
							Lazy<bool> v => v.Value,
							_ => Convert.ToBoolean(fieldValue)
						};

						if (!enabled) {
							return;
						}
					}
				}
				catch (Exception ex) {
					Debug.Trace($"Failed to conditionally disable Method Patch {GetFullMethodName(type, method, attribute)} ({method.GetFullName()})", ex);
				}
			}

			if (attribute.ForMod is not null && SpriteMaster.Self.Helper.ModRegistry.Get(attribute.ForMod) is null) {
				Debug.Trace($"Skipping Method Patch {GetFullMethodName(type, method, attribute)} ({method.GetFullName()}): mod '{attribute.ForMod}' not loaded");
				return;
			}

			string methodName = GetMethodName(method, attribute);
			Debug.Trace($"Patching Method {GetFullMethodName(type, method, attribute)} ({method.GetFullName()})");

			var instanceType = attribute.Type;
			if (instanceType is null) {
				var instancePar = method.GetParameters().WhereF(p => p.Name == "__instance");
				(instancePar.Count != 0).AssertTrue($"Type not specified for method {method.GetFullName()}, but no __instance argument present");
				instanceType = instancePar[0].ParameterType.RemoveRef();
			}

			bool wasError = false;

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
						reversePatched: (attribute.PatchFixation == Fixation.ReversePatched) ? method : null,
						instanceMethod: attribute.Instance,
						attribute: attribute
					);
					break;
				case Generic.Struct:
				case Generic.GfxIndex:
				case Generic.GfxVertex:

					static Type[] GetTypes(Generic generic) => generic switch {
						Generic.Struct => StructTypes,
						Generic.GfxIndex => IndexTypes,
						Generic.GfxVertex => VertexTypes,
						_ => StructTypes
					};

					foreach (var structType in attribute.GenericTypes ?? GetTypes(attribute.GenericType)) {
						if (
							attribute.GenericConstraints is { } constraints &&
							!constraints.AnyF(constraint => structType.IsAssignableTo(constraint))
						) {
							continue;
						}

						try {
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
								reversePatched: (attribute.PatchFixation == Fixation.ReversePatched) ? method : null,
								instanceMethod: attribute.Instance,
								attribute: attribute
							);
						}
						catch (Exception ex) {
							Debug.ConditionalError(attribute.Critical, $"Exception Patching Method {GetFullMethodName(type, method, attribute)}<{structType.Name}> ({(method?.GetFullName() ?? "[null]")})", ex);
							wasError = true;
						}
					}
					break;
				case Generic.Class:
					foreach (var objectType in attribute.GenericTypes ?? new[] {typeof(object)}) {

						if (
							attribute.GenericConstraints is { } constraints &&
							!constraints.AnyF(constraint => objectType.IsAssignableTo(constraint))
						) {
							continue;
						}

						try {
							Patch(
								@this,
								instanceType,
								objectType,
								methodName,
								pre: (attribute.PatchFixation == Fixation.Prefix) ? method : null,
								post: (attribute.PatchFixation == Fixation.Postfix) ? method : null,
								finalizer: (attribute.PatchFixation == Fixation.Finalizer) ? method : null,
								trans: (attribute.PatchFixation == Fixation.Transpile) ? method : null,
								reverse: (attribute.PatchFixation == Fixation.Reverse) ? method : null,
								reversePatched: (attribute.PatchFixation == Fixation.ReversePatched) ? method : null,
								instanceMethod: attribute.Instance,
								attribute: attribute
							);
						}
						catch (Exception ex) {
							Debug.ConditionalError(attribute.Critical, $"Exception Patching Method {GetFullMethodName(type, method, attribute)}<{objectType.Name}> ({(method?.GetFullName() ?? "[null]")})", ex);
							wasError = true;
						}
					}
					break;
				default:
					throw new NotImplementedException($"Unknown Generic Enum: {attribute.GenericType}");
			}

			if (wasError) {
				Debug.Warning( $"There were exceptions patching Method {GetFullMethodName(type, method, attribute)} ({(method?.GetFullName() ?? "[null]")})");
			}
		}
		catch (Exception ex) {
			Debug.ConditionalError(attribute.Critical, $"Exception Patching Method {GetFullMethodName(type, method, attribute)} ({(method?.GetFullName() ?? "[null]")})", ex);
		}
	}

	private static void ApplyPatches(Harmony @this, Type type, MethodInfo method, IEnumerable<HarmonizeAttribute> attributes, bool early) {
		try {
			Parallel.ForEach(attributes, attribute => {
				if (early == (attribute.ForMod is null)) {
					ApplyPatch(@this, type, method, attribute);
				}
			});
		}
		catch (Exception ex) {
			Debug.Warning($"Exception Patching Method {method.GetFullName()}", ex);
		}
	}

	internal static void ApplyPatches(this Harmony @this, bool early) {
		@this.AssertNotNull();
		Debug.Trace($"Applying Patches ({(early ? "early" : "late")})");
		var assembly = typeof(Harmonize).Assembly;
		Parallel.ForEach(
			assembly.GetTypes(), type => {
				var typeAttributes = type.GetCustomAttributes<HarmonizeFinalizeCatcherFixedAttribute>();
				Parallel.ForEach(typeAttributes, attribute => {
					if (early == (attribute.ForMod is null)) {
						ApplyPatch(
							@this: @this,
							type: type,
							method: attribute.MethodInfo,
							attribute: attribute
						);
					}
				});

				Parallel.ForEach(type.GetMethods(StaticFlags), method => {
					var conditionalAttributes = method.GetCustomAttributes<HarmonizeConditionalAttribute>();
					bool enable = true;
					foreach (var conditionalAttribute in conditionalAttributes) {
						enable &= conditionalAttribute.Condition;
					}

					if (enable) {
						ApplyPatches(
							@this: @this,
							type: type,
							method: method,
							attributes: method.GetCustomAttributes<HarmonizeAttribute>(),
							early: early
						);
					}
				});
			}
		);
		Debug.Trace("Done Applying Patches");
	}

	private static Type[] GetArguments(this MethodInfo method, HarmonizeAttribute attribute) {
		if (attribute.ArgumentTypes is not null) {
			return attribute.ArgumentTypes;
		}

		var filteredParameters = method.GetParameters().WhereF(t => t.Name is null || !t.Name.StartsWith("__") || t.Name.StartsWith("__unnamed"));
		return filteredParameters.SelectF(p => p.ParameterType).ToArrayF();
	}

	private static MethodBase? GetPatchMethod(Type type, string name, MethodInfo method, HarmonizeAttribute attribute, bool instance, bool isFinalizer, bool transpile = false) {
		bool constructor = (name == Constructor);
		var flags = instance ? InstanceFlags : StaticFlags;

		if (transpile) {
			if (constructor) {
				// TODO fix me
				return type.GetConstructor(attribute.ArgumentTypes ?? Type.EmptyTypes);
			}

			if (attribute.ArgumentTypes is null) {
				Debug.Warning($"Transpile method for {method.GetFullName()} specified, but no argument types provided");
				return null;
			}
		}

		var bindingFlags = instance ? InstanceFlags : StaticFlags;

		{
			// Handle Properties
			bool get = name.EndsWith(".get");
			bool set = !get && name.EndsWith(".set");
			if (get || set) {
				var property = type.GetProperty(name[..name.IndexOf('.')], bindingFlags);
				if (property is null) {
					return null;
				}

				var result = get ? property.GetMethod : property.SetMethod;
				return result;
			}
		}

		var methodParameters = method.GetArguments(attribute);

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
					if (typeMethod.DeclaringType == typeof(object)) {
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

		if (typeMethod is not null) {
			return typeMethod;
		}

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
					testParameterRef != methodParameterRef &&
					!(
						(testBaseParameter.IsGenericType && baseParameter.IsGenericType) ||
						(testBaseParameter.IsGenericParameter && baseParameter.IsGenericParameter)
					) &&
					methodParameter != typeof(object) &&
					!(
						testParameter.IsArray &&
						methodParameter.IsArray &&
						(testBaseParameter.IsAssignableTo(baseParameter) || baseParameter == typeof(object) || baseParameter == typeof(Array))
					) &&
					!testParameter.IsAssignableTo(methodParameter)
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
		return typeMethod;
	}

	internal static int GetPriority(MethodInfo method, int defaultPriority) {
		try {
			if (method.GetCustomAttribute<HarmonyPriority>() is { } priorityAttribute) {
				return priorityAttribute.info.priority;
			}
		}
		catch {
			// ignored
		}

		return defaultPriority;
	}

	private static void Patch(
		this Harmony instance,
		Type type,
		string name,
		HarmonizeAttribute attribute,
		MethodInfo? pre = null,
		MethodInfo? post = null,
		MethodInfo? finalizer = null,
		MethodInfo? trans = null,
		MethodInfo? reverse = null,
		MethodInfo? reversePatched = null,
		int priority = Priority.Last,
		bool instanceMethod = true
	) {
		[return: NotNullIfNotNull("methodInfo")]
		HarmonyMethod? MakeHarmonyMethod(MethodInfo? methodInfo) =>
			(methodInfo is null) ? null : new(methodInfo) { priority = GetPriority(methodInfo, priority) };

		var referenceMethod = pre ?? post ?? finalizer;
		if (referenceMethod is not null) {
			var typeMethod = GetPatchMethod(
				type: type,
				name: name,
				method: referenceMethod,
				attribute: attribute,
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
				type: type,
				name: name,
				method: referenceMethod ?? trans,
				attribute: attribute,
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
				type: type,
				name: name,
				method: referenceMethod ?? reverse,
				attribute: attribute,
				instance: instanceMethod,
				isFinalizer: referenceMethod == finalizer
			) ?? throw new ArgumentException($"Method '{name}' in type '{type.FullName}' could not be found");
			instance.CreateReversePatcher(
				original: typeMethod,
				standin: MakeHarmonyMethod(reverse)
			).Patch(HarmonyReversePatchType.Original);
		}
		if (reversePatched is not null) {
			priority = Priority.Normal;

			var typeMethod = GetPatchMethod(
				type: type,
				name: name,
				method: referenceMethod ?? reversePatched,
				attribute: attribute,
				instance: instanceMethod,
				isFinalizer: referenceMethod == finalizer
			) ?? throw new ArgumentException($"Method '{name}' in type '{type.FullName}' could not be found");

			var patchInfo = Harmony.GetPatchInfo(typeMethod);
			bool isPatched =
				(patchInfo?.Finalizers.Count ?? 0) != 0 ||
				(patchInfo?.Postfixes.Count ?? 0) != 0 ||
				(patchInfo?.Prefixes.Count ?? 0) != 0 ||
				(patchInfo?.Transpilers.Count ?? 0) != 0;

			instance.CreateReversePatcher(
				original: typeMethod,
				standin: MakeHarmonyMethod(reversePatched)
			).Patch(isPatched ? HarmonyReversePatchType.Snapshot : HarmonyReversePatchType.Original);
		}
	}

	internal static void Patch(
		this Harmony instance,
		Type type,
		Type genericType,
		string name,
		HarmonizeAttribute attribute,
		MethodInfo? pre = null,
		MethodInfo? post = null,
		MethodInfo? finalizer = null,
		MethodInfo? trans = null,
		MethodInfo? reverse = null,
		MethodInfo? reversePatched = null,
		int priority = Priority.Last,
		bool instanceMethod = true
	) {
		[return: NotNullIfNotNull("methodInfo")]
		HarmonyMethod? MakeHarmonyMethod(MethodInfo? methodInfo) =>
			(methodInfo is null) ? null : new(
				methodInfo.ContainsGenericParameters ? methodInfo.MakeGenericMethod(genericType) : methodInfo
			) { priority = GetPriority(methodInfo, priority) };

		var referenceMethod = pre ?? post ?? finalizer;
		if (referenceMethod is not null) {
			var typeMethod = GetPatchMethod(
				type: type,
				name: name,
				method: referenceMethod,
				attribute: attribute,
				instance: instanceMethod,
				isFinalizer: referenceMethod == finalizer
			) ?? throw new ArgumentException($"Method '{name}' in type '{type.FullName}' could not be found");
			var typeMethodInfo = typeMethod as MethodInfo ?? throw new InvalidCastException($"Could not get MethodInfo from '{typeMethod}'");
			var typeMethodInfoGeneric = typeMethodInfo.MakeGenericMethod(genericType);
			_ = instance.Patch(
				original: typeMethodInfoGeneric,
				prefix: MakeHarmonyMethod(pre),
				postfix: MakeHarmonyMethod(post),
				finalizer: MakeHarmonyMethod(finalizer)
			);
		}
		if (trans is not null) {
			var typeMethod = GetPatchMethod(
				type: type,
				name: name,
				method: referenceMethod ?? trans,
				attribute: attribute,
				transpile: true,
				instance: instanceMethod,
				isFinalizer: (referenceMethod ?? trans) == finalizer
			) ?? throw new ArgumentException($"Method '{name}' in type '{type.FullName}' could not be found");
			var typeMethodInfo = typeMethod as MethodInfo ?? throw new InvalidCastException($"Could not get MethodInfo from '{typeMethod}'");
			var typeMethodInfoGeneric = typeMethodInfo.MakeGenericMethod(genericType);
			_ = instance.Patch(
				original: typeMethodInfoGeneric,
				transpiler: MakeHarmonyMethod(trans)
			);
		}
		if (reverse is not null) {
			priority = Priority.Normal;

			var typeMethod = GetPatchMethod(
				type: type,
				name: name,
				method: referenceMethod ?? reverse,
				attribute: attribute,
				instance: instanceMethod,
				isFinalizer: referenceMethod == finalizer
			) ?? throw new ArgumentException($"Method '{name}' in type '{type.FullName}' could not be found");
			var typeMethodInfo = typeMethod as MethodInfo ?? throw new InvalidCastException($"Could not get MethodInfo from '{typeMethod}'");
			var typeMethodInfoGeneric = typeMethodInfo.MakeGenericMethod(genericType);
			_ = instance.CreateReversePatcher(
				original: typeMethodInfoGeneric,
				standin: MakeHarmonyMethod(reverse)
			).Patch(HarmonyReversePatchType.Original);
		}
		if (reversePatched is not null) {
			priority = Priority.Normal;

			var typeMethod = GetPatchMethod(
				type: type,
				name: name,
				method: referenceMethod ?? reversePatched,
				attribute: attribute,
				instance: instanceMethod,
				isFinalizer: referenceMethod == finalizer
			) ?? throw new ArgumentException($"Method '{name}' in type '{type.FullName}' could not be found");
			var typeMethodInfo = typeMethod as MethodInfo ?? throw new InvalidCastException($"Could not get MethodInfo from '{typeMethod}'");
			var typeMethodInfoGeneric = typeMethodInfo.MakeGenericMethod(genericType);
			_ = instance.CreateReversePatcher(
				original: typeMethodInfoGeneric,
				standin: MakeHarmonyMethod(reversePatched)
			).Patch(HarmonyReversePatchType.Snapshot);
		}
	}
}
