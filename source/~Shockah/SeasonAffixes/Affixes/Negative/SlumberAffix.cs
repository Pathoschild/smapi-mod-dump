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
using Shockah.Kokoro.GMCM;
using Shockah.Kokoro.UI;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using System;

namespace Shockah.SeasonAffixes;

partial class ModConfig
{
	[JsonProperty] public float SlumberHours { get; internal set; } = 4f;
}

internal sealed class SlumberAffix : BaseSeasonAffix, ISeasonAffix
{
	private static string ShortID => "Slumber";
	public string LocalizedDescription => Mod.Helper.Translation.Get($"{I18nPrefix}.description", new { Hours = $"{(int)Mod.Config.SlumberHours:0.#}" });
	public TextureRectangle Icon => new(Game1.emoteSpriteSheet, new(32, 96, 16, 16));

	public SlumberAffix() : base(ShortID, "negative") { }

	public int GetPositivity(OrdinalSeason season)
		=> 0;

	public int GetNegativity(OrdinalSeason season)
		=> 1;

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
		helper.AddNumberOption($"{I18nPrefix}.config.hours", () => Mod.Config.SlumberHours, min: 0.5f, max: 12f, interval: 0.5f);
	}

	private void OnDayStarted(object? sender, DayStartedEventArgs e)
	{
		if (!Context.IsMainPlayer)
			return;

		int minutesToSkip = (int)Math.Round(Mod.Config.SlumberHours * 60) / 10 * 10;
		while (minutesToSkip > 0)
		{
			Game1.performTenMinuteClockUpdate();
			minutesToSkip -= 10;
		}
	}
}