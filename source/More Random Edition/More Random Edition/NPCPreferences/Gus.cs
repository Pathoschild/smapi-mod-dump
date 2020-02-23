using System.Collections.Generic;

namespace Randomizer
{
	public class Gus : NPC
	{
		public static List<Item> Loves = new List<Item>
		{
			ItemList.Items[(int)ObjectIndexes.Diamond],
			ItemList.Items[(int)ObjectIndexes.Escargot],
			ItemList.Items[(int)ObjectIndexes.FishTaco],
			ItemList.Items[(int)ObjectIndexes.Orange]
		};
	}
}
