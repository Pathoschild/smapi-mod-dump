/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TehPers/StardewValleyMods
**
*************************************************/

using System.Collections.Generic;
using System.Linq;
using StardewValley;
using TehPers.CoreMod.Api.Items.Inventory;

namespace TehPers.CoreMod.Items.Inventory {
    internal class FarmerInventory : SimpleInventory {
        private readonly Farmer _who;

        public FarmerInventory(Farmer who) {
            this._who = who;
        }

        protected override IList<Item> GetItems() {
            return this._who.Items;
        }

        public override IEnumerator<Item> GetEnumerator() {
            return this._who.Items.GetEnumerator();
        }

        public override Item Add(Item item) {
            return this._who.addItemToInventory(item);
        }

        public override bool Remove(Item item) {
            if (!this._who.Items.Contains(item)) {
                return false;
            }

            this._who.removeItemFromInventory(item);
            return true;
        }

        public override bool Contains(params IItemRequest[] items) => this.Contains(items.AsEnumerable());
        public override bool Contains(IEnumerable<IItemRequest> items) {
            return this.TryMeetRequests(items, out _, out _);
        }
    }
}