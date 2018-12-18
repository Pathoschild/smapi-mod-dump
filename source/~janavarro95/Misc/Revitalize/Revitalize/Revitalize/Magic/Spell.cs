using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Revitalize.Resources;
using Revitalize.Resources.DataNodes;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace Revitalize.Objects
{
    /// <summary>
    /// Stardew Spell Class. VERY Broken. Only extend from this class.
    /// </summary>
    /// 
    public class Spell : Revitalize.CoreObject
    {
        public delegate void  spellFunction(Spell s) ;

        public List<SpellFunctionDataNode> magicToCast;

        public int spellCostModifierInt;
        public float spellCostModifierPercent;

        public int spellIndex;

        public string vanillaDescription;
        public int usesRemaining;


        public override string Name
        {
            get
            {
                return this.name;
            }
        }

        public override void InitializeBasics(int InvMaxSize, Vector2 tile)
        {
            this.inventory = new List<Item>();
            this.magicToCast = new List<SpellFunctionDataNode>();
            this.inventoryMaxSize = InvMaxSize;
            this.tileLocation = tile;
            lightsOn = false;
            lightColor = Color.Black;
            //thisType = this.GetType();
            
        }

        public Spell()
        {
            this.updateDrawPosition();
        }


        public Spell(int which,Vector2 Tile, SpellFunctionDataNode spellFunction,Color drawColor,int textureIndex=0, int InventoryMaxSize = 0, bool isRemovable = true)
        {
            InitializeBasics(InventoryMaxSize, Tile);
                this.drawColor = drawColor;
            spellIndex = which;
           
            magicToCast.Add(spellFunction);
            removable = isRemovable;
            this.lightColor = Color.Black;
            if (TextureSheet == null)
            {
                TextureSheet = Game1.content.Load<Texture2D>(Path.Combine(Magic.MagicMonitor.magicalTexturesPath,"Spells"));
                texturePath = (Path.Combine(Magic.MagicMonitor.magicalTexturesPath, "Spells"));
            }
            Dictionary<int, string> dictionary = Game1.content.Load<Dictionary<int, string>>(Path.Combine(Magic.MagicMonitor.magicalDataPath, "Spells"));
            string[] array = dictionary[which].Split(new char[]
            {
                '/'
            });
            this.name = array[0];
            this.Decoration_type = this.getTypeNumberFromName(array[1]);
            this.description = "Can be placed inside your house.";
            this.defaultSourceRect = new Rectangle(textureIndex * 16 % TextureSheet.Width, textureIndex * 16 / TextureSheet.Width * 16, 1, 1);
                this.defaultSourceRect.Width = Convert.ToInt32(array[2].Split(new char[]
                {
                    ' '
                })[0]);
                this.defaultSourceRect.Height = Convert.ToInt32(array[2].Split(new char[]
                {
                    ' '
                })[1]);
                this.sourceRect = new Rectangle(textureIndex * 16 % TextureSheet.Width, textureIndex * 16 / TextureSheet.Width * 16, this.defaultSourceRect.Width * 16, this.defaultSourceRect.Height * 16);
                this.defaultSourceRect = this.sourceRect;
            
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
            this.parentSheetIndex = textureIndex ;
            this.ParentSheetIndex = textureIndex;

            try
            {
                this.spellCostModifierInt = Convert.ToInt32(array[6]);
                int meh = Convert.ToInt32(array[7]);
                this.spellCostModifierPercent = meh / 100;
                this.vanillaDescription = array[8];
                try
                {
                    this.usesRemaining = Convert.ToInt32(array[9]);
                }
                catch(Exception err)
                {
                    this.usesRemaining = -1;
                }
                if (usesRemaining == -1)
                {
                    this.description = vanillaDescription;
                }
                else
                {
                    this.description = this.vanillaDescription += "\nUses Remaining: " + usesRemaining;
                }
            }
            catch(Exception e)
            {
                this.spellCostModifierInt = 0;
                this.spellCostModifierPercent = 0;
                this.description = "A spell book. You aren't sure what it does so why don't you test it out?";
            }

        }

        public Spell(int which, Vector2 Tile, List<SpellFunctionDataNode> spellFunctions,Color tomeColor,int textureIndex=0, int InventoryMaxSize = 0, bool isRemovable = true)
        {      
            drawColor = tomeColor;
            spellIndex = which;
            InitializeBasics(InventoryMaxSize, Tile);

            foreach(var v in spellFunctions) {
                magicToCast.Add(v);
                    }

            removable = isRemovable;
            this.lightColor = Color.Black;
            if (TextureSheet == null)
            {
                TextureSheet = Game1.content.Load<Texture2D>(Path.Combine(Magic.MagicMonitor.magicalTexturesPath, "Spells"));
                texturePath = (Path.Combine(Magic.MagicMonitor.magicalTexturesPath, "Spells"));
            }
            Dictionary<int, string> dictionary = Game1.content.Load<Dictionary<int, string>>(Path.Combine(Magic.MagicMonitor.magicalDataPath, "Spells"));
            string[] array = dictionary[which].Split(new char[]
            {
                '/'
            });
            this.name = array[0];
            this.Decoration_type = this.getTypeNumberFromName(array[1]);
            this.description = "Can be placed inside your house.";
            this.defaultSourceRect = new Rectangle(textureIndex * 16 % TextureSheet.Width, textureIndex * 16 / TextureSheet.Width * 16, 1, 1);
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
                this.sourceRect = new Rectangle(textureIndex * 16 % TextureSheet.Width, textureIndex * 16 / TextureSheet.Width * 16, this.defaultSourceRect.Width * 16, this.defaultSourceRect.Height * 16);
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
            this.parentSheetIndex = textureIndex;

            try
            {
                this.spellCostModifierInt = Convert.ToInt32(array[6]);
                int meh = Convert.ToInt32(array[7]);
                this.spellCostModifierPercent = meh / 100;
                this.description = array[8];
            }
            catch (Exception e)
            {
                this.spellCostModifierInt = 0;
                this.spellCostModifierPercent = 0;
                this.description = "A spell book. You aren't sure what it does so why don't you test it out?";
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
        }



        public override bool checkForAction(StardewValley.Farmer who, bool justCheckingForActivity = false)
        {
            var mState = Microsoft.Xna.Framework.Input.Mouse.GetState();
            if (mState.RightButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
            {
                return this.RightClicked(who);

                // Game1.showRedMessage("YOOO");
                //do some stuff when the right button is down
                // rotate();
                if (this.heldObject != null)
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
        public override bool clicked(StardewValley.Farmer who)
        {
            if (removable == false) return false;
            Game1.haltAfterCheck = false;
            if (this.Decoration_type == 11 && who.ActiveObject != null && who.ActiveObject != null && this.heldObject == null)
            {
                
                return false;
            }
            if (this.heldObject == null && (who.ActiveObject == null || !(who.ActiveObject is Spell)))
            {
                if (Game1.player.currentLocation is FarmHouse)
                {
                         
                    Util.addItemToInventoryAndCleanTrackedList(this);
                    removeLights(this.thisLocation);
                    this.lightsOn = false;
                    Game1.playSound("coin");
                    //        this.flaggedForPickUp = true;
                    thisLocation = null;
                    return true;
                }
                else
                {
                    Util.addItemToInventoryAndCleanTrackedList(this);
                    removeLights(this.thisLocation);
                    this.lightsOn = false;
                    Game1.playSound("coin");
                    thisLocation = null;
                    return true;

                }
            }
            if (this.heldObject != null && who.addItemToInventoryBool(this.heldObject, false))
            {
                Util.addItemToInventoryAndCleanTrackedList(this);
                this.heldObject.performRemoveAction(this.tileLocation, who.currentLocation);
                this.heldObject = null;
                Game1.playSound("coin");
                removeLights(this.thisLocation);
                this.lightsOn = false;
                thisLocation = null;
                return true;
            }

            return false;
        }

        public virtual bool RightClicked(StardewValley.Farmer who)
        {

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
            // this.addLights(thisLocation, lightColor);
            this.addLights(thisLocation, lightColor);
        }

        public override bool performObjectDropInAction(StardewValley.Object dropIn, bool probe, StardewValley.Farmer who)
        {
            Game1.showRedMessage("HEY!");
            if ((this.Decoration_type == 11 || this.Decoration_type == 5) && this.heldObject == null && !dropIn.bigCraftable && (!(dropIn is Spell) || ((dropIn as Spell).getTilesWide() == 1 && (dropIn as Spell).getTilesHigh() == 1)))
            {
                this.heldObject = (StardewValley.Object)dropIn.getOne();
                this.heldObject.tileLocation = this.tileLocation;
                this.heldObject.boundingBox.X = this.boundingBox.X;
                this.heldObject.boundingBox.Y = this.boundingBox.Y;
                //  Log.AsyncO(getDefaultBoundingBoxForType((dropIn as Spell).Decoration_type));
                this.heldObject.performDropDownAction(who);
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
            return false;
        }


        public override bool minutesElapsed(int minutes, GameLocation environment)
        {
            // Log.Info("minutes passed in"+minutes);
            //  Log.Info("minues remaining" + this.minutesUntilReady);
            // Log.Info(this.lightColor);
            this.minutesUntilReady = (this.minutesUntilReady - minutes);
            if (Game1.isDarkOut())
            {
                // this.addLights(thisLocation, lightColor);
                this.addLights(thisLocation, lightColor);
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

            return false;
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
                            if ((obj.GetType()).ToString().Contains("Spell"))
                            {
                                Spell current = (Spell)obj;
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
                        foreach (Spell current in (l as FarmHouse).Spell)
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
                return Util.canBePlacedHere(this, l, tile);
            }
        }

        public virtual void updateDrawPosition()
        {
            this.drawPosition = new Vector2((float)this.boundingBox.X, (float)(this.boundingBox.Y - (this.sourceRect.Height * Game1.pixelZoom - this.boundingBox.Height)));
        }

        public override void drawPlacementBounds(SpriteBatch spriteBatch, GameLocation location)
        {

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

            // Game1.showRedMessage("BALLS");
            return true;

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
                        Decoration current2 = (Decoration)ehh;
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
                //  Log.AsyncO(this.boundingBox);
                //   Log.AsyncO(x);
                //   Log.AsyncY(y);
                for (int i = 0; i <= this.boundingBox.X / Game1.tileSize; i++)
                {
                    Util.placementAction(this, location, x + 1, y, who);
                }
                for (int i = 0; i <= this.boundingBox.Y / Game1.tileSize; i++)
                {
                    Util.placementAction(this, location, x + 1, y, who);
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
                //  Log.AsyncC(x);
                //   Log.AsyncY(y);
                //   Log.AsyncY(this.drawPosition);
                return Util.placementAction(this, location, x, y, who);
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
            spriteBatch.Draw(this.TextureSheet, objectPosition, new Microsoft.Xna.Framework.Rectangle?(Game1.currentLocation.getSourceRectForObject(f.ActiveObject.ParentSheetIndex)), this.drawColor, 0f, Vector2.Zero, (float)Game1.pixelZoom, SpriteEffects.None, Math.Max(0f, (float)(f.getStandingY() + 2) / 10000f));
            if (f.ActiveObject != null && f.ActiveObject.Name.Contains("="))
            {
                spriteBatch.Draw(Game1.objectSpriteSheet, objectPosition + new Vector2((float)(Game1.tileSize / 2), (float)(Game1.tileSize / 2)), new Microsoft.Xna.Framework.Rectangle?(Game1.currentLocation.getSourceRectForObject(f.ActiveObject.ParentSheetIndex)), Color.White, 0f, new Vector2((float)(Game1.tileSize / 2), (float)(Game1.tileSize / 2)), (float)Game1.pixelZoom + Math.Abs(Game1.starCropShimmerPause) / 8f, SpriteEffects.None, Math.Max(0f, (float)(f.getStandingY() + 2) / 10000f));
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
        }

        public override void drawInMenu(SpriteBatch spriteBatch, Vector2 location, float scaleSize, float transparency, float layerDepth, bool drawStackNumber)
        {
 
            spriteBatch.Draw(TextureSheet, location + new Vector2((float)(Game1.tileSize / 2), (float)(Game1.tileSize / 2)), new Rectangle?(this.defaultSourceRect),this.drawColor, 0f, new Vector2((float)(this.defaultSourceRect.Width / 2), (float)(this.defaultSourceRect.Height / 2)), 1f * this.getScaleSize() * scaleSize, SpriteEffects.None, layerDepth);
        }

        public override void draw(SpriteBatch spriteBatch, int x, int y, float alpha = 1f)
        {
            if (x == -1)
            {
                spriteBatch.Draw(TextureSheet, Game1.GlobalToLocal(Game1.viewport, this.drawPosition), new Rectangle?(this.sourceRect), this.drawColor * alpha, 0f, Vector2.Zero, (float)Game1.pixelZoom, this.flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None, (this.Decoration_type == 12) ? 0f : ((float)(this.boundingBox.Bottom - 8) / 10000f));
            }
            else
            {
                spriteBatch.Draw(TextureSheet, Game1.GlobalToLocal(Game1.viewport, new Vector2((float)(x * Game1.tileSize), (float)(y * Game1.tileSize - (this.sourceRect.Height * Game1.pixelZoom - this.boundingBox.Height)))), new Rectangle?(this.sourceRect), this.drawColor * alpha, 0f, Vector2.Zero, (float)Game1.pixelZoom, this.flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None, (this.Decoration_type == 12) ? 0f : ((float)(this.boundingBox.Bottom - 8) / 10000f));
            }
            if (this.heldObject != null)
            {
                if (this.heldObject is Spell)
                {
                    (this.heldObject as Spell).drawAtNonTileSpot(spriteBatch, Game1.GlobalToLocal(Game1.viewport, new Vector2((float)(this.boundingBox.Center.X - Game1.tileSize / 2), (float)(this.boundingBox.Center.Y - (this.heldObject as Spell).sourceRect.Height * Game1.pixelZoom - Game1.tileSize / 4))), (float)(this.boundingBox.Bottom - 7) / 10000f, alpha);
                    return;
                }
                spriteBatch.Draw(Game1.shadowTexture, Game1.GlobalToLocal(Game1.viewport, new Vector2((float)(this.boundingBox.Center.X - Game1.tileSize / 2), (float)(this.boundingBox.Center.Y - Game1.tileSize * 4 / 3))) + new Vector2((float)(Game1.tileSize / 2), (float)(Game1.tileSize * 5 / 6)), new Rectangle?(Game1.shadowTexture.Bounds), this.drawColor * alpha, 0f, new Vector2((float)Game1.shadowTexture.Bounds.Center.X, (float)Game1.shadowTexture.Bounds.Center.Y), 4f, SpriteEffects.None, (float)this.boundingBox.Bottom / 10000f);
                spriteBatch.Draw(Game1.objectSpriteSheet, Game1.GlobalToLocal(Game1.viewport, new Vector2((float)(this.boundingBox.Center.X - Game1.tileSize / 2), (float)(this.boundingBox.Center.Y - Game1.tileSize * 4 / 3))), new Rectangle?(Game1.currentLocation.getSourceRectForObject(this.heldObject.ParentSheetIndex)), this.drawColor * alpha, 0f, Vector2.Zero, (float)Game1.pixelZoom, SpriteEffects.None, (float)(this.boundingBox.Bottom + 1) / 10000f);
            }
        }

        public override void drawAtNonTileSpot(SpriteBatch spriteBatch, Vector2 location, float layerDepth, float alpha = 1f)
        {
            spriteBatch.Draw(TextureSheet, location, new Rectangle?(this.sourceRect), this.drawColor * alpha, 0f, Vector2.Zero, (float)Game1.pixelZoom, this.flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None, layerDepth);
        }

        public override Item getOne()
        {
            Spell Spell = new Spell(this.spellIndex, this.tileLocation, magicToCast ,this.drawColor, this.parentSheetIndex);
            /*
            drawPosition = this.drawPosition;
            defaultBoundingBox = this.defaultBoundingBox;
            boundingBox = this.boundingBox;
            currentRotation = this.currentRotation - 1;
            rotations = this.rotations;
            rotate();
            */
            return Spell;
        }

        public override string getCategoryName()
        {
            return "Spell";
            //  return base.getCategoryName();
        }

        public override Color getCategoryColor()
        {
            return Util.invertColor(LightColors.Aqua);
        }



        public virtual void getContents()
        {
            List<Item> removalList = new List<Item>();

            // Log.AsyncC("step 1");
            foreach (var v in this.inventory)
            {
                if (Game1.player.isInventoryFull() == true) break;
                //  Log.AsyncC("ok");
                v.hasBeenInInventory = true;
                Util.addItemToInventoryElseDrop(v);
                removalList.Add(v);

            }

            foreach (var v in removalList)
            {
                this.inventory.Remove(v);
            }
            removalList.Clear();
            if (this.inventory.Count == 0)
            {
                Game1.player.reduceActiveItemByOne();
            }
            else
            {
                Util.addItemToInventoryElseUseMenu(this.inventory);
            }

        }

        public override int maximumStackSize()
        {
            return 1;
        }

        public void castMagic()
        {
            foreach (var v in magicToCast)
            {
                for (int i = 1; i <= v.timesToCast; i++)
                {
                    v.spellToCast.Invoke(this);
                }
            }
        }

    }
}
