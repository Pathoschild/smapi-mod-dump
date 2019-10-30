using System.Collections.Generic;
using StardewModdingAPI;

namespace FarmTypeManager
{
    public partial class ModEntry : Mod
    {
        //contains configuration settings for ore generation behavior
        private class OreSettings
        {

            public OreSpawnArea[] Areas { get; set; }
            public int PercentExtraSpawnsPerMiningLevel { get; set; }
            public Dictionary<string, int> MiningLevelRequired { get; set; }
            public Dictionary<string, int> StartingSpawnChance { get; set; }
            public Dictionary<string, int> LevelTenSpawnChance { get; set; }
            public int[] CustomTileIndex { get; set; }

            //default constructor: configure default ore generation settings
            public OreSettings()
            {

                Areas = new OreSpawnArea[] { new OreSpawnArea() }; //a set of "OreSpawnArea" objects, describing where ore can spawn
                PercentExtraSpawnsPerMiningLevel = 0; //multiplier to give extra ore per level of mining skill; default is +0%, since the native game lacks this mechanic

                //mining skill level required to spawn each ore type; defaults are based on the vanilla "hilltop" map settings, though some types didn't spawn at all
                MiningLevelRequired = new Dictionary<string, int>
                {
                    { "Stone", 0 },
                    { "Geode", 0 },
                    { "FrozenGeode", 5 },
                    { "MagmaGeode", 8 },
                    { "Gem", 6 },
                    { "Copper", 0 },
                    { "Iron", 4 },
                    { "Gold", 7 },
                    { "Iridium", 9 },
                    { "Mystic", 10 },
                    { "Diamond", 6 },
                    { "Ruby", 6 },
                    { "Jade", 6 },
                    { "Amethyst", 6 },
                    { "Topaz", 6 },
                    { "Emerald", 6 },
                    { "Aquamarine", 6 }
                };

                //weighted chance to spawn ore at the minimum required skill level (e.g. by default, iron starts spawning at level 4 mining skill with a 15% chance, but is 0% before that)
                StartingSpawnChance = new Dictionary<string, int>
                {
                    { "Stone", 66 },
                    { "Geode", 8 },
                    { "FrozenGeode", 4 },
                    { "MagmaGeode", 2 },
                    { "Gem", 5 },
                    { "Copper", 21 },
                    { "Iron", 15 },
                    { "Gold", 10 },
                    { "Iridium", 1 },
                    { "Mystic", 1 },
                    { "Diamond", 0 },
                    { "Ruby", 0 },
                    { "Jade", 0 },
                    { "Amethyst", 0 },
                    { "Topaz", 0 },
                    { "Emerald", 0 },
                    { "Aquamarine", 0 }
                };

                //weighted chance to spawn ore at level 10 mining skill; for any levels in between "starting" and level 10, the odds are gradually adjusted (e.g. by default, stone is 66% at level 0, 57% at level 5, and 48% at level 10)
                LevelTenSpawnChance = new Dictionary<string, int>
                {
                    { "Stone", 48 },
                    { "Geode", 2 },
                    { "FrozenGeode", 2 },
                    { "MagmaGeode", 2 },
                    { "Gem", 5 },
                    { "Copper", 16 },
                    { "Iron", 13 },
                    { "Gold", 10 },
                    { "Iridium", 1 },
                    { "Mystic", 1 },
                    { "Diamond", 0 },
                    { "Ruby", 0 },
                    { "Jade", 0 },
                    { "Amethyst", 0 },
                    { "Topaz", 0 },
                    { "Emerald", 0 },
                    { "Aquamarine", 0 }
                };

                CustomTileIndex = new int[0]; //an extra list of tilesheet indices, for those who want to use their own custom terrain type
            }
        }
    }
}