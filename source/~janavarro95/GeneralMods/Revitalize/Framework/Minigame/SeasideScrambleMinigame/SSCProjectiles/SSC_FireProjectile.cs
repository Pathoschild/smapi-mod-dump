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
using StardustCore.Animations;

namespace Revitalize.Framework.Minigame.SeasideScrambleMinigame.SSCProjectiles
{
    public class SSC_FireProjectile: SSCProjectile
    {
        public SSC_FireProjectile()
        {

        }
        public SSC_FireProjectile(object Owner, AnimatedSprite Sprite, Rectangle HitBox, Vector2 Position, Vector2 Direction, float Speed, int LifeSpan, float Scale, int damage, SSCStatusEffects.StatusEffect Effect = null):base(Owner,Sprite,HitBox,Position,Direction,Speed,LifeSpan,Scale,damage,Effect)
        {
        }


        public override void spawnClone(Vector2 position, Vector2 direction)
        {
            //AnimatedSprite newSprite = new AnimatedSprite(this.sprite.name, position, new AnimationManager(this.sprite.animation.objectTexture.Copy(), this.sprite.animation.defaultDrawFrame), this.color);
            SSC_FireProjectile basic = new SSC_FireProjectile(this.owner, new AnimatedSprite("FireProjectile", position, new AnimationManager(SeasideScramble.self.textureUtils.getExtendedTexture("Projectiles", "Fire"), new Animation(0, 0, 18, 16)), this.color), new Rectangle((int)this.position.X, (int)this.position.Y, this.hitBox.Width, this.hitBox.Height), position, direction, this.speed, this.maxLifeSpan, this.scale, this.damage,this.effect);
            SeasideScramble.self.entities.projectiles.addProjectile(basic);
        }
    }
}
