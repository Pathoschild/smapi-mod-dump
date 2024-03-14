/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Sonozuki/StardewMods
**
*************************************************/

namespace BetterCrabPotsConfigUpdater.ModConfigs
{
    /// <summary>The object that contains data about each item, both for trash and non trash uses.</summary>
    public class Item
    {
        /// <summary>The object id.</summary>
        public int Id { get; set; }

        /// <summary>The chance of this object being found.</summary>
        public int Chance { get; set; }

        /// <summary>The quality this object will be found in.</summary>
        public int Quantity { get; set; }

        /// <summary>Construct an instance.</summary>
        /// <param name="id">The object id.</param>
        /// <param name="chance">That chance of this object being found.</param>
        /// <param name="quantity">The quality this object will be found in.</param>
        public Item(int id = -1, int chance = 1, int quantity = 1)
        {
            Id = id;
            Chance = chance;
            Quantity = quantity;
        }
    }
}
