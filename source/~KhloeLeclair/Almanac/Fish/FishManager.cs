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
using System.Linq;

using StardewModdingAPI;
using StardewModdingAPI.Events;

using StardewValley;

using Leclair.Stardew.Common.Enums;
using Leclair.Stardew.Common.Events;

using Leclair.Stardew.Almanac.Models;
using Leclair.Stardew.Almanac.Managers;

namespace Leclair.Stardew.Almanac.Fish;

public class FishManager : BaseManager {

	private List<FishInfo> Fish = new();
	private bool Loaded = false;

	private readonly List<IFishProvider> Providers = new();

	private Dictionary<string, List<LocationOverride>> Overrides = new();
	private readonly object OverrideLock = new();

	public bool OverridesLoaded { get; private set; } = false;

	#region Lifecycle

	public FishManager(ModEntry mod) : base(mod) {
		Providers.Add(new VanillaProvider(mod));
	}

	#endregion

	#region Lock Helpers

	private void WithOverrides(Action action) {
		lock(OverrideLock) {
			action();
		}
	}

	#endregion

	#region Mod Providers

	#endregion

	#region Providers

	public void AddProvider(IFishProvider provider) {
		if (Providers.Contains(provider))
			return;

		Providers.Add(provider);
		SortProviders();
	}

	public void RemoveProvider(IFishProvider provider) {
		if (!Providers.Contains(provider))
			return;

		Providers.Remove(provider);
		Invalidate();
	}

	public void SortProviders() {
		Providers.Sort((a, b) => -a.Priority.CompareTo(b.Priority));
		Invalidate();
	}

	#endregion

	#region Event Handlers

	[Subscriber]
	private void OnSaveLoaded(object sender, SaveLoadedEventArgs e) {
		Invalidate();
	}

	#endregion

	#region Data Loading

	public void Invalidate() {
		Loaded = false;
		Mod.Helper.GameContent.InvalidateCache(AssetManager.FishOverridesPath);
	}

	private void RefreshFish() {

		Dictionary<string, FishInfo> working = new();

		int providers = 0;

		foreach (IFishProvider provider in Providers) {
			int provided = 0;
			IEnumerable<FishInfo>? fish;

			try {
				fish = provider.GetFish();
			} catch(Exception ex) {
				Log($"An error occurred getting fish from provider {provider.Name}.", LogLevel.Warn, ex);
				continue;
			}

			if (fish is not null)
				foreach (FishInfo info in fish) {
					if (!working.ContainsKey(info.Id)) {
						working[info.Id] = info;
						provided++;
					}
				}

			Log($"Loaded {provided} fish from {provider.Name}");
			providers++;
		}

		Fish = working.Values.ToList();
		Loaded = true;
		Log($"Loaded {Fish.Count} fish from {Providers.Count} providers.");
	}

	public void LoadOverrides() {
		WithOverrides(() => {
			Dictionary<string, List<LocationOverride>> result = new(StringComparer.OrdinalIgnoreCase);

			// Read the primary list first.
			const string path = "assets/vanilla_fish.json";
			LocationOverride[]? rides = null;

			try {
				rides = Mod.Helper.Data.ReadJsonFile<LocationOverride[]>(path);
				if (rides == null)
					Log($"The {path} file is missing or invalid.", LogLevel.Warn);
			} catch (Exception ex) {
				Log($"The {path} file is invalid.", LogLevel.Warn, ex);
			}

			if (rides != null) {
				foreach(var ride in rides) {
					if (string.IsNullOrEmpty(ride.Map))
						continue;

					if (result.TryGetValue(ride.Map, out var list))
						list.Add(ride);
					else
						result.Add(ride.Map, new List<LocationOverride> { ride });
				}
			}

			// Now read overrides from content packs.
			foreach(var cp in Mod.Helper.ContentPacks.GetOwned()) {
				if (!cp.HasFile("fish_overrides.json"))
					continue;

				rides = null;
				try {
					rides = cp.ReadJsonFile<LocationOverride[]>("fish_overrides.json");
				} catch(Exception ex) {
					Log($"The fish_overrides.json file of {cp.Manifest.Name} is invalid.", LogLevel.Warn, ex);
				}

				if (rides != null) {
					foreach (var ride in rides) {
						if (string.IsNullOrEmpty(ride.Map))
							continue;

						if (result.TryGetValue(ride.Map, out var list))
							list.Add(ride);
						else
							result.Add(ride.Map, new List<LocationOverride> { ride });
					}
				}
			}

			Overrides = result;
			OverridesLoaded = true;
		});
	}

	#endregion

	#region Locations

	private static void AddFish(SubLocation loc, int[] seasons, int fish, Dictionary<int, Dictionary<SubLocation, List<int>>> result ) {
		if ( ! result.TryGetValue(fish, out var entry)) { 
			result[fish] = new() {
				[loc] = seasons.ToList(),
			};

			return;
		}

		if (!entry.TryGetValue(loc, out var slist)) {
			entry[loc] = seasons.ToList();
			return;
		}

		foreach(int season in seasons) {
			if (!slist.Contains(season))
				slist.Add(season);
		}
	}

	private static void RemoveFish(SubLocation loc, int[] seasons, int fish, Dictionary<int, Dictionary<SubLocation, List<int>>> result) {
		if (!result.TryGetValue(fish, out var entry))
			return;

		if (!entry.TryGetValue(loc, out var slist))
			return;

		for (int i = slist.Count - 1; i >= 0; i--) {
			if (seasons.Contains(slist[i]))
				slist.RemoveAt(i);
		}

		if (slist.Count > 0)
			return;

		entry.Remove(loc);

		if (entry.Count > 0)
			return;

		result.Remove(fish);
	}

	public Dictionary<int, Dictionary<SubLocation, List<int>>> GetFishLocations() {
		if (!OverridesLoaded)
			LoadOverrides();

		var result = FishHelper.GetFishLocations();

		WithOverrides(() => {
			foreach(var ovr in Overrides.SelectMany(x => x.Value)) {
				if (string.IsNullOrEmpty(ovr.Map))
					continue;

				int[] seasons;
				if (ovr.Season == Season.All)
					seasons = new int[] { 0, 1, 2, 3 };
				else
					seasons = new int[] { (int) ovr.Season };

				SubLocation sl = new(ovr.Map, ovr.Zone);

				if (ovr.AddFish != null)
					foreach(string sFish in ovr.AddFish) {
						if (int.TryParse(sFish, out int fish))
							AddFish(sl, seasons, fish, result);
					}

				if (ovr.RemoveFish != null)
					foreach(string sFish in ovr.RemoveFish) {
						if (int.TryParse(sFish, out int fish))
							RemoveFish(sl, seasons, fish, result);
					}

			}
		});

		return result;
	}

	#endregion

	#region Queries

	public IReadOnlyCollection<FishInfo> GetFish(bool filter = true) {
		if (!Loaded)
			RefreshFish();

		if (!filter)
			return Fish.AsReadOnly();

		var overrides = Game1.content.Load<Dictionary<string, FishOverride>>(AssetManager.FishOverridesPath);
		if (overrides == null || overrides.Count == 0)
			return Fish.AsReadOnly();

		return Fish.Where(fish =>
			(!overrides.TryGetValue(fish.Id, out var ovr) || ovr.Visible)
		).ToList();
	}

	public List<FishInfo> GetSeasonFish(int season, bool filter = true) {
		if (!Loaded)
			RefreshFish();

		var overrides = filter ?
			Game1.content.Load<Dictionary<string, FishOverride>>(AssetManager.FishOverridesPath)
			: null;

		return Fish.Where(fish =>
			(overrides == null || !overrides.TryGetValue(fish.Id, out var ovr) || ovr.Visible)
			&& fish.Seasons != null && fish.Seasons.Contains(season)
		).ToList();
	}

	public List<FishInfo> GetSeasonFish(string season, bool filter = true) {
		WorldDate start = new(1, season, 1);
		return GetSeasonFish(start.SeasonIndex, filter);
	}

	#endregion

}
