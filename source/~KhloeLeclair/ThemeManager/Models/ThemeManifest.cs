/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/KhloeLeclair/StardewMods
**
*************************************************/

using System.Collections.Generic;

using StardewModdingAPI;

namespace Leclair.Stardew.ThemeManager.Models;

public class ThemeManifest : IThemeManifest {

	#region Constructor

	internal ThemeManifest(string uniqueID, string name, IReadOnlyDictionary<string, string>? localizedNames, string? translationKey, IManifest providingMod, string[]? supportedMods, string[]? unsupportedMods, string? fallbackTheme, string? assetPrefix, bool? overrideRedirection, bool nonSelectable) {
		UniqueID = uniqueID;
		Name = name;
		LocalizedNames = localizedNames;
		TranslationKey = translationKey;
		ProvidingMod = providingMod;
		SupportedMods = supportedMods;
		UnsupportedMods = unsupportedMods;
		FallbackTheme = fallbackTheme;
		AssetPrefix = assetPrefix;
		OverrideRedirection = overrideRedirection;
		NonSelectable = nonSelectable;
	}

	#endregion

	#region Identification

	public string UniqueID { get; }

	public string Name { get; }

	public IReadOnlyDictionary<string, string>? LocalizedNames { get; }

	public string? TranslationKey { get; }

	public IManifest ProvidingMod { get; }

	public bool NonSelectable { get; }

	#endregion

	#region Theme Selection

	public string[]? SupportedMods { get; }

	public string[]? UnsupportedMods { get; }

	#endregion

	#region Asset Loading

	public string? FallbackTheme { get; }

	public string? AssetPrefix { get; }

	public bool? OverrideRedirection { get; }

	#endregion

	#region Methods

	public bool MatchesForAutomatic(IModRegistry registry) {
		if (UnsupportedMods != null)
			foreach (string mod in UnsupportedMods) {
				if (!string.IsNullOrEmpty(mod) && registry.IsLoaded(mod))
					return false;
			}

		if (SupportedMods != null)
			foreach (string mod in SupportedMods) {
				if (!string.IsNullOrEmpty(mod) && registry.IsLoaded(mod))
					return true;
			}

		return false;
	}

	#endregion
}
