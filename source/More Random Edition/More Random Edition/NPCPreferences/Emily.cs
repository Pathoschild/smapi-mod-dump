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
	public class Emily : NPC
	{
		public static List<Item> Loves = new List<Item>
		{
			ItemList.Items[(int)ObjectIndexes.Amethyst],
			ItemList.Items[(int)ObjectIndexes.Aquamarine],
			ItemList.Items[(int)ObjectIndexes.Cloth],
			ItemList.Items[(int)ObjectIndexes.Emerald],
			ItemList.Items[(int)ObjectIndexes.Jade],
			ItemList.Items[(int)ObjectIndexes.Ruby],
			ItemList.Items[(int)ObjectIndexes.SurvivalBurger],
			ItemList.Items[(int)ObjectIndexes.Topaz],
			ItemList.Items[(int)ObjectIndexes.Wool]
		};
	}
}
