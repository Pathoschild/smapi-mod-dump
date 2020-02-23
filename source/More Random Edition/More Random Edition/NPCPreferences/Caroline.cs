using System.Collections.Generic;

namespace Randomizer
{
	public class Caroline : NPC
	{
		public static List<Item> Loves = new List<Item>
		{
			ItemList.Items[(int)ObjectIndexes.FishTaco],
			ItemList.Items[(int)ObjectIndexes.SummerSpangle]
		};
	}
}
