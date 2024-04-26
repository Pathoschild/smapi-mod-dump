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
	public class PantryBundle : Bundle
	{
		public static List<BundleTypes> RoomBundleTypes { get; set; }

		/// <summary>
		/// Creates a bundle for the pantry
		/// </summary>
		protected override void Populate()
		{
            RNG rng = BundleRandomizer.Rng;

            BundleType = rng.GetAndRemoveRandomValueFromList(RoomBundleTypes);
			List<RequiredBundleItem> potentialItems = new List<RequiredBundleItem>();

			switch (BundleType)
			{
				case BundleTypes.PantryAnimal:
					SetBundleName("bundle-pantry-animal");
					potentialItems = RequiredBundleItem.CreateList(ItemList.GetAnimalProducts());
					potentialItems.Add(new RequiredBundleItem(ObjectIndexes.Hay, 25, 50));
					RequiredItems = rng.GetRandomValuesFromList(potentialItems, rng.NextIntWithinRange(6, 8));
					MinimumRequiredItems = rng.NextIntWithinRange(RequiredItems.Count - 2, RequiredItems.Count);
					Color = BundleColors.Orange;
					break;
				case BundleTypes.PantryQualityCrops:
					SetBundleName("bundle-pantry-quality-crops");
					potentialItems = RequiredBundleItem.CreateList(ItemList.GetCrops());
					potentialItems.ForEach(x => x.MinimumQuality = ItemQualities.Gold);
					RequiredItems = rng.GetRandomValuesFromList(potentialItems, 8);
					MinimumRequiredItems = rng.NextIntWithinRange(4, 6);
					Color = BundleColors.Green;
					break;
				case BundleTypes.PantryQualityForagables:
					SetBundleName("bundle-pantry-quality-foragables");
					potentialItems = RequiredBundleItem.CreateList(ItemList.GetForagables());
					potentialItems.ForEach(x => x.MinimumQuality = ItemQualities.Gold);
					RequiredItems = rng.GetRandomValuesFromList(potentialItems, 8);
					MinimumRequiredItems = rng.NextIntWithinRange(4, 6);
					Color = BundleColors.Green;
					break;
				case BundleTypes.PantryCooked:
					SetBundleName("bundle-pantry-cooked");
					potentialItems = RequiredBundleItem.CreateList(ItemList.GetCookedItems());
					RequiredItems = rng.GetRandomValuesFromList(potentialItems, rng.NextIntWithinRange(6, 8));
					MinimumRequiredItems = rng.NextIntWithinRange(3, 4);
					Color = BundleColors.Green;
					break;
				case BundleTypes.PantryFlower:
					SetBundleName("bundle-pantry-flower");
					potentialItems = RequiredBundleItem.CreateList(ItemList.GetFlowers());
					RequiredItems = rng.GetRandomValuesFromList(potentialItems, rng.NextIntWithinRange(6, 8));
					MinimumRequiredItems = RequiredItems.Count - 2;
					Color = BundleColors.Green;
					break;
				case BundleTypes.PantrySpringCrops:
					GenerateBundleForSeasonCrops(Seasons.Spring, BundleColors.Green);
					break;
				case BundleTypes.PantrySummerCrops:
					GenerateBundleForSeasonCrops(Seasons.Summer, BundleColors.Red);
					break;
				case BundleTypes.PantryFallCrops:
					GenerateBundleForSeasonCrops(Seasons.Fall, BundleColors.Orange);
					break;
				case BundleTypes.PantryEgg:
					SetBundleName("bundle-pantry-egg");
					potentialItems = RequiredBundleItem.CreateList(
						ItemList.Items.Values.Where(x => x.IsEgg).ToList());
					RequiredItems = rng.GetRandomValuesFromList(potentialItems, 8);
					MinimumRequiredItems = rng.NextIntWithinRange(RequiredItems.Count - 3, RequiredItems.Count - 2);
					Color = BundleColors.Yellow;
					break;
				case BundleTypes.PantryRareFoods:
					SetBundleName("bundle-pantry-rare-foods");

					SeedItem starFruitSeed = (SeedItem)ObjectIndexes.StarfruitSeeds.GetItem();
					SeedItem gemBerrySeed = (SeedItem)ObjectIndexes.RareSeed.GetItem();
					RequiredItems = new List<RequiredBundleItem>
					{
						new(ObjectIndexes.AncientFruit),
						new(ObjectIndexesExtentions.GetObjectIndex(starFruitSeed.CropId)),
						new(ObjectIndexesExtentions.GetObjectIndex(gemBerrySeed.CropId)),
					};
					MinimumRequiredItems = 2;
					Color = BundleColors.Blue;
					break;
				case BundleTypes.PantryDesert:
					SetBundleName("bundle-pantry-desert");
					RequiredItems = new List<RequiredBundleItem>
					{
						new RequiredBundleItem(ObjectIndexes.IridiumOre, 5),
						rng.GetRandomValueFromList(new List<RequiredBundleItem>
						{
							new(ObjectIndexes.GoldenMask),
							new(ObjectIndexes.GoldenRelic),
						}),
						rng.GetRandomValueFromList(RequiredBundleItem.CreateList(FishItem.Get(Locations.Desert))),
						rng.GetRandomValueFromList(RequiredBundleItem.CreateList(ItemList.GetUniqueDesertForagables(), 1, 3)),
						new(ObjectIndexes.StarfruitSeeds, 5)
					};
					MinimumRequiredItems = 4;
					Color = BundleColors.Yellow;
					break;
				case BundleTypes.PantryDessert:
					SetBundleName("bundle-pantry-dessert");
					potentialItems = new List<RequiredBundleItem>
					{
						new(ObjectIndexes.CranberryCandy),
						new(ObjectIndexes.PlumPudding),
						new(ObjectIndexes.PinkCake),
						new(ObjectIndexes.PumpkinPie),
						new(ObjectIndexes.RhubarbPie),
						new(ObjectIndexes.Cookie),
						new(ObjectIndexes.IceCream),
						new(ObjectIndexes.MinersTreat),
						new(ObjectIndexes.BlueberryTart),
						new(ObjectIndexes.BlackberryCobbler),
						new(ObjectIndexes.MapleBar),
					};
					RequiredItems = rng.GetRandomValuesFromList(potentialItems, 8);
					MinimumRequiredItems = 4;
					Color = BundleColors.Cyan;
					break;
				case BundleTypes.PantryMexicanFood:
					SetBundleName("bundle-pantry-mexican-food");
					RequiredItems = new List<RequiredBundleItem>
					{
						new(ObjectIndexes.Tortilla),
						new(ObjectIndexes.Corn, 1, 5),
						new(ObjectIndexes.Tomato, 1, 5),
						new(ObjectIndexes.HotPepper, 1, 5),
						new(ObjectIndexes.FishTaco),
						new(ObjectIndexes.Rice),
						new(ObjectIndexes.Cheese),
					};
					MinimumRequiredItems = rng.NextIntWithinRange(4, 5);
					Color = BundleColors.Red;
					break;
				case BundleTypes.PantryColorBrown:
					SetBundleName("bundle-pantry-brown");
					potentialItems = RequiredBundleItem.CreateList(new List<ObjectIndexes>
					{
						ObjectIndexes.WildHorseradish,
						ObjectIndexes.CaveCarrot,
						ObjectIndexes.EarthCrystal,
						ObjectIndexes.Coconut,
						ObjectIndexes.Torch,
						ObjectIndexes.ChippedAmphora,
						ObjectIndexes.ChewingStick,
						ObjectIndexes.AncientSeed,
						ObjectIndexes.DwarvishHelm,
						ObjectIndexes.Driftwood,
						ObjectIndexes.BrownEgg,
						ObjectIndexes.LargeBrownEgg,
						ObjectIndexes.BakedFish,
						ObjectIndexes.ParsnipSoup,
						ObjectIndexes.CompleteBreakfast,
						ObjectIndexes.FriedMushroom,
						ObjectIndexes.CarpSurprise,
						ObjectIndexes.Hashbrowns,
						ObjectIndexes.Pancakes,
						ObjectIndexes.CrispyBass,
						ObjectIndexes.Bread,
						ObjectIndexes.TomKhaSoup,
						ObjectIndexes.ChocolateCake,
						ObjectIndexes.Cookie,
						ObjectIndexes.EggplantParmesan,
						ObjectIndexes.SurvivalBurger,
						ObjectIndexes.WheatFlour,
						ObjectIndexes.HardwoodFence,
						ObjectIndexes.Acorn,
						ObjectIndexes.PineCone,
						ObjectIndexes.WoodFence,
						ObjectIndexes.Gate,
						ObjectIndexes.WoodFloor,
						ObjectIndexes.Clay,
						ObjectIndexes.WeatheredFloor,
						ObjectIndexes.Wood,
						ObjectIndexes.Coffee,
						ObjectIndexes.CommonMushroom,
						ObjectIndexes.WoodPath,
						ObjectIndexes.Hazelnut,
						ObjectIndexes.Truffle,
						ObjectIndexes.Geode,
						ObjectIndexes.Mudstone,
						ObjectIndexes.AmphibianFossil,
						ObjectIndexes.PalmFossil,
						ObjectIndexes.PlumPudding,
						ObjectIndexes.RoastedHazelnuts,
						ObjectIndexes.Bruschetta,
						ObjectIndexes.QualitySprinkler,
						ObjectIndexes.PoppyseedMuffin,
						ObjectIndexes.RainTotem,
						ObjectIndexes.WarpTotemMountains,
						ObjectIndexes.CorkBobber,
						ObjectIndexes.PineTar,
						ObjectIndexes.MapleBar
					});
					RequiredItems = rng.GetRandomValuesFromList(potentialItems, 8);
					MinimumRequiredItems = rng.NextIntWithinRange(3, 6);
					Color = BundleColors.Orange;
					break;
				case BundleTypes.PantryColorGreen:
					SetBundleName("bundle-pantry-green");
					potentialItems = RequiredBundleItem.CreateList(new List<ObjectIndexes>
					{
						ObjectIndexes.Emerald,
						ObjectIndexes.Jade,
						ObjectIndexes.CactusFruit,
						ObjectIndexes.DwarfScrollII,
						ObjectIndexes.StrangeDoll2,
						ObjectIndexes.Snail,
						ObjectIndexes.Seaweed,
						ObjectIndexes.GreenAlgae,
						ObjectIndexes.Salad,
						ObjectIndexes.BeanHotpot,
						ObjectIndexes.TroutSoup,
						ObjectIndexes.IceCream,
						ObjectIndexes.Stuffing,
						ObjectIndexes.FiddleheadFern,
						ObjectIndexes.GrassStarter,
						ObjectIndexes.Pickles,
						ObjectIndexes.Juice,
						ObjectIndexes.FieldSnack,
						ObjectIndexes.DuckFeather,
						ObjectIndexes.AlgaeSoup,
						ObjectIndexes.SlimeCharmerRing,
						ObjectIndexes.BurglarsRing,
						ObjectIndexes.JadeRing,
						ObjectIndexes.EmeraldRing,
						ObjectIndexes.Alamite,
						ObjectIndexes.Geminite,
						ObjectIndexes.Jamborite,
						ObjectIndexes.Malachite,
						ObjectIndexes.PetrifiedSlime,
						ObjectIndexes.OceanStone,
						ObjectIndexes.Coleslaw,
						ObjectIndexes.FiddleheadRisotto,
						ObjectIndexes.GreenSlimeEgg,
						ObjectIndexes.WarpTotemFarm,
						ObjectIndexes.OakResin,
						ObjectIndexes.FishStew,
						ObjectIndexes.Escargot,
						ObjectIndexes.Slime,
						ObjectIndexes.Fiber,
						ObjectIndexes.OilOfGarlic,
						ObjectIndexes.WildBait
					});
					RequiredItems = rng.GetRandomValuesFromList(potentialItems, 8);
					MinimumRequiredItems = rng.NextIntWithinRange(3, 6);
					Color = BundleColors.Green;
					break;
			}
		}

		/// <summary>
		/// Generates a bundle of crops belonging to the given season
		/// </summary>
		/// <param name="season">The season</param>
		/// <param name="color">The color of the bundle</param>
		private void GenerateBundleForSeasonCrops(Seasons season, BundleColors color)
		{
			string seasonString = Globals.GetTranslation($"seasons-{season.ToString().ToLower()}");
			seasonString = $"{seasonString[0].ToString().ToUpper()}{seasonString[1..]}";

			SetBundleName("bundle-pantry-crops", new { season = seasonString });
			List<RequiredBundleItem> potentialItems = RequiredBundleItem.CreateList(ItemList.GetCrops(season));
			RequiredItems = BundleRandomizer.Rng.GetRandomValuesFromList(potentialItems, 8);
			MinimumRequiredItems = 6;
			Color = color;
		}

		/// <summary>
		/// Generates the reward for completing a crafting room bundle
		/// </summary>
		protected override void GenerateReward()
		{
            RNG rng = BundleRandomizer.Rng;

            var potentialRewards = new List<RequiredBundleItem>
			{
				new(rng.GetRandomValueFromList(ItemList.GetResources()), 999),
				new(BigCraftableIndexes.Loom),
				new(BigCraftableIndexes.MayonnaiseMachine),
				new(BigCraftableIndexes.Heater),
				new(BigCraftableIndexes.AutoGrabber),
				new(BigCraftableIndexes.SeedMaker),
				new(rng.GetRandomValueFromList(ItemList.GetCrops(true)), 25, 50),
				new(rng.GetRandomValueFromList(ItemList.GetCookedItems())),
				new(rng.GetRandomValueFromList(ItemList.GetSeeds()), 50, 100),
				new(rng.GetRandomValueFromList(ItemList.GetAnimalProducts()), 25, 50),
			};

			Reward = rng.GetRandomValueFromList(potentialRewards);
		}
	}
}
