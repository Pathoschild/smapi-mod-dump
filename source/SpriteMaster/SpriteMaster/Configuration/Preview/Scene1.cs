/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

using GenericModConfigMenu;
using Microsoft.Xna.Framework;
using SpriteMaster.Extensions;
using SpriteMaster.Extensions.Reflection;
using SpriteMaster.Types;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace SpriteMaster.Configuration.Preview;

internal sealed class Scene1 : Scene {
	//private static readonly Lazy<XTexture2D> FishTexture = new(() => StardewValley.Game1.content.Load<XTexture2D>(@"Maps\springobjects"));

	private readonly struct CharacterReference : IDisposable {
		internal readonly AnimatedTexture Sprite;
		internal readonly AnimatedTexture Portrait;
		internal readonly Drawable Drawable;
		internal readonly string Name;
		internal readonly string PortraitName;
		
		internal CharacterReference(AnimatedTexture sprite, AnimatedTexture portrait, Drawable drawable, string name, string? portraitNameOverride) {
			Sprite = sprite;
			Portrait = portrait;
			Drawable = drawable;
			Name = name;
			PortraitName = portraitNameOverride ?? name;
		}

		internal CharacterReference(in TextureReference textureRef) : this(
			textureRef.Sprite, textureRef.Portrait, new(textureRef.Portrait), textureRef.Name, textureRef.PortraitName
		) {

		}

		public void Dispose() {
			Sprite.Dispose();
			Portrait.Dispose();
		}
	}

	private readonly CharacterReference CenterCharacter;

	private readonly Stopwatch DrawStopwatch = new();

	private readonly SpriteSheet OutdoorTiles;

	private readonly UniformAnimatedTexture OutdoorTilesWeed;

	private readonly UniformAnimatedTexture OutdoorTilesFlower;

	private readonly SpriteSheet TreeTexture;

	private DrawableInstance[] Drawables;

	private Vector2I TileCount = default;

	internal override PrecipitationType Precipitation { get; }

	private static int RandomSeed => Guid.NewGuid().GetHashCode();

	[DoesNotReturn]
	[MethodImpl(MethodImplOptions.NoInlining)]
	private static ref T ThrowArrayEmptyException<T>(string name) =>
		throw new ArgumentException($"{name} is empty");

	private static ref T GetRandomOf<T>(Random rand, T[] array) {
		if (array.Length == 0) {
			return ref ThrowArrayEmptyException<T>(nameof(array));
		}

		return ref array[rand.Next(array.Length)];
	}

	private const string KiwiPath = @"Characters\Kiwi";

	private static AnimatedTexture? GetCenterCharacterRSVKiwi() {
		try {
			return new UniformAnimatedTexture(
				textureName: KiwiPath,
				spriteSize: (16, 32),
				spriteOffset: (0, 4),
				spritesPerRow: 4,
				numSprites: 12,
				ticksPerFrame: 6
			);
		}
		catch {
			return null;
		}
	}

	private const string JunimoGoldenPath = @"Characters\JunimoGolden";

	private static AnimatedTexture? GetCenterCharacterESJunimoGolden() {
		try {
			return new UniformExplicitAnimatedTexture(
				textureName: JunimoGoldenPath,
				spriteSize: (16, 32),
				spriteOffset: (0, 4),
				spritesPerRow: 4,
				spriteIndices: new Vector2I[] {
					(1, 0),
					(0, 0),
					(3, 0),
					(2, 0),
					(1, 0)
				},
				ticksPerFrame: 6
			);
		}
		catch {
			return null;
		}
	}

	private const string JunimoPath = @"Characters\Junimo";

	private static AnimatedTexture? GetCenterCharacterJunimo() {
		try {
			return new UniformAnimatedTexture(
				textureName: JunimoPath,
				spriteSize: (16, 16),
				spriteOffset: (0, 0),
				spritesPerRow: 8,
				numSprites: 48,
				ticksPerFrame: 6
			);
		}
		catch {
			return null;
		}
	}

	private static AnimatedTexture? MakeCharacterSpriteTexture(string path) {
		try {
			return new UniformExplicitAnimatedTexture(
				textureName: path,
				spriteSize: (16, 32),
				spriteOffset: (0, 9),
				spritesPerRow: 4,
				spriteIndices: new Vector2I[] {
					//(0, 0),
					(0, 1),
					(1, 1),
					(0, 1),
					(2, 1),
					(0, 1),
					(3, 1),
					(3, 2),
					(2, 2),
					(3, 2),
					(3, 1),
					(0, 1)
				},
				ticksPerFrame: 24
			);
		}
		catch {
			return null;
		}
	}

	private static AnimatedTexture? MakeCharacterPortraitTexture(string path) {
		try {
			return new UniformExplicitAnimatedTexture(
				textureName: path,
				spriteSize: (64, 64),
				spriteOffset: (0, 0),
				spritesPerRow: 2,
				spriteIndices: new Vector2I[] {
					(0, 0),
					(1, 0),
					(0, 1),
					(1, 1)
				},
				ticksPerFrame: 24
			);
		}
		catch {
			return null;
		}
	}

	private static TextureReference? MakeCharacterTextures(in PathReference paths) {
		var sprite = MakeCharacterSpriteTexture(paths.Sprite);
		var portrait = MakeCharacterPortraitTexture(paths.Portrait);
		try {
			if (sprite is null || portrait is null) {
				sprite?.Dispose();
				portrait?.Dispose();
				return null;
			}

			return new(sprite, portrait, paths.Name, paths.PortraitName);
		}
		catch {
			sprite?.Dispose();
			portrait?.Dispose();
			return null;
		}
	}

	private static TextureReference? MakeCharacterTextures(AnimatedTexture? sprite, in PathReference paths) {
		if (sprite is null) {
			return null;
		}

		var portrait = MakeCharacterPortraitTexture(paths.Portrait);
		try {
			if (portrait is null) {
				sprite.Dispose();
				portrait?.Dispose();
				return null;
			}

			return new(sprite, portrait, paths.Name, paths.PortraitName);
		}
		catch {
			sprite.Dispose();
			portrait?.Dispose();
			return null;
		}
	}

	private readonly record struct TextureReference(
		AnimatedTexture Sprite,
		AnimatedTexture Portrait,
		string Name,
		string? PortraitName = null
	);

	private readonly record struct PathReference(
		string Sprite,
		string Portrait,
		string Name,
		string? PortraitName = null
	) {
		internal static PathReference CreateDefault(string name) {
			return new($@"Characters\{name}", $@"Portraits\{name}", name);
		}
	}

	[SuppressMessage("ReSharper", "StringLiteralTypo")]
	private static readonly PathReference[] CharacterPaths = {
		// vanilla
		PathReference.CreateDefault(@"Penny"),
		PathReference.CreateDefault(@"Haley"),
		PathReference.CreateDefault(@"Abigail"),
		PathReference.CreateDefault(@"Maru"),
		PathReference.CreateDefault(@"Leah"),
		// sve
		PathReference.CreateDefault(@"Alesia"),
		PathReference.CreateDefault(@"Claire"),
		PathReference.CreateDefault(@"Olivia"),
		PathReference.CreateDefault(@"Sophia"),
		// rsv
		PathReference.CreateDefault(@"Alissa"),
		PathReference.CreateDefault(@"Corine"),
		PathReference.CreateDefault(@"Daia"),
		PathReference.CreateDefault(@"Flor"),
		PathReference.CreateDefault(@"Maddie"),
		PathReference.CreateDefault(@"Ysabelle"),
		// es
		PathReference.CreateDefault(@"Aideen"),
		// rsv kiwi
		new(KiwiPath, @"Portraits\Kiwi", "Kiwi"),
		// es junimo golden
		new(JunimoGoldenPath, @"Portraits\Clint", "GoldenJunimo", "Clint"),
		// vanilla junimo
		new(JunimoPath, @"Portraits\Clint", "Junimo", "Clint"),
	};

	private static CharacterReference GetCenterCharacter() {
		var rand = new Random(RandomSeed);
		var characters = CharacterPaths.OrderBy(_ => rand.Next());

		foreach (var character in characters) {
			TextureReference? result = character switch {
				(KiwiPath, _, _, _) => MakeCharacterTextures(GetCenterCharacterRSVKiwi(), character),
				(JunimoGoldenPath, _, _, _) => MakeCharacterTextures(GetCenterCharacterESJunimoGolden(), character),
				(JunimoPath, _, _, _) => MakeCharacterTextures(GetCenterCharacterJunimo(), character),
				_ => MakeCharacterTextures(character)
			};
			if (result is {} textureReference) {
				return new(textureReference);
			}
		}

		return new(MakeCharacterTextures(CharacterPaths[0])!.Value);
	}

	private static readonly string[] Seasons = {
		"spring",
		"summer",
		"fall",
		"winter"
	};

	internal Scene1(Bounds scissor) : base(scissor) {
		CenterCharacter = GetCenterCharacter();

		var rand = new Random(RandomSeed);

		Season = Seasons[rand.Next(Seasons.Length)];

		Precipitation = Season == "winter" ? PrecipitationType.Snow : PrecipitationType.Rain;

		string outdoorsTileSheet = $@"Maps\{Season}_outdoorsTileSheet";

		OutdoorTiles = new(
			textureName: outdoorsTileSheet,
			spriteSize: (16, 16)
		);

		if (Season == "winter") {
			OutdoorTilesWeed = new(
				textureName: outdoorsTileSheet,
				spriteSize: (16, 16),
				spriteOffset: (0, 12),
				spritesPerRow: 4,
				numSprites: 1,
				ticksPerFrame: 12
			);

			OutdoorTilesFlower = new(
				textureName: outdoorsTileSheet,
				spriteSize: (16, 16),
				spriteOffset: (0, 12),
				spritesPerRow: 4,
				numSprites: 1,
				ticksPerFrame: 12
			);
		}
		else {
			OutdoorTilesWeed = new(
				textureName: outdoorsTileSheet,
				spriteSize: (16, 16),
				spriteOffset: (20, 16),
				spritesPerRow: 4,
				numSprites: 4,
				ticksPerFrame: 12
			);

			OutdoorTilesFlower = new(
				textureName: outdoorsTileSheet,
				spriteSize: (16, 16),
				spriteOffset: (1, 6),
				spritesPerRow: 2,
				numSprites: 2,
				ticksPerFrame: 12
			);
		}

		TreeTexture = new(
			textureName: outdoorsTileSheet,
			spriteSize: (48, 96)
		);

		Drawables = SetupScene(Season == "winter");
	}

	public override void Dispose() {
		CenterCharacter.Dispose();
	}

	private const string ReferenceBasicText = "Llanfairpwllgwyngyllgogerychwyrndrobwllllantysiliogogogoch";
	private static XGraphics.SpriteFont BasicTextFont => Game1.dialogueFont;
	private const string ReferenceUtilityText = "It was the best of times, it was the blurst of times.";
	private static XGraphics.SpriteFont UtilityTextFont => Game1.smallFont;

	private void DrawStringStroked(
		XSpriteBatch batch,
		XGraphics.SpriteFont font,
		string text,
		Vector2I position,
		XColor color
	) {
		var strokeColor = new XColor(
			~color.R,
			~color.G,
			~color.B,
			color.A
		);

		for (int y = -1; y <= 1; ++y) {
			for (int x = -1; x <= 1; ++x) {
				batch.DrawString(
					font,
					text,
					position + (x, y),
					strokeColor
				);
			}
		}

		batch.DrawString(
			font,
			text,
			position,
			color
		);
	}

	protected override void OnDraw(XSpriteBatch batch, in Override overrideState) {
		float lastLayerDepth = float.NaN;
		int index = 0;
		foreach (var drawable in Drawables) {
			drawable.Tick();
			if (drawable.LayerDepth == lastLayerDepth) {
				++index;
			}
			else {
				index = 0;
				lastLayerDepth = drawable.LayerDepth;
			}
			drawable.Draw(this, batch, index);
		}
	}

	private static readonly Lazy<IHDPortraitsAPI?> HdPortraitsApi = new(
		// ReSharper disable once StringLiteralTypo
		() => SpriteMaster.Self.Helper.ModRegistry.GetApi<IHDPortraitsAPI>("tlitookilakin.HDPortraits")
	);

	[Flags]
	private enum PortraitType {
		Vanilla = 1 << 0,
		SeasonLower = 1 << 1,
		SeasonUpper = 1 << 2,
		Plain = 1 << 3,
		Festival = 1 << 4,
		Beach = 1 << 5
	}

	private PortraitType InvalidPortraitTypes;

	protected override void OnDrawOverlay(XSpriteBatch batch, in Override overrideState) {
		{
			// Draw basic text
			DrawStringStroked(
				batch,
				BasicTextFont,
				ReferenceBasicText,
				Vector2I.Zero,
				XColor.White
			);
		}

		var regionOffset = new Vector2F(Region.Extent);

		{
			// Draw sprite text
			var textMeasure = UtilityTextFont.MeasureString(ReferenceUtilityText);

			var offset = regionOffset;
			offset -= (Vector2F)textMeasure;
			offset.X = 0;

			Utility.drawTextWithShadow(
				b: batch,
				text: ReferenceUtilityText,
				font: UtilityTextFont,
				position: offset,
				color: XColor.White, //Game1.textColor,
				scale: 1.0f
			);
		}

		var timeSinceDraw = DrawStopwatch.Elapsed;
		DrawStopwatch.Restart();

		void DrawAnimatedInner() {
			var portraitDrawable = CenterCharacter.Drawable;

			var offset = regionOffset;
			offset -= new Vector2F(portraitDrawable.Width, portraitDrawable.Height);

			offset += (32, 32);

			portraitDrawable.Tick();
			portraitDrawable.Draw(this, batch, offset, 0.0f);
		}

		if (HdPortraitsApi.Value is not {} portraitsApi) {
			DrawAnimatedInner();
		}
		else {
			(Bounds, XTexture2D) GetPortrait(string? suffix) =>
				portraitsApi.GetTextureAndRegion(
					CenterCharacter.PortraitName,
					suffix,
					0,
					timeSinceDraw.Milliseconds,
					false
				);

			Bounds region = default;
			XTexture2D? texture = null;

			bool InBounds() {
				return texture is not null && texture.Width >= region.Width && texture.Height >= region.Height;
			}

			void TryPortrait(string? suffix, PortraitType type) {
				if (InvalidPortraitTypes.HasFlag(type)) {
					return;
				}

				if (!InBounds()) {
					try {
						(region, texture) = GetPortrait(suffix);
					}
					catch (Exception ex) {
						InvalidPortraitTypes |= type;
						Debug.TraceOnce(ex);
					}

					if (!InBounds()) {
						InvalidPortraitTypes |= type;
					}
				}
			}

			if (CenterCharacter.PortraitName == "Clint") {
				InvalidPortraitTypes |= PortraitType.Beach;
			}

			if (Season is {} season) {
				TryPortrait(season, PortraitType.SeasonLower);
				TryPortrait(season.CapitalizeFirstLetter(), PortraitType.SeasonUpper);
				TryPortrait(null, PortraitType.Plain);
				TryPortrait("Festival", PortraitType.Festival);
				TryPortrait("Beach", PortraitType.Beach);
			}
			else {
				TryPortrait(null, PortraitType.Plain);
			}

			if (!InBounds()) {
				DrawAnimatedInner();
			}
			else {
				var offset = regionOffset;
				offset -= new Vector2F(region.Width, region.Height);

				batch.Draw(texture, offset, region, XColor.White);
			}
		}

		// Draw Portrait
		/*
		{
			var offset = Region.Extent;
			offset -= new Vector2F(CenterCharacterPortraitDrawable.Width, CenterCharacterPortraitDrawable.Height);

			offset += (32, 32);

			var tempNPC = new NPC(new AnimatedSprite(CenterCharacterPortraitTexture.Texture.Name), XVector2.Zero, "", 0, "Haley", new(), CenterCharacterPortraitTexture.Texture, true);

			var dialog = new StardewValley.Dialogue("", tempNPC) {
				showPortrait = true
			};

			var dialogBox = new StardewValley.Menus.DialogueBox(
				0,
				0,
				1000, // minimum that drawPortrait requires to actually draw a portrait
				CenterCharacterPortraitDrawable.Height * 2
			) {
				characterDialogue = dialog,
				transitioning = false
			};

			bool oldShowPortraitsOption = Game1.options.showPortraits;
			Game1.options.showPortraits = true;
			try {
				dialogBox.draw(batch);
			}
			finally {
				Game1.options.showPortraits = oldShowPortraitsOption;
			}

			//CenterCharacterPortraitDrawable.Tick();
			//CenterCharacterPortraitDrawable.Draw(this, batch, offset, 0.0f);
		}
		*/
	}

	private DrawableInstance[] SetupScene(bool winter) {
		var center = Size / 2;

		var rand = new Random(RandomSeed);

		var plainGrass = winter ?
			new[] {
				OutdoorTiles[1, 14],
				OutdoorTiles[1, 14],
				OutdoorTiles[1, 14]
			} : new[] {
				OutdoorTiles[0, 7],
				OutdoorTiles[0, 11],
				OutdoorTiles[2, 16]
			};
		var grassArray = winter ?
			new[] {
				OutdoorTilesWeed,
				OutdoorTilesFlower,
				plainGrass[0],
				plainGrass[0],
				plainGrass[0],
				plainGrass[1],
				plainGrass[1],
				plainGrass[1],
				plainGrass[2],
				plainGrass[2],
				plainGrass[2],
				plainGrass[2],
				OutdoorTiles[4, 12],
				OutdoorTiles[5, 12],
				OutdoorTiles[4, 13],
			} :
			new[] {
				OutdoorTilesWeed,
				OutdoorTilesFlower,
				plainGrass[0],
				plainGrass[0],
				plainGrass[0],
				plainGrass[1],
				plainGrass[1],
				plainGrass[1],
				plainGrass[2],
				plainGrass[2],
				plainGrass[2],
				plainGrass[2],
				OutdoorTiles[0, 11],
				OutdoorTiles[4, 10],
				OutdoorTiles[5, 10],
			};
		var debrisArray = new[] {
			OutdoorTiles[7, 9],
			OutdoorTiles[7, 10],
			OutdoorTiles[7, 11],
			OutdoorTiles[7, 12],
		};
		int debrisChance = 20;

		var spriteSize = OutdoorTiles.RenderedSize;

		// We want the grass to be centered, rather than offset to the bounds
		var half = center;
		var alignedHalfCount = half / spriteSize;
		alignedHalfCount += (1, 1);
		var alignedHalf = alignedHalfCount * spriteSize;
		var start = center - alignedHalf;

		Vector2I tileCount = (alignedHalfCount * 2) + (1, 1);
		tileCount = tileCount.Max((6, 6));
		Vector2I mid = tileCount / 2;

		int GetOffsetX(int x) => start.X + (x * spriteSize.X);
		int GetOffsetY(int y) => start.Y + (y * spriteSize.Y);

		if (GetOffsetX(tileCount.X - 1) - (spriteSize.X / 2) >= Size.Width) {
			tileCount.X--;
		}

		if (GetOffsetY(tileCount.Y - 1) - (spriteSize.Y / 2) >= Size.Height) {
			tileCount.Y--;
		}

		var tileArray = new List<Drawable>[tileCount.X, tileCount.Y];

		static Vector2I Distance(Vector2I a, Vector2I b) => (a - b).Abs;

		for (int y = 0; y < tileCount.Y; ++y) {
			for (int x = 0; x < tileCount.X; ++x) {
				var drawables = (tileArray[x, y] ??= new());

				var grassObject = GetRandomOf(rand, grassArray);
				drawables.Add(grassObject);

				var distanceFromMid = Distance((x, y), mid);
				if (distanceFromMid.MaxOf > 1 && rand.Next(100) < debrisChance) {
					var debrisObject = GetRandomOf(rand, debrisArray);
					drawables.Add(debrisObject);
				}
			}
		}

		if (winter) {
			try { tileArray[mid.X - 2, mid.Y - 2][0] = OutdoorTiles[3, 14]; } catch (IndexOutOfRangeException) { }
			try { tileArray[mid.X - 1, mid.Y - 2][0] = OutdoorTiles[1, 15];	} catch (IndexOutOfRangeException) { }
			try { tileArray[mid.X + 0, mid.Y - 2][0] = OutdoorTiles[1, 15];	} catch (IndexOutOfRangeException) { }
			try { tileArray[mid.X + 1, mid.Y - 2][0] = OutdoorTiles[1, 15];	} catch (IndexOutOfRangeException) { }
			try { tileArray[mid.X + 2, mid.Y - 2][0] = OutdoorTiles[3, 16]; } catch (IndexOutOfRangeException) { }
		}

		try { if (winter) tileArray[mid.X - 2, mid.Y - 1][0] = OutdoorTiles[2, 14]; } catch (IndexOutOfRangeException) { }
		try { tileArray[mid.X - 1, mid.Y - 1][0] = OutdoorTiles[0, 8]; } catch (IndexOutOfRangeException) { }
		try { tileArray[mid.X + 0, mid.Y - 1][0] = OutdoorTiles[1, 8]; } catch (IndexOutOfRangeException) { }
		try { tileArray[mid.X + 1, mid.Y - 1][0] = OutdoorTiles[3, 8]; } catch (IndexOutOfRangeException) { }
		try { if (winter) tileArray[mid.X + 2, mid.Y - 1][0] = OutdoorTiles[0, 14]; } catch (IndexOutOfRangeException) { }

		try { if (winter) tileArray[mid.X - 2, mid.Y + 0][0] = OutdoorTiles[2, 14]; } catch (IndexOutOfRangeException) { }
		try { tileArray[mid.X - 1, mid.Y + 0][0] = OutdoorTiles[0, 9]; } catch (IndexOutOfRangeException) { }
		try { tileArray[mid.X + 0, mid.Y + 0][0] = OutdoorTiles[6, 8]; } catch (IndexOutOfRangeException) { }
		try { tileArray[mid.X + 1, mid.Y + 0][0] = OutdoorTiles[3, 9]; } catch (IndexOutOfRangeException) { }
		try { if (winter) tileArray[mid.X + 2, mid.Y + 0][0] = OutdoorTiles[0, 14]; } catch (IndexOutOfRangeException) { }

		try { if (winter) tileArray[mid.X - 2, mid.Y + 1][0] = OutdoorTiles[2, 14]; } catch (IndexOutOfRangeException) { }
		try { tileArray[mid.X - 1, mid.Y + 1][0] = OutdoorTiles[0, 10]; } catch (IndexOutOfRangeException) { }
		try { tileArray[mid.X + 0, mid.Y + 1][0] = OutdoorTiles[1, 10]; } catch (IndexOutOfRangeException) { }
		try { tileArray[mid.X + 1, mid.Y + 1][0] = OutdoorTiles[3, 10]; } catch (IndexOutOfRangeException) { }
		try { if (winter) tileArray[mid.X + 2, mid.Y + 1][0] = OutdoorTiles[0, 14]; } catch (IndexOutOfRangeException) { }

		if (winter) {
			try { tileArray[mid.X - 2, mid.Y + 2][0] = OutdoorTiles[3, 15]; } catch (IndexOutOfRangeException) { }
			try { tileArray[mid.X - 1, mid.Y + 2][0] = OutdoorTiles[1, 13]; } catch (IndexOutOfRangeException) { }
			try { tileArray[mid.X + 0, mid.Y + 2][0] = OutdoorTiles[1, 13]; } catch (IndexOutOfRangeException) { }
			try { tileArray[mid.X + 1, mid.Y + 2][0] = OutdoorTiles[1, 13]; } catch (IndexOutOfRangeException) { }
			try { tileArray[mid.X + 2, mid.Y + 2][0] = OutdoorTiles[3, 13]; } catch (IndexOutOfRangeException) { }
		}

		var shadowTexture = Game1.shadowTexture;

		// insert Character's shadow
		try { tileArray[mid.X, mid.Y].Add(new(shadowTexture)); } catch (IndexOutOfRangeException) { }

		// insert Character
		try { tileArray[mid.X, mid.Y].Add(new(CenterCharacter.Sprite, offset: -(TileSizeRendered / 2))); } catch (IndexOutOfRangeException) { }

		// insert Tree
		try { tileArray[mid.X - 4, mid.Y + 2].Add(TreeTexture[1, 0]); } catch (IndexOutOfRangeException) { }

		var drawableInstances = new List<DrawableInstance>();

		for (int y = 0; y < tileCount.Y; ++y) {
			int yOffset = GetOffsetY(y);

			for (int x = 0; x < tileCount.X; ++x) {
				int xOffset = GetOffsetX(x);
				Vector2I offset = (xOffset, yOffset);

				var list = tileArray[x, y];

				foreach (var drawable in list) {
					drawableInstances.Add(new(drawable.Clone(rand), offset));
				}
			}
		}

		TileCount = tileCount;

		return drawableInstances.ToArray();
	}

	protected override void OnResize(Vector2I size, Vector2I oldSize) {
		Drawables = SetupScene(Season == "winter");
	}

	protected override void OnTick() {
		CenterCharacter.Sprite.Tick();
	}
}
