/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/b-b-blueberry/GiftWrapper
**
*************************************************/

using System;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using Colour = Microsoft.Xna.Framework.Color;
using Object = StardewValley.Object;

namespace GiftWrapper
{
	[XmlType("Mods_blueberry_GiftWrapper_WrapItem")]
	public class WrapItem : Object
	{
		public WrapItem() : base() 
		{
			// Item values
			this.Name = ModEntry.ItemPrefix + ModEntry.WrapItemName;
			this.Category = ModEntry.Definitions?.CategoryNumber ?? -1;
			this.Price = ModEntry.Definitions?.WrapValue ?? 0;
		}

		public WrapItem(int stack) : this()
		{
			this.Stack = stack;
		}

		public override string DisplayName
		{
			get => ModEntry.I18n.Get("item.giftwrap.name");
			set {}
		}

		public override string getCategoryName()
		{
			return ModEntry.I18n.Get("category.gift.name");
		}

		public override Colour getCategoryColor()
		{
			return ModEntry.Definitions.CategoryTextColour;
		}

		public override string getDescription()
		{
			return Game1.parseText(
				text: ModEntry.I18n.Get("item.giftwrap.description"),
				whichFont: Game1.smallFont,
				width: this.getDescriptionWidth());
		}

		public override Item getOne()
		{
			return new WrapItem();
		}

		public override bool performUseAction(GameLocation location)
		{
			if (!Context.CanPlayerMove
				|| this.isTemporarilyInvisible
				|| Game1.eventUp
				|| Game1.isFestival()
				|| Game1.fadeToBlack
				|| Game1.player.swimming.Value
				|| Game1.player.bathingClothes.Value
				|| Game1.player.onBridge.Value)
				return false;

			Game1.activeClickableMenu = new GiftWrapMenu();
			return false;
		}

		public override void drawInMenu(SpriteBatch spriteBatch, Vector2 location, float scaleSize, float transparency, float layerDepth, StackDrawType drawStackNumber, Color color, bool drawShadow)
		{
			Rectangle source = ModEntry.Definitions.WrapItemSource;
			Vector2 origin = source.Size.ToVector2() / 2;
			float scale = Game1.pixelZoom * Game1.smallestTileSize / Math.Max(source.Width, source.Height);

			bool shouldDrawStackNumber = (double)scaleSize > 0.3 && this.Stack != int.MaxValue
				&& ((drawStackNumber == StackDrawType.Draw && this.maximumStackSize() > 1 && this.Stack > 1)
					|| drawStackNumber == StackDrawType.Draw_OneInclusive);

			// Shadow
			if (drawShadow)
			{
				spriteBatch.Draw(
					texture: Game1.shadowTexture,
					position: location + new Vector2(Game1.tileSize / 2, Game1.tileSize / 4 * 3),
					sourceRectangle: Game1.shadowTexture.Bounds,
					color: color * 0.5f,
					rotation: 0,
					origin: Game1.shadowTexture.Bounds.Size.ToVector2() / 2,
					scale: 3,
					effects: SpriteEffects.None,
					layerDepth: layerDepth - 0.0001f);
			}

			// Item
			spriteBatch.Draw(
				texture: ModEntry.WrapSprite.Value,
				position: location + origin * scale,
				sourceRectangle: source,
				color: color * transparency,
				rotation: 0,
				origin: origin,
				scale: scale * scaleSize,
				effects: SpriteEffects.None,
				layerDepth: layerDepth);

			// Stack
			if (shouldDrawStackNumber)
			{
				float tinyScale = 3 * scaleSize;
				Utility.drawTinyDigits(
					toDraw: this.Stack,
					b: spriteBatch,
					position: location
						+ new Vector2(Game1.tileSize)
						- new Vector2(
							x: Utility.getWidthOfTinyDigitString(toDraw: this.Stack, scale: tinyScale) + tinyScale,
							y: 18 * scaleSize + 1),
					scale: tinyScale,
					layerDepth: 1,
					c: color);
			}
		}

		public override void draw(SpriteBatch spriteBatch, int x, int y, float alpha = 1)
		{
			Vector2 position = new Vector2(x: x, y: y) * Game1.tileSize;
			float layerDepth = ((y + 1) * Game1.tileSize) / 10000f + this.TileLocation.X / 50000f;
			this.DrawWrapItem(
				b: spriteBatch,
				globalX: (int)position.X,
				globalY: (int)position.Y,
				alpha: alpha,
				layerDepth: layerDepth + 1E-06f,
				drawShadow: false,
				isInWorld: true);
		}

		public override void draw(SpriteBatch spriteBatch, int xNonTile, int yNonTile, float layerDepth, float alpha = 1)
		{
			this.DrawWrapItem(
				b: spriteBatch,
				globalX: xNonTile,
				globalY: yNonTile,
				alpha: alpha,
				layerDepth: layerDepth,
				isInWorld: true);
		}

		public override void drawWhenHeld(SpriteBatch spriteBatch, Vector2 objectPosition, Farmer f)
		{
			this.DrawWrapItem(
				b: spriteBatch,
				globalX: Game1.viewport.X + (int)objectPosition.X,
				globalY: Game1.viewport.Y + (int)objectPosition.Y,
				layerDepth: Math.Max(0, (f.getStandingY() + 3) / 10000f));
		}

		public override void drawAsProp(SpriteBatch b)
		{
			Vector2 position = this.TileLocation * Game1.tileSize
				+ new Vector2(Game1.tileSize / 2)
				- Game1.viewport.ToXna().Location.ToVector2();
			this.DrawWrapItem(
				b: b,
				globalX: (int)position.X,
				globalY: (int)position.Y,
				layerDepth: this.getBoundingBox(this.TileLocation).Bottom / 15000f);
		}

		public void DrawWrapItem(SpriteBatch b, int globalX, int globalY, float alpha = 1, float layerDepth = 1, bool drawShadow = true, bool isInWorld = false)
		{
			if (this.isTemporarilyInvisible)
				return;
			if (isInWorld && (Game1.CurrentEvent?.isTileWalkedOn(x: globalX / Game1.tileSize, y: globalY / Game1.tileSize) ?? false))
				return;

			Vector2 position;
			Rectangle source;
			Colour colour = Colour.White * alpha;
			float rotation = 0;
			float baseScale = Game1.pixelZoom;

			// Shadow
			if (drawShadow)
			{
				source = Game1.shadowTexture.Bounds;
				position = new Vector2(Game1.tileSize / 2)
					+ new Vector2(x: globalX, y: globalY)
					+ new Vector2(x: 0, y: 16 + 3 + 4)
					- Game1.viewport.ToXna().Location.ToVector2();
				b.Draw(
					texture: Game1.shadowTexture,
					position: position,
					sourceRectangle: source,
					color: colour,
					rotation: rotation,
					origin: source.Size.ToVector2() / 2,
					scale: baseScale,
					effects: SpriteEffects.None,
					layerDepth: layerDepth - 1E-06f);
			}

			// Item
			source = ModEntry.Definitions.WrapItemSource;
			position = new Vector2(Game1.tileSize / 2)
				+ new Vector2(
					x: globalX,
					y: globalY)
				+ (this.shakeTimer > 0
					? new Vector2(
						x: Game1.random.Next(-2, 2),
						y: Game1.random.Next(-2, 2))
					: Vector2.Zero)
				- Game1.viewport.ToXna().Location.ToVector2();
			b.Draw(
				texture: ModEntry.WrapSprite.Value,
				position: position,
				sourceRectangle: source,
				color: colour,
				rotation: rotation,
				origin: source.Size.ToVector2() / 2,
				scale: (this.scale.Y > 1) ? this.getScale().Y : baseScale,
				effects: this.Flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
				layerDepth: layerDepth);
		}
	}
}
