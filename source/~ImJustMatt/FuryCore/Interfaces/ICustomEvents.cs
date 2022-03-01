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
using StardewModdingAPI.Events;
using StardewMods.FuryCore.Interfaces.CustomEvents;

/// <summary>
///     Custom Events raised by FuryCore.
/// </summary>
public interface ICustomEvents
{
    /// <summary>
    ///     Similar to SMAPI's own MenuChanged event, except it is invoked for not-yet-active menus in their constructor, and
    ///     has tick monitoring for specific fields that substantially change the menu's functionality.
    /// </summary>
    public event EventHandler<IClickableMenuChangedEventArgs> ClickableMenuChanged;

    /// <summary>
    ///     Similar to SMAPI's own RenderedActiveMenu event, except it ensures that anything drawn to SpriteBatch will be above
    ///     the background fade, but below the actual menu.
    /// </summary>
    public event EventHandler<RenderedActiveMenuEventArgs> RenderedClickableMenu;

    /// <summary>
    ///     Similar to SMAPI's own RenderingActiveMenu event, except it ensures that anything drawn to SpriteBatch will be
    ///     above
    ///     the menu but below the cursor and hover text/items.
    /// </summary>
    public event EventHandler<RenderingActiveMenuEventArgs> RenderingClickableMenu;
}