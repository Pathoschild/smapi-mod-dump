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
using StardustCore.Animations;

namespace Revitalize.Framework.Minigame.SeasideScrambleMinigame.SSCProjectiles
{
    public class SSCProjectile
    {
        /// <summary>
        /// The sprite for the projectile/
        /// </summary>
        public AnimatedSprite sprite;
        /// <summary>
        /// The direction the projectile travels.
        /// </summary>
        public Vector2 direction;
        /// <summary>
        /// The speed for the projectile.
        /// </summary>
        public float speed;
        /// <summary>
        /// The scale of the projectile.
        /// </summary>
        public float scale;
        /// <summary>
        /// The hitbox for the projectile.
        /// </summary>
        public Rectangle hitBox;
        /// <summary>
        /// The damage the projectile does upon contact.
        /// </summary>
        public int damage;
        /// <summary>
        /// The position of the projectile. Also resets the bounding box x,y location.
        /// </summary>
        public Vector2 position
        {
            get
            {
                return this.sprite.position;
            }
            set
            {
                this.sprite.position = value;
                this.hitBox.X =(int) this.sprite.position.X;
                this.hitBox.Y = (int)this.sprite.position.Y;
            }
        }
        /// <summary>
        /// The color of the projectile.
        /// </summary>
        public Color color
        {
            get
            {
                return this.sprite.color;
            }
            set
            {
                this.sprite.color = value;
            }
        }

        /// <summary>
        /// The max amount of frames this projectile lives for.
        /// </summary>
        public int maxLifeSpan;
        /// <summary>
        /// The current lifespan for the projectile.
        /// </summary>
        public int currentLifeSpan;

        /// <summary>
        /// The object that spawned this projectile.
        /// </summary>
        public object owner;

        /// <summary>
        /// The velocity of the projectile.
        /// </summary>
        public Vector2 Velocity
        {
            get
            {
                return this.direction * this.speed;
            }
        }
        /// <summary>
        /// The status effect the projectile inflicts upon contact.
        /// </summary>
        public SSCStatusEffects.StatusEffect effect;

        public bool pierces;

        /// <summary>
        /// Constructor.
        /// </summary>
        public SSCProjectile()
        {

        }
        public SSCProjectile(object Owner,AnimatedSprite Sprite,Rectangle HitBox ,Vector2 Position,Vector2 Direction, float Speed, int LifeSpan ,float Scale,int damage,SSCStatusEffects.StatusEffect Effect=null)
        {
            this.sprite = Sprite;
            this.hitBox = HitBox;
            this.direction = Direction;
            this.speed = Speed;
            this.position = Position;
            this.scale = Scale;
            this.maxLifeSpan = LifeSpan;
            this.currentLifeSpan = LifeSpan;
            this.owner = Owner;
            this.damage = damage;
            this.effect = Effect;
        }

        /// <summary>
        /// Update the projectile.
        /// </summary>
        /// <param name="time"></param>
        public virtual void update(GameTime time)
        {
            this.tickLifeSpan();
            this.updateMovement();
        }

        /// <summary>
        /// Update the movement for the projectile.
        /// </summary>
        public virtual void updateMovement()
        {
            this.position += this.Velocity;
            this.hitBox.X = (int)this.position.X;
            this.hitBox.Y = (int)this.position.Y;
        }

        /// <summary>
        /// Tick the lifespan of the projectile.
        /// </summary>
        public virtual void tickLifeSpan()
        {
            if (this.currentLifeSpan <= 0)
            {
                //ModCore.log("Lifespan is over!");
                this.die();
            }
            else
            {
                this.currentLifeSpan--;
            }
        }
        /// <summary>
        /// What happens when this projectile dies.
        /// </summary>
        public virtual void die()
        {
            //Make projectile manager that handles deleting this projectile.
            //Make projectile manager have a pool of projectiles????
            SeasideScramble.self.entities.projectiles.deleteProjectile(this);
        }

        /// <summary>
        /// Draw the projectile.
        /// </summary>
        /// <param name="b"></param>
        public virtual void draw(SpriteBatch b)
        {
            this.sprite.draw(b, SeasideScramble.GlobalToLocal(SeasideScramble.self.camera.viewport, this.position), this.scale, 0.5f);
        }

        /// <summary>
        /// Checks if the projectile collides with something.
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public bool collidesWith(Vector2 position)
        {
            return this.hitBox.Contains(new Point((int)position.X,(int)position.Y));
        }
        public bool collidesWith(Rectangle rec)
        {
            return this.hitBox.Intersects(rec);
        }

        /// <summary>
        /// What happens to this projectile when it collides with something.
        /// </summary>
        public virtual void collisionLogic()
        {
            //Do something I guess like play an animation.
            if (this.pierces == false)
            {
                this.die();
            }
        }

        /// <summary>
        /// What happens when the projectile collides with something.
        /// </summary>
        /// <param name="other"></param>
        public virtual void onCollision(object other)
        {
            if(other is SSCPlayer)
            {
                if (this.hasOwner())
                {
                    //ModCore.log("Has an owner!");
                    if (this.owner == other)
                    {
                        //ModCore.log("Can't get hit by own projectile.");
                        return;
                    }

                    //if projectile.owner is player and friendly fire is off do nothing.
                    else if (SeasideScramble.self.friendlyFireEnabled == false && this.owner!=other)
                    {
                        return;
                    }
                }
            }
            //ModCore.log("COllision!!!!");
            this.collisionLogic();
        }

        /// <summary>
        /// Checks if this projectile has an owner in the weird case I spawn some without owners.
        /// </summary>
        /// <returns></returns>
        public bool hasOwner()
        {
            return this.owner != null;
        }

        /// <summary>
        /// Spawns a clone at the given position with the given direction.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="direction"></param>
        public virtual void spawnClone(Vector2 position,Vector2 direction)
        {
            //AnimatedSprite newSprite = new AnimatedSprite(this.sprite.name, position, new AnimationManager(this.sprite.animation.objectTexture.Copy(), this.sprite.animation.defaultDrawFrame), this.color);
            SSCProjectile basic = new SSCProjectile(this.owner, new AnimatedSprite("DefaultProjectile", position, new AnimationManager(SeasideScramble.self.textureUtils.getExtendedTexture("Projectiles", "Basic"), new Animation(0, 0, 4, 4)), this.color), new Rectangle(this.hitBox.X,this.hitBox.Y,this.hitBox.Width,this.hitBox.Height), position, direction, this.speed, this.maxLifeSpan, this.scale,this.damage,this.effect);
            SeasideScramble.self.entities.projectiles.addProjectile(basic);
        }
    }
}
