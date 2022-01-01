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
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Revitalize.Framework.Objects.InformationFiles;
using Revitalize.Framework.Utilities;
using Revitalize.Framework.World.Objects.InformationFiles;
using StardewValley;

using CustomObject = Revitalize.Framework.World.Objects.CustomObject;

namespace Revitalize.Framework.Objects.Resources.OreVeins
{
    public class OreVein:CustomObject
    {
        /// <summary>
        /// Deals with information tied to the resource itself.
        /// </summary>
        public OreResourceInformation resourceInfo;
        public List<ResourceInformation> extraDrops;


        private int _healthValue;
        public int healthValue
        {
            get
            {
                return this._healthValue;
            }
            set
            {
                this._healthValue = value;
            }
        }


        public OreVein() : base()
        {

        }

        public OreVein(BasicItemInformation Info, OreResourceInformation Resource,List<ResourceInformation> ExtraDrops,int Health) : base(Info)
        {
            this.healthValue = Health;
            this.resourceInfo = Resource;
            this.extraDrops = ExtraDrops != null ? ExtraDrops : new List<ResourceInformation>();
            this.setHealth(this.healthValue);
            this.Price = Info.price;
        }

        public OreVein(BasicItemInformation Info, Vector2 TileLocation, OreResourceInformation Resource, List<ResourceInformation> ExtraDrops,int Health) : base(Info,TileLocation)
        {

            this.healthValue = Health;
            this.resourceInfo = Resource;
            this.extraDrops = ExtraDrops != null ? ExtraDrops : new List<ResourceInformation>();
            this.setHealth(this.healthValue);
            this.Price = Info.price;
        }


        public override bool performObjectDropInAction(Item dropInItem, bool probe, Farmer who)
        {
            return false; //this.pickUpItem()==PickUpState.DoNothing;
        }

        public override void actionOnPlayerEntry()
        {
            base.actionOnPlayerEntry();
            this.setHealth(this.healthValue);
        }

        public override void updateWhenCurrentLocation(GameTime time, GameLocation environment)
        {
            base.updateWhenCurrentLocation(time, environment);
            this.setHealth(this.healthValue);
            this.basicItemInfo.shakeTimer -= time.ElapsedGameTime.Milliseconds;
        }

        public override void DayUpdate(GameLocation location)
        {
            base.DayUpdate(location);
            this.setHealth(this.healthValue);
        }

        /// <summary>
        /// What happens when the player hits this with a tool.
        /// </summary>
        /// <param name="t"></param>
        /// <param name="location"></param>
        /// <returns></returns>
        public override bool performToolAction(Tool t, GameLocation location)
        {

            if(t is StardewValley.Tools.Pickaxe)
            {
                this.damage((t as StardewValley.Tools.Pickaxe).UpgradeLevel+1);
                if (this.getCurrentLocation() != null)
                {
                    this.getCurrentLocation().playSound("hammer");
                    //ModCore.log("Ore has this much health left and location is not null: "+this.healthValue);
                    this.basicItemInfo.shakeTimer = 200;
                }
                else
                {
                    Game1.player.currentLocation.playSound("hammer");
                    //ModCore.log("Ore has this much health left and location is null!: "+this.healthValue);
                    this.basicItemInfo.shakeTimer = 200;
                }
                return false;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// What happens when an explosion occurs for this object.
        /// </summary>
        /// <param name="who"></param>
        /// <param name="location"></param>
        /// <returns></returns>
        public override bool onExplosion(Farmer who, GameLocation location)
        {
            this.destoryVein();
            return true;
            //return base.onExplosion(who, location);
        }

        /// <summary>
        /// Applies damage to the ore vein.
        /// </summary>
        /// <param name="amount"></param>
        public void damage(int amount)
        {
            if (amount <= 0) return;
            else
            {
                this.healthValue -= amount;
                if (this.healthValue <= 0)
                {
                    this.destoryVein();
                }
            }
        }

        /// <summary>
        /// Destroys this tile for the ore vein.
        /// </summary>
        public void destoryVein()
        {
            int amount = this.resourceInfo.getNumberOfDropsToSpawn();
            Item newItem = this.resourceInfo.droppedItem.getOne();
            GameLocation loc = this.getCurrentLocation();
            for(int i = 0; i < amount; i++)
            {
                Game1.createItemDebris(newItem.getOne(), this.TileLocation*Game1.tileSize, Game1.random.Next(0, 3), loc);
            }

            if (this.extraDrops != null)
            {
                foreach (ResourceInformation extra in this.extraDrops)
                {
                    if (extra.shouldDropResource())
                    {
                        Item extraItem = extra.droppedItem.getOne();
                        int extraAmount = extra.getNumberOfDropsToSpawn();
                        for (int i = 0; i < amount; i++)
                        {
                            Game1.createItemDebris(extraItem.getOne(), this.TileLocation * Game1.tileSize, Game1.random.Next(0, 3), loc);
                        }
                    }
                    else
                    {
                        //Resource did not meet spawn chance.
                    }
                }
            }

            if (loc != null)
            {
                this.getCurrentLocation().playSound("stoneCrack");
                Game1.createRadialDebris(this.getCurrentLocation(), 14, (int)this.TileLocation.X, (int)this.TileLocation.Y, Game1.random.Next(4, 10), false, -1, false, -1);
                loc.removeObject(this.TileLocation, false);
            }
            else
            {
                Game1.player.currentLocation.playSound("stoneCrack");
                Game1.createRadialDebris(Game1.player.currentLocation, 14, (int)this.TileLocation.X, (int)this.TileLocation.Y, Game1.random.Next(4, 10), false, -1, false, -1);
                Game1.player.currentLocation.removeObject(this.TileLocation, false);
            }
        }

        /// <summary>
        /// Gets called when there is no actively held item on the tile.
        /// </summary>
        /// <param name="who"></param>
        /// <returns></returns>
        public override bool clicked(Farmer who)
        {
            return false;
        }

        public override bool rightClicked(Farmer who)
        {
            return false;
        }


        public override bool shiftRightClicked(Farmer who)
        {
            return base.shiftRightClicked(who);
        }

        public override Item getOne()
        {
            OreVein component = new OreVein(this.basicItemInfo.Copy(),this.resourceInfo,this.extraDrops,this.healthValue);
            return component;
        }


        /// <summary>What happens when the object is drawn at a tile location.</summary>
        public override void draw(SpriteBatch spriteBatch, int x, int y, float alpha = 1f)
        {

            //The actual planter box being drawn.
            if (this.basicItemInfo.animationManager == null)
            {
                if (this.basicItemInfo.animationManager.getExtendedTexture() == null)

                spriteBatch.Draw(this.basicItemInfo.animationManager.getTexture(), Game1.GlobalToLocal(Game1.viewport, new Vector2((float)(x * Game1.tileSize)+this.basicItemInfo.shakeTimerOffset(), (y * Game1.tileSize)+this.basicItemInfo.shakeTimerOffset())), new Rectangle?(this.AnimationManager.getCurrentAnimation().getCurrentAnimationFrameRectangle()), this.basicItemInfo.DrawColor * alpha, 0f, Vector2.Zero, (float)Game1.pixelZoom, this.flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None, Math.Max(0f, (float)(y * Game1.tileSize) / 10000f));
            }

            else
            {
                float addedDepth = 0;

                this.basicItemInfo.animationManager.draw(spriteBatch, this.basicItemInfo.animationManager.getTexture(), Game1.GlobalToLocal(Game1.viewport, new Vector2((float)(x * Game1.tileSize)+this.basicItemInfo.shakeTimerOffset(), (y * Game1.tileSize)+this.basicItemInfo.shakeTimerOffset())), new Rectangle?(this.AnimationManager.getCurrentAnimation().getCurrentAnimationFrameRectangle()), this.basicItemInfo.DrawColor * alpha, 0f, Vector2.Zero, (float)Game1.pixelZoom, this.flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None, Math.Max(0f, (float)((y + addedDepth) * Game1.tileSize) / 10000f) + .00001f);

                try
                {
                    this.basicItemInfo.animationManager.tickAnimation();
                }
                catch (Exception err)
                {
                    ModCore.ModMonitor.Log(err.ToString());
                }
                if (this.heldObject.Value != null) SpriteBatchUtilities.Draw(spriteBatch, this, this.heldObject.Value, alpha, addedDepth);
            }
        }


    }
}
