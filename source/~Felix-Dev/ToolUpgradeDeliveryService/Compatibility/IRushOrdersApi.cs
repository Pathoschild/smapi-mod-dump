using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FelixDev.StardewMods.ToolUpgradeDeliveryService.Compatibility
{
    /// <summary>
    /// Provides an API to interact with the external mod [Rush Orders] (https://www.nexusmods.com/stardewvalley/mods/605).
    /// </summary>
    public interface IRushOrdersApi
    {
        /// <summary>
        /// Raised when the player places a rushed tool order at the Blacksmith's shop.
        /// </summary>
        event EventHandler<Tool> ToolRushed;
    }
}
