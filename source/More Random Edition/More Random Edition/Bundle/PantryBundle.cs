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
					Name = Globals.GetTranslation("bundle-pantry-animal");
					potentialItems = RequiredItem.CreateList(ItemList.GetAnimalProducts());
					potentialItems.Add(new RequiredItem((int)ObjectIndexes.Hay, 25, 50));
					RequiredItems = Globals.RNGGetRandomValuesFromList(potentialItems, Range.GetRandomValue(6, 8));
					MinimumRequiredItems = Range.GetRandomValue(RequiredItems.Count - 2, RequiredItems.Count);
					Color = BundleColors.Orange;
					break;
				case BundleTypes.PantryQualityCrops:
					Name = Globals.GetTranslation("bundle-pantry-quality-crops");
					potentialItems = RequiredItem.CreateList(ItemList.GetCrops());
					potentialItems.ForEach(x => x.MinimumQuality = ItemQualities.Gold);
					RequiredItems = Globals.RNGGetRandomValuesFromList(potentialItems, 8);
					MinimumRequiredItems = Range.GetRandomValue(4, 6);
					Color = BundleColors.Green;
					break;
				case BundleTypes.PantryQualityForagables:
					Name = Globals.GetTranslation("bundle-pantry-quality-foragables");
					potentialItems = RequiredItem.CreateList(ItemList.GetForagables());
					potentialItems.ForEach(x => x.MinimumQuality = ItemQualities.Gold);
					RequiredItems = Globals.RNGGetRandomValuesFromList(potentialItems, 8);
					MinimumRequiredItems = Range.GetRandomValue(4, 6);
					Color = BundleColors.Green;
					break;
				case BundleTypes.PantryCooked:
					Name = Globals.GetTranslation("bundle-pantry-cooked");
					potentialItems = RequiredItem.CreateList(ItemList.GetCookeditems());
					RequiredItems = Globals.RNGGetRandomValuesFromList(potentialItems, Range.GetRandomValue(6, 8));
					MinimumRequiredItems = Range.GetRandomValue(3, 4);
					Color = BundleColors.Green;
					break;
				case BundleTypes.PantryFlower:
					Name = Globals.GetTranslation("bundle-pantry-flower");
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
					Name = Globals.GetTranslation("bundle-pantry-egg");
					potentialItems = RequiredItem.CreateList(
						ItemList.Items.Values.Where(x => x.Name.Contains("Egg") && x.Id > -4).ToList());
					RequiredItems = Globals.RNGGetRandomValuesFromList(potentialItems, 8);
					MinimumRequiredItems = Range.GetRandomValue(RequiredItems.Count - 3, RequiredItems.Count - 2);
					Color = BundleColors.Yellow;
					break;
				case BundleTypes.PantryRareFoods:
					Name = Globals.GetTranslation("bundle-pantry-rare-foods");
					RequiredItems = new List<RequiredItem>
					{
						new RequiredItem((int)ObjectIndexes.AncientFruit),
						new RequiredItem((int)ObjectIndexes.Starfruit),
						new RequiredItem((int)ObjectIndexes.SweetGemBerry)
					};
					Color = BundleColors.Blue;
					break;
				case BundleTypes.PantryDesert:
					Name = Globals.GetTranslation("bundle-pantry-desert");
					RequiredItems = new List<RequiredItem>
					{
						new RequiredItem((int)ObjectIndexes.IridiumOre, 5),
						Globals.RNGGetRandomValueFromList(new List<RequiredItem>
						{
							new RequiredItem((int)ObjectIndexes.GoldenMask),
							new RequiredItem((int)ObjectIndexes.GoldenRelic),
						}),
						Globals.RNGGetRandomValueFromList(RequiredItem.CreateList(FishItem.Get(Locations.Desert))),
						Globals.RNGGetRandomValueFromList(RequiredItem.CreateList(ItemList.GetUniqueDesertForagables(), 1, 3)),
						new RequiredItem((int)ObjectIndexes.StarfruitSeeds, 5)
					};
					MinimumRequiredItems = 4;
					Color = BundleColors.Yellow;
					break;
				case BundleTypes.PantryDessert:
					Name = Globals.GetTranslation("bundle-pantry-dessert");
					potentialItems = new List<RequiredItem>
					{
						new RequiredItem((int)ObjectIndexes.CranberryCandy),
						new RequiredItem((int)ObjectIndexes.PlumPudding),
						new RequiredItem((int)ObjectIndexes.PinkCake),
						new RequiredItem((int)ObjectIndexes.PumpkinPie),
						new RequiredItem((int)ObjectIndexes.RhubarbPie),
						new RequiredItem((int)ObjectIndexes.Cookie),
						new RequiredItem((int)ObjectIndexes.IceCream),
						new RequiredItem((int)ObjectIndexes.MinersTreat),
						new RequiredItem((int)ObjectIndexes.BlueberryTart),
						new RequiredItem((int)ObjectIndexes.BlackberryCobbler),
						new RequiredItem((int)ObjectIndexes.MapleBar),
					};
					RequiredItems = Globals.RNGGetRandomValuesFromList(potentialItems, 8);
					MinimumRequiredItems = 4;
					Color = BundleColors.Cyan;
					break;
				case BundleTypes.PantryMexicanFood:
					Name = Globals.GetTranslation("bundle-pantry-mexican-food");
					RequiredItems = new List<RequiredItem>
					{
						new RequiredItem((int)ObjectIndexes.Tortilla),
						new RequiredItem((int)ObjectIndexes.Corn, 1, 5),
						new RequiredItem((int)ObjectIndexes.Tomato, 1, 5),
						new RequiredItem((int)ObjectIndexes.HotPepper, 1, 5),
						new RequiredItem((int)ObjectIndexes.FishTaco),
						new RequiredItem((int)ObjectIndexes.Rice),
						new RequiredItem((int)ObjectIndexes.Cheese),
					};
					MinimumRequiredItems = Range.GetRandomValue(4, 5);
					Color = BundleColors.Red;
					break;
				case BundleTypes.PantryColorBrown:
					Name = Globals.GetTranslation("bundle-pantry-brown");
					potentialItems = RequiredItem.CreateList(new List<int>
					{
						(int)ObjectIndexes.WildHorseradish,
						(int)ObjectIndexes.CaveCarrot,
						(int)ObjectIndexes.EarthCrystal,
						(int)ObjectIndexes.Coconut,
						(int)ObjectIndexes.Torch,
						(int)ObjectIndexes.ChippedAmphora,
						(int)ObjectIndexes.ChewingStick,
						(int)ObjectIndexes.AncientSeed,
						(int)ObjectIndexes.DwarvishHelm,
						(int)ObjectIndexes.SmallmouthBass,
						(int)ObjectIndexes.Walleye,
						(int)ObjectIndexes.Pike,
						(int)ObjectIndexes.Stonefish,
						(int)ObjectIndexes.Driftwood,
						(int)ObjectIndexes.BrownEgg,
						(int)ObjectIndexes.LargeBrownEgg,
						(int)ObjectIndexes.BakedFish,
						(int)ObjectIndexes.ParsnipSoup,
						(int)ObjectIndexes.CompleteBreakfast,
						(int)ObjectIndexes.FriedMushroom,
						(int)ObjectIndexes.CarpSurprise,
						(int)ObjectIndexes.Hashbrowns,
						(int)ObjectIndexes.Pancakes,
						(int)ObjectIndexes.CrispyBass,
						(int)ObjectIndexes.Bread,
						(int)ObjectIndexes.TomKhaSoup,
						(int)ObjectIndexes.ChocolateCake,
						(int)ObjectIndexes.Cookie,
						(int)ObjectIndexes.EggplantParmesan,
						(int)ObjectIndexes.SurvivalBurger,
						(int)ObjectIndexes.WheatFlour,
						(int)ObjectIndexes.HardwoodFence,
						(int)ObjectIndexes.Acorn,
						(int)ObjectIndexes.PineCone,
						(int)ObjectIndexes.WoodFence,
						(int)ObjectIndexes.Gate,
						(int)ObjectIndexes.WoodFloor,
						(int)ObjectIndexes.Clay,
						(int)ObjectIndexes.WeatheredFloor,
						(int)ObjectIndexes.Wood,
						(int)ObjectIndexes.Coffee,
						(int)ObjectIndexes.CommonMushroom,
						(int)ObjectIndexes.WoodPath,
						(int)ObjectIndexes.Hazelnut,
						(int)ObjectIndexes.Truffle,
						(int)ObjectIndexes.Geode,
						(int)ObjectIndexes.Mudstone,
						(int)ObjectIndexes.AmphibianFossil,
						(int)ObjectIndexes.PalmFossil,
						(int)ObjectIndexes.PlumPudding,
						(int)ObjectIndexes.RoastedHazelnuts,
						(int)ObjectIndexes.Bruschetta,
						(int)ObjectIndexes.QualitySprinkler,
						(int)ObjectIndexes.PoppyseedMuffin,
						(int)ObjectIndexes.RainTotem,
						(int)ObjectIndexes.WarpTotemMountains,
						(int)ObjectIndexes.CorkBobber,
						(int)ObjectIndexes.PineTar,
						(int)ObjectIndexes.MapleBar
					});
					RequiredItems = Globals.RNGGetRandomValuesFromList(potentialItems, 8);
					MinimumRequiredItems = Range.GetRandomValue(3, 6);
					Color = BundleColors.Orange;
					break;
				case BundleTypes.PantryColorGreen:
					Name = Globals.GetTranslation("bundle-pantry-green");
					potentialItems = RequiredItem.CreateList(new List<int>
					{
						(int)ObjectIndexes.Emerald,
						(int)ObjectIndexes.Jade,
						(int)ObjectIndexes.CactusFruit,
						(int)ObjectIndexes.DwarfScrollII,
						(int)ObjectIndexes.StrangeDoll2,
						(int)ObjectIndexes.LargemouthBass,
						(int)ObjectIndexes.Shad,
						(int)ObjectIndexes.Slimejack,
						(int)ObjectIndexes.Legend,
						(int)ObjectIndexes.MutantCarp,
						(int)ObjectIndexes.Snail,
						(int)ObjectIndexes.Seaweed,
						(int)ObjectIndexes.GreenAlgae,
						(int)ObjectIndexes.Salad,
						(int)ObjectIndexes.BeanHotpot,
						(int)ObjectIndexes.TroutSoup,
						(int)ObjectIndexes.IceCream,
						(int)ObjectIndexes.Stuffing,
						(int)ObjectIndexes.FiddleheadFern,
						(int)ObjectIndexes.GrassStarter,
						(int)ObjectIndexes.Pickles,
						(int)ObjectIndexes.Juice,
						(int)ObjectIndexes.FieldSnack,
						(int)ObjectIndexes.DuckFeather,
						(int)ObjectIndexes.AlgaeSoup,
						(int)ObjectIndexes.SlimeCharmerRing,
						(int)ObjectIndexes.BurglarsRing,
						(int)ObjectIndexes.JadeRing,
						(int)ObjectIndexes.EmeraldRing,
						(int)ObjectIndexes.Alamite,
						(int)ObjectIndexes.Geminite,
						(int)ObjectIndexes.Jamborite,
						(int)ObjectIndexes.Malachite,
						(int)ObjectIndexes.PetrifiedSlime,
						(int)ObjectIndexes.OceanStone,
						(int)ObjectIndexes.Coleslaw,
						(int)ObjectIndexes.FiddleheadRisotto,
						(int)ObjectIndexes.GreenSlimeEgg,
						(int)ObjectIndexes.WarpTotemFarm,
						(int)ObjectIndexes.OakResin,
						(int)ObjectIndexes.FishStew,
						(int)ObjectIndexes.Escargot,
						(int)ObjectIndexes.Slime,
						(int)ObjectIndexes.Fiber,
						(int)ObjectIndexes.OilOfGarlic,
						(int)ObjectIndexes.WildBait
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

			Name = Globals.GetTranslation("bundle-pantry-crops", new { season = seasonString });
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
				new RequiredItem(Globals.RNGGetRandomValueFromList(ItemList.GetResources()), 999),
				new RequiredItem((int)ObjectIndexes.Loom),
				new RequiredItem((int)ObjectIndexes.MayonnaiseMachine),
				new RequiredItem((int)ObjectIndexes.Heater),
				new RequiredItem((int)ObjectIndexes.AutoGrabber),
				new RequiredItem((int)ObjectIndexes.SeedMaker),
				new RequiredItem(Globals.RNGGetRandomValueFromList(ItemList.GetCrops(true)), 25, 50),
				new RequiredItem(Globals.RNGGetRandomValueFromList(ItemList.GetCookeditems())),
				new RequiredItem(Globals.RNGGetRandomValueFromList(ItemList.GetSeeds()), 50, 100),
				new RequiredItem(Globals.RNGGetRandomValueFromList(ItemList.GetAnimalProducts()), 25, 50),
			};

			Reward = Globals.RNGGetRandomValueFromList(potentialRewards);
		}
	}
}
