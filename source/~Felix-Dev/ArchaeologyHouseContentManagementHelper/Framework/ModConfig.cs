using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StardewMods.ArchaeologyHouseContentManagementHelper.Framework
{
    /// <summary>The parsed mod configuration.</summary>
    internal class ModConfig
    {
        /// <summary>
        /// The display duration of the description for a museum item. In milliseconds. 
        /// </summary>
        public int MuseumItemDisplayTime { get; set; } = 3000;

        /// <summary>
        /// Whether to show the tile can-place indicator for tiles with a placed item placement as well.
        /// </summary>
        public bool ShowVisualSwapIndicator { get; set; } = false;


    }
}
