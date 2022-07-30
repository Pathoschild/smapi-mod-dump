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

#region using directives

using StardewValley;

#endregion using directives

/// <summary>The secondary <see cref="BaseWeaponEnchantment"/> which characterizes Infinity weapons.</summary>
public class InfinityEnchantment : BaseWeaponEnchantment
{
    public override bool IsSecondaryEnchantment()
    {
        return true;
    }

    public override bool IsForge()
    {
        return false;
    }

    public override int GetMaximumLevel()
    {
        return 1;
    }

    public override bool ShouldBeDisplayed()
    {
        return false;
    }
}