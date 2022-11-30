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
using System.Collections;
using System.Diagnostics.CodeAnalysis;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewValley.BellsAndWhistles;
using StardewModdingAPI;

using Leclair.Stardew.Common;
using Leclair.Stardew.ThemeManager.Models;

namespace Leclair.Stardew.ThemeManager;

public class ModAPI : IThemeManagerApi {

	internal readonly ModEntry Mod;
	internal readonly IManifest Other;

	internal ModAPI(ModEntry mod, IManifest other) {
		Mod = mod;
		Other = other;

		Mod.BaseThemeManager!.ThemeChanged += BaseThemeManager_ThemeChanged;
	}

	private void BaseThemeManager_ThemeChanged(object? sender, IThemeChangedEvent<BaseTheme> e) {
		BaseThemeChanged?.Invoke(sender, new ThemeChangedEventArgs<IBaseTheme>(
			e.OldId, e.OldManifest, e.OldData,
			e.NewId, e.NewManifest, e.NewData
		));
	}

	#region Base Theme

	public IBaseTheme BaseTheme => Mod.BaseTheme!;

	public event EventHandler<IThemeChangedEvent<IBaseTheme>>? BaseThemeChanged;

	#endregion

	#region Custom Themes

	public ITypedThemeManager<DataT> GetOrCreateManager<DataT>(DataT? defaultTheme = null, string? embeddedThemesPath = "assets/themes", string? assetPrefix = "assets", string? assetLoaderPrefix = null, string? themeLoaderPath = null, bool? forceAssetRedirection = null, bool? forceThemeRedirection = null) where DataT : class, new() {
		ITypedThemeManager<DataT>? manager;
		lock ((Mod.Managers as ICollection).SyncRoot) {
			if (Mod.Managers.TryGetValue(Other, out var mdata)) {
				manager = mdata.Item2 as ITypedThemeManager<DataT>;
				if (manager is null || mdata.Item1 != typeof(DataT))
					throw new InvalidCastException($"Cannot convert {mdata.Item1} to {typeof(DataT)}");

				return manager;
			}
		}

		if (!Mod.Config.SelectedThemes.TryGetValue(Other.UniqueID, out string? selected))
			selected = "automatic";

		manager = new ThemeManager<DataT>(
			mod: Mod,
			other: Other,
			selectedThemeId: selected,
			defaultTheme: defaultTheme,
			embeddedThemesPath: embeddedThemesPath,
			assetPrefix: assetPrefix,
			assetLoaderPrefix: assetLoaderPrefix,
			themeLoaderPath: themeLoaderPath,
			forceAssetRedirection: forceAssetRedirection,
			forceThemeRedirection: forceThemeRedirection
		);

		lock ((Mod.Managers as ICollection).SyncRoot) {
			Mod.Managers[Other] = (typeof(DataT), manager);
		}

		if (manager.UsingThemeRedirection)
			lock ((Mod.ManagersByThemeAsset as ICollection).SyncRoot) {
				Mod.ManagersByThemeAsset[manager.ThemeLoaderPath] = manager;
			}

		manager.Discover();
		return manager;
	}

	public bool TryGetManager([NotNullWhen(true)] out IThemeManager? themeManager, IManifest? forMod = null) {
		lock((Mod.Managers as ICollection).SyncRoot) {
			if (!Mod.Managers.TryGetValue(forMod ?? Other, out var manager)) {
				themeManager = null;
				return false;
			}

			themeManager = manager.Item2;
			return true;
		}
	}

	public bool TryGetManager<DataT>([NotNullWhen(true)] out ITypedThemeManager<DataT>? themeManager, IManifest? forMod = null) where DataT : class, new() {
		lock ((Mod.Managers as ICollection).SyncRoot) {
			if (!Mod.Managers.TryGetValue(forMod ?? Other, out var manager) || manager.Item1 != typeof(DataT)) {
				themeManager = null;
				return false;
			}

			themeManager = manager.Item2 as ITypedThemeManager<DataT>;
			return themeManager is not null;
		}
	}

	#endregion

	#region Color Parsing

	public bool TryParseColor(string value, [NotNullWhen(true)] out Color? color) {
		if (string.IsNullOrWhiteSpace(value)) {
			color = default;
			return false;
		}

		if (value.StartsWith('$')) {
			if (BaseTheme is not null && BaseTheme.Variables.TryGetValue(value[1..], out var c)) {
				color = c;
				return true;
			}

			color = default;
			return false;
		}

		return CommonHelper.TryParseColor(value, out color);
	}

	#endregion

	#region Colored SpriteText

	public void DrawSpriteText(SpriteBatch batch, string text, int x, int y, Color? color, float alpha = 1f) {
		int c = color.HasValue ? CommonHelper.PackColor(color.Value) + 100 : -1;

		SpriteText.drawString(batch, text, x, y, alpha: alpha, color: c);
	}

	#endregion

}
