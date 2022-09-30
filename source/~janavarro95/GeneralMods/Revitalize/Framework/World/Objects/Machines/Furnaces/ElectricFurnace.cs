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
using Omegasis.Revitalize.Framework.Constants.ItemIds.Items;
using Omegasis.Revitalize.Framework.Player;
using Omegasis.Revitalize.Framework.Utilities;
using Omegasis.Revitalize.Framework.World.Objects.InformationFiles;
using Omegasis.Revitalize.Framework.World.WorldUtilities;
using StardewValley;

namespace Omegasis.Revitalize.Framework.World.Objects.Machines.Furnaces
{
    [XmlType("Mods_Revitalize.Framework.World.Objects.Machines.Furnaces.ElectricFurnace")]
    public class ElectricFurnace : Machine
    {
        public const string ELECTRIC_WORKING_ANIMATION_KEY = "Electric_Working";
        public const string ELECTRIC_IDLE_ANIMATION_KEY = "Electric_Idle";

        public const string NUCLEAR_WORKING_ANIMATION_KEY = "Nuclear_Working";
        public const string NUCLEAR_IDLE_ANIMATION_KEY = "Nuclear_Idle";

        public const string MAGICAL_WORKING_ANIMATION_KEY = "Magical_Working";
        public const string MAGICAL_IDLE_ANIMATION_KEY = "Magical_Idle";

        public enum FurnaceType
        {
            Electric,
            Nuclear,
            Magical
        }

        public readonly NetInt chargesRemaining = new NetInt();
        public readonly NetEnum<Enums.SDVObject> smeltingItem = new NetEnum<Enums.SDVObject>(Enums.SDVObject.NULL);
        public readonly NetEnum<FurnaceType> furnaceType = new NetEnum<FurnaceType>(FurnaceType.Electric);



        public ElectricFurnace()
        {

        }


        public ElectricFurnace(BasicItemInformation info, FurnaceType furnaceType) : base(info)
        {
            this.createStatusBubble();
            this.furnaceType.Value = furnaceType;
        }

        public ElectricFurnace(BasicItemInformation info, Vector2 TileLocation, FurnaceType furnaceType) : base(info, TileLocation)
        {
            this.createStatusBubble();
            this.furnaceType.Value = furnaceType;
        }

        protected override void initializeNetFieldsPostConstructor()
        {
            base.initializeNetFieldsPostConstructor();
            this.NetFields.AddFields(this.chargesRemaining, this.smeltingItem, this.furnaceType);
        }

        public override bool minutesElapsed(int minutes, GameLocation environment)
        {
            base.minutesElapsed(minutes, environment);

            if (this.MinutesUntilReady == 0 && this.smeltingItem.Value != Enums.SDVObject.NULL && this.heldObject.Value == null)
            {
                if (this.smeltingItem.Value == Enums.SDVObject.CopperOre)
                {
                    this.heldObject.Value = new StardewValley.Object((int)Enums.SDVObject.CopperBar, 1);
                }
                if (this.smeltingItem.Value == Enums.SDVObject.IronOre)
                {
                    this.heldObject.Value = new StardewValley.Object((int)Enums.SDVObject.IronBar, 1);
                }
                if (this.smeltingItem.Value == Enums.SDVObject.GoldOre)
                {
                    this.heldObject.Value = new StardewValley.Object((int)Enums.SDVObject.GoldBar, 1);
                }
                if (this.smeltingItem.Value == Enums.SDVObject.IridiumOre)
                {
                    this.heldObject.Value = new StardewValley.Object((int)Enums.SDVObject.IridiumBar, 1);
                }
                if (this.smeltingItem.Value == Enums.SDVObject.RadioactiveOre)
                {
                    this.heldObject.Value = new StardewValley.Object((int)Enums.SDVObject.RadioactiveBar, 1);
                }
                if (this.smeltingItem.Value == Enums.SDVObject.Quartz)
                {
                    this.heldObject.Value = new StardewValley.Object((int)Enums.SDVObject.RefinedQuartz, 1);
                }
                if (this.smeltingItem.Value == Enums.SDVObject.FireQuartz)
                {
                    this.heldObject.Value = new StardewValley.Object((int)Enums.SDVObject.RefinedQuartz, 3);
                }
                if (this.smeltingItem.Value == Enums.SDVObject.Bouquet)
                {
                    this.heldObject.Value = new StardewValley.Object((int)Enums.SDVObject.WiltedBouquet, 1);
                }
                this.consumeFuelCharge();
                this.updateAnimation();
            }

            return true;
        }

        public override bool rightClicked(Farmer who)
        {

            if (this.heldObject.Value != null)
                if (who.IsLocalPlayer)
                    this.cleanOutFurnace(true);

            return base.rightClicked(who);
        }

        /// <summary>
        /// Cleans out the furnace to produce more items.
        /// </summary>
        /// <param name="addToPlayersInventory"></param>
        public virtual void cleanOutFurnace(bool addToPlayersInventory)
        {
            if (addToPlayersInventory)
            {
                SoundUtilities.PlaySound(Enums.StardewSound.coin);
                bool added = Game1.player.addItemToInventoryBool(this.heldObject.Value);
                if (added == false) return;
            }
            this.heldObject.Value = null;
            this.updateAnimation();
            this.smeltingItem.Value = Enums.SDVObject.NULL;

        }

        public override bool performObjectDropInAction(Item dropInItem, bool probe, Farmer who)
        {
            if (probe == true || this.MinutesUntilReady > 0) return false;

            //Cleans out the furnace as necessary to ensure it works properly when dropping in another item.
            if (this.finishedProduction())
            {
                this.cleanOutFurnace(who != null);
            }

            //Smelting times are about 25% faster than normal.
            if (dropInItem.ParentSheetIndex == (int)Enums.SDVObject.CopperOre)
            {
                if (dropInItem.Stack >= 5)
                {
                    return this.smeltItem(who, Enums.SDVObject.CopperOre);
                }
                else
                {
                    Game1.showRedMessage("Requires 5 ore!");
                }
            }

            if (dropInItem.ParentSheetIndex == (int)Enums.SDVObject.IronOre)
            {
                if (dropInItem.Stack >= 5)
                {
                    return this.smeltItem(who, Enums.SDVObject.IronOre);
                }
                else
                {
                    Game1.showRedMessage("Requires 5 ore!");
                }
            }

            if (dropInItem.ParentSheetIndex == (int)Enums.SDVObject.GoldOre)
            {
                if (dropInItem.Stack >= 5)
                {
                    return this.smeltItem(who, Enums.SDVObject.GoldOre);
                }
                else
                {
                    Game1.showRedMessage("Requires 5 ore!");
                }
            }

            if (dropInItem.ParentSheetIndex == (int)Enums.SDVObject.IridiumOre)
            {
                if (dropInItem.Stack >= 5)
                {
                    return this.smeltItem(who, Enums.SDVObject.IridiumOre);
                }
                else
                {
                    Game1.showRedMessage("Requires 5 ore!");
                }
            }

            if (dropInItem.ParentSheetIndex == (int)Enums.SDVObject.RadioactiveOre)
            {
                if (dropInItem.Stack >= 5)
                {
                    return this.smeltItem(who, Enums.SDVObject.RadioactiveOre);
                }
                else
                {
                    Game1.showRedMessage("Requires 5 ore!");
                }
            }

            if (dropInItem.ParentSheetIndex == (int)Enums.SDVObject.Quartz)
            {
                return this.smeltItem(who, Enums.SDVObject.Quartz);

            }


            if (dropInItem.ParentSheetIndex == (int)Enums.SDVObject.FireQuartz)
            {
                return this.smeltItem(who, Enums.SDVObject.FireQuartz);
            }

            if (dropInItem.ParentSheetIndex == (int)Enums.SDVObject.Bouquet)
            {
                return this.smeltItem(who, Enums.SDVObject.Bouquet);
            }
            this.updateAnimation();

            //return base.performObjectDropInAction(dropInItem, probe, who);
            return false;
        }

        /// <summary>
        /// Gets the time it takes for a given object to be smelted in this furnace.
        /// </summary>
        /// <param name="dropInObject"></param>
        /// <returns></returns>
        public virtual int getTimeToSmelt(Enums.SDVObject dropInObject)
        {

            float multiplier = 1f;
            int minutesToCraft = 0;

            if (this.furnaceType.Value == FurnaceType.Electric)
            {
                multiplier = .75f;
            }
            if (this.furnaceType.Value == FurnaceType.Nuclear)
            {
                multiplier = .5f;
            }
            if (this.furnaceType.Value == FurnaceType.Magical)
            {
                multiplier = .25f;
            }

            if (dropInObject == Enums.SDVObject.CopperOre)
            {
                minutesToCraft = TimeUtilities.GetMinutesFromTime(0, 0, 30);
            }
            if (dropInObject == Enums.SDVObject.IronOre)
            {
                minutesToCraft = TimeUtilities.GetMinutesFromTime(0, 2, 0);
            }
            if (dropInObject == Enums.SDVObject.GoldOre)
            {
                minutesToCraft = TimeUtilities.GetMinutesFromTime(0, 5, 0);
            }
            if (dropInObject == Enums.SDVObject.IridiumOre)
            {
                minutesToCraft = TimeUtilities.GetMinutesFromTime(0, 8, 0);
            }
            if (dropInObject == Enums.SDVObject.RadioactiveOre)
            {
                minutesToCraft = TimeUtilities.GetMinutesFromTime(0, 10, 0);
            }

            if (dropInObject == Enums.SDVObject.Quartz)
            {
                minutesToCraft = TimeUtilities.GetMinutesFromTime(0, 1, 30);
            }

            if (dropInObject == Enums.SDVObject.FireQuartz)
            {
                minutesToCraft = TimeUtilities.GetMinutesFromTime(0, 1, 30);
            }

            if (dropInObject == Enums.SDVObject.Bouquet)
            {
                minutesToCraft = TimeUtilities.GetMinutesFromTime(0, 0, 10);
            }


            minutesToCraft = (int)(minutesToCraft * multiplier);

            minutesToCraft -= minutesToCraft % 10; //Round down to nearest 10 minute mark.

            if (minutesToCraft < 10) minutesToCraft = 10; //Make sure there is still a valid time to craft the object.

            return minutesToCraft;

        }

        public virtual int getStackSizeNecessaryForSmelting(Enums.SDVObject obj)
        {
            if(obj== Enums.SDVObject.CopperOre || obj== Enums.SDVObject.IronOre || obj== Enums.SDVObject.GoldOre || obj== Enums.SDVObject.IridiumOre || obj== Enums.SDVObject.RadioactiveOre)
            {
                return 5;
            }
            if(obj== Enums.SDVObject.Quartz || obj== Enums.SDVObject.FireQuartz || obj== Enums.SDVObject.WiltedBouquet)
            {
                return 1;
            }
            return 0;
        }

        public virtual bool smeltItem(Farmer who, Enums.SDVObject sdvObjectToSmelt, bool showRedMessage = true)
        {

            bool success = this.chargesRemaining.Value <= 0 ? this.consumeFuelItemFromFarmersInventory(who) : true;

            if (success == false && showRedMessage)
            {
                this.showRedMessageForMissingFuel();
                return false;
            }

            this.smeltItem(sdvObjectToSmelt);

            if (who != null)
            {
                SoundUtilities.PlaySound(Enums.StardewSound.furnace);
                PlayerUtilities.ReduceInventoryItemIfEnoughFound(who, sdvObjectToSmelt, this.getStackSizeNecessaryForSmelting(sdvObjectToSmelt));
            }

            return false;

        }

        public virtual bool smeltItem(Enums.SDVObject sdvObjectToSmelt)
        {

            if (this.chargesRemaining.Value <= 0)
            {
                this.increaseFuelCharges();
            }

            this.smeltingItem.Value = sdvObjectToSmelt;
            this.MinutesUntilReady = this.getTimeToSmelt(sdvObjectToSmelt);
            this.updateAnimation();

            return false;
        }

        /// <summary>
        /// Updates the animation manager to play the correct animation.
        /// </summary>
        protected virtual void updateAnimation()
        {
            if (this.furnaceType.Value == FurnaceType.Electric)
            {
                if (this.MinutesUntilReady > 0)
                {
                    this.AnimationManager.playAnimation(ELECTRIC_WORKING_ANIMATION_KEY, true);
                    return;
                }
                else
                {
                    this.AnimationManager.playAnimation(ELECTRIC_IDLE_ANIMATION_KEY, true);
                    return;
                }

            }
            if (this.furnaceType.Value == FurnaceType.Nuclear)
            {
                if (this.MinutesUntilReady > 0)
                {
                    this.AnimationManager.playAnimation(NUCLEAR_WORKING_ANIMATION_KEY, true);
                    return;
                }
                else
                {
                    this.AnimationManager.playAnimation(NUCLEAR_IDLE_ANIMATION_KEY, true);
                    return;
                }
            }
            if (this.furnaceType.Value == FurnaceType.Magical)
            {
                if (this.MinutesUntilReady > 0)
                {
                    this.AnimationManager.playAnimation(MAGICAL_WORKING_ANIMATION_KEY, true);
                    return;
                }
                else
                {
                    this.AnimationManager.playAnimation(MAGICAL_IDLE_ANIMATION_KEY, true);
                    return;
                }
            }
        }

        /// <summary>
        /// Attempts to consume the necessary fuel item from the player's inventory.
        /// </summary>
        /// <param name="who"></param>
        /// <returns></returns>
        protected virtual bool consumeFuelItemFromFarmersInventory(Farmer who)
        {
            if (this.furnaceType.Value == FurnaceType.Magical)
            {
                return true;
            }
            if (this.furnaceType.Value == FurnaceType.Electric)
            {
                return PlayerUtilities.ReduceInventoryItemIfEnoughFound(who, Enums.SDVObject.BatteryPack, 1);
            }
            if (this.furnaceType.Value == FurnaceType.Nuclear)
            {
                return PlayerUtilities.ReduceInventoryItemIfEnoughFound(who, MiscItemIds.RadioactiveFuel, 1);
            }
            return true;
            //Magical does not consume fuel.

        }

        /// <summary>
        /// Consumes a single charge of fuel used on this funace.
        /// </summary>
        protected virtual void consumeFuelCharge()
        {
            if (this.furnaceType.Value == FurnaceType.Magical) return;
            this.chargesRemaining.Value--;
            if (this.chargesRemaining.Value <= 0) this.chargesRemaining.Value = 0;
        }

        /// <summary>
        /// Increases the fuel type for the furnace.
        /// </summary>
        protected virtual void increaseFuelCharges()
        {
            if (this.furnaceType.Value == FurnaceType.Electric)
            {
                this.chargesRemaining.Value = 5;
            }
            if (this.furnaceType.Value == FurnaceType.Nuclear)
            {
                this.chargesRemaining.Value = 25;
            }

            if (this.furnaceType.Value == FurnaceType.Magical)
            {
                this.chargesRemaining.Value = 999;
            }
        }

        /// <summary>
        /// Shows an error message if there is no correct fuel present for the furnace.
        /// </summary>
        protected virtual void showRedMessageForMissingFuel()
        {
            if (this.furnaceType.Value == FurnaceType.Electric)
            {
                Game1.showRedMessage("Needs a battery pack to operate!");
                return;
            }
            if (this.furnaceType.Value == FurnaceType.Nuclear)
            {
                Game1.showRedMessage("Needs nuclear fuel to operate!");
                return;
            }
            Game1.showRedMessage("Magical furnace failure! Please contact Omegasis to address this issue!");
            return;
        }


        protected override void drawStatusBubble(SpriteBatch b, int x, int y, float Alpha)
        {
            if (this.machineStatusBubbleBox == null || this.machineStatusBubbleBox.Value == null) this.createStatusBubble();
            if (this.finishedProduction())
            {
                y--;
                float num = (float)(4.0 * Math.Round(Math.Sin(DateTime.UtcNow.TimeOfDay.TotalMilliseconds / 250.0), 2));
                this.machineStatusBubbleBox.Value.playAnimation(MachineStatusBubble_BlankBubbleAnimationKey);
                this.machineStatusBubbleBox.Value.draw(b, this.machineStatusBubbleBox.Value.getTexture(), Game1.GlobalToLocal(Game1.viewport, new Vector2(x * Game1.tileSize, y * Game1.tileSize + num)), new Rectangle?(this.machineStatusBubbleBox.Value.getCurrentAnimationFrameRectangle()), Color.White, 0f, Vector2.Zero, Game1.pixelZoom, SpriteEffects.None, Math.Max(0f, (y + 2) * Game1.tileSize / 10000f) + .00001f);

                Rectangle itemSourceRectangle = GameLocation.getSourceRectForObject(this.heldObject.Value.ParentSheetIndex);
                this.machineStatusBubbleBox.Value.draw(b, Game1.objectSpriteSheet, Game1.GlobalToLocal(Game1.viewport, new Vector2((float)(x * Game1.tileSize) + 8, y * Game1.tileSize + num + 16)), new Rectangle?(itemSourceRectangle), Color.White, 0f, Vector2.Zero, Game1.pixelZoom, SpriteEffects.None, Math.Max(0f, (y + 2) * Game1.tileSize / 10000f) + .00002f);

            }
        }

        /// <summary>What happens when the object is drawn at a tile location.</summary>
        public override void draw(SpriteBatch spriteBatch, int x, int y, float alpha = 1f)
        {

            x = (int)this.TileLocation.X;

            y = (int)this.TileLocation.Y;

            if (this.MinutesUntilReady > 0)
            {
                Vector2 origin = new Vector2(this.AnimationManager.getCurrentAnimationFrameRectangle().Width / 2, this.AnimationManager.getCurrentAnimationFrameRectangle().Height);

                this.basicItemInformation.animationManager.draw(spriteBatch, this.basicItemInformation.animationManager.getTexture(), Game1.GlobalToLocal(Game1.viewport, new Vector2((float)((x + this.basicItemInformation.drawOffset.X) * Game1.tileSize) + this.basicItemInformation.shakeTimerOffset() + Game1.tileSize * origin.X / this.AnimationManager.getCurrentAnimationFrameRectangle().Width, (y + this.basicItemInformation.drawOffset.Y) * Game1.tileSize + this.basicItemInformation.shakeTimerOffset() + Game1.tileSize * (origin.Y / this.AnimationManager.getCurrentAnimationFrameRectangle().Height + 1))), new Rectangle?(this.AnimationManager.getCurrentAnimation().getCurrentAnimationFrameRectangle()), this.basicItemInformation.DrawColor * alpha, 0f, origin, this.getScaleSizeForWorkingMachine(), this.flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None, Math.Max(0f, (this.TileLocation.Y - this.basicItemInformation.drawOffset.Y) * Game1.tileSize / 10000f) + .00001f);
            }
            else
                this.basicItemInformation.animationManager.draw(spriteBatch, this.basicItemInformation.animationManager.getTexture(), Game1.GlobalToLocal(Game1.viewport, new Vector2((float)((x + this.basicItemInformation.drawOffset.X) * Game1.tileSize) + this.basicItemInformation.shakeTimerOffset(), (y + this.basicItemInformation.drawOffset.Y) * Game1.tileSize + this.basicItemInformation.shakeTimerOffset())), new Rectangle?(this.AnimationManager.getCurrentAnimation().getCurrentAnimationFrameRectangle()), this.basicItemInformation.DrawColor * alpha, 0f, Vector2.Zero, Game1.pixelZoom, this.flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None, Math.Max(0f, (this.TileLocation.Y - this.basicItemInformation.drawOffset.Y) * Game1.tileSize / 10000f) + .00001f);

            if (this.finishedProduction())
                this.drawStatusBubble(spriteBatch, x + (int)this.basicItemInformation.drawOffset.X, y + (int)this.basicItemInformation.drawOffset.Y, alpha);

        }


        public override Item getOne()
        {
            return new ElectricFurnace(this.basicItemInformation.Copy(), this.furnaceType.Value);
        }

        public override bool canStackWith(ISalable other)
        {
            if (!(other is ElectricFurnace)) return false;
            ElectricFurnace otherFurnace = (ElectricFurnace)other;
            return base.canStackWith(other) && otherFurnace.furnaceType.Value == this.furnaceType.Value;
        }



    }
}
