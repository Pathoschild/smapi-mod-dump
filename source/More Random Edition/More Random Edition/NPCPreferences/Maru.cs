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
	public class Maru : NPC
	{
		public static List<Item> Loves = new List<Item>
		{
			ItemList.Items[(int)ObjectIndexes.Battery],
			ItemList.Items[(int)ObjectIndexes.Cauliflower],
			ItemList.Items[(int)ObjectIndexes.CheeseCauliflower],
			ItemList.Items[(int)ObjectIndexes.Diamond],
			ItemList.Items[(int)ObjectIndexes.GoldBar],
			ItemList.Items[(int)ObjectIndexes.IridiumBar],
			ItemList.Items[(int)ObjectIndexes.MinersTreat],
			ItemList.Items[(int)ObjectIndexes.PepperPoppers],
			ItemList.Items[(int)ObjectIndexes.RhubarbPie],
			ItemList.Items[(int)ObjectIndexes.Strawberry]
		};
	}
}
