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

using SObject = StardewValley.Object;

namespace Leclair.Stardew.Hydrology {
	public class WaterFeatureObject : SObject {

		public WaterFeatureObject() {
			name = "Water Feature";
			Category = -24;
			Price = 100;
			CanBeSetDown = true;
		}

		public override bool canStackWith(ISalable other) {
			return other is WaterFeatureObject;
		}

		public override bool canBeGivenAsGift() {
			return false;
		}

		public override bool canBeTrashed() {
			return true;
		}

		public override int salePrice() {
			return Price;
		}

		public override Item getOne() {
			return new WaterFeatureObject();
		}

		public override bool canBePlacedHere(GameLocation l, Vector2 tile) {
			return /*l.IsOutdoors &&*/ !l.terrainFeatures.ContainsKey(tile) && !l.isTileOccupiedForPlacement(tile);
		}

		public override bool isPlaceable() {
			return true;
		}

		public override bool placementAction(GameLocation location, int x, int y, Farmer who = null) {
			//if (!location.IsOutdoors)
			//	return false;

			Vector2 tile = new Vector2(x / Game1.tileSize, y / Game1.tileSize);
			if (location.terrainFeatures.ContainsKey(tile))
				return false;

			WaterFeature feature = new WaterFeature();
			location.terrainFeatures.Add(tile, feature);
			feature.AddTile(location, (int) tile.X, (int) tile.Y);

			Game1.playSound("hoeHit");
			return true;
		}

		public override void drawInMenu(SpriteBatch spriteBatch, Vector2 location, float scaleSize, float transparency, float layerDepth, StackDrawType drawStackNumber, Color color, bool drawShadow) {
			int center = (int) (Game1.tileSize / 2 * scaleSize);

			spriteBatch.Draw(
				ModEntry.instance.Objects,
				location + new Vector2(center, center),
				new Rectangle(0, 0, 16, 16),
				Color.White * transparency,
				0f,
				new Vector2(8f, 8f) * scaleSize,
				Game1.pixelZoom * scaleSize,
				SpriteEffects.None,
				layerDepth
			);

			if (drawStackNumber != StackDrawType.Hide && maximumStackSize() > 1 && scaleSize > 0.3 && Stack <= 999 && Stack > 1)
				Utility.drawTinyDigits(Stack, spriteBatch, location + new Vector2((Game1.tileSize - Utility.getWidthOfTinyDigitString(Stack, 3f * scaleSize)) + 3f * scaleSize, Game1.tileSize - 18f * scaleSize + 2f), 3f * scaleSize, 1f, Color.White);
		}
		public override void drawWhenHeld(SpriteBatch spriteBatch, Vector2 objectPosition, Farmer f) {
			spriteBatch.Draw(
				ModEntry.instance.Objects,
				objectPosition,
				new Rectangle(0, 0, 16, 16),
				Color.White,
				0f,
				Vector2.Zero,
				Game1.pixelZoom,
				SpriteEffects.None,
				1
			);
		}

		public override void draw(SpriteBatch spriteBatch, int xNonTile, int yNonTile, float layerDepth, float alpha = 1) {
			Vector2 pos = Game1.GlobalToLocal(Game1.viewport, new Vector2(xNonTile * Game1.tileSize + Game1.tileSize / 2, yNonTile * Game1.tileSize + Game1.tileSize / 2));
			spriteBatch.Draw(
				ModEntry.instance.Objects,
				pos,
				new Rectangle(0, 0, 16, 16),
				Color.White * alpha,
				0f,
				new Vector2(8f, 8f),
				Game1.pixelZoom,
				SpriteEffects.None,
				0
			);
		}

	}
}
