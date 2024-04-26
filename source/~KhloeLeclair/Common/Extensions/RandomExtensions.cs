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
using System.Text;

namespace Leclair.Stardew.Common.Extensions;

internal static class RandomExtensions {

	public static bool GetChance(this Random rnd, double chance) {
		if (chance < 0.0) return false;
		if (chance >= 1.0) return true;
		return rnd.NextDouble() < chance;
	}

}
