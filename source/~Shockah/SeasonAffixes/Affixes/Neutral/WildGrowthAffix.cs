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
using Netcode;
using Newtonsoft.Json;
using Shockah.CommonModCode.GMCM;
using Shockah.Kokoro;
using Shockah.Kokoro.GMCM;
using Shockah.Kokoro.Stardew;
using Shockah.Kokoro.UI;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Network;
using StardewValley.TerrainFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using xTile.Dimensions;
using SObject = StardewValley.Object;

namespace Shockah.SeasonAffixes;

partial class ModConfig
{
	[JsonProperty] public float WildGrowthAdvanceChance { get; internal set; } = 1f;
	[JsonProperty] public float WildGrowthNewSeedChance { get; internal set; } = 0.5f;
}

internal sealed class WildGrowthAffix : BaseSeasonAffix, ISeasonAffix
{
	private static string ShortID => "WildGrowth";
	public string LocalizedDescription => Mod.Helper.Translation.Get($"{I18nPrefix}.description");
	public TextureRectangle Icon => new(Game1.objectSpriteSheet, new(336, 192, 16, 16));

	public WildGrowthAffix() : base(ShortID, "neutral") { }

	public int GetPositivity(OrdinalSeason season)
		=> 1;

	public int GetNegativity(OrdinalSeason season)
		=> 1;

	public double GetProbabilityWeight(OrdinalSeason season)
		=> Mod.Config.ChoicePeriod == AffixSetChoicePeriod.Day ? 0 : 1;

	public IReadOnlySet<string> Tags { get; init; } = new HashSet<string> { VanillaSkill.WoodcuttingAspect };

	public void OnActivate(AffixActivationContext context)
	{
		Mod.Helper.Events.GameLoop.DayStarted += OnDayStarted;
	}

	public void OnDeactivate(AffixActivationContext context)
	{
		Mod.Helper.Events.GameLoop.DayStarted -= OnDayStarted;
	}

	public void SetupConfig(IManifest manifest)
	{
		var api = Mod.Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu")!;
		GMCMI18nHelper helper = new(api, Mod.ModManifest, Mod.Helper.Translation);
		helper.AddNumberOption($"{I18nPrefix}.config.advanceChance", () => Mod.Config.WildGrowthAdvanceChance, min: 0.05f, max: 1f, interval: 0.05f, value => $"{(int)(value * 100):0.##}%");
		helper.AddNumberOption($"{I18nPrefix}.config.newSeedChance", () => Mod.Config.WildGrowthNewSeedChance, min: 0.05f, max: 1f, interval: 0.05f, value => $"{(int)(value * 100):0.##}%");
	}

	private void OnDayStarted(object? sender, DayStartedEventArgs e)
	{
		if (!Context.IsMainPlayer)
			return;

		var farm = Game1.getFarm();
		foreach (var tree in farm.terrainFeatures.Values.OfType<Tree>().ToList())
		{
			if (Game1.random.Next() < Mod.Config.WildGrowthAdvanceChance)
			{
				bool wasFertilized = tree.fertilized.Value;
				tree.fertilized.Value = true;
				tree.dayUpdate(new FakeLocation(farm), tree.currentTileLocation);
				tree.fertilized.Value = wasFertilized;
			}
			if (tree.growthStage.Value >= 5 && Game1.random.Next() < Mod.Config.WildGrowthNewSeedChance)
			{
				int xCoord = Game1.random.Next(-3, 4) + (int)tree.currentTileLocation.X;
				int yCoord = Game1.random.Next(-3, 4) + (int)tree.currentTileLocation.Y;
				Vector2 location = new(xCoord, yCoord);
				var noSpawn = farm.doesTileHaveProperty(xCoord, yCoord, "NoSpawn", "Back");
				if ((noSpawn is null || (!noSpawn.Equals("Tree") && !noSpawn.Equals("All") && !noSpawn.Equals("True"))) && farm.isTileLocationOpen(new Location(xCoord, yCoord)) && !farm.isTileOccupied(location) && farm.doesTileHaveProperty(xCoord, yCoord, "Water", "Back") is null && farm.isTileOnMap(location))
					farm.terrainFeatures.Add(location, new Tree(tree.treeType.Value, 0));
			}
		}
	}

	private sealed class FakeLocation : GameLocation
	{
		private static readonly Lazy<Action<GameLocation, NetVector2Dictionary<TerrainFeature, NetRef<TerrainFeature>>>> TerrainFeaturesSetter = new(() => AccessTools.Field(typeof(GameLocation), "terrainFeatures").EmitInstanceSetter<GameLocation, NetVector2Dictionary<TerrainFeature, NetRef<TerrainFeature>>>());
		private static readonly Lazy<Action<GameLocation, OverlaidDictionary>> ObjectsSetter = new(() => AccessTools.Field(typeof(GameLocation), "objects").EmitInstanceSetter<GameLocation, OverlaidDictionary>());

		private readonly GameLocation Wrapped;

		public FakeLocation(GameLocation wrapped)
		{
			this.Wrapped = wrapped;
			TerrainFeaturesSetter.Value(this, wrapped.terrainFeatures);
			ObjectsSetter.Value(this, wrapped.objects);
		}

		public override SObject? getObjectAt(int x, int y)
			=> Wrapped.getObjectAt(x, y);

		public override string? doesTileHaveProperty(int xTile, int yTile, string propertyName, string layerName)
			=> Wrapped.doesTileHaveProperty(xTile, yTile, propertyName, layerName);

		public override bool CanPlantTreesHere(int sapling_index, int tile_x, int tile_y)
			=> Wrapped.CanPlantTreesHere(sapling_index, tile_x, tile_y);

		public override bool isTileOccupied(Vector2 tileLocation, string? characterToIgnore = "", bool ignoreAllCharacters = false)
			=> Wrapped.isTileOccupied(tileLocation, characterToIgnore, ignoreAllCharacters);
	}
}