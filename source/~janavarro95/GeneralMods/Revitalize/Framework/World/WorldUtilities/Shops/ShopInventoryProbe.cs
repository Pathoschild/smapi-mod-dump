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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Omegasis.Revitalize.Framework.World.WorldUtilities.Shops.ShopUtilities;

namespace Omegasis.Revitalize.Framework.World.WorldUtilities.Shops
{
    /// <summary>
    /// Keeps track of methods to update a shop.
    /// Used because I can't initialize a KeyValue pair with a {} constructor.
    /// </summary>
    public class ShopInventoryProbe
    {

        public ItemFoundInShopInventory searchCondition;
        public UpdateShopInventory onSearchConditionMetAddItems;

        public ShopInventoryProbe(ItemFoundInShopInventory SearchCondition, UpdateShopInventory OnSearchConditionMetAddItems)
        {
            this.searchCondition = SearchCondition;
            this.onSearchConditionMetAddItems = OnSearchConditionMetAddItems;
        }

    }
}
