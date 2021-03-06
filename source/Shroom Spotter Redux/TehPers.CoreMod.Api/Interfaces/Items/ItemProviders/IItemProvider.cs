/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/strobel1ght/StardewValleyMods
**
*************************************************/

using TehPers.CoreMod.Api.Drawing.Sprites;

namespace TehPers.CoreMod.Api.Items.ItemProviders {
    public interface IItemProvider : IItemCreator, IItemComparer, IItemDrawingProvider {
        /// <summary>Invalidates any assets used by the items registered by this provider. This is called after each item is assigned an index, which occurs when a save is loaded or when connecting to a multiplayer game.</summary>
        void InvalidateAssets();
    }
}