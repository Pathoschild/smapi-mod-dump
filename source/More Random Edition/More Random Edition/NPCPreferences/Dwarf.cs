using System.Collections.Generic;

namespace Randomizer
{
	public class Dwarf : NPC
	{
		public static List<Item> Loves = new List<Item>
		{
			ItemList.Items[(int)ObjectIndexes.Amethyst],
			ItemList.Items[(int)ObjectIndexes.Aquamarine],
			ItemList.Items[(int)ObjectIndexes.Emerald],
			ItemList.Items[(int)ObjectIndexes.Jade],
			ItemList.Items[(int)ObjectIndexes.OmniGeode],
			ItemList.Items[(int)ObjectIndexes.Ruby],
			ItemList.Items[(int)ObjectIndexes.Topaz]
		};
	}
}
