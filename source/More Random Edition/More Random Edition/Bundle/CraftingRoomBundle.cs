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
			RNG rng = BundleRandomizer.Rng;

			// Force one resource bundle so that there's one possible bundle to complete
			if (!RoomBundleTypes.Contains(BundleTypes.CraftingResource))
			{
				BundleType = rng.GetAndRemoveRandomValueFromList(RoomBundleTypes);
			}
			else
			{
				RoomBundleTypes.Remove(BundleTypes.CraftingResource);
				BundleType = BundleTypes.CraftingResource;
			}

			List<RequiredBundleItem> potentialItems;
			int numberOfChoices;

			switch (BundleType)
			{
				case BundleTypes.CraftingResource:
					SetBundleName("bundle-crafting-resource");
					RequiredItems = new List<RequiredBundleItem>
					{
						new(ObjectIndexes.Wood, 100, 250),
						new(ObjectIndexes.Stone, 100, 250),
						new(ObjectIndexes.Fiber, 10, 50),
						new(ObjectIndexes.Clay, 10, 50),
						new(ObjectIndexes.Hardwood, 1, 10)
					};
					Color = BundleColors.Orange;
					break;
				case BundleTypes.CraftingHappyCrops:
					SetBundleName("bundle-crafting-happy-crops");
					RequiredBundleItem qualityCrop = new(rng.GetRandomValueFromList(ItemList.GetCrops()));
					qualityCrop.MinimumQuality = ItemQualities.Gold;
					potentialItems = new List<RequiredBundleItem>
					{
						new(ObjectIndexes.Sprinkler, 1, 5),
						new(ObjectIndexes.QualitySprinkler, 1, 5),
						new(ObjectIndexes.IridiumSprinkler, 1),
						new(ObjectIndexes.BasicFertilizer, 10, 20),
						new(ObjectIndexes.QualityFertilizer, 10, 20),
						new(ObjectIndexes.BasicRetainingSoil, 10, 20),
						new(ObjectIndexes.QualityRetainingSoil, 10, 20),
						qualityCrop
					};
					numberOfChoices = rng.NextIntWithinRange(6, 8);
					RequiredItems = rng.GetRandomValuesFromList(potentialItems, numberOfChoices);
					MinimumRequiredItems = rng.NextIntWithinRange(numberOfChoices - 2, numberOfChoices);
					Color = BundleColors.Green;
					break;
				case BundleTypes.CraftingTree:
					SetBundleName("bundle-crafting-tree");
					potentialItems = new List<RequiredBundleItem>
					{
						new(ObjectIndexes.MapleSeed, 1, 5),
						new(ObjectIndexes.Acorn, 1, 5),
						new(ObjectIndexes.PineCone, 1),
						new(ObjectIndexes.OakResin, 1),
						new(ObjectIndexes.MapleSyrup, 1),
						new(ObjectIndexes.PineTar, 1),
						new(rng.GetRandomValueFromList(ItemList.GetFruit()), 1),
						new(ObjectIndexes.Wood, 100, 200),
						new(ObjectIndexes.Hardwood, 25, 50),
						new(ObjectIndexes.Driftwood, 5, 10),
					};
					numberOfChoices = rng.NextIntWithinRange(6, 8);
					RequiredItems = rng.GetRandomValuesFromList(potentialItems, numberOfChoices);
					MinimumRequiredItems = rng.NextIntWithinRange(numberOfChoices - 2, numberOfChoices);
					Color = BundleColors.Green;
					break;
				case BundleTypes.CraftingTotems:
					SetBundleName("bundle-crafting-totems");
					RequiredItems = new()
					{
						new(ObjectIndexes.WarpTotemFarm),
						new(ObjectIndexes.WarpTotemBeach),
						new(ObjectIndexes.WarpTotemMountains),
						new(ObjectIndexes.WarpTotemDesert),
						new(ObjectIndexes.RainTotem),
					};
					MinimumRequiredItems = rng.NextIntWithinRange(3, 4);
					Color = BundleColors.Red;
					break;
				case BundleTypes.CraftingBindle:
					SetBundleName("bundle-crafting-bindle");
					potentialItems = new List<RequiredBundleItem>()
					{

						new(rng.GetRandomValueFromList(ItemList.GetCookedItems())),
						new(rng.GetRandomValueFromList(ItemList.GetForagables())),
						new(rng.GetRandomValueFromList(FishItem.Get())),
						new(rng.GetRandomValueFromList(
							ItemList.Items.Values
								.Where(x => 
									x.DifficultyToObtain <= ObtainingDifficulties.LargeTimeRequirements &&
									!x.IsCooked &&
									!x.IsForagable &&
									!x.IsFish)
								.ToList()
							).ObjectIndex
						)
					};
					RequiredItems = new()
					{
						new(ObjectIndexes.ChewingStick),
						new(ObjectIndexes.Cloth),
					};
					RequiredItems.AddRange(rng.GetRandomValuesFromList(potentialItems, rng.NextIntWithinRange(2, 3)));
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
					potentialItems = RequiredBundleItem.CreateList(new List<ObjectIndexes>
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
					RequiredItems = rng.GetRandomValuesFromList(potentialItems, 8);
					MinimumRequiredItems = rng.NextIntWithinRange(3, 6);
					Color = BundleColors.Orange;
					break;
				case BundleTypes.CraftingColorYellow:
					SetBundleName("bundle-crafting-yellow");
					potentialItems = RequiredBundleItem.CreateList(new List<ObjectIndexes>
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
					RequiredItems = rng.GetRandomValuesFromList(potentialItems, 8);
					MinimumRequiredItems = rng.NextIntWithinRange(3, 6);
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
			List<RequiredBundleItem> potentialItems = RequiredBundleItem.CreateList(ItemList.GetForagables(season));
			int numberOfChoices = Math.Min(potentialItems.Count, 8);

			RNG rng = BundleRandomizer.Rng;
			RequiredItems = rng.GetRandomValuesFromList(potentialItems, numberOfChoices);
			MinimumRequiredItems = rng.NextIntWithinRange(4, numberOfChoices);
			Color = color;
		}

		/// <summary>
		/// Generates the reward for completing a crafting room bundle
		/// </summary>
		/// <returns />
		protected override void GenerateReward()
		{
            RNG rng = BundleRandomizer.Rng;

            var potentialRewards = new List<RequiredBundleItem>
			{
				new(rng.GetRandomValueFromList(ItemList.GetResources()), 999),
				new(ObjectIndexes.Sprinkler, 2, 5),
				new(ObjectIndexes.QualitySprinkler, 1, 4),
				new(ObjectIndexes.IridiumSprinkler, 1, 3),
				new(ObjectIndexes.BasicFertilizer, 100),
				new(ObjectIndexes.QualityFertilizer, 100),
				new(ObjectIndexes.BasicRetainingSoil, 100),
				new(ObjectIndexes.QualityRetainingSoil, 100),
				new(ObjectIndexes.OakResin, 25, 50),
				new(ObjectIndexes.MapleSyrup, 25, 50),
				new(ObjectIndexes.PineTar, 25, 50),
				new(ObjectIndexes.Acorn, 25, 50),
				new(ObjectIndexes.MapleSeed, 25, 50),
				new(ObjectIndexes.PineCone, 25, 50),
				new(ObjectIndexes.SpringSeeds, 25, 50),
				new(ObjectIndexes.SummerSeeds, 25, 50),
				new(ObjectIndexes.FallSeeds, 25, 50),
				new(ObjectIndexes.WinterSeeds, 25, 50),
				new(rng.GetRandomValueFromList(ItemList.GetForagables()), 10, 20),
				new(BigCraftableIndexes.SeedMaker)
			};

			Reward = rng.GetRandomValueFromList(potentialRewards);
		}
	}
}
