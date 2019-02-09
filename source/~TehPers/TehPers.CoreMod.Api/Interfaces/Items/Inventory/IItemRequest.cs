using StardewValley;

namespace TehPers.CoreMod.Api.Items.Inventory {
    public interface IItemRequest {
        /// <summary>The quantity of this item request.</summary>
        int Quantity { get; }

        /// <summary>Whether an item matches this request.</summary>
        /// <param name="item">The item to check.</param>
        /// <returns>True if the item matches this request, false otherwise.</returns>
        bool Matches(Item item);
    }
}