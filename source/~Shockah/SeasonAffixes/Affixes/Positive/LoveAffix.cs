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
using Shockah.Kokoro.Stardew;
using Shockah.Kokoro.UI;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Shockah.SeasonAffixes;

partial class ModConfig
{
	[JsonProperty] public float LoveValue { get; internal set; } = 2f;
}

internal sealed class LoveAffix : BaseSeasonAffix, ISeasonAffix
{
	private static string ShortID => "Love";
	public string LocalizedDescription => Mod.Helper.Translation.Get($"{I18nPrefix}.description", new { Value = $"{Mod.Config.LoveValue:0.##}x" });
	public TextureRectangle Icon => new(Game1.mouseCursors, new(626, 1892, 9, 8));

	private readonly PerScreen<Dictionary<string, int>> OldFriendship = new(() => new());

	public LoveAffix() : base(ShortID, "positive") { }

	public int GetPositivity(OrdinalSeason season)
		=> 1;

	public int GetNegativity(OrdinalSeason season)
		=> 0;

	public IReadOnlySet<string> Tags { get; init; } = new HashSet<string> { new SpaceCoreSkill("drbirbdev.Socializing").UniqueID };

	public void OnActivate(AffixActivationContext context)
	{
		UpdateDispositions();
		Mod.Helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;
		Mod.Helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
		Mod.Helper.Events.Content.AssetsInvalidated += OnAssetsInvalidated;
	}

	public void OnDeactivate(AffixActivationContext context)
	{
		Mod.Helper.Events.GameLoop.UpdateTicked -= OnUpdateTicked;
		Mod.Helper.Events.GameLoop.SaveLoaded -= OnSaveLoaded;
		Mod.Helper.Events.Content.AssetsInvalidated -= OnAssetsInvalidated;
	}

	public void SetupConfig(IManifest manifest)
	{
		var api = Mod.Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu")!;
		GMCMI18nHelper helper = new(api, Mod.ModManifest, Mod.Helper.Translation);
		helper.AddNumberOption($"{I18nPrefix}.config.value", () => Mod.Config.LoveValue, min: 0.25f, max: 4f, interval: 0.05f, value => $"{value:0.##}x");
	}

	private void OnUpdateTicked(object? sender, UpdateTickedEventArgs e)
	{
		if (!Context.IsWorldReady)
			return;
		UpdateFriendship();
	}

	private void OnSaveLoaded(object? sender, SaveLoadedEventArgs e)
	{
		UpdateDispositions();
	}

	private void OnAssetsInvalidated(object? sender, AssetsInvalidatedEventArgs e)
	{
		if (e.NamesWithoutLocale.Any(a => a.IsEquivalentTo("Data\\NPCDispositions")))
		{
			UpdateFriendship();
			UpdateDispositions();
		}
	}

	private void UpdateDispositions()
	{
		OldFriendship.Value.Clear();
		Dictionary<string, string> dispositions = Game1.content.Load<Dictionary<string, string>>("Data\\NPCDispositions");
		foreach (var (npcName, data) in dispositions)
		{
			string[] split = data.Split('/');
			if (!split[5].Equals("datable", StringComparison.InvariantCultureIgnoreCase))
				continue;
			OldFriendship.Value[npcName] = Game1.player.getFriendshipLevelForNPC(npcName);
		}
	}

	private void UpdateFriendship()
	{
		foreach (var (npcName, oldFriendship) in OldFriendship.Value)
		{
			int newFriendship = Game1.player.getFriendshipLevelForNPC(npcName);
			int extraFriendship = newFriendship - oldFriendship;
			OldFriendship.Value[npcName] += extraFriendship;
			if (extraFriendship > 0)
			{
				int bonusFriendship = (int)Math.Round(-extraFriendship + extraFriendship * Mod.Config.LoveValue);
				Game1.player.friendshipData[npcName].Points += bonusFriendship;
				OldFriendship.Value[npcName] += bonusFriendship;
			}
		}
	}
}