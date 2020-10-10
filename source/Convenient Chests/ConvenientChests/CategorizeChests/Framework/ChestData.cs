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
using StardewValley.Objects;

namespace ConvenientChests.CategorizeChests.Framework
{
    /// <summary>
    /// The extra data associated with a chest object, such as the list of
    /// items it should accept.
    /// </summary>
    class ChestData
    {
        public Chest Chest { get; }
        public HashSet<ItemKey> AcceptedItemKinds { get; set; } = new HashSet<ItemKey>();

        public ChestData(Chest chest) => Chest = chest;

        /// <summary>
        /// Set this chest to accept the specified kind of item.
        /// </summary>
        public void AddAccepted(ItemKey itemKey)
        {
            if (!AcceptedItemKinds.Contains(itemKey))
                AcceptedItemKinds.Add(itemKey);
        }

        /// <summary>
        /// Set this chest to not accept the specified kind of item.
        /// </summary>
        public void AddRejected(ItemKey itemKey)
        {
            if (AcceptedItemKinds.Contains(itemKey))
                AcceptedItemKinds.Remove(itemKey);
        }

        /// <summary>
        /// Toggle whether this chest accepts the specified kind of item.
        /// </summary>
        public void Toggle(ItemKey itemKey)
        {
            if (Accepts(itemKey))
                AddRejected(itemKey);

            else
                AddAccepted(itemKey);
        }

        /// <summary>
        /// Return whether this chest accepts the given kind of item.
        /// </summary>
        public bool Accepts(ItemKey itemKey) => AcceptedItemKinds.Contains(itemKey);
    }
}