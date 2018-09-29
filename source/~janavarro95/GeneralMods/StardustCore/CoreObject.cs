using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Objects;
using StardustCore.Animations;
using StardustCore.UIUtilities;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace StardustCore
{
    /// <summary>
    ///  Extends StardewValley.Object. Might be broken and just mainly uses a ton of junk code. Just extend off of this.
    /// </summary>
    /// 
 
    public class CoreObject : StardewValley.Object
    {
        public Vector2 position;

        public int Decoration_type;

        public int rotations;

        public int currentRotation;

        public int sourceIndexOffset;

        protected Vector2 drawPosition;

        public Rectangle sourceRect;

        public Rectangle defaultSourceRect;

        public Rectangle defaultBoundingBox;

        public string description;

        [XmlIgnore]
        public Texture2DExtended TextureSheet;

        public new bool flipped;

        [XmlIgnore]
        public bool flaggedForPickUp;

        public bool lightGlowAdded;

        public string texturePath;

        public List<Item> inventory;

        public int inventoryMaxSize;

        public bool itemReadyForHarvest;

        public bool lightsOn;

        public GameLocation thisLocation;

        public Color lightColor;

        public string thisType;

        public bool removable;

        public string locationsName;

        public Color drawColor;

        public bool useXML;

        public AnimationManager animationManager;

        public string serializationName;


        public override string Name
        {
            get
            {
                return this.name;
            }

        }

        /// <summary>
        /// Initialize some basic custom logic such as inventory for CoreObjects.
        /// </summary>
        /// <param name="InvMaxSize">The max size of this object's internal inventory.</param>
        /// <param name="tile">The location for this object to be positioned.</param>
        public virtual void InitializeBasics(int InvMaxSize, Vector2 tile)
        {
            this.inventory = new List<Item>();
            this.inventoryMaxSize = InvMaxSize;
            this.TileLocation = tile;
            lightsOn = false;

            lightColor = Color.Black;
            thisType = this.GetType().ToString();
            
        }

        public CoreObject()
        {
            this.updateDrawPosition();
        }

        public CoreObject(bool f)
        {
            //does nothng
        }

        public CoreObject(Texture2DExtended texture,int which, Vector2 Tile, int InventoryMaxSize)
        {
            InitializeBasics(InventoryMaxSize, Tile);
            if (TextureSheet == null)
            {
                TextureSheet = texture;
                this.texturePath = texture.path;
            }
            Dictionary<int, string> dictionary = Game1.content.Load<Dictionary<int, string>>("Data\\Furniture");
            string[] array = dictionary[which].Split(new char[]
            {
                '/'
            });
            this.name = array[0];

            this.Decoration_type = this.getTypeNumberFromName(array[1]);
            this.description = "Can be placed inside your house.";
            this.defaultSourceRect = new Rectangle(which * 16 % TextureSheet.texture.Width, which * 16 / TextureSheet.texture.Width * 16, 1, 1);
            if (array[2].Equals("-1"))
            {
                this.sourceRect = this.getDefaultSourceRectForType(which, this.Decoration_type);
                this.defaultSourceRect = this.sourceRect;
            }
            else
            {
                this.defaultSourceRect.Width = Convert.ToInt32(array[2].Split(new char[]
                {
                    ' '
                })[0]);
                this.defaultSourceRect.Height = Convert.ToInt32(array[2].Split(new char[]
                {
                    ' '
                })[1]);
                this.sourceRect = new Rectangle(which * 16 % TextureSheet.texture.Width, which * 16 / TextureSheet.texture.Width * 16, this.defaultSourceRect.Width * 16, this.defaultSourceRect.Height * 16);
                this.defaultSourceRect = this.sourceRect;
            }
            this.defaultBoundingBox = new Rectangle((int)this.TileLocation.X, (int)this.TileLocation.Y, 1, 1);
            if (array[3].Equals("-1"))
            {
                this.boundingBox.Value = this.getDefaultBoundingBoxForType(this.Decoration_type);
                this.defaultBoundingBox = this.boundingBox.Value;
            }
            else
            {
                this.defaultBoundingBox.Width = Convert.ToInt32(array[3].Split(new char[]
                {
                    ' '
                })[0]);
                this.defaultBoundingBox.Height = Convert.ToInt32(array[3].Split(new char[]
                {
                    ' '
                })[1]);
                this.boundingBox.Value = new Rectangle((int)this.TileLocation.X * Game1.tileSize, (int)this.TileLocation.Y * Game1.tileSize, this.defaultBoundingBox.Width * Game1.tileSize, this.defaultBoundingBox.Height * Game1.tileSize);
                this.defaultBoundingBox = this.boundingBox.Value;
            }
            this.updateDrawPosition();
            this.rotations = Convert.ToInt32(array[4]);
            this.Price = Convert.ToInt32(array[5]);
            this.ParentSheetIndex = which;
        }

        public override string getDescription()
        {
            return this.description;
        }

        /// <summary>
        /// A "placement" action that doesn't put the object in the world, but set it's position accordingly.
        /// </summary>
        /// <param name="location">The GameLocation that this object will be placed at.</param>
        /// <param name="tileX">The x tile location to "place".</param>
        /// <param name="tileY">The y tile location to "place".</param>
        /// <param name="trackSerialization">If true then the Serialization manager will keep track of this object.</param>
        public void fakePlacementAction(GameLocation location, int tileX, int tileY,bool trackSerialization=true)
        {
            this.thisLocation = location;
            this.TileLocation = new Vector2(tileX, tileY);
            this.position = this.TileLocation*(Game1.tileSize);
            if (trackSerialization)
            {
                if (ModCore.SerializationManager.trackedObjectList.Contains(this)) return;
                ModCore.SerializationManager.trackedObjectList.Add(this);
            }
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
        }

        public override bool checkForAction(StardewValley.Farmer who, bool justCheckingForActivity = false)
        {
            var mState = Microsoft.Xna.Framework.Input.Mouse.GetState();
            if (mState.RightButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
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
                //this.minutesUntilReady = 30;
                //  Log.AsyncC("placed item!");
            }
            else
            {
                //Game1.showRedMessage("CRY");
            }

            if (justCheckingForActivity)
            {
                return true;
            }
            return this.clicked(who);
        }

        //DONT USE THIS BASE IT IS TERRIBLE
        /*
        public override bool clicked(StardewValley.Farmer who)
        {

            //  Game1.showRedMessage("THIS IS CLICKED!!!");
            Game1.haltAfterCheck = false;

            if (this.heldObject != null)
            {
                this.spillInventoryEverywhere();
                return false;
            }

            if (this.heldObject == null && (who.ActiveObject == null || !(who.ActiveObject is CoreObject)))
            {
                if (Game1.player.currentLocation is FarmHouse)
                {
                    // Game1.showRedMessage("Why2?");
                    //  this.spillInventoryEverywhere();

                    if (this.heldObject != null) Util.addItemToInventoryElseDrop(this.heldObject.getOne());
                    this.heldObject = new CoreObject(parentSheetIndex, Vector2.Zero, this.inventoryMaxSize);
                  //  Util.addItemToInventoryElseDrop(this.heldObject.getOne());
                    this.heldObject = null;
                    this.flaggedForPickUp = true;
                    thisLocation = null;
                    return true;
                }
                else
                {
                    // return true;

                    this.flaggedForPickUp = true;
                    if (this is TV)
                    {
                        // this.heldObject = new TV(parentSheetIndex, Vector2.Zero);
                    }
                    else
                    {

                        //    Util.addItemToInventoryElseDrop(this.heldObject);

                        var obj = new CoreObject(parentSheetIndex, Vector2.Zero, this.inventoryMaxSize);
                       // Util.addItemToInventoryElseDrop(obj);
                        //     this.spillInventoryEverywhere();
                        if (this.heldObject != null) this.heldObject.performRemoveAction(this.tileLocation, who.currentLocation);

                        this.heldObject = null;
                        Game1.playSound("coin");
                        thisLocation = null;
                        return true;
                    }

                }
            }
            if (this.heldObject != null && who.addItemToInventoryBool(this.heldObject, false))
            {
                // Game1.showRedMessage("Why3?");
                // if(this.heldObject!=null) Game1.player.addItemByMenuIfNecessary((Item)this.heldObject);
                // this.spillInventoryEverywhere();
                var obj = new CoreObject(parentSheetIndex, Vector2.Zero, this.inventoryMaxSize);
              //  Util.addItemToInventoryElseDrop(obj);
                if (this.heldObject != null) this.heldObject.performRemoveAction(this.tileLocation, who.currentLocation);
                this.heldObject = null;
                Game1.playSound("coin");
                thisLocation = null;
                return true;
            }



            return false;
        }
        */

        public virtual bool RightClicked(StardewValley.Farmer who)
        {
            //  StardewModdingAPI.Log.AsyncC(lightColor);
            //  Game1.activeClickableMenu = new Revitalize.Menus.LightCustomizer(this);

            // Game1.showRedMessage("THIS IS CLICKED!!!");
            //var mState = Microsoft.Xna.Framework.Input.Mouse.GetState();
            /*

            Game1.haltAfterCheck = false;
            if (this.Decoration_type == 11 && who.ActiveObject != null && who.ActiveObject != null && this.heldObject == null)
            {
                //    Game1.showRedMessage("Why1?");
                return false;
            }
            if (this.heldObject == null && (who.ActiveObject == null || !(who.ActiveObject is Light)))
            {
                if (Game1.player.currentLocation is FarmHouse)
                {
                    //       
                    Game1.player.addItemByMenuIfNecessary(this);
                    removeLights(this.thisLocation);
                    this.lightsOn = false;
                    Game1.playSound("coin");
                    //        this.flaggedForPickUp = true;
                    return true;
                }
                else
                {
                    // return true;
                    // this.heldObject = new Light(parentSheetIndex, Vector2.Zero, this.lightColor, this.inventoryMaxSize);
                    Game1.player.addItemByMenuIfNecessary(this);
                    removeLights(this.thisLocation);
                    this.lightsOn = false;
                    Game1.playSound("coin");
                    return true;

                }
            }
            if (this.heldObject != null && who.addItemToInventoryBool(this.heldObject, false))
            {
                //    Game1.showRedMessage("Why3?");
                // if(this.heldObject!=null) Game1.player.addItemByMenuIfNecessary((Item)this.heldObject);
                Util.addItemToInventoryElseDrop(this);
                this.heldObject.performRemoveAction(this.tileLocation, who.currentLocation);
                this.heldObject = null;
                Game1.playSound("coin");
                removeLights(this.thisLocation);
                this.lightsOn = false;
                return true;
            }

            */

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

        public virtual void resetOnPlayerEntry(GameLocation environment)
        {
            this.removeLights(environment);
            if (Game1.isDarkOut())
            {
                this.addLights(environment);
            }
        }

        /// <summary>
        /// Drops in an item into this object. I.E putting an egg in a mayo maker.
        /// </summary>
        /// <param name="dropIn"></param>
        /// <param name="probe"></param>
        /// <param name="who"></param>
        /// <returns></returns>
        public override bool performObjectDropInAction(Item dropIn, bool probe, StardewValley.Farmer who)
        {
            // Log.AsyncG("HEY!");
            if (this.heldObject.Value == null && (dropIn is Item)) //Basically who cares accept whatever.
            {
                try {
                    this.heldObject.Value = (StardewValley.Object)dropIn.getOne();
                    this.heldObject.Value.TileLocation = this.TileLocation;
                    this.heldObject.Value.boundingBox.X = this.boundingBox.X;
                    this.heldObject.Value.boundingBox.Y = this.boundingBox.Y;
                    // Log.AsyncO(getDefaultBoundingBoxForType((dropIn as CoreObject).Decoration_type));
                    this.heldObject.Value.performDropDownAction(who);
                    if (!probe)
                    {
                        Game1.playSound("woodyStep");
                        //  Log.AsyncC("HUH?");
                        if (who != null)
                        {
                            who.reduceActiveItemByOne();
                        }
                    }
                    return true;
                }
                catch(Exception err)
                {
                    ModCore.ModMonitor.Log(err.ToString(),LogLevel.Error);
                    return false;
                }
            }
            return false;
        }

        public virtual void addLights(GameLocation environment)
        {
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
                    this.lightSource = new LightSource(4, new Vector2((float)(this.boundingBox.X + Game1.tileSize / 2), (float)(this.boundingBox.Y - Game1.tileSize)), 2f, lightColor, (int)(this.TileLocation.X * 2000f + this.TileLocation.Y));
                    Game1.currentLightSources.Add(this.lightSource);
                    // Log.AsyncG("LIGHT SOURCE ADDED FFFFFFF");
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
            else
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
                    this.lightSource = new LightSource(4, new Vector2((float)(this.boundingBox.X + Game1.tileSize / 2), (float)(this.boundingBox.Y - Game1.tileSize)), 2f, lightColor, (int)(this.TileLocation.X * 2000f + this.TileLocation.Y));
                    Game1.currentLightSources.Add(this.lightSource);
                    return;
                }

            }
        }


        /// <summary>
        /// Add a light source to this location.
        /// </summary>
        /// <param name="environment">The game location to add the light source in.</param>
        /// <param name="c">The color of the light to be added</param>
        public virtual void addLights(GameLocation environment, Color c)
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
                this.lightSource = new LightSource(4, new Vector2((float)(this.boundingBox.X + Game1.tileSize / 2), (float)(this.boundingBox.Y - Game1.tileSize)), 2f, c, (int)(this.TileLocation.X * 2000f + this.TileLocation.Y));
                Game1.currentLightSources.Add(this.lightSource);
                return;
            }
        }

        /// <summary>
        /// Removes a lightsource from the game location.
        /// </summary>
        /// <param name="environment">The game location to remove the light source from.</param>
        public void removeLights(GameLocation environment)
        {
            if (this.Decoration_type == 7)
            {
                if (this.sourceIndexOffset == 1)
                {
                    this.sourceRect = this.defaultSourceRect;
                }
                this.sourceIndexOffset = 0;
                Utility.removeLightSource((int)(this.TileLocation.X * 2000f + this.TileLocation.Y));
                this.lightSource = null;
                return;
            }
            if (this.Decoration_type == 13)
            {
                if (this.sourceIndexOffset == 1)
                {
                    this.sourceRect = this.defaultSourceRect;
                }
                this.sourceIndexOffset = 0;
                if (Game1.isRaining)
                {
                    this.sourceRect = this.defaultSourceRect;
                    this.sourceRect.X = this.sourceRect.X + this.sourceRect.Width;
                    this.sourceIndexOffset = 1;
                    return;
                }
                if (!this.lightGlowAdded && !environment.lightGlows.Contains(new Vector2((float)(this.boundingBox.X + Game1.tileSize / 2), (float)(this.boundingBox.Y + Game1.tileSize))))
                {
                    environment.lightGlows.Add(new Vector2((float)(this.boundingBox.X + Game1.tileSize / 2), (float)(this.boundingBox.Y + Game1.tileSize)));
                }
                this.lightGlowAdded = true;
            }

            if (this.sourceIndexOffset == 1)
            {
                this.sourceRect = this.defaultSourceRect;
            }
            this.sourceIndexOffset = 0;
            Utility.removeLightSource((int)(this.TileLocation.X * 2000f + this.TileLocation.Y));
            this.lightSource = null;
            return;
        }




        public override bool minutesElapsed(int minutes, GameLocation environment)
        {
            // Log.Info("minutes passed in"+minutes);
            //  Log.Info("minues remaining" + this.minutesUntilReady);
            this.MinutesUntilReady = (this.MinutesUntilReady - minutes);
            if (Game1.isDarkOut())
            {
                // this.addLights(environment,this.lightColor);
                this.addLights(environment);
            }
            else
            {
                this.removeLights(environment);
            }

            if (minutesUntilReady == 0)
            {
                // Log.AsyncC(this.name + "Is ready!");
                // Log.AsyncC(Game1.player.getStandingPosition());
                // Vector2 v2 = new Vector2(this.tileLocation.X * Game1.tileSize, this.tileLocation.Y * Game1.tileSize);
                //Game1.createItemDebris((Item)this.heldObject, v2, Game1.player.getDirection());
                // minutesUntilReady = 30;
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
            this.spillInventoryEverywhere();
            base.performRemoveAction(tileLocation, environment);
        }

        public virtual void rotate()
        {
            if (this.rotations < 2)
            {
                return;
            }
            int num = (this.rotations == 4) ? 1 : 2;
            this.currentRotation += num;
            this.currentRotation %= 4;
            this.flipped = false;
            Point point = default(Point);
            int num2 = this.Decoration_type;
            switch (num2)
            {
                case 2:
                    point.Y = 1;
                    point.X = -1;
                    break;
                case 3:
                    point.X = -1;
                    point.Y = 1;
                    break;
                case 4:
                    break;
                case 5:
                    point.Y = 0;
                    point.X = -1;
                    break;
                default:
                    if (num2 == 12)
                    {
                        point.X = 0;
                        point.Y = 0;
                    }
                    break;
            }
            bool flag = this.Decoration_type == 5 || this.Decoration_type == 12 || this.ParentSheetIndex == 724 || this.ParentSheetIndex == 727;
            bool flag2 = this.defaultBoundingBox.Width != this.defaultBoundingBox.Height;
            if (flag && this.currentRotation == 2)
            {
                this.currentRotation = 1;
            }
            if (flag2)
            {
                int height = this.boundingBox.Height;
                switch (this.currentRotation)
                {
                    case 0:
                    case 2:
                        this.boundingBox.Height = this.defaultBoundingBox.Height;
                        this.boundingBox.Width = this.defaultBoundingBox.Width;
                        break;
                    case 1:
                    case 3:
                        this.boundingBox.Height = this.boundingBox.Width + point.X * Game1.tileSize;
                        this.boundingBox.Width = height + point.Y * Game1.tileSize;
                        break;
                }
            }
            Point point2 = default(Point);
            int num3 = this.Decoration_type;
            if (num3 == 12)
            {
                point2.X = 1;
                point2.Y = -1;
            }
            if (flag2)
            {
                switch (this.currentRotation)
                {
                    case 0:
                        this.sourceRect = this.defaultSourceRect;
                        break;
                    case 1:
                        this.sourceRect = new Rectangle(this.defaultSourceRect.X + this.defaultSourceRect.Width, this.defaultSourceRect.Y, this.defaultSourceRect.Height - 16 + point.Y * 16 + point2.X * 16, this.defaultSourceRect.Width + 16 + point.X * 16 + point2.Y * 16);
                        break;
                    case 2:
                        this.sourceRect = new Rectangle(this.defaultSourceRect.X + this.defaultSourceRect.Width + this.defaultSourceRect.Height - 16 + point.Y * 16 + point2.X * 16, this.defaultSourceRect.Y, this.defaultSourceRect.Width, this.defaultSourceRect.Height);
                        break;
                    case 3:
                        this.sourceRect = new Rectangle(this.defaultSourceRect.X + this.defaultSourceRect.Width, this.defaultSourceRect.Y, this.defaultSourceRect.Height - 16 + point.Y * 16 + point2.X * 16, this.defaultSourceRect.Width + 16 + point.X * 16 + point2.Y * 16);
                        this.flipped = true;
                        break;
                }
            }
            else
            {
                this.flipped = (this.currentRotation == 3);
                if (this.rotations == 2)
                {
                    this.sourceRect = new Rectangle(this.defaultSourceRect.X + ((this.currentRotation == 2) ? 1 : 0) * this.defaultSourceRect.Width, this.defaultSourceRect.Y, this.defaultSourceRect.Width, this.defaultSourceRect.Height);
                }
                else
                {
                    this.sourceRect = new Rectangle(this.defaultSourceRect.X + ((this.currentRotation == 3) ? 1 : this.currentRotation) * this.defaultSourceRect.Width, this.defaultSourceRect.Y, this.defaultSourceRect.Width, this.defaultSourceRect.Height);
                }
            }
            if (flag && this.currentRotation == 1)
            {
                this.currentRotation = 2;
            }
            this.updateDrawPosition();
        }

        public virtual bool isGroundFurniture()
        {
            return this.Decoration_type != 13 && this.Decoration_type != 6 && this.Decoration_type != 13;
        }

        /// <summary>
        /// Determines whether or not this item can be given as a gift. Default is always false.
        /// </summary>
        /// <returns></returns>
        public override bool canBeGivenAsGift()
        {
            return false;
        }

        /*
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
                        foreach (KeyValuePair<Vector2, StardewValley.Object> something in l.objects)
                        {
                            StardewValley.Object obj = something.Value;
                            if ((obj.GetType()).ToString().Contains("CoreObject"))
                            {
                                CoreObject current = (CoreObject)obj;
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
                        }
                    }
                }
                return Util.canBePlacedHere(this, l, tile);
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
                        foreach (CoreObject current in (l as FarmHouse).CoreObject)
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
                        
                    }
                }
                return Util.canBePlacedHere(this, l, tile);
            }
        }
        */


        public virtual void updateDrawPosition()
        {
            this.drawPosition = new Vector2((float)this.boundingBox.X, (float)(this.boundingBox.Y - (this.sourceRect.Height * Game1.pixelZoom - this.boundingBox.Height)));
        }

        public virtual int getTilesWide()
        {
            return this.boundingBox.Width / Game1.tileSize;
        }

        public virtual int getTilesHigh()
        {
            return this.boundingBox.Height / Game1.tileSize;
        }

        /*
        public override bool placementAction(GameLocation location, int x, int y, StardewValley.Farmer who = null)
        {
          //  Log.AsyncC(x);
          //  Log.AsyncM(y);
           
            if (location is FarmHouse)
            {
                Point point = new Point(x / Game1.tileSize, y / Game1.tileSize);
                List<Rectangle> walls = FarmHouse.getWalls((location as FarmHouse).upgradeLevel);
                this.tileLocation = new Vector2((float)point.X, (float)point.Y);
                bool flag = false;
                if (this.Decoration_type == 6 || this.Decoration_type == 13 || this.parentSheetIndex == 1293)
                {
                    int num = (this.parentSheetIndex == 1293) ? 3 : 0;
                    bool flag2 = false;
                    foreach (Rectangle current in walls)
                    {
                        if ((this.Decoration_type == 6 || this.Decoration_type == 13 || num != 0) && current.Y + num == point.Y && current.Contains(point.X, point.Y - num))
                        {
                            flag2 = true;
                            break;
                        }
                    }
                    if (!flag2)
                    {
                        Game1.showRedMessage("Must be placed on wall");
                        return false;
                    }
                    flag = true;
                }
                for (int i = point.X; i < point.X + this.getTilesWide(); i++)
                {
                    for (int j = point.Y; j < point.Y + this.getTilesHigh(); j++)
                    {
                        if (location.doesTileHaveProperty(i, j, "NoFurniture", "Back") != null)
                        {
                            Game1.showRedMessage("Furniture can't be placed here");
                            return false;
                        }
                        if (!flag && Utility.pointInRectangles(walls, i, j))
                        {
                            Game1.showRedMessage("Can't place on wall");
                            return false;
                        }
                        if (location.getTileIndexAt(i, j, "Buildings") != -1)
                        {
                            return false;
                        }
                    }
                }
                this.boundingBox = new Rectangle(x, y, this.boundingBox.Width, this.boundingBox.Height);
                foreach (KeyValuePair<Vector2, StardewValley.Object> c in location.objects)
                {
                    StardewValley.Object ehh = c.Value;
                    if (((ehh.GetType()).ToString()).Contains("CoreObject"))
                    {
                        CoreObject current2 = (CoreObject)ehh;
                        if (current2.Decoration_type == 11 && current2.heldObject == null && current2.getBoundingBox(current2.tileLocation).Intersects(this.boundingBox))
                        {
                            current2.performObjectDropInAction(this, false, (who == null) ? Game1.player : who);
                            bool result = true;
                            return result;
                        }
                    }
                }
                foreach (StardewValley.Farmer current3 in location.getStardewValley.Farmers())
                {
                    if (current3.GetBoundingBox().Intersects(this.boundingBox))
                    {
                        Game1.showRedMessage("Can't place on top of a person.");
                        bool result = false;
                        return result;
                    }
                }
                this.updateDrawPosition();
              //  Log.AsyncO(this.boundingBox);
              //  Log.AsyncO(x);
              //  Log.AsyncY(y);
                for (int i = 0; i <= this.boundingBox.X / Game1.tileSize; i++)
                {
                    base.placementAction(location, x + 1, y, who);
                }
                for (int i = 0; i <= this.boundingBox.Y / Game1.tileSize; i++)
                {
                    base.placementAction(location, x, y + 1, who);
                }
                return true;
            }
            else
            {
                Point point = new Point(x / Game1.tileSize, y / Game1.tileSize);
                //  List<Rectangle> walls = FarmHouse.getWalls((location as FarmHouse).upgradeLevel);
                this.tileLocation = new Vector2((float)point.X, (float)point.Y);
                bool flag = false;
                if (this.Decoration_type == 6 || this.Decoration_type == 13 || this.parentSheetIndex == 1293)
                {
                    int num = (this.parentSheetIndex == 1293) ? 3 : 0;
                    bool flag2 = false;
                    /*
                    foreach (Rectangle current in walls)
                    {
                        if ((this.Decoration_type == 6 || this.Decoration_type == 13 || num != 0) && current.Y + num == point.Y && current.Contains(point.X, point.Y - num))
                        {
                            flag2 = true;
                            break;
                        }
                    }
                    
                    if (!flag2)
                    {
                        Game1.showRedMessage("Must be placed on wall");
                        return false;
                    }
                    flag = true;
                }
                for (int i = point.X; i < point.X + this.getTilesWide(); i++)
                {
                    for (int j = point.Y; j < point.Y + this.getTilesHigh(); j++)
                    {
                        if (location.doesTileHaveProperty(i, j, "NoFurniture", "Back") != null)
                        {
                            Game1.showRedMessage("Furniture can't be placed here");
                            return false;
                        }
                        /*
                        if (!flag && Utility.pointInRectangles(walls, i, j))
                        {
                            Game1.showRedMessage("Can't place on wall");
                            return false;
                        }
                        
                        if (location.getTileIndexAt(i, j, "Buildings") != -1)
                        {
                            return false;
                        }
                    }
                }
                this.boundingBox = new Rectangle(x, y, this.boundingBox.Width, this.boundingBox.Height);
                /*
                foreach (Furniture current2 in (location as FarmHouse).furniture)
                {
                    if (current2.furniture_type == 11 && current2.heldObject == null && current2.getBoundingBox(current2.tileLocation).Intersects(this.boundingBox))
                    {
                        current2.performObjectDropInAction(this, false, (who == null) ? Game1.player : who);
                        bool result = true;
                        return result;
                    }
                }
                
                foreach (StardewValley.Farmer current3 in location.getStardewValley.Farmers())
                {
                    if (current3.GetBoundingBox().Intersects(this.boundingBox))
                    {
                        Game1.showRedMessage("Can't place on top of a person.");
                        bool result = false;
                        return result;
                    }
                }
                this.updateDrawPosition();
                thisLocation = Game1.player.currentLocation;
                return base.placementAction(location, x, y, who);
            }

        }
        */


        public override bool placementAction(GameLocation location, int x, int y, StardewValley.Farmer who = null)
        {


            if (location is FarmHouse)
            {
                Point point = new Point(x / Game1.tileSize, y / Game1.tileSize);
                List<Rectangle> walls = FarmHouse.getWalls((location as FarmHouse).upgradeLevel);
                this.TileLocation = new Vector2((float)point.X, (float)point.Y);
                bool flag = false;
                if (this.Decoration_type == 6 || this.Decoration_type == 13 || this.ParentSheetIndex == 1293)
                {
                    int num = (this.ParentSheetIndex == 1293) ? 3 : 0;
                    bool flag2 = false;
                    foreach (Rectangle current in walls)
                    {
                        if ((this.Decoration_type == 6 || this.Decoration_type == 13 || num != 0) && current.Y + num == point.Y && current.Contains(point.X, point.Y - num))
                        {
                            flag2 = true;
                            break;
                        }
                    }
                    if (!flag2)
                    {
                        Game1.showRedMessage("Must be placed on wall");
                        return false;
                    }
                    flag = true;
                }
                for (int i = point.X; i < point.X + this.getTilesWide(); i++)
                {
                    for (int j = point.Y; j < point.Y + this.getTilesHigh(); j++)
                    {
                        if (location.doesTileHaveProperty(i, j, "NoFurniture", "Back") != null)
                        {
                            Game1.showRedMessage("Furniture can't be placed here");
                            return false;
                        }
                        if (!flag && Utility.pointInRectangles(walls, i, j))
                        {
                            Game1.showRedMessage("Can't place on wall");
                            return false;
                        }
                        if (location.getTileIndexAt(i, j, "Buildings") != -1)
                        {
                            return false;
                        }
                    }
                }
                this.boundingBox.Value = new Rectangle(x / Game1.tileSize, y / Game1.tileSize, this.boundingBox.Width, this.boundingBox.Height);
                foreach (var c in location.objects.Pairs)
                {                    
                    StardewValley.Object ehh = c.Value;
                    if (((ehh.GetType()).ToString()).Contains("Spawner"))
                    {
                        CoreObject current2 = (CoreObject)ehh;
                        if (current2.Decoration_type == 11 && current2.heldObject.Value == null && current2.getBoundingBox(current2.TileLocation).Intersects(this.boundingBox.Value))
                        {
                            current2.performObjectDropInAction(this, false, (who == null) ? Game1.player : who);
                            bool result = true;
                            return result;
                        }
                    }
                }
                foreach (StardewValley.Farmer current3 in location.getFarmers())
                {
                    if (current3.GetBoundingBox().Intersects(this.boundingBox.Value))
                    {
                        Game1.showRedMessage("Can't place on top of a person.");
                        bool result = false;
                        return result;
                    }
                }
                this.updateDrawPosition();
                //  Log.AsyncO(this.boundingBox);
                //   Log.AsyncO(x);
                //   Log.AsyncY(y);
                for (int i = 0; i <= this.boundingBox.X / Game1.tileSize; i++)
                {
                    base.placementAction(location, x + 1, y, who);
                }
                for (int i = 0; i <= this.boundingBox.Y / Game1.tileSize; i++)
                {
                    base.placementAction(location, x, y + 1, who);
                }
                return true;
            }
            else
            {
                Point point = new Point(x / Game1.tileSize, y / Game1.tileSize);
                //  List<Rectangle> walls = FarmHouse.getWalls((location as FarmHouse).upgradeLevel);
                this.TileLocation = new Vector2((float)point.X, (float)point.Y);
                if (this.Decoration_type == 6 || this.Decoration_type == 13 || this.ParentSheetIndex == 1293)
                {
                    int num = (this.ParentSheetIndex == 1293) ? 3 : 0;
                    bool flag2 = false;
                    /*
                    foreach (Rectangle current in walls)
                    {
                        if ((this.Decoration_type == 6 || this.Decoration_type == 13 || num != 0) && current.Y + num == point.Y && current.Contains(point.X, point.Y - num))
                        {
                            flag2 = true;
                            break;
                        }
                    }
                    */
                    if (!flag2)
                    {
                        Game1.showRedMessage("Must be placed on wall");
                        return false;
                    }
                }
                for (int i = point.X; i < point.X + this.getTilesWide(); i++)
                {
                    for (int j = point.Y; j < point.Y + this.getTilesHigh(); j++)
                    {
                        if (location.doesTileHaveProperty(i, j, "NoFurniture", "Back") != null)
                        {
                            Game1.showRedMessage("Furniture can't be placed here");
                            return false;
                        }
                        /*
                        if (!flag && Utility.pointInRectangles(walls, i, j))
                        {
                            Game1.showRedMessage("Can't place on wall");
                            return false;
                        }
                        */
                        if (location.getTileIndexAt(i, j, "Buildings") != -1)
                        {
                            return false;
                        }
                    }
                }
                this.boundingBox.Value = new Rectangle(x / Game1.tileSize, y / Game1.tileSize, this.boundingBox.Width, this.boundingBox.Height);
                /*
                foreach (Furniture current2 in (location as FarmHouse).furniture)
                {
                    if (current2.furniture_type == 11 && current2.heldObject == null && current2.getBoundingBox(current2.tileLocation).Intersects(this.boundingBox))
                    {
                        current2.performObjectDropInAction(this, false, (who == null) ? Game1.player : who);
                        bool result = true;
                        return result;
                    }
                }
                */
                foreach (StardewValley.Farmer current3 in location.getFarmers())
                {
                    if (current3.GetBoundingBox().Intersects(this.boundingBox.Value))
                    {
                        Game1.showRedMessage("Can't place on top of a person.");
                        bool result = false;
                        return result;
                    }
                }
                this.updateDrawPosition();
                this.thisLocation = Game1.player.currentLocation;
                return base.placementAction(location, x * Game1.tileSize, y * Game1.tileSize, who);
            }
        }

        public override bool isPlaceable()
        {
            return true;
        }

        public override Rectangle getBoundingBox(Vector2 tileLocation)
        {
            return this.boundingBox.Value;
        }

        private Rectangle getDefaultSourceRectForType(int tileIndex, int type)
        {
            int num;
            int num2;
            switch (type)
            {
                case 0:
                    num = 1;
                    num2 = 2;
                    goto IL_94;
                case 1:
                    num = 2;
                    num2 = 2;
                    goto IL_94;
                case 2:
                    num = 3;
                    num2 = 2;
                    goto IL_94;
                case 3:
                    num = 2;
                    num2 = 2;
                    goto IL_94;
                case 4:
                    num = 2;
                    num2 = 2;
                    goto IL_94;
                case 5:
                    num = 5;
                    num2 = 3;
                    goto IL_94;
                case 6:
                    num = 2;
                    num2 = 2;
                    goto IL_94;
                case 7:
                    num = 1;
                    num2 = 3;
                    goto IL_94;
                case 8:
                    num = 1;
                    num2 = 2;
                    goto IL_94;
                case 10:
                    num = 2;
                    num2 = 3;
                    goto IL_94;
                case 11:
                    num = 2;
                    num2 = 3;
                    goto IL_94;
                case 12:
                    num = 3;
                    num2 = 2;
                    goto IL_94;
                case 13:
                    num = 1;
                    num2 = 2;
                    goto IL_94;
            }
            num = 1;
            num2 = 2;
            IL_94:
            return new Rectangle(tileIndex * 16 % TextureSheet.texture.Width, tileIndex * 16 / TextureSheet.texture.Width * 16, num * 16, num2 * 16);
        }

        private Rectangle getDefaultBoundingBoxForType(int type)
        {
            int num;
            int num2;
            switch (type)
            {
                case 0:
                    num = 1;
                    num2 = 1;
                    goto IL_94;
                case 1:
                    num = 2;
                    num2 = 1;
                    goto IL_94;
                case 2:
                    num = 3;
                    num2 = 1;
                    goto IL_94;
                case 3:
                    num = 2;
                    num2 = 1;
                    goto IL_94;
                case 4:
                    num = 2;
                    num2 = 1;
                    goto IL_94;
                case 5:
                    num = 5;
                    num2 = 2;
                    goto IL_94;
                case 6:
                    num = 2;
                    num2 = 2;
                    goto IL_94;
                case 7:
                    num = 1;
                    num2 = 1;
                    goto IL_94;
                case 8:
                    num = 1;
                    num2 = 1;
                    goto IL_94;
                case 10:
                    num = 2;
                    num2 = 1;
                    goto IL_94;
                case 11:
                    num = 2;
                    num2 = 2;
                    goto IL_94;
                case 12:
                    num = 3;
                    num2 = 2;
                    goto IL_94;
                case 13:
                    num = 1;
                    num2 = 2;
                    goto IL_94;
            }
            num = 1;
            num2 = 1;
            IL_94:
            return new Rectangle((int)this.TileLocation.X * Game1.tileSize, (int)this.TileLocation.Y * Game1.tileSize, num * Game1.tileSize, num2 * Game1.tileSize);
        }

        private int getTypeNumberFromName(string typeName)
        {
            string key;
            switch (key = typeName.ToLower())
            {
                case "chair":
                    return 0;
                case "bench":
                    return 1;
                case "couch":
                    return 2;
                case "armchair":
                    return 3;
                case "dresser":
                    return 4;
                case "long table":
                    return 5;
                case "painting":
                    return 6;
                case "lamp":
                    return 7;
                case "decor":
                    return 8;
                case "bookcase":
                    return 10;
                case "table":
                    return 11;
                case "rug":
                    return 12;
                case "window":
                    return 13;
            }
            return 9;
        }

        public override int salePrice()
        {
            return this.Price;
        }

        public override int maximumStackSize()
        {
            return 999;
        }

        public override int getStack()
        {
            return this.Stack;
        }


        private float getScaleSize()
        {
            int num = this.sourceRect.Width / 16;
            int num2 = this.sourceRect.Height / 16;
            if (num >= 5)
            {
                return 0.75f;
            }
            if (num2 >= 3)
            {
                return 1f;
            }
            if (num <= 2)
            {
                return 2f;
            }
            if (num <= 4)
            {
                return 1f;
            }
            return 0.1f;
        }

        public override void drawWhenHeld(SpriteBatch spriteBatch, Vector2 objectPosition, StardewValley.Farmer f)
        {
            if (f.ActiveObject.bigCraftable.Value)
            {
                spriteBatch.Draw(Game1.bigCraftableSpriteSheet, objectPosition, new Microsoft.Xna.Framework.Rectangle?(StardewValley.Object.getSourceRectForBigCraftable(f.ActiveObject.ParentSheetIndex)), Color.White, 0f, Vector2.Zero, (float)Game1.pixelZoom, SpriteEffects.None, Math.Max(0f, (float)(f.getStandingY() + 2) / 10000f));
                return;
            }
            
            spriteBatch.Draw(Game1.objectSpriteSheet, objectPosition, new Microsoft.Xna.Framework.Rectangle?(GameLocation.getSourceRectForObject(f.ActiveObject.ParentSheetIndex)), Color.White, 0f, Vector2.Zero, (float)Game1.pixelZoom, SpriteEffects.None, Math.Max(0f, (float)(f.getStandingY() + 2) / 10000f));
            if (f.ActiveObject != null && f.ActiveObject.Name.Contains("="))
            {
                spriteBatch.Draw(Game1.objectSpriteSheet, objectPosition + new Vector2((float)(Game1.tileSize / 2), (float)(Game1.tileSize / 2)), new Microsoft.Xna.Framework.Rectangle?(GameLocation.getSourceRectForObject(f.ActiveObject.ParentSheetIndex)), Color.White, 0f, new Vector2((float)(Game1.tileSize / 2), (float)(Game1.tileSize / 2)), (float)Game1.pixelZoom + Math.Abs(Game1.starCropShimmerPause) / 8f, SpriteEffects.None, Math.Max(0f, (float)(f.getStandingY() + 2) / 10000f));
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

        /// <summary>
        /// Draws the item's icon in the menu.
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="location"></param>
        /// <param name="scaleSize"></param>
        /// <param name="transparency"></param>
        /// <param name="layerDepth"></param>
        /// <param name="drawStackNumber"></param>
        /// <param name="c"></param>
        /// <param name="drawShadow"></param>
        public override void drawInMenu(SpriteBatch spriteBatch, Vector2 location, float scaleSize, float transparency, float layerDepth, bool drawStackNumber, Color c, bool drawShadow)
        {
            if (drawStackNumber && this.maximumStackSize() > 1 && ((double)scaleSize > 0.3 && this.Stack != int.MaxValue) && this.Stack > 1)
                Utility.drawTinyDigits(this.Stack, spriteBatch, location + new Vector2((float)(Game1.tileSize - Utility.getWidthOfTinyDigitString(this.Stack, 3f * scaleSize)) + 3f * scaleSize, (float)((double)Game1.tileSize - 18.0 * (double)scaleSize + 2.0)), 3f * scaleSize, 1f, Color.White);
            if (drawStackNumber && this.Quality > 0)
            {
                float num = this.Quality < 4 ? 0.0f : (float)((Math.Cos((double)Game1.currentGameTime.TotalGameTime.Milliseconds * Math.PI / 512.0) + 1.0) * 0.0500000007450581);
                spriteBatch.Draw(Game1.mouseCursors, location + new Vector2(12f, (float)(Game1.tileSize - 12) + num), new Microsoft.Xna.Framework.Rectangle?(this.Quality < 4 ? new Microsoft.Xna.Framework.Rectangle(338 + (this.Quality - 1) * 8, 400, 8, 8) : new Microsoft.Xna.Framework.Rectangle(346, 392, 8, 8)), Color.White * transparency, 0.0f, new Vector2(4f, 4f), (float)(3.0 * (double)scaleSize * (1.0 + (double)num)), SpriteEffects.None, layerDepth);
            }
            spriteBatch.Draw(TextureSheet.texture, location + new Vector2((float)(Game1.tileSize), (float)(Game1.tileSize)), new Rectangle?(this.defaultSourceRect), Color.White * transparency, 0f, new Vector2((float)(this.defaultSourceRect.Width / 2), (float)(this.defaultSourceRect.Height)), 1f * this.getScaleSize() * scaleSize * .5f, SpriteEffects.None, layerDepth);
        }

        /// <summary>
        /// Draws the object to the screen.
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="alpha"></param>
        public override void draw(SpriteBatch spriteBatch, int x, int y, float alpha = 1f)
        {
            if (x == -1)
            {
                spriteBatch.Draw(TextureSheet.texture, Game1.GlobalToLocal(Game1.viewport, this.drawPosition), new Rectangle?(this.sourceRect), Color.White * alpha, 0f, Vector2.Zero, (float)Game1.pixelZoom, this.flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None, (this.Decoration_type == 12) ? 0f : ((float)(this.boundingBox.Bottom - 8) / 10000f));
            }
            else
            {
                spriteBatch.Draw(TextureSheet.texture, Game1.GlobalToLocal(Game1.viewport, new Vector2((float)(x * Game1.tileSize), (float)(y * Game1.tileSize - (this.sourceRect.Height * Game1.pixelZoom - this.boundingBox.Height)))), new Rectangle?(this.sourceRect), Color.White * alpha, 0f, Vector2.Zero, (float)Game1.pixelZoom, this.flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None, (this.Decoration_type == 12) ? 0f : ((float)(this.boundingBox.Bottom - 8) / 10000f));
            }
            if (this.heldObject.Value != null)
            {
                if (this.heldObject.Value is CoreObject)
                {
                    (this.heldObject.Value as CoreObject).drawAtNonTileSpot(spriteBatch, Game1.GlobalToLocal(Game1.viewport, new Vector2((float)(this.boundingBox.Center.X - Game1.tileSize / 2), (float)(this.boundingBox.Center.Y - (this.heldObject.Value as CoreObject).sourceRect.Height * Game1.pixelZoom - Game1.tileSize / 4))), (float)(this.boundingBox.Bottom - 7) / 10000f, alpha);
                    return;
                }
                spriteBatch.Draw(Game1.shadowTexture, Game1.GlobalToLocal(Game1.viewport, new Vector2((float)(this.boundingBox.Center.X - Game1.tileSize / 2), (float)(this.boundingBox.Center.Y - Game1.tileSize * 4 / 3))) + new Vector2((float)(Game1.tileSize / 2), (float)(Game1.tileSize * 5 / 6)), new Rectangle?(Game1.shadowTexture.Bounds), Color.White * alpha, 0f, new Vector2((float)Game1.shadowTexture.Bounds.Center.X, (float)Game1.shadowTexture.Bounds.Center.Y), 4f, SpriteEffects.None, (float)this.boundingBox.Bottom / 10000f);
                spriteBatch.Draw(Game1.objectSpriteSheet, Game1.GlobalToLocal(Game1.viewport, new Vector2((float)(this.boundingBox.Center.X - Game1.tileSize / 2), (float)(this.boundingBox.Center.Y - Game1.tileSize * 4 / 3))), new Rectangle?(GameLocation.getSourceRectForObject(this.heldObject.Value.ParentSheetIndex)), Color.White * alpha, 0f, Vector2.Zero, (float)Game1.pixelZoom, SpriteEffects.None, (float)(this.boundingBox.Bottom + 1) / 10000f);
            }
        }

        public virtual void drawAtNonTileSpot(SpriteBatch spriteBatch, Vector2 location, float layerDepth, float alpha = 1f)
        {
            spriteBatch.Draw(TextureSheet.texture, location, new Rectangle?(this.sourceRect), Color.White * alpha, 0f, Vector2.Zero, (float)Game1.pixelZoom, this.flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None, layerDepth);
        }

        public override Item getOne()
        {
            CoreObject CoreObject = new CoreObject(this.TextureSheet,this.ParentSheetIndex, this.TileLocation, this.inventoryMaxSize);

            CoreObject.drawPosition = this.drawPosition;
            CoreObject.defaultBoundingBox = this.defaultBoundingBox;
            CoreObject.boundingBox.Value = this.boundingBox.Value;
            CoreObject.currentRotation = this.currentRotation - 1;
            CoreObject.rotations = this.rotations;
            //rotate();

            return CoreObject;
        }


        /// <summary>
        /// Used in vertain cases where the mod needs to get an object relative to a content pack's directory.
        /// </summary>
        /// <param name="helper"></param>
        /// <returns></returns>
        public virtual Item getOne(IModHelper helper)
        {
            Texture2DExtended texture = new Texture2DExtended(helper, this.TextureSheet.path);
            CoreObject CoreObject = new CoreObject(texture,this.ParentSheetIndex, this.TileLocation, this.inventoryMaxSize);

            CoreObject.drawPosition = this.drawPosition;
            CoreObject.defaultBoundingBox = this.defaultBoundingBox;
            CoreObject.boundingBox.Value = this.boundingBox.Value;
            CoreObject.currentRotation = this.currentRotation - 1;
            CoreObject.rotations = this.rotations;
            //rotate();

            return CoreObject;
        }

        public virtual bool isInventoryFull()
        {
            //   Log.AsyncC("Count" + inventory.Count);
            //    Log.AsyncC("size" + inventoryMaxSize);
            if (inventory.Count >= inventoryMaxSize)
            {

                return true;
            }
            else
            {
                return false;
            }

        }

        public virtual bool addItemToInventory(Item I)
        {
            if (isInventoryFull() == false)
            {
                inventory.Add(I.getOne());
                return true;
            }
            else return false;
        }

        public virtual void spillInventoryEverywhere()
        {
            Game1.activeClickableMenu = new StorageContainer(this.inventory, 3, 3);
            this.itemReadyForHarvest = false;
            /*
            Log.AsyncC("DROPPING INVENTORY!");

            Random random = new Random(inventory.Count);
            int i = random.Next();
            i = i % 4;
            Vector2 v2 = new Vector2(this.tileLocation.X * Game1.tileSize, this.tileLocation.Y * Game1.tileSize);
            foreach (var I in inventory)
            {
                Log.AsyncY(I.Name);
                Log.AsyncO(I.getStack());
                Log.AsyncM(I.Stack);
                Log.AsyncC("Dropping an item!");
                Game1.createItemDebris(I, v2, i);
            }
            inventory.Clear();
            */
        }

        /// <summary>
        /// Adds an item to this item's internal inventory if it can. Otherwise drop it on the ground as item debris.
        /// </summary>
        /// <param name="I"></param>
        /// <returns></returns>
        public virtual bool addItemToInventoryElseDrop(Item I)
        {

            if (isInventoryFull() == false)
            {
                foreach (Item C in inventory)
                {
                    if (C == null) continue;
                    if (I.canStackWith(C) == true)
                    {
                        C.addToStack(I.Stack);
                        return true;
                    }
                    else
                    {
                        inventory.Add(I.getOne());
                        return true;
                    }
                }
                inventory.Add(I.getOne());
                return true;
            }
            else
            {
                Random random = new Random(inventory.Count);
                int i = random.Next();
                i = i % 4;
                Vector2 v2 = new Vector2(this.TileLocation.X * Game1.tileSize, this.TileLocation.Y * Game1.tileSize);
                Game1.createItemDebris(I.getOne(), v2, i);
                return false;
            }
        }

        public virtual void toggleLights()
        {

            if (lightsOn == false)
            {

                // Log.AsyncG("ADD LIGHTS");
                this.Decoration_type = 7;
                this.Type = "Lamp";

                //     this.lightSource.lightTexture = Game1.content.Load<Texture2D>("LooseSprites\\Lighting\\Lantern");
                // this.lightSource.position = tileLocation;

                // this.addLights(thisLocation, lightColor);
                this.addLights(thisLocation, lightColor);
                lightsOn = true;
            }
            if (lightsOn == true)
            {
                this.removeLights(Game1.player.currentLocation);
                lightsOn = false;
            }

        }

        public virtual void resetTexture(IModHelper helper)
        {
            TextureSheet = new Texture2DExtended(helper, TextureSheet.path);
        }

        public virtual void resetTexture()
        {
            TextureSheet = new Texture2DExtended(TextureSheet.getHelper(), TextureSheet.path);
        }

        public override string getCategoryName()
        {
            return "Core Mod Object";
            //  return base.getCategoryName();
        }

        public override Color getCategoryColor()
        {
            return Color.Black;
        }


        public static void Serialize(Item I)
        {

        }

        public static Item ParseIntoInventory()
        {
            Texture2DExtended texture = new Texture2DExtended();
            Item I = new CoreObject(texture,0, Vector2.Zero, 0);
            return I;
        }

        public static void ParseIntoWorld()
        {
            //Item I = new CoreObject(0, Vector2.Zero, 0);
            //return I;
        }

    }
}

