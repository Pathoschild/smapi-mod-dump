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
    /// Interface for Better Farm Animal Variety API
    /// </summary>
    public interface IBFAVApi
    {
        bool IsEnabled();
        List<Object> GetAnimalShopStock(Farm farm);
        Dictionary<string, List<string>> GetFarmAnimalCategories();

    }
}