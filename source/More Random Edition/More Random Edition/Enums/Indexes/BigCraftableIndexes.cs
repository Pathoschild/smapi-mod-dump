/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Chikakoo/stardew-valley-randomizer
**
*************************************************/

using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using SVObject = StardewValley.Object;

namespace Randomizer
{
    /// <summary>
    /// BigCraftables with the proper ingame indices
    /// </summary>
    public enum BigCraftableIndexes
    {
        HousePlant = 0,
        HousePlant1 = 1,
        HousePlant2 = 2,
        HousePlant3 = 3,
        HousePlant4 = 4,
        HousePlant5 = 5,
        HousePlant6 = 6,
        HousePlant7 = 7,
        Scarecrow = 8,
        LightningRod = 9,
        BeeHouse = 10,
        Keg = 12,
        Furnace = 13,
        PreservesJar = 15,
        CheesePress = 16,
        Loom = 17,
        OilMaker = 19,
        RecyclingMachine = 20,
        Crystalarium = 21,
        TablePieceL = 22,
        TablePieceR = 23,
        MayonnaiseMachine = 24,
        SeedMaker = 25,
        WoodChair = 26,
        WoodChair1 = 27,
        SkeletonModel = 28,
        Obelisk = 29,
        ChickenStatue = 31,
        StoneCairn = 32,
        SuitOfArmor = 33,
        SignOfTheVessel = 34,
        BasicLog = 35,
        LawnFlamingo = 36,
        WoodSign = 37,
        StoneSign = 38,
        DarkSign = 39,
        BigGreenCane = 40,
        GreenCanes = 41,
        MixedCane = 42,
        RedCanes = 43,
        BigRedCane = 44,
        OrnamentalHayBale = 45,
        LogSection = 46,
        GraveStone = 47,
        SeasonalDecor = 48,
        StoneFrog = 52,
        StoneParrot = 53,
        StoneOwl = 54,
        StoneOwl1 = 95,
        StoneJunimo = 55,
        SlimeBall = 56,
        GardenPot = 62,
        Bookcase = 64,
        FancyTable = 65,
        AncientTable = 66,
        AncientStool = 67,
        GrandfatherClock = 68,
        TeddyTimer = 69,
        DeadTree = 70,
        Staircase = 71,
        TallTorch = 72,
        RitualMask = 73,
        Bonfire = 74,
        Bongo = 75,
        DecorativeSpears = 76,
        Boulder = 78,
        Door = 79,
        Door1 = 80,
        LockedDoor = 81,
        LockedDoor1 = 82,
        WickedStatue = 83,
        WickedStatue1 = 84,
        SlothSkeletonL = 85,
        SlothSkeletonM = 86,
        SlothSkeletonR = 87,
        StandingGeode = 88,
        ObsidianVase = 89,
        BoneMill = 90,
        SingingStone = 94,
        StrangeCapsule = 96,
        EmptyCapsule = 98,
        FeedHopper = 99,
        Incubator = 101,
        Heater = 104,
        Tapper = 105,
        Camera = 106,
        PlushBunny = 107,
        TubOFlowers = 108,
        TubOFlowers1 = 109,
        DecorativePitcher = 111,
        DriedSunflowers = 112,
        CharcoalKiln = 114,
        StardewHeroTrophy = 116,
        SodaMachine = 117,
        Barrel = 118,
        Barrel1 = 120,
        Barrel2 = 122,
        Barrel3 = 124,
        Barrel4 = 174,
        Crate = 119,
        Crate1 = 121,
        Crate2 = 123,
        Crate3 = 125,
        Crate4 = 175,
        StatueOfEndlessFortune = 127,
        MushroomBox = 128,
        Chest = 130,
        Rarecrow1 = 110,
        Rarecrow2 = 113,
        Rarecrow3 = 126,
        Rarecrow4 = 136,
        Rarecrow5 = 137,
        Rarecrow6 = 138,
        Rarecrow7 = 139,
        Rarecrow8 = 140,
        PrairieKingArcadeSystem = 141,
        WoodenBrazier = 143,
        StoneBrazier = 144,
        GoldBrazier = 145,
        Campfire = 146,
        Campfire1 = 278,
        StumpBrazier = 147,
        CarvedBrazier = 148,
        SkullBrazier = 149,
        BarrelBrazier = 150,
        MarbleBrazier = 151,
        WoodLampPost = 152,
        IronLampPost = 153,
        WormBin = 154,
        HMTGF = 155,
        SlimeIncubator = 156,
        SlimeEggPress = 158,
        JunimoKartArcadeSystem = 159,
        StatueOfPerfection = 160,
        PinkyLemon = 161,
        Foroguemon = 162,
        Cask = 163,
        SolidGoldLewis = 164,
        AutoGrabber = 165,
        DeluxeScarecrow = 167,
        GeodeCrusher = 182,
        SeasonalPlant1 = 184,
        SeasonalPlant2 = 188,
        SeasonalPlant3 = 192,
        SeasonalPlant4 = 196,
        SeasonalPlant5 = 200,
        SeasonalPlant6 = 204,
        Workbench = 208,
        MiniJukebox = 209,
        WoodChipper = 211,
        Telephone = 214,
        MiniFridge = 216,
        CursedPKArcadeSystem = 219,
        SolarPanel = 231,
        StoneChest = 232,
        MiniObelisk = 238,
        FarmComputer = 239,
        MiniShippingBin = 248,
        CoffeeMaker = 246,
        SewingMachine = 247,
        OstrichIncubator = 254,
        JunimoChest = 256,
        HeavyTapper = 264,
        Deconstructor = 265,
        AutoPetter = 272,
        Hopper = 275,
        StatueOfTruePerfection = 280,

        // Big Craftables that don't use integers will get arbitrary negative numbers
        // DO NOT use these anywhere in the randomizer - use GetId instead
        Anvil = -10000,
        BaitMaker,
        BigChest,
        BigStoneChest,
        Dehydrator,
        DeluxeWormBin,
        FishSmoker,
        HeavyFurnace,
        MiniForge,
        MushroomLog,
        StatueOfBlessings,
        StatueOfTheDwarfKing,
        TextSign
    }

    public static class BigCraftableIndexesExtentions
    {
        private class BigCraftableIndexData
        {
            public static readonly Dictionary<BigCraftableIndexes, string> BigCraftableIndexIdMap = new();
            public static readonly Dictionary<string, BigCraftableIndexes> IdBigCraftableIndexMap = new();

            public static readonly Dictionary<BigCraftableIndexes, string> NonIntBigCraftableMap = new()
            {
                [BigCraftableIndexes.Anvil] = "Anvil",
                [BigCraftableIndexes.BaitMaker] = "BaitMaker",
                [BigCraftableIndexes.BigChest] = "BigChest",
                [BigCraftableIndexes.BigStoneChest] = "BigStoneChest",
                [BigCraftableIndexes.Dehydrator] = "Dehydrator",
                [BigCraftableIndexes.DeluxeWormBin] = "DeluxeWormBin",
                [BigCraftableIndexes.FishSmoker] = "FishSmoker",
                [BigCraftableIndexes.HeavyFurnace] = "HeavyFurnace",
                [BigCraftableIndexes.MiniForge] = "MiniForge",
                [BigCraftableIndexes.MushroomLog] = "MushroomLog",
                [BigCraftableIndexes.StatueOfBlessings] = "StatueOfBlessings",
                [BigCraftableIndexes.StatueOfTheDwarfKing] = "StatueOfTheDwarfKing",
                [BigCraftableIndexes.TextSign] = "TextSign"
            };

            static BigCraftableIndexData()
            {
                Enum.GetValues(typeof(BigCraftableIndexes))
                    .Cast<BigCraftableIndexes>()
                    .ToList()
                    .ForEach(index =>
                    {
                        int indexAsInt = (int)index;
                        string indexAsString = indexAsInt >= 0
                            ? indexAsInt.ToString()
                            : NonIntBigCraftableMap[index];

                        BigCraftableIndexIdMap[index] = indexAsString;
                        IdBigCraftableIndexMap[indexAsString] = index;
                    });
            }
        };

        public static string GetId(this BigCraftableIndexes index) =>
            BigCraftableIndexData.BigCraftableIndexIdMap[index];

        public static Item GetItem(this BigCraftableIndexes index) =>
            ItemList.BigCraftableItems[GetId(index)];

        public static BigCraftableIndexes GetBigCraftableIndex(string id) =>
            BigCraftableIndexData.IdBigCraftableIndexMap[id];
    }

    public class BigCraftableFunctions
    {
        public const string BigCraftableIdPrefix = "(BC)";

        /// <summary>
        /// Returns whether the given qualified id is for a big craftable
        /// </summary>
        /// <param name="id">The id</param>
        /// <returns>True if the given id is for a big craftable, false otherwise</returns>
        public static bool IsQualifiedIdForBigCraftable(string id)
        {
            return id.StartsWith(BigCraftableIdPrefix);
        }

        /// <summary>
        /// Gets the big craftable object from the given index
        /// </summary>
        /// <param name="index">The index of the big craftable</param>
        /// <returns />
        public static SVObject GetItem(BigCraftableIndexes index)
        {
            return new SVObject(Vector2.Zero, index.GetId());
        }

        /// <summary>
        /// Gets the qualified id for the given big craftable index
        /// </summary>
        /// <param name="index">The index of the big craftable</param>
        public static string GetQualifiedId(BigCraftableIndexes index)
        {
            return $"(BC){index.GetId()}";
        }

        /// <summary>
        /// Gets a random furniture's qualified id
        /// </summary>
        /// <param name="rng">The rng to use</param>
        /// <param name="idsToExclude">A list of ids to not include in the selection</param>
        /// <returns>The qualified id</returns>
        public static string GetRandomBigCraftableQualifiedId(RNG rng, List<string> idsToExclude = null)
        {
            var allBigCraftableIds = Enum.GetValues(typeof(BigCraftableIndexes))
                .Cast<BigCraftableIndexes>()
                .Select(index => GetQualifiedId(index))
                .Where(id => idsToExclude == null || !idsToExclude.Contains(id))
                .ToList();

            return rng.GetRandomValueFromList(allBigCraftableIds);
        }
    }
}