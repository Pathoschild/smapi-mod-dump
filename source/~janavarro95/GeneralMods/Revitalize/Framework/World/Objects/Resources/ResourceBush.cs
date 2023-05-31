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
    [XmlType("Mods_Omegasis.Revitalize.Framework.World.Objects.Resources.OreResourceBush")]
    public class ResourceBush:CustomObject
    {


        public NetInt daysItTakesToGrow = new NetInt();
        public NetRef<Item> itemToGrow = new NetRef<Item>();

        public NetInt daysRemainingUntilGrowth = new NetInt();

        public NetRef<Drawable> itemToDraw = new NetRef<Drawable>();
        public NetRef<Drawable> itemToDraw2 = new NetRef<Drawable>();
        public NetRef<Drawable> itemToDraw3 = new NetRef<Drawable>();


        public ResourceBush()
        {

        }

        public ResourceBush(BasicItemInformation Info, Item ItemToGrow, int DaysToGrow) : this(Info,Vector2.Zero,ItemToGrow,DaysToGrow)
        {

        }

        public ResourceBush(BasicItemInformation Info, Vector2 TilePosition, Item ItemToGrow, int DaysToGrow) : base(Info, TilePosition)
        {
            this.itemToGrow.Value = ItemToGrow;
            this.daysItTakesToGrow.Value = DaysToGrow;
            this.daysRemainingUntilGrowth.Value = DaysToGrow;
        }

        protected override void initializeNetFieldsPostConstructor()
        {
            base.initializeNetFieldsPostConstructor();
            this.NetFields.AddFields(this.daysItTakesToGrow, this.itemToGrow, this.daysRemainingUntilGrowth, this.itemToDraw, this.itemToDraw2, this.itemToDraw3);
        }

        public override void doActualDayUpdateLogic(GameLocation location)
        {
            base.doActualDayUpdateLogic(location);

            if (!this.allResourceSlotsFull())
            {
                this.daysRemainingUntilGrowth.Value--;
            }
            if (this.daysRemainingUntilGrowth.Value == 0)
            {
                this.daysRemainingUntilGrowth.Value = this.daysItTakesToGrow.Value;
                if (this.itemToDraw.Value == null)
                {
                    this.itemToDraw.Value = new Drawable(this.itemToGrow.Value.getOne());
                    this.heldObject.Value = (StardewValley.Object) this.itemToGrow.Value.getOne();
                    return;
                }
                if (this.itemToDraw2.Value == null)
                {
                    this.itemToDraw2.Value = new Drawable(this.itemToGrow.Value.getOne());
                    this.heldObject.Value.Stack++;
                    return;
                }
                if (this.itemToDraw3.Value == null)
                {
                    this.itemToDraw3.Value = new Drawable(this.itemToGrow.Value.getOne());
                    this.heldObject.Value.Stack++;
                    return;
                }
            }

        }

        /// <summary>
        /// Since <see cref="Furniture"/> and <see cref="StardewValley.Object"/> each do a seperate day update tick, we need to actually not do a day update for one of them.
        /// </summary>
        /// <returns></returns>
        public override bool shouldDoDayUpdate()
        {
            return this.dayUpdateCounter.Value >= 2;
        }

        /// <summary>
        /// Returns true if no more resources can be grown on this bush.
        /// </summary>
        /// <returns></returns>
        public virtual bool allResourceSlotsFull()
        {
            return this.itemToDraw.Value != null && this.itemToDraw2.Value != null && this.itemToDraw3.Value != null;
        }

        public override bool checkForAction(Farmer who, bool justCheckingForActivity)
        {

            if (this.isReadyForHarvest())
                if (who.IsLocalPlayer)
                    this.harvest(true);

            return base.checkForAction(who,justCheckingForActivity);
        }


        public virtual void harvest(bool AddToPlayersInventory)
        {

            Item item = this.heldObject.Value;
            if (this.heldObject.Value == null) return;

            if (AddToPlayersInventory)
            {
                SoundUtilities.PlaySound(Enums.StardewSound.coin);
                bool added = Game1.player.addItemToInventoryBool(this.heldObject.Value);
                this.heldObject.Value = null;
                if (added == false)
                {
                    WorldUtility.CreateItemDebrisAtTileLocation(this.getCurrentLocation(), item, this.TileLocation);
                    return;
                }
            }
            else
            {
                WorldUtility.CreateItemDebrisAtTileLocation(this.getCurrentLocation(), item, this.TileLocation);
            }


        }

        public override void clearHeldObject()
        {
            base.clearHeldObject();
            this.itemToDraw.Value = null;
            this.itemToDraw2.Value = null;
            this.itemToDraw3.Value = null;
        }

        public override Item getOne()
        {
            return new ResourceBush(this.basicItemInformation.Copy(), this.itemToGrow.Value.getOne(), this.daysItTakesToGrow.Value);
        }

        public override void draw(SpriteBatch spriteBatch, int x, int y, float alpha = 1)
        {
            base.draw(spriteBatch, x, y, alpha,false);

            Vector2 drawPosition1 = new Vector2(x  - this.basicItemInformation.drawOffset.X, y + .25f + this.basicItemInformation.drawOffset.Y);
            Vector2 drawPosition2 = new Vector2(x - .25f - this.basicItemInformation.drawOffset.X, y + .75f + this.basicItemInformation.drawOffset.Y);
            Vector2 drawPosition3 = new Vector2(x + .25f - this.basicItemInformation.drawOffset.X, y + .75f + this.basicItemInformation.drawOffset.Y);

            if (this.itemToDraw.Value != null)
            {
                //For some reason some of the drawing logic is a bit off for displays, so we want to skip drawing items in the incorrect positions.
                if (drawPosition1.X<0 || drawPosition1.Y<0)
                {
                    return;
                }
                this.itemToDraw.Value.drawAsHeldObject(spriteBatch,drawPosition1 , alpha, -this.basicItemInformation.drawOffset.Y);
            }
            if (this.itemToDraw2.Value != null)
            {
                if (drawPosition2.X < 0 || drawPosition2.Y < 0)
                {
                    return;
                }
                //Change the depth value a bit so that it doesn't draw on top of the player as well.
                this.itemToDraw2.Value.drawAsHeldObject(spriteBatch,drawPosition2 , alpha, -this.basicItemInformation.drawOffset.Y-.75f);
            }
            if (this.itemToDraw3.Value != null)
            {
                if (drawPosition3.X < 0 || drawPosition3.Y < 0)
                {
                    return;
                }
                this.itemToDraw3.Value.drawAsHeldObject(spriteBatch,drawPosition3 , alpha, -this.basicItemInformation.drawOffset.Y-.75f);
            }
            
        }

        public override void performRemoveAction(Vector2 tileLocation, GameLocation environment)
        {
            this.harvest(true);
            base.performRemoveAction(tileLocation, environment);
        }

        public virtual bool isReadyForHarvest()
        {
            return this.itemToDraw.Value!= null || this.itemToDraw2.Value != null || this.itemToDraw3.Value != null;
        }

    }
}
