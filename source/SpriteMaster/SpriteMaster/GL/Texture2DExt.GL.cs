/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.OpenGL;
using SpriteMaster.Extensions;
using SpriteMaster.Harmonize.Patches;
using SpriteMaster.Types;
using SpriteMaster.Types.Spans;
using StardewModdingAPI;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using static Microsoft.Xna.Framework.Graphics.Texture2D;

namespace SpriteMaster.GL;

internal static class Texture2DExt {
	[StructLayout(LayoutKind.Auto)]
	private readonly ref struct RebindTexture {
		private readonly int? PreviousTexture = null;

		[MethodImpl(Runtime.MethodImpl.Inline)]
		internal RebindTexture(int texture) {
			var currentTexture = GraphicsExtensions.GetBoundTexture2D();
			if (texture == currentTexture) {
				PreviousTexture = null;
			}
			else {
				PreviousTexture = currentTexture;
				MonoGame.OpenGL.GL.BindTexture(TextureTarget.Texture2D, texture);
				GLExt.CheckError();
			}
		}

		[MethodImpl(Runtime.MethodImpl.Inline)]
		internal void Dispose() {
			if (!PreviousTexture.HasValue) {
				return;
			}

			MonoGame.OpenGL.GL.BindTexture(TextureTarget.Texture2D, PreviousTexture.Value);
			GLExt.CheckError();
		}
	}

	private sealed class Texture2DGLMeta : IDisposable {
		private static readonly ConditionalWeakTable<Texture2D, Texture2DGLMeta> Map = new();

		[MethodImpl(Runtime.MethodImpl.Inline)]
		internal static bool TryGet(Texture2D texture, out Texture2DGLMeta meta) => Map.TryGetValue(texture, out meta!);
		[MethodImpl(Runtime.MethodImpl.Inline)]
		internal static Texture2DGLMeta Get(Texture2D texture) => Map.GetValue(texture, t => new(t));

		[Flags]
		internal enum Flag {
			None = 0,
			Initialized = 1 << 0
		}

		internal Flag Flags = Flag.Initialized;

		[MethodImpl(Runtime.MethodImpl.Inline)]
		private Texture2DGLMeta(Texture2D texture) {
			texture.Disposing += (_, _) => Dispose();
		}

		~Texture2DGLMeta() {

		}

		[MethodImpl(Runtime.MethodImpl.Inline)]
		public void Dispose() => Dispose(true);
		
		[MethodImpl(Runtime.MethodImpl.Inline)]
		private void Dispose(bool disposing) {
			if (disposing) {
				GC.SuppressFinalize(this);
			}
		}
	}


	internal static unsafe bool SetDataInternal<T>(
		Texture2D @this,
		int level,
		Bounds? rect,
		ReadOnlyPinnedSpan<T> data,
		bool initialized = true,
		bool isSet = false
 	) where T : unmanaged {
		var fixedData = data.Fixed;

		rect ??= (@this.Extent() >> level).Max(1);


		int layerSize = fixedData.IsEmpty ? @this.Format.SizeBytes(rect.Value.Area) : fixedData.Length * sizeof(T);

		bool success = true;
		Threading.BlockOnUIThread(
			() => {
				try {
					GLExt.CheckError();

					Texture2DGLMeta? glMeta = null;
					if (!initialized) {
						/*
						// If the texture lacks a glMeta, it was not constructed by us and is thus always initialized
						// by the underlying MonoGame constructor.
.						// The default Flags value for us is Initialized.
						*/
						glMeta = Texture2DGLMeta.Get(@this);
						initialized = glMeta.Flags.HasFlag(Texture2DGLMeta.Flag.Initialized);
					}

					using var reboundTexture = isSet ? default : new RebindTexture(@this.glTexture);

					if (!isSet) {
						MonoGame.OpenGL.GL.PixelStore(PixelStoreParameter.UnpackAlignment, Math.Min(@this.Format.GetSize(), 8));
						GLExt.CheckError();
					}

					if (@this.glFormat == PixelFormat.CompressedTextureFormats) {
						if (initialized) {
							MonoGame.OpenGL.GL.CompressedTexSubImage2D(
								TextureTarget.Texture2D,
								level,
								rect.Value.X,
								rect.Value.Y,
								rect.Value.Width,
								rect.Value.Height,
								@this.glInternalFormat,
								layerSize,
								(IntPtr)fixedData.Pointer
							);
							GLExt.CheckError();

						}
						else {
							MonoGame.OpenGL.GL.CompressedTexImage2D(
								TextureTarget.Texture2D,
								level,
								@this.glInternalFormat,
								rect.Value.Width,
								rect.Value.Height,
								0,
								layerSize,
								(IntPtr)fixedData.Pointer
							);
							GLExt.CheckError();
						}
					}
					else {
						if (initialized) {
							MonoGame.OpenGL.GL.TexSubImage2D(
								TextureTarget.Texture2D,
								level,
								rect.Value.X,
								rect.Value.Y,
								rect.Value.Width,
								rect.Value.Height,
								@this.glFormat,
								@this.glType,
								(IntPtr)fixedData.Pointer
							);
							GLExt.CheckError();

						}
						else {
							MonoGame.OpenGL.GL.TexImage2D(
								TextureTarget.Texture2D,
								level,
								@this.glInternalFormat,
								rect.Value.Width,
								rect.Value.Height,
								0,
								@this.glFormat,
								@this.glType,
								(IntPtr)fixedData.Pointer
							);
							GLExt.CheckError();

						}
					}

					GLExt.CheckError();

					if (@this is not InternalTexture2D) {
						PTexture2D.OnPlatformSetDataPost(
							__instance: @this,
							level: level,
							arraySlice: 0,
							rect: rect.Value,
							data: fixedData.Array,
							startIndex: 0,
							elementCount: fixedData.Length
						);
					}

					GLExt.CheckError();

					if (!initialized && glMeta is not null) {
						glMeta.Flags |= Texture2DGLMeta.Flag.Initialized;
					}
				}
				catch (MonoGameGLException ex) {
					Debug.Warning("GL Exception, disabling GL extensions", ex);
					Debug.Break();
					Working = false;
					success = false;
				}
				catch (SystemException ex) {
					Debug.Warning("System Exception, disabling GL extensions", ex);
					Debug.Break();
					Working = false;
					success = false;
				}
			}
		);

		return success;
	}

	internal static volatile bool Working = true;
	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static GLExt.SizedInternalFormat GetSizedFormat(PixelInternalFormat format) => format switch {
		PixelInternalFormat.Rgba => GLExt.SizedInternalFormat.RGBA8,
		PixelInternalFormat.Rgb => GLExt.SizedInternalFormat.RGB8,
		PixelInternalFormat.Luminance => GLExt.SizedInternalFormat.R8,
		PixelInternalFormat.Srgb => GLExt.SizedInternalFormat.SRGB8,
		_ => (GLExt.SizedInternalFormat)format
	};

	internal static bool Construct<T>(
	Texture2D @this,
	ReadOnlyPinnedSpan<T>.FixedSpan dataIn,
	Vector2I size,
	bool mipmap,
	SurfaceFormat format,
	SurfaceType type,
	bool shared
) where T : unmanaged {
		if (!Configuration.Config.Extras.OptimizeOpenGL) {
			return false;
		}

		if (!Working) {
			return false;
		}

		try {
			@this.glTarget = TextureTarget.Texture2D;
			format.GetGLFormat(@this.GraphicsDevice, out @this.glInternalFormat, out @this.glFormat, out @this.glType);

			// Use glTexStorage2D if it's available.
			// Presently, since we are not yet overriding 'SetData' to use glMeowTexSubImage2D,
			// only use it if we are populating the texture now
			bool useStorage = GLExt.TexStorage2D is not null;
			bool buildLayers = !dataIn.IsEmpty || !useStorage;

			// Calculate the number of texture levels
			int levels = 1;
			if (useStorage) {
				if (mipmap) {
					var tempDimensions = size;
					while (tempDimensions != (1, 1)) {
						tempDimensions >>= 1;
						tempDimensions = tempDimensions.Min(1);
						++levels;
					}
				}
			}

			// Mostly taken from MonoGame, but completely refactored.
			// Returns the size given dimensions, adjusted/aligned for block formats.
			Func<Vector2I, int> getLevelSize = format switch {
				// PVRTC has explicit calculations for imageSize
				// https://www.khronos.org/registry/OpenGL/extensions/IMG/IMG_texture_compression_pvrtc.txt
				SurfaceFormat.RgbPvrtc2Bpp or SurfaceFormat.RgbaPvrtc2Bpp =>
					static size => {
						var maxDimensions = size.Max((16, 8));
						return ((maxDimensions.X * maxDimensions.Y) << 1 + 7) >> 3;
					}
				,

				SurfaceFormat.RgbPvrtc4Bpp or SurfaceFormat.RgbaPvrtc4Bpp =>
					static size => {
						var maxDimensions = size.Max((8, 8));
						return ((maxDimensions.X * maxDimensions.Y) << 2 + 7) >> 3;
					}
				,

				_ when @this.glFormat == PixelFormat.CompressedTextureFormats =>
					size => {
						int blockSize = format.GetSize();
						var blockDimensions = format.BlockEdge();

						var blocks = (size + (blockDimensions - 1)) / blockDimensions;
						return blocks.X * blocks.Y * blockSize;
					}
				,

				_ =>
					size => format.SizeBytes(size.Area)
			};

			bool success = true;
			Threading.BlockOnUIThread(
				() => {
					GLExt.EnableDebugging();

					ReadOnlyPinnedSpan<byte> data = default;

					if (!dataIn.IsEmpty) {
						data = dataIn.AsSpan.AsBytes();
					}

					GLExt.CheckError();

					@this.GenerateGLTextureIfRequired();

					GLExt.CheckError();

					if (useStorage) {
						// https://www.khronos.org/registry/OpenGL-Refpages/gl4/html/glTexStorage2D.xhtml
						// Using glTexStorage and glMeowTexSubImage2D to populate textures is more efficient,
						// as the way that MonoGame normally does it requires the texture to be kept largely in flux,
						// and also requires it to be discarded a significant number of times.

						GLExt.CheckError();

						var sizedFormat = GetSizedFormat(@this.glInternalFormat);

						GLExt.TexStorage2D!(
							TextureTarget.Texture2D,
							levels,
							sizedFormat,
							size.Width,
							size.Height
						);
						GLExt.CheckError();

						Texture2DGLMeta.Get(@this).Flags |= Texture2DGLMeta.Flag.Initialized;
					}
					else {
						Texture2DGLMeta.Get(@this).Flags &= ~Texture2DGLMeta.Flag.Initialized;
					}

					if (buildLayers) {
						if (!data.IsEmpty) {
							MonoGame.OpenGL.GL.PixelStore(PixelStoreParameter.UnpackAlignment, Math.Min(@this.Format.GetSize(), 8));
							GLExt.CheckError();
						}

						var levelDimensions = size;
						int level = 0;
						int currentOffset = 0;

						// Loop over every level and populate it, starting from the largest.
						while (true) {
							int levelSize = getLevelSize(levelDimensions);
							if (!SetDataInternal(
								@this: @this,
								level: level++,
								rect: null,
								data: data.IsEmpty ? default : data.SliceUnsafe(currentOffset, levelSize),
								initialized: useStorage,
								isSet: true
							)) {
								success = false;
								return;
							}

							currentOffset += levelSize;

							if (levelDimensions == (1, 1) || !mipmap)
								break;

							levelDimensions >>= 1;
							levelDimensions = levelDimensions.Min(1);
							++level;
						}
					}
				}
			);
			return success;
		}
		catch (MonoGameGLException ex) {
			Debug.Warning("GL Exception, disabling GL extensions", ex);
			Debug.Break();
			Working = false;
			return false;
		}
		catch (SystemException ex) {
			Debug.Warning("System Exception, disabling GL extensions", ex);
			Debug.Break();
			Working = false;
			return false;
		}
	}

	internal static bool CopyTexture(
		XTexture2D source,
		Bounds sourceArea,
		XTexture2D target,
		Bounds targetArea,
		PatchMode patchMode
	) {
		if (!Configuration.Config.Extras.OptimizeOpenGL) {
			return false;
		}

		if (GLExt.CopyImageSubData is null) {
			return false;
		}

		if (patchMode != PatchMode.Replace) {
			return false;
		}

		GLExt.CheckError();
		GLExt.CopyImageSubData(
			(uint)source.glTexture,
			TextureTarget.Texture2D,
			0,
			sourceArea.X,
			sourceArea.Y,
			0,
			(uint)target.glTexture,
			TextureTarget.Texture2D,
			0,
			targetArea.X,
			targetArea.Y,
			0,
			(uint)sourceArea.Width,
			(uint)sourceArea.Height,
			1u
		);
		GLExt.CheckError();

		// We have to perform an internal SetData to make sure SM's caches are kept intact
		var cachedData = PTexture2D.GetCachedData<byte>(
			__instance: source,
			level: 0,
			arraySlice: 0,
			rect: sourceArea,
			data: default
		);

		PTexture2D.OnPlatformSetDataPostInternal<byte>(
			__instance: target,
			level: 0,
			arraySlice: 0,
			rect: targetArea,
			data: cachedData
		);

		return true;
	}
}
