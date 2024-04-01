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

namespace SpriteMaster.Configuration.Preview;

internal class BasicTexture : MetaTexture {
	internal Vector2I Size => new(Texture.Width, Texture.Height);
	internal Vector2I RenderedSize => Size * 4;

	internal BasicTexture(
		string textureName
	) : base(textureName) {
	}
}
