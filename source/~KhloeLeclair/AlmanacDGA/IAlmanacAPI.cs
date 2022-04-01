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

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewModdingAPI;

using StardewValley;

namespace Leclair.Stardew.Almanac {

	public interface IAlmanacAPI {

		int DaysPerMonth { get; }

		void SetCropPriority(IManifest manifest, int priority);

		void SetCropCallback(IManifest manifest, Action action);
		void ClearCropCallback(IManifest manifest);

		void AddCrop(
			IManifest manifest,

			string id,

			Item item,
			string name,

			int regrow,
			bool isGiantCrop,
			bool isPaddyCrop,
			bool isTrellisCrop,

			Item[] seeds,

			WorldDate start,
			WorldDate end,

			Tuple<Texture2D, Rectangle?, Color?, Texture2D, Rectangle?, Color?> sprite,
			Tuple<Texture2D, Rectangle?, Color?, Texture2D, Rectangle?, Color?> giantSprite,

			IReadOnlyCollection<int> phases,
			IReadOnlyCollection<Tuple<Texture2D, Rectangle?, Color?, Texture2D, Rectangle?, Color?>> phaseSprites
		);

		void RemoveCrop(IManifest manifest, string id);

		void ClearCrops(IManifest manifest);

		void InvalidateCrops();

	}
}
