/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/b-b-blueberry/CooksAssistant
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Network;
using StardewValley.TerrainFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace LoveOfCooking
{
	[XmlType("Mods_blueberry_cac_custombush")] // SpaceCore serialisation signature
	public class CustomBush : Bush
	{
		public class ItemRule
		{
			public string Object;
			public string Conditions = null;
			public int[] BaseQuantity = null;
		}

		public class ProduceRule : ItemRule
		{
			public int DaysToProduce = 0;
			public int DaysToAccumulateProduce = 0;
			public int MaximumAccumulatedProduce = 0;
			public int[] AccumulatedQuantity = null;
		}

		public class BushDefinition
		{
			public int Health = 99999;
			public bool DrawShadow = false;
			public int EffectiveSize = 0;
			public int EffectiveGrowthSize = 0;
			public int[] GrowthStages = new int[0];
			public bool IsActionable = false;
			public bool IsDestroyable = false;
			public bool IsPassable = false;
			public ItemRule[] ItemsDroppedWhenDestroyed = null;
			public ItemRule[] ItemsProduced = null;
			public string[] SeasonsToGrow = null;
			public string[] SeasonsToProduce = null;
			public string SoundWhenDestroyed = null;
			public string SoundWhenShaken = null;
			public int TilesHigh = 1;
			public int TilesWide = 1;
			public string[] ToolsToDestroy;
			public string[] ToolsToHarvest;

			// Textures
			private string _sourceTexture;
			public string SourceTexture
			{
				get => _sourceTexture;
				set => _sourceTexture = StardewModdingAPI.Utilities.PathUtilities.NormalizeAssetName(value);
			}
			public Dictionary<string, Rectangle> SourceAreas = new Dictionary<string, Rectangle>();
		}

		// Definitions
		[XmlIgnore]
		public static Dictionary<string, BushDefinition> BushDefinitions;
		[XmlIgnore]
		public static List<Texture2D> BushTextures;

		// Preserved values
		public readonly NetStringDictionary<int, NetInt> HeldProduce = new NetStringDictionary<int, NetInt>();
		public readonly NetInt GrowthDays = new NetInt();
		public readonly NetInt DaysSinceLastProduce = new NetInt();

		// Reflected fields
		[XmlIgnore]
		protected IReflectedField<float> AlphaField, ShakeField, MaxShakeField;
		[XmlIgnore]
		protected IReflectedMethod ShakeMethod;

		// Temp fields
		[XmlIgnore]
		private readonly NetString _variety = new NetString();
		[XmlIgnore]
		public BushDefinition Definition;
		[XmlIgnore]
		public Rectangle? SourceRectangle;
		public bool HasProduce => this.HeldProduce.Keys.Any(p => this.HeldProduce[p] > 0);
		public bool IsMature => this.Definition.GrowthStages != null
			&& (this.GrowthStage > this.Definition.GrowthStages.Length
				|| this.GrowthDays.Value >= this.Definition.GrowthStages.Sum());
		public int GrowthStage
		{
			get
			{
				if (this.HasProduce)
					return this.Definition.GrowthStages.Length;
				if (this.Definition.GrowthStages == null)
					return 0;
				
				int i, days = 0;
				for (i = 0; i < this.Definition.GrowthStages.Length && days < this.GrowthDays.Value; ++i)
				{
					days += this.Definition.GrowthStages[i];
				}
				return i;
			}
		}

		// Const values
		public const int HealthRemovedOnHit = 50;
		private const int DummyConditionEventId = 87008;

		// Huge issues
		public object Variety
		{
			// we've all made mistakes in life
			get => _variety.Value;
			set
			{
				// i know i have
				string variety = null;
				if (value is System.Xml.XmlNode[] xml)
				{
					value = xml[0].Value;
				}

				if (value is string s && !string.IsNullOrEmpty(s))
				{
					if (CustomBush.BushDefinitions.ContainsKey(s))
						variety = s;
					if (int.TryParse(s, out int i1))
						value = i1;
				}

				if (string.IsNullOrEmpty(variety))
				{
					variety = ModEntry.BushNameNettle;
					Log.W($"Undefined value for {nameof(this.Variety)}: {value.GetType().Name} {value}."
						+ $" Defaulted to {variety.GetType().Name} {variety}");
				}

				Log.D($"Set {nameof(CustomBush)}.{nameof(this.Variety)} to {variety.GetType().Name} {variety}",
					ModEntry.Config.DebugMode);

				this.SetForVariety(variety: variety, setNetField: true);
			}
		}

		public CustomBush()
			: base()
		{
			this.Init();
		}

		public CustomBush(Vector2 tile, GameLocation location, string variety)
			: base(tileLocation: tile, size: 0, location: location)
		{
			this.Init();
			this.currentTileLocation = tile;
			this.currentLocation = location;
			this.SetForVariety(variety: variety, setNetField: true);
		}

		private void Init()
		{
			this.AddNetFields();
			this.GetReflectedMembers();
		}

		private void AddNetFields()
		{
            this._variety.fieldChangeEvent += (NetString field, string oldValue, string newValue) =>
			{
				this.SetForVariety(variety: newValue, setNetField: false);
			};

			this.NetFields.AddFields(this._variety, this.HeldProduce);
		}

        private void GetReflectedMembers()
		{
			// Fields
			this.AlphaField = ModEntry.Instance.Helper.Reflection.GetField<float>(this, "alpha");
			this.ShakeField = ModEntry.Instance.Helper.Reflection.GetField<float>(this, "shakeRotation");
			this.MaxShakeField = ModEntry.Instance.Helper.Reflection.GetField<float>(this, "maxShake");
			// Properties
			// . . .
			// Methods
			this.ShakeMethod = ModEntry.Instance.Helper.Reflection.GetMethod(this, "shake");
		}

		public bool IsInSeason(string season)
		{
			return this.Definition.SeasonsToProduce != null
				&& this.Definition.SeasonsToProduce.Any(s => s.Equals(season, StringComparison.InvariantCultureIgnoreCase));
		}

		public string GetSeason(GameLocation location)
		{
			return this.overrideSeason.Value == -1
				? Game1.GetSeasonForLocation(location)
				: Utility.getSeasonNameFromNumber(this.overrideSeason.Value);
		}

		public bool CanProduce(string season)
		{
			bool isInSeason = this.IsInSeason(season: season);
			return this.IsMature && isInSeason;
		}

		public bool CanGrow(string season)
		{
			bool isInSeason = this.IsInSeason(season: season);
			int growthRadius = this.Definition.EffectiveGrowthSize < 0 || this.Definition.EffectiveGrowthSize > 5
					? 0
					: this.Definition.EffectiveGrowthSize;
			if (!isInSeason)
				return false;
			if (growthRadius < 1)
				return true;

			bool isGrowthAreaClear = true;
			int growthRectSize = ((growthRadius * 2) + 1) * Game1.tileSize;
			Rectangle growthRect = new Rectangle(
			(int)((this.tilePosition.Value.X - growthRadius) * Game1.tileSize),
			(int)((this.tilePosition.Value.Y - growthRadius) * Game1.tileSize),
			growthRectSize,
			growthRectSize);
			if (this.currentLocation.largeTerrainFeatures.Any(ltf => ltf.getBoundingBox(ltf.tilePosition.Value).Intersects(growthRect)))
			{
				isGrowthAreaClear = false;
			}
			else
			{
				foreach (KeyValuePair<Vector2, TerrainFeature> pair in this.currentLocation.terrainFeatures.Pairs
					.Where(pair => pair.Value.getBoundingBox(pair.Key).Intersects(growthRect)))
				{
					if (!pair.Value.Equals(this)
						&& (pair.Value is Tree || pair.Value is Bush || pair.Value is CustomBush || (pair.Value is HoeDirt h && h.crop != null)))
					{
						isGrowthAreaClear = false;
						break;
					}
				}
			}

			return isGrowthAreaClear;
		}

		private void SetForVariety(string variety, bool setNetField)
		{
			if (setNetField)
			{
				this._variety.Set(variety);
			}

			this.Definition = CustomBush.BushDefinitions[variety];
			this.Definition.SourceTexture = StardewModdingAPI.Utilities.PathUtilities.NormalizePath(this.Definition.SourceTexture);

			this.health = this.Definition.Health;
			this.size.Set(this.Definition.EffectiveSize);
			this.drawShadow.Set(this.Definition.DrawShadow);
			this.flipped.Set(Game1.random.NextDouble() < 0.5);
			this.loadSprite();
		}

		public void Shake(GameLocation location, Vector2 tileLocation, bool doEvenIfStillShaking)
		{
			bool isShaking = Math.Abs(0f - this.MaxShakeField.GetValue()) > 0.001f;
			if (isShaking || (!this.greenhouseBush.Value && Game1.currentSeason.Equals("winter")))
			{
				return;
			}

			this.ShakeMethod.Invoke(tileLocation, doEvenIfStillShaking);
		}

		public void TossItems(bool isDestroyed)
		{
			// Harvest produce

			// 'Botanist' profession improves harvest quality to best
			int quality = Game1.player.professions.Contains(Farmer.botanist)
				? StardewValley.Object.bestQuality
				: StardewValley.Object.lowQuality;
			Point centre = this.getBoundingBox().Center;

			foreach (string produce in this.HeldProduce.Keys.Where(p => this.HeldProduce[p] > 0))
			{
				StardewValley.Object o = Utility.fuzzyItemSearch(query: produce) as StardewValley.Object;
				o.Quality = quality;
				for (int i = 0; i < this.HeldProduce[produce]; ++i)
				{
					Game1.createItemDebris(
						item: o,
						origin: Utility.PointToVector2(centre),
						direction: Game1.random.Next(1, 4));
				}
			}

			if (!isDestroyed)
				return;

			foreach (ItemRule dropItem in this.Definition.ItemsDroppedWhenDestroyed)
			{
				// Ignore profession bonuses to quality
				StardewValley.Object o = Utility.fuzzyItemSearch(query: dropItem.Object) as StardewValley.Object;
				int quantity = Game1.random.Next(dropItem.BaseQuantity[0], dropItem.BaseQuantity[1] + 1);
				for (int i = 0; i < quantity; ++i)
				{
					Game1.createItemDebris(
						item: o,
						origin: Utility.PointToVector2(centre),
						direction: Game1.random.Next(1, 4));
				}
			}
		}

		public void TryGrowProduce(GameLocation location)
		{
			string season = this.GetSeason(location: location);
			if (this.CanGrow(season: season))
			{
				++this.GrowthDays.Value;
			}
			if (this.CanProduce(season: season))
			{
				foreach (ProduceRule produceRule in this.Definition.ItemsProduced)
				{
					// Require produce preconditions to be met if not null
					if (!string.IsNullOrWhiteSpace(produceRule.Conditions))
					{
						string precondition = $"{CustomBush.DummyConditionEventId}/{produceRule.Conditions}";
						if (this.currentLocation.checkEventPrecondition(precondition: precondition) < 0)
							continue;
					}

					if ((!this.HasProduce && this.DaysSinceLastProduce.Value < produceRule.DaysToProduce)
						|| (this.HasProduce && this.DaysSinceLastProduce.Value < produceRule.DaysToAccumulateProduce
							&& produceRule.AccumulatedQuantity != null))
						continue;

					// Add to held stacks of each produce if valid
					int quantity;
					if (!this.HeldProduce.ContainsKey(produceRule.Object))
					{
						quantity = Game1.random.Next(produceRule.BaseQuantity[0], produceRule.BaseQuantity[1] + 1);
						this.HeldProduce[produceRule.Object] = quantity;
					}
					else
					{
						quantity = Game1.random.Next(produceRule.AccumulatedQuantity[0], produceRule.AccumulatedQuantity[1] + 1);
						this.HeldProduce[produceRule.Object] += quantity;
					}

					this.DaysSinceLastProduce.Set(0);
				}
			}
		}

		public override void dayUpdate(GameLocation environment, Vector2 tileLocation)
		{
			this.greenhouseBush.Value = environment.IsGreenhouse;

			this.TryGrowProduce(location: environment);
			this.SetUpSourceRectangle();
		}

		public override bool seasonUpdate(bool onLoad)
		{
			if (!Game1.IsMultiplayer || Game1.IsServer)
			{
				this.tileSheetOffset.Value = 0;
				this.loadSprite();
			}
			return false;
		}

		public override bool isActionable()
		{
			return this.Definition.IsActionable;
		}

		public override bool isPassable(Character c = null)
		{
			return this.Definition.IsPassable || base.isPassable(c);
		}

		public override bool performUseAction(Vector2 tileLocation, GameLocation location)
		{
			bool canShake = !this.HasProduce || this.Definition.ToolsToHarvest == null
				|| (Game1.player.CurrentTool != null && this.Definition.ToolsToHarvest.Any(
					tool => Game1.player.CurrentTool.GetType().Name.Equals(tool, StringComparison.InvariantCultureIgnoreCase)));
			if (canShake)
			{
				this.Shake(location: location, tileLocation: tileLocation, doEvenIfStillShaking: true);
			}
			else
			{
				Game1.playSound("cancel");
			}
			return true;
		}

		public override bool performToolAction(Tool t, int explosion, Vector2 tileLocation, GameLocation location)
		{
			location ??= this.currentLocation;
			
			if (explosion > 0)
			{
				this.Shake(location: location, tileLocation, doEvenIfStillShaking: true);
				return false;
			}

			Events.InvokeOnBushToolUsed(bush: this, tool: t, explosion: explosion, tileLocation: tileLocation, location: location);

			if (t != null
				&& this.Definition.ToolsToDestroy.Any(n => n.Equals(t.GetType().Name, StringComparison.InvariantCultureIgnoreCase))
				&& this.isDestroyable(location, tileLocation))
			{
				this.health -= CustomBush.HealthRemovedOnHit;

				if (this.health > 0)
				{
					this.Shake(location: location, tileLocation, doEvenIfStillShaking: true);
				}
				else
				{
					this.TossItems(isDestroyed: true);

					string season = this.GetSeason(location: location);

					if (!string.IsNullOrEmpty(this.Definition.SoundWhenDestroyed))
						location.playSound(this.Definition.SoundWhenDestroyed);

					if (!string.IsNullOrEmpty(this.Definition.SoundWhenShaken))
						DelayedAction.playSoundAfterDelay(this.Definition.SoundWhenShaken, 100);
					Color leafColour;
					switch (season)
					{
						case "winter":
							leafColour = Color.Cyan;
							break;
						case "fall":
							leafColour = Color.IndianRed;
							break;
						case "summer":
							leafColour = Color.ForestGreen;
							break;
						default:
							leafColour = Color.Green;
							break;
					}
					Multiplayer multiplayer = ModEntry.Instance.Helper.Reflection.GetField<Multiplayer>(typeof(Game1), "multiplayer").GetValue();
					const int leafSize = 16;
					Rectangle leafSource = new Rectangle(355, 1200 + (season == "fall" ? leafSize : (season == "winter" ? (-leafSize) : 0)), leafSize, leafSize);
					Rectangle bushArea = this.getBoundingBox();
					const int leafCount = 6;
					for (int j = 0; j <= this.Definition.EffectiveSize; j++)
					{
						for (int i = 0; i < leafCount; i++)
						{
							multiplayer.broadcastSprites(location, new TemporaryAnimatedSprite(
								textureName: "LooseSprites\\Cursors",
								sourceRect: leafSource,
								position: Utility.getRandomPositionInThisRectangle(bushArea, Game1.random)
									- new Vector2(0f, Game1.random.Next(Game1.tileSize)),
								flipped: false,
								alphaFade: 0.01f,
								color: leafColour)
							{
								motion = new Vector2(Game1.random.Next(-10, 11) / 10f, -Game1.random.Next(1, 4)),
								acceleration = new Vector2(0f, Game1.random.Next(13, 17) / 100f),
								accelerationChange = new Vector2(0f, -0.001f),
								scale = 2f,
								layerDepth = (tileLocation.Y + 1) * Game1.tileSize / 10000f,
								animationLength = 11,
								totalNumberOfLoops = 99,
								interval = Game1.random.Next(10, 40),
								delayBeforeAnimationStart = (j + 1) * i * 20
							});
							if (i % leafCount == 0)
							{
								multiplayer.broadcastSprites(
									location: location,
									sprites: new TemporaryAnimatedSprite(
										rowInAnimationTexture: 50,
										position: Utility.getRandomPositionInThisRectangle(bushArea, Game1.random)
											- new Vector2(bushArea.Width / 2, bushArea.Height * (float)((Game1.random.NextDouble() / 2) + 0.5f)),
										color: leafColour));
								multiplayer.broadcastSprites(
									location: location,
									sprites: new TemporaryAnimatedSprite(
										rowInAnimationTexture: 12,
										position: Utility.getRandomPositionInThisRectangle(bushArea, Game1.random)
											- new Vector2(bushArea.Width / 2, Game1.random.Next(1, 2)),
										color: Color.White));
							}
						}
					}
					return true;
				}
			}
			return false;
		}

		public override Rectangle getBoundingBox(Vector2 tileLocation)
		{
			return new Rectangle(
				(int)tileLocation.X * Game1.tileSize,
				(int)tileLocation.Y * Game1.tileSize,
				Game1.tileSize * this.Definition.TilesWide,
				Game1.tileSize * this.Definition.TilesHigh);
		}

		public override void loadSprite()
		{
			this.tileSheetOffset.Value = this.inBloom(Game1.currentSeason, Game1.dayOfMonth) ? 1 : 0;
			this.SetUpSourceRectangle();
		}

		public void SetUpSourceRectangle()
		{
			string season = this.GetSeason(this.currentLocation);
			if (this.Definition != null && this.Definition.SourceAreas.ContainsKey(season))
			{
				Rectangle sourceRectangle = this.Definition.SourceAreas[this.GetSeason(this.currentLocation)];
				sourceRectangle.X += (sourceRectangle.Width * this.GrowthStage);
				this.SourceRectangle = sourceRectangle;
			}
			else
			{
				this.SourceRectangle = null;
			}
		}
		
		public override void draw(SpriteBatch spriteBatch, Vector2 tileLocation)
		{
			if (!this.SourceRectangle.HasValue)
				return;
			Rectangle bounds = this.getBoundingBox(tileLocation);
			Vector2 screenPosition = Game1.GlobalToLocal(
				viewport: Game1.viewport,
				globalPosition: new Vector2(
					tileLocation.X * Game1.tileSize + (bounds.Width / 2),
					(tileLocation.Y + this.Definition.TilesHigh) * Game1.tileSize));
			spriteBatch.Draw(
				texture: CustomBush.BushTextures.First(tx => tx.Name == this.Definition.SourceTexture),
				position: screenPosition,
				sourceRectangle: this.SourceRectangle.Value,
				color: Color.White * this.AlphaField.GetValue(),
				rotation: this.ShakeField.GetValue(),
				origin: new Vector2(
					this.SourceRectangle.Value.Width / 2,
					this.SourceRectangle.Value.Height),
				scale: Game1.pixelZoom,
				effects: this.flipped.Value ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
				layerDepth: ((bounds.Center.Y + ((this.Definition.TilesHigh * Game1.tileSize) - Game1.tileSize / Game1.pixelZoom)) / 10000f) - (tileLocation.X / 1000000f));
		}

		/*
		public override void drawInMenu(SpriteBatch spriteBatch, Vector2 positionOnScreen,
			Vector2 tileLocation, float scale, float layerDepth)
		{
			layerDepth += positionOnScreen.X / 100000f;
			spriteBatch.Draw(
				texture: texture.Value,
				position: positionOnScreen + new Vector2(0f, -64f * scale),
				sourceRectangle: new Rectangle(32, 96, 16, 32),
				color: Color.White,
				rotation: 0f,
				origin: Vector2.Zero,
				scale: scale,
				effects: flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
				layerDepth: layerDepth + (positionOnScreen.Y + 448f * scale - 1f) / 20000f);
		}
		*/

		// HarmonyPatch behaviours 

		public static bool InBloomBehaviour(CustomBush bush)
		{
			return bush.HasProduce;
		}

		public static int GetEffectiveSizeBehaviour(CustomBush bush)
		{
			return bush.Definition.EffectiveSize;
		}

		public static bool IsDestroyableBehaviour(CustomBush bush)
		{
			return bush.Definition.IsDestroyable;
		}

		public static void ShakeBehaviour(CustomBush bush, Vector2 tileLocation)
		{
			bush.currentLocation.localSound(bush.Definition.SoundWhenShaken);

			bush.TossItems(isDestroyed: false);

			// Shake actions
			Events.InvokeOnBushShaken(bush: bush);
		}

		internal static void FindBushesGlobally(string variety, bool remove)
		{
			IEnumerable<GameLocation> locations = variety == ModEntry.BushNameNettle
				? ModEntry.ItemDefinitions["NettlesLocations"].Select(name => Game1.getLocationFromName(name))
				: Game1.locations;
			foreach (GameLocation location in locations)
			{
				List<CustomBush> bushes = location.largeTerrainFeatures
					.OfType<CustomBush>()
					.ToList();
				Log.D($"Found {bushes.Count} bushes in {location.Name} ({variety})"
					+ (bushes.Any()
						? bushes.Aggregate("\n=> ",
							(str, nb) => $"{str} ({nb.tilePosition.Value.X},{nb.tilePosition.Value.Y})")
						: string.Empty),
					ModEntry.Config.DebugMode);
				if (remove)
				{
					for (int i = location.largeTerrainFeatures.Count - 1; i >= 0; --i)
					{
						LargeTerrainFeature ltf = location.largeTerrainFeatures[i];
						if (ltf is CustomBush customBush)
						{
							Log.D($"Removing from {ltf.currentTileLocation}...",
								ModEntry.Config.DebugMode);
							location.largeTerrainFeatures.RemoveAt(i);
						}
					}
				}
			}
		}

		internal static void Reload()
		{
			CustomBush.BushDefinitions = ModEntry.Instance.Helper.Data.ReadJsonFile
				<Dictionary<string, CustomBush.BushDefinition>>
				(path: $"{AssetManager.LocalBushDataPath}.json");
			CustomBush.BushTextures = CustomBush.BushDefinitions.Values
				.Select(d => d.SourceTexture)
				.Distinct()
				.Select(assetKey => Game1.content.Load<Texture2D>(assetKey))
				.ToList();
		}
	}
}
