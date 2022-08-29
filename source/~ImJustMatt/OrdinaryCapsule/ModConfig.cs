/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ImJustMatt/StardewMods
**
*************************************************/

namespace StardewMods.OrdinaryCapsule;

using System.Globalization;
using System.Text;

/// <summary>
///     Mod config data.
/// </summary>
public class ModConfig
{
    /// <summary>
    ///     Gets or sets a value indicating whether to unlock the recipe automatically.
    /// </summary>
    public bool UnlockAutomatically { get; set; }

    /// <inheritdoc />
    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.AppendLine($"UnlockAutomatically: {this.UnlockAutomatically.ToString(CultureInfo.InvariantCulture)}");
        return sb.ToString();
    }
}