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

		public ItemDrop(int itemId, double probability)
		{
			ItemToDrop = ItemList.Items[itemId];
			Probability = probability;
		}

		/// <summary>
		/// Parses an item drop string into a list of item drops
		/// </summary>
		/// <param name="itemDropString">The string to parse</param>
		/// <returns />
		public static List<ItemDrop> ParseString(string itemDropString)
		{
			List<ItemDrop> itemDrops = new List<ItemDrop>();

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

				itemDrops.Add(new ItemDrop(itemId, probability));
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
