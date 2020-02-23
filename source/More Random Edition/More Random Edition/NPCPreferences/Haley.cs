using System.Collections.Generic;

namespace Randomizer
{
	public class Haley : NPC
	{
		public static List<Item> Loves = new List<Item>
		{
			ItemList.Items[(int)ObjectIndexes.Coconut],
			ItemList.Items[(int)ObjectIndexes.FruitSalad],
			ItemList.Items[(int)ObjectIndexes.PinkCake],
			ItemList.Items[(int)ObjectIndexes.Sunflower]
		};
	}
}
