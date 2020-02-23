using System.Collections.Generic;

namespace Randomizer
{
	public class Abigail : NPC
	{
		public static List<Item> Loves = new List<Item>
		{
			ItemList.Items[(int)ObjectIndexes.Amethyst],
			ItemList.Items[(int)ObjectIndexes.BlackberryCobbler],
			ItemList.Items[(int)ObjectIndexes.ChocolateCake],
			ItemList.Items[(int)ObjectIndexes.Pufferfish],
			ItemList.Items[(int)ObjectIndexes.Pumpkin],
			ItemList.Items[(int)ObjectIndexes.SpicyEel]
		};
	}
}
