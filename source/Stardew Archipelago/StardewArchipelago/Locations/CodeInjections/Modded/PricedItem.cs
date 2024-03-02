/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/agilbert1412/StardewArchipelago
**
*************************************************/

namespace StardewArchipelago.Locations.CodeInjections.Modded
{
    public class PricedItem
    {
        public string ItemName { get; set; }
        public int[] Price { get; set; }

        public PricedItem(string itemName, int priceInGold) : this(itemName, new[] { priceInGold, 1 })
        {
        }

        public PricedItem(string itemName, int[] price)
        {
            ItemName = itemName;
            Price = price;
        }
    }
}
