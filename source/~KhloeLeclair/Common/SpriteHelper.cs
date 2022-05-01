/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/KhloeLeclair/StardewMods
**
*************************************************/

#nullable enable

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using Leclair.Stardew.Common.Enums;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewModdingAPI;

using StardewValley;
using StardewValley.Menus;
using StardewValley.Objects;
using StardewValley.Projectiles;
using StardewValley.Tools;

using SObject = StardewValley.Object;

namespace Leclair.Stardew.Common;

public static class SpriteHelper {

	// Helper
	private static IModHelper? Helper;

	[MemberNotNull(nameof(Helper))]
	public static void SetHelper(IModHelper helper) {
		Helper = helper;
	}

	// Shared Textures

	public static Texture2D? KeyTexture { get; private set; } = null;
	private readonly static object loadLock = new();

	[MemberNotNull(nameof(KeyTexture))]
	public static void LoadKeyTexture() {
		lock (loadLock) {
			if (KeyTexture == null)
				KeyTexture = Helper!.Content.Load<Texture2D>("assets/keys.png");
				//KeyTexture = Helper!.ModContent.Load<Texture2D>("assets/keys.png");
		}
	}

	public static class Tabs {
		public static readonly Rectangle BACKGROUND = new(16, 368, 16, 16);
	}

	public static class MouseIcons {
		public static readonly Rectangle BACKPACK = new(268, 1436, 11, 13);
	}

	public static class MouseIcons2 {
		public static readonly Rectangle GOLDEN_NUT = new(0, 240, 16, 16);
	}

	internal static void DrawBounded(this ClickableTextureComponent cmp, SpriteBatch b, Color? color = null, float? layerDepth = null, int frameOffset = 0) {
		if (!cmp.visible || cmp.texture == null)
			return;

		Rectangle source = cmp.sourceRect;
		if (frameOffset != 0)
			source = new Rectangle(source.X + source.Width * frameOffset, source.Y, source.Width, source.Height);

		float width = source.Width;
		float height = source.Height;

		float bWidth = cmp.bounds.Width;
		float bHeight = cmp.bounds.Height;

		float s = Math.Min(cmp.baseScale, Math.Min(bWidth / width, bHeight / height));
		float offsetScale = cmp.scale - cmp.baseScale;

		Point middle = cmp.bounds.Center;
		Vector2 pos = new(middle.X, middle.Y);
		Vector2 center = new(source.Width / 2, source.Height / 2);
		Color c = color ?? Color.White;
		float depth = layerDepth ?? GUIHelper.GetLayerDepth(pos.Y);

		if (cmp.drawShadow)
			Utility.drawWithShadow(b, cmp.texture, pos, source, c, 0f, center, s + offsetScale, layerDepth: depth);
		else
			b.Draw(cmp.texture, pos, source, c, 0f, center, s + offsetScale, SpriteEffects.None, depth);
	}

	public static Texture2D? GetTexture(GameTexture? tex) {
		switch (tex) {
			// Game1
			case GameTexture.Concessions:
				return Game1.concessionsSpriteSheet;
			case GameTexture.Birds:
				return Game1.birdsSpriteSheet;
			case GameTexture.DayBG:
				return Game1.daybg;
			case GameTexture.NightBG:
				return Game1.nightbg;
			case GameTexture.Menu:
				return Game1.menuTexture;
			case GameTexture.UncoloredMenu:
				return Game1.uncoloredMenuTexture;
			case GameTexture.Lantern:
				return Game1.lantern;
			case GameTexture.WindowLight:
				return Game1.windowLight;
			case GameTexture.SconceLight:
				return Game1.sconceLight;
			case GameTexture.CauldronLight:
				return Game1.cauldronLight;
			case GameTexture.IndoorWindowLight:
				return Game1.indoorWindowLight;
			case GameTexture.Shadow:
				return Game1.shadowTexture;
			case GameTexture.MouseCursors:
				return Game1.mouseCursors;
			case GameTexture.MouseCursors2:
				return Game1.mouseCursors2;
			case GameTexture.Giftbox:
				return Game1.giftboxTexture;
			case GameTexture.ControllerMap:
				return Game1.controllerMaps;
			case GameTexture.Animations:
				return Game1.animations;
			case GameTexture.Object:
				return Game1.objectSpriteSheet;
			case GameTexture.Crop:
				return Game1.cropSpriteSheet;
			case GameTexture.Emote:
				return Game1.emoteSpriteSheet;
			case GameTexture.Debris:
				return Game1.debrisSpriteSheet;
			case GameTexture.BigCraftable:
				return Game1.bigCraftableSpriteSheet;
			case GameTexture.Rain:
				return Game1.rainTexture;
			case GameTexture.Buffs:
				return Game1.buffsIcons;
			case GameTexture.Greenhouse:
				return Game1.greenhouseTexture;
			case GameTexture.Barn:
				return Game1.currentBarnTexture;
			case GameTexture.Coop:
				return Game1.currentCoopTexture;
			case GameTexture.House:
				return Game1.currentHouseTexture;
			case GameTexture.Mailbox:
				return Game1.mailboxTexture;
			case GameTexture.TVStation:
				return Game1.tvStationTexture;
			case GameTexture.Cloud:
				return Game1.cloud;
			case GameTexture.Tool:
				return Game1.toolSpriteSheet;

			// Farmer
			case GameTexture.HairStyles:
				return FarmerRenderer.hairStylesTexture;
			case GameTexture.Shirts:
				return FarmerRenderer.shirtsTexture;
			case GameTexture.Pants:
				return FarmerRenderer.pantsTexture;
			case GameTexture.Hats:
				return FarmerRenderer.hatsTexture;
			case GameTexture.Accessories:
				return FarmerRenderer.accessoriesTexture;

			// Furniture
			case GameTexture.Furniture:
				return Furniture.furnitureTexture;
			case GameTexture.FurnitureFront:
				return Furniture.furnitureFrontTexture;

			// Misc
			case GameTexture.Chair:
				return MapSeat.mapChairTexture;
			case GameTexture.Weapon:
				return Tool.weaponsTexture;
			case GameTexture.Projectile:
				return Projectile.projectileSheet;
			case GameTexture.Wallpaper:
				return Game1.content.Load<Texture2D>("Maps\\walls_and_floors");
			case GameTexture.Emoji:
				if (ChatBox.emojiTexture == null)
					ChatBox.emojiTexture = Game1.content.Load<Texture2D>("LooseSprites\\emojis");
				return ChatBox.emojiTexture;
		}

		return null;
	}

	public static SpriteInfo? GetSprite(SButton button) { 
		Texture2D texture = Game1.mouseCursors;
		Rectangle source;

		if (Helper == null && KeyTexture == null)
			return null;

		switch (button) {
			case SButton.MouseLeft:
				LoadKeyTexture();
				texture = KeyTexture;
				source = Game1.getSourceRectForStandardTileSheet(texture, 0, 11, 11);
				break;

			case SButton.MouseMiddle:
				LoadKeyTexture();
				texture = KeyTexture;
				source = Game1.getSourceRectForStandardTileSheet(texture, 2, 11, 11);
				break;

			case SButton.MouseRight:
				LoadKeyTexture();
				texture = KeyTexture;
				source = Game1.getSourceRectForStandardTileSheet(texture, 1, 11, 11);
				break;

			case SButton.ControllerA:
				source = Game1.getSourceRectForStandardTileSheet(texture, 45, 16, 16);
				source.Width = source.Height = 11;
				break;

			case SButton.ControllerX:
				source = Game1.getSourceRectForStandardTileSheet(texture, 46, 16, 16);
				source.Width = source.Height = 11;
				break;

			case SButton.ControllerB:
				source = Game1.getSourceRectForStandardTileSheet(texture, 47, 16, 16);
				source.Width = source.Height = 11;
				break;

			case SButton.ControllerY:
				source = Game1.getSourceRectForStandardTileSheet(texture, 48, 16, 16);
				source.Width = source.Height = 11;
				break;

			case SButton.ControllerBack:
				LoadKeyTexture();
				texture = KeyTexture;
				source = Game1.getSourceRectForStandardTileSheet(texture, 3, 11, 11);
				break;

			case SButton.ControllerStart:
				LoadKeyTexture();
				texture = KeyTexture;
				source = Game1.getSourceRectForStandardTileSheet(texture, 4, 11, 11);
				break;

			default:
				return null;
		}

		return new SpriteInfo(texture, source);
	}


	#region Stupid DynamicGameAssets Reflection

	private static SpriteInfo? GetDGACustomObjectSprite(Item item) {
		object? Data = Helper?.Reflection.GetProperty<object>(item, "Data", required: false)?.GetValue();
		if (Data == null)
			return null;

		object? Tex = Helper?.Reflection.GetMethod(Data, "GetTexture", required: false)?.Invoke<object>();
		if (Tex == null)
			return null;

		Texture2D? texture = Helper?.Reflection.GetProperty<Texture2D>(Tex, "Texture", required: false)?.GetValue();
		Rectangle? rect = Helper?.Reflection.GetProperty<Rectangle?>(Tex, "Rect", required: false)?.GetValue();

		if (texture == null)
			return null;

		return new SpriteInfo(
			texture,
			rect ?? texture.Bounds
		);
	}

	#endregion

	#region Stupid RaisedGardenBeds Reflection

	private static Type? RGBEntry = null;

	private static Texture2D? GetRGBTexture(string spriteKey) {
		if (RGBEntry == null) {
			RGBEntry = Type.GetType("RaisedGardenBeds.ModEntry, RaisedGardenBeds");
			if (RGBEntry == null)
				return null;
		}

		var dict = Helper?.Reflection.GetField<Dictionary<string, Texture2D>>(RGBEntry, "Sprites", required: false)?.GetValue();
		if (dict == null)
			return null;

		dict.TryGetValue(spriteKey, out Texture2D? result);
		return result;
	}

	private static SpriteInfo? GetGardenPotSprite(Item item) {
		Type type = item.GetType();
		if (type.ToString() != "RaisedGardenBeds.OutdoorPot")
			return null;

		string? spriteKey = Helper?.Reflection.GetProperty<string>(item, "SpriteKey", required: false)?.GetValue();
		int spriteIndex = Helper?.Reflection.GetProperty<int>(item, "SpriteIndex", required: false)?.GetValue() ?? -1;
		IReflectedMethod? method = Helper?.Reflection.GetMethod(type, "GetSpriteSourceRectangle", required: false);

		if (!string.IsNullOrEmpty(spriteKey) && method != null) {
			Texture2D? tex = GetRGBTexture(spriteKey);
			Rectangle? source = null;
			try {
				source = method.Invoke<Rectangle>(spriteIndex, false);
			} catch (Exception) { /* ignore */ }

			if (tex != null && source.HasValue)
				return new SpriteInfo(
					texture: tex,
					baseSource: source.Value
				);
		}

		return null;
	}

	#endregion

	public static SpriteInfo? GetSprite(Item? item) {
		if (item is null)
			return null;

		int tileSize = SObject.spriteSheetTileSize;
		Type type = item.GetType();
		string ts = type.ToString();

		// DynamicGameAssets Compatibility
		if (ts.Equals("DynamicGameAssets.Game.CustomObject")) {
			SpriteInfo? sprite = GetDGACustomObjectSprite(item);
			if (sprite is not null)
				return sprite;
		}

		// RaisedGardenBed Compatibility
		if (ts.Equals("RaisedGardenBeds.OutdoorPot")) {
			SpriteInfo? sprite = GetGardenPotSprite(item);
			if (sprite is not null)
				return sprite;
		}

		// Furniture
		if (item is Furniture furniture) {
			Texture2D texture = Furniture.furnitureTexture;

			if (ts.Equals("CustomFurniture.CustomFurniture")) {
				// TODO: More advanced furniture with layers.
				if (Helper != null)
					texture = Helper.Reflection.GetField<Texture2D>(item, "texture", required: false)?.GetValue() ?? texture;
			}

			return new SpriteInfo(
				texture,
				furniture.defaultSourceRect.Value
			);
		}

		// Fence
		if (item is Fence fence) {
			Rectangle source;
			int drawSum = fence.getDrawSum(Game1.currentLocation);
			int tile = Fence.fenceDrawGuide[drawSum];
			bool gate = fence.isGate.Value;

			if (gate && drawSum == 110)
				source = new Rectangle(0, 512, 88, 24);
			else if (gate && drawSum == 1500)
				source = new Rectangle(112, 512, 16, 64);
			else
				source = Game1.getArbitrarySourceRect(fence.fenceTexture.Value, 64, 128, tile);

			return new SpriteInfo(
				fence.fenceTexture.Value,
				source
			);
		}

		// Wallpaper
		if (item is Wallpaper wallpaper) {
			// Logic taken from the Wallpaper drawInMenu method.
			var moddata = wallpaper.GetModData();
			Texture2D? texture = null;
			if (moddata != null) {
				try {
					texture = Game1.content.Load<Texture2D>(moddata.Texture);
				} catch (Exception) { /* no-op */ }
			}

			if (texture == null)
				texture = Game1.content.Load<Texture2D>("Maps\\walls_and_floors");

			Rectangle source;

			if (Helper == null)
				source = Rectangle.Empty;
			else if (wallpaper.isFloor.Value)
				source = Helper.Reflection.GetField<Rectangle>(typeof(Wallpaper), "floorContainerRect").GetValue();
			else
				source = Helper.Reflection.GetField<Rectangle>(typeof(Wallpaper), "wallpaperContainerRect").GetValue();

			return new SpriteInfo(
				texture: Game1.mouseCursors2,
				baseSource: source,
				overlayTexture: texture,
				overlaySource: wallpaper.sourceRect.Value,
				overlayScale: 0.5f
			);
		}

		// Colored Object
		if (item is ColoredObject colored) {
			int idx = colored.ParentSheetIndex;
			Color color = colored.color.Value;

			if (colored.bigCraftable.Value)
				return new SpriteInfo(
					texture: Game1.bigCraftableSpriteSheet,
					baseSource: SObject.getSourceRectForBigCraftable(idx),
					overlaySource: SObject.getSourceRectForBigCraftable(idx + 1),
					overlayColor: color
				);

			int offset = colored.ColorSameIndexAsParentSheetIndex ? 0 : 1;

			return new SpriteInfo(
				texture: Game1.objectSpriteSheet,
				baseSource: Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, idx, tileSize, tileSize),
				overlaySource: Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, idx + offset, tileSize, tileSize),
				overlayColor: color
			);
		}

		// Objects
		if (item is SObject obj) {
			if (obj.bigCraftable.Value)
				return new SpriteInfo(Game1.bigCraftableSpriteSheet, SObject.getSourceRectForBigCraftable(obj.ParentSheetIndex));

			return new SpriteInfo(
				Game1.objectSpriteSheet,
				Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, obj.ParentSheetIndex, tileSize, tileSize)
			);
		}

		// Weapons
		if (item is MeleeWeapon weapon) {
			return new SpriteInfo(
				Tool.weaponsTexture,
				Game1.getSquareSourceRectForNonStandardTileSheet(Tool.weaponsTexture, 16, 16, weapon.IndexOfMenuItemView)
			);
		}

		// Tools
		if (item is Tool tool) {
			return new SpriteInfo(
				Game1.toolSpriteSheet,
				Game1.getSquareSourceRectForNonStandardTileSheet(Game1.toolSpriteSheet, 16, 16, tool.IndexOfMenuItemView)
			);
		}

		// Combined Ring
		if (item is CombinedRing ring) {
			// CombinedRings are crazy.
			if (ring.combinedRings.Count > 1)
				return new CombinedRingSpriteInfo(ring);
		}

		// Boots and Rings
		if (item is Boots || item is Ring) {
			int idx;
			if (item is Boots boots)
				idx = boots.indexInTileSheet.Value;
			else
				idx = ((Ring) item).indexInTileSheet.Value;

			return new SpriteInfo(
				Game1.objectSpriteSheet,
				Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, idx, tileSize, tileSize)
			);
		}

		// Clothing
		if (item is Clothing clothing) {
			int idx;

			switch (clothing.clothesType.Value) {
				case (int) Clothing.ClothesType.SHIRT:
					idx = clothing.indexInTileSheetMale.Value;
					Rectangle source = new(
						idx * 8 % 128,
						idx * 8 / 128 * 32,
						8, 8
					);

					return new SpriteInfo(
						FarmerRenderer.shirtsTexture,
						source,
						overlaySource: new Rectangle(
							source.X + 128, source.Y,
							source.Width, source.Height
						),
						overlayColor: clothing.clothesColor.Value,
						isPrismatic: clothing.isPrismatic.Value
					);

				case (int) Clothing.ClothesType.PANTS:
					int tiles = FarmerRenderer.pantsTexture.Width / 192;
					idx = clothing.indexInTileSheetMale.Value;

					return new SpriteInfo(
						FarmerRenderer.pantsTexture,
						new Rectangle(
							192 * (idx % tiles),
							688 * (idx / tiles) + 672,
							16, 16
						),
						baseColor: clothing.clothesColor.Value,
						isPrismatic: clothing.isPrismatic.Value
					);
			}
		}

		// Hat
		if (item is Hat hat) {
			int idx = hat.which.Value;

			return new SpriteInfo(
				FarmerRenderer.hatsTexture,
				new Rectangle(
					idx * 20 % FarmerRenderer.hatsTexture.Width,
					idx * 20 / FarmerRenderer.hatsTexture.Width * 20 * 4,
					20, 20
				)
			);
		}

		// Unknown
		return null;
	}


}
