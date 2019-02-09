using TehPers.CoreMod.Api.Drawing.Sprites;

namespace TehPers.CoreMod.Api.Items.ItemProviders {
    public interface IItemProvider : IItemCreator, IItemComparer, IItemDrawingProvider {
        /// <summary>Invalidates any assets used by the items registered by this provider. This is called after each item is assigned an index, which occurs when a save is loaded or when connecting to a multiplayer game.</summary>
        void InvalidateAssets();
    }
}