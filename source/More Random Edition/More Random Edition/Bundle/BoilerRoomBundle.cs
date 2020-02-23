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
					Name = Globals.GetTranslation("bundle-boiler-artifacts");
					potentialItems = RequiredItem.CreateList(
						ItemList.GetArtifacts().Where(x => x.DifficultyToObtain < ObtainingDifficulties.RareItem).ToList()
					);
					RequiredItems = Globals.RNGGetRandomValuesFromList(potentialItems, 8);
					MinimumRequiredItems = 3;
					Color = BundleColors.Orange;
					break;
				case BundleTypes.BoilerMinerals:
					Name = Globals.GetTranslation("bundle-boiler-minerals");
					potentialItems = RequiredItem.CreateList(ItemList.GetGeodeMinerals());
					RequiredItems = Globals.RNGGetRandomValuesFromList(potentialItems, 8);
					MinimumRequiredItems = Range.GetRandomValue(4, 6);
					Color = BundleColors.Purple;
					break;
				case BundleTypes.BoilerGeode:
					Name = Globals.GetTranslation("bundle-boiler-geode");
					RequiredItems = new List<RequiredItem>
					{
						new RequiredItem((int)ObjectIndexes.Geode, 1, 10),
						new RequiredItem((int)ObjectIndexes.FrozenGeode, 1, 10),
						new RequiredItem((int)ObjectIndexes.MagmaGeode, 1, 10),
						new RequiredItem((int)ObjectIndexes.OmniGeode, 1, 10),
					};
					Color = BundleColors.Red;
					break;
				case BundleTypes.BoilerGemstone:
					Name = Globals.GetTranslation("bundle-boiler-gemstone");
					potentialItems = RequiredItem.CreateList(new List<int>
					{
						(int)ObjectIndexes.Quartz,
						(int)ObjectIndexes.FireQuartz,
						(int)ObjectIndexes.EarthCrystal,
						(int)ObjectIndexes.FrozenTear,
						(int)ObjectIndexes.Aquamarine,
						(int)ObjectIndexes.Amethyst,
						(int)ObjectIndexes.Emerald,
						(int)ObjectIndexes.Ruby,
						(int)ObjectIndexes.Topaz,
						(int)ObjectIndexes.Jade,
						(int)ObjectIndexes.Diamond
					}, 1, 5);
					RequiredItems = Globals.RNGGetRandomValuesFromList(potentialItems, Range.GetRandomValue(6, 8));
					MinimumRequiredItems = Range.GetRandomValue(RequiredItems.Count - 2, RequiredItems.Count);
					Color = BundleColors.Blue;
					break;
				case BundleTypes.BoilerMetal:
					Name = Globals.GetTranslation("bundle-boiler-metal");
					potentialItems = new List<RequiredItem>
					{
						new RequiredItem((int)ObjectIndexes.CopperOre, 5, 10),
						new RequiredItem((int)ObjectIndexes.IronOre, 5, 10),
						new RequiredItem((int)ObjectIndexes.GoldOre, 5, 10),
						new RequiredItem((int)ObjectIndexes.IridiumOre, 5, 10),
						new RequiredItem((int)ObjectIndexes.CopperBar, 1, 5),
						new RequiredItem((int)ObjectIndexes.IronBar, 1, 5),
						new RequiredItem((int)ObjectIndexes.GoldBar, 1, 5),
						new RequiredItem((int)ObjectIndexes.IridiumBar, 1, 5),
					};
					RequiredItems = Globals.RNGGetRandomValuesFromList(potentialItems, Range.GetRandomValue(6, 8));
					MinimumRequiredItems = Range.GetRandomValue(RequiredItems.Count - 2, RequiredItems.Count);
					Color = BundleColors.Red;
					break;
				case BundleTypes.BoilerExplosive:
					Name = Globals.GetTranslation("bundle-boiler-explosive");
					RequiredItems = new List<RequiredItem>
					{
						new RequiredItem((int)ObjectIndexes.CherryBomb, 1, 5),
						new RequiredItem((int)ObjectIndexes.Bomb, 1, 5),
						new RequiredItem((int)ObjectIndexes.MegaBomb, 1, 5),
					};
					Color = BundleColors.Red;
					break;
				case BundleTypes.BoilerRing:
					Name = Globals.GetTranslation("bundle-boiler-ring");
					RequiredItems = RequiredItem.CreateList(
						Globals.RNGGetRandomValuesFromList(ItemList.GetRings(), 8)
					);
					MinimumRequiredItems = Range.GetRandomValue(4, 6);
					Color = BundleColors.Yellow;
					break;
				case BundleTypes.BoilerSpoopy:
					Name = Globals.GetTranslation("bundle-boiler-spoopy");
					potentialItems = new List<RequiredItem>
					{
						new RequiredItem((int)ObjectIndexes.Pumpkin, 6),
						new RequiredItem((int)ObjectIndexes.JackOLantern, 6),
						new RequiredItem((int)ObjectIndexes.Ghostfish, 6),
						new RequiredItem((int)ObjectIndexes.BatWing, 6),
						new RequiredItem((int)ObjectIndexes.VoidEssence, 6),
						new RequiredItem((int)ObjectIndexes.VoidEgg, 6),
						new RequiredItem((int)ObjectIndexes.PurpleMushroom, 6),
						new RequiredItem((int)ObjectIndexes.GhostCrystal, 6),
						new RequiredItem((int)ObjectIndexes.SpookFish, 6)
					};
					RequiredItems = Globals.RNGGetRandomValuesFromList(potentialItems, 6);
					MinimumRequiredItems = 3;
					Color = BundleColors.Purple;
					break;
				case BundleTypes.BoilerMonster:
					Name = Globals.GetTranslation("bundle-boiler-monster");
					RequiredItems = new List<RequiredItem>()
					{
						new RequiredItem((int)ObjectIndexes.BugMeat, 10, 50),
						new RequiredItem((int)ObjectIndexes.Slime, 10, 50),
						new RequiredItem((int)ObjectIndexes.BatWing, 10, 50),
						new RequiredItem((int)ObjectIndexes.SolarEssence, 10, 50),
						new RequiredItem((int)ObjectIndexes.VoidEssence, 10, 50)
					};
					Color = BundleColors.Red;
					break;
				case BundleTypes.BoilerColorBlack:
					Name = Globals.GetTranslation("bundle-boiler-black");
					potentialItems = RequiredItem.CreateList(new List<int>
					{
						(int)ObjectIndexes.RareDisc,
						(int)ObjectIndexes.Catfish,
						(int)ObjectIndexes.ScorpionCarp,
						(int)ObjectIndexes.TigerTrout,
						(int)ObjectIndexes.Halibut,
						(int)ObjectIndexes.MakiRoll,
						(int)ObjectIndexes.Bomb,
						(int)ObjectIndexes.VoidEgg,
						(int)ObjectIndexes.VoidMayonnaise,
						(int)ObjectIndexes.Coal,
						(int)ObjectIndexes.Blackberry,
						(int)ObjectIndexes.VampireRing,
						(int)ObjectIndexes.Neptunite,
						(int)ObjectIndexes.ThunderEgg,
						(int)ObjectIndexes.BatWing,
						(int)ObjectIndexes.VoidEssence,
					});
					RequiredItems = Globals.RNGGetRandomValuesFromList(potentialItems, 8);
					MinimumRequiredItems = Range.GetRandomValue(3, 6);
					Color = BundleColors.Purple;
					break;
				case BundleTypes.BoilerColorRed:
					Name = Globals.GetTranslation("bundle-boiler-red");
					potentialItems = RequiredItem.CreateList(new List<int>
					{
						(int)ObjectIndexes.Ruby,
						(int)ObjectIndexes.FireQuartz,
						(int)ObjectIndexes.DwarfScrollI,
						(int)ObjectIndexes.RedMullet,
						(int)ObjectIndexes.RedSnapper,
						(int)ObjectIndexes.LavaEel,
						(int)ObjectIndexes.Bullhead,
						(int)ObjectIndexes.Woodskip,
						(int)ObjectIndexes.Crimsonfish,
						(int)ObjectIndexes.PepperPoppers,
						(int)ObjectIndexes.RhubarbPie,
						(int)ObjectIndexes.RedPlate,
						(int)ObjectIndexes.CranberrySauce,
						(int)ObjectIndexes.Holly,
						(int)ObjectIndexes.CherryBomb,
						(int)ObjectIndexes.MegaBomb,
						(int)ObjectIndexes.Jelly,
						(int)ObjectIndexes.EnergyTonic,
						(int)ObjectIndexes.RedMushroom,
						(int)ObjectIndexes.RedSlimeEgg,
						(int)ObjectIndexes.ExplosiveAmmo,
						(int)ObjectIndexes.RubyRing,
						(int)ObjectIndexes.MagmaGeode,
						(int)ObjectIndexes.Helvite,
						(int)ObjectIndexes.Jasper,
						(int)ObjectIndexes.RadishSalad,
						(int)ObjectIndexes.FruitSalad,
						(int)ObjectIndexes.CranberryCandy,
						(int)ObjectIndexes.Apple,
						(int)ObjectIndexes.Pomegranate,
						(int)ObjectIndexes.Cherry,
						(int)ObjectIndexes.TreasureHunter,
						(int)ObjectIndexes.CrabPot,
						(int)ObjectIndexes.LifeElixir
					});
					RequiredItems = Globals.RNGGetRandomValuesFromList(potentialItems, 8);
					MinimumRequiredItems = Range.GetRandomValue(3, 6);
					Color = BundleColors.Red;
					break;
				case BundleTypes.BoilerColorGray:
					Name = Globals.GetTranslation("bundle-boiler-gray");
					potentialItems = RequiredItem.CreateList(new List<int>
					{
						(int)ObjectIndexes.Stone,
						(int)ObjectIndexes.Arrowhead,
						(int)ObjectIndexes.AncientSword,
						(int)ObjectIndexes.RustySpoon,
						(int)ObjectIndexes.PrehistoricTool,
						(int)ObjectIndexes.Anchor,
						(int)ObjectIndexes.PrehistoricHandaxe,
						(int)ObjectIndexes.DwarfGadget,
						(int)ObjectIndexes.Tilapia,
						(int)ObjectIndexes.Chub,
						(int)ObjectIndexes.Lingcod,
						(int)ObjectIndexes.Crayfish,
						(int)ObjectIndexes.Cockle,
						(int)ObjectIndexes.Mussel,
						(int)ObjectIndexes.Oyster,
						(int)ObjectIndexes.Trash,
						(int)ObjectIndexes.SoggyNewspaper,
						(int)ObjectIndexes.StoneFence,
						(int)ObjectIndexes.IronFence,
						(int)ObjectIndexes.StoneFloor,
						(int)ObjectIndexes.CrystalFloor,
						(int)ObjectIndexes.TeaSet,
						(int)ObjectIndexes.GravelPath,
						(int)ObjectIndexes.MagnetRing,
						(int)ObjectIndexes.SmallMagnetRing,
						(int)ObjectIndexes.WarriorRing,
						(int)ObjectIndexes.SavageRing,
						(int)ObjectIndexes.Bixite,
						(int)ObjectIndexes.Granite,
						(int)ObjectIndexes.Basalt,
						(int)ObjectIndexes.Limestone,
						(int)ObjectIndexes.Sprinkler,
						(int)ObjectIndexes.BarbedHook,
						(int)ObjectIndexes.TrapBobber,
						(int)ObjectIndexes.OmniGeode,
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
				new RequiredItem((int)ObjectIndexes.CopperOre, 100),
				new RequiredItem((int)ObjectIndexes.IronOre, 100),
				new RequiredItem((int)ObjectIndexes.GoldOre, 100),
				new RequiredItem((int)ObjectIndexes.IridiumOre, 10),
			});

			RequiredItem randomBar = Globals.RNGGetRandomValueFromList(new List<RequiredItem>()
			{
				new RequiredItem((int)ObjectIndexes.CopperBar, 15),
				new RequiredItem((int)ObjectIndexes.IronBar, 15),
				new RequiredItem((int)ObjectIndexes.GoldBar, 15),
				new RequiredItem((int)ObjectIndexes.IridiumBar)
			});

			RequiredItem randomGeode = Globals.RNGGetRandomValueFromList(new List<RequiredItem>()
			{
				new RequiredItem((int)ObjectIndexes.Geode, 25),
				new RequiredItem((int)ObjectIndexes.FrozenGeode, 25),
				new RequiredItem((int)ObjectIndexes.MagmaGeode, 25),
				new RequiredItem((int)ObjectIndexes.OmniGeode, 25)
			});

			RequiredItem randomMonsterDrop = Globals.RNGGetRandomValueFromList(new List<RequiredItem>()
			{
				new RequiredItem((int)ObjectIndexes.BugMeat, 200),
				new RequiredItem((int)ObjectIndexes.Slime, 150),
				new RequiredItem((int)ObjectIndexes.BatWing, 100),
				new RequiredItem((int)ObjectIndexes.SolarEssence, 50),
				new RequiredItem((int)ObjectIndexes.VoidEssence, 50)
			});

			RequiredItem randomExplosive = Globals.RNGGetRandomValueFromList(new List<RequiredItem>()
			{
				new RequiredItem((int)ObjectIndexes.CherryBomb, 25, 50),
				new RequiredItem((int)ObjectIndexes.Bomb, 25, 50),
				new RequiredItem((int)ObjectIndexes.MegaBomb, 25, 50)
			});

			RequiredItem randomGemstone = Globals.RNGGetRandomValueFromList(new List<RequiredItem>()
			{
				new RequiredItem((int)ObjectIndexes.Quartz, 25, 50),
				new RequiredItem((int)ObjectIndexes.FireQuartz, 25, 50),
				new RequiredItem((int)ObjectIndexes.EarthCrystal, 25, 50),
				new RequiredItem((int)ObjectIndexes.FrozenTear, 25, 50),
				new RequiredItem((int)ObjectIndexes.Aquamarine, 25, 50),
				new RequiredItem((int)ObjectIndexes.Amethyst, 25, 50),
				new RequiredItem((int)ObjectIndexes.Emerald, 25, 50),
				new RequiredItem((int)ObjectIndexes.Ruby, 25, 50),
				new RequiredItem((int)ObjectIndexes.Topaz, 25, 50),
				new RequiredItem((int)ObjectIndexes.Jade, 25, 50),
				new RequiredItem((int)ObjectIndexes.Diamond, 10, 30),
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
				new RequiredItem((int)ObjectIndexes.Crystalarium),
				new RequiredItem((int)ObjectIndexes.MayonnaiseMachine),
				new RequiredItem((int)ObjectIndexes.Coal, 100)
			};

			if (Globals.RNGGetNextBoolean(1)) // 1% chance of a prismatic shard reward
			{
				Reward = new RequiredItem((int)ObjectIndexes.PrismaticShard);
			}

			else
			{
				Reward = Globals.RNGGetRandomValueFromList(potentialRewards);
			}
		}
	}
}
