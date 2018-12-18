using Microsoft.Xna.Framework;
using Revitalize.Objects;
using Revitalize.Persistence;
using Revitalize.Resources;
using Revitalize.Resources.DataNodes;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xTile;
using xTile.Dimensions;

namespace Revitalize
{
    class Util
    {
        public static bool hasWateredAllCropsToday;

        public static void ResetAllDailyBooleans(object sender, EventArgs e)
        {
            SetUp.createDirectories();
            hasWateredAllCropsToday = false;
            if (Lists.trackedTerrainFeatures != null)
            {
             if(Class1.hasLoadedTerrainList== true)   Serialize.serializeTrackedTerrainDataNodeList(Lists.trackedTerrainFeatures);
            }

            Util.WaterAllCropsInAllLocations();
        }

        public static Microsoft.Xna.Framework.Rectangle parseRectFromJson(string s){

           

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
                for (int i=0; i<inventory.Capacity;i++)
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
        public static bool isInventoryFull(List<Item> inventory, bool logInfo=false)
        {
            if (logInfo)
            {
                //Log.AsyncG("size " + inventory.Count);
                //Log.AsyncG("max " + inventory.Capacity);
            }

            if (inventory.Count == inventory.Capacity) return true;
            else return false;
        }



        public static bool addItemToInventoryElseDrop(Item I)
        {
            if (I == null) return false;
            if (Game1.player.isInventoryFull() == false)
            {
                Game1.player.addItemToInventoryBool(I, false);
                return true;
            }
            else {
                Random random = new Random(129);
                int i = random.Next();
                i = i % 4;
                Vector2 v2 = new Vector2(Game1.player.getTileX() * Game1.tileSize, Game1.player.getTileY() * Game1.tileSize);
                Game1.createItemDebris(I, v2, i);
                return false;
            }
        }

        public static bool addItemToInventorySilently(Item I)
        {
            if (I == null) return false;
            if (Game1.player.isInventoryFull() == false)
            {
                Game1.player.addItemToInventory(I);
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



        public static bool addItemToInventoryAndCleanTrackedList(CoreObject I)
        {
            if (Game1.player.isInventoryFull() == false)
            {
                Game1.player.addItemToInventoryBool(I, false);
                Lists.trackedObjectList.Remove(I);
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


        public static bool addItemToInventoryElseUseMenu (List<Item> I)
        {
          
            Game1.player.addItemsByMenuIfNecessary(I);
            return true;
        }

        public static Color invertColor(Color c,int Alpha=255)
        {
            
            int r;
            int g;
            int b;
            int a=Alpha;

            r = 255-c.R;
            g = 255-c.G;
            b = 255 - c.B;
            // a = 255 - c.A;
            if (a == 0)
            {
                return new Color(0, 0, 0, 0);
            }
            return new Color(r, g, b, a);

        }


        public static bool placementAction(CoreObject cObj, GameLocation location, int x, int y, StardewValley.Farmer who = null, bool playSound=true)
        {
            Vector2 vector = new Vector2((float)(x / Game1.tileSize), (float)(y / Game1.tileSize));
            //  cObj.health = 10;
            if (who != null)
            {
                cObj.owner = who.uniqueMultiplayerID;
            }
            else
            {
                cObj.owner = Game1.player.uniqueMultiplayerID;
            }

            if (!cObj.bigCraftable && !(cObj is Furniture))
            {
                int num = cObj.ParentSheetIndex;
                if (num <= 298)
                {
                    if (num > 94)
                    {
                        bool result;
                        switch (num)
                        {
                            case 286:
                                {
                                    using (List<TemporaryAnimatedSprite>.Enumerator enumerator = Game1.currentLocation.temporarySprites.GetEnumerator())
                                    {
                                        while (enumerator.MoveNext())
                                        {
                                            if (enumerator.Current.position.Equals(vector * (float)Game1.tileSize))
                                            {
                                                result = false;
                                                return result;
                                            }
                                        }
                                    }
                                    int num2 = Game1.random.Next();
                                    Game1.playSound("thudStep");
                                    Game1.currentLocation.TemporarySprites.Add(new TemporaryAnimatedSprite(cObj.parentSheetIndex, 100f, 1, 24, vector * (float)Game1.tileSize, true, false, Game1.currentLocation, who)
                                    {
                                        shakeIntensity = 0.5f,
                                        shakeIntensityChange = 0.002f,
                                        extraInfoForEndBehavior = num2,
                                        endFunction = new TemporaryAnimatedSprite.endBehavior(Game1.currentLocation.removeTemporarySpritesWithID)
                                    });
                                    Game1.currentLocation.TemporarySprites.Add(new TemporaryAnimatedSprite(Game1.mouseCursors, new Microsoft.Xna.Framework.Rectangle(598, 1279, 3, 4), 53f, 5, 9, vector * (float)Game1.tileSize + new Vector2(5f, 3f) * (float)Game1.pixelZoom, true, false, (float)(y + 7) / 10000f, 0f, Color.Yellow, (float)Game1.pixelZoom, 0f, 0f, 0f, false)
                                    {
                                        id = (float)num2
                                    });
                                    Game1.currentLocation.TemporarySprites.Add(new TemporaryAnimatedSprite(Game1.mouseCursors, new Microsoft.Xna.Framework.Rectangle(598, 1279, 3, 4), 53f, 5, 9, vector * (float)Game1.tileSize + new Vector2(5f, 3f) * (float)Game1.pixelZoom, true, true, (float)(y + 7) / 10000f, 0f, Color.Orange, (float)Game1.pixelZoom, 0f, 0f, 0f, false)
                                    {
                                        delayBeforeAnimationStart = 100,
                                        id = (float)num2
                                    });
                                    Game1.currentLocation.TemporarySprites.Add(new TemporaryAnimatedSprite(Game1.mouseCursors, new Microsoft.Xna.Framework.Rectangle(598, 1279, 3, 4), 53f, 5, 9, vector * (float)Game1.tileSize + new Vector2(5f, 3f) * (float)Game1.pixelZoom, true, false, (float)(y + 7) / 10000f, 0f, Color.White, (float)Game1.pixelZoom * 0.75f, 0f, 0f, 0f, false)
                                    {
                                        delayBeforeAnimationStart = 200,
                                        id = (float)num2
                                    });
                                    if (Game1.fuseSound != null && !Game1.fuseSound.IsPlaying)
                                    {
                                        Game1.fuseSound = Game1.soundBank.GetCue("fuse");
                                        Game1.fuseSound.Play();
                                    }
                                    return true;
                                }
                            case 287:
                                {
                                    using (List<TemporaryAnimatedSprite>.Enumerator enumerator = Game1.currentLocation.temporarySprites.GetEnumerator())
                                    {
                                        while (enumerator.MoveNext())
                                        {
                                            if (enumerator.Current.position.Equals(vector * (float)Game1.tileSize))
                                            {
                                                result = false;
                                                return result;
                                            }
                                        }
                                    }
                                    int num2 = Game1.random.Next();
                                    Game1.playSound("thudStep");
                                    Game1.currentLocation.TemporarySprites.Add(new TemporaryAnimatedSprite(cObj.parentSheetIndex, 100f, 1, 24, vector * (float)Game1.tileSize, true, false, Game1.currentLocation, who)
                                    {
                                        shakeIntensity = 0.5f,
                                        shakeIntensityChange = 0.002f,
                                        extraInfoForEndBehavior = num2,
                                        endFunction = new TemporaryAnimatedSprite.endBehavior(Game1.currentLocation.removeTemporarySpritesWithID)
                                    });
                                    Game1.currentLocation.TemporarySprites.Add(new TemporaryAnimatedSprite(Game1.mouseCursors, new Microsoft.Xna.Framework.Rectangle(598, 1279, 3, 4), 53f, 5, 9, vector * (float)Game1.tileSize, true, false, (float)(y + 7) / 10000f, 0f, Color.Yellow, (float)Game1.pixelZoom, 0f, 0f, 0f, false)
                                    {
                                        id = (float)num2
                                    });
                                    Game1.currentLocation.TemporarySprites.Add(new TemporaryAnimatedSprite(Game1.mouseCursors, new Microsoft.Xna.Framework.Rectangle(598, 1279, 3, 4), 53f, 5, 9, vector * (float)Game1.tileSize, true, false, (float)(y + 7) / 10000f, 0f, Color.Orange, (float)Game1.pixelZoom, 0f, 0f, 0f, false)
                                    {
                                        delayBeforeAnimationStart = 100,
                                        id = (float)num2
                                    });
                                    Game1.currentLocation.TemporarySprites.Add(new TemporaryAnimatedSprite(Game1.mouseCursors, new Microsoft.Xna.Framework.Rectangle(598, 1279, 3, 4), 53f, 5, 9, vector * (float)Game1.tileSize, true, false, (float)(y + 7) / 10000f, 0f, Color.White, (float)Game1.pixelZoom * 0.75f, 0f, 0f, 0f, false)
                                    {
                                        delayBeforeAnimationStart = 200,
                                        id = (float)num2
                                    });
                                    if (Game1.fuseSound != null && !Game1.fuseSound.IsPlaying)
                                    {
                                        Game1.fuseSound = Game1.soundBank.GetCue("fuse");
                                        Game1.fuseSound.Play();
                                    }
                                    return true;
                                }
                            case 288:
                                {
                                    using (List<TemporaryAnimatedSprite>.Enumerator enumerator = Game1.currentLocation.temporarySprites.GetEnumerator())
                                    {
                                        while (enumerator.MoveNext())
                                        {
                                            if (enumerator.Current.position.Equals(vector * (float)Game1.tileSize))
                                            {
                                                result = false;
                                                return result;
                                            }
                                        }
                                    }
                                    int num2 = Game1.random.Next();
                                    Game1.playSound("thudStep");
                                    Game1.currentLocation.TemporarySprites.Add(new TemporaryAnimatedSprite(cObj.parentSheetIndex, 100f, 1, 24, vector * (float)Game1.tileSize, true, false, Game1.currentLocation, who)
                                    {
                                        shakeIntensity = 0.5f,
                                        shakeIntensityChange = 0.002f,
                                        extraInfoForEndBehavior = num2,
                                        endFunction = new TemporaryAnimatedSprite.endBehavior(Game1.currentLocation.removeTemporarySpritesWithID)
                                    });
                                    Game1.currentLocation.TemporarySprites.Add(new TemporaryAnimatedSprite(Game1.mouseCursors, new Microsoft.Xna.Framework.Rectangle(598, 1279, 3, 4), 53f, 5, 9, vector * (float)Game1.tileSize + new Vector2(5f, 0f) * (float)Game1.pixelZoom, true, false, (float)(y + 7) / 10000f, 0f, Color.Yellow, (float)Game1.pixelZoom, 0f, 0f, 0f, false)
                                    {
                                        id = (float)num2
                                    });
                                    Game1.currentLocation.TemporarySprites.Add(new TemporaryAnimatedSprite(Game1.mouseCursors, new Microsoft.Xna.Framework.Rectangle(598, 1279, 3, 4), 53f, 5, 9, vector * (float)Game1.tileSize + new Vector2(5f, 0f) * (float)Game1.pixelZoom, true, true, (float)(y + 7) / 10000f, 0f, Color.Orange, (float)Game1.pixelZoom, 0f, 0f, 0f, false)
                                    {
                                        delayBeforeAnimationStart = 100,
                                        id = (float)num2
                                    });
                                    Game1.currentLocation.TemporarySprites.Add(new TemporaryAnimatedSprite(Game1.mouseCursors, new Microsoft.Xna.Framework.Rectangle(598, 1279, 3, 4), 53f, 5, 9, vector * (float)Game1.tileSize + new Vector2(5f, 0f) * (float)Game1.pixelZoom, true, false, (float)(y + 7) / 10000f, 0f, Color.White, (float)Game1.pixelZoom * 0.75f, 0f, 0f, 0f, false)
                                    {
                                        delayBeforeAnimationStart = 200,
                                        id = (float)num2
                                    });
                                    if (Game1.fuseSound != null && !Game1.fuseSound.IsPlaying)
                                    {
                                        Game1.fuseSound = Game1.soundBank.GetCue("fuse");
                                        Game1.fuseSound.Play();
                                    }
                                    return true;
                                }
                            default:
                                if (num != 297)
                                {
                                    if (num != 298)
                                    {
                                        goto IL_FD7;
                                    }
                                    if (location.objects.ContainsKey(vector))
                                    {
                                        return false;
                                    }
                                    location.objects.Add(vector, new Fence(vector, 5, false));
                                    Game1.playSound("axe");
                                    return true;
                                }
                                else
                                {
                                    if (location.objects.ContainsKey(vector) || location.terrainFeatures.ContainsKey(vector))
                                    {
                                        return false;
                                    }
                                    location.terrainFeatures.Add(vector, new Grass(1, 4));
                                    Game1.playSound("dirtyHit");
                                    return true;
                                }
                                break;
                        }
                        return result;
                    }
                    if (num != 93)
                    {
                        if (num == 94)
                        {
                            if (location.objects.ContainsKey(vector))
                            {
                                return false;
                            }
                            new Torch(vector, 1, 94).placementAction(location, x, y, who);
                            return true;
                        }
                    }
                    else
                    {
                        if (location.objects.ContainsKey(vector))
                        {
                            return false;
                        }
                        Utility.removeLightSource((int)(cObj.tileLocation.X * 2000f + cObj.tileLocation.Y));
                        Utility.removeLightSource((int)Game1.player.uniqueMultiplayerID);
                        new Torch(vector, 1).placementAction(location, x, y, (who == null) ? Game1.player : who);
                        return true;
                    }
                }
                else if (num <= 401)
                {
                    switch (num)
                    {
                        case 309:
                        case 310:
                        case 311:
                            {
                                bool flag = location.terrainFeatures.ContainsKey(vector) && location.terrainFeatures[vector] is HoeDirt && (location.terrainFeatures[vector] as HoeDirt).crop == null;
                                if (!flag && (location.objects.ContainsKey(vector) || location.terrainFeatures.ContainsKey(vector) || (!(location is Farm) && !location.name.Contains("Greenhouse"))))
                                {
                                    Game1.showRedMessage("Invalid Position");
                                    return false;
                                }
                                string text = location.doesTileHaveProperty(x, y, "NoSpawn", "Back");
                                if ((text == null || (!text.Equals("Tree") && !text.Equals("All"))) && (flag || (location.isTileLocationOpen(new Location(x * Game1.tileSize, y * Game1.tileSize)) && !location.isTileOccupied(new Vector2((float)x, (float)y), "") && location.doesTileHaveProperty(x, y, "Water", "Back") == null)))
                                {
                                    int which = 1;
                                    num = cObj.parentSheetIndex;
                                    if (num != 310)
                                    {
                                        if (num == 311)
                                        {
                                            which = 3;
                                        }
                                    }
                                    else
                                    {
                                        which = 2;
                                    }
                                    location.terrainFeatures.Remove(vector);
                                    location.terrainFeatures.Add(vector, new Tree(which, 0));
                                    Game1.playSound("dirtyHit");
                                    return true;
                                }
                                break;
                            }
                        default:
                            switch (num)
                            {
                                case 322:
                                    if (location.objects.ContainsKey(vector))
                                    {
                                        return false;
                                    }
                                    location.objects.Add(vector, new Fence(vector, 1, false));
                                    Game1.playSound("axe");
                                    return true;
                                case 323:
                                    if (location.objects.ContainsKey(vector))
                                    {
                                        return false;
                                    }
                                    location.objects.Add(vector, new Fence(vector, 2, false));
                                    Game1.playSound("stoneStep");
                                    return true;
                                case 324:
                                    if (location.objects.ContainsKey(vector))
                                    {
                                        return false;
                                    }
                                    location.objects.Add(vector, new Fence(vector, 3, false));
                                    Game1.playSound("hammer");
                                    return true;
                                case 325:
                                    if (location.objects.ContainsKey(vector))
                                    {
                                        return false;
                                    }
                                    location.objects.Add(vector, new Fence(vector, 4, true));
                                    Game1.playSound("axe");
                                    return true;
                                case 326:
                                case 327:
                                case 330:
                                case 332:
                                    break;
                                case 328:
                                    if (location.terrainFeatures.ContainsKey(vector))
                                    {
                                        return false;
                                    }
                                    location.terrainFeatures.Add(vector, new Flooring(0));
                                    Game1.playSound("axchop");
                                    return true;
                                case 329:
                                    if (location.terrainFeatures.ContainsKey(vector))
                                    {
                                        return false;
                                    }
                                    location.terrainFeatures.Add(vector, new Flooring(1));
                                    Game1.playSound("thudStep");
                                    return true;
                                case 331:
                                    if (location.terrainFeatures.ContainsKey(vector))
                                    {
                                        return false;
                                    }
                                    location.terrainFeatures.Add(vector, new Flooring(2));
                                    Game1.playSound("axchop");
                                    return true;
                                case 333:
                                    if (location.terrainFeatures.ContainsKey(vector))
                                    {
                                        return false;
                                    }
                                    location.terrainFeatures.Add(vector, new Flooring(3));
                                    Game1.playSound("thudStep");
                                    return true;
                                default:
                                    if (num == 401)
                                    {
                                        if (location.terrainFeatures.ContainsKey(vector))
                                        {
                                            return false;
                                        }
                                        location.terrainFeatures.Add(vector, new Flooring(4));
                                        Game1.playSound("thudStep");
                                        return true;
                                    }
                                    break;
                            }
                            break;
                    }
                }
                else
                {
                    switch (num)
                    {
                        case 405:
                            if (location.terrainFeatures.ContainsKey(vector))
                            {
                                return false;
                            }
                            location.terrainFeatures.Add(vector, new Flooring(6));
                            Game1.playSound("woodyStep");
                            return true;
                        case 406:
                        case 408:
                        case 410:
                            break;
                        case 407:
                            if (location.terrainFeatures.ContainsKey(vector))
                            {
                                return false;
                            }
                            location.terrainFeatures.Add(vector, new Flooring(5));
                            Game1.playSound("dirtyHit");
                            return true;
                        case 409:
                            if (location.terrainFeatures.ContainsKey(vector))
                            {
                                return false;
                            }
                            location.terrainFeatures.Add(vector, new Flooring(7));
                            Game1.playSound("stoneStep");
                            return true;
                        case 411:
                            if (location.terrainFeatures.ContainsKey(vector))
                            {
                                return false;
                            }
                            location.terrainFeatures.Add(vector, new Flooring(8));
                            Game1.playSound("stoneStep");
                            return true;
                        default:
                            if (num != 415)
                            {
                                if (num == 710)
                                {
                                    if (location.objects.ContainsKey(vector) || location.doesTileHaveProperty((int)vector.X, (int)vector.Y, "Water", "Back") == null)
                                    {
                                        return false;
                                    }
                                    new CrabPot(vector, 1).placementAction(location, x, y, who);
                                    return true;
                                }
                            }
                            else
                            {
                                if (location.terrainFeatures.ContainsKey(vector))
                                {
                                    return false;
                                }
                                location.terrainFeatures.Add(vector, new Flooring(9));
                                Game1.playSound("stoneStep");
                                return true;
                            }
                            break;
                    }
                }
            }
            else
            {
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
                            new Torch(vector, cObj.parentSheetIndex, true)
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
            }
            IL_FD7:
            if (cObj.name.Equals("Tapper"))
            {
                if (location.terrainFeatures.ContainsKey(vector) && location.terrainFeatures[vector] is Tree && (location.terrainFeatures[vector] as Tree).growthStage >= 5 && !(location.terrainFeatures[vector] as Tree).stump && !location.objects.ContainsKey(vector))
                {
                    cObj.tileLocation = vector;
                    location.objects.Add(vector, cObj);
                    int treeType = (location.terrainFeatures[vector] as Tree).treeType;
                    (location.terrainFeatures[vector] as Tree).tapped = true;
                    switch (treeType)
                    {
                        case 1:
                            cObj.heldObject = new StardewValley.Object(725, 1, false, -1, 0);
                            cObj.minutesUntilReady = 13000 - Game1.timeOfDay;
                            break;
                        case 2:
                            cObj.heldObject = new StardewValley.Object(724, 1, false, -1, 0);
                            cObj.minutesUntilReady = 16000 - Game1.timeOfDay;
                            break;
                        case 3:
                            cObj.heldObject = new StardewValley.Object(726, 1, false, -1, 0);
                            cObj.minutesUntilReady = 10000 - Game1.timeOfDay;
                            break;
                        case 7:
                            cObj.heldObject = new StardewValley.Object(420, 1, false, -1, 0);
                            cObj.minutesUntilReady = 3000 - Game1.timeOfDay;
                            if (!Game1.currentSeason.Equals("fall"))
                            {
                                cObj.heldObject = new StardewValley.Object(404, 1, false, -1, 0);
                                cObj.minutesUntilReady = 6000 - Game1.timeOfDay;
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
                    location.terrainFeatures.Add(vector, new FruitTree(cObj.parentSheetIndex));
                    return true;
                }
                Game1.showRedMessage("Can't be planted here.");
                return false;
            }
            else
            {

                //Game1.showRedMessage("STEP 1");

                if (cObj.category == -74)
                {
                    return true;
                }
                if (!cObj.performDropDownAction(who))
                {
                    CoreObject @object = (CoreObject)cObj.getOne();
                    @object.shakeTimer = 50;
                    @object.tileLocation = vector;
                    @object.performDropDownAction(who);
                    if (location.objects.ContainsKey(vector))
                    {
                        if (location.objects[vector].ParentSheetIndex != cObj.parentSheetIndex)
                        {
                            Game1.createItemDebris(location.objects[vector], vector * (float)Game1.tileSize, Game1.random.Next(4));
                            location.objects[vector] = @object;
                        }
                    }

                    else
                    {
                     //   Game1.showRedMessage("STEP 2");
                        //Log.Info(vector);

                        Vector2 newVec = new Vector2(vector.X, vector.Y);
                        // cObj.boundingBox.Inflate(32, 32);
                        location.objects.Add(newVec, cObj);
                    }
                    @object.initializeLightSource(vector);
                }
              if(playSound==true)  Game1.playSound("woodyStep");
              else
                {
                    //Log.AsyncG("restoring item from file");
                }
                //Log.AsyncM("Placed and object");
                cObj.locationsName = location.name;
                Lists.trackedObjectList.Add(cObj);
                return true;

            }
        }



        public static bool canBePlacedHere(CoreObject cObj,GameLocation l, Vector2 tile)
        {
            return (cObj.parentSheetIndex == 710 && l.doesTileHaveProperty((int)tile.X, (int)tile.Y, "Water", "Back") != null && !l.objects.ContainsKey(tile) && l.doesTileHaveProperty((int)tile.X + 1, (int)tile.Y, "Water", "Back") != null && l.doesTileHaveProperty((int)tile.X - 1, (int)tile.Y, "Water", "Back") != null) || (l.doesTileHaveProperty((int)tile.X, (int)tile.Y + 1, "Water", "Back") != null && l.doesTileHaveProperty((int)tile.X, (int)tile.Y - 1, "Water", "Back") != null) || (cObj.parentSheetIndex == 105  && l.terrainFeatures.ContainsKey(tile) && l.terrainFeatures[tile] is Tree && !l.objects.ContainsKey(tile)) || (cObj.name != null && cObj.name.Contains("Bomb") && (!l.isTileOccupiedForPlacement(tile, cObj) || l.isTileOccupiedByFarmer(tile) != null)) || !l.isTileOccupiedForPlacement(tile, cObj);
        }


        public static void plantCropHere()
        {
          //Log.AsyncY(Game1.player.ActiveObject.name);
            if (Lists.saplingNames.Contains(Game1.player.ActiveObject.name))
            {
                //Log.AsyncY("PLANT THE SAPLING");
                bool f = plantSappling();

                if (f == true) return;
            }
            HoeDirt t;
            TerrainFeature r;
            bool plant = Game1.player.currentLocation.terrainFeatures.TryGetValue(Game1.currentCursorTile, out r);
            t = (r as HoeDirt);
            if (t is HoeDirt)
            {
                if ((t as HoeDirt).crop == null)
                {
                    (t as HoeDirt).crop = new Crop(Game1.player.ActiveObject.parentSheetIndex, (int)Game1.currentCursorTile.X, (int)Game1.currentCursorTile.Y);
                    Game1.player.reduceActiveItemByOne();
                    Game1.playSound("dirtyHit");
                    Revitalize.Resources.Lists.trackedTerrainFeatures.Add(new Resources.DataNodes.TrackedTerrainDataNode(Game1.player.currentLocation,t,new Vector2((int)Game1.currentCursorTile.X,(int)Game1.currentCursorTile.Y)));
                }
            }
        }


        public static void plantExtraCropHere()
        {
            if (Lists.saplingNames.Contains(Game1.player.ActiveObject.name))
            {
                bool f = plantSappling();
                if (f == true) return;
            }

            //Log.AsyncC("HELLO");
            HoeDirt t;
            TerrainFeature r;
            bool plant = Game1.player.currentLocation.terrainFeatures.TryGetValue(Game1.currentCursorTile, out r);
            t = (r as HoeDirt);
            if (t is HoeDirt)
            {
                if ((t as HoeDirt).crop == null)
                {
                //    Log.AsyncG("BOOP");
                    SeedDataNode f;
                    bool g =Dictionaries.seedList.TryGetValue(((ExtraSeeds)Game1.player.ActiveObject).name, out f);

                    (t as HoeDirt).crop = new Crop(f.cropIndex, (int)Game1.currentCursorTile.X, (int)Game1.currentCursorTile.Y);
                    Game1.player.reduceActiveItemByOne();
                    Game1.playSound("dirtyHit");
                    Revitalize.Resources.Lists.trackedTerrainFeatures.Add(new Resources.DataNodes.TrackedTerrainDataNode(Game1.player.currentLocation, t, new Vector2((int)Game1.currentCursorTile.X, (int)Game1.currentCursorTile.Y)));
                }
            }
        }


        public static bool plantSappling()
        {

            if (Lists.saplingNames.Contains(Game1.player.ActiveObject.name))
            {
                Vector2 vector = Game1.currentCursorTile;
                GameLocation location = Game1.player.currentLocation;
                Vector2 key = default(Vector2);
                for (int i = (int)vector.X / Game1.tileSize - 2; i <= vector.X / Game1.tileSize + 2; i++)
                {
                    for (int j = (int)vector.Y / Game1.tileSize - 2; j <= vector.Y / Game1.tileSize + 2; j++)
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
                      //  Log.AsyncC("UMMM BUT BUT BUT");
                        return false;
                    }
                    location.terrainFeatures.Remove(vector);
                }
                if ( (location.doesTileHaveProperty((int)vector.X, (int)vector.Y, "Diggable", "Back") != null || location.doesTileHavePropertyNoNull((int)vector.X, (int)vector.Y, "Type", "Back").Equals("Grass")))
                {
                    Game1.playSound("dirtyHit");
                    DelayedAction.playSoundAfterDelay("coin", 100);
                    location.terrainFeatures.Add(vector, new FruitTree(Game1.player.ActiveObject.parentSheetIndex));
                    return true;
                }
                else
                {
                    // Game1.playSound("dirtyHit");


                    int which = 1;
                    int num = Game1.player.ActiveObject.parentSheetIndex;
                    if (num != 310)
                    {
                        if (num == 311)
                        {
                            which = 3;
                        }
                    }
                    else
                    {
                        which = 2;
                    }
                    location.terrainFeatures.Remove(vector);
                    location.terrainFeatures.Add(vector, new Tree(which, 0));
                    Game1.player.reduceActiveItemByOne();
                    Game1.playSound("dirtyHit");
                    DelayedAction.playSoundAfterDelay("coin", 100);
                }
                Game1.showRedMessage("Can't be planted here.");
                return false;
            }
            //Log.AsyncR("MAKES NO SENSE");
            return false;
        }


        /// <summary>
        /// Static wrapper;
        /// </summary>
        public static void getGiftPackageContents()
        {
            if (Game1.player.ActiveObject as GiftPackage != null)
            {
                (Game1.player.ActiveObject as GiftPackage).getContents();
            }

        }


        public static void WaterAllCropsInAllLocations()
        {
           
           // Game1.weatherForTomorrow = Game1.weather_rain;
            
            List<Revitalize.Resources.DataNodes.TrackedTerrainDataNode> removalList = new List<Resources.DataNodes.TrackedTerrainDataNode>();
            if (Game1.isRaining)
            {
                // Log.AsyncC("WHY");
                
               
                foreach (var v in Lists.trackedTerrainFeatures)
                {
                    if ((v.terrainFeature as HoeDirt).crop==null)
                    {
                        removalList.Add(v);
                        continue;
                    }
                    if ((v.terrainFeature as HoeDirt).state == 0) (v.terrainFeature as HoeDirt).state = 1;
                    hasWateredAllCropsToday = true;

                }

                foreach (var v in removalList)
                {
                    Lists.trackedTerrainFeatures.Remove(v);
                }
                removalList.Clear();
            }
        }

    public static void loadMapSwapData()
        {
            Map m = new Map();
            Class1.persistentMapSwap = Serialize.parseMapSwapData();
            if (Class1.persistentMapSwap == null)
            {
                //Log.AsyncG("IS NULL");
                Class1.persistentMapSwap = new MapSwapData();
            }
            else
            {
                try
                {
                    //Log.AsyncM(parseOutContent(Class1.persistentMapSwap.mapPath));
                    m = Game1.content.Load<Map>(parseOutContent(Class1.persistentMapSwap.mapPath));
                    //Log.AsyncG("Successfully loaded custom farm map.");
                }
                catch (Exception err)
                {
                    m = null;
                    //Log.AsyncM(err);


                }
                if (m != null)
                {
                    foreach (GameLocation v in Game1.locations)
                    {
                        if (v.name == "Farm")
                        {
                            Vector2 oldLocDimensions = Utilities.MapUtilities.getMapDimensions(v);
                            bool[,] oldWaterTiles = v.waterTiles;

                            v.map = m;//change this to  v.map =(Game1.content.Load<Map>("Path.Combine("Maps,"Farms",folderName,"Farm")));
                            //Log.AsyncG("Sucesfully injected custom farm map");
                            Utilities.MapUtilities.loadCustomFarmMap(v, oldLocDimensions, oldWaterTiles);
                        }
                    }
                }
                else
                {
                    //Log.AsyncM("WTF");
                }
            }
        }

        public static string parseOutContent(string s)
        {
            return s.Remove(0, 8);
        }



    }
}
