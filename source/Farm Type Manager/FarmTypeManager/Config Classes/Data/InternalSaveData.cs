using Microsoft.Xna.Framework;
using System.Collections.Generic;
using StardewModdingAPI;

namespace FarmTypeManager
{
    public partial class ModEntry : Mod
    {
        /// <summary>A class containing any per-farm information that needs to be saved by the mod. Not normally intended to be edited by the player.</summary>
        private class InternalSaveData
        {
            //class added in version 1.3.0; defaults used here to automatically fill in values with SMAPI's json interface
            //note that as of version 1.4.0, this is being moved from within FarmConfig to its own json file

            public Utility.Weather WeatherForYesterday { get; set; } = Utility.Weather.Sunny;
            public Dictionary<string, int> LNOSCounter { get; set; } = new Dictionary<string, int>(); //added in version 1.4.0
            public Dictionary<string, string[]> ExistingObjectLocations { get; set; } = new Dictionary<string, string[]>(); //added in version 1.4.1
            public List<SavedObject> SavedObjects { get; set; } = new List<SavedObject>(); //added in version 1.5.0

            public InternalSaveData()
            {
                
            }
        }

        /// <summary>A class containing all necessary information about an in-game object.</summary>
        private class SavedObject
        {
            public string MapName { get; set; }
            public Vector2 Tile { get; set; }
            public ObjectType Type { get; set; }
            public int ID { get; set; }
            public string Name { get; set; }
            public int? DaysUntilExpire { get; set; }

            /// <param name="mapName">The name of the in-game location where this object exists.</param>
            /// <param name="tile">A tile indicating where this object exists.</param>
            /// <param name="id">The object's ID, often called parentSheetIndex.</param>
            /// <param name="type">The general type of the object.</param>
            /// <param name="name">The object's name. Used informally by spawners that do not rely on ID.</param>
            /// <param name="days">The remaining number of days before this object should be removed from the game.</param>
            public SavedObject(string mapName, Vector2 tile, ObjectType type, int id, string name, int? days)
            {
                MapName = mapName;
                Tile = tile;
                Type = type;
                ID = id;
                Name = name;
                DaysUntilExpire = days;
            }

            /// <summary>Enumerated list of object types managed by Farm Type Manager. Used to process saved object information.</summary>
            public enum ObjectType { Forage, LargeObject, Ore }
        }
    }
}