/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

namespace SpriteMaster.Types;

internal enum TextureType {
	Sprite = 0, // sprite in a spritesheet
	Image = 1, // the entire texture is an image, drawn at once
	SlicedImage = 2, // the entire texture is an image, drawn in slices
}
