using Harmony;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Monsters;
using StardewValley.Projectiles;
using System;

namespace DeluxeHats.Hats
{
    public static class SkeletonMask
    {
        public const string Name = "Skeleton Mask";
        public const string Description = "Shoot out bones when you get hit that deal 40 damage on impact.";
        public static void Activate()
        {
            HatService.Harmony.Patch(
                original: AccessTools.Method(typeof(Farmer), nameof(Farmer.takeDamage)), 
                prefix: new HarmonyMethod(typeof(SkeletonMask), nameof(SkeletonMask.TakeDamage_Prefix)));
        }
        public static void Disable()
        {
            HatService.Harmony.Unpatch(
                AccessTools.Method(typeof(Farmer), nameof(Farmer.takeDamage)),
                HarmonyPatchType.Prefix,
                HatService.HarmonyId);
        }

        private static bool TakeDamage_Prefix(Monster damager)
        {
            try
            {
                if (Game1.eventUp || Game1.player.FarmerSprite.isPassingOut())
                { 
                    return true;
                }

                bool flag1 = (damager == null || !damager.isInvincible()) && (damager == null || !(damager is GreenSlime) && !(damager is BigSlime) || !Game1.player.isWearingRing(520));
                bool flag3 = !Game1.player.temporarilyInvincible && !Game1.player.isEating && !Game1.fadeToBlack && !Game1.buffsDisplay.hasBuff(21);

                if (!(flag1 & flag3)) 
                {
                    return true;
                }

                Game1.currentLocation.projectiles.Add(new BasicProjectile(40, 4, 0, 0, 0.202f, 10, 10, new Vector2(Game1.player.Position.X, Game1.player.Position.Y - 32), "skeletonHit", "skeletonStep", false, true, Game1.currentLocation, Game1.player, false, null));
                Game1.currentLocation.projectiles.Add(new BasicProjectile(40, 4, 0, 0, 0.202f, -10, 10, new Vector2(Game1.player.Position.X, Game1.player.Position.Y - 32), "skeletonHit", "skeletonStep", false, true, Game1.currentLocation, Game1.player, false, null));
                Game1.currentLocation.projectiles.Add(new BasicProjectile(40, 4, 0, 0, 0.202f, 10, -10, new Vector2(Game1.player.Position.X, Game1.player.Position.Y - 32), "skeletonHit", "skeletonStep", false, true, Game1.currentLocation, Game1.player, false, null));
                Game1.currentLocation.projectiles.Add(new BasicProjectile(40, 4, 0, 0, 0.202f, -10, -10, new Vector2(Game1.player.Position.X, Game1.player.Position.Y - 32), "skeletonHit", "skeletonStep", false, true, Game1.currentLocation, Game1.player, false, null));
                return true;
            }
            catch (Exception ex)
            {
                HatService.Monitor.Log($"Failed in {nameof(TakeDamage_Prefix)}:\n{ex}");
                return true;
            }
        }
    }
}
