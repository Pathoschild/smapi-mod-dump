using System.Collections.Generic;

namespace Randomizer
{
	public class Leah : NPC
	{
		public static List<Item> Loves = new List<Item>
		{
			ItemList.Items[(int)ObjectIndexes.GoatCheese],
			ItemList.Items[(int)ObjectIndexes.PoppyseedMuffin],
			ItemList.Items[(int)ObjectIndexes.Salad],
			ItemList.Items[(int)ObjectIndexes.StirFry],
			ItemList.Items[(int)ObjectIndexes.Truffle],
			ItemList.Items[(int)ObjectIndexes.VegetableMedley],
			ItemList.Items[(int)ObjectIndexes.Wine]
		};
	}
}
