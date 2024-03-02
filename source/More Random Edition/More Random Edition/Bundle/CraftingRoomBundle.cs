/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Chikakoo/stardew-valley-randomizer
**
*************************************************/

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
					SetBundleName("bundle-crafting-resource");
					RequiredItems = new List<RequiredItem>
					{
						new RequiredItem(ObjectIndexes.Wood, 100, 250),
						new RequiredItem(ObjectIndexes.Stone, 100, 250),
						new RequiredItem(ObjectIndexes.Fiber, 10, 50),
						new RequiredItem(ObjectIndexes.Clay, 10, 50),
						new RequiredItem(ObjectIndexes.Hardwood, 1, 10)
					};
					Color = BundleColors.Orange;
					break;
				case BundleTypes.CraftingHappyCrops:
					SetBundleName("bundle-crafting-happy-crops");
					RequiredItem qualityCrop = new RequiredItem(Globals.RNGGetRandomValueFromList(ItemList.GetCrops()));
					qualityCrop.MinimumQuality = ItemQualities.Gold;
					potentialItems = new List<RequiredItem>
					{
						new RequiredItem(ObjectIndexes.Sprinkler, 1, 5),
						new RequiredItem(ObjectIndexes.QualitySprinkler, 1, 5),
						new RequiredItem(ObjectIndexes.IridiumSprinkler, 1),
						new RequiredItem(ObjectIndexes.BasicFertilizer, 10, 20),
						new RequiredItem(ObjectIndexes.QualityFertilizer, 10, 20),
						new RequiredItem(ObjectIndexes.BasicRetainingSoil, 10, 20),
						new RequiredItem(ObjectIndexes.QualityRetainingSoil, 10, 20),
						qualityCrop
					};
					numberOfChoices = Range.GetRandomValue(6, 8);
					RequiredItems = Globals.RNGGetRandomValuesFromList(potentialItems, numberOfChoices);
					MinimumRequiredItems = Range.GetRandomValue(numberOfChoices - 2, numberOfChoices);
					Color = BundleColors.Green;
					break;
				case BundleTypes.CraftingTree:
					SetBundleName("bundle-crafting-tree");
					potentialItems = new List<RequiredItem>
					{
						new RequiredItem(ObjectIndexes.MapleSeed, 1, 5),
						new RequiredItem(ObjectIndexes.Acorn, 1, 5),
						new RequiredItem(ObjectIndexes.PineCone, 1),
						new RequiredItem(ObjectIndexes.OakResin, 1),
						new RequiredItem(ObjectIndexes.MapleSyrup, 1),
						new RequiredItem(ObjectIndexes.PineTar, 1),
						new RequiredItem(Globals.RNGGetRandomValueFromList(ItemList.GetFruit()), 1),
						new RequiredItem(ObjectIndexes.Wood, 100, 200),
						new RequiredItem(ObjectIndexes.Hardwood, 25, 50),
						new RequiredItem(ObjectIndexes.Driftwood, 5, 10),
					};
					numberOfChoices = Range.GetRandomValue(6, 8);
					RequiredItems = Globals.RNGGetRandomValuesFromList(potentialItems, numberOfChoices);
					MinimumRequiredItems = Range.GetRandomValue(numberOfChoices - 2, numberOfChoices);
					Color = BundleColors.Green;
					break;
				case BundleTypes.CraftingTotems:
					SetBundleName("bundle-crafting-totems");
					RequiredItems = new List<RequiredItem>
					{
						new RequiredItem(ObjectIndexes.WarpTotemFarm),
						new RequiredItem(ObjectIndexes.WarpTotemBeach),
						new RequiredItem(ObjectIndexes.WarpTotemMountains),
						new RequiredItem(ObjectIndexes.WarpTotemDesert),
						new RequiredItem(ObjectIndexes.RainTotem),
					};
					MinimumRequiredItems = Range.GetRandomValue(3, 4);
					Color = BundleColors.Red;
					break;
				case BundleTypes.CraftingBindle:
					SetBundleName("bundle-crafting-bindle");
					potentialItems = new List<RequiredItem>
					{

						new(Globals.RNGGetRandomValueFromList(ItemList.GetCookedItems())),
						new(Globals.RNGGetRandomValueFromList(ItemList.GetForagables())),
						new(Globals.RNGGetRandomValueFromList(FishItem.Get())),
						new((ObjectIndexes)Globals.RNGGetRandomValueFromList(
							ItemList.Items.Values
								.Where(x => 
									x.Id > 0 && x.DifficultyToObtain <= ObtainingDifficulties.LargeTimeRequirements)
								.ToList()
							).Id
						),
					};
					RequiredItems = new List<RequiredItem>
					{
						new RequiredItem(ObjectIndexes.ChewingStick),
						new RequiredItem(ObjectIndexes.Cloth),
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
					SetBundleName("bundle-crafting-orange");
					potentialItems = RequiredItem.CreateList(new List<ObjectIndexes>
					{
						ObjectIndexes.RustySpur,
						ObjectIndexes.RustyCog,
						ObjectIndexes.Lobster,
						ObjectIndexes.Crab,
						ObjectIndexes.GlazedYams,
						ObjectIndexes.FriedEel,
						ObjectIndexes.SpicyEel,
						ObjectIndexes.PaleAle,
						ObjectIndexes.Chanterelle,
						ObjectIndexes.CopperBar,
						ObjectIndexes.QualityFertilizer,
						ObjectIndexes.CopperOre,
						ObjectIndexes.NautilusShell,
						ObjectIndexes.SpiceBerry,
						ObjectIndexes.WinterRoot,
						ObjectIndexes.Tigerseye,
						ObjectIndexes.Baryte,
						ObjectIndexes.LemonStone,
						ObjectIndexes.Orpiment,
						ObjectIndexes.PumpkinPie,
						ObjectIndexes.Apricot,
						ObjectIndexes.Orange,
						ObjectIndexes.LobsterBisque,
						ObjectIndexes.CrabCakes,
						ObjectIndexes.JackOLantern
					});
					RequiredItems = Globals.RNGGetRandomValuesFromList(potentialItems, 8);
					MinimumRequiredItems = Range.GetRandomValue(3, 6);
					Color = BundleColors.Orange;
					break;
				case BundleTypes.CraftingColorYellow:
					SetBundleName("bundle-crafting-yellow");
					potentialItems = RequiredItem.CreateList(new List<ObjectIndexes>
					{
						ObjectIndexes.Daffodil,
						ObjectIndexes.Dandelion,
						ObjectIndexes.Topaz,
						ObjectIndexes.Sap,
						ObjectIndexes.DwarfScrollIV,
						ObjectIndexes.DriedStarfish,
						ObjectIndexes.BoneFlute,
						ObjectIndexes.GoldenMask,
						ObjectIndexes.GoldenRelic,
						ObjectIndexes.StrangeDoll1,
						ObjectIndexes.Hay,
						ObjectIndexes.Omelet,
						ObjectIndexes.CheeseCauliflower,
						ObjectIndexes.FriedCalamari,
						ObjectIndexes.LuckyLunch,
						ObjectIndexes.Pizza,
						ObjectIndexes.FishTaco,
						ObjectIndexes.Spaghetti,
						ObjectIndexes.Tortilla,
						ObjectIndexes.FarmersLunch,
						ObjectIndexes.Oil,
						ObjectIndexes.Morel,
						ObjectIndexes.DuckMayonnaise,
						ObjectIndexes.MapleSeed,
						ObjectIndexes.GoldBar,
						ObjectIndexes.Honey,
						ObjectIndexes.Beer,
						ObjectIndexes.MuscleRemedy,
						ObjectIndexes.BasicFertilizer,
						ObjectIndexes.GoldenPumpkin,
						ObjectIndexes.GoldOre,
						ObjectIndexes.StrawFloor,
						ObjectIndexes.Cheese,
						ObjectIndexes.TruffleOil,
						ObjectIndexes.CoffeeBean,
						ObjectIndexes.TreasureChest,
						ObjectIndexes.Mead,
						ObjectIndexes.GlowRing,
						ObjectIndexes.SmallGlowRing,
						ObjectIndexes.RingOfYoba,
						ObjectIndexes.TopazRing,
						ObjectIndexes.Calcite,
						ObjectIndexes.Jagoite,
						ObjectIndexes.Pyrite,
						ObjectIndexes.Sandstone,
						ObjectIndexes.Hematite,
						ObjectIndexes.MapleSyrup,
						ObjectIndexes.SolarEssence
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

			SetBundleName($"bundle-crafting-foraging", new { season = seasonString });
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
				new RequiredItem(ObjectIndexes.Sprinkler, 2, 5),
				new RequiredItem(ObjectIndexes.QualitySprinkler, 1, 4),
				new RequiredItem(ObjectIndexes.IridiumSprinkler, 1, 3),
				new RequiredItem(ObjectIndexes.BasicFertilizer, 100),
				new RequiredItem(ObjectIndexes.QualityFertilizer, 100),
				new RequiredItem(ObjectIndexes.BasicRetainingSoil, 100),
				new RequiredItem(ObjectIndexes.QualityRetainingSoil, 100),
				new RequiredItem(ObjectIndexes.OakResin, 25, 50),
				new RequiredItem(ObjectIndexes.MapleSyrup, 25, 50),
				new RequiredItem(ObjectIndexes.PineTar, 25, 50),
				new RequiredItem(ObjectIndexes.Acorn, 25, 50),
				new RequiredItem(ObjectIndexes.MapleSeed, 25, 50),
				new RequiredItem(ObjectIndexes.PineCone, 25, 50),
				new RequiredItem(ObjectIndexes.SpringSeeds, 25, 50),
				new RequiredItem(ObjectIndexes.SummerSeeds, 25, 50),
				new RequiredItem(ObjectIndexes.FallSeeds, 25, 50),
				new RequiredItem(ObjectIndexes.WinterSeeds, 25, 50),
				new RequiredItem(Globals.RNGGetRandomValueFromList(ItemList.GetForagables()), 10, 20),
				new RequiredItem(BigCraftableIndexes.SeedMaker)
			};

			Reward = Globals.RNGGetRandomValueFromList(potentialRewards);
		}
	}
}
