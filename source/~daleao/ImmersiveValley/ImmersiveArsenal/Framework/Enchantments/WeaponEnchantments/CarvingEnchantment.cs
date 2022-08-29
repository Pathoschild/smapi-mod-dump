/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/sdv-mods
**
*************************************************/

using System;

namespace DaLion.Stardew.Arsenal.Framework.Enchantments;

#region using directives

using StardewValley.Monsters;
using System.Xml.Serialization;

#endregion using directives

/// <summary>Attacks on-hit decrease the enemy's defense.</summary>
[XmlType("Mods_DaLion_CarvingEnchantment")]
public class CarvingEnchantment : BaseWeaponEnchantment
{
    protected override void _OnDealDamage(Monster monster, GameLocation location, Farmer who, ref int amount)
    {
        monster.resilience.Value = Math.Max(monster.resilience.Value - 1, -1);
    }

    public override string GetName() => ModEntry.i18n.Get("enchantments.carving");
}