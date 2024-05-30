/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/KhloeLeclair/StardewMods
**
*************************************************/

#if COMMON_SIMPLELAYOUT

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewValley;
using StardewValley.BellsAndWhistles;

namespace Leclair.Stardew.Common.UI.SimpleLayout;

public class TextNode : ISimpleNode {

	public Alignment Alignment { get; }
	public string? Text { get; }
	public TextStyle Style { get; }

	public TextNode(string? text, TextStyle? style = null, Alignment alignment = Alignment.None) {
		Text = text;
		Style = style ?? TextStyle.EMPTY;
		Alignment = alignment;
	}

	public bool DeferSize => false;

	public static Vector2 GetSpriteSize(string text) {
		return new Vector2(
			SpriteText.getWidthOfString(text),
			SpriteText.getHeightOfString(text)
		);
	}

	public Vector2 GetSize(SpriteFont defaultFont, Vector2 containerSize) {
		if (string.IsNullOrEmpty(Text))
			return Vector2.Zero;

		Vector2 size;
		float scale = Style.Scale ?? 1f;
		SpriteFont font = Style.Font ?? defaultFont ?? Game1.smallFont;

		if (Style.IsFancy() || Style.IsJunimo())
			size = GetSpriteSize(Text);
		else
			size = font.MeasureString(Text) * scale;

		return size;
	}

	public void Draw(SpriteBatch batch, Vector2 position, Vector2 size, Vector2 containerSize, float alpha, SpriteFont defaultFont, Color? defaultColor, Color? defaultShadowColor) {
		if (string.IsNullOrEmpty(Text))
			return;

		float scale = Style.Scale ?? 1f;
		SpriteFont font = Style.Font ?? defaultFont ?? Game1.smallFont;
		Color color = Style.Color ?? defaultColor ?? Game1.textColor;
		Color background = Style.BackgroundColor ?? Color.Transparent;
		Color? shadowColor = Style.ShadowColor ?? defaultShadowColor;
		if (Style.IsPrismatic())
			color = Utility.GetPrismaticColor();

		if (Style.IsInverted())
			(color, background) = (background, color);

		if (background.A > 0) {
			float a = (float) background.A / 255;

			batch.Draw(
				Game1.fadeToBlackRect,
				new Rectangle(
					(int) position.X, (int) position.Y,
					(int) size.X, (int) size.Y
				),
				background * a
			);
		}

		if (Style.IsFancy() || Style.IsJunimo())
			SpriteText.drawString(
				batch,
				Text,
				(int) position.X, (int) position.Y,
				junimoText: Style.IsJunimo(),
				color: color
			);
		else if (Style.IsBold())
			Utility.drawBoldText(batch, Text, font, position, color, scale);
		else if (Style.HasShadow()) {
			if (shadowColor.HasValue)
				Utility.drawTextWithColoredShadow(batch, Text, font, position, color, shadowColor.Value, scale);
			else
				Utility.drawTextWithShadow(batch, Text, font, position, color, scale);
		} else
			batch.DrawString(font, Text, position, color, 0f, Vector2.Zero, scale, SpriteEffects.None, GUIHelper.GetLayerDepth(position.Y));

		if (Style.IsStrikethrough())
			Utility.drawLineWithScreenCoordinates(
				(int) position.X,
				(int) (position.Y + size.Y / 2),
				(int) (position.X + size.X),
				(int) (position.Y + size.Y / 2),
				batch,
				color
			);

		if (Style.IsUnderline())
			Utility.drawLineWithScreenCoordinates(
				(int) position.X,
				(int) (position.Y + size.Y * 3 / 4),
				(int) (position.X + size.X),
				(int) (position.Y + size.Y * 3 / 4),
				batch,
				color
			);
	}
}

#endif
