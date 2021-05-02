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
using Revitalize.Framework.Objects.InformationFiles.Furniture;
using Revitalize.Framework.Utilities;
using StardewValley;
using StardewValley.Objects;

namespace Revitalize.Framework.Objects.Furniture
{
    /// <summary>
    /// Chair "piece" which represents one of the objects in the game that takes up roughly one tile.
    /// </summary>
    public class ChairTileComponent:FurnitureTileComponent
    {
        public ChairInformation furnitureInfo;

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
                string furnitureInfo = ModCore.Serializer.ToJSONString(this.furnitureInfo);
                return info + "<" + guidStr + "<" + pyTkData + "<" + offsetKey + "<" + container + "<" + furnitureInfo;
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
                string furnitureInfo = data[5];
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

                if (string.IsNullOrEmpty(furnitureInfo) == false)
                {
                    this.furnitureInfo = ModCore.Serializer.DeserializeFromJSONString<ChairInformation>(furnitureInfo);
                }

            }
        }
        /// <summary>
        /// Checks if the player can sit "on" this component.
        /// </summary>
        [JsonIgnore]
        public bool CanSitHere
        {
            get
            {
                return this.furnitureInfo.canSitHere;
            }
        }

        public ChairTileComponent():base()
        {

        }

        public ChairTileComponent(CustomObjectData PyTKData,BasicItemInformation Info,ChairInformation FurnitureInfo) : base(PyTKData,Info)
        {
            this.furnitureInfo = FurnitureInfo;
            this.Price = Info.price;
        }

        public ChairTileComponent(CustomObjectData PyTKData,BasicItemInformation Info,Vector2 TileLocation, ChairInformation FurnitureInfo) : base(PyTKData,Info, TileLocation)
        {
            this.furnitureInfo = FurnitureInfo;
            this.Price = Info.price;
        }

        

        /// <summary>
        /// When the chair is right clicked ensure that all pieces associated with it are also rotated.
        /// </summary>
        /// <param name="who"></param>
        /// <returns></returns>
        public override bool rightClicked(Farmer who)
        {
            if (this.framesUntilNextRotation <= 0)
            {
                this.containerObject.rotate(); //Ensure that all of the chair pieces rotate at the same time.
                this.checkForSpecialUpSittingAnimation();
                this.framesUntilNextRotation = ModCore.Configs.furnitureConfig.furnitureFrameRotationDelay;
                return true;
            }
            else
            {
                return true;
            }
            //return base.rightClicked(who);
        }

        /// <summary>
        /// Used for more object interactions.
        /// When the chair is shift right clicked sit on that specific chair tile if you can sit there.
        /// </summary>
        /// <param name="who"></param>
        /// <returns></returns>
        public override bool shiftRightClicked(Farmer who)
        {
            if (this.CanSitHere)
            {
                Revitalize.ModCore.playerInfo.sittingInfo.sit(this.containerObject, this.TileLocation*Game1.tileSize);
                if(this.containerObject is Bench)
                {
                    (this.containerObject as Bench).playersSittingHere.Add(Game1.player.uniqueMultiplayerID);
                }
                foreach(KeyValuePair<Vector2, StardewValley.Object> pair in this.containerObject.objects)
                {
                    (pair.Value as ChairTileComponent).checkForSpecialUpSittingAnimation();
                }
                
            }
            return base.shiftRightClicked(who);
        }


        public override Item getOne()
        {
            ChairTileComponent component = new ChairTileComponent(this.data,this.info.Copy(), (ChairInformation)this.furnitureInfo);
            //component.containerObject = this.containerObject;
            //component.offsetKey = this.offsetKey;
            return component;
        }

        public override ICustomObject recreate(Dictionary<string, string> additionalSaveData, object replacement)
        {
            Vector2 offsetKey = new Vector2(Convert.ToInt32(additionalSaveData["offsetKeyX"]), Convert.ToInt32(additionalSaveData["offsetKeyY"]));
            string GUID = additionalSaveData["GUID"];
            ChairTileComponent self = Revitalize.ModCore.Serializer.DeserializeGUID<ChairTileComponent>(additionalSaveData["GUID"]);
            if (ModCore.IsNullOrDefault<ChairTileComponent>(self)) return null;
            try
            {
                if (!Revitalize.ModCore.ObjectGroups.ContainsKey(additionalSaveData["ParentGUID"]))
                {
                    ChairMultiTiledObject obj = (ChairMultiTiledObject)Revitalize.ModCore.Serializer.DeserializeGUID<ChairMultiTiledObject>(additionalSaveData["ParentGUID"]);
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


        /// <summary>
        ///Used to manage graphics for chairs that need to deal with special "layering" for transparent chair backs. Otherwise the player would be hidden.
        /// </summary>
        public void checkForSpecialUpSittingAnimation()
        {
            if (this.info.facingDirection == Enums.Direction.Up && Revitalize.ModCore.playerInfo.sittingInfo.SittingObject == this.containerObject)
            {
                string animationKey = "Sitting_" + (int)Enums.Direction.Up;
                if (this.animationManager.animations.ContainsKey(animationKey))
                {
                    this.animationManager.setAnimation(animationKey);
                }
            }
        }


        /// <summary>What happens when the object is drawn at a tile location.</summary>
        public override void draw(SpriteBatch spriteBatch, int x, int y, float alpha = 1f)
        {
            /*
            if (this.info.ignoreBoundingBox == true)
            {
                x *= -1;
                y *= -1;
            }
            */

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
                if (this.framesUntilNextRotation > 0)
                this.framesUntilNextRotation--;
                if (this.framesUntilNextRotation < 0) this.framesUntilNextRotation = 0;
                
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
                if (this.framesUntilNextRotation > 0)
                    this.framesUntilNextRotation--;
                if (this.framesUntilNextRotation < 0) this.framesUntilNextRotation = 0;
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
