/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Chikakoo/stardew-valley-randomizer
**
*************************************************/

using StardewValley;
using StardewValley.GameData.Crops;
using System.Collections.Generic;
using System.Linq;

namespace Randomizer
{
	/// <summary>
	/// Represents a seed
	/// </summary>
	public class SeedItem : Item
	{
		public int Price { get; set; }
		public string Description
		{
			get
			{
				if (ObjectIndex == ObjectIndexes.CoffeeBean)
				{
					Item coffee = ObjectIndexes.Coffee.GetItem();
					string coffeeName = Globals.GetTranslation("item-coffee-name", new { itemName = coffee.CoffeeIngredient });
					return Globals.GetTranslation("item-coffee-bean-description", new { itemName = coffee.CoffeeIngredient, coffeeName });
				}

				CropItem growsCrop = (CropItem)ItemList.Items[CropId];
				string flowerString = growsCrop.IsFlower ? $"{Globals.GetTranslation("crop-tooltip-flower")} " : "";
				string scytheString = NeedsScythe ? $"{Globals.GetTranslation("crop-tooltip-needs-scythe")} " : "";
				string trellisString = IsTrellisCrop ? $"{Globals.GetTranslation("crop-tooltip-trellis")} " : "";
				string growthString = RegrowsAfterHarvest
					? $"{Globals.GetTranslation($"crop-tooltip-growth-time-reproduces", new { daysToGrow = TimeToGrow })} "
					: $"{Globals.GetTranslation($"crop-tooltip-growth-time", new { daysToGrow = TimeToGrow })} ";
				string seasonsString = $"{Globals.GetTranslation("crop-tooltip-seasons", new { seasons = GetSeasonsStringForDisplay() })} ";
				string indoorsString = IsIndoorCrop ? $"{Globals.GetTranslation("crop-tooltip-indoor-crop")} " : "";
				string waterString = CropGrowthInfo.IsPaddyCrop ? $"{Globals.GetTranslation("crop-tooltip-paddy-crop")} " : "";

				return $"{flowerString}{scytheString}{trellisString}{growthString}{seasonsString}{indoorsString}{waterString}";
			}
		}
		public CropData CropGrowthInfo { get; set; }
		public List<Seasons> GrowingSeasons { 
			get
			{
				return CropGrowthInfo.Seasons
					.Cast<Seasons>()
					.ToList();
            }
		}
		public bool IsTrellisCrop { get => CropGrowthInfo.IsRaised; }
        public string CropId { get => CropGrowthInfo.HarvestItemId; }
		public int TimeToGrow { get => CropGrowthInfo.DaysInPhase.Sum(); }
		public bool NeedsScythe { get => CropGrowthInfo.HarvestMethod == HarvestMethod.Scythe; }
        public bool RegrowsAfterHarvest { get => CropGrowthInfo.RegrowDays != -1; }
		public bool CanGetExtraCrops { get => CropGrowthInfo.ExtraHarvestChance > 0; }
		public bool IsIndoorCrop
		{
			get => CropGrowthInfo.PlantableLocationRules?
				.Any(rule => rule.Id == "NotOutsideUnlessGingerIsland") ?? false;
		}
		public bool HasTint { get => CropGrowthInfo.TintColors.Any(); }

        public bool Randomize { get; set; } = true;

		/// <summary>
		/// Whether to shuffle this item amongst all the other seeds
		/// Currently setting the new artifact seeds to false for balance reasons
		/// </summary>
		public bool ShuffleBetweenSeeds { get; set; } = true;

		/// <summary>
		/// The constructor
		/// The crop growth information is modified in the CropRandomizer
		/// </summary>
		/// <param name="index">The index of the seed item</param>
		public SeedItem(ObjectIndexes index) : base(index)
		{
			IsSeed = true;
			CropGrowthInfo = DataLoader.Crops(Game1.content)[Id];
            DifficultyToObtain = ObtainingDifficulties.LargeTimeRequirements;
		}

        /// <summary>
        /// Get the string that's used for seasons when displaying the tooltip
        /// </summary>
        /// <returns>The seasons string</returns>
        public string GetSeasonsStringForDisplay()
        {
			return string.Join(", ",
				GrowingSeasons.Select(season =>
					Globals.GetTranslation($"seasons-{season.ToString().ToLower()}")));
        }
    }
}
