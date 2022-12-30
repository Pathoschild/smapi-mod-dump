/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LeFauxMatt/StardewMods
**
*************************************************/

namespace StardewMods.GarbageDay;

using System.Text;
using StardewMods.Common.Enums;

/// <summary>
///     Mod config data for Garbage Day.
/// </summary>
internal sealed class ModConfig
{
    /// <summary>
    ///     Gets or sets the day of the week that garbage is collected.
    /// </summary>
    public DayOfWeek GarbageDay { get; set; } = DayOfWeek.Monday;

    /// <inheritdoc />
    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.AppendLine($"GarbageDay: {this.GarbageDay.ToStringFast()}");
        return sb.ToString();
    }
}