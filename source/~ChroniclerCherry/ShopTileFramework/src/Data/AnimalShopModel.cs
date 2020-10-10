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

namespace ShopTileFramework.Data
{
    abstract class AnimalShopModel
    {
        public string ShopName { get; set; }
        public List<string> AnimalStock { get; set; }
        public string[] ExcludeFromMarnies { get; set; }
        public string[] When { get; set; } = null;
        public string ClosedMessage { get; set; } = null;
        public Dictionary<string, string> LocalizedClosedMessage { get; set; }
    }
}
