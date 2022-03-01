/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/KhloeLeclair/StardewMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;

using Leclair.Stardew.Common.Events;

using StardewModdingAPI;
using StardewModdingAPI.Events;

using StardewValley;

namespace Leclair.Stardew.Almanac.Crops {
	public class CropManager : EventSubscriber<ModEntry> {

		public List<CropInfo> Crops = new();
		private bool Loaded = false;

		private readonly List<ICropProvider> Providers = new();

		private readonly Dictionary<string, ModProvider> ModProviders = new();

		public CropManager(ModEntry mod) : base(mod) {
			Providers.Add(new VanillaProvider());
		}

		protected void Log(string message, LogLevel level = LogLevel.Debug, Exception ex = null) {
			string Name = GetType().Name;
			Mod.Monitor.Log($"[{Name}] {message}", level: level);
			if (ex != null)
				Mod.Monitor.Log($"[{Name}] Details:\n{ex}", level: level);
		}

		#region Mod Providers

		public ModProvider GetModProvider(IManifest manifest, bool create = true) {
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
		private void OnSaveLoaded(object sender, SaveLoadedEventArgs e) {
			Invalidate();
		}

		#endregion

		public void Invalidate() {
			Loaded = false;
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
				IEnumerable<CropInfo> crops;

				try {
					crops = provider.GetCrops();
				} catch (Exception ex) {
					Log($"An error occurred getting crops from provider {provider.GetType().Name}.", LogLevel.Error, ex);
					continue;
				}

				foreach (CropInfo info in crops) {
					if (!working.ContainsKey(info.Id)) {
						working[info.Id] = info;
						provided++;
					}
				}

				Log($"Loaded {provided} crops from {provider.GetType().Name}");
				if (provided > 0)
					providers++;
			}

			Crops = working.Values.ToList();

			Log($"Loaded {Crops.Count} crops from {Providers.Count} providers.");

			Loaded = true;
		}

		public List<CropInfo> GetSeasonCrops(int season) {
			return GetSeasonCrops(WeatherHelper.GetSeasonName(season));
		}

		public List<CropInfo> GetSeasonCrops(string season) {
			if (!Loaded)
				RefreshCrops();

			WorldDate start = new(1, season, 1);
			WorldDate end = new(1, season, WorldDate.DaysPerMonth);

			return Crops.Where(crop => crop.StartDate <= end && crop.EndDate >= start).ToList();
		}

	}
}
