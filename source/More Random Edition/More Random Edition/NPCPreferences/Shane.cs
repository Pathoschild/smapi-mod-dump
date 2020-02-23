using System.Collections.Generic;

namespace Randomizer
{
	public class Shane : NPC
	{
		public static List<Item> Loves = new List<Item>
		{
			ItemList.Items[(int)ObjectIndexes.Beer],
			ItemList.Items[(int)ObjectIndexes.HotPepper],
			ItemList.Items[(int)ObjectIndexes.PepperPoppers],
			ItemList.Items[(int)ObjectIndexes.Pizza]
		};
	}
}
