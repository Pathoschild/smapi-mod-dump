using System.Collections.Generic;

namespace Randomizer
{
	public class Pierre : NPC
	{
		public static List<Item> Loves = new List<Item>
		{
			ItemList.Items[(int)ObjectIndexes.FriedCalamari]
		};
	}
}
