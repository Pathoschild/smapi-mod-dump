/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/KhloeLeclair/StardewMods
**
*************************************************/

#if COMMON_CRAFTING

using System;

using Leclair.Stardew.Common.Crafting;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewValley;

namespace Leclair.Stardew.Common;


public class DynamicRecipeSpriteInfo : SpriteInfo {

	public readonly IDynamicDrawingRecipe Recipe;

	public DynamicRecipeSpriteInfo(IDynamicDrawingRecipe recipe, int quality = 0) : base(recipe.Texture, recipe.SourceRectangle, quality: quality) {
		Recipe = recipe;
	}

	public override void Draw(SpriteBatch batch, Vector2 location, float scale, int frame = -1, float size = 16, Color? baseColor = null, Color? overlayColor = null, float alpha = 1) {
		Draw(batch, location, scale, new Vector2(size, size), frame, baseColor, overlayColor, alpha);
	}

	public override void Draw(SpriteBatch batch, Vector2 location, float scale, Vector2 size, int frame = -1, Color? baseColor = null, Color? overlayColor = null, float alpha = 1) {
		if (Recipe.ShouldDoDynamicDrawing) {
			Recipe.Draw(
				batch,
				new Rectangle((int) location.X, (int) location.Y, (int) (size.X * scale), (int) (size.Y * scale)),
				(baseColor ?? Color.White) * alpha,
				false,
				false,
				1f,
				null
			);
			return;
		}

		Rectangle source = Recipe.SourceRectangle;

		float width = source.Width * BaseScale;
		float height = source.Height * BaseScale;

		float max = Math.Max(width, height);

		float targetWidth = scale * size.X;
		float targetHeight = scale * size.Y;

		float s = Math.Min(scale, Math.Min(targetWidth / width, targetHeight / height));

		// Draw the base.
		float bs = s * BaseScale;
		float offsetX = Math.Max((targetWidth - (source.Width * bs)) / 2, 0);
		float offsetY = Math.Max((targetHeight - (source.Height * bs)) / 2, 0);

		var pos = new Vector2(
			(float) Math.Floor(location.X + offsetX),
			(float) Math.Floor(location.Y + offsetY)
		);

		batch.Draw(
			Recipe.Texture,
			pos,
			source,
			Color.White * alpha,
			0f,
			Vector2.Zero,
			bs,
			SpriteEffects.None,
			1f
		);

		if (Quality != 0) {
			Rectangle qualityRect = Quality < 4
				? new Rectangle(338 + (Quality - 1) * 8, 400, 8, 8)
				: new Rectangle(346, 392, 8, 8);

			float qualityYOffset = (Quality < 4)
				? 0f :
				(((float) Math.Cos(Game1.currentGameTime.TotalGameTime.Milliseconds * Math.PI / 512.0) + 1f) * 0.05f);

			float qualityScale = 0.75f * s; // * (1f - qualityYOffset);
			int qHeight = (int) (qualityRect.Height * qualityScale);

			batch.Draw(
				Game1.mouseCursors,
				new Vector2(
					location.X,
					location.Y + (targetHeight - qHeight) + qualityYOffset
			),
				qualityRect,
				Color.White * alpha,
				0f,
				Vector2.Zero,
				qualityScale,
				SpriteEffects.None,
				1f
			);
		}

	}
}

#endif
