/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Overhaul.Modules.Enchantments.Projectiles;

#region using directives

using DaLion.Overhaul.Modules.Enchantments.Ranged;
using DaLion.Overhaul.Modules.Slingshots.VirtualProperties;
using Microsoft.Xna.Framework;
using StardewValley.Monsters;
using StardewValley.Projectiles;
using StardewValley.Tools;

#endregion using directives

/// <summary>An energy projectile fired by a <see cref="Slingshot"/> which has the <see cref="QuincyEnchantment"/>.</summary>
internal sealed class QuincyProjectile : BasicProjectile
{
    public const int TileSheetIndex = 14;

    /// <summary>Initializes a new instance of the <see cref="QuincyProjectile"/> class.</summary>
    /// <remarks>Required for multiplayer syncing.</remarks>
    public QuincyProjectile()
        : base()
    {
    }

    /// <summary>Initializes a new instance of the <see cref="QuincyProjectile"/> class.</summary>
    /// <param name="source">The <see cref="Slingshot"/> which fired this projectile.</param>
    /// <param name="firer">The <see cref="Farmer"/> who fired this projectile.</param>
    /// <param name="damage">The un-mitigated damage this projectile will cause.</param>
    /// <param name="overcharge">The amount of overcharge with which the projectile was fired.</param>
    /// <param name="startingPosition">The projectile's starting position.</param>
    /// <param name="xVelocity">The projectile's starting velocity in the horizontal direction.</param>
    /// <param name="yVelocity">The projectile's starting velocity in the vertical direction.</param>
    /// <param name="rotationVelocity">The projectile's starting rotational velocity.</param>
    public QuincyProjectile(
        Slingshot source,
        Farmer firer,
        float damage,
        float overcharge,
        Vector2 startingPosition,
        float xVelocity,
        float yVelocity,
        float rotationVelocity)
        : base(
            (int)damage,
            TileSheetIndex,
            0,
            5,
            rotationVelocity,
            xVelocity,
            yVelocity,
            startingPosition,
            "debuffHit",
            "debuffSpell",
            false,
            true,
            firer.currentLocation,
            firer)
    {
        this.Firer = firer;
        this.Damage = (int)(this.damageToFarmer.Value * source.Get_EffectiveDamageModifier() *
                            (1f + firer.attackIncreaseModifier) * overcharge);
        this.Overcharge = overcharge;
        this.startingScale.Value *= overcharge * overcharge;
        if (SlingshotsModule.Config.DisableGracePeriod)
        {
            this.ignoreTravelGracePeriod.Value = true;
        }
    }

    public Farmer? Firer { get; }

    public int Damage { get; }

    public float Overcharge { get; }

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

    /// <summary>Replaces BasicProjectile.explosionAnimation.</summary>
    /// <param name="location">The <see cref="GameLocation"/>.</param>
    public void ExplosionAnimation(GameLocation location)
    {
        //location.temporarySprites.Add(
        //    new TemporaryAnimatedSprite(
        //        $"{Manifest.UniqueID}/QuincyCollisionAnimation",
        //        new Rectangle(    0, 0, 64, 64),
        //        50f,
        //        1,
        //        1,
        //        this.position,
        //        false,
        //        Game1.random.NextBool())
        //    {
        //        scale = this.Overcharge,
        //    });
    }
}
