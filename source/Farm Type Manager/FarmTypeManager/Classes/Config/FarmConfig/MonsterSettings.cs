using StardewModdingAPI;

namespace FarmTypeManager
{
    public partial class ModEntry : Mod
    {
        //contains configuration settings for spawning monsters
        private class MonsterSettings
        {
            public MonsterSpawnArea[] Areas { get; set; }
            public int[] CustomTileIndex { get; set; }

            public MonsterSettings()
            {
                Areas = new MonsterSpawnArea[2]; //a set of "MonsterSpawnArea", describing where monsters can spawn
                CustomTileIndex = new int[0]; //an extra list of tilesheet indices, for those who want to use their own custom terrain type

                //create the default "ground monsters" area for the farm
                //this should spawn 1 "ground" monster at ~15% to ~22.5% of available times from 7pm to 2am
                MonsterSpawnArea monsterArea1 = new MonsterSpawnArea()
                {
                    MinimumSpawnsPerDay = 5,
                    MaximumSpawnsPerDay = 8,
                    IncludeTerrainTypes = new string[] { "Dirt", "Diggable", "Grass" },
                    SpawnTiming = new SpawnTiming()
                    {
                        //from 5pm onward, up to 1 monster can spawn every 10 minutes
                        StartTime = 1900,
                        EndTime = 2550,
                        MinimumTimeBetweenSpawns = 10,
                        MaximumSimultaneousSpawns = 1,
                        OnlySpawnIfAPlayerIsPresent = true,
                        SpawnSound = ""
                    }
                };

                //create and add the various ground monster types
                MonsterType brute = new MonsterType("shadow brute");
                brute.Settings.Add("RelatedSkill", "Combat");
                brute.Settings.Add("MinimumSkillLevel", 8);
                monsterArea1.MonsterTypes.Add(brute);

                MonsterType golem = new MonsterType("wilderness golem");
                golem.Settings.Add("SpawnWeight", 4);
                monsterArea1.MonsterTypes.Add(golem);

                MonsterType purpleSlime = new MonsterType("purple slime");
                purpleSlime.Settings.Add("RelatedSkill", "Combat");
                purpleSlime.Settings.Add("MinimumSkillLevel", 10);
                purpleSlime.Settings.Add("SpawnWeight", 2);
                monsterArea1.MonsterTypes.Add(purpleSlime);

                MonsterType redSlime = new MonsterType("red slime");
                redSlime.Settings.Add("RelatedSkill", "Combat");
                redSlime.Settings.Add("MinimumSkillLevel", 8);
                redSlime.Settings.Add("MaximumSkillLevel", 9);
                redSlime.Settings.Add("SpawnWeight", 2);
                monsterArea1.MonsterTypes.Add(redSlime);

                MonsterType blueSlime = new MonsterType("blue slime");
                blueSlime.Settings.Add("RelatedSkill", "Combat");
                blueSlime.Settings.Add("MinimumSkillLevel", 4);
                blueSlime.Settings.Add("MaximumSkillLevel", 7);
                blueSlime.Settings.Add("SpawnWeight", 2);
                monsterArea1.MonsterTypes.Add(blueSlime);

                MonsterType greenSlime = new MonsterType("green slime");
                greenSlime.Settings.Add("RelatedSkill", "Combat");
                greenSlime.Settings.Add("MaximumSkillLevel", 3);
                greenSlime.Settings.Add("SpawnWeight", 2);
                monsterArea1.MonsterTypes.Add(greenSlime);

                //add area 1 to the array
                Areas[0] = monsterArea1; 

                //create the default "flying monsters" area for the farm
                //this should spawn 1 "flying" monster at ~5% to 7.5% of available times from 7pm to 2am
                MonsterSpawnArea monsterArea2 = new MonsterSpawnArea()
                {
                    MinimumSpawnsPerDay = 2,
                    MaximumSpawnsPerDay = 3,
                    IncludeTerrainTypes = new string[] { "Dirt", "Diggable", "Grass" },
                    SpawnTiming = new SpawnTiming()
                    {
                        //from 5pm onward, up to 1 monster can spawn every 10 minutes
                        StartTime = 1900,
                        EndTime = 2550,
                        MinimumTimeBetweenSpawns = 10,
                        MaximumSimultaneousSpawns = 1,
                        OnlySpawnIfAPlayerIsPresent = true,
                        SpawnSound = ""
                    }
                };

                //create and add the various flying monster types
                MonsterType iridiumBat = new MonsterType("iridium bat");
                iridiumBat.Settings.Add("RelatedSkill", "Combat");
                iridiumBat.Settings.Add("MinimumSkillLevel", 10);
                iridiumBat.Settings.Add("SpawnWeight", 4);
                monsterArea2.MonsterTypes.Add(iridiumBat);

                MonsterType serpent = new MonsterType("serpent");
                serpent.Settings.Add("RelatedSkill", "Combat");
                serpent.Settings.Add("MinimumSkillLevel", 10);
                serpent.Settings.Add("SpawnWeight", 3);
                monsterArea2.MonsterTypes.Add(serpent);

                MonsterType lavaBat = new MonsterType("lava bat");
                lavaBat.Settings.Add("RelatedSkill", "Combat");
                lavaBat.Settings.Add("MinimumSkillLevel", 8);
                lavaBat.Settings.Add("SpawnWeight", 4);
                monsterArea2.MonsterTypes.Add(lavaBat);

                MonsterType frostBat = new MonsterType("frost bat");
                frostBat.Settings.Add("RelatedSkill", "Combat");
                frostBat.Settings.Add("MinimumSkillLevel", 5);
                frostBat.Settings.Add("SpawnWeight", 2);
                monsterArea2.MonsterTypes.Add(frostBat);

                MonsterType bat = new MonsterType("bat");
                bat.Settings.Add("SpawnWeight", 2);
                monsterArea2.MonsterTypes.Add(bat);

                //add area 2 to the array
                Areas[1] = monsterArea2;
            }
        }
    }
}