/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Pathoschild/StardewMods
**
*************************************************/

using System.Collections.Generic;
using StardewValley;
using StardewValley.Menus;

namespace Pathoschild.Stardew.ChestsAnywhere.Framework.Containers
{
    /// <summary>An in-game container which can store items.</summary>
    internal interface IContainer
    {
        /*********
        ** Accessors
        *********/
        /// <summary>The underlying inventory.</summary>
        IList<Item?> Inventory { get; }

        /// <summary>The persisted data for this container.</summary>
        ContainerData Data { get; }

        /// <summary>Whether Automate options can be configured for this chest.</summary>
        bool CanConfigureAutomate { get; }


        /*********
        ** Public methods
        *********/
        /// <summary>Get whether the inventory can accept the item type.</summary>
        /// <param name="item">The item.</param>
        bool CanAcceptItem(Item item);

        /// <summary>Get whether another instance wraps the same underlying container.</summary>
        /// <param name="container">The other container.</param>
        bool IsSameAs(IContainer? container);

        /// <summary>Get whether another instance wraps the same underlying container.</summary>
        /// <param name="inventory">The other container's inventory.</param>
        bool IsSameAs(IList<Item?>? inventory);

        /// <summary>Open a menu to transfer items between the player's inventory and this container.</summary>
        IClickableMenu OpenMenu();

        /// <summary>Persist the container data.</summary>
        void SaveData();
    }
}
