/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ceruleandeep/CeruleanStardewMods
**
*************************************************/

using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace MarketDay.Data
{
    public class MarketDataModel
    {
        public List<Vector2> ShopLocations { get; set; }
        public Dictionary<string, string> ShopOwners { get; set; }
    }
}