/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LeFauxMatt/StardewMods
**
*************************************************/

namespace StardewMods.BetterChests.Framework.Interfaces;

using StardewMods.Common.Services.Integrations.BetterChests.Interfaces;
using StardewValley.Menus;

/// <summary>Manages the inventory menu by adding, removing, and filtering item filters.</summary>
internal interface IInventoryMenuManager
{
    /// <summary>Gets the instance of the inventory menu that is being managed.</summary>
    public InventoryMenu? Menu { get; }

    /// <summary>Gets the container associated with the inventory menu.</summary>
    public IStorageContainer? Container { get; }

    /// <summary>Gets the capacity of the inventory menu.</summary>
    public int Capacity { get; }

    /// <summary>Gets the number of columns of the inventory menu.</summary>
    public int Columns { get; }

    /// <summary>Gets the number of rows of the inventory menu.</summary>
    public int Rows { get; }
}