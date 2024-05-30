/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LeFauxMatt/StardewMods
**
*************************************************/

namespace StardewMods.ToolbarIcons.Framework.Models.Events;

using StardewMods.Common.Services.Integrations.ToolbarIcons;

/// <inheritdoc cref="IIconPressedEventArgs" />
internal sealed class IconPressedEventArgs : EventArgs, IIconPressedEventArgs
{
    /// <summary>Initializes a new instance of the <see cref="IconPressedEventArgs" /> class.</summary>
    /// <param name="id">The icon id.</param>
    /// <param name="button">The button.</param>
    public IconPressedEventArgs(string id, SButton button)
    {
        this.Button = button;
        this.Id = id;
    }

    /// <inheritdoc />
    public SButton Button { get; }

    /// <inheritdoc />
    public string Id { get; }
}