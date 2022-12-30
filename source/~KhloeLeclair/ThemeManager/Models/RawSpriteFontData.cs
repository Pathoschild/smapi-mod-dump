/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/KhloeLeclair/StardewMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;

namespace Leclair.Stardew.ThemeManager.Models;

public class RawSpriteFontData {

	public string? TextureName { get; set; }

	public int LineSpacing { get; set; }

	public float Spacing { get; set; }

	public char? DefaultCharacter { get; set; }

	public RawGlyphData[]? Glyphs { get; set; }

}

public class RawGlyphData {

	public char? Character { get; set; }

	public Rectangle? BoundsInTexture { get; set; }

	public Rectangle? Cropping { get; set; }

	public Vector3? Kerning { get; set; }

}
