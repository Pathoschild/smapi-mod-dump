using System.Collections.Generic;

namespace Randomizer
{
	public class Kent : NPC
	{
		public static List<Item> Loves = new List<Item>
		{
			ItemList.Items[(int)ObjectIndexes.FiddleheadRisotto],
			ItemList.Items[(int)ObjectIndexes.RoastedHazelnuts]
		};
	}
}
