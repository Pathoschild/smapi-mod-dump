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
using Shockah.Kokoro.Stardew;
using Shockah.Kokoro.UI;
using StardewModdingAPI.Events;
using StardewValley;
using System;
using System.Collections.Generic;

namespace Shockah.SeasonAffixes;

internal sealed class TidesAffix : BaseSeasonAffix, ISeasonAffix
{
	private static string ShortID => "Tides";
	public string LocalizedDescription => Mod.Helper.Translation.Get($"{I18nPrefix}.description");
	public TextureRectangle Icon => new(Game1.content.Load<Texture2D>("Minigames\\MineCart"), new(48, 256, 16, 16));

	public TidesAffix() : base(ShortID, "neutral") { }

	public int GetPositivity(OrdinalSeason season)
		=> 1;

	public int GetNegativity(OrdinalSeason season)
		=> 1;

	public IReadOnlySet<string> Tags { get; init; } = new HashSet<string> { VanillaSkill.FishingAspect };

	public void OnActivate(AffixActivationContext context)
	{
		Mod.Helper.Events.Content.AssetRequested += OnAssetRequested;
		Mod.Helper.GameContent.InvalidateCache("Data\\Fish");
	}

	public void OnDeactivate(AffixActivationContext context)
	{
		Mod.Helper.Events.Content.AssetRequested -= OnAssetRequested;
		Mod.Helper.GameContent.InvalidateCache("Data\\Fish");
	}

	private void OnAssetRequested(object? sender, AssetRequestedEventArgs e)
	{
		if (!e.Name.IsEquivalentTo("Data\\Fish"))
			return;
		e.Edit(asset =>
		{
			var data = asset.AsDictionary<int, string>();
			foreach (var kvp in data.Data)
			{
				string[] split = kvp.Value.Split('/');
				if (split[1] == "trap")
					continue;

				double spawnMultiplier = double.Parse(split[10]);
				double depthMultiplier = double.Parse(split[11]);
				double totalMultiplier = spawnMultiplier + depthMultiplier;

				double newMultiplier = (1.0 - Math.Pow(totalMultiplier, 0.75)) * Math.Sin(totalMultiplier * Math.PI) + Math.Pow(totalMultiplier, 4.0) / 8.0;
				split[10] = $"{spawnMultiplier * newMultiplier / totalMultiplier}";
				split[11] = $"{depthMultiplier * newMultiplier / totalMultiplier}";
				data.Data[kvp.Key] = string.Join("/", split);
			}
		}, priority: AssetEditPriority.Late);
	}
}