using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using StardewValley.Objects;

namespace RemoteFridgeStorage.controller.saving
{
 
    /// <summary>
    /// Storing the locations of the chests
    /// </summary>
    public class SaveData
    {
        public IEnumerable<ChestData> Chests;

        public SaveData(IEnumerable<ChestData> chests)
        {
            Chests = chests;
        }
    }

    public class ChestData
    {
        public Vector2 ChestTileLocation { get; }
        public string LocationName { get; }

        public ChestData(Vector2 chestTileLocation, string locationName)
        {
            ChestTileLocation = chestTileLocation;
            LocationName = locationName;
        }
    }
}