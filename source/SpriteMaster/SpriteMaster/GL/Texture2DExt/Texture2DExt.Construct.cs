/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

//#define GL_TEXSTORAGE_ALWAYS_CHECK

using Microsoft.Xna.Framework.Graphics;
using MonoGame.OpenGL;
using SpriteMaster.Extensions;
using SpriteMaster.Types;
using SpriteMaster.Types.Spans;
using System;
using System.Runtime.CompilerServices;
using static Microsoft.Xna.Framework.Graphics.Texture2D;

namespace SpriteMaster.GL;

internal static partial class Texture2DExt {
	internal static volatile bool StorageEnabled = true;

	private static class Implementations {
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static void TextureStorage2D(Texture2D @this, int levels, GLExt.SizedInternalFormat format, Vector2I size) {
			GLExt.TextureStorage2D.Function!(
				(GLExt.ObjectId)@this.glTexture,
				levels,
				format,
				size.Width,
				size.Height
			);

			MonoGame.OpenGL.GL.BindTexture(TextureTarget.Texture2D, @this.glTexture);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static void TexStorage2D(Texture2D @this, int levels, GLExt.SizedInternalFormat format, Vector2I size) {
			GLExt.TexStorage2D.Function!(
				TextureTarget.Texture2D,
				levels,
				format,
				size.Width,
				size.Height
			);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static void TextureStorage2DExt(Texture2D @this, int levels, GLExt.SizedInternalFormat format, Vector2I size) {
			GLExt.TextureStorage2DExt.Function!(
				(GLExt.ObjectId)@this.glTexture,
				levels,
				format,
				size.Width,
				size.Height
			);

			MonoGame.OpenGL.GL.BindTexture(TextureTarget.Texture2D, @this.glTexture);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static void TexStorage2DExt(Texture2D @this, int levels, GLExt.SizedInternalFormat format, Vector2I size) {
			GLExt.TexStorage2DExt.Function!(
				TextureTarget.Texture2D,
				levels,
				format,
				size.Width,
				size.Height
			);
		}
	}

	private unsafe readonly struct DelegatePair {
		internal readonly GLExt.IToggledDelegate Delegator;
		internal readonly delegate* <Texture2D, int, GLExt.SizedInternalFormat, Vector2I, void> Function;

		internal DelegatePair(
			GLExt.IToggledDelegate delegator,
			delegate*<Texture2D, int, GLExt.SizedInternalFormat, Vector2I, void> function
		) {
			Delegator = delegator;
			Function = function;
		}
	};

	private static unsafe delegate* <Texture2D, int, GLExt.SizedInternalFormat, Vector2I, void> TexStorageCallback = null;
	private static bool TexStorageDisabled = false;
	private static unsafe bool TryTexStorage(Texture2D @this, int levels, GLExt.SizedInternalFormat format, Vector2I size) {
		if (TexStorageDisabled) {
			return false;
		}

		if (TexStorageCallback is not null) {
#if GL_TEXSTORAGE_ALWAYS_CHECK
			GLExt.AlwaysSwallowErrors();
			try {
#endif
				TexStorageCallback(@this, levels, format, size);
				return true;
#if GL_TEXSTORAGE_ALWAYS_CHECK
			}
			catch (Exception ex) {
				Debug.Warning(ex);
				return false;
			}
#endif
		}

		// First pass through is just scanning for support, effectively.
		GLExt.AlwaysSwallowErrors();

		DelegatePair[] delegates = {
			new(GLExt.TextureStorage2D,			&Implementations.TextureStorage2D),
			new(GLExt.TexStorage2D,					&Implementations.TexStorage2D),
			new(GLExt.TextureStorage2DExt,	&Implementations.TextureStorage2DExt),
			new(GLExt.TexStorage2DExt,			&Implementations.TexStorage2DExt),
		};

		foreach (var delegator in delegates) {
			if (!delegator.Delegator.Enabled) {
				continue;
			}

			try {
				GLExt.AlwaysChecked(
					(a0, a1, a2, a3) => delegator.Function(a0, a1, a2, a3),
					@this, levels, format, size
				);

				TexStorageCallback = delegator.Function;
				return true;
			}
			catch {
				delegator.Delegator.Disable();
			}
		}
		
		Debug.Warning("glTexStorage does not appear to be supported on this platform...");
		TexStorageDisabled = true;
		return false;
	}

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
			bool useStorage =
				StorageEnabled &&
				(GLExt.TexStorage2D.Enabled || GLExt.TextureStorage2D.Enabled || GLExt.TexStorage2DExt.Enabled || GLExt.TextureStorage2DExt.Enabled) &&
				Configuration.Config.Extras.UseTexStorage;
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
			ThreadingExt.ExecuteOnMainThread(
				() => {
					ReadOnlyPinnedSpan<byte> data = default;

					if (!dataIn.IsEmpty) {
						data = dataIn.AsSpan.AsBytes();
					}

					GLExt.CheckError();

					GenerateTexture(@this, useStorage);
					GLExt.Checked(new(MonoGame.OpenGL.GL.BindTexture), TextureTarget.Texture2D, @this.glTexture);

					if (useStorage) {
						// https://www.khronos.org/registry/OpenGL-Refpages/gl4/html/glTexStorage2D.xhtml
						// Using glTexStorage and glMeowTexSubImage2D to populate textures is more efficient,
						// as the way that MonoGame normally does it requires the texture to be kept largely in flux,
						// and also requires it to be discarded a significant number of times.

						var sizedFormat = GetSizedFormat(@this.glInternalFormat);

						bool usedTexStorage = TryTexStorage(@this, levels, sizedFormat, size);

						if (usedTexStorage) {
							@this.GetGlMeta().Flags |= Texture2DOpenGlMeta.Flag.Initialized | Texture2DOpenGlMeta.Flag.Storage;
						}
						else {
							useStorage = false;
							StorageEnabled = false;
							buildLayers = !dataIn.IsEmpty;

							GLExt.Checked(() => MonoGame.OpenGL.GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureBaseLevel, 0));

							if (@this.GraphicsDevice.GraphicsCapabilities.SupportsTextureMaxLevel) {
								GLExt.Checked(() => MonoGame.OpenGL.GL.TexParameter(TextureTarget.Texture2D, SamplerState.TextureParameterNameTextureMaxLevel, (@this.LevelCount > 0) ? @this.LevelCount - 1 : 1000));
							}
						}
					}

					if (!useStorage) {
						@this.GetGlMeta().Flags &= ~Texture2DOpenGlMeta.Flag.Initialized;
					}

					if (buildLayers) {
						if (!data.IsEmpty) {
							GLExt.Checked(() => MonoGame.OpenGL.GL.PixelStore(PixelStoreParameter.UnpackAlignment, Math.Min(@this.Format.GetSize(), 8)));
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

							if (!mipmap || levelDimensions == (1, 1))
								break;

							currentOffset += levelSize;

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
}
