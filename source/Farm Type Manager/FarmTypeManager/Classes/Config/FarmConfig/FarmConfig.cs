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

            public int[] QuarryTileIndex { get; set; }

            public FileConditions File_Conditions { get; set; }

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

                //a list of every tilesheet index commonly used by "quarry" tiles on maps, e.g. the vanilla hilltop (mining) farm
                //these should be compared to [an instance of GameLocation].getTileIndexAt(x, y, "Back")
                QuarryTileIndex = new int[] { 556, 558, 583, 606, 607, 608, 630, 635, 636, 680, 681, 685 };
                //NOTE: swap in the following code to cover more tiles, e.g. the grassy edges of the "quarry" dirt; this tends to cover too much ground and weird spots, though, such as the farm's cave entrance
                //{ 556, 558, 583, 606, 607, 608, 630, 631, 632, 633, 634, 635, 636, 654, 655, 656, 657, 658, 659, 679, 680, 681, 682, 683, 684, 685, 704, 705, 706, 707 };

                File_Conditions = new FileConditions(); //settings determining whether this config is used for a given farm
            }
        }
    }
}