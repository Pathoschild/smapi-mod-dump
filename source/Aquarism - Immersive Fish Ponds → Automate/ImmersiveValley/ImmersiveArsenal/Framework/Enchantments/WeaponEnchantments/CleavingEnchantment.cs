/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/smapi-mods
**
*************************************************/

namespace DaLion.Stardew.Arsenal.Framework.Enchantments;

#region using directives

using Common.Extensions.Stardew;
using StardewValley.Monsters;
using System.Xml.Serialization;

#endregion using directives

/// <summary>Attacks on-hit deal 60% - 20% (based on distance) damage to other enemies near the target.</summary>
[XmlType("Mods_DaLion_CleavingEnchantment")]
public class CleavingEnchantment : BaseWeaponEnchantment
{
    protected override void _OnDealDamage(Monster monster, GameLocation location, Farmer who, ref int amount)
    {
        for (var i = location.characters.Count - 1; i >= 0; --i)
        {
            var character = location.characters[i];
            if (character is not Monster { IsMonster: true, Health: > 0 } other || other.IsInvisible ||
                other.isInvincible())
                continue;

            var distance = other.DistanceTo(monster);
            if (distance > 3) continue;

            var damage = (int)(amount * (0.8 - 0.2 * distance));
            var (x, y) = Utility.getAwayFromPositionTrajectory(other.GetBoundingBox(), monster.Position);
            other.takeDamage(damage, (int)x, (int)y, false, double.MaxValue, who);
        }
    }

    public override string GetName() => ModEntry.i18n.Get("enchantments.cleaving");
}