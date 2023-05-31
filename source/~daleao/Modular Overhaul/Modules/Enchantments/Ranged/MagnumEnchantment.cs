/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Overhaul.Modules.Enchantments.Ranged;

#region using directives

using System.Xml.Serialization;

#endregion using directives

/// <summary>Chance to not consume ammo.</summary>
[XmlType("Mods_DaLion_MagnumEnchantment")]
public sealed class MagnumEnchantment : BaseSlingshotEnchantment
{
    /// <inheritdoc />
    public override string GetName()
    {
        return I18n.Enchantments_Magnum_Name();
    }
}
