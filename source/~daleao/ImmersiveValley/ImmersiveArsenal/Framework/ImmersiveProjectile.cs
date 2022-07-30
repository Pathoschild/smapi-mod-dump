/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Stardew.Arsenal.Framework;

#region using directives

using Common.Extensions.Reflection;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Monsters;
using StardewValley.Projectiles;
using StardewValley.Tools;
using System;

#endregion using directives

/// <summary>A Slingshot <see cref="BasicProjectile"/> that remembers where it came from.</summary>
internal class ImmersiveProjectile : BasicProjectile
{
    private static Action<BasicProjectile, GameLocation>? _ExplosionAnimation;
    public Slingshot WhatFiredMe { get; init; }

    public ImmersiveProjectile(Slingshot whatFiredMe, int damageToFarmer, int parentSheetIndex, int bouncesTillDestruct,
        int tailLength, float rotationVelocity, float xVelocity, float yVelocity, Vector2 startingPosition,
        string collisionSound, string firingSound, bool explode, bool damagesMonsters = false,
        GameLocation? location = null, Character? firer = null, bool spriteFromObjectSheet = false,
        onCollisionBehavior? collisionBehavior = null)
        : base(damageToFarmer, parentSheetIndex, bouncesTillDestruct, tailLength, rotationVelocity, xVelocity,
            yVelocity, startingPosition, collisionSound, firingSound, explode, damagesMonsters, location, firer,
            spriteFromObjectSheet, collisionBehavior)
    {
        WhatFiredMe = whatFiredMe;
        if (damagesMonsters && firer is Farmer && ModEntry.Config.RemoveSlingshotGracePeriod)
            ignoreTravelGracePeriod.Value = true;
    }

    public override void behaviorOnCollisionWithMonster(NPC n, GameLocation location)
    {
        if (!damagesMonsters.Value) return;

        if (n is not Monster monster)
        {
            base.behaviorOnCollisionWithMonster(n, location);
            return;
        }

        _ExplosionAnimation ??= typeof(BasicProjectile).RequireMethod("explosionAnimation")
            .CompileUnboundDelegate<Action<BasicProjectile, GameLocation>>();
        _ExplosionAnimation(this, location);

        var firer = theOneWhoFiredMe.Get(location) is Farmer farmer ? farmer : Game1.player;
        var damage = damageToFarmer.Value;
        var knockback = WhatFiredMe.GetEnchantmentLevel<AmethystEnchantment>() * (1f + firer.knockbackModifier);
        var crate = ModEntry.Config.AllowSlingshotCrit
            ? (0.05f + 0.046f * WhatFiredMe.GetEnchantmentLevel<AmethystEnchantment>()) *
              (1f + firer.critChanceModifier)
            : 0;
        var cpower =
            (1f + (ModEntry.Config.RebalancedEnchants ? 0.5f : 0.1f) *
                WhatFiredMe.GetEnchantmentLevel<JadeEnchantment>()) * (1f + firer.critPowerModifier);
        location.damageMonster(monster.GetBoundingBox(), damage, damage + 1, false, knockback, 0, crate, cpower, false,
            firer);
    }
}