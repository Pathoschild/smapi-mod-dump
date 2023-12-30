/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/maxvollmer/DeepWoodsMod
**
*************************************************/


using StardewModdingAPI;
using System.Collections.Generic;
using xTile.Dimensions;
using static DeepWoodsMod.DeepWoodsRandom;
using static DeepWoodsMod.DeepWoodsGlobals;
using Newtonsoft.Json;
using StardewValley.Tools;
using StardewValley;
using System.IO;
using StardewValley.TerrainFeatures;

namespace DeepWoodsMod
{
    public struct SimpleCoord
    {
        public int X;
        public int Y;

        public SimpleCoord(int x, int y)
        {
            X = x;
            Y = y;
        }
    }

    public class WoodsPassageSettings
    {
        public SimpleCoord[] DeleteBuildingTiles { get; set; } = new SimpleCoord[] {
            new SimpleCoord(29, 25),
            new SimpleCoord(29, 26),
        };

        public SimpleCoord[] AddBuildingTiles { get; set; } = new SimpleCoord[] {
            new SimpleCoord(24, 24),
            new SimpleCoord(25, 24),
            new SimpleCoord(26, 24),
            new SimpleCoord(27, 24),
            new SimpleCoord(28, 24),
        };

        public SimpleCoord[] WarpLocations { get; set; } = new SimpleCoord[] {
            new SimpleCoord(24, 32),
            new SimpleCoord(25, 32),
            new SimpleCoord(26, 32),
            new SimpleCoord(27, 32),
            new SimpleCoord(28, 32),
        };

        public SimpleCoord WoodsWarpLocation { get; set; } = new SimpleCoord(26, 31);
    }

    public class MapSettings
    {
        public int MaxMapWidth { get; set; } = 64;
        public int MaxMapHeight { get; set; } = 64;
        public int MaxMapWidthForClearing { get; set; } = 32;

        public int MaxBumpSizeForForestBorder { get; set; } = 2;

        public int MinTilesForCorner { get; set; } = 3;
        public int MaxTilesForCorner { get; set; } = 8;

        public int ExitRadius { get; set; } = 2;
        public int ExitLength { get; set; } = 5;

        public int MinSizeForForestPatch { get; set; } = 12;
        public int MaxSizeForForestPatch { get; set; } = 24;

        [JsonIgnore]
        public int ForestPatchMinGapToMapBorder
        {
            get
            {
                return MaxBumpSizeForForestBorder * 2 + 2;
            }
        }

        [JsonIgnore]
        public int ForestPatchMinGapToEachOther
        {
            get
            {
                return MaxBumpSizeForForestBorder * 2;
            }
        }

        [JsonIgnore]
        public int ForestPatchCenterMinDistanceToMapBorder
        {
            get
            {
                return ForestPatchMinGapToMapBorder + MinSizeForForestPatch / 2;
            }
        }

        [JsonIgnore]
        public int ForestPatchCenterMinDistanceToEachOther
        {
            get
            {
                return ForestPatchMinGapToEachOther + MinSizeForForestPatch / 2;
            }
        }

        [JsonIgnore]
        public int MinCornerDistanceForEnterLocation
        {
            get
            {
                return MaxTilesForCorner + ExitRadius + 2;   // => 12
            }
        }

        [JsonIgnore]
        public int MinMapWidth
        {
            get
            {
                return MinCornerDistanceForEnterLocation * 2 + 4;   // => 28
            }
        }

        [JsonIgnore]
        public int MinMapHeight
        {
            get
            {
                return MinCornerDistanceForEnterLocation * 2 + 4;  // => 28
            }
        }

        [JsonIgnore]
        public int MinimumTilesForForestPatch
        {
            get
            {
                return MinSizeForForestPatch * MinSizeForForestPatch;
            }
        }

        [JsonIgnore]
        public Location RootLevelEnterLocation
        {
            get
            {
                return new Location(MinMapWidth / 2, 0);
            }
        }
    }

    public class GenerationSettings
    {
        public int NumTilesPerLightSource { get; set; } = 16;
        public int MinTilesForBauble { get; set; } = 16;
        public int MinTilesForLeaves { get; set; } = 16;
        public int MinTilesForMonsters { get; set; } = 16;
        public int MinTilesForTerrainFeature { get; set; } = 4;

        public Chance ChanceForWaterLily { get; set; } = new Chance(8);
        public Chance ChanceForBlossomOnWaterLily { get; set; } = new Chance(30);
    }

    public class LevelSettings
    {
        public int MinLevelForFlowers { get; set; } = 3;
        public int MinLevelForFruits { get; set; } = 5;
        public int MinLevelForSilverFruits { get; set; } = 25;
        public int MinLevelForGoldFruits { get; set; } = 75;
        public int MinLevelForIridiumFruits { get; set; } = 100;
        public int MinLevelForThornyBushes { get; set; } = 10;
        public int MinLevelForDangerousMonsters { get; set; } = 15;
        public int MinLevelForBuffedMonsters { get; set; } = 15;
        public int MinLevelForWoodsObelisk { get; set; } = 20;
        public int MinLevelForMeteorite { get; set; } = 25;
        public int MinLevelForClearing { get; set; } = 30;
        public int MinLevelForGingerbreadHouse { get; set; } = 50;
        public Chance ChanceForThornyBushesOnExit { get; set; } = new Chance(new LuckValue(50, 5));
        public Chance ChanceForSignOnExit { get; set; } = new Chance(new LuckValue(20, 30));
        public bool EnableGuaranteedClearings { get; set; } = true;
        public int GuaranteedClearingsFrequency { get; set; } = 50;
    }

    public class ExcaliburSettings
    {
        public int MinDamage { get; set; } = 120;
        public int MaxDamage { get; set; } = 180;
        public float Knockback { get; set; } = 1.5f;
        public int Speed { get; set; } = 10;
        public int Precision { get; set; } = 5;
        public int Defense { get; set; } = 5;
        public int AreaOfEffect { get; set; } = 5;
        public float CriticalChance { get; set; } = .05f;
        public float CriticalMultiplier { get; set; } = 5;

        public ExcaliburWorthinessSettings Worthiness { get; set; } = new ExcaliburWorthinessSettings();
    }

    public class ExcaliburWorthinessSettings
    {
        // The maximum possible is actually 0.125, but we use 0.07 as that's the threshold for max daily luck on the TV.
        public float MinimumDailyLuck { get; set; } = 0.07f;
        public int MinimumLuckLevel { get; set; } = 8;
        public int MinimumMiningLevel { get; set; } = 10;
        public int MinimumForagingLevel { get; set; } = 10;
        public int MinimumFishingLevel { get; set; } = 10;
        public int MinimumFarmingLevel { get; set; } = 10;
        public int MinimumCombatLevel { get; set; } = 10;
        public int MinimumGrandpaScore { get; set; } = 4;
        public bool MustHaveCompletedCommunityCenter { get; set; } = true;
        public bool MustHaveReachedMineBottom { get; set; } = true;
        public bool MustNotBeJojaMember { get; set; } = true;
    }

    public class GingerBreadHouseSettings
    {
        public int MinimumAxeLevel { get; set; } = Axe.gold;
        public int Health { get; set; } = 200;
        public int DamageIntervalForFoodDrop { get; set; } = 20;

        public WeightedInt[] FootItems { get; set; } = new WeightedInt[] {
            // ITEM NAME // SELL PRICE // WEIGHT
            GingerBreadHouse.CreateWeightedValueForFootType(245), // Sugar               //  50 // 2000
            GingerBreadHouse.CreateWeightedValueForFootType(246), // Wheat Flour         //  50 // 2000
            GingerBreadHouse.CreateWeightedValueForFootType(229), // Tortilla            //  50 // 2000
            GingerBreadHouse.CreateWeightedValueForFootType(216), // Bread               //  60 // 1666
            GingerBreadHouse.CreateWeightedValueForFootType(223), // Cookie              // 140 //  714
            GingerBreadHouse.CreateWeightedValueForFootType(234), // Blueberry Tart      // 150 //  666
            GingerBreadHouse.CreateWeightedValueForFootType(220), // Chocolate Cake      // 200 //  500
            GingerBreadHouse.CreateWeightedValueForFootType(243), // Miner's Treat       // 200 //  500
            GingerBreadHouse.CreateWeightedValueForFootType(203), // Strange Bun         // 225 //  444
            GingerBreadHouse.CreateWeightedValueForFootType(651), // Poppyseed Muffin    // 250 //  400
            GingerBreadHouse.CreateWeightedValueForFootType(611), // Blackberry Cobbler  // 260 //  384
            GingerBreadHouse.CreateWeightedValueForFootType(607), // Roasted Hazelnuts   // 270 //  370
            GingerBreadHouse.CreateWeightedValueForFootType(731), // Maple Bar           // 300 //  333
            GingerBreadHouse.CreateWeightedValueForFootType(608), // Pumpkin Pie         // 385 //  259
            GingerBreadHouse.CreateWeightedValueForFootType(222), // Rhubarb Pie         // 400 //  250
            GingerBreadHouse.CreateWeightedValueForFootType(221), // Pink Cake           // 480 //  208
            // Non-food items with hardcoded weight (their price is too low, they would always spawn)
            new WeightedInt(388, 3000),  // Wood // 2 // 3000 (50000)
            new WeightedInt(92, 3000),   // Sap  // 2 // 3000 (50000)
        };
    }

    public class IridiumTreeSettings
    {
        public int MinimumAxeLevel { get; set; } = Axe.iridium;
        public int Health { get; set; } = 200;
        public int DamageIntervalForOreDrop { get; set; } = 20;
    }

    public class WoodsObeliskSettings
    {
        public int MoneyRequired { get; set; } = 10000000;
        public Dictionary<int, int> ItemsRequired { get; set; } = new Dictionary<int, int>()
        {
            { 388, 999},    // Wood
            {  92, 999},    // Sap
            { 337, 20}      // Iridium Bar
        };
    }

    public class UnicornSettings
    {
        public int FarmerScareDistance { get; set; } = 8;
        public int FarmerScareSpeed { get; set; } = 3;
        public int FleeSpeed { get; set; } = 12;
    }

    public class BushSettings
    {
        public int MinAxeLevel { get; set; } = Axe.steel;
        public int ThornyBushMinAxeLevel { get; set; } = Axe.iridium;
        public int ThornyBushDamagePerLevel { get; set; } = 5;
    }

    public class ObjectsSettings
    {
        public ExcaliburSettings Excalibur { get; set; } = new ExcaliburSettings();
        public GingerBreadHouseSettings GingerBreadHouse { get; set; } = new GingerBreadHouseSettings();
        public IridiumTreeSettings IridiumTree { get; set; } = new IridiumTreeSettings();
        public WoodsObeliskSettings WoodsObelisk { get; set; } = new WoodsObeliskSettings();
        public UnicornSettings Unicorn { get; set; } = new UnicornSettings();
        public BushSettings Bush { get; set; } = new BushSettings();
    }

    public class ResourceClumpLuckSettings
    {
        public Chance ChanceForMeteorite { get; set; } = new Chance(1, 500);
        public Chance ChanceForBoulder { get; set; } = new Chance(1);
        public Chance ChanceForHollowLog { get; set; } = new Chance(2);
        public Chance ChanceForStump { get; set; } = new Chance(3);
    }

    public class TerrainLuckSettings
    {
        public Chance ChanceForGingerbreadHouse { get; set; } = new Chance(1);
        public Chance ChanceForLargeBush { get; set; } = new Chance(10);
        public Chance ChanceForMediumBush { get; set; } = new Chance(5);
        public Chance ChanceForSmallBush { get; set; } = new Chance(5);
        public Chance ChanceForGrownTree { get; set; } = new Chance(25);
        public Chance ChanceForMediumTree { get; set; } = new Chance(10);
        public Chance ChanceForSmallTree { get; set; } = new Chance(10);
        public Chance ChanceForGrownFruitTree { get; set; } = new Chance(1);
        public Chance ChanceForSmallFruitTree { get; set; } = new Chance(5);
        public Chance ChanceForSilverFruits { get; set; } = new Chance(new LuckValue(15, 25));
        public Chance ChanceForGoldFruits { get; set; } = new Chance(new LuckValue(5, 35));
        public Chance ChanceForIridiumFruits { get; set; } = new Chance(new LuckValue(0, 45));
        public Chance ChanceForWeed { get; set; } = new Chance(20);
        public Chance ChanceForTwig { get; set; } = new Chance(10);
        public Chance ChanceForStone { get; set; } = new Chance(10);
        public Chance ChanceForMushroom { get; set; } = new Chance(10);
        public Chance ChanceForFlower { get; set; } = new Chance(7);
        public Chance ChanceForFlowerInWinter { get; set; } = new Chance(3);
        public Chance ChanceForFlowerOnClearing { get; set; } = new Chance(5);
        public Chance ChanceForExtraForageable { get; set; } = new Chance(0);
        public ResourceClumpLuckSettings ResourceClump { get; set; } = new ResourceClumpLuckSettings();

        public WeightedInt[] TreeTypes { get; set; } = new WeightedInt[]{
            new WeightedInt(Tree.bushyTree, 300),
            new WeightedInt(Tree.leafyTree, 300),
            new WeightedInt(Tree.pineTree, 300),
            new WeightedInt(Tree.mahoganyTree, 100),
        };

        public WeightedInt[] FruitTreeTypes { get; set; } = new WeightedInt[]{
            new WeightedInt(628, 200),  // apricot
            new WeightedInt(629, 200),  // cherry
            new WeightedInt(630, 200),  // orange
            new WeightedInt(631, 200),  // peach
            new WeightedInt(632, 200),  // apple
            new WeightedInt(633, 200),  // pomegrenade
            new WeightedInt(835, 200),  // mango
        };

        public WeightedInt[] FruitCount { get; set; } = new WeightedInt[]{
            new WeightedInt(0, 60),
            new WeightedInt(1, 30),
            new WeightedInt(2, 20),
            new WeightedInt(3, 10),
        };

        public WeightedInt[] WinterFlowers { get; set; } = new WeightedInt[] {
            new WeightedInt(429, 290),  // 597, // BlueJazz, spring (50g)
            new WeightedInt(425, 50),   // 595, // FairyRose, fall (290g)
        };

        public WeightedInt[] Flowers { get; set; } = new WeightedInt[] {
            new WeightedInt(427, 290), // 591, // Tulip, spring (30g)
            new WeightedInt(429, 140), // 597, // BlueJazz, spring (50g)
            new WeightedInt(455, 90),  // 593, // SummerSpangle, summer (90g)
            new WeightedInt(453, 50),  // 376, // Poppy, summer (140g)
            new WeightedInt(425, 30),  // 595, // FairyRose, fall (290g)
        };

        public WeightedInt[] WinterForageables { get; set; } = new WeightedInt[] { };
        public WeightedInt[] SummerForageables { get; set; } = new WeightedInt[] { };
        public WeightedInt[] FallForageables { get; set; } = new WeightedInt[] { };
        public WeightedInt[] SpringForageables { get; set; } = new WeightedInt[] { };
    }

    public class ClearingLuckSettings
    {
        public Chance ChanceForClearing { get; set; } = new Chance(new LuckValue(1, 10));

        public WeightedValue<string>[] Perks { get; set; } = new WeightedValue<string>[]{
            new WeightedValue<string>(LichtungStuff.Nothing, 1500),
            new WeightedValue<string>(LichtungStuff.Lake, 1250),
            new WeightedValue<string>(LichtungStuff.Infested, 1250),
            new WeightedValue<string>(LichtungStuff.MushroomTrees, 1000),
            new WeightedValue<string>(LichtungStuff.HealingFountain, 1000),
            new WeightedValue<string>(LichtungStuff.GingerbreadHouse, 1000),
            new WeightedValue<string>(LichtungStuff.IridiumTree, 500),
            new WeightedValue<string>(LichtungStuff.Unicorn, 500),
            new WeightedValue<string>(LichtungStuff.Treasure, 250),
            new WeightedValue<string>(LichtungStuff.ExcaliburStone, 250),
        };

        public WeightedValue<string>[] InfestedPerks { get; set; } = new WeightedValue<string>[]{
            new WeightedValue<string>(InfestedStuff.InfestedTree, 200),
            new WeightedValue<string>(InfestedStuff.ThornyBush, 400),
        };

        public Chance ChanceForTrashOrTreasure { get; set; } = new Chance(new LuckValue(0, 100));

        public TreasureLuckSettings Treasure { get; set; } = new TreasureLuckSettings();
        public TrashLuckSettings Trash { get; set; } = new TrashLuckSettings();
    }

    public class TreasureLuckSettings
    {
        public WeightedInt[] PileItems { get; set; } = new WeightedInt[] {
            new WeightedInt(384, 2000),   // Gold ore
            new WeightedInt(386, 300),    // Iridium ore
            new WeightedInt(80, 75),      // Quartz (25g)
            new WeightedInt(82, 75),      // Fire Quartz (80g)
            new WeightedInt(66, 75),      // Amethyst (100g)
            new WeightedInt(62, 50),      // Aquamarine (180g)
            new WeightedInt(60, 40),      // Emerald (250g)
            new WeightedInt(64, 30),      // Ruby (250g)
            new WeightedInt(72, 10),      // Diamond
            new WeightedInt(74, 1),       // Prismatic Shard
            new WeightedInt(166, 1),      // Treasure Chest
        };

        public Chance ChanceForMetalBarsInChest { get; set; } = new Chance(new LuckValue(10, 50));
        public Chance ChanceForElixirsInChest { get; set; } = new Chance(new LuckValue(10, 30));
        public Chance ChanceForArtefactInChest { get; set; } = new Chance(new LuckValue(0, 20));
        public Chance ChanceForDwarfScrollInChest { get; set; } = new Chance(new LuckValue(0, 20));
        public Chance ChanceForRingInChest { get; set; } = new Chance(new LuckValue(0, 20));
        public Chance ChanceForRandomPileItemInChest { get; set; } = new Chance(new LuckValue(0, 10));

        public LuckRange MetalBarStackSize { get; set; } = new LuckRange(new LuckValue(0, 100), new LuckValue(0, 100));
        public LuckRange ElixirStackSize { get; set; } = new LuckRange(new LuckValue(0, 100), new LuckValue(0, 100));
        public LuckRange PileItemStackSize { get; set; } = new LuckRange(new LuckValue(0, 100), new LuckValue(0, 100));
    }

    public class TrashLuckSettings
    {
        public WeightedInt[] PileItems { get; set; } = new WeightedInt[] {
            new WeightedInt(168, 2000),   // Trash
            new WeightedInt(172, 1000),   // Old Newspaper
            new WeightedInt(170, 1000),   // Glasses
            new WeightedInt(171, 500),    // CD
            new WeightedInt(167, 100),    // Joja Cola
            new WeightedInt(122, 5),      // Ancient Dwarf Computer
            new WeightedInt(118, 5),      // Glass Shards
            // new WeightedInt(169, 2000),// Driftwood
        };

        public Chance ChanceForLewisShortsInGarbagebin { get; set; } = new Chance(new LuckValue(0, 1, 1), 1000);
        public Chance ChanceForBoneInGarbagebin { get; set; } = new Chance(new LuckValue(0, 10));
        public Chance ChanceForArtefactInGarbagebin { get; set; } = new Chance(new LuckValue(0, 10));
        public Chance ChanceForPuppetInGarbagebin { get; set; } = new Chance(new LuckValue(0, 10));
        public Chance ChanceForRandomPileItemInGarbagebin { get; set; } = new Chance(new LuckValue(0, 10));

        public LuckRange PileItemStackSize { get; set; } = new LuckRange(new LuckValue(0, 100), new LuckValue(0, 100));
    }

    public class PerformanceSettings
    {
        public float LightSourceDensity { get; set; } = 100;
        public float BaubleDensity { get; set; } = 100;
        public float LeafDensity { get; set; } = 100;
        public float GrassDensity { get; set; } = 100;
    }

    public class MonstersSettings
    {
        public float MonsterDensity { get; set; } = 100;
        public float InfestedMonsterDensity { get; set; } = 100;
        public bool DisableBuffedMonsters { get; set; } = false;
        public bool DisableDangerousMonsters { get; set; } = false;

        public Chance ChanceForHalfMonsterCount { get; set; } = new Chance(new LuckValue(0, 0, 50));
        public Chance ChanceForUnbuffedMonster { get; set; } = new Chance(new LuckValue(0, 25, 75));
        public Chance ChanceForNonDangerousMonster { get; set; } = new Chance(new LuckValue(0, 25, 75));

        public DeepWoodsMonsters.MonsterDecider Bat { get; set; } = new DeepWoodsMonsters.MonsterDecider(1, 10);
        public DeepWoodsMonsters.MonsterDecider BigSlime { get; set; } = new DeepWoodsMonsters.MonsterDecider(2, 10);
        public DeepWoodsMonsters.MonsterDecider Grub { get; set; } = new DeepWoodsMonsters.MonsterDecider(2, 10);
        public DeepWoodsMonsters.MonsterDecider Fly { get; set; } = new DeepWoodsMonsters.MonsterDecider(2, 10);
        public DeepWoodsMonsters.MonsterDecider Brute { get; set; } = new DeepWoodsMonsters.MonsterDecider(5, 10);
        public DeepWoodsMonsters.MonsterDecider Golem { get; set; } = new DeepWoodsMonsters.MonsterDecider(5, 10);
        public DeepWoodsMonsters.MonsterDecider RockCrab { get; set; } = new DeepWoodsMonsters.MonsterDecider(10, 10);
        public DeepWoodsMonsters.MonsterDecider Ghost { get; set; } = new DeepWoodsMonsters.MonsterDecider(10, 10);
        public DeepWoodsMonsters.MonsterDecider PurpleSlime { get; set; } = new DeepWoodsMonsters.MonsterDecider(10, 10);
        public DeepWoodsMonsters.MonsterDecider Bug { get; set; } = new DeepWoodsMonsters.MonsterDecider(15, 10);
        public DeepWoodsMonsters.MonsterDecider Spider { get; set; } = new DeepWoodsMonsters.MonsterDecider(15, 10);
        public DeepWoodsMonsters.MonsterDecider ArmoredBug { get; set; } = new DeepWoodsMonsters.MonsterDecider(25, 10);
        public DeepWoodsMonsters.MonsterDecider PutridGhost { get; set; } = new DeepWoodsMonsters.MonsterDecider(25, 10);
        public DeepWoodsMonsters.MonsterDecider DustSprite { get; set; } = new DeepWoodsMonsters.MonsterDecider(25, 10);
    }

    public class FishiesLuckSettings
    {
        /*
        163: Legend: 5000
        159: Crimsonfish: 1500
        682: Mutant Carp: 1000
        775: Glacierfish: 1000
        160: Angler: 900
        162: Lava Eel: 700
        161: Ice Pip: 500
        800: Blobfish: 500
        158: Stonefish: 300
        155: Super Cucumber: 250
        799: Spook Fish: 220
        128: Pufferfish: 200
        143: Catfish: 200
        698: Sturgeon: 200
        699: Tiger Trout: 150
        149: Octopus: 150
        165: Scorpion Carp: 150
        795: Void Salmon: 150
        796: Slimejack: 100
        798: Midnight Squid: 100
        */

        public Chance ChanceForAwesomeFish { get; set; } = new Chance(new LuckValue(0, 0, 25));
        public WeightedInt[] AwesomeFishies { get; set; } = new WeightedInt[] {
            new WeightedInt(163, 10),   // 163: Legend: 5000
            new WeightedInt(159, 33),   // 159: Crimsonfish: 1500
            new WeightedInt(682, 50),   // 682: Mutant Carp: 1000
            new WeightedInt(775, 50),   // 775: Glacierfish: 1000
            new WeightedInt(160, 55),   // 160: Angler: 900
            new WeightedInt(162, 71),   // 162: Lava Eel: 700
            new WeightedInt(161, 100),   // 161: Ice Pip: 500
            new WeightedInt(800, 100),   // 800: Blobfish: 500
            new WeightedInt(158, 166),   // 158: Stonefish: 300
            new WeightedInt(155, 200),   // 155: Super Cucumber: 250
            new WeightedInt(799, 227),   // 799: Spook Fish: 220
            new WeightedInt(128, 250),   // 128: Pufferfish: 200
            new WeightedInt(143, 250),   // 143: Catfish: 200
            new WeightedInt(698, 250),   // 698: Sturgeon: 200
            new WeightedInt(699, 333),   // 699: Tiger Trout: 150
            new WeightedInt(149, 333),   // 149: Octopus: 150
            new WeightedInt(165, 333),   // 165: Scorpion Carp: 150
            new WeightedInt(795, 333),   // 795: Void Salmon: 150
            new WeightedInt(796, 500),   // 796: Slimejack: 100
            new WeightedInt(798, 500),   // 798: Midnight Squid: 100
        };
    }

    public class LuckSettings
    {
        public TerrainLuckSettings Terrain { get; set; } = new TerrainLuckSettings();
        public ClearingLuckSettings Clearings { get; set; } = new ClearingLuckSettings();
        public FishiesLuckSettings Fishies { get; set; } = new FishiesLuckSettings();
    }

    public class DeepWoodsStateData
    {
        private int lowestLevelReached = 0;
        private int orbStonesSaved = 0;

        public HashSet<long> PlayersWhoGotStardropFromUnicorn { get; set; } = new HashSet<long>();
        public HashSet<XY> WoodsObeliskLocations { get; set; } = new HashSet<XY>();

        public int LowestLevelReached
        {
            get
            {
                return lowestLevelReached;
            }

            set
            {
                if (value > lowestLevelReached && Game1.IsMasterGame)
                {
                    foreach (Farmer who in Game1.otherFarmers.Values)
                    {
                        if (who != Game1.player)
                        {
                            ModEntry.SendMessage(value, MessageId.SetLowestLevelReached, who.UniqueMultiplayerID);
                        }
                    }
                }
                lowestLevelReached = value;
            }
        }

        public int OrbStonesSaved
        {
            get
            {
                return orbStonesSaved;
            }

            set
            {
                if (value > orbStonesSaved && Game1.IsMasterGame)
                {
                    foreach (Farmer who in Game1.otherFarmers.Values)
                    {
                        if (who != Game1.player)
                        {
                            ModEntry.SendMessage(value, MessageId.SetOrbStonesSaved, who.UniqueMultiplayerID);
                        }
                    }
                }
                orbStonesSaved = value;
            }
        }
    }

    public class DeepWoodsSettings
    {
        // Save stuff
        public static DeepWoodsStateData DeepWoodsState { get; set; } = new DeepWoodsStateData();

        // Configurable settings
        public static DeepWoodsSettings Settings { get; set; } = new DeepWoodsSettings();

        // Settings subcategories
        public WoodsPassageSettings WoodsPassage { get; set; } = new WoodsPassageSettings();
        public LevelSettings Level { get; set; } = new LevelSettings();
        public MapSettings Map { get; set; } = new MapSettings();
        public ObjectsSettings Objects { get; set; } = new ObjectsSettings();
        public LuckSettings Luck { get; set; } = new LuckSettings();
        public MonstersSettings Monsters { get; set; } = new MonstersSettings();
        public PerformanceSettings Performance { get; set; } = new PerformanceSettings();
        public GenerationSettings Generation { get; set; } = new GenerationSettings();


        public static void DoSave()
        {
            if (!Game1.IsMasterGame)
                return;

            // save data
            ModEntry.GetHelper().Data.WriteSaveData("data", DeepWoodsState);

            // remove legacy file
            FileInfo legacyFile = new FileInfo($"{Constants.CurrentSavePath}/{SAVE_FILE_NAME}.json");
            if (legacyFile.Exists)
                legacyFile.Delete();
        }

        public static void DoLoad()
        {
            if (!Game1.IsMasterGame)
                return;

            ModEntry.Log("DeepWoodsSettings.DoLoad()", StardewModdingAPI.LogLevel.Trace);

            // load data
            DeepWoodsState = ModEntry.GetHelper().Data.ReadSaveData<DeepWoodsStateData>("data");
            if (DeepWoodsState == null)
            {
                // legacy file
                FileInfo legacyFile = new FileInfo($"{Constants.CurrentSavePath}/{SAVE_FILE_NAME}.json");
                ModEntry.Log("DeepWoodsSettings.DoLoad: Couldn't find savedata, trying legacy save: " + legacyFile.FullName, StardewModdingAPI.LogLevel.Trace);
                if (legacyFile.Exists)
                {
                    ModEntry.Log("DeepWoodsSettings.DoLoad: Loading legacy save.", StardewModdingAPI.LogLevel.Trace);
                    DeepWoodsState = JsonConvert.DeserializeObject<DeepWoodsStateData>(File.ReadAllText(legacyFile.FullName));
                }
            }
            if (DeepWoodsState == null)
            {
                ModEntry.Log("DeepWoodsSettings.DoLoad: No savedata, falling back to default.", StardewModdingAPI.LogLevel.Trace);
                DeepWoodsState = new DeepWoodsStateData();
            }

            // init settings
            ModEntry.Log("DeepWoodsSettings.DoLoad: Loading settings.", StardewModdingAPI.LogLevel.Trace);
            DeepWoodsSettings settings = ModEntry.GetHelper().ReadConfig<DeepWoodsSettings>();
            if (settings == null)
            {
                ModEntry.Log("Settings are null, using defaults.", StardewModdingAPI.LogLevel.Trace);
                Settings = new DeepWoodsSettings();
            }
            else
            {
                ModEntry.Log("Settings loaded successfully.", StardewModdingAPI.LogLevel.Trace);
                Settings = settings;

                if (Settings.WoodsPassage.AddBuildingTiles.Length == 0
                    && Settings.WoodsPassage.DeleteBuildingTiles.Length == 0
                    && Settings.WoodsPassage.WarpLocations.Length == 0)
                {
                    Settings.WoodsPassage = new WoodsPassageSettings();
                }
            }

            ModEntry.Log("DeepWoodsSettings.DoLoad: Done.", StardewModdingAPI.LogLevel.Trace);
        }
    }
}
