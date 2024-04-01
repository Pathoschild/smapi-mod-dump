/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LeFauxMatt/StardewMods
**
*************************************************/

namespace StardewMods.Common.Services.Integrations.ToolbarIcons;

#pragma warning disable CA1711

/// <summary>Represents the event arguments for a toolbar icon being pressed.</summary>
public interface IIconPressedEventArgs
{
    /// <summary>Gets the id of the icon that was pressed.</summary>
    string Id { get; }

    /// <summary>Gets the button that was pressed.</summary>
    SButton Button { get; }
}