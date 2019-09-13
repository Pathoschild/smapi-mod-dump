using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FelixDev.StardewMods.ToolUpgradeDeliveryService.Framework
{
    /// <summary>The parsed mod configuration.</summary>
    internal class ModConfig
    {
        /// <summary>
        /// Indicates whether to remove tool duplicates from the player's inventory when retrieving upgraded tools by mail.
        /// </summary>
        public bool RemoveToolDuplicates { get; set; } = false;
    }
}
