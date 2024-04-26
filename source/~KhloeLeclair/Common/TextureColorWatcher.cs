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
using System.Linq;

using Leclair.Stardew.Common.Events;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewModdingAPI;
using StardewModdingAPI.Events;

using StardewValley;

namespace Leclair.Stardew.Common;

public class TextureColorWatcher : IDisposable {

	private static readonly Dictionary<Color, Point[]> PALETTE_SOURCE = new() {
		// Button Outer Edge
        { new Color(91, 43, 42), [new Point(162, 441), new Point(172, 440), new Point(168, 455)] },

        // Button Inside Edge
        { new Color(247, 186, 0), [new Point(163, 444), new Point(171, 441), new Point(176, 447)] },

        // Button Icon Shadow
        { new Color(177, 78, 5), [new Point(165, 443), new Point(166, 454), new Point(173, 454)] },

        // Button Background
        { new Color(220, 123, 5), [new Point(163, 441), new Point(170, 442), new Point(176, 454)] },

        // Menu background
        { new Color(255, 210, 132), [new Point(21, 373), new Point(26, 377), new Point(21, 381)] },

		// Greyed Out Icons
		{ new Color(198, 100, 5), [new Point(165, 443), new Point(166, 454), new Point(173, 454)] }

	};

	// Stupid hack for me inventing colors.
	private static readonly Dictionary<Color, Color> DO_NOT_SET_IF_MATCHED = new() {
		{ new Color(198, 100, 5), new Color(177, 78, 5) }
	};


	private readonly Mod Mod;
	private readonly string AssetPrefix;
	private readonly Func<string, AssetRequestedEventArgs, Func<IRawTextureData>?> Loader;

	private bool disposedValue;

	private bool CheckForChanges = false;
	private Dictionary<Color, Color>? Palette;
	private HashSet<IAssetName>? RequestedAssets;

	#region Life Cycle

	public TextureColorWatcher(Mod mod, string assetPrefix, Func<string, AssetRequestedEventArgs, Func<IRawTextureData>?> loader) {

		Mod = mod;
		AssetPrefix = assetPrefix;
		Loader = loader;

		Mod.Helper.Events.Content.AssetsInvalidated += OnAssetInvalidated;
		Mod.Helper.Events.Content.AssetRequested += OnAssetRequested;
		Mod.Helper.Events.Content.AssetReady += OnAssetReady;
	}

	protected virtual void Dispose(bool disposing) {
		if (!disposedValue) {

			Mod.Helper.Events.Content.AssetsInvalidated -= OnAssetInvalidated;
			Mod.Helper.Events.Content.AssetRequested -= OnAssetRequested;
			Mod.Helper.Events.Content.AssetReady -= OnAssetReady;

			Palette = null;
			RequestedAssets = null;

			disposedValue = true;
		}
	}

	public void Dispose() {
		// Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	#endregion

	public void Invalidate() {
		Palette = null;
		if (RequestedAssets is not null) {
			var assets = RequestedAssets;
			RequestedAssets = null;
			Mod.Helper.GameContent.InvalidateCache(asset => assets.Contains(asset.Name));
		}
	}

	#region Events

	private void OnAssetInvalidated(object? sender, AssetsInvalidatedEventArgs e) {
		foreach (var name in e.Names)
			if (name.IsEquivalentTo(@"LooseSprites/Cursors")) {
				CheckForChanges = true;
				return;
			}
	}

	private void OnAssetReady(object? sender, AssetReadyEventArgs e) {
		if (CheckForChanges && e.Name.IsEquivalentTo(@"LooseSprites/Cursors"))
			CheckPaletteChanged();
	}

	private void OnAssetRequested(object? sender, AssetRequestedEventArgs e) {
		if (!e.Name.StartsWith(AssetPrefix))
			return;

		var provider = Loader(e.Name.BaseName[AssetPrefix.Length..], e);
		if (provider == null)
			return;

		e.LoadFrom(() => {

			RequestedAssets ??= new();
			RequestedAssets.Add(e.Name);

			LoadPalette();

			IRawTextureData data = provider();

			var copy = data.Data.ToArray();

			for (int i = 0; i < copy.Length; i++)
				if (Palette.TryGetValue(copy[i], out var color))
					copy[i] = color;

			var tex = new Texture2D(Game1.spriteBatch.GraphicsDevice, data.Width, data.Height);
			tex.SetData(copy);
			return tex;

		}, AssetLoadPriority.Low);

	}

	#endregion

	#region Loading the Palette

	private void CheckPaletteChanged() {
		CheckForChanges = false;

		if (Palette is null)
			return;

		if (RequestedAssets is null) {
			Palette = null;
			return;
		}

		var old_palette = Palette;
		Palette = null;
		if (LoadPalette(old_palette) && RequestedAssets is not null) { 
			var assets = RequestedAssets;
			RequestedAssets = null;
			Mod.Helper.GameContent.InvalidateCache(asset => assets.Contains(asset.Name));
		}
	}

	[MemberNotNull(nameof(Palette))]
	public bool LoadPalette(Dictionary<Color, Color>? oldValues = null) {
		if (Palette is not null)
			return false;

		var source = Mod.Helper.GameContent.Load<Texture2D>(@"LooseSprites/Cursors");

		var colors = new Color[source.Width * source.Height];
		source.GetData(colors);

		Palette = new();
		bool changed = false;

		foreach (var pair in PALETTE_SOURCE) {
			Color value = pair.Value
				.Select(point => colors[point.X + (point.Y * source.Width)])
				.GroupBy(sample => sample)
				.OrderByDescending(group => group.Count())
				.First()
				.Key;

			if (DO_NOT_SET_IF_MATCHED.TryGetValue(pair.Key, out var doNotMatch) && value == doNotMatch)
				value = pair.Key;

			if (oldValues != null && oldValues.TryGetValue(pair.Key, out var oldColor) && oldColor != value)
				changed = true;

			if (pair.Key != value)
				Palette[pair.Key] = value;
		}

		if (Palette.Count != (oldValues?.Count ?? 0))
			changed = true;

		return changed;
	}

	#endregion

}
