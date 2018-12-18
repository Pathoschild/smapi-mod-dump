using AdditionalCropsFramework.Framework;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Characters;
using StardewValley.Locations;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using StardustCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xTile.Dimensions;

namespace AdditionalCropsFramework
{
    class Utilities
    {
        public static readonly string EntensionsFolderName = "Extensions"; 

        public static List<TerrainDataNode> trackedTerrainFeatures= new List<TerrainDataNode>();

        public static List<CoreObject> NonSolidThingsToDraw = new List<CoreObject>();


        public static void createObjectDebris(Item I, int xTileOrigin, int yTileOrigin, int xTileTarget, int yTileTarget, int groundLevel = -1, int itemQuality = 0, float velocityMultiplyer = 1f, GameLocation location = null)
        {
            Debris debris = new Debris(I, new Vector2(xTileOrigin, yTileOrigin), new Vector2(xTileTarget, yTileTarget))
            {
                itemQuality = itemQuality,
            };
       
            /*
            Debris debris = new Debris(objectIndex, new Vector2((float)(xTile * Game1.tileSize + Game1.tileSize / 2), (float)(yTile * Game1.tileSize + Game1.tileSize / 2)), new Vector2((float)Game1.player.getStandingX(), (float)Game1.player.getStandingY()))
            {
                itemQuality = itemQuality
            };
            */
            foreach (Chunk chunk in debris.Chunks)
            {
                double num1 = (double)chunk.xVelocity.Value * (double)velocityMultiplyer;
                chunk.xVelocity.Value = (float)num1;
                double num2 = (double)chunk.yVelocity.Value * (double)velocityMultiplyer;
                chunk.yVelocity.Value = (float)num2;
            }
            if (groundLevel != -1)
                debris.chunkFinalYLevel = groundLevel;
            (location == null ? Game1.currentLocation : location).debris.Add(debris);
        }

        /*
        public static void plantModdedCropHere(ModularSeeds seeds)
        {
            /*
            if (Lists.saplingNames.Contains(Game1.player.ActiveObject.name))
            {
                bool f = plantSappling();
                if (f == true) return;
            }
            
            //Log.AsyncC("HELLO");
            try
            {
                HoeDirt t;
                TerrainFeature r;
                bool plant = Game1.player.currentLocation.terrainFeatures.TryGetValue(Game1.currentCursorTile, out r);
                t = (r as HoeDirt);
                if (t is HoeDirt)
                {
                    if ((t as HoeDirt).crop == null)
                    {
                        //    Log.AsyncG("BOOP");
                       (t as HoeDirt).crop = new ModularCrop(seeds.parentSheetIndex, (int)Game1.currentCursorTile.X, (int)Game1.currentCursorTile.Y, seeds.cropDataFilePath, seeds.cropTextureFilePath, seeds.cropObjectTextureFilePath, seeds.cropObjectDataFilePath);
                        //Game1.player.reduceActiveItemByOne();
                        Game1.playSound("dirtyHit");
                        trackedTerrainFeatures.Add(new TerrainDataNode(Game1.player.currentLocation, (int)Game1.currentCursorTile.X, (int)Game1.currentCursorTile.Y, t));
                    }
                }
            }catch(Exception err)
            {
                Log.AsyncG("BUBBLES");
            }
        }
    
        public static void plantRegularCropHere()
        {
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
                   trackedTerrainFeatures.Add(new TerrainDataNode(Game1.player.currentLocation, (int)Game1.currentCursorTile.X, (int)Game1.currentCursorTile.Y,t));
                }
            }
        }
        */


        public static bool placementAction(CoreObject cObj, GameLocation location, int x, int y, StardewValley.Farmer who = null, bool playSound = true)
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

            if (1 == 2)
            {

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
                       // Log.Info(vector);

                        Vector2 newVec = new Vector2(vector.X, vector.Y);
                        // cObj.boundingBox.Inflate(32, 32);
                        location.objects.Add(newVec, cObj);
                    }
                    @object.initializeLightSource(vector);
                }
                if (playSound == true) Game1.playSound("woodyStep");
                else
                {
                  //  Log.AsyncG("restoring item from file");
                }
                //Log.AsyncM("Placed and object");
                cObj.locationsName = location.Name;
                StardustCore.ModCore.SerializationManager.trackedObjectList.Add(cObj);
                return true;

            }
        }



        public static bool addItemToInventoryAndCleanTrackedList(CoreObject I)
        {
            if (Game1.player.isInventoryFull() == false)
            {
                Game1.player.addItemToInventoryBool(I, false);
                StardustCore.ModCore.SerializationManager.trackedObjectList.Remove(I);
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

       public static bool isCropFullGrown(Crop c)
        {

            if (c.currentPhase.Value >= c.phaseDays.Count - 1)
            {
               c.currentPhase.Value = c.phaseDays.Count - 1;
               c.dayOfCurrentPhase.Value = 0;
                return true;
            }
            return false;
    }

        public static void cropNewDay(PlanterBox p,Crop c,int state, int fertilizer, int xTile, int yTile, GameLocation environment)
        {
            if (p.greenHouseEffect == false)
            {
                if ((c.dead.Value || !c.seasonsToGrowIn.Contains(Game1.currentSeason)))
                {
                    c.dead.Value = true;
                }
            }


                if (state == 1)
                {
                    c.dayOfCurrentPhase.Value++;
                  //  Log.AsyncG("DaY OF CURRRENT PHASE BISCUITS!"+c.dayOfCurrentPhase);

                   // Log.AsyncC(c.currentPhase);
                    if (c.dayOfCurrentPhase.Value >= c.phaseDays[c.currentPhase.Value])
                    {
                        c.currentPhase.Value++;
                        c.dayOfCurrentPhase.Value = 0;
                    }

                    //c.dayOfCurrentPhase = c.fullyGrown ? c.dayOfCurrentPhase - 1 : Math.Min(c.dayOfCurrentPhase + 1, c.phaseDays.Count > 0 ? c.phaseDays[Math.Min(c.phaseDays.Count - 1, c.currentPhase)] : 0);
                    if (c.dayOfCurrentPhase.Value >= (c.phaseDays.Count > 0 ? c.phaseDays[Math.Min(c.phaseDays.Count - 1, c.currentPhase.Value)] : 0) && c.currentPhase.Value < c.phaseDays.Count - 1)
                    {
                        c.currentPhase.Value = c.currentPhase.Value + 1;
                        c.dayOfCurrentPhase.Value = 0;
                    }

                    while (c.currentPhase.Value < c.phaseDays.Count - 1 && c.phaseDays.Count > 0 && c.phaseDays[c.currentPhase.Value] <= 0)
                        c.currentPhase.Value = c.currentPhase.Value + 1;
                    if (c.rowInSpriteSheet.Value == 23 && c.phaseToShow.Value == -1 && c.currentPhase.Value > 0)
                        c.phaseToShow.Value = Game1.random.Next(1, 7);
                    if (c.currentPhase.Value == c.phaseDays.Count - 1 && (c.indexOfHarvest.Value == 276 || c.indexOfHarvest.Value == 190 || c.indexOfHarvest.Value == 254) && new Random((int)Game1.uniqueIDForThisGame + (int)Game1.stats.DaysPlayed + xTile * 2000 + yTile).NextDouble() < 0.01)
                    {
                        for (int index1 = xTile - 1; index1 <= xTile + 1; ++index1)
                        {
                            for (int index2 = yTile - 1; index2 <= yTile + 1; ++index2)
                            {
                                Vector2 key = new Vector2((float)index1, (float)index2);
                                if (!environment.terrainFeatures.ContainsKey(key) || !(environment.terrainFeatures[key] is HoeDirt) || ((environment.terrainFeatures[key] as HoeDirt).crop == null || (environment.terrainFeatures[key] as HoeDirt).crop.indexOfHarvest != c.indexOfHarvest))
                                    return;
                            }
                        }
                        for (int index1 = xTile - 1; index1 <= xTile + 1; ++index1)
                        {
                            for (int index2 = yTile - 1; index2 <= yTile + 1; ++index2)
                            {
                                Vector2 index3 = new Vector2((float)index1, (float)index2);
                                (environment.terrainFeatures[index3] as HoeDirt).crop = (Crop)null;
                            }
                        }
                     // (environment as Farm).resourceClumps.Add((ResourceClump)new GiantCrop(c.indexOfHarvest, new Vector2((float)(xTile - 1), (float)(yTile - 1))));
                    }
                }
                if (c.fullyGrown.Value && c.dayOfCurrentPhase.Value > 0 || (c.currentPhase.Value < c.phaseDays.Count - 1 || c.rowInSpriteSheet.Value != 23))
                    return;
                Vector2 index = new Vector2((float)xTile, (float)yTile);
                environment.objects.Remove(index);
                string season = Game1.currentSeason;
                switch (c.whichForageCrop.Value)
                {
                    case 495:
                        season = "spring";
                        break;
                    case 496:
                        season = "summer";
                        break;
                    case 497:
                        season = "fall";
                        break;
                    case 498:
                        season = "winter";
                        break;
                }
            StardewValley.Object o = new StardewValley.Object(index, c.getRandomWildCropForSeason(season), 1);
            o.IsSpawnedObject = true;
            o.CanBeGrabbed = true;
            environment.objects.Add(index, o);
              
                if (environment.terrainFeatures[index] == null || !(environment.terrainFeatures[index] is HoeDirt))
                    return;
                (environment.terrainFeatures[index] as HoeDirt).crop = (Crop)null;
            
        }


       

        public static void cropNewDayModded(PlanterBox p,ModularCrop c, int state, int fertilizer, int xTile, int yTile, GameLocation environment)
        {
            if (p.greenHouseEffect == false)
            {
                if ((c.dead || !c.seasonsToGrowIn.Contains(Game1.currentSeason)))
                {
                    c.dead = true;
                }
            }

                if (state == 1)
                {
                    c.dayOfCurrentPhase++;



                    //c.dayOfCurrentPhase = c.fullyGrown ? c.dayOfCurrentPhase - 1 : Math.Min(c.dayOfCurrentPhase + 1, c.phaseDays.Count > 0 ? c.phaseDays[Math.Min(c.phaseDays.Count - 1, c.currentPhase)] : 0);
                    if (c.dayOfCurrentPhase >= (c.phaseDays.Count > 0 ? c.phaseDays[Math.Min(c.phaseDays.Count - 1, c.currentPhase)] : 0) && c.currentPhase < c.phaseDays.Count - 1)
                    {
                        c.currentPhase = c.currentPhase + 1;
                        c.dayOfCurrentPhase = 0;
                    }




                    while (c.currentPhase < c.phaseDays.Count - 1 && c.phaseDays.Count > 0 && c.phaseDays[c.currentPhase] <= 0)
                        c.currentPhase = c.currentPhase + 1;
                    if (c.rowInSpriteSheet == 23 && c.phaseToShow == -1 && c.currentPhase > 0)
                        c.phaseToShow = Game1.random.Next(1, 7);
                    if (c.currentPhase == c.phaseDays.Count - 1 && (c.indexOfHarvest == 276 || c.indexOfHarvest == 190 || c.indexOfHarvest == 254) && new Random((int)Game1.uniqueIDForThisGame + (int)Game1.stats.DaysPlayed + xTile * 2000 + yTile).NextDouble() < 0.01)
                    {
                        for (int index1 = xTile - 1; index1 <= xTile + 1; ++index1)
                        {
                            for (int index2 = yTile - 1; index2 <= yTile + 1; ++index2)
                            {
                                Vector2 key = new Vector2((float)index1, (float)index2);
                                if (!environment.terrainFeatures.ContainsKey(key) || !(environment.terrainFeatures[key] is HoeDirt) || ((environment.terrainFeatures[key] as HoeDirt).crop == null || (environment.terrainFeatures[key] as HoeDirt).crop.indexOfHarvest.Value != c.indexOfHarvest))
                                    return;
                            }
                        }
                        for (int index1 = xTile - 1; index1 <= xTile + 1; ++index1)
                        {
                            for (int index2 = yTile - 1; index2 <= yTile + 1; ++index2)
                            {
                                Vector2 index3 = new Vector2((float)index1, (float)index2);
                                (environment.terrainFeatures[index3] as HoeDirt).crop = (Crop)null;
                            }
                        }
                        // (environment as Farm).resourceClumps.Add((ResourceClump)new GiantCrop(c.indexOfHarvest, new Vector2((float)(xTile - 1), (float)(yTile - 1))));
                    }
                }
                if (c.fullyGrown && c.dayOfCurrentPhase > 0 || (c.currentPhase < c.phaseDays.Count - 1 || c.rowInSpriteSheet != 23))
                    return;
                Vector2 index = new Vector2((float)xTile, (float)yTile);
                environment.objects.Remove(index);
                string season = Game1.currentSeason;
                switch (c.whichForageCrop)
                {
                    case 495:
                        season = "spring";
                        break;
                    case 496:
                        season = "summer";
                        break;
                    case 497:
                        season = "fall";
                        break;
                    case 498:
                        season = "winter";
                        break;
                }



                if (environment.terrainFeatures[index] == null || !(environment.terrainFeatures[index] is HoeDirt))
                    return;
                (environment.terrainFeatures[index] as HoeDirt).crop = (Crop)null;
            
        }


        public static bool harvestCrop(Crop c,int xTile, int yTile, int fertilizer, JunimoHarvester junimoHarvester = null)
        {
            

            int amountToHarvest = 1;
            Random r = new Random(xTile + yTile + c.rowInSpriteSheet.Value);

            if (c.minHarvest.Value > 1)
            {
                for (int i = c.minHarvest.Value; i <= c.maxHarvest.Value; i++)
                {

                    int chanceAgainst = r.Next(0, 100);
                    float chanceFor = (float)c.chanceForExtraCrops.Value + (Game1.player.farmingLevel * .03f);
                    if (chanceFor > chanceAgainst)
                    {
                        amountToHarvest++;
                    }
                }
            }
            Item I = (Item)new StardewValley.Object(c.indexOfHarvest.Value, amountToHarvest);

            int howMuch = 3;
            if (Game1.player.addItemToInventoryBool(I, false))
            {
                Vector2 vector2 = new Vector2((float)xTile, (float)yTile);

                Game1.player.animateOnce(279 + Game1.player.facingDirection);
                   // StardustCore.Utilities.animateOnce(Game1.player, 279 + Game1.player.facingDirection, 10f, 6, null, false, false, false);
                
                Game1.player.canMove = true;
                Game1.playSound("harvest");
                DelayedAction.playSoundAfterDelay("coin", 260);
                if (c.regrowAfterHarvest.Value == -1)
                {
                    Game1.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite(17, new Vector2(vector2.X * (float)Game1.tileSize, vector2.Y * (float)Game1.tileSize), Color.White, 7, Game1.random.NextDouble() < 0.5, 125f, 0, -1, -1f, -1, 0));
                    Game1.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite(14, new Vector2(vector2.X * (float)Game1.tileSize, vector2.Y * (float)Game1.tileSize), Color.White, 7, Game1.random.NextDouble() < 0.5, 50f, 0, -1, -1f, -1, 0));
                }
                else
                {
                    c.currentPhase.Value = c.regrowAfterHarvest.Value;
                }
                Game1.player.gainExperience(2, howMuch*amountToHarvest);
                return true;
            }

            return false;


        }

        public static bool harvestModularCrop(ModularCrop c, int xTile, int yTile, int fertilizer, JunimoHarvester junimoHarvester = null)
        {

            int amountToHarvest = 1;
            Random r = new Random(xTile + yTile + c.rowInSpriteSheet);

            if (c.minHarvest > 1)
            {
                for(int i = c.minHarvest; i <= c.maxHarvest; i++)
                {
        
                    int chanceAgainst = r.Next(0, 100);
                    float chanceFor =(float)c.chanceForExtraCrops + (Game1.player.farmingLevel * .03f);
                    if (chanceFor > chanceAgainst)
                    {
                        amountToHarvest++;
                    }
                }
            }

            Item I = (Item)new ModularCropObject(c.spriteSheet.getHelper(),c.indexOfHarvest, amountToHarvest, c.cropObjectTexture, c.cropObjectData);
            int howMuch = 3;
            if (Game1.player.addItemToInventoryBool(I, false))
            {
                Vector2 vector2 = new Vector2((float)xTile, (float)yTile);
                Game1.player.animateOnce(279 + Game1.player.facingDirection);
                  //  StardustCore.Utilities.animateOnce(Game1.player, 279 + Game1.player.facingDirection, 10f, 6, null, false, false, false);
                
                Game1.player.canMove = true;
                Game1.playSound("harvest");
                DelayedAction.playSoundAfterDelay("coin", 260);
                if (c.regrowAfterHarvest == -1)
                {
                    Game1.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite(17, new Vector2(vector2.X * (float)Game1.tileSize, vector2.Y * (float)Game1.tileSize), Color.White, 7, Game1.random.NextDouble() < 0.5, 125f, 0, -1, -1f, -1, 0));
                    Game1.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite(14, new Vector2(vector2.X * (float)Game1.tileSize, vector2.Y * (float)Game1.tileSize), Color.White, 7, Game1.random.NextDouble() < 0.5, 50f, 0, -1, -1f, -1, 0));

                }
                else
                {
                    c.currentPhase = c.regrowAfterHarvest;
                }
                Game1.player.gainExperience(2, howMuch*amountToHarvest);
                return true;
            }
            return false;
        }
    }
}
