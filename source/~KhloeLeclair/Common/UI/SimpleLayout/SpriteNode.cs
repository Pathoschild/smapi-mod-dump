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

using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewValley;

namespace Leclair.Stardew.Common.UI.SimpleLayout;

public class SpriteNode : ISimpleNode {

	public SpriteInfo? Sprite { get; }
	public float Scale { get; }
	public int Quantity { get; }
	public string? Label { get; }
	public float? OverrideHeight { get; }


	public Alignment Alignment { get; }

	public bool DeferSize => false;

	public SpriteNode(SpriteInfo? sprite, float scale = 4f, string? label = null, int quantity = 0, Alignment alignment = Alignment.None, float? overrideHeight = null) {
		Sprite = sprite;
		Scale = scale;
		Label = label;
		Quantity = quantity;
		Alignment = alignment;
		OverrideHeight = overrideHeight;
	}

	public Vector2 GetSize(SpriteFont defaultFont, Vector2 containerSize) {
		float height = 16 * Scale;
		float width = height;

		if (!string.IsNullOrEmpty(Label)) {
			Vector2 size = Game1.smallFont.MeasureString(Label);
			width += (4 * Scale) + size.X;
			height = Math.Max(height, size.Y);
		}

		if (Quantity > 0) {
			float qScale = (float) Math.Round(Scale * 0.75f);
			float qX = (16 * Scale) - Utility.getWidthOfTinyDigitString(Quantity, qScale) + qScale;
			if (qX < 0)
				width -= qX;
		}

		if (OverrideHeight.HasValue)
			height = OverrideHeight.Value;

		return new Vector2(width, height);
	}

	public void Draw(SpriteBatch batch, Vector2 position, Vector2 size, Vector2 containerSize, float alpha, SpriteFont defaultFont, Color? defaultColor, Color? defaultShadowColor) {

		float itemSize = 16 * Scale;
		float offsetY = (size.Y - itemSize);

		if (OverrideHeight.HasValue)
			offsetY += (OverrideHeight.Value - itemSize);

		offsetY /= 2;

		float offsetX = 0;

		// Draw Object
		Sprite?.Draw(
			batch,
			offsetY != 0 || offsetX != 0 ?
				new Vector2(position.X + offsetX, position.Y + offsetY)
				: position,
			Scale
		);

		// Draw Quantity
		if (Quantity > 0) {
			float qScale = (float) Math.Round(Scale * 0.75f);
			float qX = position.X + itemSize - Utility.getWidthOfTinyDigitString(Quantity, qScale) + qScale;
			float qY = position.Y + itemSize - 6f * qScale + 2f;

			if (qX < position.X)
				offsetX = position.X - qX;

			Utility.drawTinyDigits(Quantity, batch, new Vector2(qX + offsetX, qY), qScale, 1f, Color.White * alpha);
		}

		// Draw Label
		if (!string.IsNullOrEmpty(Label)) {
			Vector2 labelSize = Game1.smallFont.MeasureString(Label);
			if (defaultShadowColor.HasValue)
				Utility.drawTextWithColoredShadow(
					b: batch,
					text: Label,
					font: Game1.smallFont,
					position: new Vector2(
						position.X + offsetX + itemSize + (4 * Scale),
						position.Y + ((size.Y - labelSize.Y) / 2)
						),
					color: (defaultColor ?? Game1.textColor) * alpha,
					shadowColor: (defaultShadowColor.Value * alpha)
				);
			else
				Utility.drawTextWithShadow(
					b: batch,
					text: Label,
					font: Game1.smallFont,
					position: new Vector2(
						position.X + offsetX + itemSize + (4 * Scale),
						position.Y + ((size.Y - labelSize.Y) / 2)
						),
					color: (defaultColor ?? Game1.textColor) * alpha
				);
		}
	}
}

#endif
