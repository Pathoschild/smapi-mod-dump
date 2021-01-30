/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/GenDeathrow/SDV_BlessingsAndCurses
**
*************************************************/

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley;
using StardewValley.Monsters;
using StardewValley.Projectiles;
using StardewValley.TerrainFeatures;
using static StardewValley.Projectiles.BasicProjectile;

namespace BNC
{
    public class Meteor : Projectile
    {
        public delegate void onCollisionBehavior(GameLocation location, int xPosition, int yPosition, Character who);

        public readonly NetInt damageToFarmer = new NetInt();

        private readonly NetString collisionSound = new NetString();

        private readonly NetBool explode = new NetBool();

        private onCollisionBehavior collisionBehavior;

        public NetInt debuff = new NetInt(-1);

        public NetInt StartHeight = new NetInt(24);

        public NetString debuffSound = new NetString("debuffHit");

        public String who = null;

        public Meteor(Vector2 startingPosition, int height, GameLocation location = null, String who = null) : this()
        {
            this.position.Value = startingPosition;
            //BNC_Core.Logger.Log($"HS: {this.height.Value} | In: {height}", StardewModdingAPI.LogLevel.Debug);
            this.StartHeight.Value = height;
            this.height.Value = height;
            // BNC_Core.Logger.Log($"ES: {this.height.Value}", StardewModdingAPI.LogLevel.Debug);
            //this.xVelocity.Value = 8;
            //this.YVelocity.Value = 8;
            this.xVelocity.Value = Game1.random.Next(7, 10);
            this.yVelocity.Value = Game1.random.Next(7, 10); 
            this.maxTravelDistance.Value = 10000;
            this.hasLit = true;
            this.rotation = -0.45f;
            this.who = who;
            this.light.Value = true;


        }

        public Meteor()
        {
            base.NetFields.AddFields(this.damageToFarmer, this.collisionSound, this.explode, this.debuff, this.debuffSound);
        }

        public override void behaviorOnCollisionWithMineWall(int tileX, int tileY)
        {
            
        }

        public override void behaviorOnCollisionWithMonster(NPC n, GameLocation location)
        {
            
        }

        public override void behaviorOnCollisionWithOther(GameLocation location)
        {
           
        }

        public override void behaviorOnCollisionWithPlayer(GameLocation location, Farmer player)
        {
           
        }

        public override void behaviorOnCollisionWithTerrainFeature(TerrainFeature t, Vector2 tileLocation, GameLocation location)
        {
           
        }
        public override bool isColliding(GameLocation location)
        {
            if (this.StartHeight.Value <= 0)
            {
                explodeOnImpact(location, (int)this.position.X, (int)this.position.Y, Game1.player);

                Vector2 tileLocation = new Vector2((int)(this.position.X / 64F), (int)(this.position.Y / 64F));

                if (location is Farm && location.isTileOnMap((int)tileLocation.X, (int)tileLocation.Y) && location.isTileLocationTotallyClearAndPlaceable(tileLocation))
                {
                    /*
                    if (Game1.random.NextDouble() < 0.2)
                    {
                        Spawner.addMonsterToSpawn(new RockCrab(Vector2.Zero), this.who != null ? this.who : "");
                    }
                    else
                    {*/
                        BNC_Core.Logger.Log($"Adding Stone X:{tileLocation.X} Y: {tileLocation.Y}", StardewModdingAPI.LogLevel.Debug);
                        StardewValley.Object stone = new StardewValley.Object(tileLocation, 450, 1);
                        location.Objects.Add(tileLocation, stone);
                    //}
                    
                }
                return true;
            }

            return false;//base.isColliding(location);
        }

        public void SetPosition(Vector2 vector2)
        {
            this.position.Value = vector2;
        }

        public override void draw(SpriteBatch b)
        {
            float current_scale = 4f * this.localScale * MathHelper.Clamp(((float)this.StartHeight.Value / 75f), 1f, 5f);
            //BNC_Core.Logger.Log($"scale: {current_scale} : {MathHelper.Clamp(((float)this.StartHeight.Value / 10f), 1f, 20f)}", StardewModdingAPI.LogLevel.Debug);
            float alpha = 1;
            b.Draw(BNC_Core.meteorTileSheet, Game1.GlobalToLocal(Game1.viewport, this.position + new Vector2(0f, 0f - (float)this.height) + new Vector2(32f, 32f)), Game1.getSourceRectForStandardTileSheet(BNC_Core.meteorTileSheet, this.currentTileSheetIndex.Value, 32, 32), this.color.Value * 1, this.rotation, new Vector2(8f, 8f), current_scale, SpriteEffects.None, (this.position.Y + 96f) / 10000f);

            if (this.StartHeight.Value > 0f)
            {
                b.Draw(Game1.shadowTexture, Game1.GlobalToLocal(Game1.viewport, this.position + new Vector2(96f, MathHelper.Clamp(((float)this.StartHeight.Value / 10f), 1f, 30f) * 96f )), Game1.shadowTexture.Bounds, Color.White * alpha * 0.75f, 0f, new Vector2(Game1.shadowTexture.Bounds.Center.X, Game1.shadowTexture.Bounds.Center.Y), current_scale, SpriteEffects.None, (this.position.Y - 3f) / 10000f);
            }
        }


        public override bool update(GameTime time, GameLocation location)
        {
            bool test = base.update(time, location);
            this.updateAnimation(time);

            //BNC_Core.Logger.Log(this.travelDistance +" - "+ this.maxTravelDistance.Value + " : "+ test, StardewModdingAPI.LogLevel.Debug);
            return test;
        }

        protected  int lastAnimationUpdate = 100;
        public void updateAnimation(GameTime time)
        {
            this.lastAnimationUpdate -= time.ElapsedGameTime.Milliseconds;
            if (this.lastAnimationUpdate <= 0)
            {
                this.currentTileSheetIndex.Value += 1;
                this.lastAnimationUpdate = 100;
            }
            if (this.currentTileSheetIndex.Value > 2)
                this.currentTileSheetIndex.Value = 0;
        }

        public override void updatePosition(GameTime time)
        {
            //BNC_Core.Logger.Log($"h:{this.StartHeight.Value}", StardewModdingAPI.LogLevel.Debug);
            this.position.X += base.xVelocity;
            this.position.Y += base.yVelocity;
            this.StartHeight.Value -= 2;
        }


        public static void explodeOnImpact(GameLocation location, int x, int y, Character who)
        {
            location.explode(new Vector2(x / 64, y / 64), 2, null, true, Game1.random.Next(10,25));
            location.playSound("explosion");
        }
    }
}
