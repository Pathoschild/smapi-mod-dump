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

using StardewModdingAPI;

using Leclair.Stardew.Common.Extensions;

namespace Leclair.Stardew.ThemeManager.Models;

internal class ManagedAsset<TValue> : IManagedAsset<TValue> where TValue : notnull {

	#region Fields

	private readonly ModEntry Mod;
	private bool _IsStale;
	private bool _IsLoaded;
	private TValue? _Value;

	#endregion

	#region Life Cycle

	public ManagedAsset(ModEntry mod, IAssetName assetName) {
		Mod = mod;
		AssetName = assetName;
	}

	#endregion

	#region IManagedAsset Properties

	/// <inheritdoc />
	public IAssetName AssetName { get; }

	/// <inheritdoc />
	public bool IsLoaded => _IsLoaded;

	/// <inheritdoc />
	public bool IsStale => _IsStale;

	/// <inheritdoc />
	public object? RawValue => Value;

	/// <inheritdoc />
	public event EventHandler? MarkedStale;

	#endregion

	#region IManagedAsset<T> Properties

	/// <inheritdoc />
	public TValue? Value {
		get {
			if (!_IsLoaded || _IsStale)
				Load();

			return _Value;
		}
	}

	#endregion

	#region Methods

	/// <inheritdoc />
	public void Load() {
		if (_IsLoaded && ! _IsStale)
			return;

		_IsLoaded = true;
		_IsStale = false;

		try {
			_Value = Mod.Helper.GameContent.Load<TValue>(AssetName);
		} catch(Exception ex) {
			Mod.Log($"Unable to load value from {AssetName} for managed asset: {ex}", LogLevel.Error);
		}
	}

	/// <inheritdoc />
	public void MarkStale() {
		_IsStale = true;
		MarkedStale?.SafeInvoke(this, monitor: Mod.Monitor);
	}

	#endregion

}
