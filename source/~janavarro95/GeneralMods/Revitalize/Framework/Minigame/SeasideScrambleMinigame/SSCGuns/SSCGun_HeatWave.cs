/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/janavarro95/Stardew_Valley_Mods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Revitalize.Framework.Minigame.SeasideScrambleMinigame.SSCProjectiles;
using StardustCore.Animations;

namespace Revitalize.Framework.Minigame.SeasideScrambleMinigame.SSCGuns
{
    public class SSCGun_HeatWave:SSCGun
    {

        public SSCGun_HeatWave(AnimatedSprite Sprite, SSCProjectile Projectile, int MaxAmmo, double FiringRate, double ReloadSpeed, int ConsumesXAmmo = 1):base(Sprite,Projectile,MaxAmmo,FiringRate,ReloadSpeed,ConsumesXAmmo)
        {
        }

        public override void tryToShoot(Vector2 Position, Vector2 Direction)
        {
            if (this.hasAmmo == false)
            {
                this.startReload();
                return;
            }
            if (this.canShoot())
            {
                this.shoot(Position, Direction);
                StardewValley.Game1.playSound("fireball");
                //StardewValley.Game1.playSound("Cowboy_gunshot");
            }
        }

        public override SSCGun getCopy()
        {
            return new SSCGun_HeatWave(new AnimatedSprite(this.sprite.name, this.Position, new AnimationManager(this.sprite.animation.objectTexture, this.sprite.animation.defaultDrawFrame, this.sprite.animation.animations, this.sprite.animation.currentAnimationName), this.sprite.color), this._projectile, this.maxAmmo, this.firingDelay, this.reloadSpeed, this.consumesXAmmoPerShot);
        }
    }
}
