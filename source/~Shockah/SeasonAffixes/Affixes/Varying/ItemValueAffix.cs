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
using Shockah.Kokoro.UI;
using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using SObject = StardewValley.Object;

namespace Shockah.SeasonAffixes;

partial class ModConfig
{
	[JsonProperty] public float AgricultureValue { get; internal set; } = 2f;
	[JsonProperty] public float RanchingValue { get; internal set; } = 2f;
	[JsonProperty] public float SeafoodValue { get; internal set; } = 2f;
}

internal sealed class ItemValueAffix : BaseSeasonAffix, ISeasonAffix
{
	private static bool IsHarmonySetup = false;

	public string LocalizedDescription => Mod.Helper.Translation.Get($"{I18nPrefix}.description", new { Value = $"{(int)(ValueGetter() * 100):0.##}%" });
	public TextureRectangle Icon => IconProvider();
	public IReadOnlySet<string> Tags { get; init; }

	private Func<TextureRectangle> IconProvider { get; init; }
	private Func<SObject, bool> ItemPredicate { get; init; }
	private Func<float> ValueGetter { get; init; }
	private Action<float> ValueSetter { get; init; }
	private Func<OrdinalSeason, double> ProbabilityWeightProvider { get; init; }

	public ItemValueAffix(
		string shortID,
		IReadOnlySet<string> tags,
		Func<TextureRectangle> iconProvider,
		Func<SObject, bool> itemPredicate,
		Func<float> valueGetter,
		Action<float> valueSetter,
		Func<OrdinalSeason, double>? probabilityWeightProvider = null
	) : base(shortID, "positive")
	{
		this.Tags = tags;
		this.IconProvider = iconProvider;
		this.ItemPredicate = itemPredicate;
		this.ValueGetter = valueGetter;
		this.ValueSetter = valueSetter;
		this.ProbabilityWeightProvider = probabilityWeightProvider ?? (season => 1);
	}

	public int GetPositivity(OrdinalSeason season)
		=> ValueGetter() > 1f ? 1 : 0;

	public int GetNegativity(OrdinalSeason season)
		=> ValueGetter() < 1f ? 1 : 0;

	public double GetProbabilityWeight(OrdinalSeason season)
		=> ProbabilityWeightProvider(season);

	public void OnRegister()
		=> Apply(Mod.Harmony);

	public void SetupConfig(IManifest manifest)
	{
		var api = Mod.Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu")!;
		GMCMI18nHelper helper = new(api, Mod.ModManifest, Mod.Helper.Translation);
		helper.AddNumberOption($"{I18nPrefix}.config.value", ValueGetter, ValueSetter, min: 0f, max: 4f, interval: 0.05f, value => $"{(int)(value * 100):0.##}%");
	}

	private void Apply(Harmony harmony)
	{
		if (IsHarmonySetup)
			return;
		IsHarmonySetup = true;

		harmony.TryPatch(
			monitor: Mod.Monitor,
			original: () => AccessTools.Method(typeof(SObject), nameof(SObject.sellToStorePrice)),
			postfix: new HarmonyMethod(AccessTools.Method(GetType(), nameof(SObject_sellToStorePrice_Postfix)))
		);
	}

	private static void SObject_sellToStorePrice_Postfix(SObject __instance, ref int __result)
	{
		if (__result <= 0)
			return;
		foreach (var affix in Mod.ActiveAffixes.OfType<ItemValueAffix>())
		{
			if (!affix.ItemPredicate(__instance))
				continue;
			__result = (int)Math.Round(__result * affix.ValueGetter());
			break;
		}
	}
}