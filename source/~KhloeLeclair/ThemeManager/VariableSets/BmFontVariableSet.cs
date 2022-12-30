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
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Web;

using BmFont;

using Microsoft.Xna.Framework.Graphics;

using Newtonsoft.Json;

using StardewModdingAPI.Utilities;

using Leclair.Stardew.Common.Extensions;
using Leclair.Stardew.ThemeManager.Serialization;
using Leclair.Stardew.ThemeManager.Managers;
using StardewModdingAPI;
using Leclair.Stardew.ThemeManager.Models;

namespace Leclair.Stardew.ThemeManager.VariableSets;


[JsonConverter(typeof(RealVariableSetConverter))]
public class BmFontVariableSet : BaseVariableSet<IManagedAsset<IBmFontData>> {

	public override bool TryParseValue(string input, [NotNullWhen(true)] out IManagedAsset<IBmFontData>? result) {
		if (Manifest is not null && Manager is not null) {
			try {
				Uri url = new(new Uri("theme:"), input);
				string path = HttpUtility.UrlDecode(url.AbsolutePath);

				IManagedAsset<XmlSource>? loader;
				bool is_game;

				if (url.Scheme.Equals("game", StringComparison.OrdinalIgnoreCase)) {
					is_game = true;
					if (!ModEntry.Instance.TryGetManagedAsset(path, out loader))
						throw new Exception($"Could not load managed asset for: {path}");

				} else if (url.Scheme.Equals("theme", StringComparison.OrdinalIgnoreCase)) {
					is_game = false;
					loader = Manager.GetManagedAsset<XmlSource>(path, themeId: Manifest.UniqueID);

				} else if (url.Scheme.Equals("default", StringComparison.OrdinalIgnoreCase)) {
					IAssetName name = ModEntry.Instance.Helper.GameContent.ParseAssetName(SpriteTextManager.FONT_DATA_ASSET);
					if (name.IsEquivalentTo(path) && ModEntry.Instance.TryGetManagedAsset(path, out result))
						return true;

					throw new Exception($"Unsupported default value for BmFont: {path}");
				} else
					throw new Exception($"Invalid scheme for BmFont: {url.Scheme}");

				var values = string.IsNullOrWhiteSpace(url.Query) ? null : HttpUtility.ParseQueryString(url.Query);
				float zoom = values?.ReadFloat("PixelZoom") ?? 3f;

				string[] segments = PathUtilities.GetSegments(path);
				string texPath = Path.Join(segments.Take(segments.Length - 1).ToArray());

				IBmFontData Process(Action<IManagedAsset> useAsset, Action markStale) {
					useAsset(loader);

					FontFile file = FontLoader.Parse(loader!.Value!.Source);
					List<Texture2D> pages = new();

					// Load the textures.
					foreach (var page in file.Pages) {
						string asset = string.Join(PathUtilities.PreferredAssetSeparator, texPath, page.File);
						IManagedAsset<Texture2D>? managed;

						if (is_game) {
							if (!ModEntry.Instance.TryGetManagedAsset(asset, out managed))
								throw new Exception($"Could not load managed asset for: {asset}");

						} else
							managed = Manager.GetManagedAsset<Texture2D>(asset, themeId: Manifest.UniqueID);

						if (managed?.Value is null)
							throw new Exception($"Could not load texture for page from: {asset}");

						pages.Add(managed.Value);
						useAsset(managed);
					}

					return new BmFontData(
						file: file, fontPages: pages, pixelZoom: zoom
					);
				}

				result = new ProcessedManagedAsset<IBmFontData>(loader.AssetName, Process);

			} catch (Exception ex) {
				ModEntry.Instance.Log($"Failed to load BmFont from \"{input}\" for {Manifest.UniqueID}: {ex}", LogLevel.Error);
				result = null;
				return false;
			}

			return true;
		}

		result = null;
		return false;
	}

	public override bool TryBackupVariable(string key, [NotNullWhen(true)] out IManagedAsset<IBmFontData>? result) {
		bool tryBase = Manager != null && Manager != ModEntry.Instance.GameThemeManager;
		if (tryBase && ModEntry.Instance.GameTheme?.GetBmFontVariable(key) is IManagedAsset<IBmFontData> data) {
			result = data;
			return true;
		}

		result = null;
		return false;
	}

}
