using System.Collections.Generic;

namespace Randomizer
{
	public class Willy : NPC
	{
		public static List<Item> Loves = new List<Item>
		{
			ItemList.Items[(int)ObjectIndexes.Catfish],
			ItemList.Items[(int)ObjectIndexes.Diamond],
			ItemList.Items[(int)ObjectIndexes.IridiumBar],
			ItemList.Items[(int)ObjectIndexes.Mead],
			ItemList.Items[(int)ObjectIndexes.Octopus],
			ItemList.Items[(int)ObjectIndexes.Pumpkin],
			ItemList.Items[(int)ObjectIndexes.SeaCucumber],
			ItemList.Items[(int)ObjectIndexes.Sturgeon]
		};
	}
}
