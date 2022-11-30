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

using Microsoft.Xna.Framework.Graphics;

using Netcode;
using StardewModdingAPI;

using Leclair.Stardew.Common.Integrations;
using MoreGiantCrops;
using System.Linq;

namespace Leclair.Stardew.Almanac.Integrations.MoreGiantCrops;

public class MGCIntegration : BaseAPIIntegration<IMoreGiantCropsApi, ModEntry> {

	private HashSet<int>? Crops;

	public MGCIntegration(ModEntry mod)
	: base(mod, "spacechase0.MoreGiantCrops", "1.2.0") { }
	public bool IsGiantCrop(int id) {
		if (!IsLoaded)
			return false;

		Crops ??= new HashSet<int>(API.RegisteredCrops() ?? Array.Empty<int>());
		return Crops.Contains(id);
	}

	public Texture2D? GetGiantCropTexture(int id) {
		if (!IsLoaded)
			return null;

		return API.GetTexture(id);
	}
}
