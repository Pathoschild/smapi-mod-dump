using System.Collections.Generic;

namespace Randomizer
{
	public class Evelyn : NPC
	{
		public static List<Item> Loves = new List<Item>
		{
			ItemList.Items[(int)ObjectIndexes.Beet],
			ItemList.Items[(int)ObjectIndexes.ChocolateCake],
			ItemList.Items[(int)ObjectIndexes.Diamond],
			ItemList.Items[(int)ObjectIndexes.FairyRose],
			ItemList.Items[(int)ObjectIndexes.Stuffing],
			ItemList.Items[(int)ObjectIndexes.Tulip]
		};
	}
}
