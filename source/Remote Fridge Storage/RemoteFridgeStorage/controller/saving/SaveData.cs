/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/SoapStuff/Remote-Fridge-Storage
**
*************************************************/

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