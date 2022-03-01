/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/KhloeLeclair/StardewMods
**
*************************************************/

using System.Collections.Generic;

namespace Leclair.Stardew.Almanac {
	public interface ICropProvider {

		int Priority { get; }

		IEnumerable<CropInfo> GetCrops();

	}
}
