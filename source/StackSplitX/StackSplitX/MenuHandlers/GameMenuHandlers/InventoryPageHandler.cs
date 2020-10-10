/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tstaples/StackSplitX
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace StackSplitX.MenuHandlers
{
    public class InventoryPageHandler : GameMenuPageHandler<InventoryPage>
    {
        /// <summary>Constructs and instance.</summary>
        /// <param name="helper">Mod helper instance.</param>
        /// <param name="monitor">Monitor instance.</param>
        public InventoryPageHandler(IModHelper helper, IMonitor monitor)
            : base(helper, monitor)
        {
        }
    }
}
