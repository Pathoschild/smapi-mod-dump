/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LeFauxMatt/StardewMods
**
*************************************************/

namespace StardewMods.OrdinaryCapsule;

using System.Globalization;
using System.Text;

/// <summary>
///     Mod config data for Ordinary Capsule.
/// </summary>
internal sealed class ModConfig
{
    /// <summary>
    ///     Gets or sets the chance that an OrdinaryCapsule will break.
    /// </summary>
    public float BreakChance { get; set; } = 0;

    /// <summary>
    ///     Gets or sets the default production time for supported items.
    /// </summary>
    public int DefaultProductionTime { get; set; } = 1440;

    /// <summary>
    ///     Gets or sets a value indicating whether everything can be duplicated.
    /// </summary>
    public bool DuplicateEverything { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether to unlock the recipe automatically.
    /// </summary>
    public bool UnlockAutomatically { get; set; }

    /// <inheritdoc />
    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.AppendLine($"BreakChance: {this.DefaultProductionTime.ToString(CultureInfo.InvariantCulture)}");
        sb.AppendLine($"DefaultProductionTime: {this.DefaultProductionTime.ToString(CultureInfo.InvariantCulture)}");
        sb.AppendLine($"DuplicateEverything: {this.DefaultProductionTime.ToString(CultureInfo.InvariantCulture)}");
        sb.AppendLine($"UnlockAutomatically: {this.UnlockAutomatically.ToString(CultureInfo.InvariantCulture)}");
        return sb.ToString();
    }
}