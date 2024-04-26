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
            RNG rng = BundleRandomizer.Rng;

            BundleType = rng.GetAndRemoveRandomValueFromList(RoomBundleTypes);
			List<RequiredBundleItem> potentialItems = new();

			switch (BundleType)
			{
				case BundleTypes.BoilerArtifacts:
					SetBundleName("bundle-boiler-artifacts");
					potentialItems = RequiredBundleItem.CreateList(
						ItemList.GetArtifacts().Where(x => x.DifficultyToObtain < ObtainingDifficulties.RareItem).ToList()
					);
					RequiredItems = rng.GetRandomValuesFromList(potentialItems, 8);
					MinimumRequiredItems = 3;
					Color = BundleColors.Orange;
					break;
				case BundleTypes.BoilerMinerals:
					SetBundleName("bundle-boiler-minerals");
					potentialItems = RequiredBundleItem.CreateList(ItemList.GetGeodeMinerals());
					RequiredItems = rng.GetRandomValuesFromList(potentialItems, 8);
					MinimumRequiredItems = rng.NextIntWithinRange(4, 6);
					Color = BundleColors.Purple;
					break;
				case BundleTypes.BoilerGeode:
					SetBundleName("bundle-boiler-geode");
					RequiredItems = new List<RequiredBundleItem>
					{
						new(ObjectIndexes.Geode, 1, 10),
						new(ObjectIndexes.FrozenGeode, 1, 10),
						new(ObjectIndexes.MagmaGeode, 1, 10),
						new(ObjectIndexes.OmniGeode, 1, 10),
					};
					Color = BundleColors.Red;
					break;
				case BundleTypes.BoilerGemstone:
					SetBundleName("bundle-boiler-gemstone");
					potentialItems = RequiredBundleItem.CreateList(new List<ObjectIndexes>
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
					RequiredItems = rng.GetRandomValuesFromList(potentialItems, rng.NextIntWithinRange(6, 8));
					MinimumRequiredItems = rng.NextIntWithinRange(RequiredItems.Count - 2, RequiredItems.Count);
					Color = BundleColors.Blue;
					break;
				case BundleTypes.BoilerMetal:
					SetBundleName("bundle-boiler-metal");
					potentialItems = new List<RequiredBundleItem>
					{
						new(ObjectIndexes.CopperOre, 5, 10),
						new(ObjectIndexes.IronOre, 5, 10),
						new(ObjectIndexes.GoldOre, 5, 10),
						new(ObjectIndexes.IridiumOre, 5, 10),
						new(ObjectIndexes.CopperBar, 1, 5),
						new(ObjectIndexes.IronBar, 1, 5),
						new(ObjectIndexes.GoldBar, 1, 5),
						new(ObjectIndexes.IridiumBar, 1, 5),
					};
					RequiredItems = rng.GetRandomValuesFromList(potentialItems, rng.NextIntWithinRange(6, 8));
					MinimumRequiredItems = rng.NextIntWithinRange(RequiredItems.Count - 2, RequiredItems.Count);
					Color = BundleColors.Red;
					break;
				case BundleTypes.BoilerExplosive:
					SetBundleName("bundle-boiler-explosive");
					RequiredItems = new List<RequiredBundleItem>
					{
						new RequiredBundleItem(ObjectIndexes.CherryBomb, 1, 5),
						new RequiredBundleItem(ObjectIndexes.Bomb, 1, 5),
						new RequiredBundleItem(ObjectIndexes.MegaBomb, 1, 5),
					};
					Color = BundleColors.Red;
					break;
				case BundleTypes.BoilerRing:
					SetBundleName("bundle-boiler-ring");
					RequiredItems = RequiredBundleItem.CreateList(
						rng.GetRandomValuesFromList(ItemList.GetRings(), 8)
					);
					MinimumRequiredItems = rng.NextIntWithinRange(4, 6);
					Color = BundleColors.Yellow;
					break;
				case BundleTypes.BoilerSpoopy:
					SetBundleName("bundle-boiler-spoopy");
					potentialItems = new List<RequiredBundleItem>
					{
						new RequiredBundleItem(ObjectIndexes.Pumpkin, 6),
						new RequiredBundleItem(ObjectIndexes.JackOLantern, 6),
						new RequiredBundleItem(ObjectIndexes.Ghostfish, 6),
						new RequiredBundleItem(ObjectIndexes.BatWing, 6),
						new RequiredBundleItem(ObjectIndexes.VoidEssence, 6),
						new RequiredBundleItem(ObjectIndexes.VoidEgg, 6),
						new RequiredBundleItem(ObjectIndexes.PurpleMushroom, 6),
						new RequiredBundleItem(ObjectIndexes.GhostCrystal, 6),
						new RequiredBundleItem(ObjectIndexes.SpookFish, 6)
					};
					RequiredItems = rng.GetRandomValuesFromList(potentialItems, 6);
					MinimumRequiredItems = 3;
					Color = BundleColors.Purple;
					break;
				case BundleTypes.BoilerMonster:
					SetBundleName("bundle-boiler-monster");
					RequiredItems = new List<RequiredBundleItem>()
					{
						new RequiredBundleItem(ObjectIndexes.BugMeat, 10, 50),
						new RequiredBundleItem(ObjectIndexes.Slime, 10, 50),
						new RequiredBundleItem(ObjectIndexes.BatWing, 10, 50),
						new RequiredBundleItem(ObjectIndexes.SolarEssence, 10, 50),
						new RequiredBundleItem(ObjectIndexes.VoidEssence, 10, 50)
					};
					Color = BundleColors.Red;
					break;
				case BundleTypes.BoilerColorBlack:
					SetBundleName("bundle-boiler-black");
					potentialItems = RequiredBundleItem.CreateList(new List<ObjectIndexes>
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
					RequiredItems = rng.GetRandomValuesFromList(potentialItems, 8);
					MinimumRequiredItems = rng.NextIntWithinRange(3, 6);
					Color = BundleColors.Purple;
					break;
				case BundleTypes.BoilerColorRed:
					SetBundleName("bundle-boiler-red");
					potentialItems = RequiredBundleItem.CreateList(new List<ObjectIndexes>
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
					RequiredItems = rng.GetRandomValuesFromList(potentialItems, 8);
					MinimumRequiredItems = rng.NextIntWithinRange(3, 6);
					Color = BundleColors.Red;
					break;
				case BundleTypes.BoilerColorGray:
					SetBundleName("bundle-boiler-gray");
					potentialItems = RequiredBundleItem.CreateList(new List<ObjectIndexes>
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
					RequiredItems = rng.GetRandomValuesFromList(potentialItems, 8);
					MinimumRequiredItems = rng.NextIntWithinRange(3, 6);
					Color = BundleColors.Blue;
					break;
			}
		}

		/// <summary>
		/// Generates the reward for the bundle
		/// </summary>
		protected override void GenerateReward()
		{
            RNG rng = BundleRandomizer.Rng;

            RequiredBundleItem randomOre = rng.GetRandomValueFromList(new List<RequiredBundleItem>()
			{
				new(ObjectIndexes.CopperOre, 100),
				new(ObjectIndexes.IronOre, 100),
				new(ObjectIndexes.GoldOre, 100),
				new(ObjectIndexes.IridiumOre, 10),
			});

			RequiredBundleItem randomBar = rng.GetRandomValueFromList(new List<RequiredBundleItem>()
			{
				new(ObjectIndexes.CopperBar, 15),
				new(ObjectIndexes.IronBar, 15),
				new(ObjectIndexes.GoldBar, 15),
				new(ObjectIndexes.IridiumBar)
			});

			RequiredBundleItem randomGeode = rng.GetRandomValueFromList(new List<RequiredBundleItem>()
			{
				new(ObjectIndexes.Geode, 25),
				new(ObjectIndexes.FrozenGeode, 25),
				new(ObjectIndexes.MagmaGeode, 25),
				new(ObjectIndexes.OmniGeode, 25)
			});

			RequiredBundleItem randomMonsterDrop = rng.GetRandomValueFromList(new List<RequiredBundleItem>()
			{
				new(ObjectIndexes.BugMeat, 200),
				new(ObjectIndexes.Slime, 150),
				new(ObjectIndexes.BatWing, 100),
				new(ObjectIndexes.BoneFragment, 100),
				new(ObjectIndexes.SolarEssence, 50),
				new(ObjectIndexes.VoidEssence, 50),
			});

			RequiredBundleItem randomExplosive = rng.GetRandomValueFromList(new List<RequiredBundleItem>()
			{
				new(ObjectIndexes.CherryBomb, 25, 50),
				new(ObjectIndexes.Bomb, 25, 50),
				new(ObjectIndexes.MegaBomb, 25, 50)
			});

			RequiredBundleItem randomGemstone = rng.GetRandomValueFromList(new List<RequiredBundleItem>()
			{
				new(ObjectIndexes.Quartz, 25, 50),
				new(ObjectIndexes.FireQuartz, 25, 50),
				new(ObjectIndexes.EarthCrystal, 25, 50),
				new(ObjectIndexes.FrozenTear, 25, 50),
				new(ObjectIndexes.Aquamarine, 25, 50),
				new(ObjectIndexes.Amethyst, 25, 50),
				new(ObjectIndexes.Emerald, 25, 50),
				new(ObjectIndexes.Ruby, 25, 50),
				new(ObjectIndexes.Topaz, 25, 50),
				new(ObjectIndexes.Jade, 25, 50),
				new(ObjectIndexes.Diamond, 10, 30),
			});

			var potentialRewards = new List<RequiredBundleItem>
			{
				randomOre,
				randomBar,
				randomGeode,
				randomMonsterDrop,
				randomExplosive,
				randomGemstone,
				new(rng.GetRandomValueFromList(ItemList.GetGeodeMinerals()), 25),
				new(rng.GetRandomValueFromList(ItemList.GetArtifacts())),
				new(rng.GetRandomValueFromList(ItemList.GetRings())),
				new(BigCraftableIndexes.Crystalarium),
				new(BigCraftableIndexes.MayonnaiseMachine),
				new(ObjectIndexes.Coal, 100)
			};

            // 1% chance of a prismatic shard reward
            Reward = rng.NextBoolean(1)
				? new RequiredBundleItem(ObjectIndexes.PrismaticShard)
				: rng.GetRandomValueFromList(potentialRewards);
		}
	}
}
