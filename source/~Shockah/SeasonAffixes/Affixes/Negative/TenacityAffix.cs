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
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;

namespace Shockah.SeasonAffixes;

partial class ModConfig
{
	[JsonProperty] public float TenacityValue { get; internal set; } = 1.5f;
}

internal sealed class TenacityAffix : BaseSeasonAffix, ISeasonAffix
{
	private static readonly Lazy<Func<BobberBar, bool>> BobberBarBobberInBarGetter = new(() => AccessTools.Field(typeof(BobberBar), "bobberInBar").EmitInstanceGetter<BobberBar, bool>());
	private static readonly Lazy<Func<BobberBar, bool>> BobberBarHandledFishResultGetter = new(() => AccessTools.Field(typeof(BobberBar), "handledFishResult").EmitInstanceGetter<BobberBar, bool>());
	private static readonly Lazy<Func<BobberBar, float>> BobberBarDistanceFromCatchingGetter = new(() => AccessTools.Field(typeof(BobberBar), "distanceFromCatching").EmitInstanceGetter<BobberBar, float>());
	private static readonly Lazy<Action<BobberBar, float>> BobberBarDistanceFromCatchingSetter = new(() => AccessTools.Field(typeof(BobberBar), "distanceFromCatching").EmitInstanceSetter<BobberBar, float>());

	private static string ShortID => "Tenacity";
	public string LocalizedDescription => Mod.Helper.Translation.Get($"{I18nPrefix}.description", new { Value = $"{Mod.Config.TenacityValue:0.##}x" });
	public TextureRectangle Icon => new(Game1.objectSpriteSheet, new(368, 80, 16, 16));

	public TenacityAffix() : base(ShortID, "negative") { }

	public int GetPositivity(OrdinalSeason season)
		=> Mod.Config.TenacityValue < 1f ? 1 : 0;

	public int GetNegativity(OrdinalSeason season)
		=> Mod.Config.TenacityValue > 1f ? 1 : 0;

	public IReadOnlySet<string> Tags { get; init; } = new HashSet<string> { VanillaSkill.FishingAspect };

	public void OnActivate(AffixActivationContext context)
	{
		Mod.Helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;
	}

	public void OnDeactivate(AffixActivationContext context)
	{
		Mod.Helper.Events.GameLoop.UpdateTicked -= OnUpdateTicked;
	}

	public void SetupConfig(IManifest manifest)
	{
		var api = Mod.Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu")!;
		GMCMI18nHelper helper = new(api, Mod.ModManifest, Mod.Helper.Translation);
		helper.AddNumberOption($"{I18nPrefix}.config.value", () => Mod.Config.TenacityValue, min: 0.25f, max: 4f, interval: 0.05f, value => $"{value:0.##}x");
	}

	private void OnUpdateTicked(object? sender, UpdateTickedEventArgs e)
	{
		if (!Game1.game1.IsActive)
			return;
		if (Game1.activeClickableMenu is not BobberBar bar)
			return;
		if (BobberBarHandledFishResultGetter.Value(bar))
			return;
		if (!BobberBarBobberInBarGetter.Value(bar))
			return;

		float distanceFromCatching = BobberBarDistanceFromCatchingGetter.Value(bar);
		distanceFromCatching -= 0.002f;
		distanceFromCatching += 0.002f / Mod.Config.TenacityValue;
		BobberBarDistanceFromCatchingSetter.Value(bar, distanceFromCatching);
	}
}