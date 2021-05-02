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
using Revitalize.Framework.Minigame.SeasideScrambleMinigame.SSCMaps;
using Revitalize.Framework.Minigame.SeasideScrambleMinigame.SSCProjectiles;
using Revitalize.Framework.Utilities;
using StardewValley;
using StardustCore.Animations;
using AnimatedSprite = StardustCore.Animations.AnimatedSprite;

namespace Revitalize.Framework.Minigame.SeasideScrambleMinigame.SSCEnemies
{
    public class SSCE_Target:SSCEnemies.SSCEnemy
    {

        private bool targetHit;

        public Vector2 direction;
        public float speed;
        public Vector2 Velocity
        {
            get
            {
                return this.direction * this.speed;
            }
        }

        public SSCE_Target():base()
        {

        }

        public SSCE_Target(StardustCore.Animations.AnimatedSprite Sprite, int MoveSpeed, int MaxHealth, Vector2 HitBoxDimensions,float Scale,Vector2 Direction,float Speed):base(Sprite,MoveSpeed,MaxHealth,HitBoxDimensions,Scale)
        {
            this.direction = Direction;
            this.speed = Speed;
        }

        public override void die()
        {
            this.playDeathAnimation();
            Game1.soundBank.PlayCue("breakingGlass");
        }

        public void playDeathAnimation()
        {
            this.sprite.animation.playAnimationOnce("Die");
            this.targetHit = true;
        }

        /// <summary>
        /// Updates the state of the enemy.
        /// </summary>
        /// <param name="time"></param>
        public override void update(GameTime time)
        {
            if(this.sprite.animation.IsAnimationPlaying==false && this.targetHit)
            {
                this.shouldDie = true;
            }
            this.updateMovement();
        }

        public override void updateMovement()
        {
            this.Position += this.Velocity;
        }

        /// <summary>
        /// What happens when the target collides with a projectile.
        /// </summary>
        /// <param name="other"></param>
        public override void onCollision(SSCProjectile other)
        {
            if (this.targetHit) return;
            if (other is SSCProjectiles.SSCProjectile)
            {
                this.CurrentHealth -= other.damage;
                this.die();
                if (SeasideScramble.self.currentMap is ShootingGallery)
                {
                    (SeasideScramble.self.currentMap as ShootingGallery).addScore((other.owner as SSCPlayer).playerID, 1);
                }
            }
        }


        public static void Spawn_SSCE_Target(Vector2 Position,Color Color)
        {
            Spawn_SSCE_Target(Position, Color, Vector2.Zero, 0f);
        }

        /// <summary>
        /// Spawn a target enemy with the given paramaters.
        /// </summary>
        /// <param name="Position"></param>
        /// <param name="Color"></param>
        /// <param name="Direction"></param>
        /// <param name="Speed"></param>
        public static void Spawn_SSCE_Target(Vector2 Position, Color Color,Vector2 Direction, float Speed)
        {
            SSCE_Target target = new SSCE_Target(new AnimatedSprite("TargetPractice", Position, new AnimationManager(SeasideScramble.self.textureUtils.getExtendedTexture("Enemies", "Target"), new Animation(0, 0, 16, 16), new Dictionary<string, List<Animation>>() {
                { "None",new List<Animation>(){
                    new Animation(0,0,16,16)
                } },
                {"Die",new List<Animation>()
                {
                    new Animation(0,0,16,16,10),
                    new Animation(16,0,16,16,10),
                    new Animation(32,0,16,16,10)
                }
                }

            }, "None"), Color), 0, 1, new Vector2(16, 16), 4f, Direction.UnitVector(), Speed);
            SeasideScramble.self.entities.addEnemy(target);
        }
    }
}
