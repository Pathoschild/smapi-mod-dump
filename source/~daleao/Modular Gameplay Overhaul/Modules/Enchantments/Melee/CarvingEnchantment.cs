/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Overhaul.Modules.Enchantments.Melee;

#region using directives

using System.Xml.Serialization;
using Netcode;
using StardewValley.Monsters;

#endregion using directives

/// <summary>
///     Attacks on-hit reduce enemy defense, down to a minimum of -1. Armored enemies (i.e., Armored Bugs and shelled Rock Crabs)
///     lose their armor upon hitting 0 defense.
/// </summary>
[XmlType("Mods_DaLion_CarvingEnchantment")]
public sealed class CarvingEnchantment : BaseWeaponEnchantment
{
    /// <inheritdoc />
    public override string GetName()
    {
        return I18n.Get("enchantments.carving.name");
    }

    /// <inheritdoc />
    protected override void _OnDealDamage(Monster monster, GameLocation location, Farmer who, ref int amount)
    {
        if (!who.IsLocalPlayer)
        {
            return;
        }

        monster.resilience.Value--;
        switch (monster)
        {
            case Bug { isArmoredBug.Value: true, resilience.Value: < -3 } bug:
                bug.isArmoredBug.Value = false;
                break;
            case RockCrab crab:
                var shellHealth = Reflector
                    .GetUnboundFieldGetter<RockCrab, NetInt>(crab, "shellHealth")
                    .Invoke(crab).Value;
                if (shellHealth <= 0)
                {
                    break;
                }

                shellHealth--;
                Reflector
                    .GetUnboundFieldGetter<RockCrab, NetInt>(crab, "shellHealth")
                    .Invoke(crab).Value = shellHealth;
                crab.shake(500);
                if (shellHealth <= 0)
                {
                    Reflector
                        .GetUnboundFieldGetter<RockCrab, NetBool>(crab, "shellGone")
                        .Invoke(crab).Value = true;
                    crab.moveTowardPlayer(-1);
                    location.playSound("stoneCrack");
                    Game1.createRadialDebris(location, 14, crab.getTileX(), crab.getTileY(), Game1.random.Next(2, 7), resource: false);
                    Game1.createRadialDebris(location, 14, crab.getTileX(), crab.getTileY(), Game1.random.Next(2, 7), resource: false);
                }

                break;
        }
    }
}
