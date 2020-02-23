using System.Collections.Generic;

namespace Randomizer
{
	public class Sam : NPC
	{
		public static List<Item> Loves = new List<Item>
		{
			ItemList.Items[(int)ObjectIndexes.CactusFruit],
			ItemList.Items[(int)ObjectIndexes.MapleBar],
			ItemList.Items[(int)ObjectIndexes.Pizza],
			ItemList.Items[(int)ObjectIndexes.Tigerseye]
		};
	}
}
