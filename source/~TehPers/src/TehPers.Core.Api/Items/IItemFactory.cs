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

namespace TehPers.Core.Api.Items
{
    /// <summary>
    /// A factory which can create instances of an <see cref="Item"/>.
    /// </summary>
    public interface IItemFactory
    {
        /// <summary>
        /// Gets the type of item this factory creates.
        /// </summary>
        string ItemType { get; }

        /// <summary>
        /// Creates an instance of this item. If the item can be stacked, then the stack size
        /// should be 1.
        /// </summary>
        /// <returns>An instance of this item.</returns>
        Item Create();
    }
}