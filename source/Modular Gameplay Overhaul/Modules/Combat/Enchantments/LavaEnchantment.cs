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
using DaLion.Overhaul.Modules.Combat.Extensions;
using Microsoft.Xna.Framework;
using StardewValley.Monsters;

#endregion using directives

/// <summary>The secondary <see cref="BaseWeaponEnchantment"/> which characterizes the Lava Katana.</summary>
[XmlType("Mods_DaLion_LavaEnchantment")]
public class LavaEnchantment : BaseWeaponEnchantment
{
    private readonly Random _random = new(Guid.NewGuid().GetHashCode());

    /// <inheritdoc />
    public override bool IsSecondaryEnchantment()
    {
        return true;
    }

    /// <inheritdoc />
    public override bool IsForge()
    {
        return false;
    }

    /// <inheritdoc />
    public override int GetMaximumLevel()
    {
        return 1;
    }

    /// <inheritdoc />
    public override bool ShouldBeDisplayed()
    {
        return false;
    }

    /// <inheritdoc />
    protected override void _OnDealDamage(Monster monster, GameLocation location, Farmer who, ref int amount)
    {
        base._OnDealDamage(monster, location, who, ref amount);
        if (CombatModule.ShouldEnable && this._random.NextDouble() < 0.2)
        {
            monster.Burn(who);
        }

        var monsterBox = monster.GetBoundingBox();
        var sprites = new TemporaryAnimatedSprite(
            362,
            Game1.random.Next(50, 120),
            6,
            1,
            new Vector2(monsterBox.Center.X - 32, monsterBox.Center.Y - 32),
            flicker: false,
            flipped: false);
        sprites.color = Color.OrangeRed;

        Reflector
            .GetStaticFieldGetter<Multiplayer>(typeof(Game1), "multiplayer")
            .Invoke()
            .broadcastSprites(location, sprites);
    }
}
