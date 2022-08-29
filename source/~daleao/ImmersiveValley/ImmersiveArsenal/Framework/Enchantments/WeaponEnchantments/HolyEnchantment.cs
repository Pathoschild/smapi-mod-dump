/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Stardew.Arsenal.Framework.Enchantments;

/// <summary>The secondary <see cref="BaseWeaponEnchantment"/> which characterizes the Holy Blade.</summary>
public class HolyEnchantment : BaseWeaponEnchantment
{
    public override bool IsSecondaryEnchantment() => true;

    public override bool IsForge() => false;

    public override int GetMaximumLevel() => 1;

    public override bool ShouldBeDisplayed() => false;
}