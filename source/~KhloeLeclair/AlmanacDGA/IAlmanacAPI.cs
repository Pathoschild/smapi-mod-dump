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

		void SetCropPriority(IManifest manifest, int priority);

		void SetCropCallback(IManifest manifest, Action action);
		void ClearCropCallback(IManifest manifest);

		void AddCrop(
			IManifest manifest,

			string id,

			Item item,
			string name,

			bool isTrellisCrop,
			bool isGiantCrop,
			bool isPaddyCrop,

			IList<int> phases,
			IList<Texture2D> phaseSpriteTextures,
			IList<Rectangle?> phaseSpriteSources,
			IList<Color?> phaseSpriteColors,
			IList<Texture2D> phaseSpriteOverlayTextures,
			IList<Rectangle?> phaseSpriteOverlaySources,
			IList<Color?> phaseSpriteOverlayColors,

			int regrow,

			WorldDate start,
			WorldDate end
		);

		void AddCrop(
			IManifest manifest,

			string id,

			Item item,
			string name,

			Texture2D spriteTexture,
			Rectangle? spriteSource,
			Color? spriteColor,
			Texture2D spriteOverlayTexture,
			Rectangle? spriteOverlaySource,
			Color? spriteOverlayColor,

			bool isTrellisCrop,
			bool isGiantCrop,
			bool isPaddyCrop,

			IList<int> phases,
			IList<Texture2D> phaseSpriteTextures,
			IList<Rectangle?> phaseSpriteSources,
			IList<Color?> phaseSpriteColors,
			IList<Texture2D> phaseSpriteOverlayTextures,
			IList<Rectangle?> phaseSpriteOverlaySources,
			IList<Color?> phaseSpriteOverlayColors,

			int regrow,

			WorldDate start,
			WorldDate end
		);

		void RemoveCrop(IManifest manifest, string id);

		void ClearCrops(IManifest manifest);

		void InvalidateCrops();
	}
}
