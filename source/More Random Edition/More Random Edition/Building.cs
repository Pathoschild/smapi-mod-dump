using System.Collections.Generic;
using System.Linq;

namespace Randomizer
{
	/// <summary>
	/// Represents a building - basically, any of the things that Robin can build you
	/// </summary>
	public class Building
	{
		public string Name { get; set; }
		public List<RequiredItem> RequiredItems { get; set; } = new List<RequiredItem>();
		public int Price { get; set; }
		public double MoneyVariablePercentage { get; set; } = 0.25;

		/// <summary>
		/// The center text in the blueprint string - between the items required and the price
		/// </summary>
		public string CenterText { get; set; }

		/// <summary>
		/// The end text in the blueprint string - after the price
		/// </summary>
		public string EndText { get; set; }

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="name">The name of the building - used for the key in the objects to replace</param>
		/// <param name="itemsRequired">The items required and their multipliers</param>
		/// <param name="baseMoneyRequired">The base money required to build this</param>
		/// <param name="centerText">The center text in the blueprint string - between the items required and the price</param>
		/// <param name="endText">The end text in the blueprint string - after the price</param>
		public Building(
			string name,
			List<ItemAndMultiplier> itemsRequired,
			int baseMoneyRequired,
			string centerText,
			string endText = "false")
		{
			Name = name;
			CenterText = centerText;
			EndText = endText;

			PopulateRequiredItems(itemsRequired);
			ComputePrice(baseMoneyRequired);
		}

		/// <summary>
		/// Populates the required items based on the items and their multipliers
		/// Sums up the amounts of items if they have the same id to prevent errors
		/// </summary>
		/// <param name="itemsRequired">The items this building requires</param>
		private void PopulateRequiredItems(List<ItemAndMultiplier> itemsRequired)
		{
			Dictionary<int, RequiredItem> requiredItemsDict = new Dictionary<int, RequiredItem>();
			foreach (ItemAndMultiplier itemAndMultiplier in itemsRequired)
			{
				RequiredItem requiredItem = new RequiredItem(itemAndMultiplier.Item, itemAndMultiplier.Amount);
				int reqiredItemId = requiredItem.Item.Id;
				if (requiredItemsDict.ContainsKey(reqiredItemId))
				{
					requiredItemsDict[reqiredItemId].NumberOfItems += requiredItem.NumberOfItems;
				}
				else
				{
					requiredItemsDict.Add(reqiredItemId, requiredItem);
				}
			}

			RequiredItems = requiredItemsDict.Values.ToList();
		}

		/// <summary>
		/// Computes the price based on the base money
		/// This is any value between the base money, plus or minus the money variable percentage
		/// </summary>
		/// <param name="baseMoneyRequired">The amount the building normally costs</param>
		private void ComputePrice(int baseMoneyRequired)
		{
			int variableAmount = (int)(baseMoneyRequired * MoneyVariablePercentage);
			Price = Range.GetRandomValue(baseMoneyRequired - variableAmount, baseMoneyRequired + variableAmount);
		}

		/// <summary>
		/// Returns the string used for the blueprint object
		/// </summary>
		/// <returns />
		public override string ToString()
		{
			string requiredItemsString = string.Join(" ", RequiredItems.Select(x => x.GetStringForBuildings()));
			return $"{requiredItemsString}/{CenterText}/{Price}/{EndText}";
		}
	}


	/// <summary>
	/// An item and how much to multiply the amount required by
	/// </summary>
	public class ItemAndMultiplier
	{
		public Item Item { get; set; }
		public int Multiplier { get; set; }

		public ItemAndMultiplier(Item item, int multiplier = 1)
		{
			Item = item;
			Multiplier = multiplier;
		}

		/// <summary>
		/// The number of items - note that this will NOT return the same value each time it's called!
		/// </summary>
		public int Amount
		{
			get
			{
				return Item.GetAmountRequiredForCrafting() * Multiplier;
			}
		}
	}
}
