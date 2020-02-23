using System.Collections.Generic;

namespace Randomizer
{
	public class Sandy : NPC
	{
		public static List<Item> Loves = new List<Item>
		{
			ItemList.Items[(int)ObjectIndexes.Crocus],
			ItemList.Items[(int)ObjectIndexes.Daffodil],
			ItemList.Items[(int)ObjectIndexes.SweetPea]
		};
	}
}
