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
using Revitalize.Framework.Minigame.SeasideScrambleMinigame.Interfaces;
using StardustCore.Animations;

namespace Revitalize.Framework.Minigame.SeasideScrambleMinigame.SSCEnemies
{
    public class SSCEnemy : ISSCLivingEntity
    {
        public float MovementSpeed { get; set; }
        public int CurrentHealth { get; set; }
        public int MaxHealth { get; set; }
        public Rectangle HitBox { get; set; }

        public AnimatedSprite sprite;
        public bool shouldDie;
        public float scale;

        public SSCStatusEffects.StatusEffectManager statusEffects;

        public Vector2 Position
        {
            get
            {
                return this.sprite.position;
            }
            set
            {
                this.sprite.position = value;
                Rectangle hit = this.HitBox;
                hit.X = (int)this.Position.X;
                hit.Y = (int)this.Position.Y;
                this.HitBox = hit;
            }
        }

        public SSCEnemy()
        {

        }

        public SSCEnemy(AnimatedSprite Sprite,int MoveSpeed, int MaxHealth,Vector2 hitBoxDimensions,float Scale)
        {
            this.sprite = Sprite;
            this.MovementSpeed = MoveSpeed;
            this.MaxHealth = MaxHealth;
            this.HitBox = new Rectangle((int)this.sprite.position.X, (int)this.sprite.position.Y, (int)(hitBoxDimensions.X * Scale),(int)(hitBoxDimensions.Y * Scale));
            this.CurrentHealth = MaxHealth;
            this.scale = Scale;
            this.statusEffects = new SSCStatusEffects.StatusEffectManager(this);
        }

        public virtual void update(GameTime time)
        {

        }
        public virtual void draw(SpriteBatch b)
        {
            this.sprite.draw(b, SeasideScramble.GlobalToLocal(SeasideScramble.self.camera.viewport, this.Position), this.scale,0f);
        }

        public virtual void draw(SpriteBatch b, Vector2 Position, float Scale)
        {
            this.sprite.draw(b,Position, Scale, 0f);
        }

        public virtual void die()
        {

        }

        public virtual void onCollision(SSCProjectiles.SSCProjectile other)
        {

        }

        public virtual void updateMovement()
        {

        }

    }
}
