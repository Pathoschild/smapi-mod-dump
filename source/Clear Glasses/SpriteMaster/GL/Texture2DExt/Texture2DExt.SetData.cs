/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aurpine/Stardew-SpriteMaster
**
*************************************************/

using Microsoft.Xna.Framework.Graphics;
using MonoGame.OpenGL;
using SpriteMaster.Extensions;
using SpriteMaster.Harmonize.Patches;
using SpriteMaster.Types;
using SpriteMaster.Types.Spans;
using System;
using System.Runtime.CompilerServices;

namespace SpriteMaster.GL;

internal static partial class Texture2DExt {
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static unsafe bool SetDataInternal<T>(
		Texture2D @this,
		int level,
		Bounds? rect,
		ReadOnlySpan<T> data,
		bool initialized = false,
		bool isSet = false
	) where T : unmanaged {
		fixed (T* dataPtr = data) {
			return SetDataInternal(
				@this,
				level,
				rect,
				new PointerSpan<T>(dataPtr, data.Length),
				initialized,
				isSet
			);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static bool SetDataInternal<T>(
		Texture2D @this,
		int level,
		Bounds? rect,
		ReadOnlyPinnedSpan<T> data,
		bool initialized = false,
		bool isSet = false
	) where T : unmanaged {
		return SetDataInternal(
			@this,
			level,
			rect,
			(PointerSpan<T>)data,
			initialized,
			isSet
		);
	}

	internal static unsafe bool SetDataInternal<T>(
		Texture2D @this,
		int level,
		Bounds? rect,
		PointerSpan<T> data,
		bool initialized = false,
		bool isSet = false
	) where T : unmanaged {
		if (!SMConfig.Extras.OpenGL.OptimizeTexture2DSetData) {
			return false;
		}

		rect ??= (@this.Extent() >> level).Max(1);

		int layerSize = data.IsEmpty ? @this.Format.SizeBytes(rect.Value.Size) : data.Length * sizeof(T);

		bool success = true;
		ThreadingExt.ExecuteOnMainThread(
			() => {
				try {
					// Flush errors
					GLExt.SwallowOrReportErrors();


					Texture2DOpenGlMeta? glMeta = null;
					if (!initialized) {
						/*
						// If the texture lacks a glMeta, it was not constructed by us and is thus always initialized
						// by the underlying MonoGame constructor.
.						// The default Flags value for us is Initialized.
						*/
						glMeta = @this.GetGlMeta();
						initialized = glMeta.Flags.HasFlag(Texture2DOpenGlMeta.Flag.Initialized);
					}

					using var reboundTexture = new TextureBinder(() => {
						if (@this.glTexture <= 0) {
							GenerateTexture(@this, false);
							GLExt.PixelStoreChecked(PixelStoreName.UnpackAlignment, Math.Min(@this.Format.GetSize(), 8));
							isSet = true;
							initialized = false;
							return (GLExt.ObjectId)@this.glTexture;
						}
						else {
							return (GLExt.ObjectId)@this.glTexture;
						}
					});

					if (!isSet) {
						GLExt.PixelStoreChecked(PixelStoreName.UnpackAlignment, Math.Min(@this.Format.GetSize(), 8));
					}

					bool isCompressed = @this.glFormat == PixelFormat.CompressedTextureFormats;
					switch (isCompressed, initialized) {
						case (true, true):
							GLExt.Checked(() => MonoGame.OpenGL.GL.CompressedTexSubImage2D(
								TextureTarget.Texture2D,
								level,
								rect.Value.X,
								rect.Value.Y,
								rect.Value.Width,
								rect.Value.Height,
								@this.glInternalFormat,
								layerSize,
								data
							));
							break;
						case (true, false):
							GLExt.Checked(() => MonoGame.OpenGL.GL.CompressedTexImage2D(
								TextureTarget.Texture2D,
								level,
								@this.glInternalFormat,
								rect.Value.Width,
								rect.Value.Height,
								0,
								layerSize,
								data
							));
							break;
						case (false, true):
							GLExt.Checked(() => MonoGame.OpenGL.GL.TexSubImage2D(
								TextureTarget.Texture2D,
								level,
								rect.Value.X,
								rect.Value.Y,
								rect.Value.Width,
								rect.Value.Height,
								@this.glFormat,
								@this.glType,
								data
							));
							break;
						case (false, false):
							GLExt.Checked(() => MonoGame.OpenGL.GL.TexImage2D(
								TextureTarget.Texture2D,
								level,
								@this.glInternalFormat,
								rect.Value.Width,
								rect.Value.Height,
								0,
								@this.glFormat,
								@this.glType,
								data
							));
							break;
					}

					if (!initialized && glMeta is not null) {
						glMeta.Flags |= Texture2DOpenGlMeta.Flag.Initialized;
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

		if (success) {
			if (@this is not InternalTexture2D) {
				PTexture2D.OnPlatformSetDataPostInternal<T>(
					__instance: @this,
					level: level,
					arraySlice: 0,
					rect: rect.Value,
					data: data.AsReadOnlySpan,
					startIndex: 0,
					elementCount: data.Length
				);
			}
		}

		return success;
	}
}
