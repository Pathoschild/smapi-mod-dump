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
using StardewValley.Objects;

namespace ProductionStats.Containers;

internal class FurnitureContainer(StorageFurniture furniture) : IStorageContainer
{
    private readonly StorageFurniture _furniture = furniture;

    public IEnumerable<Item> GetItemsForPlayer(long uniqueMultiplayerID)
    {
        return _furniture.heldItems;
    }
}