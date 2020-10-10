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

namespace FastTravel
{
    [Serializable]
    public class ModConfig
    {
        /// <summary>
        /// If the game is to run in a balanced mode.
        /// See the mod page for an explanation.
        /// </summary>
        public bool BalancedMode = false;

        /// <summary>
        /// List of locations which can be teleported to.
        /// </summary>
        public FastTravelPoint[] FastTravelPoints;
    }
}
