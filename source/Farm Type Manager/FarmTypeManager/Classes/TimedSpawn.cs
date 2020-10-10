/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Esca-MMC/FarmTypeManager
**
*************************************************/

using Microsoft.Xna.Framework;
using System.Collections.Generic;
using StardewModdingAPI;

namespace FarmTypeManager
{
    public partial class ModEntry : Mod
    {
        /// <summary>A class containing a saved object and the data necessary to choose its spawn location.</summary>
        private class TimedSpawn
        {
            public SavedObject SavedObject { get; set; }
            public FarmData FarmData { get; set; }
            public SpawnArea SpawnArea { get; set; }

            public TimedSpawn(SavedObject savedObject, FarmData farmData, SpawnArea spawnArea)
            {
                SavedObject = savedObject;
                FarmData = farmData;
                SpawnArea = spawnArea;
            }
        }
    }
}