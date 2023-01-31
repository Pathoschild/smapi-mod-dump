/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Overhaul.Modules.Arsenal.Enchantments;

#region using directives

using System.Xml.Serialization;
using StardewValley.Monsters;

#endregion using directives

/// <summary>
///     Attacks that would leave an enemy below 10% max health immediately execute the enemy, converting the remaining health into gold.
///     For undead enemies, the threshold is 20%.
/// </summary>
[XmlType("Mods_DaLion_TributeEnchantment")]
public class TributeEnchantment : BaseWeaponEnchantment
{
    internal float Threshold { get; set; } = 0.1f;

    /// <inheritdoc />
    public override string GetName()
    {
        return I18n.Get("enchantments.tribute");
    }

    /// <inheritdoc />
    protected override void _OnDealDamage(Monster monster, GameLocation location, Farmer who, ref int amount)
    {
        var tribute = (int)((monster.MaxHealth * this.Threshold) - monster.Health);
        if (tribute <= 0)
        {
            return;
        }

        monster.Health = 0;
        who.Money += tribute;
    }

    /// <inheritdoc />
    protected override void _OnMonsterSlay(Monster m, GameLocation location, Farmer who)
    {
        this.Threshold += 0.01f;
    }

    /// <inheritdoc />
    protected override void _OnUnequip(Farmer who)
    {
        base._OnUnequip(who);
        if (who.IsLocalPlayer)
        {
            this.Threshold = 0;
        }
    }
}
