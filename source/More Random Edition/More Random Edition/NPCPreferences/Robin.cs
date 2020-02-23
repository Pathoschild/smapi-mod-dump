using System.Collections.Generic;

namespace Randomizer
{
	public class Robin : NPC
	{
		public static List<Item> Loves = new List<Item>
		{
			ItemList.Items[(int)ObjectIndexes.GoatCheese],
			ItemList.Items[(int)ObjectIndexes.Peach],
			ItemList.Items[(int)ObjectIndexes.Spaghetti]
		};
	}
}
