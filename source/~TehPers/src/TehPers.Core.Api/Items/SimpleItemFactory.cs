/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TehPers/StardewValleyMods
**
*************************************************/

using System;
using StardewValley;

namespace TehPers.Core.Api.Items
{
    /// <summary>
    /// An item factory for simple items.
    /// </summary>
    public class SimpleItemFactory : IItemFactory
    {
        private readonly Func<Item> createItem;

        /// <summary>
        /// The type of item this factory creates.
        /// </summary>
        public string ItemType { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleItemFactory"/> class.
        /// </summary>
        /// <param name="itemType">The type of item this factory creates.</param>
        /// <param name="createItem">A function that creates an instance of the item.</param>
        public SimpleItemFactory(string itemType, Func<Item> createItem)
        {
            this.ItemType = itemType ?? throw new ArgumentNullException(nameof(itemType));
            this.createItem = createItem ?? throw new ArgumentNullException(nameof(createItem));
        }

        /// <inheritdoc/>
        public Item Create()
        {
            return this.createItem();
        }
    }
}