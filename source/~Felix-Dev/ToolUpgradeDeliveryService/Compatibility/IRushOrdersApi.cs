/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Felix-Dev/StardewMods
**
*************************************************/

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
