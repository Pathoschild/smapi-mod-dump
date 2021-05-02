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
using PyTK.CustomElementHandler;
using Revitalize.Framework.Utilities;
using Revitalize.Framework.Utilities.Serialization;
using StardewValley;

namespace Revitalize.Framework.Objects.Extras
{
    public class TrashCanTile : MultiTiledComponent
    {
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
        public TrashCanTile() : base()
        {

        }

        public TrashCanTile(CustomObjectData PyTKData, BasicItemInformation Info) : base(PyTKData, Info)
        {
            this.Price = Info.price;
        }

        public TrashCanTile(CustomObjectData PyTKData, BasicItemInformation Info, Vector2 TileLocation) : base(PyTKData, Info, TileLocation)
        {
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

        public override bool performToolAction(Tool t, GameLocation location)
        {
            return base.performToolAction(t, location);
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
            return base.clicked(who);
        }

        public override bool rightClicked(Farmer who)
        {
            if (Game1.player.ActiveObject != null)
            {
                if (this.containerObject.info.inventory != null && Game1.activeClickableMenu == null)
                {
                    this.containerObject.info.inventory.addItem(Game1.player.ActiveObject);
                    Game1.player.Items.Remove(Game1.player.ActiveObject);
                }
            }
            else
            {
                if (this.containerObject.info.inventory != null && Game1.activeClickableMenu == null)
                {
                    this.containerObject.info.inventory.clear();
                    Game1.activeClickableMenu = new Revitalize.Framework.Menus.InventoryTransferMenu(100, 100, 500, 500, this.containerObject.info.inventory.items, this.containerObject.info.inventory.capacity);
                }
            }
            return false;
        }


        public override bool shiftRightClicked(Farmer who)
        {
            return base.shiftRightClicked(who);
        }


        public override Item getOne()
        {
            TrashCanTile component = new TrashCanTile(this.data, this.info.Copy());
            component.containerObject = this.containerObject;
            component.offsetKey = this.offsetKey;
            return component;
        }

        public override ICustomObject recreate(Dictionary<string, string> additionalSaveData, object replacement)
        {
            Vector2 offsetKey = new Vector2(Convert.ToInt32(additionalSaveData["offsetKeyX"]), Convert.ToInt32(additionalSaveData["offsetKeyY"]));
            string GUID = additionalSaveData["GUID"];
            TrashCanTile self = Revitalize.ModCore.Serializer.DeserializeGUID<TrashCanTile>(additionalSaveData["GUID"]);
            if (ModCore.IsNullOrDefault<TrashCanTile>(self)) return null;
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

        /*
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

                spriteBatch.Draw(this.displayTexture, Game1.GlobalToLocal(Game1.viewport, new Vector2((float)(x * Game1.tileSize), y * Game1.tileSize)), new Rectangle?(this.animationManager.currentAnimation.sourceRectangle), this.info.drawColor * alpha, 0f, Vector2.Zero, (float)Game1.pixelZoom, this.Flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None, Math.Max(0f, (float)(y * Game1.tileSize) / 10000f));
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
                this.animationManager.draw(spriteBatch, this.displayTexture, Game1.GlobalToLocal(Game1.viewport, new Vector2((float)(x * Game1.tileSize), y * Game1.tileSize)), new Rectangle?(this.animationManager.currentAnimation.sourceRectangle), this.info.drawColor * alpha, 0f, Vector2.Zero, (float)Game1.pixelZoom, this.Flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None, Math.Max(0f, (float)((y + addedDepth) * Game1.tileSize) / 10000f) + .00001f);
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
        */

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
                this.animationManager.draw(spriteBatch, this.displayTexture, Game1.GlobalToLocal(Game1.viewport, new Vector2((float)(x * Game1.tileSize), y * Game1.tileSize)), new Rectangle?(this.animationManager.currentAnimation.sourceRectangle), this.info.DrawColor * alpha, 0f, Vector2.Zero, (float)Game1.pixelZoom, this.Flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None, Math.Max(0f, (float)((y + addedDepth) * Game1.tileSize) / 10000f) + .00001f);
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

        public override void drawWhenHeld(SpriteBatch spriteBatch, Vector2 objectPosition, Farmer f)
        {
            if (objectPosition.X < 0) objectPosition.X *= -1;
            if (objectPosition.Y < 0) objectPosition.Y *= -1;
            base.drawWhenHeld(spriteBatch, objectPosition, f);
        }
    }
}
