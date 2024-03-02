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
	public class FishTankBundle : Bundle
	{
		public static List<BundleTypes> RoomBundleTypes { get; set; }

		/// <summary>
		/// Populates the bundle with the name, required items, minimum required, and color
		/// </summary>
		protected override void Populate()
		{
			BundleType = Globals.RNGGetAndRemoveRandomValueFromList(RoomBundleTypes);
			List<RequiredItem> potentialItems = new List<RequiredItem>();

			switch (BundleType)
			{
				case BundleTypes.FishTankSpringFish:
					GenerateSeasonBundle(Seasons.Spring, BundleColors.Green);
					break;
				case BundleTypes.FishTankSummerFish:
					GenerateSeasonBundle(Seasons.Summer, BundleColors.Red);
					break;
				case BundleTypes.FishTankFallFish:
					GenerateSeasonBundle(Seasons.Fall, BundleColors.Orange);
					break;
				case BundleTypes.FishTankWinterFish:
					GenerateSeasonBundle(Seasons.Winter, BundleColors.Cyan);
					break;
				case BundleTypes.FishTankOceanFood:
					SetBundleName("bundle-fishtank-ocean-food");
					potentialItems = RequiredItem.CreateList(new List<ObjectIndexes>
					{
						ObjectIndexes.CrispyBass,
						ObjectIndexes.FriedEel,
						ObjectIndexes.AlgaeSoup,
						ObjectIndexes.CrabCakes,
						ObjectIndexes.SpicyEel,
						ObjectIndexes.PaleBroth,
						ObjectIndexes.Sashimi,
						ObjectIndexes.MakiRoll,
						ObjectIndexes.TomKhaSoup,
						ObjectIndexes.BakedFish,
						ObjectIndexes.TroutSoup,
						ObjectIndexes.Chowder,
						ObjectIndexes.LobsterBisque,
						ObjectIndexes.DishOTheSea,
						ObjectIndexes.FishStew,
						ObjectIndexes.FriedCalamari,
						ObjectIndexes.SalmonDinner,
						ObjectIndexes.FishTaco
					});
					RequiredItems = Globals.RNGGetRandomValuesFromList(potentialItems, 8);
					MinimumRequiredItems = 4;
					Color = BundleColors.Yellow;
					break;
				case BundleTypes.FishTankRandom:
					SetBundleName("bundle-fishtank-random");
                    RequiredItems = RequiredItem.CreateList(Globals.RNGGetRandomValuesFromList(FishItem.Get(), 8));
                    MinimumRequiredItems = Range.GetRandomValue(3, 4);
					Color = BundleColors.Red;
					break;
				case BundleTypes.FishTankLocation:
					List<Locations> locations = new()
					{
						Locations.Town,
						Locations.Mountain,
						Locations.Desert,
						Locations.Woods,
						Locations.Forest,
						Locations.NightMarket,
						Locations.Beach
					};
					Locations location = Globals.RNGGetRandomValueFromList(locations);
					string locationString = Globals.GetTranslation($"fish-{location.ToString().ToLower()}-location");

					SetBundleName("bundle-fishtank-location", new { location = locationString });
					RequiredItems = RequiredItem.CreateList(Globals.RNGGetRandomValuesFromList(FishItem.Get(location), 8));
					MinimumRequiredItems = Math.Min(RequiredItems.Count, Range.GetRandomValue(2, 4));
					Color = BundleColors.Blue;
					break;
				case BundleTypes.FishTankRainFish:
					SetBundleName("bundle-fishtank-rain-fish");
					RequiredItems = RequiredItem.CreateList(
						Globals.RNGGetRandomValuesFromList(FishItem.Get(Weather.Rainy), 8)
					);
					MinimumRequiredItems = Math.Min(RequiredItems.Count, 4);
					Color = BundleColors.Blue;
					break;
				case BundleTypes.FishTankNightFish:
					SetBundleName("bundle-fishtank-night-fish");
					RequiredItems = RequiredItem.CreateList(
						Globals.RNGGetRandomValuesFromList(FishItem.GetNightFish(), 8)
					);
					MinimumRequiredItems = Math.Min(RequiredItems.Count, 4);
					Color = BundleColors.Purple;
					break;
				case BundleTypes.FishTankQualityFish:
					SetBundleName("bundle-fishtank-quality-fish");
					potentialItems = RequiredItem.CreateList(
						Globals.RNGGetRandomValuesFromList(FishItem.Get(), 8)
					);
					potentialItems.ForEach(x => x.MinimumQuality = ItemQualities.Gold);
					RequiredItems = potentialItems;
					MinimumRequiredItems = 4;
					Color = BundleColors.Yellow;
					break;
				case BundleTypes.FishTankBeachForagables:
					SetBundleName("bundle-fishtank-beach-foragables");
					RequiredItems = RequiredItem.CreateList(
						Globals.RNGGetRandomValuesFromList(ItemList.GetUniqueBeachForagables(), 6)
					);
					Color = BundleColors.Yellow;
					break;
				case BundleTypes.FishTankFishingTools:
					SetBundleName("bundle-fishtank-fishing-tools");
					potentialItems = new List<RequiredItem>
					{
						new RequiredItem(ObjectIndexes.Spinner, 1),
						new RequiredItem(ObjectIndexes.DressedSpinner, 1),
						new RequiredItem(ObjectIndexes.TrapBobber, 1),
						new RequiredItem(ObjectIndexes.CorkBobber, 1),
						new RequiredItem(ObjectIndexes.LeadBobber, 1),
						new RequiredItem(ObjectIndexes.TreasureHunter, 1),
						new RequiredItem(ObjectIndexes.Bait, 25, 50),
						new RequiredItem(ObjectIndexes.WildBait, 10, 20)
					};
					RequiredItems = Globals.RNGGetRandomValuesFromList(potentialItems, 4);
					Color = BundleColors.Blue;
					break;
				case BundleTypes.FishTankUnique:
					SetBundleName("bundle-fishtank-unique");

					List<Item> nightFish = FishItem.Get(Locations.NightMarket);
					List<Item> minesFish = FishItem.Get(Locations.UndergroundMine);
					List<Item> desertFish = FishItem.Get(Locations.Desert);
					List<Item> woodsFish = FishItem.Get(Locations.Woods);

					RequiredItems = new List<RequiredItem>
					{
						new RequiredItem(Globals.RNGGetRandomValueFromList(nightFish)),
						new RequiredItem(Globals.RNGGetRandomValueFromList(minesFish)),
						new RequiredItem(Globals.RNGGetRandomValueFromList(desertFish)),
						new RequiredItem(Globals.RNGGetRandomValueFromList(woodsFish))
					};
					MinimumRequiredItems = Range.GetRandomValue(RequiredItems.Count - 1, RequiredItems.Count);
					Color = BundleColors.Cyan;
					break;
				case BundleTypes.FishTankColorBlue:
					SetBundleName("bundle-fishtank-blue");

					potentialItems = RequiredItem.CreateList(new List<ObjectIndexes>
					{
						ObjectIndexes.Aquamarine,
						ObjectIndexes.Diamond,
						ObjectIndexes.FrozenTear,
						ObjectIndexes.DwarfScrollIII,
						ObjectIndexes.ElvishJewelry,
						ObjectIndexes.GlassShards,
						ObjectIndexes.Clam,
						ObjectIndexes.Periwinkle,
						ObjectIndexes.JojaCola,
						ObjectIndexes.BrokenGlasses,
						ObjectIndexes.BrokenCD,
						ObjectIndexes.BlueberryTart,
						ObjectIndexes.Sugar,
						ObjectIndexes.BasicRetainingSoil,
						ObjectIndexes.QualityRetainingSoil,
						ObjectIndexes.RainbowShell,
						ObjectIndexes.BlueSlimeEgg,
						ObjectIndexes.CrystalFruit,
						ObjectIndexes.SturdyRing,
						ObjectIndexes.AquamarineRing,
						ObjectIndexes.FrozenGeode,
						ObjectIndexes.Opal,
						ObjectIndexes.Aerinite,
						ObjectIndexes.Kyanite,
						ObjectIndexes.GhostCrystal,
						ObjectIndexes.Celestine,
						ObjectIndexes.Soapstone,
						ObjectIndexes.Slate,
						ObjectIndexes.Spinner,
						ObjectIndexes.WarpTotemBeach,
						ObjectIndexes.Battery
					});
					RequiredItems = Globals.RNGGetRandomValuesFromList(potentialItems, 8);
					MinimumRequiredItems = Range.GetRandomValue(3, 6);
					Color = BundleColors.Blue;
					break;
				case BundleTypes.FishTankColorPurple:
					SetBundleName("bundle-fishtank-purple");
					potentialItems = RequiredItem.CreateList(new List<ObjectIndexes>
					{
						ObjectIndexes.Amethyst,
						ObjectIndexes.AncientDrum,
						ObjectIndexes.PumpkinSoup,
						ObjectIndexes.RootsPlatter,
						ObjectIndexes.IridiumBar,
						ObjectIndexes.Wine,
						ObjectIndexes.IridiumOre,
						ObjectIndexes.SeaUrchin,
						ObjectIndexes.SweetPea,
						ObjectIndexes.WildPlum,
						ObjectIndexes.Blackberry,
						ObjectIndexes.Crocus,
						ObjectIndexes.Vinegar,
						ObjectIndexes.PurpleMushroom,
						ObjectIndexes.SpeedGro,
						ObjectIndexes.DeluxeSpeedGro,
						ObjectIndexes.IridiumBand,
						ObjectIndexes.AmethystRing,
						ObjectIndexes.FireOpal,
						ObjectIndexes.Fluorapatite,
						ObjectIndexes.Obsidian,
						ObjectIndexes.FairyStone,
						ObjectIndexes.BlackberryCobbler,
						ObjectIndexes.IridiumSprinkler,
						ObjectIndexes.DressedSpinner
					});
					RequiredItems = Globals.RNGGetRandomValuesFromList(potentialItems, 8);
					MinimumRequiredItems = Range.GetRandomValue(3, 6);
					Color = BundleColors.Purple;
					break;
			}
		}

		/// <summary>
		/// Generates the bundle for the given season
		/// </summary>
		/// <param name="season">The season</param>
		/// <param name="color">The color to use</param>
		private void GenerateSeasonBundle(Seasons season, BundleColors color)
		{
			string seasonString = Globals.GetTranslation($"seasons-{season.ToString().ToLower()}");
			seasonString = $"{seasonString[0].ToString().ToUpper()}{seasonString.Substring(1)}";

			SetBundleName("bundle-fishtank-seasonal", new { season = seasonString });
			List<RequiredItem> potentialItems = RequiredItem.CreateList(FishItem.Get(season));
			RequiredItems = Globals.RNGGetRandomValuesFromList(potentialItems, 8);
			MinimumRequiredItems = Math.Min(Range.GetRandomValue(6, 8), RequiredItems.Count);
			Color = color;
		}

		/// <summary>
		/// Generates the reward for the bundle
		/// </summary>
		protected override void GenerateReward()
		{
			var tackles = new List<RequiredItem>
			{
				new(ObjectIndexes.Spinner, 1),
				new(ObjectIndexes.DressedSpinner),
				new(ObjectIndexes.TrapBobber),
				new(ObjectIndexes.CorkBobber),
				new(ObjectIndexes.LeadBobber),
				new(ObjectIndexes.TreasureHunter)
			};

			var potentialRewards = new List<RequiredItem>
			{
				new(BigCraftableIndexes.RecyclingMachine),
				new(ObjectIndexes.Bait, 500),
				new(ObjectIndexes.WildBait, 500),
				Globals.RNGGetRandomValueFromList(tackles),
				Globals.RNGGetRandomValueFromList(RequiredItem.CreateList(FishItem.Get(), 25, 50)),
				Globals.RNGGetRandomValueFromList(RequiredItem.CreateList(ItemList.GetUniqueBeachForagables(), 25, 50)),
			};

			Reward = Globals.RNGGetRandomValueFromList(potentialRewards);
		}
	}
}
