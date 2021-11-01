/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tylergibbs2/BattleRoyalley-Year2
**
*************************************************/

using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BattleRoyale
{
    class Spawns
    {
        public Dictionary<Farmer, TileLocation> ScatterPlayers(IEnumerable<Farmer> players, bool gingerIsland = false)
        {
            List<TileLocation> spawnList;
            if (ModEntry.BRGame.IsSpecialRoundType(SpecialRoundType.SLUGFEST))
                spawnList = SpawnLocationsDataModel.SlugFestSpawns.ToList();
            else if (gingerIsland)
                spawnList = SpawnLocationsDataModel.IslandSpawns.ToList();
            else
                spawnList = SpawnLocationsDataModel.Spawns.ToList();

            var random = new Random();
            var viableSpawns = spawnList.OrderBy(x => random.NextDouble());

            var enumerator = players.GetEnumerator();

            var chosenSpawns = new Dictionary<Farmer, TileLocation>();

            foreach (TileLocation spawnLocation in viableSpawns)
            {
                if (enumerator.MoveNext())
                    chosenSpawns.Add(enumerator.Current, spawnLocation);
                else
                    return chosenSpawns;
            }

            //There are more players than spawn locations. We will have to spawn players on top of eachother
            while (enumerator.MoveNext())
                chosenSpawns.Add(enumerator.Current, spawnList[random.Next(spawnList.Count)]);

            return chosenSpawns;
        }
    }

    class SpawnLocationsDataModel
    {

        public static List<TileLocation> Spawns { get; set; } = new List<TileLocation>()
        {
            new TileLocation("Town", 16, 14),
            new TileLocation("Town", 63, 17),
            new TileLocation("Town", 60, 65),
            new TileLocation("Town", 46, 92),
            new TileLocation("Town", 92, 85),
            new TileLocation("ArchaeologyHouse", 11, 11),
            new TileLocation("SamHouse", 4, 18),
            new TileLocation("Beach", 29, 35),
            new TileLocation("Mountain", 32, 9),
            new TileLocation("Mountain", 56, 32),
            new TileLocation("Mountain", 64, 10),
            new TileLocation("Railroad", 7, 43),
            new TileLocation("Railroad", 62, 44),
            new TileLocation("Backwoods", 20, 10),
            new TileLocation("FarmCave", 6, 7),
            new TileLocation("Forest", 79, 16),
            new TileLocation("Forest", 5, 29),
            new TileLocation("Forest", 8, 80),
            new TileLocation("Forest", 41, 75),
            new TileLocation("Forest", 99, 92),
            new TileLocation("Forest", 84, 50),
            new TileLocation("Farm", 28, 16),
            new TileLocation("Desert", 37, 11),
            new TileLocation("Desert", 2, 51),
            new TileLocation("Backwoods", 39, 30),
            new TileLocation("BusStop", 32, 6),
            new TileLocation("Hospital", 4, 5),
            new TileLocation("SeedShop", 20, 13),
            new TileLocation("JoshHouse", 3, 19),
            new TileLocation("CommunityCenter", 33, 13),
            new TileLocation("Mine", 13, 10),
            new TileLocation("Sewer", 16, 21),
            new TileLocation("WizardHouse", 7, 17),
            new TileLocation("Farm", 73, 14),
            new TileLocation("JojaMart", 14, 7),
            new TileLocation("SebastianRoom", 6, 7),
            new TileLocation("Mountain", 12, 30),
            new TileLocation("Mountain", 9, 11),
            new TileLocation("Town", 53, 72),
            new TileLocation("Town", 42, 79),
            new TileLocation("Town", 11, 72),
            new TileLocation("Town", 53, 72),
            new TileLocation("Town", 25, 51),
            new TileLocation("Town", 26, 30),
            new TileLocation("Town", 39, 19),
            new TileLocation("Town", 26, 30),
            new TileLocation("ManorHouse", 3, 7),
            new TileLocation("Town", 79, 94),
            new TileLocation("Town", 104, 79),
            new TileLocation("Town", 104, 89),
            new TileLocation("Blacksmith", 5, 14),
            new TileLocation("Town", 91, 51),
            new TileLocation("Town", 104, 41),
            new TileLocation("Town", 98, 20),
            new TileLocation("Town", 86, 20),
            new TileLocation("Town", 63, 30),
            new TileLocation("Town", 22, 78),
            new TileLocation("HaleyHouse", 5, 16),
            new TileLocation("HaleyHouse", 16, 19),
            new TileLocation("Town", 17, 90),
            new TileLocation("Town", 35, 99),
            new TileLocation("Town", 62, 94),
            new TileLocation("Saloon", 34, 17),
            new TileLocation("Saloon", 6, 17),
            new TileLocation("Trailer", 14, 5),
            new TileLocation("ElliottHouse", 3, 6),
            new TileLocation("Beach", 68, 6),
            new TileLocation("Beach", 89, 6),
            new TileLocation("Beach", 88, 22),
            new TileLocation("Beach", 67, 21),
            new TileLocation("Beach", 87, 38),
            new TileLocation("Beach", 42, 16),
            new TileLocation("Beach", 17, 11),
            new TileLocation("BusStop", 14, 23),
            new TileLocation("BusStop", 13, 4),
            new TileLocation("Desert", 14, 17),
            new TileLocation("Desert", 35, 43),
            new TileLocation("BackWoods", 13, 25),
            new TileLocation("BackWoods", 42, 14),
            new TileLocation("Sewer", 3, 30),
            new TileLocation("BugLand", 30, 46),
            new TileLocation("BugLand", 44, 41),
            new TileLocation("BugLand", 23, 23),
            new TileLocation("BugLand", 42, 19),
            new TileLocation("Forest", 68, 64),
            new TileLocation("Forest", 11, 12),
            new TileLocation("Forest", 51, 14),
            new TileLocation("Forest", 94, 33),
            new TileLocation("Forest", 55, 33)
        };

        public static List<TileLocation> SlugFestSpawns { get; set; } = new List<TileLocation>()
        {
            new TileLocation("Town", 30, 67)
        };

        public static List<TileLocation> IslandSpawns { get; set; } = new List<TileLocation>()
        {
            new TileLocation("IslandSouth", 32, 28),
            new TileLocation("IslandSouth", 20, 24),
            new TileLocation("IslandSouth", 12, 28),
            new TileLocation("IslandSouth", 7, 13),
            new TileLocation("IslandSouth", 19, 8),
            new TileLocation("IslandEast", 7, 47),
            new TileLocation("IslandEast", 27, 40),
            new TileLocation("IslandEast", 16, 31),
            new TileLocation("IslandEast", 22, 18),
            new TileLocation("IslandShrine", 19, 27),
            new TileLocation("IslandWest", 96, 37),
            new TileLocation("IslandWest", 94, 60),
            new TileLocation("IslandWest", 14, 12),
            new TileLocation("IslandWest", 70, 76),
            new TileLocation("IslandWest", 94, 80),
            new TileLocation("IslandWest", 45, 74),
            new TileLocation("IslandWest", 26, 62),
            new TileLocation("IslandWest", 26, 44),
            new TileLocation("IslandWest", 45, 21),
            new TileLocation("IslandWestCave1", 6, 9),
            new TileLocation("IslandWest", 73, 14),
            new TileLocation("IslandWest", 86, 29),
            new TileLocation("IslandSouthEast", 12, 23),
            new TileLocation("IslandSouthEast", 26, 16),
            new TileLocation("IslandSouthEastCave", 26, 7),
            new TileLocation("IslandSouthEastCave", 20, 25),
            new TileLocation("IslandNorth", 36, 78),
            new TileLocation("IslandNorth", 57, 80),
            new TileLocation("IslandNorth", 60, 62),
            new TileLocation("IslandNorth", 46, 50),
            new TileLocation("IslandNorth", 58, 21),
            new TileLocation("IslandNorth", 20, 15),
            new TileLocation("IslandNorth", 27, 56),
            new TileLocation("IslandNorth", 8, 49),
            new TileLocation("IslandNorth", 19, 37),
            new TileLocation("VolcanoDungeon0", 31, 45),
            new TileLocation("Caldera", 25, 28),
            new TileLocation("IslandNorth", 12, 83),
            new TileLocation("IslandNorth", 21, 40),
        };
    }
}
