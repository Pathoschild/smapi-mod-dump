/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LeFauxMatt/StardewMods
**
*************************************************/

namespace StardewMods.ToolbarIcons.Framework.Models;

using StardewMods.Common.Integrations.ToolbarIcons;

/// <inheritdoc />
public sealed class ToolbarIcon : IToolbarIcon
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="ToolbarIcon" /> class.
    /// </summary>
    /// <param name="id">The id of the toolbar icon.</param>
    /// <param name="enabled">Whether the toolbar icon is enabled.</param>
    public ToolbarIcon(string id, bool enabled = true)
    {
        this.Id = id;
        this.Enabled = enabled;
    }

    /// <inheritdoc />
    public bool Enabled { get; set; }

    /// <inheritdoc />
    public string Id { get; set; }
}