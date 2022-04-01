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
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using Leclair.Stardew.Common;

using StardewModdingAPI;
using StardewModdingAPI.Utilities;

namespace Leclair.Stardew.Almanac {
	public class AssetManager : IAssetLoader, IAssetEditor {

		private readonly ModEntry Mod;

		public static readonly string ModAssetPath = Path.Combine("Mods", "leclair.almanac");

		public static readonly string CropOverridesPath = Path.Combine(ModAssetPath, "CropOverrides");
		public static readonly string FishOverridesPath = Path.Combine(ModAssetPath, "FishOverrides");
		public static readonly string LocalNoticesPath = Path.Combine(ModAssetPath, "ExtraLocalNotices");
		public static readonly string NPCOverridesPath = Path.Combine(ModAssetPath, "NPCOverrides");

		// Events
		private readonly string EventPath = PathUtilities.NormalizeAssetName("Data/Events");

		public Dictionary<string, List<EventData>> ModEvents;
		private bool Loaded = false;
		private string Locale = null;

		public AssetManager(ModEntry mod) {
			Mod = mod;
			Mod.Helper.Content.AssetLoaders.Add(this);
			Mod.Helper.Content.AssetEditors.Add(this);
		}

		public void Invalidate() {
			Loaded = false;
			Mod.Helper.Content.InvalidateCache(asset => asset.AssetName.StartsWith(EventPath));
		}

		private void Load(string locale) {
			if (Loaded && Locale == locale)
				return;

			try {
				ModEvents = Mod.Helper.Content.LoadLocalized<Dictionary<string, List<EventData>>>("assets/events.json");
			} catch (Exception ex) {
				Mod.Log("Unable to load custom mod events.", ex: ex);
				ModEvents = null;
			}

			Loaded = true;
			Locale = locale;
		}

		#region IAssetLoader
		public bool CanLoad<T>(IAssetInfo asset) {
			return
				asset.AssetNameEquals(CropOverridesPath) ||
				asset.AssetNameEquals(FishOverridesPath) ||
				asset.AssetNameEquals(LocalNoticesPath) ||
				asset.AssetNameEquals(NPCOverridesPath);
		}

		public T Load<T>(IAssetInfo asset) {
			if (asset.AssetNameEquals(CropOverridesPath)) {
				var data = new Dictionary<string, Models.CropOverride>();
				return (T) (object) data;
			}

			if (asset.AssetNameEquals(FishOverridesPath)) {
				var data = new Dictionary<string, Models.FishOverride>();
				return (T) (object) data;
			}

			if (asset.AssetNameEquals(LocalNoticesPath)) {
				var data = new Dictionary<string, Models.LocalNotice>();
				return (T) (object) data;
			}

			if (asset.AssetNameEquals(NPCOverridesPath)) {
				var data = new Dictionary<string, Models.NPCOverride>();
				return (T) (object) data;
			}

			return (T) (object) null;
		}

		#endregion

		#region IAssetEditor

		public bool CanEdit<T>(IAssetInfo asset) {
			if (asset.AssetName.StartsWith(EventPath)) {
				Load(asset.Locale);
				if (ModEvents == null)
					return false;

				string[] bits = PathUtilities.GetSegments(asset.AssetName);
				string end = bits[^1];

				if (ModEvents.ContainsKey(end))
					return true;
			}

			return false;
		}

		public void Edit<T>(IAssetData asset) {
			if (!asset.AssetName.StartsWith(EventPath))
				return;

			Load(asset.Locale);
			if (ModEvents == null)
				return;

			string[] bits = PathUtilities.GetSegments(asset.AssetName);
			string end = bits[^1];

			if (!ModEvents.TryGetValue(end, out var events) || events == null)
				return;

			var editor = asset.AsDictionary<string, string>();
			foreach (var entry in events)
				editor.Data[entry.Key] = entry.Localize(Mod.Helper.Translation);
		}

		#endregion
	}

	public struct EventData {
		public static readonly Regex I18N_SPLITTER = new(@"{{(.+?)}}", RegexOptions.Compiled);

		public string Id { get; set; }
		public string[] Conditions { get; set; }
		public string[] Script { get; set; }

		public string Key => $"{Id}/{string.Join("/", Conditions)}";
		public string RealScript => string.Join("/", Script);

		public string Localize(ITranslationHelper helper) {
			string id = Id;

			return I18N_SPLITTER.Replace(RealScript, match => {
				string key = match.Groups[1].Value;
				if (key.StartsWith('.'))
					key = $"event.{id}{key}";
				return helper.Get(key).ToString();
			});
		}

	}
}
