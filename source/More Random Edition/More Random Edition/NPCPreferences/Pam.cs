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
	public class Pam : NPC
	{
		public static List<Item> Loves = new List<Item>
		{
			ItemList.Items[(int)ObjectIndexes.Beer],
			ItemList.Items[(int)ObjectIndexes.CactusFruit],
			ItemList.Items[(int)ObjectIndexes.GlazedYams],
			ItemList.Items[(int)ObjectIndexes.Mead],
			ItemList.Items[(int)ObjectIndexes.PaleAle],
			ItemList.Items[(int)ObjectIndexes.Parsnip],
			ItemList.Items[(int)ObjectIndexes.ParsnipSoup]
		};
	}
}
