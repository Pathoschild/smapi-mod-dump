/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/KhloeLeclair/StardewMods
**
*************************************************/

#nullable enable

using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Leclair.Stardew.Common.UI;

public struct TextStyle {

	public readonly static TextStyle EMPTY = new();
	public readonly static TextStyle BOLD = new(bold: true);
	public readonly static TextStyle FANCY = new(fancy: true);

	public bool? Fancy { get; }
	public bool? Junimo { get; }
	public bool? Shadow { get; }
	public Color? ShadowColor { get; }
	public bool? Bold { get; }
	public bool? Invert { get; }
	public Color? Color { get; }
	public Color? BackgroundColor { get; }
	public bool? Prismatic { get; }
	public SpriteFont? Font { get; }
	public bool? Strikethrough { get; }
	public bool? Underline { get; }
	public float? Scale { get; }

	public TextStyle(Color? color = null, Color? backgroundColor = null, bool? prismatic = null, SpriteFont? font = null, bool? fancy = null, bool? junimo = null, bool? shadow = null, Color? shadowColor = null, bool? bold = null, bool? strikethrough = null, bool? underline = null, bool? invert = null, float? scale = null) {
		Fancy = fancy;
		Junimo = junimo;
		Bold = bold;
		Shadow = shadow;
		ShadowColor = shadowColor;
		Color = color;
		BackgroundColor = backgroundColor;
		Prismatic = prismatic;
		Font = font;
		Scale = scale;
		Strikethrough = strikethrough;
		Underline = underline;
		Invert = invert;
	}

	/// <summary>
	/// Copy all the distinct null-able properties from an existing style, and
	/// include the specified font, color, and shadow color. This method is
	/// necessary to avoid null values falling back to values from the existing
	/// style when trying to null out a value.
	/// </summary>
	/// <param name="existing"></param>
	/// <param name="font"></param>
	/// <param name="color"></param>
	/// <param name="shadowColor"></param>
	public TextStyle(TextStyle existing, SpriteFont? font, Color? color, Color? backgroundColor, Color? shadowColor, float? scale) {
		Fancy = existing.Fancy;
		Junimo = existing.Junimo;
		Bold = existing.Bold;
		Shadow = existing.Shadow;
		ShadowColor = shadowColor;
		Color = color;
		BackgroundColor = backgroundColor;
		Prismatic = existing.Prismatic;
		Font = font;
		Scale = scale;
		Strikethrough = existing.Strikethrough;
		Underline = existing.Underline;
		Invert = existing.Invert;
	}

	public TextStyle(TextStyle existing, Color? color = null, Color? backgroundColor = null, bool? prismatic = null, SpriteFont? font = null, bool? fancy = null, bool? junimo = null, bool? shadow = null, Color? shadowColor = null, bool? bold = null, bool? strikethrough = null, bool? underline = null, bool? invert = null, float? scale = null) {
		Fancy = fancy ?? existing.Fancy;
		Junimo = junimo ?? existing.Junimo;
		Bold = bold ?? existing.Bold;
		Shadow = shadow ?? existing.Shadow;
		ShadowColor = shadowColor ?? existing.ShadowColor;
		Color = color ?? existing.Color;
		BackgroundColor = backgroundColor ?? existing.BackgroundColor;
		Prismatic = prismatic ?? existing.Prismatic;
		Font = font ?? existing.Font;
		Scale = scale ?? existing.Scale;
		Strikethrough = strikethrough ?? existing.Strikethrough;
		Underline = underline ?? existing.Underline;
		Invert = invert ?? existing.Invert;
	}

	public bool HasShadow() {
		return Shadow ?? true;
	}

	public bool IsInverted() {
		return Invert ?? false;
	}

	public bool IsFancy() {
		return Fancy ?? false;
	}

	public bool IsJunimo() {
		return Junimo ?? false;
	}

	public bool IsBold() {
		return Bold ?? false;
	}

	public bool IsPrismatic() {
		return Prismatic ?? false;
	}

	public bool IsStrikethrough() {
		return Strikethrough ?? false;
	}

	public bool IsUnderline() {
		return Underline ?? false;
	}

	public bool IsEmpty() {
		return Equals(EMPTY);
	}

	#region Equality

	public override bool Equals(object? obj) {
		return obj is TextStyle style &&
			   Fancy == style.Fancy &&
			   Junimo == style.Junimo &&
			   Shadow == style.Shadow &&
			   EqualityComparer<Color?>.Default.Equals(ShadowColor, style.ShadowColor) &&
			   Bold == style.Bold &&
			   Invert == style.Invert &&
			   EqualityComparer<Color?>.Default.Equals(Color, style.Color) &&
			   EqualityComparer<Color?>.Default.Equals(BackgroundColor, style.BackgroundColor) &&
			   Prismatic == style.Prismatic &&
			   EqualityComparer<SpriteFont>.Default.Equals(Font, style.Font) &&
			   Strikethrough == style.Strikethrough &&
			   Underline == style.Underline &&
			   Scale == style.Scale;
	}

	public override int GetHashCode() {
		HashCode hash = new();
		hash.Add(Fancy);
		hash.Add(Junimo);
		hash.Add(Shadow);
		hash.Add(ShadowColor);
		hash.Add(Bold);
		hash.Add(Invert);
		hash.Add(Color);
		hash.Add(BackgroundColor);
		hash.Add(Prismatic);
		hash.Add(Font);
		hash.Add(Strikethrough);
		hash.Add(Underline);
		hash.Add(Scale);
		return hash.ToHashCode();
	}

	public static bool operator ==(TextStyle left, TextStyle right) {
		return left.Equals(right);
	}

	public static bool operator !=(TextStyle left, TextStyle right) {
		return !(left == right);
	}

	#endregion
}
