using StardewModdingAPI;

namespace FarmTypeManager
{
    public partial class ModEntry : Mod
    {
        //per-character configuration file, e.g. PlayerName12345.json; contains most of the mod's functional settings, split up this way to allow for different settings between saves & farm types
        private class FarmConfig
        {
            public bool ForageSpawnEnabled { get; set; }
            public bool LargeObjectSpawnEnabled { get; set; }
            public bool OreSpawnEnabled { get; set; }
            public bool MonsterSpawnEnabled { get; set; }

            public ForageSettings Forage_Spawn_Settings { get; set; }
            public LargeObjectSettings Large_Object_Spawn_Settings { get; set; }
            public OreSettings Ore_Spawn_Settings { get; set; }
            public MonsterSettings Monster_Spawn_Settings { get; set; }

            /// <summary>A list of tile index numbers for the Hilltop Farm's quarry areas.</summary>
            public int[] QuarryTileIndex { get; set; } = { 556, 558, 583, 606, 607, 608, 630, 635, 636, 680, 681, 685 };

            /// <summary>A group of settings for conditionally disabling this config file.</summary>
            public FileConditions File_Conditions { get; set; } = new FileConditions();

            public FarmConfig()
            {
                //basic on/off toggles
                ForageSpawnEnabled = false;
                LargeObjectSpawnEnabled = false;
                OreSpawnEnabled = false;
                MonsterSpawnEnabled = false;

                //settings for each generation type (assigned in the default constructor for each of these "Settings" objects; see those for details)
                Forage_Spawn_Settings = new ForageSettings();
                Large_Object_Spawn_Settings = new LargeObjectSettings();
                Ore_Spawn_Settings = new OreSettings();
                Monster_Spawn_Settings = new MonsterSettings();
            }
        }
    }
}