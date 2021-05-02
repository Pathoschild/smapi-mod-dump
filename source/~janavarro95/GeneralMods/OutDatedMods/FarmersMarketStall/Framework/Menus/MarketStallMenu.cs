/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/janavarro95/Stardew_Valley_Mods
**
*************************************************/

using System;
using StardewValley.Menus;

namespace FarmersMarketStall.Framework.Menus
{
    class MarketStallMenu
    {
        public MarketStallMenu(MarketStall marketStall)
        {
            openMenu(marketStall);
        }

        public static IClickableMenu openMenu(MarketStall marketStall)
        {
            throw new NotImplementedException("This menu isn't implemented because the author is busy/lazy. Please encorage Omegasis to finish it!", null);
            //return new StardewValley.Menus.InventoryMenu((int)(Game1.viewport.Width*.25f),(int)(Game1.viewport.Height*.25f),true,marketStall.stock);
        }
    }
}
