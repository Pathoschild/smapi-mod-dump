/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/KhloeLeclair/StardewMods
**
*************************************************/

using System.Collections.Generic;
using System.Linq;

using Microsoft.Xna.Framework;

namespace Leclair.Stardew.ThemeManager.Models;

public class XNBHackFontData {

	public int LineSpacing { get; set; }

	public float Spacing { get; set; }

	public char? DefaultCharacter { get; set; }

	public Dictionary<char, XNBHackGlyphData>? Glyphs { get; set; }

	public RawSpriteFontData ToRaw() {
		List<RawGlyphData>? glyphs = null;
		if (Glyphs != null) {
			glyphs = new List<RawGlyphData>();
			foreach (XNBHackGlyphData glyph in Glyphs.Values)
				glyphs.Add(new RawGlyphData {
					Character = glyph.Character,
					BoundsInTexture = glyph.BoundsInTexture,
					Cropping = glyph.Cropping,
					Kerning = new Vector3(glyph.LeftSideBearing, glyph.Width, glyph.RightSideBearing)
				});
		}

		return new RawSpriteFontData {
			LineSpacing = LineSpacing,
			Spacing = Spacing,
			DefaultCharacter = DefaultCharacter,
			Glyphs = glyphs?.ToArray()
		};
	}

}

public class XNBHackGlyphData {

	public char? Character { get; set; }

	public Rectangle? BoundsInTexture { get; set; }

	public Rectangle? Cropping { get; set; }

	public float LeftSideBearing { get; set; }

	public float RightSideBearing { get; set; }

	public float Width { get; set; }

}
