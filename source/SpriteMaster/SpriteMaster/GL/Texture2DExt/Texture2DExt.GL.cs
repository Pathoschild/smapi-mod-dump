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
using MonoGame.OpenGL;
using System.Runtime.CompilerServices;

namespace SpriteMaster.GL;

internal static partial class Texture2DExt {
	internal static volatile bool Working = true;
	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static GLExt.SizedInternalFormat GetSizedFormat(PixelInternalFormat format) => format switch {
		PixelInternalFormat.Rgba => GLExt.SizedInternalFormat.RGBA8,
		PixelInternalFormat.Rgb => GLExt.SizedInternalFormat.RGB8,
		PixelInternalFormat.Luminance => GLExt.SizedInternalFormat.R8,
		PixelInternalFormat.Srgb => GLExt.SizedInternalFormat.SRGB8,
		_ => (GLExt.SizedInternalFormat)format
	};

	[MethodImpl(Runtime.MethodImpl.Inline)]
	private static bool IsPow2(this int value) {
		return value != 0 && (value & (value - 1)) == 0;
	}

	[MethodImpl(Runtime.MethodImpl.Inline)]
	internal static int GenerateTexture(Texture2D @this, bool usingStorage) {
		int texture = @this.glTexture;
		if (texture > 0) {
			return texture;
		}

		GLExt.Checked(() => MonoGame.OpenGL.GL.GenTextures(1, out texture));

		@this.glTexture = texture;

		GLExt.Checked(() => MonoGame.OpenGL.GL.BindTexture(TextureTarget.Texture2D, texture));

		GLExt.Checked(
			() => MonoGame.OpenGL.GL.TexParameter(
				TextureTarget.Texture2D,
				TextureParameterName.TextureMinFilter,
				(int)((@this.LevelCount > 1) ? TextureMinFilter.LinearMipmapLinear : TextureMinFilter.Linear)
			)
		);
		GLExt.Checked(
			() => MonoGame.OpenGL.GL.TexParameter(
				TextureTarget.Texture2D,
				TextureParameterName.TextureMagFilter,
				(int)TextureMagFilter.Linear
			)
		);

		// XNA compat
		var wrap = (!@this.Width.IsPow2() || !@this.Height.IsPow2())
			? TextureWrapMode.ClampToEdge
			: TextureWrapMode.Repeat;

		GLExt.Checked(() => MonoGame.OpenGL.GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)wrap));
		GLExt.Checked(() => MonoGame.OpenGL.GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)wrap));

		if (!usingStorage) {
			GLExt.Checked(() => MonoGame.OpenGL.GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureBaseLevel, 0));

			if (@this.GraphicsDevice.GraphicsCapabilities.SupportsTextureMaxLevel) {
				GLExt.Checked(() => MonoGame.OpenGL.GL.TexParameter(TextureTarget.Texture2D, SamplerState.TextureParameterNameTextureMaxLevel, (@this.LevelCount > 0) ? @this.LevelCount - 1 : 1000));
			}
		}

		return texture;
	}
}
