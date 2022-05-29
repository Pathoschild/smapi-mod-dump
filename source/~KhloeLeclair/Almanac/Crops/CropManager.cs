/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/KhloeLeclair/StardewMods
**
*************************************************/

#nullable enable

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using Leclair.Stardew.Common.Events;

using StardewModdingAPI;
using StardewModdingAPI.Events;

using StardewValley;

using Leclair.Stardew.Almanac.Managers;

namespace Leclair.Stardew.Almanac.Crops;

public class CropManager : BaseManager {

	public List<CropInfo> Crops = new();
	private bool Loaded = false;

	private readonly List<ICropProvider> Providers = new();

	private readonly Dictionary<string, ModProvider> ModProviders = new();

	public CropManager(ModEntry mod) : base(mod) {
		Providers.Add(new VanillaProvider(mod));
	}

	#region Mod Providers

	public ModProvider? GetModProvider(IManifest manifest, bool create = true) {
		if (!ModProviders.ContainsKey(manifest.UniqueID)) {
			if (!create)
				return null;

			var provider = new ModProvider(manifest);
			ModProviders.Add(manifest.UniqueID, provider);
			Providers.Add(provider);
			Providers.Sort((a, b) => -a.Priority.CompareTo(b.Priority));
			Invalidate();
			return provider;
		}

		return ModProviders[manifest.UniqueID];
	}

	public void SortProviders() {
		Providers.Sort((a, b) => -a.Priority.CompareTo(b.Priority));
		Invalidate();
	}

	#endregion

	#region Events

	[Subscriber]
	private void OnSaveLoaded(object? sender, SaveLoadedEventArgs e) {
		Invalidate();
	}

	#endregion

	public void Invalidate() {
		Loaded = false;
		Mod.Helper.GameContent.InvalidateCache(AssetManager.CropOverridesPath);
	}

	public void AddProvider(ICropProvider provider) {
		if (Providers.Contains(provider))
			return;

		Providers.Add(provider);
		Providers.Sort((a, b) => -a.Priority.CompareTo(b.Priority));
		Invalidate();
	}

	public void RemoveProvider(ICropProvider provider) {
		if (!Providers.Contains(provider))
			return;

		Providers.Remove(provider);
		Invalidate();
	}

	public void RefreshCrops() {
		Dictionary<string, CropInfo> working = new();

		int providers = 0;

		foreach (ICropProvider provider in Providers) {
			int provided = 0;
			IEnumerable<CropInfo>? crops;

			try {
				crops = provider.GetCrops();
			} catch (Exception ex) {
				Log($"An error occurred getting crops from provider {provider.Name}.", LogLevel.Error, ex);
				continue;
			}

			if (crops is not null)
				foreach (CropInfo info in crops) {
					if (!working.ContainsKey(info.Id)) {
						working[info.Id] = info;
						provided++;
					}
				}

			Log($"Loaded {provided} crops from {provider.Name}");
			if (provided > 0)
				providers++;
		}

		Crops = working.Values.ToList();

		Log($"Loaded {Crops.Count} crops from {Providers.Count} providers.");

		Loaded = true;
	}

	public List<CropInfo> GetSeasonCrops(int season, bool filtered = true) {
		return GetSeasonCrops(WeatherHelper.GetSeasonName(season), filtered);
	}

	public List<CropInfo> GetSeasonCrops(string season, bool filtered = true) {
		if (!Loaded)
			RefreshCrops();

		WorldDate start = new(1, season, 1);
		WorldDate end = new(1, season, ModEntry.DaysPerMonth);

		var overrides = filtered ?
			Game1.content.Load<Dictionary<string, Models.CropOverride>>(AssetManager.CropOverridesPath)
			: null;

		return Crops.Where(crop =>
			(overrides == null || ! overrides.TryGetValue(crop.Id, out var ovr) || ovr.Visible)
			&& crop.StartDate <= end && crop.EndDate >= start
		).ToList();
	}

}
