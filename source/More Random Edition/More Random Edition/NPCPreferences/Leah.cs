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
	public class Leah : NPC
	{
		public static List<Item> Loves = new List<Item>
		{
			ItemList.Items[(int)ObjectIndexes.GoatCheese],
			ItemList.Items[(int)ObjectIndexes.PoppyseedMuffin],
			ItemList.Items[(int)ObjectIndexes.Salad],
			ItemList.Items[(int)ObjectIndexes.StirFry],
			ItemList.Items[(int)ObjectIndexes.Truffle],
			ItemList.Items[(int)ObjectIndexes.VegetableMedley],
			ItemList.Items[(int)ObjectIndexes.Wine]
		};
	}
}
