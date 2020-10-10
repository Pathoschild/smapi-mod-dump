/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/doncollins/StardewValleyMods
**
*************************************************/

using StardewValley;
using StardewValley.Objects;
using System.Collections.Generic;

namespace StardewValleyMods.CategorizeChests.Framework
{
    /// <summary>
    /// The extra data associated with a chest object, such as the list of
    /// items it should accept.
    /// </summary>
    class ChestData
    {
        private readonly IItemDataManager ItemDataManager;
        private readonly Chest Chest;

        private HashSet<ItemKey> ItemsEnabled = new HashSet<ItemKey>();

        /// <summary>
        /// The set of item keys that have been assigned to this chest.
        /// </summary>
        public IEnumerable<ItemKey> AcceptedItemKinds
        {
            get { return ItemsEnabled; }

            set
            {
                ItemsEnabled.Clear();

                foreach (var itemKey in value)
                {
                    ItemsEnabled.Add(itemKey);
                }
            }
        }

        public ChestData(Chest chest, IItemDataManager itemDataManager)
        {
            Chest = chest;
            ItemDataManager = itemDataManager;
        }

        /// <summary>
        /// Set this chest to accept the specified kind of item.
        /// </summary>
        public void Accept(ItemKey itemKey)
        {
            if (!ItemsEnabled.Contains(itemKey))
                ItemsEnabled.Add(itemKey);
        }

		/// <summary>
		/// Set this chest to not accept the specified kind of item.
		/// </summary>
		public void Reject(ItemKey itemKey)
        {
            if (ItemsEnabled.Contains(itemKey))
                ItemsEnabled.Remove(itemKey);
        }

		/// <summary>
		/// Toggle whether this chest accepts the specified kind of item.
		/// </summary>
		public void Toggle(ItemKey itemKey)
        {
            if (Accepts(itemKey))
                Reject(itemKey);
            else
                Accept(itemKey);
        }

		/// <summary>
		/// Return whether this chest should accept the given item.
		/// </summary>
		public bool Accepts(Item item)
		{
			return Accepts(ItemDataManager.GetKey(item));
		}

		/// <summary>
		/// Return whether this chest accepts the given kind of item.
		/// </summary>
		public bool Accepts(ItemKey itemKey)
        {
            return ItemsEnabled.Contains(itemKey);
        }
    }
}