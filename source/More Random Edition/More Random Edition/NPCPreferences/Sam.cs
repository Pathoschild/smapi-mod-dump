/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Chikakoo/stardew-valley-randomizer
**
*************************************************/

using System.Collections.Generic;

namespace Randomizer
{
	public class Sam : NPC
	{
		public static List<Item> Loves = new List<Item>
		{
			ItemList.Items[(int)ObjectIndexes.CactusFruit],
			ItemList.Items[(int)ObjectIndexes.MapleBar],
			ItemList.Items[(int)ObjectIndexes.Pizza],
			ItemList.Items[(int)ObjectIndexes.Tigerseye]
		};
	}
}
