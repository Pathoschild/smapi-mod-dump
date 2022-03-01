/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ImJustMatt/StardewMods
**
*************************************************/

namespace StardewMods.FuryCore.Interfaces;

using System;
using StardewMods.FuryCore.Interfaces.ClickableComponents;
using StardewMods.FuryCore.Models.CustomEvents;
using StardewValley.Menus;

/// <summary>
///     Adds icons above/below the items toolbar.
/// </summary>
public interface IHudComponents
{
    /// <summary>
    ///     Triggers when a custom <see cref="IClickableComponent" /> is pressed from the <see cref="Toolbar" />.
    /// </summary>
    public event EventHandler<ClickableComponentPressedEventArgs> HudComponentPressed;

    /// <summary>
    ///     Add a component to the toolbar.
    /// </summary>
    /// <param name="component">The component to add.</param>
    /// <param name="index">The index to add the component at.</param>
    public void AddToolbarIcon(IClickableComponent component, int index = -1);

    /// <summary>
    ///     Remove a component from the toolbar.
    /// </summary>
    /// <param name="component">The component to remove.</param>
    public void RemoveToolbarIcon(IClickableComponent component);
}