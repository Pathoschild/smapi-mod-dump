using AdditionalCropsFramework.Framework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json.Linq;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Objects;
using StardustCore;
using StardustCore.Animations;
using StardustCore.UIUtilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace AdditionalCropsFramework
{
    /// <summary>
    /// Original Stardew Furniture Class but rewritten to be placed anywhere.
    /// </summary>
    public class PlanterBox : CoreObject
    {

        public Texture2DExtended TextureSheet;

        public new bool flipped;

        [XmlIgnore]
        public bool flaggedForPickUp;

        private bool lightGlowAdded;

        public string texturePath;
        public string dataPath;

        public bool IsSolid;

        public Crop crop;
        public ModularCrop modularCrop;
        public bool isWatered;
        public string cropInformationString;

        public string normalCropSeedName;
        public int normalCropSeedIndex;

        public bool greenHouseEffect;
        public bool selfWatering;

        public override string Name
        {
            get
            {
                return this.name;
            }
        }

        public PlanterBox()
        {
            this.updateDrawPosition();
        }

        public PlanterBox(bool f)
        {
            //does nothng
        }

        /// <summary>
        /// Don't use this unless you just want to lol with some defaults.
        /// </summary>
        /// <param name="which"></param>
        /// <param name="tile"></param>
        /// <param name="isRemovable"></param>
        /// <param name="price"></param>
        /// <param name="isSolid"></param>
        public PlanterBox(IModHelper modHelper,int which, Vector2 tile, bool isRemovable = true, int price = 0, bool isSolid = false)
        {
            this.cropInformationString = "";
            removable = isRemovable;
            this.serializationName =Convert.ToString(GetType());
            // this.thisType = GetType();
            this.TileLocation = tile;
            this.InitializeBasics(0, tile);
            if (TextureSheet == null)
            {
                Texture2D text = ModCore.ModHelper.Content.Load<Texture2D>(Path.Combine(Utilities.EntensionsFolderName, "PlanterBox.png")); //Game1.content.Load<Texture2D>("TileSheets\\furniture");
                this.TextureSheet = new Texture2DExtended(modHelper, Path.Combine(Utilities.EntensionsFolderName, "PlanterBox.png"));
                texturePath = "PlanterBoxGraphic";
            }
            dataPath = "";

            this.name = "Planter Box";
            this.description = "A planter box that can be used to grow many different crops in many different locations.";
            this.defaultSourceRect = new Rectangle(which * 16 % TextureSheet.texture.Width, which * 16 / TextureSheet.texture.Width * 16, 1, 1);

            this.defaultSourceRect.Width = 1;
            this.defaultSourceRect.Height = 1;
            this.sourceRect = new Rectangle(which * 16 % TextureSheet.texture.Width, which * 16 / TextureSheet.texture.Width * 16, this.defaultSourceRect.Width * 16, this.defaultSourceRect.Height * 16);
            this.defaultSourceRect = this.sourceRect;

            this.animationManager = new StardustCore.Animations.AnimationManager(this.TextureSheet, new StardustCore.Animations.Animation(this.defaultSourceRect, -1));

            this.defaultBoundingBox = new Rectangle((int)this.TileLocation.X, (int)this.TileLocation.Y, 1, 1);

            this.defaultBoundingBox.Width = 1;
            this.defaultBoundingBox.Height = 1;
            IsSolid = isSolid;
            if (isSolid == true)
            {
                this.boundingBox.Value = new Rectangle((int)this.TileLocation.X * Game1.tileSize, (int)this.TileLocation.Y * Game1.tileSize, this.defaultBoundingBox.Width * Game1.tileSize, this.defaultBoundingBox.Height * Game1.tileSize);
            }
            else
            {
                this.boundingBox.Value = new Rectangle(int.MinValue, (int)this.TileLocation.Y * Game1.tileSize, 0, 0); //Throw the bounding box away as far as possible.
            }
         //   this.cropBoundingBox = new Rectangle((int)this.TileLocation.X * Game1.tileSize, (int)this.TileLocation.Y * Game1.tileSize, this.defaultBoundingBox.Width * Game1.tileSize, this.defaultBoundingBox.Height * Game1.tileSize);
            this.defaultBoundingBox = this.boundingBox.Value;
            this.updateDrawPosition();
            this.Price = price;
            this.ParentSheetIndex = which;
        }

        /// <summary>
        /// Planterbox Constructor. Does not use an animation manager.
        /// </summary>
        /// <param name="which"></param>
        /// <param name="tile"></param>
        /// <param name="ObjectTexture"></param>
        /// <param name="isRemovable"></param>
        /// <param name="price"></param>
        /// <param name="isSolid"></param>
        public PlanterBox(IModHelper helper,int which, Vector2 tile, string ObjectTexture, bool isRemovable = true, int price = 0, bool isSolid = false)
        {
      
            this.cropInformationString = "";
            removable = isRemovable;
            this.serializationName = Convert.ToString(GetType());
            // this.thisType = GetType();
            this.TileLocation = tile;
            this.InitializeBasics(0, tile);
            if (TextureSheet == null)
            {
                Texture2D text = ModCore.ModHelper.Content.Load<Texture2D>(Path.Combine(Utilities.EntensionsFolderName, ObjectTexture)); //Game1.content.Load<Texture2D>("TileSheets\\furniture");
               
                this.TextureSheet = new Texture2DExtended(helper, Path.Combine(Utilities.EntensionsFolderName, ObjectTexture));

                texturePath = ObjectTexture;
            }
            this.dataPath = "";

            this.name = "Planter Box";
            this.description = "A planter box that can be used to grow many different crops in many different locations.";

          

            this.defaultSourceRect = new Rectangle(which * 16 % TextureSheet.texture.Width, which * 16 / TextureSheet.texture.Width * 16, 1, 1);

            this.defaultSourceRect.Width = 1;
            this.defaultSourceRect.Height = 1;
            this.sourceRect = new Rectangle(which * 16 % TextureSheet.texture.Width, which * 16 / TextureSheet.texture.Width * 16, this.defaultSourceRect.Width * 16, this.defaultSourceRect.Height * 16);
            this.defaultSourceRect = this.sourceRect;

            this.defaultBoundingBox = new Rectangle((int)this.TileLocation.X, (int)this.TileLocation.Y, 1, 1);

            this.defaultBoundingBox.Width = 1;
            this.defaultBoundingBox.Height = 1;
            IsSolid = isSolid;
            if (isSolid == true)
            {
                this.boundingBox.Value = new Rectangle((int)this.TileLocation.X * Game1.tileSize, (int)this.TileLocation.Y * Game1.tileSize, this.defaultBoundingBox.Width * Game1.tileSize, this.defaultBoundingBox.Height * Game1.tileSize);
            }
            else
            {
                this.boundingBox.Value = new Rectangle(int.MinValue, (int)this.TileLocation.Y * Game1.tileSize, 0, 0); //Throw the bounding box away as far as possible.
            }
            this.defaultBoundingBox = this.boundingBox.Value;
            this.updateDrawPosition();
            this.Price = price;
            this.ParentSheetIndex = which;
        }


        public PlanterBox(IModHelper helper,int which, Vector2 tile, string ObjectTexture, string DataPath, bool isRemovable = true, bool isSolid = false)
        {
            this.cropInformationString = "";
            this.serializationName = Convert.ToString(GetType());
            removable = isRemovable;
            // this.thisType = GetType();
            this.TileLocation = tile;
            this.InitializeBasics(0, tile);
            Texture2D text = ModCore.ModHelper.Content.Load<Texture2D>(Path.Combine(Utilities.EntensionsFolderName, ObjectTexture)); //Game1.content.Load<Texture2D>("TileSheets\\furniture");
            TextureSheet = new Texture2DExtended(helper, Path.Combine(Utilities.EntensionsFolderName, ObjectTexture));
            texturePath = ObjectTexture;
            Dictionary<int, string> dictionary;
            try
            {
                
               dictionary = ModCore.ModHelper.Content.Load<Dictionary<int, string>>(Path.Combine(Utilities.EntensionsFolderName, DataPath));
               dataPath = DataPath;
                

                string s = "";
                dictionary.TryGetValue(which, out s);
                string[] array = s.Split('/');
                this.name = array[0];
                this.description = array[1];
                this.defaultSourceRect = new Rectangle(which * 16 % TextureSheet.texture.Width, which * 16 / TextureSheet.texture.Width * 16, 1, 1);

                this.defaultSourceRect.Width = 1;
                this.defaultSourceRect.Height = 1;
                this.sourceRect = new Rectangle(which * 16 % TextureSheet.texture.Width, which * 16 / TextureSheet.texture.Width * 16, this.defaultSourceRect.Width * 16, this.defaultSourceRect.Height * 16);
                this.defaultSourceRect = this.sourceRect;
                try
                {                
                    this.animationManager = new StardustCore.Animations.AnimationManager(this.TextureSheet, new StardustCore.Animations.Animation(this.defaultSourceRect, -1), AnimationManager.parseAnimationsFromXNB(array[3]), "Default");
                    this.animationManager.setAnimation("Default", 0);
                    //Log.AsyncC("Using animation manager");
                }
                catch (Exception errr)
                {
                    errr.ToString();
                    this.animationManager = new StardustCore.Animations.AnimationManager(this.TextureSheet, new StardustCore.Animations.Animation(this.defaultSourceRect, -1));
                }
                this.defaultBoundingBox = new Rectangle((int)this.TileLocation.X, (int)this.TileLocation.Y, 1, 1);

                this.defaultBoundingBox.Width = 1;
                this.defaultBoundingBox.Height = 1;
                IsSolid = isSolid;
                if (isSolid == true)
                {
                    this.boundingBox.Value = new Rectangle((int)this.TileLocation.X * Game1.tileSize, (int)this.TileLocation.Y * Game1.tileSize, this.defaultBoundingBox.Width * Game1.tileSize, this.defaultBoundingBox.Height * Game1.tileSize);
                }
                else
                {
                    this.boundingBox.Value = new Rectangle(int.MinValue, (int)this.TileLocation.Y * Game1.tileSize, 0, 0); //Throw the bounding box away as far as possible.
                }
                this.defaultBoundingBox = this.boundingBox.Value;
                this.updateDrawPosition();
                this.Price = Convert.ToInt32(array[2]);
                this.ParentSheetIndex = which;

                try
                {
                    this.selfWatering = Convert.ToBoolean(array[4]);
                }
                catch(Exception e)
                {
                    e.ToString();
                    this.selfWatering = false;
                }
                try
                {
                    this.greenHouseEffect = Convert.ToBoolean(array[5]);
                }
                catch (Exception e)
                {
                    e.ToString();
                    this.greenHouseEffect = false;
                }
                
            }
            catch(Exception e)
            {
                e.ToString();
              //  Log.AsyncC(e);
            }
           
        
        }


        public override string getDescription()
        {
            return this.description;
        }

        public override bool performDropDownAction(StardewValley.Farmer who)
        {
            this.resetOnPlayerEntry((who == null) ? Game1.currentLocation : who.currentLocation);
            return false;
        }

        public override void hoverAction()
        {
            base.hoverAction();
            if (!Game1.player.isInventoryFull())
            {
                Game1.mouseCursor = 2;
            }
            if (this.crop != null)
            {
                if (Utilities.isCropFullGrown(this.crop))
                {
                    StardustCore.Utilities.drawGreenPlus();
                }
            }
            if (this.modularCrop != null)
            {
        
                if (this.modularCrop.isFullyGrown())
                {
                    StardustCore.Utilities.drawGreenPlus();
                }
            }
            
        }

        public override bool checkForAction(StardewValley.Farmer who, bool justCheckingForActivity = false)
        {
            var mState = Microsoft.Xna.Framework.Input.Mouse.GetState();
            if (mState.RightButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
            {
                // Game1.showRedMessage("YOOO");
                //do some stuff when the right button is down
                // rotate();
                if (Game1.player.ActiveObject != null)
                {
                    if (Game1.player.ActiveObject is ModularSeeds || Game1.player.ActiveObject.getCategoryName() == "Modular Seeds")
                    {
                        this.plantModdedCrop((Game1.player.ActiveObject as ModularSeeds));
                        // Log.AsyncO("Modded seeds");
                    }
                    // Log.AsyncO(Game1.player.CurrentItem.getCategoryName());
                    if (Game1.player.CurrentItem.getCategoryName() == "Seed" || Game1.player.CurrentItem.getCategoryName() == "seed")
                    {
                        this.plantRegularCrop();
                        // Log.AsyncY("regular seeds");
                    }
                }
                if (this.crop != null)
                {
                    if (Utilities.isCropFullGrown(this.crop) == true)
                    {
                        //this.crop.harvest();
                       bool f= Utilities.harvestCrop(this.crop, (int)this.TileLocation.X, (int)this.TileLocation.Y, 0);
                        if (f == true && this.crop.regrowAfterHarvest.Value == -1) this.crop = null;
                    }
                }
                if (this.modularCrop != null)
                {
                
                    if (this.modularCrop.isFullyGrown() == true)
                    {
                 
                        bool f = Utilities.harvestModularCrop(this.modularCrop, (int)this.TileLocation.X, (int)this.TileLocation.Y, 0);
                        if (f == true)
                        {
                            //this.modularCrop = null;
                            if (f == true && this.modularCrop.regrowAfterHarvest == -1) this.modularCrop = null;
                         
                            return true;
                        }
                        else
                        {
                           // Log.AsyncC("failed to harvest crop. =/");
                        }
                    }
                }
                return true;
            }
            else
            {
                //Game1.showRedMessage("CRY");
            }

            if (justCheckingForActivity)
            {
                return true;
            }
            if (this.ParentSheetIndex == 1402)
            {
                Game1.activeClickableMenu = new Billboard(false);
            }
            return this.clicked(who); //check for left clicked action.
        }

        public void plantModdedCrop(ModularSeeds seeds)
        {
          if (this.modularCrop != null) return;
          this.modularCrop = new ModularCrop(this.TextureSheet.getHelper(),seeds.ParentSheetIndex, (int)Game1.currentCursorTile.X, (int)Game1.currentCursorTile.Y, seeds.cropDataFilePath, seeds.cropTextureFilePath, seeds.cropObjectTextureFilePath, seeds.cropObjectDataFilePath);
          Game1.player.reduceActiveItemByOne();
          Game1.playSound("dirtyHit");
        }

        public void plantRegularCrop()
        {
            if (this.crop != null) return;
            this.normalCropSeedName = Game1.player.CurrentItem.Name;
            this.normalCropSeedIndex = Game1.player.CurrentItem.ParentSheetIndex;

            try
            {
                this.crop = new Crop(Game1.player.CurrentItem.ParentSheetIndex, (int)Game1.currentCursorTile.X, (int)Game1.currentCursorTile.Y);
            }
            catch(Exception e)
            {
                //   Log.AsyncM(e);
                e.ToString();
            }

            foreach (var v in this.crop.phaseDays)
            {
              //  Log.AsyncC("I grow! " + v);
            }
            Game1.player.reduceActiveItemByOne();
            Game1.playSound("dirtyHit");
        }



        public override bool clicked(StardewValley.Farmer who)
        {
            int range = 2;
            if (StardustCore.Utilities.isWithinRange(range, this.TileLocation) == false) return false;

            if (StardustCore.Utilities.isWithinDirectionRange(Game1.player.FacingDirection, range, this.TileLocation))
            {
                if (Game1.player.CurrentItem != null)
                {
                    if (Game1.player.getToolFromName(Game1.player.CurrentItem.Name) is StardewValley.Tools.WateringCan)
                    {
                        this.isWatered = true;
                        this.animationManager.setAnimation("Watered", 0);
                        return false;
                    }
                }

                if (Game1.player.CurrentItem != null)
                {
                    if (Game1.player.CurrentItem is StardewValley.Tools.MeleeWeapon || Game1.player.CurrentItem is StardewValley.Tools.Sword)
                    {
                        if (this.modularCrop != null)
                        {
                            if (this.modularCrop.dead == true)
                            {
                                this.modularCrop = null;
                                return false;
                            }
                        }
                        if (this.crop != null)
                        {
                            if (this.crop.dead.Value == true)
                            {
                                this.crop = null;
                                return false;
                            }
                        }
                    }
                }
            }
            



            if (removable == false) return false;
            //   Game1.showRedMessage("THIS IS CLICKED!!!");
            Game1.haltAfterCheck = false;
            if (this.Decoration_type == 11 && who.ActiveObject != null && who.ActiveObject != null && this.heldObject.Value == null)
            {
                //  Game1.showRedMessage("Why1?");
                return false;
            }
            if (this.heldObject.Value == null && (who.ActiveObject == null || !(who.ActiveObject is PlanterBox)))
            {
                if (Game1.player.currentLocation is FarmHouse)
                {
                    //   Game1.showRedMessage("Why2?");
                    // this.heldObject = new PlanterBox(parentSheetIndex, Vector2.Zero);
                    Utilities.addItemToInventoryAndCleanTrackedList(this);
                    this.flaggedForPickUp = true;
                    this.thisLocation = null;
                    this.locationsName = "";
                    return true;
                }
                else
                {
                    // return true;

                    this.flaggedForPickUp = true;

                        //  this.heldObject = new PlanterBox(parentSheetIndex, Vector2.Zero);
                        Utilities.addItemToInventoryAndCleanTrackedList(this);
                        //  this.heldObject.performRemoveAction(this.TileLocation, who.currentLocation);
                        //   this.heldObject = null;
                        Game1.playSound("coin");
                        this.thisLocation = null;
                        this.locationsName = "";
                    return true;

                }
            }
            if (this.heldObject.Value != null && who.addItemToInventoryBool(this.heldObject.Value, false))
            {
                // Game1.showRedMessage("Why3?");
                this.heldObject.Value.performRemoveAction(this.TileLocation, who.currentLocation);
                this.heldObject.Value = null;
                Utilities.addItemToInventoryAndCleanTrackedList(this);
                Game1.playSound("coin");
                this.thisLocation = null;
                this.locationsName = "";
                return true;
            }



            return false;
        }

        public override void DayUpdate(GameLocation location)
        {
            base.DayUpdate(location);
            this.lightGlowAdded = false;
            if (!Game1.isDarkOut() || (Game1.newDay && !Game1.isRaining))
            {
                this.removeLights(location);
                return;
            }
            this.addLights(location);

        }

        public void dayUpdate()
        {

            if (ModCore.ModConfig.removeCropsDayofDying == false)
            {
                if (this.crop != null)
                {
                    if (this.crop.dead.Value) this.crop = null;

                }
            }

            if (this.modularCrop != null)
            {
                if (this.modularCrop.dead) this.modularCrop = null;
            }


            if (this.selfWatering == true ||(this.thisLocation.IsOutdoors && Game1.isRaining))
            {
                this.isWatered = true;
                this.animationManager.setAnimation("Watered", 0);
            }


            if (this.isWatered==true)
            {
                if (this.crop != null)
                {
                    Utilities.cropNewDay(this,this.crop,1, 0, (int)this.TileLocation.X, (int)this.TileLocation.Y, this.thisLocation);
                   
                }

                if (this.modularCrop != null)
                {
                   Utilities.cropNewDayModded(this,this.modularCrop,1, 0, (int)this.TileLocation.X, (int)this.TileLocation.Y, this.thisLocation);
                }
                if (this.selfWatering == false)
                {
                    this.isWatered = false;
                    this.animationManager.setAnimation("Default", 0);
                }
            }
            else //if planterbox isn't watered
            {
                if (this.crop != null)
                {
                    Utilities.cropNewDay(this, this.crop, 0, 0, (int)this.TileLocation.X, (int)this.TileLocation.Y, this.thisLocation);

                }

                if (this.modularCrop != null)
                {
                    Utilities.cropNewDayModded(this, this.modularCrop, 0, 0, (int)this.TileLocation.X, (int)this.TileLocation.Y, this.thisLocation);
                }
                if (this.selfWatering == false)
                {
                    this.isWatered = false;
                    this.animationManager.setAnimation("Default", 0);
                }
            }
            //Update ticks occ
            if (ModCore.ModConfig.removeCropsDayofDying == true)
            {
                if (this.crop != null)
                {
                    if (this.crop.dead.Value) this.crop = null;

                }
            }


        }

        public override void resetOnPlayerEntry(GameLocation environment)
        {
            this.removeLights(environment);
            if (Game1.isDarkOut())
            {
                this.addLights(environment);
            }
        }

        public override bool performObjectDropInAction(Item dropIn, bool probe, StardewValley.Farmer who)
        {
            return base.performObjectDropInAction(dropIn, probe, who);
        }

        public override void addLights(GameLocation environment)
        {
            // this.lightSource.lightTexture = Game1.content.Load<Texture2D>("LooseSprites\\Lighting\\lantern");

            if (this.Decoration_type == 7)
            {
                if (this.sourceIndexOffset == 0)
                {
                    this.sourceRect = this.defaultSourceRect;
                    this.sourceRect.X = this.sourceRect.X + this.sourceRect.Width;
                }
                this.sourceIndexOffset = 1;
                if (this.lightSource == null)
                {
                    Utility.removeLightSource((int)(this.TileLocation.X * 2000f + this.TileLocation.Y));
                    this.lightSource = new LightSource(4, new Vector2((float)(this.boundingBox.X + Game1.tileSize / 2), (float)(this.boundingBox.Y - Game1.tileSize)), 2f, Color.Black, (int)(this.TileLocation.X * 2000f + this.TileLocation.Y));
                    Game1.currentLightSources.Add(this.lightSource);
                    return;
                }
            }
            else if (this.Decoration_type == 13)
            {
                if (this.sourceIndexOffset == 0)
                {
                    this.sourceRect = this.defaultSourceRect;
                    this.sourceRect.X = this.sourceRect.X + this.sourceRect.Width;
                }
                this.sourceIndexOffset = 1;
                if (this.lightGlowAdded)
                {
                    environment.lightGlows.Remove(new Vector2((float)(this.boundingBox.X + Game1.tileSize / 2), (float)(this.boundingBox.Y + Game1.tileSize)));
                    this.lightGlowAdded = false;
                }
            }
        }

        public override bool minutesElapsed(int minutes, GameLocation environment)
        {
            if (Game1.isDarkOut())
            {
                this.addLights(environment);
            }
            else
            {
                this.removeLights(environment);
            }
            return false;
        }

        public override void performRemoveAction(Vector2 tileLocation, GameLocation environment)
        {
            this.removeLights(environment);
            if (this.Decoration_type == 13 && this.lightGlowAdded)
            {
                environment.lightGlows.Remove(new Vector2((float)(this.boundingBox.X + Game1.tileSize / 2), (float)(this.boundingBox.Y + Game1.tileSize)));
                this.lightGlowAdded = false;
            }
            base.performRemoveAction(tileLocation, environment);
        }
        public override bool isGroundFurniture()
        {
            return this.Decoration_type != 13 && this.Decoration_type != 6 && this.Decoration_type != 13;
        }

        public override bool canBeGivenAsGift()
        {
            return false;
        }

        public override bool canBePlacedHere(GameLocation l, Vector2 tile)
        {
            if ((l is FarmHouse))
            {
                for (int i = 0; i < this.boundingBox.Width / Game1.tileSize; i++)
                {
                    for (int j = 0; j < this.boundingBox.Height / Game1.tileSize; j++)
                    {
                        Vector2 vector = tile * (float)Game1.tileSize + new Vector2((float)i, (float)j) * (float)Game1.tileSize;
                        vector.X += (float)(Game1.tileSize / 2);
                        vector.Y += (float)(Game1.tileSize / 2);
                        foreach (KeyValuePair<Vector2, StardewValley.Object> something in l.objects.Pairs)
                        {
                            StardewValley.Object obj = something.Value;
                            if ((obj.GetType()).ToString().Contains("PlanterBox"))
                            {
                                PlanterBox current = (PlanterBox)obj;
                                if (current.Decoration_type == 11 && current.getBoundingBox(current.TileLocation).Contains((int)vector.X, (int)vector.Y) && current.heldObject.Value == null && this.getTilesWide() == 1)
                                {
                                    bool result = true;
                                    return result;
                                }
                                if ((current.Decoration_type != 12 || this.Decoration_type == 12) && current.getBoundingBox(current.TileLocation).Contains((int)vector.X, (int)vector.Y))
                                {
                                    bool result = false;
                                    return result;
                                }
                            }
                        }
                    }
                }
                return base.canBePlacedHere(l, tile);
            }
            else
            {
                // Game1.showRedMessage("NOT FARMHOUSE");
                for (int i = 0; i < this.boundingBox.Width / Game1.tileSize; i++)
                {
                    for (int j = 0; j < this.boundingBox.Height / Game1.tileSize; j++)
                    {
                        Vector2 vector = tile * (float)Game1.tileSize + new Vector2((float)i, (float)j) * (float)Game1.tileSize;
                        vector.X += (float)(Game1.tileSize / 2);
                        vector.Y += (float)(Game1.tileSize / 2);
                        /*
                        foreach (PlanterBox current in (l as FarmHouse).PlanterBox)
                        {
                            if (current.Decoration_type == 11 && current.getBoundingBox(current.tileLocation).Contains((int)vector.X, (int)vector.Y) && current.heldObject == null && this.getTilesWide() == 1)
                            {
                                bool result = true;
                                return result;
                            }
                            if ((current.Decoration_type != 12 || this.Decoration_type == 12) && current.getBoundingBox(current.tileLocation).Contains((int)vector.X, (int)vector.Y))
                            {
                                bool result = false;
                                return result;
                            }
                        }
                        */
                    }
                }
                return base.canBePlacedHere(l, tile);
            }
        }

        public override void updateDrawPosition()
        {
            this.drawPosition = new Vector2((float)this.boundingBox.X, (float)(this.boundingBox.Y - (this.sourceRect.Height * Game1.pixelZoom - this.boundingBox.Height)));
        }

        public override int getTilesWide()
        {
            return this.boundingBox.Width / Game1.tileSize;
        }

        public override int getTilesHigh()
        {
            return this.boundingBox.Height / Game1.tileSize;
        }

        public override bool placementAction(GameLocation location, int x, int y, StardewValley.Farmer who = null)
        {

            Point point = new Point(x / Game1.tileSize, y / Game1.tileSize);


            this.TileLocation = new Vector2((float)point.X, (float)point.Y);

            if (this.IsSolid)
            {
                this.boundingBox.Value = new Rectangle(x / Game1.tileSize * Game1.tileSize, y / Game1.tileSize * Game1.tileSize, this.boundingBox.Width, this.boundingBox.Height);
            }
            else
            {
                this.boundingBox.Value = new Rectangle(int.MinValue, y / Game1.tileSize * Game1.tileSize, 0, 0);
            }
            
            /*
        foreach (Furniture current2 in (location as DecoratableLocation).furniture)
            {
                if (current2.furniture_type == 11 && current2.heldObject == null && current2.getBoundingBox(current2.tileLocation).Intersects(this.boundingBox))
                {
                    current2.performObjectDropInAction(this, false, (who == null) ? Game1.player : who);
                    bool result = true;
                    return result;
                }
            }
            */
            using (List<StardewValley.Farmer>.Enumerator enumerator3 = location.getFarmers().GetEnumerator())
            {
                while (enumerator3.MoveNext())
                {
                    if (enumerator3.Current.GetBoundingBox().Intersects(this.boundingBox.Value))
                    {
                        Game1.showRedMessage("Can't place on top of a person.");
                        bool result = false;
                        return result;
                    }
                }
            }
            this.updateDrawPosition();

            bool f=Utilities.placementAction(this, location, x, y, who);
            this.thisLocation = Game1.player.currentLocation;
            return f;
            //  Game1.showRedMessage("Can only be placed in House");
            //  return false;
        }

        public override bool isPlaceable()
        {
            return true;
        }

        public override Rectangle getBoundingBox(Vector2 tileLocation)
        {
            return this.boundingBox.Value;
        }

        public override int salePrice()
        {
            return this.Price;
        }

        public override int maximumStackSize()
        {
            return 1;
        }

        public override int getStack()
        {
            return this.Stack;
        }

        public override int addToStack(int amount)
        {
            return 1;
        }

        public override void drawWhenHeld(SpriteBatch spriteBatch, Vector2 objectPosition, StardewValley.Farmer f)
        {
            if (animationManager == null)
            {
                spriteBatch.Draw(this.TextureSheet.texture, objectPosition, new Microsoft.Xna.Framework.Rectangle?(GameLocation.getSourceRectForObject(f.ActiveObject.ParentSheetIndex)), Color.White, 0f, Vector2.Zero, (float)Game1.pixelZoom, SpriteEffects.None, Math.Max(0f, (float)(f.getStandingY() + 2) / 10000f));
            }
            else
            {
                spriteBatch.Draw(this.TextureSheet.texture, objectPosition,this.animationManager.currentAnimation.sourceRectangle, Color.White, 0f, Vector2.Zero, (float)Game1.pixelZoom, SpriteEffects.None, Math.Max(0f, (float)(f.getStandingY() + 2) / 10000f));
            }

            if (f.ActiveObject != null && f.ActiveObject.Name.Contains("="))
            {
                spriteBatch.Draw(this.TextureSheet.texture, objectPosition + new Vector2((float)(Game1.tileSize / 2), (float)(Game1.tileSize / 2)), new Microsoft.Xna.Framework.Rectangle?(GameLocation.getSourceRectForObject(f.ActiveObject.ParentSheetIndex)), Color.White, 0f, new Vector2((float)(Game1.tileSize / 2), (float)(Game1.tileSize / 2)), (float)Game1.pixelZoom + Math.Abs(Game1.starCropShimmerPause) / 8f, SpriteEffects.None, Math.Max(0f, (float)(f.getStandingY() + 2) / 10000f));
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
            this.drawCrops(spriteBatch, (int)objectPosition.X,(int)objectPosition.Y);
            //base.drawWhenHeld(spriteBatch, objectPosition, f);
        }

        public override void drawInMenu(SpriteBatch spriteBatch, Vector2 location, float scaleSize, float transparency, float layerDepth, bool drawStackNumber, Color c, bool drawShadows)
        {
            if(animationManager==null) spriteBatch.Draw(TextureSheet.texture, location + new Vector2((float)(Game1.tileSize / 2), (float)(Game1.tileSize / 2)), new Rectangle?(this.defaultSourceRect), Color.White * transparency, 0f, new Vector2((float)(this.defaultSourceRect.Width / 2), (float)(this.defaultSourceRect.Height / 2)), 1f * (3) * scaleSize, SpriteEffects.None, layerDepth);
            else
            {
            spriteBatch.Draw(animationManager.objectTexture.texture, location + new Vector2((float)(Game1.tileSize / 2), (float)(Game1.tileSize / 2)), new Rectangle?(animationManager.currentAnimation.sourceRectangle), Color.White * transparency, 0f, new Vector2((float)(this.defaultSourceRect.Width / 2), (float)(this.defaultSourceRect.Height / 2)), 1f * (3) * scaleSize, SpriteEffects.None, layerDepth);


                //this.modularCrop.drawInMenu(spriteBatch, location + new Vector2((float)(Game1.tileSize / 2), (float)(Game1.tileSize / 2)), Color.White, 0f,true);

            if (Game1.player.CurrentItem != this) animationManager.tickAnimation();
            }
            Vector2 v = location + new Vector2((float)(Game1.tileSize / 2), (float)(Game1.tileSize / 2));
            this.drawCrops(spriteBatch, (int)v.X, (int)v.Y, 1, true);
        }

        public void drawCropWhenPlanterBoxHeld(PlanterBox p, SpriteBatch spriteBatch, Vector2 location, float layerDepth, float alpha = 1f)
        {
            spriteBatch.Draw(Game1.cropSpriteSheet, Game1.GlobalToLocal(Game1.viewport, new Vector2(Game1.player.GetBoundingBox().Center.X - Game1.tileSize / 2, (Game1.player.GetBoundingBox().Center.Y - Game1.tileSize * 4 / 3) - (Game1.tileSize * 2))), getCropSourceRect(this.crop.rowInSpriteSheet.Value,this.crop), Color.White * alpha, 0f, Vector2.Zero, (float)Game1.pixelZoom, SpriteEffects.None, (float)(p.boundingBox.Bottom + 1) / 10000f);
        }

        private Rectangle getCropSourceRect(int number, Crop c)
        {
            if (c.dead.Value)
                return new Rectangle(192 + number % 4 * 16, 384, 16, 32);
            return new Rectangle(Math.Min(240, (c.fullyGrown.Value ? (c.dayOfCurrentPhase.Value <= 0 ? 6 : 7) : (c.phaseToShow.Value!= -1 ? c.phaseToShow : c.currentPhase) + ((c.phaseToShow.Value != -1 ? c.phaseToShow : c.currentPhase) != 0 || number % 2 != 0 ? 0 : -1) + 1) * 16 + (c.rowInSpriteSheet.Value % 2 != 0 ? 128 : 0)), c.rowInSpriteSheet.Value / 2 * 16 * 2, 16, 32);
        }

        public override void draw(SpriteBatch spriteBatch, int x, int y, float alpha = 1f)
        {
            if (x == -1)
            {
                spriteBatch.Draw(TextureSheet.texture, Game1.GlobalToLocal(Game1.viewport, this.drawPosition), new Rectangle?(this.sourceRect), Color.White * alpha, 0f, Vector2.Zero, (float)Game1.pixelZoom, this.flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None, (this.Decoration_type == 12) ? 0f : ((float)(this.boundingBox.Bottom - 8) / 10000f));
                this.drawCrops(Game1.spriteBatch,(int) Game1.GlobalToLocal(Game1.viewport, this.drawPosition).X,(int) Game1.GlobalToLocal(Game1.viewport, this.drawPosition).Y);
            }
            else
            {
                //The actual planter box being drawn.
                if (animationManager == null)
                {
                    spriteBatch.Draw(TextureSheet.texture, Game1.GlobalToLocal(Game1.viewport, new Vector2((float)(x * Game1.tileSize), y * Game1.tileSize)), new Rectangle?(this.sourceRect), Color.White * alpha, 0f, Vector2.Zero, (float)Game1.pixelZoom, this.flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
                   // Log.AsyncG("ANIMATION IS NULL?!?!?!?!");
                }

                else
                {
                    //Log.AsyncC("Animation Manager is working!");
                    this.animationManager.draw(spriteBatch,animationManager.objectTexture.texture, Game1.GlobalToLocal(Game1.viewport, new Vector2((float)(x * Game1.tileSize), y * Game1.tileSize)), new Rectangle?(animationManager.currentAnimation.sourceRectangle), Color.White * alpha, 0f, Vector2.Zero, (float)Game1.pixelZoom, this.flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
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

                if (this.heldObject.Value != null)
                {
                    if (this.heldObject.Value is PlanterBox)
                    {
                        (this.heldObject.Value as PlanterBox).drawAtNonTileSpot(spriteBatch, Game1.GlobalToLocal(Game1.viewport, new Vector2((float)(this.boundingBox.Center.X - Game1.tileSize / 2), (float)(this.boundingBox.Center.Y - (this.heldObject.Value as PlanterBox).sourceRect.Height * Game1.pixelZoom - Game1.tileSize / 4))), (float)(this.boundingBox.Bottom - 7) / 10000f, alpha);
                        return;
                    }
                    spriteBatch.Draw(Game1.shadowTexture, Game1.GlobalToLocal(Game1.viewport, new Vector2((float)(this.boundingBox.Center.X - Game1.tileSize / 2), (float)(this.boundingBox.Center.Y - Game1.tileSize * 4 / 3))) + new Vector2((float)(Game1.tileSize / 2), (float)(Game1.tileSize * 5 / 6)), new Rectangle?(Game1.shadowTexture.Bounds), Color.White * alpha, 0f, new Vector2((float)Game1.shadowTexture.Bounds.Center.X, (float)Game1.shadowTexture.Bounds.Center.Y), 4f, SpriteEffects.None, (float)this.boundingBox.Bottom / 10000f);
                    spriteBatch.Draw(Game1.objectSpriteSheet, Game1.GlobalToLocal(Game1.viewport, new Vector2((float)(this.boundingBox.Center.X - Game1.tileSize / 2), (float)(this.boundingBox.Center.Y - Game1.tileSize * 4 / 3))), new Rectangle?(GameLocation.getSourceRectForObject(this.heldObject.Value.ParentSheetIndex)), Color.White * alpha, 0f, Vector2.Zero, (float)Game1.pixelZoom, SpriteEffects.None, (float)(this.boundingBox.Bottom + 1) / 10000f);
                }
                this.drawCrops(Game1.spriteBatch, (int)x, (int)y);

            }
        }


        public void drawCrops(SpriteBatch spriteBatch, int x, int y, float alpha = 1f, bool inMenu = false)
        {

            if (inMenu == false)
            {
                if (this.thisLocation != null)
                {
                    if (this.modularCrop != null)
                    {
                        this.modularCrop.draw(Game1.spriteBatch, this.TileLocation, Color.White, 0);
                        // Log.AsyncM("draw a modular crop now");
                    }
                    // Log.AsyncC("wait WTF");

                    if (this.crop != null)
                    {

                        this.crop.draw(Game1.spriteBatch, this.TileLocation, Color.White, 0);
                        //Log.AsyncG("COWS GO MOO");
                    }
                }
                else //if is in inventory
                {
                    if (this.modularCrop != null)
                    {
                        this.modularCrop.drawWhenPlanterBoxHeld(this ,Game1.spriteBatch, new Vector2(x,y),0);                      
                        // Log.AsyncM("draw a modular crop now");
                    }
                    // Log.AsyncC("wait WTF");

                    if (this.crop != null)
                    {

                        this.drawCropWhenPlanterBoxHeld(this,Game1.spriteBatch, new Vector2(x,y), 0);
                        //Log.AsyncG("COWS GO MOO");
                    }
                }
            }
            else
            {
                if (this.modularCrop != null)
                {
                    this.modularCrop.drawInMenu(this,Game1.spriteBatch, new Vector2(x,y), Color.White, 0,1,0);
                    // Log.AsyncM("draw a modular crop now");
                }
                // Log.AsyncC("wait WTF");

                if (this.crop != null)
                {

                    this.drawCropInMenu(this, Game1.spriteBatch, new Vector2(x, y), Color.White, 0, 1, 0);
                    //Log.AsyncG("COWS GO MOO");
                }
            }
           //else Log.AsyncM("I DONT UNDERSTAND");
        }

        public void drawCropInMenu(PlanterBox p, SpriteBatch b, Vector2 screenPosition, Color c, float roation, float scale, float layerDepth)
        {            
            b.Draw(Game1.cropSpriteSheet, new Vector2(screenPosition.X, screenPosition.Y - (Game1.tileSize / 2)), new Rectangle?(this.getCropSourceRect(this.crop.rowInSpriteSheet.Value,this.crop)), Color.White, 0f, new Vector2((float)(p.defaultSourceRect.Width / 2), (float)(p.defaultSourceRect.Height / 2)), 1f * (2) * scale, SpriteEffects.None, layerDepth);
        }

        public new void drawAtNonTileSpot(SpriteBatch spriteBatch, Vector2 location, float layerDepth, float alpha = 1f)
        {
            spriteBatch.Draw(TextureSheet.texture, location, new Rectangle?(this.sourceRect), Color.White * alpha, 0f, Vector2.Zero, (float)Game1.pixelZoom, this.flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None, layerDepth);

          //  this.drawCrops(spriteBatch, (int)location.X*Game1.tileSize, (int)location.Y*Game1.tileSize, 1, true);
        }

        public override Item getOne(IModHelper helper)
        {
           
            if (this.dataPath == "") return new PlanterBox(helper,this.ParentSheetIndex, this.TileLocation);
            else return  new PlanterBox(helper,this.ParentSheetIndex, this.TileLocation,this.texturePath,this.dataPath);

            /*
            drawPosition = this.drawPosition;
            defaultBoundingBox = this.defaultBoundingBox;
            boundingBox = this.boundingBox;
            currentRotation = this.currentRotation - 1;
            rotations = this.rotations;
            rotate();
            */
        }

        public override string getCategoryName()
        {
            return "Planter Box";
            //  return base.getCategoryName();
        }

        public override Color getCategoryColor()
        {
            return Color.Purple;
        }

        public static new void Serialize(Item I)
        {
            makeCropInformationString(I);
            StardustCore.ModCore.SerializationManager.WriteToJsonFile(Path.Combine(StardustCore.ModCore.SerializationManager.playerInventoryPath, I.Name + ".json"), (PlanterBox)I);
        }

        public static void Serialize(Item I,string s)
        {
            makeCropInformationString(I);
            StardustCore.ModCore.SerializationManager.WriteToJsonFile(Path.Combine(s, I.Name + ".json"), (PlanterBox)I);
        }
        
        /// <summary>
        /// Needs to be rewritten.......
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static PlanterBox ParseIntoInventory(string s)
        {
            return ModCore.ModHelper.ReadJsonFile<PlanterBox>(s);

            //return base.ParseIntoInventory();
        }

        public static void SerializeFromWorld(Item I)
        {
            makeCropInformationString(I);
            //  ModCore.serilaizationManager.WriteToJsonFile(Path.Combine(ModCore.serilaizationManager.objectsInWorldPath, (c as CoreObject).thisLocation.name, c.Name + ".json"), (PlanterBox)c);
            StardustCore.ModCore.SerializationManager.WriteToJsonFile(Path.Combine(StardustCore.ModCore.SerializationManager.objectsInWorldPath, I.Name + ".json"), (PlanterBox)I);
        }

        public static void makeCropInformationString(Item I)
        {
            (I as PlanterBox).cropInformationString = "";

            if ((I as PlanterBox).crop != null)
            {
                Crop c = (I as PlanterBox).crop;
                (I as PlanterBox).cropInformationString = "false" + "/" + (I as PlanterBox).normalCropSeedIndex + "/" + (I as PlanterBox).TileLocation.X + "/" + (I as PlanterBox).TileLocation.Y + "/" + c.currentPhase.Value + "/" + c.dayOfCurrentPhase.Value + "/" + c.fullyGrown.Value; ;
            }

            if ((I as PlanterBox).modularCrop != null)
            {
                ModularCrop m = (I as PlanterBox).modularCrop;
                (I as PlanterBox).cropInformationString = "true" + "/" + m.seedIndex + "/" + (I as PlanterBox).TileLocation.X + "/" + (I as PlanterBox).TileLocation.Y + "/" + m.dataFileName + "/" + m.spriteSheetName + "/" + m.cropObjectTexture + "/" + m.cropObjectData + "/" + m.currentPhase + "/" + m.dayOfCurrentPhase+"/"+m.fullyGrown;
            }
        }

        

    }
}