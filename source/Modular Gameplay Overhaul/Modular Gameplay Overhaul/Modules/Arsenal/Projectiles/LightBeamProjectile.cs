/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Overhaul.Modules.Arsenal.Projectiles;

#region using directives

using DaLion.Overhaul.Modules.Arsenal.Enchantments;
using DaLion.Overhaul.Modules.Arsenal.VirtualProperties;
using DaLion.Shared.Extensions;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Monsters;
using StardewValley.Projectiles;
using StardewValley.Tools;

#endregion using directives

/// <summary>A beam of energy fired by <see cref="MeleeWeapon"/>s with the <see cref="InfinityEnchantment"/>.</summary>
internal sealed class LightBeamProjectile : BasicProjectile
{
    public const int TileSheetIndex = 11;

    /// <summary>Initializes a new instance of the <see cref="LightBeamProjectile"/> class.</summary>
    /// <param name="source">The <see cref="MeleeWeapon"/> which fired this projectile.</param>
    /// <param name="firer">The <see cref="Farmer"/> who fired this projectile.</param>
    /// <param name="startingPosition">The projectile's starting position.</param>
    /// <param name="xVelocity">The projectile's starting velocity in the horizontal direction.</param>
    /// <param name="yVelocity">The projectile's starting velocity in the vertical direction.</param>
    /// <param name="rotation">The projectile's starting rotation.</param>
    public LightBeamProjectile(
        MeleeWeapon source,
        Farmer firer,
        Vector2 startingPosition,
        float xVelocity,
        float yVelocity,
        float rotation)
        : base(
            1,
            TileSheetIndex,
            0,
            3,
            0f,
            xVelocity,
            yVelocity,
            startingPosition,
            string.Empty,
            string.Empty,
            false,
            true,
            firer.currentLocation,
            firer)
    {
        this.Firer = firer;
        this.Damage = (int)(source.Get_MinDamage() * (1f + firer.attackIncreaseModifier) / 4f);
        this.rotation = rotation;
        this.ignoreTravelGracePeriod.Value = true;
        this.ignoreMeleeAttacks.Value = true;
        this.maxTravelDistance.Value = 256;
        this.height.Value = 32f;
    }

    public Farmer Firer { get; }

    public int Damage { get; }

    /// <inheritdoc />
    public override void behaviorOnCollisionWithMonster(NPC n, GameLocation location)
    {
        if (n is not Monster { IsMonster: true } monster)
        {
            base.behaviorOnCollisionWithMonster(n, location);
            return;
        }

        Reflector
            .GetUnboundMethodDelegate<Action<BasicProjectile, GameLocation>>(this, "explosionAnimation")
            .Invoke(this, location);
        location.damageMonster(
            monster.GetBoundingBox(),
            this.Damage,
            this.Damage + 1,
            false,
            0f,
            0,
            0f,
            0f,
            true,
            this.Firer);
    }
}
