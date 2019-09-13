
using StardewModdingAPI;
using System.Collections.Generic;
using xTile.Dimensions;
using static DeepWoodsMod.DeepWoodsRandom;
using static DeepWoodsMod.DeepWoodsGlobals;
using Newtonsoft.Json;
using StardewValley.Tools;
using StardewValley;
using System.IO;
using System;

namespace DeepWoodsMod
{
    class WoodsPassageSettings
    {
        public Location[] DeleteBuildingTiles { get; set; } = new Location[] {
            new Location(29, 25),
            new Location(29, 26),
        };

        public Location[] AddBuildingTiles { get; set; } = new Location[] {
            new Location(24, 24),
            new Location(25, 24),
            new Location(26, 24),
            new Location(27, 24),
            new Location(28, 24),
        };

        public Location[] WarpLocations { get; set; } = new Location[] {
            new Location(24, 32),
            new Location(25, 32),
            new Location(26, 32),
            new Location(27, 32),
            new Location(28, 32),
        };
    }

    class MapSettings
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

    class LevelSettings
    {
        public int MinLevelForFlowers { get; set; } = 3;
        public int MinLevelForFruits { get; set; } = 5;
        public int MinLevelForThornyBushes { get; set; } = 10;
        public int MinLevelForBuffedMonsters { get; set; } = 15;
        public int MinLevelForWoodsObelisk { get; set; } = 20;
        public int MinLevelForMeteorite { get; set; } = 25;
        public int MinLevelForClearing { get; set; } = 30;
        public int MinLevelForGingerbreadHouse { get; set; } = 50;
        public Chance ChanceForThornyBushesOnExit { get; set; } = new Chance(new LuckValue(50, 5));
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
        public Chance ChanceForWeed { get; set; } = new Chance(20);
        public Chance ChanceForTwig { get; set; } = new Chance(10);
        public Chance ChanceForStone { get; set; } = new Chance(10);
        public Chance ChanceForMushroom { get; set; } = new Chance(10);
        public Chance ChanceForFlower { get; set; } = new Chance(7);
        public Chance ChanceForFlowerInWinter { get; set; } = new Chance(3);
        public Chance ChanceForFlowerOnClearing { get; set; } = new Chance(5);
        public ResourceClumpLuckSettings ResourceClump { get; set; } = new ResourceClumpLuckSettings();

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

        public Chance ChanceForEasterEgg { get; set; } = new Chance(new LuckValue(5, 25), 1000);
        public Chance ChanceForExtraEasterEgg { get; set; } = new Chance(new LuckValue(0, 25));
        public Chance ChanceForEasterEggsDoubled { get; set; } = new Chance(new LuckValue(75, 100));

        public LuckRange MaxEasterEggsPerLevel { get; set; } = new LuckRange(new LuckValue(0, 2), new LuckValue(2, 6));
    }

    public class ClearingLuckSettings
    {
        public Chance ChanceForClearing { get; set; } = new Chance(new LuckValue(1, 10));

        public WeightedValue<string>[] Perks { get; set; } = new WeightedValue<string>[]{
            new WeightedValue<string>(LichtungStuff.Nothing, 1500),
            new WeightedValue<string>(LichtungStuff.Lake, 1250),
            new WeightedValue<string>(LichtungStuff.MushroomTrees, 1000),
            new WeightedValue<string>(LichtungStuff.HealingFountain, 1000),
            new WeightedValue<string>(LichtungStuff.GingerbreadHouse, 1000),
            new WeightedValue<string>(LichtungStuff.IridiumTree, 500),
            new WeightedValue<string>(LichtungStuff.Unicorn, 500),
            new WeightedValue<string>(LichtungStuff.Treasure, 250),
            new WeightedValue<string>(LichtungStuff.ExcaliburStone, 250)
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

    public class MonstersSettings
    {
        public Chance ChanceForHalfMonsterCount { get; set; } = new Chance(new LuckValue(0, 0, 50));
        public Chance ChanceForUnbuffedMonster { get; set; } = new Chance(new LuckValue(0, 25, 75));

        public DeepWoodsMonsters.MonsterDecider Bat { get; set; } = new DeepWoodsMonsters.MonsterDecider(1, 10);
        public DeepWoodsMonsters.MonsterDecider BigSlime { get; set; } = new DeepWoodsMonsters.MonsterDecider(2, 10);
        public DeepWoodsMonsters.MonsterDecider Grub { get; set; } = new DeepWoodsMonsters.MonsterDecider(2, 10);
        public DeepWoodsMonsters.MonsterDecider Fly { get; set; } = new DeepWoodsMonsters.MonsterDecider(2, 10);
        public DeepWoodsMonsters.MonsterDecider Brute { get; set; } = new DeepWoodsMonsters.MonsterDecider(5, 10);
        public DeepWoodsMonsters.MonsterDecider Golem { get; set; } = new DeepWoodsMonsters.MonsterDecider(5, 10);
        public DeepWoodsMonsters.MonsterDecider RockCrab { get; set; } = new DeepWoodsMonsters.MonsterDecider(10, 10);
        public DeepWoodsMonsters.MonsterDecider Ghost { get; set; } = new DeepWoodsMonsters.MonsterDecider(10, 10);
        public DeepWoodsMonsters.MonsterDecider PurpleSlime { get; set; } = new DeepWoodsMonsters.MonsterDecider(10, 10);
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

    public class I18NData
    {
        private readonly ITranslationHelper I18n;

        public string ExcaliburDisplayName => this.I18n.Get("excalibur.name");
        public string ExcaliburDescription => this.I18n.Get("excalibur.description");
        public string WoodsObeliskDisplayName => this.I18n.Get("woods-obelisk.name");
        public string WoodsObeliskDescription => this.I18n.Get("woods-obelisk.description");
        public string EasterEggDisplayName => this.I18n.Get("easter-egg.name");
        public string EasterEggHatchedMessage => this.I18n.Get("easter-egg.hatched-message");
        public string LostMessage => this.I18n.Get("lost-message");
        public string WoodsObeliskWizardMailMessage => this.I18n.Get("woods-obelisk.wizard-mail");

        public I18NData(ITranslationHelper i18n)
        {
            this.I18n = i18n;
        }
    }

    class DeepWoodsStateData
    {
        private int lowestLevelReached = 0;

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
    }

    class DeepWoodsSettings
    {
        // I18N
        public static I18NData I18N { get; private set; }

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

        public static void Init(ITranslationHelper i18n)
        {
            DeepWoodsSettings.I18N = new I18NData(i18n);
        }

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
