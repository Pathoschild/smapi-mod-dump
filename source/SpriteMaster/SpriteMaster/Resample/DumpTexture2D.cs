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
using SpriteMaster.Harmonize.Patches;
using SpriteMaster.Types.Spans;

namespace SpriteMaster.Types;

/// <summary>
/// A Texture2D that represents a texture being used to dump a PNG
/// </summary>
internal sealed class DumpTexture2D : InternalTexture2D {
	private const string DefaultName = "Dump Texture (Internal)";

	internal DumpTexture2D(GraphicsDevice graphicsDevice, int width, int height) : base(graphicsDevice, width, height) {
		Name = DefaultName;
	}

	internal DumpTexture2D(GraphicsDevice graphicsDevice, int width, int height, bool mipmap, SurfaceFormat format) : base(graphicsDevice, width, height, mipmap, format) {
		Name = DefaultName;
	}

	internal DumpTexture2D(GraphicsDevice graphicsDevice, int width, int height, bool mipmap, SurfaceFormat format, int arraySize) : base(graphicsDevice, width, height, mipmap, format, arraySize) {
		Name = DefaultName;
	}

	internal DumpTexture2D(ReadOnlyPinnedSpan<byte>.FixedSpan data, GraphicsDevice graphicsDevice, int width, int height, bool mipmap, SurfaceFormat format) :
		base(
			graphicsDevice,
			width,
			height,
			mipmap,
			format,
			 PTexture2D.PlatformConstruct is null ? SurfaceType.Texture : SurfaceType.SwapChainRenderTarget, // this prevents the texture from being constructed immediately
			false,
			1
		) {
		Name = DefaultName;

		if (PTexture2D.PlatformConstruct is not null && !GL.Texture2DExt.Construct(this, data, (width, height), mipmap, format, SurfaceType.Texture, false)) {
			PTexture2D.PlatformConstruct(this, width, height, mipmap, format, SurfaceType.Texture, false);
			SetData(data.Array);
		}
	}
}
