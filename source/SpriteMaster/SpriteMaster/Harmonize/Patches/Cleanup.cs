/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

using Microsoft.Xna.Framework.Graphics;
using SpriteMaster.Extensions;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Threading;

using static SpriteMaster.Harmonize.Harmonize;

namespace SpriteMaster.Harmonize.Patches;

[SuppressMessage("Code Quality", "IDE0051:Remove unused private members", Justification = "Harmony")]
[SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Harmony")]
static class Cleanup {
	[Harmonize("~GraphicsResource", Harmonize.Fixation.Postfix, PriorityLevel.Last, platform: Harmonize.Platform.XNA)]
	private static void Finalize(GraphicsResource __instance) {
		FinalizePost(__instance);
	}

	private static readonly ThreadLocal<object?> CurrentFinalizer = new();
	/*
	[Harmonize("Finalize", Harmonize.Fixation.Prefix, PriorityLevel.First, platform: Harmonize.Platform.All)]
	private static bool FinalizePre(object __instance) {
		try {
			return (CurrentFinalizer.Value != __instance);
		}
		catch (ObjectDisposedException) { return true; }
	}
	*/

	[Harmonize("Finalize", Harmonize.Fixation.Postfix, PriorityLevel.Last, platform: Harmonize.Platform.All)]
	private static void FinalizePost(object __instance) {
		try {
			if (CurrentFinalizer.Value == __instance) {
				return;
			}
			Contracts.AssertNull(CurrentFinalizer.Value);
			if (__instance is GraphicsResource resource) {
				if (Config.Garbage.LeakPreventTexture) {
					if (!resource.IsDisposed) {
						CurrentFinalizer.Value = resource;
						try {
							resource.Dispose();
						}
						finally {
							CurrentFinalizer.Value = null;
						}
						if (__instance is Texture2D texture) {
							Debug.ErrorLn($"Leak Corrected for {resource.GetType().FullName} {resource.ToString()} ({texture.SizeBytes().AsDataSize()})");
						}
						else {
							Debug.ErrorLn($"Leak Corrected for {resource.GetType().FullName} {resource.ToString()}");
						}
					}
				}
			}
			else if (__instance is IDisposable @this) {
				if (Config.Garbage.LeakPreventAll) {
					// does it have an 'IsDisposed' like much of XNA?
					var type = @this.GetType();
					const BindingFlags bindingAttr = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

					if (type.TryGetProperty("IsDisposed", out var disposedProperty, bindingAttr) && ((bool?)disposedProperty?.GetValue(@this) ?? false)) {
						return;
					}
					if (type.TryGetField("IsDisposed", out var disposedField, bindingAttr) && ((bool?)disposedProperty?.GetValue(@this) ?? false)) {
						return;
					}

					Contracts.AssertNull(CurrentFinalizer.Value);
					CurrentFinalizer.Value = @this;

					if (disposedProperty is not null || disposedField is not null) {
						//Debug.WarningLn($"Leak Corrected for {@this.GetType().FullName} {@this.ToString()}");
					}

					try {
						@this.Dispose();
					}
					finally {
						CurrentFinalizer.Value = null;
					}
				}
			}
		}
		catch (ObjectDisposedException) { }
	}
}
