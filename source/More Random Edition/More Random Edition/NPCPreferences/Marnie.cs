using System.Collections.Generic;

namespace Randomizer
{
	public class Marnie : NPC
	{
		public static List<Item> Loves = new List<Item>
		{
			ItemList.Items[(int)ObjectIndexes.Diamond],
			ItemList.Items[(int)ObjectIndexes.FarmersLunch],
			ItemList.Items[(int)ObjectIndexes.PinkCake],
			ItemList.Items[(int)ObjectIndexes.PumpkinPie]
		};
	}
}
