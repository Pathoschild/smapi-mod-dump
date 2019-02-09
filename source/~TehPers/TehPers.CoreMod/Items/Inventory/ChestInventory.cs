using System;
using System.Collections.Generic;
using System.Linq;
using StardewValley;
using StardewValley.Objects;
using TehPers.CoreMod.Api.Items.Inventory;

namespace TehPers.CoreMod.Items.Inventory {
    internal class ChestInventory : SimpleInventory {
        private readonly Chest _chest;

        public ChestInventory(Chest chest) {
            this._chest = chest;
        }

        protected override IList<Item> GetItems() {
            return this._chest.items;
        }

        public override IEnumerator<Item> GetEnumerator() {
            return this._chest.items.GetEnumerator();
        }

        public override Item Add(Item item) {
            return this._chest.addItem(item);
        }

        public override bool Remove(Item item) {
            return this._chest.items.Remove(item);
        }

        public override bool Contains(params IItemRequest[] items) => this.Contains(items.AsEnumerable());
        public override bool Contains(IEnumerable<IItemRequest> items) {
            return this.TryMeetRequests(items, out _, out _);
        }
    }
}