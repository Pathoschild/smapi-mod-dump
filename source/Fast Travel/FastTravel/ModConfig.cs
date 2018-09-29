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
