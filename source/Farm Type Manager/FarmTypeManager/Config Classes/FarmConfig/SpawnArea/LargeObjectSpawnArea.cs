using StardewModdingAPI;

namespace FarmTypeManager
{
    public partial class ModEntry : Mod
    {
        //a subclass of "SpawnArea" specifically for large object generation, including settings for which object types to spawn & a one-time switch to find and respawn pre-existing objects
        private class LargeObjectSpawnArea : SpawnArea
        {
            public string[] ObjectTypes { get; set; }
            public bool FindExistingObjectLocations { get; set; }
            public int PercentExtraSpawnsPerSkillLevel { get; set; }
            public string RelatedSkill { get; set; }

            //default constructor, providing settings for hardwood stump respawning (roughly similar to the Forest Farm)
            public LargeObjectSpawnArea()
                : base()
            {
                UniqueAreaID = "";
                MapName = "Farm";
                MinimumSpawnsPerDay = 999;
                MaximumSpawnsPerDay = 999;
                AutoSpawnTerrainTypes = new string[0];
                IncludeAreas = new string[0];
                ExcludeAreas = new string[0];
                StrictTileChecking = "High";
                ExtraConditions = new ExtraConditions();

                ObjectTypes = new string[] { "Stump" };
                FindExistingObjectLocations = true;
                PercentExtraSpawnsPerSkillLevel = 0;
                RelatedSkill = "Foraging";
            }
        }
    }
}