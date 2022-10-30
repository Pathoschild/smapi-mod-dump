/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

#if false

using FastExpressionCompiler.LightExpression;
using SpriteMaster.Types;
using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace SpriteMaster.Utility;

internal static class TextureUtility {

	private static readonly Func<IList<object>>? PatchesGetter = GetPatchesGetter();
	private static IList<object>? Patches => PatchesGetter?.Invoke();

	private static Func<IList<object>>? GetPatchesGetter() {
		try {
			// ReSharper disable once StringLiteralTypo
			if (SpriteMaster.Self.Helper.ModRegistry.Get(@"Pathoschild.ContentPatcher") is not { } contentPatcher) {
				return null;
			}

			const BindingFlags bindingFlags = BindingFlags.FlattenHierarchy | BindingFlags.Instance |
																				BindingFlags.Public | BindingFlags.NonPublic;

			object? GetInstanceValue(object obj, string name) {
				var objType = obj.GetType();

				if (objType.GetProperty(name, bindingFlags) is { } propertyInfo) {
					return propertyInfo.GetValue(obj);
				}

				if (objType.GetField(name, bindingFlags) is { } fieldInfo) {
					return fieldInfo.GetValue(obj);
				}

				return null;
			}

			if (GetInstanceValue(contentPatcher, "Mod") is not IMod mod) {
				return null;
			}

			if (GetInstanceValue(mod, "ScreenManager") is not { } screenManagerPerScreen) {
				return null;
			}

			if (screenManagerPerScreen.GetType().GetProperty("Value", bindingFlags) is not { } screenManagerValueProperty) {
				return null;
			}

			if (screenManagerValueProperty.PropertyType.GetProperty("PatchManager", bindingFlags) is not { } patchManagerProperty) {
				return null;
			}

			if (patchManagerProperty.PropertyType.GetProperty("Patches", bindingFlags) is not { } patchesProperty) {
				return null;
			}

			var screenManagerExpression = Expression.Property(Expression.Constant(mod), screenManagerValueProperty);
			var patchManagerExpression = Expression.Property(screenManagerExpression, patchManagerProperty);
			var patchesExpression = Expression.Property(patchManagerExpression, patchesProperty);
			return Expression.Lambda<Func<IList<object>>>(patchesExpression).CompileFast();
		}
		catch {
			// Swallow Exceptions
			return null;
		}
	}

	internal struct PatchInfo {
	}

	internal static PatchInfo[] GetPatches(XTexture2D texture, Bounds source) {
		if (Patches is not { } patches) {
			return Array.Empty<PatchInfo>();
		}

		foreach (var patch in patches) {

		}

		return Array.Empty<PatchInfo>();
	}
}

#endif
