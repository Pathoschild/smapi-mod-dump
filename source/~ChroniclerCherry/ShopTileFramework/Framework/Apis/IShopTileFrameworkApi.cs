/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ChroniclerCherry/stardew-valley-mods
**
*************************************************/

using System.Collections.Generic;
using StardewValley;

namespace ShopTileFramework.Framework.Apis
{
    /// <summary>
    /// Interface for Shop Tile Framework
    /// </summary>
    public interface IShopTileFrameworkApi
    {
        bool RegisterShops(string dir);
        bool OpenItemShop(string shopName);
        bool ResetShopStock(string shopName);
        Dictionary<ISalable, ItemStockInformation> GetItemPriceAndStock(string shopName);
    }
}
