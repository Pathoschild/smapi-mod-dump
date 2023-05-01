/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv-mods
**
*************************************************/

// ReSharper disable EqualExpressionComparison
namespace DaLion.Overhaul.Modules.Enchantments.Melee;

#region using directives

using System.Xml.Serialization;

#endregion using directives

/// <summary>Converts critical strike chance and power into raw damage.</summary>
[XmlType("Mods_DaLion_SteadfastEnchantment")]
public sealed class SteadfastEnchantment : BaseWeaponEnchantment
{
    /// <inheritdoc />
    public override string GetName()
    {
        return I18n.Get("enchantments.steadfast.name");
    }
}
