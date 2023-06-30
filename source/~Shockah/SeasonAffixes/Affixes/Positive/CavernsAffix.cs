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
using Newtonsoft.Json;
using Shockah.CommonModCode.GMCM;
using Shockah.Kokoro;
using Shockah.Kokoro.GMCM;
using Shockah.Kokoro.Stardew;
using Shockah.Kokoro.UI;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Locations;
using System;
using System.Collections.Generic;
using System.Linq;
using SObject = StardewValley.Object;

namespace Shockah.SeasonAffixes;

partial class ModConfig
{
	[JsonProperty] public int CavernsMinFloors { get; internal set; } = 2;
	[JsonProperty] public int CavernsMaxFloors { get; internal set; } = 3;
	[JsonProperty] public int CavernsMinGems { get; internal set; } = 12;
	[JsonProperty] public int CavernsMaxGems { get; internal set; } = 20;
	[JsonProperty] public bool CavernsAllowPrismaticShard { get; internal set; } = false;
}

internal sealed class CavernsAffix : BaseSeasonAffix, ISeasonAffix
{
	private static bool IsHarmonySetup = false;

	private static string ShortID => "Caverns";
	public string LocalizedDescription => Mod.Helper.Translation.Get($"{I18nPrefix}.description");
	public TextureRectangle Icon => new(Game1.objectSpriteSheet, new(128, 208, 16, 16));

	private static readonly Lazy<Func<MineShaft, bool>> IsDinoAreaGetter = new(() => AccessTools.Property(typeof(MineShaft), "isDinoArea").EmitInstanceGetter<MineShaft, bool>());
	private static readonly Lazy<Func<MineShaft, bool>> IsMonsterAreaGetter = new(() => AccessTools.Property(typeof(MineShaft), "isMonsterArea").EmitInstanceGetter<MineShaft, bool>());
	private static readonly Lazy<Func<MineShaft, int>> StonesLeftOnThisLevelGetter = new(() => AccessTools.Property(typeof(MineShaft), "stonesLeftOnThisLevel").EmitInstanceGetter<MineShaft, int>());
	private static readonly Lazy<Action<MineShaft, int>> StonesLeftOnThisLevelSetter = new(() => AccessTools.Property(typeof(MineShaft), "stonesLeftOnThisLevel").EmitInstanceSetter<MineShaft, int>());
	private static readonly PerScreen<HashSet<int>> GemCavernFloors = new(() => new());

	public CavernsAffix() : base(ShortID, "positive") { }

	public int GetPositivity(OrdinalSeason season)
		=> 1;

	public int GetNegativity(OrdinalSeason season)
		=> 0;

	public IReadOnlySet<string> Tags { get; init; } = new HashSet<string> { VanillaSkill.GemAspect };

	public void OnRegister()
		=> Apply(Mod.Harmony);

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
		helper.AddNumberOption($"{I18nPrefix}.config.minFloors", () => Mod.Config.CavernsMinFloors, min: 1, max: 40, interval: 1);
		helper.AddNumberOption($"{I18nPrefix}.config.maxFloors", () => Mod.Config.CavernsMaxFloors, min: 1, max: 40, interval: 1);
		helper.AddNumberOption($"{I18nPrefix}.config.minGems", () => Mod.Config.CavernsMinGems, min: 1, max: 60, interval: 1);
		helper.AddNumberOption($"{I18nPrefix}.config.maxGems", () => Mod.Config.CavernsMaxGems, min: 1, max: 60, interval: 1);
		helper.AddBoolOption($"{I18nPrefix}.config.allowPrismaticShard", () => Mod.Config.CavernsAllowPrismaticShard);
	}

	private void Apply(Harmony harmony)
	{
		if (IsHarmonySetup)
			return;
		IsHarmonySetup = true;

		harmony.TryPatch(
			monitor: Mod.Monitor,
			original: () => AccessTools.Method(typeof(MineShaft), "populateLevel"),
			postfix: new HarmonyMethod(AccessTools.Method(GetType(), nameof(MineShaft_populateLevel_Postfix)))
		);
	}

	private void OnDayStarted(object? sender, DayStartedEventArgs e)
	{
		GemCavernFloors.Value.Clear();

		var shafts = Enumerable.Range(1, MineShaft.bottomOfMineLevel)
			.Select(i => new MineShaft(i))
			.Where(IsPotentialPreLoadGemCavernFloor)
			.Select(shaft =>
			{
				shaft.loadLevel(shaft.mineLevel);
				return shaft;
			})
			.Where(IsPotentialPostLoadGemCavernFloor)
			.ToList();

		Random random = new((int)Game1.stats.DaysPlayed + (int)Game1.uniqueIDForThisGame / 2);
		int totalFloors = Math.Min(random.Next(Mod.Config.CavernsMinFloors, Mod.Config.CavernsMaxFloors + 1), shafts.Count);

		for (int i = 0; i < totalFloors; i++)
		{
			var shaft = random.NextElement(shafts);
			shafts.Remove(shaft);
			GemCavernFloors.Value.Add(shaft.mineLevel);
		}
	}

	private static bool IsPotentialPreLoadGemCavernFloor(MineShaft shaft)
	{
		if (shaft.mineLevel > MineShaft.bottomOfMineLevel)
			return false;
		if (shaft.mineLevel % 10 == 0)
			return false;
		return true;
	}

	private static bool IsPotentialPostLoadGemCavernFloor(MineShaft shaft)
	{
		if (shaft.isLevelSlimeArea() || IsMonsterAreaGetter.Value(shaft) || IsDinoAreaGetter.Value(shaft))
			return false;
		return true;
	}

	private static void MineShaft_populateLevel_Postfix(MineShaft __instance)
	{
		if (!Context.IsMainPlayer)
			return;
		if (!Mod.IsAffixActive(a => a is CavernsAffix))
			return;
		if (!GemCavernFloors.Value.Contains(__instance.mineLevel))
			return;

		GemCavernFloors.Value.Remove(__instance.mineLevel);

		List<IntPoint> possibleTiles = new();
		for (int y = 0; y < __instance.Map.DisplayHeight / Game1.tileSize; y++)
		{
			for (int x = 0; x < __instance.Map.DisplayWidth / Game1.tileSize; x++)
			{
				if (__instance.isTileClearForMineObjects(new(x, y)))
					possibleTiles.Add(new(x, y));
			}
		}

		Random random = new((int)Game1.stats.DaysPlayed + __instance.mineLevel * 150 + (int)Game1.uniqueIDForThisGame / 2);
		int gemsToSpawn = Math.Min(random.Next(Mod.Config.CavernsMinGems, Mod.Config.CavernsMaxGems + 1), (int)(possibleTiles.Count * 0.75f));

		WeightedRandom<int> weightedRandom = new();
		foreach (var (itemIndex, price) in GetGemDefinitions())
			weightedRandom.Add(new(1.0 / Math.Sqrt(price), itemIndex));

		for (int i = 0; i < gemsToSpawn; i++)
		{
			IntPoint point = Game1.random.NextElement(possibleTiles);
			possibleTiles.Remove(point);

			int gemIndex = weightedRandom.Next(random);
			int? stoneIndex = GetStoneIndexForGem(gemIndex);

			if (stoneIndex is null)
			{
				__instance.dropObject(new SObject(new(point.X, point.Y), gemIndex, null, canBeSetDown: false, canBeGrabbed: true, isHoedirt: false, isSpawnedObject: true), new(point.X * Game1.tileSize, point.Y * Game1.tileSize), Game1.viewport, initialPlacement: true);
			}
			else
			{
				var stone = new SObject(new(point.X, point.Y), stoneIndex.Value, "Stone", canBeSetDown: true, canBeGrabbed: false, isHoedirt: false, isSpawnedObject: false)
				{
					MinutesUntilReady = 5
				};
				__instance.Objects.Add(new(point.X, point.Y), stone);
				StonesLeftOnThisLevelSetter.Value(__instance, StonesLeftOnThisLevelGetter.Value(__instance) + 1);
			}
		}
	}

	private static List<(int ItemIndex, int Price)> GetGemDefinitions()
	{
		List<(int ItemIndex, int Price)> results = new();
		var data = Game1.content.Load<Dictionary<int, string>>("Data\\ObjectInformation");
		foreach (var (itemIndex, rawItemData) in data)
		{
			if (itemIndex == 74 && !Mod.Config.CavernsAllowPrismaticShard)
				continue;
			var split = rawItemData.Split("/");
			if (split[3] != "Minerals -2")
				continue;
			results.Add((ItemIndex: itemIndex, Price: int.Parse(split[1])));
		}
		return results;
	}

	private static int? GetStoneIndexForGem(int gemIndex)
		=> gemIndex switch
		{
			72 => 2, // Diamond
			64 => 4, // Ruby
			70 => 6, // Jade
			66 => 8, // Amethyst
			68 => 10, // Topaz
			60 => 12, // Emerald
			62 => 14, // Aquamarine
			_ => null,
		};
}