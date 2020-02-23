using System.Collections.Generic;

namespace Randomizer
{
	public class Sebastian : NPC
	{
		public static List<Item> Loves = new List<Item>
		{
			ItemList.Items[(int)ObjectIndexes.FrozenTear],
			ItemList.Items[(int)ObjectIndexes.Obsidian],
			ItemList.Items[(int)ObjectIndexes.PumpkinSoup],
			ItemList.Items[(int)ObjectIndexes.Sashimi],
			ItemList.Items[(int)ObjectIndexes.VoidEgg]
		};
	}
}
