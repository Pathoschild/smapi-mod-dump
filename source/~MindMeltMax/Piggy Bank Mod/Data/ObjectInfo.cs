/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MindMeltMax/Stardew-Valley-Mods
**
*************************************************/

using StardewValley.GameData.BigCraftables;
using StardewValley.GameData.Shops;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PiggyBank.Data
{
    public class ObjectInformation
    {
        public string Id { get; set; }

        public BigCraftableData Object { get; set; }

        public string Recipe { get; set; }

        public ShopItemData ShopItem { get; set; }
    }
}
