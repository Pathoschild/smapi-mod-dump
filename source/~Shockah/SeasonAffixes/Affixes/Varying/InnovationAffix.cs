/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Shockah/Stardew-Valley-Mods
**
*************************************************/

using Newtonsoft.Json;
using Shockah.CommonModCode.GMCM;
using Shockah.Kokoro;
using Shockah.Kokoro.GMCM;
using Shockah.Kokoro.Stardew;
using Shockah.Kokoro.UI;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using SObject = StardewValley.Object;

namespace Shockah.SeasonAffixes;

partial class ModConfig
{
	[JsonProperty] public float InnovationDecrease { get; internal set; } = 0.25f;
	[JsonProperty] public float RustIncrease { get; internal set; } = 0.5f;
}

internal sealed class InnovationAffix : BaseVariantedSeasonAffix, ISeasonAffix
{
	private static string ShortPositiveID => "Innovation";
	private static string ShortNegativeID => "Rust";

	public string LocalizedDescription
		=> Variant == AffixVariant.Positive
		? Mod.Helper.Translation.Get($"{I18nPrefix}.description", new { Decrease = $"{(int)(Mod.Config.InnovationDecrease * 100):0.##}%" })
		: Mod.Helper.Translation.Get($"{I18nPrefix}.description", new { Increase = $"{(int)(Mod.Config.RustIncrease * 100):0.##}%" });

	public TextureRectangle Icon
		=> Variant == AffixVariant.Positive
		? new(Game1.objectSpriteSheet, new(32, 80, 16, 16))
		: new(Game1.objectSpriteSheet, new(256, 64, 16, 16));

	private List<WeakReference<SObject>> AffixApplied = new();

	public InnovationAffix(AffixVariant variant) : base(variant == AffixVariant.Positive ? ShortPositiveID : ShortNegativeID, variant) { }

	public int GetPositivity(OrdinalSeason season)
		=> Variant == AffixVariant.Positive ? 1 : 0;

	public int GetNegativity(OrdinalSeason season)
		=> Variant == AffixVariant.Negative ? 1 : 0;

	public void OnActivate(AffixActivationContext context)
	{
		AffixApplied.Clear();
		Mod.Helper.Events.GameLoop.DayEnding += OnDayEnding;
		MachineTracker.MachineChangedEvent += OnMachineChanged;
	}

	public void OnDeactivate(AffixActivationContext context)
	{
		Mod.Helper.Events.GameLoop.DayEnding -= OnDayEnding;
		MachineTracker.MachineChangedEvent -= OnMachineChanged;
	}

	public void SetupConfig(IManifest manifest)
	{
		var api = Mod.Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu")!;
		GMCMI18nHelper helper = new(api, Mod.ModManifest, Mod.Helper.Translation);
		if (Variant == AffixVariant.Positive)
			helper.AddNumberOption($"{I18nPrefix}.config.decrease", () => Mod.Config.InnovationDecrease, min: 0.05f, max: 0.9f, interval: 0.05f, value => $"{(int)(value * 100):0.##}%");
		else
			helper.AddNumberOption($"{I18nPrefix}.config.increase", () => Mod.Config.RustIncrease, min: 0.05f, max: 2f, interval: 0.05f, value => $"{(int)(value * 100):0.##}%");
	}

	private void OnDayEnding(object? sender, DayEndingEventArgs e)
	{
		AffixApplied = AffixApplied
			.Where(r => r.TryGetTarget(out _))
			.ToList();
	}

	private void OnMachineChanged(GameLocation location, SObject machine, MachineProcessingState? oldState, MachineProcessingState? newState)
	{
		if (!Context.IsMainPlayer)
			return;
		if (oldState is null || newState is null)
			return;

		var existingIndex = AffixApplied.FirstIndex(weakMachine => weakMachine.TryGetTarget(out var appliedMachine) && ReferenceEquals(machine, appliedMachine));
		if (existingIndex is not null)
		{
			AffixApplied.RemoveAt(existingIndex.Value);
			return;
		}

		if (!newState.Value.ReadyForHarvest && newState.Value.MinutesUntilReady > 0 && (oldState.Value.ReadyForHarvest || oldState.Value.MinutesUntilReady < newState.Value.MinutesUntilReady))
		{
			AffixApplied.Add(new(machine));
			machine.MinutesUntilReady = (int)Math.Ceiling(machine.MinutesUntilReady * (1f + (Variant == AffixVariant.Positive ? -Mod.Config.InnovationDecrease : Mod.Config.RustIncrease)));
		}
	}
}