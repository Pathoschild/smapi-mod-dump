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
    public class SSCProjectileManager
    {
        public List<SSCProjectile> projectiles;
        private List<SSCProjectile> garbageCollection;

        public SSCProjectileManager()
        {
            this.projectiles = new List<SSCProjectile>();
            this.garbageCollection = new List<SSCProjectile>();
        }

        public void addProjectile(SSCProjectile projectile)
        {
            this.projectiles.Add(projectile);
        }

        public void deleteProjectile(SSCProjectile projectile)
        {
            this.garbageCollection.Add(projectile);
            //this.projectiles.Remove(projectile);
        }

        public void update(GameTime Time)
        {
            foreach(SSCProjectile p in this.garbageCollection)
            {
                this.projectiles.Remove(p);
            }
            this.garbageCollection.Clear();

            foreach(SSCProjectile p in this.projectiles)
            {
                p.update(Time);

                //Do collision checking.
                foreach(SSCPlayer player in SeasideScramble.self.players.Values)
                {
                    if (p.collidesWith(player.hitBox))
                    {
                        p.onCollision(player);
                        player.onCollision(p);
                    }
                }
                foreach(SSCEnemies.SSCEnemy enemy in SeasideScramble.self.entities.enemies.enemies)
                {
                    if (p.collidesWith(enemy.HitBox))
                    {
                        p.onCollision(enemy); //What happens to the projectile.
                        enemy.onCollision(p); //What happens to the entity.
                    }
                }

                //Quietly clean up stray projectiles just incase their timer hasn't ticked out yet.
                Vector2 mapSize = SeasideScramble.self.currentMap.getPixelSize();
                if (p.position.X > mapSize.X * 2 || p.position.X < -mapSize.X || p.position.Y > mapSize.Y * 2 || p.position.Y < -mapSize.Y)
                {
                    //ModCore.log("Clean up projectile. Position is: " + p.position);
                    this.deleteProjectile(p);
                }
            }
        }

        public void draw(SpriteBatch b)
        {
            foreach (SSCProjectile p in this.projectiles)
            {             
                p.draw(b);
            }
        }

        //~~~~~~~~~~~~~~~~~~~~//
        //   Spawning Logic   //
        //~~~~~~~~~~~~~~~~~~~~//
        #region

        public void spawnDefaultProjectile(object Owner,Vector2 Position,Vector2 Direction,float Speed,Rectangle HitBox,Color Color,float Scale,int LifeSpan=300)
        {

            SSCProjectile basic = new SSCProjectile(Owner, new StardustCore.Animations.AnimatedSprite("DefaultProjectile", Position, new StardustCore.Animations.AnimationManager(SeasideScramble.self.textureUtils.getExtendedTexture("Projectiles", "Basic"), new StardustCore.Animations.Animation(0, 0, 4, 4)), Color), HitBox, Position, Direction, Speed, LifeSpan, Scale,1);
            this.addProjectile(basic);
        }
        public SSCProjectile getDefaultProjectile(object Owner, Vector2 Position, Vector2 Direction, float Speed, Rectangle HitBox, Color Color, float Scale, int LifeSpan = 300)
        {

            SSCProjectile basic = new SSCProjectile(Owner, new StardustCore.Animations.AnimatedSprite("DefaultProjectile", Position, new StardustCore.Animations.AnimationManager(SeasideScramble.self.textureUtils.getExtendedTexture("Projectiles", "Basic"), new StardustCore.Animations.Animation(0, 0, 4, 4)), Color), HitBox, Position, Direction, Speed, LifeSpan, Scale, 1);
            return basic;
        }

        public void spawnIcicleProjectile(object Owner, Vector2 Position, Vector2 Direction, float Speed, Rectangle HitBox, Color Color, float Scale,int Damage ,int LifeSpan = 300)
        {

            SSC_IcicleProjectile basic = new SSC_IcicleProjectile(Owner, new StardustCore.Animations.AnimatedSprite("Icicle", Position, new StardustCore.Animations.AnimationManager(SeasideScramble.self.textureUtils.getExtendedTexture("Projectiles", "Icicle"), new StardustCore.Animations.Animation(0, 0, 16, 16)), Color), HitBox, Position, Direction, Speed, LifeSpan, Scale, Damage);
            this.addProjectile(basic);
        }
        public SSC_IcicleProjectile getIcicleProjectile(object Owner, Vector2 Position, Vector2 Direction, float Speed, Rectangle HitBox, Color Color, float Scale,int Damage ,int LifeSpan = 300)
        {

            SSC_IcicleProjectile basic = new SSC_IcicleProjectile(Owner, new StardustCore.Animations.AnimatedSprite("Icicle", Position, new StardustCore.Animations.AnimationManager(SeasideScramble.self.textureUtils.getExtendedTexture("Projectiles", "Icicle"), new StardustCore.Animations.Animation(0, 0, 16, 16)), Color), HitBox, Position, Direction, Speed, LifeSpan, Scale, Damage);
            return basic;
        }

        public void spawnFireProjectile(object Owner, Vector2 Position, Vector2 Direction, float Speed, Rectangle HitBox, Color Color, float Scale,int Damage,SSCStatusEffects.SE_Burn BurnEffect ,int LifeSpan = 300)
        {

            SSC_FireProjectile basic = new SSC_FireProjectile(Owner, new AnimatedSprite("FireProjectile", Position, new AnimationManager(SeasideScramble.self.textureUtils.getExtendedTexture("Projectiles", "Fire"), new Animation(0, 0, 4, 4)), Color), new Rectangle(HitBox.X, HitBox.Y, HitBox.Width, HitBox.Height), Position, Direction, Speed, LifeSpan, Scale, Damage, BurnEffect);
            this.addProjectile(basic);
        }
        public SSCProjectile getFireProjectile(object Owner, Vector2 Position, Vector2 Direction, float Speed, Rectangle HitBox, Color Color, float Scale,int Damage, SSCStatusEffects.SE_Burn BurnEffect, int LifeSpan = 150)
        {

            SSC_FireProjectile basic = new SSC_FireProjectile(Owner, new AnimatedSprite("FireProjectile", Position, new AnimationManager(SeasideScramble.self.textureUtils.getExtendedTexture("Projectiles", "Fire"), new Animation(0, 0, 4, 4)), Color), new Rectangle(HitBox.X, HitBox.Y, HitBox.Width, HitBox.Height), Position, Direction, Speed, LifeSpan, Scale, Damage,BurnEffect);
            return basic;
        }

        #endregion
    }
}
