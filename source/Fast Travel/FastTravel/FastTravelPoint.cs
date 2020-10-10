/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/DeathGameDev/SDV-FastTravel
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace FastTravel
{
    [Serializable]
    public struct FastTravelPoint
    {
        /// <summary>
        /// Display name of this point on the map.
        /// </summary>
        public string MapName;

        /// <summary>
        /// Integer index of this location in Game1.locations
        /// </summary>
        public int GameLocationIndex;

        /// <summary>
        /// The position at which the player will be spawned.
        /// </summary>
        public Point SpawnPosition;

        /// <summary>
        /// If this will actually spawn the player into another place, this will be set.
        /// </summary>
        public string RerouteName;
    }
}
