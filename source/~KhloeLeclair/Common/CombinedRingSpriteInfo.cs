/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/KhloeLeclair/StardewMods
**
*************************************************/


using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewValley;
using StardewValley.Menus;
using StardewValley.Objects;

using SObject = StardewValley.Object;

namespace Leclair.Stardew.Common {
	public class CombinedRingSpriteInfo : SpriteInfo {

		public CombinedRing Ring;

		public CombinedRingSpriteInfo(CombinedRing ring) : base(Game1.objectSpriteSheet, Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, ring.indexInTileSheet.Value, SObject.spriteSheetTileSize, SObject.spriteSheetTileSize)) {
			Ring = ring;
		}

		public override void Draw(SpriteBatch batch, Vector2 location, float scale, Vector2 size, int frame = -1, Color? baseColor = null, Color? overlayColor = null, float alpha = 1) {
			Draw(batch, location, scale, frame, size.X, baseColor, overlayColor, alpha);
		}

		public override void Draw(SpriteBatch batch, Vector2 location, float scale, int frame = -1, float size = 16, Color? baseColor = null, Color? overlayColor = null, float alpha = 1) {
			int firstIdx = Ring.combinedRings[0].indexInTileSheet.Value;
			int secondIdx = Ring.combinedRings[1].indexInTileSheet.Value;

			//location.Y -= 8 * scale;

			Vector2 Origin = new(2f, 1f);

			// Part One - Left Ring Arc
			Rectangle p1 = Game1.getSourceRectForStandardTileSheet(Texture, firstIdx, 16, 16);
			p1.X += 5;
			p1.Y += 7;
			p1.Width = 4;
			p1.Height = 6;

			batch.Draw(
				texture: Texture,
				position: location + new Vector2(5f, 7f) * scale,
				sourceRectangle: p1,
				color: Color.White,
				rotation: 0f,
				origin: Origin,
				scale: scale,
				effects: SpriteEffects.None,
				layerDepth: 1f
			);


			// Part Two - Left Ring Slice
			p1.X++;
			p1.Y += 4;
			p1.Width = 3;
			p1.Height = 1;

			batch.Draw(
				texture: Texture,
				position: location + new Vector2(6f, 6f) * scale,
				sourceRectangle: p1,
				color: Color.White,
				rotation: 0f,
				origin: Origin,
				scale: scale,
				effects: SpriteEffects.None,
				layerDepth: 1f
			);


			// Part Three - Right Ring Arc
			Rectangle p2 = Game1.getSourceRectForStandardTileSheet(Texture, secondIdx, 16, 16);
			p2.X += 9;
			p2.Y += 7;
			p2.Width = 4;
			p2.Height = 6;

			batch.Draw(
				texture: Texture,
				position: location + new Vector2(9f, 7f) * scale,
				sourceRectangle: p2,
				color: Color.White,
				rotation: 0f,
				origin: Origin,
				scale: scale,
				effects: SpriteEffects.None,
				layerDepth: 1f
			);


			// Part Four - Right Ring Slice
			p2.Y += 4;
			p2.Width = 3;
			p2.Height = 1;

			batch.Draw(
				texture: Texture,
				position: location + new Vector2(9f, 6f) * scale,
				sourceRectangle: p2,
				color: Color.White,
				rotation: 0f,
				origin: Origin,
				scale: scale,
				effects: SpriteEffects.None,
				layerDepth: 1f
			);


			// Dye Colors
			Color firstColor = TailoringMenu.GetDyeColor(Ring.combinedRings[0]) ?? Color.Red;
			Color secondColor = TailoringMenu.GetDyeColor(Ring.combinedRings[1]) ?? Color.Blue;

			// Standard Draw
			batch.Draw(
				texture: Texture,
				position: location,
				sourceRectangle: BaseSource,
				color: Utility.Get2PhaseColor(firstColor, secondColor),
				rotation: 0f,
				origin: new Vector2(2f, 1f),
				scale: scale,
				effects: SpriteEffects.None,
				layerDepth: 1f
			);

			// Final Drawing
			Rectangle RING_BIT = new(263, 579, 4, 2);

			// Left Ring Bit
			batch.Draw(
				texture: Texture,
				position: location + new Vector2(2f, 8f) * scale,
				sourceRectangle: RING_BIT,
				color: Utility.Get2PhaseColor(firstColor, secondColor, timeOffset: 1125f),
				rotation: -1.570796f,
				origin: new Vector2(2f, 1f),
				scale: scale,
				effects: SpriteEffects.None,
				layerDepth: 1f
			);

			// Right Ring Bit
			batch.Draw(
				texture: Texture,
				position: location + new Vector2(12f, 8f) * scale,
				sourceRectangle: RING_BIT,
				color: Utility.Get2PhaseColor(firstColor, secondColor, timeOffset: 375f),
				rotation: 1.570796f,
				origin: new Vector2(2f, 1f),
				scale: scale,
				effects: SpriteEffects.None,
				layerDepth: 1f
			);

			// Bottom Ring Bit
			batch.Draw(
				texture: Texture,
				position: location + new Vector2(7f, 13f) * scale,
				sourceRectangle: RING_BIT,
				color: Utility.Get2PhaseColor(firstColor, secondColor, timeOffset: 750f),
				rotation: 3.141593f,
				origin: new Vector2(2f, 1f),
				scale: scale,
				effects: SpriteEffects.None,
				layerDepth: 1f
			);
		}
	}
}
