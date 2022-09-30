/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ImJustMatt/StardewMods
**
*************************************************/

namespace StardewMods.ShoppingCart;

using System.Globalization;
using System.Text;

/// <summary>
///     Mod config data for Shopping Cart.
/// </summary>
internal sealed class ModConfig
{
    /// <summary>
    ///     Gets or sets the amount of an item to purchase when holding shift.
    /// </summary>
    public int ShiftClickQuantity { get; set; } = 5;

    /// <inheritdoc />
    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.AppendLine($"ShiftClickQuantity: {this.ShiftClickQuantity.ToString(CultureInfo.InvariantCulture)}");
        return sb.ToString();
    }
}