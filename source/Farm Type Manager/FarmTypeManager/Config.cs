using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;

namespace FarmTypeManager
{
    public partial class ModEntry : Mod
    {
        //config.json, used by all players/saves for shared functions
        private class ModConfig
        {
            public bool EnableWhereAmICommand { get; set; }
            public bool EnableContentPacks { get; set; } = true; //added in version 1.4; default used here to automatically fill in values with SMAPI's json interface

            public ModConfig()
            {
                EnableWhereAmICommand = true; //enable the "whereami" command in the SMAPI console
                EnableContentPacks = true; //enable any content packs for this mod
            }
        }

        //a simple class used to package FarmConfig and InternalSaveData together with the content pack providing them
        private class FarmData
        {
            public FarmConfig Config { get; set; }
            public InternalSaveData Save { get; set; }
            public IContentPack Pack { get; set; } //NOTE: this should be null when no content pack was involved, i.e. the data came from files in the mod's own folder

            public FarmData(FarmConfig config, InternalSaveData save, IContentPack pack)
            {
                Config = config;
                Save = save;
                Pack = pack;
            }
        }

        //per-character configuration file, e.g. PlayerName12345.json; contains most of the mod's functional settings, split up this way to allow for different settings between saves & farm types
        private class FarmConfig
        {
            public bool ForageSpawnEnabled { get; set; }
            public bool LargeObjectSpawnEnabled { get; set; }
            public bool OreSpawnEnabled { get; set; }

            public ForageSettings Forage_Spawn_Settings { get; set; }
            public LargeObjectSettings Large_Object_Spawn_Settings { get; set; }
            public OreSettings Ore_Spawn_Settings { get; set; }

            public int[] QuarryTileIndex { get; set; }

            public FileConditions File_Conditions { get; set; }

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

                File_Conditions = new FileConditions(); //settings determining whether this config is used for a given farm
            }
        }

        /// <summary>A class containing any per-farm information that needs to be saved by the mod. Not normally intended to be edited by the player.</summary>
        private class InternalSaveData
        {
            //class added in version 1.3; defaults used here to automatically fill in values with SMAPI's json interface
            //note that as of version 1.4, this is being moved from within FarmConfig to its own json file

            public Utility.Weather WeatherForYesterday { get; set; } = Utility.Weather.Sunny;
            public Dictionary<string, int> LNOSCounter { get; set; } = new Dictionary<string, int>(); //added in version 1.4

            public InternalSaveData()
            {

            }

            public InternalSaveData(Utility.Weather wyesterday, Dictionary<string, int> counter)
            {
                WeatherForYesterday = wyesterday; //an enum (int) value corresponding to yesterday's weather
                LNOSCounter = counter; //dictionary for LimitedNumberOfSpawns tracking; designed to use SpawnArea.UniqueAreaID as a key, and increment the value each day items spawn in an area
            }
        }

        //contains configuration settings for forage item generation behavior
        private class ForageSettings
        {
            public ForageSpawnArea[] Areas { get; set; }
            public int PercentExtraSpawnsPerForagingLevel { get; set; }
            public object[] SpringItemIndex { get; set; } //changed from int[] to object[] in version 1.4
            public object[] SummerItemIndex { get; set; } //changed from int[] to object[] in version 1.4
            public object[] FallItemIndex { get; set; } //changed from int[] to object[] in version 1.4
            public object[] WinterItemIndex { get; set; } //changed from int[] to object[] in version 1.4
            public int[] CustomTileIndex { get; set; }

            //default constructor: configure default forage generation settings
            public ForageSettings()
            {
                Areas = new ForageSpawnArea[] { new ForageSpawnArea("", "Farm", 0, 3, new string[] { "Grass", "Diggable" }, new string[0], new string[0], "High", new ExtraConditions(), null, null, null, null) }; //a set of "SpawnArea" objects, describing where forage items can spawn on each map
                PercentExtraSpawnsPerForagingLevel = 0; //multiplier to give extra forage per level of foraging skill; default is +0%, since the native game lacks this mechanic

                //the "parentSheetIndex" values for each type of forage item allowed to spawn in each season (the numbers found in ObjectInformation.xnb)
                SpringItemIndex = new object[] { 16, 20, 22, 257 };
                SummerItemIndex = new object[] { 396, 398, 402, 404 };
                FallItemIndex = new object[] { 281, 404, 420, 422 };
                WinterItemIndex = new object[0];

                CustomTileIndex = new int[0]; //an extra list of tilesheet indices, for use by players who want to make some custom tile detection
            }
        }

        //contains configuration settings for spawning large objects (e.g. stumps and logs)
        private class LargeObjectSettings
        {
            public LargeObjectSpawnArea[] Areas { get; set; }
            public int[] CustomTileIndex { get; set; }

            public LargeObjectSettings()
            {
                Areas = new LargeObjectSpawnArea[] { new LargeObjectSpawnArea("", "Farm", 999, 999, new string[0], new string[0], new string[0], "High", new ExtraConditions(), new string[] { "Stump" }, true, 0, "Foraging") }; //a set of "LargeObjectSpawnArea", describing where large objects can spawn on each map
                CustomTileIndex = new int[0]; //an extra list of tilesheet indices, for use by players who want to make some custom tile detection
            }
        }

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

                Areas = new OreSpawnArea[] { new OreSpawnArea("", "Farm", 1, 5, new string[] { "Quarry" }, new string[0], new string[0], "High", new ExtraConditions(), null, null, null) }; //a set of "OreSpawnArea" objects, describing where ore can spawn on each map
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
        private class SpawnArea
        {
            public string UniqueAreaID { get; set; } = ""; //added in version 1.4
            public string MapName { get; set; }
            public int MinimumSpawnsPerDay { get; set; }
            public int MaximumSpawnsPerDay { get; set; }
            public string[] AutoSpawnTerrainTypes { get; set; } //Valid properties include "Quarry", "Custom", "Diggable", "All", and any tile Type properties ("Grass", "Dirt", "Stone", "Wood")
            public string[] IncludeAreas { get; set; }
            public string[] ExcludeAreas { get; set; }
            public string StrictTileChecking { get; set; } = "High"; //added in version 1.1; default used here to automatically fill it in with SMAPI's json interface
            public ExtraConditions ExtraConditions { get; set; } = new ExtraConditions(); //added in version 1.3
            

            public SpawnArea()
            {

            }

            public SpawnArea(string id, string name, int min, int max, string[] types, string[] include, string[] exclude, string safety, ExtraConditions extra)
            {
                UniqueAreaID = id; //a unique name assigned to this SpawnArea; used by the save data system
                MapName = name; //a name of the targeted map, e.g. "Farm" or "BusStop"
                MinimumSpawnsPerDay = min; //minimum number of items to be spawned each day (before multipliers)
                MaximumSpawnsPerDay = max; //maximum number of items to be spawned each day (before multipliers)
                AutoSpawnTerrainTypes = types; //a list of strings describing the terrain on which objects may spawn
                IncludeAreas = include; //a list of strings describing coordinates for object spawning
                ExcludeAreas = exclude; //a list of strings describing coordinates *preventing* object spawning
                StrictTileChecking = safety; //the degree of safety-checking to use before spawning objects on a tile
                ExtraConditions = extra; //a list of additional conditions that may be used to limit object spawning
            }
        }

        /// <summary>A set of additional requirements needed to spawn objects in a given area.</summary>
        private class ExtraConditions
        {
            //class added in version 1.3; defaults used here to automatically fill in values with SMAPI's json interface
            public string[] Years { get; set; } = new string[0];
            public string[] Seasons { get; set; } = new string[0];
            public string[] Days { get; set; } = new string[0];
            public string[] WeatherYesterday { get; set; } = new string[0];
            public string[] WeatherToday { get; set; } = new string[0];
            public string[] WeatherTomorrow { get; set; } = new string[0];
            public int? LimitedNumberOfSpawns { get; set; } = null;

            public ExtraConditions()
            {

            }

            public ExtraConditions(string[] years, string[] seasons, string[] days, string[] wyesterday, string[] wtoday, string[] wtomorrow, int? spawns)
            {
                Years = years; //a list of years on which objects are allowed to spawn
                Seasons = seasons; //a list of seasons in which objects are allowed to spawn
                Days = days; //a list of days (individual or ranges) on which objects are allowed to spawn
                WeatherYesterday = wyesterday; //if yesterday's weather is listed here, objects are allowed to spawn
                WeatherToday = wtoday; //if yesterday's weather is listed here, objects are allowed to spawn
                WeatherTomorrow = wtomorrow; //if yesterday's weather is listed here, objects are allowed to spawn
                LimitedNumberOfSpawns = spawns; //a number that will count down each day until 0, preventing any further spawns once that is reached
            }
        }

        /// <summary>A set of additional requirements needed for a config file to be used on a given farm.</summary>
        private class FileConditions
        {
            //class added in version 1.4; defaults used here to automatically fill in values with SMAPI's json interface
            public string[] FarmTypes { get; set; } = new string[0]; //a list of farm types on which the config may be used
            public string[] FarmerNames { get; set; } = new string[0]; //a list of farmer names; if the current farmer matches, the config file may be used
            public string[] SaveFileNames { get; set; } = new string[0]; //a list of save file (technically folder) names; if they match the current farm, the config file may be used

            public FileConditions()
            {

            }
        }

        //a subclass of "SpawnArea" specifically for forage generation, providing the ability to override each area's seasonal forage items
        private class ForageSpawnArea : SpawnArea
        {
            //this subclass was added in version 1.2; defaults are used here to automatically fill it in with SMAPI's json interface

            public object[] SpringItemIndex { get; set; } = null;
            public object[] SummerItemIndex { get; set; } = null;
            public object[] FallItemIndex { get; set; } = null;
            public object[] WinterItemIndex { get; set; } = null;

            public ForageSpawnArea()
                : base()
            {

            }

            public ForageSpawnArea(string id, string name, int min, int max, string[] types, string[] include, string[] exclude, string safety, ExtraConditions extra, object[] spring, object[] summer, object[] fall, object[] winter)
                : base(id, name, min, max, types, include, exclude, safety, extra) //uses the original "SpawnArea" constructor to fill in the shared fields as usual
            {
                SpringItemIndex = spring;
                SummerItemIndex = summer;
                FallItemIndex = fall;
                WinterItemIndex = winter;
            }
        }

        //a subclass of "SpawnArea" specifically for large object generation, including settings for which object types to spawn & a one-time switch to find and respawn pre-existing objects
        private class LargeObjectSpawnArea : SpawnArea
        {
            public string[] ObjectTypes { get; set; }
            public bool FindExistingObjectLocations { get; set; }
            public int PercentExtraSpawnsPerSkillLevel { get; set; }
            public string RelatedSkill { get; set; }

            public LargeObjectSpawnArea()
                : base()
            {

            }

            public LargeObjectSpawnArea(string id, string name, int min, int max, string[] types, string[] include, string[] exclude, string safety, ExtraConditions extra, string[] objTypes, bool find, int extraSpawns, string skill)
                : base(id, name, min, max, types, include, exclude, safety, extra) //uses the original "SpawnArea" constructor to fill in the shared fields as usual
            {
                ObjectTypes = objTypes;
                FindExistingObjectLocations = find;
                PercentExtraSpawnsPerSkillLevel = extraSpawns;
                RelatedSkill = skill;
            }
        }

        //a subclass of "SpawnArea" specifically for ore generation, providing the ability to optionally override each area's skill requirements & spawn chances for ore
        private class OreSpawnArea : SpawnArea
        {
            public Dictionary<string, int> MiningLevelRequired { get; set; }
            public Dictionary<string, int> StartingSpawnChance { get; set; }
            public Dictionary<string, int> LevelTenSpawnChance { get; set; }

            public OreSpawnArea()
                : base()
            {

            }

            public OreSpawnArea(string id, string name, int min, int max, string[] types, string[] include, string[] exclude, string safety, ExtraConditions extra, Dictionary<string, int> skill, Dictionary<string, int> starting, Dictionary<string, int> levelTen)
                : base(id, name, min, max, types, include, exclude, safety, extra) //uses the original "SpawnArea" constructor to fill in the shared fields as usual
            {
                MiningLevelRequired = skill;
                StartingSpawnChance = starting;
                LevelTenSpawnChance = levelTen;
            }
        }
    }
}
