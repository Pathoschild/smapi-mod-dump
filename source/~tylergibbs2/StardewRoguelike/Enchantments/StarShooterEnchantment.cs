/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tylergibbs2/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Projectiles;
using StardewValley.Tools;
using System;

namespace StardewRoguelike.Enchantments
{
    internal class StarShooterEnchantment : BaseWeaponEnchantment
    {
        protected override void _OnSwing(MeleeWeapon weapon, Farmer farmer)
        {
            if (farmer != Game1.player)
                return;

            Point playerCenter = farmer.GetBoundingBox().Center;
            Vector2 shotOrigin = new(playerCenter.X, playerCenter.Y);
            float fireAngle = 0f;

            switch (farmer.FacingDirection)
            {
                case Game1.up:
                    fireAngle = 90f;
                    shotOrigin.X -= 32;
                    shotOrigin.Y -= 112;
                    break;
                case Game1.right:
                    fireAngle = 0f;
                    shotOrigin.Y -= 64;
                    break;
                case Game1.down:
                    fireAngle = 270f;
                    shotOrigin.X -= 32;
                    shotOrigin.Y -= 16;
                    break;
                case Game1.left:
                    fireAngle = 180f;
                    shotOrigin.Y -= 64;
                    shotOrigin.X -= 64;
                    break;
            }

            Vector2 shotVelocity = new((float)Math.Cos(RoguelikeUtility.DegreesToRadians(fireAngle)), -(float)Math.Sin(RoguelikeUtility.DegreesToRadians(fireAngle)));
            shotVelocity *= 10f;
            BasicProjectile projectile = new(Game1.random.Next(weapon.minDamage.Value, weapon.maxDamage.Value), 16, 0, 1, 0.16f, shotVelocity.X, shotVelocity.Y, shotOrigin, "", "fallenstar", false, true, farmer.currentLocation, farmer, false, null);
            projectile.ignoreTravelGracePeriod.Value = true;
            projectile.ignoreMeleeAttacks.Value = true;
            farmer.currentLocation.projectiles.Add(projectile);
        }

        public override string GetName()
        {
            return I18n.Enchantments_StarShooter();
        }
    }
}
