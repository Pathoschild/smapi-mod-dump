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
using StardewValley;
using Netcode;
using Omegasis.Revitalize.Framework.Utilities;
using Omegasis.Revitalize.Framework.World.Objects.InformationFiles;
using Omegasis.Revitalize.Framework.World.Objects.Interfaces;
using Omegasis.StardustCore.Animations;
using Omegasis.StardustCore.UIUtilities;
using Omegasis.Revitalize.Framework.Utilities.JsonContentLoading;
using System.IO;
using Omegasis.Revitalize.Framework.HUD;
using Omegasis.Revitalize.Framework.World.WorldUtilities;
using Omegasis.Revitalize.Framework.Constants;
using Omegasis.Revitalize.Framework.Crafting;
using Newtonsoft.Json;

namespace Omegasis.Revitalize.Framework.World.Objects.Machines
{
    [XmlType("Mods_Omegasis.Revitalize.Framework.World.Objects.Machines.Machine")]
    public class Machine : CustomObject, IInventoryManagerProvider
    {

        public const string DEFAULT_ANINMATION_KEY = "Default";
        public const string WORKING_ANIMATION_KEY = "Working";

        public const string MachineStatusBubble_DefaultAnimationKey = "Default";
        public const string MachineStatusBubble_BlankBubbleAnimationKey = "Blank";
        public const string MachineStatusBubble_InventoryFullAnimationKey = "InventoryFull";
        public NetBool lerpScaleIncreasing = new NetBool(true);

        [XmlIgnore]
        public NetRef<AnimationManager> machineStatusBubbleBox = new NetRef<AnimationManager>();


        [JsonIgnore]
        public readonly NetObjectList<Item> heldItems = new NetObjectList<Item>();
        public Machine()
        {

        }


        public Machine(BasicItemInformation info) : base(info)
        {
            this.createStatusBubble();
        }

        public Machine(BasicItemInformation info, Vector2 TileLocation) : base(info, TileLocation)
        {
            this.createStatusBubble();
        }

        protected override void initializeNetFieldsPostConstructor()
        {
            base.initializeNetFieldsPostConstructor();
            this.NetFields.AddFields(this.machineStatusBubbleBox, this.lerpScaleIncreasing, this.heldItems);
        }

        public override bool minutesElapsed(int minutes, GameLocation environment)
        {
            base.minutesElapsed(minutes, environment);

            if (this.finishedProduction())
            {
                this.updateAnimation();
            }

            return false;
        }

        /// <summary>
        /// A simple method for calculating the scale size for showing a machine working.
        /// </summary>
        /// <returns></returns>
        public virtual float getScaleSizeForWorkingMachine()
        {
            float zoomSpeed = 0.01f;
            if (this.Scale.X < Game1.pixelZoom)
                this.Scale = new Vector2(Game1.pixelZoom, Game1.pixelZoom);

            if (this.isWorking())
            {
                if (this.lerpScaleIncreasing.Value == true)
                {
                    this.Scale = new Vector2(this.scale.X + zoomSpeed, this.scale.Y + zoomSpeed);
                    if (this.Scale.X >= 5.0)
                        this.lerpScaleIncreasing.Value = false;
                }
                else
                {
                    this.Scale = new Vector2(this.scale.X - zoomSpeed, this.scale.Y - zoomSpeed);
                    if (this.Scale.X <= Game1.pixelZoom)
                        this.lerpScaleIncreasing.Value = true;
                }
                return this.Scale.X * Game1.options.zoomLevel;

            }
            else
            {
                float zoom = Game1.pixelZoom * Game1.options.zoomLevel;
                return zoom;
            }
        }

        protected virtual void createStatusBubble()
        {
            this.machineStatusBubbleBox.Value = new AnimationManager(TextureManager.GetExtendedTexture(RevitalizeModCore.Manifest, "Omegasis.Revitalize.HUD", "MachineStatusBubble"), new SerializableDictionary<string, Animation>()
            {
                {MachineStatusBubble_DefaultAnimationKey,new Animation(0,0,20,24)},
                {MachineStatusBubble_BlankBubbleAnimationKey,new Animation(20,0,20,24)},
                {MachineStatusBubble_InventoryFullAnimationKey,new Animation(40,0,20,24)}
            }, MachineStatusBubble_DefaultAnimationKey, MachineStatusBubble_DefaultAnimationKey, 0);
        }


        /// <summary>
        /// Used to process input for machines as well as providing an accessor to the Automate mod for this mod.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="who"></param>
        /// <param name="ShowRedMessage"></param>
        /// <returns></returns>
        public virtual CraftingResult processInput(IList<Item> items, Farmer who, bool ShowRedMessage = true)
        {
            return new CraftingResult(false);
        }

        /// <summary>
        /// Gets the output for this machine.
        /// </summary>
        /// <param name="AddToPlayersInventory">Attempts to add the items to the player's inventory, or to the ground if they can't pickup any more items.</param>
        /// <param name="DropAsItemDebris">Just drops the items to the ground as item debris.</param>
        /// <returns>The items produced by this machine.</returns>
        public virtual List<Item> getMachineOutputs(bool AddToPlayersInventory, bool DropAsItemDebris, bool ShowInventoryFullError)
        {
            List<Item> outputItems = this.getMachineOutputItems(false);
            List<Item> itemsToRequeue = new List<Item>();
            List<Item> itemsAdded=new List<Item>();
            bool anyAdded = false;
            bool shouldShowInventoryFullError = false;

            foreach (Item item in outputItems)
            {
                if (item == null) continue;
                if (AddToPlayersInventory)
                {

                    bool added = Game1.player.addItemToInventoryBool(item);
                    if (added)
                    {
                        itemsAdded.Add(item);
                        continue;
                    }

                    if (added == false && DropAsItemDebris)
                    {
                        WorldUtility.CreateItemDebrisAtTileLocation(this.getCurrentLocation(), item, this.TileLocation);
                        SoundUtilities.PlaySound(Enums.StardewSound.coin);
                        continue;
                    }
                    else if (added == false && DropAsItemDebris == false)
                    {
                        shouldShowInventoryFullError = true;
                        itemsToRequeue.Add(item);
                        continue;
                    }
                    else if(added==false && DropAsItemDebris == true)
                    {
                        WorldUtility.CreateItemDebrisAtTileLocation(this.getCurrentLocation(), item, this.TileLocation);
                        SoundUtilities.PlaySound(Enums.StardewSound.coin);
                        continue;
                    }
                }
                else
                {
                    if(DropAsItemDebris)
                    {
                        WorldUtility.CreateItemDebrisAtTileLocation(this.getCurrentLocation(), item, this.TileLocation);
                        SoundUtilities.PlaySound(Enums.StardewSound.coin);
                        continue;
                    }
                    else
                    {
                        shouldShowInventoryFullError = true;
                        itemsToRequeue.Add(item);
                        continue;
                    }
                }
            }

            if (shouldShowInventoryFullError && ShowInventoryFullError)
            {
                //Show inventory full error.
                HudUtilities.ShowInventoryFullErrorMessage();
            }

            if (itemsAdded.Count > 0)
            {
                this.heldItems.Clear();
                this.heldObject.Value = null;
                foreach (Item item in itemsToRequeue)
                {
                    this.addItemToHeldItemQueue(item);
                }

                //Update animations according to if there are still held objects that need to be processed or not.
                if (!this.hasItemsInHeldItemQueue())
                {
                    this.heldObject.Value = null;
                    this.updateAnimation();
                }
                else
                {
                    this.heldObject.Value =(StardewValley.Object) this.getItemFromHeldItemQueue();
                }
            }





            return itemsAdded;
        }

        /// <summary>
        /// Used for automate compatibility.
        /// </summary>
        /// <param name="ClearValue"></param>
        /// <returns></returns>
        public virtual Item getMachineOutputItem(bool ClearValue = false)
        {
            if (this.heldObject.Value == null) return null;
            if (!this.finishedProduction()) return null;
            Item item = this.heldObject.Value;
            if (ClearValue)
            {
                this.heldObject.Value = null;
            }
            return item;
        }

        public virtual List<Item> getMachineOutputItems(bool ClearValue = false)
        {
            if (this.heldObject.Value == null) return new List<Item>();
            List<Item> items= new List<Item>() { this.getMachineOutputItem(ClearValue) };
            items.AddRange(this.heldItems);
            if (ClearValue)
            {
                this.heldItems.Clear();
            }
            return items;
        }

        public override void performRemoveAction(Vector2 tileLocation, GameLocation environment)
        {
            this.getMachineOutputs(true, true, false);
            base.performRemoveAction(tileLocation, environment);
        }

        public override bool checkForAction(Farmer who, bool justCheckingForActivity)
        {
            if (Game1.menuUp || Game1.currentMinigame != null) return false;
            if (this.finishedProduction() && who.IsLocalPlayer)
            {
                this.getMachineOutputs(true, false, true);
            }

            return base.checkForAction(who);
        }


        public virtual InventoryManager GetInventoryManager()
        {
            if (this.basicItemInformation == null)
                return this.basicItemInformation.inventory;
            return this.basicItemInformation.inventory;
        }

        public virtual void SetInventoryManager(InventoryManager Manager)
        {
            this.basicItemInformation.inventory = Manager;
        }

        /// <summary>
        /// Is this machine finished producing it's item.
        /// </summary>
        /// <returns></returns>
        public virtual bool finishedProduction()
        {
            return this.MinutesUntilReady == 0 && this.heldObject.Value != null;
        }

        /// <summary>
        /// Checks to see if this machine is currently working.
        /// </summary>
        /// <returns></returns>
        public virtual bool isWorking()
        {
            return this.MinutesUntilReady > 0;
        }

        /// <summary>
        /// Checks to see if the machine is ready to start working.
        /// </summary>
        /// <returns></returns>
        public virtual bool isIdle()
        {
            return this.MinutesUntilReady == 0 && this.heldObject.Value == null;
        }

        public virtual void addItemToHeldItemQueue(Item item)
        {
            this.heldItems.Add(item);
        }

        /// <summary>
        /// Gets the first item in the held item queue for this object.
        /// </summary>
        /// <returns></returns>
        public virtual Item getItemFromHeldItemQueue()
        {
            if (!this.hasItemsInHeldItemQueue())
            {
                return null;
            }
            Item item = this.heldItems[0];
            this.heldItems.RemoveAt(0);
            return item;
        }

        /// <summary>
        /// Checks to see if there are any held items that should be removed from the held item queue for this machine.
        /// </summary>
        /// <returns></returns>
        public virtual bool hasItemsInHeldItemQueue()
        {
            return this.heldItems.Count > 0;
        }


        /// <summary>
        /// Returns a common error string to display to the player that more items are necessary to use a machine for drop in purposes.
        /// </summary>
        /// <param name="AmountRequired"></param>
        /// <param name="NeededDropInItemDisplayName"></param>
        /// <returns></returns>
        public virtual string getErrorString_NeedMoreInputItems(int AmountRequired, Item NeededDropInItemDisplayName)
        {
            return JsonContentPackUtilities.LoadErrorString(Path.Combine("Objects", "CommonErrorStrings.json"), "NeedMoreInputItems", AmountRequired, NeededDropInItemDisplayName.DisplayName);
        }

        /// <summary>
        /// Updates the animation manager to play the correct animation.
        /// </summary>
        public virtual void updateAnimation()
        {
            this.AnimationManager.playDefaultAnimation();
        }

        public override bool canBeRemoved(Farmer who = null)
        {
            
            //Prevent the player from accidentally picking up this machine if it is working.
            if (this.isWorking()) return false;
            if (this.heldObject.Value != null) return false;

            
            return base.canBeRemoved(who);
        }

        public override Item getOne()
        {
            Machine component = new Machine(this.basicItemInformation.Copy());
            return component;
        }

        /// <summary>What happens when the object is drawn at a tile location.</summary>
        public override void draw(SpriteBatch spriteBatch, int x, int y, float alpha = 1f)
        {
            x = (int)this.TileLocation.X;

            y = (int)this.TileLocation.Y;

            if (this.isWorking())
            {
                Vector2 origin = new Vector2(this.AnimationManager.getCurrentAnimationFrameRectangle().Width / 2, this.AnimationManager.getCurrentAnimationFrameRectangle().Height);

                this.basicItemInformation.animationManager.draw(spriteBatch, this.basicItemInformation.animationManager.getTexture(), Game1.GlobalToLocal(Game1.viewport, new Vector2((float)((x + this.basicItemInformation.drawOffset.X) * Game1.tileSize) + this.basicItemInformation.shakeTimerOffset() + Game1.tileSize * origin.X / this.AnimationManager.getCurrentAnimationFrameRectangle().Width, (y + this.basicItemInformation.drawOffset.Y) * Game1.tileSize + this.basicItemInformation.shakeTimerOffset() + Game1.tileSize * (origin.Y / this.AnimationManager.getCurrentAnimationFrameRectangle().Height + 1))), new Rectangle?(this.AnimationManager.getCurrentAnimation().getCurrentAnimationFrameRectangle()), this.basicItemInformation.DrawColor * alpha, 0f, origin, this.getScaleSizeForWorkingMachine(), this.Flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None, Math.Max(0f, (this.TileLocation.Y - this.basicItemInformation.drawOffset.Y) * Game1.tileSize / 10000f) + .00001f);
            }
            else
                this.basicItemInformation.animationManager.draw(spriteBatch, this.basicItemInformation.animationManager.getTexture(), Game1.GlobalToLocal(Game1.viewport, new Vector2((float)((x + this.basicItemInformation.drawOffset.X) * Game1.tileSize) + this.basicItemInformation.shakeTimerOffset(), (y + this.basicItemInformation.drawOffset.Y) * Game1.tileSize + this.basicItemInformation.shakeTimerOffset())), new Rectangle?(this.AnimationManager.getCurrentAnimation().getCurrentAnimationFrameRectangle()), this.basicItemInformation.DrawColor * alpha, 0f, Vector2.Zero, Game1.pixelZoom, this.Flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None, Math.Max(0f, (this.TileLocation.Y - this.basicItemInformation.drawOffset.Y) * Game1.tileSize / 10000f) + .00001f);

            if (this.finishedProduction())
                this.drawStatusBubble(spriteBatch, x + (int)this.basicItemInformation.drawOffset.X, y + (int)this.basicItemInformation.drawOffset.Y, alpha);
        }

        protected virtual void drawStatusBubble(SpriteBatch b, int x, int y, float Alpha)
        {
            if (this.machineStatusBubbleBox.Value == null || this.machineStatusBubbleBox.Value == null) this.createStatusBubble();
            if (this.finishedProduction())
            {
                y--;
                float num = (float)(4.0 * Math.Round(Math.Sin(DateTime.UtcNow.TimeOfDay.TotalMilliseconds / 250.0), 2));
                this.machineStatusBubbleBox.Value.playAnimation(MachineStatusBubble_BlankBubbleAnimationKey);
                this.machineStatusBubbleBox.Value.draw(b, this.machineStatusBubbleBox.Value.getTexture(), Game1.GlobalToLocal(Game1.viewport, new Vector2(x * Game1.tileSize, y * Game1.tileSize + num)), new Rectangle?(this.machineStatusBubbleBox.Value.getCurrentAnimationFrameRectangle()), Color.White * .75f, 0f, Vector2.Zero, Game1.pixelZoom, SpriteEffects.None, Math.Max(0f, (y + 3) * Game1.tileSize / 10000f) + .00002f);

                this.drawStatusBubbleItemOutput(b, x, y, Alpha);
            }
        }

        protected virtual void drawStatusBubbleItemOutput(SpriteBatch b, int x, int y, float Alpha)
        {
            if (this.heldObject.Value == null) return;
            float num = (float)(4.0 * Math.Round(Math.Sin(DateTime.UtcNow.TimeOfDay.TotalMilliseconds / 250.0), 2));
            Rectangle itemSourceRectangle = GameLocation.getSourceRectForObject(this.heldObject.Value.ParentSheetIndex);
            this.machineStatusBubbleBox.Value.draw(b, Game1.objectSpriteSheet, Game1.GlobalToLocal(Game1.viewport, new Vector2((float)(x * Game1.tileSize) + 8, y * Game1.tileSize + num + 16)), new Rectangle?(itemSourceRectangle), Color.White * .75f, 0f, Vector2.Zero, Game1.pixelZoom, SpriteEffects.None, Math.Max(0f, (y + 3) * Game1.tileSize / 10000f) + .00003f);
        }
    }
}
