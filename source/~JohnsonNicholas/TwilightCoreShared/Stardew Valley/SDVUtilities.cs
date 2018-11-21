using Microsoft.Xna.Framework;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Monsters;
using StardewValley.TerrainFeatures;
using System;
using System.Linq;
using System.Collections.Generic;
using TwilightShards.Common;
using xTile.Dimensions;
using SObject = StardewValley.Object;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;

namespace TwilightShards.Stardew.Common
{
    public static class SDVUtilities
    {
        public static bool TileIsClearForSpawning(GameLocation checkLoc, Vector2 tileVector,  StardewValley.Object tile)
        {
            if (tile == null && checkLoc.doesTileHaveProperty((int)tileVector.X, (int)tileVector.Y, "Diggable", "Back") != null && (checkLoc.isTileLocationOpen(new Location((int)tileVector.X * Game1.tileSize, (int)tileVector.Y * Game1.tileSize)) && !checkLoc.isTileOccupied(tileVector, "")) && checkLoc.doesTileHaveProperty((int)tileVector.X, (int)tileVector.Y, "Water", "Back") == null)
            {
                string PropCheck = checkLoc.doesTileHaveProperty((int)tileVector.X, (int)tileVector.Y, "NoSpawn", "Back");

                if (PropCheck == null || !PropCheck.Equals("Grass") && !PropCheck.Equals("All") && !PropCheck.Equals("True"))
                {
                    return true;
                }
            }
            return false;
        }

        public static string GetFestivalName() => GetFestivalName(Game1.dayOfMonth, Game1.currentSeason);
        public static string GetTomorrowFestivalName() => GetFestivalName(Game1.dayOfMonth + 1, Game1.currentSeason);
        
        public static string PrintStringArray(string[] array)
        {
            string s = "";
            for (int i = 0; i < array.Length; i++)
            {
                s = s + $"Command {i} is {array[i]}";
            }

            return s;
        }

        public static string GetWeatherName()
        {
            if ((!Game1.isRaining) && (!Game1.isDebrisWeather) && (!Game1.isSnowing) && (!Game1.isLightning) && (!Utility.isFestivalDay(Game1.dayOfMonth, Game1.currentSeason)) && (!Game1.weddingToday))
                return "sunny";
            if (SDVUtilities.IsFestivalDay)
                return "festival";
            if (Game1.weddingToday)
                return "wedding";
            if (Game1.isRaining)
                return "rain";
            if (Game1.isDebrisWeather)
                return "debris";
            if (Game1.isSnowing)
                return "snowy";
            if (Game1.isRaining && Game1.isLightning)
                return "stormy";

            return "ERROR";
        }

        public static string PrintCurrentWeatherStatus()
        {
            return $"Printing current weather status:" +
                    $"It is Raining: {Game1.isRaining} {Environment.NewLine}" +
                    $"It is Stormy: {Game1.isLightning} {Environment.NewLine}" +
                    $"It is Snowy: {Game1.isSnowing} {Environment.NewLine}" +
                    $"It is Debris Weather: {Game1.isDebrisWeather} {Environment.NewLine}";
        }

        internal static bool IsFestivalDay => Utility.isFestivalDay(SDate.Now().Day, SDate.Now().Season);

        internal static string GetFestivalName(SDate date) => SDVUtilities.GetFestivalName(date.Day, date.Season);

        private static string GetFestivalName(int dayOfMonth, string currentSeason)
        {
            switch (currentSeason)
            {
                case ("spring"):
                    if (dayOfMonth == 13) return "Egg Festival";
                    if (dayOfMonth == 24) return "Flower Dance";
                    break;
                case ("winter"):
                    if (dayOfMonth == 8) return "Festival of Ice";
                    if (dayOfMonth == 25) return "Feast of the Winter Star";
                    if (dayOfMonth == 14) return "Night Festival";
                    if (dayOfMonth == 15) return "Night Festival";
                    if (dayOfMonth == 16) return "Night Festival";
                    break;
                case ("fall"):
                    if (dayOfMonth == 16) return "Stardew Valley Fair";
                    if (dayOfMonth == 27) return "Spirit's Eve";
                    break;
                case ("summer"):
                    if (dayOfMonth == 11) return "Luau";
                    if (dayOfMonth == 28) return "Dance of the Moonlight Jellies";
                    break;
                default:
                    return $"";
            }

            return $"";

        }

        public static T GetModApi<T>(IMonitor Monitor, IModHelper Helper, string name, string minVersion) where T : class
        {
            var modManifest = Helper.ModRegistry.Get(name);
            if (modManifest != null)
            {
                if (!modManifest.Manifest.Version.IsOlderThan(minVersion))
                {
                    T api = Helper.ModRegistry.GetApi<T>(name);
                    if (api == null)
                    {
                        Monitor.Log($"{name}'s API returned null.", LogLevel.Info);
                    }

                    if (api != null)
                    {
                        Monitor.Log($"{name} {modManifest.Manifest.Version} Integration enabled", LogLevel.Info);
                    }
                    return api;

                }
                else
                    Monitor.Log($"{name} detected, but not of a sufficient version. Req:{minVersion} Detected:{modManifest.Manifest.Version}. Skipping..", LogLevel.Debug);
            }
            else
                Monitor.Log($"{name} not present. Skipping Integration.", LogLevel.Debug);
            return null;
        }

        public static void ShowMessage(string msg, int whatType)
        {
            var hudmsg = new HUDMessage(msg, Color.SeaGreen, 5250f, true)
            {
                whatType = whatType
            };
            Game1.addHUDMessage(hudmsg);
        }

        public static int CropCountInFarm(Farm f)
        {
            return f.terrainFeatures.Values.Count(c => c is HoeDirt curr && curr.crop != null);
        }

        public static Color SubtractTwoColors(Color one, Color two)
        {
            Color three = new Color(0, 0, 0)
            {
                R = (byte)(one.R - two.R),
                B = (byte)(one.B - two.B),
                G = (byte)(one.G - two.G),
                A = (byte)(one.A - two.A)
            };

            return three;
        }

        public static void SpawnGhostOffScreen(MersenneTwister Dice)
        {
            Vector2 zero = Vector2.Zero;
            if (Game1.getFarm() is Farm ourFarm)
            {
                switch (Game1.random.Next(4))
                {
                    case 0:
                        zero.X = Dice.Next(ourFarm.map.Layers[0].LayerWidth);
                        break;
                    case 1:
                        zero.X = (ourFarm.map.Layers[0].LayerWidth - 1);
                        zero.Y = Dice.Next(ourFarm.map.Layers[0].LayerHeight);
                        break;
                    case 2:
                        zero.Y = (ourFarm.map.Layers[0].LayerHeight - 1);
                        zero.X = Dice.Next(ourFarm.map.Layers[0].LayerWidth);
                        break;
                    case 3:
                        zero.Y = Game1.random.Next(ourFarm.map.Layers[0].LayerHeight);
                        break;
                }

                if (Utility.isOnScreen(zero * Game1.tileSize, Game1.tileSize))
                    zero.X -= Game1.viewport.Width;

                List<NPC> characters = ourFarm.characters.ToList();
                Ghost ghost = new Ghost(zero * Game1.tileSize)
                {
                    focusedOnFarmers = true,
                    wildernessFarmMonster = true
                };
                ghost.reloadSprite();
                characters.Add(ghost);
            }
        }

        public static double GetDistance(Vector2 alpha, Vector2 beta)
        {
            return Math.Sqrt(Math.Pow(beta.X - alpha.X, 2) + Math.Pow(beta.Y - alpha.Y, 2));
        }

        public static void SpawnMonster(GameLocation location)
        {
            Vector2 zero = Vector2.Zero;
            Vector2 randomTile = Vector2.Zero;
            int numTries = 0;
            do
            {
                randomTile = location.getRandomTile();
                numTries++;
            } while (GetDistance(randomTile, Game1.player.position) > 45 && numTries < 10000);            

            if (Utility.isOnScreen(Utility.Vector2ToPoint(randomTile), Game1.tileSize, location))
                randomTile.X -= (Game1.viewport.Width / Game1.tileSize);

            if (location.isTileLocationTotallyClearAndPlaceable(randomTile))
            {
                if (Game1.player.CombatLevel >= 10 && Game1.random.NextDouble() < .05)
                {
                    Skeleton skeleton = new Skeleton(randomTile * Game1.tileSize)
                    {
                        focusedOnFarmers = true
                    };
                    location.characters.Add(skeleton);
                }

                if (Game1.player.CombatLevel >= 8 && Game1.random.NextDouble() < 0.15)
                {
                    ShadowBrute shadowBrute = new ShadowBrute(randomTile * Game1.tileSize)
                    {
                        focusedOnFarmers = true
                    };
                    location.characters.Add(shadowBrute);
                }
                else if (Game1.random.NextDouble() < 0.65 && location.isTileLocationTotallyClearAndPlaceable(randomTile))
                {
                    RockGolem rockGolem = new RockGolem(randomTile * Game1.tileSize, Game1.player.CombatLevel)
                    {
                        focusedOnFarmers = true
                    };
                    location.characters.Add(rockGolem);
                }
                else
                {
                    int mineLevel = 1;
                    if (Game1.player.CombatLevel >= 10)
                        mineLevel = 140;
                    else if (Game1.player.CombatLevel >= 8)
                        mineLevel = 100;
                    else if (Game1.player.CombatLevel >= 4)
                        mineLevel = 41;

                    GreenSlime greenSlime = new GreenSlime(randomTile * Game1.tileSize, mineLevel);
                    location.characters.Add(greenSlime);
                }
            }
        }

        private static void AdvanceCropOneStep(GameLocation loc, HoeDirt h, Vector2 position)
        {
            Crop currCrop = h.crop;
            int xPos = (int)position.X;
            int yPos = (int)position.Y;

            if (currCrop == null)
                return;

            //due to how this will be called, we do need to some checking
            if (!loc.Name.Equals("Greenhouse") && (currCrop.dead.Value || !currCrop.seasonsToGrowIn.Contains(Game1.currentSeason)))
            {
                currCrop.dead.Value = true;
            }
            else
            {
                if (h.state.Value == HoeDirt.watered)
                {
                    //get the day of the current phase - if it's fully grown, we can just leave it here.
                    if (currCrop.fullyGrown.Value)
                        currCrop.dayOfCurrentPhase.Value = currCrop.dayOfCurrentPhase.Value - 1;
                    else
                    {
                        //check to sere what the count of current days is

                        int phaseCount = 0; //get the count of days in the current phase
                        if (currCrop.phaseDays.Count > 0)
                            phaseCount = currCrop.phaseDays[Math.Min(currCrop.phaseDays.Count - 1, currCrop.currentPhase.Value)];
                        else
                            phaseCount = 0;

                        currCrop.dayOfCurrentPhase.Value = Math.Min(currCrop.dayOfCurrentPhase.Value + 1, phaseCount);

                        //check phases
                        if (currCrop.dayOfCurrentPhase.Value >= phaseCount && currCrop.currentPhase.Value < currCrop.phaseDays.Count - 1)
                        {
                            currCrop.currentPhase.Value++;
                            currCrop.dayOfCurrentPhase.Value = 0;
                        }

                        //skip negative day or 0 day crops.
                        while (currCrop.currentPhase.Value < currCrop.phaseDays.Count - 1 && currCrop.phaseDays.Count > 0 && currCrop.phaseDays[currCrop.currentPhase.Value] <= 0)
                        {
                            currCrop.currentPhase.Value++;
                        }

                        //handle wild crops
                        if (currCrop.isWildSeedCrop() && currCrop.phaseToShow.Value == -1 && currCrop.currentPhase.Value > 0)
                            currCrop.phaseToShow.Value = Game1.random.Next(1, 7);

                        //and now giant crops
                        double giantChance = new Random((int)Game1.uniqueIDForThisGame + (int)Game1.stats.daysPlayed + xPos * 2000 + yPos).NextDouble();

                        if (loc is Farm && currCrop.currentPhase.Value == currCrop.phaseDays.Count - 1 && IsValidGiantCrop(currCrop.indexOfHarvest.Value) &&
                            giantChance <= 0.01)
                        {
                            for (int i = xPos - 1; i <= xPos + 1; i++)
                            {
                                for (int j = yPos - 1; j <= yPos + 1; j++)
                                {
                                    Vector2 tile = new Vector2(i, j);
                                    if (!loc.terrainFeatures.ContainsKey(tile) || !(loc.terrainFeatures[tile] is HoeDirt) ||
                                        (loc.terrainFeatures[tile] as HoeDirt).crop?.indexOfHarvest == currCrop.indexOfHarvest)
                                    {
                                        return; //no longer needs to process.
                                    }
                                }
                            }


                            //replace for giant crops.
                            for (int i = xPos - 1; i <= xPos + 1; i++)
                            {
                                for (int j = yPos - 1; j <= yPos + 1; j++)
                                {
                                    Vector2 tile = new Vector2(i, j);
                                    (loc.terrainFeatures[tile] as HoeDirt).crop = null;
                                }
                            }

                        (loc as Farm).resourceClumps.Add(new GiantCrop(currCrop.indexOfHarvest.Value, new Vector2(xPos - 1, yPos - 1)));

                        }
                    }
                }
                //process some edge cases for non watered crops.
                if (currCrop.fullyGrown.Value && currCrop.dayOfCurrentPhase.Value > 0 ||
                    currCrop.currentPhase.Value < currCrop.phaseDays.Count - 1 ||
                    !currCrop.isWildSeedCrop())

                    return; //stop processing

                //replace wild crops**

                //remove any object here. o.O
                loc.objects.Remove(position);

                string season = Game1.currentSeason;
                switch (currCrop.whichForageCrop.Value)
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
                loc.objects.Add(position, new SObject(position, currCrop.getRandomWildCropForSeason(season), 1)
                {
                    IsSpawnedObject = true,
                    CanBeGrabbed = true
                });

                //the normal iteration has a safe-call that isn't neded here               
            }
        }

        private static bool IsValidGiantCrop(int cropID)
        {
            int[] crops = new int[] { 276, 190, 254 };

            if (crops.Contains(cropID))
                return true;

            return false;
        }

        public static void AdvanceArbitrarySteps(GameLocation loc, HoeDirt h, Vector2 position, int numDays = 1)
        {
            for (int i = 0; i < numDays; i++)
                AdvanceCropOneStep(loc, h, position);
        }

        public static void RedrawMouseCursor()
        {
            if (Game1.activeClickableMenu == null && Game1.mouseCursor > -1 && (Mouse.GetState().X != 0 || Mouse.GetState().Y != 0) && (Game1.getOldMouseX() != 0 || Game1.getOldMouseY() != 0))
            {
                if (Game1.mouseCursorTransparency <= 0.0 || !Utility.canGrabSomethingFromHere(Game1.getOldMouseX() + Game1.viewport.X, Game1.getOldMouseY() + Game1.viewport.Y, Game1.player) || Game1.mouseCursor == 3)
                {
                    if (Game1.player.ActiveObject != null && Game1.mouseCursor != 3 && !Game1.eventUp)
                    {
                        if (Game1.mouseCursorTransparency > 0.0 || Game1.options.showPlacementTileForGamepad)
                        {
                            Game1.player.ActiveObject.drawPlacementBounds(Game1.spriteBatch, Game1.currentLocation);
                            if (Game1.mouseCursorTransparency > 0.0)
                            {
                                bool flag = Utility.playerCanPlaceItemHere(Game1.currentLocation, Game1.player.CurrentItem, Game1.getMouseX() + Game1.viewport.X, Game1.getMouseY() + Game1.viewport.Y, Game1.player) || Utility.isThereAnObjectHereWhichAcceptsThisItem(Game1.currentLocation, Game1.player.CurrentItem, Game1.getMouseX() + Game1.viewport.X, Game1.getMouseY() + Game1.viewport.Y) && Utility.withinRadiusOfPlayer(Game1.getMouseX() + Game1.viewport.X, Game1.getMouseY() + Game1.viewport.Y, 1, Game1.player);
                                Game1.player.CurrentItem.drawInMenu(Game1.spriteBatch, new Vector2((Game1.getMouseX() + Game1.tileSize / 4), (Game1.getMouseY() + Game1.tileSize / 4)), flag ? (float)(Game1.dialogueButtonScale / 75.0 + 1.0) : 1f, flag ? 1f : 0.5f, 0.999f);
                            }
                        }
                    }
                    else if (Game1.mouseCursor == 0 && Game1.isActionAtCurrentCursorTile)
                        Game1.mouseCursor = Game1.isInspectionAtCurrentCursorTile ? 5 : 2;
                }
                if (!Game1.options.hardwareCursor)
                    Game1.spriteBatch.Draw(Game1.mouseCursors, new Vector2(Game1.getMouseX(), Game1.getMouseY()), new Microsoft.Xna.Framework.Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, Game1.mouseCursor, 16, 16)), Color.White * Game1.mouseCursorTransparency, 0.0f, Vector2.Zero, Game1.pixelZoom + Game1.dialogueButtonScale / 150f, SpriteEffects.None, 1f);
                Game1.wasMouseVisibleThisFrame = Game1.mouseCursorTransparency > 0.0;
            }
            Game1.mouseCursor = 0;
            if (Game1.isActionAtCurrentCursorTile || Game1.activeClickableMenu != null)
                return;
            Game1.mouseCursorTransparency = 1f;
        }

        public static int CreateWeeds(GameLocation spawnLoc, int numOfWeeds)
        {
            if (spawnLoc == null)
                throw new Exception("The passed spawn location cannot be null!");

            int CreatedWeeds = 0;

            for (int i = 0; i <= numOfWeeds; i++)
            {
                //limit number of attempts per attempt to 10.
                int numberOfAttempts = 0;
                while (numberOfAttempts < 3)
                {
                    //get a random tile.
                    int xTile = Game1.random.Next(spawnLoc.map.DisplayWidth / Game1.tileSize);
                    int yTile = Game1.random.Next(spawnLoc.map.DisplayHeight / Game1.tileSize);
                    Vector2 randomVector = new Vector2((float)xTile, (float)yTile);
                    spawnLoc.objects.TryGetValue(randomVector, out SObject @object);

                    if (SDVUtilities.TileIsClearForSpawning(spawnLoc, randomVector, @object))
                    {
                        //for now, don't spawn in winter.
                        if (Game1.currentSeason != "winter")
                        {
                            //spawn the weed
                            spawnLoc.objects.Add(randomVector, new SObject(randomVector, GameLocation.getWeedForSeason(Game1.random, Game1.currentSeason), 1));
                            CreatedWeeds++;
                        }
                    }
                    numberOfAttempts++; // this might have been more useful INSIDE the while loop.
                }
            }
            return CreatedWeeds;
        }

    }
}
