/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ImJustMatt/StardewMods
**
*************************************************/

namespace StardewMods.Common.Integrations.ToolbarIcons;

/// <summary>
///     A single Toolbar Icon.
/// </summary>
public interface IToolbarIcon
{
    /// <summary>
    ///     Gets or sets a value indicating whether the Toolbar Icon is enabled.
    /// </summary>
    public bool Enabled { get; set; }

    /// <summary>
    ///     Gets the Id of the Toolbar Icon.
    /// </summary>
    public string Id { get; }
}