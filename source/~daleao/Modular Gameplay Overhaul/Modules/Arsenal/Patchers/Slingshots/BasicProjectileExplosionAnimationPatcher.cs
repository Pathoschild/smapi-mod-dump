/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Overhaul.Modules.Arsenal.Patchers.Slingshots;

#region using directives

using DaLion.Overhaul.Modules.Arsenal.Projectiles;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley.Projectiles;

#endregion using directives

[UsedImplicitly]
internal class BasicProjectileExplosionAnimationPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="BasicProjectileExplosionAnimationPatcher"/> class.</summary>
    internal BasicProjectileExplosionAnimationPatcher()
    {
        this.Target = this.RequireMethod<BasicProjectile>("explosionAnimation");
    }

    #region harmony patches

    /// <summary>
    ///     Snowball collision animation, which prefers <see cref="Projectile.position"/> instead of the <see cref="Rectangle.Center"/>
    ///     of <see cref="Projectile.getBoundingBox"/>.
    /// </summary>
    [HarmonyPostfix]
    private static void BasicProjectileExplosionAnimationPostfix(BasicProjectile __instance, GameLocation location)
    {
        switch (__instance)
        {
            case SnowballProjectile snowball:
                snowball.ExplosionAnimation(location);
                break;
        }
    }

    #endregion harmony patches
}
