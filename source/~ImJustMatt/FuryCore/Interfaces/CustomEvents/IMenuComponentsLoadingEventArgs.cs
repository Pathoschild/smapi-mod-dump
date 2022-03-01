/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ImJustMatt/StardewMods
**
*************************************************/

namespace StardewMods.FuryCore.Interfaces.CustomEvents;

using System;
using StardewMods.FuryCore.Events;
using StardewMods.FuryCore.Interfaces.ClickableComponents;
using StardewMods.FuryCore.Interfaces.GameObjects;
using StardewValley.Menus;

/// <summary>
///     <see cref="EventArgs" /> for the <see cref="MenuComponentsLoading" /> event.
/// </summary>
public interface IMenuComponentsLoadingEventArgs
{
    /// <summary>
    ///     Gets the storage object for the menu being displayed.
    /// </summary>
    public IStorageContainer Context { get; }

    /// <summary>
    ///     Gets the Menu to add components to.
    /// </summary>
    public IClickableMenu Menu { get; }

    /// <summary>
    ///     Adds a component to the menu.
    /// </summary>
    /// <param name="component">The component to add.</param>
    /// <param name="index">Where to insert the component.</param>
    public void AddComponent(IClickableComponent component, int index = -1);
}