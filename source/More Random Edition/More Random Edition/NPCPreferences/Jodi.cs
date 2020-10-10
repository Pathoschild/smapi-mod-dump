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
	public class Jodi : NPC
	{
		public static List<Item> Loves = new List<Item>
		{
			ItemList.Items[(int)ObjectIndexes.ChocolateCake],
			ItemList.Items[(int)ObjectIndexes.CrispyBass],
			ItemList.Items[(int)ObjectIndexes.Diamond],
			ItemList.Items[(int)ObjectIndexes.EggplantParmesan],
			ItemList.Items[(int)ObjectIndexes.FriedEel],
			ItemList.Items[(int)ObjectIndexes.Pancakes],
			ItemList.Items[(int)ObjectIndexes.RhubarbPie],
			ItemList.Items[(int)ObjectIndexes.VegetableMedley]
		};
	}
}
