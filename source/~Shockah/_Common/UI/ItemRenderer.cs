/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Shockah/Stardew-Valley-Mods
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Objects;
using System;
using SObject = StardewValley.Object;

namespace Shockah.CommonModCode.UI
{
	public class ItemRenderer
	{
		private Texture2D GetItemTexture(SObject @object)
		{
			if (@object.ParentSheetIndex < 0)
				return Game1.mouseCursors;

			if (@object.bigCraftable.Value)
				return Game1.bigCraftableSpriteSheet;
			else
				return Game1.objectSpriteSheet;
		}

		private Rectangle GetItemSourceRectangle(SObject @object)
		{
			var index = @object.ParentSheetIndex;
			if (index < 0)
				return new(322, 498, 12, 12);

			if (@object.showNextIndex.Value)
				index++;
			if (@object.bigCraftable.Value)
				return SObject.getSourceRectForBigCraftable(index);
			else
				return Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, index, 16, 16);
		}

		private Rectangle GetColoredItemSourceRectangle(ColoredObject @object)
		{
			var index = @object.ParentSheetIndex;
			if (index < 0)
				return new(322, 498, 12, 12);

			if (!@object.ColorSameIndexAsParentSheetIndex)
				index++;
			if (@object.bigCraftable.Value)
				return SObject.getSourceRectForBigCraftable(index);
			else
				return Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, index, 16, 16);
		}

		public void DrawItem(SpriteBatch batch, SObject @object, Vector2 rectLocation, Vector2 rectSize, Color color, UIAnchorSide rectAnchorSide = UIAnchorSide.TopLeft, Vector2? scale = null, float layerDepth = 0f)
		{
			var texture = GetItemTexture(@object);
			var sourceRectangle = GetItemSourceRectangle(@object);

			var itemSize = new Vector2(sourceRectangle.Size.X, sourceRectangle.Size.Y);
			if (itemSize.X < rectSize.X && itemSize.Y < rectSize.Y)
				itemSize *= Math.Max(rectSize.X, rectSize.Y);
			if (itemSize.X > rectSize.X)
				itemSize = itemSize / itemSize.X * rectSize.X;
			if (itemSize.Y > rectSize.Y)
				itemSize = itemSize / itemSize.Y * rectSize.Y;
			var itemPosition = rectAnchorSide.GetAnchorPoint(
				new Vector2(
					rectLocation.X + (rectSize.X - itemSize.X) * 0.5f,
					rectLocation.Y + (rectSize.Y - itemSize.Y) * 0.5f
				),
				-rectSize
			);
			var finalScale = itemSize.X / sourceRectangle.Width;
			var origin = new Vector2(sourceRectangle.Width * 0.5f, sourceRectangle.Height);
			batch.Draw(texture, itemPosition + origin * finalScale, sourceRectangle, color, 0f, origin, finalScale * (scale ?? Vector2.One), SpriteEffects.None, layerDepth);
			if (@object is ColoredObject coloredObject)
				batch.Draw(texture, itemPosition + origin * finalScale, GetColoredItemSourceRectangle(coloredObject), Color.Lerp(color, coloredObject.color.Value, 0.5f), 0f, origin, finalScale * (scale ?? Vector2.One), SpriteEffects.None, layerDepth);
		}
	}
}
