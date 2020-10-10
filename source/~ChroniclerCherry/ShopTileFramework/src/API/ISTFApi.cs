/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ChroniclerCherry/stardew-valley-mods
**
*************************************************/

using StardewValley;
using System.Collections.Generic;

namespace ShopTileFramework.API
{
    /// <summary>
    /// Interface for Shop Tile Framework
    /// </summary>
    public interface ISTFApi
    {
        bool RegisterShops(string dir);
        bool OpenItemShop(string shopName);
        bool ResetShopStock(string shopName);
        Dictionary<ISalable, int[]> GetItemPriceAndStock(string shopName);
    }
}
