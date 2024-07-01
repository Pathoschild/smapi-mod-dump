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
using StardewValley.Extensions;
using StardewValley.Monsters;

#endregion using directives

/// <summary>
///     Attacks that would leave an enemy below 10% max health immediately execute the enemy, converting the remaining health into gold.
///     Consecutive takedowns without taking damage increase the health threshold by 1%.
/// </summary>
[XmlType("Mods_DaLion_MammoniteEnchantment")]
public sealed class MammoniteEnchantment : BaseWeaponEnchantment
{
    internal float Threshold { get; set; } = 0.1f;

    /// <inheritdoc />
    public override string GetName()
    {
        return I18n.Enchantments_Mammonite_Name();
    }

    /// <inheritdoc />
    protected override void _OnDealDamage(Monster monster, GameLocation location, Farmer who, ref int amount)
    {
        base._OnDealDamage(monster, location, who, ref amount);
        if (monster.Health is > 1000 or < 0 || monster.Health >= monster.MaxHealth * this.Threshold)
        {
            return;
        }

        var chance = 1d - (monster.Health / 1000d);
        if (!Game1.random.NextBool(chance))
        {
            return;
        }

        who.Money += monster.Health;
        monster.Health = 0;
        this.Threshold += 0.01f;
    }
}
