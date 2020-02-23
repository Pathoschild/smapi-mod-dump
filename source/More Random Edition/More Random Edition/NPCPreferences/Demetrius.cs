using System.Collections.Generic;

namespace Randomizer
{
	public class Demetrius : NPC
	{
		public static List<Item> Loves = new List<Item>
		{
			ItemList.Items[(int)ObjectIndexes.BeanHotpot],
			ItemList.Items[(int)ObjectIndexes.IceCream],
			ItemList.Items[(int)ObjectIndexes.RicePudding],
			ItemList.Items[(int)ObjectIndexes.Strawberry]
		};
	}
}
