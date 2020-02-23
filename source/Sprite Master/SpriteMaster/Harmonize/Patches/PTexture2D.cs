using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpriteMaster.Types;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using static SpriteMaster.Harmonize.Harmonize;

namespace SpriteMaster.Harmonize.Patches {
	[SuppressMessage("Code Quality", "IDE0051:Remove unused private members", Justification = "Harmony")]
	[SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Harmony")]
	internal static class PTexture2D {
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static unsafe byte[] ExtractByteArray<T>(T[] data, int length, int typeSize) where T : struct {
			var byteData = new byte[length * typeSize];
			var handle = GCHandle.Alloc(data, GCHandleType.Pinned);
			try {
				Marshal.Copy(handle.AddrOfPinnedObject(), byteData, 0, byteData.Length);
			}
			finally {
				handle.Free();
			}
			return byteData;
		}

		// Always returns a duplicate of the array, since we do not own the source array.
		// It performs a shallow copy, which is fine.
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static unsafe byte[] GetByteArray<T>(T[] data, out int typeSize) where T : struct {
			switch (data) {
				case null:
					typeSize = Marshal.SizeOf(typeof(T));
					return null;
				case byte[] byteData:
					typeSize = sizeof(byte);
					return (byte[])byteData.Clone();
				case Color[] colorData:
					typeSize = sizeof(Color);
					return ExtractByteArray(colorData, data.Length, typeSize);
				case var _ when (typeof(T).IsPrimitive || typeof(T).IsPointer || typeof(T).IsEnum):
					typeSize = Marshal.SizeOf(typeof(T));
					var byteArray = new byte[data.Length * typeSize];
					Buffer.BlockCopy(data, 0, byteArray, 0, byteArray.Length);
					return byteArray;
				default:
					typeSize = Marshal.SizeOf(typeof(T));
					return ExtractByteArray(data, data.Length, typeSize);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static bool Cacheable(Texture2D texture) {
			return texture.LevelCount <= 1;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static void SetDataPurge<T>(Texture2D texture, Rectangle? rect, T[] data, int startIndex, int elementCount) where T : struct {
			if (texture is ManagedTexture2D) {
				return;
			}

			if (!ScaledTexture.Validate(texture)) {
				return;
			}

			int elementSize = 0;
			var byteData = Cacheable(texture) ? GetByteArray(data, out elementSize) : null;

#if ASYNC_SETDATA
			ThreadQueue.Queue((data) =>
				Thread.CurrentThread.Priority = ThreadPriority.BelowNormal;
				ScaledTexture.Purge(
					reference: texture,
					bounds: rect,
					data: new DataRef<byte>(
						data: data,
						offset: startIndex * elementSize,
						length: elementCount * elementSize
					)
				), byteData);
#else
			ScaledTexture.Purge(
				reference: texture,
				bounds: rect,
				data: new DataRef<byte>(
					data: byteData,
					offset: startIndex * elementSize,
					length: elementCount * elementSize
				)
			);
#endif
		}

		[Harmonize("SetData", HarmonizeAttribute.Fixation.Postfix, PriorityLevel.Last, HarmonizeAttribute.Generic.Struct)]
		private static void OnSetDataPost<T> (Texture2D __instance, T[] data) where T : struct {
			using var _ = Performance.Track("SetData1");
			SetDataPurge(
				__instance,
				null,
				data,
				0,
				data.Length
			);
		}

		[Harmonize("SetData", HarmonizeAttribute.Fixation.Postfix, PriorityLevel.Last, HarmonizeAttribute.Generic.Struct)]
		private static void OnSetDataPost<T> (Texture2D __instance, T[] data, int startIndex, int elementCount) where T : struct {
			using var _ = Performance.Track("SetData3");
			SetDataPurge(
				__instance,
				null,
				data,
				startIndex,
				elementCount
			);
		}

		[Harmonize("SetData", HarmonizeAttribute.Fixation.Postfix, PriorityLevel.Last, HarmonizeAttribute.Generic.Struct)]
		private static void OnSetDataPost<T> (Texture2D __instance, int level, Rectangle? rect, T[] data, int startIndex, int elementCount) where T : struct {
			using var _ = Performance.Track("SetData4");
			SetDataPurge(
				__instance,
				rect,
				data,
				startIndex,
				elementCount
			);
		}

		// A horrible, horrible hack to stop a rare-ish crash when zooming or when the device resets. It doesn't appear to originate in SpriteMaster, but SM most certainly
		// makes it worse. This will force the texture to regenerate on the fly if it is in a zombie state.
		[Harmonize("Microsoft.Xna.Framework", "Microsoft.Xna.Framework.Helpers", "CheckDisposed", HarmonizeAttribute.Fixation.Prefix, PriorityLevel.Last, instance: false, platform: HarmonizeAttribute.Platform.Windows)]
		private static unsafe bool CheckDisposed (object obj, ref IntPtr pComPtr) {
			if (obj is ManagedTexture2D) {
				return true;
			}

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
