/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv
**
*************************************************/

namespace DaLion.Enchantments.Framework.Enchantments;

#region using directives

using System.Xml.Serialization;
using DaLion.Core.Framework.Enchantments;

#endregion using directives

/// <summary>Fire an energy projectile when ammunition is not equipped. The projectile is stronger the lower the firer's HP.</summary>
/// <remarks>
///     The quincy projectile has zero knockback and cannot crit, but scales in size and damage with Desperado's
///     overcharge.
/// </remarks>
[XmlType("Mods_DaLion_QuincyEnchantment")]
public sealed class QuincyEnchantment : BaseSlingshotEnchantment
{
    /// <inheritdoc />
    public override string GetName()
    {
        return I18n.Enchantments_Quincy_Name();
    }
}
