
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json.Linq;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using StardustCore;
using StardustCore.Animations;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace StarAI.PathFindingCore
{
    /// <summary>
    /// Original Stardew Furniture Class but rewritten to be placed anywhere.
    /// </summary>
    public class TileNode : CoreObject
    {
        public Vector2 position;
        public List<TileNode> children = new List<TileNode>();
        public enum stateEnum { NotVisited, Seen, Visited };
        public int seenState;

        
        public int Decoration_type;

        public int rotations;

        public int currentRotation;

        private int sourceIndexOffset;

        public Vector2 drawPosition;

        public Rectangle sourceRect;

        public Rectangle defaultSourceRect;

        public Rectangle defaultBoundingBox;


        public string description;

        public Texture2D TextureSheet;

        public new bool flipped;

        [XmlIgnore]
        public bool flaggedForPickUp;

        private bool lightGlowAdded;

        public string texturePath;
        public string dataPath;

        public bool IsSolid;

        public int cost;
        public int costToThisPoint;

        public TileNode parent;

        public static bool checkIfICanPlaceHere(TileNode t, Vector2 pos, GameLocation loc = null, bool checkForPassableTerrainFeatures = true)
        {
            bool cry = false;
            if (t.thisLocation == null)
            {
                t.thisLocation = loc;
                cry = true;
            }
            

            if (t == null)
            {
                Console.WriteLine("OK T IS NULL");
            }
            if (t.thisLocation == null)
            {
                Console.WriteLine("OK T LOCATION IS NULL");
            }


            if (t.thisLocation.isObjectAt((int)pos.X, (int)pos.Y))
            {
                //ModCore.CoreMonitor.Log("Object at this tile position!: " + t.thisLocation.objects[new Vector2(pos.X/Game1.tileSize,pos.Y/Game1.tileSize)].name, LogLevel.Warn);
                if (cry == true) t.thisLocation = null;
                return false;
            }



            if (checkForPassableTerrainFeatures)
            {
                bool terrainFeature = t.thisLocation.terrainFeatures.ContainsKey(pos / Game1.tileSize);
                if (terrainFeature)
                {
                    TerrainFeature terrain = t.thisLocation.terrainFeatures[pos / Game1.tileSize];
                    if (terrain.isPassable()) return true;
                }
            }
            if (t.thisLocation.isTileOccupied(pos / Game1.tileSize))
            {
               // ModCore.CoreMonitor.Log("Tile occupied!: " + t.thisLocation.name, LogLevel.Error);
                if (cry == true) t.thisLocation = null;
                return false;
            }

            if (t.thisLocation.isTilePlaceable(pos / Game1.tileSize) == false)
            {
                //ModCore.CoreMonitor.Log("Tile Not placeable at location. " + t.thisLocation.name, LogLevel.Error);
                if (cry == true) t.thisLocation = null;
                return false;
            }



            if (t.thisLocation.isTilePassable(new xTile.Dimensions.Location((int)(pos.X/Game1.tileSize), (int)(pos.Y/Game1.tileSize)), Game1.viewport)==false)
            {
               // ModCore.CoreMonitor.Log("Tile not passable check 2?????!!!!: " + t.thisLocation.name, LogLevel.Error);
                if (cry == true) t.thisLocation = null;
                return false;
            }
            


            if (cry == true) t.thisLocation = null;
            return true;
        }

        public static void setSingleTileAsChild(TileNode t,int x, int y,bool placementAction=true)
        {
            
            Vector2 pos = new Vector2(x * Game1.tileSize, y * Game1.tileSize);
          
           bool f= checkIfICanPlaceHere(t, new Vector2(pos.X,pos.Y));
            if (f == false) return;
            else
            {
                
               // ModCore.CoreMonitor.Log("Adding a child!");
                System.Threading.Thread.Sleep(PathFindingCore.PathFindingLogic.delay);
                TileNode child = new TileNode(1, Vector2.Zero, t.texturePath,t.dataPath,StardustCore.IlluminateFramework.Colors.invertColor(StardustCore.IlluminateFramework.ColorsList.Cyan));
                child.seenState = (int)stateEnum.NotVisited;
                child.parent = t;
                if (placementAction) child.placementAction(t.thisLocation, (int)pos.X, (int)pos.Y);
                else child.fakePlacementAction(t.thisLocation, x, y);
                //StarAI.PathFindingCore.Utilities.masterAdditionList.Add(new StarAI.PathFindingCore.PlacementNode(child, Game1.currentLocation, (int)pos.X, (int)pos.Y));
                t.children.Add(child);
            }
        }



        public override string Name
        {
            get
            {
                return this.name;
            }
        }

        public TileNode()
        {
            this.updateDrawPosition();
        }

        public TileNode(bool f)
        {
            //does nothng
        }

        /// <summary>
        /// TileNode Constructor. Does not use an animation manager.
        /// </summary>
        /// <param name="which"></param>
        /// <param name="tile"></param>
        /// <param name="ObjectTexture"></param>
        /// <param name="isRemovable"></param>
        /// <param name="price"></param>
        /// <param name="isSolid"></param>
        public TileNode(int which, Vector2 tile, string ObjectTexture, Color DrawColor, bool isRemovable = true, int price = 0, bool isSolid = false,int Cost=1)
        {
            this.cost = Cost;
            Color c=DrawColor;
            if (c == null)
            {
                c = StardustCore.IlluminateFramework.ColorsList.White;
            }
            this.drawColor = c;

            removable = isRemovable;
            this.serializationName = Convert.ToString(GetType());
            // this.thisType = GetType();
            this.tileLocation = tile;
            this.InitializeBasics(0, tile);
            if (TextureSheet == null)
            {
                TextureSheet = ModCore.CoreHelper.Content.Load<Texture2D>(ObjectTexture); //Game1.content.Load<Texture2D>("TileSheets\\furniture");
                texturePath = ObjectTexture;
            }
            this.dataPath = "";
           
            this.name = "Tile Node";

            this.description = "A tile that is used to depict information.";




            this.defaultSourceRect = new Rectangle(which * 16 % TextureSheet.Width, which * 16 / TextureSheet.Width * 16, 1, 1);

            this.defaultSourceRect.Width = 1;
            this.defaultSourceRect.Height = 1;
            this.sourceRect = new Rectangle(which * 16 % TextureSheet.Width, which * 16 / TextureSheet.Width * 16, this.defaultSourceRect.Width * 16, this.defaultSourceRect.Height * 16);
            this.defaultSourceRect = this.sourceRect;

            this.defaultBoundingBox = new Rectangle((int)this.tileLocation.X, (int)this.tileLocation.Y, 1, 1);

            this.defaultBoundingBox.Width = 1;
            this.defaultBoundingBox.Height = 1;
            IsSolid = isSolid;
            if (isSolid == true)
            {
                this.boundingBox = new Rectangle((int)this.tileLocation.X * Game1.tileSize, (int)this.tileLocation.Y * Game1.tileSize, this.defaultBoundingBox.Width * Game1.tileSize, this.defaultBoundingBox.Height * Game1.tileSize);
            }
            else
            {
                this.boundingBox = new Rectangle(int.MinValue, (int)this.tileLocation.Y * Game1.tileSize, 0, 0); //Throw the bounding box away as far as possible.
            }
            this.defaultBoundingBox = this.boundingBox;
            this.updateDrawPosition();
            this.price = price;
            this.parentSheetIndex = which;
        }


        public TileNode(int which, Vector2 tile, string ObjectTexture, string DataPath, Color DrawColor, bool isRemovable = true, bool isSolid = false,int Cost=1)
        {
            try
            {
                this.cost = Cost;
                Color c = DrawColor;
                if (c == null)
                {
                    c = StardustCore.IlluminateFramework.ColorsList.White;
                }
                this.drawColor = c;
                this.serializationName = Convert.ToString(GetType());
                removable = isRemovable;
                // this.thisType = GetType();
                this.tileLocation = tile;
                this.InitializeBasics(0, tile);
                TextureSheet = ModCore.CoreHelper.Content.Load<Texture2D>(ObjectTexture); //Game1.content.Load<Texture2D>("TileSheets\\furniture");
                texturePath = ObjectTexture;
                Dictionary<int, string> dictionary;
                try
                {

                    dictionary = ModCore.CoreHelper.Content.Load<Dictionary<int, string>>(DataPath);
                    dataPath = DataPath;


                    string s = "";
                    dictionary.TryGetValue(which, out s);
                    string[] array = s.Split('/');
                    this.name = array[0];
                    this.displayName = array[0];
                   
                    this.description = array[1];
                    this.defaultSourceRect = new Rectangle(which * 16 % TextureSheet.Width, which * 16 / TextureSheet.Width * 16, 1, 1);

                    this.defaultSourceRect.Width = 1;
                    this.defaultSourceRect.Height = 1;
                    this.sourceRect = new Rectangle(which * 16 % TextureSheet.Width, which * 16 / TextureSheet.Width * 16, this.defaultSourceRect.Width * 16, this.defaultSourceRect.Height * 16);
                    this.defaultSourceRect = this.sourceRect;
                    try
                    {
                        this.animationManager = new StardustCore.Animations.AnimationManager(this.TextureSheet, new StardustCore.Animations.Animation(this.defaultSourceRect, -1), AnimationManager.parseAnimationsFromXNB(array[3]), "Default");
                        this.animationManager.setAnimation("Default", 0);
                        //Log.AsyncC("Using animation manager");
                    }
                    catch (Exception errr)
                    {
                        this.animationManager = new StardustCore.Animations.AnimationManager(this.TextureSheet, new StardustCore.Animations.Animation(this.defaultSourceRect, -1));
                        ModCore.CoreMonitor.Log(errr.ToString());
                    }
                    this.defaultBoundingBox = new Rectangle((int)this.tileLocation.X, (int)this.tileLocation.Y, 1, 1);

                    this.defaultBoundingBox.Width = 1;
                    this.defaultBoundingBox.Height = 1;
                    IsSolid = isSolid;
                    if (isSolid == true)
                    {
                        this.boundingBox = new Rectangle((int)this.tileLocation.X * Game1.tileSize, (int)this.tileLocation.Y * Game1.tileSize, this.defaultBoundingBox.Width * Game1.tileSize, this.defaultBoundingBox.Height * Game1.tileSize);
                    }
                    else
                    {
                        this.boundingBox = new Rectangle(int.MinValue, (int)this.tileLocation.Y * Game1.tileSize, 0, 0); //Throw the bounding box away as far as possible.
                    }
                    this.defaultBoundingBox = this.boundingBox;
                    this.updateDrawPosition();
                    this.price = Convert.ToInt32(array[2]);
                    this.parentSheetIndex = which;



                }
                catch (Exception e)
                {
                    ModCore.CoreMonitor.Log(e.ToString());
                    //  Log.AsyncC(e);
                }

            }
            catch(Exception fu)
            {
                ModCore.CoreMonitor.Log(fu.ToString());
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
        }

        public override bool checkForAction(StardewValley.Farmer who, bool justCheckingForActivity = false)
        {
            var mState = Microsoft.Xna.Framework.Input.Mouse.GetState();
            if (mState.RightButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
            {
                // Game1.showRedMessage("YOOO");
                //do some stuff when the right button is down
                // rotate();
                return true;
            }

            if (justCheckingForActivity)
            {
                return true;
            }

            return this.clicked(who); //check for left clicked action.
        }

        public override bool clicked(StardewValley.Farmer who)
        {
            int range = 2;
            if (StardustCore.Utilities.isWithinRange(range, this.tileLocation) == false) return false;

            if (StardustCore.Utilities.isWithinDirectionRange(Game1.player.FacingDirection, range, this.tileLocation))
            {
                if (Game1.player.CurrentItem != null)
                {
                    if (Game1.player.getToolFromName(Game1.player.CurrentItem.Name) is StardewValley.Tools.WateringCan)
                    {
                        //this.animationManager.setAnimation("Watered", 0);
                        //TileExceptionMetaData t = new TileExceptionMetaData(this, "Water");
                        bool foundRemoval = false;
                        foreach(var exc in Utilities.tileExceptionList)
                        {
                            if (exc.tile == this && exc.actionType=="Water")
                            {
                                this.thisLocation.objects.Remove(this.tileLocation);
                                foundRemoval = true;
                                StardustCore.ModCore.SerializationManager.trackedObjectList.Remove(this);
                            }
                        }
                        if(foundRemoval==true) PathFindingCore.Utilities.cleanExceptionList(this);
                        return false;
                    }
                }

                if (Game1.player.CurrentItem != null)
                {
                    if (Game1.player.CurrentItem is StardewValley.Tools.MeleeWeapon || Game1.player.CurrentItem is StardewValley.Tools.Sword)
                    {
                       
                    }
                }
            }




            if (removable == false) return false;
            //   Game1.showRedMessage("THIS IS CLICKED!!!");
            Game1.haltAfterCheck = false;
            if (this.Decoration_type == 11 && who.ActiveObject != null && who.ActiveObject != null && this.heldObject == null)
            {
                //  Game1.showRedMessage("Why1?");
                return false;
            }
            if (this.heldObject == null && (who.ActiveObject == null || !(who.ActiveObject is TileNode)))
            {
                if (Game1.player.currentLocation is FarmHouse)
                {
                    //   Game1.showRedMessage("Why2?");
                    // this.heldObject = new TileNode(parentSheetIndex, Vector2.Zero);
                    StardustCore.Utilities.addItemToInventoryAndCleanTrackedList(this,StardustCore.ModCore.SerializationManager);
                    this.flaggedForPickUp = true;
                    this.thisLocation = null;
                    this.locationsName = "";
                    return true;
                }
                else
                {
                    // return true;

                    this.flaggedForPickUp = true;
                    if (this is TV)
                    {
                        this.heldObject = new TV(parentSheetIndex, Vector2.Zero);
                    }
                    else
                    {
                        //  this.heldObject = new TileNode(parentSheetIndex, Vector2.Zero);
                        StardustCore.Utilities.addItemToInventoryAndCleanTrackedList(this,StardustCore.ModCore.SerializationManager);
                        //  this.heldObject.performRemoveAction(this.tileLocation, who.currentLocation);
                        //   this.heldObject = null;
                        Game1.playSound("coin");
                        this.thisLocation = null;
                        this.locationsName = "";
                        return true;
                    }

                }
            }
            if (this.heldObject != null && who.addItemToInventoryBool(this.heldObject, false))
            {
                // Game1.showRedMessage("Why3?");
                this.heldObject.performRemoveAction(this.tileLocation, who.currentLocation);
                this.heldObject = null;
                StardustCore.Utilities.addItemToInventoryAndCleanTrackedList(this,StardustCore.ModCore.SerializationManager);
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

        }

        public void resetOnPlayerEntry(GameLocation environment)
        {
            this.removeLights(environment);
            if (Game1.isDarkOut())
            {
                this.addLights(environment);
            }
        }

        public override bool performObjectDropInAction(StardewValley.Object dropIn, bool probe, StardewValley.Farmer who)
        {
            if ((this.Decoration_type == 11 || this.Decoration_type == 5) && this.heldObject == null && !dropIn.bigCraftable && (!(dropIn is TileNode) || ((dropIn as TileNode).getTilesWide() == 1 && (dropIn as TileNode).getTilesHigh() == 1)))
            {
                this.heldObject = (StardewValley.Object)dropIn.getOne();
                this.heldObject.tileLocation = this.tileLocation;
                this.heldObject.boundingBox.X = this.boundingBox.X;
                this.heldObject.boundingBox.Y = this.boundingBox.Y;
                //  Log.AsyncO(getDefaultBoundingBoxForType((dropIn as TileNode).Decoration_type));
                this.heldObject.performDropDownAction(who);
                if (!probe)
                {
                    Game1.playSound("woodyStep");
                    //   Log.AsyncC("HUH?");
                    if (who != null)
                    {
                        who.reduceActiveItemByOne();
                    }
                }
                return true;
            }
            return false;
        }

        private void addLights(GameLocation environment)
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
                    Utility.removeLightSource((int)(this.tileLocation.X * 2000f + this.tileLocation.Y));
                    this.lightSource = new LightSource(4, new Vector2((float)(this.boundingBox.X + Game1.tileSize / 2), (float)(this.boundingBox.Y - Game1.tileSize)), 2f, Color.Black, (int)(this.tileLocation.X * 2000f + this.tileLocation.Y));
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
        public bool isGroundFurniture()
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
                            if ((obj.GetType()).ToString().Contains("TileNode"))
                            {
                                TileNode current = (TileNode)obj;
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
                        foreach (TileNode current in (l as FarmHouse).TileNode)
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

        public void updateDrawPosition()
        {
            this.drawPosition = new Vector2((float)this.boundingBox.X, (float)(this.boundingBox.Y - (this.sourceRect.Height * Game1.pixelZoom - this.boundingBox.Height)));
        }

        public int getTilesWide()
        {
            return this.boundingBox.Width / Game1.tileSize;
        }

        public int getTilesHigh()
        {
            return this.boundingBox.Height / Game1.tileSize;
        }

        public override bool placementAction(GameLocation location, int x, int y, StardewValley.Farmer who = null)
        {

            Point point = new Point(x / Game1.tileSize, y / Game1.tileSize);
            this.position = new Vector2(x, y);

            this.tileLocation = new Vector2((float)point.X, (float)point.Y);
            bool flag = false;

            if (this.IsSolid)
            {
                this.boundingBox = new Rectangle(x / Game1.tileSize * Game1.tileSize, y / Game1.tileSize * Game1.tileSize, this.boundingBox.Width, this.boundingBox.Height);
            }
            else
            {
                this.boundingBox = new Rectangle(int.MinValue, y / Game1.tileSize * Game1.tileSize, 0, 0);
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
                    if (enumerator3.Current.GetBoundingBox().Intersects(this.boundingBox))
                    {
                        Game1.showRedMessage("Can't place on top of a person.");
                        bool result = false;
                        return result;
                    }
                }
            }
            this.updateDrawPosition();
            try
            {
                bool f = StardustCore.Utilities.placementAction(this, location, x, y, StardustCore.ModCore.SerializationManager, who);
               
                this.thisLocation = Game1.player.currentLocation;
                return f;
            }
            catch(Exception err)
            {
                ModCore.CoreMonitor.Log(err.ToString());
                return false;
            }
            
            //  Game1.showRedMessage("Can only be placed in House");
            //  return false;
        }

        public override bool isPlaceable()
        {
            return true;
        }

        public override Rectangle getBoundingBox(Vector2 tileLocation)
        {
            return this.boundingBox;
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

        public override void drawWhenHeld(SpriteBatch spriteBatch, Vector2 objectPosition, StardewValley.Farmer f)
        {
            if (animationManager == null)
            {
                spriteBatch.Draw(this.TextureSheet, objectPosition, new Microsoft.Xna.Framework.Rectangle?(Game1.currentLocation.getSourceRectForObject(f.ActiveObject.ParentSheetIndex)), this.drawColor, 0f, Vector2.Zero, (float)Game1.pixelZoom, SpriteEffects.None, Math.Max(0f, (float)(f.getStandingY() + 2) / 10000f));
            }
            else
            {
                spriteBatch.Draw(this.TextureSheet, objectPosition, this.animationManager.currentAnimation.sourceRectangle, this.drawColor, 0f, Vector2.Zero, (float)Game1.pixelZoom, SpriteEffects.None, Math.Max(0f, (float)(f.getStandingY() + 2) / 10000f));
            }

            if (f.ActiveObject != null && f.ActiveObject.Name.Contains("="))
            {
                spriteBatch.Draw(this.TextureSheet, objectPosition + new Vector2((float)(Game1.tileSize / 2), (float)(Game1.tileSize / 2)), new Microsoft.Xna.Framework.Rectangle?(Game1.currentLocation.getSourceRectForObject(f.ActiveObject.ParentSheetIndex)), this.drawColor, 0f, new Vector2((float)(Game1.tileSize / 2), (float)(Game1.tileSize / 2)), (float)Game1.pixelZoom + Math.Abs(Game1.starCropShimmerPause) / 8f, SpriteEffects.None, Math.Max(0f, (float)(f.getStandingY() + 2) / 10000f));
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

        public override void drawInMenu(SpriteBatch spriteBatch, Vector2 location, float scaleSize, float transparency, float layerDepth, bool drawStackNumber)
        {
            try
            {
                if (animationManager == null) spriteBatch.Draw(TextureSheet, location + new Vector2((float)(Game1.tileSize / 2), (float)(Game1.tileSize / 2)), new Rectangle?(this.defaultSourceRect), this.drawColor * transparency, 0f, new Vector2((float)(this.defaultSourceRect.Width / 2), (float)(this.defaultSourceRect.Height / 2)), 1f * (3) * scaleSize, SpriteEffects.None, layerDepth);
                else
                {
                    spriteBatch.Draw(animationManager.objectTexture, location + new Vector2((float)(Game1.tileSize / 2), (float)(Game1.tileSize / 2)), new Rectangle?(animationManager.currentAnimation.sourceRectangle), this.drawColor * transparency, 0f, new Vector2((float)(this.defaultSourceRect.Width / 2), (float)(this.defaultSourceRect.Height / 2)), 1f * (3) * scaleSize, SpriteEffects.None, layerDepth);


                    //this.modularCrop.drawInMenu(spriteBatch, location + new Vector2((float)(Game1.tileSize / 2), (float)(Game1.tileSize / 2)), this.drawColor, 0f,true);

                    if (Game1.player.CurrentItem != this) animationManager.tickAnimation();
                }
                Vector2 v = location + new Vector2((float)(Game1.tileSize / 2), (float)(Game1.tileSize / 2));
            }
            catch(Exception err)
            {
                ModCore.CoreMonitor.Log(err.ToString());
            }
        }



        public override void draw(SpriteBatch spriteBatch, int x, int y, float alpha = 1f)
        {
            try
            {
                    //The actual planter box being drawn.
                    if (animationManager == null)
                    {
                        spriteBatch.Draw(TextureSheet, Game1.GlobalToLocal(Game1.viewport, new Vector2((float)(x * Game1.tileSize), y * Game1.tileSize)), new Rectangle?(this.sourceRect), this.drawColor * alpha, 0f, Vector2.Zero, (float)Game1.pixelZoom, this.flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
                        // Log.AsyncG("ANIMATION IS NULL?!?!?!?!");
                    }

                    else
                    {
                        //Log.AsyncC("Animation Manager is working!");
                        spriteBatch.Draw(animationManager.objectTexture, Game1.GlobalToLocal(Game1.viewport, new Vector2((float)(x * Game1.tileSize), y * Game1.tileSize)), new Rectangle?(animationManager.currentAnimation.sourceRectangle), this.drawColor * alpha, 0f, Vector2.Zero, (float)Game1.pixelZoom, this.flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
                        try
                        {
                            this.animationManager.tickAnimation();
                            // Log.AsyncC("Tick animation");
                        }
                        catch (Exception err)
                        {
                            ModCore.CoreMonitor.Log(err.ToString());
                        }
                    }

                    // spriteBatch.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2((float)((double)tileLocation.X * (double)Game1.tileSize + (((double)tileLocation.X * 11.0 + (double)tileLocation.Y * 7.0) % 10.0 - 5.0)) + (float)(Game1.tileSize / 2), (float)((double)tileLocation.Y * (double)Game1.tileSize + (((double)tileLocation.Y * 11.0 + (double)tileLocation.X * 7.0) % 10.0 - 5.0)) + (float)(Game1.tileSize / 2))), new Rectangle?(new Rectangle((int)((double)tileLocation.X * 51.0 + (double)tileLocation.Y * 77.0) % 3 * 16, 128 + this.whichForageCrop * 16, 16, 16)), this.drawColor, 0.0f, new Vector2(8f, 8f), (float)Game1.pixelZoom, SpriteEffects.None, (float)(((double)tileLocation.Y * (double)Game1.tileSize + (double)(Game1.tileSize / 2) + (((double)tileLocation.Y * 11.0 + (double)tileLocation.X * 7.0) % 10.0 - 5.0)) / 10000.0));

                    if (this.heldObject != null)
                    {
                        if (this.heldObject is TileNode)
                        {
                            (this.heldObject as TileNode).drawAtNonTileSpot(spriteBatch, Game1.GlobalToLocal(Game1.viewport, new Vector2((float)(this.boundingBox.Center.X - Game1.tileSize / 2), (float)(this.boundingBox.Center.Y - (this.heldObject as TileNode).sourceRect.Height * Game1.pixelZoom - Game1.tileSize / 4))), (float)(this.boundingBox.Bottom - 7) / 10000f, alpha);
                            return;
                        }
                        spriteBatch.Draw(Game1.shadowTexture, Game1.GlobalToLocal(Game1.viewport, new Vector2((float)(this.boundingBox.Center.X - Game1.tileSize / 2), (float)(this.boundingBox.Center.Y - Game1.tileSize * 4 / 3))) + new Vector2((float)(Game1.tileSize / 2), (float)(Game1.tileSize * 5 / 6)), new Rectangle?(Game1.shadowTexture.Bounds), this.drawColor * alpha, 0f, new Vector2((float)Game1.shadowTexture.Bounds.Center.X, (float)Game1.shadowTexture.Bounds.Center.Y), 4f, SpriteEffects.None, (float)this.boundingBox.Bottom / 10000f);
                        spriteBatch.Draw(Game1.objectSpriteSheet, Game1.GlobalToLocal(Game1.viewport, new Vector2((float)(this.boundingBox.Center.X - Game1.tileSize / 2), (float)(this.boundingBox.Center.Y - Game1.tileSize * 4 / 3))), new Rectangle?(Game1.currentLocation.getSourceRectForObject(this.heldObject.ParentSheetIndex)), this.drawColor * alpha, 0f, Vector2.Zero, (float)Game1.pixelZoom, SpriteEffects.None, (float)(this.boundingBox.Bottom + 1) / 10000f);
                    }


                
            }
            catch(Exception err)
            {
                ModCore.CoreMonitor.Log(err.ToString());
            }
        }



        public new void drawAtNonTileSpot(SpriteBatch spriteBatch, Vector2 location, float layerDepth, float alpha = 1f)
        {
            try
            {
                spriteBatch.Draw(TextureSheet, location, new Rectangle?(this.sourceRect), this.drawColor * alpha, 0f, Vector2.Zero, (float)Game1.pixelZoom, this.flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None, layerDepth);
            }
            catch(Exception err)
            {
                ModCore.CoreMonitor.Log(err.ToString());

            }
            //  this.drawCrops(spriteBatch, (int)location.X*Game1.tileSize, (int)location.Y*Game1.tileSize, 1, true);
        }

        public override Item getOne()
        {
            try
            {
                if (this.dataPath == "") return new TileNode(this.parentSheetIndex, this.tileLocation, this.texturePath,this.drawColor, this.removable, this.price, this.IsSolid);
                else return new TileNode(this.parentSheetIndex, this.tileLocation, this.texturePath, this.dataPath,this.drawColor, this.removable, this.IsSolid);
            }
            catch (Exception err)
            {
                ModCore.CoreMonitor.Log(err.ToString());
                return null;
            }
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
            return "Tile Node";
            //  return base.getCategoryName();
        }

        public override Color getCategoryColor()
        {
            return Color.Purple;
        }

        public static new void Serialize(Item I)
        {
            StardustCore.ModCore.SerializationManager.WriteToJsonFile(Path.Combine(StardustCore.ModCore.SerializationManager.playerInventoryPath, I.Name + ".json"), (TileNode)I);
        }

        public static void Serialize(Item I, string s)
        {
            StardustCore.ModCore.SerializationManager.WriteToJsonFile(Path.Combine(s, I.Name + ".json"), (TileNode)I);
        }

        public static Item ParseIntoInventory(string s)
        {
            // TileNode p = new TileNode();
            // return p;



            dynamic obj = JObject.Parse(s);

            TileNode d = new TileNode();

            d.dataPath = obj.dataPath;
            d.price = obj.price;
            d.Decoration_type = obj.Decoration_type;
            d.rotations = obj.rotations;
            d.currentRotation = obj.currentRotation;
            string s1 = Convert.ToString(obj.sourceRect);
            d.sourceRect = StardustCore.Utilities.parseRectFromJson(s1);
            string s2 = Convert.ToString(obj.defaultSourceRect);
            d.defaultSourceRect = StardustCore.Utilities.parseRectFromJson(s2);
            string s3 = Convert.ToString(obj.defaultBoundingBox);
            d.defaultBoundingBox = StardustCore.Utilities.parseRectFromJson(s3);
            d.description = obj.description;
            d.flipped = obj.flipped;
            d.flaggedForPickUp = obj.flaggedForPickUp;
            d.tileLocation = obj.tileLocation;
            d.parentSheetIndex = obj.parentSheetIndex;
            d.owner = obj.owner;
            d.name = obj.name;
            d.type = obj.type;
            d.canBeSetDown = obj.canBeSetDown;
            d.canBeGrabbed = obj.canBeGrabbed;
            d.isHoedirt = obj.isHoedirt;
            d.isSpawnedObject = obj.isSpawnedObject;
            d.questItem = obj.questItem;
            d.isOn = obj.isOn;
            d.fragility = obj.fragility;
            d.edibility = obj.edibility;
            d.stack = obj.stack;
            d.quality = obj.quality;
            d.bigCraftable = obj.bigCraftable;
            d.setOutdoors = obj.setOutdoors;
            d.setIndoors = obj.setIndoors;
            d.readyForHarvest = obj.readyForHarvest;
            d.showNextIndex = obj.showNextIndex;
            d.hasBeenPickedUpByFarmer = obj.hasBeenPickedUpByFarmer;
            d.isRecipe = obj.isRecipe;
            d.isLamp = obj.isLamp;
            d.heldObject = obj.heldObject;
            d.minutesUntilReady = obj.minutesUntilReady;
            string s4 = Convert.ToString(obj.boundingBox);
            d.boundingBox = StardustCore.Utilities.parseRectFromJson(s4);
            d.scale = obj.scale;
            d.lightSource = obj.lightSource;
            d.shakeTimer = obj.shakeTimer;
            d.internalSound = obj.internalSound;
            d.specialVariable = obj.specialVariable;
            d.category = obj.category;
            d.specialItem = obj.specialItem;
            d.hasBeenInInventory = obj.hasBeenInInventory;


            string t = obj.texturePath;
            d.TextureSheet = ModCore.CoreHelper.Content.Load<Texture2D>(t);
            d.texturePath = t;
            JArray array = obj.inventory;
            d.inventory = array.ToObject<List<Item>>();
            d.inventoryMaxSize = obj.inventoryMaxSize;
            d.itemReadyForHarvest = obj.itemReadyForHarvest;
            d.lightsOn = obj.lightsOn;
            d.lightColor = obj.lightColor;
            d.thisType = obj.thisType;
            d.removable = obj.removable;
            d.locationsName = obj.locationsName;

            d.drawColor = obj.drawColor;

            d.serializationName = obj.serializationName;
            d.IsSolid = obj.IsSolid;

            string IsWatered = obj.isWatered;
            // Log.AsyncC("AM I WATERED OR NOT?!?!?: "+IsWatered);
            //ANIMATIONS
            var q = obj.animationManager;
            dynamic obj1 = q;
            string name = Convert.ToString(obj1.currentAnimationName);
            int frame = Convert.ToInt32(obj1.currentAnimationListIndex);
            TileNode dummy = new TileNode(d.parentSheetIndex, d.tileLocation, d.texturePath, d.dataPath,d.drawColor, d.removable, d.IsSolid);
            d.animationManager = dummy.animationManager;
            bool f = d.animationManager.setAnimation(name, frame);
            bool f2;
            if (f == false)
            {
                f2 = d.animationManager.setAnimation(name, 0);
                if (f2 == false) d.animationManager.currentAnimation = d.animationManager.defaultDrawFrame;
            }


            try
            {
                return d;
            }
            catch (Exception e)
            {
                // Log.AsyncM(e);
                return null;
            }

            //return base.ParseIntoInventory();
        }

        public static void SerializeFromWorld(Item I)
        {
            //makeCropInformationString(I);
            //  ModCore.serilaizationManager.WriteToJsonFile(Path.Combine(ModCore.serilaizationManager.objectsInWorldPath, (c as CoreObject).thisLocation.name, c.Name + ".json"), (TileNode)c);
            StardustCore.ModCore.SerializationManager.WriteToJsonFile(Path.Combine(StardustCore.ModCore.SerializationManager.objectsInWorldPath, I.Name + ".json"), (TileNode)I);
        }





    }
}