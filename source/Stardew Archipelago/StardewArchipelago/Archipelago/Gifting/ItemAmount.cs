/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/agilbert1412/StardewArchipelago
**
*************************************************/

namespace StardewArchipelago.Archipelago.Gifting
{
    public class ItemAmount
    {
        public string ItemName { get; set; }
        public int Amount { get; set; }

        public ItemAmount(string itemName, int amount)
        {
            ItemName = itemName;
            Amount = amount;
        }

        public static implicit operator ItemAmount((string, int) tuple) => new(tuple.Item1, tuple.Item2);
    }
}
