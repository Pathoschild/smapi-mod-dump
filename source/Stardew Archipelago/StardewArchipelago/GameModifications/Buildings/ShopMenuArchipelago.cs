/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/agilbert1412/StardewArchipelago
**
*************************************************/

using System.Collections.Generic;
using StardewValley;
using StardewValley.Menus;

namespace StardewArchipelago.GameModifications.Buildings
{
    public class ShopMenuArchipelago : ShopMenu
    {
        public ShopMenuArchipelago(Dictionary<ISalable, int[]> itemPriceAndStock, string who) : base(itemPriceAndStock, who: who)
        {

        }
    }
}
