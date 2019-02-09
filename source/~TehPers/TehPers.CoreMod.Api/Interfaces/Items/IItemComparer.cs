using StardewValley;

namespace TehPers.CoreMod.Api.Items {
    public interface IItemComparer {
        /// <summary>Checks if an item is associated with a particular key.</summary>
        /// <param name="key">The key of the item.</param>
        /// <param name="item">The item to compare against the key.</param>
        /// <returns>True if the item and key are associated, false otherwise.</returns>
        bool IsInstanceOf(in ItemKey key, Item item);
    }
}