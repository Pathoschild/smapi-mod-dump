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
using System.Collections.Immutable;
using System.Linq;

namespace Leclair.Stardew.ThemeManager.Models;

/// <inheritdoc />
public class ThemesDiscoveredEventArgs<DataT> : EventArgs, IThemesDiscoveredEvent<DataT> {

	/// <inheritdoc />
	public IReadOnlyDictionary<string, IThemeManifest> Manifests { get; }

	/// <inheritdoc />
	public IReadOnlyDictionary<string, DataT> Data { get; }

	internal ThemesDiscoveredEventArgs(Dictionary<string, Theme<DataT>> data) {
		Manifests = data.Select(x => new KeyValuePair<string, IThemeManifest>(x.Key, x.Value.Manifest)).ToImmutableDictionary();
		Data = data.Select(x => new KeyValuePair<string, DataT>(x.Key, x.Value.Data)).ToImmutableDictionary();
	}

}
