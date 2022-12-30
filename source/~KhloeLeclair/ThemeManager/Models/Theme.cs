/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/KhloeLeclair/StardewMods
**
*************************************************/

using StardewModdingAPI;

namespace Leclair.Stardew.ThemeManager.Models;

/// <summary>
/// Theme records group together a theme's <typeparamref name="DataT"/>
/// and <see cref="IContentPack"/> instances for convenience.
/// </summary>
/// <typeparam name="DataT">Your mod's BaseThemeData subclass</typeparam>
/// <param name="Data">The theme's theme data.</param>
/// <param name="Content">The theme's content pack.</param>
internal record Theme<DataT>(
	DataT Data,
	ThemeManifest Manifest,
	IContentPack? Content,
	string? RelativePath
);
