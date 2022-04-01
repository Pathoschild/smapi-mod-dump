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

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewModdingAPI;

using StardewValley;


namespace Leclair.Stardew.Common.UI.SimpleLayout {
	public struct FallbackItem : ISimpleNode {

		private readonly IModHelper Helper;

		public Item Value { get; }
		public Alignment Alignment { get; }
		public float Scale { get; }
		public bool DrawQuantity { get; }
		public bool DrawLabel { get; }
		public bool DeferSize => false;

		public FallbackItem(Item value, IModHelper helper, Alignment alignment = Alignment.None, float scale = 4, bool drawQuantity = false, bool drawLabel = false) {
			Value = value;
			Helper = helper;
			Alignment = alignment;
			Scale = scale;
			DrawQuantity = drawQuantity;
			DrawLabel = drawLabel;
		}

		public Vector2 GetSize(SpriteFont defaultFont, Vector2 containerSize) {

			float height = 16 * Scale;
			float width = height;

			if (DrawLabel) {
				string name = Value?.DisplayName ?? "???";
				Vector2 size = Game1.smallFont.MeasureString(name);
				width += (4 * Scale) + size.X;
				height = Math.Max(height, size.Y);
			}

			return new Vector2(width, height);
		}

		public void Draw(SpriteBatch batch, Vector2 position, Vector2 size, Vector2 containerSize, float alpha, SpriteFont defaultFont, Color? defaultColor, Color? defaultShadowColor) {

			float itemSize = 16 * Scale;

			// Draw Object
			float offsetY = (size.Y - itemSize) / 2;

			if (Value != null) {
				Vector2 pos = new(position.X, position.Y + offsetY);
				if (SpriteHelper.GetSprite(Value) is SpriteInfo sprite)
					sprite.Draw(batch, pos, Scale);
				else
					// Fallback. This is basically guaranteed to be
					// inaccurate, but we don't care.
					Value.drawInMenu(
						batch,
						location: pos,
						scaleSize: Scale / 4f,
						transparency: alpha,
						layerDepth: 0.865f,
						drawStackNumber: StackDrawType.Hide,
						color: Color.White,
						drawShadow: false
					);
			}

			// Draw Label
			if (DrawLabel) {
				string name = Value?.DisplayName ?? "???";
				Vector2 labelSize = Game1.smallFont.MeasureString(name);
				if (defaultShadowColor.HasValue)
					Utility.drawTextWithColoredShadow(
						batch,
						name,
						Game1.smallFont,
						new Vector2(
							position.X + itemSize + (4 * Scale),
							position.Y + ((size.Y - labelSize.Y) / 2)
						),
						(defaultColor ?? Game1.textColor) * alpha,
						(defaultShadowColor.Value * alpha)
					);
				else
					Utility.drawTextWithShadow(
						batch,
						name,
						Game1.smallFont,
						new Vector2(
							position.X + itemSize + (4 * Scale),
							position.Y + ((size.Y - labelSize.Y) / 2)
						),
						(defaultColor ?? Game1.textColor) * alpha
					);
			}

			// Draw Quantity
			if (DrawQuantity && Value != null && Value.Stack > 1) {
				float qScale = (float) Math.Round(Scale * 0.75f);
				float qX = itemSize - Utility.getWidthOfTinyDigitString(Value.Stack, qScale) + qScale;
				float qY = itemSize - 6f * qScale + 2f + offsetY;

				Utility.drawTinyDigits(Value.Stack, batch, new Vector2(position.X + qX, position.Y + qY), qScale, 1f, Color.White * alpha);
			}
		}
	}
}
