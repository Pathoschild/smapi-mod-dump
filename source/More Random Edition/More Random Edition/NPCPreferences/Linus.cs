using System.Collections.Generic;

namespace Randomizer
{
	public class Linus : NPC
	{
		public static List<Item> Loves = new List<Item>
		{
			ItemList.Items[(int)ObjectIndexes.BlueberryTart],
			ItemList.Items[(int)ObjectIndexes.CactusFruit],
			ItemList.Items[(int)ObjectIndexes.Coconut],
			ItemList.Items[(int)ObjectIndexes.DishOTheSea],
			ItemList.Items[(int)ObjectIndexes.Yam]
		};
	}
}
