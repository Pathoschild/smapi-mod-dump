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
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using StardewModdingAPI;
using StardewModdingAPI.Utilities;

using Leclair.Stardew.ThemeManager.Models;

namespace Leclair.Stardew.ThemeManager;

public interface IThemeSelection {
	void _SelectTheme(string? themeId, bool postReload = false);
}

public class ThemeManager<DataT> : ITypedThemeManager<DataT>, IThemeSelection where DataT : class, new() {

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
	private readonly Dictionary<string, Theme<DataT>> Themes = new();

	/// <summary>
	/// Storage of the <see cref="DefaultTheme"/>. This should not be accessed directly,
	/// instead prefering the use of <see cref="DefaultTheme"/> (which is public).
	/// </summary>
	private DataT _DefaultTheme;

	/// <summary>
	/// The currently active theme. We store this directly to avoid needing
	/// constant dictionary lookups when working with a theme.
	/// </summary>
	private Theme<DataT>? BaseThemeData = null;

	#endregion

	#region Public Fields

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
	/// <param name="forceAssetRedirection">If set to a value, override
	/// the default asset redirection behavior.
	/// <seealso cref="UsingAssetRedirection"/></param>
	public ThemeManager(
		ModEntry mod,
		IManifest other,
		string selectedThemeId = "automatic",
		DataT? defaultTheme = null,
		string? manifestKey = null,
		string? embeddedThemesPath = "assets/themes",
		string? assetPrefix = "assets",
		string? assetLoaderPrefix = null,
		bool? forceAssetRedirection = null
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
		UsingAssetRedirection = forceAssetRedirection ?? hasCP;

		Log($"New ThemeManager. CP: {hasCP}, Force: {forceAssetRedirection}, Redirection: {UsingAssetRedirection}");

		// Always run the AssetLoaderPrefix through NormalizeAssetName,
		// otherwise we'll run into issues actually using our custom
		// asset loader.
		AssetLoaderPrefix = PathUtilities.NormalizeAssetName(
			string.IsNullOrEmpty(assetLoaderPrefix) ?
				Path.Join("Mods", ModManifest.UniqueID, "Themes") :
				assetLoaderPrefix
			);
	}

	#endregion

	#region Properties

	public IManifest ModManifest => Other;

	#endregion

	#region Logging

	private void Log(string message, LogLevel level = LogLevel.Trace, Exception? ex = null, LogLevel? exLevel = null) {
		Type t = typeof(DataT);
		string? name = t?.Name ?? t?.FullName;

		Mod.Monitor.Log($"[{Other.UniqueID}:{name}] {message}", level: level);
		if (ex != null)
			Mod.Monitor.Log($"[{Other.UniqueID}:{name}] Details:\n{ex}", level: exLevel ?? level);
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
		lock ((Themes as ICollection).SyncRoot) {
			// Start by wiping the existing theme data.
			Themes.Clear();

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
					overrideRedirection: loadable.OverrideRedirection
				);

				Themes[cp.Key] = new(data, manifest, cp.Value.Pack, cp.Value.RelativePath);
			}
		}

		// Store our currently selected theme.
		string? oldKey = SelectedThemeId;

		// Clear our data.
		BaseThemeData = null;
		ActiveThemeId = "default";

		// And select the new theme.
		_SelectTheme(oldKey, true);
		Mod.ConfigStale = true;
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
		// bother with a dictionary lookp.
		Theme<DataT>? theme;
		if (themeId == ActiveThemeId)
			theme = BaseThemeData;
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
		if (!string.IsNullOrEmpty(theme.Manifest.TranslationKey)) {
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

		foreach (string theme in Themes.Keys)
			result.Add(theme, GetThemeName(theme, locale));

		return result;
	}

	/// <inheritdoc />
	public IReadOnlyDictionary<string, Func<string>> GetThemeChoiceMethods() {
		Dictionary<string, Func<string>> result = new();

		result.Add("automatic", I18n.Theme_Automatic);
		result.Add("default", I18n.Theme_Default);

		foreach (string theme in Themes.Keys)
			result.Add(theme, () => GetThemeName(theme));

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
		var old_data = BaseThemeData;

		// Deal with the default theme quickly.
		if (themeId.Equals("default", StringComparison.OrdinalIgnoreCase)) {
			BaseThemeData = null;
			SelectedThemeId = "default";
			ActiveThemeId = "default";
		}

		// Does this string match something?
		else if (!themeId.Equals("automatic", StringComparison.OrdinalIgnoreCase) && Themes.TryGetValue(themeId, out var theme)) {
			BaseThemeData = theme;
			SelectedThemeId = themeId;
			ActiveThemeId = themeId;
		}

		// Determine the best theme
		else {
			ActiveThemeId = "default";
			BaseThemeData = null;

			string[] ids = Themes.Keys.ToArray();
			for (int i = ids.Length - 1; i >= 0; i--) {
				if (!Themes.TryGetValue(ids[i], out var themeData))
					continue;

				if (themeData.Manifest.MatchesForAutomatic(Mod.Helper.ModRegistry)) {
					BaseThemeData = themeData;
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
			ThemeChanged?.Invoke(this, new ThemeChangedEventArgs<DataT>(old_active, old_data?.Data, ActiveThemeId, Theme));
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
				ThemeChanged?.Invoke(this, new ThemeChangedEventArgs<DataT>("default", oldData, "default", _DefaultTheme));
		}
	}

	/// <inheritdoc />
	public string ActiveThemeId { get; private set; } = "default";

	/// <inheritdoc />
	public IThemeManifest? ActiveThemeManifest => BaseThemeData?.Manifest;

	/// <inheritdoc />
	public DataT ActiveTheme => BaseThemeData?.Data ?? _DefaultTheme;

	/// <inheritdoc />
	public DataT Theme => BaseThemeData?.Data ?? _DefaultTheme;

	/// <inheritdoc />
	public event EventHandler<IThemeChangedEvent<DataT>>? ThemeChanged;

	#endregion

	#region Asset Loading

	/// <inheritdoc />
	public void Invalidate(string? themeId = null) {
		string key = AssetLoaderPrefix;
		if (!string.IsNullOrEmpty(themeId))
			key = PathUtilities.NormalizeAssetName(Path.Join(AssetLoaderPrefix, themeId));

		Mod.Helper.GameContent.InvalidateCache(info => info.Name.StartsWith(key));
	}

	/// <inheritdoc />
	public T Load<T>(string path, string? themeId = null) where T : notnull {
		// Either check the specified theme, or the active theme.
		Theme<DataT>? theme;
		if (themeId != null)
			Themes.TryGetValue(themeId, out theme);
		else {
			themeId = ActiveThemeId;
			theme = BaseThemeData;
		}

		// Does this theme have this file?
		if (theme is not null && !HasFile(path, themeId, false, false)) {
			// If not, does the fallback theme have it? If so, then load it.
			if (!string.IsNullOrEmpty(theme.Manifest.FallbackTheme) && HasFile(path, theme.Manifest.FallbackTheme, false, false))
				return Load<T>(path, theme.Manifest.FallbackTheme);
		}

		// Just go straight to InternalLoad if we're not redirecting this
		// asset in some way. We only redirect if redirection is enabled
		// and this isn't the default theme and this theme doesn't prevent
		// asset redirection.
		bool redirect = theme?.Manifest?.OverrideRedirection ?? (theme != null && UsingAssetRedirection);
		if (!redirect)
			return InternalLoad<T>(path, themeId);

		string assetPath = PathUtilities.NormalizeAssetName(Path.Join(AssetLoaderPrefix, themeId, path));

		// We might not actually need this, depending on how the asset load
		// works out. It might already be cached, too. Don't actually
		// perform a load, just store a method that will perform a load.
		lock ((Mod.LoadingAssets as ICollection).SyncRoot) {
			Mod.LoadingAssets[assetPath] = () => InternalLoad<T>(path, themeId);
		}

		try {
			return Mod.Helper.GameContent.Load<T>(assetPath);
		} finally {
			lock ((Mod.LoadingAssets as ICollection).SyncRoot) {
				Mod.LoadingAssets.Remove(assetPath);
			}
		}
	}

	/// <inheritdoc />
	public bool HasFile(string path, string? themeId = null, bool useFallback = true, bool useDefault = true) {
		// Either check the specified theme, or the active theme.
		Theme<DataT>? theme;
		if (themeId != null)
			Themes.TryGetValue(themeId, out theme);
		else
			theme = BaseThemeData;

		if (theme != null) {
			string lpath = path;
			if (!string.IsNullOrEmpty(theme.Manifest.AssetPrefix))
				lpath = Path.Join(theme.Manifest.AssetPrefix, lpath);

			if (!string.IsNullOrEmpty(theme.RelativePath))
				lpath = Path.Join(theme.RelativePath, lpath);

			if (theme.Content.HasFile(lpath))
				return true;

			// Only fall-back once when using a fallback theme.
			if (useFallback && !string.IsNullOrEmpty(theme.Manifest.FallbackTheme) && HasFile(path, theme.Manifest.FallbackTheme, false, false))
				return true;
		}

		if (!useDefault || OtherCP is null)
			return false;

		// Now check the default theme.
		if (!string.IsNullOrEmpty(DefaultAssetPrefix))
			path = Path.Join(DefaultAssetPrefix, path);

		return OtherCP.HasFile(path);
	}

	/// <inheritdoc />
	private T InternalLoad<T>(string path, string? themeId = null) where T : notnull {
		// Either check the specified theme, or the active theme.
		Theme<DataT>? theme;
		if (themeId != null)
			Themes.TryGetValue(themeId, out theme);
		else {
			themeId = ActiveThemeId;
			theme = BaseThemeData;
		}

		// If we have a theme, then try loading from it.
		if (theme != null) {
			string lpath = path;
			if (!string.IsNullOrEmpty(theme.Manifest.AssetPrefix))
				lpath = Path.Join(theme.Manifest.AssetPrefix, lpath);

			if (!string.IsNullOrEmpty(theme.RelativePath))
				lpath = Path.Join(theme.RelativePath, lpath);

			if (theme.Content.HasFile(lpath))
				try {
					return theme.Content.ModContent.Load<T>(lpath);
				} catch (Exception ex) {
					Log($"Failed to load asset \"{path}\" from theme {themeId}.", LogLevel.Warn, ex);
				}
		}

		// Now fallback to the default theme
		if (!string.IsNullOrEmpty(DefaultAssetPrefix))
			path = Path.Join(DefaultAssetPrefix, path);

		return OtherCP!.ModContent.Load<T>(path);
	}

	#endregion
}
