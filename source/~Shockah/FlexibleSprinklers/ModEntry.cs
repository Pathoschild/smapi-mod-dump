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
using Shockah.CommonModCode.GMCM;
using Shockah.Kokoro;
using Shockah.Kokoro.GMCM;
using Shockah.Kokoro.Stardew;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using SObject = StardewValley.Object;

namespace Shockah.FlexibleSprinklers
{
	public class ModEntry : BaseMod<ModConfig>, IFlexibleSprinklersApi
	{
		private const int PressureNozzleParentSheetIndex = 915;
		internal static readonly string LineSprinklersModID = "hootless.LineSprinklers";
		internal static readonly string BetterSprinklersModID = "Speeder.BetterSprinklers";
		internal static readonly string PrismaticToolsModID = "stokastic.PrismaticTools";

		private const int FPS = 60;
		private const float SprinklerCoverageAlphaDecrement = 1f / FPS; // 1f per second

		public static ModEntry Instance { get; private set; } = null!;
		private bool IsSlimeHutchWaterSpotsInstalled = false;

		public bool IsSprinklerBehaviorIndependent
			=> SprinklerBehavior is ISprinklerBehavior.Independent;

		internal ISprinklerBehavior SprinklerBehavior { get; private set; } = null!;
		private readonly List<Func<SObject, IReadOnlySet<IntPoint>?>> SprinklerCoverageProviders = [];
		private readonly List<Action<GameLocation, ISet<SprinklerInfo>>> SprinklerInfoInterceptors = [];
		internal List<Func<GameLocation, IntPoint, bool?>> CustomWaterableTileProviders { get; private set; } = [];
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
			helper.Events.World.FurnitureListChanged += OnFurnitureListChanged;
			helper.Events.World.TerrainFeatureListChanged += OnTerrainFeatureListChanged;
			helper.Events.World.LargeTerrainFeatureListChanged += OnLargeTerrainFeatureListChanged;
			helper.Events.Input.ButtonPressed += OnButtonPressed;

			RegisterCustomWaterableTileProvider((location, v) =>
			{
				if (location is not SlimeHutch)
					return null;

				if (IsSlimeHutchWaterSpotsInstalled)
				{
					var tileIndex = location.getTileIndexAt(v.X, v.Y, "Buildings");
					if (tileIndex is 2134 or 2135)
						return true;
				}
				else if (v.X == 16f && v.Y >= 6f && v.Y <= 9f)
				{
					return true;
				}

				return null;
			});
			RegisterCustomWaterableTileProvider((location, v) => Config.WaterPetBowl && location.getTileIndexAt(v.X, v.Y, "Buildings") == 1938 ? true : null);

			SetupSprinklerBehavior();
		}

		public override void MigrateConfig(ISemanticVersion? configVersion, ISemanticVersion modVersion)
		{
			if (configVersion is null)
				return;

			if (configVersion.IsOlderThan("2.0.0"))
			{
				if (Config.SprinklerBehavior == SprinklerBehaviorEnum.Flexible)
					Config.SprinklerBehavior = SprinklerBehaviorEnum.Cluster;
				else if (Config.SprinklerBehavior == SprinklerBehaviorEnum.FlexibleWithoutVanilla)
					Config.SprinklerBehavior = SprinklerBehaviorEnum.ClusterWithoutVanilla;
			}

			if (configVersion.IsOlderThan("2.1.0"))
			{
				if (Config.ExtensionData.TryGetValue("Tier1Power", out var powerToken))
					Config.Tier1Coverage = IntPoint.Zero.GetSpiralingTiles().Distinct().Take((int)powerToken).ToHashSet();
				if (Config.ExtensionData.TryGetValue("Tier2Power", out powerToken))
					Config.Tier2Coverage = IntPoint.Zero.GetSpiralingTiles().Distinct().Take((int)powerToken).ToHashSet();
				if (Config.ExtensionData.TryGetValue("Tier3Power", out powerToken))
					Config.Tier3Coverage = IntPoint.Zero.GetSpiralingTiles().Distinct().Take((int)powerToken).ToHashSet();
				if (Config.ExtensionData.TryGetValue("Tier4Power", out powerToken))
					Config.Tier4Coverage = IntPoint.Zero.GetSpiralingTiles().Distinct().Take((int)powerToken).ToHashSet();
				if (Config.ExtensionData.TryGetValue("Tier5Power", out powerToken))
					Config.Tier5Coverage = IntPoint.Zero.GetSpiralingTiles().Distinct().Take((int)powerToken).ToHashSet();
				if (Config.ExtensionData.TryGetValue("Tier6Power", out powerToken))
					Config.Tier6Coverage = IntPoint.Zero.GetSpiralingTiles().Distinct().Take((int)powerToken).ToHashSet();
				if (Config.ExtensionData.TryGetValue("Tier7Power", out powerToken))
					Config.Tier7Coverage = IntPoint.Zero.GetSpiralingTiles().Distinct().Take((int)powerToken).ToHashSet();
				if (Config.ExtensionData.TryGetValue("Tier8Power", out powerToken))
					Config.Tier8Coverage = IntPoint.Zero.GetSpiralingTiles().Distinct().Take((int)powerToken).ToHashSet();

				for (int i = 1; i <= 8; i++)
					Config.ExtensionData.Remove($"Tier{i}Power");
			}
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

			GameLocationMap map = new(location, CustomWaterableTileProviders);
			var sprinklers = GetAllSprinklers(location);
			var sprinklerTilesWithSteps = SprinklerBehavior.GetSprinklerTilesWithSteps(map, sprinklers);
			var alreadyShown = new HashSet<IntPoint>();
			foreach (var step in sprinklerTilesWithSteps)
			{
				if (Config.CoverageAnimationInSeconds > 0f && step.Time * Config.CoverageAnimationInSeconds > SprinklerCoverageCurrentAnimationTime)
					break;
				foreach (var sprinklerTile in step.Tiles)
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
			foreach (var @object in e.Added)
				if (@object.Value.IsSprinkler())
					OnSprinklerAdded(e.Location, @object.Value);
		}

		private void OnFurnitureListChanged(object? sender, FurnitureListChangedEventArgs e)
		{
			SprinklerBehavior.ClearCacheForMap(new GameLocationMap(e.Location, CustomWaterableTileProviders));
			foreach (var @object in e.Added)
				if (@object.IsSprinkler())
					OnSprinklerAdded(e.Location, @object);
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
			var @object = Game1.currentLocation.getObjectAtTile((int)tile.X, (int)tile.Y);
			if (@object is null || !@object.IsSprinkler())
				return;

			var heldItem = Game1.player.CurrentItem;
			if (heldItem?.ParentSheetIndex == PressureNozzleParentSheetIndex && @object.heldObject?.Value?.ParentSheetIndex != PressureNozzleParentSheetIndex)
				return;

			#if DEBUG
			SprinklerBehavior.ClearCache();
			#endif

			if (Config.ActivateOnAction && SprinklerBehavior is ISprinklerBehavior.Independent)
				ActivateSprinkler(@object);
			if (Config.ShowCoverageOnAction)
				DisplaySprinklerCoverage();
			Helper.Input.Suppress(e.Button);
		}

		private void OnSprinklerAdded(GameLocation location, SObject sprinkler)
		{
			if (Config.ActivateOnPlacement && SprinklerBehavior is ISprinklerBehavior.Independent)
				ActivateSprinkler(sprinkler);
			if (Config.ShowCoverageOnPlacement)
				DisplaySprinklerCoverage();
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

			helper.AddEnumOption("config.sprinklerBehavior", () => Config.SprinklerBehavior, isAllowed: b => b != SprinklerBehaviorEnum.Flexible && b != SprinklerBehaviorEnum.FlexibleWithoutVanilla);
			helper.AddBoolOption("config.ignoreRange", () => Config.IgnoreRange);
			helper.AddBoolOption("config.waterGardenPots", () => Config.WaterGardenPots);
			helper.AddBoolOption("config.waterPetBowl", () => Config.WaterPetBowl);
			helper.AddBoolOption("config.waterAtSprinkler", () => Config.WaterAtSprinkler);
			helper.AddBoolOption("config.compatibilityMode", () => Config.CompatibilityMode);

			helper.AddSectionTitle("config.cluster.section");
			helper.AddBoolOption("config.cluster.splitDisconnected", () => Config.SplitDisconnectedClusters);
			helper.AddEnumOption("config.cluster.ordering", () => Config.ClusterBehaviorClusterOrdering);
			helper.AddEnumOption("config.cluster.betweenClusterBalance", () => Config.ClusterBehaviorBetweenClusterBalanceMode);
			helper.AddEnumOption("config.cluster.inClusterBalance", () => Config.ClusterBehaviorInClusterBalanceMode);

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

			helper.AddSectionTitle("config.sprinklerCoverage.section");
			helper.AddSelectableGridOption("config.sprinklerCoverage.tier1", () => (IReadOnlySet<IntPoint>)Config.Tier1Coverage, value => Config.Tier1Coverage = value.ToHashSet());
			helper.AddSelectableGridOption("config.sprinklerCoverage.tier2", () => (IReadOnlySet<IntPoint>)Config.Tier2Coverage, value => Config.Tier2Coverage = value.ToHashSet());
			helper.AddSelectableGridOption("config.sprinklerCoverage.tier3", () => (IReadOnlySet<IntPoint>)Config.Tier3Coverage, value => Config.Tier3Coverage = value.ToHashSet());
			helper.AddSelectableGridOption("config.sprinklerCoverage.tier4", () => (IReadOnlySet<IntPoint>)Config.Tier4Coverage, value => Config.Tier4Coverage = value.ToHashSet());
			helper.AddSelectableGridOption("config.sprinklerCoverage.tier5", () => (IReadOnlySet<IntPoint>)Config.Tier5Coverage, value => Config.Tier5Coverage = value.ToHashSet());
			helper.AddSelectableGridOption("config.sprinklerCoverage.tier6", () => (IReadOnlySet<IntPoint>)Config.Tier6Coverage, value => Config.Tier6Coverage = value.ToHashSet());
			helper.AddSelectableGridOption("config.sprinklerCoverage.tier7", () => (IReadOnlySet<IntPoint>)Config.Tier7Coverage, value => Config.Tier7Coverage = value.ToHashSet());
			helper.AddSelectableGridOption("config.sprinklerCoverage.tier8", () => (IReadOnlySet<IntPoint>)Config.Tier8Coverage, value => Config.Tier8Coverage = value.ToHashSet());
		}

		private void SetupSprinklerBehavior()
		{
			ISprinklerBehavior sprinklerBehavior = Config.SprinklerBehavior switch
			{
				SprinklerBehaviorEnum.Cluster => new ClusterSprinklerBehavior(Config.ClusterBehaviorClusterOrdering, Config.ClusterBehaviorBetweenClusterBalanceMode, Config.ClusterBehaviorInClusterBalanceMode, Config.IgnoreRange, Config.SplitDisconnectedClusters, new VanillaSprinklerBehavior()),
				SprinklerBehaviorEnum.ClusterWithoutVanilla => new ClusterSprinklerBehavior(Config.ClusterBehaviorClusterOrdering, Config.ClusterBehaviorBetweenClusterBalanceMode, Config.ClusterBehaviorInClusterBalanceMode, Config.IgnoreRange, Config.SplitDisconnectedClusters, null),
				SprinklerBehaviorEnum.Vanilla => new VanillaSprinklerBehavior(),
				_ => throw new ArgumentException($"{nameof(SprinklerBehaviorEnum)} has an invalid value."),
			};

			if (Config.WaterAtSprinkler)
			{
				if (sprinklerBehavior is ISprinklerBehavior.Independent independent)
					sprinklerBehavior = new SelfWaterSprinklerBehavior.Independent(independent);
				else
					sprinklerBehavior = new SelfWaterSprinklerBehavior(sprinklerBehavior);
			}

			this.SprinklerBehavior = sprinklerBehavior;
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

			var sprinklers = GetAllSprinklers(location);
			if (sprinklers.Count == 0)
				return;

			Monitor.Log($"Activating all ({sprinklers.Count}) sprinkler(s) in location {GetNameForLocation(location)} with behavior {Config.SprinklerBehavior}.", LogLevel.Trace);
			GameLocationMap map = new(location, CustomWaterableTileProviders);
			
			var sprinklerTiles = SprinklerBehavior.GetSprinklerTiles(map, sprinklers);
			foreach (var sprinklerTile in sprinklerTiles)
				WaterTile(location, sprinkler: null, sprinklerTile);
			foreach (var sprinkler in location.Objects.Values.Where(o => o.IsSprinkler()))
				sprinkler.ApplySprinklerAnimation();
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

		public void ActivateSprinkler(SObject sprinkler)
		{
			if (SprinklerBehavior is not ISprinklerBehavior.Independent)
				throw new InvalidOperationException("Current sprinkler behavior does not allow independent sprinkler activation.");
			if (Game1.player.team.SpecialOrderRuleActive("NO_SPRINKLER"))
				return;
			if (!sprinkler.IsSprinkler())
				return;
			if (sprinkler.Location is null)
				return;

			foreach (var sprinklerTile in GetModifiedSprinklerCoverage(sprinkler))
				WaterTile(sprinkler.Location, sprinkler, sprinklerTile);
			sprinkler.ApplySprinklerAnimation();
		}

		internal SprinklerInfo GetSprinklerInfo(SObject sprinkler)
		{
			IntPoint sprinklerLocation = new((int)sprinkler.TileLocation.X, (int)sprinkler.TileLocation.Y);
			var layout = GetUnmodifiedSprinklerCoverage(sprinkler);
			var offsetLayout = layout.Select(t => t - sprinklerLocation).ToHashSet();

			if (offsetLayout.SetEquals(SprinklerInfo.DefaultTier1Coverage.Value))
				offsetLayout = Config.Tier1Coverage.ToHashSet();
			else if (offsetLayout.SetEquals(SprinklerInfo.DefaultTier2Coverage.Value))
				offsetLayout = Config.Tier2Coverage.ToHashSet();
			else if (offsetLayout.SetEquals(SprinklerInfo.DefaultTier3Coverage.Value))
				offsetLayout = Config.Tier3Coverage.ToHashSet();
			else if (offsetLayout.SetEquals(SprinklerInfo.DefaultTier4Coverage.Value))
				offsetLayout = Config.Tier4Coverage.ToHashSet();
			else if (offsetLayout.SetEquals(SprinklerInfo.DefaultTier5Coverage.Value))
				offsetLayout = Config.Tier5Coverage.ToHashSet();
			else if (offsetLayout.SetEquals(SprinklerInfo.DefaultTier6Coverage.Value))
				offsetLayout = Config.Tier6Coverage.ToHashSet();
			else if (offsetLayout.SetEquals(SprinklerInfo.DefaultTier7Coverage.Value))
				offsetLayout = Config.Tier7Coverage.ToHashSet();
			else if (offsetLayout.SetEquals(SprinklerInfo.DefaultTier8Coverage.Value))
				offsetLayout = Config.Tier8Coverage.ToHashSet();

			layout = offsetLayout.Select(t => t + sprinklerLocation).ToHashSet();
			return new SprinklerInfo(sprinkler, new(sprinklerLocation), layout);
		}

		public int GetSprinklerPower(SObject sprinkler)
			=> GetSprinklerInfo(sprinkler).Power;

		public IReadOnlySet<SprinklerInfo> GetAllSprinklers(GameLocation location)
			=> new InterceptingSprinklerProvider(new ObjectSprinklerProvider(), SprinklerInfoInterceptors).GetSprinklers(location);

		private static void WaterTile(GameLocation location, SObject? sprinkler, IntPoint point)
		{
			var can = new WateringCan();
			var tileVector = new Vector2(point.X, point.Y);

			// only calling this so other mods know a tile is watered via a sprinkler
			sprinkler?.ApplySprinkler(tileVector);

			if (location.terrainFeatures.TryGetValue(tileVector, out TerrainFeature feature))
				feature.performToolAction(can, 0, tileVector);
			if (location.Objects.TryGetValue(tileVector, out SObject @object))
				@object.performToolAction(can);
			location.performToolAction(can, point.X, point.Y);
		}

		public int GetSprinklerSpreadRange(int power)
			=> (int)Math.Floor(Math.Pow(power, 0.62) + 1);

		public int GetSprinklerFocusedRange(IntRectangle occupiedSpace, IReadOnlyCollection<IntPoint> coverage)
		{
			if (coverage.Count == 0)
				return 0;
			Vector2 center = new((occupiedSpace.Min.X + occupiedSpace.Max.X) / 2f, (occupiedSpace.Min.Y + occupiedSpace.Max.Y) / 2f);
			IReadOnlyList<Vector2> offsetCoverage = coverage.Select(c => new Vector2(c.X - center.X, c.Y - center.Y)).ToList();
			int manhattanDistance = (int)(offsetCoverage.Max(t => Math.Abs(t.X)) + offsetCoverage.Max(t => Math.Abs(t.Y)));
			Vector2 sum = offsetCoverage.Select(c => new Vector2(c.X, c.Y)).Aggregate((a, b) => a + b);
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

		public int GetSprinklerMaxRange(SprinklerInfo info)
		{
			int spreadRange = GetSprinklerSpreadRange(info.Power);
			int focusedRange = GetSprinklerFocusedRange(info.OccupiedSpace, info.Coverage);
			return Math.Max(spreadRange, focusedRange);
		}

		public bool IsTileInRangeOfSprinkler(SObject sprinkler, IntPoint tileLocation)
		{
			if (SprinklerBehavior is not ISprinklerBehavior.Independent)
				throw new InvalidOperationException("Current sprinkler behavior does not allow independent sprinkler activation.");
			IntPoint sprinklerTileLocation = new((int)sprinkler.TileLocation.X, (int)sprinkler.TileLocation.Y);

			var info = GetSprinklerInfo(sprinkler);
			var manhattanDistance = Math.Abs(tileLocation.X - sprinklerTileLocation.X) + Math.Abs(tileLocation.Y - sprinklerTileLocation.Y);
			if (manhattanDistance > GetSprinklerMaxRange(info))
			{
				if (!info.Coverage.Contains(tileLocation))
					return false;
			}
			return GetModifiedSprinklerCoverage(sprinkler).Contains(tileLocation);
		}

		public bool IsTileInRangeOfAnySprinkler(GameLocation location, IntPoint tileLocation)
			=> PrivateIsTileInRangeOfSprinklers(GetAllSprinklers(location), location, tileLocation);

		public bool IsTileInRangeOfSprinklers(IEnumerable<SObject> sprinklers, IntPoint tileLocation)
		{
			if (SprinklerBehavior is not ISprinklerBehavior.Independent)
				throw new InvalidOperationException("Current sprinkler behavior does not allow independent sprinkler activation.");

			var sprinklersSet = sprinklers.ToHashSet();
			if (sprinklersSet.Count == 0)
				return false;

			var location = sprinklersSet.First().Location;
			var sprinklerInfos = GetAllSprinklers(location)
				.Where(s => s.Owner is SObject @object && sprinklersSet.Contains(@object))
				.ToHashSet();
			return PrivateIsTileInRangeOfSprinklers(sprinklerInfos, location, tileLocation);
		}

		public bool IsTileInRangeOfSprinklers(IEnumerable<SprinklerInfo> sprinklers, GameLocation location, IntPoint tileLocation)
		{
			if (SprinklerBehavior is not ISprinklerBehavior.Independent)
				throw new InvalidOperationException("Current sprinkler behavior does not allow independent sprinkler activation.");
			return PrivateIsTileInRangeOfSprinklers(sprinklers.ToHashSet(), location, tileLocation);
		}

		public IReadOnlySet<IntPoint> GetAllTilesInRangeOfSprinklers(GameLocation location)
		{
			GameLocationMap map = new(location, CustomWaterableTileProviders);
			var sprinklers = GetAllSprinklers(location);
			return SprinklerBehavior.GetSprinklerTiles(map, sprinklers);
		}

		private bool PrivateIsTileInRangeOfSprinklers(IReadOnlySet<SprinklerInfo> sprinklers, GameLocation location, IntPoint tileLocation)
		{
			var sprinklersList = sprinklers.ToList();
			foreach (var sprinkler in sprinklersList)
			{
				Vector2 center = new((sprinkler.OccupiedSpace.Min.X + sprinkler.OccupiedSpace.Max.X) / 2f, (sprinkler.OccupiedSpace.Min.Y + sprinkler.OccupiedSpace.Max.Y) / 2f);
				var manhattanDistance = Math.Abs(tileLocation.X - center.X) + Math.Abs(tileLocation.Y - center.Y);
				if (manhattanDistance > GetSprinklerMaxRange(sprinkler))
				{
					if (SprinklerBehavior is not ISprinklerBehavior.Independent || !sprinkler.Coverage.Contains(tileLocation))
						continue;
				}
				goto afterSimpleCheck;
			}

			return false;
			afterSimpleCheck:;

			GameLocationMap map = new(location, CustomWaterableTileProviders);
			return SprinklerBehavior.GetSprinklerTiles(map, sprinklers).Contains(tileLocation);
		}

		public IReadOnlySet<IntPoint> GetModifiedSprinklerCoverage(SObject sprinkler)
		{
			if (SprinklerBehavior is not ISprinklerBehavior.Independent)
				throw new InvalidOperationException("Current sprinkler behavior does not allow independent sprinkler activation.");

			var wasVanillaQueryInProgress = VanillaPatches.IsVanillaQueryInProgress;
			VanillaPatches.IsVanillaQueryInProgress = false;
			var layout = sprinkler.GetSprinklerTiles().Select(t => new IntPoint((int)t.X, (int)t.Y)).ToHashSet();
			VanillaPatches.IsVanillaQueryInProgress = wasVanillaQueryInProgress;
			return layout;
		}

		public IReadOnlySet<IntPoint> GetUnmodifiedSprinklerCoverage(SObject sprinkler)
		{
			foreach (var sprinklerCoverageProvider in SprinklerCoverageProviders)
			{
				var coverage = sprinklerCoverageProvider(sprinkler);
				if (coverage != null)
					return coverage;
			}

			if (LineSprinklersApi != null)
			{
				if (LineSprinklersApi.GetSprinklerCoverage().TryGetValue(sprinkler.ParentSheetIndex, out var tilePositions))
					return tilePositions
						.Where(t => t != Vector2.Zero)
						.Select(t => t + sprinkler.TileLocation)
						.Select(t => new IntPoint((int)t.X, (int)t.Y))
						.ToHashSet();
			}

			if (BetterSprinklersApi != null)
			{
				if (BetterSprinklersApi.GetSprinklerCoverage().TryGetValue(sprinkler.ParentSheetIndex, out var tilePositions))
					return tilePositions
						.Where(t => t != Vector2.Zero)
						.Select(t => t + sprinkler.TileLocation)
						.Select(t => new IntPoint((int)t.X, (int)t.Y))
						.ToHashSet();
			}

			var wasVanillaQueryInProgress = VanillaPatches.IsVanillaQueryInProgress;
			VanillaPatches.IsVanillaQueryInProgress = true;
			var layout = sprinkler.GetSprinklerTiles()
				.Where(t => t != sprinkler.TileLocation)
				.Select(t => new IntPoint((int)t.X, (int)t.Y))
				.ToHashSet();
			VanillaPatches.IsVanillaQueryInProgress = wasVanillaQueryInProgress;
			return layout;
		}

		public void RegisterSprinklerCoverageProvider(Func<SObject, IReadOnlySet<IntPoint>?> provider)
			=> SprinklerCoverageProviders.Add(provider);

		public void RegisterSprinklerInfoInterceptor(Action<GameLocation, ISet<SprinklerInfo>> interceptor)
			=> SprinklerInfoInterceptors.Add(interceptor);

		public void RegisterCustomWaterableTileProvider(Func<GameLocation, IntPoint, bool?> provider)
			=> CustomWaterableTileProviders.Add(provider);

		public void DisplaySprinklerCoverage(float? seconds = null)
		{
			SprinklerCoverageAlpha = SprinklerCoverageAlphaDecrement * FPS * (seconds ?? Config.CoverageTimeInSeconds);
			SprinklerCoverageCurrentAnimationTime = 0f;
		}
	}
}