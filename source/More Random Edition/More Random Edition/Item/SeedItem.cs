using System.Collections.Generic;

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
				if (Id == (int)ObjectIndexes.CoffeeBean)
				{
					Item coffee = ItemList.Items[(int)ObjectIndexes.Coffee];
					string coffeeName = Globals.GetTranslation("item-coffee-name", new { itemName = coffee.CoffeeIngredient });
					return Globals.GetTranslation("item-coffee-bean-description", new { itemName = coffee.CoffeeIngredient, coffeeName });
				}

				CropItem growsCrop = (CropItem)ItemList.Items[CropGrowthInfo.CropId];
				string flowerString = growsCrop.IsFlower ? $"{Globals.GetTranslation("crop-tooltip-flower")} " : "";
				string scytheString = CropGrowthInfo.CanScythe ? $"{Globals.GetTranslation("crop-tooltip-needs-scythe")} " : "";
				string trellisString = CropGrowthInfo.IsTrellisCrop ? $"{Globals.GetTranslation("crop-tooltip-trellis")} " : "";
				string growthString = CropGrowthInfo.RegrowsAfterHarvest ?
					$"{Globals.GetTranslation($"crop-tooltip-growth-time-reproduces", new { daysToGrow = CropGrowthInfo.TimeToGrow })} " :
					$"{Globals.GetTranslation($"crop-tooltip-growth-time", new { daysToGrow = CropGrowthInfo.TimeToGrow })} ";
				string seasonsString = $"{Globals.GetTranslation("crop-tooltip-seasons", new { seasons = CropGrowthInfo.GetSeasonsStringForDisplay() })} ";
				string indoorsString = growsCrop.Id == (int)ObjectIndexes.CactusFruit ? $"{Globals.GetTranslation("crop-tooltip-cactus-fruit")} " : "";
				string waterString = growsCrop.Id == (int)ObjectIndexes.UnmilledRice ? $"{Globals.GetTranslation("crop-tooltip-rice-shoot")} " : "";

				return $"{flowerString}{scytheString}{trellisString}{growthString}{seasonsString}{indoorsString}{waterString}";
			}
		}
		public CropGrowthInformation CropGrowthInfo { get { return CropGrowthInformation.CropIdsToInfo[Id]; } }
		public List<Seasons> GrowingSeasons { get; set; }

		public bool Randomize { get; set; } = true;

		public SeedItem(int id, List<Seasons> growingSeasons) : base(id)
		{
			IsSeed = true;
			DifficultyToObtain = ObtainingDifficulties.LargeTimeRequirements;
			GrowingSeasons = growingSeasons;
		}

		/// <summary>
		/// Gets the string that's part of Data/ObjectInformation
		/// </summary>
		/// <returns />
		public override string ToString()
		{
			string displayName = string.IsNullOrEmpty(OverrideDisplayName) ? Name : OverrideDisplayName;
			return $"{Name}/{Price}/-300/Seeds -74/{displayName}/{Description}";
		}
	}
}
