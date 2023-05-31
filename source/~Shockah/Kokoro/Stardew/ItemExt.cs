/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Shockah/Stardew-Valley-Mods
**
*************************************************/

using StardewValley;
using System;
using System.Linq;

namespace Shockah.Kokoro.Stardew
{
	public static class ItemExt
	{
		public static bool IsSameItem(this Item self, Item other)
			=> self.CompareTo(other) == 0
			&& self.modData.Pairs.OrderBy(kvp => kvp.Key).SequenceEqual(other.modData.Pairs.OrderBy(kvp => kvp.Key));
	}
}