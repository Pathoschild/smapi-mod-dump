/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Rafseazz/Ridgeside-Village-Mod
**
*************************************************/

using StardewValley;
using System.Collections.Generic;

namespace RidgesideVillage
{
    /// <summary>
    /// Interface for Shop Tile Framework
    /// </summary>
    public interface IShopTileFrameworkAPI
    {
        bool OpenItemShop(string shopName);
    }
}