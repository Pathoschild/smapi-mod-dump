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

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Leclair.Stardew.Common.UI {
	public struct TextStyle {

		public readonly static TextStyle EMPTY = new();

		public bool? Fancy { get; }
		public bool? Junimo { get; }
		public bool? Shadow { get; }
		public Color? ShadowColor { get; }
		public bool? Bold { get; }
		public Color? Color { get; }
		public bool? Prismatic { get; }
		public SpriteFont Font { get; }
		public bool? Strikethrough { get; }
		public bool? Underline { get; }
		public float? Scale { get; }

		public TextStyle(Color? color = null, bool? prismatic = null, SpriteFont font = null, bool? fancy = null, bool? junimo = null, bool? shadow = null, Color? shadowColor = null, bool? bold = null, bool? strikethrough = null, bool? underline = null, float? scale = null) {
			Fancy = fancy;
			Junimo = junimo;
			Bold = bold;
			Shadow = shadow;
			ShadowColor = shadowColor;
			Color = color;
			Prismatic = prismatic;
			Font = font;
			Scale = scale;
			Strikethrough = strikethrough;
			Underline = underline;
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
		public TextStyle(TextStyle existing, SpriteFont font, Color? color, Color? shadowColor, float? scale) {
			Fancy = existing.Fancy;
			Junimo = existing.Junimo;
			Bold = existing.Bold;
			Shadow = existing.Shadow;
			ShadowColor = shadowColor;
			Color = color;
			Prismatic = existing.Prismatic;
			Font = font;
			Scale = scale;
			Strikethrough = existing.Strikethrough;
			Underline = existing.Underline;
		}

		public TextStyle(TextStyle existing, Color? color = null, bool? prismatic = null, SpriteFont font = null, bool? fancy = null, bool? junimo = null, bool? shadow = null, Color? shadowColor = null, bool? bold = null, bool? strikethrough = null, bool? underline = null, float? scale = null) {
			Fancy = fancy ?? existing.Fancy;
			Junimo = junimo ?? existing.Junimo;
			Bold = bold ?? existing.Bold;
			Shadow = shadow ?? existing.Shadow;
			ShadowColor = shadowColor ?? existing.ShadowColor;
			Color = color ?? existing.Color;
			Prismatic = prismatic ?? existing.Prismatic;
			Font = font ?? existing.Font;
			Scale = scale ?? existing.Scale;
			Strikethrough = strikethrough ?? existing.Strikethrough;
			Underline = underline ?? existing.Underline;
		}

		public bool HasShadow() {
			return Shadow ?? true;
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

		public override bool Equals(object obj) {
			if (!(obj is TextStyle))
				return false;

			TextStyle other = (TextStyle) obj;
			return
				Fancy.Equals(other.Fancy) &&
				Junimo.Equals(other.Junimo) &&
				Shadow.Equals(other.Shadow) &&
				ShadowColor.Equals(other.ShadowColor) &&
				Bold.Equals(other.Bold) &&
				Color.Equals(other.Color) &&
				Prismatic.Equals(other.Prismatic) &&
				Font.Equals(other.Font) &&
				Scale.Equals(other.Scale) &&
				Strikethrough.Equals(other.Strikethrough) &&
				Underline.Equals(other.Underline);
		}

		public override int GetHashCode() {
			int hashCode = -412244955;
			hashCode = hashCode * -1521134295 + EqualityComparer<bool?>.Default.GetHashCode(Fancy);
			hashCode = hashCode * -1521134295 + EqualityComparer<bool?>.Default.GetHashCode(Junimo);
			hashCode = hashCode * -1521134295 + EqualityComparer<bool?>.Default.GetHashCode(Shadow);
			hashCode = hashCode * -1521134295 + EqualityComparer<Color?>.Default.GetHashCode(ShadowColor);
			hashCode = hashCode * -1521134295 + EqualityComparer<bool?>.Default.GetHashCode(Bold);
			hashCode = hashCode * -1521134295 + EqualityComparer<Color?>.Default.GetHashCode(Color);
			hashCode = hashCode * -1521134295 + EqualityComparer<bool?>.Default.GetHashCode(Prismatic);
			hashCode = hashCode * -1521134295 + EqualityComparer<SpriteFont>.Default.GetHashCode(Font);
			hashCode = hashCode * -1521134295 + EqualityComparer<bool?>.Default.GetHashCode(Strikethrough);
			hashCode = hashCode * -1521134295 + EqualityComparer<bool?>.Default.GetHashCode(Underline);
			hashCode = hashCode * -1521134295 + EqualityComparer<float?>.Default.GetHashCode(Scale);
			return hashCode;
		}
	}
}
