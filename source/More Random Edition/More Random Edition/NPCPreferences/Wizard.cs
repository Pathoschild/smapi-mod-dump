using System.Collections.Generic;

namespace Randomizer
{
	public class Wizard : NPC
	{
		public static List<Item> Loves = new List<Item>
		{
			ItemList.Items[(int)ObjectIndexes.PurpleMushroom],
			ItemList.Items[(int)ObjectIndexes.SolarEssence],
			ItemList.Items[(int)ObjectIndexes.SuperCucumber],
			ItemList.Items[(int)ObjectIndexes.VoidEssence]
		};
	}
}
