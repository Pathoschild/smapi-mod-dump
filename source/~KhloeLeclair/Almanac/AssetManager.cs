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
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using Leclair.Stardew.Common;

using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;

namespace Leclair.Stardew.Almanac;

public class AssetManager {

	private readonly ModEntry Mod;

	public static readonly string ModAssetPath = PathUtilities.NormalizeAssetName(Path.Combine("Mods", "leclair.almanac"));

	public static readonly string CropOverridesPath = PathUtilities.NormalizeAssetName(Path.Combine(ModAssetPath, "CropOverrides"));
	public static readonly string FishOverridesPath = PathUtilities.NormalizeAssetName(Path.Combine(ModAssetPath, "FishOverrides"));
	public static readonly string LocalNoticesPath = PathUtilities.NormalizeAssetName(Path.Combine(ModAssetPath, "ExtraLocalNotices"));
	public static readonly string FortuneEventsPath = PathUtilities.NormalizeAssetName(Path.Combine(ModAssetPath, "FortuneEvents"));
	public static readonly string NPCOverridesPath = PathUtilities.NormalizeAssetName(Path.Combine(ModAssetPath, "NPCOverrides"));

	// Events
	private readonly string EventPath = PathUtilities.NormalizeAssetName("Data/Events");

	public Dictionary<string, List<EventData>>? ModEvents;
	private bool Loaded = false;
	private string? Locale = null;

	public AssetManager(ModEntry mod) {
		Mod = mod;
		Mod.Helper.Events.Content.AssetRequested += OnAssetRequested;
	}

	public void Invalidate() {
		Loaded = false;
		Mod.Helper.GameContent.InvalidateCache(asset => asset.Name.StartsWith(EventPath));
	}

	private void OnAssetRequested(object? sender, AssetRequestedEventArgs e) {
		// Events
		if (e.Name.StartsWith(EventPath)) {
			Load(e.Name.LocaleCode);
			if (ModEvents == null)
				return;
			string[] bits = PathUtilities.GetSegments(e.Name.BaseName);
			string end = bits[^1];
			if (!ModEvents.TryGetValue(end, out var events) || events == null)
				return;
			e.Edit(data => {
				var editor = data.AsDictionary<string, string>();
				foreach (var entry in events)
					editor.Data[entry.Key] = entry.Localize(Mod.Helper.Translation);
			});
			return;
		}
		if (e.Name.IsEquivalentTo(CropOverridesPath))
			e.LoadFrom(
				() => new Dictionary<string, Models.CropOverride>(),
				priority: AssetLoadPriority.Low
			);
		if (e.Name.IsEquivalentTo(FishOverridesPath))
			e.LoadFrom(
				() => new Dictionary<string, Models.FishOverride>(),
				priority: AssetLoadPriority.Low
			);
	}
	#region IAssetLoader
	public bool CanLoad<T>(IAssetInfo asset) {
		return
			asset.Name.IsEquivalentTo(CropOverridesPath) ||
			asset.Name.IsEquivalentTo(FishOverridesPath) ||
			asset.Name.IsEquivalentTo(LocalNoticesPath) ||
			asset.Name.IsEquivalentTo(NPCOverridesPath);
	}
	public T Load<T>(IAssetInfo asset) {
		if (asset.Name.IsEquivalentTo(CropOverridesPath)) {
			var data = new Dictionary<string, Models.CropOverride>();
			return (T) (object) data;
		}
		if (asset.Name.IsEquivalentTo(FishOverridesPath)) {
			var data = new Dictionary<string, Models.FishOverride>();
			return (T) (object) data;
		}
		if (asset.Name.IsEquivalentTo(LocalNoticesPath)) {
			var data = new Dictionary<string, Models.LocalNotice>();
			return (T) (object) data;
		}
		if (asset.Name.IsEquivalentTo(NPCOverridesPath)) {
			var data = new Dictionary<string, Models.NPCOverride>();
			return (T) (object) data;
		}
		return (T) (object) null;
	}

	#endregion

	#region IAssetEditor

	public bool CanEdit<T>(IAssetInfo asset) {
		if (asset.Name.StartsWith(EventPath)) {
			Load(asset.Locale);
			if (ModEvents == null)
				return false;

			string[] bits = PathUtilities.GetSegments(asset.Name.BaseName);
			string end = bits[^1];
			if (ModEvents.ContainsKey(end))
				return true;
		}
		return false;
	}

	public void Edit<T>(IAssetData asset) {
		if (!asset.Name.StartsWith(EventPath))
			return;
		Load(asset.Locale);
		if (ModEvents == null)
			return;
		string[] bits = PathUtilities.GetSegments(asset.Name.BaseName);
		string end = bits[^1];
		if (!ModEvents.TryGetValue(end, out var events) || events == null)
			return;
		var editor = asset.AsDictionary<string, string>();
		foreach (var entry in events)
			editor.Data[entry.Key] = entry.Localize(Mod.Helper.Translation);
	}

	#endregion
	private void Load(string? locale) {
		if (Loaded && Locale == locale)
			return;
		try {
			ModEvents = Mod.Helper.ModContent.LoadLocalized<Dictionary<string, List<EventData>>>("assets/events.json");
		} catch (Exception ex) {
			Mod.Log("Unable to load custom mod events.", ex: ex);
			ModEvents = null;
		}

		Loaded = true;
		Locale = locale;
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
