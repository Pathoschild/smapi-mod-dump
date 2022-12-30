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
using System.Diagnostics.CodeAnalysis;
using System.Web;

using Leclair.Stardew.ThemeManager.Serialization;

using Microsoft.Xna.Framework.Graphics;

using Newtonsoft.Json;

namespace Leclair.Stardew.ThemeManager.VariableSets;

[JsonConverter(typeof(RealVariableSetConverter))]
public class TextureVariableSet : BaseVariableSet<IManagedAsset<Texture2D>> {

	public override bool TryParseValue(string input, [NotNullWhen(true)] out IManagedAsset<Texture2D>? result) {
		if (Manifest is not null && Manager is not null) {
			try {
				Uri url = new(new Uri("theme:"), input);
				string path = HttpUtility.UrlDecode(url.AbsolutePath);

				if (url.Scheme.Equals("game", StringComparison.OrdinalIgnoreCase)) {
					if (!ModEntry.Instance.TryGetManagedAsset(path, out result))
						throw new Exception($"Could not load managed asset for: {path}");

				} else if (url.Scheme.Equals("theme", StringComparison.OrdinalIgnoreCase)) {
					result = Manager.GetManagedAsset<Texture2D>(path, themeId: Manifest.UniqueID);

				} else
					throw new Exception($"Invalid scheme for texture: {url.Scheme}");

			} catch (Exception ex) {
				ModEntry.Instance.Log($"Failed to load Texture2D asset from \"{input}\" for {Manifest.UniqueID}: {ex}", StardewModdingAPI.LogLevel.Error);
				result = null;
			}

			return result is not null;
		}

		result = null;
		return false;
	}

	public override bool TryBackupVariable(string key, [NotNullWhen(true)] out IManagedAsset<Texture2D>? result) {
		bool tryBase = Manager != null && Manager != ModEntry.Instance.GameThemeManager;
		if (tryBase && ModEntry.Instance.GameTheme?.GetManagedTextureVariable(key) is IManagedAsset<Texture2D> tex) {
			result = tex;
			return true;
		}

		result = null;
		return false;
	}

}
