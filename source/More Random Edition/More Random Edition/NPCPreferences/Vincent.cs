using System.Collections.Generic;

namespace Randomizer
{
	public class Vincent : NPC
	{
		public static List<Item> Loves = new List<Item>
		{
			ItemList.Items[(int)ObjectIndexes.CranberryCandy],
			ItemList.Items[(int)ObjectIndexes.Grape],
			ItemList.Items[(int)ObjectIndexes.PinkCake]
		};
	}
}
