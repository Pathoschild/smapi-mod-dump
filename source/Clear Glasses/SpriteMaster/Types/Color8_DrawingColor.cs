/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aurpine/Stardew-SpriteMaster
**
*************************************************/

using System;

namespace SpriteMaster.Types;

internal readonly partial struct Color8 : IEquatable<DrawingColor> {
	// ToArgb

	public static implicit operator DrawingColor(Color8 value) => DrawingColor.FromArgb((int)value.ARGB);
	public static implicit operator Color8(DrawingColor value) => new(r: value.R, g: value.G, b: value.B, a: value.A);

	public readonly bool Equals(DrawingColor other) => Equals((Color8)other);
}
