/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/theLion/smapi-mods
**
*************************************************/

#nullable enable
namespace DaLion.Stardew.Professions.Framework.Patches.Combat;

#region using directives

using System;
using System.Reflection;
using HarmonyLib;
using JetBrains.Annotations;
using Netcode;
using StardewValley;
using StardewValley.Monsters;
using StardewValley.Network;
using StardewValley.Projectiles;

using Stardew.Common.Extensions;
using Extensions;
using Ultimate;

#endregion using directives

[UsedImplicitly]
internal class BasicProjectileBehaviorOnCollisionWithMonsterPatch : BasePatch
{
    private static readonly MethodInfo _ExplosionAnimation = typeof(BasicProjectile).MethodNamed("explosionAnimation");

    /// <summary>Construct an instance.</summary>
    internal BasicProjectileBehaviorOnCollisionWithMonsterPatch()
    {
        Original = RequireMethod<BasicProjectile>(nameof(BasicProjectile.behaviorOnCollisionWithMonster));
    }

    #region harmony patches

    /// <summary>Patch for detect overcharged pierce shots + perform prestiged Rascal stun shot.</summary>
    [HarmonyPrefix]
    private static bool BasicProjectileBehaviorOnCollisionWithMonsterPrefix(BasicProjectile __instance,
        NetBool ___damagesMonsters, NetCharacterRef ___theOneWhoFiredMe, int ___travelTime, NPC n,
        GameLocation location)
    {
        try
        {
            if (!___damagesMonsters.Value) return false; // don't run original logic

            if (n is not Monster monster) return true; // run original logic

            var firer = ___theOneWhoFiredMe.Get(location) is Farmer farmer ? farmer : Game1.player;
            
            // get overcharge
            var bulletPower = 1f;
            if (ModEntry.PlayerState.OverchargedBullets.TryGetValue(__instance.GetHashCode(), out var overcharge))
                bulletPower += overcharge;

            // check for piercing
            if (Game1.random.NextDouble() < (bulletPower - 1f) / 2f)
            {
                ModEntry.PlayerState.PiercedBullets.Add(__instance.GetHashCode());
            }
            else
            {
                _ExplosionAnimation.Invoke(__instance, new object?[] {location});
                ModEntry.PlayerState.OverchargedBullets.Remove(__instance.GetHashCode());
            }

            location.damageMonster(monster.GetBoundingBox(), __instance.damageToFarmer.Value,
                __instance.damageToFarmer.Value + 1, false, bulletPower, 0, 0f, 1f, false, firer);

            // check for stun
            var didStun = false;
            if (firer.HasProfession(Profession.Rascal, true) &&
                ModEntry.PlayerState.BouncedBullets.Remove(__instance.GetHashCode()))
            {
                monster.stunTime = 5000;
                didStun = true;
            }

            // increment Desperado ultimate meter
            if (firer.IsLocalPlayer && ModEntry.PlayerState.RegisteredUltimate is DeathBlossom {IsActive: false})
                ModEntry.PlayerState.RegisteredUltimate.ChargeValue += (didStun ? 18 : 12) - 10 * firer.health / firer.maxHealth;

            return false; // don't run original logic
        }
        catch (Exception ex)
        {
            Log.E($"Failed in {MethodBase.GetCurrentMethod()?.Name}:\n{ex}");
            return true; // default to original logic
        }
    }

    #endregion harmony patches
}