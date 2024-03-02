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

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">The name of the building - used for the key in the objects to replace</param>
        /// <param name="itemsRequired">The items required and their multipliers</param>
        /// <param name="baseMoneyRequired">The base money required to build this</param>
        public Building(
			string name,
			List<ItemAndMultiplier> itemsRequired,
			int baseMoneyRequired)
		{
			Name = name;
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
				RequiredItem requiredItem = new(itemAndMultiplier.Item, itemAndMultiplier.Amount);
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
            const double MoneyVariablePercentage = 0.25;
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

			string[] originalData = BlueprintRandomizer.BuildingData[Name].Split("/");
			originalData[(int)BlueprintIndexes.RequiredItems] = requiredItemsString;
            originalData[(int)BlueprintIndexes.MoneyRequired] = Price.ToString();

			return string.Join("/", originalData);
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
