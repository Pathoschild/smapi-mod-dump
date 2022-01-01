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
using Revitalize.Framework.Objects.InformationFiles;
using Revitalize.Framework.World.Objects.InformationFiles;
using Revitalize.Framework.World.Objects.Interfaces;
using Revitalize.Framework;
using Revitalize.Framework.Crafting;
using Revitalize.Framework.Menus;
using Revitalize.Framework.Objects;
using Revitalize.Framework.Utilities;
using StardewValley;
using StardustCore.Animations;
using StardustCore.UIUtilities;
using StardustCore.UIUtilities.MenuComponents.ComponentsV2.Buttons;

namespace Revitalize.Framework.World.Objects.Machines
{
    public class Machine : CustomObject, IInventoryManagerProvider
    {

        [XmlIgnore]
        public List<ResourceInformation> producedResources
        {
            get
            {
                return MachineUtilities.GetResourcesProducedByThisMachine(this.basicItemInfo.id);
            }
            set
            {
                if (MachineUtilities.ResourcesForMachines == null) MachineUtilities.InitializeResourceList();
                if (MachineUtilities.ResourcesForMachines.ContainsKey(this.basicItemInfo.id)) return;
                MachineUtilities.ResourcesForMachines.Add(this.basicItemInfo.id, value);
            }
        }
        public int energyRequiredPer10Minutes;
        public int timeToProduce;

        public string craftingRecipeBook;

        [XmlIgnore]
        protected AnimationManager machineStatusBubbleBox;

        public Machine()
        {

        }


        public Machine(BasicItemInformation info, List<ResourceInformation> ProducedResources = null, int EnergyRequiredPer10Minutes = 0, int TimeToProduce = 0, string CraftingBook = "") : base(info)
        {
            this.producedResources = ProducedResources ?? new List<ResourceInformation>();
            this.energyRequiredPer10Minutes = EnergyRequiredPer10Minutes;
            this.timeToProduce = TimeToProduce;
            this.MinutesUntilReady = TimeToProduce;
            this.craftingRecipeBook = CraftingBook;
            this.createStatusBubble();
        }

        public Machine(BasicItemInformation info, Vector2 TileLocation, List<ResourceInformation> ProducedResources = null, int EnergyRequiredPer10Minutes = 0, int TimeToProduce = 0, string CraftingBook = "") : base(info, TileLocation)
        {
            this.producedResources = ProducedResources ?? new List<ResourceInformation>();
            this.energyRequiredPer10Minutes = EnergyRequiredPer10Minutes;
            this.timeToProduce = TimeToProduce;
            this.MinutesUntilReady = TimeToProduce;
            this.craftingRecipeBook = CraftingBook;
            this.createStatusBubble();
        }

        public virtual bool doesMachineProduceItems()
        {
            return this.producedResources.Count > 0;
        }

        public override bool minutesElapsed(int minutes, GameLocation environment)
        {

            return true;
            //return base.minutesElapsed(minutes, environment);
        }


        protected virtual void createStatusBubble()
        {
            this.machineStatusBubbleBox = new AnimationManager(TextureManager.GetExtendedTexture(ModCore.Manifest, "HUD", "MachineStatusBubble"), new Dictionary<string, Animation>()
            {
                {"Default",new Animation(0,0,20,24)},
                {"Empty",new Animation(20,0,20,24)},
                {"InventoryFull",new Animation(40,0,20,24)}
            }, "Default","Default", 0);
        }

        public override void updateWhenCurrentLocation(GameTime time, GameLocation environment)
        {
            base.updateWhenCurrentLocation(time, environment);

        }


        public override bool rightClicked(Farmer who)
        {
            /*
            if (this.getCurrentLocation() == null)
                this.location = Game1.player.currentLocation;
            if (Game1.menuUp || Game1.currentMinigame != null) return false;

            //ModCore.playerInfo.sittingInfo.sit(this, Vector2.Zero);
            */
            if (Game1.menuUp || Game1.currentMinigame != null) return false;
            return false;
        }

        public override Item getOne()
        {
            Machine component = new Machine(this.basicItemInfo.Copy(), this.producedResources, this.energyRequiredPer10Minutes, this.timeToProduce, this.craftingRecipeBook);
            return component;
        }

        /// <summary>What happens when the object is drawn at a tile location.</summary>
        public override void draw(SpriteBatch spriteBatch, int x, int y, float alpha = 1f)
        {


            if (x <= -1)
            {
                return;
                //spriteBatch.Draw(this.basicItemInfo.animationManager.getTexture(), Game1.GlobalToLocal(Game1.viewport, this.TileLocation), new Rectangle?(this.AnimationManager.currentAnimation.sourceRectangle), this.basicItemInfo.DrawColor * alpha, 0f, Vector2.Zero, (float)Game1.pixelZoom, this.flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None, Math.Max(0f, (float)(this.TileLocation.Y * Game1.tileSize) / 10000f));
            }
            else
            {
                if (this.AnimationManager == null)
                {
                    if (this.CurrentTextureToDisplay == null)
                    {
                        ModCore.log("Texture null for item: " + this.basicItemInfo.id);
                        return;
                    }
                }
                if (this.AnimationManager == null)
                {
                    if (this.AnimationManager.getExtendedTexture() == null)
                        ModCore.ModMonitor.Log("Tex Extended is null???");

                    spriteBatch.Draw(this.CurrentTextureToDisplay, Game1.GlobalToLocal(Game1.viewport, new Vector2((float)(x * Game1.tileSize), y * Game1.tileSize)), new Rectangle?(this.AnimationManager.getCurrentAnimationFrameRectangle()), this.basicItemInfo.DrawColor * alpha, 0f, Vector2.Zero, (float)Game1.pixelZoom, this.flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None, Math.Max(0f, (float)(this.TileLocation.Y * Game1.tileSize) / 10000f));
                    this.drawStatusBubble(spriteBatch, x,y, alpha);
                    // Log.AsyncG("ANIMATION IS NULL?!?!?!?!");
                }

                else
                {
                    //Log.AsyncC("Animation Manager is working!");
                    this.AnimationManager.draw(spriteBatch, this.CurrentTextureToDisplay, Game1.GlobalToLocal(Game1.viewport, new Vector2((float)(x * Game1.tileSize), y * Game1.tileSize)), new Rectangle?(this.AnimationManager.getCurrentAnimationFrameRectangle()), this.basicItemInfo.DrawColor * alpha, 0f, Vector2.Zero, (float)Game1.pixelZoom, this.flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None, Math.Max(0f, (float)((this.TileLocation.Y) * Game1.tileSize) / 10000f));
                    this.drawStatusBubble(spriteBatch, x, y, alpha);
                }

                // spriteBatch.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2((float)((double)tileLocation.X * (double)Game1.tileSize + (((double)tileLocation.X * 11.0 + (double)tileLocation.Y * 7.0) % 10.0 - 5.0)) + (float)(Game1.tileSize / 2), (float)((double)tileLocation.Y * (double)Game1.tileSize + (((double)tileLocation.Y * 11.0 + (double)tileLocation.X * 7.0) % 10.0 - 5.0)) + (float)(Game1.tileSize / 2))), new Rectangle?(new Rectangle((int)((double)tileLocation.X * 51.0 + (double)tileLocation.Y * 77.0) % 3 * 16, 128 + this.whichForageCrop * 16, 16, 16)), Color.White, 0.0f, new Vector2(8f, 8f), (float)Game1.pixelZoom, SpriteEffects.None, (float)(((double)tileLocation.Y * (double)Game1.tileSize + (double)(Game1.tileSize / 2) + (((double)tileLocation.Y * 11.0 + (double)tileLocation.X * 7.0) % 10.0 - 5.0)) / 10000.0));
            }
        }

        public virtual void produceItem()
        {
            foreach (ResourceInformation r in this.producedResources)
            {
                if (r.shouldDropResource())
                {
                    Item i = r.getItemDrops();
                    this.GetInventoryManager().addItem(i);
                    //ModCore.log("Produced an item!");
                }
            }

        }

        protected virtual void drawStatusBubble(SpriteBatch b, int x, int y, float Alpha)
        {
            if (this.machineStatusBubbleBox == null) this.createStatusBubble();
            if (this.GetInventoryManager() == null) return;
            if (this.GetInventoryManager().IsFull && this.doesMachineProduceItems() && ModCore.Configs.machinesConfig.showMachineNotificationBubble_InventoryFull)
            {
                y--;
                float num = (float)(4.0 * Math.Round(Math.Sin(DateTime.UtcNow.TimeOfDay.TotalMilliseconds / 250.0), 2));
                this.machineStatusBubbleBox.playAnimation("InventoryFull");
                this.machineStatusBubbleBox.draw(b, this.machineStatusBubbleBox.getTexture(), Game1.GlobalToLocal(Game1.viewport, new Vector2((float)(x * Game1.tileSize), y * Game1.tileSize + num)), new Rectangle?(this.machineStatusBubbleBox.getCurrentAnimationFrameRectangle()), Color.White * ModCore.Configs.machinesConfig.machineNotificationBubbleAlpha, 0f, Vector2.Zero, (float)Game1.pixelZoom, SpriteEffects.None, Math.Max(0f, (float)((y + 2) * Game1.tileSize) / 10000f) + .00001f);
            }
            else
            {

            }
        }


        public virtual ref InventoryManager GetInventoryManager()
        {
            if (this.basicItemInfo == null)
            {
                return ref this.basicItemInfo.inventory;
            }
            return ref this.basicItemInfo.inventory;
        }

        public virtual void SetInventoryManager(InventoryManager Manager)
        {
            this.basicItemInfo.inventory = Manager;
        }



    }
}
