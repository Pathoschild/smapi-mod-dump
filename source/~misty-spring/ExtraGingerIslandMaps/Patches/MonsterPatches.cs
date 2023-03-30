/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/misty-spring/StardewMods
**
*************************************************/

using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Monsters;
using StardewValley.Projectiles;
using comparison = System.StringComparison;

namespace ExtraGingerIslandMaps
{

    [HarmonyPatch(typeof(Monster))]
    internal static class MonsterPatches
    {
        internal static void Apply(Harmony harmony)
        {
            ModEntry.Mon.Log($"Applying Harmony patch \"{nameof(MonsterPatches)}\": postfixing SDV method \"Monster.behaviorAtGameTick\".");
            harmony.Patch(
                original: AccessTools.Method(typeof(Monster), nameof(Monster.behaviorAtGameTick)),
                postfix: new HarmonyMethod(typeof(MonsterPatches), nameof(MonsterPatches.PostGameTick))
                );
        }

        [HarmonyPostfix]
        [HarmonyPatch(nameof(Monster.behaviorAtGameTick))]
        internal static void PostGameTick(ref LavaLurk __instance, GameTime time)
        {
            if(Game1.player.currentLocation != __instance.currentLocation)
            {
                return;
            }

            if(__instance.Name.StartsWith("Fire", comparison.OrdinalIgnoreCase) || __instance.Name.StartsWith("Magma",comparison.OrdinalIgnoreCase))
            {
                if (Game1.player.isWearingRing(ModEntry.FireRing))
                {
                    if(__instance.DamageToFarmer != 0)
                    {
                        __instance.DamageToFarmer = 0;
                    }
                }
            }
        }
    }

    [HarmonyPatch(typeof(Bat))]
    internal static class BatPatches
    {
        internal static void Apply(Harmony harmony)
        {
            harmony.Patch(
                original: AccessTools.Method(typeof(StardewValley.Monsters.Bat), nameof(StardewValley.Monsters.Bat.onDealContactDamage)),
                prefix: new HarmonyMethod(typeof(BatPatches), nameof(BatPatches.PrefixDealContactDamage))
                );
        }

        [HarmonyPrefix]
        [HarmonyPatch(nameof(Bat.onDealContactDamage))]
        internal static bool PrefixDealContactDamage(ref Bat __instance, Farmer who)
        {
            //__instance.onDealContactDamage(who);

            if (__instance.magmaSprite.Value && __instance.Name.Contains("Magma") && who.isWearingRing(ModEntry.FireRing))
            {
                __instance.DamageToFarmer = 0;
                return false;
            }

            return true;
        }
    }

    [HarmonyPatch(typeof(BasicProjectile))]
    internal static class ProjectilePatches
    {
        internal static void Apply(Harmony harmony)
        {

            harmony.Patch(
                original: AccessTools.Method(typeof(StardewValley.Projectiles.BasicProjectile), nameof(StardewValley.Projectiles.BasicProjectile.behaviorOnCollisionWithPlayer)),
                prefix: new HarmonyMethod(typeof(ProjectilePatches), nameof(ProjectilePatches.PrefixbehaviorOnCollisionWithPlayer))
                );
        }
    
        
        [HarmonyPrefix]
        [HarmonyPatch(nameof(BasicProjectile.behaviorOnCollisionWithPlayer))]
        internal static bool PrefixbehaviorOnCollisionWithPlayer(BasicProjectile __instance, GameLocation location, Farmer player)
        {
            if(player.isWearingRing(ModEntry.FireRing))
            {
                return false;
            }
            return true;
        }
    }
}