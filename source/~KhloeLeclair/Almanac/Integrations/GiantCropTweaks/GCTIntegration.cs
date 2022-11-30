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
using System.Text;
using System.Threading.Tasks;

using Leclair.Stardew.Common.Integrations;
using Leclair.Stardew.GiantCropTweaks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Leclair.Stardew.Almanac.Integrations.GiantCropTweaks;

internal class GCTIntegration : BaseAPIIntegration<IGiantCropTweaks, ModEntry> {

	public GCTIntegration(ModEntry mod)
	: base(mod, "leclair.giantcroptweaks", "0.0.1") { }

	public bool IsGiantCrop(int id) {
		if (!IsLoaded)
			return false;

		return API.GiantCrops.ContainsKey(id.ToString());
	}

	public Texture2D? GetGiantCropTexture(int id) {
		if (!IsLoaded)
			return null;

		if (API.TryGetTexture(id.ToString(), out var tex))
			return tex;

		return null;
	}

	public Rectangle? GetGiantCropSource(int id) {
		if (!IsLoaded)
			return null;

		if (API.TryGetSource(id.ToString(), out var src))
			return src;

		return null;
	}

}
