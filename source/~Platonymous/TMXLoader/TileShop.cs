/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Platonymous/Stardew-Valley-Mods
**
*************************************************/

using System.Collections.Generic;

namespace TMXLoader
{
    public class TileShop
    {
        public string id { get; set; }
        public List<TileShopItem> inventory { get; set; } = new List<TileShopItem>();

        public string inventoryId { get; set; } = null;

        public List<string> portraits { get; set; } = new List<string>();
    }
}
