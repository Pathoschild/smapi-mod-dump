using System;
using System.Collections.Generic;
using StardewModdingAPI;

namespace FarmTypeManager
{
    public partial class ModEntry : Mod
    {
        //a subclass of "SpawnArea" specifically for monster generation
        private class MonsterSpawnArea : SpawnArea
        {
            public List<MonsterType> MonsterTypes { get; set; } = new List<MonsterType>(); //a list of MonsterType objects (each containing a name and optional dictionary of customization settings)

            //default constructor, providing Wilderness Farm style monster spawns on the farm
            public MonsterSpawnArea()
                : base()
            {

            }
        }
    }
}