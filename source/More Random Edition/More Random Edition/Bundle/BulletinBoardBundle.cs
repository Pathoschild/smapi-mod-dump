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
	public class BulletinBoardBundle : Bundle
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
				case BundleTypes.BulletinNews:
					SetBundleName("bundle-bulletin-news");
					potentialItems = new List<RequiredItem>()
					{
						new RequiredItem(ObjectIndexes.SoggyNewspaper),
						new RequiredItem(ObjectIndexes.SoggyNewspaper),
						new RequiredItem(ObjectIndexes.SoggyNewspaper),
						new RequiredItem(ObjectIndexes.SoggyNewspaper),
						new RequiredItem(ObjectIndexes.SoggyNewspaper),
						new RequiredItem(ObjectIndexes.SoggyNewspaper),
						new RequiredItem(ObjectIndexes.SoggyNewspaper),
						new RequiredItem(ObjectIndexes.SoggyNewspaper),
					};
					RequiredItems = Globals.RNGGetRandomValuesFromList(potentialItems, Range.GetRandomValue(1, 8));
					Color = BundleColors.Orange;
					break;
				case BundleTypes.BulletinCleanup:
					SetBundleName("bundle-bulletin-cleanup");
					RequiredItems = RequiredItem.CreateList(ItemList.GetTrash(), 1, 5);
					Color = BundleColors.Green;
					break;
				case BundleTypes.BulletinHated:
					SetBundleName("bundle-bulletin-hated"); 
					potentialItems = RequiredItem.CreateList(
						PreferenceRandomizer.GetUniversalPreferences(UniversalPreferencesIndexes.Hated));
					RequiredItems = Globals.RNGGetRandomValuesFromList(potentialItems, 8, forceNumberOfValuesRNGCalls: true);
					MinimumRequiredItems = Range.GetRandomValue(4, 6);
					Color = BundleColors.Red;
					break;
				case BundleTypes.BulletinLoved:
					SetBundleName("bundle-bulletin-loved");
					potentialItems = RequiredItems = RequiredItem.CreateList(
						PreferenceRandomizer.GetUniversalPreferences(UniversalPreferencesIndexes.Loved));
                    RequiredItems = Globals.RNGGetRandomValuesFromList(potentialItems, 8, forceNumberOfValuesRNGCalls: true);
                    MinimumRequiredItems = Range.GetRandomValue(2, 6);
                    Color = BundleColors.Red;
					break;
				case BundleTypes.BulletinAbigail:
					SetBundleName("Abigail-name");
					potentialItems = RequiredItem.CreateList(
                        PreferenceRandomizer.GetLovedItems(GiftableNPCIndexes.Abigail));
                    RequiredItems = Globals.RNGGetRandomValuesFromList(potentialItems, 8, forceNumberOfValuesRNGCalls: true);
					MinimumRequiredItems = Math.Min(Math.Max(RequiredItems.Count - 2, 3), RequiredItems.Count);
					Color = BundleColors.Purple;
					break;
				case BundleTypes.BulletinAlex:
					SetBundleName("Alex-name");
					potentialItems = RequiredItem.CreateList(
                        PreferenceRandomizer.GetLovedItems(GiftableNPCIndexes.Alex));
                    RequiredItems = Globals.RNGGetRandomValuesFromList(potentialItems, 8, forceNumberOfValuesRNGCalls: true);
					MinimumRequiredItems = Math.Min(Math.Max(RequiredItems.Count - 2, 3), RequiredItems.Count);
					Color = BundleColors.Green;
					break;
				case BundleTypes.BulletinCaroline:
					SetBundleName("Caroline-name");
					potentialItems = RequiredItem.CreateList(
                        PreferenceRandomizer.GetLovedItems(GiftableNPCIndexes.Caroline));
                    RequiredItems = Globals.RNGGetRandomValuesFromList(potentialItems, 8, forceNumberOfValuesRNGCalls: true);
					MinimumRequiredItems = Math.Min(Math.Max(RequiredItems.Count - 2, 3), RequiredItems.Count);
					Color = BundleColors.Blue;
					break;
				case BundleTypes.BulletinClint:
					SetBundleName("Clint-name");
					potentialItems = RequiredItem.CreateList(
                        PreferenceRandomizer.GetLovedItems(GiftableNPCIndexes.Clint));
                    RequiredItems = Globals.RNGGetRandomValuesFromList(potentialItems, 8, forceNumberOfValuesRNGCalls: true);
					MinimumRequiredItems = Math.Min(Math.Max(RequiredItems.Count - 2, 3), RequiredItems.Count);
					Color = BundleColors.Orange;
					break;
				case BundleTypes.BulletinDwarf:
					SetBundleName("Dwarf-name");
					potentialItems = RequiredItem.CreateList(
                        PreferenceRandomizer.GetLovedItems(GiftableNPCIndexes.Dwarf));
                    RequiredItems = Globals.RNGGetRandomValuesFromList(potentialItems, 8, forceNumberOfValuesRNGCalls: true);
					MinimumRequiredItems = Math.Min(Math.Max(RequiredItems.Count - 2, 3), RequiredItems.Count);
					Color = BundleColors.Orange;
					break;
				case BundleTypes.BulletinDemetrius:
					SetBundleName("Demetrius-name");
					potentialItems = RequiredItem.CreateList(
                        PreferenceRandomizer.GetLovedItems(GiftableNPCIndexes.Demetrius));
                    RequiredItems = Globals.RNGGetRandomValuesFromList(potentialItems, 8, forceNumberOfValuesRNGCalls: true);
					MinimumRequiredItems = Math.Min(Math.Max(RequiredItems.Count - 2, 3), RequiredItems.Count);
					Color = BundleColors.Blue;
					break;
				case BundleTypes.BulletinElliott:
					SetBundleName("Elliott-name");
					potentialItems = RequiredItem.CreateList(
                        PreferenceRandomizer.GetLovedItems(GiftableNPCIndexes.Elliott));
                    RequiredItems = Globals.RNGGetRandomValuesFromList(potentialItems, 8, forceNumberOfValuesRNGCalls: true);
					MinimumRequiredItems = Math.Min(Math.Max(RequiredItems.Count - 2, 3), RequiredItems.Count);
					Color = BundleColors.Red;
					break;
				case BundleTypes.BulletinEmily:
					SetBundleName("Emily-name");
					potentialItems = RequiredItem.CreateList(
                        PreferenceRandomizer.GetLovedItems(GiftableNPCIndexes.Emily));
                    RequiredItems = Globals.RNGGetRandomValuesFromList(potentialItems, 8, forceNumberOfValuesRNGCalls: true);
					MinimumRequiredItems = Math.Min(Math.Max(RequiredItems.Count - 2, 3), RequiredItems.Count);
					Color = BundleColors.Red;
					break;
				case BundleTypes.BulletinEvelyn:
					SetBundleName("Evelyn-name");
					potentialItems = RequiredItem.CreateList(
                        PreferenceRandomizer.GetLovedItems(GiftableNPCIndexes.Evelyn));
                    RequiredItems = Globals.RNGGetRandomValuesFromList(potentialItems, 8, forceNumberOfValuesRNGCalls: true);
					MinimumRequiredItems = Math.Min(Math.Max(RequiredItems.Count - 2, 3), RequiredItems.Count);
					Color = BundleColors.Red;
					break;
				case BundleTypes.BulletinGeorge:
					SetBundleName("George-name");
					potentialItems = RequiredItem.CreateList(
                        PreferenceRandomizer.GetLovedItems(GiftableNPCIndexes.George));
                    RequiredItems = Globals.RNGGetRandomValuesFromList(potentialItems, 8, forceNumberOfValuesRNGCalls: true);
					MinimumRequiredItems = Math.Min(Math.Max(RequiredItems.Count - 2, 3), RequiredItems.Count);
					Color = BundleColors.Green;
					break;
				case BundleTypes.BulletinGus:
					SetBundleName("Gus-name");
					potentialItems = RequiredItem.CreateList(
                        PreferenceRandomizer.GetLovedItems(GiftableNPCIndexes.Gus));
                    RequiredItems = Globals.RNGGetRandomValuesFromList(potentialItems, 8, forceNumberOfValuesRNGCalls: true);
					MinimumRequiredItems = Math.Min(Math.Max(RequiredItems.Count - 2, 3), RequiredItems.Count);
					Color = BundleColors.Orange;
					break;
				case BundleTypes.BulletinHaley:
					SetBundleName("Haley-name");
					potentialItems = RequiredItem.CreateList(
                        PreferenceRandomizer.GetLovedItems(GiftableNPCIndexes.Haley));
                    RequiredItems = Globals.RNGGetRandomValuesFromList(potentialItems, 8, forceNumberOfValuesRNGCalls: true);
					MinimumRequiredItems = Math.Min(Math.Max(RequiredItems.Count - 2, 3), RequiredItems.Count);
					Color = BundleColors.Blue;
					break;
				case BundleTypes.BulletinHarvey:
					SetBundleName("Harvey-name");
					potentialItems = RequiredItem.CreateList(
                        PreferenceRandomizer.GetLovedItems(GiftableNPCIndexes.Harvey));
                    RequiredItems = Globals.RNGGetRandomValuesFromList(potentialItems, 8, forceNumberOfValuesRNGCalls: true);
					MinimumRequiredItems = Math.Min(Math.Max(RequiredItems.Count - 2, 3), RequiredItems.Count);
					Color = BundleColors.Green;
					break;
				case BundleTypes.BulletinJas:
					SetBundleName("Jas-name");
					potentialItems = RequiredItem.CreateList(
                        PreferenceRandomizer.GetLovedItems(GiftableNPCIndexes.Jas));
                    RequiredItems = Globals.RNGGetRandomValuesFromList(potentialItems, 8, forceNumberOfValuesRNGCalls: true);
					MinimumRequiredItems = Math.Min(Math.Max(RequiredItems.Count - 2, 3), RequiredItems.Count);
					Color = BundleColors.Purple;
					break;
				case BundleTypes.BulletinJodi:
					SetBundleName("Jodi-name");
					potentialItems = RequiredItem.CreateList(
                        PreferenceRandomizer.GetLovedItems(GiftableNPCIndexes.Jodi));
                    RequiredItems = Globals.RNGGetRandomValuesFromList(potentialItems, 8, forceNumberOfValuesRNGCalls: true);
					MinimumRequiredItems = Math.Min(Math.Max(RequiredItems.Count - 2, 3), RequiredItems.Count);
					Color = BundleColors.Purple;
					break;
				case BundleTypes.BulletinKent:
					SetBundleName("Kent-name");
					potentialItems = RequiredItem.CreateList(
                        PreferenceRandomizer.GetLovedItems(GiftableNPCIndexes.Kent));
                    RequiredItems = Globals.RNGGetRandomValuesFromList(potentialItems, 8, forceNumberOfValuesRNGCalls: true);
					MinimumRequiredItems = Math.Min(Math.Max(RequiredItems.Count - 2, 3), RequiredItems.Count);
					Color = BundleColors.Green;
					break;
				case BundleTypes.BulletinKrobus:
					SetBundleName("Krobus-name");
					potentialItems = RequiredItem.CreateList(
						PreferenceRandomizer.GetLovedItems(GiftableNPCIndexes.Krobus));
					RequiredItems = Globals.RNGGetRandomValuesFromList(potentialItems, 8, forceNumberOfValuesRNGCalls: true);
					MinimumRequiredItems = Math.Min(Math.Max(RequiredItems.Count - 2, 3), RequiredItems.Count);
					Color = BundleColors.Blue;
					break;
				case BundleTypes.BulletinLeah:
					SetBundleName("Leah-name");
					potentialItems = RequiredItem.CreateList(
						PreferenceRandomizer.GetLovedItems(GiftableNPCIndexes.Leah));
					RequiredItems = Globals.RNGGetRandomValuesFromList(potentialItems, 8, forceNumberOfValuesRNGCalls: true);
					MinimumRequiredItems = Math.Min(Math.Max(RequiredItems.Count - 2, 3), RequiredItems.Count);
					Color = BundleColors.Green;
					break;
				case BundleTypes.BulletinLewis:
					SetBundleName("Lewis-name");
					potentialItems = RequiredItem.CreateList(
                        PreferenceRandomizer.GetLovedItems(GiftableNPCIndexes.Lewis));
                    RequiredItems = Globals.RNGGetRandomValuesFromList(potentialItems, 8, forceNumberOfValuesRNGCalls: true);
					MinimumRequiredItems = Math.Min(Math.Max(RequiredItems.Count - 2, 3), RequiredItems.Count);
					Color = BundleColors.Green;
					break;
				case BundleTypes.BulletinLinus:
					SetBundleName("Linus-name");
					potentialItems = RequiredItem.CreateList(
                        PreferenceRandomizer.GetLovedItems(GiftableNPCIndexes.Linus));
					RequiredItems = Globals.RNGGetRandomValuesFromList(potentialItems, 8, forceNumberOfValuesRNGCalls: true);
					MinimumRequiredItems = Math.Min(Math.Max(RequiredItems.Count - 2, 3), RequiredItems.Count);
					Color = BundleColors.Orange;
					break;
				case BundleTypes.BulletinMarnie:
					SetBundleName("Marnie-name");
					potentialItems = RequiredItem.CreateList(
                        PreferenceRandomizer.GetLovedItems(GiftableNPCIndexes.Marnie));
                    RequiredItems = Globals.RNGGetRandomValuesFromList(potentialItems, 8, forceNumberOfValuesRNGCalls: true);
					MinimumRequiredItems = Math.Min(Math.Max(RequiredItems.Count - 2, 3), RequiredItems.Count);
					Color = BundleColors.Orange;
					break;
				case BundleTypes.BulletinMaru:
					SetBundleName("Maru-name");
					potentialItems = RequiredItem.CreateList(
                        PreferenceRandomizer.GetLovedItems(GiftableNPCIndexes.Maru));
                    RequiredItems = Globals.RNGGetRandomValuesFromList(potentialItems, 8, forceNumberOfValuesRNGCalls: true);
					MinimumRequiredItems = Math.Min(Math.Max(RequiredItems.Count - 2, 3), RequiredItems.Count);
					Color = BundleColors.Purple;
					break;
				case BundleTypes.BulletinPam:
					SetBundleName("Pam-name");
					potentialItems = RequiredItem.CreateList(
                        PreferenceRandomizer.GetLovedItems(GiftableNPCIndexes.Pam));
                    RequiredItems = Globals.RNGGetRandomValuesFromList(potentialItems, 8, forceNumberOfValuesRNGCalls: true);
					MinimumRequiredItems = Math.Min(Math.Max(RequiredItems.Count - 2, 3), RequiredItems.Count);
					Color = BundleColors.Purple;
					break;
				case BundleTypes.BulletinPenny:
					SetBundleName("Penny-name");
					potentialItems = RequiredItem.CreateList(
                        PreferenceRandomizer.GetLovedItems(GiftableNPCIndexes.Penny));
                    RequiredItems = Globals.RNGGetRandomValuesFromList(potentialItems, 8, forceNumberOfValuesRNGCalls: true);
					MinimumRequiredItems = Math.Min(Math.Max(RequiredItems.Count - 2, 3), RequiredItems.Count);
					Color = BundleColors.Yellow;
					break;
				case BundleTypes.BulletinPierre:
					SetBundleName("Pierre-name");
					potentialItems = RequiredItem.CreateList(
                        PreferenceRandomizer.GetLovedItems(GiftableNPCIndexes.Pierre));
                    RequiredItems = Globals.RNGGetRandomValuesFromList(potentialItems, 8, forceNumberOfValuesRNGCalls: true);
					MinimumRequiredItems = Math.Min(Math.Max(RequiredItems.Count - 2, 3), RequiredItems.Count);
					Color = BundleColors.Orange;
					break;
				case BundleTypes.BulletinRobin:
					SetBundleName("Robin-name");
					potentialItems = RequiredItem.CreateList(
                        PreferenceRandomizer.GetLovedItems(GiftableNPCIndexes.Robin));
                    RequiredItems = Globals.RNGGetRandomValuesFromList(potentialItems, 8, forceNumberOfValuesRNGCalls: true);
					MinimumRequiredItems = Math.Min(Math.Max(RequiredItems.Count - 2, 3), RequiredItems.Count);
					Color = BundleColors.Yellow;
					break;
				case BundleTypes.BulletinSam:
					SetBundleName("Sam-name");
					potentialItems = RequiredItem.CreateList(
                        PreferenceRandomizer.GetLovedItems(GiftableNPCIndexes.Sam));
                    RequiredItems = Globals.RNGGetRandomValuesFromList(potentialItems, 8, forceNumberOfValuesRNGCalls: true);
					MinimumRequiredItems = Math.Min(Math.Max(RequiredItems.Count - 2, 3), RequiredItems.Count);
					Color = BundleColors.Blue;
					break;
				case BundleTypes.BulletinSandy:
					SetBundleName("Sandy-name");
					potentialItems = RequiredItem.CreateList(
                        PreferenceRandomizer.GetLovedItems(GiftableNPCIndexes.Sandy));
                    RequiredItems = Globals.RNGGetRandomValuesFromList(potentialItems, 8, forceNumberOfValuesRNGCalls: true);
					MinimumRequiredItems = Math.Min(Math.Max(RequiredItems.Count - 2, 3), RequiredItems.Count);
					Color = BundleColors.Blue;
					break;
				case BundleTypes.BulletinSebastian:
					SetBundleName("Sebastian-name");
					potentialItems = RequiredItem.CreateList(
                        PreferenceRandomizer.GetLovedItems(GiftableNPCIndexes.Sebastian));
                    RequiredItems = Globals.RNGGetRandomValuesFromList(potentialItems, 8, forceNumberOfValuesRNGCalls: true);
					MinimumRequiredItems = Math.Min(Math.Max(RequiredItems.Count - 2, 3), RequiredItems.Count);
					Color = BundleColors.Purple;
					break;
				case BundleTypes.BulletinShane:
					SetBundleName("Shane-name");
					potentialItems = RequiredItem.CreateList(
                        PreferenceRandomizer.GetLovedItems(GiftableNPCIndexes.Shane));
                    RequiredItems = Globals.RNGGetRandomValuesFromList(potentialItems, 8, forceNumberOfValuesRNGCalls: true);
					MinimumRequiredItems = Math.Min(Math.Max(RequiredItems.Count - 2, 3), RequiredItems.Count);
					Color = BundleColors.Blue;
					break;
				case BundleTypes.BulletinVincent:
					SetBundleName("Vincent-name");
					potentialItems = RequiredItem.CreateList(
                        PreferenceRandomizer.GetLovedItems(GiftableNPCIndexes.Vincent));
                    RequiredItems = Globals.RNGGetRandomValuesFromList(potentialItems, 8, forceNumberOfValuesRNGCalls: true);
					MinimumRequiredItems = Math.Min(Math.Max(RequiredItems.Count - 2, 3), RequiredItems.Count);
					Color = BundleColors.Red;
					break;
				case BundleTypes.BulletinWilly:
					SetBundleName("Willy-name");
					potentialItems = RequiredItem.CreateList(
                        PreferenceRandomizer.GetLovedItems(GiftableNPCIndexes.Willy));
                    RequiredItems = Globals.RNGGetRandomValuesFromList(potentialItems, 8, forceNumberOfValuesRNGCalls: true);
					MinimumRequiredItems = Math.Min(Math.Max(RequiredItems.Count - 2, 3), RequiredItems.Count);
					Color = BundleColors.Red;
					break;
				case BundleTypes.BulletinWizard:
					SetBundleName("Wizard-name");
					potentialItems = RequiredItem.CreateList(
                        PreferenceRandomizer.GetLovedItems(GiftableNPCIndexes.Wizard));
                    RequiredItems = Globals.RNGGetRandomValuesFromList(potentialItems, 8, forceNumberOfValuesRNGCalls: true);
					MinimumRequiredItems = Math.Min(Math.Max(RequiredItems.Count - 2, 3), RequiredItems.Count);
					Color = BundleColors.Purple;
					break;
				case BundleTypes.BulletinColorPink:
					SetBundleName("bundle-bulletin-pink");
					potentialItems = RequiredItem.CreateList(new List<ObjectIndexes>
					{
						ObjectIndexes.Shrimp,
						ObjectIndexes.StrangeBun,
						ObjectIndexes.SalmonDinner,
						ObjectIndexes.PinkCake,
						ObjectIndexes.Sashimi,
						ObjectIndexes.IceCream,
						ObjectIndexes.Salmonberry,
						ObjectIndexes.Coral,
						ObjectIndexes.Dolomite,
						ObjectIndexes.Nekoite,
						ObjectIndexes.StarShards,
						ObjectIndexes.Peach,
						ObjectIndexes.BugMeat,
						ObjectIndexes.Bait
					});
					RequiredItems = Globals.RNGGetRandomValuesFromList(potentialItems, 8);
					MinimumRequiredItems = Range.GetRandomValue(3, 6);
					Color = BundleColors.Red;
					break;
				case BundleTypes.BulletinColorWhite:
					SetBundleName("bundle-bulletin-white");
					potentialItems = RequiredItem.CreateList(new List<ObjectIndexes>
					{
						ObjectIndexes.Leek,
						ObjectIndexes.Quartz,
						ObjectIndexes.OrnamentalFan,
						ObjectIndexes.DinosaurEgg,
						ObjectIndexes.ChickenStatue,
						ObjectIndexes.WhiteAlgae,
						ObjectIndexes.WhiteEgg,
						ObjectIndexes.LargeWhiteEgg,
						ObjectIndexes.Milk,
						ObjectIndexes.LargeMilk,
						ObjectIndexes.FriedEgg,
						ObjectIndexes.RicePudding,
						ObjectIndexes.IceCream,
						ObjectIndexes.Mayonnaise,
						ObjectIndexes.IronBar,
						ObjectIndexes.RefinedQuartz,
						ObjectIndexes.IronOre,
						ObjectIndexes.SpringOnion,
						ObjectIndexes.SnowYam,
						ObjectIndexes.Rice,
						ObjectIndexes.GoatCheese,
						ObjectIndexes.Cloth,
						ObjectIndexes.GoatMilk,
						ObjectIndexes.LargeGoatMilk,
						ObjectIndexes.Wool,
						ObjectIndexes.DuckEgg,
						ObjectIndexes.RabbitsFoot,
						ObjectIndexes.PaleBroth,
						ObjectIndexes.Esperite,
						ObjectIndexes.Lunarite,
						ObjectIndexes.Marble,
						ObjectIndexes.PrehistoricScapula,
						ObjectIndexes.PrehistoricTibia,
						ObjectIndexes.PrehistoricSkull,
						ObjectIndexes.SkeletalHand,
						ObjectIndexes.PrehistoricRib,
						ObjectIndexes.PrehistoricVertebra,
						ObjectIndexes.SkeletalTail,
						ObjectIndexes.NautilusFossil,
						ObjectIndexes.Trilobite,
						ObjectIndexes.ArtichokeDip,
						ObjectIndexes.LeadBobber,
						ObjectIndexes.Chowder
					});
					RequiredItems = Globals.RNGGetRandomValuesFromList(potentialItems, 8);
					MinimumRequiredItems = Range.GetRandomValue(3, 6);
					Color = BundleColors.Yellow;
					break;
			}
		}

		/// <summary>
		/// Generates the reward for the bundle
		/// </summary>
		protected override void GenerateReward()
		{
			if (Globals.RNGGetNextBoolean(1))
			{
				Reward = new RequiredItem(ObjectIndexes.PrismaticShard);
			}

			else if (Globals.RNGGetNextBoolean(5))
			{
				// The idea is that we want a GOOD reward, so use the original data in case the
				// generated love list is really bad
				List<Item> universalLoves = PreferenceRandomizer
					.GetUniversalPreferences(UniversalPreferencesIndexes.Loved, forceOriginalData: true)
					.Where(x => x.Id != (int)ObjectIndexes.PrismaticShard)
					.ToList();

				Reward = Globals.RNGGetRandomValueFromList(RequiredItem.CreateList(universalLoves, 5, 10));
			}

			List<RequiredItem> potentialRewards = new()
			{
				new(BigCraftableIndexes.JunimoKartArcadeSystem),
				new(BigCraftableIndexes.PrairieKingArcadeSystem),
				new(BigCraftableIndexes.SodaMachine),
				new(ObjectIndexes.Beer, 43),
				new(ObjectIndexes.Salad, Range.GetRandomValue(5, 25)),
				new(ObjectIndexes.Bread, Range.GetRandomValue(5, 25)),
				new(ObjectIndexes.Spaghetti, Range.GetRandomValue(5, 25)),
				new(ObjectIndexes.Pizza, Range.GetRandomValue(5, 25)),
				new(ObjectIndexes.Coffee, Range.GetRandomValue(5, 25))
			};

			Reward = Globals.RNGGetRandomValueFromList(potentialRewards);
		}
	}
}
