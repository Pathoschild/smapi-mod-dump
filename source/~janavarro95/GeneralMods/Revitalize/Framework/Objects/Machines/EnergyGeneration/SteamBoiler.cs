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
using Revitalize.Framework.Utilities;
using StardewValley;

namespace Revitalize.Framework.Objects.Machines.EnergyGeneration
{
    public class SteamBoiler : Machine
    {

        public SteamBoiler() { }

        public SteamBoiler(CustomObjectData PyTKData, BasicItemInformation info, List<ResourceInformation> ProducedResources = null, int EnergyRequiredPer10Minutes = 0, int TimeToProduce = 0, bool UpdatesContainer = false, string CraftingBook = "") : base(PyTKData, info)
        {
            this.producedResources = ProducedResources ?? new List<ResourceInformation>();
            this.energyRequiredPer10Minutes = EnergyRequiredPer10Minutes;
            this.timeToProduce = TimeToProduce;
            this.updatesContainerObjectForProduction = UpdatesContainer;
            this.MinutesUntilReady = TimeToProduce;
            this.craftingRecipeBook = CraftingBook;
            this.createStatusBubble();

        }

        public SteamBoiler(CustomObjectData PyTKData, BasicItemInformation info, Vector2 TileLocation, List<ResourceInformation> ProducedResources = null, int EnergyRequiredPer10Minutes = 0, int TimeToProduce = 0, bool UpdatesContainer = false, string CraftingBook = "", MultiTiledObject obj = null) : base(PyTKData, info, TileLocation)
        {
            this.containerObject = obj;
            this.producedResources = ProducedResources ?? new List<ResourceInformation>();
            this.energyRequiredPer10Minutes = EnergyRequiredPer10Minutes;
            this.timeToProduce = TimeToProduce;
            this.updatesContainerObjectForProduction = UpdatesContainer;
            this.MinutesUntilReady = TimeToProduce;
            this.craftingRecipeBook = CraftingBook;
            this.createStatusBubble();
        }

        public SteamBoiler(CustomObjectData PyTKData, BasicItemInformation info, Vector2 TileLocation, Vector2 offsetKey, List<ResourceInformation> ProducedResources = null, int EnergyRequiredPer10Minutes = 0, int TimeToProduce = 0, bool UpdatesContainer = false, string CraftingBook = "", MultiTiledObject obj = null) : base(PyTKData, info, TileLocation)
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
        }

        public override void updateWhenCurrentLocation(GameTime time, GameLocation environment)
        {
            base.updateWhenCurrentLocation(time, environment);

            this.animationManager.prepareForNextUpdateTick();
        }


        public override bool minutesElapsed(int minutes, GameLocation environment)
        {
            this.updateInfo();

            if (this.containerObject.MinutesUntilReady > 0)
            {
                this.animationManager.playAnimation("Working");
            }
            else
            {
                this.animationManager.playDefaultAnimation();
            }
            this.pullFluidFromNetworkOutputs(ModCore.ObjectManager.resources.getFluid("Water"));

            if (this.updatesContainerObjectForProduction)
            {
                int remaining = minutes;
                if (this.containerObject.MinutesUntilReady <= 0)
                {
                    this.searchInventoryForBurnableObjects();
                }
                while (remaining > 0 && this.containerObject.MinutesUntilReady > 0)
                {
                    remaining -= 10;
                    this.processFluidLogic();
                    if (this.containerObject.MinutesUntilReady <= 0)
                    {
                        this.searchInventoryForBurnableObjects();
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


        public override Item getOne()
        {
            SteamBoiler component = new SteamBoiler(this.data, this.info.Copy(), this.producedResources, this.energyRequiredPer10Minutes, this.timeToProduce, this.updatesContainerObjectForProduction, this.craftingRecipeBook);
            //component.containerObject = this.containerObject;
            //component.offsetKey = this.offsetKey;
            return component;
            return component;
        }

        public override ICustomObject recreate(Dictionary<string, string> additionalSaveData, object replacement)
        {
            Vector2 offsetKey = new Vector2(Convert.ToInt32(additionalSaveData["offsetKeyX"]), Convert.ToInt32(additionalSaveData["offsetKeyY"]));
            string GUID = additionalSaveData["GUID"];
            SteamBoiler self = Revitalize.ModCore.Serializer.DeserializeGUID<SteamBoiler>(additionalSaveData["GUID"]);
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

        public override void produceEnergy()
        {
            /*
            if (this.GetEnergyManager().canReceieveEnergy)
            {
                this.GetEnergyManager().produceEnergy(this.energyRequiredPer10Minutes);
                this.containerObject.MinutesUntilReady -= 10;
            }
            */
        }

        public virtual void processFluidLogic()
        {
            if (this.GetFluidManager().doTheInputTanksHaveEnoughFluid(ModCore.ObjectManager.resources.getFluid("Water"), ModCore.Configs.machinesConfig.steamBoilerV1_requiredWaterPerOperation))
            {
                this.GetFluidManager().consumeFluid(ModCore.ObjectManager.resources.getFluid("Water"), ModCore.Configs.machinesConfig.steamBoilerV1_requiredWaterPerOperation);
                this.GetFluidManager().produceFluid(ModCore.ObjectManager.resources.getFluid("Steam"), ModCore.Configs.machinesConfig.steamBoilerV1_producedSteamPerOperation);
                this.containerObject.MinutesUntilReady -= 10;
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

        protected virtual void searchInventoryForBurnableObjects()
        {
            Item removed = null;
            foreach (Item I in this.GetInventoryManager().items)
            {
                if (ModCore.ObjectManager.resources.burnableObjects.ContainsKey(I.Name))
                {
                    this.containerObject.MinutesUntilReady = ModCore.ObjectManager.resources.burnableObjects[I.Name];
                    removed = I;
                    break;
                }
            }
            if (removed == null) return;
            if (removed.Stack == 1) this.GetInventoryManager().items.Remove(removed);
            else removed.Stack -= 1;
        }
    }
}
