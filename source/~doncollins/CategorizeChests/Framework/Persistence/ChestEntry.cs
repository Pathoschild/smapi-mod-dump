using System.Collections.Generic;

namespace StardewValleyMods.CategorizeChests.Framework.Persistence
{
    /// <summary>
    /// A piece of saved data describing the location of a chest and what items
    /// the chest at that location has been assigned to.
    /// </summary>
    class ChestEntry
    {
        /// <summary>
        /// The chest's location in the world.
        /// </summary>
        public ChestAddress Address;

        /// <summary>
        /// The set of item keys that were configured to be accepted
        /// by the chest at <see cref="Address"/> .
        /// </summary>
        public IEnumerable<ItemKey> AcceptedItemKinds;
    }
}