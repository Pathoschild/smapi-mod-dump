using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Revitalize.Resources;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Revitalize.Objects.Machines
{
    class Spawner : Machine
    {


        public override string Name
        {
            get
            {
                return this.name;
            }
        }


        

        public Spawner()
        {
            this.updateDrawPosition();
        }

        public Spawner(bool f)
        {
            //does nothng
        }


        public Spawner(int which, Vector2 Tile, int InventoryMaxSize)
        {
            InitializeBasics(InventoryMaxSize, Tile);
          //  this.thisType = this.GetType();
            if (TextureSheet == null)
            {
                TextureSheet = Game1.content.Load<Texture2D>("TileSheets\\furniture");
                texturePath = "TileSheets\\furniture";
            }
            Dictionary<int, string> dictionary = Game1.content.Load<Dictionary<int, string>>("Data\\Furniture");
            string[] array = dictionary[which].Split(new char[]
            {
                '/'
            });
            this.name = array[0];
            this.Decoration_type = this.getTypeNumberFromName(array[1]);
            this.description = "Can be placed inside your house.";
            this.defaultSourceRect = new Rectangle(which * 16 % TextureSheet.Width, which * 16 / TextureSheet.Width * 16, 1, 1);
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
                this.sourceRect = new Rectangle(which * 16 % TextureSheet.Width, which * 16 / TextureSheet.Width * 16, this.defaultSourceRect.Width * 16, this.defaultSourceRect.Height * 16);
                this.defaultSourceRect = this.sourceRect;
            }
            this.defaultBoundingBox = new Rectangle((int)this.tileLocation.X, (int)this.tileLocation.Y, 1, 1);
            if (array[3].Equals("-1"))
            {
                this.boundingBox = this.getDefaultBoundingBoxForType(this.Decoration_type);
                this.defaultBoundingBox = this.boundingBox;
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
                this.boundingBox = new Rectangle((int)this.tileLocation.X * Game1.tileSize, (int)this.tileLocation.Y * Game1.tileSize, this.defaultBoundingBox.Width * Game1.tileSize, this.defaultBoundingBox.Height * Game1.tileSize);
                this.defaultBoundingBox = this.boundingBox;
            }
            this.updateDrawPosition();
            this.rotations = Convert.ToInt32(array[4]);
            this.price = Convert.ToInt32(array[5]);
            this.parentSheetIndex = which;
        }

        //TODO: Add in path for loading in this machine type plus it's name
   

        public Spawner(int which, Vector2 Tile, string TexturePath, int InventoryMaxSize)
        {
            InitializeBasics(InventoryMaxSize, Tile);
            if (TextureSheet == null)
            {
                TextureSheet = Game1.content.Load<Texture2D>(texturePath);
                texturePath = TexturePath;
            }
            Dictionary<int, string> dictionary = Game1.content.Load<Dictionary<int, string>>("Data\\Furniture");
            string[] array = dictionary[which].Split(new char[]
            {
                '/'
            });
            this.name = array[0];
            this.Decoration_type = this.getTypeNumberFromName(array[1]);
            this.description = "Can be placed inside your house.";
            this.defaultSourceRect = new Rectangle(which * 16 % TextureSheet.Width, which * 16 / TextureSheet.Width * 16, 1, 1);
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
                this.sourceRect = new Rectangle(which * 16 % TextureSheet.Width, which * 16 / TextureSheet.Width * 16, this.defaultSourceRect.Width * 16, this.defaultSourceRect.Height * 16);
                this.defaultSourceRect = this.sourceRect;
            }
            this.defaultBoundingBox = new Rectangle((int)this.tileLocation.X, (int)this.tileLocation.Y, 1, 1);
            if (array[3].Equals("-1"))
            {
                this.boundingBox = this.getDefaultBoundingBoxForType(this.Decoration_type);
                this.defaultBoundingBox = this.boundingBox;
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
                this.boundingBox = new Rectangle((int)this.tileLocation.X * Game1.tileSize, (int)this.tileLocation.Y * Game1.tileSize, this.defaultBoundingBox.Width * Game1.tileSize, this.defaultBoundingBox.Height * Game1.tileSize);
                this.defaultBoundingBox = this.boundingBox;
            }
            this.updateDrawPosition();
            this.rotations = Convert.ToInt32(array[4]);
            this.price = Convert.ToInt32(array[5]);
            this.parentSheetIndex = which;
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
        }

        public override bool checkForAction(StardewValley.Farmer who, bool justCheckingForActivity = false)
        {
            var mState = Microsoft.Xna.Framework.Input.Mouse.GetState();
            if (mState.RightButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
            {
                // Game1.showRedMessage("YOOO");
                //do some stuff when the right button is down
                // rotate();
                if (this.heldObject != null)
                {
                    Game1.player.addItemByMenuIfNecessary(this.heldObject);
                    this.heldObject = null;
                    this.itemReadyForHarvest = false;
                }

                else {
                    if (Game1.player.ActiveObject != null)
                    {
                        var I = Game1.player.ActiveObject.getOne();
                        Game1.player.reduceActiveItemByOne();
                        this.heldObject = (StardewValley.Object)I;
                    }
                    else
                    {
                       // this.toggleLights();
                    }
                    //  Game1.player.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite(30, this.tileLocation * (float)Game1.tileSize + new Vector2(0f, (float)(-(float)Game1.tileSize / 4)), Color.White, 4, false, 50f, 10, Game1.tileSize, (this.tileLocation.Y + 1f) * (float)Game1.tileSize / 10000f + 0.0001f, -1, 0));
                    //  Game1.player.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite(Game1.animations, new Microsoft.Xna.Framework.Rectangle(256, 1856, 64, 128), 80f, 6, 999999, this.tileLocation * (float)Game1.tileSize + new Vector2(0f, (float)(-(float)Game1.tileSize * 2)), false, false, (this.tileLocation.Y + 1f) * (float)Game1.tileSize / 10000f + 0.0001f, 0f, Color.Yellow, 1f, 0f, 0f, 0f, false));
                }
                this.minutesUntilReady = 30;
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

        public override bool clicked(StardewValley.Farmer who)
        {

          //  Game1.showRedMessage("THIS IS CLICKED!!!");
            Game1.haltAfterCheck = false;

            if (this.heldObject != null)
            {
                this.spillInventoryEverywhere();
                return false;
            }

            if (this.heldObject == null && (who.ActiveObject == null || !(who.ActiveObject is Spawner)))
            {

                ///REALLY SHITTY MAKESHIFT CODE UNTIL THE REAL UI IS DONE
                if (inventory.Count != 0)
                {
                  

                    bool clear = true;
                    foreach(var v in inventory)
                    {
                        if(v ==null)
                        {
                            continue;
                        }
                        else
                        {
                            clear = false;
                        }
                    }
                    if (clear == true)
                    {
                        inventory.Clear();
                        if (this.heldObject != null) Util.addItemToInventoryElseDrop(this.heldObject.getOne());
                        //this.heldObject = new Spawner(parentSheetIndex, Vector2.Zero, this.inventoryMaxSize);
                        Util.addItemToInventoryElseDrop(this);
                        this.heldObject = null;
                        this.flaggedForPickUp = true;
                        this.thisLocation = null;
                        return true;
                    }
                    this.spillInventoryEverywhere();
                    return false;
                }


                if (Game1.player.currentLocation is FarmHouse)
                {
                    // Game1.showRedMessage("Why2?");
                  //  this.spillInventoryEverywhere();

                  if(this.heldObject!=null)  Util.addItemToInventoryElseDrop(this.heldObject.getOne());
                    this.heldObject = new Spawner(parentSheetIndex, Vector2.Zero, this.inventoryMaxSize);
                    Util.addItemToInventoryAndCleanTrackedList(this);
                    this.heldObject = null;
                    this.flaggedForPickUp = true;
                    this.thisLocation = null;
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
                    else {
                       
                        //    Util.addItemToInventoryElseDrop(this.heldObject);

                        var obj = new Spawner(parentSheetIndex, Vector2.Zero,this.inventoryMaxSize);
                        Util.addItemToInventoryAndCleanTrackedList(this);
                        //     this.spillInventoryEverywhere();
                        if (this.heldObject!=null) this.heldObject.performRemoveAction(this.tileLocation, who.currentLocation);

                        this.heldObject = null;
                        Game1.playSound("coin");
                        this.thisLocation = null;
                        return true;
                    }

                }
            }
            if (this.heldObject != null && who.addItemToInventoryBool(this.heldObject, false))
            {
                // Game1.showRedMessage("Why3?");
                // if(this.heldObject!=null) Game1.player.addItemByMenuIfNecessary((Item)this.heldObject);
               // this.spillInventoryEverywhere();
                var obj = new Spawner(parentSheetIndex, Vector2.Zero, this.inventoryMaxSize);
                Util.addItemToInventoryAndCleanTrackedList(this);
                if (this.heldObject!=null) this.heldObject.performRemoveAction(this.tileLocation, who.currentLocation);
                this.heldObject = null;
                Game1.playSound("coin");
                this.thisLocation = null;
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
               // this.removeLights(location);
                return;
            }

            // this.addLights(thisLocation, lightColor);
           // this.addLights(thisLocation, lightColor);
        }

        public override void resetOnPlayerEntry(GameLocation environment)
        {
            this.removeLights(environment);
            if (Game1.isDarkOut())
            {

              //   this.addLights(thisLocation, lightColor);
                //this.addLights(thisLocation, lightColor);
            }
        }

        public override bool performObjectDropInAction(StardewValley.Object dropIn, bool probe, StardewValley.Farmer who)
        {
            // Log.AsyncG("HEY!");

            if ((this.Decoration_type == 11 || this.Decoration_type == 5) && this.heldObject == null && !dropIn.bigCraftable && (!(dropIn is Spawner) || ((dropIn as Spawner).getTilesWide() == 1 && (dropIn as Spawner).getTilesHigh() == 1)))
            {
                this.heldObject = (StardewValley.Object)dropIn.getOne();
                this.heldObject.tileLocation = this.tileLocation;
                this.heldObject.boundingBox.X = this.boundingBox.X;
                this.heldObject.boundingBox.Y = this.boundingBox.Y;
               // Log.AsyncO(getDefaultBoundingBoxForType((dropIn as Spawner).Decoration_type));
                this.heldObject.performDropDownAction(who);
                this.heldObject = null;
                if (!probe)
                {
                    Game1.playSound("woodyStep");
                   // Log.AsyncC("HUH?");
                    if (who != null)
                    {
                        who.reduceActiveItemByOne();
                    }
                }
                return true;
            }
            return false;
        }

        public override void addLights(GameLocation environment)
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
                    Utility.removeLightSource((int)(this.tileLocation.X * 2000f + this.tileLocation.Y));
                    this.lightSource = new LightSource(4, new Vector2((float)(this.boundingBox.X + Game1.tileSize / 2), (float)(this.boundingBox.Y - Game1.tileSize)), 2f, Color.Black, (int)(this.tileLocation.X * 2000f + this.tileLocation.Y));
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
        }
        
        public override void addLights(GameLocation environment, Color c)
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
                    Utility.removeLightSource((int)(this.tileLocation.X * 2000f + this.tileLocation.Y));
                    this.lightSource = new LightSource(4, new Vector2((float)(this.boundingBox.X + Game1.tileSize / 2), (float)(this.boundingBox.Y - Game1.tileSize)), 2f, c, (int)(this.tileLocation.X * 2000f + this.tileLocation.Y));
                   // this.lightSource.lightTexture = Game1.content.Load<Texture2D>("LooseSprites\\Lighting\\BlueLight");
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
                    Utility.removeLightSource((int)(this.tileLocation.X * 2000f + this.tileLocation.Y));
                    this.lightSource = new LightSource(4, new Vector2((float)(this.boundingBox.X + Game1.tileSize / 2), (float)(this.boundingBox.Y - Game1.tileSize)), 2f, c, (int)(this.tileLocation.X * 2000f + this.tileLocation.Y));
                    Game1.currentLightSources.Add(this.lightSource);
                    return;
                }

            }
        }
        
        private void removeLights(GameLocation environment)
        {
            if (this.Decoration_type == 7)
            {
                if (this.sourceIndexOffset == 1)
                {
                    this.sourceRect = this.defaultSourceRect;
                }
                this.sourceIndexOffset = 0;
                Utility.removeLightSource((int)(this.tileLocation.X * 2000f + this.tileLocation.Y));
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
        }

        public override bool minutesElapsed(int minutes, GameLocation environment)
        {
           // Log.Info("minutes passed in" + minutes);
       //  if(this.lightSource!=null)   Log.AsyncO(this.lightSource.color.ToString());
         //   Log.Info(this.Decoration_type);
            if (this.lightSource != null) this.lightSource.color = new Color(0, 255, 255, 255);  //THE COLORS ARE REVERSED!!!!!!!!!!!!
            this.minutesUntilReady = (this.minutesUntilReady - minutes);
           // Log.Info("minues remaining" + this.minutesUntilReady);

          // if(this.heldObject!= null) Log.Info("stack size"+this.heldObject.stack);
            if (Game1.isDarkOut())
            {

                // this.addLights(thisLocation, lightColor);
              //  this.addLights(thisLocation, lightColor);
                //  if (this.lightSource != null) this.lightSource.color = Color.DarkMagenta;
            }
            else
            {
                this.removeLights(environment);
            }

            if (minutesUntilReady == 0)
            {
               // Log.AsyncC(this.name + "Is ready!");
                // Log.AsyncC(Game1.player.getStandingPosition());
                  Vector2 v2 = new Vector2(this.tileLocation.X * Game1.tileSize, this.tileLocation.Y * Game1.tileSize);
                //Log.AsyncC(v2);
                //Game1.createItemDebris((Item)this.heldObject, v2, Game1.player.getDirection());
                if (this.heldObject != null)
                {
                    addItemToInventoryElseDrop(this.heldObject.getOne());

                    minutesUntilReady = 30;
                    this.itemReadyForHarvest = true;
                }
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

        public override void rotate()
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
            bool flag = this.Decoration_type == 5 || this.Decoration_type == 12 || this.parentSheetIndex == 724 || this.parentSheetIndex == 727;
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
                        foreach (KeyValuePair<Vector2, StardewValley.Object> something in l.objects)
                        {
                            StardewValley.Object obj = something.Value;
                            if ((obj.GetType()).ToString().Contains("Spawner"))
                            {
                                Spawner current = (Spawner)obj;
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
                        foreach (Spawner current in (l as FarmHouse).Spawner)
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
                this.boundingBox = new Rectangle(x / Game1.tileSize * Game1.tileSize, y / Game1.tileSize * Game1.tileSize, this.boundingBox.Width, this.boundingBox.Height);
                foreach (KeyValuePair<Vector2, StardewValley.Object> c in location.objects)
                {
                    StardewValley.Object ehh = c.Value;
                    if (((ehh.GetType()).ToString()).Contains("Spawner"))
                    {
                        Spawner current2 = (Spawner)ehh;
                        if (current2.Decoration_type == 11 && current2.heldObject == null && current2.getBoundingBox(current2.tileLocation).Intersects(this.boundingBox))
                        {
                            current2.performObjectDropInAction(this, false, (who == null) ? Game1.player : who);
                            bool result = true;
                            return result;
                        }
                    }
                }
                foreach (StardewValley.Farmer current3 in location.getFarmers())
                {
                    if (current3.GetBoundingBox().Intersects(this.boundingBox))
                    {
                        Game1.showRedMessage("Can't place on top of a person.");
                        bool result = false;
                        return result;
                    }
                }
                this.updateDrawPosition();
                //Log.AsyncO(this.boundingBox);
                //Log.AsyncO(x);
                //Log.AsyncY(y);
                for (int i = 0; i <= this.boundingBox.X / Game1.tileSize; i++)
                {
                    return Util.placementAction(this, location, x+1, y, who);
                }
                for (int i = 0; i <= this.boundingBox.Y / Game1.tileSize; i++)
                {
                    return Util.placementAction(this, location, x, y+1, who);
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
                    */
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
                        */
                        if (location.getTileIndexAt(i, j, "Buildings") != -1)
                        {
                            return false;
                        }
                    }
                }
                this.boundingBox = new Rectangle(x / Game1.tileSize * Game1.tileSize, y / Game1.tileSize * Game1.tileSize, this.boundingBox.Width, this.boundingBox.Height);
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
                    if (current3.GetBoundingBox().Intersects(this.boundingBox))
                    {
                        Game1.showRedMessage("Can't place on top of a person.");
                        bool result = false;
                        return result;
                    }
                }
                this.updateDrawPosition();
                this.thisLocation = Game1.player.currentLocation;
                return Util.placementAction(this,location, x, y, who);
            }

        }

        public override bool isPlaceable()
        {
            return true;
        }

        public override Rectangle getBoundingBox(Vector2 tileLocation)
        {
            return this.boundingBox;
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
            return new Rectangle(tileIndex * 16 % TextureSheet.Width, tileIndex * 16 / TextureSheet.Width * 16, num * 16, num2 * 16);
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
            return new Rectangle((int)this.tileLocation.X * Game1.tileSize, (int)this.tileLocation.Y * Game1.tileSize, num * Game1.tileSize, num2 * Game1.tileSize);
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
            return this.price;
        }

        public override int maximumStackSize()
        {
            return 1;
        }

        public override int getStack()
        {
            return this.stack;
        }

        public override int addToStack(int amount)
        {
            return 1;
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
            spriteBatch.Draw(TextureSheet, objectPosition ,Color.White);
            //base.drawWhenHeld(spriteBatch, objectPosition, f);
        }

        public override void drawInMenu(SpriteBatch spriteBatch, Vector2 location, float scaleSize, float transparency, float layerDepth, bool drawStackNumber)
        {
            spriteBatch.Draw(TextureSheet, location + new Vector2((float)(Game1.tileSize / 2), (float)(Game1.tileSize / 2)), new Rectangle?(this.defaultSourceRect), Color.White * transparency, 0f, new Vector2((float)(this.defaultSourceRect.Width / 2), (float)(this.defaultSourceRect.Height / 2)), 1f * this.getScaleSize() * scaleSize, SpriteEffects.None, layerDepth);
        }

        public override void draw(SpriteBatch spriteBatch, int x, int y, float alpha = 1f)
        {
            if (x == -1)
            {
                spriteBatch.Draw(TextureSheet, Game1.GlobalToLocal(Game1.viewport, this.drawPosition), new Rectangle?(this.sourceRect), Color.White * alpha, 0f, Vector2.Zero, (float)Game1.pixelZoom, this.flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None, (this.Decoration_type == 12) ? 0f : ((float)(this.boundingBox.Bottom - 8) / 10000f));
            }
            else
            {
                spriteBatch.Draw(TextureSheet, Game1.GlobalToLocal(Game1.viewport, new Vector2((float)(x * Game1.tileSize), (float)(y * Game1.tileSize - (this.sourceRect.Height * Game1.pixelZoom - this.boundingBox.Height)))), new Rectangle?(this.sourceRect), Color.White * alpha, 0f, Vector2.Zero, (float)Game1.pixelZoom, this.flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None, (this.Decoration_type == 12) ? 0f : ((float)(this.boundingBox.Bottom - 8) / 10000f));
            }
            if (this.heldObject != null)
            {
                if (this.heldObject is Spawner)
                {
                    (this.heldObject as Spawner).drawAtNonTileSpot(spriteBatch, Game1.GlobalToLocal(Game1.viewport, new Vector2((float)(this.boundingBox.Center.X - Game1.tileSize / 2), (float)(this.boundingBox.Center.Y - (this.heldObject as Spawner).sourceRect.Height * Game1.pixelZoom - Game1.tileSize / 4))), (float)(this.boundingBox.Bottom - 7) / 10000f, alpha);
                    return;
                }
                spriteBatch.Draw(Game1.shadowTexture, Game1.GlobalToLocal(Game1.viewport, new Vector2((float)(this.boundingBox.Center.X - Game1.tileSize / 2), (float)(this.boundingBox.Center.Y - Game1.tileSize * 4 / 3))) + new Vector2((float)(Game1.tileSize / 2), (float)(Game1.tileSize * 5 / 6)), new Rectangle?(Game1.shadowTexture.Bounds), Color.White * alpha, 0f, new Vector2((float)Game1.shadowTexture.Bounds.Center.X, (float)Game1.shadowTexture.Bounds.Center.Y), 4f, SpriteEffects.None, (float)this.boundingBox.Bottom / 10000f);
                spriteBatch.Draw(Game1.objectSpriteSheet, Game1.GlobalToLocal(Game1.viewport, new Vector2((float)(this.boundingBox.Center.X - Game1.tileSize / 2), (float)(this.boundingBox.Center.Y - Game1.tileSize * 4 / 3))), new Rectangle?(Game1.currentLocation.getSourceRectForObject(this.heldObject.ParentSheetIndex)), Color.White * alpha, 0f, Vector2.Zero, (float)Game1.pixelZoom, SpriteEffects.None, (float)(this.boundingBox.Bottom + 1) / 10000f);
            }

            if (this.itemReadyForHarvest)
            {
                if (this.heldObject != null)
                {
                    float num = 4f * (float)Math.Round(Math.Sin(DateTime.Now.TimeOfDay.TotalMilliseconds / 250.0), 2);
                    spriteBatch.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2((float)(x * Game1.tileSize - 8), (float)(y * Game1.tileSize - Game1.tileSize * 3 / 2 - 16) + num)), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(141, 465, 20, 24)), Color.White * 0.75f, 0f, Vector2.Zero, 4f, SpriteEffects.None, (float)((y + 1) * Game1.tileSize) / 10000f + 1E-06f + this.tileLocation.X / 10000f + ((this.parentSheetIndex == 105) ? 0.0015f : 0f));
                    spriteBatch.Draw(Game1.objectSpriteSheet, Game1.GlobalToLocal(Game1.viewport, new Vector2((float)(x * Game1.tileSize + Game1.tileSize / 2), (float)(y * Game1.tileSize - Game1.tileSize - Game1.tileSize / 8) + num)), new Microsoft.Xna.Framework.Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, this.heldObject.parentSheetIndex, 16, 16)), Color.White * 0.75f, 0f, new Vector2(8f, 8f), (float)Game1.pixelZoom, SpriteEffects.None, (float)((y + 1) * Game1.tileSize) / 10000f + 1E-05f + this.tileLocation.X / 10000f + ((this.parentSheetIndex == 105) ? 0.0015f : 0f));
                }
            }
        }

        public override void drawAtNonTileSpot(SpriteBatch spriteBatch, Vector2 location, float layerDepth, float alpha = 1f)
        {
            spriteBatch.Draw(TextureSheet, location, new Rectangle?(this.sourceRect), Color.White * alpha, 0f, Vector2.Zero, (float)Game1.pixelZoom, this.flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None, layerDepth);
        }

        public override Item getOne()
        {
            Spawner machine = new Spawner(this.parentSheetIndex, this.tileLocation,this.inventoryMaxSize);
            /*
            drawPosition = this.drawPosition;
            defaultBoundingBox = this.defaultBoundingBox;
            boundingBox = this.boundingBox;
            currentRotation = this.currentRotation - 1;
            rotations = this.rotations;
            rotate();
            */
            return machine;
        }

        public override string getCategoryName()
        {
            return "Duplicator";
            //  return base.getCategoryName();
        }

        public override Color getCategoryColor()
        {
            return Util.invertColor(LightColors.CornflowerBlue);
        }

    }
}
