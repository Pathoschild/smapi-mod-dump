using System;
using System.Collections.Generic;
using System.Linq;

namespace Randomizer
{
	public class CraftingRoomBundle : Bundle
	{
		public static List<BundleTypes> RoomBundleTypes { get; set; }

		/// <summary>
		/// Creates a bundle for the crafts room
		/// </summary>
		protected override void Populate()
		{
			// Force one resource bundle so that there's one possible bundle to complete
			if (!RoomBundleTypes.Contains(BundleTypes.CraftingResource))
			{
				BundleType = Globals.RNGGetAndRemoveRandomValueFromList(RoomBundleTypes);
			}
			else
			{
				RoomBundleTypes.Remove(BundleTypes.CraftingResource);
				BundleType = BundleTypes.CraftingResource;
			}

			List<RequiredItem> potentialItems;
			int numberOfChoices;

			switch (BundleType)
			{
				case BundleTypes.CraftingResource:
					Name = Globals.GetTranslation("bundle-crafting-resource");
					RequiredItems = new List<RequiredItem>
					{
						new RequiredItem((int)ObjectIndexes.Wood, 100, 250),
						new RequiredItem((int)ObjectIndexes.Stone, 100, 250),
						new RequiredItem((int)ObjectIndexes.Fiber, 10, 50),
						new RequiredItem((int)ObjectIndexes.Clay, 10, 50),
						new RequiredItem((int)ObjectIndexes.Hardwood, 1, 10)
					};
					Color = BundleColors.Orange;
					break;
				case BundleTypes.CraftingHappyCrops:
					Name = Globals.GetTranslation("bundle-crafting-happy-crops");
					RequiredItem qualityCrop = new RequiredItem(Globals.RNGGetRandomValueFromList(ItemList.GetCrops()));
					qualityCrop.MinimumQuality = ItemQualities.Gold;
					potentialItems = new List<RequiredItem>
					{
						new RequiredItem((int)ObjectIndexes.Sprinkler, 1, 5),
						new RequiredItem((int)ObjectIndexes.QualitySprinkler, 1, 5),
						new RequiredItem((int)ObjectIndexes.IridiumSprinkler, 1),
						new RequiredItem((int)ObjectIndexes.BasicFertilizer, 10, 20),
						new RequiredItem((int)ObjectIndexes.QualityFertilizer, 10, 20),
						new RequiredItem((int)ObjectIndexes.BasicRetainingSoil, 10, 20),
						new RequiredItem((int)ObjectIndexes.QualityRetainingSoil, 10, 20),
						qualityCrop
					};
					numberOfChoices = Range.GetRandomValue(6, 8);
					RequiredItems = Globals.RNGGetRandomValuesFromList(potentialItems, numberOfChoices);
					MinimumRequiredItems = Range.GetRandomValue(numberOfChoices - 2, numberOfChoices);
					Color = BundleColors.Green;
					break;
				case BundleTypes.CraftingTree:
					Name = Globals.GetTranslation("bundle-crafting-tree");
					potentialItems = new List<RequiredItem>
					{
						new RequiredItem((int)ObjectIndexes.MapleSeed, 1, 5),
						new RequiredItem((int)ObjectIndexes.Acorn, 1, 5),
						new RequiredItem((int)ObjectIndexes.PineCone, 1),
						new RequiredItem((int)ObjectIndexes.OakResin, 1),
						new RequiredItem((int)ObjectIndexes.MapleSyrup, 1),
						new RequiredItem((int)ObjectIndexes.PineTar, 1),
						new RequiredItem(Globals.RNGGetRandomValueFromList(ItemList.GetFruit()), 1),
						new RequiredItem((int)ObjectIndexes.Wood, 100, 200),
						new RequiredItem((int)ObjectIndexes.Hardwood, 25, 50),
						new RequiredItem((int)ObjectIndexes.Driftwood, 5, 10),
					};
					numberOfChoices = Range.GetRandomValue(6, 8);
					RequiredItems = Globals.RNGGetRandomValuesFromList(potentialItems, numberOfChoices);
					MinimumRequiredItems = Range.GetRandomValue(numberOfChoices - 2, numberOfChoices);
					Color = BundleColors.Green;
					break;
				case BundleTypes.CraftingTotems:
					Name = Globals.GetTranslation("bundle-crafting-totems");
					RequiredItems = new List<RequiredItem>
					{
						new RequiredItem((int)ObjectIndexes.WarpTotemFarm),
						new RequiredItem((int)ObjectIndexes.WarpTotemBeach),
						new RequiredItem((int)ObjectIndexes.WarpTotemMountains),
						new RequiredItem((int)ObjectIndexes.RainTotem),
					};
					MinimumRequiredItems = Range.GetRandomValue(3, 4);
					Color = BundleColors.Red;
					break;
				case BundleTypes.CraftingBindle:
					Name = Globals.GetTranslation("bundle-crafting-bindle");
					potentialItems = new List<RequiredItem>
					{

						new RequiredItem(Globals.RNGGetRandomValueFromList(ItemList.GetCookeditems())),
						new RequiredItem(Globals.RNGGetRandomValueFromList(ItemList.GetForagables())),
						new RequiredItem(Globals.RNGGetRandomValueFromList(FishItem.Get())),
						new RequiredItem(Globals.RNGGetRandomValueFromList(
							ItemList.Items.Values.Where(x => x.Id > 0 && x.DifficultyToObtain <= ObtainingDifficulties.LargeTimeRequirements).ToList()).Id
						),
					};
					RequiredItems = new List<RequiredItem>
					{
						new RequiredItem((int)ObjectIndexes.ChewingStick),
						new RequiredItem((int)ObjectIndexes.Cloth),
					};
					RequiredItems.AddRange(Globals.RNGGetRandomValuesFromList(potentialItems, Range.GetRandomValue(2, 3)));
					MinimumRequiredItems = RequiredItems.Count - 1;
					Color = BundleColors.Yellow;
					break;
				case BundleTypes.CraftingSpringForaging:
					GenerateForagingBundle(Seasons.Spring, BundleColors.Green);
					break;
				case BundleTypes.CraftingSummerForaging:
					GenerateForagingBundle(Seasons.Summer, BundleColors.Red);
					break;
				case BundleTypes.CraftingFallForaging:
					GenerateForagingBundle(Seasons.Fall, BundleColors.Orange);
					break;
				case BundleTypes.CraftingWinterForaging:
					GenerateForagingBundle(Seasons.Winter, BundleColors.Cyan);
					break;
				case BundleTypes.CraftingColorOrange:
					Name = Globals.GetTranslation("bundle-crafting-orange");
					potentialItems = RequiredItem.CreateList(new List<int>
					{
						(int)ObjectIndexes.RustySpur,
						(int)ObjectIndexes.RustyCog,
						(int)ObjectIndexes.Sunfish,
						(int)ObjectIndexes.Octopus,
						(int)ObjectIndexes.Sandfish,
						(int)ObjectIndexes.Dorado,
						(int)ObjectIndexes.Lobster,
						(int)ObjectIndexes.Crab,
						(int)ObjectIndexes.GlazedYams,
						(int)ObjectIndexes.FriedEel,
						(int)ObjectIndexes.SpicyEel,
						(int)ObjectIndexes.PaleAle,
						(int)ObjectIndexes.Chanterelle,
						(int)ObjectIndexes.CopperBar,
						(int)ObjectIndexes.QualityFertilizer,
						(int)ObjectIndexes.CopperOre,
						(int)ObjectIndexes.NautilusShell,
						(int)ObjectIndexes.SpiceBerry,
						(int)ObjectIndexes.WinterRoot,
						(int)ObjectIndexes.Tigerseye,
						(int)ObjectIndexes.Baryte,
						(int)ObjectIndexes.LemonStone,
						(int)ObjectIndexes.Orpiment,
						(int)ObjectIndexes.PumpkinPie,
						(int)ObjectIndexes.Apricot,
						(int)ObjectIndexes.Orange,
						(int)ObjectIndexes.LobsterBisque,
						(int)ObjectIndexes.CrabCakes,
						(int)ObjectIndexes.JackOLantern
					});
					RequiredItems = Globals.RNGGetRandomValuesFromList(potentialItems, 8);
					MinimumRequiredItems = Range.GetRandomValue(3, 6);
					Color = BundleColors.Orange;
					break;
				case BundleTypes.CraftingColorYellow:
					Name = Globals.GetTranslation("bundle-crafting-yellow");
					potentialItems = RequiredItem.CreateList(new List<int>
					{
						(int)ObjectIndexes.Daffodil,
						(int)ObjectIndexes.Dandelion,
						(int)ObjectIndexes.Topaz,
						(int)ObjectIndexes.Sap,
						(int)ObjectIndexes.DwarfScrollIV,
						(int)ObjectIndexes.DriedStarfish,
						(int)ObjectIndexes.BoneFlute,
						(int)ObjectIndexes.GoldenMask,
						(int)ObjectIndexes.GoldenRelic,
						(int)ObjectIndexes.StrangeDoll1,
						(int)ObjectIndexes.Pufferfish,
						(int)ObjectIndexes.RainbowTrout,
						(int)ObjectIndexes.Perch,
						(int)ObjectIndexes.Carp,
						(int)ObjectIndexes.Eel,
						(int)ObjectIndexes.SeaCucumber,
						(int)ObjectIndexes.Angler,
						(int)ObjectIndexes.Hay,
						(int)ObjectIndexes.Omelet,
						(int)ObjectIndexes.CheeseCauliflower,
						(int)ObjectIndexes.FriedCalamari,
						(int)ObjectIndexes.LuckyLunch,
						(int)ObjectIndexes.Pizza,
						(int)ObjectIndexes.FishTaco,
						(int)ObjectIndexes.Spaghetti,
						(int)ObjectIndexes.Tortilla,
						(int)ObjectIndexes.FarmersLunch,
						(int)ObjectIndexes.Oil,
						(int)ObjectIndexes.Morel,
						(int)ObjectIndexes.DuckMayonnaise,
						(int)ObjectIndexes.MapleSeed,
						(int)ObjectIndexes.GoldBar,
						(int)ObjectIndexes.Honey,
						(int)ObjectIndexes.Beer,
						(int)ObjectIndexes.MuscleRemedy,
						(int)ObjectIndexes.BasicFertilizer,
						(int)ObjectIndexes.GoldenPumpkin,
						(int)ObjectIndexes.GoldOre,
						(int)ObjectIndexes.StrawFloor,
						(int)ObjectIndexes.Cheese,
						(int)ObjectIndexes.TruffleOil,
						(int)ObjectIndexes.CoffeeBean,
						(int)ObjectIndexes.TreasureChest,
						(int)ObjectIndexes.Mead,
						(int)ObjectIndexes.GlowRing,
						(int)ObjectIndexes.SmallGlowRing,
						(int)ObjectIndexes.RingOfYoba,
						(int)ObjectIndexes.TopazRing,
						(int)ObjectIndexes.Calcite,
						(int)ObjectIndexes.Jagoite,
						(int)ObjectIndexes.Pyrite,
						(int)ObjectIndexes.Sandstone,
						(int)ObjectIndexes.Hematite,
						(int)ObjectIndexes.MapleSyrup,
						(int)ObjectIndexes.SolarEssence
					});
					RequiredItems = Globals.RNGGetRandomValuesFromList(potentialItems, 8);
					MinimumRequiredItems = Range.GetRandomValue(3, 6);
					Color = BundleColors.Yellow;
					break;
			}
		}

		/// <summary>
		/// Generates the bundle for foraging items
		/// </summary>
		/// <param name="season">The season</param>
		/// <param name="color">The color of the bundle</param>
		private void GenerateForagingBundle(Seasons season, BundleColors color)
		{
			string seasonString = Globals.GetTranslation($"seasons-{season.ToString().ToLower()}");
			seasonString = $"{seasonString[0].ToString().ToUpper()}{seasonString.Substring(1)}";

			Name = Globals.GetTranslation($"bundle-crafting-foraging", new { season = seasonString });
			List<RequiredItem> potentialItems = RequiredItem.CreateList(ItemList.GetForagables(season));
			int numberOfChoices = Math.Min(potentialItems.Count, 8);
			RequiredItems = Globals.RNGGetRandomValuesFromList(potentialItems, numberOfChoices);
			MinimumRequiredItems = Range.GetRandomValue(4, numberOfChoices);
			Color = color;
		}

		/// <summary>
		/// Generates the reward for completing a crafting room bundle
		/// </summary>
		/// <returns />
		protected override void GenerateReward()
		{
			var potentialRewards = new List<RequiredItem>
			{
				new RequiredItem(Globals.RNGGetRandomValueFromList(ItemList.GetResources()), 999),
				new RequiredItem((int)ObjectIndexes.Sprinkler, 2, 5),
				new RequiredItem((int)ObjectIndexes.QualitySprinkler, 1, 4),
				new RequiredItem((int)ObjectIndexes.IridiumSprinkler, 1, 3),
				new RequiredItem((int)ObjectIndexes.BasicFertilizer, 100),
				new RequiredItem((int)ObjectIndexes.QualityFertilizer, 100),
				new RequiredItem((int)ObjectIndexes.BasicRetainingSoil, 100),
				new RequiredItem((int)ObjectIndexes.QualityRetainingSoil, 100),
				new RequiredItem((int)ObjectIndexes.OakResin, 25, 50),
				new RequiredItem((int)ObjectIndexes.MapleSyrup, 25, 50),
				new RequiredItem((int)ObjectIndexes.PineTar, 25, 50),
				new RequiredItem((int)ObjectIndexes.Acorn, 25, 50),
				new RequiredItem((int)ObjectIndexes.MapleSeed, 25, 50),
				new RequiredItem((int)ObjectIndexes.PineCone, 25, 50),
				new RequiredItem((int)ObjectIndexes.SpringSeeds, 25, 50),
				new RequiredItem((int)ObjectIndexes.SummerSeeds, 25, 50),
				new RequiredItem((int)ObjectIndexes.FallSeeds, 25, 50),
				new RequiredItem((int)ObjectIndexes.WinterSeeds, 25, 50),
				new RequiredItem(Globals.RNGGetRandomValueFromList(ItemList.GetForagables()), 10, 20),
				new RequiredItem((int)ObjectIndexes.SeedMaker)
			};

			Reward = Globals.RNGGetRandomValueFromList(potentialRewards);
		}
	}
}
