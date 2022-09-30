/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Shockah/Stardew-Valley-Mods
**
*************************************************/

using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Shockah.CommonModCode;
using Shockah.CommonModCode.GMCM;
using Shockah.CommonModCode.Stardew;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using SObject = StardewValley.Object;

namespace Shockah.FlexibleSprinklers
{
	public class FlexibleSprinklers : BaseMod<ModConfig>, IFlexibleSprinklersApi
	{
		private const int PressureNozzleParentSheetIndex = 915;
		internal static readonly string LineSprinklersModID = "hootless.LineSprinklers";
		internal static readonly string BetterSprinklersModID = "Speeder.BetterSprinklers";
		internal static readonly string PrismaticToolsModID = "stokastic.PrismaticTools";

		private const int FPS = 60;
		private const float SprinklerCoverageAlphaDecrement = 1f / FPS; // 1f per second

		public static FlexibleSprinklers Instance { get; private set; } = null!;
		private bool IsSlimeHutchWaterSpotsInstalled = false;

		public bool IsSprinklerBehaviorIndependent
			=> SprinklerBehavior is ISprinklerBehavior.Independent;

		internal ISprinklerBehavior SprinklerBehavior { get; private set; } = null!;
		private readonly IList<Func<SObject, int?>> SprinklerTierProviders = new List<Func<SObject, int?>>();
		private readonly IList<Func<SObject, Vector2[]>> SprinklerCoverageProviders = new List<Func<SObject, Vector2[]>>();
		internal IList<Func<GameLocation, Vector2, bool?>> CustomWaterableTileProviders { get; private set; } = new List<Func<GameLocation, Vector2, bool?>>();
		private float SprinklerCoverageAlpha = 0f;
		private float SprinklerCoverageCurrentAnimationTime = 0f;

		internal ILineSprinklersApi? LineSprinklersApi { get; private set; }
		internal IBetterSprinklersApi? BetterSprinklersApi { get; private set; }

		public override void OnEntry(IModHelper helper)
		{
			Instance = this;

			helper.Events.GameLoop.GameLaunched += OnGameLaunched;
			helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;
			helper.Events.Display.RenderedWorld += OnRenderedWorld;
			helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
			helper.Events.GameLoop.DayStarted += OnDayStarted;
			helper.Events.GameLoop.DayEnding += OnDayEnding;
			helper.Events.World.ObjectListChanged += OnObjectListChanged;
			helper.Events.World.TerrainFeatureListChanged += OnTerrainFeatureListChanged;
			helper.Events.World.LargeTerrainFeatureListChanged += OnLargeTerrainFeatureListChanged;
			helper.Events.Input.ButtonPressed += OnButtonPressed;

			RegisterCustomWaterableTileProvider((location, v) =>
			{
				if (location is not SlimeHutch)
					return null;

				if (IsSlimeHutchWaterSpotsInstalled)
				{
					var tileIndex = location.getTileIndexAt(new((int)v.X, (int)v.Y), "Buildings");
					if (tileIndex is 2134 or 2135)
						return true;
				}
				else if (v.X == 16f && v.Y >= 6f && v.Y <= 9f)
				{
					return true;
				}

				return null;
			});
			RegisterCustomWaterableTileProvider((location, v) => Config.WaterPetBowl && location.getTileIndexAt((int)v.X, (int)v.Y, "Buildings") == 1938 ? true : null);

			SetupSprinklerBehavior();
		}

		public override void MigrateConfig(ISemanticVersion? configVersion, ISemanticVersion modVersion)
		{
			// do nothing, for now
			// later on, migrate users from Flood Fill to Cluster
		}

		private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
		{
			var harmony = new Harmony(ModManifest.UniqueID);
			VanillaPatches.Apply(harmony);

			LineSprinklersApi = Helper.ModRegistry.GetApi<ILineSprinklersApi>(LineSprinklersModID);
			if (LineSprinklersApi != null)
				LineSprinklersPatches.Apply(harmony);

			BetterSprinklersApi = Helper.ModRegistry.GetApi<IBetterSprinklersApi>(BetterSprinklersModID);
			if (BetterSprinklersApi != null)
				BetterSplinklersPatches.Apply(harmony);

			IsSlimeHutchWaterSpotsInstalled = Helper.ModRegistry.IsLoaded("aedenthorn.SlimeHutchWaterSpots");

			SetupConfig();
		}

		private void OnUpdateTicked(object? sender, UpdateTickedEventArgs e)
		{
			SprinklerCoverageAlpha = Math.Max(SprinklerCoverageAlpha - SprinklerCoverageAlphaDecrement, 0f);
			SprinklerCoverageCurrentAnimationTime = Math.Min(SprinklerCoverageCurrentAnimationTime + 1f / FPS, Config.CoverageAnimationInSeconds);
		}

		private void OnRenderedWorld(object? sender, RenderedWorldEventArgs e)
		{
			if (SprinklerCoverageAlpha <= 0f)
				return;
			GameLocation location = Game1.currentLocation;
			if (location is null)
				return;

			var sprinklerTilesWithSteps = SprinklerBehavior.GetSprinklerTilesWithSteps(new GameLocationMap(location, CustomWaterableTileProviders));
			var alreadyShown = new HashSet<IntPoint>();
			foreach (var step in sprinklerTilesWithSteps)
			{
				if (Config.CoverageAnimationInSeconds > 0f && step.Item2 * Config.CoverageAnimationInSeconds > SprinklerCoverageCurrentAnimationTime)
					break;
				foreach (var sprinklerTile in step.Item1)
				{
					if (!Config.CoverageOverlayDuplicates)
					{
						if (alreadyShown.Contains(sprinklerTile))
							continue;
						alreadyShown.Add(sprinklerTile);
					}

					var position = new Vector2(sprinklerTile.X * Game1.tileSize, sprinklerTile.Y * Game1.tileSize);
					e.SpriteBatch.Draw(
						Game1.mouseCursors,
						Game1.GlobalToLocal(position),
						new Rectangle(194, 388, 16, 16),
						Color.White * Math.Clamp(SprinklerCoverageAlpha, 0f, 1f),
						0.0f,
						Vector2.Zero,
						Game1.pixelZoom,
						SpriteEffects.None,
						0.01f
					);
				}
			}
		}

		// Solid Foundations compatibility: watering late, after SF restores the locations
		[EventPriority(EventPriority.Low - 10)]
		private void OnDayStarted(object? sender, DayStartedEventArgs e)
		{
			ActivateAllSprinklers();
		}

		// Solid Foundations compatibility: watering early, before SF hides the locations
		[EventPriority(EventPriority.High + 10)]
		private void OnDayEnding(object? sender, DayEndingEventArgs e)
		{
			if (!Config.ActivateBeforeSleep)
				return;
			ActivateAllSprinklers();
		}

		private void OnObjectListChanged(object? sender, ObjectListChangedEventArgs e)
		{
			SprinklerBehavior.ClearCacheForMap(new GameLocationMap(e.Location, CustomWaterableTileProviders));
			if (!e.Added.Where(o => o.Value.IsSprinkler()).Any() && !e.Removed.Where(o => o.Value.IsSprinkler()).Any())
				return;

			if (Config.ActivateOnPlacement && SprinklerBehavior is ISprinklerBehavior.Independent)
			{
				foreach (var (_, @object) in e.Added)
					if (@object.IsSprinkler())
						ActivateSprinkler(@object, e.Location);
			}
			if (Config.ShowCoverageOnPlacement)
				DisplaySprinklerCoverage();
		}

		private void OnSaveLoaded(object? sender, SaveLoadedEventArgs e)
		{
			SprinklerBehavior.ClearCache();
			SprinklerCoverageAlpha = 0f;
			SprinklerCoverageCurrentAnimationTime = 0f;
		}

		private void OnTerrainFeatureListChanged(object? sender, TerrainFeatureListChangedEventArgs e)
		{
			SprinklerBehavior.ClearCacheForMap(new GameLocationMap(e.Location, CustomWaterableTileProviders));
		}

		private void OnLargeTerrainFeatureListChanged(object? sender, LargeTerrainFeatureListChangedEventArgs e)
		{
			SprinklerBehavior.ClearCacheForMap(new GameLocationMap(e.Location, CustomWaterableTileProviders));
		}

		private void OnButtonPressed(object? sender, ButtonPressedEventArgs e)
		{
			if (!Config.ActivateOnAction && !Config.ShowCoverageOnAction)
				return;
			if (!Context.IsPlayerFree)
				return;
			if (!e.Button.IsActionButton())
				return;

			var tile = e.Cursor.GrabTile;
			var location = Game1.currentLocation;
			var @object = location.getObjectAtTile((int)tile.X, (int)tile.Y);
			if (@object == null || !@object.IsSprinkler())
				return;

			var heldItem = Game1.player.CurrentItem;
			if (heldItem?.ParentSheetIndex == PressureNozzleParentSheetIndex && @object.heldObject?.Value?.ParentSheetIndex != PressureNozzleParentSheetIndex)
				return;

			if (Config.ActivateOnAction && SprinklerBehavior is ISprinklerBehavior.Independent)
				ActivateSprinkler(@object, location);
			if (Config.ShowCoverageOnAction)
				DisplaySprinklerCoverage();
			Helper.Input.Suppress(e.Button);
		}

		private void SetupConfig()
		{
			var api = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
			if (api is null)
				return;
			var helper = new GMCMI18nHelper(api, ModManifest, Helper.Translation);

			api.Register(
				ModManifest,
				reset: () => Config = new ModConfig(),
				save: () =>
				{
					WriteConfig();
					LogConfig();
					SetupSprinklerBehavior();
				}
			);

			helper.AddEnumOption("config.sprinklerBehavior", () => Config.SprinklerBehavior);
			helper.AddBoolOption("config.ignoreRange", () => Config.IgnoreRange);
			helper.AddBoolOption("config.waterGardenPots", () => Config.WaterGardenPots);
			helper.AddBoolOption("config.waterPetBowl", () => Config.WaterPetBowl);
			helper.AddBoolOption("config.compatibilityMode", () => Config.CompatibilityMode);

			helper.AddSectionTitle("config.cluster.section");
			helper.AddBoolOption("config.cluster.splitDisconnected", () => Config.SplitDisconnectedClusters);
			helper.AddEnumOption("config.cluster.ordering", () => Config.ClusterBehaviorClusterOrdering);
			helper.AddEnumOption("config.cluster.betweenClusterBalance", () => Config.ClusterBehaviorBetweenClusterBalanceMode);
			helper.AddEnumOption("config.cluster.inClusterBalance", () => Config.ClusterBehaviorInClusterBalanceMode);

			helper.AddSectionTitle("config.floodFill.section");
			helper.AddEnumOption("config.floodFill.balanceMode", () => Config.TileWaterBalanceMode);

			helper.AddSectionTitle("config.activation.section");
			helper.AddBoolOption("config.activation.beforeSleep", () => Config.ActivateBeforeSleep);
			helper.AddBoolOption("config.activation.onPlacement", () => Config.ActivateOnPlacement);
			helper.AddBoolOption("config.activation.onAction", () => Config.ActivateOnAction);

			helper.AddSectionTitle("config.coverage.section");
			helper.AddNumberOption("config.coverage.displayTime", () => Config.CoverageTimeInSeconds, min: 1f);
			helper.AddNumberOption("config.coverage.animationTime", () => Config.CoverageAnimationInSeconds, min: 0f);
			helper.AddNumberOption("config.coverage.alpha", () => Config.CoverageAlpha, min: 0f, max: 1f, interval: 0.05f);
			helper.AddBoolOption("config.coverage.overlayDuplicates", () => Config.CoverageOverlayDuplicates);
			helper.AddBoolOption("config.coverage.onPlacement", () => Config.ShowCoverageOnPlacement);
			helper.AddBoolOption("config.coverage.onAction", () => Config.ShowCoverageOnAction);

			helper.AddSectionTitle("config.sprinklerPower.section");
			helper.AddNumberOption("config.sprinklerPower.tier1", () => Config.Tier1Power, min: 0);
			helper.AddNumberOption("config.sprinklerPower.tier2", () => Config.Tier2Power, min: 0);
			helper.AddNumberOption("config.sprinklerPower.tier3", () => Config.Tier3Power, min: 0);
			helper.AddNumberOption("config.sprinklerPower.tier4", () => Config.Tier4Power, min: 0);
			helper.AddNumberOption("config.sprinklerPower.tier5", () => Config.Tier5Power, min: 0);
			helper.AddNumberOption("config.sprinklerPower.tier6", () => Config.Tier6Power, min: 0);
			helper.AddNumberOption("config.sprinklerPower.tier7", () => Config.Tier7Power, min: 0);
			helper.AddNumberOption("config.sprinklerPower.tier8", () => Config.Tier8Power, min: 0);
		}

		private void SetupSprinklerBehavior()
		{
			if (Config.SprinklerBehavior is SprinklerBehaviorEnum.Flexible or SprinklerBehaviorEnum.FlexibleWithoutVanilla)
				Monitor.LogOnce("The \"Flood fill\"-family behaviors are obsolete and will be removed in a future update. Please switch to using the \"Cluster\"-family behaviors.", LogLevel.Warn);
			SprinklerBehavior = Config.SprinklerBehavior switch
			{
				SprinklerBehaviorEnum.Cluster => new ClusterSprinklerBehavior(Config.ClusterBehaviorClusterOrdering, Config.ClusterBehaviorBetweenClusterBalanceMode, Config.ClusterBehaviorInClusterBalanceMode, new VanillaSprinklerBehavior()),
				SprinklerBehaviorEnum.ClusterWithoutVanilla => new ClusterSprinklerBehavior(Config.ClusterBehaviorClusterOrdering, Config.ClusterBehaviorBetweenClusterBalanceMode, Config.ClusterBehaviorInClusterBalanceMode, null),
				SprinklerBehaviorEnum.Flexible => new FloodFillSprinklerBehavior(Config.TileWaterBalanceMode, new VanillaSprinklerBehavior()),
				SprinklerBehaviorEnum.FlexibleWithoutVanilla => new FloodFillSprinklerBehavior(Config.TileWaterBalanceMode, null),
				SprinklerBehaviorEnum.Vanilla => new VanillaSprinklerBehavior(),
				_ => throw new ArgumentException($"{nameof(SprinklerBehaviorEnum)} has an invalid value."),
			};
		}

		public void ActivateAllSprinklers()
		{
			if (Game1.player.team.SpecialOrderRuleActive("NO_SPRINKLER"))
				return;
			foreach (GameLocation location in GameExt.GetAllLocations())
				ActivateSprinklersInLocation(location);
		}

		public void ActivateSprinklersInLocation(GameLocation location)
		{
			if (location.IsOutdoors && Game1.IsRainingHere(location))
				return;
			if (Game1.player.team.SpecialOrderRuleActive("NO_SPRINKLER"))
				return;

			GameLocationMap map = new(location, CustomWaterableTileProviders);
			var sprinklers = map.GetAllSprinklers().ToList();
			if (sprinklers.Count != 0)
				Monitor.Log($"Activating all ({sprinklers.Count}) sprinkler(s) in location {GetNameForLocation(location)} with behavior {Config.SprinklerBehavior}.", LogLevel.Trace);
			
			var sprinklerTiles = SprinklerBehavior.GetSprinklerTiles(map);
			foreach (var sprinklerTile in sprinklerTiles)
				map.WaterTile(sprinklerTile);
			foreach (var sprinkler in location.Objects.Values.Where(o => o.IsSprinkler()))
				sprinkler.ApplySprinklerAnimation(location);
		}

		internal static string GetNameForLocation(GameLocation location)
		{
			var selfName = location.NameOrUniqueName ?? "";
			var rootName = location.Root?.Value?.NameOrUniqueName ?? "";
			if (selfName == rootName)
				return selfName;
			else if (selfName == "" && rootName == "")
				return "";
			else if (selfName == "" && rootName != "")
				return $"<unknown> @ {rootName}";
			else if (selfName != "" && rootName == "")
				return selfName;
			else
				return $"{selfName} @ {rootName}";
		}

		public void ActivateSprinkler(SObject sprinkler, GameLocation location)
		{
			if (SprinklerBehavior is not ISprinklerBehavior.Independent)
				throw new InvalidOperationException("Current sprinkler behavior does not allow independent sprinkler activation.");
			if (Game1.player.team.SpecialOrderRuleActive("NO_SPRINKLER"))
				return;
			if (!sprinkler.IsSprinkler())
				return;

			foreach (var sprinklerTile in GetModifiedSprinklerCoverage(sprinkler, location))
				sprinkler.ApplySprinkler(location, new Vector2(sprinklerTile.X, sprinklerTile.Y));
			sprinkler.ApplySprinklerAnimation(location);
		}

		internal int GetSprinklerPower(SObject sprinkler, Vector2[] layout)
		{
			int? GetTier()
			{
				foreach (var sprinklerTierProvider in SprinklerTierProviders)
				{
					var tier = sprinklerTierProvider(sprinkler);
					if (tier != null)
						return tier;
				}

				// Line Sprinklers is patched, no need for custom handling here

				var radius = sprinkler.GetModifiedRadiusForSprinkler();
				return radius == -1 ? null : radius + 1;
			}

			var tier = GetTier();

			if (tier == null)
			{
				return layout.Length;
			}
			else
			{
				var powers = new[] { Config.Tier1Power, Config.Tier2Power, Config.Tier3Power, Config.Tier4Power, Config.Tier5Power, Config.Tier6Power, Config.Tier7Power, Config.Tier8Power };
				return tier.Value < powers.Length ? powers[tier.Value - 1] : layout.Length;
			}
		}

		internal SprinklerInfo GetSprinklerInfo(SObject sprinkler)
		{
			var layout = GetUnmodifiedSprinklerCoverage(sprinkler);
			var power = GetSprinklerPower(sprinkler, layout);
			return new SprinklerInfo(layout.ToHashSet(), power);
		}

		public int GetSprinklerPower(SObject sprinkler)
			=> GetSprinklerInfo(sprinkler).Power;

		[Obsolete("Sprinkler range now also depends on its unmodified coverage shape. Use `GetSprinklerSpreadRange` instead to achieve the same result as before. This method will be removed in a future update.")]
		public int GetFloodFillSprinklerRange(int power)
		{
			Monitor.LogOnce("An obsolete method `GetFloodFillSprinklerRange` was called, most likely by another mod. This method will be removed in a future update. Any mods using it should be updated before then.", LogLevel.Warn);
			return GetSprinklerSpreadRange(power);
		}

		public int GetSprinklerSpreadRange(int power)
			=> (int)Math.Floor(Math.Pow(power, 0.62) + 1);

		public int GetSprinklerFocusedRange(IReadOnlyCollection<Vector2> coverage)
		{
			if (coverage.Count == 0)
				return 0;
			int manhattanDistance = (int)coverage.Max(t => Math.Abs(t.X)) + (int)coverage.Max(t => Math.Abs(t.Y));
			Vector2 sum = coverage.Aggregate((a, b) => a + b);
			sum.Normalize();
			return (int)(Math.Max(Math.Abs(sum.X), Math.Abs(sum.Y)) * manhattanDistance);
		}

		public int GetSprinklerMaxRange(SObject sprinkler)
		{
			if (!sprinkler.IsSprinkler())
				return 0;
			SprinklerInfo info = GetSprinklerInfo(sprinkler);
			return GetSprinklerMaxRange(info);
		}

		internal int GetSprinklerMaxRange(SprinklerInfo info)
		{
			int spreadRange = GetSprinklerSpreadRange(info.Power);
			int focusedRange = GetSprinklerFocusedRange(info.Layout.ToArray());
			return Math.Max(spreadRange, focusedRange);
		}

		public bool IsTileInRangeOfSprinkler(SObject sprinkler, GameLocation location, Vector2 tileLocation)
		{
			if (SprinklerBehavior is not ISprinklerBehavior.Independent)
				throw new InvalidOperationException("Current sprinkler behavior does not allow independent sprinkler activation.");

			var info = GetSprinklerInfo(sprinkler);
			var manhattanDistance = ((int)tileLocation.X - (int)sprinkler.TileLocation.X) + ((int)tileLocation.Y - (int)sprinkler.TileLocation.Y);
			if (manhattanDistance > GetSprinklerMaxRange(info))
			{
				if (!info.Layout.Contains(tileLocation - sprinkler.TileLocation))
					return false;
			}
			return GetModifiedSprinklerCoverage(sprinkler, location).Contains(tileLocation);
		}

		public bool IsTileInRangeOfAnySprinkler(GameLocation location, Vector2 tileLocation)
			=> PrivateIsTileInRangeOfSprinklers(location.Objects.Values.Where(o => o.IsSprinkler()), location, tileLocation, true);

		public bool IsTileInRangeOfSprinklers(IEnumerable<SObject> sprinklers, GameLocation location, Vector2 tileLocation)
		{
			if (SprinklerBehavior is not ISprinklerBehavior.Independent)
				throw new InvalidOperationException("Current sprinkler behavior does not allow independent sprinkler activation.");
			return PrivateIsTileInRangeOfSprinklers(sprinklers, location, tileLocation, false);
		}

		public IReadOnlySet<Vector2> GetAllTilesInRangeOfSprinklers(GameLocation location)
		{
			IMap map = new GameLocationMap(location, CustomWaterableTileProviders);
			return SprinklerBehavior.GetSprinklerTiles(map).Select(t => new Vector2(t.X, t.Y)).ToHashSet();
		}

		private bool PrivateIsTileInRangeOfSprinklers(IEnumerable<SObject> sprinklers, GameLocation location, Vector2 tileLocation, bool isForAllSprinklers)
		{
			var sprinklersList = sprinklers.ToList();
			foreach (var sprinkler in sprinklersList)
			{
				if (!sprinkler.IsSprinkler())
					continue;

				var info = GetSprinklerInfo(sprinkler);
				var manhattanDistance = ((int)tileLocation.X - (int)sprinkler.TileLocation.X) + ((int)tileLocation.Y - (int)sprinkler.TileLocation.Y);
				if (manhattanDistance > GetSprinklerMaxRange(info))
				{
					if (SprinklerBehavior is not ISprinklerBehavior.Independent || !info.Layout.Contains(tileLocation - sprinkler.TileLocation))
						continue;
				}
				goto afterSimpleCheck;
			}

			return false;
			afterSimpleCheck:;

			if (isForAllSprinklers)
			{
				return SprinklerBehavior.GetSprinklerTiles(
					new GameLocationMap(location, CustomWaterableTileProviders)
				).Contains(new IntPoint((int)tileLocation.X, (int)tileLocation.Y));
			}
			else if (SprinklerBehavior is ISprinklerBehavior.Independent independent)
			{
				return independent.GetSprinklerTiles(
					new GameLocationMap(location, CustomWaterableTileProviders),
					sprinklersList
						.Where(s => s.IsSprinkler())
						.Select(s => (position: new IntPoint((int)s.TileLocation.X, (int)s.TileLocation.Y), info: GetSprinklerInfo(s)))
				).Contains(new IntPoint((int)tileLocation.X, (int)tileLocation.Y));
			}
			else
			{
				throw new InvalidOperationException("Current sprinkler behavior does not allow independent sprinkler activation.");
			}
		}

		public Vector2[] GetModifiedSprinklerCoverage(SObject sprinkler, GameLocation location)
		{
			if (SprinklerBehavior is not ISprinklerBehavior.Independent)
				throw new InvalidOperationException("Current sprinkler behavior does not allow independent sprinkler activation.");

			var wasVanillaQueryInProgress = VanillaPatches.IsVanillaQueryInProgress;
			VanillaPatches.FindGameLocationContextOverride = FindGameLocationContext.FlexibleSprinklersGetModifiedSprinklerCoverage;
			VanillaPatches.IsVanillaQueryInProgress = false;
			VanillaPatches.CurrentLocation = location;
			var layout = sprinkler.GetSprinklerTiles().ToArray();
			VanillaPatches.IsVanillaQueryInProgress = wasVanillaQueryInProgress;
			VanillaPatches.FindGameLocationContextOverride = null;
			return layout;
		}

		public Vector2[] GetUnmodifiedSprinklerCoverage(SObject sprinkler)
		{
			foreach (var sprinklerCoverageProvider in SprinklerCoverageProviders)
			{
				var coverage = sprinklerCoverageProvider(sprinkler);
				if (coverage != null)
					return coverage;
			}

			if (LineSprinklersApi != null)
			{
				if (LineSprinklersApi.GetSprinklerCoverage().TryGetValue(sprinkler.ParentSheetIndex, out Vector2[]? tilePositions))
					return tilePositions.Where(t => t != Vector2.Zero).ToArray();
			}

			if (BetterSprinklersApi != null)
			{
				if (BetterSprinklersApi.GetSprinklerCoverage().TryGetValue(sprinkler.ParentSheetIndex, out Vector2[]? tilePositions))
					return tilePositions.Where(t => t != Vector2.Zero).ToArray();
			}

			var wasVanillaQueryInProgress = VanillaPatches.IsVanillaQueryInProgress;
			VanillaPatches.FindGameLocationContextOverride = FindGameLocationContext.FlexibleSprinklersGetUnmodifiedSprinklerCoverage;
			VanillaPatches.IsVanillaQueryInProgress = true;
			var layout = sprinkler.GetSprinklerTiles()
				.Select(t => t - sprinkler.TileLocation)
				.Where(t => t != Vector2.Zero).ToArray();
			VanillaPatches.IsVanillaQueryInProgress = wasVanillaQueryInProgress;
			VanillaPatches.FindGameLocationContextOverride = null;
			return layout;
		}

		public void RegisterSprinklerTierProvider(Func<SObject, int?> provider)
			=> SprinklerTierProviders.Add(provider);

		public void RegisterSprinklerCoverageProvider(Func<SObject, Vector2[]> provider)
			=> SprinklerCoverageProviders.Add(provider);

		public void RegisterCustomWaterableTileProvider(Func<GameLocation, Vector2, bool?> provider)
			=> CustomWaterableTileProviders.Add(provider);

		public void DisplaySprinklerCoverage(float? seconds = null)
		{
			SprinklerCoverageAlpha = SprinklerCoverageAlphaDecrement * FPS * (seconds ?? Config.CoverageTimeInSeconds);
			SprinklerCoverageCurrentAnimationTime = 0f;
		}
	}
}