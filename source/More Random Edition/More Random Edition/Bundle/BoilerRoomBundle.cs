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
	public class BoilerRoomBundle : Bundle
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
				case BundleTypes.BoilerArtifacts:
					SetBundleName("bundle-boiler-artifacts");
					potentialItems = RequiredItem.CreateList(
						ItemList.GetArtifacts().Where(x => x.DifficultyToObtain < ObtainingDifficulties.RareItem).ToList()
					);
					RequiredItems = Globals.RNGGetRandomValuesFromList(potentialItems, 8);
					MinimumRequiredItems = 3;
					Color = BundleColors.Orange;
					break;
				case BundleTypes.BoilerMinerals:
					SetBundleName("bundle-boiler-minerals");
					potentialItems = RequiredItem.CreateList(ItemList.GetGeodeMinerals());
					RequiredItems = Globals.RNGGetRandomValuesFromList(potentialItems, 8);
					MinimumRequiredItems = Range.GetRandomValue(4, 6);
					Color = BundleColors.Purple;
					break;
				case BundleTypes.BoilerGeode:
					SetBundleName("bundle-boiler-geode");
					RequiredItems = new List<RequiredItem>
					{
						new RequiredItem(ObjectIndexes.Geode, 1, 10),
						new RequiredItem(ObjectIndexes.FrozenGeode, 1, 10),
						new RequiredItem(ObjectIndexes.MagmaGeode, 1, 10),
						new RequiredItem(ObjectIndexes.OmniGeode, 1, 10),
					};
					Color = BundleColors.Red;
					break;
				case BundleTypes.BoilerGemstone:
					SetBundleName("bundle-boiler-gemstone");
					potentialItems = RequiredItem.CreateList(new List<ObjectIndexes>
					{
						ObjectIndexes.Quartz,
						ObjectIndexes.FireQuartz,
						ObjectIndexes.EarthCrystal,
						ObjectIndexes.FrozenTear,
						ObjectIndexes.Aquamarine,
						ObjectIndexes.Amethyst,
						ObjectIndexes.Emerald,
						ObjectIndexes.Ruby,
						ObjectIndexes.Topaz,
						ObjectIndexes.Jade,
						ObjectIndexes.Diamond
					}, 1, 5);
					RequiredItems = Globals.RNGGetRandomValuesFromList(potentialItems, Range.GetRandomValue(6, 8));
					MinimumRequiredItems = Range.GetRandomValue(RequiredItems.Count - 2, RequiredItems.Count);
					Color = BundleColors.Blue;
					break;
				case BundleTypes.BoilerMetal:
					SetBundleName("bundle-boiler-metal");
					potentialItems = new List<RequiredItem>
					{
						new RequiredItem(ObjectIndexes.CopperOre, 5, 10),
						new RequiredItem(ObjectIndexes.IronOre, 5, 10),
						new RequiredItem(ObjectIndexes.GoldOre, 5, 10),
						new RequiredItem(ObjectIndexes.IridiumOre, 5, 10),
						new RequiredItem(ObjectIndexes.CopperBar, 1, 5),
						new RequiredItem(ObjectIndexes.IronBar, 1, 5),
						new RequiredItem(ObjectIndexes.GoldBar, 1, 5),
						new RequiredItem(ObjectIndexes.IridiumBar, 1, 5),
					};
					RequiredItems = Globals.RNGGetRandomValuesFromList(potentialItems, Range.GetRandomValue(6, 8));
					MinimumRequiredItems = Range.GetRandomValue(RequiredItems.Count - 2, RequiredItems.Count);
					Color = BundleColors.Red;
					break;
				case BundleTypes.BoilerExplosive:
					SetBundleName("bundle-boiler-explosive");
					RequiredItems = new List<RequiredItem>
					{
						new RequiredItem(ObjectIndexes.CherryBomb, 1, 5),
						new RequiredItem(ObjectIndexes.Bomb, 1, 5),
						new RequiredItem(ObjectIndexes.MegaBomb, 1, 5),
					};
					Color = BundleColors.Red;
					break;
				case BundleTypes.BoilerRing:
					SetBundleName("bundle-boiler-ring");
					RequiredItems = RequiredItem.CreateList(
						Globals.RNGGetRandomValuesFromList(ItemList.GetRings(), 8)
					);
					MinimumRequiredItems = Range.GetRandomValue(4, 6);
					Color = BundleColors.Yellow;
					break;
				case BundleTypes.BoilerSpoopy:
					SetBundleName("bundle-boiler-spoopy");
					potentialItems = new List<RequiredItem>
					{
						new RequiredItem(ObjectIndexes.Pumpkin, 6),
						new RequiredItem(ObjectIndexes.JackOLantern, 6),
						new RequiredItem(ObjectIndexes.Ghostfish, 6),
						new RequiredItem(ObjectIndexes.BatWing, 6),
						new RequiredItem(ObjectIndexes.VoidEssence, 6),
						new RequiredItem(ObjectIndexes.VoidEgg, 6),
						new RequiredItem(ObjectIndexes.PurpleMushroom, 6),
						new RequiredItem(ObjectIndexes.GhostCrystal, 6),
						new RequiredItem(ObjectIndexes.SpookFish, 6)
					};
					RequiredItems = Globals.RNGGetRandomValuesFromList(potentialItems, 6);
					MinimumRequiredItems = 3;
					Color = BundleColors.Purple;
					break;
				case BundleTypes.BoilerMonster:
					SetBundleName("bundle-boiler-monster");
					RequiredItems = new List<RequiredItem>()
					{
						new RequiredItem(ObjectIndexes.BugMeat, 10, 50),
						new RequiredItem(ObjectIndexes.Slime, 10, 50),
						new RequiredItem(ObjectIndexes.BatWing, 10, 50),
						new RequiredItem(ObjectIndexes.SolarEssence, 10, 50),
						new RequiredItem(ObjectIndexes.VoidEssence, 10, 50)
					};
					Color = BundleColors.Red;
					break;
				case BundleTypes.BoilerColorBlack:
					SetBundleName("bundle-boiler-black");
					potentialItems = RequiredItem.CreateList(new List<ObjectIndexes>
					{
						ObjectIndexes.RareDisc,
						ObjectIndexes.MakiRoll,
						ObjectIndexes.Bomb,
						ObjectIndexes.VoidEgg,
						ObjectIndexes.VoidMayonnaise,
						ObjectIndexes.Coal,
						ObjectIndexes.Blackberry,
						ObjectIndexes.VampireRing,
						ObjectIndexes.Neptunite,
						ObjectIndexes.ThunderEgg,
						ObjectIndexes.BatWing,
						ObjectIndexes.VoidEssence,
					});
					RequiredItems = Globals.RNGGetRandomValuesFromList(potentialItems, 8);
					MinimumRequiredItems = Range.GetRandomValue(3, 6);
					Color = BundleColors.Purple;
					break;
				case BundleTypes.BoilerColorRed:
					SetBundleName("bundle-boiler-red");
					potentialItems = RequiredItem.CreateList(new List<ObjectIndexes>
					{
						ObjectIndexes.Ruby,
						ObjectIndexes.FireQuartz,
						ObjectIndexes.DwarfScrollI,
						ObjectIndexes.PepperPoppers,
						ObjectIndexes.RhubarbPie,
						ObjectIndexes.RedPlate,
						ObjectIndexes.CranberrySauce,
						ObjectIndexes.Holly,
						ObjectIndexes.CherryBomb,
						ObjectIndexes.MegaBomb,
						ObjectIndexes.Jelly,
						ObjectIndexes.EnergyTonic,
						ObjectIndexes.RedMushroom,
						ObjectIndexes.RedSlimeEgg,
						ObjectIndexes.ExplosiveAmmo,
						ObjectIndexes.RubyRing,
						ObjectIndexes.MagmaGeode,
						ObjectIndexes.Helvite,
						ObjectIndexes.Jasper,
						ObjectIndexes.RadishSalad,
						ObjectIndexes.FruitSalad,
						ObjectIndexes.CranberryCandy,
						ObjectIndexes.Apple,
						ObjectIndexes.Pomegranate,
						ObjectIndexes.Cherry,
						ObjectIndexes.TreasureHunter,
						ObjectIndexes.CrabPot,
						ObjectIndexes.LifeElixir
					});
					RequiredItems = Globals.RNGGetRandomValuesFromList(potentialItems, 8);
					MinimumRequiredItems = Range.GetRandomValue(3, 6);
					Color = BundleColors.Red;
					break;
				case BundleTypes.BoilerColorGray:
					SetBundleName("bundle-boiler-gray");
					potentialItems = RequiredItem.CreateList(new List<ObjectIndexes>
					{
						ObjectIndexes.Stone,
						ObjectIndexes.Arrowhead,
						ObjectIndexes.AncientSword,
						ObjectIndexes.RustySpoon,
						ObjectIndexes.PrehistoricTool,
						ObjectIndexes.Anchor,
						ObjectIndexes.PrehistoricHandaxe,
						ObjectIndexes.DwarfGadget,
						ObjectIndexes.Crayfish,
						ObjectIndexes.Cockle,
						ObjectIndexes.Mussel,
						ObjectIndexes.Oyster,
						ObjectIndexes.Trash,
						ObjectIndexes.SoggyNewspaper,
						ObjectIndexes.StoneFence,
						ObjectIndexes.IronFence,
						ObjectIndexes.StoneFloor,
						ObjectIndexes.CrystalFloor,
						ObjectIndexes.TeaSet,
						ObjectIndexes.GravelPath,
						ObjectIndexes.MagnetRing,
						ObjectIndexes.SmallMagnetRing,
						ObjectIndexes.WarriorRing,
						ObjectIndexes.SavageRing,
						ObjectIndexes.Bixite,
						ObjectIndexes.Granite,
						ObjectIndexes.Basalt,
						ObjectIndexes.Limestone,
						ObjectIndexes.Sprinkler,
						ObjectIndexes.BarbedHook,
						ObjectIndexes.TrapBobber,
						ObjectIndexes.OmniGeode,
					});
					RequiredItems = Globals.RNGGetRandomValuesFromList(potentialItems, 8);
					MinimumRequiredItems = Range.GetRandomValue(3, 6);
					Color = BundleColors.Blue;
					break;
			}
		}

		/// <summary>
		/// Generates the reward for the bundle
		/// </summary>
		protected override void GenerateReward()
		{
			RequiredItem randomOre = Globals.RNGGetRandomValueFromList(new List<RequiredItem>()
			{
				new RequiredItem(ObjectIndexes.CopperOre, 100),
				new RequiredItem(ObjectIndexes.IronOre, 100),
				new RequiredItem(ObjectIndexes.GoldOre, 100),
				new RequiredItem(ObjectIndexes.IridiumOre, 10),
			});

			RequiredItem randomBar = Globals.RNGGetRandomValueFromList(new List<RequiredItem>()
			{
				new RequiredItem(ObjectIndexes.CopperBar, 15),
				new RequiredItem(ObjectIndexes.IronBar, 15),
				new RequiredItem(ObjectIndexes.GoldBar, 15),
				new RequiredItem(ObjectIndexes.IridiumBar)
			});

			RequiredItem randomGeode = Globals.RNGGetRandomValueFromList(new List<RequiredItem>()
			{
				new RequiredItem(ObjectIndexes.Geode, 25),
				new RequiredItem(ObjectIndexes.FrozenGeode, 25),
				new RequiredItem(ObjectIndexes.MagmaGeode, 25),
				new RequiredItem(ObjectIndexes.OmniGeode, 25)
			});

			RequiredItem randomMonsterDrop = Globals.RNGGetRandomValueFromList(new List<RequiredItem>()
			{
				new RequiredItem(ObjectIndexes.BugMeat, 200),
				new RequiredItem(ObjectIndexes.Slime, 150),
				new RequiredItem(ObjectIndexes.BatWing, 100),
				new RequiredItem(ObjectIndexes.SolarEssence, 50),
				new RequiredItem(ObjectIndexes.VoidEssence, 50)
			});

			RequiredItem randomExplosive = Globals.RNGGetRandomValueFromList(new List<RequiredItem>()
			{
				new RequiredItem(ObjectIndexes.CherryBomb, 25, 50),
				new RequiredItem(ObjectIndexes.Bomb, 25, 50),
				new RequiredItem(ObjectIndexes.MegaBomb, 25, 50)
			});

			RequiredItem randomGemstone = Globals.RNGGetRandomValueFromList(new List<RequiredItem>()
			{
				new RequiredItem(ObjectIndexes.Quartz, 25, 50),
				new RequiredItem(ObjectIndexes.FireQuartz, 25, 50),
				new RequiredItem(ObjectIndexes.EarthCrystal, 25, 50),
				new RequiredItem(ObjectIndexes.FrozenTear, 25, 50),
				new RequiredItem(ObjectIndexes.Aquamarine, 25, 50),
				new RequiredItem(ObjectIndexes.Amethyst, 25, 50),
				new RequiredItem(ObjectIndexes.Emerald, 25, 50),
				new RequiredItem(ObjectIndexes.Ruby, 25, 50),
				new RequiredItem(ObjectIndexes.Topaz, 25, 50),
				new RequiredItem(ObjectIndexes.Jade, 25, 50),
				new RequiredItem(ObjectIndexes.Diamond, 10, 30),
			});

			var potentialRewards = new List<RequiredItem>
			{
				randomOre,
				randomBar,
				randomGeode,
				randomMonsterDrop,
				randomExplosive,
				randomGemstone,
				new RequiredItem(Globals.RNGGetRandomValueFromList(ItemList.GetGeodeMinerals()), 25),
				new RequiredItem(Globals.RNGGetRandomValueFromList(ItemList.GetArtifacts())),
				new RequiredItem(Globals.RNGGetRandomValueFromList(ItemList.GetRings())),
				new RequiredItem(BigCraftableIndexes.Crystalarium),
				new RequiredItem(BigCraftableIndexes.MayonnaiseMachine),
				new RequiredItem(ObjectIndexes.Coal, 100)
			};

			if (Globals.RNGGetNextBoolean(1)) // 1% chance of a prismatic shard reward
			{
				Reward = new RequiredItem(ObjectIndexes.PrismaticShard);
			}

			else
			{
				Reward = Globals.RNGGetRandomValueFromList(potentialRewards);
			}
		}
	}
}
