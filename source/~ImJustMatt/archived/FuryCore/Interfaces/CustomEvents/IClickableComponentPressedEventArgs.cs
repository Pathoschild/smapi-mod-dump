/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ImJustMatt/StardewMods
**
*************************************************/

#nullable disable

namespace StardewMods.FuryCore.Interfaces.CustomEvents;

using System;
using StardewModdingAPI;
using StardewMods.FuryCore.Interfaces.ClickableComponents;

/// <summary>
///     <see cref="EventArgs" /> for the ClickableComponentPressed event.
/// </summary>
public interface IClickableComponentPressedEventArgs
{
    /// <summary>
    ///     Gets the button that was pressed.
    /// </summary>
    public SButton Button { get; }

    /// <summary>
    ///     Gets the component which was pressed.
    /// </summary>
    public IClickableComponent Component { get; }

    /// <summary>
    ///     Gets a value indicating if the input is currently suppressed.
    /// </summary>
    public Func<bool> IsSuppressed { get; }

    /// <summary>
    ///     Gets an method that will suppress the input.
    /// </summary>
    public Action SuppressInput { get; }
}