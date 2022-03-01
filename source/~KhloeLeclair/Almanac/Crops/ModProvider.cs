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
using System.Linq;

using Leclair.Stardew.Common;

using StardewModdingAPI;

using StardewValley;

namespace Leclair.Stardew.Almanac.Crops {
	public class ModProvider : ICropProvider {

		public readonly IManifest Manifest;

		private Action CropCallback;
		private Dictionary<string, CropInfo> Crops;

		public ModProvider(IManifest manifest, int priority = 0) {
			Manifest = manifest;
			Priority = priority;
			Crops = new();
		}

		public int Priority { get; set; } = 0;

		public void SetCallback(Action action) {
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
			SpriteInfo sprite,

			bool isTrellisCrop,
			bool isGiantCrop,
			bool isPaddyCrop,

			IEnumerable<int> phases,
			IEnumerable<SpriteInfo> phaseSprites,

			int regrow,

			WorldDate start,
			WorldDate end
		) {
			Crops[id] = new CropInfo(
				id,
				item,
				name,
				sprite,
				isGiantCrop,
				isTrellisCrop,
				phases.ToArray(),
				regrow,
				isPaddyCrop,
				phaseSprites.ToArray(),
				start,
				end
			);
		}

	}
}
