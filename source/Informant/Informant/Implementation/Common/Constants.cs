/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/slothsoft/stardew-informant
**
*************************************************/

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedType.Global

using System.Linq;

namespace Slothsoft.Informant.Implementation.Common;

/// <summary>
/// Contains constants for all the seasons.
/// </summary>
internal static class Seasons {
    public const string Spring = "spring";
    public const string Summer = "summer";
    public const string Fall = "fall";
    public const string Winter = "winter";
    public static readonly string[] All = {Spring, Summer, Fall, Winter};

    public const int LengthInDays = 28;
}

/// <summary>
/// These constants can be used to compare to <code>Object.ParentSheetIndex</code>.
/// See https://stardewcommunitywiki.com/Modding:Big_craftables_data
/// </summary>
public static class BigCraftableIds {
    public const int BeeHouse = 10;
    public const int Cask = 163;
    public const int CheesePress = 16;
    public const int Keg = 12;
    public const int Loom = 17;
    public const int MayonnaiseMachine = 24;
    public const int OilMaker = 19;
    public const int PreservesJar = 15;
    public const int BoneMill = 90;
    public const int CharcoalKiln = 114;
    public const int Crystalarium = 21;
    public const int Furnace = 13;
    public const int GeodeCrusher = 182;
    public const int HeavyTapper = 264;
    public const int LightningRod = 9;
    public const int OstrichIncubator = 254;
    public const int RecyclingMachine = 20;
    public const int SeedMaker = 25;
    public const int SlimeEggPress = 158;
    public const int SlimeIncubator = 156;
    public const int SlimeIncubator2 = 157; // game has both of these IDs?
    public const int SolarPanel = 231;
    public const int Tapper = 105;
    public const int WoodChipper = 211;
    public const int WormBin = 154;
    public const int Incubator = 101;
    public const int Incubator2 = 102; // the Wiki above shows Incubator as 101, but the game as 102?
    public const int Incubator3 = 103; // maybe it's the egg color?
    public const int CoffeeMaker = 246;
    public const int Deconstructor = 265;
    public const int StatueOfPerfection = 160;
    public const int StatueOfTruePerfection = 280;

    public static readonly int[] AllMachines = {
        BeeHouse, Cask, CheesePress, Keg, Loom, MayonnaiseMachine, OilMaker, PreservesJar,
        BoneMill, CharcoalKiln, Crystalarium, Furnace, GeodeCrusher, HeavyTapper, LightningRod, OstrichIncubator, RecyclingMachine, SeedMaker, SlimeEggPress,
        SlimeIncubator, SlimeIncubator2, SolarPanel, Tapper, WoodChipper, WormBin, Incubator, Incubator2, Incubator3, CoffeeMaker, Deconstructor,
        StatueOfPerfection, StatueOfTruePerfection
    };

    public const int Chest = 130;
    public const int JunimoChest = 256;
    public const int MiniFridge = 216;
    public const int StoneChest = 232;
    public const int MiniShippingBin = 248;

    public static readonly int[] AllChests = {
        Chest, JunimoChest, MiniFridge, StoneChest, MiniShippingBin
    };

    public static readonly int[] HousePlants = Enumerable.Range(0, 7 + 1).ToArray();
    public const int Scarecrow = 8;
    public const int TablePieceL = 22;
    public const int TablePieceR = 23;
    public const int WoodChair = 26;
    public const int WoodChair2 = 27;
    public const int SkeletonModel = 28;
    public const int Obelisk = 29;
    public const int ChickenStatue = 31;
    public const int StoneCairn = 32;
    public const int SuitOfArmor = 33;
    public const int SignOfTheVessel = 34;
    public const int BasicLog = 35;
    public const int LawnFlamingo = 36;
    public const int WoodSign = 37;
    public const int StoneSign = 38;
    public const int DarkSign = 39;
    public const int BigGreenCane = 40;
    public const int GreenCanes = 41;
    public const int MixedCane = 42;
    public const int RedCanes = 43;
    public const int BigRedCane = 44;
    public const int OrnamentalHayBale = 45;
    public const int LogSection = 46;
    public const int GraveStone = 47;
    public const int SeasonalDecor = 48;
    public const int StoneFrog = 52;
    public const int StoneParrot = 53;
    public const int StoneOwl = 54;
    public const int StoneJunimo = 55;
    public const int SlimeBall = 56;
    public const int GardenPot = 62;
    public const int Bookcase = 64;
    public const int FancyTable = 65;
    public const int AncientTable = 66;
    public const int AncientStool = 67;
    public const int GrandfatherClock = 68;
    public const int TeddyTimer = 69;
    public const int DeadTree = 70;
    public const int Staircase = 71;
    public const int TallTorch = 72;
    public const int RitualMask = 73;
    public const int Bonfire = 74;
    public const int Bongo = 75;
    public const int DecorativeSpears = 76;
    public const int Boulder = 78;
    public const int Door = 79;
    public const int Door2 = 80;
    public const int LockedDoor = 81;
    public const int LockedDoor2 = 82;
    public const int WickedStatue = 83;
    public const int WickedStatue2 = 84;
    public const int SlothSkeletonL = 85;
    public const int SlothSkeletonM = 86;
    public const int SlothSkeletonR = 87;
    public const int StandingGeode = 88;
    public const int ObsidianVase = 89;
    public const int SingingStone = 94;
    public const int StoneOwl2 = 95;
    public const int StrangeCapsule = 96;
    public const int EmptyCapsule = 98;
    public const int FeedHopper = 99;
    public const int Heater = 104;
    public const int Camera = 106;
    public const int PlushBunny = 107;
    public const int TubOFlowers = 108;
    public const int TubOFlowers2 = 109;
    public const int Rarecrow = 110;
    public const int DecorativePitcher = 111;
    public const int DriedSunflowers = 112;
    public const int Rarecrow2 = 113;
    public const int StardewHeroTrophy = 116;
    public const int SodaMachine = 117;
    public static readonly int[] BarrelsAndCrates = Enumerable.Range(118, 125 - 118 + 1).ToArray();
    public const int Rarecrow3 = 126;
    public const int StatueOfEndlessFortune = 127;
    public const int MushroomBox = 128;
    public const int Rarecrow4 = 136;
    public const int Rarecrow5 = 137;
    public const int Rarecrow6 = 138;
    public const int Rarecrow7 = 139;
    public const int Rarecrow8 = 140;
    public const int PrairieKingArcadeSystem = 141;
    public const int WoodenBrazier = 143;
    public const int StoneBrazier = 144;
    public const int GoldBrazier = 145;
    public const int Campfire = 146;
    public const int StumpBrazier = 147;
    public const int CarvedBrazier = 148;
    public const int SkullBrazier = 149;
    public const int BarrelBrazier = 150;
    public const int MarbleBrazier = 151;
    public const int WoodLamppost = 152;
    public const int IronLamppost = 153;
    public const int Hmtgf = 155;
    public const int JunimoKartArcadeSystem = 159;
    public const int PinkyLemon = 161;
    public const int Foroguemo = 162;
    public const int SolidGoldLewis = 164;
    public const int AutoGrabber = 165;
    public const int DeluxeScarecrow = 167;
    public const int Barrel = 174;
    public const int Crate = 175;
    public static readonly int[] SeasonalPlants = Enumerable.Range(184, 207 - 184 + 1).ToArray();
    public const int Workbench = 208;
    public const int MiniJukebox = 209;
    public const int Telephone = 214;
    public const int CursedPkArcadeSystem = 219;
    public const int MiniObelisk = 238;
    public const int FarmComputer = 239;
    public const int SewingMachine = 247;
    public const int AutoPetter = 272;
    public const int Hopper = 275;
    public const int Campfire2 = 278;

    public static readonly int[] AllStaticCraftables = HousePlants.Concat(BarrelsAndCrates).Concat(SeasonalPlants).Concat(new[] {
        Scarecrow, TablePieceL, TablePieceR, WoodChair, WoodChair2, SkeletonModel, Obelisk, ChickenStatue, StoneCairn, SuitOfArmor, SignOfTheVessel, BasicLog,
        LawnFlamingo, WoodSign, StoneSign, DarkSign, BigGreenCane, GreenCanes, MixedCane, RedCanes, BigRedCane, OrnamentalHayBale, LogSection, GraveStone,
        SeasonalDecor, StoneFrog, StoneParrot, StoneOwl, StoneJunimo, SlimeBall, GardenPot, Bookcase, FancyTable, AncientTable, AncientStool, GrandfatherClock,
        TeddyTimer, DeadTree, Staircase, TallTorch, RitualMask, Bonfire, Bongo, DecorativeSpears, Boulder, Door, Door2, LockedDoor, LockedDoor2, WickedStatue,
        WickedStatue2, SlothSkeletonL, SlothSkeletonM, SlothSkeletonR, StandingGeode, ObsidianVase, SingingStone, StoneOwl2, StrangeCapsule, EmptyCapsule,
        FeedHopper, Heater, Camera, PlushBunny, TubOFlowers, TubOFlowers2, Rarecrow, DecorativePitcher, DriedSunflowers, Rarecrow2, StardewHeroTrophy,
        SodaMachine, Rarecrow3, StatueOfEndlessFortune, MushroomBox, Rarecrow4, Rarecrow5, Rarecrow6, Rarecrow7, Rarecrow8,
        PrairieKingArcadeSystem, WoodenBrazier, StoneBrazier, GoldBrazier, Campfire, StumpBrazier, CarvedBrazier, SkullBrazier, BarrelBrazier, MarbleBrazier,
        WoodLamppost, IronLamppost, Hmtgf, JunimoKartArcadeSystem, PinkyLemon, Foroguemo, SolidGoldLewis, AutoGrabber, DeluxeScarecrow, Barrel, Crate,
        Workbench, MiniJukebox, Telephone, CursedPkArcadeSystem, MiniObelisk, FarmComputer, SewingMachine, AutoPetter, Hopper, Campfire2,
    }).ToArray();
}

/// <summary>
/// These constants can be used to compare to <code>Object.ParentSheetIndex</code>.
/// See https://stardewcommunitywiki.com/Modding:Object_data
/// </summary>
public static class ObjectIds {
    public const int GingerForageCropId = 2;
    public const int Ginger = 829;
}