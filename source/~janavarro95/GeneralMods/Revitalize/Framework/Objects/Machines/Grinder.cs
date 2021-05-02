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
using PyTK.CustomElementHandler;
using Revitalize.Framework.Objects.InformationFiles;
using StardewValley;
using StardustCore.Animations;
using StardustCore.UIUtilities;

namespace Revitalize.Framework.Objects.Machines
{
    public class Grinder : Machine
    {

        public Grinder() { }

        public Grinder(CustomObjectData PyTKData, BasicItemInformation info, List<ResourceInformation> ProducedResources = null, int EnergyRequiredPer10Minutes = 0, int TimeToProduce = 0, bool UpdatesContainer = false, string CraftingBook = "") : base(PyTKData, info)
        {
            this.producedResources = ProducedResources ?? new List<ResourceInformation>();
            this.energyRequiredPer10Minutes = ModCore.Configs.machinesConfig.grinderEnergyConsumption;
            this.timeToProduce = TimeToProduce;
            this.updatesContainerObjectForProduction = UpdatesContainer;
            this.MinutesUntilReady = TimeToProduce;
            this.craftingRecipeBook = CraftingBook;
            this.createStatusBubble();

        }

        public Grinder(CustomObjectData PyTKData, BasicItemInformation info, Vector2 TileLocation, List<ResourceInformation> ProducedResources = null, int EnergyRequiredPer10Minutes = 0, int TimeToProduce = 0, bool UpdatesContainer = false, string CraftingBook = "", MultiTiledObject obj = null) : base(PyTKData, info, TileLocation)
        {
            this.containerObject = obj;
            this.producedResources = ProducedResources ?? new List<ResourceInformation>();
            this.energyRequiredPer10Minutes = ModCore.Configs.machinesConfig.grinderEnergyConsumption;
            this.timeToProduce = TimeToProduce;
            this.updatesContainerObjectForProduction = UpdatesContainer;
            this.MinutesUntilReady = TimeToProduce;
            this.craftingRecipeBook = CraftingBook;
            this.createStatusBubble();
        }

        public Grinder(CustomObjectData PyTKData, BasicItemInformation info, Vector2 TileLocation, Vector2 offsetKey, List<ResourceInformation> ProducedResources = null, int EnergyRequiredPer10Minutes = 0, int TimeToProduce = 0, bool UpdatesContainer = false, string CraftingBook = "", MultiTiledObject obj = null) : base(PyTKData, info, TileLocation)
        {
            this.offsetKey = offsetKey;
            this.containerObject = obj;
            this.producedResources = ProducedResources ?? new List<ResourceInformation>();
            this.energyRequiredPer10Minutes = ModCore.Configs.machinesConfig.grinderEnergyConsumption;
            this.timeToProduce = TimeToProduce;
            this.updatesContainerObjectForProduction = UpdatesContainer;
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
            this.updateInfo();
            if (this.updatesContainerObjectForProduction)
            {
                if (this.energyRequiredPer10Minutes != ModCore.Configs.machinesConfig.grinderEnergyConsumption) this.energyRequiredPer10Minutes = ModCore.Configs.machinesConfig.grinderEnergyConsumption;

                //this.MinutesUntilReady -= minutes;
                int remaining = minutes;
                //ModCore.log("Minutes elapsed: " + remaining);
                List<MultiTiledObject> energySources = new List<MultiTiledObject>();
                if (this.ConsumesEnergy)
                {
                    //ModCore.log("This machine drains energy: " + this.info.name);
                    energySources = this.EnergyGraphSearchSources(); //Only grab the network once.
                }

                while (remaining > 0)
                {

                    if (this.ConsumesEnergy)
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
                    this.containerObject.MinutesUntilReady -= 10;

                    if (this.containerObject.MinutesUntilReady <= 0 && this.GetInventoryManager().IsFull == false)
                    {
                        this.produceItem();
                        this.consumeItemForGrinding();
                    }
                }


                return false;
            }
            else
            {

                return false;
            }

            //return base.minutesElapsed(minutes, environment);
        }

        public override void rebuild(Dictionary<string, string> additionalSaveData, object replacement)
        {
            base.rebuild(additionalSaveData, replacement);
        }

        public override bool rightClicked(Farmer who)
        {
            if (this.location == null)
                this.location = Game1.player.currentLocation;
            if (Game1.menuUp || Game1.currentMinigame != null) return false;

            //ModCore.playerInfo.sittingInfo.sit(this, Vector2.Zero);
            this.createMachineMenu();
            return true;
        }

        public override Item getOne()
        {
            Grinder component = new Grinder(this.data, this.info.Copy(), this.TileLocation, this.offsetKey, this.producedResources, ModCore.Configs.machinesConfig.grinderEnergyConsumption, this.timeToProduce, this.updatesContainerObjectForProduction, this.craftingRecipeBook);
            component.containerObject = this.containerObject;
            component.offsetKey = this.offsetKey;
            return component;
            return component;
        }

        public override ICustomObject recreate(Dictionary<string, string> additionalSaveData, object replacement)
        {
            Vector2 offsetKey = new Vector2(Convert.ToInt32(additionalSaveData["offsetKeyX"]), Convert.ToInt32(additionalSaveData["offsetKeyY"]));
            string GUID = additionalSaveData["GUID"];
            Grinder self = Revitalize.ModCore.Serializer.DeserializeGUID<Grinder>(additionalSaveData["GUID"]);
            if (ModCore.IsNullOrDefault<Machine>(self)) return null;
            try
            {
                if (!Revitalize.ModCore.ObjectGroups.ContainsKey(additionalSaveData["ParentGUID"]))
                {
                    MultiTiledObject obj = (MultiTiledObject)Revitalize.ModCore.Serializer.DeserializeGUID<MultiTiledObject>(additionalSaveData["ParentGUID"]);
                    self.containerObject = obj;
                    self.containerObject.removeComponent(offsetKey);
                    self.containerObject.addComponent(offsetKey, self);
                    Revitalize.ModCore.ObjectGroups.Add(additionalSaveData["ParentGUID"], obj);
                }
                else
                {
                    self.containerObject = Revitalize.ModCore.ObjectGroups[additionalSaveData["ParentGUID"]];
                    self.containerObject.removeComponent(offsetKey);
                    self.containerObject.addComponent(offsetKey, self);
                }
            }
            catch (Exception err)
            {
                ModCore.log(err);
            }

            return self;
        }

        public override Dictionary<string, string> getAdditionalSaveData()
        {
            Dictionary<string, string> saveData = base.getAdditionalSaveData();
            Revitalize.ModCore.Serializer.SerializeGUID(this.containerObject.childrenGuids[this.offsetKey].ToString(), this);
            this.containerObject.getAdditionalSaveData();
            return saveData;

        }

        public override void draw(SpriteBatch spriteBatch, int x, int y, float alpha = 1f)
        {
            this.updateInfo();

            if (this.info == null)
            {
                Revitalize.ModCore.log("info is null");
                if (this.syncObject == null) Revitalize.ModCore.log("DEAD SYNC");
            }
            if (this.animationManager == null) Revitalize.ModCore.log("Animation Manager Null");
            if (this.displayTexture == null) Revitalize.ModCore.log("Display texture is null");

            //The actual planter box being drawn.
            if (this.animationManager == null)
            {
                if (this.animationManager.getExtendedTexture() == null)
                    ModCore.ModMonitor.Log("Tex Extended is null???");

                spriteBatch.Draw(this.displayTexture, Game1.GlobalToLocal(Game1.viewport, new Vector2((float)(x * Game1.tileSize), y * Game1.tileSize)), new Rectangle?(this.animationManager.currentAnimation.sourceRectangle), this.info.DrawColor * alpha, 0f, Vector2.Zero, (float)Game1.pixelZoom, this.Flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None, Math.Max(0f, (float)(y * Game1.tileSize) / 10000f));
                // Log.AsyncG("ANIMATION IS NULL?!?!?!?!");
            }

            else
            {
                //Log.AsyncC("Animation Manager is working!");
                float addedDepth = 0;


                if (Revitalize.ModCore.playerInfo.sittingInfo.SittingObject == this.containerObject && this.info.facingDirection == Enums.Direction.Up)
                {
                    addedDepth += (this.containerObject.Height - 1) - ((int)(this.offsetKey.Y));
                    if (this.info.ignoreBoundingBox) addedDepth += 1.5f;
                }
                else if (this.info.ignoreBoundingBox)
                {
                    addedDepth += 1.0f;
                }
                this.animationManager.draw(spriteBatch, this.displayTexture, Game1.GlobalToLocal(Game1.viewport, new Vector2((float)(x * Game1.tileSize), y * Game1.tileSize)), new Rectangle?(this.animationManager.currentAnimation.sourceRectangle), this.info.DrawColor * alpha, 0f, Vector2.Zero, (float)Game1.pixelZoom, this.Flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None, Math.Max(0f, (float)((y + addedDepth) * Game1.tileSize) / 10000f) + .00001f);
                this.drawStatusBubble(spriteBatch, x, y, alpha);

                try
                {
                    if (this.animationManager.canTickAnimation())
                    {
                        this.animationManager.tickAnimation();
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
                    this.containerObject.MinutesUntilReady = ModCore.Configs.machinesConfig.grinderTimeToGrind;
                    break;
                }
                if (I.canStackWith(new StardewValley.Object((int)Enums.SDVObject.IronOre, 1)))
                {
                    this.GetInventoryManager().bufferItems.Add(ModCore.ObjectManager.resources.getResource("IronSand", 2));
                    itemToRemove = I;
                    this.containerObject.MinutesUntilReady = ModCore.Configs.machinesConfig.grinderTimeToGrind;
                    break;
                }
                if (I.canStackWith(new StardewValley.Object((int)Enums.SDVObject.GoldOre, 1)))
                {
                    this.GetInventoryManager().bufferItems.Add(ModCore.ObjectManager.resources.getResource("GoldSand", 2));
                    itemToRemove = I;
                    this.containerObject.MinutesUntilReady = ModCore.Configs.machinesConfig.grinderTimeToGrind;
                    break;
                }
                if (I.canStackWith(new StardewValley.Object((int)Enums.SDVObject.IridiumOre, 1)))
                {
                    this.GetInventoryManager().bufferItems.Add(ModCore.ObjectManager.resources.getResource("IridiumSand", 2));
                    itemToRemove = I;
                    this.containerObject.MinutesUntilReady = ModCore.Configs.machinesConfig.grinderTimeToGrind;
                    break;
                }

                if (I.canStackWith(ModCore.ObjectManager.resources.getOre("BauxiteOre")))
                {
                    this.GetInventoryManager().bufferItems.Add(ModCore.ObjectManager.resources.getResource("BauxiteSand", 2));
                    itemToRemove = I;
                    this.containerObject.MinutesUntilReady = ModCore.Configs.machinesConfig.grinderTimeToGrind;
                    break;
                }
                if (I.canStackWith(ModCore.ObjectManager.resources.getOre("LeadOre")))
                {
                    this.GetInventoryManager().bufferItems.Add(ModCore.ObjectManager.resources.getResource("LeadSand", 2));
                    itemToRemove = I;
                    this.containerObject.MinutesUntilReady = ModCore.Configs.machinesConfig.grinderTimeToGrind;
                    break;
                }
                if (I.canStackWith(ModCore.ObjectManager.resources.getOre("SilverOre")))
                {
                    this.GetInventoryManager().bufferItems.Add(ModCore.ObjectManager.resources.getResource("SilverSand", 2));
                    itemToRemove = I;
                    this.containerObject.MinutesUntilReady = ModCore.Configs.machinesConfig.grinderTimeToGrind;
                    break;
                }
                if (I.canStackWith(ModCore.ObjectManager.resources.getOre("TinOre")))
                {
                    this.GetInventoryManager().bufferItems.Add(ModCore.ObjectManager.resources.getResource("TinSand", 2));
                    itemToRemove = I;
                    this.containerObject.MinutesUntilReady = ModCore.Configs.machinesConfig.grinderTimeToGrind;
                    break;
                }
                if (I.canStackWith(ModCore.ObjectManager.resources.getOre("TitaniumOre")))
                {
                    this.GetInventoryManager().bufferItems.Add(ModCore.ObjectManager.resources.getResource("TitaniumSand", 2));
                    itemToRemove = I;
                    this.containerObject.MinutesUntilReady = ModCore.Configs.machinesConfig.grinderTimeToGrind;
                    break;
                }

                if (I.canStackWith(new StardewValley.Object((int)Enums.SDVObject.Stone, 1)))
                {
                    this.GetInventoryManager().bufferItems.Add(ModCore.ObjectManager.resources.getResource("Sand", 1));
                    itemToRemove = I;
                    this.containerObject.MinutesUntilReady = ModCore.Configs.machinesConfig.grinderTimeToGrind;
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
