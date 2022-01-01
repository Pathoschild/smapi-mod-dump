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
using Newtonsoft.Json;
using Omegasis.Revitalize.Framework;
using Omegasis.Revitalize.Framework.Objects.InformationFiles;
using Omegasis.Revitalize.Framework.World.Objects.InformationFiles;
using Omegasis.Revitalize.Framework.World.Objects.Interfaces;
using Omegasis.Revitalize.Framework.World.Objects.Machines;
using StardewValley;
using StardustCore.Animations;
using StardustCore.UIUtilities;

namespace Omegasis.Revitalize.Framework.World.Objects.Machines
{
    public class Grinder : Machine
    {

        public Grinder() { }

        public Grinder(BasicItemInformation info, List<ResourceInformation> ProducedResources = null, int EnergyRequiredPer10Minutes = 0, int TimeToProduce = 0, string CraftingBook = "") : base(info)
        {
            this.producedResources = ProducedResources ?? new List<ResourceInformation>();
            this.energyRequiredPer10Minutes = EnergyRequiredPer10Minutes;
            this.timeToProduce = TimeToProduce;
            this.MinutesUntilReady = TimeToProduce;
            this.craftingRecipeBook = CraftingBook;
            this.createStatusBubble();

        }

        public Grinder( BasicItemInformation info, Vector2 TileLocation, List<ResourceInformation> ProducedResources = null, int EnergyRequiredPer10Minutes = 0, int TimeToProduce = 0, string CraftingBook = "") : base(info, TileLocation)
        {
            this.producedResources = ProducedResources ?? new List<ResourceInformation>();
            this.energyRequiredPer10Minutes = EnergyRequiredPer10Minutes;
            this.timeToProduce = TimeToProduce;
            this.MinutesUntilReady = TimeToProduce;
            this.craftingRecipeBook = CraftingBook;
            this.createStatusBubble();
        }

        public override void updateWhenCurrentLocation(GameTime time, GameLocation environment)
        {
            base.updateWhenCurrentLocation(time, environment);
        }


        public override bool minutesElapsed(int minutes, GameLocation environment)
        {
          
                if (this.energyRequiredPer10Minutes != ModCore.Configs.machinesConfig.grinderEnergyConsumption) this.energyRequiredPer10Minutes = ModCore.Configs.machinesConfig.grinderEnergyConsumption;

                //this.MinutesUntilReady -= minutes;
                int remaining = minutes;
                //ModCore.log("Minutes elapsed: " + remaining);
                List<IEnergyManagerProvider> energySources = new List<IEnergyManagerProvider>();
                if (this.doesMachineConsumeEnergy())
                {
                    //ModCore.log("This machine drains energy: " + this.info.name);
                    energySources = this.EnergyGraphSearchSources(); //Only grab the network once.
                }

                while (remaining > 0)
                {

                    if (this.doesMachineConsumeEnergy())
                    {
                        this.drainEnergyFromNetwork(energySources); //Continually drain from the network.                        
                        if (this.GetEnergyManager().remainingEnergy < ModCore.Configs.machinesConfig.grinderEnergyConsumption) return false;
                        else
                        {
                            this.GetEnergyManager().consumeEnergy(ModCore.Configs.machinesConfig.grinderEnergyConsumption); //Consume the required amount of energy necessary.
                        }
                    }
                    else
                    {
                        //ModCore.log("Does not produce energy or consume energy so do whatever!");
                    }
                    remaining -= 10;
                    this.MinutesUntilReady -= 10;

                    if (this.MinutesUntilReady <= 0 && this.GetInventoryManager().IsFull == false)
                    {
                        this.produceItem();
                        this.consumeItemForGrinding();
                    }
                }


                return false;

            //return base.minutesElapsed(minutes, environment);
        }

        public override bool rightClicked(Farmer who)
        {
            this.createMachineMenu();
            return true;
        }

        public override Item getOne()
        {
            Grinder component = new Grinder(this.basicItemInfo.Copy(), this.TileLocation, this.producedResources, this.energyRequiredPer10Minutes, this.timeToProduce, this.craftingRecipeBook);
            return component;
        }

        public override void draw(SpriteBatch spriteBatch, int x, int y, float alpha = 1f)
        {

            //The actual planter box being drawn.
            if (this.AnimationManager == null)
            {
                if (this.AnimationManager.getExtendedTexture() == null)
                    ModCore.ModMonitor.Log("Tex Extended is null???");

                spriteBatch.Draw(this.CurrentTextureToDisplay, Game1.GlobalToLocal(Game1.viewport, new Vector2((float)(x * Game1.tileSize), y * Game1.tileSize)), new Rectangle?(this.AnimationManager.currentAnimation.sourceRectangle), this.basicItemInfo.DrawColor * alpha, 0f, Vector2.Zero, (float)Game1.pixelZoom, this.Flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None, Math.Max(0f, (float)(y * Game1.tileSize) / 10000f));
            }

            else
            {
                //Log.AsyncC("Animation Manager is working!");
                float addedDepth = 0;
                this.AnimationManager.draw(spriteBatch, this.CurrentTextureToDisplay, Game1.GlobalToLocal(Game1.viewport, new Vector2((float)(x * Game1.tileSize), y * Game1.tileSize)), new Rectangle?(this.AnimationManager.currentAnimation.sourceRectangle), this.basicItemInfo.DrawColor * alpha, 0f, Vector2.Zero, (float)Game1.pixelZoom, this.Flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None, Math.Max(0f, (float)((y + addedDepth) * Game1.tileSize) / 10000f) + .00001f);
                this.drawStatusBubble(spriteBatch, x, y, alpha);

                try
                {
                    if (this.AnimationManager.canTickAnimation())
                    {
                        this.AnimationManager.tickAnimation();
                    }
                    // Log.AsyncC("Tick animation");
                }
                catch (Exception err)
                {
                    ModCore.ModMonitor.Log(err.ToString());
                }
            }

            // spriteBatch.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2((float)((double)tileLocation.X * (double)Game1.tileSize + (((double)tileLocation.X * 11.0 + (double)tileLocation.Y * 7.0) % 10.0 - 5.0)) + (float)(Game1.tileSize / 2), (float)((double)tileLocation.Y * (double)Game1.tileSize + (((double)tileLocation.Y * 11.0 + (double)tileLocation.X * 7.0) % 10.0 - 5.0)) + (float)(Game1.tileSize / 2))), new Rectangle?(new Rectangle((int)((double)tileLocation.X * 51.0 + (double)tileLocation.Y * 77.0) % 3 * 16, 128 + this.whichForageCrop * 16, 16, 16)), Color.White, 0.0f, new Vector2(8f, 8f), (float)Game1.pixelZoom, SpriteEffects.None, (float)(((double)tileLocation.Y * (double)Game1.tileSize + (double)(Game1.tileSize / 2) + (((double)tileLocation.Y * 11.0 + (double)tileLocation.X * 7.0) % 10.0 - 5.0)) / 10000.0));
        }

        public override void produceItem()
        {
            if (this.GetInventoryManager().hasItemsInBuffer)
            {
                this.GetInventoryManager().dumpBufferToItems();
            }
        }

        protected bool consumeItemForGrinding()
        {

            Item itemToRemove = null;
            foreach(Item I in this.GetInventoryManager().items)
            {
                if(I.canStackWith(new StardewValley.Object((int)Enums.SDVObject.CopperOre, 1))){
                    this.GetInventoryManager().bufferItems.Add(ModCore.ObjectManager.resources.getResource("CopperSand", 2));
                    itemToRemove = I;
                    this.MinutesUntilReady = ModCore.Configs.machinesConfig.grinderTimeToGrind;
                    break;
                }
                if (I.canStackWith(new StardewValley.Object((int)Enums.SDVObject.IronOre, 1)))
                {
                    this.GetInventoryManager().bufferItems.Add(ModCore.ObjectManager.resources.getResource("IronSand", 2));
                    itemToRemove = I;
                    this.MinutesUntilReady = ModCore.Configs.machinesConfig.grinderTimeToGrind;
                    break;
                }
                if (I.canStackWith(new StardewValley.Object((int)Enums.SDVObject.GoldOre, 1)))
                {
                    this.GetInventoryManager().bufferItems.Add(ModCore.ObjectManager.resources.getResource("GoldSand", 2));
                    itemToRemove = I;
                    this.MinutesUntilReady = ModCore.Configs.machinesConfig.grinderTimeToGrind;
                    break;
                }
                if (I.canStackWith(new StardewValley.Object((int)Enums.SDVObject.IridiumOre, 1)))
                {
                    this.GetInventoryManager().bufferItems.Add(ModCore.ObjectManager.resources.getResource("IridiumSand", 2));
                    itemToRemove = I;
                    this.MinutesUntilReady = ModCore.Configs.machinesConfig.grinderTimeToGrind;
                    break;
                }

                if (I.canStackWith(ModCore.ObjectManager.resources.getOre("BauxiteOre")))
                {
                    this.GetInventoryManager().bufferItems.Add(ModCore.ObjectManager.resources.getResource("BauxiteSand", 2));
                    itemToRemove = I;
                    this.MinutesUntilReady = ModCore.Configs.machinesConfig.grinderTimeToGrind;
                    break;
                }
                if (I.canStackWith(ModCore.ObjectManager.resources.getOre("LeadOre")))
                {
                    this.GetInventoryManager().bufferItems.Add(ModCore.ObjectManager.resources.getResource("LeadSand", 2));
                    itemToRemove = I;
                    this.MinutesUntilReady = ModCore.Configs.machinesConfig.grinderTimeToGrind;
                    break;
                }
                if (I.canStackWith(ModCore.ObjectManager.resources.getOre("SilverOre")))
                {
                    this.GetInventoryManager().bufferItems.Add(ModCore.ObjectManager.resources.getResource("SilverSand", 2));
                    itemToRemove = I;
                    this.MinutesUntilReady = ModCore.Configs.machinesConfig.grinderTimeToGrind;
                    break;
                }
                if (I.canStackWith(ModCore.ObjectManager.resources.getOre("TinOre")))
                {
                    this.GetInventoryManager().bufferItems.Add(ModCore.ObjectManager.resources.getResource("TinSand", 2));
                    itemToRemove = I;
                    this.MinutesUntilReady = ModCore.Configs.machinesConfig.grinderTimeToGrind;
                    break;
                }
                if (I.canStackWith(ModCore.ObjectManager.resources.getOre("TitaniumOre")))
                {
                    this.GetInventoryManager().bufferItems.Add(ModCore.ObjectManager.resources.getResource("TitaniumSand", 2));
                    itemToRemove = I;
                    this.MinutesUntilReady = ModCore.Configs.machinesConfig.grinderTimeToGrind;
                    break;
                }

                if (I.canStackWith(new StardewValley.Object((int)Enums.SDVObject.Stone, 1)))
                {
                    this.GetInventoryManager().bufferItems.Add(ModCore.ObjectManager.resources.getResource("Sand", 1));
                    itemToRemove = I;
                    this.MinutesUntilReady = ModCore.Configs.machinesConfig.grinderTimeToGrind;
                    break;
                }
            }

            if (itemToRemove == null) return false;

            if (itemToRemove.Stack > 1)
            {
                itemToRemove.Stack--;
            }
            else if (itemToRemove.Stack == 1)
            {
                this.GetInventoryManager().items.Remove(itemToRemove);
            }
            return true;

        }

    }
}
