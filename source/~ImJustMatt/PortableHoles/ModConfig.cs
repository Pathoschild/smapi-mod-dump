/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ImJustMatt/StardewMods
**
*************************************************/

namespace StardewMods.PortableHoles;

using System.Globalization;
using System.Text;

/// <summary>
///     Mod config data for Portable Holes.
/// </summary>
internal sealed class ModConfig
{
    /// <summary>
    ///     Gets or sets a value indicating whether damage while falling will be negated.
    /// </summary>
    public bool SoftFall { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether to unlock the recipe automatically.
    /// </summary>
    public bool UnlockAutomatically { get; set; }

    /// <inheritdoc />
    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.AppendLine($"SoftFall: {this.SoftFall.ToString(CultureInfo.InvariantCulture)}");
        sb.AppendLine($"UnlockAutomatically: {this.UnlockAutomatically.ToString(CultureInfo.InvariantCulture)}");
        return sb.ToString();
    }
}