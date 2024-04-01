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
    /// Interface for Better Farm Animal Variety API
    /// </summary>
    public interface IBetterFarmAnimalVarietyApi
    {
        bool IsEnabled();
        List<Object> GetAnimalShopStock(Farm farm);
        Dictionary<string, List<string>> GetFarmAnimalCategories();

    }
}
