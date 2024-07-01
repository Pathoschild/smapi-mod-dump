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
using StardewValley.Tools;

#endregion using directives

/// <summary>Replaces the defensive parry special move with a stabbing lunge attack.</summary>
[XmlType("Mods_DaLion_StabbingEnchantment")]
public sealed class StabbingEnchantment : BaseWeaponEnchantment
{
    /// <inheritdoc />
    public override bool CanApplyTo(Item item)
    {
        return item is MeleeWeapon weapon && weapon.type.Value == MeleeWeapon.defenseSword && !weapon.isScythe();
    }

    /// <inheritdoc />
    public override string GetName()
    {
        return I18n.Enchantments_Stabbing_Name();
    }
}
