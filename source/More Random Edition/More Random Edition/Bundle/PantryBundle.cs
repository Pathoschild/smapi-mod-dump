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
			BundleType = Globals.RNGGetAndRemoveRandomValueFromList(RoomBundleTypes);
			List<RequiredItem> potentialItems = new List<RequiredItem>();

			switch (BundleType)
			{
				case BundleTypes.PantryAnimal:
					SetBundleName("bundle-pantry-animal");
					potentialItems = RequiredItem.CreateList(ItemList.GetAnimalProducts());
					potentialItems.Add(new RequiredItem(ObjectIndexes.Hay, 25, 50));
					RequiredItems = Globals.RNGGetRandomValuesFromList(potentialItems, Range.GetRandomValue(6, 8));
					MinimumRequiredItems = Range.GetRandomValue(RequiredItems.Count - 2, RequiredItems.Count);
					Color = BundleColors.Orange;
					break;
				case BundleTypes.PantryQualityCrops:
					SetBundleName("bundle-pantry-quality-crops");
					potentialItems = RequiredItem.CreateList(ItemList.GetCrops());
					potentialItems.ForEach(x => x.MinimumQuality = ItemQualities.Gold);
					RequiredItems = Globals.RNGGetRandomValuesFromList(potentialItems, 8);
					MinimumRequiredItems = Range.GetRandomValue(4, 6);
					Color = BundleColors.Green;
					break;
				case BundleTypes.PantryQualityForagables:
					SetBundleName("bundle-pantry-quality-foragables");
					potentialItems = RequiredItem.CreateList(ItemList.GetForagables());
					potentialItems.ForEach(x => x.MinimumQuality = ItemQualities.Gold);
					RequiredItems = Globals.RNGGetRandomValuesFromList(potentialItems, 8);
					MinimumRequiredItems = Range.GetRandomValue(4, 6);
					Color = BundleColors.Green;
					break;
				case BundleTypes.PantryCooked:
					SetBundleName("bundle-pantry-cooked");
					potentialItems = RequiredItem.CreateList(ItemList.GetCookedItems());
					RequiredItems = Globals.RNGGetRandomValuesFromList(potentialItems, Range.GetRandomValue(6, 8));
					MinimumRequiredItems = Range.GetRandomValue(3, 4);
					Color = BundleColors.Green;
					break;
				case BundleTypes.PantryFlower:
					SetBundleName("bundle-pantry-flower");
					potentialItems = RequiredItem.CreateList(ItemList.GetFlowers());
					RequiredItems = Globals.RNGGetRandomValuesFromList(potentialItems, Range.GetRandomValue(6, 8));
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
					potentialItems = RequiredItem.CreateList(
						ItemList.Items.Values.Where(x => x.Name.Contains("Egg") && x.Id > -4).ToList());
					RequiredItems = Globals.RNGGetRandomValuesFromList(potentialItems, 8);
					MinimumRequiredItems = Range.GetRandomValue(RequiredItems.Count - 3, RequiredItems.Count - 2);
					Color = BundleColors.Yellow;
					break;
				case BundleTypes.PantryRareFoods:
					SetBundleName("bundle-pantry-rare-foods");

					SeedItem starFruitSeed = (SeedItem)ItemList.Items[ObjectIndexes.StarfruitSeeds];
					SeedItem gemBerrySeed = (SeedItem)ItemList.Items[ObjectIndexes.RareSeed];
					RequiredItems = new List<RequiredItem>
					{
						new(ObjectIndexes.AncientFruit),
						new((ObjectIndexes)starFruitSeed.CropGrowthInfo.CropId),
						new((ObjectIndexes)gemBerrySeed.CropGrowthInfo.CropId),
					};
					MinimumRequiredItems = 2;
					Color = BundleColors.Blue;
					break;
				case BundleTypes.PantryDesert:
					SetBundleName("bundle-pantry-desert");
					RequiredItems = new List<RequiredItem>
					{
						new RequiredItem(ObjectIndexes.IridiumOre, 5),
						Globals.RNGGetRandomValueFromList(new List<RequiredItem>
						{
							new RequiredItem(ObjectIndexes.GoldenMask),
							new RequiredItem(ObjectIndexes.GoldenRelic),
						}),
						Globals.RNGGetRandomValueFromList(RequiredItem.CreateList(FishItem.Get(Locations.Desert))),
						Globals.RNGGetRandomValueFromList(RequiredItem.CreateList(ItemList.GetUniqueDesertForagables(), 1, 3)),
						new RequiredItem(ObjectIndexes.StarfruitSeeds, 5)
					};
					MinimumRequiredItems = 4;
					Color = BundleColors.Yellow;
					break;
				case BundleTypes.PantryDessert:
					SetBundleName("bundle-pantry-dessert");
					potentialItems = new List<RequiredItem>
					{
						new RequiredItem(ObjectIndexes.CranberryCandy),
						new RequiredItem(ObjectIndexes.PlumPudding),
						new RequiredItem(ObjectIndexes.PinkCake),
						new RequiredItem(ObjectIndexes.PumpkinPie),
						new RequiredItem(ObjectIndexes.RhubarbPie),
						new RequiredItem(ObjectIndexes.Cookie),
						new RequiredItem(ObjectIndexes.IceCream),
						new RequiredItem(ObjectIndexes.MinersTreat),
						new RequiredItem(ObjectIndexes.BlueberryTart),
						new RequiredItem(ObjectIndexes.BlackberryCobbler),
						new RequiredItem(ObjectIndexes.MapleBar),
					};
					RequiredItems = Globals.RNGGetRandomValuesFromList(potentialItems, 8);
					MinimumRequiredItems = 4;
					Color = BundleColors.Cyan;
					break;
				case BundleTypes.PantryMexicanFood:
					SetBundleName("bundle-pantry-mexican-food");
					RequiredItems = new List<RequiredItem>
					{
						new RequiredItem(ObjectIndexes.Tortilla),
						new RequiredItem(ObjectIndexes.Corn, 1, 5),
						new RequiredItem(ObjectIndexes.Tomato, 1, 5),
						new RequiredItem(ObjectIndexes.HotPepper, 1, 5),
						new RequiredItem(ObjectIndexes.FishTaco),
						new RequiredItem(ObjectIndexes.Rice),
						new RequiredItem(ObjectIndexes.Cheese),
					};
					MinimumRequiredItems = Range.GetRandomValue(4, 5);
					Color = BundleColors.Red;
					break;
				case BundleTypes.PantryColorBrown:
					SetBundleName("bundle-pantry-brown");
					potentialItems = RequiredItem.CreateList(new List<ObjectIndexes>
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
					RequiredItems = Globals.RNGGetRandomValuesFromList(potentialItems, 8);
					MinimumRequiredItems = Range.GetRandomValue(3, 6);
					Color = BundleColors.Orange;
					break;
				case BundleTypes.PantryColorGreen:
					SetBundleName("bundle-pantry-green");
					potentialItems = RequiredItem.CreateList(new List<ObjectIndexes>
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
					RequiredItems = Globals.RNGGetRandomValuesFromList(potentialItems, 8);
					MinimumRequiredItems = Range.GetRandomValue(3, 6);
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
			seasonString = $"{seasonString[0].ToString().ToUpper()}{seasonString.Substring(1)}";

			SetBundleName("bundle-pantry-crops", new { season = seasonString });
			List<RequiredItem> potentialItems = RequiredItem.CreateList(ItemList.GetCrops(season));
			RequiredItems = Globals.RNGGetRandomValuesFromList(potentialItems, 8);
			MinimumRequiredItems = 6;
			Color = color;
		}

		/// <summary>
		/// Generates the reward for completing a crafting room bundle
		/// </summary>
		protected override void GenerateReward()
		{
			var potentialRewards = new List<RequiredItem>
			{
				new(Globals.RNGGetRandomValueFromList(ItemList.GetResources()), 999),
				new(BigCraftableIndexes.Loom),
				new(BigCraftableIndexes.MayonnaiseMachine),
				new(BigCraftableIndexes.Heater),
				new(BigCraftableIndexes.AutoGrabber),
				new(BigCraftableIndexes.SeedMaker),
				new(Globals.RNGGetRandomValueFromList(ItemList.GetCrops(true)), 25, 50),
				new(Globals.RNGGetRandomValueFromList(ItemList.GetCookedItems())),
				new(Globals.RNGGetRandomValueFromList(ItemList.GetSeeds()), 50, 100),
				new(Globals.RNGGetRandomValueFromList(ItemList.GetAnimalProducts()), 25, 50),
			};

			Reward = Globals.RNGGetRandomValueFromList(potentialRewards);
		}
	}
}
