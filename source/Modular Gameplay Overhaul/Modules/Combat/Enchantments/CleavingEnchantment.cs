/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/modular-overhaul
**
*************************************************/

namespace DaLion.Overhaul.Modules.Combat.Enchantments;

#region using directives

using System.Xml.Serialization;
using DaLion.Shared.Extensions.Stardew;
using StardewValley.Monsters;

#endregion using directives

/// <summary>Attacks on-hit spread 60% - 20% (based on distance) of the damage to other enemies around the player.</summary>
[XmlType("Mods_DaLion_CleavingEnchantment")]
public sealed class CleavingEnchantment : BaseWeaponEnchantment
{
    /// <inheritdoc />
    public override string GetName()
    {
        return I18n.Enchantments_Cleaving_Name();
    }

    /// <inheritdoc />
    protected override void _OnDealDamage(Monster monster, GameLocation location, Farmer who, ref int amount)
    {
        if (!who.IsLocalPlayer)
        {
            return;
        }

        for (var i = location.characters.Count - 1; i >= 0; i--)
        {
            var character = location.characters[i];
            if (character == monster || character is not Monster { IsMonster: true, Health: > 0 } other || other.IsInvisible ||
                other.isInvincible())
            {
                continue;
            }

            var distance = other.DistanceTo(who);
            if (distance > 3)
            {
                continue;
            }

            var damage = (int)(amount * (0.8 - (0.2 * distance)));
            var (x, y) = Utility.getAwayFromPositionTrajectory(other.GetBoundingBox(), monster.Position);
            other.takeDamage(damage, (int)x, (int)y, false, double.MaxValue, who);
            if (other.Health <= 0)
            {
                other.Die(who);
            }
        }
    }
}
