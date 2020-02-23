using System.Collections.Generic;

namespace Randomizer
{
	public class George : NPC
	{
		public static List<Item> Loves = new List<Item>
		{
			ItemList.Items[(int)ObjectIndexes.FriedMushroom],
			ItemList.Items[(int)ObjectIndexes.Leek]
		};
	}
}
