using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Newtonsoft.Json;
using PyTK.CustomElementHandler;
using Revitalize.Framework.Graphics.Animations;
using StardewValley;
using StardewValley.Objects;

namespace Revitalize.Framework.Objects
{
    // TODO:
    //     -Multiple Lights
    //     -Events when walking over?
    //     -Inventories

    /// <summary>A custom object template.</summary>
    public class CustomObject : PySObject
    {
        public string id;


        public BasicItemInformation info;
        public GameLocation location;


        public Guid guid;
      
        /// <summary>The animation manager.</summary>
        public AnimationManager animationManager => this.info.animationManager;

        /// <summary>The display texture for this object.</summary>
        [JsonIgnore]
        public Texture2D displayTexture => this.animationManager.getTexture();

        public string ItemInfo
        {
            get
            {
                return Revitalize.ModCore.Serializer.ToJSONString(this.info);
            }
            set
            {
                Revitalize.ModCore.log("GUESS SERIALIZATION IS WORKING???");
                this.info =(BasicItemInformation)Revitalize.ModCore.Serializer.DeserializeFromJSONString(value, typeof(BasicItemInformation));
            }
        }

        

        protected Netcode.NetString netItemInfo; 

        /// <summary>Empty constructor.</summary>
        public CustomObject() {
            this.guid = Guid.NewGuid();
            InitNetFields();
        }

        /// <summary>Construct an instance.</summary>
        public CustomObject(BasicItemInformation info)
            : base(info, Vector2.Zero)
        {
            this.info = info;
            this.initializeBasics();
            this.guid = Guid.NewGuid();
        }

        /// <summary>Construct an instance.</summary>
        public CustomObject(BasicItemInformation info, Vector2 TileLocation)
            : base(info, TileLocation)
        {
            this.info = info;
            this.initializeBasics();
            this.guid = Guid.NewGuid();
        }

        /// <summary>Sets some basic information up.</summary>
        public virtual void initializeBasics()
        {
            this.name = this.info.name;
            this.displayName = this.getDisplayNameFromStringsFile(this.id);
            this.Edibility = this.info.edibility;
            this.Category = -9; //For crafting.
            this.displayName = this.info.name;
            this.setOutdoors.Value = true;
            this.setIndoors.Value = true;
            this.isLamp.Value = false;
            this.fragility.Value = 0;

            this.updateDrawPosition(0, 0);

            this.bigCraftable.Value = false;

            //this.initNetFields();
            InitNetFields();
            //if (this.info.ignoreBoundingBox)
            //    this.boundingBox.Value = new Rectangle(int.MinValue, int.MinValue, 0, 0);
        }



        public override bool isPassable()
        {
            return this.info.ignoreBoundingBox || Revitalize.ModCore.playerInfo.sittingInfo.SittingObject==this;
        }

        public override Rectangle getBoundingBox(Vector2 tileLocation)
        {
            //Revitalize.ModCore.log(System.Environment.StackTrace);
            return this.info.ignoreBoundingBox
                ? new Rectangle(int.MinValue, int.MinValue, 0, 0)
                : base.getBoundingBox(tileLocation);
        }

        /// <summary>Checks for interaction with the object.</summary>
        public override bool checkForAction(Farmer who, bool justCheckingForActivity = false)
        {
            MouseState mState = Mouse.GetState();
            KeyboardState keyboardState = Game1.GetKeyboardState();

            if (mState.RightButton == ButtonState.Pressed && (keyboardState.IsKeyDown(Keys.LeftShift) || !keyboardState.IsKeyDown(Keys.RightShift)))
            {
                ModCore.log("Right clicked!");
                return this.rightClicked(who);
            }

            if (mState.RightButton == ButtonState.Pressed && (keyboardState.IsKeyDown(Keys.LeftShift) || keyboardState.IsKeyDown(Keys.RightShift)))
                return this.shiftRightClicked(who);

            if (justCheckingForActivity)
                return true;
            ModCore.log("Left clicked!");
            return this.clicked(who);
        }

        public override ICustomObject recreate(Dictionary<string, string> additionalSaveData, object replacement)
        {
            BasicItemInformation data = (BasicItemInformation)CustomObjectData.collection[additionalSaveData["id"]];
            return new CustomObject(data, (replacement as Chest).TileLocation);
        }

        /// <summary>What happens when the player right clicks the object.</summary>
        public virtual bool rightClicked(Farmer who)
        {
            // Game1.showRedMessage("YOOO");
            //do some stuff when the right button is down
            // rotate();
            if (this.heldObject.Value != null)
            {
                //  Game1.player.addItemByMenuIfNecessary(this.heldObject);
                // this.heldObject = null;
            }
            else
            {
                //   this.heldObject = Game1.player.ActiveObject;
                //  Game1.player.removeItemFromInventory(heldObject);
            }
            return true;
        }

        /// <summary>What happens when the player shift-right clicks this object.</summary>
        public virtual bool shiftRightClicked(Farmer who)
        {
            ModCore.log("Shift right clicked!");
            return true;
        }

        /// <summary>What happens when the player left clicks the object.</summary>
        public override bool clicked(Farmer who)
        {
            ModCore.log("Clicky click!");

            ModCore.log(System.Environment.StackTrace);

            return this.removeAndAddToPlayersInventory();
            //return base.clicked(who);
        }

        /// <summary>What happens when a player uses a tool on this object.</summary>
        public override bool performToolAction(Tool t, GameLocation location)
        {
            if (t.GetType() == typeof(StardewValley.Tools.Axe) || t.GetType() == typeof(StardewValley.Tools.Pickaxe))
            {
                Game1.createItemDebris(this, Game1.player.getStandingPosition(), Game1.player.getDirection());
                this.location = null;
                this.updateDrawPosition(0, 0);
                Game1.player.currentLocation.removeObject(this.TileLocation, false);
                this.updateDrawPosition(0, 0);
                return false;
            }

            return false;
            //return base.performToolAction(t, location);
        }

        /// <summary>Remove the object from the world and add it to the player's inventory if possible.</summary>
        public virtual bool removeAndAddToPlayersInventory()
        {
            if (Game1.player.isInventoryFull())
            {
                Game1.showRedMessage("Inventory full.");
                return false;
            }
            this.location = null;
            Game1.player.currentLocation.removeObject(this.TileLocation, false);
            Game1.player.addItemToInventory(this);
            this.updateDrawPosition(0, 0);
            return true;
        }

        /// <summary>Gets the category color for the object.</summary>
        public override Color getCategoryColor()
        {
            return this.info.categoryColor;
            //return data.categoryColor;
        }

        /// <summary>Gets the category name for the object.</summary>
        public override string getCategoryName()
        {
            return this.info.categoryName;
        }

        /// <summary>Gets the description for the object.</summary>
        public override string getDescription()
        {
            string text = this.info.description;
            SpriteFont smallFont = Game1.smallFont;
            int width = Game1.tileSize * 4 + Game1.tileSize / 4;
            return Game1.parseText(text, smallFont, width);
        }

        /// <summary>Places an object down.</summary>
        public override bool placementAction(GameLocation location, int x, int y, Farmer who = null)
        {
            this.updateDrawPosition(x, y);
            this.location = location;
            return base.placementAction(location, x, y, who);
        }

        public override bool canBePlacedHere(GameLocation l, Vector2 tile)
        {
            if (this.info.ignoreBoundingBox) return true;
            return base.canBePlacedHere(l, tile);
        }

        public virtual void rotate()
        {
            if (this.info.facingDirection == Enums.Direction.Down) this.info.facingDirection = Enums.Direction.Right;
            else if (this.info.facingDirection == Enums.Direction.Right) this.info.facingDirection = Enums.Direction.Up;
            else if (this.info.facingDirection == Enums.Direction.Up) this.info.facingDirection = Enums.Direction.Left;
            else if (this.info.facingDirection == Enums.Direction.Left) this.info.facingDirection = Enums.Direction.Down;

            if (this.info.animationManager.animations.ContainsKey(generateRotationalAnimationKey()))
            {
                this.info.animationManager.setAnimation(generateRotationalAnimationKey());
            }
            else
            {
                //Revitalize.ModCore.log("Animation does not exist...." + generateRotationalAnimationKey());
            }
        }


        public string generateRotationalAnimationKey()
        {          
            return (this.info.animationManager.currentAnimationName.Split('_')[0]) +"_"+ (int)this.info.facingDirection;
        }

        public string generateDefaultRotationalAnimationKey()
        {
            return ("Default" + "_" + (int)this.info.facingDirection);
        }

        /// <summary>Updates a visual draw position.</summary>
        public virtual void updateDrawPosition(int x, int y)
        {
            this.info.drawPosition = new Vector2((int)(x / Game1.tileSize), (int)(y / Game1.tileSize));
            //this.info.drawPosition = new Vector2((float)this.boundingBox.X, (float)(this.boundingBox.Y - (this.animationManager.currentAnimation.sourceRectangle.Height * Game1.pixelZoom - this.boundingBox.Height)));
        }

        /// <summary>Gets a clone of the game object.</summary>
        public override Item getOne()
        {
            return new CustomObject((BasicItemInformation)this.data);
        }

        /// <summary>What happens when the object is drawn at a tile location.</summary>
        public override void draw(SpriteBatch spriteBatch, int x, int y, float alpha = 1f)
        {
            if (x <= -1)
            {
                spriteBatch.Draw(this.info.animationManager.getTexture(), Game1.GlobalToLocal(Game1.viewport, this.info.drawPosition), new Rectangle?(this.animationManager.currentAnimation.sourceRectangle), this.info.drawColor * alpha, 0f, Vector2.Zero, (float)Game1.pixelZoom, this.flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None, Math.Max(0f, (float)(this.TileLocation.Y * Game1.tileSize) / 10000f));
            }
            else
            {
                //The actual planter box being drawn.
                if (this.animationManager == null)
                {
                    if (this.animationManager.getExtendedTexture() == null)
                        ModCore.ModMonitor.Log("Tex Extended is null???");

                    spriteBatch.Draw(this.displayTexture, Game1.GlobalToLocal(Game1.viewport, new Vector2((float)(x * Game1.tileSize), y * Game1.tileSize)), new Rectangle?(this.animationManager.currentAnimation.sourceRectangle), this.info.drawColor * alpha, 0f, Vector2.Zero, (float)Game1.pixelZoom, this.flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None, Math.Max(0f, (float)(this.TileLocation.Y * Game1.tileSize) / 10000f));
                    // Log.AsyncG("ANIMATION IS NULL?!?!?!?!");
                }

                else
                {
                    //Log.AsyncC("Animation Manager is working!");
                    int addedDepth = 0;
                    if (this.info.ignoreBoundingBox) addedDepth++;
                    if (Revitalize.ModCore.playerInfo.sittingInfo.SittingObject == this) addedDepth++;
                    this.animationManager.draw(spriteBatch, this.displayTexture, Game1.GlobalToLocal(Game1.viewport, new Vector2((float)(x * Game1.tileSize), y * Game1.tileSize)), new Rectangle?(this.animationManager.currentAnimation.sourceRectangle), this.info.drawColor * alpha, 0f, Vector2.Zero, (float)Game1.pixelZoom, this.flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None, Math.Max(0f, (float)( (this.TileLocation.Y+addedDepth) * Game1.tileSize) / 10000f));
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

        /// <summary>Draw the game object at a non-tile spot. Aka like debris.</summary>
        public override void draw(SpriteBatch spriteBatch, int xNonTile, int yNonTile, float layerDepth, float alpha = 1f)
        {
            if (Game1.eventUp && Game1.CurrentEvent.isTileWalkedOn(xNonTile / 64, yNonTile / 64))
                return;
            if ((int)(this.ParentSheetIndex) != 590 && (int)(this.Fragility) != 2)
                spriteBatch.Draw(Game1.shadowTexture, Game1.GlobalToLocal(Game1.viewport, new Vector2((float)(xNonTile + 32), (float)(yNonTile + 51 + 4))), new Microsoft.Xna.Framework.Rectangle?(Game1.shadowTexture.Bounds), Color.White * alpha, 0.0f, new Vector2((float)Game1.shadowTexture.Bounds.Center.X, (float)Game1.shadowTexture.Bounds.Center.Y), 4f, SpriteEffects.None, layerDepth - 1E-06f);
            SpriteBatch spriteBatch1 = spriteBatch;
            Texture2D objectSpriteSheet = Game1.objectSpriteSheet;
            Vector2 local = Game1.GlobalToLocal(Game1.viewport, new Vector2((float)(xNonTile + 32 + (this.shakeTimer > 0 ? Game1.random.Next(-1, 2) : 0)), (float)(yNonTile + 32 + (this.shakeTimer > 0 ? Game1.random.Next(-1, 2) : 0))));
            Microsoft.Xna.Framework.Rectangle? sourceRectangle = new Microsoft.Xna.Framework.Rectangle?(GameLocation.getSourceRectForObject(this.ParentSheetIndex));
            Color color = Color.White * alpha;
            double num1 = 0.0;
            Vector2 origin = new Vector2(8f, 8f);
            Vector2 scale = this.scale;
            double num2 = (double)this.scale.Y > 1.0 ? (double)this.getScale().Y : 4.0;
            int num3 = (bool)(this.flipped) ? 1 : 0;
            double num4 = (double)layerDepth;

            spriteBatch1.Draw(this.displayTexture, local, this.animationManager.defaultDrawFrame.sourceRectangle, this.info.drawColor * alpha, (float)num1, origin, (float)4f, (SpriteEffects)num3, (float)num4);
        }

        /// <summary>What happens when the object is drawn in a menu.</summary>
        public override void drawInMenu(SpriteBatch spriteBatch, Vector2 location, float scaleSize, float transparency, float layerDepth, bool drawStackNumber, Color c, bool drawShadow)
        {
            if (drawStackNumber && this.maximumStackSize() > 1 && ((double)scaleSize > 0.3 && this.Stack != int.MaxValue) && this.Stack > 1)
                Utility.drawTinyDigits(this.Stack, spriteBatch, location + new Vector2((float)(Game1.tileSize - Utility.getWidthOfTinyDigitString(this.Stack, 3f * scaleSize)) + 3f * scaleSize, (float)((double)Game1.tileSize - 18.0 * (double)scaleSize + 2.0)), 3f * scaleSize, 1f, Color.White);
            if (drawStackNumber && this.Quality > 0)
            {
                float num = this.Quality < 4 ? 0.0f : (float)((Math.Cos((double)Game1.currentGameTime.TotalGameTime.Milliseconds * Math.PI / 512.0) + 1.0) * 0.0500000007450581);
                spriteBatch.Draw(Game1.mouseCursors, location + new Vector2(12f, (float)(Game1.tileSize - 12) + num), new Microsoft.Xna.Framework.Rectangle?(this.Quality < 4 ? new Microsoft.Xna.Framework.Rectangle(338 + (this.Quality - 1) * 8, 400, 8, 8) : new Microsoft.Xna.Framework.Rectangle(346, 392, 8, 8)), Color.White * transparency, 0.0f, new Vector2(4f, 4f), (float)(3.0 * (double)scaleSize * (1.0 + (double)num)), SpriteEffects.None, layerDepth);
            }
            spriteBatch.Draw(this.displayTexture, location + new Vector2((float)(Game1.tileSize / 2), (float)(Game1.tileSize * .75)), new Rectangle?(this.animationManager.currentAnimation.sourceRectangle), this.info.drawColor * transparency, 0f, new Vector2((float)(this.animationManager.currentAnimation.sourceRectangle.Width / 2), (float)(this.animationManager.currentAnimation.sourceRectangle.Height)), 3f, SpriteEffects.None, layerDepth);
        }

        /// <summary>What happens when the object is drawn when held by a player.</summary>
        public override void drawWhenHeld(SpriteBatch spriteBatch, Vector2 objectPosition, StardewValley.Farmer f)
        {
            
            if (this.animationManager == null) Revitalize.ModCore.log("Animation Manager Null");
            if (this.displayTexture == null) Revitalize.ModCore.log("Display texture is null");
            if (f.ActiveObject.bigCraftable.Value)
            {
                spriteBatch.Draw(this.displayTexture, objectPosition, this.animationManager.currentAnimation.sourceRectangle, this.info.drawColor, 0f, Vector2.Zero, (float)Game1.pixelZoom, SpriteEffects.None, Math.Max(0f, (float)(f.getStandingY() + 2) / 10000f));
                return;
            }

            spriteBatch.Draw(this.displayTexture, objectPosition, this.animationManager.currentAnimation.sourceRectangle, this.info.drawColor, 0f, Vector2.Zero, (float)Game1.pixelZoom, SpriteEffects.None, Math.Max(0f, (float)(f.getStandingY() + 2) / 10000f));
            if (f.ActiveObject != null && f.ActiveObject.Name.Contains("="))
            {
                spriteBatch.Draw(this.displayTexture, objectPosition + new Vector2((float)(Game1.tileSize / 2), (float)(Game1.tileSize / 2)), this.animationManager.currentAnimation.sourceRectangle, Color.White, 0f, new Vector2((float)(Game1.tileSize / 2), (float)(Game1.tileSize / 2)), (float)Game1.pixelZoom + Math.Abs(Game1.starCropShimmerPause) / 8f, SpriteEffects.None, Math.Max(0f, (float)(f.getStandingY() + 2) / 10000f));
                if (Math.Abs(Game1.starCropShimmerPause) <= 0.05f && Game1.random.NextDouble() < 0.97)
                {
                    return;
                }
                Game1.starCropShimmerPause += 0.04f;
                if (Game1.starCropShimmerPause >= 0.8f)
                {
                    Game1.starCropShimmerPause = -0.8f;
                }
            }
            //base.drawWhenHeld(spriteBatch, objectPosition, f);
        }

        public void InitNetFields()
        {
            if (Game1.IsMultiplayer == false &&(Game1.IsClient==false || Game1.IsClient==false)) return;
            this.initNetFields();
            this.syncObject = new PySync(this);
            this.NetFields.AddField(this.syncObject);
            this.netItemInfo = new Netcode.NetString(this.ItemInfo);
            this.NetFields.AddField(this.netItemInfo);
        }

        /// <summary>
        /// Gets all of the data necessary for syncing.
        /// </summary>
        /// <returns></returns>
        public override Dictionary<string, string> getSyncData()
        {
            Dictionary<string,string> syncData= base.getSyncData();
            syncData.Add("BasicItemInfo", Revitalize.ModCore.Serializer.ToJSONString(this.info));
            return syncData;
        }

        /// <summary>
        /// Syncs all of the info to all players.
        /// </summary>
        /// <param name="syncData"></param>
        public override void sync(Dictionary<string, string> syncData)
        {
            Revitalize.ModCore.log("SYNC OBJECT DATA!");
            base.sync(syncData);
            this.info = Revitalize.ModCore.Serializer.DeserializeFromJSONString<BasicItemInformation>(syncData["BasicItemInfo"]);
        }

        public string getDisplayNameFromStringsFile(string objectID)
        {
            //Load in a file that has all object names referenced here or something.
            return this.info.name;
        }
    }
}
