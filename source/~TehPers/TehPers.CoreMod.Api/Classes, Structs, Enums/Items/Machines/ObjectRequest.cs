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