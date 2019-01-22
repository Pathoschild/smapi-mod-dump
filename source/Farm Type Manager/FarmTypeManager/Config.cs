using System.Collections.Generic;

namespace FarmTypeManager
{
    //config.json, used by all players/saves for shared functions
    class ModConfig
    {
        public bool EnableWhereAmICommand { get; set; }

        public ModConfig()
        {
            EnableWhereAmICommand = true; //should enable the "whereami" command in the SMAPI console
        }
    }

    //per-character configuration file, e.g. PlayerName12345.json; contains most of the mod's functional settings, split up this way to allow for different settings between saves & farm types
    class FarmConfig
    {
        public bool ForageSpawnEnabled { get; set; }
        public bool LargeObjectSpawnEnabled { get; set; }
        public bool OreSpawnEnabled { get; set; }

        public ForageSettings Forage_Spawn_Settings { get; set; }
        public LargeObjectSettings Large_Object_Spawn_Settings { get; set; }
        public OreSettings Ore_Spawn_Settings { get; set; }

        public int[] QuarryTileIndex { get; set; }

        public FarmConfig()
        {
            //basic on/off toggles
            ForageSpawnEnabled = false;
            LargeObjectSpawnEnabled = false;
            OreSpawnEnabled = false;

            //settings for each generation type (assigned in the constructor for each of these "Settings" objects; see those for details)
            Forage_Spawn_Settings = new ForageSettings();
            Large_Object_Spawn_Settings = new LargeObjectSettings();
            Ore_Spawn_Settings = new OreSettings();

            //a list of every tilesheet index commonly used by "quarry" tiles on maps, e.g. the vanilla hilltop (mining) farm
            //these should be compared to [an instance of GameLocation].getTileIndexAt(x, y, "Back")
            QuarryTileIndex = new int[] { 556, 558, 583, 606, 607, 608, 630, 635, 636, 680, 681, 685 };
            //NOTE: swap in the following code to cover more tiles, e.g. the grassy edges of the "quarry" dirt; this tends to cover too much ground and weird spots, though, such as the farm's cave entrance
            //{ 556, 558, 583, 606, 607, 608, 630, 631, 632, 633, 634, 635, 636, 654, 655, 656, 657, 658, 659, 679, 680, 681, 682, 683, 684, 685, 704, 705, 706, 707 };
        }
    }

    //contains configuration settings for forage item generation behavior
    public class ForageSettings
    {
        public ForageSpawnArea[] Areas { get; set; }
        public int PercentExtraSpawnsPerForagingLevel { get; set; }
        public int[] SpringItemIndex { get; set; }
        public int[] SummerItemIndex { get; set; }
        public int[] FallItemIndex { get; set; }
        public int[] WinterItemIndex { get; set; }
        public int[] CustomTileIndex { get; set; }

        //default constructor: configure default forage generation settings
        public ForageSettings()
        {
            Areas = new ForageSpawnArea[] { new ForageSpawnArea("Farm", 0, 3, new string[] { "Grass", "Diggable" }, new string[0], new string[0], "High", null, null, null, null) }; //a set of "SpawnArea" objects, describing where forage items can spawn on each map
            PercentExtraSpawnsPerForagingLevel = 0; //multiplier to give extra forage per level of foraging skill; default is +0%, since the native game lacks this mechanic

            //the "parentSheetIndex" values for each type of forage item allowed to spawn in each season (the numbers found in ObjectInformation.xnb)
            SpringItemIndex = new int[] { 16, 20, 22, 257 };
            SummerItemIndex = new int[] { 396, 398, 402, 404 };
            FallItemIndex = new int[] { 281, 404, 420, 422 };
            WinterItemIndex = new int[0];

            CustomTileIndex = new int[0]; //an extra list of tilesheet indices, for use by players who want to make some custom tile detection
        }
    }

    //contains configuration settings for spawning large objects (e.g. stumps and logs)
    public class LargeObjectSettings
    {
        public LargeObjectSpawnArea[] Areas { get; set; }
        public int[] CustomTileIndex { get; set; }

        public LargeObjectSettings()
        {
            Areas = new LargeObjectSpawnArea[] { new LargeObjectSpawnArea("Farm", 999, 999, new string[0], new string[0], new string[0], "High", new string[] { "Stump" }, true, 0, "Foraging") }; //a set of "LargeObjectSpawnArea", describing where large objects can spawn on each map
            CustomTileIndex = new int[0]; //an extra list of tilesheet indices, for use by players who want to make some custom tile detection
        }
    }

    //contains configuration settings for ore generation behavior
    public class OreSettings
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
            
            Areas = new OreSpawnArea[] { new OreSpawnArea("Farm", 1, 5, new string[] { "Quarry" }, new string[0], new string[0], "High", null, null, null) }; //a set of "OreSpawnArea" objects, describing where ore can spawn on each map
            PercentExtraSpawnsPerMiningLevel = 0; //multiplier to give extra ore per level of mining skill; default is +0%, since the native game lacks this mechanic

            //mining skill level required to spawn each ore type; defaults are based on the vanilla "hilltop" map settings, though some types didn't spawn at all
            MiningLevelRequired = new Dictionary<string, int>();
            MiningLevelRequired.Add("Stone", 0);
            MiningLevelRequired.Add("Geode", 0);
            MiningLevelRequired.Add("FrozenGeode", 5);
            MiningLevelRequired.Add("MagmaGeode", 8);
            MiningLevelRequired.Add("Gem", 6);
            MiningLevelRequired.Add("Copper", 0);
            MiningLevelRequired.Add("Iron", 4);
            MiningLevelRequired.Add("Gold", 7);
            MiningLevelRequired.Add("Iridium", 9);
            MiningLevelRequired.Add("Mystic", 10);
            
            //weighted chance to spawn ore at the minimum required skill level (e.g. by default, iron starts spawning at level 4 mining skill with a 15% chance, but is 0% before that)
            StartingSpawnChance = new Dictionary<string, int>();
            StartingSpawnChance.Add("Stone", 66);
            StartingSpawnChance.Add("Geode", 8);
            StartingSpawnChance.Add("FrozenGeode", 4);
            StartingSpawnChance.Add("MagmaGeode", 2);
            StartingSpawnChance.Add("Gem", 5);
            StartingSpawnChance.Add("Copper", 21);
            StartingSpawnChance.Add("Iron", 15);
            StartingSpawnChance.Add("Gold", 10);
            StartingSpawnChance.Add("Iridium", 1);
            StartingSpawnChance.Add("Mystic", 1);
            
            //weighted chance to spawn ore at level 10 mining skill; for any levels in between "starting" and level 10, the odds are gradually adjusted (e.g. by default, stone is 66% at level 0, 57% at level 5, and 48% at level 10)
            LevelTenSpawnChance = new Dictionary<string, int>();
            LevelTenSpawnChance.Add("Stone", 48);
            LevelTenSpawnChance.Add("Geode", 2);
            LevelTenSpawnChance.Add("FrozenGeode", 2);
            LevelTenSpawnChance.Add("MagmaGeode", 2);
            LevelTenSpawnChance.Add("Gem", 5);
            LevelTenSpawnChance.Add("Copper", 16);
            LevelTenSpawnChance.Add("Iron", 13);
            LevelTenSpawnChance.Add("Gold", 10);
            LevelTenSpawnChance.Add("Iridium", 1);
            LevelTenSpawnChance.Add("Mystic", 1);

            CustomTileIndex = new int[0]; //an extra list of tilesheet indices, for use by players who want to make some custom tile detection
        }
    }

    //a set of variables including a map name, terrain type(s) to auto-detect, and manually defined included/excluded areas for object spawning
    public class SpawnArea
    {
        public string MapName { get; set; }
        public int MinimumSpawnsPerDay { get; set; }
        public int MaximumSpawnsPerDay { get; set; }
        public string[] AutoSpawnTerrainTypes { get; set; } //Valid properties include "Quarry", "Custom", "Diggable", "All", and any tile Type properties ("Grass", "Dirt", "Stone", "Wood")
        public string[] IncludeAreas { get; set; }
        public string[] ExcludeAreas { get; set; }
        public string StrictTileChecking { get; set; } = "High"; //added in version 1.1; default used here to automatically fill it in with SMAPI's json reader

        public SpawnArea()
        {

        }

        public SpawnArea(string name, int min, int max, string[] types, string[] include, string[] exclude, string safety)
        {
            MapName = name;
            MinimumSpawnsPerDay = min;
            MaximumSpawnsPerDay = max;
            AutoSpawnTerrainTypes = types;
            IncludeAreas = include;
            ExcludeAreas = exclude;
            StrictTileChecking = safety;
        }
    }

    //a subclass of "SpawnArea" specifically for forage generation, providing the ability to override each area's seasonal forage items
    public class ForageSpawnArea : SpawnArea
    {
        public int[] SpringItemIndex { get; set; } = null; //added in version 1.2; default used here to automatically fill it in with SMAPI's json reader
        public int[] SummerItemIndex { get; set; } = null; //""
        public int[] FallItemIndex { get; set; } = null;   //""
        public int[] WinterItemIndex { get; set; } = null; //""

        public ForageSpawnArea()
            :base()
        {

        }

        public ForageSpawnArea(string name, int min, int max, string[] types, string[] include, string[] exclude, string safety, int[] spring, int[] summer, int[] fall, int[] winter)
            :base(name, min, max, types, include, exclude, safety) //uses the original "SpawnArea" constructor to fill in the shared fields as usual
        {
            SpringItemIndex = spring;
            SummerItemIndex = summer;
            FallItemIndex = fall;
            WinterItemIndex = winter;
        }
    }

    //a subclass of "SpawnArea" specifically for large object generation, including settings for which object types to spawn & a one-time switch to find and respawn pre-existing objects
    public class LargeObjectSpawnArea : SpawnArea
    {
        public string[] ObjectTypes { get; set; }
        public bool FindExistingObjectLocations { get; set; }
        public int PercentExtraSpawnsPerSkillLevel { get; set; }
        public string RelatedSkill { get; set; }

        public LargeObjectSpawnArea()
            : base()
        {

        }

        public LargeObjectSpawnArea(string name, int min, int max, string[] types, string[] include, string[] exclude, string safety, string[] objTypes, bool find, int extra, string skill)
            : base(name, min, max, types, include, exclude, safety) //uses the original "SpawnArea" constructor to fill in the shared fields as usual
        {
            ObjectTypes = objTypes;
            FindExistingObjectLocations = find;
            PercentExtraSpawnsPerSkillLevel = extra;
            RelatedSkill = skill;
        }
    }

    //a subclass of "SpawnArea" specifically for ore generation, providing the ability to optionally override each area's skill requirements & spawn chances for ore
    public class OreSpawnArea : SpawnArea
    {
        public Dictionary<string, int> MiningLevelRequired { get; set; }
        public Dictionary<string, int> StartingSpawnChance { get; set; }
        public Dictionary<string, int> LevelTenSpawnChance { get; set; }

        public OreSpawnArea()
            : base ()
        {

        }

        public OreSpawnArea(string name, int min, int max, string[] types, string[] include, string[] exclude, string safety, Dictionary<string, int> skill, Dictionary<string, int> starting, Dictionary<string, int> levelTen)
            : base (name, min, max, types, include, exclude, safety) //uses the original "SpawnArea" constructor to fill in the shared fields as usual
        {
            MiningLevelRequired = skill;
            StartingSpawnChance = starting;
            LevelTenSpawnChance = levelTen;
        }
    }
}
