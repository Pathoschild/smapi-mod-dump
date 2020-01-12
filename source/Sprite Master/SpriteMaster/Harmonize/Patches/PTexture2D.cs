using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpriteMaster.Types;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using static SpriteMaster.Harmonize.Harmonize;
using static SpriteMaster.ScaledTexture;

namespace SpriteMaster.Harmonize.Patches {
	[SuppressMessage("Code Quality", "IDE0051:Remove unused private members", Justification = "Harmony")]
	[SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Harmony")]
	internal static class PTexture2D {
		[Harmonize("SetData", HarmonizeAttribute.Fixation.Postfix, PriorityLevel.Last, HarmonizeAttribute.Generic.Struct)]
		private static void OnSetDataPost<T> (Texture2D __instance, T[] data) where T : unmanaged {
			if (__instance is ManagedTexture2D) {
				return;
			}

			if (!ScaledTexture.Validate(__instance)) {
				return;
			}

			var dataRef = DataRef<byte>.Null;
			if (__instance.LevelCount <= 1) {
				dataRef = (byte[])new Span<T>(data).As<byte>().ToArray().Clone();
			}

			ScaledTexture.Purge(__instance, null, dataRef);
		}

		[Harmonize("SetData", HarmonizeAttribute.Fixation.Postfix, PriorityLevel.Last, HarmonizeAttribute.Generic.Struct)]
		private static void OnSetDataPost<T> (Texture2D __instance, T[] data, int startIndex, int elementCount) where T : unmanaged {
			if (__instance is ManagedTexture2D) {
				return;
			}

			if (!ScaledTexture.Validate(__instance)) {
				return;
			}

			var dataRef = DataRef<byte>.Null;
			if (__instance.LevelCount <= 1) {
				dataRef = new DataRef<byte>((byte[])new Span<T>(data).As<byte>().ToArray().Clone(), startIndex, elementCount);
			}

			ScaledTexture.Purge(__instance, null, dataRef);
		}

		[Harmonize("SetData", HarmonizeAttribute.Fixation.Postfix, PriorityLevel.Last, HarmonizeAttribute.Generic.Struct)]
		private static void OnSetDataPost<T> (Texture2D __instance, int level, Rectangle? rect, T[] data, int startIndex, int elementCount) where T : unmanaged {
			if (__instance is ManagedTexture2D) {
				return;
			}

			if (!ScaledTexture.Validate(__instance)) {
				return;
			}

			var dataRef = DataRef<byte>.Null;
			if (__instance.LevelCount <= 1) {
				dataRef = new DataRef<byte>((byte[])new Span<T>(data).As<byte>().ToArray().Clone(), startIndex, elementCount);
			}

			ScaledTexture.Purge(__instance, rect, dataRef);
		}

		// A horrible, horrible hack to stop a rare-ish crash when zooming or when the device resets. It doesn't appear to originate in SpriteMaster, but SM most certainly
		// makes it worse. This will force the texture to regenerate on the fly if it is in a zombie state.
		[Harmonize("Microsoft.Xna.Framework", "Microsoft.Xna.Framework.Helpers", "CheckDisposed", HarmonizeAttribute.Fixation.Prefix, PriorityLevel.Last, instance: false, platform: HarmonizeAttribute.Platform.Windows)]
		private static unsafe bool CheckDisposed (object obj, ref IntPtr pComPtr) {
			if (obj is GraphicsResource resource) {
				if (pComPtr == IntPtr.Zero || resource.IsDisposed) {
					if (!resource.IsDisposed) {
						resource.Dispose();
					}

					if (resource is Texture2D texture) {
						Debug.WarningLn("CheckDisposed is going to throw, attempting to restore state");

						// TODO : we should probably use the helper function it calls instead, just in case the user defined a child class.
						var ctor = texture.GetType().GetConstructor(
							BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
							null,
							new [] {
								typeof(GraphicsDevice),
								typeof(int),
								typeof(int),
								typeof(bool),
								typeof(SurfaceFormat)
							},
							null
						);

						ctor.Invoke(texture, new object[] { DrawState.Device, texture.Width, texture.Height, texture.LevelCount > 1, texture.Format });
						//pComPtr = (IntPtr)(void*)texture.GetField("pComPtr");
						return false;
					}
				}
			}
			return true;
		}
	}
}
