/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ImJustMatt/StardewMods
**
*************************************************/

namespace StardewMods.TooManyAnimals;

using System.Globalization;
using System.Text;

/// <summary>
///     Mod config data.
/// </summary>
internal class ModConfig
{
    /// <summary>
    ///     Gets or sets a value indicating how many animals will be shown in the Animal Purchase menu at once.
    /// </summary>
    public int AnimalShopLimit { get; set; } = 30;

    /// <summary>
    ///     Gets or sets the control scheme.
    /// </summary>
    public Controls ControlScheme { get; set; } = new();

    /// <inheritdoc />
    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.AppendLine($"AnimalShopLimit: {this.AnimalShopLimit.ToString(CultureInfo.InvariantCulture)}");
        return sb.ToString();
    }
}