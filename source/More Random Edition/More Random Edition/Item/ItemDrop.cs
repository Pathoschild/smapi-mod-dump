/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Chikakoo/stardew-valley-randomizer
**
*************************************************/

using System.Collections.Generic;

namespace Randomizer
{
	/// <summary>
	/// Represents an item drop - contains an item and probability
	/// </summary>
	public class ItemDrop
	{
		public Item ItemToDrop { get; set; }
		public double Probability { get; set; }

		public ItemDrop(ObjectIndexes itemId, double probability)
		{
			ItemToDrop = ItemList.Items[itemId];
			Probability = probability;
		}

        /// <summary>
        /// Parses an item drop string into a list of item drops
        /// - The value -4 is parsed as coal
        /// - The value -6 is parsed as gold ore
		/// 
		/// See the MonsterData Initialize function for more
        /// </summary>
        /// <param name="itemDropString">The string to parse</param>
        /// <returns />
        public static List<ItemDrop> ParseString(string itemDropString)
		{
			List<ItemDrop> itemDrops = new();

			string[] itemTokens = itemDropString.Split(' ');
			for (int i = 0; i + 1 < itemTokens.Length; i += 2)
			{
				if (!int.TryParse(itemTokens[i], out int itemId))
				{
					Globals.ConsoleError($"Invalid token when parsing monster item drop in string: {itemDropString}");
					itemId = (int)ObjectIndexes.Slime;
				}

				if (!double.TryParse(itemTokens[i + 1], out double probability))
				{
					Globals.ConsoleError($"Invalid token when parsing monster item probability in string: {itemDropString}");
					probability = 0.75;
				}

				if (itemId == -4)
				{
					itemId = (int)ObjectIndexes.Coal;
				}
				else if (itemId == -6)
				{
					itemId = (int)ObjectIndexes.GoldOre;
				}

				itemDrops.Add(new ItemDrop((ObjectIndexes)itemId, probability));
			}

			return itemDrops;
		}

		/// <summary>
		/// The string format to use in output dictionaries
		/// </summary>
		/// <returns>String formatted like: "Id Probability" (e.g. 72 0.01)</returns>
		public override string ToString()
		{
			return $"{ItemToDrop.Id} {Probability}";
		}
	}
}
