using Microsoft.Xna.Framework.Graphics;
using SpriteMaster.Extensions;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Threading;
using static SpriteMaster.Harmonize.Harmonize;

namespace SpriteMaster.Harmonize.Patches {
	[SuppressMessage("Code Quality", "IDE0051:Remove unused private members", Justification = "Harmony")]
	[SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Harmony")]
	internal static class Cleanup {
		[Harmonize("~GraphicsResource", HarmonizeAttribute.Fixation.Postfix, PriorityLevel.Last, platform: HarmonizeAttribute.Platform.Windows)]
		private static void Finalize (GraphicsResource __instance) {
			FinalizePost(__instance);
		}

		private static readonly ThreadLocal<object> CurrentFinalizer = new ThreadLocal<object>();
		[Harmonize("Finalize", HarmonizeAttribute.Fixation.Prefix, PriorityLevel.First, platform: HarmonizeAttribute.Platform.Windows)]
		private static bool FinalizePre (object __instance) {
			try {
				return (CurrentFinalizer.Value != __instance);
			}
			catch (ObjectDisposedException) { return true; }
		}

		[Harmonize("Finalize", HarmonizeAttribute.Fixation.Postfix, PriorityLevel.Last, platform: HarmonizeAttribute.Platform.Windows)]
		private static void FinalizePost (object __instance) {
			try {
				if (CurrentFinalizer.Value == __instance) {
					return;
				}
				Contract.AssertNull(CurrentFinalizer.Value);
				if (__instance is GraphicsResource resource) {
					if (Config.LeakPreventTexture) {
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
					if (Config.LeakPreventAll) {
						// does it have an 'IsDisposed' like much of XNA?
						var type = @this.GetType();
						const BindingFlags bindingAttr = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

						if (type.TryGetProperty("IsDisposed", out var disposedProperty, bindingAttr) && (bool)disposedProperty.GetValue(@this)) {
							return;
						}
						if (type.TryGetField("IsDisposed", out var disposedField, bindingAttr) && (bool)disposedProperty.GetValue(@this)) {
							return;
						}

						Contract.AssertNull(CurrentFinalizer.Value);
						CurrentFinalizer.Value = @this;

						if (disposedProperty != null || disposedField != null) {
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
}
