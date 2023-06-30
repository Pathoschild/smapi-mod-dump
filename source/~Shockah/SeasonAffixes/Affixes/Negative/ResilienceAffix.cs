/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Shockah/Stardew-Valley-Mods
**
*************************************************/

using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using Shockah.CommonModCode.GMCM;
using Shockah.Kokoro.GMCM;
using Shockah.Kokoro.Stardew;
using Shockah.Kokoro.UI;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using System;
using System.Collections.Generic;

namespace Shockah.SeasonAffixes;

partial class ModConfig
{
	[JsonProperty] public float ResilienceValue { get; internal set; } = 2f;
}

internal sealed class ResilienceAffix : BaseSeasonAffix, ISeasonAffix
{
	private static string ShortID => "Resilience";
	public string LocalizedDescription => Mod.Helper.Translation.Get($"{I18nPrefix}.description", new { Value = $"{Mod.Config.ResilienceValue:0.##}x" });
	public TextureRectangle Icon => new(Game1.content.Load<Texture2D>("Characters\\Monsters\\Metal Head"), new(0, 0, 16, 16));

	public ResilienceAffix() : base(ShortID, "negative") { }

	public int GetPositivity(OrdinalSeason season)
		=> Mod.Config.ResilienceValue < 1 ? 1 : 0;

	public int GetNegativity(OrdinalSeason season)
		=> Mod.Config.ResilienceValue > 1 ? 1 : 0;

	public IReadOnlySet<string> Tags { get; init; } = new HashSet<string> { VanillaSkill.Combat.UniqueID };

	public double GetProbabilityWeight(OrdinalSeason season)
		=> season.Season == Season.Winter ? 0 : 1;

	public void OnActivate(AffixActivationContext context)
	{
		Mod.Helper.Events.Content.AssetRequested += OnAssetRequested;
		Mod.Helper.GameContent.InvalidateCache("Data\\Monsters");
	}

	public void OnDeactivate(AffixActivationContext context)
	{
		Mod.Helper.Events.Content.AssetRequested -= OnAssetRequested;
		Mod.Helper.GameContent.InvalidateCache("Data\\Monsters");
	}

	public void SetupConfig(IManifest manifest)
	{
		var api = Mod.Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu")!;
		GMCMI18nHelper helper = new(api, Mod.ModManifest, Mod.Helper.Translation);
		helper.AddNumberOption($"{I18nPrefix}.config.value", () => Mod.Config.ResilienceValue, min: 0.25f, max: 4f, interval: 0.05f, value => $"{value:0.##}x");
	}

	private void OnAssetRequested(object? sender, AssetRequestedEventArgs e)
	{
		if (!e.Name.IsEquivalentTo("Data\\Monsters"))
			return;
		e.Edit(asset =>
		{
			var data = asset.AsDictionary<string, string>();
			foreach (var kvp in data.Data)
			{
				string[] split = kvp.Value.Split('/');
				split[0] = $"{(int)Math.Round(int.Parse(split[0]) * Mod.Config.ResilienceValue)}";
				data.Data[kvp.Key] = string.Join("/", split);
			}
		}, priority: AssetEditPriority.Late);
	}
}