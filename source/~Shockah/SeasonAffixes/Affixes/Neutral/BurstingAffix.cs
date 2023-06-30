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
using Newtonsoft.Json;
using Shockah.CommonModCode.GMCM;
using Shockah.Kokoro;
using Shockah.Kokoro.GMCM;
using Shockah.Kokoro.Stardew;
using Shockah.Kokoro.UI;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Monsters;
using System;
using System.Collections.Generic;
using SObject = StardewValley.Object;

namespace Shockah.SeasonAffixes;

partial class ModConfig
{
	[JsonProperty] public float BurstingNoBombWeight { get; internal set; } = 0f;
	[JsonProperty] public float BurstingCherryBombWeight { get; internal set; } = 2f;
	[JsonProperty] public float BurstingBombWeight { get; internal set; } = 2f;
	[JsonProperty] public float BurstingMegaBombWeight { get; internal set; } = 1f;
}

internal sealed class BurstingAffix : BaseSeasonAffix, ISeasonAffix
{
	private const int CherryBombID = 286;
	private const int BombID = 287;
	private const int MegaBombID = 288;

	private static bool IsHarmonySetup = false;
	private static readonly WeakCounter<GameLocation> MonsterDropCallCounter = new();

	private static string ShortID => "Bursting";
	public TextureRectangle Icon => new(Game1.objectSpriteSheet, new(368, 176, 16, 16));

	public BurstingAffix() : base(ShortID, "neutral") { }

	public string LocalizedDescription
	{
		get
		{
			float totalWeight = Mod.Config.BurstingNoBombWeight + Mod.Config.BurstingCherryBombWeight + Mod.Config.BurstingBombWeight + Mod.Config.BurstingMegaBombWeight;
			if (Mod.Config.BurstingNoBombWeight > 0f)
				return Mod.Helper.Translation.Get($"{I18nPrefix}.description.chance", new { Chance = $"{(int)((1f - (Mod.Config.BurstingNoBombWeight / totalWeight)) * 100):0.##}%" });
			else
				return Mod.Helper.Translation.Get($"{I18nPrefix}.description.always");
		}
	}

	public int GetPositivity(OrdinalSeason season)
		=> 1;

	public int GetNegativity(OrdinalSeason season)
		=> 1;

	public double GetProbabilityWeight(OrdinalSeason season)
	{
		if (Mod.Config.BurstingCherryBombWeight <= 0f && Mod.Config.BurstingBombWeight <= 0f && Mod.Config.BurstingMegaBombWeight <= 0f)
			return 0; // invalid config, skipping affix
		return 1;
	}

	public IReadOnlySet<string> Tags { get; init; } = new HashSet<string> { VanillaSkill.MetalAspect, VanillaSkill.GemAspect, VanillaSkill.Combat.UniqueID };

	public void OnRegister()
		=> Apply(Mod.Harmony);

	public void SetupConfig(IManifest manifest)
	{
		var api = Mod.Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu")!;
		GMCMI18nHelper helper = new(api, Mod.ModManifest, Mod.Helper.Translation);
		helper.AddNumberOption($"{I18nPrefix}.config.weight.noBomb", () => Mod.Config.BurstingNoBombWeight, min: 0f, max: 10f, interval: 0.1f);
		helper.AddNumberOption($"{I18nPrefix}.config.weight.cherryBomb", () => Mod.Config.BurstingBombWeight, min: 0f, max: 10f, interval: 0.1f);
		helper.AddNumberOption($"{I18nPrefix}.config.weight.bomb", () => Mod.Config.BurstingCherryBombWeight, min: 0f, max: 10f, interval: 0.1f);
		helper.AddNumberOption($"{I18nPrefix}.config.weight.megaBomb", () => Mod.Config.BurstingMegaBombWeight, min: 0f, max: 10f, interval: 0.1f);
	}

	private void Apply(Harmony harmony)
	{
		if (IsHarmonySetup)
			return;
		IsHarmonySetup = true;

		harmony.TryPatchVirtual(
			monitor: Mod.Monitor,
			original: () => AccessTools.Method(typeof(GameLocation), nameof(GameLocation.monsterDrop)),
			prefix: new HarmonyMethod(AccessTools.Method(GetType(), nameof(GameLocation_monsterDrop_Prefix)), priority: Priority.First),
			finalizer: new HarmonyMethod(AccessTools.Method(GetType(), nameof(GameLocation_monsterDrop_Finalizer)), priority: Priority.Last)
		);

		Type? stopRugRemovalSObjectPatchesType = AccessTools.TypeByName("StopRugRemoval.HarmonyPatches.SObjectPatches, StopRugRemoval");
		if (stopRugRemovalSObjectPatchesType is not null)
		{
			harmony.TryPatch(
				monitor: Mod.Monitor,
				original: () => AccessTools.Method(stopRugRemovalSObjectPatchesType, "PrefixPlacementAction"),
				prefix: new HarmonyMethod(AccessTools.Method(GetType(), nameof(StopRugRemoval_SObjectPatches_PrefixPlacementAction_Prefix)))
			);
		}
	}

	private static bool TryToPlaceItemRecursively(int itemIndex, GameLocation location, Vector2 centerTile, Farmer player, int maxIterations = 16)
	{
		Queue<Vector2> queue = new();
		queue.Enqueue(centerTile);
		List<Vector2> list = new();
		for (int i = 0; i < maxIterations; i++)
		{
			if (queue.Count <= 0)
				break;

			Vector2 vector = queue.Dequeue();
			list.Add(vector);
			if (!location.isTileOccupied(vector, "ignoreMe") && IsTileOnClearAndSolidGround(location, vector) && location.isTileOccupiedByFarmer(vector) is null && location.doesTileHaveProperty((int)vector.X, (int)vector.Y, "Type", "Back") is not null && location.doesTileHaveProperty((int)vector.X, (int)vector.Y, "Type", "Back").Equals("Stone"))
			{
				PlaceItem(itemIndex, location, vector, player);
				return true;
			}

			Vector2[] directionsTileVectors = Utility.DirectionsTileVectors;
			foreach (Vector2 vector2 in directionsTileVectors)
				if (!list.Contains(vector + vector2))
					queue.Enqueue(vector + vector2);
		}

		return false;
	}

	private static bool IsTileOnClearAndSolidGround(GameLocation location, Vector2 v)
	{
		if (location.map.GetLayer("Back").Tiles[(int)v.X, (int)v.Y] is null)
			return false;
		if (location.map.GetLayer("Front").Tiles[(int)v.X, (int)v.Y] is not null || location.map.GetLayer("Buildings").Tiles[(int)v.X, (int)v.Y] is not null)
			return false;
		if (location is MineShaft && location.getTileIndexAt((int)v.X, (int)v.Y, "Back") == 77)
			return false;
		return true;
	}

	private static void PlaceItem(int itemIndex, GameLocation location, Vector2 point, Farmer player)
	{
		var bomb = new SObject(point, itemIndex, 1);
		bomb.placementAction(location, (int)point.X * 64, (int)point.Y * 64, player);
	}

	private static void GameLocation_monsterDrop_Prefix(GameLocation __instance, Monster __0, Farmer __3)
	{
		if (!Mod.IsAffixActive(a => a is BurstingAffix))
			return;

		uint counter = MonsterDropCallCounter.Push(__instance);
		if (counter != 1)
			return;

		WeightedRandom<int?> weightedRandom = new();
		weightedRandom.Add(new(Mod.Config.BurstingNoBombWeight, null));
		weightedRandom.Add(new(Mod.Config.BurstingCherryBombWeight, CherryBombID));
		weightedRandom.Add(new(Mod.Config.BurstingBombWeight, BombID));
		weightedRandom.Add(new(Mod.Config.BurstingMegaBombWeight, MegaBombID));

		int? itemToSpawn = weightedRandom.Next(Game1.random);
		if (itemToSpawn is null)
			return;

		TryToPlaceItemRecursively(itemToSpawn.Value, __instance, __0.getTileLocation(), __3, 50);
	}

	private static void GameLocation_monsterDrop_Finalizer(GameLocation __instance)
	{
		if (!Mod.IsAffixActive(a => a is BurstingAffix))
			return;
		MonsterDropCallCounter.Pop(__instance);
	}

	private static bool StopRugRemoval_SObjectPatches_PrefixPlacementAction_Prefix(SObject __0, GameLocation location, ref bool __result)
	{
		if (!Mod.IsAffixActive(a => a is BurstingAffix))
			return true;
		if (__0.bigCraftable.Value || !(__0.ParentSheetIndex is CherryBombID or BombID or MegaBombID))
			return true;
		if (MonsterDropCallCounter.Get(location) == 0)
			return true;

		__result = true;
		return false;
	}
}