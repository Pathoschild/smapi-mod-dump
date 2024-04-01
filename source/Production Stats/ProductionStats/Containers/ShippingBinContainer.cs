/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/FlameHorizon/ProductionStats
**
*************************************************/

// derived from code by Jesse Plamondon-Willard under MIT license: https://github.com/Pathoschild/StardewMods

using StardewValley;

namespace ProductionStats.Containers
{
    internal class ShippingBinContainer(GameLocation location) : IStorageContainer
    {
        private readonly GameLocation _location = location;

        public IEnumerable<Item> GetItemsForPlayer(long uniqueMultiplayerID)
        {
            Farm farm = _location as Farm ?? Game1.getFarm();
            StardewValley.Inventories.IInventory shippingBin = farm.getShippingBin(Game1.player);
            return shippingBin;
        }
    }
}