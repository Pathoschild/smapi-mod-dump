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
using Microsoft.Xna.Framework.Input;
using Newtonsoft.Json;
using PyTK.CustomElementHandler;
using Revitalize.Framework.Objects.InformationFiles;
using Revitalize.Framework.Utilities;
using Revitalize.Framework.Utilities.Serialization;
using StardewValley;

namespace Revitalize.Framework.Objects.Resources.OreVeins
{
    public class OreVeinTile:MultiTiledComponent
    {
        /// <summary>
        /// Deals with information tied to the resource itself.
        /// </summary>
        public OreResourceInformation resourceInfo;
        public List<ResourceInformation> extraDrops;

        private int _healthValue;
        public int healthValue
        {
            get
            {
                return this._healthValue;
            }
            set
            {
                this._healthValue = value;
                if (this.info != null)
                {
                    this.info.forceUpdate();
                }
            }
        }


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
                string health = this.healthValue.ToString();
                return info + "<" + guidStr + "<" + pyTkData + "<" + offsetKey + "<" + container + "<"+health;
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
                string health = data[5];
                this.healthValue = Convert.ToInt32(health);

                this.info = (BasicItemInformation)Revitalize.ModCore.Serializer.DeserializeFromJSONString(infoString, typeof(BasicItemInformation));

                //Instead of serializing this info it's static pretty much always so just pull the info from the resource manager.
                OreResourceInformation oreResource = ModCore.ObjectManager.resources.getOreResourceInfo(this.info.id);
                List<ResourceInformation> extraDrops = ModCore.ObjectManager.resources.getExtraDropInformationFromOres(this.info.id);
                if (this.resourceInfo == null)
                {
                    this.resourceInfo = oreResource;
                }
                if (this.extraDrops == null)
                {
                    this.extraDrops = extraDrops;
                }
;
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


        public OreVeinTile() : base()
        {

        }

        public OreVeinTile(CustomObjectData PyTKData, BasicItemInformation Info, OreResourceInformation Resource,List<ResourceInformation> ExtraDrops,int Health) : base(PyTKData, Info)
        {
            this.healthValue = Health;
            this.resourceInfo = Resource;
            this.extraDrops = ExtraDrops != null ? ExtraDrops : new List<ResourceInformation>();
            this.setHealth(this.healthValue);
            this.Price = Info.price;
        }

        public OreVeinTile(CustomObjectData PyTKData, BasicItemInformation Info, Vector2 TileLocation, OreResourceInformation Resource, List<ResourceInformation> ExtraDrops,int Health) : base(PyTKData, Info, TileLocation)
        {

            this.healthValue = Health;
            this.resourceInfo = Resource;
            this.extraDrops = ExtraDrops != null ? ExtraDrops : new List<ResourceInformation>();
            this.setHealth(this.healthValue);
            this.Price = Info.price;
        }


        public override bool performObjectDropInAction(Item dropInItem, bool probe, Farmer who)
        {
            return false; //this.pickUpItem()==PickUpState.DoNothing;
            //return base.performObjectDropInAction(dropInItem, probe, who);
        }

        public override bool performDropDownAction(Farmer who)
        {
            return base.performDropDownAction(who);
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
            this.info.shakeTimer -= time.ElapsedGameTime.Milliseconds;
        }

        public override void DayUpdate(GameLocation location)
        {
            base.DayUpdate(location);
            this.setHealth(this.healthValue);
        }

        //Checks for any sort of interaction IF and only IF there is a held object on this tile.
        public override bool checkForAction(Farmer who, bool justCheckingForActivity = false)
        {
            MouseState mState = Mouse.GetState();
            KeyboardState keyboardState = Game1.GetKeyboardState();

            if (mState.RightButton == ButtonState.Pressed && (!keyboardState.IsKeyDown(Keys.LeftShift) || !keyboardState.IsKeyDown(Keys.RightShift)))
            {
                return this.rightClicked(who);
            }

            if (mState.RightButton == ButtonState.Pressed && (keyboardState.IsKeyDown(Keys.LeftShift) || keyboardState.IsKeyDown(Keys.RightShift)))
                return this.shiftRightClicked(who);


            //return base.checkForAction(who, justCheckingForActivity);

            if (justCheckingForActivity)
                return true;

            return true;

            //return this.clicked(who);
            //return false;
        }

        /// <summary>
        /// What happens when the player hits this with a tool.
        /// </summary>
        /// <param name="t"></param>
        /// <param name="location"></param>
        /// <returns></returns>
        public override bool performToolAction(Tool t, GameLocation location)
        {

            if(t is StardewValley.Tools.Pickaxe)
            {
                //ModCore.log("Hit the ore vein with a pickaxe!");
                this.damage((t as StardewValley.Tools.Pickaxe).UpgradeLevel+1);
                if (this.location != null)
                {
                    this.location.playSound("hammer");
                    //ModCore.log("Ore has this much health left and location is not null: "+this.healthValue);
                    this.info.shakeTimer = 200;
                }
                else
                {
                    Game1.player.currentLocation.playSound("hammer");
                    //ModCore.log("Ore has this much health left and location is null!: "+this.healthValue);
                    this.info.shakeTimer = 200;
                }
                return false;
            }
            else
            {
                return false;
            }

            //return base.performToolAction(t, location);
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
                this.healthValue -= amount;
                if (this.healthValue <= 0)
                {
                    this.destoryVein();
                }
            }
        }

        /// <summary>
        /// Destroys this tile for the ore vein.
        /// </summary>
        public void destoryVein()
        {
            int amount = this.resourceInfo.getNumberOfDropsToSpawn();
            Item newItem = this.resourceInfo.droppedItem.getOne();
            for(int i = 0; i < amount; i++)
            {
                Game1.createItemDebris(newItem.getOne(), this.TileLocation*Game1.tileSize, Game1.random.Next(0, 3), this.location);
            }

            if (this.extraDrops != null)
            {
                foreach (ResourceInformation extra in this.extraDrops)
                {
                    if (extra.shouldDropResource())
                    {
                        Item extraItem = extra.droppedItem.getOne();
                        int extraAmount = extra.getNumberOfDropsToSpawn();
                        for (int i = 0; i < amount; i++)
                        {
                            Game1.createItemDebris(extraItem.getOne(), this.TileLocation * Game1.tileSize, Game1.random.Next(0, 3), this.location);
                        }
                    }
                    else
                    {
                        //Resource did not meet spawn chance.
                    }
                }
            }

            if (this.location != null)
            {
                this.location.playSound("stoneCrack");
                Game1.createRadialDebris(this.location, 14, (int)this.TileLocation.X, (int)this.TileLocation.Y, Game1.random.Next(4, 10), false, -1, false, -1);
                this.location.removeObject(this.TileLocation, false);
                this.containerObject.removeComponent(this.offsetKey);
                ModCore.CustomObjects.Remove(this.containerObject.guid);
                ModCore.CustomObjects.Remove(this.guid);
            }
            else
            {
                Game1.player.currentLocation.playSound("stoneCrack");
                Game1.createRadialDebris(Game1.player.currentLocation, 14, (int)this.TileLocation.X, (int)this.TileLocation.Y, Game1.random.Next(4, 10), false, -1, false, -1);
                Game1.player.currentLocation.removeObject(this.TileLocation, false);
                this.containerObject.removeComponent(this.offsetKey);
                //Remove both tile and container from sync.
                ModCore.CustomObjects.Remove(this.containerObject.guid);
                ModCore.CustomObjects.Remove(this.guid);
            }
        }

        public override bool performUseAction(GameLocation location)
        {
            return base.performUseAction(location);
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
            OreVeinTile component = new OreVeinTile(this.data, this.info.Copy(),this.resourceInfo,this.extraDrops,this.healthValue);
            component.containerObject = this.containerObject;
            component.offsetKey = this.offsetKey;
            return component;
        }

        public override ICustomObject recreate(Dictionary<string, string> additionalSaveData, object replacement)
        {
            Vector2 offsetKey = new Vector2(Convert.ToInt32(additionalSaveData["offsetKeyX"]), Convert.ToInt32(additionalSaveData["offsetKeyY"]));
            string GUID = additionalSaveData["GUID"];
            OreVeinTile self = Revitalize.ModCore.Serializer.DeserializeGUID<OreVeinTile>(additionalSaveData["GUID"]);
            if (ModCore.IsNullOrDefault<OreVeinTile>(self)) return null;
            try
            {
                if (!Revitalize.ModCore.ObjectGroups.ContainsKey(additionalSaveData["ParentGUID"]))
                {
                    OreVeinObj obj = (OreVeinObj)Revitalize.ModCore.Serializer.DeserializeGUID<OreVeinObj>(additionalSaveData["ParentGUID"]);
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

        /// <summary>What happens when the object is drawn at a tile location.</summary>
        public override void draw(SpriteBatch spriteBatch, int x, int y, float alpha = 1f)
        {
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

                spriteBatch.Draw(this.displayTexture, Game1.GlobalToLocal(Game1.viewport, new Vector2((float)(x * Game1.tileSize)+this.info.shakeTimerOffset(), (y * Game1.tileSize)+this.info.shakeTimerOffset())), new Rectangle?(this.animationManager.currentAnimation.sourceRectangle), this.info.DrawColor * alpha, 0f, Vector2.Zero, (float)Game1.pixelZoom, this.flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None, Math.Max(0f, (float)(y * Game1.tileSize) / 10000f));
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
                this.animationManager.draw(spriteBatch, this.displayTexture, Game1.GlobalToLocal(Game1.viewport, new Vector2((float)(x * Game1.tileSize)+this.info.shakeTimerOffset(), (y * Game1.tileSize)+this.info.shakeTimerOffset())), new Rectangle?(this.animationManager.currentAnimation.sourceRectangle), this.info.DrawColor * alpha, 0f, Vector2.Zero, (float)Game1.pixelZoom, this.flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None, Math.Max(0f, (float)((y + addedDepth) * Game1.tileSize) / 10000f) + .00001f);
                try
                {
                    this.animationManager.tickAnimation();
                    // Log.AsyncC("Tick animation");
                }
                catch (Exception err)
                {
                    ModCore.ModMonitor.Log(err.ToString());
                }
                if (this.heldObject.Value != null) SpriteBatchUtilities.Draw(spriteBatch, this, this.heldObject.Value, alpha, addedDepth);
            }
        }


    }
}
