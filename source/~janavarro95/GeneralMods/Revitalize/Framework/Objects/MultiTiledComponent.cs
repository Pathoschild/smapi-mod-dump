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
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using PyTK.CustomElementHandler;
using Revitalize.Framework.Energy;
using Revitalize.Framework.Managers;
using Revitalize.Framework.Utilities;
using StardewValley;

namespace Revitalize.Framework.Objects
{
    public class MultiTiledComponent : CustomObject, ISaveElement
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
                return info + "<" + guidStr + "<" + pyTkData + "<" + offsetKey + "<" + container;
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
                this.info = (BasicItemInformation)Revitalize.ModCore.Serializer.DeserializeFromJSONString(infoString, typeof(BasicItemInformation));
                this.data = Revitalize.ModCore.Serializer.DeserializeFromJSONString<CustomObjectData>(pyTKData);
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

        public MultiTiledObject containerObject;

        public Vector2 offsetKey;


        public MultiTiledComponent() { }

        public MultiTiledComponent(CustomObjectData PyTKData, BasicItemInformation info) : base(PyTKData, info) { }

        public MultiTiledComponent(CustomObjectData PyTKData, BasicItemInformation info, Vector2 TileLocation, MultiTiledObject obj = null) : base(PyTKData, info, TileLocation)
        {
            this.containerObject = obj;
        }

        public MultiTiledComponent(CustomObjectData PyTKData, BasicItemInformation info, Vector2 TileLocation, Vector2 offsetKey, MultiTiledObject obj = null) : base(PyTKData, info, TileLocation)
        {
            this.offsetKey = offsetKey;
            this.containerObject = obj;
        }

        public override void updateWhenCurrentLocation(GameTime time, GameLocation environment)
        {
            base.updateWhenCurrentLocation(time, environment);
        }
        public override bool isPassable()
        {
            return this.info.ignoreBoundingBox || Revitalize.ModCore.playerInfo.sittingInfo.SittingObject == this.containerObject;
        }

        public override bool checkForAction(Farmer who, bool justCheckingForActivity = false)
        {
            //ModCore.log("Checking for a clicky click???");
            return base.checkForAction(who, justCheckingForActivity);
        }

        public override bool canBePlacedHere(GameLocation l, Vector2 tile)
        {
            return base.canBePlacedHere(l, tile);
        }



        public override bool clicked(Farmer who)
        {
            //ModCore.log("Clicked a multiTiledComponent!");
            if (ModCore.playerInfo.justPlacedACustomObject == true) return false;
            if (PlayerUtilities.CanPlayerInventoryReceiveThisItem(this.containerObject))
            {
                this.containerObject.pickUp(who);
            }
            else
            {
                return false;
            }
            return true;
            //return base.clicked(who);
        }

        public override bool rightClicked(Farmer who)
        {
            if (this.location == null)
                this.location = Game1.player.currentLocation;

            //ModCore.playerInfo.sittingInfo.sit(this, Vector2.Zero);

            return true;
        }



        public override void performRemoveAction(Vector2 tileLocation, GameLocation environment)
        {
            this.location = null;
            base.performRemoveAction(this.TileLocation, environment);
        }

        public virtual void removeFromLocation(GameLocation location, Vector2 offset)
        {
            this.cleanUpLights();
            location.removeObject(this.TileLocation, false);
            this.location = null;
            //this.performRemoveAction(this.TileLocation,location);
        }

        public virtual void cleanUpLights()
        {
            if (this.info.lightManager != null) this.info.lightManager.removeForCleanUp(this.location);
        }

        /// <summary>Places an object down.</summary>
        public override bool placementAction(GameLocation location, int x, int y, Farmer who = null)
        {
            this.updateInfo();
            this.updateDrawPosition(x, y);
            this.location = location;

            if (this.location == null) this.location = Game1.player.currentLocation;
            this.TileLocation = new Vector2((int)(x / Game1.tileSize), (int)(y / Game1.tileSize));
            //ModCore.log("TileLocation: " + this.TileLocation);
            /*
            return base.placementAction(location, x, y, who);
            */
            //this.updateLightManager();

            this.performDropDownAction(who);
            location.objects.Add(this.TileLocation, this);



            if (this.getBoundingBox(this.TileLocation).Width == 0 && this.getBoundingBox(this.TileLocation).Height == 0)
            {
                this.boundingBox.Value = new Rectangle(this.boundingBox.X, this.boundingBox.Y, Game1.tileSize, Game1.tileSize);
            }
            //ModCore.log(this.getBoundingBox(this.TileLocation));

            return true;
        }

        public override void drawInMenu(SpriteBatch spriteBatch, Vector2 location, float scaleSize, float transparency, float layerDepth, StackDrawType drawStackNumber, Color color, bool drawShadow)
        {
            bool shouldDrawStackNumber = drawStackNumber.ShouldDrawFor(this);

            this.updateInfo();
            if (shouldDrawStackNumber && this.maximumStackSize() > 1 && ((double)scaleSize > 0.3 && this.Stack != int.MaxValue))
                Utility.drawTinyDigits(this.Stack, spriteBatch, location + new Vector2((float)(Game1.tileSize - Utility.getWidthOfTinyDigitString(this.Stack, 3f * scaleSize)) + 3f * scaleSize, (float)((double)Game1.tileSize - 18.0 * (double)scaleSize + 2.0)), 3f * scaleSize, 1f, Color.White);
            if (shouldDrawStackNumber && this.Quality > 0)
            {
                float num = this.Quality < 4 ? 0.0f : (float)((Math.Cos((double)Game1.currentGameTime.TotalGameTime.Milliseconds * Math.PI / 512.0) + 1.0) * 0.0500000007450581);
                spriteBatch.Draw(Game1.mouseCursors, location + new Vector2(12f, (float)(Game1.tileSize - 12) + num), new Microsoft.Xna.Framework.Rectangle?(this.Quality < 4 ? new Microsoft.Xna.Framework.Rectangle(338 + (this.Quality - 1) * 8, 400, 8, 8) : new Microsoft.Xna.Framework.Rectangle(346, 392, 8, 8)), Color.White * transparency, 0.0f, new Vector2(4f, 4f), (float)(3.0 * (double)scaleSize * (1.0 + (double)num)), SpriteEffects.None, layerDepth);
            }

            spriteBatch.Draw(this.displayTexture, location, new Rectangle?(this.animationManager.currentAnimation.sourceRectangle), this.info.DrawColor * transparency, 0f, new Vector2((float)(this.animationManager.currentAnimation.sourceRectangle.Width / 2), (float)(this.animationManager.currentAnimation.sourceRectangle.Height)), scaleSize, SpriteEffects.None, layerDepth);
        }

        public override Item getOne()
        {
            MultiTiledComponent component = new MultiTiledComponent(this.data, this.info.Copy(), this.TileLocation, this.offsetKey, this.containerObject);
            return component;
        }

        public override object getReplacement()
        {
            return base.getReplacement();
        }

        public override ICustomObject recreate(Dictionary<string, string> additionalSaveData, object replacement)
        {
            //instead of using this.offsetkey.x use get additional save data function and store offset key there

            Vector2 offsetKey = new Vector2(Convert.ToInt32(additionalSaveData["offsetKeyX"]), Convert.ToInt32(additionalSaveData["offsetKeyY"]));
            MultiTiledComponent self = Revitalize.ModCore.Serializer.DeserializeGUID<MultiTiledComponent>(additionalSaveData["GUID"]);
            if (self == null)
            {
                return null;
            }

            if (!Revitalize.ModCore.ObjectGroups.ContainsKey(additionalSaveData["ParentGUID"]))
            {
                //Get new container
                MultiTiledObject obj = (MultiTiledObject)Revitalize.ModCore.Serializer.DeserializeGUID<MultiTiledObject>(additionalSaveData["ParentGUID"]);
                self.containerObject = obj;
                obj.addComponent(offsetKey, self);
                //Revitalize.ModCore.log("ADD IN AN OBJECT!!!!");
                Revitalize.ModCore.ObjectGroups.Add(additionalSaveData["ParentGUID"], (MultiTiledObject)obj);
            }
            else
            {
                self.containerObject = Revitalize.ModCore.ObjectGroups[additionalSaveData["ParentGUID"]];
                Revitalize.ModCore.ObjectGroups[additionalSaveData["GUID"]].addComponent(offsetKey, self);
                //Revitalize.ModCore.log("READD AN OBJECT!!!!");
            }

            return (ICustomObject)self;
        }

        public override Dictionary<string, string> getAdditionalSaveData()
        {
            Dictionary<string, string> saveData = base.getAdditionalSaveData();
            saveData.Add("ParentID", this.containerObject.info.id);
            saveData.Add("offsetKeyX", this.offsetKey.X.ToString());
            saveData.Add("offsetKeyY", this.offsetKey.Y.ToString());
            string saveLocation = "";
            if (this.location == null)
            {
                //Revitalize.ModCore.log("WHY IS LOCTION NULL???");
                saveLocation = "";
            }
            else
            {
                if (!string.IsNullOrEmpty(this.location.uniqueName.Value)) saveLocation = this.location.uniqueName.Value;
                else
                {
                    saveLocation = this.location.Name;
                }
            }


            saveData.Add("GameLocationName", saveLocation);
            saveData.Add("Rotation", ((int)this.info.facingDirection).ToString());

            saveData.Add("ParentGUID", this.containerObject.guid.ToString());
            saveData.Add("GUID", this.guid.ToString());


            if (this.containerObject.childrenGuids.ContainsKey(this.offsetKey))
            {
                Revitalize.ModCore.Serializer.SerializeGUID(this.containerObject.childrenGuids[this.offsetKey].ToString(), this);
            }


            this.containerObject.getAdditionalSaveData();
            return saveData;

        }

        protected string recreateParentId(string id)
        {
            StringBuilder b = new StringBuilder();
            string[] splits = id.Split('.');
            for (int i = 0; i < splits.Length - 1; i++)
            {
                b.Append(splits[i]);
                if (i == splits.Length - 2) continue;
                b.Append(".");
            }
            return b.ToString();
        }

        /// <summary>What happens when the object is drawn at a tile location.</summary>
        public override void draw(SpriteBatch spriteBatch, int x, int y, float alpha = 1f)
        {
            this.updateInfo();
            if (this.info.ignoreBoundingBox == true)
            {
                x *= -1;
                y *= -1;
            }

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
                try
                {
                    this.animationManager.tickAnimation();
                    // Log.AsyncC("Tick animation");
                }
                catch (Exception err)
                {
                    ModCore.ModMonitor.Log(err.ToString());
                }
            }

            // spriteBatch.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2((float)((double)tileLocation.X * (double)Game1.tileSize + (((double)tileLocation.X * 11.0 + (double)tileLocation.Y * 7.0) % 10.0 - 5.0)) + (float)(Game1.tileSize / 2), (float)((double)tileLocation.Y * (double)Game1.tileSize + (((double)tileLocation.Y * 11.0 + (double)tileLocation.X * 7.0) % 10.0 - 5.0)) + (float)(Game1.tileSize / 2))), new Rectangle?(new Rectangle((int)((double)tileLocation.X * 51.0 + (double)tileLocation.Y * 77.0) % 3 * 16, 128 + this.whichForageCrop * 16, 16, 16)), Color.White, 0.0f, new Vector2(8f, 8f), (float)Game1.pixelZoom, SpriteEffects.None, (float)(((double)tileLocation.Y * (double)Game1.tileSize + (double)(Game1.tileSize / 2) + (((double)tileLocation.Y * 11.0 + (double)tileLocation.X * 7.0) % 10.0 - 5.0)) / 10000.0));

        }

        public override void drawFullyInMenu(SpriteBatch spriteBatch, Vector2 objectPosition, float Depth)
        {
            this.updateInfo();
            if (this.animationManager == null)
            {
                Revitalize.ModCore.log("Animation Manager Null");
            }
            if (this.displayTexture == null) Revitalize.ModCore.log("Display texture is null");

            spriteBatch.Draw(this.displayTexture, objectPosition, this.animationManager.currentAnimation.sourceRectangle, this.info.DrawColor, 0f, Vector2.Zero, (float)Game1.pixelZoom, SpriteEffects.None, Depth);
            //base.drawWhenHeld(spriteBatch, objectPosition, f);
        }

        public override void updateInfo()
        {
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
                MultiplayerUtilities.RequestUpdateSync(this.guid);
            }
        }




        /// <summary>
        /// Gets a list of neighboring tiled objects that produce or transfer energy. This should be used for machines/objects that consume or transfer energy
        /// </summary>
        /// <returns></returns>
        protected virtual List<MultiTiledComponent> GetNeighboringOutputEnergySources()
        {
            Vector2 tileLocation = this.TileLocation;
            List<MultiTiledComponent> customObjects = new List<MultiTiledComponent>();
            if (this.location != null)
            {
                for (int i = -1; i <= 1; i++)
                {
                    for (int j = -1; j <= 1; j++)
                    {
                        if (i == j || i == (-j)) continue;

                        Vector2 neighborTile = tileLocation + new Vector2(i, j);
                        if (this.location.isObjectAtTile((int)neighborTile.X, (int)neighborTile.Y))
                        {
                            StardewValley.Object obj = this.location.getObjectAtTile((int)neighborTile.X, (int)neighborTile.Y);
                            if (obj is MultiTiledComponent)
                            {
                                if ((obj as MultiTiledComponent).GetEnergyManager().energyInteractionType == Enums.EnergyInteractionType.Produces || (obj as MultiTiledComponent).GetEnergyManager().energyInteractionType == Enums.EnergyInteractionType.Transfers || (obj as MultiTiledComponent).GetEnergyManager().energyInteractionType == Enums.EnergyInteractionType.Storage)
                                {
                                    if ((obj as MultiTiledComponent).containerObject == this.containerObject) continue;
                                    customObjects.Add((MultiTiledComponent)obj);
                                }
                            }
                            else continue;
                        }
                    }
                }
            }


            return customObjects;

        }

        /// <summary>
        /// Gets a list of neighboring tiled objects that consume or transfer energy. This should be used for machines/objects that produce or transfer energy
        /// </summary>
        /// <returns></returns>
        protected virtual List<MultiTiledComponent> GetNeighboringInputEnergySources()
        {
            Vector2 tileLocation = this.TileLocation;
            List<MultiTiledComponent> customObjects = new List<MultiTiledComponent>();
            if (this.location != null)
            {
                for (int i = -1; i <= 1; i++)
                {
                    for (int j = -1; j <= 1; j++)
                    {
                        if (i == j || i == (-j)) continue;

                        Vector2 neighborTile = tileLocation + new Vector2(i, j);
                        if (this.location.isObjectAtTile((int)neighborTile.X, (int)neighborTile.Y))
                        {
                            StardewValley.Object obj = this.location.getObjectAtTile((int)neighborTile.X, (int)neighborTile.Y);
                            if (obj is MultiTiledComponent)
                            {
                                if ((obj as MultiTiledComponent).GetEnergyManager().energyInteractionType == Enums.EnergyInteractionType.Consumes || (obj as MultiTiledComponent).GetEnergyManager().energyInteractionType == Enums.EnergyInteractionType.Transfers || (obj as MultiTiledComponent).GetEnergyManager().energyInteractionType == Enums.EnergyInteractionType.Storage)
                                {
                                    if ((obj as MultiTiledComponent).containerObject == this.containerObject) continue;
                                    customObjects.Add((MultiTiledComponent)obj);
                                }
                            }
                            else continue;
                        }
                    }
                }
            }


            return customObjects;
        }

        /// <summary>
        /// Gets the appropriate energy neighbors to move energy around from/to.
        /// </summary>
        /// <returns></returns>
        protected virtual List<MultiTiledComponent> getAppropriateEnergyNeighbors()
        {
            if (this.GetEnergyManager().consumesEnergy)
            {
                return this.GetNeighboringOutputEnergySources();
            }
            else if (this.GetEnergyManager().producesEnergy)
            {
                return this.GetNeighboringInputEnergySources();
            }
            else if (this.GetEnergyManager().transfersEnergy)
            {
                List<MultiTiledComponent> objs = new List<MultiTiledComponent>();
                objs.AddRange(this.GetNeighboringInputEnergySources());
                objs.AddRange(this.GetNeighboringOutputEnergySources());
                return objs;
            }
            return new List<MultiTiledComponent>();
        }

        /// <summary>
        /// Gets all of the energy nodes in a network that are either producers or is storage.
        /// </summary>
        /// <returns></returns>
        protected virtual List<MultiTiledObject> EnergyGraphSearchSources()
        {
            List<MultiTiledObject> energySources = new List<MultiTiledObject>();
            List<MultiTiledComponent> searchedComponents = new List<MultiTiledComponent>();
            List<MultiTiledComponent> entitiesToSearch = new List<MultiTiledComponent>();
            entitiesToSearch.AddRange(this.getAppropriateEnergyNeighbors());
            searchedComponents.Add(this);
            while (entitiesToSearch.Count > 0)
            {
                MultiTiledComponent searchComponent = entitiesToSearch[0];
                entitiesToSearch.Remove(searchComponent);
                if (searchedComponents.Contains(searchComponent))
                {
                    continue;
                }
                else
                {
                    searchedComponents.Add(searchComponent);
                    entitiesToSearch.AddRange(searchComponent.getAppropriateEnergyNeighbors());

                    if (searchComponent.containerObject.info.EnergyManager.energyInteractionType == Enums.EnergyInteractionType.Produces || searchComponent.containerObject.info.EnergyManager.energyInteractionType == Enums.EnergyInteractionType.Storage)
                    {
                        energySources.Add(searchComponent.containerObject);
                    }
                }

            }
            return energySources;
        }

        /// <summary>
        /// Gets all of the energy nodes in a network that are either consumers or storage. This should ALWAYS be ran after EnergyGraphSearchSources
        /// </summary>
        /// <returns></returns>
        protected virtual List<MultiTiledObject> EnergyGraphSearchConsumers()
        {
            List<MultiTiledObject> energySources = new List<MultiTiledObject>();
            List<MultiTiledComponent> searchedComponents = new List<MultiTiledComponent>();
            List<MultiTiledComponent> entitiesToSearch = new List<MultiTiledComponent>();
            entitiesToSearch.AddRange(this.getAppropriateEnergyNeighbors());
            searchedComponents.Add(this);
            while (entitiesToSearch.Count > 0)
            {
                MultiTiledComponent searchComponent = entitiesToSearch[0];
                entitiesToSearch.Remove(searchComponent);
                if (searchedComponents.Contains(searchComponent))
                {
                    continue;
                }
                else
                {
                    searchedComponents.Add(searchComponent);
                    entitiesToSearch.AddRange(searchComponent.getAppropriateEnergyNeighbors());

                    if (searchComponent.containerObject.info.EnergyManager.energyInteractionType == Enums.EnergyInteractionType.Consumes || searchComponent.containerObject.info.EnergyManager.energyInteractionType == Enums.EnergyInteractionType.Storage)
                    {
                        energySources.Add(searchComponent.containerObject);
                    }
                }

            }
            return energySources;
        }

        /// <summary>
        /// Gets all nodes in a connected energy network and tries to drain the necessary amount of energy from the network.
        /// </summary>
        public void drainEnergyFromNetwork()
        {
            //Machines that consume should ALWAYS try to drain energy from a network first.
            //Then producers should always try to store energy to a network.
            //Storage should never try to push or pull energy from a network as consumers will pull from storage and producers will push to storage.
            //Transfer nodes are used just to connect the network.
            List<MultiTiledObject> energySources = this.EnergyGraphSearchSources();

            int index = 0;

            for (int i = 0; i < energySources.Count; i++)
            {
                this.GetEnergyManager().transferEnergyFromAnother(energySources[i].GetEnergyManager(), this.GetEnergyManager().capacityRemaining);
                if (this.GetEnergyManager().hasMaxEnergy) break;
            }
        }

        public void drainEnergyFromNetwork(List<MultiTiledObject> energySources)
        {
            int index = 0;

            for (int i = 0; i < energySources.Count; i++)
            {
                this.GetEnergyManager().transferEnergyFromAnother(energySources[i].GetEnergyManager(), this.GetEnergyManager().capacityRemaining);
                if (this.GetEnergyManager().hasMaxEnergy) break;
            }
        }

        /// <summary>
        /// Gets all of the nodes in a connected energy network and tries to store the necessary amount of energy from the network.
        /// </summary>
        public void storeEnergyToNetwork()
        {
            List<MultiTiledObject> energySources = this.EnergyGraphSearchConsumers();

            int index = 0;

            for (int i = 0; i < energySources.Count; i++)
            {
                this.GetEnergyManager().transferEnergyToAnother(energySources[i].GetEnergyManager(), this.GetEnergyManager().capacityRemaining);
                if (this.GetEnergyManager().hasEnergy == false) break;
            }
        }

        public void storeEnergyToNetwork(List<MultiTiledObject> energySources)
        {

            int index = 0;

            for (int i = 0; i < energySources.Count; i++)
            {
                this.GetEnergyManager().transferEnergyToAnother(energySources[i].GetEnergyManager(), this.GetEnergyManager().capacityRemaining);
                if (this.GetEnergyManager().hasEnergy == false) break;
            }
        }

        public override ref EnergyManager GetEnergyManager()
        {
            if (this.info == null || this.containerObject == null)
            {
                this.updateInfo();
                if (this.containerObject == null) return ref this.info.EnergyManager;
                return ref this.containerObject.info.EnergyManager;
            }
            return ref this.containerObject.info.EnergyManager;
        }

        public override void SetEnergyManager(ref EnergyManager Manager)
        {
            this.info.EnergyManager = Manager;
        }

        public override ref InventoryManager GetInventoryManager()
        {
            if (this.info == null || this.containerObject == null)
            {
                this.updateInfo();
                if (this.containerObject == null)
                {
                    return ref this.info.inventory;
                }
                return ref this.containerObject.info.inventory;
            }
            return ref this.containerObject.info.inventory;
        }

        public override void SetInventoryManager(InventoryManager Manager)
        {
            this.info.inventory = Manager;
            this.containerObject.info.inventory = Manager;
        }


        public override ref FluidManagerV2 GetFluidManager()
        {
            if (this.info == null || this.containerObject == null)
            {
                this.updateInfo();
                if (this.containerObject == null) return ref this.info.fluidManager;
                return ref this.containerObject.info.fluidManager;
            }
            return ref this.containerObject.info.fluidManager;
        }

        public override void SetFluidManager(FluidManagerV2 FluidManager)
        {
            this.info.fluidManager = FluidManager;
        }


        /// <summary>
        /// Gets corresponding neighbor objects that can interact with fluid.
        /// </summary>
        /// <returns></returns>
        protected virtual List<MultiTiledComponent> GetNeighboringFluidManagers()
        {
            Vector2 tileLocation = this.TileLocation;
            List<MultiTiledComponent> customObjects = new List<MultiTiledComponent>();
            if (this.location != null)
            {
                for (int i = -1; i <= 1; i++)
                {
                    for (int j = -1; j <= 1; j++)
                    {
                        if (i == j || i == (-j)) continue;

                        Vector2 neighborTile = tileLocation + new Vector2(i, j);
                        if (this.location.isObjectAtTile((int)neighborTile.X, (int)neighborTile.Y))
                        {
                            StardewValley.Object obj = this.location.getObjectAtTile((int)neighborTile.X, (int)neighborTile.Y);
                            if (obj is MultiTiledComponent)
                            {

                                if ((obj as MultiTiledComponent).GetFluidManager().InteractsWithFluids)
                                {
                                    if ((obj as MultiTiledComponent).containerObject == this.containerObject) continue;
                                    customObjects.Add((MultiTiledComponent)obj);
                                    //ModCore.log("Found a neighboring fluid manager");
                                }
                                else
                                {
                                    //ModCore.log("Found a neighboring object but it isn't a valid fluid manager.");
                                }
                            }
                            else continue;
                        }
                    }
                }
            }
            return customObjects;
        }

        /// <summary>
        /// Searches a network of fluid managers to see if any of these fluid managers have an output tank with the corresponding fluid.
        /// </summary>
        /// <param name="L"></param>
        /// <returns></returns>
        protected virtual List<MultiTiledObject> FluidGraphSearchForFluidFromOutputTanks(Fluid L)
        {
            List<MultiTiledObject> fluidSources = new List<MultiTiledObject>();
            HashSet<Guid> searchedComponents = new HashSet<Guid>();
            Queue<MultiTiledComponent> entitiesToSearch = new Queue<MultiTiledComponent>();
            HashSet<Guid> searchedObjects = new HashSet<Guid>();
            foreach (MultiTiledComponent tile in this.GetNeighboringFluidManagers())
            {
                entitiesToSearch.Enqueue(tile);
            }
            //entitiesToSearch.AddRange(this.GetNeighboringFluidManagers());
            searchedComponents.Add(this.guid);
            while (entitiesToSearch.Count > 0)
            {
                MultiTiledComponent searchComponent = entitiesToSearch.Dequeue();
                //entitiesToSearch.Remove(searchComponent);
                if (searchedComponents.Contains(searchComponent.guid))
                {
                    continue;
                }
                /*
                else if (searchedObjects.Contains(searchComponent.containerObject))
                {
                    continue;
                }
                */
                else
                {
                    searchedComponents.Add(searchComponent.guid);
                    searchedObjects.Add(searchComponent.containerObject.guid);

                    List<MultiTiledComponent> neighbors = searchComponent.GetNeighboringFluidManagers();

                    foreach (MultiTiledComponent tile in neighbors)
                    {
                        if (searchedObjects.Contains(tile.containerObject.guid) || searchedComponents.Contains(tile.guid)) continue;
                        else
                        {
                            entitiesToSearch.Enqueue(tile);
                        }
                    }

                    if (searchComponent.containerObject.info.fluidManager.doesThisOutputTankContainThisFluid(L))
                    {
                        fluidSources.Add(searchComponent.containerObject);
                        //ModCore.log("Found a tank that contains this fluid!");
                    }
                }

            }
            return fluidSources;
        }

        protected virtual List<MultiTiledObject> FluidGraphSearchInputTanksThatCanAcceptThisFluid(Fluid L)
        {
            List<MultiTiledObject> fluidSources = new List<MultiTiledObject>();
            List<MultiTiledComponent> searchedComponents = new List<MultiTiledComponent>();
            List<MultiTiledComponent> entitiesToSearch = new List<MultiTiledComponent>();
            entitiesToSearch.AddRange(this.GetNeighboringFluidManagers());
            searchedComponents.Add(this);
            while (entitiesToSearch.Count > 0)
            {
                MultiTiledComponent searchComponent = entitiesToSearch[0];
                entitiesToSearch.Remove(searchComponent);
                if (searchedComponents.Contains(searchComponent))
                {
                    continue;
                }
                else
                {
                    searchedComponents.Add(searchComponent);
                    entitiesToSearch.AddRange(searchComponent.GetNeighboringFluidManagers());

                    if (searchComponent.containerObject.info.fluidManager.canRecieveThisFluid(L))
                    {
                        fluidSources.Add(searchComponent.containerObject);
                    }
                }

            }
            return fluidSources;
        }

        /// <summary>
        /// Searches for output tanks that have the corresponding fluid and tries to drain from them.
        /// </summary>
        /// <param name="L"></param>
        public void pullFluidFromNetworkOutputs(Fluid L)
        {
            List<MultiTiledObject> energySources = this.FluidGraphSearchForFluidFromOutputTanks(L);

            int index = 0;

            for (int i = 0; i < energySources.Count; i++)
            {
                FluidManagerV2 other = energySources[i].GetFluidManager();
                other.outputFluidToOtherSources(this.GetFluidManager());
                if (this.GetFluidManager().canRecieveThisFluid(L) == false) break; //Since we already check for valid tanks this will basically check again to see if the tanks are full.
            }
        }

        /// <summary>
        /// Searches for output tanks that have the corresponding fluid and tries to drain from them.
        /// </summary>
        /// <param name="L"></param>
        /// <param name="FluidSources"></param>
        public void pullFluidFromNetworkOutputs(List<MultiTiledObject> FluidSources, Fluid L)
        {
            List<MultiTiledObject> energySources = FluidSources;

            int index = 0;

            for (int i = 0; i < energySources.Count; i++)
            {
                FluidManagerV2 other = energySources[i].GetFluidManager();
                other.outputFluidToOtherSources(this.GetFluidManager());
                if (this.GetFluidManager().canRecieveThisFluid(L) == false) break; //Since we already check for valid tanks this will basically check again to see if the tanks are full.
            }
        }

        public override bool requiresUpdate()
        {
            if (this.info.requiresSyncUpdate() || this.containerObject.info.requiresSyncUpdate())
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
