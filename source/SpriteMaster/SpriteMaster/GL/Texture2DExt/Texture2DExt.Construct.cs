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

			GLExt.BindTexture(TextureTarget.Texture2D, @this.glTexture);
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
				TextureTarget.Texture2D,
				levels,
				format,
				size.Width,
				size.Height
			);

			GLExt.BindTexture(TextureTarget.Texture2D, @this.glTexture);
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
			new(GLExt.TextureStorage2DExt,  &Implementations.TextureStorage2DExt),
			new(GLExt.TextureStorage2D,			&Implementations.TextureStorage2D),
			new(GLExt.TexStorage2D,					&Implementations.TexStorage2D)
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

	private delegate int LevelSizeGetter(Vector2I size);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static LevelSizeGetter GetLevelSizeGetter(
		Texture2D texture,
		SurfaceFormat format
	) => format switch {
		// PVRTC has explicit calculations for imageSize
		// https://www.khronos.org/registry/OpenGL/extensions/IMG/IMG_texture_compression_pvrtc.txt
		SurfaceFormat.RgbPvrtc2Bpp or SurfaceFormat.RgbaPvrtc2Bpp =>
			static dimensions => {
				var maxDimensions = dimensions.Max((16, 8));
				return ((maxDimensions.X * maxDimensions.Y) << 1 + 7) >> 3;
			}
		,

		SurfaceFormat.RgbPvrtc4Bpp or SurfaceFormat.RgbaPvrtc4Bpp =>
			static dimensions => {
				var maxDimensions = dimensions.Max((8, 8));
				return ((maxDimensions.X * maxDimensions.Y) << 2 + 7) >> 3;
			}
		,

		_ when texture.glFormat == PixelFormat.CompressedTextureFormats =>
			dimensions => {
				int blockSize = format.GetSize();
				var blockDimensions = format.BlockEdge();

				var blocks = (dimensions + (blockDimensions - 1)) / blockDimensions;
				return blocks.X * blocks.Y * blockSize;
			}
		,

		_ => dimensions => format.SizeBytes(dimensions.Area)
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
		if (!Configuration.Config.Extras.OpenGL.Enabled) {
			return false;
		}

		if (!Working) {
			return false;
		}

		try {
			var meta = @this.GetGlMeta();

			@this.glTarget = TextureTarget.Texture2D;
			format.GetGLFormat(@this.GraphicsDevice, out @this.glInternalFormat, out @this.glFormat, out @this.glType);

			// Use glTexStorage2D if it's available.
			// Presently, since we are not yet overriding 'SetData' to use glMeowTexSubImage2D,
			// only use it if we are populating the texture now
			bool useStorage =
				StorageEnabled &&
				(GLExt.TexStorage2D.Enabled || GLExt.TextureStorage2D.Enabled || GLExt.TextureStorage2DExt.Enabled) &&
				SMConfig.Extras.OpenGL.UseTexStorage;
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
			var getLevelSize = GetLevelSizeGetter(@this, format);

			bool success = true;
			ThreadingExt.ExecuteOnMainThread(
				() => {
					ReadOnlyPinnedSpan<byte> data = default;

					if (!dataIn.IsEmpty) {
						data = dataIn.AsSpan.AsBytes();
					}

					GLExt.CheckError();

					GenerateTexture(@this, useStorage);
					@this.CheckTextureMip();
					GLExt.BindTextureChecked(TextureTarget.Texture2D, @this.glTexture);

					if (useStorage) {
						// https://www.khronos.org/registry/OpenGL-Refpages/gl4/html/glTexStorage2D.xhtml
						// Using glTexStorage and glMeowTexSubImage2D to populate textures is more efficient,
						// as the way that MonoGame normally does it requires the texture to be kept largely in flux,
						// and also requires it to be discarded a significant number of times.

						var sizedFormat = GetSizedFormat(@this.glInternalFormat);

						bool usedTexStorage = TryTexStorage(@this, levels, sizedFormat, size);

						if (usedTexStorage) {
							meta.Flags |= Texture2DOpenGlMeta.Flag.Initialized | Texture2DOpenGlMeta.Flag.Storage;
						}
						else {
							useStorage = false;
							StorageEnabled = false;
							buildLayers = !dataIn.IsEmpty;
						}
					}

					if (!useStorage) {
						meta.Flags &= ~Texture2DOpenGlMeta.Flag.Initialized;
					}

					if (buildLayers) {
						success = InitializeTexture(
							@this,
							data,
							meta,
							getLevelSize
						) && success;
					}
				}
			);

			if (!success) {
				meta.Flags &= ~Texture2DOpenGlMeta.Flag.Managed;
			}
			else {
				meta.Flags |= Texture2DOpenGlMeta.Flag.Managed;
				@this.CheckTextureMip();
			}

			return success;
		}
		catch (MonoGameGLException ex) {
			Debug.Warning("GL Exception, disabling GL extensions", ex);
			Debug.Break();
			Working = false;
			@this.GetGlMeta().Flags &= ~Texture2DOpenGlMeta.Flag.Managed;
			return false;
		}
		catch (SystemException ex) {
			Debug.Warning("System Exception, disabling GL extensions", ex);
			Debug.Break();
			Working = false;
			@this.GetGlMeta().Flags &= ~Texture2DOpenGlMeta.Flag.Managed;
			return false;
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static bool InitializeTexture(Texture2D texture, ReadOnlySpan<byte> data = default) =>
		InitializeTexture(
			texture: texture,
			data: data,
			meta: texture.GetGlMeta(),
			GetLevelSizeGetter(texture, texture.Format)
		);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static bool InitializeTexture(
		Texture2D texture,
		ReadOnlySpan<byte> data,
		Texture2DOpenGlMeta? meta,
		LevelSizeGetter levelSizeGetter
	) {
		texture.CheckTextureMip();

		if (!data.IsEmpty) {
			GLExt.PixelStoreChecked(PixelStoreName.UnpackAlignment, Math.Min(texture.Format.GetSize(), 8));
		}

		meta ??= texture.GetGlMeta();

		bool mipmap = texture.LevelCount != 1;
		var levelDimensions = new Vector2I(texture);
		int level = 0;
		int currentOffset = 0;

		bool hasData = !data.IsEmpty;

		// Loop over every level and populate it, starting from the largest.
		while (true) {
			int levelSize = hasData ? levelSizeGetter(levelDimensions) : 0;
			if (!SetDataInternal(
						@this: texture,
						level: level++,
						rect: null,
						data: hasData ? data.Slice(currentOffset, levelSize) : default,
						initialized: meta.Flags.HasFlag(Texture2DOpenGlMeta.Flag.Storage | Texture2DOpenGlMeta.Flag.Initialized),
						isSet: true
					)) {
				return false;
			}

			if (!mipmap || levelDimensions == (1, 1))
				break;

			currentOffset += levelSize;

			levelDimensions >>= 1;
			levelDimensions = levelDimensions.Min(1);
			++level;
		}

		meta.Flags |= Texture2DOpenGlMeta.Flag.Initialized;

		texture.CheckTextureMip();

		return true;
	}
}
