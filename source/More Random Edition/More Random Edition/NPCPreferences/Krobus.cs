using System.Collections.Generic;

namespace Randomizer
{
	public class Krobus : NPC
	{
		public static List<Item> Loves = new List<Item>
		{
			ItemList.Items[(int)ObjectIndexes.Diamond],
			ItemList.Items[(int)ObjectIndexes.IridiumBar],
			ItemList.Items[(int)ObjectIndexes.Pumpkin],
			ItemList.Items[(int)ObjectIndexes.VoidEgg],
			ItemList.Items[(int)ObjectIndexes.VoidMayonnaise],
			ItemList.Items[(int)ObjectIndexes.WildHorseradish]
		};
	}
}
