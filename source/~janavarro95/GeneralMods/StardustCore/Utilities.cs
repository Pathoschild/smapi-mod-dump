
using Microsoft.Xna.Framework;
using Netcode;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Characters;
using StardewValley.Locations;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;
using StardustCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xTile.Dimensions;

namespace StardustCore
{
    

    public class Utilities
    {

        public static List<CoreObject> masterRemovalList = new List<CoreObject>();


        public static string getShortenedDirectory(string modName,string path)
        {
            string lol = (string)path.Clone();
            string[] spliter = lol.Split(new string[] { modName }, StringSplitOptions.None);
            return spliter[1];
        }

        public static string getRelativeDirectory(string modName,string path)
        {
            string s = getShortenedDirectory(modName,path);
            return s.Remove(0, 1);
        }

        public static string getRelativeDiretory(IModHelper modHelper, string path)
        {
            string s = getShortenedDirectory(modHelper, path);
            return s.Remove(0, 1);
        }

        public static string getShortenedDirectory(IModHelper modHelper, string path)
        {
            string lol = (string)path.Clone();
            string[] spliter = lol.Split(new string[] { modHelper.DirectoryPath }, StringSplitOptions.None);
            return spliter[1];
        }

        public static int sellToStorePrice(CoreObject c)
        {
            return  (int)((double)c.Price * (1.0 + (double)c.Quality * 0.25));
        }

        /// <summary>
        /// Returns an absolute path past the mod's directory.
        /// </summary>
        /// <param name="absolutePath"></param>
        /// <returns></returns>
        public static string getRelativePath(string absolutePath)
        {
            var ok= absolutePath.Split(new string[] { "StardustCore" }, StringSplitOptions.None);
            return ok.ElementAt(1);
        }


        /// <summary>
        /// Create some object debris at my game location.
        /// </summary>
        /// <param name="objectIndex"></param>
        /// <param name="xTile"></param>
        /// <param name="yTile"></param>
        /// <param name="groundLevel"></param>
        /// <param name="itemQuality"></param>
        /// <param name="velocityMultiplyer"></param>
        /// <param name="location"></param>
        public static void createObjectDebris(int objectIndex, int xTile, int yTile, int groundLevel = -1, int itemQuality = 0, float velocityMultiplyer = 1f, GameLocation location = null)
        {
            if (location == null)
                location = Game1.currentLocation;
            Debris debris = new Debris(objectIndex, new Vector2((float)(xTile * 64 + 32), (float)(yTile * 64 + 32)), new Vector2((float)Game1.player.getStandingX(), (float)Game1.player.getStandingY()))
            {
                itemQuality = itemQuality
            };
            foreach (Chunk chunk in (IEnumerable<Chunk>)debris.Chunks)
            {
                chunk.xVelocity.Value *= (float)(double)velocityMultiplyer;
                chunk.yVelocity.Value *= (float)(double)velocityMultiplyer;
            }
            if (groundLevel != -1)
                debris.chunkFinalYLevel = groundLevel;
            location.debris.Add(debris);
        }



        /// <summary>
        /// Place a core object into a game location.
        /// </summary>
        /// <param name="cObj"></param>
        /// <param name="location"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="s"></param>
        /// <param name="who"></param>
        /// <param name="playSound"></param>
        /// <returns></returns>
        public static bool placementAction(CoreObject cObj, GameLocation location, int x, int y,Serialization.SerializationManager s, StardewValley.Farmer who = null, bool playSound = true)
        {
            Vector2 vector = new Vector2((float)(x / Game1.tileSize), (float)(y / Game1.tileSize));
            //  cObj.health = 10;
            if (who != null)
            {
                cObj.owner.Value = who.UniqueMultiplayerID;
            }
            else
            {
                cObj.owner.Value = Game1.player.UniqueMultiplayerID;
            }

                int num = cObj.ParentSheetIndex;
                if (num <= 130)
                {
                    if (num == 71)
                    {
                        if (location is MineShaft)
                        {
                            if ((location as MineShaft).mineLevel != 120 && (location as MineShaft).recursiveTryToCreateLadderDown(vector, "hoeHit", 16))
                            {
                                return true;
                            }
                            Game1.showRedMessage("Unsuitable Location");
                        }
                        return false;
                    }
                    if (num == 130)
                    {
                        if (location.objects.ContainsKey(vector) || Game1.currentLocation is MineShaft)
                        {
                            Game1.showRedMessage("Unsuitable Location");
                            return false;
                        }
                        location.objects.Add(vector, new Chest(true)
                        {
                            shakeTimer = 50
                        });
                        Game1.playSound("axe");
                        return true;
                    }
                }
                else
                {
                    switch (num)
                    {
                        case 143:
                        case 144:
                        case 145:
                        case 146:
                        case 147:
                        case 148:
                        case 149:
                        case 150:
                        case 151:
                            if (location.objects.ContainsKey(vector))
                            {
                                return false;
                            }
                            new Torch(vector, cObj.ParentSheetIndex, true)
                            {
                                shakeTimer = 25
                            }.placementAction(location, x, y, who);
                            return true;
                        default:
                            if (num == 163)
                            {
                                location.objects.Add(vector, new Cask(vector));
                                Game1.playSound("hammer");
                            }
                            break;
                    }
                }
            
            if (cObj.name.Equals("Tapper"))
            {
                if (location.terrainFeatures.ContainsKey(vector) && location.terrainFeatures[vector] is Tree && (location.terrainFeatures[vector] as Tree).growthStage.Value >= 5 && !(location.terrainFeatures[vector] as Tree).stump.Value && !location.objects.ContainsKey(vector))
                {
                    cObj.TileLocation = vector;
                    location.objects.Add(vector, cObj);
                    int treeType = (location.terrainFeatures[vector] as Tree).treeType.Value;
                    (location.terrainFeatures[vector] as Tree).tapped.Value = true;
                    switch (treeType)
                    {
                        case 1:
                            cObj.heldObject.Value = new StardewValley.Object(725, 1, false, -1, 0);
                            cObj.MinutesUntilReady = 13000 - Game1.timeOfDay;
                            break;
                        case 2:
                            cObj.heldObject.Value = new StardewValley.Object(724, 1, false, -1, 0);
                            cObj.MinutesUntilReady = 16000 - Game1.timeOfDay;
                            break;
                        case 3:
                            cObj.heldObject.Value = new StardewValley.Object(726, 1, false, -1, 0);
                            cObj.MinutesUntilReady = 10000 - Game1.timeOfDay;
                            break;
                        case 7:
                            cObj.heldObject.Value = new StardewValley.Object(420, 1, false, -1, 0);
                            cObj.MinutesUntilReady = 3000 - Game1.timeOfDay;
                            if (!Game1.currentSeason.Equals("fall"))
                            {
                                cObj.heldObject.Value = new StardewValley.Object(404, 1, false, -1, 0);
                                cObj.MinutesUntilReady = 6000 - Game1.timeOfDay;
                            }
                            break;
                    }
                    Game1.playSound("axe");
                    return true;
                }
                return false;
            }
            else if (cObj.name.Contains("Sapling"))
            {
                Vector2 key = default(Vector2);
                for (int i = x / Game1.tileSize - 2; i <= x / Game1.tileSize + 2; i++)
                {
                    for (int j = y / Game1.tileSize - 2; j <= y / Game1.tileSize + 2; j++)
                    {
                        key.X = (float)i;
                        key.Y = (float)j;
                        if (location.terrainFeatures.ContainsKey(key) && (location.terrainFeatures[key] is Tree || location.terrainFeatures[key] is FruitTree))
                        {
                            Game1.showRedMessage("Too close to another tree");
                            return false;
                        }
                    }
                }
                if (location.terrainFeatures.ContainsKey(vector))
                {
                    if (!(location.terrainFeatures[vector] is HoeDirt) || (location.terrainFeatures[vector] as HoeDirt).crop != null)
                    {
                        return false;
                    }
                    location.terrainFeatures.Remove(vector);
                }
                if (location is Farm && (location.doesTileHaveProperty((int)vector.X, (int)vector.Y, "Diggable", "Back") != null || location.doesTileHavePropertyNoNull((int)vector.X, (int)vector.Y, "Type", "Back").Equals("Grass")))
                {
                    Game1.playSound("dirtyHit");
                    DelayedAction.playSoundAfterDelay("coin", 100);
                    location.terrainFeatures.Add(vector, new FruitTree(cObj.ParentSheetIndex));
                    return true;
                }
                Game1.showRedMessage("Can't be planted here.");
                return false;
            }
            else
            {

                //Game1.showRedMessage("STEP 1");

                if (cObj.Category == -74)
                {
                    return true;
                }
                if (!cObj.performDropDownAction(who))
                {
                    CoreObject @object = (CoreObject)cObj.getOne();
                    @object.shakeTimer = 50;
                    @object.TileLocation = vector;
                    @object.performDropDownAction(who);
                    if (location.objects.ContainsKey(vector))
                    {
                        if (location.objects[vector].ParentSheetIndex != cObj.ParentSheetIndex)
                        {
                            Game1.createItemDebris(location.objects[vector], vector * (float)Game1.tileSize, Game1.random.Next(4));
                            location.objects[vector] = @object;
                        }
                    }

                    else
                    {
                        //   Game1.showRedMessage("STEP 2");
                        //ModCore.ModMonitor.Log(vector.ToString());

                        Vector2 newVec = new Vector2(vector.X, vector.Y);
                        // cObj.boundingBox.Inflate(32, 32);
                        location.objects.Add(newVec, cObj);
                    }
                    @object.initializeLightSource(vector);
                }
                if (playSound == true) Game1.playSound("woodyStep");
                else
                {
                    ModCore.ModMonitor.Log("restoring item from file");
                }
                //Log.AsyncM("Placed and object");
                cObj.locationsName = location.Name;
                s.trackedObjectList.Add(cObj);
                return true;

            }
        }
        

        
        public static bool addItemToInventoryAndCleanTrackedList(CoreObject I,Serialization.SerializationManager s)
        {
            if (Game1.player.isInventoryFull() == false)
            {
                Game1.player.addItemToInventoryBool(I, false);
                s.trackedObjectList.Remove(I);
                return true;
            }
            else
            {
                Random random = new Random(129);
                int i = random.Next();
                i = i % 4;
                Vector2 v2 = new Vector2(Game1.player.getTileX() * Game1.tileSize, Game1.player.getTileY() * Game1.tileSize);
                Game1.createItemDebris(I, v2, i);
                return false;
            }
        }
        

        public static Microsoft.Xna.Framework.Rectangle parseRectFromJson(string s)
        {



            s = s.Replace('{', ' ');
            s = s.Replace('}', ' ');
            s = s.Replace('^', ' ');
            s = s.Replace(':', ' ');
            string[] parsed = s.Split(' ');
            foreach (var v in parsed)
            {
                //Log.AsyncY(v);
            }
            return new Microsoft.Xna.Framework.Rectangle(Convert.ToInt32(parsed[2]), Convert.ToInt32(parsed[4]), Convert.ToInt32(parsed[6]), Convert.ToInt32(parsed[8]));
        }

        /// <summary>
        /// Add an object to a list fo items.
        /// </summary>
        /// <param name="inventory"></param>
        /// <param name="I"></param>
        /// <returns></returns>
        public static bool addItemToOtherInventory(List<Item> inventory, Item I)
        {
            if (I == null) return false;
            if (isInventoryFull(inventory) == false)
            {
                if (inventory == null)
                {
                    return false;
                }
                if (inventory.Count == 0)
                {
                    inventory.Add(I);
                    return true;
                }
                for (int i = 0; i < inventory.Capacity; i++)
                {
                    //   Log.AsyncC("OK????");

                    foreach (var v in inventory)
                    {

                        if (inventory.Count == 0)
                        {
                            addItemToOtherInventory(inventory, I);
                            return true;
                        }
                        if (v == null) continue;
                        if (v.canStackWith(I))
                        {
                            v.addToStack(I.getStack());
                            return true;
                        }
                    }
                }

                inventory.Add(I);
                return true;
            }
            else
            {
                return false;
            }
        }


        /// <summary>
        /// Add an object to a netList of items.
        /// </summary>
        /// <param name="inventory"></param>
        /// <param name="I"></param>
        /// <returns></returns>
        public static bool addItemToOtherInventory(NetObjectList<Item> inventory, Item I)
        {
            if (I == null) return false;
            if (isInventoryFull(inventory) == false)
            {
                if (inventory == null)
                {
                    return false;
                }
                if (inventory.Count == 0)
                {
                    inventory.Add(I);
                    return true;
                }
                for (int i = 0; i < inventory.Capacity; i++)
                {
                    //   Log.AsyncC("OK????");

                    foreach (var v in inventory)
                    {

                        if (inventory.Count == 0)
                        {
                            addItemToOtherInventory(inventory, I);
                            return true;
                        }
                        if (v == null) continue;
                        if (v.canStackWith(I))
                        {
                            v.addToStack(I.getStack());
                            return true;
                        }
                    }
                }

                inventory.Add(I);
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Checks whether or not the inventory list is full of items.
        /// </summary>
        /// <param name="inventory"></param>
        /// <param name="logInfo"></param>
        /// <returns></returns>
        public static bool isInventoryFull(List<Item> inventory, bool logInfo = false)
        {
            if (logInfo)
            {
                ModCore.ModMonitor.Log("size " + inventory.Count);
                ModCore.ModMonitor.Log("max " + inventory.Capacity);
            }

            if (inventory.Count == inventory.Capacity) return true;
            else return false;
        }


        /// <summary>
        /// Checks whether or not the net inventory list is full of items.
        /// </summary>
        /// <param name="inventory"></param>
        /// <param name="logInfo"></param>
        /// <returns></returns>
        public static bool isInventoryFull(NetObjectList<Item> inventory, bool logInfo = false)
        {
            if (logInfo)
            {
                ModCore.ModMonitor.Log("size " + inventory.Count);
                ModCore.ModMonitor.Log("max " + inventory.Capacity);
            }

            if (inventory.Count == inventory.Capacity) return true;
            else return false;
        }


        public static bool isWithinRange(int tileLength,Vector2 positionToCheck)
        {
            Vector2 v = Game1.player.getTileLocation();
            if (v.X < positionToCheck.X - tileLength || v.X > positionToCheck.X + tileLength) return false;
            if (v.Y < positionToCheck.Y - tileLength || v.Y > positionToCheck.Y + tileLength) return false;

            return true;
        }

        public static bool isWithinDirectionRange(int direction,int range, Vector2 positionToCheck)
        {
            Vector2 v = Game1.player.getTileLocation();
            if (direction==3 && (v.X >= positionToCheck.X - range)) return true; //face left
            if (direction==1 && (v.X <= positionToCheck.X + range)) return true; //face right
            if (direction==0 && (v.Y <= positionToCheck.Y + range)) return true; //face up
            if (direction==2 && (v.Y >= positionToCheck.Y - range)) return true; //face down

            return true;
        }



        /// <summary>
        /// Draws the green mouse cursor plus sign.
        /// </summary>
        public static void drawGreenPlus()
        {
            try
            {
                Game1.spriteBatch.Draw(Game1.mouseCursors, new Vector2(Game1.getMouseX() + 34, Game1.getMouseY() + 34), new Microsoft.Xna.Framework.Rectangle(0, 410, 17, 17), Color.White, 0, new Vector2(0, 0), 2f, Microsoft.Xna.Framework.Graphics.SpriteEffects.None, 0);
            }
            catch(Exception e)
            {
                e.ToString();
            }
        }


        public static StardewValley.Object checkRadiusForObject(int radius, string name)
        {
            for (int x = -radius; x <= radius; x++)
            {
                for (int y = -radius; y <= radius; y++)
                {
                    bool f = Game1.player.currentLocation.isObjectAt((Game1.player.getTileX() + x) * Game1.tileSize, (Game1.player.getTileY() + y) * Game1.tileSize);
                    if (f == false) continue;
                    StardewValley.Object obj = Game1.player.currentLocation.getObjectAt((Game1.player.getTileX() + x) * Game1.tileSize, (Game1.player.getTileY() + y) * Game1.tileSize);
                    if (obj == null) continue;
                    if (obj.name == name)
                    {
                        return obj;
                    }
                }
            }
            return null;
        }

        public static StardewValley.Object checkCardinalForObject(string name)
        {
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    if (x == -1 && y == -1) continue; //upper left
                    if (x == -1 && y == 1) continue; //bottom left
                    if (x == 1 && y == -1) continue; //upper right
                    if (x == 1 && y == 1) continue; //bottom right
                    bool f = Game1.player.currentLocation.isObjectAt((Game1.player.getTileX() + x) * Game1.tileSize, (Game1.player.getTileY() + y) * Game1.tileSize);
                    if (f == false) continue;
                    StardewValley.Object obj = Game1.player.currentLocation.getObjectAt((Game1.player.getTileX() + x) * Game1.tileSize, (Game1.player.getTileY() + y) * Game1.tileSize);
                    if (obj == null) continue;
                    if (obj.name == name)
                    {
                        return obj;
                    }
                }
            }
            return null;
        }

        public static void faceDirectionTowardsSomething(Vector2 tileLocation)
        {

            if (tileLocation.X < Game1.player.getTileX())
            {
                Game1.player.faceDirection(3);
            }
            else if (tileLocation.X > Game1.player.getTileX())
            {
                Game1.player.faceDirection(1);
            }
            else if (tileLocation.Y < Game1.player.getTileY())
            {
                Game1.player.faceDirection(0);
            }
            else if (tileLocation.Y > Game1.player.getTileY())
            {
                Game1.player.faceDirection(2);
            }
        }

        /// <summary>
        /// Checks if a game location contains an object with the exact name passed in.
        /// </summary>
        /// <param name="location">The location to check.</param>
        /// <param name="name">The name of the object to check.</param>
        /// <returns></returns>
        public static bool doesLocationContainObject(GameLocation location, string name)
        {
            foreach (KeyValuePair<Vector2, StardewValley.Object> v in location.objects.Pairs)
            {
                if (name == v.Value.name) return true;
            }
            return false;
        }


        public static KeyValuePair<Vector2,TerrainFeature> checkRadiusForTerrainFeature(int radius, Type terrainType)
        {
            for (int x = -radius; x <= radius; x++)
            {
                for (int y = -radius; y <= radius; y++)
                {
                    Vector2 pos = new Vector2((Game1.player.getTileX() + x), (Game1.player.getTileY() + y));
                    bool f = Game1.player.currentLocation.isTerrainFeatureAt((int)pos.X,(int)pos.Y);
                    if (f == false) continue;
                    TerrainFeature t = Game1.player.currentLocation.terrainFeatures[pos];  //((Game1.player.getTileX() + x) * Game1.tileSize, (Game1.player.getTileY() + y) * Game1.tileSize);
                    if (t == null) continue;
                    if (t.GetType() == terrainType)
                    {
                        return new KeyValuePair<Vector2, TerrainFeature> (pos,t);
                    }
                }
            }
            return new KeyValuePair<Vector2, TerrainFeature>(new Vector2(),null);
        }

        public static KeyValuePair<Vector2, TerrainFeature> checkCardinalForTerrainFeature(Type terrainType)
        {
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    if (x == -1 && y == -1) continue; //upper left
                    if (x == -1 && y == 1) continue; //bottom left
                    if (x == 1 && y == -1) continue; //upper right
                    if (x == 1 && y == 1) continue; //bottom right
                    Vector2 pos = new Vector2((Game1.player.getTileX() + x), (Game1.player.getTileY() + y));
                    bool f = Game1.player.currentLocation.isTerrainFeatureAt((int)pos.X, (int)pos.Y);
                    if (f == false) continue;
                    TerrainFeature t = Game1.player.currentLocation.terrainFeatures[pos];  //((Game1.player.getTileX() + x) * Game1.tileSize, (Game1.player.getTileY() + y) * Game1.tileSize);
                    if (t == null) continue;
                    if (t.GetType() == terrainType)
                    {
                        return new KeyValuePair<Vector2, TerrainFeature>(pos, t);
                    }
                }
            }
            return new KeyValuePair<Vector2, TerrainFeature>(new Vector2(), null);
        }


        /// <summary>
        /// Checks if the game location has this terrain feature.
        /// </summary>
        /// <param name="location">The game location to check.</param>
        /// <param name="terrain">The terrain feature type to check if it exists at said location.</param>
        /// <returns></returns>
        public static bool doesLocationContainTerrainFeature(GameLocation location, Type terrain)
        {
            foreach (KeyValuePair<Vector2, StardewValley.TerrainFeatures.TerrainFeature> v in location.terrainFeatures.Pairs)
            {
                if (terrain == v.Value.GetType()) return true;
            }
            return false;
        }

        /// <summary>
        /// Get an item from the player's inventory.
        /// </summary>
        /// <param name="index">The index in the player's inventory of the item.</param>
        /// <returns></returns>
        public static Item getItemFromInventory(int index)
        {
            foreach(var v in Game1.player.Items)
            {
                if (v.ParentSheetIndex == index) return v;
            }
            return null;
        }

        /// <summary>
        /// Get an item from the player's inventory.
        /// </summary>
        /// <param name="name">The name of the item in the player's inventory</param>
        /// <returns></returns>
        public static Item getItemFromInventory(string name)
        {
            foreach (var v in Game1.player.Items)
            {
                if (v.Name == name) return v;
            }
            return null;
        }
    }
}
