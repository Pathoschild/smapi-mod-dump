using System.Collections.Generic;

namespace Randomizer
{
	public class Jodi : NPC
	{
		public static List<Item> Loves = new List<Item>
		{
			ItemList.Items[(int)ObjectIndexes.ChocolateCake],
			ItemList.Items[(int)ObjectIndexes.CrispyBass],
			ItemList.Items[(int)ObjectIndexes.Diamond],
			ItemList.Items[(int)ObjectIndexes.EggplantParmesan],
			ItemList.Items[(int)ObjectIndexes.FriedEel],
			ItemList.Items[(int)ObjectIndexes.Pancakes],
			ItemList.Items[(int)ObjectIndexes.RhubarbPie],
			ItemList.Items[(int)ObjectIndexes.VegetableMedley]
		};
	}
}
