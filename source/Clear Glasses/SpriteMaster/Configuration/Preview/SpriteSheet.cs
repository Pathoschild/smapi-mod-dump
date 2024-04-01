/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aurpine/Stardew-SpriteMaster
**
*************************************************/

using SpriteMaster.Types;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace SpriteMaster.Configuration.Preview;

internal class SpriteSheet : MetaTexture {
	internal readonly Vector2I Size;
	internal Vector2I RenderedSize => Size * 4;
	internal readonly Vector2I Dimensions;

	[DoesNotReturn]
	[MethodImpl(MethodImplOptions.NoInlining)]
	private static T ThrowIndexOutOfRangeException<T>(string name, int value, int constraint) =>
		throw new IndexOutOfRangeException($"Argument '{name}' ({value}) is out of range (< 0 or >= {constraint})");

	internal Drawable this[int x, int y] {
		get {
			if (x < 0 || x >= Dimensions.X) {
				return ThrowIndexOutOfRangeException<Drawable>(nameof(x), x, Dimensions.X);
			}
			if (y < 0 || y >= Dimensions.Y) {
				return ThrowIndexOutOfRangeException<Drawable>(nameof(y), y, Dimensions.Y);
			}

			Bounds bounds = new(Size * (x, y), Size);

			return new(Texture, bounds);
		}
	}

	internal SpriteSheet(
		string textureName,
		Vector2I spriteSize,
		Vector2I? dimensions = null
	) : base(textureName) {
		Size = spriteSize;
		Dimensions = dimensions ?? (new Vector2I(Texture.Width, Texture.Height) / spriteSize);
	}
}
