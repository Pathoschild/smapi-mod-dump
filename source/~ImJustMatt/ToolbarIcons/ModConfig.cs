/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ImJustMatt/StardewMods
**
*************************************************/

namespace StardewMods.ToolbarIcons;

using System.Collections.Generic;
using System.Globalization;
using System.Text;
using StardewMods.ToolbarIcons.Models;

/// <summary>
///     Mod config data.
/// </summary>
internal class ModConfig
{
    /// <summary>
    ///     Gets or sets a value of the detected toolbar icons.
    /// </summary>
    public List<ToolbarIcon> Icons { get; set; } = new();

    /// <inheritdoc />
    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.AppendLine("Icons:");
        foreach (var icon in this.Icons)
        {
            sb.AppendLine($"{icon.Id}: {icon.Enabled.ToString(CultureInfo.InvariantCulture)}");
        }

        return sb.ToString();
    }
}