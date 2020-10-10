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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using StardewValley;
using TehPers.CoreMod.Api.Items.Inventory;

namespace TehPers.CoreMod.Items.Inventory {
    internal abstract class SimpleInventory : IInventory {
        /// <summary>Gets the list of items contained by this inventory.</summary>
        /// <returns>The list of items in this inventory.</returns>
        protected abstract IList<Item> GetItems();

        /// <inheritdoc />
        public abstract IEnumerator<Item> GetEnumerator();

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator() {
            return this.GetEnumerator();
        }

        /// <inheritdoc />
        public abstract Item Add(Item item);

        /// <inheritdoc />
        public abstract bool Remove(Item item);

        /// <inheritdoc />
        public abstract bool Contains(params IItemRequest[] items);

        /// <inheritdoc />
        public abstract bool Contains(IEnumerable<IItemRequest> items);

        /// <inheritdoc />
        public virtual bool TryRemoveAll(IEnumerable<IItemRequest> items, out IDictionary<IItemRequest, int> remaining) {
            if (!this.TryMeetRequests(items, out remaining, out (Item item, int stack)[] newInv)) {
                // Don't remove anything if there are any items that couldn't fully be removed
                return false;
            }

            // Apply the changes
            foreach ((Item invItem, int stack) in newInv) {
                // Ignore empty slots
                if (invItem == null) {
                    continue;
                }

                if (stack <= 0) {
                    // Remove the item
                    this.Remove(invItem);
                } else {
                    // Update the stack size
                    invItem.Stack = stack;
                }
            }

            // Items were removed
            return true;
        }

        protected virtual bool TryMeetRequests(IEnumerable<IItemRequest> items, out IDictionary<IItemRequest, int> remaining, out (Item item, int stack)[] newInventory) {
            remaining = new Dictionary<IItemRequest, int>();

            // Create a virtual copy of the inventory to track each item's new stack size
            (Item item, int stack)[] virtualInv = this.GetItems().Select(item => (item, item?.Stack ?? 0)).ToArray();
            newInventory = virtualInv;

            // Loop through each item being removed
            foreach (IItemRequest request in items) {
                int curRemaining = request.Quantity;

                // Search the virtual inventory for stacks that can be pulled from
                for (int i = 0; curRemaining > 0 && i < virtualInv.Length; i++) {
                    (Item invItem, int stack) = virtualInv[i];

                    // Skip stacks with no items remaining
                    if (stack == 0) {
                        continue;
                    }

                    // Check if this stack can be removed from
                    if (!request.Matches(invItem)) {
                        continue;
                    }

                    // Remove from this stack
                    int removed = Math.Min(stack, curRemaining);
                    virtualInv[i] = (invItem, stack - removed);
                    curRemaining -= removed;
                }

                // Check if the item was fully removed
                if (curRemaining > 0) {
                    // Track it if it wasn't
                    remaining.Add(request, curRemaining);
                }
            }

            return !remaining.Values.Any(n => n > 0);
        }
    }
}