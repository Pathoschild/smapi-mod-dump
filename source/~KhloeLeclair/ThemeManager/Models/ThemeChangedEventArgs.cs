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

namespace Leclair.Stardew.ThemeManager.Models;

/// <inheritdoc />
public class ThemeChangedEventArgs<DataT> : EventArgs, IThemeChangedEvent<DataT> {
	/// <inheritdoc />
	public string OldId { get; }

	/// <inheritdoc />
	public string NewId { get; }

	/// <inheritdoc />
	public IThemeManifest? OldManifest { get; }

	/// <inheritdoc />
	public IThemeManifest? NewManifest { get; }

	/// <inheritdoc />
	public DataT? OldData { get; }

	/// <inheritdoc />
	public DataT NewData { get; }

	public ThemeChangedEventArgs(string oldId, IThemeManifest? oldManifest, DataT? oldData, string newID, IThemeManifest? newManifest, DataT newData) {
		OldId = oldId;
		NewId = newID;
		OldManifest = oldManifest;
		NewManifest = newManifest;
		OldData = oldData;
		NewData = newData;
	}
}
