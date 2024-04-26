/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Chikakoo/stardew-valley-randomizer
**
*************************************************/

using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Randomizer
{
    /// <summary>
    /// An enum representing all the furniture
    /// </summary>
    public enum FurnitureIndexes
    {
        OakChair = 0,
        WalnutChair = 3,
        BirchChair = 6,
        MahoganyChair = 9,
        RedDinerChair = 12,
        BlueDinerChair = 15,
        CountryChair = 18,
        BreakfastChair = 21,
        PinkOfficeChair = 24,
        PurpleOfficeChair = 27,
        GreenOfficeStool = 30,
        OrangeOfficeStool = 31,
        DarkThrone = 64,
        DiningChair1 = 67,
        DiningChair2 = 70,
        GreenPlushSeat = 73,
        PinkPlushSeat = 76,
        WinterChair = 79,
        GroovyChair = 82,
        CuteChair = 85,
        StumpSeat = 88,
        MetalChair = 91,
        GreenStool = 94,
        BlueStool = 95,
        KingChair = 128,
        CrystalChair = 131,
        TropicalChair = 134,
        OakBench = 192,
        WalnutBench = 197,
        BirchBench = 202,
        MahoganyBench = 207,
        ModernBench = 212,
        BlueArmchair = 288,
        RedArmchair = 294,
        GreenArmchair = 300,
        YellowArmchair = 306,
        BrownArmchair = 312,
        BlueCouch = 416,
        RedCouch = 424,
        GreenCouch = 432,
        YellowCouch = 440,
        BrownCouch = 512,
        DarkCouch = 520,
        WizardCouch = 528,
        WoodsyCouch = 536,
        OakDresser = 704,
        WalnutDresser = 709,
        BirchDresser = 714,
        MahoganyDresser = 719,
        CoffeeTable = 724,
        StoneSlab = 727,
        WinterDiningTable = 800,
        FestiveDiningTable = 807,
        MahoganyDiningTable = 814,
        ModernDiningTable = 821,
        LongCactus = 984,
        LongPalm = 985,
        ExoticTree = 986,
        DeluxeTree = 989,
        OakTable = 1120,
        WalnutTable = 1122,
        BirchTable = 1124,
        MahoganyTable = 1126,
        SunTable = 1128,
        MoonTable = 1130,
        ModernTable = 1132,
        PubTable = 1134,
        LuxuryTable = 1136,
        DivinerTable = 1138,
        NeolithicTable = 1140,
        PuzzleTable = 1142,
        WinterTable = 1144,
        CandyTable = 1146,
        LuauTable = 1148,
        DarkTable = 1150,
        OakTeaTable = 1216,
        WalnutTeaTable = 1218,
        BirchTeaTable = 1220,
        MahoganyTeaTable = 1222,
        ModernTeaTable = 1224,
        FurnitureCatalogue = 1226,
        OceanicRug = 1228,
        ChinaCabinet = 1280,
        ArtistBookcase = 1283,
        LuxuryBookcase = 1285,
        ModernBookcase = 1287,
        DarkBookcase = 1289,
        CeramicPillar = 1291,
        GoldPillar = 1292,
        IndustrialPipe = 1293,
        IndoorPalm = 1294,
        TotemPole = 1295,
        ManicuredPine = 1296,
        TopiaryTree = 1297,
        StandingGeode = 1298,
        ObsidianVase = 1299,
        SingingStone = 1300,
        SlothSkeletonL = 1301,
        SlothSkeletonM = 1302,
        SlothSkeletonR = 1303,
        Skeleton = 1304,
        ChickenStatue = 1305,
        LeahsSculpture = 1306,
        DriedSunflowers = 1307,
        Catalogue = 1308,
        SamsBoombox = 1309,
        SmallPlant = 1362,
        TablePlant = 1363,
        DecorativeBowl = 1364,
        FutanBear = 1365,
        Globe = 1366,
        ModelShip = 1367,
        SmallCrystal = 1368,
        DecorativeLantern = 1369,
        WumbusStatue = 1371,
        BoboStatue = 1373,
        PurpleSerpentStatue = 1375,
        HousePlant1 = 1376,
        HousePlant2 = 1377,
        HousePlant3 = 1378,
        HousePlant4 = 1379,
        HousePlant5 = 1380,
        HousePlant6 = 1381,
        HousePlant7 = 1382,
        HousePlant8 = 1383,
        HousePlant9 = 1384,
        HousePlant10 = 1385,
        HousePlant11 = 1386,
        HousePlant12 = 1387,
        HousePlant13 = 1388,
        HousePlant14 = 1389,
        HousePlant15 = 1390,
        OakEndTable = 1391,
        WalnutEndTable = 1393,
        BirchEndTable = 1395,
        MahoganyEndTable = 1397,
        ModernEndTable = 1399,
        GrandmotherEndTable = 1400,
        WinterEndTable = 1401,
        Calendar = 1402,
        TreeoftheWinterStar = 1440,
        CountryLamp = 1443,
        BoxLamp = 1445,
        ModernLamp = 1447,
        ClassicLamp = 1449,
        RedRug = 1451,
        PatchworkRug = 1456,
        DarkRug = 1461,
        BudgetTV = 1466,
        PlasmaTV = 1468,
        GreenSerpentStatue = 1471,
        TheMuzzamaroo = 1539,
        ANightOnEcoHill = 1541,
        Pathways = 1543,
        BurntOffering = 1545,
        QueenoftheGemSea = 1547,
        VanillaVilla = 1550,
        PrimalMotion = 1552,
        JadeHills = 1554,
        SunNumber44 = 1557,
        WallflowerPal = 1559,
        Spires = 1561,
        Highway89 = 1563,
        CalicoFalls = 1565,
        NeedlepointFlower = 1567,
        SkullPoster = 1600,
        SunNumber45 = 1601,
        LittleTree = 1602,
        Blueberries = 1603,
        BlueCity = 1604,
        LittlePhotos = 1605,
        DancingGrass = 1606,
        VGAParadise = 1607,
        JColaLight = 1609,
        Kitemaster95 = 1612,
        BasicWindow = 1614,
        SmallWindow = 1616,
        RedCottageRug = 1618,
        GreenCottageRug = 1623,
        MonsterRug = 1628,
        BoardedWindow = 1630,
        MysticRug = 1664,
        LgFutanBear = 1669,
        BearStatue = 1671,
        Porthole = 1673,
        Anchor = 1675,
        WorldMap = 1676,
        OrnateWindow = 1678,
        FloorTV = 1680,
        CarvedWindow = 1682,
        ColorfulSet = 1684,
        CloudDecal1 = 1687,
        CloudDecal2 = 1692,
        JunimoPlush = 1733,
        NauticalRug = 1737,
        BurlapRug = 1742,
        TreeColumn = 1744,
        LLightString = 1745,
        SPine = 1747,
        BonsaiTree = 1748,
        MetalWindow = 1749,
        CandleLamp = 1751,
        MinersCrest = 1753,
        BambooMat = 1755,
        OrnateLamp = 1758,
        SmallJunimoPlush1 = 1760,
        SmallJunimoPlush2 = 1761,
        SmallJunimoPlush3 = 1762,
        SmallJunimoPlush4 = 1763,
        FutanRabbit = 1764,
        WoodcutRug = 1777,
        BrickFireplace = 1792,
        StoneFireplace = 1794,
        IridiumFireplace = 1796,
        StoveFireplace = 1798,
        MonsterFireplace = 1800,
        MyFirstPainting = 1802,
        HangingShield = 1811,
        MonsterDanglers = 1812,
        CeilingFlags = 1814,
        CeilingLeaves1 = 1817,
        CeilingLeaves2 = 1818,
        CeilingLeaves3 = 1819,
        CeilingLeaves4 = 1820,
        CeilingLeaves5 = 1821,
        RedEagle = 1838,
        PortraitOfAMermaid = 1840,
        SolarKingdom = 1842,
        Clouds = 1844,
        OneThousandYearsFromNow = 1846,
        ThreeTrees = 1848,
        TheSerpent = 1850,
        TropicalFishNumber173 = 1852,
        LandOfClay = 1854,
        ElegantFireplace = 1866,
        PirateFlag = 1900,
        PirateRug = 1902,
        StrawberryDecal = 1907,
        FruitSaladRug = 1909,
        NightSkyDecalNumber1 = 1914,
        NightSkyDecalNumber2 = 1915,
        NightSkyDecalNumber3 = 1916,
        WallPumpkin = 1917,
        SmallWallPumpkin = 1918,
        TheBraveLittleSapling = 1952,
        Mysterium = 1953,
        JourneyOfThePrairieKingTheMotionPicture = 1954,
        Wumbus = 1955,
        TheZuzuCityExpress = 1956,
        TheMiracleAtColdstarRanch = 1957,
        NaturalWondersExploringOurVibrantWorld = 1958,
        ItHowlsInTheRain = 1959,
        IndoorHangingBasket = 1960,
        WinterTreeDecal = 1961,
        BoneRug = 1964,
        ButterflyHutch = 1971,
        WallFlower = 1973,
        SWallFlower = 1974,
        CloudsBanner = 1975,
        SnowyRug = 1978,
        Bed = 2048,
        DoubleBed = 2052,
        StarryDoubleBed = 2058,
        StrawberryDoubleBed = 2064,
        PirateDoubleBed = 2070,
        ChildBed = 2076,
        TropicalBed = 2176,
        TropicalDoubleBed = 2180,
        DeluxeRedDoubleBed = 2186,
        ModernDoubleBed = 2192,
        LargeFishTank = 2304,
        DeluxeFishTank = 2312,
        SmallFishTank = 2322,
        TropicalTV = 2326,
        VolcanoPhoto = 2329,
        JungleTorch = 2331,
        GourmandStatue = 2332,
        PyramidDecal = 2334,
        PalmWallOrnament = 2393,
        IridiumKrobus = 2396,
        PlainTorch = 2397,
        StumpTorch = 2398,
        AquaticSanctuary = 2400,
        ModernFishTank = 2414,
        Lifesaver = 2418,
        FoliagePrint = 2419,
        Boat = 2421,
        Vista = 2423,
        WallBasket = 2425,
        DecorativeTrashCan = 2427,
        LightGreenRug = 2488,
        WildDoubleBed = 2496,
        FisherDoubleBed = 2502,
        BirchDoubleBed = 2508,
        ExoticDoubleBed = 2514,
        JadeHillsExtended = 2584,
        PastelBanner = 2624,
        WinterBanner = 2625,
        MoonlightJelliesBanner = 2626,
        JungleDecal1 = 2627,
        JungleDecal2 = 2628,
        JungleDecal3 = 2629,
        JungleDecal4 = 2630,
        StarportDecal = 2631,
        DecorativePitchfork = 2632,
        WoodPanel = 2633,
        DecorativeAxe = 2634,
        LogPanel1 = 2635,
        LogPanel2 = 2636,
        FloorDividerR1 = 2637,
        FloorDividerL1 = 2638,
        FloorDividerR2 = 2639,
        FloorDividerL2 = 2640,
        FloorDividerR3 = 2641,
        FloorDividerL3 = 2642,
        FloorDividerR4 = 2643,
        FloorDividerL4 = 2644,
        FloorDividerR5 = 2645,
        FloorDividerL5 = 2646,
        FloorDividerR6 = 2647,
        FloorDividerL6 = 2648,
        FloorDividerR7 = 2649,
        FloorDividerL7 = 2650,
        FloorDividerR8 = 2651,
        FloorDividerL8 = 2652,
        IcyBanner = 2653,
        WallPalm = 2654,
        WallCactus = 2655,
        LargeBrownCouch = 2720,
        FrozenDreams = 2730,
        Physics101 = 2732,
        WallSconce1 = 2734,
        WallSconce2 = 2736,
        WallSconce3 = 2738,
        WallSconce4 = 2740,
        BlossomRug = 2742,
        WallSconce5 = 2748,
        WallSconce6 = 2750,
        LargeGreenRug = 2784,
        IcyRug = 2790,
        OldWorldRug = 2794,
        LargeRedRug = 2798,
        LargeCottageRug = 2802,
        WallSconce = 2812,
        SquirrelFigurine = 2814,
        FunkyRug = 2870,
        ModernRug = 2875
    }

    public class FurnitureFunctions
    {
        public const string FurnitureIdPrefix = "(F)";

        /// <summary>
        /// Returns whether the given qualified id is for furniture
        /// </summary>
        /// <param name="id">The id</param>
        /// <returns>True if the given id is for furniture, false otherwise</returns>
        public static bool IsQualifiedIdForFurniture(string id)
        {
            return id.StartsWith(FurnitureIdPrefix);
        }

        /// <summary>
        /// Gets the Stardew furniture item from the given index
        /// </summary>
        /// <param name="index">The furniture index</param>
        /// <returns />
        public static Furniture GetItem(FurnitureIndexes index)
        {
            return Furniture.GetFurnitureInstance(((int)index).ToString());
        }
        
        /// <summary>
        /// Gets the qualified id for the given furniture index
        /// </summary>
        /// <param name="index">The index of the furniture</param>
        public static string GetQualifiedId(FurnitureIndexes index)
        {
            return $"{FurnitureIdPrefix}{(int)index}";
        }

        /// <summary>
        /// Gets a random furniture's qualified id
        /// </summary>
        /// <param name="rng">The rng to use</param>
        /// <param name="idsToExclude">A list of ids to not include in the selection</param>
        /// <returns>The qualified id</returns>
        public static string GetRandomFurnitureQualifiedId(RNG rng, List<string> idsToExclude = null)
        {
            return GetRandomFurnitureQualifiedIds(rng, numberToGet: 1, idsToExclude)
                .FirstOrDefault();
        }

        /// <summary>
        /// Gets a list of random furniture qualified ids
        /// </summary>
        /// <param name="rng">The rng to use</param>
        /// <param name="numberToGet">The number of ids to get</param>
        /// <param name="idsToExclude">A list of ids to not include in the selection</param>
        /// <returns>The qualified id</returns>
        public static List<string> GetRandomFurnitureQualifiedIds(
            RNG rng,
            int numberToGet, 
            List<string> idsToExclude = null)
        {
            var allFurnitureIds = Enum.GetValues(typeof(FurnitureIndexes))
                .Cast<FurnitureIndexes>()
                .Select(index => GetQualifiedId(index))
                .Where(id => idsToExclude == null || !idsToExclude.Contains(id))
                .ToList();

            return rng.GetRandomValuesFromList(allFurnitureIds, numberToGet);
        }
    }
}