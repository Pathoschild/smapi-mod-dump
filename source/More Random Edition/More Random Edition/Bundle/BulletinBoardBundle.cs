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
					Name = Globals.GetTranslation("bundle-bulletin-news");
					potentialItems = new List<RequiredItem>()
					{
						new RequiredItem((int)ObjectIndexes.SoggyNewspaper),
						new RequiredItem((int)ObjectIndexes.SoggyNewspaper),
						new RequiredItem((int)ObjectIndexes.SoggyNewspaper),
						new RequiredItem((int)ObjectIndexes.SoggyNewspaper),
						new RequiredItem((int)ObjectIndexes.SoggyNewspaper),
						new RequiredItem((int)ObjectIndexes.SoggyNewspaper),
						new RequiredItem((int)ObjectIndexes.SoggyNewspaper),
						new RequiredItem((int)ObjectIndexes.SoggyNewspaper),
					};
					RequiredItems = Globals.RNGGetRandomValuesFromList(potentialItems, Range.GetRandomValue(1, 8));
					Color = BundleColors.Orange;
					break;
				case BundleTypes.BulletinCleanup:
					Name = Globals.GetTranslation("bundle-bulletin-cleanup");
					RequiredItems = RequiredItem.CreateList(ItemList.GetTrash(), 1, 5);
					Color = BundleColors.Green;
					break;
				case BundleTypes.BulletinHated:
					Name = Globals.GetTranslation("bundle-bulletin-hated"); potentialItems = RequiredItem.CreateList(NPC.UniversalHates);
					RequiredItems = Globals.RNGGetRandomValuesFromList(potentialItems, 8);
					MinimumRequiredItems = Range.GetRandomValue(4, 6);
					Color = BundleColors.Red;
					break;
				case BundleTypes.BulletinLoved:
					Name = Globals.GetTranslation("bundle-bulletin-loved");
					RequiredItems = RequiredItem.CreateList(NPC.UniversalLoves);
					MinimumRequiredItems = 2;
					Color = BundleColors.Red;
					break;
				case BundleTypes.BulletinAbigail:
					Name = Globals.GetTranslation("Abigail-name");
					potentialItems = RequiredItem.CreateList(Abigail.Loves);
					RequiredItems = Globals.RNGGetRandomValuesFromList(potentialItems, 8);
					MinimumRequiredItems = Math.Min(Math.Max(RequiredItems.Count - 2, 3), RequiredItems.Count);
					Color = BundleColors.Purple;
					break;
				case BundleTypes.BulletinAlex:
					Name = Globals.GetTranslation("Alex-name");
					potentialItems = RequiredItem.CreateList(Alex.Loves);
					RequiredItems = Globals.RNGGetRandomValuesFromList(potentialItems, 8);
					MinimumRequiredItems = Math.Min(Math.Max(RequiredItems.Count - 2, 3), RequiredItems.Count);
					Color = BundleColors.Green;
					break;
				case BundleTypes.BulletinCaroline:
					Name = Globals.GetTranslation("Caroline-name");
					potentialItems = RequiredItem.CreateList(Caroline.Loves);
					RequiredItems = Globals.RNGGetRandomValuesFromList(potentialItems, 8);
					MinimumRequiredItems = Math.Min(Math.Max(RequiredItems.Count - 2, 3), RequiredItems.Count);
					Color = BundleColors.Blue;
					break;
				case BundleTypes.BulletinClint:
					Name = Globals.GetTranslation("Clint-name");
					potentialItems = RequiredItem.CreateList(Clint.Loves);
					RequiredItems = Globals.RNGGetRandomValuesFromList(potentialItems, 8);
					MinimumRequiredItems = Math.Min(Math.Max(RequiredItems.Count - 2, 3), RequiredItems.Count);
					Color = BundleColors.Orange;
					break;
				case BundleTypes.BulletinDwarf:
					Name = Globals.GetTranslation("Dwarf-name");
					potentialItems = RequiredItem.CreateList(Dwarf.Loves);
					RequiredItems = Globals.RNGGetRandomValuesFromList(potentialItems, 8);
					MinimumRequiredItems = Math.Min(Math.Max(RequiredItems.Count - 2, 3), RequiredItems.Count);
					Color = BundleColors.Orange;
					break;
				case BundleTypes.BulletinDemetrius:
					Name = Globals.GetTranslation("Demetrius-name");
					potentialItems = RequiredItem.CreateList(Demetrius.Loves);
					RequiredItems = Globals.RNGGetRandomValuesFromList(potentialItems, 8);
					MinimumRequiredItems = Math.Min(Math.Max(RequiredItems.Count - 2, 3), RequiredItems.Count);
					Color = BundleColors.Blue;
					break;
				case BundleTypes.BulletinElliot:
					Name = Globals.GetTranslation("Elliot-name");
					potentialItems = RequiredItem.CreateList(Elliot.Loves);
					RequiredItems = Globals.RNGGetRandomValuesFromList(potentialItems, 8);
					MinimumRequiredItems = Math.Min(Math.Max(RequiredItems.Count - 2, 3), RequiredItems.Count);
					Color = BundleColors.Red;
					break;
				case BundleTypes.BulletinEmily:
					Name = Globals.GetTranslation("Emily-name");
					potentialItems = RequiredItem.CreateList(Emily.Loves);
					RequiredItems = Globals.RNGGetRandomValuesFromList(potentialItems, 8);
					MinimumRequiredItems = Math.Min(Math.Max(RequiredItems.Count - 2, 3), RequiredItems.Count);
					Color = BundleColors.Red;
					break;
				case BundleTypes.BulletinEvelyn:
					Name = Globals.GetTranslation("Evelyn-name");
					potentialItems = RequiredItem.CreateList(Evelyn.Loves);
					RequiredItems = Globals.RNGGetRandomValuesFromList(potentialItems, 8);
					MinimumRequiredItems = Math.Min(Math.Max(RequiredItems.Count - 2, 3), RequiredItems.Count);
					Color = BundleColors.Red;
					break;
				case BundleTypes.BulletinGeorge:
					Name = Globals.GetTranslation("George-name");
					potentialItems = RequiredItem.CreateList(George.Loves);
					RequiredItems = Globals.RNGGetRandomValuesFromList(potentialItems, 8);
					MinimumRequiredItems = Math.Min(Math.Max(RequiredItems.Count - 2, 3), RequiredItems.Count);
					Color = BundleColors.Green;
					break;
				case BundleTypes.BulletinGus:
					Name = Globals.GetTranslation("Gus-name");
					potentialItems = RequiredItem.CreateList(Gus.Loves);
					RequiredItems = Globals.RNGGetRandomValuesFromList(potentialItems, 8);
					MinimumRequiredItems = Math.Min(Math.Max(RequiredItems.Count - 2, 3), RequiredItems.Count);
					Color = BundleColors.Orange;
					break;
				case BundleTypes.BulletinHaley:
					Name = Globals.GetTranslation("Haley-name");
					potentialItems = RequiredItem.CreateList(Haley.Loves);
					RequiredItems = Globals.RNGGetRandomValuesFromList(potentialItems, 8);
					MinimumRequiredItems = Math.Min(Math.Max(RequiredItems.Count - 2, 3), RequiredItems.Count);
					Color = BundleColors.Blue;
					break;
				case BundleTypes.BulletinHarvey:
					Name = Globals.GetTranslation("Harvey-name");
					potentialItems = RequiredItem.CreateList(Harvey.Loves);
					RequiredItems = Globals.RNGGetRandomValuesFromList(potentialItems, 8);
					MinimumRequiredItems = Math.Min(Math.Max(RequiredItems.Count - 2, 3), RequiredItems.Count);
					Color = BundleColors.Green;
					break;
				case BundleTypes.BulletinJas:
					Name = Globals.GetTranslation("Jas-name");
					potentialItems = RequiredItem.CreateList(Jas.Loves);
					RequiredItems = Globals.RNGGetRandomValuesFromList(potentialItems, 8);
					MinimumRequiredItems = Math.Min(Math.Max(RequiredItems.Count - 2, 3), RequiredItems.Count);
					Color = BundleColors.Purple;
					break;
				case BundleTypes.BulletinJodi:
					Name = Globals.GetTranslation("Jodi-name");
					potentialItems = RequiredItem.CreateList(Jodi.Loves);
					RequiredItems = Globals.RNGGetRandomValuesFromList(potentialItems, 8);
					MinimumRequiredItems = Math.Min(Math.Max(RequiredItems.Count - 2, 3), RequiredItems.Count);
					Color = BundleColors.Purple;
					break;
				case BundleTypes.BulletinKent:
					Name = Globals.GetTranslation("Kent-name");
					potentialItems = RequiredItem.CreateList(Kent.Loves);
					RequiredItems = Globals.RNGGetRandomValuesFromList(potentialItems, 8);
					MinimumRequiredItems = Math.Min(Math.Max(RequiredItems.Count - 2, 3), RequiredItems.Count);
					Color = BundleColors.Green;
					break;
				case BundleTypes.BulletinKrobus:
					Name = Globals.GetTranslation("Krobus-name");
					potentialItems = RequiredItem.CreateList(Krobus.Loves);
					RequiredItems = Globals.RNGGetRandomValuesFromList(potentialItems, 8);
					MinimumRequiredItems = Math.Min(Math.Max(RequiredItems.Count - 2, 3), RequiredItems.Count);
					Color = BundleColors.Blue;
					break;
				case BundleTypes.BulletinLeah:
					Name = Globals.GetTranslation("Leah-name");
					potentialItems = RequiredItem.CreateList(Leah.Loves);
					RequiredItems = Globals.RNGGetRandomValuesFromList(potentialItems, 8);
					MinimumRequiredItems = Math.Min(Math.Max(RequiredItems.Count - 2, 3), RequiredItems.Count);
					Color = BundleColors.Green;
					break;
				case BundleTypes.BulletinLewis:
					Name = Globals.GetTranslation("Lewis-name");
					potentialItems = RequiredItem.CreateList(Lewis.Loves);
					RequiredItems = Globals.RNGGetRandomValuesFromList(potentialItems, 8);
					MinimumRequiredItems = Math.Min(Math.Max(RequiredItems.Count - 2, 3), RequiredItems.Count);
					Color = BundleColors.Green;
					break;
				case BundleTypes.BulletinLinus:
					Name = Globals.GetTranslation("Linus-name");
					potentialItems = RequiredItem.CreateList(Linus.Loves);
					RequiredItems = Globals.RNGGetRandomValuesFromList(potentialItems, 8);
					MinimumRequiredItems = Math.Min(Math.Max(RequiredItems.Count - 2, 3), RequiredItems.Count);
					Color = BundleColors.Orange;
					break;
				case BundleTypes.BulletinMarnie:
					Name = Globals.GetTranslation("Marnie-name");
					potentialItems = RequiredItem.CreateList(Marnie.Loves);
					RequiredItems = Globals.RNGGetRandomValuesFromList(potentialItems, 8);
					MinimumRequiredItems = Math.Min(Math.Max(RequiredItems.Count - 2, 3), RequiredItems.Count);
					Color = BundleColors.Orange;
					break;
				case BundleTypes.BulletinMaru:
					Name = Globals.GetTranslation("Maru-name");
					potentialItems = RequiredItem.CreateList(Maru.Loves);
					RequiredItems = Globals.RNGGetRandomValuesFromList(potentialItems, 8);
					MinimumRequiredItems = Math.Min(Math.Max(RequiredItems.Count - 2, 3), RequiredItems.Count);
					Color = BundleColors.Purple;
					break;
				case BundleTypes.BulletinPam:
					Name = Globals.GetTranslation("Pam-name");
					potentialItems = RequiredItem.CreateList(Pam.Loves);
					RequiredItems = Globals.RNGGetRandomValuesFromList(potentialItems, 8);
					MinimumRequiredItems = Math.Min(Math.Max(RequiredItems.Count - 2, 3), RequiredItems.Count);
					Color = BundleColors.Purple;
					break;
				case BundleTypes.BulletinPenny:
					Name = Globals.GetTranslation("Penny-name");
					potentialItems = RequiredItem.CreateList(Penny.Loves);
					RequiredItems = Globals.RNGGetRandomValuesFromList(potentialItems, 8);
					MinimumRequiredItems = Math.Min(Math.Max(RequiredItems.Count - 2, 3), RequiredItems.Count);
					Color = BundleColors.Yellow;
					break;
				case BundleTypes.BulletinPierre:
					Name = Globals.GetTranslation("Pierre-name");
					potentialItems = RequiredItem.CreateList(Pierre.Loves);
					RequiredItems = Globals.RNGGetRandomValuesFromList(potentialItems, 8);
					MinimumRequiredItems = Math.Min(Math.Max(RequiredItems.Count - 2, 3), RequiredItems.Count);
					Color = BundleColors.Orange;
					break;
				case BundleTypes.BulletinRobin:
					Name = Globals.GetTranslation("Robin-name");
					potentialItems = RequiredItem.CreateList(Robin.Loves);
					RequiredItems = Globals.RNGGetRandomValuesFromList(potentialItems, 8);
					MinimumRequiredItems = Math.Min(Math.Max(RequiredItems.Count - 2, 3), RequiredItems.Count);
					Color = BundleColors.Yellow;
					break;
				case BundleTypes.BulletinSam:
					Name = Globals.GetTranslation("Sam-name");
					potentialItems = RequiredItem.CreateList(Sam.Loves);
					RequiredItems = Globals.RNGGetRandomValuesFromList(potentialItems, 8);
					MinimumRequiredItems = Math.Min(Math.Max(RequiredItems.Count - 2, 3), RequiredItems.Count);
					Color = BundleColors.Blue;
					break;
				case BundleTypes.BulletinSandy:
					Name = Globals.GetTranslation("Sandy-name");
					potentialItems = RequiredItem.CreateList(Sandy.Loves);
					RequiredItems = Globals.RNGGetRandomValuesFromList(potentialItems, 8);
					MinimumRequiredItems = Math.Min(Math.Max(RequiredItems.Count - 2, 3), RequiredItems.Count);
					Color = BundleColors.Blue;
					break;
				case BundleTypes.BulletinSebastian:
					Name = Globals.GetTranslation("Sebastian-name");
					potentialItems = RequiredItem.CreateList(Sebastian.Loves);
					RequiredItems = Globals.RNGGetRandomValuesFromList(potentialItems, 8);
					MinimumRequiredItems = Math.Min(Math.Max(RequiredItems.Count - 2, 3), RequiredItems.Count);
					Color = BundleColors.Purple;
					break;
				case BundleTypes.BulletinShane:
					Name = Globals.GetTranslation("Shane-name");
					potentialItems = RequiredItem.CreateList(Shane.Loves);
					RequiredItems = Globals.RNGGetRandomValuesFromList(potentialItems, 8);
					MinimumRequiredItems = Math.Min(Math.Max(RequiredItems.Count - 2, 3), RequiredItems.Count);
					Color = BundleColors.Blue;
					break;
				case BundleTypes.BulletinVincent:
					Name = Globals.GetTranslation("Vincent-name");
					potentialItems = RequiredItem.CreateList(Vincent.Loves);
					RequiredItems = Globals.RNGGetRandomValuesFromList(potentialItems, 8);
					MinimumRequiredItems = Math.Min(Math.Max(RequiredItems.Count - 2, 3), RequiredItems.Count);
					Color = BundleColors.Red;
					break;
				case BundleTypes.BulletinWilly:
					Name = Globals.GetTranslation("Willy-name");
					potentialItems = RequiredItem.CreateList(Willy.Loves);
					RequiredItems = Globals.RNGGetRandomValuesFromList(potentialItems, 8);
					MinimumRequiredItems = Math.Min(Math.Max(RequiredItems.Count - 2, 3), RequiredItems.Count);
					Color = BundleColors.Red;
					break;
				case BundleTypes.BulletinWizard:
					Name = Globals.GetTranslation("Wizard-name");
					potentialItems = RequiredItem.CreateList(Wizard.Loves);
					RequiredItems = Globals.RNGGetRandomValuesFromList(potentialItems, 8);
					MinimumRequiredItems = Math.Min(Math.Max(RequiredItems.Count - 2, 3), RequiredItems.Count);
					Color = BundleColors.Purple;
					break;
				case BundleTypes.BulletinColorPink:
					Name = Globals.GetTranslation("bundle-bulletin-pink");
					potentialItems = RequiredItem.CreateList(new List<int>
					{
						(int)ObjectIndexes.Blobfish,
						(int)ObjectIndexes.VoidSalmon,
						(int)ObjectIndexes.Shrimp,
						(int)ObjectIndexes.StrangeBun,
						(int)ObjectIndexes.SalmonDinner,
						(int)ObjectIndexes.PinkCake,
						(int)ObjectIndexes.Sashimi,
						(int)ObjectIndexes.IceCream,
						(int)ObjectIndexes.Salmonberry,
						(int)ObjectIndexes.Coral,
						(int)ObjectIndexes.Dolomite,
						(int)ObjectIndexes.Nekoite,
						(int)ObjectIndexes.StarShards,
						(int)ObjectIndexes.Peach,
						(int)ObjectIndexes.BugMeat,
						(int)ObjectIndexes.Bait
					});
					RequiredItems = Globals.RNGGetRandomValuesFromList(potentialItems, 8);
					MinimumRequiredItems = Range.GetRandomValue(3, 6);
					Color = BundleColors.Red;
					break;
				case BundleTypes.BulletinColorWhite:
					Name = Globals.GetTranslation("bundle-bulletin-white");
					potentialItems = RequiredItem.CreateList(new List<int>
					{
						(int)ObjectIndexes.Leek,
						(int)ObjectIndexes.Quartz,
						(int)ObjectIndexes.OrnamentalFan,
						(int)ObjectIndexes.DinosaurEgg,
						(int)ObjectIndexes.ChickenStatue,
						(int)ObjectIndexes.Ghostfish,
						(int)ObjectIndexes.WhiteAlgae,
						(int)ObjectIndexes.WhiteEgg,
						(int)ObjectIndexes.LargeWhiteEgg,
						(int)ObjectIndexes.Milk,
						(int)ObjectIndexes.LargeMilk,
						(int)ObjectIndexes.FriedEgg,
						(int)ObjectIndexes.RicePudding,
						(int)ObjectIndexes.IceCream,
						(int)ObjectIndexes.Mayonnaise,
						(int)ObjectIndexes.IronBar,
						(int)ObjectIndexes.RefinedQuartz,
						(int)ObjectIndexes.IronOre,
						(int)ObjectIndexes.SpringOnion,
						(int)ObjectIndexes.SnowYam,
						(int)ObjectIndexes.Rice,
						(int)ObjectIndexes.GoatCheese,
						(int)ObjectIndexes.Cloth,
						(int)ObjectIndexes.GoatMilk,
						(int)ObjectIndexes.LargeGoatMilk,
						(int)ObjectIndexes.Wool,
						(int)ObjectIndexes.DuckEgg,
						(int)ObjectIndexes.RabbitsFoot,
						(int)ObjectIndexes.PaleBroth,
						(int)ObjectIndexes.Esperite,
						(int)ObjectIndexes.Lunarite,
						(int)ObjectIndexes.Marble,
						(int)ObjectIndexes.PrehistoricScapula,
						(int)ObjectIndexes.PrehistoricTibia,
						(int)ObjectIndexes.PrehistoricSkull,
						(int)ObjectIndexes.SkeletalHand,
						(int)ObjectIndexes.PrehistoricRib,
						(int)ObjectIndexes.PrehistoricVertebra,
						(int)ObjectIndexes.SkeletalTail,
						(int)ObjectIndexes.NautilusFossil,
						(int)ObjectIndexes.Trilobite,
						(int)ObjectIndexes.ArtichokeDip,
						(int)ObjectIndexes.LeadBobber,
						(int)ObjectIndexes.Chowder
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
				Reward = new RequiredItem((int)ObjectIndexes.PrismaticShard);
			}

			else if (Globals.RNGGetNextBoolean(5))
			{
				List<Item> universalLoves = NPC.UniversalLoves.Where(x =>
					x.Id != (int)ObjectIndexes.PrismaticShard).ToList();

				Reward = Globals.RNGGetRandomValueFromList(RequiredItem.CreateList(universalLoves, 5, 10));
			}

			List<RequiredItem> potentialRewards = new List<RequiredItem>
			{
				new RequiredItem((int)ObjectIndexes.JunimoKartArcadeSystem),
				new RequiredItem((int)ObjectIndexes.PrairieKingArcadeSystem),
				new RequiredItem((int)ObjectIndexes.SodaMachine),
				new RequiredItem((int)ObjectIndexes.Beer, 43),
				new RequiredItem((int)ObjectIndexes.Salad, Range.GetRandomValue(5, 25)),
				new RequiredItem((int)ObjectIndexes.Bread, Range.GetRandomValue(5, 25)),
				new RequiredItem((int)ObjectIndexes.Spaghetti, Range.GetRandomValue(5, 25)),
				new RequiredItem((int)ObjectIndexes.Pizza, Range.GetRandomValue(5, 25)),
				new RequiredItem((int)ObjectIndexes.Coffee, Range.GetRandomValue(5, 25))
			};

			Reward = Globals.RNGGetRandomValueFromList(potentialRewards);
		}
	}
}
