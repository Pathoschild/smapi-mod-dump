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
using Microsoft.Xna.Framework.Graphics;
using Revitalize.Framework.Minigame.SeasideScrambleMinigame.SSCProjectiles;
using StardustCore.Animations;

namespace Revitalize.Framework.Minigame.SeasideScrambleMinigame.SSCGuns
{
    /// <summary>
    /// A simple gun class for shooting projectiles. Doesn't necessarily *have* to be a gun. Could be a slingshot or anything really.
    /// </summary>
    public class SSCGun
    {
        /// <summary>
        /// Constant that represents the placeholding number for infinite ammo useage.
        /// </summary>
        public const int infiniteAmmo = -1;

        /// <summary>
        /// The projectile this gun uses.
        /// </summary>
        public SSCProjectile _projectile;
        /// <summary>
        /// The sprite for the gun.
        /// </summary>
        public AnimatedSprite sprite;

        /// <summary>
        /// The ammo remaining in the gun.
        /// </summary>
        public int remainingAmmo;
        /// <summary>
        /// The max ammo this gun can hold.
        /// </summary>
        public int maxAmmo;
        /// <summary>
        /// Whether or not this gun has ammo.
        /// </summary>
        public bool hasAmmo
        {
            get
            {
                return this.remainingAmmo > 0 || this.remainingAmmo == SSCGun.infiniteAmmo;
            }
        }
        /// <summary>
        /// How many bullets per shot this gun consumes.
        /// </summary>
        public int consumesXAmmoPerShot;

        /// <summary>
        /// The time in milliseconds it takes to reload a single bullet.
        /// </summary>
        public double reloadSpeed;
        /// <summary>
        /// The time remaining to reload the gun.
        /// </summary>
        public double timeRemainingUntilReload;

        /// <summary>
        /// Checks if the player is reloading the gun.
        /// </summary>
        public bool isReloading;


        /// <summary>
        /// Delay between shots in milliseconds.
        /// </summary>
        public double firingDelay;
        /// <summary>
        /// Remaining milliseconds until gun can fire again.
        /// </summary>
        public double remainingFiringDelay;

        /// <summary>
        /// A reference to the projectile this gun uses.
        /// </summary>
        public SSCProjectile Projectile
        {
            get
            {
                return this._projectile;
            }
        }
        /// <summary>
        /// The positon of the gun.
        /// </summary>
        public Vector2 Position
        {
            get
            {
                return this.sprite.position;
            }
            set
            {
                this.sprite.position = value;
            }
        }

        public SSCGun()
        {

        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="Sprite">The sprite for the gun.</param>
        /// <param name="Projectile">The projectile the gun uses.</param>
        /// <param name="MaxAmmo">The max ammo this gun has.</param>
        /// <param name="FiringRate">The delay between this gun can fire.</param>
        /// <param name="ReloadSpeed">The rate at which the gun reloads.</param>
        /// <param name="ConsumesXAmmo">How many bullets per shot the gun consumes.</param>
        public SSCGun(AnimatedSprite Sprite,SSCProjectile Projectile,int MaxAmmo,double FiringRate,double ReloadSpeed,int ConsumesXAmmo=1)
        {
            this.sprite = Sprite;
            this._projectile = Projectile;

            this.maxAmmo = MaxAmmo;
            this.remainingAmmo = this.maxAmmo;
            this.firingDelay = FiringRate;
            this.reloadSpeed = ReloadSpeed;
            this.timeRemainingUntilReload = this.reloadSpeed;
            this.consumesXAmmoPerShot = ConsumesXAmmo;
        }

        /// <summary>
        /// Update the gun's logic.
        /// </summary>
        /// <param name="time"></param>
        public virtual void update(GameTime time)
        {
            this.remainingFiringDelay -= time.ElapsedGameTime.Milliseconds;
            if (this.remainingFiringDelay <= 0) this.remainingFiringDelay = 0;

            if (this.isReloading)
            {
                this.timeRemainingUntilReload -= time.ElapsedGameTime.TotalMilliseconds;
                //ModCore.log("Reloding: " + this.timeRemainingUntilReload);
                if (this.timeRemainingUntilReload <= 0)
                {
                    this.reload();         
                }
            }
        }

        /// <summary>
        /// Draw the gun to the screen.
        /// </summary>
        /// <param name="b"></param>
        public virtual void draw(SpriteBatch b)
        {
            this.draw(b, this.Position, 4f);
        }

        /// <summary>
        /// Draw the gun to the screen.
        /// </summary>
        /// <param name="b"></param>
        /// <param name="Position"></param>
        /// <param name="Scale"></param>
        public virtual void draw(SpriteBatch b, Vector2 Position,float Scale)
        {
            this.sprite.draw(b, Position, Scale, 0f);
        }

        /// <summary>
        /// What happens when the gun shoots.
        /// </summary>
        /// <param name="Position"></param>
        /// <param name="Direction"></param>
        public virtual void shoot(Vector2 Position, Vector2 Direction)
        {
            if (this.hasAmmo == false)
            {
                this.startReload();
                return;
            }
            if (this.canShoot())
            {

                this.isReloading = false;
                this._projectile.spawnClone(Position, Direction);
                this.remainingFiringDelay = this.firingDelay;
                this.consumeAmmo();
            }
        }

        /// <summary>
        /// What happens when the player starts the reload sequence.
        /// </summary>
        public virtual void startReload()
        {
            //Maybe play a sound effect?
            this.isReloading = true;
        }

        /// <summary>
        /// Checks if the gun can shoot. If out of ammo it starts the reload sequence. If it can shoot it will shoot. 
        /// </summary>
        /// <param name="Position"></param>
        /// <param name="Direction"></param>
        public virtual void tryToShoot(Vector2 Position, Vector2 Direction)
        {
            if (this.hasAmmo == false)
            {
                this.startReload();
                return;
            }
            if (this.canShoot())
            {
                this.shoot(Position, Direction);
                StardewValley.Game1.playSound("coin");
                //StardewValley.Game1.playSound("Cowboy_gunshot");
            }
        }


        /// <summary>
        /// What happens when the gun consumes ammo.
        /// </summary>
        public virtual void consumeAmmo()
        {
            if (this.remainingAmmo == SSCGun.infiniteAmmo)
            {
                return;
            }
            else
            {
                this.remainingAmmo -= this.consumesXAmmoPerShot;
            }
        }


        /// <summary>
        /// What happens when the gun reloads. Can do either reloading a single bullet or the whole clip.
        /// </summary>
        public virtual void reload()
        {
            this.remainingAmmo = this.maxAmmo;
            this.timeRemainingUntilReload = this.reloadSpeed;
            //StardewValley.Game1.soundBank.PlayCue("dwop");
            StardewValley.Game1.playSound("cowboy_gunload");
            if (this.remainingAmmo== this.maxAmmo)
            {
                this.isReloading = false;
            }
            //Maybe play a sound here????
        }

        public virtual bool canShoot()
        {
            return this.hasAmmo && this.remainingFiringDelay <= 0 && this.isReloading == false; //Could remove the isReloding condition and do guns like revolvers.
        }

        public virtual SSCGun getCopy()
        {
            return new SSCGun(new AnimatedSprite(this.sprite.name, this.Position, new AnimationManager(this.sprite.animation.objectTexture, this.sprite.animation.defaultDrawFrame, this.sprite.animation.animations, this.sprite.animation.currentAnimationName), this.sprite.color), this._projectile, this.maxAmmo, this.firingDelay, this.reloadSpeed, this.consumesXAmmoPerShot);
        }
    }
}
