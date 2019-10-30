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
    }
}