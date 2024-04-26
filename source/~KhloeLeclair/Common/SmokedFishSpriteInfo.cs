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
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewValley;
using StardewValley.Objects;

namespace Leclair.Stardew.Common;

public class SmokedFishSpriteInfo : SpriteInfo {

	private static Color FISH_COLOR = new Color(80, 30, 10) * 0.6f;

	public ColoredObject Fish;

	public SmokedFishSpriteInfo(ColoredObject fish) : base(null!, Rectangle.Empty) {
		Fish = fish;

		var data = ItemRegistry.GetDataOrErrorItem(Fish.preservedParentSheetIndex.Value)!;

		Texture = data.GetTexture();
		BaseSource = data.GetSourceRect();
	}

	public override void Draw(SpriteBatch batch, Vector2 location, float scale, int frame = -1, float size = 16, Color? baseColor = null, Color? overlayColor = null, float alpha = 1) {
		Draw(batch, location, scale, new Vector2(size, size), frame, baseColor, overlayColor, alpha);
	}

	public override void Draw(SpriteBatch batch, Vector2 location, float scale, Vector2 size, int frame = -1, Color? baseColor = null, Color? overlayColor = null, float alpha = 1) {
		Rectangle source = BaseSource;

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
			Texture,
			pos,
			source,
			Color.White * alpha,
			0f,
			Vector2.Zero,
			bs,
			SpriteEffects.None,
			1f
		);

		batch.Draw(
			Texture,
			pos,
			source,
			FISH_COLOR * alpha,
			0f,
			Vector2.Zero,
			bs,
			SpriteEffects.None,
			1.00001f
		);

		int interval = 700 + (Fish.Price + 17) * 7777 % 20;

		// TODO: Improve this to be more like vanilla.

		batch.Draw(
			Game1.mouseCursors,
			pos + new Vector2(
				targetWidth / 2, targetHeight / 2 + (float) ((0.0 - Game1.currentGameTime.TotalGameTime.TotalMilliseconds) % 2000.0) * 0.01f
			),
			new Rectangle(372, 1956, 10, 10),
			new Color(80, 80, 80) * alpha * 0.53f * (1f - (float) (Game1.currentGameTime.TotalGameTime.TotalMilliseconds % 2000.0) / 2000f),
			(float) ((0.0 - Game1.currentGameTime.TotalGameTime.TotalMilliseconds) % 2000.0) * 0.001f,
			Vector2.Zero,
			bs / 2f,
			SpriteEffects.None,
			1.00002f
		);

		// TODO: The other three calls.

	}

}
