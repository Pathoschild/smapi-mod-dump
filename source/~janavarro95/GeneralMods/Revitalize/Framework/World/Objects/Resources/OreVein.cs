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
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using Omegasis.Revitalize.Framework.Constants;
using Omegasis.Revitalize.Framework.Utilities;
using Omegasis.Revitalize.Framework.World.Objects.InformationFiles;
using Omegasis.Revitalize.Framework.World.WorldUtilities;
using StardewValley;

namespace Omegasis.Revitalize.Framework.World.Objects.Resources
{
    [XmlType("Mods_Revitalize.Framework.World.Objects.Resources.OreVeins.OreVein")]
    public class OreVein : CustomObject
    {
        /// <summary>
        /// Deals with information tied to the resource itself.
        /// </summary>
        public NetResourceInformation<OreResourceInformation> resourceInfo = new NetResourceInformation<OreResourceInformation>();
        public readonly NetList<ResourceInformation, NetResourceInformation<ResourceInformation>> extraDrops = new NetList<ResourceInformation, NetResourceInformation<ResourceInformation>>();


        public readonly NetInt healthValue = new NetInt();


        public OreVein() : base()
        {

        }

        public OreVein(BasicItemInformation Info, OreResourceInformation Resource, List<ResourceInformation> ExtraDrops, int Health) : base(Info)
        {
            this.basicItemInformation = Info;
            this.healthValue.Value = Health;
            this.resourceInfo.Value = Resource;
            this.extraDrops.AddRange(ExtraDrops != null ? ExtraDrops : new List<ResourceInformation>());
            this.setHealth(this.healthValue);
            this.Price = Info.price;
        }

        public OreVein(BasicItemInformation Info, Vector2 TileLocation, OreResourceInformation Resource, List<ResourceInformation> ExtraDrops, int Health) : base(Info, TileLocation)
        {
            this.basicItemInformation = Info;
            this.healthValue.Value = Health;
            this.resourceInfo.Value = Resource;
            this.extraDrops.AddRange(ExtraDrops != null ? ExtraDrops : new List<ResourceInformation>());
            this.setHealth(this.healthValue);
            this.Price = Info.price;
        }

        protected override void initializeNetFieldsPostConstructor()
        {
            base.initializeNetFieldsPostConstructor();
            this.NetFields.AddFields(this.resourceInfo, this.extraDrops);
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
            this.basicItemInformation.shakeTimer.Value -= time.ElapsedGameTime.Milliseconds;
        }

        public override void doActualDayUpdateLogic(GameLocation location)
        {
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

            if (t is StardewValley.Tools.Pickaxe)
            {
                this.damage((t as StardewValley.Tools.Pickaxe).UpgradeLevel + 1);
                if (this.getCurrentLocation() != null)
                {
                    this.getCurrentLocation().PlaySound(Enums.StardewSound.hammer);
                    //ModCore.log("Ore has this much health left and location is not null: "+this.healthValue);
                    this.basicItemInformation.shakeTimer.Value = 200;
                }
                else
                {
                    this.getCurrentLocation().PlaySound(Enums.StardewSound.hammer);
                    //ModCore.log("Ore has this much health left and location is null!: "+this.healthValue);
                    this.basicItemInformation.shakeTimer.Value = 200;
                }
                return false;
            }
            else
                return false;
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
                this.healthValue.Value -= amount;
                if (this.healthValue <= 0)
                    this.destoryVein();
            }
        }

        /// <summary>
        /// Destroys this tile for the ore vein.
        /// </summary>
        public void destoryVein()
        {
            int amount = this.resourceInfo.Value.getNumberOfDropsToSpawn();
            Item newItem = this.resourceInfo.Value.droppedItem.getItem(1);
            GameLocation loc = this.getCurrentLocation();
            for (int i = 0; i < amount; i++)
                Game1.createItemDebris(newItem.getOne(), this.TileLocation * Game1.tileSize, Game1.random.Next(0, 3), loc);

            if (this.extraDrops != null)
                foreach (ResourceInformation extra in this.extraDrops)
                    if (extra.shouldDropResource())
                    {
                        Item extraItem = extra.droppedItem.getItem(1);
                        int extraAmount = extra.getNumberOfDropsToSpawn();
                        for (int i = 0; i < amount; i++)
                            Game1.createItemDebris(extraItem.getOne(), this.TileLocation * Game1.tileSize, Game1.random.Next(0, 3), loc);
                    }
                    else
                    {
                        //Resource did not meet spawn chance.
                    }

            if (loc != null)
            {
                this.getCurrentLocation().PlaySound(Enums.StardewSound.stoneCrack);
                Game1.createRadialDebris(this.getCurrentLocation(), 14, (int)this.TileLocation.X, (int)this.TileLocation.Y, Game1.random.Next(4, 10), false, -1, false, -1);
                loc.removeObject(this.TileLocation, false);
            }
            else
            {
                this.getCurrentLocation().PlaySound(Enums.StardewSound.stoneCrack);
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

            List<ResourceInformation> resourceInformation = new List<ResourceInformation>();
            foreach (ResourceInformation resources in this.extraDrops)
                resourceInformation.Add(resources);

            OreVein component = new OreVein(this.basicItemInformation.Copy(), this.resourceInfo.Value, resourceInformation, this.healthValue);
            return component;
        }


        /// <summary>What happens when the object is drawn at a tile location.</summary>
        public override void draw(SpriteBatch spriteBatch, int x, int y, float alpha = 1f)
        {

            if (this.AnimationManager == null)
                spriteBatch.Draw(this.basicItemInformation.animationManager.getTexture(), Game1.GlobalToLocal(Game1.viewport, new Vector2((float)(x * Game1.tileSize) + this.basicItemInformation.shakeTimerOffset(), y * Game1.tileSize + this.basicItemInformation.shakeTimerOffset())), new Rectangle?(this.AnimationManager.getCurrentAnimation().getCurrentAnimationFrameRectangle()), this.basicItemInformation.DrawColor * alpha, 0f, Vector2.Zero, Game1.pixelZoom, this.flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None, Math.Max(0f, y * Game1.tileSize / 10000f));
            else
            {
                this.basicItemInformation.animationManager.draw(spriteBatch, this.basicItemInformation.animationManager.getTexture(), Game1.GlobalToLocal(Game1.viewport, new Vector2((float)(x * Game1.tileSize) + this.basicItemInformation.shakeTimerOffset(), y * Game1.tileSize + this.basicItemInformation.shakeTimerOffset())), new Rectangle?(this.AnimationManager.getCurrentAnimation().getCurrentAnimationFrameRectangle()), this.basicItemInformation.DrawColor * alpha, 0f, Vector2.Zero, Game1.pixelZoom, this.flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None, Math.Max(0f, y * Game1.tileSize / 10000f) + .00001f);
                if (this.heldObject.Value != null) SpriteBatchUtilities.Draw(spriteBatch, this, this.heldObject.Value, alpha, 0);
            }
        }


    }
}
