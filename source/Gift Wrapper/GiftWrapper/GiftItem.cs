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
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using GiftWrapper.Data;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewModdingAPI;
using StardewValley;
using Colour = Microsoft.Xna.Framework.Color;
using Object = StardewValley.Object;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace GiftWrapper
{
	[XmlType("Mods_blueberry_GiftWrapper_GiftItem")]
	public class GiftItem : Object
	{
		public readonly NetLong Owner;
		public readonly NetString Style;
		public readonly NetRef<Item> ItemInGift;
		public readonly NetInt HitCount;

		public bool IsOwnerViewing => Game1.player?.UniqueMultiplayerID == this.Owner?.Value;
		public bool IsItemInside => this.ItemInGift?.Value is not null;
		public bool IsTooltipActive => this.IsOwnerViewing
			&& ModEntry.Config.GiftPreviewEnabled
			&& Context.IsMultiplayer
			&& Context.CanPlayerMove
			&& Vector2.Equals(this.TileLocation, Vector2.Floor(Game1.GetPlacementGrabTile()))
			&& Vector2.Distance(this.TileLocation, Game1.player.getTileLocation()) < ModEntry.Config.GiftPreviewTileRange;

		protected Style _style;
		protected float _tooltipAlpha;

		public override string DisplayName
		{
			get => ModEntry.I18n.Get("item.wrappedgift.name");
			set {}
		}

		public override int Stack {
			get => 1;
			set {}
		}

		public GiftItem() : base()
		{
			// Item values
			this.Name = ModEntry.ItemPrefix + ModEntry.GiftItemName;
			this.Category = ModEntry.Definitions?.CategoryNumber ?? -1;
			this.boundingBox.Value = new Rectangle(location: (this.TileLocation * Game1.tileSize).ToPoint(), size: new Point(Game1.tileSize));
			this.Price = ModEntry.Definitions.GiftValue + (this.IsItemInside ? this.ItemInGift.Value.salePrice() : 0);

			// Allow left-click interactions (pickup gift)
			this.Type = "interactive";

			// Net values
			this.Owner = new();
			this.Style = new();
			this.ItemInGift = new();
			this.HitCount = new();

			this.Style.fieldChangeEvent += this.OnStyleChanged;

			this.NetFields.AddFields(
				this.Owner,
				this.Style,
				this.ItemInGift,
				this.HitCount);
		}

		public GiftItem(long owner, string style, Item item) : this()
		{
			// Gift values
			this.Owner.Set(owner);
			this.Style.Set(style);
			this.ItemInGift.Set(item);
			this.HitCount.Set(Game1.random.Next(ModEntry.Definitions.HitCount.First(), ModEntry.Definitions.HitCount.Last() + 1));
		}

		public GiftItem(Vector2 tileLocation, int parentSheetIndex, string Givenname, bool canBeSetDown, bool canBeGrabbed, bool isHoedirt, bool isSpawnedObject) : this() {}

		public GiftItem(int parentSheetIndex, int initialStack, bool isRecipe = false, int price = -1, int quality = 0) : this() {}

		public GiftItem(Vector2 tileLocation, int parentSheetIndex, int initialStack) : this() {}

		private void OnStyleChanged(NetString field, string oldValue, string newValue)
		{
			this._style = ModEntry.Instance.Helper.GameContent.Load<Data.Data>(ModEntry.GameContentDataPath).Styles[newValue];
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
				text: ModEntry.I18n.Get(this.IsOwnerViewing ? "item.wrappedgift.description.owner" : "item.wrappedgift.description.other"),
				whichFont: Game1.smallFont,
				width: this.getDescriptionWidth());
		}

		public override Item getOne()
		{
			return new GiftItem(owner: this.Owner.Value, style: this.Style.Value, item: this.ItemInGift.Value);
		}

		public override int addToStack(Item stack)
		{
			return stack.Stack;
		}

		public override int maximumStackSize()
		{
			return 1;
		}

		public override bool canStackWith(ISalable other)
		{
			return false;
		}

		public override bool CanBuyItem(Farmer who)
		{
			return false;
		}

		public override bool canBeDropped()
		{
			return true;
		}

		public override bool canBeGivenAsGift()
		{
			return true;
		}

		public override bool canBePlacedInWater()
		{
			return false;
		}

		public override bool canBeTrashed()
		{
			return true;
		}

		public override bool canBeShipped()
		{
			return false;
		}

		public override bool canBePlacedHere(GameLocation l, Vector2 tile)
		{
			return base.canBePlacedHere(l: l, tile: tile)
				&& ModEntry.IsLocationAllowed(location: l)
				&& ModEntry.IsTileAllowed(location: l, tile: tile);
		}

		public override bool isPlaceable()
		{
			return true;
		}

		public override bool isPassable()
		{
			return false;
		}

		public override bool ShouldSerializeparentSheetIndex()
		{
			return false;
		}

		public override void updateWhenCurrentLocation(GameTime time, GameLocation environment)
		{
			base.updateWhenCurrentLocation(time, environment);

			// Allow left-click interactions (pickup gift)
			this.health = 0;

			// Shake
			if (this.shakeTimer > 0)
				this.shakeTimer -= time.ElapsedGameTime.Milliseconds;

			// Show/hide gift preview
			float alphaFade = time.ElapsedGameTime.Milliseconds * 0.0005f * ModEntry.Config.GiftPreviewFadeSpeed;
			if (this.IsTooltipActive)
				this._tooltipAlpha = Math.Min(1, this._tooltipAlpha + alphaFade);
			else
				this._tooltipAlpha = Math.Max(0, this._tooltipAlpha - alphaFade);
		}

		public override bool checkForAction(Farmer who, bool justCheckingForActivity = false)
		{
			if (this.isTemporarilyInvisible || justCheckingForActivity || who is null)
				return true;

			// Right-click interactions (unwrap gift)
			if (this.IsItemInside)
			{
				Rectangle source = this._style.Area;
				Rectangle target = this.getBoundingBox(this.TileLocation);
				int count = Game1.random.Next(4, 8);

				// Chunks (paper)
				this.shakeTimer = ModEntry.Definitions.HitShake;
				Game1.createRadialDebris(
					location: who.currentLocation,
					texture: this._style.Texture ?? ModEntry.GameContentGiftTexturePath,
					sourcerectangle: new Rectangle(
						location: source.Location,
						size: source.Size),
					sizeOfSourceRectSquares: source.Width / 7 * 3,
					xPosition: target.Center.X,
					yPosition: target.Center.Y,
					numberOfChunks: count,
					groundLevelTile: (int)this.TileLocation.Y,
					color: Colour.White,
					scale: Game1.pixelZoom);
				who.currentLocation.playSoundAt(
					audioName: this._style.HitSound ?? ModEntry.Definitions.HitSound,
					position: who.getTileLocation());

				this.HitCount.Set(this.HitCount.Value - 1);
				if (this.HitCount.Value < 1)
				{
					// Chunks (box)
					Game1.createRadialDebris(
						location: who.currentLocation,
						debrisType: 12,
						xTile: (int)this.TileLocation.X,
						yTile: (int)this.TileLocation.Y,
						numberOfChunks: Game1.random.Next(4, 8),
						resource: false);
					who.currentLocation.playSoundAt(
						audioName: this._style.LastHitSound ?? ModEntry.Definitions.LastHitSound,
						position: who.getTileLocation());

					// Sparkles
					var sprite = new TemporaryAnimatedSprite(
						textureName: "TileSheets/animations",
						sourceRect: new Rectangle(0, 640, 64, 64),
						animationInterval: 100,
						animationLength: 8,
						numberOfLoops: 0,
						position: this.TileLocation * Game1.tileSize,
						flicker: false,
						flipped: false);
					who.currentLocation.TemporarySprites.Add(sprite);
					Game1.playSound(this._style.OpenSound ?? ModEntry.Definitions.OpenSound);

					// Give gift to player
					who.currentLocation.debris.Add(new Debris(
						item: this.ItemInGift.Value,
						debrisOrigin: this.TileLocation * Game1.tileSize,
						targetLocation: Game1.player.GetBoundingBox().Center.ToVector2()));

					// Remove gift from world
					this.ItemInGift.Set(null);
					who.currentLocation.Objects.Remove(this.TileLocation);
					return true;
				}
			}

			return true;
		}

		public override bool performToolAction(Tool t, GameLocation location)
		{
			if (this.isTemporarilyInvisible || t is null || location is null)
				return false;

			// Left-click interactions (pickup gift)
			location.debris.Add(new Debris(
				item: this.getOne(),
				debrisOrigin: this.TileLocation * Game1.tileSize,
				targetLocation: Game1.player.GetBoundingBox().Center.ToVector2()));

			Game1.playSound(this._style.RemoveSound ?? ModEntry.Definitions.RemoveSound);

			return true;
		}

		public override bool performUseAction(GameLocation location)
		{
			return base.performUseAction(location);
		}

		public override void performRemoveAction(Vector2 tileLocation, GameLocation environment)
		{
			base.performRemoveAction(tileLocation, environment);
		}

		public override Point getExtraSpaceNeededForTooltipSpecialIcons(SpriteFont font, int minWidth, int horizontalBuffer, int startingHeight, StringBuilder descriptionText, string boldTitleText, int moneyAmountToDisplayAtBottom)
		{
			Point dimensions = base.getExtraSpaceNeededForTooltipSpecialIcons(
				font: font,
				minWidth: minWidth,
				horizontalBuffer: horizontalBuffer,
				startingHeight: startingHeight,
				descriptionText: descriptionText,
				boldTitleText: boldTitleText,
				moneyAmountToDisplayAtBottom: moneyAmountToDisplayAtBottom);

			string text = this.IsOwnerViewing && this.IsItemInside
				? this.ItemInGift.Value.DisplayName
				: Game1.getFarmerMaybeOffline(this.Owner.Value) is Farmer owner
					? owner.displayName
					: null;
			
			if (text is not null)
			{
				dimensions.X = (int)Math.Max(minWidth, Game1.tileSize * 2 + font.MeasureString(text).X);
				dimensions.Y = startingHeight + Game1.tileSize + 8;
			}

			return dimensions;
		}

		public override void drawTooltip(SpriteBatch spriteBatch, ref int x, ref int y, SpriteFont font, float alpha, StringBuilder overrideText)
		{
			base.drawTooltip(
				spriteBatch: spriteBatch,
				x: ref x,
				y: ref y,
				font: font,
				alpha: alpha,
				overrideText: overrideText);

			float scale = 1;
			string text = null;
			Colour colour = ModEntry.Definitions.DefaultTextColour ?? Game1.textColor;
			Vector2 position = new Vector2(x: x, y: y)
				+ new Vector2(Game1.smallestTileSize)
				+ new Vector2(4);
			if (this.IsOwnerViewing && this.IsItemInside)
			{
				// Peek wrapped item if viewed by owner
				this.ItemInGift.Value.drawInMenu(
					spriteBatch: spriteBatch,
					location: position,
					scaleSize: scale);
				text = this.ItemInGift.Value.DisplayName;
				colour = this.ItemInGift.Value.getCategoryColor();
			}
			else if (Game1.getFarmerMaybeOffline(this.Owner.Value) is Farmer owner)
			{
				// Show owner
				owner.FarmerRenderer.drawMiniPortrat(
					b: spriteBatch,
					layerDepth: 1,
					position: position,
					scale: scale * Game1.pixelZoom,
					facingDirection: Game1.down,
					who: owner);
				text = owner.displayName;
			}

			if (text is not null)
			{
				Utility.drawTextWithShadow(
					b: spriteBatch,
					text: text,
					font: font,
					position: position + new Vector2(x: Game1.tileSize * 1.25f, y: Game1.tileSize * 0.25f),
					color: colour * alpha);
			}
		}

		public override void drawInMenu(SpriteBatch spriteBatch, Vector2 location, float scaleSize, float transparency, float layerDepth, StackDrawType drawStackNumber, Color color, bool drawShadow)
		{
			if (this._style is null)
				return;

			Vector2 origin = this._style.Area.Size.ToVector2() / 2;
			float scale = Game1.pixelZoom;

			spriteBatch.Draw(
				texture: ModEntry.GetStyleTexture(this._style),
				position: location + origin * scale,
				sourceRectangle: this._style.Area,
				color: color * transparency,
				rotation: 0,
				origin: origin,
				scale: scale * 0.75f * scaleSize,
				effects: SpriteEffects.None,
				layerDepth: layerDepth);
		}

		public override void draw(SpriteBatch spriteBatch, int x, int y, float alpha = 1)
		{
			Vector2 position = new Vector2(x: x, y: y) * Game1.tileSize;
			float layerDepth = ((y + 1) * Game1.tileSize) / 10000f + this.TileLocation.X / 50000f;
			this.DrawGiftItem(
				b: spriteBatch,
				globalX: (int)position.X,
				globalY: (int)position.Y,
				alpha: alpha,
				layerDepth: layerDepth + 1E-06f,
				drawShadow: false,
				isInWorld: true);

			if (this._tooltipAlpha <= 0 || !this.IsItemInside)
				return;

			float yOffset = Game1.pixelZoom * (float)Math.Round(Math.Sin(Game1.currentGameTime.TotalGameTime.TotalMilliseconds / 250), 2);
			alpha = this._tooltipAlpha;
			position = new Vector2(
					x: x * Game1.tileSize,
					y: ((y - 1.5f) * Game1.tileSize) + yOffset)
				- Game1.viewport.ToXna().Location.ToVector2();

			// Bubble
			spriteBatch.Draw(
				texture: Game1.mouseCursors,
				position: position - new Vector2(x: 8, y: 10),
				sourceRectangle: new Rectangle(141, 465, 20, 24),
				color: Colour.White * alpha * 0.8f,
				rotation: 0,
				origin: Vector2.Zero,
				scale: Game1.pixelZoom,
				effects: SpriteEffects.None,
				layerDepth: layerDepth + 1E-06f);

			// Item
			this.ItemInGift.Value.drawInMenu(
				spriteBatch: spriteBatch,
				location: position,
				scaleSize: 1,
				transparency: alpha * 0.95f,
				layerDepth: layerDepth + 1E-05f);
		}

		public override void draw(SpriteBatch spriteBatch, int xNonTile, int yNonTile, float layerDepth, float alpha = 1)
		{
			this.DrawGiftItem(
				b: spriteBatch,
				globalX: xNonTile,
				globalY: yNonTile,
				alpha: alpha,
				layerDepth: layerDepth,
				isInWorld: true);
		}

		public override void drawWhenHeld(SpriteBatch spriteBatch, Vector2 objectPosition, Farmer f)
		{
			this.DrawGiftItem(
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
			this.DrawGiftItem(
				b: b,
				globalX: (int)position.X,
				globalY: (int)position.Y,
				layerDepth: this.getBoundingBox(this.TileLocation).Bottom / 10000f);
		}

		public void DrawGiftItem(SpriteBatch b, int globalX, int globalY, float alpha = 1, float layerDepth = 1, bool drawShadow = true, bool isInWorld = false)
		{
			if (this._style is null || this.isTemporarilyInvisible)
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
			source = this._style.Area;
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
				texture: ModEntry.GetStyleTexture(this._style),
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
