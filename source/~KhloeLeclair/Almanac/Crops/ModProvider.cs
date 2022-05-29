/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/KhloeLeclair/StardewMods
**
*************************************************/

#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;

using Leclair.Stardew.Common;

using StardewModdingAPI;

using StardewValley;

namespace Leclair.Stardew.Almanac.Crops;

public class ModProvider : ICropProvider {

	public readonly IManifest Manifest;

	private Action? CropCallback;
	private readonly Dictionary<string, CropInfo> Crops;

	public ModProvider(IManifest manifest, int priority = 0) {
		Manifest = manifest;
		Priority = priority;
		Crops = new();
	}

	public string Name => Manifest.Name;

	public int Priority { get; set; } = 0;

	public void SetCallback(Action? action) {
		CropCallback = action;
	}

	public void ClearCrops() {
		Crops.Clear();
	}

	public void RemoveCrop(string id) {
		if (Crops.ContainsKey(id))
			Crops.Remove(id);
	}

	public IEnumerable<CropInfo> GetCrops() {
		CropCallback?.Invoke();
		return Crops.Values;
	}

	public void AddCrop(
		string id,

		Item item,
		string name,
		SpriteInfo? sprite,

		bool isTrellisCrop,
		bool isGiantCrop,
		SpriteInfo? giantSprite,
		Item[]? seeds,
		bool isPaddyCrop,

		IEnumerable<int> phases,
		IEnumerable<SpriteInfo?>? phaseSprites,

		int regrow,

		WorldDate start,
		WorldDate end
	) {
		Crops[id] = new CropInfo(
			Id: id,
			Item: item,
			Name: name,
			Sprite: sprite,
			IsTrellisCrop: isTrellisCrop,
			IsGiantCrop: isGiantCrop,
			GiantSprite: giantSprite,
			Seeds: seeds,
			Phases: phases.ToArray(),
			PhaseSprites: phaseSprites?.ToArray(),
			Regrow: regrow,
			IsPaddyCrop: isPaddyCrop,
			StartDate: start,
			EndDate: end
		);
	}
}
