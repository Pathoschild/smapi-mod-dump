using StardewValley.Objects;
using StardewValleyMods.CategorizeChests.Framework.Persistence;

namespace StardewValleyMods.CategorizeChests.Framework
{
    /// <summary>
    /// A helper for finding the chest object corresponding to a given chest address.
    /// </summary>
    interface IChestFinder
    {
        Chest GetChestByAddress(ChestAddress address);
    }
}