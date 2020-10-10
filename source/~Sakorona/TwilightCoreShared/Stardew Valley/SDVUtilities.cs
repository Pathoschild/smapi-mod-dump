/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Sakorona/SDVMods
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Monsters;
using StardewValley.TerrainFeatures;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using TwilightShards.Common;
using xTile.Dimensions;
using SObject = StardewValley.Object;
using StardewModdingAPI;
using StardewValley.Locations;

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

        internal static void SetWeather(int weather)
        {
            Game1.netWorldState.Value.WeatherForTomorrow = Game1.weatherForTomorrow = weather;
        }

        internal static bool UpdateAudio()
        {

            if (!Game1.isRaining && Game1.currentSong.Name.Equals("rain"))
            {
                Game1.stopMusicTrack(Game1.MusicContext.Default);
                Game1.currentLocation.checkForMusic(Game1.currentGameTime);
            }

            if (Game1.eventUp)
                return false;

            if (Game1.currentSong.Name.Contains("none"))
            {
                if (Game1.currentLocation is FarmHouse || Game1.currentLocation is Farm)
                {
                    Game1.currentLocation.checkForMusic(Game1.currentGameTime);
                    return true;
                }
                return false;
            }

            if (Game1.currentLocation is Desert || Game1.currentLocation is LibraryMuseum || Game1.currentLocation is Club || Game1.currentLocation is CommunityCenter
                || Game1.currentLocation is LibraryMuseum || Game1.currentLocation is BathHousePool || Game1.currentLocation is MovieTheater || Game1.currentLocation is MermaidHouse
                || Game1.currentLocation is Submarine || Game1.currentLocation.Name == "SandyHouse")
                return false;

            if (Game1.currentLocation.Name == "Sunroom" && !Game1.isRaining)
            {
                Game1.changeMusicTrack("SunRoom", false, Game1.MusicContext.SubLocation);
                return true;
            }

            if (Game1.isRaining && (Game1.currentSong == null || !Game1.currentSong.Name.Equals("rain")) && !Game1.currentLocation.Name.StartsWith("UndergroundMine"))
            {
                Game1.changeMusicTrack("rain", true, Game1.MusicContext.Default);
                return true;
            }

            if (!Game1.isRaining)
            {
                if (Game1.currentLocation is Woods && Game1.timeOfDay < 1800 && !Game1.isDarkOut())
                {
                    Game1.changeMusicTrack("woodsTheme", false, Game1.MusicContext.Default);
                    return true;
                }
                if (!Game1.eventUp && (Game1.currentLocation.IsOutdoors || Game1.currentLocation is FarmHouse || Game1.currentLocation is AnimalHouse
                    || Game1.currentLocation is Shed && Game1.options.musicVolumeLevel > 0.025 && Game1.timeOfDay < 1200) && (Game1.currentSong.Name.Contains("ambient") || Game1.currentSong.Name.Contains("none")))

                    Game1.playMorningSong();
                else
                    Game1.currentLocation.checkForMusic(Game1.currentGameTime);
                return true;
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
                s += $"Command {i} is {array[i]}";
            }

            return s;
        }

        public static Type GetSDVType(string type)
        {
            const string prefix = "StardewValley.";

            return Type.GetType(prefix + type + ", Stardew Valley") ?? Type.GetType(prefix + type + ", StardewValley");
        }

        public static bool IsWinterForageable(int index)
        {
            switch (index)
            {
                case 412:
                case 414:
                case 416:
                case 418:
                    return true;
                default:
                    return false;
            }
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
                    if (dayOfMonth == 15) return "Night Festival";
                    if (dayOfMonth == 16) return "Night Festival";
                    if (dayOfMonth == 17) return "Night Festival";
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

        public static T GetModApi<T>(IMonitor Monitor, IModHelper Helper, string name, string minVersion, string friendlyName="") where T : class
        {
            var modManifest = Helper.ModRegistry.Get(name);
            if (modManifest != null)
            {
                if (!modManifest.Manifest.Version.IsOlderThan(minVersion))
                {
                    T api = Helper.ModRegistry.GetApi<T>(name);
                    if (api == null)
                    {
                        Monitor.Log($"{(String.IsNullOrEmpty(friendlyName) ? name : friendlyName)}'s API returned null. ", LogLevel.Info);
                    }

                    if (api != null)
                    {
                        Monitor.Log($"{(String.IsNullOrEmpty(friendlyName) ? name : friendlyName)} {modManifest.Manifest.Version} integration feature enabled", LogLevel.Info);
                    }
                    return api;

                }
                else
                    Monitor.Log($"{(String.IsNullOrEmpty(friendlyName) ? name : friendlyName)} detected, but not of a sufficient version. Req:{minVersion} Detected:{modManifest.Manifest.Version}. Update the other mod if you want to use the integration feature. Skipping..", LogLevel.Debug);
            }
            else
                Monitor.Log($"Didn't find mod {(String.IsNullOrEmpty(friendlyName) ? name : friendlyName)}; you can optionally install it for extra features!", LogLevel.Debug);
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

        public static Vector2 SpawnRandomMonster(GameLocation location)
        {
            for (int index = 0; index < 15; ++index)
            {
                Vector2 randomTile = location.getRandomTile();
                if (Utility.isOnScreen(Utility.Vector2ToPoint(randomTile), 64, location))
                    randomTile.X -= Game1.viewport.Width / 64;
                if (location.isTileLocationTotallyClearAndPlaceable(randomTile))
                {
                    if (Game1.player.CombatLevel >= 10 && Game1.MasterPlayer.deepestMineLevel >= 145 && Game1.random.NextDouble() <= .001 && Game1.MasterPlayer.stats.getMonstersKilled("Pepper Rex") > 0)
                    {
                        DinoMonster squidKid = new DinoMonster(randomTile * Game1.tileSize)
                        {
                            focusedOnFarmers = true
                        };
                        location.characters.Add(squidKid);

                        return (randomTile);
                    }

                    else if (Game1.player.CombatLevel >= 10 && Game1.MasterPlayer.deepestMineLevel >= 145 && Game1.random.NextDouble() <= .05)
                    {
                        MetalHead squidKid = new MetalHead(randomTile * Game1.tileSize,145)
                        {
                            focusedOnFarmers = true
                        };
                        location.characters.Add(squidKid);
                        
                        return (randomTile);
                    }

                    else if (Game1.player.CombatLevel >= 10 && Game1.random.NextDouble() <= .25)
                    {
                        Skeleton skeleton = new Skeleton(randomTile * Game1.tileSize)
                        {
                            focusedOnFarmers = true
                        };
                        location.characters.Add(skeleton);
                        
                        return (randomTile);
                    }

                    else if (Game1.player.CombatLevel >= 8 && Game1.random.NextDouble() <= 0.15)
                    {
                        ShadowBrute shadowBrute = new ShadowBrute(randomTile * Game1.tileSize)
                        {
                            focusedOnFarmers = true
                        };
                        location.characters.Add(shadowBrute);
                        
                        return (randomTile);
                    }
                    else if (Game1.random.NextDouble() < 0.65 && location.isTileLocationTotallyClearAndPlaceable(randomTile))
                     {
                         RockGolem rockGolem = new RockGolem(randomTile * Game1.tileSize, Game1.player.CombatLevel)
                         {
                             focusedOnFarmers = true
                         };
						 rockGolem.Sprite.currentFrame = 16;
						 rockGolem.Sprite.loop = false;
						 rockGolem.Sprite.UpdateSourceRect();
                         location.characters.Add(rockGolem);
                         return (randomTile);
                     } 
                    else
                    {
                        int mineLevel;
                        if (Game1.player.CombatLevel > 1 && Game1.player.CombatLevel <= 4)
							mineLevel = 41;
						else if (Game1.player.CombatLevel > 4 && Game1.player.CombatLevel <= 8)
							mineLevel = 100;
						else if (Game1.player.CombatLevel > 8 && Game1.player.CombatLevel <= 10)
							mineLevel = 140;
						else 
							mineLevel = 200;

                        GreenSlime greenSlime = new GreenSlime(randomTile * Game1.tileSize, mineLevel)
                        {
                            focusedOnFarmers = true,
                        };
                        greenSlime.color.Value = Color.IndianRed;
                        location.characters.Add(greenSlime);
                        
                        return (randomTile);
                    }
                }
            }

            return Vector2.Zero;
        }

        public static void AlterWaterStatusOfCrops(bool water = true)
        {
            foreach (GameLocation loc in Game1.locations)
            {
                if (loc is Farm)
                {
                    foreach (TerrainFeature tf in loc.terrainFeatures.Values)
                    {
                        if (tf is HoeDirt h && !(h.crop is null))
                        {
                            if (water)
                                h.state.Value = 1;
                        }
                    }
                }
            }
        }

        public static string CropStatus(HoeDirt h, Vector2 pos)
        {
            string ret = "";
            ret += $"This crop is at {pos}.";
            ret += $"Harvest index is {GetNameFromCrop(h.crop.indexOfHarvest.Value)} ({h.crop.indexOfHarvest.Value})." + Environment.NewLine;

            ret += "Days per phase are [";
            for (int i = 0; i < h.crop.phaseDays.Count; i++)
            {
                ret += " " + h.crop.phaseDays[i];
            }
            ret +=" ] " + Environment.NewLine;
			
			ret += $"Current phase is {h.crop.currentPhase.Value}, with the current day in the phase being {h.crop.dayOfCurrentPhase.Value}, and the last growth phase being {h.crop.phaseDays.Count - 1}" + Environment.NewLine;
			
			if (h.crop.programColored.Value)
				ret += $"The stored color is {h.crop.tintColor.Value}" + Environment.NewLine;
			
			if (h.crop.regrowAfterHarvest.Value != -1)
			{
				ret += $"The crop regrows every {h.crop.regrowAfterHarvest.Value}." + Environment.NewLine;
			}
			
			ret += (h.state.Value == 1 ? "The crop is watered. " : "The crop is unwatered. ");
			
			if (h.fertilizer.Value != 0)
			{
				switch (h.fertilizer.Value)
				{
					case 368:
					  ret += "The crop has Basic Fertilizer applied.";
					  break;
					case 369:
					  ret += "The crop has Deluxe Fertilizer applied.";
					  break;
					case 370:
					  ret += "The crop has basic water retaining fertilizer applied.";
					  break;
					case 371:
					  ret += "The crop has deluxe water retaining fertilizer applied.";
					  break;
					case 465:
					  ret += "The crop has basic speed-gro applied.";
					  break;
					case 466:
					  ret += "The crop has deluxe speed-gro applied.";
					  break;
					default:
					  ret += $"The crop has had unknown fertilizer {h.fertilizer.Value}";
					  break;
				}
				
				ret += Environment.NewLine;
			}
			else
			{
				ret += Environment.NewLine;
			}
			
			ret += $"Fully grown status: {h.crop.fullyGrown.Value}. This crop can be harvested: {h.readyForHarvest()}"+ Environment.NewLine;
			
			return ret;
		}
		
		public static string GetNameFromCrop(int index)
		{
			Dictionary<int, string> dictionary = Game1.content.Load<Dictionary<int, string>>("Data\\ObjectInformation");
            
            foreach(var c in dictionary)
            {
                string[] strArray1 = c.Value.Split('/');
                if (c.Key == index)
					return strArray1[0];

            }

			return "ERROR";           
		}

        /// <summary>
        /// This function prints crop data in detail.
        /// </summary>
        /// <param name="loc">Location of the crop</param>
        /// <param name="h">Hoe Dirt of the crop</param>
        /// <param name="position">The position of the crop</param>
        /// <returns>A string description of the crop</returns>
        public static string PrintCropData(GameLocation loc, HoeDirt h, Vector2 position)
        {
            Crop currentCrop = h.crop;
            string desc = "";

            if (currentCrop is null)
                return $"This is not a crop at {loc} and {position}";

            //load crop data
            string objName = Game1.objectInformation[currentCrop.indexOfHarvest.Value].Split('/')[0];

            //describe crop data.
            desc += $"This crop has harvest index: {currentCrop.indexOfHarvest.Value} ({objName}).{Environment.NewLine} Current Phase: {currentCrop.currentPhase}, phase calendar: {currentCrop.phaseDays}, final phase: {(currentCrop.phaseDays.Count)-1}";

            desc +=
                $"{Environment.NewLine} Day of Current Phase: {currentCrop.dayOfCurrentPhase}, Valid Seasons: {currentCrop.seasonsToGrowIn}. Regrowth after Harvest: {currentCrop.regrowAfterHarvest.Value}";

            return desc;
        }

        /// <summary>
        /// This function regresses crops
        /// </summary>
        /// <param name="loc">Location of the crop</param>
        /// <param name="h">Hoe Dirt of the crop</param>
        /// <param name="position">The position of the crop</param>
        /// <param name="numSteps">The number of steps the crop is being regressed by</param>
        /// <param name="giantCropRequiredSteps">The number of minimum steps to affect the giant crop (default: 4)</param>
        /// <param name="giantCropDestructionOdds">The chance of the giant crop being affected (default: 50%)</param>
        /// <exception cref="">Throws a generic exception if it finds a giant crop that doesn't have an actual crop backing.</exception>
        public static void DeAdvanceCrop(GameLocation loc, HoeDirt h, Vector2 position, int numSteps, IMonitor Logger, int giantCropRequiredSteps = 4, double giantCropDestructionOdds = .5)
        {
            //determine the phase of the crop
            Crop currentCrop = h.crop;

            //data on the crop. Outputting to debug.
            Logger.Log($"BEFORE CROP DEADVANCEMENT: {PrintCropData(loc, h, position)}", LogLevel.Debug);

            if (!(currentCrop is null))
            {
                int countPhases = currentCrop.phaseDays.Count;
                int finalPhase = countPhases - 1;

                for (int i = 0; i < numSteps; i++)
                {
                    //now, check the phase - handle the final phase.
                    if (currentCrop.currentPhase.Value == finalPhase && currentCrop.regrowAfterHarvest.Value == -1)
                    {
                        if (currentCrop.dayOfCurrentPhase.Value > 0){ 
							currentCrop.dayOfCurrentPhase.Value--;
						}
                        else if (currentCrop.dayOfCurrentPhase.Value == 0)
                        {
                            currentCrop.fullyGrown.Value = false;
                            currentCrop.currentPhase.Value--;
                            currentCrop.dayOfCurrentPhase.Value = currentCrop.phaseDays[currentCrop.currentPhase.Value];
                        }
                        Logger.Log($"AFTER CROP DEADVANCEMENT: {PrintCropData(loc, h, position)}", LogLevel.Debug);
                        continue;
                    }

                    //handle regrowth crops.
                    if (currentCrop.regrowAfterHarvest.Value != -1 && currentCrop.currentPhase.Value  == finalPhase)
                    {
                        currentCrop.dayOfCurrentPhase.Value++;
                        Logger.Log($"AFTER CROP DEADVANCEMENT: {PrintCropData(loc, h, position)}", LogLevel.Debug);
                        continue;
                    }

                    //now handle it being any phase but 0.
                    if (currentCrop.currentPhase.Value != finalPhase || currentCrop.currentPhase.Value != 0)
                    {
                        if (currentCrop.dayOfCurrentPhase.Value >= currentCrop.phaseDays[currentCrop.currentPhase.Value] && currentCrop.currentPhase.Value > 0)
                        {
                            currentCrop.currentPhase.Value--;
                            currentCrop.dayOfCurrentPhase.Value = currentCrop.phaseDays[currentCrop.currentPhase.Value];
                        }
                        else
                        {
                            currentCrop.dayOfCurrentPhase.Value++;
                        }
                        Logger.Log($"AFTER CROP DEADVANCEMENT: {PrintCropData(loc, h, position)}", LogLevel.Debug);
                        continue;
                    }

                    //final check. Phase 0.
                    if (currentCrop.currentPhase.Value == 0)
                    {
                        if (currentCrop.dayOfCurrentPhase.Value != 0 && currentCrop.dayOfCurrentPhase.Value > 0)
                        {
                            currentCrop.dayOfCurrentPhase.Value--;
                        }
                        Logger.Log($"AFTER CROP DEADVANCEMENT: {PrintCropData(loc, h, position)}", LogLevel.Debug);
                        continue;
                    }

                    //Sanity check here.
                    if (currentCrop.currentPhase.Value < 0){
                        currentCrop.currentPhase.Value = 0;
					}
                }
            }

            //check for giant crop.
            if (loc is Farm f)
            {
                foreach (ResourceClump rc in f.resourceClumps)
                {
                    if (rc is GiantCrop gc && CheckIfPositionIsWithinGiantCrop(position, gc))
                    {
                        //This breaks my heart, given the requirements...
                        if (numSteps > giantCropRequiredSteps && Game1.random.NextDouble() < giantCropDestructionOdds)
                        {
                            numSteps -= giantCropRequiredSteps;
                            Vector2 upperLeft = gc.tile.Value;
                            int cropReplacement = gc.parentSheetIndex.Value, width = gc.width.Value, height = gc.height.Value;

                            int? cropSeed = GetCropForSheetIndex(cropReplacement);
                            if (cropSeed == null)
                                throw new Exception($"Somehow, this giant crop has no valid seed from it's stored parent index. This needs to be troubleshooted. Parent seed index is {cropReplacement}");

                            f.resourceClumps.Remove(gc);
                            for (int i = 0; i < width; i++)
                            {
                                for (int j = 0; j < height; j++)
                                {
                                    Vector2 currPos = new Vector2(upperLeft.X + i, upperLeft.Y + i);
                                    HoeDirt hd = new HoeDirt(1)
                                    {
                                        crop = new Crop((int)cropSeed, (int)currPos.X, (int)currPos.Y)
                                    };
                                    hd.crop.growCompletely();
                                    loc.terrainFeatures.Add(currPos, hd);
                                }
                            }
                        }
                    }
                }
            }
            Logger.Log($"AFTER CROP DEADVANCEMENT: {PrintCropData(loc, h, position)}", LogLevel.Debug);
            //we aren't handling forage crops here.
        }
        
        /// <summary>
        /// This function returns a crop seed given the harvested crop ID
        /// </summary>
        /// <param name="sheetIndex">The sheet index of the harvested crop</param>
        /// <returns>The crop seed if it exists, null if it does not</returns>
        public static int? GetCropForSheetIndex(int sheetIndex)
        {
            Dictionary<int, string> dictionary = Game1.content.Load<Dictionary<int, string>>("Data\\Crops");
            int? val = null;
            foreach(var c in dictionary)
            {
                string[] strArray1 = c.Value.Split('/');
                if (strArray1[3] == sheetIndex.ToString())
                {
                    val = c.Key;
                }

            }

            return val;
        }

        public static bool CheckIfPositionIsWithinGiantCrop(Vector2 position, GiantCrop g)
        {
          if (position.X >= g.tile.Value.X && position.X <= g.tile.Value.X + g.width.Value)
          {
              if (position.Y >= g.tile.Value.Y && position.Y < g.tile.Value.Y + g.height.Value)
              {
                  return true;
              }
          }

          return false;
        }

        private static void AdvanceCropOneStep(GameLocation loc, HoeDirt h, Vector2 position)
        {
            Crop currentCrop = h.crop;
            int xPos = (int)position.X;
            int yPos = (int)position.Y;

            if (currentCrop == null)
                return;

            //due to how this will be called, we do need to some checking
            if (!loc.IsGreenhouse && (currentCrop.dead.Value || !currentCrop.seasonsToGrowIn.Contains(Game1.currentSeason)))
            {
                currentCrop.dead.Value = true;
            }
            else
            {
                if (h.state.Value == HoeDirt.watered)
                {
                    //get the day of the current phase - if it's fully grown, we can just leave it here.
                    if (currentCrop.fullyGrown.Value)
                        currentCrop.dayOfCurrentPhase.Value -= 1;
                    else
                    {
                        //check to sere what the count of current days is

                        int phaseCount; //get the count of days in the current phase
                        if (currentCrop.phaseDays.Count > 0)
                            phaseCount = currentCrop.phaseDays[Math.Min(currentCrop.phaseDays.Count - 1, currentCrop.currentPhase.Value)];
                        else
                            phaseCount = 0;

                        currentCrop.dayOfCurrentPhase.Value = Math.Min(currentCrop.dayOfCurrentPhase.Value + 1, phaseCount);

                        //check phases
                        if (currentCrop.dayOfCurrentPhase.Value >= phaseCount && currentCrop.currentPhase.Value < currentCrop.phaseDays.Count - 1)
                        {
                            currentCrop.currentPhase.Value++;
                            currentCrop.dayOfCurrentPhase.Value = 0;
                        }

                        //skip negative day or 0 day crops.
                        while (currentCrop.currentPhase.Value < currentCrop.phaseDays.Count - 1 && currentCrop.phaseDays.Count > 0 && currentCrop.phaseDays[currentCrop.currentPhase.Value] <= 0)
                        {
                            currentCrop.currentPhase.Value++;
                        }

                        //handle wild crops
                        if (currentCrop.isWildSeedCrop() && currentCrop.phaseToShow.Value == -1 && currentCrop.currentPhase.Value > 0)
                            currentCrop.phaseToShow.Value = Game1.random.Next(1, 7);

                        //and now giant crops
                        double giantChance = new Random((int)Game1.uniqueIDForThisGame + (int)Game1.stats.daysPlayed + xPos * 2000 + yPos).NextDouble();

                        if (loc is Farm && currentCrop.currentPhase.Value == currentCrop.phaseDays.Count - 1 && IsValidGiantCrop(currentCrop.indexOfHarvest.Value) &&
                            giantChance <= 0.01)
                        {
                            for (int i = xPos - 1; i <= xPos + 1; i++)
                            {
                                for (int j = yPos - 1; j <= yPos + 1; j++)
                                {
                                    Vector2 tile = new Vector2(i, j);
                                    if (!loc.terrainFeatures.ContainsKey(tile) || !(loc.terrainFeatures[tile] is HoeDirt hDirt) ||
                                        hDirt?.crop?.indexOfHarvest == currentCrop.indexOfHarvest)
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
                                    if (!(loc.terrainFeatures[tile] is null) && loc.terrainFeatures[tile] is HoeDirt hDirt)
                                        hDirt.crop = null;
                                }
                            }

                            if (loc is Farm f)
                                f.resourceClumps.Add(new GiantCrop(currentCrop.indexOfHarvest.Value, new Vector2(xPos - 1, yPos - 1)));

                        }
                    }
                }
                //process some edge cases for non watered crops.
                if (currentCrop.fullyGrown.Value && currentCrop.dayOfCurrentPhase.Value > 0 ||
                    currentCrop.currentPhase.Value < currentCrop.phaseDays.Count - 1 ||
                    !currentCrop.isWildSeedCrop())

                    return; //stop processing

                //replace wild crops**

                //remove any object here. o.O
                loc.objects.Remove(position);

                string season = Game1.currentSeason;
                switch (currentCrop.whichForageCrop.Value)
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
                loc.objects.Add(position, new SObject(position, currentCrop.getRandomWildCropForSeason(season), 1)
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
                    Vector2 randomVector = new Vector2(xTile, yTile);
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

        public static Dictionary<int,int> GetFishListing(GameLocation loc)
        {
            var fish = new Dictionary<int, int>();
            Dictionary<string, string> locationListing = Game1.content.Load<Dictionary<string, string>>("Data\\Locations");
            string key = loc.Name;

            if (locationListing.ContainsKey(key))
            {
                string[] rawData = locationListing[key].Split('/')[4 + Utility.getSeasonNumber(Game1.currentSeason)].Split(' ');

                Dictionary<string, string> processedData = new Dictionary<string, string>();
                if (rawData.Length > 1)
                {
                    for (int index = 0; index < rawData.Length; index += 2)
                        processedData.Add(rawData[index], rawData[index + 1]);
                }
                string[] locationFish = processedData.Keys.ToArray<string>();

                Dictionary<int, string> fishData = Game1.content.Load<Dictionary<int, string>>("Data\\Fish");
                Utility.Shuffle<string>(Game1.random, locationFish);
                for (int index1 = 0; index1 < locationFish.Length; ++index1)
                {
                    //iterate through the fish
                    bool isValid = true;
                    //get the fish data
                    string[] fishParsed = fishData[Convert.ToInt32(locationFish[index1])].Split('/');
                    int zoneData = Convert.ToInt32(processedData[locationFish[index1]]);

                    //check time requirements
                    string[] timeSpawned = fishParsed[5].Split(' ');
                    for (int index2 = 0; index2 < timeSpawned.Length; index2 += 2)
                    {
                        if (Game1.timeOfDay < Convert.ToInt32(timeSpawned[index2]) && Game1.timeOfDay >= Convert.ToInt32(timeSpawned[index2 + 1]))
                        {
                            isValid = false;
                            break;
                        }
                    }
                    
                    //check weather requirements
                    if (!fishParsed[7].Equals("both"))
                    {
                        if (fishParsed[7].Equals("rainy") && !Game1.isRaining)
                            isValid = false;
                        else if (fishParsed[7].Equals("sunny") && Game1.isRaining)
                            isValid = false;
                    }

                    if (isValid)
                        fish.Add(Convert.ToInt32(locationFish[index1]), zoneData);

                }
            }
            return fish;
        }

        public static StardewValley.Object GetRandomFish(GameLocation loc)
        {
            int parentSheetIndex = 372;
            Dictionary<string, string> locationListing = Game1.content.Load<Dictionary<string, string>>("Data\\Locations");
            string key = loc.Name;

            if (locationListing.ContainsKey(key))
            {
                string[] locationData = locationListing[key].Split('/')[4 + Utility.getSeasonNumber(Game1.currentSeason)].Split(' ');
                Dictionary<string, string> dictionary2 = new Dictionary<string, string>();
                if (locationData.Length > 1)
                {
                    for (int index = 0; index < locationData.Length; index += 2)
                        dictionary2.Add(locationData[index], locationData[index + 1]);
                }

                string[] array = dictionary2.Keys.ToArray<string>();
                Utility.Shuffle<string>(Game1.random, array);
                Dictionary<int, string> dictionary3 = Game1.content.Load<Dictionary<int, string>>("Data\\Fish");
                for (int index1 = 0; index1 < array.Length; ++index1)
                {
                    bool flag2 = true;
                    string[] strArray2 = dictionary3[Convert.ToInt32(array[index1])].Split('/');
                    string[] strArray3 = strArray2[5].Split(' ');
                    int int32 = Convert.ToInt32(dictionary2[array[index1]]);
                    if (int32 == -1)
                    {
                        for (int index2 = 0; index2 < strArray3.Length; index2 += 2)
                        {
                            if (Game1.timeOfDay >= Convert.ToInt32(strArray3[index2]) && Game1.timeOfDay < Convert.ToInt32(strArray3[index2 + 1]))
                            {
                                flag2 = false;
                                break;
                            }
                        }
                    }
                    if (!strArray2[7].Equals("both"))
                    {
                        if (strArray2[7].Equals("rainy") && !Game1.isRaining)
                            flag2 = true;
                        else if (strArray2[7].Equals("sunny") && Game1.isRaining)
                            flag2 = true;
                    }
                    
                    if (!flag2)
                    {
                        parentSheetIndex = Convert.ToInt32(array[index1]);
                    }
                }
                return new StardewValley.Object(parentSheetIndex, 1, false, -1, 0);
            }

            return new StardewValley.Object(parentSheetIndex, 1, false, -1, 0);
        }
    }
}
