using System.Collections.Generic;

namespace Randomizer
{
	public class Lewis : NPC
	{
		public static List<Item> Loves = new List<Item>
		{
			ItemList.Items[(int)ObjectIndexes.AutumnsBounty],
			ItemList.Items[(int)ObjectIndexes.GlazedYams],
			ItemList.Items[(int)ObjectIndexes.HotPepper],
			ItemList.Items[(int)ObjectIndexes.VegetableMedley]
		};
	}
}
