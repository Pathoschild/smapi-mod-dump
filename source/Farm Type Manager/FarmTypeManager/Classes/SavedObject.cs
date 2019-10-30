using Microsoft.Xna.Framework;
using System.Collections.Generic;
using StardewModdingAPI;

namespace FarmTypeManager
{
    public partial class ModEntry : Mod
    {
        /// <summary>A class containing all necessary information about an in-game object.</summary>
        private class SavedObject
        {
            public string MapName { get; set; }
            public Vector2 Tile { get; set; }
            public ObjectType Type { get; set; }
            public int? ID { get; set; }
            public string Name { get; set; }
            public int? DaysUntilExpire { get; set; }
            public MonsterType MonType { get; set; }
            public StardewTime SpawnTime { get; set; } = 600; //default to 6:00AM for backward compatibility

            /// <param name="mapName">The name of the in-game location where this object exists.</param>
            /// <param name="tile">A tile indicating where this object exists.</param>
            /// <param name="id">The object's ID, often called parentSheetIndex.</param>
            /// <param name="type">The enumerated spawn type of the object, e.g. Forage.</param>
            /// <param name="name">The object's name. Used informally by spawners that do not rely on ID.</param>
            /// <param name="daysUntilExpire">The remaining number of days before this object should be removed from the game.</param>
            /// <param name="monsterType">The monster type spawn data used to respawn a monster; null if this isn't a monster.</param>
            /// <param name="spawnTime">The specific in-game time at which this object will spawn. Uses Stardew's internal time format, i.e. multiples of 10 from 600 to 2600.</param>
            public SavedObject(string mapName, Vector2 tile, ObjectType type, int? id, string name, int? daysUntilExpire, MonsterType monsterType = null, StardewTime spawnTime = default(StardewTime))
            {
                MapName = mapName;
                Tile = tile;
                Type = type;
                ID = id;
                Name = name;
                DaysUntilExpire = daysUntilExpire;
                MonType = monsterType;
                SpawnTime = spawnTime;
            }

            /// <summary>Enumerated list of object types managed by Farm Type Manager. Used to process saved object information.</summary>
            public enum ObjectType { Forage, LargeObject, Ore, Monster }
        }
    }
}