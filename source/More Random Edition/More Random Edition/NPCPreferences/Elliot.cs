using System.Collections.Generic;

namespace Randomizer
{
	public class Elliot : NPC
	{
		public static List<Item> Loves = new List<Item>
		{
			ItemList.Items[(int)ObjectIndexes.CrabCakes],
			ItemList.Items[(int)ObjectIndexes.DuckFeather],
			ItemList.Items[(int)ObjectIndexes.Lobster],
			ItemList.Items[(int)ObjectIndexes.Pomegranate],
			ItemList.Items[(int)ObjectIndexes.TomKhaSoup]
		};
	}
}
