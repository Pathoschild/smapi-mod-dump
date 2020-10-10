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
	public class Penny : NPC
	{
		public static List<Item> Loves = new List<Item>
		{
			ItemList.Items[(int)ObjectIndexes.Diamond],
			ItemList.Items[(int)ObjectIndexes.Emerald],
			ItemList.Items[(int)ObjectIndexes.Melon],
			ItemList.Items[(int)ObjectIndexes.Poppy],
			ItemList.Items[(int)ObjectIndexes.PoppyseedMuffin],
			ItemList.Items[(int)ObjectIndexes.RedPlate],
			ItemList.Items[(int)ObjectIndexes.RootsPlatter],
			ItemList.Items[(int)ObjectIndexes.Sandfish],
			ItemList.Items[(int)ObjectIndexes.TomKhaSoup]
		};
	}
}
