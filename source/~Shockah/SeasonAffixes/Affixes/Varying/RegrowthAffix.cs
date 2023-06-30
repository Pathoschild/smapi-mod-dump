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
using Nanoray.Shrike;
using Nanoray.Shrike.Harmony;
using Shockah.Kokoro;
using Shockah.Kokoro.Stardew;
using Shockah.Kokoro.UI;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;

namespace Shockah.SeasonAffixes;

internal sealed class RegrowthAffix : BaseVariantedSeasonAffix, ISeasonAffix
{
	private static bool IsHarmonySetup = false;
	private static readonly WeakCounter<Crop> GetSourceRectCallCounter = new();
	private static readonly ConditionalWeakTable<Crop, StructRef<int>> OldPhaseToShow = new();

	private static string ShortPositiveID => "Regrowth";
	private static string ShortNegativeID => "PoorYields";
	public string LocalizedDescription => Mod.Helper.Translation.Get($"{I18nPrefix}.description");

	public TextureRectangle Icon
		=> Variant == AffixVariant.Positive
		? new(Game1.objectSpriteSheet, new(16, 528, 16, 16))
		: new(Game1.objectSpriteSheet, new(0, 0, 16, 16));

	private static readonly Lazy<Func<Crop, Vector2>> TilePositionGetter = new(() => AccessTools.Field(typeof(Crop), "tilePosition").EmitInstanceGetter<Crop, Vector2>());

	private readonly HashSet<int> AdjustedCrops = new();

	public RegrowthAffix(AffixVariant variant) : base(variant == AffixVariant.Positive ? ShortPositiveID : ShortNegativeID, variant)
	{
		Tags = Variant == AffixVariant.Positive
			? new HashSet<string> { VanillaSkill.CropsAspect, VanillaSkill.FlowersAspect }
			: new HashSet<string> { VanillaSkill.CropsAspect };
	}

	public int GetPositivity(OrdinalSeason season)
		=> Variant == AffixVariant.Positive ? 1 : 0;

	public int GetNegativity(OrdinalSeason season)
		=> Variant == AffixVariant.Negative ? 1 : 0;

	public IReadOnlySet<string> Tags { get; init; }

	public void OnRegister()
		=> Apply(Mod.Harmony);

	public double GetProbabilityWeight(OrdinalSeason season)
		=> Mod.Config.WinterCrops || season.Season != Season.Winter ? 1 : 0;

	public void OnActivate(AffixActivationContext context)
	{
		Mod.Helper.Events.Content.AssetRequested += OnAssetRequested;
		Mod.Helper.GameContent.InvalidateCache("Data\\Crops");
		UpdateExistingCrops();
	}

	public void OnDeactivate(AffixActivationContext context)
	{
		Mod.Helper.Events.Content.AssetRequested -= OnAssetRequested;
		Mod.Helper.GameContent.InvalidateCache("Data\\Crops");
		UpdateExistingCrops();
	}

	private void Apply(Harmony harmony)
	{
		if (IsHarmonySetup)
			return;
		IsHarmonySetup = true;

		harmony.TryPatch(
			monitor: Mod.Monitor,
			original: () => AccessTools.Method(typeof(Crop), nameof(Crop.getSourceRect)),
			prefix: new HarmonyMethod(AccessTools.Method(GetType(), nameof(Crop_getSourceRect_Prefix))),
			postfix: new HarmonyMethod(AccessTools.Method(GetType(), nameof(Crop_getSourceRect_Postfix))),
			transpiler: new HarmonyMethod(AccessTools.Method(GetType(), nameof(Crop_getSourceRect_Transpiler)))
		);
	}

	private void OnAssetRequested(object? sender, AssetRequestedEventArgs e)
	{
		if (!e.Name.IsEquivalentTo("Data\\Crops"))
			return;
		e.Edit(asset =>
		{
			AdjustedCrops.Clear();

			var data = asset.AsDictionary<int, string>();
			foreach (var kvp in data.Data)
			{
				string[] split = kvp.Value.Split('/');
				if (Variant == AffixVariant.Positive)
				{
					if (split[4] == "-1")
					{
						int totalGrowthDays = split[0].Split(" ").Select(growthStage => int.Parse(growthStage)).Sum();
						split[4] = $"{(int)Math.Ceiling(totalGrowthDays / 3.0)}";
						data.Data[kvp.Key] = string.Join("/", split);
						AdjustedCrops.Add(kvp.Key);
					}
				}
				else
				{
					if (split[4] != "-1")
					{
						split[4] = "-1";
						data.Data[kvp.Key] = string.Join("/", split);
						AdjustedCrops.Add(kvp.Key);
					}
				}
			}
		}, priority: AssetEditPriority.Late);
	}

	private static void UpdateExistingCrops()
	{
		foreach (var location in GameExt.GetAllLocations())
		{
			foreach (var terrainFeature in location.terrainFeatures.Values)
				if (terrainFeature is HoeDirt dirt)
					if (dirt.crop is not null)
						UpdateCrop(dirt.crop);
			foreach (var @object in location.Objects.Values)
				if (@object is IndoorPot pot)
					if (pot.hoeDirt.Value?.crop is not null)
						UpdateCrop(pot.hoeDirt.Value.crop);
		}
	}

	private static void UpdateCrop(Crop crop)
	{
		Dictionary<int, string> allCropData = Game1.content.Load<Dictionary<int, string>>("Data\\Crops");
		if (!allCropData.TryGetValue(crop.netSeedIndex.Value, out var cropData))
			return;
		string[] split = cropData.Split('/');
		crop.regrowAfterHarvest.Value = Convert.ToInt32(split[4]);
		if (crop.regrowAfterHarvest.Value == -1)
			crop.fullyGrown.Value = false;
		crop.updateDrawMath(TilePositionGetter.Value(crop));
	}

	private static void Crop_getSourceRect_Prefix(Crop __instance)
	{
		uint counter = GetSourceRectCallCounter.Push(__instance);
		if (counter != 1)
			return;

		OldPhaseToShow.AddOrUpdate(__instance, new(__instance.phaseToShow.Value));

		if (!__instance.fullyGrown.Value)
			return;
		var affix = Mod.ActiveAffixes.OfType<RegrowthAffix>().FirstOrDefault(affix => affix.Variant == AffixVariant.Positive);
		if (affix is null)
			return;
		if (!affix.AdjustedCrops.Contains(__instance.netSeedIndex.Value))
			return;

		bool harvestable = __instance.currentPhase.Value >= __instance.phaseDays.Count - 1 && (!__instance.fullyGrown.Value || __instance.dayOfCurrentPhase.Value <= 0);
		__instance.phaseToShow.Value = __instance.phaseDays.Count - (harvestable ? 1 : 2);
	}

	private static void Crop_getSourceRect_Postfix(Crop __instance)
	{
		uint counter = GetSourceRectCallCounter.Pop(__instance);
		if (counter != 0)
			return;
		if (OldPhaseToShow.TryGetValue(__instance, out var oldPhaseToShow))
			__instance.phaseToShow.Value = oldPhaseToShow.Value;
	}

	private static IEnumerable<CodeInstruction> Crop_getSourceRect_Transpiler(IEnumerable<CodeInstruction> instructions, MethodBase originalMethod)
	{
		try
		{
			return new SequenceBlockMatcher<CodeInstruction>(instructions)
				.Find(
					ILMatches.Ldarg(0),
					ILMatches.Ldfld(AccessTools.Field(typeof(Crop), nameof(Crop.fullyGrown))),
					ILMatches.Call("op_Implicit")
				)
				.Insert(
					SequenceMatcherPastBoundsDirection.After, SequenceMatcherInsertionResultingBounds.IncludingInsertion,
					new CodeInstruction(OpCodes.Ldarg_0),
					new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(RegrowthAffix), nameof(Crop_getSourceRect_Transpiler_FullyGrown)))
				)
				.AllElements();
		}
		catch (Exception ex)
		{
			Mod.Monitor.Log($"Could not patch method {originalMethod} - {Mod.ModManifest.Name} probably won't work.\nReason: {ex}", LogLevel.Error);
			return instructions;
		}
	}

	public static bool Crop_getSourceRect_Transpiler_FullyGrown(bool fullyGrown, Crop crop)
	{
		if (!fullyGrown)
			return false;
		var affix = Mod.ActiveAffixes.OfType<RegrowthAffix>().FirstOrDefault(affix => affix.Variant == AffixVariant.Positive);
		if (affix is null)
			return true;
		if (!affix.AdjustedCrops.Contains(crop.netSeedIndex.Value))
			return true;
		return false;
	}
}