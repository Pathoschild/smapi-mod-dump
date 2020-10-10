/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TehPers/StardewValleyMods
**
*************************************************/

using StardewValley;

namespace TehPers.CoreMod.Api.Items.Machines {
    public readonly struct ObjectRequest {
        /// <summary>The source item object.</summary>
        public Object Item { get; }

        /// <summary>The amount of the source item.</summary>
        public int Quantity { get; }

        public ObjectRequest(Object item) : this(item, item.Stack) { }
        public ObjectRequest(Object item, int quantity) {
            this.Item = item;
            this.Quantity = quantity;
        }
    }
}