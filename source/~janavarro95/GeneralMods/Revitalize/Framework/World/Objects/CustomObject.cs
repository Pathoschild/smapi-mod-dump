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
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Revitalize.Framework.Illuminate;
using Revitalize.Framework.World.Objects.InformationFiles;
using Revitalize.Framework.World.Objects.Interfaces;
using Revitalize.Framework;
using Revitalize.Framework.Objects;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Objects;
using StardewValley.Tools;
using StardustCore.Animations;

namespace Revitalize.Framework.World.Objects
{
    /// <summary>
    /// A base class that is to be extended by other implementations of objects.
    ///
    /// Clicking to remove and click place are bound to the samething. Need to find a way to change that.
    /// Bounding boxes work, but not for clicking to remove. Why is that?
    /// </summary>
    [XmlType("Revitalize.Framework.World.Objects.CustomObject")]
    public class CustomObject:StardewValley.Objects.Furniture, ICommonObjectInterface, ILightManagerProvider, IBasicItemInfoProvider
    {
        public bool isCurrentLocationAStructure;

        public BasicItemInformation basicItemInfo;

        public override string Name { get => this.basicItemInfo.name; set => this.basicItemInfo.name = value; }
        public override string DisplayName { get => this.basicItemInfo.name; set => this.basicItemInfo.name = value; }

        [XmlIgnore]
        public AnimationManager AnimationManager
        {
            get
            {
                if (this.basicItemInfo == null) return null;
                if (this.basicItemInfo.animationManager == null) return null;
                return this.basicItemInfo.animationManager;
            }
            set
            {
                this.basicItemInfo.animationManager = value;
            }
        }

        [XmlIgnore]
        public Texture2D CurrentTextureToDisplay
        {

            get
            {
                if (this.AnimationManager == null) return null;
                return this.AnimationManager.getTexture();
            }
        }

        public CustomObject()
        {
            this.basicItemInfo = new BasicItemInformation();
            this.furniture_type.Value = Furniture.other;
            this.bigCraftable.Value = true;
            this.Type = "interactive";
            this.name = this.Name;


            IReflectedField<int> placementRestriction = ModCore.ModHelper.Reflection.GetField<int>(this, "_placementRestriction");
            placementRestriction.SetValue(2);
            this.Fragility = 0;

        }

        public CustomObject(BasicItemInformation basicItemInfo)
        {
            this.basicItemInfo=basicItemInfo;
            this.bigCraftable.Value = true;
            this.furniture_type.Value = Furniture.other;
            this.Type = "interactive";
            this.name = this.Name;

            IReflectedField<int> placementRestriction= ModCore.ModHelper.Reflection.GetField<int>(this, "_placementRestriction");
            placementRestriction.SetValue(2);
            this.Fragility = 0;
        }

        public CustomObject(BasicItemInformation basicItemInfo, int StackSize=1)
        {
            this.basicItemInfo = basicItemInfo;
            this.TileLocation = Vector2.Zero;
            this.Stack = StackSize;
            this.bigCraftable.Value = true;
            this.furniture_type.Value = Furniture.other;
            this.Type = "interactive";
            this.name = this.Name;
            this.Fragility = 0;

            IReflectedField<int> placementRestriction = ModCore.ModHelper.Reflection.GetField<int>(this, "_placementRestriction");
            placementRestriction.SetValue(2);
        }

        public CustomObject(BasicItemInformation basicItemInfo, Vector2 TileLocation)
        {
            this.basicItemInfo = basicItemInfo;
            this.TileLocation = TileLocation;
            this.bigCraftable.Value = true;
            this.furniture_type.Value = Furniture.other;
            this.Type = "interactive";
            this.name = this.Name;
            this.Fragility = 0;

            IReflectedField<int> placementRestriction = ModCore.ModHelper.Reflection.GetField<int>(this, "_placementRestriction");
            placementRestriction.SetValue(2);
        }
        public CustomObject(BasicItemInformation basicItemInfo, Vector2 TileLocation, int StackSize=1)
        {
            this.basicItemInfo = basicItemInfo;
            this.TileLocation = TileLocation;
            this.Stack = StackSize;
            this.bigCraftable.Value = true;
            this.furniture_type.Value = Furniture.other;
            this.Type = "interactive";
            this.name = this.Name;
            this.Fragility = 0;

            IReflectedField<int> placementRestriction = ModCore.ModHelper.Reflection.GetField<int>(this, "_placementRestriction");
            placementRestriction.SetValue(2);
        }

        /// <summary>
        /// What happens when the item is placed into the game world.
        /// </summary>
        /// <param name="who"></param>
        /// <returns></returns>
        public override bool performDropDownAction(Farmer who)
        {
            return false;
        }

        public override void actionOnPlayerEntry()
        {
            base.actionOnPlayerEntry();
        }

        public override void actionWhenBeingHeld(Farmer who)
        {
            base.actionWhenBeingHeld(who);
        }

        public override bool actionWhenPurchased()
        {
            return base.actionWhenPurchased();
        }

        public override void actionWhenStopBeingHeld(Farmer who)
        {
            base.actionWhenStopBeingHeld(who);
        }

        public override void ApplySprinkler(GameLocation location, Vector2 tile)
        {
            base.ApplySprinkler(location, tile);
        }

        public override bool canBeDropped()
        {
            return true;
        }

        public override bool canBeGivenAsGift()
        {
            return base.canBeGivenAsGift();
        }

        public override bool canBePlacedHere(GameLocation l, Vector2 tile)
        {

            return base.canBePlacedHere(l, tile);
        }

        public override bool canBePlacedInWater()
        {
            return base.canBePlacedInWater();
        }

        public override bool canBeShipped()
        {
            return base.canBeShipped();
        }

        public override int attachmentSlots()
        {
            return base.attachmentSlots();
        }

        public override bool canBeTrashed()
        {
            return base.canBeTrashed();
        }

        public override bool CanBuyItem(Farmer who)
        {
            return base.CanBuyItem(who);
        }

        /// <summary>
        /// Checks to see if the object is being interacted with.
        /// </summary>
        /// <param name="who"></param>
        /// <param name="justCheckingForActivity"></param>
        /// <returns></returns>
        public override bool checkForAction(Farmer who, bool justCheckingForActivity = false)
        {
            MouseState mState = Mouse.GetState();
            KeyboardState keyboardState = Game1.GetKeyboardState();

            ModCore.log("Check for action");

            if (mState.RightButton == ButtonState.Pressed && keyboardState.IsKeyDown(Keys.LeftShift) == false && keyboardState.IsKeyDown(Keys.RightShift) == false)
            {
                ModCore.log("Right clicked!");
                return this.rightClicked(who);
            }

            if (mState.RightButton == ButtonState.Pressed && (keyboardState.IsKeyDown(Keys.LeftShift) == true || keyboardState.IsKeyDown(Keys.RightShift) == true))
                return this.shiftRightClicked(who);

            return true;
        }

        public override string checkForSpecialItemHoldUpMeessage()
        {
            return base.checkForSpecialItemHoldUpMeessage();
        }

        public override bool clicked(Farmer who)
        {
            ModCore.log("Click the thing??");
            if (Game1.player.isInventoryFull())
            {
                return false;
            }
            else
            {

                this.performRemoveAction(this.TileLocation, this.getCurrentLocation());
                return true; //needs to be true to mark actually picking up the object? Also need to play sound?
            }

        }

        public override void DayUpdate(GameLocation location)
        {
            base.DayUpdate(location);
        }

        public override void farmerAdjacentAction(GameLocation location)
        {
            base.farmerAdjacentAction(location);
        }

        public override int GetBaseRadiusForSprinkler()
        {
            return base.GetBaseRadiusForSprinkler();
        }

        /// <summary>
        /// Category color.
        /// </summary>
        /// <returns></returns>
        public override Color getCategoryColor()
        {
            return this.basicItemInfo.categoryColor;
        }

        /// <summary>
        /// Category name
        /// </summary>
        /// <returns></returns>
        public override string getCategoryName()
        {
            return this.basicItemInfo.categoryName;
        }

        /// <summary>
        /// Hover box text
        /// </summary>
        /// <param name="hoveredItem"></param>
        /// <returns></returns>
        public override string getHoverBoxText(Item hoveredItem)
        {
            return base.getHoverBoxText(hoveredItem);
        }

        /// <summary>
        /// Description
        /// </summary>
        /// <returns></returns>
        public override string getDescription()
        {
            return this.basicItemInfo.description;
        }

        public override StardewValley.Object GetDeconstructorOutput(Item item)
        {
            return base.GetDeconstructorOutput(item);
        }

        public override Item getOne()
        {
            return new CustomObject(this.basicItemInfo.Copy());
        }

        public override void _GetOneFrom(Item source)
        {
            base._GetOneFrom(source);
        }

        public override int healthRecoveredOnConsumption()
        {
            return this.basicItemInfo.healthRestoredOnEating;
        }

        public override void hoverAction()
        {
            base.hoverAction();
        }

        public override void initializeLightSource(Vector2 tileLocation, bool mineShaft = false)
        {
            base.initializeLightSource(tileLocation, mineShaft);
        }

        public override bool isActionable(Farmer who)
        {
            return true;
        }

        public override bool isAnimalProduct()
        {
            return base.isAnimalProduct();
        }

        public override bool isForage(GameLocation location)
        {
            return base.isForage(location);
        }

        public override bool isPassable()
        {
            return false;
            if (this.basicItemInfo.ignoreBoundingBox) return true;
            return base.isPassable();
        }

        public override Rectangle getBoundingBox(Vector2 tileLocation)
        {

            Microsoft.Xna.Framework.Rectangle boundingBox = this.boundingBox.Value;
            Microsoft.Xna.Framework.Rectangle newBounds = boundingBox;
            newBounds.X = (int)tileLocation.X * Game1.tileSize;
            newBounds.Y = (int)tileLocation.Y * Game1.tileSize;

            newBounds.Width = Game1.tileSize * (int)this.basicItemInfo.boundingBoxTileDimensions.X;
            newBounds.Height = Game1.tileSize * (int)this.basicItemInfo.boundingBoxTileDimensions.Y;

            if (newBounds != boundingBox)
            {
                this.boundingBox.Set(boundingBox);
            }
            return newBounds;
        }

        public override List<Vector2> GetSprinklerTiles()
        {
            return base.GetSprinklerTiles();
        }

        public override bool isPlaceable()
        {
            return true;
        }


        public override bool IsSprinkler()
        {
            return base.IsSprinkler();
        }

        public override int maximumStackSize()
        {
            return base.maximumStackSize();
        }

        /// <summary>
        /// When so many minutes pass,update this object.
        /// </summary>
        /// <param name="minutes"></param>
        /// <param name="environment"></param>
        /// <returns></returns>
        public override bool minutesElapsed(int minutes, GameLocation environment)
        {
            return base.minutesElapsed(minutes, environment);
        }

        public override bool onExplosion(Farmer who, GameLocation location)
        {
            return base.onExplosion(who, location);
        }

        public override void onReadyForHarvest(GameLocation environment)
        {
            base.onReadyForHarvest(environment);
        }

        /// <summary>
        /// When the object is droped into (???) what happens?
        /// </summary>
        /// <param name="dropInItem"></param>
        /// <param name="probe"></param>
        /// <param name="who"></param>
        /// <returns></returns>
        public override bool performObjectDropInAction(Item dropInItem, bool probe, Farmer who)
        {
            return false;
            
        }

        /// <summary>
        /// When this object is removed what happens?
        /// </summary>
        /// <param name="tileLocation"></param>
        /// <param name="environment"></param>
        public override void performRemoveAction(Vector2 tileLocation, GameLocation environment)
        {
            this.cleanUpLights();



            this.removeLights(environment);
            if ((int)this.furniture_type == 14 || (int)this.furniture_type == 16)
            {
                base.isOn.Value = false;
                this.setFireplace(environment, playSound: false);
            }
            this.RemoveLightGlow(environment);

            environment.objects.Remove(this.TileLocation);
            environment.furniture.Remove(this);
            this.TileLocation = Vector2.Zero;
            this.basicItemInfo.locationName = "";
            this.boundingBox.Value = this.getBoundingBox(Vector2.Zero);

            this.sittingFarmers.Clear();
            this.removeAndAddToPlayersInventory();


            //base.performRemoveAction(tileLocation, environment);
        }

        /// <summary>
        /// When a tool is used on this item.
        /// </summary>
        /// <param name="t"></param>
        /// <param name="location"></param>
        /// <returns></returns>
        public override bool performToolAction(Tool t, GameLocation location)
        {
            if (t == null)
            {
                ModCore.log("Null tool used! Probably just the player's hands then.");

            }
            else
            {
                ModCore.log("Player used tool: " +t.DisplayName);
            }
            return false;
        }

        /// <summary>
        /// When this item is used. (Left clicked)
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        public override bool performUseAction(GameLocation location)
        {
            ModCore.log("Perform use action");
            return base.performUseAction(location);
        }
        public override bool placementAction(GameLocation location, int x, int y, Farmer who = null)
        {

            if (!this.isGroundFurniture())
            {
                y = this.GetModifiedWallTilePosition(location, x / 64, y / 64) * 64;
            }
            if (this.GetAdditionalFurniturePlacementStatus(location, x, y, who) != 0)
            {
                return false;
            }
            base.boundingBox.Value = new Rectangle(x / 64 * 64, y / 64 * 64, base.boundingBox.Width, base.boundingBox.Height);


            //EXPERIMENTAL: UPDATe THIS IN THE CONSTRUCTOR
            base.boundingBox.Value = new Rectangle(x / 64 * 64, y / 64 * 64, Game1.tileSize*1, Game1.tileSize * 2);

            this.updateDrawPosition();



            Vector2 placementTile = new Vector2(x / 64, y / 64);
            this.health = 10;
            if (who != null)
            {
                this.owner.Value = who.UniqueMultiplayerID;
            }
            else
            {
                this.owner.Value = Game1.player.UniqueMultiplayerID;
            }
            this.TileLocation = placementTile;


            location.furniture.Add(this);
            location.objects.Add(placementTile, this);
            location.playSound("thudStep");
            this.basicItemInfo.locationName = location.NameOrUniqueName;
            //location.playSound("stoneStep");
            return true;
            //Base code throws and error so I have to do it this way.
        }

        public override int salePrice()
        {
            return this.basicItemInfo.price;
        }

        public override int sellToStorePrice(long specificPlayerID = -1)
        {
            return this.basicItemInfo.price;
            return base.sellToStorePrice(specificPlayerID);
        }

        public override void reloadSprite()
        {
            base.reloadSprite();
        }

        public override int staminaRecoveredOnConsumption()
        {
            return this.basicItemInfo.staminaRestoredOnEating;
        }

        public override void updateWhenCurrentLocation(GameTime time, GameLocation environment)
        {
            if (this.shakeTimer > 0)
            {
                this.shakeTimer -= time.ElapsedGameTime.Milliseconds;
                if (this.shakeTimer <= 0)
                {
                    this.health = 10;
                }
            }
            
        }



        /// <summary>What happens when the player right clicks the object.</summary>
        public virtual bool rightClicked(Farmer who)
        {
            return true;
        }

        /// <summary>What happens when the player shift-right clicks this object.</summary>
        public virtual bool shiftRightClicked(Farmer who)
        {
            return true;
        }
        /// <summary>Remove the object from the world and add it to the player's inventory if possible.</summary>
        public virtual bool removeAndAddToPlayersInventory()
        {
            if (Game1.player.isInventoryFull())
            {
                Game1.showRedMessage("Inventory full.");
                return false;
            }
            this.basicItemInfo.locationName = "";
            Game1.player.addItemToInventory(this);
            //this.updateDrawPosition(0, 0);
            return true;
        }


        public virtual void setGameLocation(string LocationName, bool IsStructure)
        {
            this.basicItemInfo.locationName = LocationName;
            this.isCurrentLocationAStructure = IsStructure;
        }

        public virtual GameLocation getCurrentLocation()
        {
            if (string.IsNullOrEmpty(this.basicItemInfo.locationName))
            {
                return null;
            }
            else
            {
                return Game1.getLocationFromName(this.basicItemInfo.locationName, this.isCurrentLocationAStructure);
            }
        }


        public virtual void cleanUpLights()
        {
            if (this.GetLightManager() != null) this.GetLightManager().removeForCleanUp(this.getCurrentLocation());
        }

        public virtual BasicItemInformation getItemInformation()
        {
            return this.basicItemInfo;
        }

        public virtual void setItemInformation(BasicItemInformation Info)
        {
            this.basicItemInfo = Info;
        }


        public override bool canBeRemoved(Farmer who)
        {
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="DyeColor"></param>
        public virtual void dyeColor(NamedColor DyeColor)
        {
            this.basicItemInfo.dyedColor = DyeColor;
        }

        public virtual void eraseDye()
        {
            this.basicItemInfo.dyedColor = new NamedColor("", new Color(0, 0, 0, 0));
        }

        public override bool canStackWith(ISalable other)
        {
            if (other is CustomObject == false) return false;
            CustomObject o = (CustomObject)other;

            if (this.basicItemInfo.dyedColor != o.basicItemInfo.dyedColor) return false;
            if (this.basicItemInfo.id.Equals( o.basicItemInfo.id)==false) return false;

            return base.canStackWith(other);
        }

        

        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~//
        //                            Rendering code                   //
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~//

        public override void drawAsProp(SpriteBatch b)
        {
            base.drawAsProp(b);
        }

        public override void drawInMenu(SpriteBatch spriteBatch, Vector2 location, float scaleSize, float transparency, float layerDepth, StackDrawType drawStackNumber, Color color, bool drawShadow)
        {
            if (this.AnimationManager == null)
            {

                ModCore.log("Texture null for item: " + this.basicItemInfo.id);
                return;
            }
            if (this.CurrentTextureToDisplay == null)
            {
                ModCore.log("Texture null for item: " + this.basicItemInfo.id);
                return;
            }

            if (this.basicItemInfo == null) return;

            int scaleNerfing = Math.Max(this.AnimationManager.getCurrentAnimationFrameRectangle().Width, this.AnimationManager.getCurrentAnimationFrameRectangle().Height) / 16;

            spriteBatch.Draw(this.CurrentTextureToDisplay, location + new Vector2((float)(Game1.tileSize / 2), (float)(Game1.tileSize)), new Rectangle?(this.AnimationManager.getCurrentAnimationFrameRectangle()), this.basicItemInfo.DrawColor * transparency, 0f, new Vector2((float)(this.AnimationManager.getCurrentAnimationFrameRectangle().Width / 2), (float)(this.AnimationManager.getCurrentAnimationFrameRectangle().Height)), (scaleSize * 4f) / scaleNerfing, SpriteEffects.None, layerDepth);

            if (drawStackNumber.ShouldDrawFor(this) && this.maximumStackSize() > 1 && ((double)scaleSize > 0.3 && this.Stack != int.MaxValue) && this.Stack > 1)
                Utility.drawTinyDigits(this.Stack, spriteBatch, location + new Vector2((float)(Game1.tileSize - Utility.getWidthOfTinyDigitString(this.Stack, 3f * scaleSize)) + 3f * scaleSize, (float)((double)Game1.tileSize - 18.0 * (double)scaleSize + 2.0)), 3f * scaleSize, 1f, Color.White);
            if (drawStackNumber.ShouldDrawFor(this) && this.Quality > 0)
            {
                float num = this.Quality < 4 ? 0.0f : (float)((Math.Cos((double)Game1.currentGameTime.TotalGameTime.Milliseconds * Math.PI / 512.0) + 1.0) * 0.0500000007450581);
                spriteBatch.Draw(Game1.mouseCursors, location + new Vector2(12f, (float)(Game1.tileSize - 12) + num), new Microsoft.Xna.Framework.Rectangle?(this.Quality < 4 ? new Microsoft.Xna.Framework.Rectangle(338 + (this.Quality - 1) * 8, 400, 8, 8) : new Microsoft.Xna.Framework.Rectangle(346, 392, 8, 8)), Color.White * transparency, 0.0f, new Vector2(4f, 4f), (float)(3.0 * (double)scaleSize * (1.0 + (double)num)), SpriteEffects.None, layerDepth);
            }
        }

        public override void drawAttachments(SpriteBatch b, int x, int y)
        {
            base.drawAttachments(b, x, y);
        }

        public override void drawWhenHeld(SpriteBatch spriteBatch, Vector2 objectPosition, Farmer f)
        {
            if (this.AnimationManager == null)
            {
                if (this.CurrentTextureToDisplay == null)
                {
                    ModCore.log("Texture null for item: " + this.basicItemInfo.id);
                    return;
                }
            }

            if (f.ActiveObject.bigCraftable.Value)
            {
                spriteBatch.Draw(this.CurrentTextureToDisplay, objectPosition, this.AnimationManager.getCurrentAnimationFrameRectangle(), this.basicItemInfo.DrawColor, 0f, Vector2.Zero, (float)Game1.pixelZoom, SpriteEffects.None, Math.Max(0f, (float)(f.getStandingY() + 2) / 10000f));
                return;
            }

            spriteBatch.Draw(this.CurrentTextureToDisplay, objectPosition, this.AnimationManager.getCurrentAnimationFrameRectangle(), this.basicItemInfo.DrawColor, 0f, Vector2.Zero, (float)Game1.pixelZoom, SpriteEffects.None, Math.Max(0f, (float)(f.getStandingY() + 2) / 10000f));
            if (f.ActiveObject != null && f.ActiveObject.Name.Contains("="))
            {
                spriteBatch.Draw(this.CurrentTextureToDisplay, objectPosition + new Vector2((float)(Game1.tileSize / 2), (float)(Game1.tileSize / 2)), this.AnimationManager.getCurrentAnimationFrameRectangle(), Color.White, 0f, new Vector2((float)(Game1.tileSize / 2), (float)(Game1.tileSize / 2)), (float)Game1.pixelZoom + Math.Abs(Game1.starCropShimmerPause) / 8f, SpriteEffects.None, Math.Max(0f, (float)(f.getStandingY() + 2) / 10000f));
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

        /// <summary>What happens when the object is drawn at a tile location.</summary>
        public override void draw(SpriteBatch spriteBatch, int x, int y, float alpha = 1f)
        {


            if (x <= -1)
            {
                return;
                //spriteBatch.Draw(this.basicItemInfo.animationManager.getTexture(), Game1.GlobalToLocal(Game1.viewport, this.TileLocation), new Rectangle?(this.AnimationManager.currentAnimation.sourceRectangle), this.basicItemInfo.DrawColor * alpha, 0f, Vector2.Zero, (float)Game1.pixelZoom, this.flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None, Math.Max(0f, (float)(this.TileLocation.Y * Game1.tileSize) / 10000f));
            }
            else
            {
                if (this.AnimationManager == null)
                {
                    if (this.CurrentTextureToDisplay == null)
                    {
                        ModCore.log("Texture null for item: " + this.basicItemInfo.id);
                        return;
                    }
                }
                //The actual planter box being drawn.
                if (this.AnimationManager == null)
                {
                    if (this.AnimationManager.getExtendedTexture() == null)
                        ModCore.ModMonitor.Log("Tex Extended is null???");

                    spriteBatch.Draw(this.CurrentTextureToDisplay, Game1.GlobalToLocal(Game1.viewport, new Vector2((float)(x * Game1.tileSize), y * Game1.tileSize)), new Rectangle?(this.AnimationManager.getCurrentAnimationFrameRectangle()), this.basicItemInfo.DrawColor * alpha, 0f, Vector2.Zero, (float)Game1.pixelZoom, this.flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None, Math.Max(0f, (float)(this.TileLocation.Y * Game1.tileSize) / 10000f));
                    // Log.AsyncG("ANIMATION IS NULL?!?!?!?!");
                }

                else
                {
                    //Log.AsyncC("Animation Manager is working!");
                    int addedDepth = 0;
                    if (this.basicItemInfo.ignoreBoundingBox) addedDepth++;
                    if (Revitalize.ModCore.playerInfo.sittingInfo.SittingObject == this) addedDepth++;
                    this.AnimationManager.draw(spriteBatch, this.CurrentTextureToDisplay, Game1.GlobalToLocal(Game1.viewport, new Vector2((float)(x * Game1.tileSize), y * Game1.tileSize)), new Rectangle?(this.AnimationManager.getCurrentAnimationFrameRectangle()), this.basicItemInfo.DrawColor * alpha, 0f, Vector2.Zero, (float)Game1.pixelZoom, this.flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None, Math.Max(0f, (float)((this.TileLocation.Y + addedDepth) * Game1.tileSize) / 10000f));
                }

                try
                {
                    this.AnimationManager.tickAnimation();
                    // Log.AsyncC("Tick animation");
                }
                catch (Exception err)
                {
                    ModCore.ModMonitor.Log(err.ToString());
                }

                // spriteBatch.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2((float)((double)tileLocation.X * (double)Game1.tileSize + (((double)tileLocation.X * 11.0 + (double)tileLocation.Y * 7.0) % 10.0 - 5.0)) + (float)(Game1.tileSize / 2), (float)((double)tileLocation.Y * (double)Game1.tileSize + (((double)tileLocation.Y * 11.0 + (double)tileLocation.X * 7.0) % 10.0 - 5.0)) + (float)(Game1.tileSize / 2))), new Rectangle?(new Rectangle((int)((double)tileLocation.X * 51.0 + (double)tileLocation.Y * 77.0) % 3 * 16, 128 + this.whichForageCrop * 16, 16, 16)), Color.White, 0.0f, new Vector2(8f, 8f), (float)Game1.pixelZoom, SpriteEffects.None, (float)(((double)tileLocation.Y * (double)Game1.tileSize + (double)(Game1.tileSize / 2) + (((double)tileLocation.Y * 11.0 + (double)tileLocation.X * 7.0) % 10.0 - 5.0)) / 10000.0));
            }
        }

        public override void drawPlacementBounds(SpriteBatch spriteBatch, GameLocation location)
        {
            //Need to update this????
            base.drawPlacementBounds(spriteBatch, location);
        }

        /// <summary>Draw the game object at a non-tile spot. Aka like debris.</summary>
        public override void draw(SpriteBatch spriteBatch, int xNonTile, int yNonTile, float layerDepth, float alpha = 1f)
        {
            if (this.AnimationManager == null)
            {
                if (this.CurrentTextureToDisplay == null)
                {
                    ModCore.log("Texture null for item: " + this.basicItemInfo.id);
                    return;
                }
            }

            ModCore.log("Pos is: " + new Vector2(xNonTile, yNonTile));

            //The actual planter box being drawn.
            if (this.AnimationManager == null)
            {
                if (this.AnimationManager.getExtendedTexture() == null)
                    ModCore.ModMonitor.Log("Tex Extended is null???");

                spriteBatch.Draw(this.CurrentTextureToDisplay, Game1.GlobalToLocal(Game1.viewport, new Vector2((float)(xNonTile), yNonTile)), new Rectangle?(this.AnimationManager.getCurrentAnimationFrameRectangle()), this.basicItemInfo.DrawColor * alpha, 0f, Vector2.Zero, (float)Game1.pixelZoom, this.Flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None, Math.Max(0f, layerDepth));
                // Log.AsyncG("ANIMATION IS NULL?!?!?!?!");
            }

            else
            {
                //Log.AsyncC("Animation Manager is working!");
                int addedDepth = 0;
                if (this.basicItemInfo.ignoreBoundingBox) addedDepth++;
                if (Revitalize.ModCore.playerInfo.sittingInfo.SittingObject == this) addedDepth++;
                this.AnimationManager.draw(spriteBatch, this.CurrentTextureToDisplay, Game1.GlobalToLocal(Game1.viewport, new Vector2((float)(xNonTile), yNonTile)), new Rectangle?(this.AnimationManager.getCurrentAnimationFrameRectangle()), this.basicItemInfo.DrawColor * alpha, 0f, Vector2.Zero, (float)Game1.pixelZoom, this.Flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None, Math.Max(0f, layerDepth));
                try
                {
                    this.AnimationManager.tickAnimation();
                    // Log.AsyncC("Tick animation");
                }
                catch (Exception err)
                {
                    ModCore.ModMonitor.Log(err.ToString());
                }
            }

            // spriteBatch.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2((float)((double)tileLocation.X * (double)Game1.tileSize + (((double)tileLocation.X * 11.0 + (double)tileLocation.Y * 7.0) % 10.0 - 5.0)) + (float)(Game1.tileSize / 2), (float)((double)tileLocation.Y * (double)Game1.tileSize + (((double)tileLocation.Y * 11.0 + (double)tileLocation.X * 7.0) % 10.0 - 5.0)) + (float)(Game1.tileSize / 2))), new Rectangle?(new Rectangle((int)((double)tileLocation.X * 51.0 + (double)tileLocation.Y * 77.0) % 3 * 16, 128 + this.whichForageCrop * 16, 16, 16)), Color.White, 0.0f, new Vector2(8f, 8f), (float)Game1.pixelZoom, SpriteEffects.None, (float)(((double)tileLocation.Y * (double)Game1.tileSize + (double)(Game1.tileSize / 2) + (((double)tileLocation.Y * 11.0 + (double)tileLocation.X * 7.0) % 10.0 - 5.0)) / 10000.0));

        }


        public virtual LightManager GetLightManager()
        {
            return this.basicItemInfo.lightManager;
        }


    }
}
