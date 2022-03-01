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

using StardewModdingAPI;
using StardewModdingAPI.Utilities;

namespace Leclair.Stardew.Almanac {
	public class AssetManager : IAssetEditor {

		private readonly ModEntry Mod;

		// Events
		private readonly string EventPath = PathUtilities.NormalizeAssetName("Data/Events");

		public Dictionary<string, List<EventData>> ModEvents;


		public AssetManager(ModEntry mod) {
			Mod = mod;
			Mod.Helper.Content.AssetEditors.Add(this);

			Reload(false);
		}

		public void Reload(bool invalidate = true) {
			try {
				ModEvents = Mod.Helper.Content.Load<Dictionary<string, List<EventData>>>("assets/events.json");
			} catch (Exception ex) {
				Mod.Log("Unable to load custom mod events.", ex: ex);
				ModEvents = null;
			}

			Mod.Helper.Content.InvalidateCache(asset => asset.AssetName.StartsWith(EventPath));
		}


		public bool CanEdit<T>(IAssetInfo asset) {
			if (asset.AssetName.StartsWith(EventPath) && ModEvents != null) {
				string[] bits = PathUtilities.GetSegments(asset.AssetName);
				string end = bits[bits.Length - 1];

				if (ModEvents.ContainsKey(end))
					return true;
			}

			return false;
		}

		public void Edit<T>(IAssetData asset) {
			if (!asset.AssetName.StartsWith(EventPath) || ModEvents == null)
				return;

			string[] bits = PathUtilities.GetSegments(asset.AssetName);
			string end = bits[bits.Length - 1];

			if (!ModEvents.TryGetValue(end, out var events) || events == null)
				return;

			var editor = asset.AsDictionary<string, string>();
			foreach (var entry in events)
				editor.Data[entry.Key] = entry.RealScript;
		}
	}

	public struct EventData {
		public string Id { get; set; }
		public string[] Conditions { get; set; }
		public string[] Script { get; set; }

		public string Key => $"{Id}/{string.Join("/", Conditions)}";
		public string RealScript => string.Join("/", Script);
	}
}
