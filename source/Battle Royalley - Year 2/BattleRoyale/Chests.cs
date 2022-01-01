/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tylergibbs2/BattleRoyalley-Year2
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BattleRoyale
{
    enum ChestGrade : int
    {
        LEGENDARY = 0,
        RARE = 1,
        COMMON = 2,
        INFINITY_GAVEL = 3,
        SPOUSE = 4
    }

    enum LootSecondaryType : int
    {
        FOOD_GREATER = 0,
        FOOD_LESSER = 1,
        BUFFS = 2,
        CHERRY_BOMBS = 3
    }

    class Chest
    {
        public GameLocation Location { get; set; }

        public Vector2 TilePosition { get; set; }

        public ChestGrade Grade { get; set; }

        public bool BeenOpened { get; set; } = false;

        public StardewValley.Objects.Chest StardewChest { get; set; } = null;

        public Chest(string location, Vector2 tilePosition)
        {
            Location = Game1.getLocationFromName(location);
            TilePosition = tilePosition;
        }
    }

    class Chests
    {
        private static List<Chest> chests;

        private const float minFractionToSpawn = 0.25f;
        private const float maxFractionToSpawn = 0.8f;
        private const int numberOfPlayersForMaxFraction = 15;

        private static readonly Dictionary<ChestGrade, int[]> LootPrimary = new() {
            { ChestGrade.LEGENDARY,      new int[]{ 2, 48, 23, 46, 32 } },
            { ChestGrade.RARE,           new int[]{ 5, 7, 45, 26, 32 } },
            { ChestGrade.COMMON,         new int[]{ 3, 49, 51, 32 } },
            { ChestGrade.INFINITY_GAVEL, new int[]{ 63 } },
            { ChestGrade.SPOUSE,         new int[]{ 36, 40, 35, 25, 42, 37, 39, 38, 41, 30 } }
        };

        private static readonly Dictionary<ChestGrade, int> GradeSlingshotAmmo = new() {
            { ChestGrade.LEGENDARY,      378 },
            { ChestGrade.RARE,           390 },
            { ChestGrade.COMMON,         388 }
        };

        private static readonly Dictionary<LootSecondaryType, int[]> LootSecondary = new() {
            { LootSecondaryType.FOOD_GREATER,    new int[]{ 222, 221, 649 } },
            { LootSecondaryType.FOOD_LESSER,     new int[]{ 216, 456, 198 } },
            { LootSecondaryType.BUFFS,           new int[]{ 253, 244, 231 } },
            { LootSecondaryType.CHERRY_BOMBS,    new int[]{ 286 } }
        };

        private static readonly Dictionary<LootSecondaryType, double> LootSecondaryWeights = new()
        {
            { LootSecondaryType.FOOD_GREATER,    0.25 },
            { LootSecondaryType.FOOD_LESSER,     0.40 },
            { LootSecondaryType.BUFFS,           0.20 },
            { LootSecondaryType.CHERRY_BOMBS,    0.15 }
        };

        private static readonly Dictionary<LootSecondaryType, int> LootSecondaryQuantities = new()
        {
            { LootSecondaryType.FOOD_GREATER,    1 },
            { LootSecondaryType.FOOD_LESSER,     3 },
            { LootSecondaryType.BUFFS,           1 },
            { LootSecondaryType.CHERRY_BOMBS,    3 }
        };

        private static readonly Dictionary<ChestGrade, double> GradeWeights = new()
        {
            { ChestGrade.LEGENDARY,      0.099 },
            { ChestGrade.RARE,           0.5   },
            { ChestGrade.COMMON,         0.399 },
            { ChestGrade.INFINITY_GAVEL, 0.001 },
            { ChestGrade.SPOUSE,         0.001 }
        };

        private static readonly Dictionary<ChestGrade, Color> GradeColors = new()
        {
            { ChestGrade.LEGENDARY,      Color.Gold              },
            { ChestGrade.RARE,           new Color(52, 178, 52)  },
            { ChestGrade.COMMON,         Color.White             },
            { ChestGrade.INFINITY_GAVEL, Color.Purple            },
            { ChestGrade.SPOUSE,         Color.Pink              }
        };

        private static readonly int GivenSlingshotAmmo = 20;

        private static readonly int[] SlingshotIds = new int[] { 32, 33, 34 };

        public static ChestGrade GetRandomChestGrade()
        {
            double total = 0;
            double amount = Game1.random.NextDouble();

            foreach (var kvp in GradeWeights)
            {
                ChestGrade grade = kvp.Key;
                double weight = kvp.Value;

                total += weight;
                if (amount <= total)
                    return grade;
            }

            return ChestGrade.COMMON;
        }

        public static LootSecondaryType GetRandomLootSecondaryType()
        {
            double total = 0;
            double amount = Game1.random.NextDouble();

            foreach (var kvp in LootSecondaryWeights)
            {
                LootSecondaryType grade = kvp.Key;
                double weight = kvp.Value;

                total += weight;
                if (amount <= total)
                    return grade;
            }

            return LootSecondaryType.CHERRY_BOMBS;
        }

        public static void AddItems(Chest chest, StardewValley.Objects.Chest stardewChest)
        {
            // Primary item (weapon)
            int randomIdx = Game1.random.Next(LootPrimary[chest.Grade].Length);
            int itemId = LootPrimary[chest.Grade][randomIdx];

            Item primaryItem;
            if (SlingshotIds.Contains(itemId))
            {
                primaryItem = new StardewValley.Tools.Slingshot(itemId);
                int ammoId = GradeSlingshotAmmo[chest.Grade];
                stardewChest.items.Add(new StardewValley.Object(ammoId, GivenSlingshotAmmo));
            }
            else
                primaryItem = new StardewValley.Tools.MeleeWeapon(itemId);

            stardewChest.items.Add(primaryItem);

            // Secondary item (consumable)
            LootSecondaryType secondaryItemType = GetRandomLootSecondaryType();

            if (ModEntry.BRGame.IsSpecialRoundType(SpecialRoundType.NO_FOOD) && secondaryItemType != LootSecondaryType.CHERRY_BOMBS)
                return;

            randomIdx = Game1.random.Next(LootSecondary[secondaryItemType].Length);
            itemId = LootSecondary[secondaryItemType][randomIdx];
            int quantity = LootSecondaryQuantities[secondaryItemType];

            Item secondaryItem = new StardewValley.Object(itemId, quantity);
            stardewChest.items.Add(secondaryItem);
        }

        public static void CreateStardewChest(Chest chest)
        {
            chest.Grade = GetRandomChestGrade();
            StardewValley.Objects.Chest stardewChest = new(true);

            AddItems(chest, stardewChest);

            // Set color and place chest on map
            stardewChest.playerChoiceColor.Set(GradeColors[chest.Grade]);

            chest.Location.setObject(chest.TilePosition, stardewChest);
            chest.StardewChest = stardewChest;
        }

        public static bool ShouldSpawnChests()
        {
            return (
                !ModEntry.BRGame.IsSpecialRoundType(SpecialRoundType.SLUGFEST) &&
                !ModEntry.BRGame.IsSpecialRoundType(SpecialRoundType.TRAVELLING_CART) &&
                !ModEntry.BRGame.IsSpecialRoundType(SpecialRoundType.SLINGSHOT_ONLY) &&
                !ModEntry.BRGame.IsSpecialRoundType(SpecialRoundType.BOMBS_ONLY)
            );
        }

        public static void SpawnAndFillChests(bool gingerIsland = false)
        {
            if (!ShouldSpawnChests())
                return;

            if (gingerIsland)
                chests = ChestsSpawnsDataModel.IslandChestSpawns.ToList();
            else
                chests = ChestsSpawnsDataModel.ChestsSpawns.ToList();

            chests.Shuffle();

            float onlinePlayers = Math.Min(1, Game1.getOnlineFarmers().Count / numberOfPlayersForMaxFraction);
            float fractionToSpawn = minFractionToSpawn + (maxFractionToSpawn - minFractionToSpawn) * onlinePlayers;
            int chestsToSpawn = (int)(chests.Count * (1 - fractionToSpawn));

            chests = chests.GetRange(0, chestsToSpawn);

            foreach (Chest chest in chests)
                CreateStardewChest(chest);

            var horses = new Horses(chests);
            ModEntry.Events.GameLoop.UpdateTicked += horses.Update;
        }
    }

    class ChestsSpawnsDataModel
    {
        public static List<Chest> ChestsSpawns { get; set; } = new List<Chest>()
        {
            new Chest("BusStop", new Vector2(26, 13)),
            new Chest("Town", new Vector2(18, 52)),
            new Chest("Town", new Vector2(29, 67)),
            new Chest("Saloon", new Vector2(33, 6)),
            new Chest("SamHouse", new Vector2(11, 23)),
            new Chest("HaleyHouse", new Vector2(5,4)),
            new Chest("HaleyHouse", new Vector2(13, 4)),
            new Chest("Town", new Vector2(59, 99)),
            new Chest("ManorHouse", new Vector2(19, 4)),
            new Chest("Town", new Vector2(72, 90)),
            new Chest("ArchaeologyHouse", new Vector2(42,4)),
            new Chest("Town", new Vector2(2, 69)),
            new Chest("JojaMart", new Vector2(10, 27)),
            new Chest("JojaMart", new Vector2(6, 27)),
            new Chest("Town", new Vector2(98, 4)),
            new Chest("Town", new Vector2(39, 13)),
            new Chest("Town", new Vector2(69, 11)),
            new Chest("SebastianRoom", new Vector2(9, 11)),
            new Chest("ScienceHouse", new Vector2(9, 7)),
            new Chest("Mountain", new Vector2(31, 5)),
            new Chest("BathHouse_MensLocker", new Vector2(6, 24)),
            new Chest("BathHouse_WomensLocker", new Vector2(11, 22)),
            new Chest("Railroad", new Vector2(32,40)),
            new Chest("Mine", new Vector2(22, 6)),
            new Chest("Backwoods", new Vector2(17, 11)),
            new Chest("FarmCave", new Vector2(5, 9)),
            new Chest("Beach", new Vector2(10, 39)),
            new Chest("LeahHouse", new Vector2(13, 5)),
            new Chest("Forest", new Vector2(71, 48)),
            new Chest("Forest", new Vector2(69, 84)),
            new Chest("Forest", new Vector2(36, 82)),
            new Chest("Forest", new Vector2(16, 76)),
            new Chest("WizardHouse", new Vector2(2, 7)),
            new Chest("Forest", new Vector2(3, 4)),
            new Chest("Woods", new Vector2(12, 7)),
            new Chest("Farm", new Vector2(71, 15)),
            new Chest("Backwoods", new Vector2(45, 16)),
            new Chest("Mountain", new Vector2(60, 7)),
            new Chest("Mountain", new Vector2(89, 33)),
            new Chest("ScienceHouse", new Vector2(30, 12)),
            new Chest("Town", new Vector2(88, 3)),
            new Chest("Blacksmith", new Vector2(12, 14)),
            new Chest("Town", new Vector2(91, 104)),
            new Chest("JoshHouse", new Vector2( 3, 5)),
            new Chest("Town", new Vector2(3, 65)),
            new Chest("Farm", new Vector2(8, 8)),
            new Chest("Forest", new Vector2(35, 25)),
            new Chest("AnimalShop",new Vector2(8, 6)),
            new Chest("Sewer", new Vector2(16, 28)),
            new Chest("BugLand", new Vector2(48, 46)),
            new Chest("BugLand", new Vector2(17, 29)),
            new Chest("BugLand", new Vector2(31, 5)),
            new Chest("Beach", new Vector2(83, 38)),
            new Chest("SandyHouse", new Vector2(4, 4)),
            new Chest("SkullCave", new Vector2(3, 5)),
            new Chest("Desert", new Vector2(6, 35)),
            new Chest("Backwoods", new Vector2(27, 27)),
            new Chest("SeedShop", new Vector2(37, 18)),
            new Chest("Forest",new Vector2(40, 6)),
            new Chest("Mountain", new Vector2(82, 9)),
            new Chest("BathHouse_Entry", new Vector2(4, 4)),
            new Chest("Town", new Vector2(91, 104)),
            new Chest("CommunityCenter", new Vector2(16, 4)),
            new Chest("CommunityCenter", new Vector2(65, 14)),
            new Chest("Hospital", new Vector2(13, 5)),
            new Chest("ElliottHouse", new Vector2(11, 4)),
            new Chest("Beach", new Vector2(78, 15)),
            new Chest("FishShop", new Vector2(8, 5)),
            new Chest("Beach", new Vector2(26, 4)),
            new Chest("Beach", new Vector2(47, 20)),
            new Chest("BusStop", new Vector2(9, 16)),
            new Chest("Desert", new Vector2(30, 23)),
            new Chest("Desert", new Vector2(42, 54)),
            new Chest("SandyHouse", new Vector2(4, 4)),
            new Chest("ArchaeologyHouse", new Vector2(22, 4)),
            new Chest("Sewer", new Vector2(31, 15)),
            new Chest("Sewer", new Vector2(2, 20)),
            new Chest("Town", new Vector2(91, 104)),
            new Chest("Town", new Vector2(91, 104)),
            new Chest("BugLand", new Vector2(91, 104)),
            new Chest("BugLand", new Vector2(50, 12)),
            new Chest("BugLand", new Vector2(34, 29)),
            new Chest("BugLand", new Vector2(17, 29)),
            new Chest("BugLand", new Vector2(48, 46)),
            new Chest("Forest", new Vector2(83, 82)),
            new Chest("Forest", new Vector2(80, 66)),
            new Chest("Forest", new Vector2(7, 65)),
            new Chest("Forest", new Vector2(17, 36)),
            new Chest("Forest", new Vector2(39, 11)),
            new Chest("Forest", new Vector2(74, 5)),
            new Chest("Forest", new Vector2(44, 46)),
        };

        public static List<Chest> IslandChestSpawns = new()
        {
            new Chest("IslandSouth", new Vector2(6, 32)),
            new Chest("IslandSouth", new Vector2(34, 27)),
            new Chest("IslandSouth", new Vector2(28, 11)),
            new Chest("IslandSouth", new Vector2(6, 9)),
            new Chest("IslandFarmHouse", new Vector2(27, 12)),
            new Chest("IslandFarmHouse", new Vector2(25, 4)),
            new Chest("IslandFarmHouse", new Vector2(6, 4)),
            new Chest("IslandWest", new Vector2(71, 37)),
            new Chest("IslandFarmCave", new Vector2(5, 5)),
            new Chest("IslandWest", new Vector2(97, 34)),
            new Chest("IslandWest", new Vector2(74, 29)),
            new Chest("IslandWest", new Vector2(88, 14)),
            new Chest("IslandWest", new Vector2(72, 5)),
            new Chest("IslandWest", new Vector2(48, 13)),
            new Chest("IslandWestCave1", new Vector2(6, 5)),
            new Chest("IslandWest", new Vector2(40, 24)),
            new Chest("IslandWest", new Vector2(53, 31)),
            new Chest("IslandWest", new Vector2(33, 38)),
            new Chest("IslandWest", new Vector2(20, 23)),
            new Chest("IslandWest", new Vector2(24, 42)),
            new Chest("IslandWest", new Vector2(27, 59)),
            new Chest("IslandWest", new Vector2(38, 81)),
            new Chest("IslandWest", new Vector2(58, 73)),
            new Chest("IslandWest", new Vector2(96, 76)),
            new Chest("IslandWest", new Vector2(38, 51)),
            new Chest("IslandWest", new Vector2(61, 64)),
            new Chest("CaptainRoom", new Vector2(4, 4)),
            new Chest("IslandWest", new Vector2(45, 64)),
            new Chest("IslandWest", new Vector2(14, 4)),
            new Chest("IslandEast", new Vector2(7, 44)),
            new Chest("IslandEast", new Vector2(26, 38)),
            new Chest("IslandEast", new Vector2(12, 29)),
            new Chest("IslandEast", new Vector2(24, 25)),
            new Chest("IslandEast", new Vector2(21, 11)),
            new Chest("IslandHut", new Vector2(7, 7)),
            new Chest("IslandShrine", new Vector2(24, 23)),
            new Chest("IslandSouthEast", new Vector2(9, 15)),
            new Chest("IslandSouthEast", new Vector2(22, 12)),
            new Chest("IslandSouthEast", new Vector2(28, 24)),
            new Chest("IslandSouthEastCave", new Vector2(6, 5)),
            new Chest("IslandSouthEastCave", new Vector2(26, 5)),
            new Chest("IslandSouthEastCave", new Vector2(27, 17)),
            new Chest("IslandSouthEastCave", new Vector2(24, 25)),
            new Chest("IslandNorth", new Vector2(29, 73)),
            new Chest("IslandNorth", new Vector2(49, 77)),
            new Chest("IslandNorth", new Vector2(60, 77)),
            new Chest("IslandNorth", new Vector2(58, 58)),
            new Chest("IslandNorth", new Vector2(51, 49)),
            new Chest("IslandNorth", new Vector2(36, 52)),
            new Chest("IslandFieldOffice", new Vector2(6, 4)),
            new Chest("IslandNorth", new Vector2(23, 48)),
            new Chest("IslandNorth", new Vector2(11, 61)),
            new Chest("IslandNorth", new Vector2(7, 47)),
            new Chest("IslandNorth", new Vector2(24, 35)),
            new Chest("IslandNorth", new Vector2(7, 41)),
            new Chest("VolcanoDungeon0", new Vector2(41, 42)),
            new Chest("IslandNorth", new Vector2(32, 22)),
            new Chest("IslandNorth", new Vector2(48, 23)),
            new Chest("IslandNorth", new Vector2(62, 16)),
            new Chest("IslandNorth", new Vector2(51, 29)),
            new Chest("IslandNorth", new Vector2(13, 15)),
            new Chest("IslandNorth", new Vector2(46, 39)),
            new Chest("Caldera", new Vector2(23, 24)),
            new Chest("IslandNorthCave1", new Vector2(6, 4))
        };
    }
}
