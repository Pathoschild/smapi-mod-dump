/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/KhloeLeclair/StardewMods
**
*************************************************/

//#define THEME_MANAGER_PRE_314

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

using HarmonyLib;

using Leclair.Stardew.Common.Integrations.GenericModConfigMenu;
using Leclair.Stardew.Common.Events;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewValley;
using StardewValley.Menus;

using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace Leclair.Stardew.ThemeManager;

public class ModEntry : ModSubscriber {

	#nullable disable
	public static ModEntry Instance { get; private set; }
	public static IThemeManagerApi API { get; private set; }

	internal ModConfig Config;
	#nullable enable

	internal Harmony? Harmony;
	internal GMCMIntegration<ModConfig, ModEntry>? intGMCM;

	internal bool ConfigStale = false;

	internal readonly Dictionary<IManifest, IContentPack> ContentPacks = new();
	internal readonly Dictionary<string, Func<object>> LoadingAssets = new();
	internal readonly Dictionary<IManifest, (Type, IThemeManager)> Managers = new();

#if THEME_MANAGER_PRE_314
	internal ThemeAssetLoader? Loader;
#endif

	#region Construction

	public override void Entry(IModHelper helper) {
		base.Entry(helper);

		Instance = this;
		API = new ModAPI(this);

		// Harmony
		Harmony = new Harmony(ModManifest.UniqueID);

		// Read Config
		Config = Helper.ReadConfig<ModConfig>();

		// I18n
		I18n.Init(Helper.Translation);

		// Asset Loader
		#if THEME_MANAGER_PRE_314
		Loader = new ThemeAssetLoader(this);
		Helper.Content.AssetLoaders.Add(Loader);
		#endif
	}

	public override object GetApi() {
		return API;
	}

	#endregion

	#region Configuration

	internal void ResetConfig() {
		Config = new ModConfig();
	}

	internal void SaveConfig() {
		Helper.WriteConfig(Config);
	}

	[MemberNotNullWhen(true, nameof(intGMCM))]
	internal bool HasGMCM() {
		return intGMCM is not null && intGMCM.IsLoaded;
	}

	internal void OpenGMCM() {
		if (!HasGMCM())
			return;

		if (ConfigStale)
			RegisterSettings();

		intGMCM.OpenMenu();
	}

	internal void RegisterSettings() {
		if (intGMCM is null)
			intGMCM = new(this, () => Config, ResetConfig, SaveConfig);

		// Un-register and re-register so we can redo our settings.
		intGMCM.Unregister();
		intGMCM.Register(true);

		intGMCM.AddLabel(I18n.Settings_ModThemes);

		foreach (var entry in Managers) {

			string uid = entry.Key.UniqueID;
			var choices = entry.Value.Item2.GetThemeChoiceMethods();

			string Getter(ModConfig cfg) {
				if (cfg.SelectedThemes.TryGetValue(uid, out string? value))
					return value;
				return "automatic";
			}

			void Setter(ModConfig cfg, string value) {
				cfg.SelectedThemes[uid] = value;
				if (Managers.TryGetValue(entry.Key, out var mdata) && mdata.Item2 is IThemeSelection tselect)
					tselect._SelectTheme(value);
			}

			intGMCM.AddChoice(
				name: () => entry.Key.Name,
				tooltip: null,
				get: Getter,
				set: Setter,
				choices: choices
			);
		}

		ConfigStale = false;
	}

	#endregion

	#region Content Pack Access

	internal IContentPack? GetContentPackFor(IManifest manifest) {
		if (ContentPacks.TryGetValue(manifest, out IContentPack? cp))
			return cp;

		IModInfo? info = Helper.ModRegistry.Get(manifest.UniqueID);
		if (info is null)
			return null;

		return GetContentPackFor(info);
	}

	internal IContentPack? GetContentPackFor(IModInfo mod) {
		if (ContentPacks.TryGetValue(mod.Manifest, out IContentPack? cp))
			return cp;

		if (mod.IsContentPack && mod.GetType().GetProperty("ContentPack", BindingFlags.Instance | BindingFlags.Public)?.GetValue(mod) is IContentPack pack)
			cp = pack;

		else if (mod.GetType().GetProperty("DirectoryPath", BindingFlags.Instance | BindingFlags.Public)?.GetValue(mod) is string str) {
			cp = Helper.ContentPacks.CreateTemporary(
				directoryPath: str,
				id: $"leclair.theme-loader.${mod.Manifest.UniqueID}",
				name: mod.Manifest.Name,
				description: mod.Manifest.Description,
				author: mod.Manifest.Author,
				version: mod.Manifest.Version
			);

		} else
			return null;

		ContentPacks[mod.Manifest] = cp;
		return cp;
	}

	#endregion

	#region Events

	[Subscriber]
	private void OnGameLaunched(object? sender, GameLaunchedEventArgs e) {
		// Integrations
		CheckRecommendedIntegrations();

		RegisterSettings();
	}

	[Subscriber]
	private void OnMenuChanged(object? sender, MenuChangedEventArgs e) {
		IClickableMenu? menu = e.NewMenu;
		if (menu is null)
			return;

		Type type = menu.GetType();
		string? name = type.FullName ?? type.Name;

		if (name is not null && name.Equals("GenericModConfigMenu.Framework.ModConfigMenu")) {
			if (ConfigStale)
				RegisterSettings();
		}
	}

	#if !THEME_MANAGER_PRE_314
	[Subscriber]
	private void OnAssetRequested(object? sender, AssetRequestedEventArgs e) {
		Func<object>? loader;
		lock ((LoadingAssets as ICollection).SyncRoot) {
			if (!LoadingAssets.TryGetValue(e.Name.BaseName, out loader))
				return;
		}

		e.LoadFrom(
			loader,
			priority: AssetLoadPriority.Low
		);
	}
#endif

	#endregion

}

#if THEME_MANAGER_PRE_314
internal class ThemeAssetLoader : IAssetLoader {

	private readonly ModEntry Mod;

	internal ThemeAssetLoader(ModEntry mod) {
		Mod = mod;
	}

	public bool CanLoad<T>(IAssetInfo asset) {
		return Mod.LoadingAssets.ContainsKey(asset.AssetName);
	}

	public T Load<T>(IAssetInfo asset) {
		if (!Mod.LoadingAssets.TryGetValue(asset.AssetName, out var loader))
			return (T) ((object?) null)!;

		return (T) loader();
	}
}
#endif
