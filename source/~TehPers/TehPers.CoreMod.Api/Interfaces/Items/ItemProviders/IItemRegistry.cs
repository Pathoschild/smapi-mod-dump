namespace TehPers.CoreMod.Api.Items.ItemProviders {
    public interface IItemRegistry<in T> : IItemProvider {
        /// <summary>Registers a new item with the game.</summary>
        /// <param name="localKey">The local key for this item, unique within your mod.</param>
        /// <param name="manager">The item's manager.</param>
        /// <returns>The key for the item once registered.</returns>
        ItemKey Register(string localKey, T manager);

        /// <summary>Registers a new simple item with the game.</summary>
        /// <param name="key">The key for this item, unique within all items registered through Teh's Core Mod.</param>
        /// <param name="manager">The item's manager.</param>
        /// <returns>The key for the item once registered.</returns>
        void Register(in ItemKey key, T manager);
    }
}