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
using Revitalize.Framework.Crafting;
using Revitalize.Framework.Managers;
using Revitalize.Framework.Menus;
using Revitalize.Framework.Menus.Machines;
using Revitalize.Framework.Objects.InformationFiles;
using Revitalize.Framework.Utilities;
using StardewValley;
using StardewValley.Objects;
using StardustCore.Animations;
using StardustCore.UIUtilities;
using StardustCore.UIUtilities.MenuComponents.ComponentsV2.Buttons;

namespace Revitalize.Framework.Objects.Machines
{

    // Every machine needs a recreate, a rebuild, a get additional save data,and a getOne function.
    public class Machine : MultiTiledComponent
    {

        [JsonIgnore]
        public override string ItemInfo
        {
            get
            {
                string info = Revitalize.ModCore.Serializer.ToJSONString(this.info);
                string guidStr = this.guid.ToString();
                string pyTkData = ModCore.Serializer.ToJSONString(this.data);
                string offsetKey = this.offsetKey != null ? ModCore.Serializer.ToJSONString(this.offsetKey) : "";
                string container = this.containerObject != null ? this.containerObject.guid.ToString() : "";
                string energyRequired = this.energyRequiredPer10Minutes.ToString();
                string timeToProduce = this.timeToProduce.ToString();
                string updatesContainer = this.updatesContainerObjectForProduction.ToString();
                string fluidString = this.requiredFluidForOperation != null ? ModCore.Serializer.ToJSONString(this.amountOfFluidRequiredForOperation) : "";
                string fluidAmountString = this.amountOfFluidRequiredForOperation.ToString();
                StringBuilder b = new StringBuilder();
                b.Append(info);
                b.Append("<");
                b.Append(guidStr);
                b.Append("<");
                b.Append(pyTkData);
                b.Append("<");
                b.Append(offsetKey);
                b.Append("<");
                b.Append(container);
                b.Append("<");
                b.Append(energyRequired);
                b.Append("<");
                b.Append(timeToProduce);
                b.Append("<");
                b.Append(updatesContainer);
                b.Append("<");
                b.Append(this.craftingRecipeBook);
                b.Append("<");
                b.Append(fluidString);
                b.Append("<");
                b.Append(fluidAmountString);
                //ModCore.log("Setting info: " + b.ToString());
                return b.ToString();
            }
            set
            {
                if (string.IsNullOrEmpty(value)) return;
                string[] data = value.Split('<');
                string infoString = data[0];
                string guidString = data[1];
                string pyTKData = data[2];
                string offsetVec = data[3];
                string containerObject = data[4];
                string energyRequired = data[5];
                string time = data[6];
                string updates = data[7];
                this.craftingRecipeBook = data[8];
                string fluid = data[9];
                string fluidAmount = data[10];
                this.info = (BasicItemInformation)Revitalize.ModCore.Serializer.DeserializeFromJSONString(infoString, typeof(BasicItemInformation));
                this.data = Revitalize.ModCore.Serializer.DeserializeFromJSONString<CustomObjectData>(pyTKData);
                this.energyRequiredPer10Minutes = Convert.ToInt32(energyRequired);
                this.timeToProduce = Convert.ToInt32(time);
                this.updatesContainerObjectForProduction = Convert.ToBoolean(updates);
                if (string.IsNullOrEmpty(fluid) == false)
                {
                    this.requiredFluidForOperation = ModCore.Serializer.DeserializeFromJSONString<Fluid>(fluid);
                }
                this.amountOfFluidRequiredForOperation= Convert.ToInt32(fluidAmount);
                if (string.IsNullOrEmpty(offsetVec)) return;
                if (string.IsNullOrEmpty(containerObject)) return;
                this.offsetKey = ModCore.Serializer.DeserializeFromJSONString<Vector2>(offsetVec);
                Guid oldGuid = this.guid;
                this.guid = Guid.Parse(guidString);
                if (ModCore.CustomObjects.ContainsKey(this.guid))
                {
                    //ModCore.log("Update item with guid: " + this.guid);
                    ModCore.CustomObjects[this.guid] = this;
                }
                else
                {
                    //ModCore.log("Add in new guid: " + this.guid);
                    ModCore.CustomObjects.Add(this.guid, this);
                }

                if (this.containerObject == null)
                {
                    //ModCore.log(containerObject);
                    Guid containerGuid = Guid.Parse(containerObject);
                    if (ModCore.CustomObjects.ContainsKey(containerGuid))
                    {
                        this.containerObject = (MultiTiledObject)ModCore.CustomObjects[containerGuid];
                        this.containerObject.removeComponent(this.offsetKey);
                        this.containerObject.addComponent(this.offsetKey, this);
                        //ModCore.log("Set container object from existing object!");
                    }
                    else
                    {
                        //ModCore.log("Container hasn't been synced???");
                        MultiplayerUtilities.RequestGuidObject(containerGuid);
                        MultiplayerUtilities.RequestGuidObject_Tile(this.guid);
                    }
                }
                else
                {
                    this.containerObject.updateInfo();
                }

                if (ModCore.CustomObjects.ContainsKey(oldGuid) && ModCore.CustomObjects.ContainsKey(this.guid))
                {
                    if (ModCore.CustomObjects[oldGuid] == ModCore.CustomObjects[this.guid] && oldGuid != this.guid)
                    {
                        //ModCore.CustomObjects.Remove(oldGuid);
                    }
                }
            }
        }

        public List<ResourceInformation> producedResources
        {
            get
            {
                return MachineUtilities.GetResourcesProducedByThisMachine(this.info.id);
            }
            set
            {
                if (MachineUtilities.ResourcesForMachines == null) MachineUtilities.InitializeResourceList();
                if (MachineUtilities.ResourcesForMachines.ContainsKey(this.info.id)) return;
                MachineUtilities.ResourcesForMachines.Add(this.info.id, value);
            }
        }
        public int energyRequiredPer10Minutes;
        public int timeToProduce;
        public bool updatesContainerObjectForProduction;

        public string craftingRecipeBook;

        [JsonIgnore]
        protected AnimationManager machineStatusBubbleBox;

        [JsonIgnore]
        public bool ProducesItems
        {
            get
            {
                return this.producedResources.Count > 0;
            }
        }

        public Fluid requiredFluidForOperation;
        public int amountOfFluidRequiredForOperation;

        [JsonIgnore]
        public bool ConsumesEnergy
        {
            get
            {
                this.updateInfo();
                if (ModCore.Configs.machinesConfig.doMachinesConsumeEnergy == false)
                {
                    //ModCore.log("Machine config disables energy consumption.");
                    return false;
                }
                if (this.energyRequiredPer10Minutes == 0)
                {
                    //ModCore.log("Machine rquires 0 energy to run.");
                    return false;
                }
                if (this.GetEnergyManager().energyInteractionType == Enums.EnergyInteractionType.Consumes)
                {
                    //ModCore.log("Machine does consume energy.");
                    return true;
                }
                if (this.GetEnergyManager().energyInteractionType == Enums.EnergyInteractionType.Storage)
                {

                    return true;
                }
                //ModCore.log("Unknown energy configuration.");
                return false;
            }
        }

        public Machine() { }

        public Machine(CustomObjectData PyTKData, BasicItemInformation info, List<ResourceInformation> ProducedResources = null, int EnergyRequiredPer10Minutes = 0, int TimeToProduce = 0, bool UpdatesContainer = false, string CraftingBook = "", Fluid FluidRequiredForOperation = null, int FluidAmountRequiredPerOperation =0) : base(PyTKData, info)
        {
            this.producedResources = ProducedResources ?? new List<ResourceInformation>();
            this.energyRequiredPer10Minutes = EnergyRequiredPer10Minutes;
            this.timeToProduce = TimeToProduce;
            this.updatesContainerObjectForProduction = UpdatesContainer;
            this.MinutesUntilReady = TimeToProduce;
            this.craftingRecipeBook = CraftingBook;
            this.createStatusBubble();
            this.requiredFluidForOperation = FluidRequiredForOperation;
            this.amountOfFluidRequiredForOperation = FluidAmountRequiredPerOperation;
        }

        public Machine(CustomObjectData PyTKData, BasicItemInformation info, Vector2 TileLocation, List<ResourceInformation> ProducedResources = null, int EnergyRequiredPer10Minutes = 0, int TimeToProduce = 0, bool UpdatesContainer = false, string CraftingBook = "", MultiTiledObject obj = null, Fluid FluidRequiredForOperation = null, int FluidAmountRequiredPerOperation =0) : base(PyTKData, info, TileLocation)
        {
            this.containerObject = obj;
            this.producedResources = ProducedResources ?? new List<ResourceInformation>();
            this.energyRequiredPer10Minutes = EnergyRequiredPer10Minutes;
            this.timeToProduce = TimeToProduce;
            this.updatesContainerObjectForProduction = UpdatesContainer;
            this.MinutesUntilReady = TimeToProduce;
            this.craftingRecipeBook = CraftingBook;
            this.createStatusBubble();
            this.requiredFluidForOperation = FluidRequiredForOperation;
            this.amountOfFluidRequiredForOperation = FluidAmountRequiredPerOperation;
        }

        public Machine(CustomObjectData PyTKData, BasicItemInformation info, Vector2 TileLocation, Vector2 offsetKey, List<ResourceInformation> ProducedResources = null, int EnergyRequiredPer10Minutes = 0, int TimeToProduce = 0, bool UpdatesContainer = false, string CraftingBook = "", MultiTiledObject obj = null, Fluid FluidRequiredForOperation = null, int FluidAmountRequiredPerOperation =0) : base(PyTKData, info, TileLocation)
        {
            this.offsetKey = offsetKey;
            this.containerObject = obj;
            this.producedResources = ProducedResources ?? new List<ResourceInformation>();
            this.energyRequiredPer10Minutes = EnergyRequiredPer10Minutes;
            this.timeToProduce = TimeToProduce;
            this.updatesContainerObjectForProduction = UpdatesContainer;
            this.MinutesUntilReady = TimeToProduce;
            this.craftingRecipeBook = CraftingBook;
            this.createStatusBubble();
            this.requiredFluidForOperation = FluidRequiredForOperation;
            this.amountOfFluidRequiredForOperation = FluidAmountRequiredPerOperation;
        }

        protected virtual void createStatusBubble()
        {
            this.machineStatusBubbleBox = new AnimationManager(TextureManager.GetExtendedTexture(ModCore.Manifest, "HUD", "MachineStatusBubble"), new Animation(0, 0, 20, 24), new Dictionary<string, List<Animation>>()
            {
                {"Default",new List<Animation>(){new Animation(0,0,20,24)}},
                {"Empty",new List<Animation>(){new Animation(20,0,20,24)}},
                {"InventoryFull",new List<Animation>(){new Animation(40,0,20,24)}}
            }, "Default", 0);
        }

        public override void updateWhenCurrentLocation(GameTime time, GameLocation environment)
        {
            base.updateWhenCurrentLocation(time, environment);

            this.animationManager.prepareForNextUpdateTick();
        }


        public override bool minutesElapsed(int minutes, GameLocation environment)
        {

            this.updateInfo();
            //ModCore.log(this.info.animationManager.currentAnimationName);

            if (this.updatesContainerObjectForProduction)
            {
                //ModCore.log("Update container object for production!");
                //this.MinutesUntilReady -= minutes;
                int remaining = minutes;
                //ModCore.log("Minutes elapsed: " + remaining);
                List<MultiTiledObject> energySources = new List<MultiTiledObject>();
                if (this.ConsumesEnergy || this.GetEnergyManager().energyInteractionType == Enums.EnergyInteractionType.Storage)
                {
                    //ModCore.log("This machine drains energy: " + this.info.name);
                    energySources = this.EnergyGraphSearchSources(); //Only grab the network once.
                }

                if (this.ProducesItems)
                {
                    while (remaining > 0)
                    {

                        if (this.ConsumesEnergy)
                        {
                            this.drainEnergyFromNetwork(energySources); //Continually drain from the network.                        
                            if (this.GetEnergyManager().remainingEnergy < this.energyRequiredPer10Minutes) return false;
                            else
                            {
                                this.GetEnergyManager().consumeEnergy(this.energyRequiredPer10Minutes); //Consume the required amount of energy necessary.
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
                            this.containerObject.MinutesUntilReady = this.timeToProduce;
                        }
                    }
                }
                if (this.GetEnergyManager().energyInteractionType == Enums.EnergyInteractionType.Produces)
                {
                    while (remaining > 0)
                    {
                        remaining -= 10;
                        this.produceEnergy();
                        this.storeEnergyToNetwork();
                    }
                }
                if (this.containerObject.MinutesUntilReady > 0)
                {
                    this.containerObject.MinutesUntilReady = Math.Max(0, this.MinutesUntilReady - minutes);

                    if (this.GetInventoryManager().hasItemsInBuffer && this.MinutesUntilReady == 0)
                    {
                        this.GetInventoryManager().dumpBufferToItems();
                    }

                }

                return false;
            }
            else
            {

                if (this.GetEnergyManager().energyInteractionType == Enums.EnergyInteractionType.Produces)
                {
                    this.storeEnergyToNetwork();
                }

                return false;
            }

            //return base.minutesElapsed(minutes, environment);
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

        /// <summary>
        /// Creates the necessary components to display the machine menu properly.
        /// </summary>
        protected virtual void createMachineMenu()
        {
            MachineMenu machineMenu = new MachineMenu((Game1.viewport.Width / 2) - 400, 0, 800, 600);

            MachineSummaryMenu m = new Framework.Menus.Machines.MachineSummaryMenu((Game1.viewport.Width / 2) - 400, 48, 800, 600, Color.White, this.containerObject, this.energyRequiredPer10Minutes);
            machineMenu.addInMenuTab("Summary", new AnimatedButton(new StardustCore.Animations.AnimatedSprite("SummaryTab", new Vector2(), new AnimationManager(TextureManager.GetExtendedTexture(ModCore.Manifest, "Menus", "MenuTab"), new Animation(0, 0, 24, 24)), Color.White), new Rectangle(0, 0, 24, 24), 2f), m, true);

            if (this.GetInventoryManager().capacity > 0)
            {
                InventoryTransferMenu transferMenu = new InventoryTransferMenu(100, 150, 500, 600, this.GetInventoryManager().items, this.GetInventoryManager().capacity, this.GetInventoryManager().displayRows, this.GetInventoryManager().displayColumns);
                machineMenu.addInMenuTab("Inventory", new AnimatedButton(new StardustCore.Animations.AnimatedSprite("Inventory Tab", new Vector2(), new AnimationManager(TextureManager.GetExtendedTexture(ModCore.Manifest, "Menus", "MenuTab"), new Animation(0, 0, 24, 24)), Color.White), new Rectangle(0, 0, 24, 24), 2f), transferMenu, false);
            }

            if (string.IsNullOrEmpty(this.craftingRecipeBook) == false)
            {
                CraftingMenuV1 craftingMenu = CraftingRecipeBook.CraftingRecipesByGroup[this.craftingRecipeBook].getCraftingMenuForMachine(100, 100, 400, 700, ref this.GetInventoryManager().items, ref this.GetInventoryManager().bufferItems, this);
                machineMenu.addInMenuTab("Crafting", new AnimatedButton(new StardustCore.Animations.AnimatedSprite("Crafting Tab", new Vector2(), new AnimationManager(TextureManager.GetExtendedTexture(ModCore.Manifest, "Menus", "MenuTab"), new Animation(0, 0, 24, 24)), Color.White), new Rectangle(0, 0, 24, 24), 2f), craftingMenu, false);
            }

            if (Game1.activeClickableMenu == null) Game1.activeClickableMenu = machineMenu;
        }

        public override Item getOne()
        {
            Machine component = new Machine(this.data, this.info.Copy(), this.producedResources, this.energyRequiredPer10Minutes, this.timeToProduce, this.updatesContainerObjectForProduction, this.craftingRecipeBook,this.requiredFluidForOperation,this.amountOfFluidRequiredForOperation);
            component.containerObject = this.containerObject;
            component.offsetKey = this.offsetKey;
            return component;
            return component;
        }

        public override ICustomObject recreate(Dictionary<string, string> additionalSaveData, object replacement)
        {
            Vector2 offsetKey = new Vector2(Convert.ToInt32(additionalSaveData["offsetKeyX"]), Convert.ToInt32(additionalSaveData["offsetKeyY"]));
            string GUID = additionalSaveData["GUID"];
            Machine self = Revitalize.ModCore.Serializer.DeserializeGUID<Machine>(additionalSaveData["GUID"]);
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

        public override void rebuild(Dictionary<string, string> additionalSaveData, object replacement)
        {
            base.rebuild(additionalSaveData, replacement);
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

                spriteBatch.Draw(this.displayTexture, Game1.GlobalToLocal(Game1.viewport, new Vector2((float)(x * Game1.tileSize), y * Game1.tileSize)), new Rectangle?(this.animationManager.currentAnimation.sourceRectangle), this.info.DrawColor * alpha, 0f, Vector2.Zero, (float)Game1.pixelZoom, this.flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None, Math.Max(0f, (float)(y * Game1.tileSize) / 10000f));
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
                this.animationManager.draw(spriteBatch, this.displayTexture, Game1.GlobalToLocal(Game1.viewport, new Vector2((float)(x * Game1.tileSize), y * Game1.tileSize)), new Rectangle?(this.animationManager.currentAnimation.sourceRectangle), this.info.DrawColor * alpha, 0f, Vector2.Zero, (float)Game1.pixelZoom, this.flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None, Math.Max(0f, (float)((y + addedDepth) * Game1.tileSize) / 10000f) + .00001f);
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

        public virtual void produceEnergy()
        {
            if (this.GetEnergyManager().canReceieveEnergy)
            {
                this.GetEnergyManager().produceEnergy(this.energyRequiredPer10Minutes);
            }

        }

        public virtual void produceEnergy(double ratio)
        {

            if (this.GetEnergyManager().canReceieveEnergy)
            {
                this.GetEnergyManager().produceEnergy((int)(this.energyRequiredPer10Minutes * ratio));
            }

        }

        protected virtual void drawStatusBubble(SpriteBatch b, int x, int y, float Alpha)
        {
            if (this.updatesContainerObjectForProduction == false) return;
            if (this.machineStatusBubbleBox == null) this.createStatusBubble();
            this.updateInfo();
            if (this.GetInventoryManager() == null) return;
            if (this.GetInventoryManager().IsFull && this.ProducesItems && ModCore.Configs.machinesConfig.showMachineNotificationBubble_InventoryFull)
            {
                y--;
                float num = (float)(4.0 * Math.Round(Math.Sin(DateTime.UtcNow.TimeOfDay.TotalMilliseconds / 250.0), 2));
                this.machineStatusBubbleBox.playAnimation("InventoryFull");
                this.machineStatusBubbleBox.draw(b, this.machineStatusBubbleBox.getTexture(), Game1.GlobalToLocal(Game1.viewport, new Vector2((float)(x * Game1.tileSize), y * Game1.tileSize + num)), new Rectangle?(this.machineStatusBubbleBox.currentAnimation.sourceRectangle), Color.White * ModCore.Configs.machinesConfig.machineNotificationBubbleAlpha, 0f, Vector2.Zero, (float)Game1.pixelZoom, SpriteEffects.None, Math.Max(0f, (float)((y + 2) * Game1.tileSize) / 10000f) + .00001f);
            }
            else
            {

            }
        }

        public override void updateInfo()
        {
            return;
            if (this.info == null || this.containerObject == null)
            {
                this.ItemInfo = this.text;
                //ModCore.log("Updated item info!");
                return;
            }

            if (this.requiresUpdate())
            {
                //this.ItemInfo = this.text;
                this.text = this.ItemInfo;
                this.info.cleanAfterUpdate();
                //this.containerObject.updateInfo();
                //ModCore.log("Force an update for machine: " + this.info.name);
                MultiplayerUtilities.RequestUpdateSync(this.guid);
            }
        }

        public override Dictionary<string, string> getAdditionalSaveData()
        {
            Dictionary<string, string> saveData = base.getAdditionalSaveData();
            Revitalize.ModCore.Serializer.SerializeGUID(this.containerObject.childrenGuids[this.offsetKey].ToString(), this);
            this.containerObject.getAdditionalSaveData();
            return saveData;

        }


    }
}
