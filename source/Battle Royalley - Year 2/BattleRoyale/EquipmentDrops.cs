/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tylergibbs2/BattleRoyalley-Year2
**
*************************************************/

using BattleRoyale.Utils;
using Microsoft.Xna.Framework;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BattleRoyale
{
    class EquipmentDrop
    {
        public GameLocation Location { get; set; }

        public Vector2 TilePosition { get; set; }

        public string AnnounceMessage { get; set; }

        public StardewValley.Objects.Chest StardewChest { get; set; } = null;

        public EquipmentDrop(string location, Vector2 tilePosition, string announceMessage)
        {
            Location = Game1.getLocationFromName(location);
            TilePosition = tilePosition;
            AnnounceMessage = announceMessage;
        }
    }

    enum EquipmentTier : int
    {
        A_TIER = 0,
        B_TIER = 1
    }

    class EquipmentDrops
    {
        private static List<EquipmentDrop> drops;
        private static readonly List<EquipmentDrop> spawnedDrops = new List<EquipmentDrop>();

        private static EquipmentDrop activeDrop;
        private static DateTime dropActivatedAt;

        // Announced at specified time, spawns later.
        private static readonly List<int> announceTimes = new List<int>() // Measured in seconds
        {
            25, 50, 70, 90
        };

        private static readonly int spawnAfterAnnounceTime = 5;

        private static readonly int itemCount = 2;

        private static readonly Dictionary<EquipmentTier, double> LootWeights = new Dictionary<EquipmentTier, double>()
        {
            { EquipmentTier.A_TIER, 0.2 },
            { EquipmentTier.B_TIER, 0.8 }
        };

        private static readonly Dictionary<EquipmentTier, int[]> Loot = new Dictionary<EquipmentTier, int[]>()
        {
            { EquipmentTier.A_TIER, new int[]{ 524, 810, 861, 863, 855 } },
            { EquipmentTier.B_TIER, new int[]{ 529, 533, 534, 839, 508 } }
        };

        private static readonly List<int> bootIds = new List<int> { 855, 508 };

        public static EquipmentTier GetRandomEquipmentTier()
        {
            double total = 0;
            double amount = Game1.random.NextDouble();

            foreach (var kvp in LootWeights)
            {
                EquipmentTier tier = kvp.Key;
                double weight = kvp.Value;

                total += weight;
                if (amount <= total)
                    return tier;
            }

            return EquipmentTier.B_TIER;
        }

        public static void CreateStardewChest(EquipmentDrop drop)
        {
            StardewValley.Objects.Chest stardewChest = new StardewValley.Objects.Chest(true, parentSheedIndex: 232);

            for (int i = 0; i < itemCount; i++)
            {
                EquipmentTier tier = GetRandomEquipmentTier();
                int randomIdx = Game1.random.Next(Loot[tier].Length);
                int itemId = Loot[tier][randomIdx];

                if (bootIds.Contains(itemId))
                {
                    StardewValley.Objects.Boots boots = new StardewValley.Objects.Boots(itemId);
                    stardewChest.items.Add(boots);
                }
                else
                {
                    StardewValley.Objects.Ring ring = new StardewValley.Objects.Ring(itemId);
                    stardewChest.items.Add(ring);
                }
            }

            // Place chest on map
            drop.Location.setObject(drop.TilePosition, stardewChest);
            drop.StardewChest = stardewChest;
        }
        public static bool CanSpawnDrop(EquipmentDrop drop)
        {
            Tuple<int, double> stormPhase = Storm.GetCurrentPhase();
            var amount = Storm.GetStormAmountInLocation(stormPhase.Item1, stormPhase.Item2, drop.Location);
            if (amount.Item1 > 0 || spawnedDrops.Contains(drop) || activeDrop == drop) // Item1 = amount as percentage
                return false;

            return true;
        }

        public static EquipmentDrop GetAvailableDrop()
        {
            if (drops == null)
                return null;

            foreach (EquipmentDrop drop in drops)
            {
                if (CanSpawnDrop(drop))
                    return drop;
            }

            return null;
        }

        public static void Reset(bool gingerIsland = false)
        {
            spawnedDrops.Clear();

            if (gingerIsland)
                drops = EquipmentDropsDataModel.IslandEquipmentDropSpawns.ToList();
            else
                drops = EquipmentDropsDataModel.EquipmentDropSpawns.ToList();

            drops.Shuffle();
        }

        public static bool DoEquipmentDrops()
        {
            return (
                !ModEntry.BRGame.IsSpecialRoundType(SpecialRoundType.SLUGFEST) &&
                !ModEntry.BRGame.IsSpecialRoundType(SpecialRoundType.TRAVELLING_CART) &&
                !ModEntry.BRGame.IsSpecialRoundType(SpecialRoundType.SLINGSHOT_ONLY) &&
                !ModEntry.BRGame.IsSpecialRoundType(SpecialRoundType.BOMBS_ONLY)
            );
        }

        public static void Check()
        {
            if (!DoEquipmentDrops())
                return;

            Round round = ModEntry.BRGame.GetActiveRound();

            if (spawnedDrops.Count == announceTimes.Count)
                return;

            if (activeDrop == null)
            {
                int waitingFor = announceTimes[spawnedDrops.Count];
                TimeSpan duration = (TimeSpan)(DateTime.Now - round?.StartTime);
                if (duration.TotalSeconds < waitingFor)
                    return;

                EquipmentDrop drop = GetAvailableDrop();
                if (drop == null)
                    return;

                activeDrop = drop;
                dropActivatedAt = DateTime.Now;

                NetworkUtils.SendChatMessageToAllPlayers(drop.AnnounceMessage);
            }
            else
            {
                TimeSpan duration = DateTime.Now - dropActivatedAt;

                if (duration.TotalSeconds < spawnAfterAnnounceTime)
                    return;

                CreateStardewChest(activeDrop);

                spawnedDrops.Add(activeDrop);
                activeDrop = null;
            }
        }
    }

    class EquipmentDropsDataModel
    {
        public static List<EquipmentDrop> EquipmentDropSpawns { get; set; } = new List<EquipmentDrop>()
        {
            new EquipmentDrop("Town", new Vector2(51, 21), "The Junimos have been busy! Equipment chest is about to be dropped outside the Community Center."),
            new EquipmentDrop("Town", new Vector2(95, 82), "Forged in the flames! Equipment chest is about to be dropped outside Clint's."),
            new EquipmentDrop("Town", new Vector2(57, 87), "Paid for by your taxes: Equipment chest is about to be dropped outside the Mayor's Manor."),
            new EquipmentDrop("Town", new Vector2(47, 88), "Grandpa left one last present: Equipment chest is about to be dropped in the graveyard."),
            new EquipmentDrop("Town", new Vector2(21, 89), "Beauty products? Equipment chest is about to be dropped outside Emily and Haley's house."),
            new EquipmentDrop("Town", new Vector2(9, 86), "Kent's military surplus! Equipment chest is about to be dropped outside Kent's house."),
            new EquipmentDrop("Town", new Vector2(97, 52), "New shipment at Jojamart! Equipment chest is about to be dropped outside Jojamart."),
            new EquipmentDrop("Town", new Vector2(42, 57), "Pierre has something special! Equipment chest is about to be dropped outside Pierre's."),
            new EquipmentDrop("Town", new Vector2(44, 72), "Get it while it's cold! Equipment chest is about to be dropped outside the Saloon."),
            new EquipmentDrop("Town", new Vector2(16, 13), "Get swingin'! Equipment chest is about to be dropped at the playground."),
            new EquipmentDrop("Town", new Vector2(32, 67), "Free for all! Equipment chest is about to be dropped at the town square."),
            new EquipmentDrop("Town", new Vector2(35, 57), "Pick up your prescription! Equipment chest is about to be dropped outside of the hospital."),
            new EquipmentDrop("Town", new Vector2(102, 90), "Gunther dusted off some old antiques. Equipment chest is about to be dropped outside the Museum."),
            new EquipmentDrop("Mountain", new Vector2(55, 6), "Item discovered at the mine! Equipment chest is about to be dropped outside the mine."),
            new EquipmentDrop("Mountain", new Vector2(13, 30), "Robin's latest creation! Equipment chest is about to be dropped outside Robin's."),
            new EquipmentDrop("Mountain", new Vector2(77, 12), "It's dangerous to go alone--take this! Equipment chest is about to be dropped outside the Adventurer's Guild."),
            new EquipmentDrop("Desert", new Vector2(8, 52), "Nice and refreshing! Equipment chest is about to be dropped outside the Oasis."),
            new EquipmentDrop("Desert", new Vector2(27, 23), "Uh-oh! Bags left at the bus stop? Equipment chest is about to be dropped at the desert bus stop."),
            new EquipmentDrop("Beach", new Vector2(50, 11), "Pelican Town's best selling author released a new book! Equipment chest is about to be dropped outside Elliot's shack."),
            new EquipmentDrop("Beach", new Vector2(32, 34), "Something salty? Equipment chest is about to be dropped outside Willy's."),
            new EquipmentDrop("Forest", new Vector2(89, 17), "Not for animal consumption: Equipment chest is about to be dropped outside Marnie's."),
            new EquipmentDrop("Forest", new Vector2(105, 33), "Leah's latest artistic masterpiece: Equipment chest is about to be dropped outside Leah's place."),
            new EquipmentDrop("Farm", new Vector2(69, 16), "You've got mail! Equipment chest is about to be dropped at the mailbox."),
            new EquipmentDrop("Sewer", new Vector2(18, 11), "Something funky found in the sewers... Equipment chest is about to be dropped in the sewer."),
            new EquipmentDrop("Railroad", new Vector2(11, 57), "Squeeky clean item! Equipment chest is about to be dropped outside of the bathhouse."),
            new EquipmentDrop("BusStop", new Vector2(13, 10), "Travelling soon? Equipment chest is about to be dropped at the Pelican Town bus stop."),
            new EquipmentDrop("Backwoods", new Vector2(18, 13), "The squirrels left some nuts! Equipment chest is about to be dropped in the backwoods."),
            new EquipmentDrop("Woods", new Vector2(10, 7), "The sweetest taste: Equipment chest is about to be dropped in the secret woods."),
            new EquipmentDrop("Railroad", new Vector2(32, 47), "Choo choo! Equipment chest is about to be dropped at the train station.")
        };

        public static List<EquipmentDrop> IslandEquipmentDropSpawns = new List<EquipmentDrop>()
        {
            new EquipmentDrop("IslandWest", new Vector2(75, 42), "Parrot's left you some mail! Equipment chest is about to be dropped outside the Island farm house."),
            new EquipmentDrop("IslandWest", new Vector2(95, 35), "Gourmand Frog was happy with your crops, he left you a reward. Equipment chest is about to be dropped outside the Gourmand's cave."),
            new EquipmentDrop("IslandWest", new Vector2(57, 89), "Supplies washed up on shore! Equipment chest is about to be dropped outside the abandoned shipwreck."),
            new EquipmentDrop("IslandWest", new Vector2(18, 25), "Mr. Qi is pleased with your progress. Equipment chest is about to be dropped outside Mr. Qi's nut door."),
            new EquipmentDrop("IslandWest", new Vector2(60, 7), "Time to get musical! Equipment chest is about to be dropped outside the musical crystal cave."),
            new EquipmentDrop("IslandSouth", new Vector2(14, 23), "Freshly made smoothies! Equipment chest is about to be dropped outside the resort."),
            new EquipmentDrop("IslandSouthEastCave", new Vector2(6, 5), "The pirates forgot their booty! Equipment chest is about to be dropped inside the pirate's cave."),
            new EquipmentDrop("IslandHut", new Vector2(11, 9), "Leo scavenged some equipment! Equipment chest is about to be dropped inside Leo's hut."),
            new EquipmentDrop("IslandShrine", new Vector2(20, 27), "The shrine is pleased with your offerings! Equipment chest is about to be dropped at the Island Shrine."),
            new EquipmentDrop("IslandNorth", new Vector2(45, 47), "Professor Snail's latest findings! Equipment chest is about to be dropped outside the Island Field House."),
            new EquipmentDrop("IslandNorth", new Vector2(38, 26), "The volcano's about to erupt! Equipment chest is about to be dropped outside the volcano."),
        };
    }
}
