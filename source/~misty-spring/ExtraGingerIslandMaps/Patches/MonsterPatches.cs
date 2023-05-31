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

namespace ExtraGingerIslandMaps.Patches
{

    [HarmonyPatch(typeof(Monster))]
    internal static class MonsterPatches
    {
        internal static void Apply(Harmony harmony)
        {
            ModEntry.Mon.Log($"Applying Harmony patch \"{nameof(MonsterPatches)}\": postfixing SDV method \"Monster.behaviorAtGameTick\".");
            harmony.Patch(
                original: AccessTools.Method(typeof(Monster), nameof(Monster.behaviorAtGameTick)),
                postfix: new HarmonyMethod(typeof(MonsterPatches), nameof(PostGameTick))
                );
        }

        [HarmonyPostfix]
        [HarmonyPatch(nameof(Monster.behaviorAtGameTick))]
        internal static void PostGameTick(ref LavaLurk instance, GameTime time)
        {
            if(Game1.player.currentLocation != instance.currentLocation)
            {
                return;
            }

            if (!instance.Name.StartsWith("Fire", comparison.OrdinalIgnoreCase) &&
                !instance.Name.StartsWith("Magma", comparison.OrdinalIgnoreCase)) return;
            
            if (!Game1.player.isWearingRing(ModEntry.FireRing)) return;
            if(instance.DamageToFarmer != 0)
            {
                instance.DamageToFarmer = 0;
            }
        }
    }

    [HarmonyPatch(typeof(Bat))]
    internal static class BatPatches
    {
        internal static void Apply(Harmony harmony)
        {
            harmony.Patch(
                original: AccessTools.Method(typeof(Bat), nameof(Bat.onDealContactDamage)),
                prefix: new HarmonyMethod(typeof(BatPatches), nameof(PrefixDealContactDamage))
                );
        }

        [HarmonyPrefix]
        [HarmonyPatch(nameof(Bat.onDealContactDamage))]
        internal static bool PrefixDealContactDamage(ref Bat instance, Farmer who)
        {
            //__instance.onDealContactDamage(who);

            if (!instance.magmaSprite.Value || !instance.Name.Contains("Magma") ||
                !who.isWearingRing(ModEntry.FireRing)) return true;
            instance.DamageToFarmer = 0;
            return false;

        }
    }

    [HarmonyPatch(typeof(BasicProjectile))]
    internal static class ProjectilePatches
    {
        internal static void Apply(Harmony harmony)
        {

            harmony.Patch(
                original: AccessTools.Method(typeof(BasicProjectile), nameof(BasicProjectile.behaviorOnCollisionWithPlayer)),
                prefix: new HarmonyMethod(typeof(ProjectilePatches), nameof(PrefixbehaviorOnCollisionWithPlayer))
                );
        }
    
        
        [HarmonyPrefix]
        [HarmonyPatch(nameof(BasicProjectile.behaviorOnCollisionWithPlayer))]
        internal static bool PrefixbehaviorOnCollisionWithPlayer(BasicProjectile instance, GameLocation location, Farmer player)
        {
            return !player.isWearingRing(ModEntry.FireRing);
        }
    }
}