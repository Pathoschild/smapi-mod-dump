using System.Collections.Generic;

namespace Randomizer
{
	public class Harvey : NPC
	{
		public static List<Item> Loves = new List<Item>
		{
			ItemList.Items[(int)ObjectIndexes.Coffee],
			ItemList.Items[(int)ObjectIndexes.Pickles],
			ItemList.Items[(int)ObjectIndexes.SuperMeal],
			ItemList.Items[(int)ObjectIndexes.TruffleOil],
			ItemList.Items[(int)ObjectIndexes.Wine]
		};
	}
}
