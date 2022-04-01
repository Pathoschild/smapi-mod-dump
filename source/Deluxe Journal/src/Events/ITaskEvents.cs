/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MolsonCAD/DeluxeJournal
**
*************************************************/

using StardewModdingAPI.Events;

namespace DeluxeJournal.Events
{
    /// <summary>Events provided to every ITask.</summary>
    public interface ITaskEvents
    {
        /// <summary>
        /// An Item was collected for the first time, i.e. Item.HasBeenInInventory is false.
        /// Note: Does not fire for Furniture or big craftables.
        /// </summary>
        event EventHandler<ItemReceivedEventArgs> ItemCollected;

        /// <summary>An Item has been crafted from the crafting menu.</summary>
        event EventHandler<ItemReceivedEventArgs> ItemCrafted;

        /// <summary>An Item has been given to an NPC.</summary>
        event EventHandler<GiftEventArgs> ItemGifted;

        /// <summary>SMAPI mod events bus.</summary>
        IModEvents ModEvents { get; }
    }
}
