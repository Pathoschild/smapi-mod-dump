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
using SpriteMaster.GL;

namespace SpriteMaster.Types;

/// <summary>
/// A Texture2D that represents internal SpriteMaster data, and thus shouldn't continue down any resampling pipelines
/// </summary>
internal class InternalTexture2D : XTexture2D {
	private const string DefaultName = "Texture (Internal)";

	private Texture2DExt.Texture2DOpenGlMeta? InnerGlMeta;

	internal Texture2DExt.Texture2DOpenGlMeta GlMeta {
		get {
			if (InnerGlMeta is not {} meta) {
				InnerGlMeta = meta = Texture2DExt.Texture2DOpenGlMeta.Get(this);
			}

			return meta;
		}
	}

	internal InternalTexture2D(GraphicsDevice graphicsDevice, int width, int height) : base(graphicsDevice, width, height) {
		Name = DefaultName;
	}

	internal InternalTexture2D(GraphicsDevice graphicsDevice, int width, int height, bool mipmap, SurfaceFormat format) : base(graphicsDevice, width, height, mipmap, format) {
		Name = DefaultName;
	}

	internal InternalTexture2D(GraphicsDevice graphicsDevice, int width, int height, bool mipmap, SurfaceFormat format, int arraySize) : base(graphicsDevice, width, height, mipmap, format, arraySize) {
		Name = DefaultName;
	}

	protected InternalTexture2D(GraphicsDevice graphicsDevice, int width, int height, bool mipmap, SurfaceFormat format, SurfaceType type, bool shared, int arraySize) : base(graphicsDevice, width, height, mipmap, format, type, shared, arraySize) {
		Name = DefaultName;
	}
}
