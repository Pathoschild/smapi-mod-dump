/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/EpicBellyFlop45/StardewMods
**
*************************************************/

using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace MoreTrees.Models
{
    /// <summary>Represents data about trees that gets saved.</summary>
    public class SavePersistantTreeData
    {
        /*********
        ** Accessors
        *********/
        /// <summary>Represents the location of the tree.</summary>
        public Vector2 TileLocation { get; set; }

        /// <summary>The number of days till the tree bark can be harvested.</summary>
        public int DaysTillNextBarkHarvest { get; set; }

        /// <summary>The number of days till each shake produce can be dropped.</summary>
        public List<int> DaysTillNextShakeProduct { get; set; }


        /*********
        ** Public Methods
        *********/
        /// <summary>Construct an instance.</summary>
        /// <param name="tileLocation">Represents the location of the tree.</param>
        /// <param name="daysTillNextBarkHarvest">The number of days till the tree bark can be harvested.</param>
        /// <param name="daysTillNextShakeProduct">The number of days till each shake produce can be dropped.</param>
        public SavePersistantTreeData(Vector2 tileLocation, int daysTillNextBarkHarvest, List<int> daysTillNextShakeProduct)
        {
            TileLocation = tileLocation;
            DaysTillNextBarkHarvest = daysTillNextBarkHarvest;
            DaysTillNextShakeProduct = daysTillNextShakeProduct;
        }
    }
}
