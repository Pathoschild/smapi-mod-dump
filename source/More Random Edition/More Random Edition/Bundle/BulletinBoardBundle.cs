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
			RNG rng = BundleRandomizer.Rng;
            BundleType = rng.GetAndRemoveRandomValueFromList(RoomBundleTypes);
			List<RequiredBundleItem> potentialItems = new();

			switch (BundleType)
			{
				case BundleTypes.BulletinNews:
					SetBundleName("bundle-bulletin-news");
					potentialItems = new List<RequiredBundleItem>()
					{
						new(ObjectIndexes.SoggyNewspaper),
						new(ObjectIndexes.SoggyNewspaper),
						new(ObjectIndexes.SoggyNewspaper),
						new(ObjectIndexes.SoggyNewspaper),
						new(ObjectIndexes.SoggyNewspaper),
						new(ObjectIndexes.SoggyNewspaper),
						new(ObjectIndexes.SoggyNewspaper),
						new(ObjectIndexes.SoggyNewspaper),
					};
					RequiredItems = rng.GetRandomValuesFromList(potentialItems, rng.NextIntWithinRange(1, 8));
					Color = BundleColors.Orange;
					break;
				case BundleTypes.BulletinCleanup:
					SetBundleName("bundle-bulletin-cleanup");
					RequiredItems = RequiredBundleItem.CreateList(ItemList.GetTrash(), 1, 5);
					Color = BundleColors.Green;
					break;
				case BundleTypes.BulletinHated:
					SetBundleName("bundle-bulletin-hated"); 
					potentialItems = RequiredBundleItem.CreateList(
						PreferenceRandomizer.GetUniversalPreferences(UniversalPreferencesIndexes.Hated));
					RequiredItems = rng.GetRandomValuesFromList(potentialItems, 8, forceNumberOfValuesRNGCalls: true);
					MinimumRequiredItems = rng.NextIntWithinRange(4, 6);
					Color = BundleColors.Red;
					break;
				case BundleTypes.BulletinLoved:
					SetBundleName("bundle-bulletin-loved");
					potentialItems = RequiredItems = RequiredBundleItem.CreateList(
						PreferenceRandomizer.GetUniversalPreferences(UniversalPreferencesIndexes.Loved));
                    RequiredItems = rng.GetRandomValuesFromList(potentialItems, 8, forceNumberOfValuesRNGCalls: true);
                    MinimumRequiredItems = rng.NextIntWithinRange(2, 6);
                    Color = BundleColors.Red;
					break;
				case BundleTypes.BulletinAbigail:
					SetBundleName("Abigail-name");
					potentialItems = RequiredBundleItem.CreateList(
                        PreferenceRandomizer.GetLovedItems(GiftableNPCIndexes.Abigail));
                    RequiredItems = rng.GetRandomValuesFromList(potentialItems, 8, forceNumberOfValuesRNGCalls: true);
					MinimumRequiredItems = Math.Min(Math.Max(RequiredItems.Count - 2, 3), RequiredItems.Count);
					Color = BundleColors.Purple;
					break;
				case BundleTypes.BulletinAlex:
					SetBundleName("Alex-name");
					potentialItems = RequiredBundleItem.CreateList(
                        PreferenceRandomizer.GetLovedItems(GiftableNPCIndexes.Alex));
                    RequiredItems = rng.GetRandomValuesFromList(potentialItems, 8, forceNumberOfValuesRNGCalls: true);
					MinimumRequiredItems = Math.Min(Math.Max(RequiredItems.Count - 2, 3), RequiredItems.Count);
					Color = BundleColors.Green;
					break;
				case BundleTypes.BulletinCaroline:
					SetBundleName("Caroline-name");
					potentialItems = RequiredBundleItem.CreateList(
                        PreferenceRandomizer.GetLovedItems(GiftableNPCIndexes.Caroline));
                    RequiredItems = rng.GetRandomValuesFromList(potentialItems, 8, forceNumberOfValuesRNGCalls: true);
					MinimumRequiredItems = Math.Min(Math.Max(RequiredItems.Count - 2, 3), RequiredItems.Count);
					Color = BundleColors.Blue;
					break;
				case BundleTypes.BulletinClint:
					SetBundleName("Clint-name");
					potentialItems = RequiredBundleItem.CreateList(
                        PreferenceRandomizer.GetLovedItems(GiftableNPCIndexes.Clint));
                    RequiredItems = rng.GetRandomValuesFromList(potentialItems, 8, forceNumberOfValuesRNGCalls: true);
					MinimumRequiredItems = Math.Min(Math.Max(RequiredItems.Count - 2, 3), RequiredItems.Count);
					Color = BundleColors.Orange;
					break;
				case BundleTypes.BulletinDwarf:
					SetBundleName("Dwarf-name");
					potentialItems = RequiredBundleItem.CreateList(
                        PreferenceRandomizer.GetLovedItems(GiftableNPCIndexes.Dwarf));
                    RequiredItems = rng.GetRandomValuesFromList(potentialItems, 8, forceNumberOfValuesRNGCalls: true);
					MinimumRequiredItems = Math.Min(Math.Max(RequiredItems.Count - 2, 3), RequiredItems.Count);
					Color = BundleColors.Orange;
					break;
				case BundleTypes.BulletinDemetrius:
					SetBundleName("Demetrius-name");
					potentialItems = RequiredBundleItem.CreateList(
                        PreferenceRandomizer.GetLovedItems(GiftableNPCIndexes.Demetrius));
                    RequiredItems = rng.GetRandomValuesFromList(potentialItems, 8, forceNumberOfValuesRNGCalls: true);
					MinimumRequiredItems = Math.Min(Math.Max(RequiredItems.Count - 2, 3), RequiredItems.Count);
					Color = BundleColors.Blue;
					break;
				case BundleTypes.BulletinElliott:
					SetBundleName("Elliott-name");
					potentialItems = RequiredBundleItem.CreateList(
                        PreferenceRandomizer.GetLovedItems(GiftableNPCIndexes.Elliott));
                    RequiredItems = rng.GetRandomValuesFromList(potentialItems, 8, forceNumberOfValuesRNGCalls: true);
					MinimumRequiredItems = Math.Min(Math.Max(RequiredItems.Count - 2, 3), RequiredItems.Count);
					Color = BundleColors.Red;
					break;
				case BundleTypes.BulletinEmily:
					SetBundleName("Emily-name");
					potentialItems = RequiredBundleItem.CreateList(
                        PreferenceRandomizer.GetLovedItems(GiftableNPCIndexes.Emily));
                    RequiredItems = rng.GetRandomValuesFromList(potentialItems, 8, forceNumberOfValuesRNGCalls: true);
					MinimumRequiredItems = Math.Min(Math.Max(RequiredItems.Count - 2, 3), RequiredItems.Count);
					Color = BundleColors.Red;
					break;
				case BundleTypes.BulletinEvelyn:
					SetBundleName("Evelyn-name");
					potentialItems = RequiredBundleItem.CreateList(
                        PreferenceRandomizer.GetLovedItems(GiftableNPCIndexes.Evelyn));
                    RequiredItems = rng.GetRandomValuesFromList(potentialItems, 8, forceNumberOfValuesRNGCalls: true);
					MinimumRequiredItems = Math.Min(Math.Max(RequiredItems.Count - 2, 3), RequiredItems.Count);
					Color = BundleColors.Red;
					break;
				case BundleTypes.BulletinGeorge:
					SetBundleName("George-name");
					potentialItems = RequiredBundleItem.CreateList(
                        PreferenceRandomizer.GetLovedItems(GiftableNPCIndexes.George));
                    RequiredItems = rng.GetRandomValuesFromList(potentialItems, 8, forceNumberOfValuesRNGCalls: true);
					MinimumRequiredItems = Math.Min(Math.Max(RequiredItems.Count - 2, 3), RequiredItems.Count);
					Color = BundleColors.Green;
					break;
				case BundleTypes.BulletinGus:
					SetBundleName("Gus-name");
					potentialItems = RequiredBundleItem.CreateList(
                        PreferenceRandomizer.GetLovedItems(GiftableNPCIndexes.Gus));
                    RequiredItems = rng.GetRandomValuesFromList(potentialItems, 8, forceNumberOfValuesRNGCalls: true);
					MinimumRequiredItems = Math.Min(Math.Max(RequiredItems.Count - 2, 3), RequiredItems.Count);
					Color = BundleColors.Orange;
					break;
				case BundleTypes.BulletinHaley:
					SetBundleName("Haley-name");
					potentialItems = RequiredBundleItem.CreateList(
                        PreferenceRandomizer.GetLovedItems(GiftableNPCIndexes.Haley));
                    RequiredItems = rng.GetRandomValuesFromList(potentialItems, 8, forceNumberOfValuesRNGCalls: true);
					MinimumRequiredItems = Math.Min(Math.Max(RequiredItems.Count - 2, 3), RequiredItems.Count);
					Color = BundleColors.Blue;
					break;
				case BundleTypes.BulletinHarvey:
					SetBundleName("Harvey-name");
					potentialItems = RequiredBundleItem.CreateList(
                        PreferenceRandomizer.GetLovedItems(GiftableNPCIndexes.Harvey));
                    RequiredItems = rng.GetRandomValuesFromList(potentialItems, 8, forceNumberOfValuesRNGCalls: true);
					MinimumRequiredItems = Math.Min(Math.Max(RequiredItems.Count - 2, 3), RequiredItems.Count);
					Color = BundleColors.Green;
					break;
				case BundleTypes.BulletinJas:
					SetBundleName("Jas-name");
					potentialItems = RequiredBundleItem.CreateList(
                        PreferenceRandomizer.GetLovedItems(GiftableNPCIndexes.Jas));
                    RequiredItems = rng.GetRandomValuesFromList(potentialItems, 8, forceNumberOfValuesRNGCalls: true);
					MinimumRequiredItems = Math.Min(Math.Max(RequiredItems.Count - 2, 3), RequiredItems.Count);
					Color = BundleColors.Purple;
					break;
				case BundleTypes.BulletinJodi:
					SetBundleName("Jodi-name");
					potentialItems = RequiredBundleItem.CreateList(
                        PreferenceRandomizer.GetLovedItems(GiftableNPCIndexes.Jodi));
                    RequiredItems = rng.GetRandomValuesFromList(potentialItems, 8, forceNumberOfValuesRNGCalls: true);
					MinimumRequiredItems = Math.Min(Math.Max(RequiredItems.Count - 2, 3), RequiredItems.Count);
					Color = BundleColors.Purple;
					break;
				case BundleTypes.BulletinKent:
					SetBundleName("Kent-name");
					potentialItems = RequiredBundleItem.CreateList(
                        PreferenceRandomizer.GetLovedItems(GiftableNPCIndexes.Kent));
                    RequiredItems = rng.GetRandomValuesFromList(potentialItems, 8, forceNumberOfValuesRNGCalls: true);
					MinimumRequiredItems = Math.Min(Math.Max(RequiredItems.Count - 2, 3), RequiredItems.Count);
					Color = BundleColors.Green;
					break;
				case BundleTypes.BulletinKrobus:
					SetBundleName("Krobus-name");
					potentialItems = RequiredBundleItem.CreateList(
						PreferenceRandomizer.GetLovedItems(GiftableNPCIndexes.Krobus));
					RequiredItems = rng.GetRandomValuesFromList(potentialItems, 8, forceNumberOfValuesRNGCalls: true);
					MinimumRequiredItems = Math.Min(Math.Max(RequiredItems.Count - 2, 3), RequiredItems.Count);
					Color = BundleColors.Blue;
					break;
				case BundleTypes.BulletinLeah:
					SetBundleName("Leah-name");
					potentialItems = RequiredBundleItem.CreateList(
						PreferenceRandomizer.GetLovedItems(GiftableNPCIndexes.Leah));
					RequiredItems = rng.GetRandomValuesFromList(potentialItems, 8, forceNumberOfValuesRNGCalls: true);
					MinimumRequiredItems = Math.Min(Math.Max(RequiredItems.Count - 2, 3), RequiredItems.Count);
					Color = BundleColors.Green;
					break;
				case BundleTypes.BulletinLewis:
					SetBundleName("Lewis-name");
					potentialItems = RequiredBundleItem.CreateList(
                        PreferenceRandomizer.GetLovedItems(GiftableNPCIndexes.Lewis));
                    RequiredItems = rng.GetRandomValuesFromList(potentialItems, 8, forceNumberOfValuesRNGCalls: true);
					MinimumRequiredItems = Math.Min(Math.Max(RequiredItems.Count - 2, 3), RequiredItems.Count);
					Color = BundleColors.Green;
					break;
				case BundleTypes.BulletinLinus:
					SetBundleName("Linus-name");
					potentialItems = RequiredBundleItem.CreateList(
                        PreferenceRandomizer.GetLovedItems(GiftableNPCIndexes.Linus));
					RequiredItems = rng.GetRandomValuesFromList(potentialItems, 8, forceNumberOfValuesRNGCalls: true);
					MinimumRequiredItems = Math.Min(Math.Max(RequiredItems.Count - 2, 3), RequiredItems.Count);
					Color = BundleColors.Orange;
					break;
				case BundleTypes.BulletinMarnie:
					SetBundleName("Marnie-name");
					potentialItems = RequiredBundleItem.CreateList(
                        PreferenceRandomizer.GetLovedItems(GiftableNPCIndexes.Marnie));
                    RequiredItems = rng.GetRandomValuesFromList(potentialItems, 8, forceNumberOfValuesRNGCalls: true);
					MinimumRequiredItems = Math.Min(Math.Max(RequiredItems.Count - 2, 3), RequiredItems.Count);
					Color = BundleColors.Orange;
					break;
				case BundleTypes.BulletinMaru:
					SetBundleName("Maru-name");
					potentialItems = RequiredBundleItem.CreateList(
                        PreferenceRandomizer.GetLovedItems(GiftableNPCIndexes.Maru));
                    RequiredItems = rng.GetRandomValuesFromList(potentialItems, 8, forceNumberOfValuesRNGCalls: true);
					MinimumRequiredItems = Math.Min(Math.Max(RequiredItems.Count - 2, 3), RequiredItems.Count);
					Color = BundleColors.Purple;
					break;
				case BundleTypes.BulletinPam:
					SetBundleName("Pam-name");
					potentialItems = RequiredBundleItem.CreateList(
                        PreferenceRandomizer.GetLovedItems(GiftableNPCIndexes.Pam));
                    RequiredItems = rng.GetRandomValuesFromList(potentialItems, 8, forceNumberOfValuesRNGCalls: true);
					MinimumRequiredItems = Math.Min(Math.Max(RequiredItems.Count - 2, 3), RequiredItems.Count);
					Color = BundleColors.Purple;
					break;
				case BundleTypes.BulletinPenny:
					SetBundleName("Penny-name");
					potentialItems = RequiredBundleItem.CreateList(
                        PreferenceRandomizer.GetLovedItems(GiftableNPCIndexes.Penny));
                    RequiredItems = rng.GetRandomValuesFromList(potentialItems, 8, forceNumberOfValuesRNGCalls: true);
					MinimumRequiredItems = Math.Min(Math.Max(RequiredItems.Count - 2, 3), RequiredItems.Count);
					Color = BundleColors.Yellow;
					break;
				case BundleTypes.BulletinPierre:
					SetBundleName("Pierre-name");
					potentialItems = RequiredBundleItem.CreateList(
                        PreferenceRandomizer.GetLovedItems(GiftableNPCIndexes.Pierre));
                    RequiredItems = rng.GetRandomValuesFromList(potentialItems, 8, forceNumberOfValuesRNGCalls: true);
					MinimumRequiredItems = Math.Min(Math.Max(RequiredItems.Count - 2, 3), RequiredItems.Count);
					Color = BundleColors.Orange;
					break;
				case BundleTypes.BulletinRobin:
					SetBundleName("Robin-name");
					potentialItems = RequiredBundleItem.CreateList(
                        PreferenceRandomizer.GetLovedItems(GiftableNPCIndexes.Robin));
                    RequiredItems = rng.GetRandomValuesFromList(potentialItems, 8, forceNumberOfValuesRNGCalls: true);
					MinimumRequiredItems = Math.Min(Math.Max(RequiredItems.Count - 2, 3), RequiredItems.Count);
					Color = BundleColors.Yellow;
					break;
				case BundleTypes.BulletinSam:
					SetBundleName("Sam-name");
					potentialItems = RequiredBundleItem.CreateList(
                        PreferenceRandomizer.GetLovedItems(GiftableNPCIndexes.Sam));
                    RequiredItems = rng.GetRandomValuesFromList(potentialItems, 8, forceNumberOfValuesRNGCalls: true);
					MinimumRequiredItems = Math.Min(Math.Max(RequiredItems.Count - 2, 3), RequiredItems.Count);
					Color = BundleColors.Blue;
					break;
				case BundleTypes.BulletinSandy:
					SetBundleName("Sandy-name");
					potentialItems = RequiredBundleItem.CreateList(
                        PreferenceRandomizer.GetLovedItems(GiftableNPCIndexes.Sandy));
                    RequiredItems = rng.GetRandomValuesFromList(potentialItems, 8, forceNumberOfValuesRNGCalls: true);
					MinimumRequiredItems = Math.Min(Math.Max(RequiredItems.Count - 2, 3), RequiredItems.Count);
					Color = BundleColors.Blue;
					break;
				case BundleTypes.BulletinSebastian:
					SetBundleName("Sebastian-name");
					potentialItems = RequiredBundleItem.CreateList(
                        PreferenceRandomizer.GetLovedItems(GiftableNPCIndexes.Sebastian));
                    RequiredItems = rng.GetRandomValuesFromList(potentialItems, 8, forceNumberOfValuesRNGCalls: true);
					MinimumRequiredItems = Math.Min(Math.Max(RequiredItems.Count - 2, 3), RequiredItems.Count);
					Color = BundleColors.Purple;
					break;
				case BundleTypes.BulletinShane:
					SetBundleName("Shane-name");
					potentialItems = RequiredBundleItem.CreateList(
                        PreferenceRandomizer.GetLovedItems(GiftableNPCIndexes.Shane));
                    RequiredItems = rng.GetRandomValuesFromList(potentialItems, 8, forceNumberOfValuesRNGCalls: true);
					MinimumRequiredItems = Math.Min(Math.Max(RequiredItems.Count - 2, 3), RequiredItems.Count);
					Color = BundleColors.Blue;
					break;
				case BundleTypes.BulletinVincent:
					SetBundleName("Vincent-name");
					potentialItems = RequiredBundleItem.CreateList(
                        PreferenceRandomizer.GetLovedItems(GiftableNPCIndexes.Vincent));
                    RequiredItems = rng.GetRandomValuesFromList(potentialItems, 8, forceNumberOfValuesRNGCalls: true);
					MinimumRequiredItems = Math.Min(Math.Max(RequiredItems.Count - 2, 3), RequiredItems.Count);
					Color = BundleColors.Red;
					break;
				case BundleTypes.BulletinWilly:
					SetBundleName("Willy-name");
					potentialItems = RequiredBundleItem.CreateList(
                        PreferenceRandomizer.GetLovedItems(GiftableNPCIndexes.Willy));
                    RequiredItems = rng.GetRandomValuesFromList(potentialItems, 8, forceNumberOfValuesRNGCalls: true);
					MinimumRequiredItems = Math.Min(Math.Max(RequiredItems.Count - 2, 3), RequiredItems.Count);
					Color = BundleColors.Red;
					break;
				case BundleTypes.BulletinWizard:
					SetBundleName("Wizard-name");
					potentialItems = RequiredBundleItem.CreateList(
                        PreferenceRandomizer.GetLovedItems(GiftableNPCIndexes.Wizard));
                    RequiredItems = rng.GetRandomValuesFromList(potentialItems, 8, forceNumberOfValuesRNGCalls: true);
					MinimumRequiredItems = Math.Min(Math.Max(RequiredItems.Count - 2, 3), RequiredItems.Count);
					Color = BundleColors.Purple;
					break;
				case BundleTypes.BulletinColorPink:
					SetBundleName("bundle-bulletin-pink");
					potentialItems = RequiredBundleItem.CreateList(new List<ObjectIndexes>
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
					RequiredItems = rng.GetRandomValuesFromList(potentialItems, 8);
					MinimumRequiredItems = rng.NextIntWithinRange(3, 6);
					Color = BundleColors.Red;
					break;
				case BundleTypes.BulletinColorWhite:
					SetBundleName("bundle-bulletin-white");
					potentialItems = RequiredBundleItem.CreateList(new List<ObjectIndexes>
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
					RequiredItems = rng.GetRandomValuesFromList(potentialItems, 8);
					MinimumRequiredItems = rng.NextIntWithinRange(3, 6);
					Color = BundleColors.Yellow;
					break;
			}
		}

		/// <summary>
		/// Generates the reward for the bundle
		/// </summary>
		protected override void GenerateReward()
		{
            RNG rng = BundleRandomizer.Rng;

            if (rng.NextBoolean(1))
			{
				Reward = new RequiredBundleItem(ObjectIndexes.PrismaticShard);
			}

			else if (rng.NextBoolean(5))
			{
				// The idea is that we want a GOOD reward, so use the original data in case the
				// generated love list is really bad
				List<Item> universalLoves = PreferenceRandomizer
					.GetUniversalPreferences(UniversalPreferencesIndexes.Loved, forceOriginalData: true)
					.Where(x => x.ObjectIndex != ObjectIndexes.PrismaticShard)
					.ToList();

				Reward = rng.GetRandomValueFromList(RequiredBundleItem.CreateList(universalLoves, 5, 10));
			}

			List<RequiredBundleItem> potentialRewards = new()
			{
				new(BigCraftableIndexes.JunimoKartArcadeSystem),
				new(BigCraftableIndexes.PrairieKingArcadeSystem),
				new(BigCraftableIndexes.SodaMachine),
				new(ObjectIndexes.Beer, 43),
				new(ObjectIndexes.Salad, rng.NextIntWithinRange(5, 25)),
				new(ObjectIndexes.Bread, rng.NextIntWithinRange(5, 25)),
				new(ObjectIndexes.Spaghetti, rng.NextIntWithinRange(5, 25)),
				new(ObjectIndexes.Pizza, rng.NextIntWithinRange(5, 25)),
				new(ObjectIndexes.Coffee, rng.NextIntWithinRange(5, 25))
			};

			Reward = rng.GetRandomValueFromList(potentialRewards);
		}
	}
}
