/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aEnigmatic/StardewValley
**
*************************************************/

using System.Collections.Generic;
using System.Linq;

namespace ConvenientChests.CategorizeChests.Framework.Persistence
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
        public Dictionary<ItemType, string> AcceptedItems;


        public ChestEntry()
        {
        }

        public ChestEntry(ChestData data, ChestAddress address)
        {
            Address = address;
            AcceptedItems = data.AcceptedItemKinds
                                .GroupBy(i => i.GetItemType())
                                .ToDictionary(
                                              g => g.Key,
                                              g => string.Join(",", g.Select(i => i.ItemId))
                                             );
        }


        public HashSet<ItemKey> GetItemSet()
            => AcceptedItems
              .Select(e => new {
                   Type = e.Key,
                   ItemIDs = e.Value.Split(','),
               })
              .SelectMany(e => e.ItemIDs.Select(itemId => new ItemKey(e.Type.GetTypeDefinition(), itemId)))
              .ToHashSet();
    }
}