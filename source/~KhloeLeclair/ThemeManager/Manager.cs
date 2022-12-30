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
using System.Linq;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using BmFont;

using HarmonyLib;

using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;

using xTile;

using Leclair.Stardew.Common.Extensions;
using Leclair.Stardew.ThemeManager.Models;
using Leclair.Stardew.ThemeManager.VariableSets;
using Nanoray.Pintail;

namespace Leclair.Stardew.ThemeManager;

internal interface IThemeManagerInternal {
	void _SelectTheme(string? themeId, bool postReload = false);

	bool TryGetThemeRaw(string themeId, [NotNullWhen(true)] out object? theme);

	void InvalidateThemeData();

	void HandleAssetRequested(AssetRequestedEventArgs e);

	void Log(string message, LogLevel level = LogLevel.Trace, Exception? ex = null, LogLevel? exLevel = null);

	IGameContentHelper GameContent { get; }

}

public class ThemeManager<DataT> : IThemeManager<DataT>, IThemeManagerInternal where DataT : class, new() {

	public static readonly string ContentPatcher_UniqueID = "Pathoschild.ContentPatcher";

	#region Internal Fields

	/// <summary>
	/// Our mod.
	/// </summary>
	private readonly ModEntry Mod;

	/// <summary>
	/// The remote mod's manifest.
	/// </summary>
	private readonly IManifest Other;

	/// <summary>
	/// A ContentPack for the remote mod.
	/// </summary>
	private readonly IContentPack? OtherCP;

	/// <summary>
	/// A dictionary mapping all known themes to their own UniqueIDs.
	/// </summary>
	private Dictionary<string, Theme<DataT>> Themes = new();

	/// <summary>
	/// A dictionary mapping all known themes to their own UniqueIDs.
	/// This object represents the objects as read from disk, while
	/// <see cref="Themes"/> represents the objects after processing.
	/// </summary>
	private Dictionary<string, Theme<DataT>> DiskThemes = new();

	/// <summary>
	/// When this is set to true, we ignore invalidation events because
	/// we expect one to be coming. Probably from our own invalidation.
	/// </summary>
	private bool ExpectInvalidate;

	/// <summary>
	/// Storage of the <see cref="DefaultTheme"/>. This should not be accessed directly,
	/// instead prefering the use of <see cref="DefaultTheme"/> (which is public).
	/// </summary>
	private DataT _DefaultTheme;

	/// <summary>
	/// The currently active theme. We store this directly to avoid needing
	/// constant dictionary lookups when working with a theme.
	/// </summary>
	private Theme<DataT>? ActiveThemeData = null;

	/// <summary>
	/// The following properties are used for magically setting values on
	/// instanced DataT instances.
	/// </summary>
	private readonly Dictionary<PropertyInfo, bool>? ManifestProperties;
	private readonly Dictionary<PropertyInfo, bool>? ManagerProperties;
	private readonly Dictionary<PropertyInfo, bool>? VariableSetProperties;

	#endregion

	#region Public Fields

	/// <summary>
	/// A string that represents the path used when redirecting theme
	/// data through game content to allow Content Patcher access to your
	/// mod's theme data.
	/// </summary>
	public string ThemeLoaderPath { get; }

	/// <summary>
	/// A string that is prepended to asset names when redirecting
	/// asset loading through game content to allow Content Patcher access
	/// to your mod's themed resources.
	/// </summary>
	public string AssetLoaderPrefix { get; }

	/// <summary>
	/// The key that should be checked when attempting to discover
	/// themes in other mods' manifest files. If this is <c>null</c>,
	/// a string will be built using the pattern <c>{UniqueId}:theme</c>
	/// </summary>
	public string? ManifestKey { get; }

	/// <summary>
	/// The relative path to where your mod keeps its embedded themes.
	/// By default, this is <c>assets/themes</c>. If this is <c>null</c>,
	/// no embedded themes will be loaded.
	/// </summary>
	public string? EmbeddedThemesPath { get; }

	/// <summary>
	/// Whether or not <see cref="ThemeManager{DataT}"/> is redirecting
	/// theme data loading through <see cref="IGameContentHelper"/> to
	/// allow other mods, such as Content Patcher, to modify theme data.
	/// </summary>
	public bool UsingThemeRedirection { get; }

	/// <summary>
	/// Whether or not <see cref="ThemeManager{DataT}"/> is redirecting
	/// asset loading through <see cref="IGameContentHelper"/> to allow
	/// other mods, such as Content Patcher, to modify theme assets.
	/// </summary>
	public bool UsingAssetRedirection { get; }

	/// <summary>
	/// A string that is prepended to asset paths when loading assets from
	/// the <c>default</c> theme. This prefix is not added to paths when
	/// loading assets from other themes, as themes have their own
	/// <c>AssetPrefix</c> to use.
	/// </summary>
	public string? DefaultAssetPrefix { get; set; }

	#endregion

	#region Constructor

	/// <summary>
	/// Create a new ThemeManager.
	/// </summary>
	/// <param name="mod">Your mod's Mod class. We need access to your
	/// mod's ModManifest, Helper, and Monitor.</param>
	/// <param name="selectedThemeId">The initial selected theme ID. When
	/// Discover is run, this value will be used to immediately select the
	/// desired theme. <seealso cref="SelectedThemeId"/></param>
	/// <param name="defaultTheme">The <typeparamref name="DataT"/> instance
	/// to use when the default theme is selected.
	/// <seealso cref="DefaultTheme"/></param>
	/// <param name="embeddedThemesPath">The path to search your mod for
	/// embedded themes at. <seealso cref="EmbeddedThemesPath"/></param>
	/// <param name="assetPrefix">The asset prefix to use when
	/// the default theme is selected.
	/// <seealso cref="DefaultAssetPrefix"/></param>
	/// <param name="assetLoaderPrefix">A prefix prepended to all asset
	/// paths when redirecting asset loading through IAssetLoader. By
	/// default, this value is <c>Mods/{yourMod.UniqueId}/Themes</c>.
	/// <seealso cref="AssetLoaderPrefix"/></param>
	/// <param name="themeLoaderPath">A path to use when redirecting
	/// theme data loading through the game's content pipeline. By
	/// default, this value is <c>Mods/{yourMod.UniqueId}/ThemeData</c>.
	/// <seealso cref="ThemeLoaderPath"/></param>
	/// <param name="forceAssetRedirection">If set to a value, override
	/// the default asset redirection behavior.
	/// <seealso cref="UsingAssetRedirection"/></param>
	/// <param name="forceThemeRedirection">If set to a value, override
	/// the default theme data redirection behavior.
	/// <seealso cref="UsingThemeRedirection"/></param>
	public ThemeManager(
		ModEntry mod,
		IManifest other,
		string selectedThemeId = "automatic",
		DataT? defaultTheme = null,
		string? manifestKey = null,
		string? embeddedThemesPath = "assets/themes",
		string? assetPrefix = "assets",
		string? assetLoaderPrefix = null,
		string? themeLoaderPath = null,
		bool? forceAssetRedirection = null,
		bool? forceThemeRedirection = null
	) {
		// Store the basic initial values.
		Mod = mod;
		Other = other;
		OtherCP = Mod.GetContentPackFor(other);
		SelectedThemeId = selectedThemeId;
		DefaultAssetPrefix = assetPrefix;
		EmbeddedThemesPath = embeddedThemesPath;
		ManifestKey = manifestKey;

		_DefaultTheme = defaultTheme ?? new DataT();

		// Detect Content Patcher
		bool hasCP = Mod.Helper.ModRegistry.IsLoaded(ContentPatcher_UniqueID);
		UsingAssetRedirection = forceAssetRedirection ?? true; //hasCP;
		UsingThemeRedirection = forceThemeRedirection ?? true; // hasCP;

		Log($"New ThemeManager. CP: {hasCP}, ForceAsset: {forceAssetRedirection}, RedirectAsset: {UsingAssetRedirection}, ForceTheme: {forceThemeRedirection}, RedirectTheme: {UsingThemeRedirection}", LogLevel.Trace);

		// Always run the AssetLoaderPrefix through NormalizeAssetName,
		// otherwise we'll run into issues actually using our custom
		// asset loader.
		AssetLoaderPrefix = PathUtilities.NormalizeAssetName(
			string.IsNullOrEmpty(assetLoaderPrefix) ?
				Path.Join("Mods", ModManifest.UniqueID, "Themes") :
				assetLoaderPrefix
			);

		ThemeLoaderPath = PathUtilities.NormalizeAssetName(
			string.IsNullOrEmpty(themeLoaderPath) ?
				Path.Join("Mods", ModManifest.UniqueID, "ThemeData") :
				themeLoaderPath
			);

		// Identify properties we should be filling in.
		Dictionary<PropertyInfo, bool> mfProps = new();
		Dictionary<PropertyInfo, bool> vsProps = new();
		Dictionary<PropertyInfo, bool> tmProps = new();

		Type tmType = typeof(IThemeManager<>).MakeGenericType(typeof(DataT));

		foreach (var prop in AccessTools.GetDeclaredProperties(typeof(DataT))) {
			if (prop.CanWrite) {
				if (typeof(IThemeManifest).IsAssignableFrom(prop.PropertyType))
					mfProps[prop] = false;
				else if (Mod.CanProxy(typeof(IThemeManifest), Mod.ModManifest.UniqueID, prop.PropertyType, Other.UniqueID))
					mfProps[prop] = true;

				if (typeof(IThemeManager).IsAssignableFrom(prop.PropertyType) || tmType.IsAssignableFrom(prop.PropertyType))
					tmProps[prop] = false;
				else if (Mod.CanProxy(typeof(IThemeManager), Mod.ModManifest.UniqueID, prop.PropertyType, Other.UniqueID))
					tmProps[prop] = true;
				else if (Mod.CanProxy(tmType, Mod.ModManifest.UniqueID, prop.PropertyType, Other.UniqueID))
					tmProps[prop] = true;
			}

			if (prop.CanRead) {
				if (typeof(IVariableSet).IsAssignableFrom(prop.PropertyType))
					vsProps[prop] = true;
                else if (Mod.CanProxy(prop.PropertyType, Other.UniqueID, typeof(IVariableSet), Mod.ModManifest.UniqueID))
                    vsProps[prop] = true;
			}
		}

		ManifestProperties = mfProps.Count > 0 ? mfProps : null;
		VariableSetProperties = vsProps.Count > 0 ? vsProps : null;
		ManagerProperties = tmProps.Count > 0 ? tmProps : null;
	}

	#endregion

	#region Properties

	public IManifest ModManifest => Other;

	#endregion

	#region Logging

	void IThemeManagerInternal.Log(string message, LogLevel level, Exception? ex, LogLevel? exLevel) {
		Log(message, level: level, ex: ex, exLevel: exLevel);
	}

	private void Log(string message, LogLevel level = LogLevel.Trace, Exception? ex = null, LogLevel? exLevel = null) {
		Type t = typeof(DataT);
		string? name = t?.Name ?? t?.FullName;

		Mod.Monitor.Log($"[{Other.UniqueID}:{name}] {message}", level: level);
		if (ex != null)
			Mod.Monitor.Log($"[{Other.UniqueID}:{name}] Exception Details:\n{ex}", level: exLevel ?? level);
	}

	#endregion

	#region Theme Discovery

	private record LoadingTheme(
		IManifest Mod,
		LoadableManifest? Manifest,
		IContentPack Pack,
		string? RelativePath,
		string ThemeFileName
	);

	/// <summary>
	/// Perform theme discovery, reloading all theme data, and then update
	/// the active theme.
	/// </summary>
	/// <param name="checkOwned">Whether or not to load themes from owned content packs</param>
	/// <param name="checkEmbedded">Whether or not to load embedded themes.</param>
	/// <param name="checkExternal">Whether or not to load external themes from other mods.</param>
	public void Discover(
		bool checkOwned = true,
		bool checkEmbedded = true,
		bool checkExternal = true
	) {
		lock ((DiskThemes as ICollection).SyncRoot) {
			// Start by wiping the existing theme data.
			DiskThemes.Clear();

			// Add an entry for the default theme.
			/*ThemeManifest defaultManifest = new(
				uniqueID: "default",
				name: I18n.Theme_Default(),
				localizedNames: null,
				translationKey: "theme.default",
				providingMod: ModManifest,
				supportedMods: null,
				unsupportedMods: null,
				fallbackTheme: null,
				assetPrefix: DefaultAssetPrefix,
				overrideRedirection: null,
				nonSelectable: false
			);

			DefaultThemeData = new(DefaultTheme, defaultManifest, OtherCP, null);
			DiskThemes["default"] = DefaultThemeData;*/

			// We want to keep track of packs with custom IDs so that we
			// can use better IDs for embedded packs.
			Dictionary<string, LoadingTheme> packsWithIds = new();

			// If we haven't been forbidden, check for embedded themes and
			// add them to our packs. We do this first so that any content
			// pack themes can override our embedded themes, if they really
			// need to. They shouldn't, however.
			if (checkEmbedded) {
				var embedded = FindEmbeddedThemes();
				if (embedded != null)
					foreach (var cp in embedded)
						packsWithIds[cp.Key] = cp.Value;
			}

			// Now, check for your mod's owned content packs.
			if (checkOwned) {
				var owned = Mod.Helper.ModRegistry.GetAll()
					.Where(x => x.IsContentPack && x.Manifest.ContentPackFor.UniqueID.Equals(Other.UniqueID, StringComparison.OrdinalIgnoreCase))
					.Select(x => Mod.GetContentPackFor(x));
				foreach (var cp in owned) {
					if (cp is not null && cp.HasFile("theme.json"))
						packsWithIds[cp.Manifest.UniqueID] = new(cp.Manifest, null, cp, null, "theme.json");
				}
			}

			// Finally, check for external mods that provide themes.
			if (checkExternal) {
				var external = FindExternalThemes();
				if (external != null)
					foreach (var cp in external)
						packsWithIds[cp.Key] = cp.Value;
			}

			// Now, load each of our packs.
			foreach (var cp in packsWithIds) {
				string file = string.IsNullOrEmpty(cp.Value.ThemeFileName) ? "theme.json" : cp.Value.ThemeFileName;
				string fpath = string.IsNullOrEmpty(cp.Value.RelativePath) ? file : Path.Join(cp.Value.RelativePath, file);

				if (!cp.Value.Pack.HasFile(fpath))
					continue;

				if (cp.Key == "default" || cp.Key == "automatic") {
					Log($"Skipping theme at {fpath} in {cp.Value.Pack.Manifest.Name} because it has an invalid key: {cp.Key}", LogLevel.Warn);
					continue;
				}

				LoadableManifest? loadable;
				DataT? data;
				try {
					loadable = cp.Value.Manifest ?? cp.Value.Pack.ReadJsonFile<LoadableManifest>(fpath);
					if (loadable is null)
						throw new Exception($"{file} is empty or invalid");

					data = Mod.ReadJsonFile<DataT>(fpath, cp.Value.Pack);
					if (data is null)
						throw new Exception($"{file} is empty or invalid");
				} catch (Exception ex) {
					Log($"The theme at {fpath} in {cp.Value.Pack.Manifest.Name} has an invalid theme json file and could not be loaded.", LogLevel.Warn, ex);
					continue;
				}

				string uid = loadable.UniqueID ?? cp.Key;

				if (string.IsNullOrEmpty(loadable.Name)) {
					if (cp.Value.Pack.Manifest.UniqueID != Other.UniqueID)
						loadable.Name = cp.Value.Pack.Manifest.Name;
				}

				ThemeManifest manifest = new(
					uniqueID: uid,
					name: loadable.Name ?? uid,
					localizedNames: loadable.LocalizedNames,
					translationKey: loadable.TranslationKey,
					providingMod: cp.Value.Mod,
					supportedMods: loadable.SupportedMods ?? loadable.For,
					unsupportedMods: loadable.UnsupportedMods,
					fallbackTheme: loadable.FallbackTheme,
					assetPrefix: loadable.AssetPrefix,
					// If asset redirection is enabled, it cannot be disabled
					// on a per-theme basis. This is to ensure that managed
					// asset access is reliable.
					overrideRedirection: UsingAssetRedirection ? null : loadable.OverrideRedirection,
					nonSelectable: loadable.NonSelectable
				);

				if (UsingAssetRedirection && loadable.OverrideRedirection.HasValue && !loadable.OverrideRedirection.Value)
					Log($"Ignoring \"OverrideRedirection\": false in theme '{manifest.Name}' ({manifest.UniqueID}) due to manager settings.", LogLevel.Warn);

				UpdateProperties(manifest, data);
				DiskThemes[cp.Key] = new(data, manifest, cp.Value.Pack, cp.Value.RelativePath);
			}
		}

		// Use a separate method to finish discovery. One that can be called when
		// theme data is invalidated.
		FinishDiscover();
	}

	private void FinishDiscover() {
		Themes = DiskThemes;

		if (UsingThemeRedirection) {
			Dictionary<string, DataT>? data;

			ExpectInvalidate = true;
			Mod.Helper.GameContent.InvalidateCache(ThemeLoaderPath);
			ExpectInvalidate = false;

			try {
				data = Mod.Helper.GameContent.Load<Dictionary<string, DataT>>(ThemeLoaderPath);
			} catch (Exception ex) {
				Log($"An error occurred while running theme data through game content: {ex}", LogLevel.Error);
				data = null;
			}

			if (data is not null) {
				Themes = new(DiskThemes);
				foreach (var entry in data) {
					if (Themes.TryGetValue(entry.Key, out var edata) && entry.Value is not null && edata.Data != entry.Value) {
						Themes[entry.Key] = new(entry.Value, edata.Manifest, edata.Content, edata.RelativePath);
						UpdateProperties(edata.Manifest, entry.Value);
					}
				}
			}
		}

		// Invoke the discovery event.
		ThemesDiscovered?.SafeInvoke(this, new ThemesDiscoveredEventArgs<DataT>(Themes), monitor: Mod.Monitor);

		// Store our currently selected theme.
		string? oldKey = SelectedThemeId;

		// Clear our data.
		ActiveThemeData = null;
		ActiveThemeId = "default";

		// And select the new theme.
		_SelectTheme(oldKey, true);
		Mod.ConfigStale = true;
	}

	private void UpdateProperties(IThemeManifest manifest, DataT instance) {
		if (ManifestProperties is not null)
			foreach(var prop in ManifestProperties) {
				try {
					if (prop.Value) {
						Mod.TryProxy(manifest, Mod.ModManifest.UniqueID, prop.Key.PropertyType, Other.UniqueID, out object? proxy, sourceType: typeof(IThemeManifest));
						prop.Key.SetValue(instance, proxy);
					} else
						prop.Key.SetValue(instance, manifest);
				} catch (Exception ex) {
					Log($"Unable to store manifest in theme: {ex}", LogLevel.Warn);
				}
			}

		if (ManagerProperties is not null)
			foreach (var prop in ManagerProperties) {
				try {
					if (prop.Value) {
						Mod.TryProxy(this, Mod.ModManifest.UniqueID, prop.Key.PropertyType, Other.UniqueID, out object? proxy, sourceType: typeof(IThemeManager<DataT>));
						prop.Key.SetValue(instance, proxy);
					} else
						prop.Key.SetValue(instance, this);
				} catch (Exception ex) {
					Log($"Unable to store manager in theme: {ex}", LogLevel.Warn);
				}
			}

		if (VariableSetProperties is not null)
			foreach(var prop in VariableSetProperties) {
				object? obj;
				try {
					obj = prop.Key.GetValue(instance);
				} catch(Exception ex) {
					Log($"Unable to read variable set instance from theme: {ex}", LogLevel.Warn);
					continue;
				}

				if (obj is IVariableSet vs)
					vs.SetReferences(this, manifest);
				else if (Mod.TryProxyRemote<IVariableSet>(obj, Other.UniqueID, out var vss))
					vss.SetReferences(this, manifest);
			}
	}

	public void InvalidateThemeData() {
		if (!ExpectInvalidate) {
			Log($"Reloading theme data after another mod invalidated it.");
			FinishDiscover();
		}
	}

	/// <summary>
	/// Search for loose themes in your mod's embedded themes folder.
	/// </summary>
	/// <returns>A dictionary of temporary IContentPacks for each embedded theme.</returns>
	private Dictionary<string, LoadingTheme>? FindEmbeddedThemes() {
		if (string.IsNullOrEmpty(EmbeddedThemesPath))
			return null;

		if (OtherCP is null)
			return null;

		string path = Path.Join(OtherCP.DirectoryPath, PathUtilities.NormalizePath(EmbeddedThemesPath));
		if (!Directory.Exists(path))
			return null;

		Dictionary<string, LoadingTheme> results = new();
		int count = 0;

		// Start iterating subdirectories of our embedded themes folder.
		foreach (string dir in Directory.GetDirectories(path)) {
			string t_path = Path.Join(dir, "theme.json");

			// If the subdirectory has no theme.json, ignore it.
			if (!File.Exists(t_path))
				continue;

			string rel_path = Path.GetRelativePath(OtherCP.DirectoryPath, dir);
			string folder = Path.GetRelativePath(path, dir);

			Log($"Found Embedded Theme At: {dir}", LogLevel.Trace);

			LoadableManifest? manifest = null;
			try {
				manifest = OtherCP.ReadJsonFile<LoadableManifest>(
					Path.Join(rel_path, "theme.json"));

				if (manifest is null)
					throw new Exception("theme.json is empty or invalid");
			} catch (Exception ex) {
				Log($"Unable to read embedded theme manifest.", LogLevel.Warn, ex);
				continue;
			}

			// TODO: Slugify the folder name.

			string uid = manifest.UniqueID ?? $"embedded.{folder}";

			results[uid] = new(Other, manifest, OtherCP, rel_path, "theme.json");
			count++;

			string name = manifest.Name ?? uid;

			Log($"Found Embedded Theme: {name} ({uid})", LogLevel.Trace);
		}

		Log($"Found {count} Embedded Themes.", LogLevel.Trace);
		return results;
	}

	/// <summary>
	/// Search for themes in other mods' manifests.
	/// </summary>
	/// <returns>A dictionary of temporary IContentPacks for each discovered theme.</returns>
	private Dictionary<string, LoadingTheme> FindExternalThemes() {
		Dictionary<string, LoadingTheme> results = new();
		int count = 0;

		string themeKey;
		if (string.IsNullOrEmpty(ManifestKey)) {
			if (ManifestKey != null)
				return results;

			themeKey = $"{Other.UniqueID}:theme";
		} else
			themeKey = ManifestKey;

		string themeFile = $"{themeKey.Replace(':', '.')}.json";

		foreach (var mod in Mod.Helper.ModRegistry.GetAll()) {
			// For every mod, try reading a special value from its manifest.
			if (mod?.Manifest?.ExtraFields == null)
				continue;

			if (!mod.Manifest.ExtraFields.TryGetValue(themeKey, out object? value))
				continue;

			string file = themeFile;

			// If the value is a boolean, and false for some reason,
			// just skip the mod. If it's true, assume the default
			// filename for our theme JSON.
			if (value is bool bv) {
				if (!bv)
					continue;

				// If the value is a string, use that string as a relative
				// filename for the theme.json file, relative to that mod's
				// directory root.
			} else if (value is string str) {
				file = str;

				// Display a warning for any other value, as we can only handle
				// strings and booleans.
			} else {
				Log($"Unknown or unsupported value for {themeKey} in mod {mod.Manifest.Name} ({mod.Manifest.UniqueID})", LogLevel.Warn);
				continue;
			}

			// Get an IContentPack for this mod.
			var cp = Mod.GetContentPackFor(mod);
			if (cp is null) {
				Log($"Unable to get IContentPack for mod: {mod.Manifest.UniqueID}", LogLevel.Warn);
				continue;
			}

			file = PathUtilities.NormalizePath(file);

			if (!cp.HasFile(file)) {
				Log($"Unable to find {file} in mod {mod.Manifest.Name} ({mod.Manifest.UniqueID})", LogLevel.Warn);
				continue;
			}

			// Get the relative path for the file, and just the filename.
			string? folder = Path.GetDirectoryName(file);
			file = Path.GetFileName(file);

			results[mod.Manifest.UniqueID] = new(mod.Manifest, null, cp, folder, file);
			count++;
		}

		Log($"Found {count} External Themes");
		return results;
	}

	#endregion

	#region Theme Enumeration

	/// <summary>
	/// Get the human readable name of a theme, optionally in a given locale.
	/// If no locale is given, the game's current locale will be used.
	/// </summary>
	/// <param name="themeId">The <see cref="ThemeManifest.UniqueID"/> of the
	/// theme you want to get the name of.</param>
	/// <param name="locale">The locale to get the name in, if possible.</param>
	public string GetThemeName(string themeId, string? locale = null) {
		// For the default theme, return a translation from the host mod's
		// translation layer.
		if (themeId.Equals("default", StringComparison.OrdinalIgnoreCase))
			return Mod.Helper.Translation.Get($"theme.default").ToString();

		// Get the theme data. If the theme is the active theme, don't
		// bother with a dictionary lookup.
		Theme<DataT>? theme;
		if (themeId == ActiveThemeId)
			theme = ActiveThemeData;
		else {
			lock ((Themes as ICollection).SyncRoot) {
				if (!Themes.TryGetValue(themeId, out theme))
					return themeId;
			}
		}

		// If we don't have theme data, just return the themeId.
		if (theme == null)
			return themeId;

		// Check for the translation in our theme data.
		if (string.IsNullOrEmpty(locale))
			locale = Mod.Helper.Translation.Locale;

		// 
		if (!string.IsNullOrEmpty(theme.Manifest.TranslationKey) && theme.Content is not null) {
			var tls = theme.Content.Translation.GetInAllLocales(theme.Manifest.TranslationKey);
			if (tls != null && tls.TryGetValue(locale, out var tl)) {
				string? result = tl.ToString();
				if (!string.IsNullOrEmpty(result))
					return result;
			}
		}

		if (theme.Manifest.LocalizedNames?.TryGetValue(locale, out string? name) ?? false && name != null)
			return name;

		// Manifest Name
		return theme.Manifest.Name;
	}

	/// <summary>
	/// Get an enumeration of the available themes, suitable for display in
	/// a configuration menu such as Generic Mod Config Menu. This list will
	/// always include an <c>automatic</c> and <c>default</c> entry.
	///
	/// The keys are <see cref="ThemeManifest.UniqueID"/>s while the values
	/// are human readable names in the current (or selected) locale.
	/// </summary>
	/// <param name="locale">The locale to get names in, if possible.</param>
	public IReadOnlyDictionary<string, string> GetThemeChoices(string? locale = null) {
		Dictionary<string, string> result = new();

		result.Add("automatic", I18n.Theme_Automatic());
		result.Add("default", I18n.Theme_Default());

		foreach(var entry in Themes) {
			if (!entry.Value.Manifest.NonSelectable)
				result.Add(entry.Key, GetThemeName(entry.Key, locale));
		}

		return result;
	}

	/// <inheritdoc />
	public IReadOnlyDictionary<string, Func<string>> GetThemeChoiceMethods() {
		Dictionary<string, Func<string>> result = new();

		result.Add("automatic", I18n.Theme_Automatic);
		result.Add("default", I18n.Theme_Default);

		foreach (var entry in Themes) {
			if (!entry.Value.Manifest.NonSelectable)
				result.Add(entry.Key, () => GetThemeName(entry.Key));
		}

		return result;
	}

	/// <inheritdoc />
	public IReadOnlyCollection<IThemeManifest> GetThemeManifests() {
		return Themes.Values.Select(x => x.Manifest).ToList().AsReadOnly();
	}

	/// <inheritdoc />
	public bool HasTheme(string themeId) {
		return Themes.ContainsKey(themeId);
	}

	/// <inheritdoc />
	public bool TryGetTheme(string themeId, [NotNullWhen(true)] out DataT? theme) {
		if (Themes.TryGetValue(themeId, out var tdata)) {
			theme = tdata.Data;
			return true;
		}

		theme = null;
		return false;
	}

	public bool TryGetThemeRaw(string themeId, [NotNullWhen(true)] out object? theme) {
		if (Themes.TryGetValue(themeId, out var tdata)) {
			theme = tdata.Data;
			return true;
		}

		theme = null;
		return false;
	}

	/// <inheritdoc />
	public bool TryGetManifest(string themeId, [NotNullWhen(true)] out IThemeManifest? manifest) {
		if (Themes.TryGetValue(themeId, out var tdata)) {
			manifest = tdata.Manifest;
			return true;
		}

		manifest = null;
		return false;
	}

	#endregion

	#region Theme Selection

	/// <inheritdoc />
	public string SelectedThemeId { get; private set; }

	/// <inheritdoc />
	public void SelectTheme(string? themeId) {
		_SelectTheme(themeId, false);

		Mod.Config.SelectedThemes[Other.UniqueID] = SelectedThemeId;
		Mod.SaveConfig();
	}

	public void _SelectTheme(string? themeId, bool postReload = false) {
		if (string.IsNullOrEmpty(themeId))
			themeId = "automatic";

		string old_active = ActiveThemeId;
		var old_data = ActiveThemeData;

		// Deal with the default theme quickly.
		if (themeId.Equals("default", StringComparison.OrdinalIgnoreCase)) {
			ActiveThemeData = null;
			SelectedThemeId = "default";
			ActiveThemeId = "default";
		}

		// Does this string match something?
		else if (!themeId.Equals("automatic", StringComparison.OrdinalIgnoreCase) && Themes.TryGetValue(themeId, out var theme) && ! theme.Manifest.NonSelectable) {
			ActiveThemeData = theme;
			SelectedThemeId = themeId;
			ActiveThemeId = themeId;
		}

		// Determine the best theme
		else {
			ActiveThemeId = "default";
			ActiveThemeData = null;

			string[] ids = Themes.Keys.ToArray();
			for (int i = ids.Length - 1; i >= 0; i--) {
				if (!Themes.TryGetValue(ids[i], out var themeData) || themeData.Manifest.NonSelectable)
					continue;

				if (themeData.Manifest.MatchesForAutomatic(Mod.Helper.ModRegistry)) {
					ActiveThemeData = themeData;
					ActiveThemeId = ids[i];
					break;
				}
			}

			SelectedThemeId = "automatic";
		}

		Log($"Selected Theme: {SelectedThemeId} => {GetThemeName(ActiveThemeId)} ({ActiveThemeId})", LogLevel.Trace);

		// Did the active theme actually change?
		if (ActiveThemeId != old_active || postReload) {
			// Invalidate old resources to kick them out of memory when
			// we're no longer using that theme. If this is being called
			// after a theme reload, however, invalidate everything in
			// case other themes have been manually used.
			Invalidate(postReload ? null : old_active);

			// And emit our event.
			ThemeChanged?.SafeInvoke(this, new ThemeChangedEventArgs<DataT>(
				old_active,
				old_data?.Manifest,
				old_data?.Data,
				ActiveThemeId,
				ActiveThemeManifest,
				Theme
			), monitor: Mod.Monitor);
		}
	}

	#endregion

	#region Default / Active Theme Access

	/// <inheritdoc />
	public DataT DefaultTheme {
		get => _DefaultTheme;
		set {
			bool is_default = ActiveThemeId == "default";
			DataT? oldData = Theme;
			_DefaultTheme = value ?? new DataT();
			if (is_default)
				ThemeChanged?.SafeInvoke(this, new ThemeChangedEventArgs<DataT>(
					"default",
					null,
					oldData,
					"default",
					null,
					_DefaultTheme
				), monitor: Mod.Monitor);
		}
	}

	/// <inheritdoc />
	public string ActiveThemeId { get; private set; } = "default";

	/// <inheritdoc />
	public IThemeManifest? ActiveThemeManifest => ActiveThemeData?.Manifest;

	/// <inheritdoc />
	public DataT ActiveTheme => ActiveThemeData?.Data ?? _DefaultTheme;

	/// <inheritdoc />
	public DataT Theme => ActiveThemeData?.Data ?? _DefaultTheme;

	/// <inheritdoc />
	public event EventHandler<IThemeChangedEvent<DataT>>? ThemeChanged;

	/// <inheritdoc />
	public event EventHandler<IThemesDiscoveredEvent<DataT>>? ThemesDiscovered;

	/// <inheritdoc />
	public void RegisterCPToken(string name = "Theme") {
		if (Mod.intCP is not null && Mod.intCP.IsLoaded)
			Mod.intCP.RegisterManager(ModManifest, name, this);
	}

	#endregion

	#region Asset Invalidation

	/// <inheritdoc />
	public void Invalidate(string? themeId = null) {
		// If we have a target theme, focus our key for that theme. Otherwise
		// we just invalidate every asset starting with our asset prefix.
		string key = AssetLoaderPrefix;
		if (!string.IsNullOrEmpty(themeId))
			key = PathUtilities.NormalizeAssetName(string.Join(PathUtilities.PreferredAssetSeparator, AssetLoaderPrefix, themeId));

		Mod.Helper.GameContent.InvalidateCache(info => info.Name.StartsWith(key, allowPartialWord: false, allowSubfolder: true));
	}

	/// <inheritdoc />
	public void Invalidate(string path, string? themeId = null, bool stripFileExtensions = true) {
		// Strip the file extension if we have one.
		if (stripFileExtensions)
			path = StripExtensionFromPath(path);

		// If we do have a specific theme, we only have to invalidate a single asset.
		if (!string.IsNullOrEmpty(themeId)) {
			Mod.Helper.GameContent.InvalidateCache(PathUtilities.NormalizeAssetName(
				string.Join(PathUtilities.PreferredAssetSeparator, AssetLoaderPrefix, themeId, path)
			));
			return;
		}

		// If we don't have a specific theme, generate a key for every theme we do have.
		string[] keys = Themes.Keys
			.Select(key => PathUtilities.NormalizeAssetName(string.Join(PathUtilities.PreferredAssetSeparator, AssetLoaderPrefix, key, path)))
			.ToArray();

		// ... and use them to invalidate assets.
		Mod.Helper.GameContent.InvalidateCache(info => {
			if (info.Name.StartsWith(AssetLoaderPrefix, allowPartialWord: false, allowSubfolder: true)) {
				foreach (string key in keys) {
					if (info.Name.IsEquivalentTo(key))
						return true;
				}
			}

			return false;
		});
	}

	#endregion

	#region Asset Loading

	private string StripExtensionFromPath(string path) {
		string? folder = Path.GetDirectoryName(path);
		string file = Path.GetFileNameWithoutExtension(path);
		if (string.IsNullOrEmpty(folder))
			return file;
		else
			return string.Join(PathUtilities.PreferredAssetSeparator, folder, file);
	}

	/// <inheritdoc />
	public IAssetName? GetAssetName(string path, string? themeId = null, bool stripFileExtensions = true) {
		// Either check the specified theme, or the active theme.
		Theme<DataT>? theme;
		if (themeId == "default")
			theme = null;
		else if (themeId == ActiveThemeId || themeId is null) {
			themeId = ActiveThemeId;
			theme = ActiveThemeData;
		} else
			Themes.TryGetValue(themeId, out theme);

		// If asset redirection has been disabled, just return null.
		if (!UsingAssetRedirection && !(theme?.Manifest.OverrideRedirection ?? false))
			return null;

		// Strip the file extension if we have one. And if we do, store this
		// extension so we can reuse it later.
		string? ext;
		if (stripFileExtensions) {
			ext = Path.GetExtension(path);
			path = StripExtensionFromPath(path);
		} else
			ext = null;

		// And use the IGameContentHelper to parse the asset name.
		var result = Mod.Helper.GameContent.ParseAssetName(string.Join(PathUtilities.PreferredAssetSeparator, AssetLoaderPrefix, themeId, path));
		Mod.AssetExtensions[result] = string.IsNullOrWhiteSpace(ext) ? null : ext;
		return result;
	}

	/// <inheritdoc />
	public bool DoesAssetExist<T>(string path, string? themeId = null, bool stripFileExtensions = true, bool allowFallback = true, bool allowDefault = true) where T : notnull {
		// This method should always have a relevant theme, if we did not
		// get invalid input from our consumer. Do this now, to set themeId
		// if it's still the default null.
		Theme<DataT>? theme;
		if (themeId == "default")
			theme = null;
		else if (themeId == ActiveThemeId || themeId is null) {
			themeId = ActiveThemeId;
			theme = ActiveThemeData;
		} else
			Themes.TryGetValue(themeId, out theme);

		// Get the asset name. This will return a null value if the theme does
		// not allow asset redirection.
		IAssetName? assetName = GetAssetName(path, themeId: themeId, stripFileExtensions: stripFileExtensions);

		// If we can load the asset via game content, it exists.
		if (Mod.DoesAssetExist<T>(assetName))
			return true;

		// We weren't, so can we load the asset directly?
		if (HasFile(path, themeId: themeId, file: out string? _, useDefault: allowDefault))
			return true;

		// Finally, if we have a fall back theme, check that.
		if (allowFallback && !string.IsNullOrEmpty(theme?.Manifest.FallbackTheme))
			return DoesAssetExist<T>(path, themeId: theme.Manifest.FallbackTheme, stripFileExtensions: stripFileExtensions, allowFallback: false, allowDefault: false);

		return false;
	}

	public IManagedAsset<T> GetManagedAsset<T>(string path, string? themeId = null, bool stripFileExtensions = true, bool allowFallback = true) where T : notnull {
		// This method should always have a relevant theme, if we did not
		// get invalid input from our consumer. Do this now, to set themeId
		// if it's still the default null.
		Theme<DataT>? theme;
		if (themeId == "default")
			theme = null;
		else if (themeId == ActiveThemeId || themeId is null) {
			themeId = ActiveThemeId;
			theme = ActiveThemeData;
		} else
			Themes.TryGetValue(themeId, out theme);

		// Get the asset name. This will return a null value if the theme does
		// not allow asset redirection.
		IAssetName? assetName = GetAssetName(path, themeId: themeId, stripFileExtensions: stripFileExtensions);

		// If the asset doesn't exist, clear the name.
		if (! Mod.DoesAssetExist<T>(assetName))
			assetName = null;

		// If there's no name, and we have a fall back theme, try it.
		if (assetName is null && allowFallback && !string.IsNullOrEmpty(theme?.Manifest?.FallbackTheme)) {
			assetName = GetAssetName(path, themeId: theme.Manifest.FallbackTheme, stripFileExtensions: stripFileExtensions);
			if (! Mod.DoesAssetExist<T>(assetName))
				assetName = null;
		}

		// Finally, what about the default theme?
		if (themeId != "default" && assetName is null) {
			assetName = GetAssetName(path, themeId: "default", stripFileExtensions: stripFileExtensions);
			if (! Mod.DoesAssetExist<T>(assetName))
				assetName = null;
		}

		// If we don't have an asset name, that's a problem.
		if (assetName is null)
			throw new ContentLoadException($"Failed loading themed asset '{path}' from theme '{themeId}': the specified path doesn't exist");

		if (Mod.TryGetManagedAsset<T>(assetName, out var result))
			return result;

		throw new ContentLoadException($"Failed loading themed asset '{path}' from theme '{themeId}': could not create IManagedAsset due to type mismatch with existing asset");
	}

	/// <inheritdoc />
	public T Load<T>(string path, string? themeId = null, bool stripFileExtensions = true, bool allowFallback = true) where T : notnull {
		// This method should always have a relevant theme, if we did not
		// get invalid input from our consumer. Do this now, to set themeId
		// if it's still the default null.
		Theme<DataT>? theme;
		if (themeId == "default")
			theme = null;
		else if (themeId == ActiveThemeId || themeId is null) {
			themeId = ActiveThemeId;
			theme = ActiveThemeData;
		} else
			Themes.TryGetValue(themeId, out theme);

		// Get the asset name. This will return a null value if the theme does
		// not allow asset redirection.
		IAssetName? assetName = GetAssetName(path, themeId: themeId, stripFileExtensions: stripFileExtensions);

		// If we have an asset name, we can load via game content. Let's see if
		// anyone is providing the asset. If they are, load that.
		if (Mod.DoesAssetExist<T>(assetName))
			return Mod.Helper.GameContent.Load<T>(assetName);

		// We are not able to load the asset via game content. Next, let's see
		// if we can load the asset directly.
		if (HasFile(path, themeId: themeId, file: out string? found, useDefault: false)) {
			if (typeof(T) == typeof(SpriteFont))
				return (T) (object) InternalLoadFont(path: found ?? path, themeId: themeId);
			return InternalLoad<T>(found ?? path, themeId: themeId);
		}

		// We do not have the asset in this theme. Next, let's see if we can
		// load the asset from a fall back theme.
		if (allowFallback && !string.IsNullOrEmpty(theme?.Manifest.FallbackTheme)) {
			assetName = GetAssetName(path, themeId: theme.Manifest.FallbackTheme, stripFileExtensions: stripFileExtensions);
			if (Mod.DoesAssetExist<T>(assetName))
				return Mod.Helper.GameContent.Load<T>(assetName);

			if (HasFile(path, themeId: theme.Manifest.FallbackTheme, file: out found, useDefault: false)) {
				if (typeof(T) == typeof(SpriteFont))
					return (T) (object) InternalLoadFont(path: found ?? path, themeId: theme.Manifest.FallbackTheme);
				return InternalLoad<T>(found ?? path, themeId: theme.Manifest.FallbackTheme);
			}
		}

		// Finally, try loading from the default theme.
		if (themeId != "default") {
			assetName = GetAssetName(path, themeId: "default", stripFileExtensions: stripFileExtensions);
			if (Mod.DoesAssetExist<T>(assetName))
				return Mod.Helper.GameContent.Load<T>(assetName);
		}

		// We weren't able to load from anywhere else. As a last effort, try
		// loading via InternalLoad from the base mod's assets.
		if (typeof(T) == typeof(SpriteFont))
			return (T) (object) InternalLoadFont(path: path, themeId: "default");
		return InternalLoad<T>(path, themeId: "default");
	}

	/// <summary>
	/// Attempt to get a matching file in a directory, in case we're looking
	/// for an asset without a file extension. Which we probably will be.
	/// </summary>
	/// <param name="filename">The file we're searching for.</param>
	/// <param name="root">The root path we're searching in.</param>
	/// <param name="pack">The content pack we expect to load it with.</param>
	/// <returns>A matching file path, relative to root, or <c>null</c> if no file matched.</returns>
	/// <exception cref="Exception">Throws an exception if more than one file matches.</exception>
	private static string? GetMatchingFile(string filename, string? extension, string? root, IContentPack pack) {
		string relative = string.IsNullOrEmpty(root) ? filename : Path.Join(root, filename);
		if (pack.HasFile(relative))
			return relative;

		List<string> results = new();

		string? basedir = Path.GetDirectoryName(relative);
		if (basedir is not null) {
			basedir = Path.Join(pack.DirectoryPath, basedir);
			if (Directory.Exists(basedir)) {
				// Get the filename that we're supposed to be matching.
				string matchname = Path.GetFileName(filename);

				foreach (string entry in Directory.EnumerateFiles(basedir)) {
					// Does this file match the name that was requested?
					string entryname = Path.GetFileNameWithoutExtension(entry);

					if (entryname.Equals(matchname, StringComparison.OrdinalIgnoreCase) && (extension == null || string.Equals(extension, Path.GetExtension(entry), StringComparison.OrdinalIgnoreCase)))
						results.Add(Path.GetRelativePath(pack.DirectoryPath, entry));
				}
			}
		}

		if (results.Count > 1)
			throw new Exception($"ambiguous match when attempting to load \"{filename}\" from \"{root}\"");
		else if (results.Count == 1)
			return results[0];
		else
			return null;
	}

	private bool HasFile(string path, string? themeId, [NotNullWhen(true)] out string? file, bool useDefault = true) {
		// Either check the specified theme, or the active theme.
		Theme<DataT>? theme;
		if (themeId == "default" || themeId is null)
			theme = null;
		else if (themeId == ActiveThemeId)
			theme = ActiveThemeData;
		else
			Themes.TryGetValue(themeId, out theme);

		string? root;
		string? ext = Path.GetExtension(path);
		
		// If we have a theme, then try loading from it.
		if (theme != null && theme.Content is not null) {
			string lpath = path;

			if (!string.IsNullOrEmpty(theme.RelativePath))
				root = theme.RelativePath;
			else
				root = null;

			if (!string.IsNullOrEmpty(theme.Manifest.AssetPrefix)) {
				if (root is null)
					root = theme.Manifest.AssetPrefix;
				else
					root = Path.Join(root, theme.Manifest.AssetPrefix);
			}

			try {
				file = GetMatchingFile(lpath, ext, root, theme.Content);
			} catch(Exception ex) {
				Log($"Error when checking for an asset file in theme {theme.Manifest.Name} ({theme.Manifest.UniqueID}): {ex}", LogLevel.Error);
				file = null;
			}

			if (!string.IsNullOrEmpty(file))
				return true;
		}

		// If we aren't using the default provider, or can't for some reason,
		// then return false now.
		if (!useDefault || OtherCP is null) {
			file = null;
			return false;
		}

		// Now fall back to the base mod content.
		try {
			file = GetMatchingFile(path, ext, DefaultAssetPrefix, OtherCP);

		} catch(Exception ex) {
			Log($"Error when checking for an asset file in mod assets: {ex}", LogLevel.Error);
			file = null;
		}

		return !string.IsNullOrEmpty(file);
	}

	/// <summary>
	/// Attempt to load a SpriteFont from disk. If the font cannot be loaded
	/// normally, use several different JSON formats.
	/// </summary>
	/// <param name="path"></param>
	/// <param name="themeId"></param>
	private SpriteFont InternalLoadFont(string path, string? themeId = null) {
		// First, just try doing a standard load in case that works.
		try {
			return InternalLoad<SpriteFont>(path, themeId, logErrors: false);
		} catch { /* no-op */ }

		// Next, try loading the JSON format returned from StardewXNBHack.
		RawSpriteFontData? data;

		try {
			data = InternalLoad<RawSpriteFontData>(path, themeId, logErrors: false);
		} catch {
			data = null;
		}

		if (data is null)
			try {
				XNBHackFontData? rawData = InternalLoad<XNBHackFontData>(path, themeId, logErrors: false);
				data = rawData?.ToRaw();
			} catch {
				data = null;
			}

		// If we couldn't read any format, complain about it.
		if (data is null)
			throw new ContentLoadException($"Unable to load font from '{path}' due to missing file or unsupported format.");

		List<RawGlyphData> glyphs = new();
		if (data.Glyphs is not null)
			foreach (var glyph in data.Glyphs) {
				if (glyph.Character is null || glyph.BoundsInTexture is null || glyph.Cropping is null)
					continue;

				glyph.Kerning ??= new Microsoft.Xna.Framework.Vector3(0, glyph.BoundsInTexture.Value.Width, 0);
				glyphs.Add(glyph);
			}

		if (glyphs.Count == 0)
			throw new ContentLoadException($"Invalid font: the loaded font has no valid glyphs");

		data.TextureName ??= Path.GetFileNameWithoutExtension(path);
		if (string.IsNullOrWhiteSpace(Path.GetExtension(data.TextureName)))
			data.TextureName += ".png";

		string? dir = Path.GetDirectoryName(path);
		string texPath = string.IsNullOrWhiteSpace(dir) ? data.TextureName : Path.Combine(dir, data.TextureName);

		Texture2D? texture = InternalLoad<Texture2D>(texPath, themeId: themeId);

		return new SpriteFont(
			texture: texture,
			glyphBounds: glyphs.Select(x => x.BoundsInTexture!.Value).ToList(),
			cropping: glyphs.Select(x => x.Cropping!.Value).ToList(),
			characters: glyphs.Select(x => x.Character!.Value).ToList(),
			lineSpacing: data.LineSpacing,
			spacing: data.Spacing,
			kerning: glyphs.Select(x => x.Kerning!.Value).ToList(),
			defaultCharacter: data.DefaultCharacter
		);
	}

	/// <summary>
	/// Attempt to load an asset from disk. If the asset cannot be loaded
	/// from the theme, it will be loaded from the base mod's assets.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="path"></param>
	/// <param name="themeId"></param>
	/// <returns></returns>
	private T InternalLoad<T>(string path, string? themeId = null, bool logErrors = true) where T : notnull {
		// Either check the specified theme, or the active theme.
		Theme<DataT>? theme;
		if (themeId == "default" || themeId is null)
			theme = null;
		else if (themeId == ActiveThemeId)
			theme = ActiveThemeData;
		else
			Themes.TryGetValue(themeId, out theme);

		string? relative;
		string? ext = Path.GetExtension(path);

		// If we have a theme, then try loading from it.
		if (theme?.Content != null) {
			string lpath = path;
			string? root;

			if (!string.IsNullOrEmpty(theme.RelativePath))
				root = theme.RelativePath;
			else
				root = null;

			if (!string.IsNullOrEmpty(theme.Manifest.AssetPrefix)) {
				if (root is null)
					root = theme.Manifest.AssetPrefix;
				else
					root = Path.Join(root, theme.Manifest.AssetPrefix);
			}

			try {
				relative = GetMatchingFile(lpath, ext, root, theme.Content);
			} catch(Exception ex) {
				if (logErrors)
					Log($"Error when checking for an asset file in theme {theme.Manifest.Name} ({theme.Manifest.UniqueID}): {ex}", LogLevel.Error);
				relative = null;
			}

			if (relative != null)
				try { 
					return theme.Content.ModContent.Load<T>(relative);
				} catch(Exception ex) {
					if (logErrors)
						Log($"Failed to load asset \"{path}\" from theme {themeId}.", LogLevel.Warn, ex);
				}
		}

		// Now fall back to the default theme
		try {
			relative = GetMatchingFile(path, ext, DefaultAssetPrefix, OtherCP!);
		} catch (Exception ex) {
			if (logErrors)
				Log($"Error when checking for an asset file in mod assets: {ex}", LogLevel.Error);
			relative = null;
		}

		return OtherCP!.ModContent.Load<T>(relative ?? path);
	}

	public void HandleAssetRequested(AssetRequestedEventArgs e) {
		// Is it loading theme data?
		if (ThemeLoaderPath != null && e.Name.IsEquivalentTo(ThemeLoaderPath)) {
			if (UsingThemeRedirection)
				e.LoadFrom(
					load: () => DiskThemes.ToDictionary(x => x.Key, x => Mod.Clone(x.Value.Data)),
					priority: AssetLoadPriority.Exclusive
				);

			return;
		}

		// Is it loading an asset?
		if (!e.Name.StartsWith(AssetLoaderPrefix))
			return;

		// Figure out the theme component.
		int start = AssetLoaderPrefix.Length + 1;
		int idx = e.Name.Name.IndexOf(PathUtilities.PreferredAssetSeparator, start);

		string themeId = e.Name.Name[start..idx];
		string trail = e.Name.Name[(idx + 1)..];

		Theme<DataT>? theme;
		bool isDefault = themeId == "default";

		if (isDefault)
			theme = null;
		else if (themeId == ActiveThemeId)
			theme = ActiveThemeData;
		else if (!Themes.TryGetValue(themeId, out theme))
			return;

		// Try to find the file from:
		// 1. The theme
		// 2. The fallback theme
		// 3. The mod's assets

		// Try to get our file extension.
		if (!Mod.AssetExtensions.TryGetValue(e.Name, out string? ext))
			Mod.AssetExtensions.TryGetValue(e.NameWithoutLocale, out ext);

		string? usingTheme;
		string? path;

		string trailExt = ext is null ? trail : $"{trail}{ext}";

		if (!isDefault && HasFile(trailExt, themeId: themeId, file: out path, useDefault: false))
			usingTheme = themeId;
		else if (!isDefault && !string.IsNullOrEmpty(theme?.Manifest.FallbackTheme) && HasFile(trailExt, themeId: theme.Manifest.FallbackTheme, file: out path, useDefault: false))
			usingTheme = theme.Manifest.FallbackTheme;
		else if (HasFile(trailExt, themeId: null, file: out path, useDefault: true))
			usingTheme = null;
		else
			return;

		// Now we need to figure out what type we want to load.
		// Generally, we should know this because we already called our own
		// Load<> or DoesAssetExist<> methods by this point.

		Type? type;

		if (!Mod.AssetTypes.TryGetValue(e.Name, out type) && !Mod.AssetTypes.TryGetValue(e.NameWithoutLocale, out type)) {
			string extn = Path.GetExtension(path);
			switch(extn) {
				case ".png":
					type = typeof(Texture2D);
					break;
				case ".fnt":
					type = typeof(XmlSource);
					break;
				case ".tbin":
				case ".tbx":
					type = typeof(Map);
					break;
				default:
					Log($"Unknown file extension and no known type while attempting to load {path} for {e.Name}", LogLevel.Warn);
					return;
			}
		}

		if (type is null) {
			Log($"Unable to determine type while attempting to load {path} for {e.Name}", LogLevel.Warn);
			return;
		}

		// Special font logic.
		if (type == typeof(SpriteFont)) {
			Log($"Using font loader for {e.Name} from \"{path}\"", LogLevel.Debug);
			e.LoadFrom(() => InternalLoadFont(path, themeId: usingTheme), priority: AssetLoadPriority.Low);
			return;
		}

		// If we have a type, get a loader.
		TypedInternalLoad? Loader = GetTypedLoad(type);
		if (Loader is null)
			return;

		// ... and load it!
		if (usingTheme is null)
			Log($"Loading {e.Name} from \"{path}\" in mod assets with type {type.FullName ?? type.Name}", LogLevel.Debug);
		else
			Log($"Loading {e.Name} from \"{path}\" in theme \"{usingTheme}\" with type {type.FullName ?? type.Name}", LogLevel.Debug);

		e.LoadFrom(
			load: () => Loader(path, themeId: usingTheme),
			priority: AssetLoadPriority.Low
		);
	}

	IGameContentHelper IThemeManagerInternal.GameContent => Mod.Helper.GameContent;

	#endregion

	#region Generic Internal Load Binding

	private readonly Hashtable InternalLoad_Bindings = new();
	private MethodInfo? InternalLoad_Ptr;

	private delegate object TypedInternalLoad(string path, string? themeId = null, bool logErrors = true);

	private TypedInternalLoad? GetTypedLoad(Type type) {
		lock (InternalLoad_Bindings.SyncRoot) {
			if (!InternalLoad_Bindings.ContainsKey(type)) {
				TypedInternalLoad? @delegate;
				try {
					InternalLoad_Ptr ??= GetType().GetMethod(nameof(InternalLoad), BindingFlags.Instance | BindingFlags.NonPublic);
					var generic = InternalLoad_Ptr!.MakeGenericMethod(type);
					@delegate = generic.CreateDelegate<TypedInternalLoad>(this);
				} catch (Exception ex) {
					Log($"Unable to create InternalLoad delegate: {ex}", LogLevel.Error);
					@delegate = null;
				}

				InternalLoad_Bindings[type] = @delegate;
				return @delegate;
			}

			return InternalLoad_Bindings[type] as TypedInternalLoad;
		}
	}

	#endregion
}
