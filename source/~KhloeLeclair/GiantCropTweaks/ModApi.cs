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

using Leclair.Stardew.GiantCropTweaks.Models;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewModdingAPI;

namespace Leclair.Stardew.GiantCropTweaks;

public class ModApi : IGiantCropTweaks {

	internal readonly ModEntry Mod;
	internal readonly IManifest Other;

	public ModApi(ModEntry mod, IManifest other) {
		Mod = mod;
		Other = other;
	}

	public IReadOnlyDictionary<string, IGiantCropData> GiantCrops {
		get {
			Mod.LoadCropData();
			return Mod.ApiData;
		}
	}

	public bool TryGetSource(string id, [NotNullWhen(true)] out Rectangle? source) {
		Mod.LoadCropData();
		if (Mod.ApiData.TryGetValue(id, out var data)) {
			source = new(
				x: data.Corner.X,
				y: data.Corner.Y,
				width: 16 * data.TileSize.X,
				height: 16 * (data.TileSize.Y + 1)
			);
			return true;
		}

		source = null;
		return false;
	}

	public bool TryGetTexture(string id, [NotNullWhen(true)] out Texture2D? texture) {
		Mod.LoadCropData();
		if (Mod.ApiData.TryGetValue(id, out var data)) {
			if (!string.IsNullOrEmpty(data.Texture)) {
				try {
					texture = Mod.Helper.GameContent.Load<Texture2D>(data.Texture);
				} catch (Exception ex) {
					Mod.Log($"Unable to load texture \"{data.Texture}\" for giant crop \"{id}\".", LogLevel.Error, ex);
					texture = null;
				}

				return texture is not null;
			}
		}

		texture = null;
		return false;
	}
}
