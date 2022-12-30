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

using Leclair.Stardew.Common.Extensions;

using StardewModdingAPI;

namespace Leclair.Stardew.ThemeManager.Models;

public class ProcessedManagedAsset<TValue> : IManagedAsset<TValue>, IDisposable where TValue : notnull {

	public delegate TValue ProcessDelegate(Action<IManagedAsset> useAsset, Action markStale);

	public readonly ProcessDelegate ProcessingMethod;

	private HashSet<IManagedAsset> UsedAssets = new();

	private bool _IsLoaded;
	private bool _IsStale;
	private bool _IsDisposed;

	private TValue? _Value;

	#region Life Cycle

	public ProcessedManagedAsset(IAssetName assetName, ProcessDelegate processingMethod) {
		AssetName = assetName;
		ProcessingMethod = processingMethod;
	}

	protected virtual void Dispose(bool disposing) {
		if (!_IsDisposed) {
			if (disposing) {
				foreach(var asset in UsedAssets)
					asset.MarkedStale -= OnMarkedStale;
			}

			_Value = default!;
			_IsDisposed = true;
		}
	}

	public void Dispose() {
		// Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	#endregion

	private void OnMarkedStale(object? sender, EventArgs e) {
		MarkStale();
	}

	#region IManagedAsset Properties

	public IAssetName AssetName { get; }

	public bool IsLoaded => _IsLoaded;

	public bool IsStale => _IsStale;

	public object? RawValue => Value;

	public event EventHandler? MarkedStale;

	#endregion

	#region IManagedAsset<T> Properties

	public TValue? Value {
		get {
			if (!_IsLoaded || _IsStale)
				Load();

			return _Value;
		}
	}

	#endregion

	#region Methods

	public void Load() {
		if (_IsLoaded && !_IsStale)
			return;

		_IsLoaded = true;
		_IsStale = false;

		HashSet<IManagedAsset> newAssets = new();

		void AddAsset(IManagedAsset asset) {
			if (!UsedAssets.Remove(asset))
				asset.MarkedStale += OnMarkedStale;

			newAssets.Add(asset);
		}

		try {
			_Value = ProcessingMethod(AddAsset, MarkStale);
		} catch (Exception ex) {
			ModEntry.Instance.Log($"Unable to process value for processed managed asset: {ex}", LogLevel.Error);
		}

		foreach(var asset in UsedAssets)
			asset.MarkedStale -= OnMarkedStale;

		UsedAssets = newAssets;
	}

	public void MarkStale() {
		_IsStale = true;
		MarkedStale?.SafeInvoke(this, monitor: ModEntry.Instance.Monitor);
	}

	#endregion

}
