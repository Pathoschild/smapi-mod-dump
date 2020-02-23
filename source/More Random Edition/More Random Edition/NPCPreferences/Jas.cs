using System.Collections.Generic;

namespace Randomizer
{
	public class Jas : NPC
	{
		public static List<Item> Loves = new List<Item>
		{
			ItemList.Items[(int)ObjectIndexes.FairyRose],
			ItemList.Items[(int)ObjectIndexes.PinkCake],
			ItemList.Items[(int)ObjectIndexes.PlumPudding]
		};
	}
}
