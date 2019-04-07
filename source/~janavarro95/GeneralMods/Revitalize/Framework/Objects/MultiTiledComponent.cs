using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PyTK.CustomElementHandler;
using StardewValley;
using StardewValley.Objects;

namespace Revitalize.Framework.Objects
{
    public class MultiTiledComponent : CustomObject,ISaveElement
    {
        public MultiTiledObject containerObject;

        public Vector2 offsetKey;

        public MultiTiledComponent() { }

        public MultiTiledComponent(BasicItemInformation info) : base(info) { }

        public MultiTiledComponent(BasicItemInformation info, Vector2 TileLocation,MultiTiledObject obj=null) : base(info, TileLocation) {
            this.containerObject = obj;
        }

        public MultiTiledComponent(BasicItemInformation info, Vector2 TileLocation,Vector2 offsetKey ,MultiTiledObject obj = null) : base(info, TileLocation) {
            this.offsetKey = offsetKey;
            this.containerObject = obj;
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

        public override bool clicked(Farmer who)
        {
            //ModCore.log("Clicked a multiTiledComponent!");
            this.containerObject.pickUp();
            return true;
            //return base.clicked(who);
        }

        public override bool rightClicked(Farmer who)
        {
            if (this.location == null)
                this.location = Game1.player.currentLocation;
            //this.info.lightManager.toggleLights(this.location, this);

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
            location.removeObject(this.TileLocation, false);
            this.location = null;
            //this.performRemoveAction(this.TileLocation,location);
        }

        /// <summary>Places an object down.</summary>
        public override bool placementAction(GameLocation location, int x, int y, Farmer who = null)
        {
            this.updateDrawPosition(x, y);
            this.location = location;

            if (this.location == null) this.location = Game1.player.currentLocation;
            this.TileLocation = new Vector2((int)(x / Game1.tileSize), (int)(y / Game1.tileSize));

            return base.placementAction(location, x, y, who);
        }

        public override void drawInMenu(SpriteBatch spriteBatch, Vector2 location, float scaleSize, float transparency, float layerDepth, bool drawStackNumber, Color c, bool drawShadow)
        {
            if (drawStackNumber && this.maximumStackSize() > 1 && ((double)scaleSize > 0.3 && this.Stack != int.MaxValue) && this.Stack > 1)
                Utility.drawTinyDigits(this.Stack, spriteBatch, location + new Vector2((float)(Game1.tileSize - Utility.getWidthOfTinyDigitString(this.Stack, 3f * scaleSize)) + 3f * scaleSize, (float)((double)Game1.tileSize - 18.0 * (double)scaleSize + 2.0)), 3f * scaleSize, 1f, Color.White);
            if (drawStackNumber && this.Quality > 0)
            {
                float num = this.Quality < 4 ? 0.0f : (float)((Math.Cos((double)Game1.currentGameTime.TotalGameTime.Milliseconds * Math.PI / 512.0) + 1.0) * 0.0500000007450581);
                spriteBatch.Draw(Game1.mouseCursors, location + new Vector2(12f, (float)(Game1.tileSize - 12) + num), new Microsoft.Xna.Framework.Rectangle?(this.Quality < 4 ? new Microsoft.Xna.Framework.Rectangle(338 + (this.Quality - 1) * 8, 400, 8, 8) : new Microsoft.Xna.Framework.Rectangle(346, 392, 8, 8)), Color.White * transparency, 0.0f, new Vector2(4f, 4f), (float)(3.0 * (double)scaleSize * (1.0 + (double)num)), SpriteEffects.None, layerDepth);
            }
            spriteBatch.Draw(this.displayTexture, location + new Vector2((float)(Game1.tileSize / 4), (float)(Game1.tileSize * .75)), new Rectangle?(this.animationManager.currentAnimation.sourceRectangle), this.info.drawColor * transparency, 0f, new Vector2((float)(this.animationManager.currentAnimation.sourceRectangle.Width / 2), (float)(this.animationManager.currentAnimation.sourceRectangle.Height)), scaleSize, SpriteEffects.None, layerDepth);
        }

        public override Item getOne()
        {
            MultiTiledComponent component = new MultiTiledComponent(this.info, this.TileLocation,this.offsetKey,this.containerObject);
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
            MultiTiledComponent self = Revitalize.ModCore.Serializer.DeserializeGUID<MultiTiledComponent>( additionalSaveData["GUID"]);
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
            Dictionary<string,string> saveData= base.getAdditionalSaveData();
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
            Revitalize.ModCore.Serializer.SerializeGUID(this.containerObject.childrenGuids[this.offsetKey].ToString(),this);

            return saveData;

        }

        protected string recreateParentId(string id)
        {
            StringBuilder b = new StringBuilder();
            string[] splits = id.Split('.');
            for(int i = 0; i < splits.Length - 1; i++)
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

                    spriteBatch.Draw(this.displayTexture, Game1.GlobalToLocal(Game1.viewport, new Vector2((float)(x * Game1.tileSize), y * Game1.tileSize)), new Rectangle?(this.animationManager.currentAnimation.sourceRectangle), this.info.drawColor * alpha, 0f, Vector2.Zero, (float)Game1.pixelZoom, this.flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None, Math.Max(0f, (float)(y * Game1.tileSize) / 10000f));
                    // Log.AsyncG("ANIMATION IS NULL?!?!?!?!");
                }

                else
                {
                    //Log.AsyncC("Animation Manager is working!");
                    float addedDepth = 0;
                    

                    if (Revitalize.ModCore.playerInfo.sittingInfo.SittingObject == this.containerObject && this.info.facingDirection == Enums.Direction.Up)
                    {
                        addedDepth += (this.containerObject.Height - 1) - ((int)(this.offsetKey.Y));
                        if (this.info.ignoreBoundingBox) addedDepth+=1.5f;
                    }
                    else if (this.info.ignoreBoundingBox)
                    {
                        addedDepth += 1.0f;
                    }
                    this.animationManager.draw(spriteBatch, this.displayTexture, Game1.GlobalToLocal(Game1.viewport, new Vector2((float)(x * Game1.tileSize), y * Game1.tileSize)), new Rectangle?(this.animationManager.currentAnimation.sourceRectangle), this.info.drawColor * alpha, 0f, Vector2.Zero, (float)Game1.pixelZoom, this.flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None, Math.Max(0f, (float)((y + addedDepth) * Game1.tileSize) / 10000f)+.00001f);
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



    }
}
