/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

using Microsoft.Toolkit.HighPerformance;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.OpenGL;
using SpriteMaster.Extensions;
using SpriteMaster.Types;
using SpriteMaster.Types.Spans;
using System;
using System.Runtime.CompilerServices;

namespace SpriteMaster.GL;

using OGL = MonoGame.OpenGL.GL;

internal static partial class Texture2DExt {
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static unsafe bool GetDataInternal<T>(
		Texture2D @this,
		int level,
		Bounds? rect,
		Span<T> data
	) where T : unmanaged {
		fixed (T* dataPtr = data) {
			return GetDataInternal(
				@this,
				level,
				rect,
				new PointerSpan<T>(dataPtr, data.Length)
			);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static bool GetDataInternal<T>(
		Texture2D @this,
		int level,
		Bounds? rect,
		PinnedSpan<T> data,
		bool initialized = false,
		bool isSet = false
	) where T : unmanaged {
		return GetDataInternal(
			@this,
			level,
			rect,
			(PointerSpan<T>)data
		);
	}

	private static void CopyBlocked<TDest>(ReadOnlySpan<byte> source, Span<TDest> destination, SurfaceFormat format, Vector2I block, Vector2I sourceSize, Bounds destBounds) where TDest : unmanaged {
		// If the rect is width-aligned, the copy is much simpler
		if (sourceSize.Width == destBounds.Width) {
			int sourceOffset = format.SizeBytes((destBounds.Width, destBounds.Y));
			int length = format.SizeBytes(destBounds.Size);

			var offsetSource = source.Slice(sourceOffset, length);
			offsetSource.CopyTo(destination.AsBytes());
		}
		else {
			int sourceBlockRowOffsetLength = format.SizeBytes((sourceSize.Width, block.Y));
			int destBlockRowOffsetLength = format.SizeBytes((destBounds.Width, block.Y));
			int startReadOffset = (sourceBlockRowOffsetLength * (destBounds.Y / block.Y)) + format.SizeBytes((destBounds.Left, block.Y));
			source = source[startReadOffset..];

			int sourceOffset = 0;
			int destOffset = 0;
			for (int y = 0; y < destBounds.Height; y += block.Y) {
				var innerSource = source.Slice(sourceOffset, destBlockRowOffsetLength);
				var dest = destination.AsBytes().Slice(destOffset, destBlockRowOffsetLength);

				innerSource.CopyTo(dest);

				sourceOffset += sourceBlockRowOffsetLength;
				destOffset += destBlockRowOffsetLength;
			}
		}
	}

	private static void CopyRaw<TDest>(ReadOnlySpan<byte> source, Span<TDest> destination, SurfaceFormat format, Vector2I sourceSize, Bounds destBounds) where TDest : unmanaged {
		var destBytes = destination.AsBytes();

		// If the rect is width-aligned, the copy is much simpler
		if (sourceSize.Width == destBounds.Width) {
			int sourceOffset = format.SizeBytes((sourceSize.Width, destBounds.Offset.Y));
			int length = format.SizeBytes(destBounds.Size);

			source = source.Slice(sourceOffset, length);
			source.CopyTo(destBytes);
		}
		else {
			int sourceRowOffsetLength = format.SizeBytes(sourceSize.Width);
			int destRowOffsetLength = format.SizeBytes(destBounds.Width);

			int startReadOffset = (sourceRowOffsetLength * destBounds.Offset.Y) + format.SizeBytes(destBounds.Left);
			source = source[startReadOffset..];

			int sourceOffset = 0;
			int destOffset = 0;
			for (int y = 0; y < destBounds.Height; ++y) {
				var innerSource = source.Slice(sourceOffset, destRowOffsetLength);
				var dest = destBytes.Slice(destOffset, destRowOffsetLength);

				innerSource.CopyTo(dest);

				sourceOffset += sourceRowOffsetLength;
				destOffset += destRowOffsetLength;
			}
		}
	}
	 
	internal static unsafe bool GetDataInternal<T>(
		Texture2D @this,
		int level,
		Bounds? rect,
		PointerSpan<T> data
	) where T : unmanaged {
		ThreadingExt.EnsureMainThread();

		if (!SMConfig.Extras.OpenGL.OptimizeTexture2DGetData) {
			return false;
		}

		Bounds fullRect = (@this.Extent() >> level).Max(1);
		bool entireTexture;
		if (!rect.HasValue) {
			rect = fullRect;
			entireTexture = true;
		}
		else {
			entireTexture = rect.Value == fullRect;
		}

		int layerSize = data.IsEmpty ? @this.Format.SizeBytes(rect.Value.Size) : data.Length * sizeof(T);

		if (data.Length * sizeof(T) < layerSize) {
			ThrowHelper.ThrowArgumentException("The data array is too small.");
		}

		bool success = true;

		void ReadCompressed() {
			if (entireTexture && GLExt.GetCompressedTexImage.Enabled) {
				GLExt.Checked(() => GLExt.GetCompressedTexImage.Function(
					TextureTarget.Texture2D,
					level,
					(nint)data.Pointer
				));
				return;
			}

			if (GLExt.GetCompressedTextureSubImage.Enabled) {
				GLExt.Checked(() => GLExt.GetCompressedTextureSubImage.Function(
					(GLExt.ObjectId)@this.glTexture,
					level,
					rect.Value.X,
					rect.Value.Y,
					0,
					(uint)rect.Value.Width,
					(uint)rect.Value.Height,
					1,
					(uint)(data.Length * sizeof(T)),
					(nint)data.Pointer
				));
				GLExt.BindTextureChecked(TextureTarget.Texture2D, @this.glTexture);
			}
			else {
				int fullLayerSize = @this.Format.SizeBytes(fullRect.Size);
				Span<byte> tempBuffer = fullLayerSize <= 0x2000
					? stackalloc byte[fullLayerSize]
					: GC.AllocateUninitializedArray<byte>(fullLayerSize);

				fixed (byte* buffer = tempBuffer) {
					byte* localBuffer = buffer;
					GLExt.Checked(
						() => GLExt.GetCompressedTexImage.Function!(
							TextureTarget.Texture2D,
							level,
							(nint)localBuffer
						)
					);
				}

				CopyBlocked(tempBuffer, data.AsSpan, @this.Format, @this.Format.BlockEdge(), fullRect.Size, rect.Value);
			}
		}

		void ReadUncompressed() {
			if (entireTexture && GLExt.GetTexImage.Enabled) {
				GLExt.Checked(() => GLExt.GetTexImage.Function(
					TextureTarget.Texture2D,
					level,
					@this.glFormat,
					@this.glType,
					(nint)data.Pointer
				));
				return;
			}

			if (GLExt.GetTextureSubImage.Enabled) {
				GLExt.Checked(() => GLExt.GetTextureSubImage.Function(
					(GLExt.ObjectId)@this.glTexture,
					level,
					rect.Value.X,
					rect.Value.Y,
					0,
					(uint)rect.Value.Width,
					(uint)rect.Value.Height,
					1,
					@this.glFormat,
					@this.glType,
					(uint)(data.Length * sizeof(T)),
					(nint)data.Pointer
				));
				GLExt.BindTextureChecked(TextureTarget.Texture2D, @this.glTexture);
			}
			else {
				int fullLayerSize = @this.Format.SizeBytes(fullRect.Size);
				Span<byte> tempBuffer = fullLayerSize <= 0x2000
					? stackalloc byte[fullLayerSize]
					: GC.AllocateUninitializedArray<byte>(fullLayerSize);

				fixed (byte* buffer = tempBuffer) {
					byte* localBuffer = buffer;
					GLExt.Checked(
						() => GLExt.GetTexImage.Function!(
							TextureTarget.Texture2D,
							level,
							@this.glFormat,
							@this.glType,
							(nint)localBuffer
						)
					);
				}

				CopyRaw(tempBuffer, data.AsSpan, @this.Format, fullRect.Size, rect.Value);
			}
		}

		bool usedStorage = @this.GetGlMeta().Flags.HasFlag(Texture2DOpenGlMeta.Flag.Storage);

		try {
			// Flush errors
			GLExt.SwallowOrReportErrors();

			GLExt.BindTextureChecked(TextureTarget.Texture2D, @this.glTexture);
			GLExt.PixelStoreChecked(PixelStoreName.PackAlignment, Math.Min(sizeof(T), 8));

			if (@this.glFormat == PixelFormat.CompressedTextureFormats) {
				ReadCompressed();
			}
			else {
				ReadUncompressed();
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

		return success;
	}
}
