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
using StardewValley;
using StardewValley.Menus;

namespace DeluxeJournal.Events
{
    /// <summary>Events provided to every ITask.</summary>
    public interface ITaskEvents
    {
        /// <summary>An <see cref="Item"/> was collected for the first time, i.e. Item.HasBeenInInventory is false.</summary>
        event EventHandler<ItemReceivedEventArgs> ItemCollected;

        /// <summary>An <see cref="Item"/> has been crafted from the crafting menu.</summary>
        event EventHandler<ItemReceivedEventArgs> ItemCrafted;

        /// <summary>An <see cref="Item"/> has been given to an NPC.</summary>
        event EventHandler<GiftEventArgs> ItemGifted;

        /// <summary>A <see cref="ISalable"/> has been purchased.</summary>
        event EventHandler<SalableEventArgs> SalablePurchased;

        /// <summary>An <see cref="ISalable"/> has been sold.</summary>
        event EventHandler<SalableEventArgs> SalableSold;

        /// <summary>A <see cref="FarmAnimal"> has been purchased.</summary>
        event EventHandler<FarmAnimalEventArgs> FarmAnimalPurchased;

        /// <summary>A <see cref="FarmAnimal"> has been sold.</summary>
        event EventHandler<FarmAnimalEventArgs> FarmAnimalSold;

        /// <summary>A Building has been constructed. Fires for both new and upgraded buildings.</summary>
        /// <remarks>Upgrades are currently only detected via a <see cref="CarpenterMenu"/> (i.e. Robin or the Wizard).</remarks>
        event EventHandler<BuildingConstructedEventArgs> BuildingConstructed;

        /// <summary>SMAPI mod events bus.</summary>
        IModEvents ModEvents { get; }
    }
}
