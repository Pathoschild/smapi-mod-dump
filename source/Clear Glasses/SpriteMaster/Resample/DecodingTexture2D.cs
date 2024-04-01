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

namespace SpriteMaster.Types;

/// <summary>
/// A Texture2D that represents a texture being used to decode block-compressed data
/// </summary>
internal sealed class DecodingTexture2D : InternalTexture2D {
	private const string DefaultName = "Decoding Texture (Internal)";

	internal DecodingTexture2D(GraphicsDevice graphicsDevice, int width, int height) : base(graphicsDevice, width, height) {
		Name = DefaultName;
	}

	internal DecodingTexture2D(GraphicsDevice graphicsDevice, int width, int height, bool mipmap, SurfaceFormat format) : base(graphicsDevice, width, height, mipmap, format) {
		Name = DefaultName;
	}

	internal DecodingTexture2D(GraphicsDevice graphicsDevice, int width, int height, bool mipmap, SurfaceFormat format, int arraySize) : base(graphicsDevice, width, height, mipmap, format, arraySize) {
		Name = DefaultName;
	}
}
