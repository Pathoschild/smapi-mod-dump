using System.Collections.Generic;

namespace Randomizer
{
	public class Alex : NPC
	{
		public static List<Item> Loves = new List<Item>
		{
			ItemList.Items[(int)ObjectIndexes.CompleteBreakfast],
			ItemList.Items[(int)ObjectIndexes.SalmonDinner]
		};
	}
}
