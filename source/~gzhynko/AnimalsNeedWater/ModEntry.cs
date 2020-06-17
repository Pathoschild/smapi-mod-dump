using Harmony;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Buildings;
using System;
using System.Collections.Generic;
using System.Linq;
using xTile.Layers;
using xTile.Tiles;

namespace AnimalsNeedWater
{
    /// <summary> The mod config class. More info here: https://stardewvalleywiki.com/Modding:Modder_Guide/APIs/Config </summary>
    public class ModConfig
    {
        // whether to show "love" bubbles over animals inside the building when watered the trough
        public bool ShowLoveBubblesOverAnimalsWhenWateredTrough { get; set; } = true;

        // whether to enable the watering system in Deluxe Coops and Deluxe Barns
        public bool WateringSystemInDeluxeBuildings { get; set; } = true;

        // whether to replace coop's and big coop's textures when troughs inside them are empty
        public bool ReplaceCoopTextureIfTroughIsEmpty { get; set; } = true;

        // the amount of friendship points player gets for watering the trough
        public int FriendshipPointsForWateredTrough { get; set; } = 15;

        // the amount of friendship points player gets for watering the trough with animals inside the building
        public int AdditionalFriendshipPointsForWateredTroughWithAnimalsInsideBuilding { get; set; } = 15;

        // the amount of friendship points player loses for not watering the trough
        public int NegativeFriendshipPointsForNotWateredTrough { get; set; } = 20;

        // whether animals can drink outside
        public bool AnimalsCanDrinkOutside { get; set; } = true;

        // whether animals can only drink from lakes/rivers/seas etc. If set to false, animals will drink from any place you can refill your watering can at (well, troughs, water bodies etc.)
        public bool AnimalsCanOnlyDrinkFromWaterBodies { get; set; } = true;
    }

    /// <summary> The mod entry class loaded by SMAPI. </summary>
    public class ModEntry : Mod
    {
        public static IMonitor ModMonitor;
        public static IModHelper ModHelper;
        public static ModEntry instance;
        public ModConfig Config;
        public Profile CurrentTroughPlacementProfile;
        public List<AnimalLeftThirsty> AnimalsLeftThirstyYesterday;

        /*********
        ** Public methods
        *********/

        /// <summary> The mod entry point, called after the mod is first loaded. </summary>
        /// <param name="helper"> Provides simplified APIs for writing mods. </param>
        public override void Entry(IModHelper helper)
        {
            ModHelper = helper;
            ModMonitor = Monitor;
            instance = this;

            AnimalsLeftThirstyYesterday = new List<AnimalLeftThirsty>();

            this.Config = Helper.ReadConfig<ModConfig>();
            helper.Events.GameLoop.GameLaunched += OnGameLaunched;
            helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
            helper.Events.GameLoop.DayEnding += HandleDayUpdate;

            DetermineTroughPlacementProfile();
        }

        /// <summary> Get ANW's API </summary>
        /// <returns> API instance </returns>
        public override object GetApi()
        {
            return new API();
        }

        /// <summary> Look for known mods that modify coop's/barn's interiors and load corresponding profiles. </summary>
        public void DetermineTroughPlacementProfile()
        {
            if (Helper.ModRegistry.IsLoaded("AairTheGreat.MoreBarnCoopAnimals"))
            {
                CurrentTroughPlacementProfile = Profiles.MoreBarnAndCoopAnimalsByAair;
                Monitor.Log("Loading trough placement profile for More Barn and Coop Animals mod by AairTheGreat.", LogLevel.Debug);
            } 
            else if (Helper.ModRegistry.IsLoaded("Froststar11.CleanBarnsCoops"))
            {
                CurrentTroughPlacementProfile = Profiles.CleanerBarnsAndCoopsByFroststar11;
                Monitor.Log("Loading trough placement profile for Froststar11's Cleaner Barns & Coops mod.", LogLevel.Debug);
            }
            else if (Helper.ModRegistry.IsLoaded("DaisyNiko.CCBB"))
            {
                CurrentTroughPlacementProfile = Profiles.CuterCoopsAndBetterBarnsByDaisyNiko;
                Monitor.Log("Loading trough placement profile for Cuter Coops and Better Barns mod by DaisyNiko.", LogLevel.Debug);
            }
            else if (Helper.ModRegistry.IsLoaded("nykachu.coopbarnfacelift"))
            {
                CurrentTroughPlacementProfile = Profiles.CoopAndBarnFaceliftByNykachu;
                Monitor.Log("Loading trough placement profile for Coop and Barn Facelift mod by nykachu.", LogLevel.Debug);
            }
            else if (Helper.ModRegistry.IsLoaded("pepoluan.cleanblockbarncoop"))
            {
                CurrentTroughPlacementProfile = Profiles.CleanAndBlockForBarnsAndCoopsByPepoluan;
                Monitor.Log("Loading trough placement profile for Clean and Block for Barns and Coops mod by pepoluan.", LogLevel.Debug);
            }
            else
            {
                CurrentTroughPlacementProfile = Profiles.Default;
                Monitor.Log("No known mods that affect trough placement in Barns and Coops found loaded. Loading the default trough placement profile.", LogLevel.Debug);
            }
        }

        /// <summary> Empty water troughs in animal houses. </summary>
        public void EmptyWaterTroughs()
        {
            ModData.BarnsWithWateredTrough = new List<string>();
            ModData.CoopsWithWateredTrough = new List<string>();

            foreach (Building building in Game1.getFarm().buildings)
            {
                int animalCount = 0;

                foreach (FarmAnimal animal in Game1.getFarm().getAllFarmAnimals())
                {
                    if (animal.home.nameOfIndoors.ToLower().Equals(building.nameOfIndoors.ToLower())) animalCount++;
                }

                if(building.nameOfIndoorsWithoutUnique.ToLower().Contains("3") && Config.WateringSystemInDeluxeBuildings)
                {
                    if (building.nameOfIndoorsWithoutUnique.ToLower() == "barn3") 
                    { 
                        if (!ModData.BarnsWithWateredTrough.Contains(building.nameOfIndoors.ToLower()))
                            ModData.BarnsWithWateredTrough.Add(building.nameOfIndoors.ToLower());
                    }
                    else if (building.nameOfIndoorsWithoutUnique.ToLower() == "coop3")
                    {
                        if (!ModData.CoopsWithWateredTrough.Contains(building.nameOfIndoors.ToLower()))
                            ModData.CoopsWithWateredTrough.Add(building.nameOfIndoors.ToLower());
                    }
                    continue;
                }

                if (building.nameOfIndoorsWithoutUnique.ToLower().Contains("coop") && animalCount > 0)
                {
                    if (building.nameOfIndoorsWithoutUnique.ToLower() == "coop")
                    {
                        GameLocation gameLocation = building.indoors.Value;

                        ChangeCoopTexture(building, true);

                        foreach (TroughTile tile in CurrentTroughPlacementProfile.coopTroughTiles)
                        {
                            gameLocation.removeTile(tile.TileX, tile.TileY, tile.Layer);
                        }

                        Layer buildingsLayer = gameLocation.map.GetLayer("Buildings");
                        Layer frontLayer = gameLocation.map.GetLayer("Front");
                        TileSheet tilesheet = gameLocation.map.GetTileSheet("z_waterTroughTilesheet");

                        foreach (TroughTile tile in CurrentTroughPlacementProfile.coopTroughTiles)
                        {
                            if (tile.Layer.Equals("Buildings"))
                                buildingsLayer.Tiles[tile.TileX, tile.TileY] = new StaticTile(buildingsLayer, tilesheet, BlendMode.Alpha, tileIndex: tile.EmptyTroughTilesheetIndex);
                            else if (tile.Layer.Equals("Front"))
                                frontLayer.Tiles[tile.TileX, tile.TileY] = new StaticTile(frontLayer, tilesheet, BlendMode.Alpha, tileIndex: tile.EmptyTroughTilesheetIndex);
                        }
                    }
                    else if (building.nameOfIndoorsWithoutUnique.ToLower() == "coop2")
                    {
                        GameLocation gameLocation = building.indoors.Value;

                        ChangeBigCoopTexture(building, true);

                        foreach (TroughTile tile in CurrentTroughPlacementProfile.coop2TroughTiles)
                        {
                            gameLocation.removeTile(tile.TileX, tile.TileY, tile.Layer);
                        }

                        Layer buildingsLayer = gameLocation.map.GetLayer("Buildings");
                        Layer frontLayer = gameLocation.map.GetLayer("Front");
                        TileSheet tilesheet = gameLocation.map.GetTileSheet("z_waterTroughTilesheet");

                        foreach (TroughTile tile in CurrentTroughPlacementProfile.coop2TroughTiles)
                        {
                            if (tile.Layer.Equals("Buildings"))
                                buildingsLayer.Tiles[tile.TileX, tile.TileY] = new StaticTile(buildingsLayer, tilesheet, BlendMode.Alpha, tileIndex: tile.EmptyTroughTilesheetIndex);
                            else if (tile.Layer.Equals("Front"))
                                frontLayer.Tiles[tile.TileX, tile.TileY] = new StaticTile(frontLayer, tilesheet, BlendMode.Alpha, tileIndex: tile.EmptyTroughTilesheetIndex);
                        }
                    }
                    else if (building.nameOfIndoorsWithoutUnique.ToLower() == "coop3")
                    {
                        GameLocation gameLocation = building.indoors.Value;

                        foreach (TroughTile tile in CurrentTroughPlacementProfile.coop3TroughTiles)
                        {
                            gameLocation.removeTile(tile.TileX, tile.TileY, tile.Layer);
                        }

                        Layer buildingsLayer = gameLocation.map.GetLayer("Buildings");
                        Layer frontLayer = gameLocation.map.GetLayer("Front");
                        TileSheet tilesheet = gameLocation.map.GetTileSheet("z_waterTroughTilesheet");

                        foreach (TroughTile tile in CurrentTroughPlacementProfile.coop3TroughTiles)
                        {
                            if (tile.Layer.Equals("Buildings"))
                                buildingsLayer.Tiles[tile.TileX, tile.TileY] = new StaticTile(buildingsLayer, tilesheet, BlendMode.Alpha, tileIndex: tile.EmptyTroughTilesheetIndex);
                            else if (tile.Layer.Equals("Front"))
                                frontLayer.Tiles[tile.TileX, tile.TileY] = new StaticTile(frontLayer, tilesheet, BlendMode.Alpha, tileIndex: tile.EmptyTroughTilesheetIndex);
                        }
                    }
                }
                else if (building.nameOfIndoorsWithoutUnique.ToLower().Contains("barn") && animalCount > 0)
                {
                    if (building.nameOfIndoorsWithoutUnique.ToLower() == "barn")
                    {
                        GameLocation gameLocation = building.indoors.Value;

                        foreach (TroughTile tile in CurrentTroughPlacementProfile.barnTroughTiles)
                        {
                            gameLocation.removeTile(tile.TileX, tile.TileY, tile.Layer);
                        }

                        Layer buildingsLayer = gameLocation.map.GetLayer("Buildings");
                        Layer frontLayer = gameLocation.map.GetLayer("Front");
                        TileSheet tilesheet = gameLocation.map.GetTileSheet("z_waterTroughTilesheet");

                        foreach (TroughTile tile in CurrentTroughPlacementProfile.barnTroughTiles)
                        {
                            if (tile.Layer.Equals("Buildings"))
                                buildingsLayer.Tiles[tile.TileX, tile.TileY] = new StaticTile(buildingsLayer, tilesheet, BlendMode.Alpha, tileIndex: tile.EmptyTroughTilesheetIndex);
                            else if (tile.Layer.Equals("Front"))
                                frontLayer.Tiles[tile.TileX, tile.TileY] = new StaticTile(frontLayer, tilesheet, BlendMode.Alpha, tileIndex: tile.EmptyTroughTilesheetIndex);
                        }
                    }
                    else if (building.nameOfIndoorsWithoutUnique.ToLower() == "barn2")
                    {
                        GameLocation gameLocation = building.indoors.Value;

                        foreach (TroughTile tile in CurrentTroughPlacementProfile.barn2TroughTiles)
                        {
                            gameLocation.removeTile(tile.TileX, tile.TileY, tile.Layer);
                        }

                        Layer buildingsLayer = gameLocation.map.GetLayer("Buildings");
                        Layer frontLayer = gameLocation.map.GetLayer("Front");
                        TileSheet tilesheet = gameLocation.map.GetTileSheet("z_waterTroughTilesheet");

                        foreach (TroughTile tile in CurrentTroughPlacementProfile.barn2TroughTiles)
                        {
                            if (tile.Layer.Equals("Buildings"))
                                buildingsLayer.Tiles[tile.TileX, tile.TileY] = new StaticTile(buildingsLayer, tilesheet, BlendMode.Alpha, tileIndex: tile.EmptyTroughTilesheetIndex);
                            else if (tile.Layer.Equals("Front"))
                                frontLayer.Tiles[tile.TileX, tile.TileY] = new StaticTile(frontLayer, tilesheet, BlendMode.Alpha, tileIndex: tile.EmptyTroughTilesheetIndex);
                        }
                    }
                    else if (building.nameOfIndoorsWithoutUnique.ToLower() == "barn3")
                    {
                        GameLocation gameLocation = building.indoors.Value;

                        foreach (TroughTile tile in CurrentTroughPlacementProfile.barn3TroughTiles)
                        {
                            gameLocation.removeTile(tile.TileX, tile.TileY, tile.Layer);
                        }

                        Layer buildingsLayer = gameLocation.map.GetLayer("Buildings");
                        Layer frontLayer = gameLocation.map.GetLayer("Front");
                        TileSheet tilesheet = gameLocation.map.GetTileSheet("z_waterTroughTilesheet");

                        foreach (TroughTile tile in CurrentTroughPlacementProfile.barn3TroughTiles)
                        {
                            if (tile.Layer.Equals("Buildings"))
                                buildingsLayer.Tiles[tile.TileX, tile.TileY] = new StaticTile(buildingsLayer, tilesheet, BlendMode.Alpha, tileIndex: tile.EmptyTroughTilesheetIndex);
                            else if (tile.Layer.Equals("Front"))
                                frontLayer.Tiles[tile.TileX, tile.TileY] = new StaticTile(frontLayer, tilesheet, BlendMode.Alpha, tileIndex: tile.EmptyTroughTilesheetIndex);
                        }
                    }
                }
                else if (animalCount == 0)
                {
                    if (building.nameOfIndoorsWithoutUnique.ToLower().Contains("coop"))
                    {
                        ModData.CoopsWithWateredTrough.Add(building.nameOfIndoors.ToLower());
                    }
                    else if (building.nameOfIndoorsWithoutUnique.ToLower().Contains("barn"))
                    {
                        ModData.BarnsWithWateredTrough.Add(building.nameOfIndoors.ToLower());
                    }
                }
            }
        }

        /*********
        ** Private methods
        *********/

        /// <summary> Looks for animals left thirsty and notifies player of them. </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void HandleDayUpdate(object sender, DayEndingEventArgs e)
        {
            List<AnimalLeftThirsty> animalsLeftThirsty = new List<AnimalLeftThirsty>();

            foreach (Building building in Game1.getFarm().buildings)
            {
                if (building.nameOfIndoors.ToLower().Contains("coop"))
                {
                    foreach (FarmAnimal animal in ((AnimalHouse)building.indoors.Value).animals.Values)
                    {
                        if (ModData.CoopsWithWateredTrough.Contains(animal.home.nameOfIndoors.ToLower()) == false && ModData.FullAnimals.Contains(((Character)animal).displayName) == false)
                        {
                            if (!animalsLeftThirsty.Any(item => item.DisplayName == animal.displayName))
                            {
                                animal.friendshipTowardFarmer.Value -= Math.Abs(Config.NegativeFriendshipPointsForNotWateredTrough);
                                animalsLeftThirsty.Add(new AnimalLeftThirsty(animal.displayName, (animal.isMale() ? "male" : "female")));
                            }
                        }
                    }
                } 
                else if (building.nameOfIndoors.ToLower().Contains("barn"))
                {
                    foreach(FarmAnimal animal in ((AnimalHouse)building.indoors.Value).animals.Values)
                    {
                        if (ModData.BarnsWithWateredTrough.Contains(animal.home.nameOfIndoors.ToLower()) == false && ModData.FullAnimals.Contains(((Character)animal).displayName) == false)
                        {
                            if (!animalsLeftThirsty.Any(item => item.DisplayName == animal.displayName))
                            {
                                animal.friendshipTowardFarmer.Value -= Math.Abs(Config.NegativeFriendshipPointsForNotWateredTrough);
                                animalsLeftThirsty.Add(new AnimalLeftThirsty(animal.displayName, (animal.isMale() ? "male" : "female")));
                            }
                        }
                    }
                }
            }

            // Check for animals outside their buildings
            foreach (FarmAnimal animal in Game1.getFarm().animals.Values)
            {
                if (animal.home.nameOfIndoorsWithoutUnique.ToLower().Contains("coop"))
                {
                    if ((ModData.CoopsWithWateredTrough.Contains(animal.home.nameOfIndoors.ToLower()) == false && ModData.FullAnimals.Contains(((Character)animal).displayName) == false) || animal.home.animalDoorOpen.Value == false)
                    {
                        if (!animalsLeftThirsty.Any(item => item.DisplayName == animal.displayName))
                        {
                            animal.friendshipTowardFarmer.Value -= Math.Abs(Config.NegativeFriendshipPointsForNotWateredTrough);
                            animalsLeftThirsty.Add(new AnimalLeftThirsty(animal.displayName, (animal.isMale() ? "male" : "female")));
                        }
                    }
                } 
                else if(animal.home.nameOfIndoorsWithoutUnique.ToLower().Contains("barn"))
                {
                    if ((ModData.BarnsWithWateredTrough.Contains(animal.home.nameOfIndoors.ToLower()) == false && ModData.FullAnimals.Contains(((Character)animal).displayName) == false) || animal.home.animalDoorOpen.Value == false)
                    {
                        if (!animalsLeftThirsty.Any(item => item.DisplayName == animal.displayName))
                        {
                            animal.friendshipTowardFarmer.Value -= Math.Abs(Config.NegativeFriendshipPointsForNotWateredTrough);
                            animalsLeftThirsty.Add(new AnimalLeftThirsty(animal.displayName, (animal.isMale() ? "male" : "female")));
                        }
                    }
                }
            }

            if (animalsLeftThirsty.Count() > 0)
            {                
                if (animalsLeftThirsty.Count() == 1)
                {
                    if (animalsLeftThirsty[0].Gender == "male")
                    {
                        Game1.showGlobalMessage(ModHelper.Translation.Get("AnimalsLeftWithoutWaterYesterday.globalMessage.oneAnimal_Male", new { firstAnimalName = animalsLeftThirsty[0].DisplayName }));
                    } 
                    else if (animalsLeftThirsty[0].Gender == "female")
                    {
                        Game1.showGlobalMessage(ModHelper.Translation.Get("AnimalsLeftWithoutWaterYesterday.globalMessage.oneAnimal_Female", new { firstAnimalName = animalsLeftThirsty[0].DisplayName }));
                    }
                    else 
                    {
                        Game1.showGlobalMessage(ModHelper.Translation.Get("AnimalsLeftWithoutWaterYesterday.globalMessage.oneAnimal_UnknownGender", new { firstAnimalName = animalsLeftThirsty[0].DisplayName }));
                    }
                }
                else if (animalsLeftThirsty.Count() == 2)
                {
                    Game1.showGlobalMessage(ModHelper.Translation.Get("AnimalsLeftWithoutWaterYesterday.globalMessage.twoAnimals", new { firstAnimalName = animalsLeftThirsty[0].DisplayName, secondAnimalName = animalsLeftThirsty[1].DisplayName }));
                }
                else if (animalsLeftThirsty.Count() == 3)
                {
                    Game1.showGlobalMessage(ModHelper.Translation.Get("AnimalsLeftWithoutWaterYesterday.globalMessage.threeAnimals", new { firstAnimalName = animalsLeftThirsty[0].DisplayName, secondAnimalName = animalsLeftThirsty[1].DisplayName, thirdAnimalName = animalsLeftThirsty[2].DisplayName }));
                }
                else
                {
                    Game1.showGlobalMessage(ModHelper.Translation.Get("AnimalsLeftWithoutWaterYesterday.globalMessage.multipleAnimals", new { firstAnimalName = animalsLeftThirsty[0].DisplayName, secondAnimalName = animalsLeftThirsty[1].DisplayName, thirdAnimalName = animalsLeftThirsty[2].DisplayName, totalAmountExcludingFirstThree = animalsLeftThirsty.Count() - 3 }));
                }
            }

            AnimalsLeftThirstyYesterday = animalsLeftThirsty;

            ModData.FullAnimals = new List<string>();

            List<object> nextDayAndSeasonList = GetNextDayAndSeason(Game1.dayOfMonth, Game1.currentSeason);

            if (!Utility.isFestivalDay((int)nextDayAndSeasonList[0], (string)nextDayAndSeasonList[1]))
            {
                EmptyWaterTroughs();
            }
            else
            {
                foreach (Building building in Game1.getFarm().buildings)
                {
                    if (building.nameOfIndoorsWithoutUnique.ToLower().Contains("coop"))
                    {
                        ModData.CoopsWithWateredTrough.Add(building.nameOfIndoors.ToLower());
                    }
                    else if (building.nameOfIndoorsWithoutUnique.ToLower().Contains("barn"))
                    {
                        ModData.BarnsWithWateredTrough.Add(building.nameOfIndoors.ToLower());
                    }
                }
            }
        }

        /// <summary> Raised after the game is launched, right before the first update tick. This happens once per game session (unrelated to loading saves). All mods are loaded and initialised at this point, so this is a good time to set up mod integrations. </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            var harmony = HarmonyInstance.Create("GZhynko.AnimalsNeedWater");

            harmony.Patch(
                original: AccessTools.Method(typeof(AnimalHouse), nameof(AnimalHouse.performToolAction)),
                prefix: new HarmonyMethod(typeof(Overrides), nameof(Overrides.AnimalHouseToolAction))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(FarmAnimal), nameof(FarmAnimal.dayUpdate)),
                prefix: new HarmonyMethod(typeof(Overrides), nameof(Overrides.AnimalDayUpdate))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(FarmAnimal), "behaviors", new Type[] {
                    typeof(GameTime),
                    typeof(GameLocation)
                }),
                prefix: new HarmonyMethod(typeof(Overrides), nameof(Overrides.AnimalBehaviors))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(Game1), nameof(Game1.warpFarmer), new Type[] {
                    typeof(string),
                    typeof(int),
                    typeof(int),
                    typeof(int),
                    typeof(bool)
                }),
                prefix: new HarmonyMethod(typeof(Overrides), nameof(Overrides.WarpFarmer))
            );
        }

        /// <summary> Raised after the save is loaded. </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            foreach (Building building in Game1.getFarm().buildings)
            {
                if (building.nameOfIndoorsWithoutUnique.ToLower().Contains("coop"))
                {
                    var coopMap = ((GameLocation)building.indoors.Value).map;

                    TileSheet tileSheet = new TileSheet(
                      id: "z_waterTroughTilesheet", 
                      map: coopMap,
                      imageSource: ModEntry.instance.Helper.Content.GetActualAssetKey("assets/waterTroughTilesheet.xnb", ContentSource.ModFolder),
                      sheetSize: new xTile.Dimensions.Size(160, 16), 
                      tileSize: new xTile.Dimensions.Size(16, 16)
                   );

                    coopMap.AddTileSheet(tileSheet);
                    coopMap.LoadTileSheets(Game1.mapDisplayDevice);

                    if (building.nameOfIndoorsWithoutUnique.ToLower() == "coop3" && Config.WateringSystemInDeluxeBuildings)
                    {
                        var coop3Map = ((GameLocation)building.indoors.Value).map;
                        var gameLocation = building.indoors.Value;

                        TileSheet tileSheet3 = new TileSheet(
                            id: "z_wateringSystemTilesheet",
                            map: coop3Map,
                            imageSource: ModEntry.instance.Helper.Content.GetActualAssetKey("assets/wateringSystemTilesheet.xnb", ContentSource.ModFolder),
                            sheetSize: new xTile.Dimensions.Size(48, 16),
                            tileSize: new xTile.Dimensions.Size(16, 16)
                        );

                        coop3Map.AddTileSheet(tileSheet3);
                        coop3Map.LoadTileSheets(Game1.mapDisplayDevice);

                        foreach (SimplifiedTile tile in CurrentTroughPlacementProfile.coop3WateringSystem.TilesToRemove)
                        {
                            gameLocation.removeTile(tile.TileX, tile.TileY, tile.Layer);
                        }

                        Layer buildingsLayer = gameLocation.map.GetLayer("Buildings");
                        Layer frontLayer = gameLocation.map.GetLayer("Front");
                        TileSheet tilesheet = gameLocation.map.GetTileSheet("z_wateringSystemTilesheet");

                        if (CurrentTroughPlacementProfile.coop3WateringSystem.Layer.Equals("Buildings"))
                            buildingsLayer.Tiles[CurrentTroughPlacementProfile.coop3WateringSystem.TileX, CurrentTroughPlacementProfile.coop3WateringSystem.TileY] = new StaticTile(buildingsLayer, tilesheet, BlendMode.Alpha, tileIndex: CurrentTroughPlacementProfile.coop3WateringSystem.SystemTilesheetIndex);
                        else if (CurrentTroughPlacementProfile.coop3WateringSystem.Layer.Equals("Front"))
                            frontLayer.Tiles[CurrentTroughPlacementProfile.coop3WateringSystem.TileX, CurrentTroughPlacementProfile.coop3WateringSystem.TileY] = new StaticTile(frontLayer, tilesheet, BlendMode.Alpha, tileIndex: CurrentTroughPlacementProfile.coop3WateringSystem.SystemTilesheetIndex);
                    }
                } 
                else if(building.nameOfIndoorsWithoutUnique.ToLower().Contains("barn"))
                {
                    var barnMap = ((GameLocation)building.indoors.Value).map;

                    TileSheet tileSheet = new TileSheet(
                        id: "z_waterTroughTilesheet",
                        map: barnMap,
                        imageSource: ModEntry.instance.Helper.Content.GetActualAssetKey("assets/waterTroughTilesheet.xnb", ContentSource.ModFolder),
                        sheetSize: new xTile.Dimensions.Size(160, 16),
                        tileSize: new xTile.Dimensions.Size(16, 16)
                    );

                    barnMap.AddTileSheet(tileSheet);
                    barnMap.LoadTileSheets(Game1.mapDisplayDevice);

                    if(building.nameOfIndoorsWithoutUnique.ToLower() == "barn3" && Config.WateringSystemInDeluxeBuildings)
                    {
                        var barn3Map = ((GameLocation)building.indoors.Value).map;
                        var gameLocation = building.indoors.Value;

                        TileSheet tileSheet3 = new TileSheet(
                            id: "z_wateringSystemTilesheet",
                            map: barn3Map,
                            imageSource: ModEntry.instance.Helper.Content.GetActualAssetKey("assets/wateringSystemTilesheet.xnb", ContentSource.ModFolder),
                            sheetSize: new xTile.Dimensions.Size(48, 16),
                            tileSize: new xTile.Dimensions.Size(16, 16)
                        );

                        barn3Map.AddTileSheet(tileSheet3);
                        barn3Map.LoadTileSheets(Game1.mapDisplayDevice);

                        foreach (SimplifiedTile tile in CurrentTroughPlacementProfile.barn3WateringSystem.TilesToRemove)
                        {
                            gameLocation.removeTile(tile.TileX, tile.TileY, tile.Layer);
                        }

                        Layer buildingsLayer = gameLocation.map.GetLayer("Buildings");
                        Layer frontLayer = gameLocation.map.GetLayer("Front");
                        TileSheet tilesheet = gameLocation.map.GetTileSheet("z_wateringSystemTilesheet");

                        if (CurrentTroughPlacementProfile.barn3WateringSystem.Layer.Equals("Buildings"))
                            buildingsLayer.Tiles[CurrentTroughPlacementProfile.barn3WateringSystem.TileX, CurrentTroughPlacementProfile.barn3WateringSystem.TileY] = new StaticTile(buildingsLayer, tilesheet, BlendMode.Alpha, tileIndex: CurrentTroughPlacementProfile.barn3WateringSystem.SystemTilesheetIndex);
                        else if (CurrentTroughPlacementProfile.barn3WateringSystem.Layer.Equals("Front"))
                            frontLayer.Tiles[CurrentTroughPlacementProfile.barn3WateringSystem.TileX, CurrentTroughPlacementProfile.barn3WateringSystem.TileY] = new StaticTile(frontLayer, tilesheet, BlendMode.Alpha, tileIndex: CurrentTroughPlacementProfile.barn3WateringSystem.SystemTilesheetIndex);
                    }
                }
            }

            HandleDayStart();
        }

        private void HandleDayStart()
        {
            if (!Utility.isFestivalDay(Game1.dayOfMonth, Game1.currentSeason))
            {
                EmptyWaterTroughs();
            } 
            else
            {
                foreach (Building building in Game1.getFarm().buildings)
                {
                    if (building.nameOfIndoorsWithoutUnique.ToLower().Contains("coop"))
                    {
                        ModData.CoopsWithWateredTrough.Add(building.nameOfIndoors.ToLower());

                        ChangeCoopTexture(building, false);
                        ChangeBigCoopTexture(building, false);
                    }
                    else if (building.nameOfIndoorsWithoutUnique.ToLower().Contains("barn"))
                    {
                        ModData.BarnsWithWateredTrough.Add(building.nameOfIndoors.ToLower());
                    }
                }
            }
        }

        /*********
        ** Utilities 
        *********/

        #region Utils

        public List<object> GetNextDayAndSeason(int currDay, string currSeason)
        {
            if (currDay + 1 <= 28)
            {
                List<object> returnList = new List<object>
                {
                    currDay + 1,
                    currSeason
                };
                return returnList;
            }
            else
            {
                List<object> returnList = new List<object>
                {
                    1,
                    NextSeason(currSeason)
                };
                return returnList;
            }
        }

        public void ChangeCoopTexture(Building building, bool empty)
        {
            if (Config.ReplaceCoopTextureIfTroughIsEmpty)
            {
                if (empty)
                {
                    building.texture = new Lazy<Texture2D>(() => Helper.Content.Load<Texture2D>("assets/Coop_emptyWaterTrough.png", ContentSource.ModFolder));
                }
                else
                {
                    building.texture = new Lazy<Texture2D>(() => Helper.Content.Load<Texture2D>("assets/Coop_fullWaterTrough.png", ContentSource.ModFolder));
                }
            }
        }

        public void ChangeBigCoopTexture(Building building, bool empty)
        {
            if (Config.ReplaceCoopTextureIfTroughIsEmpty)
            {
                if (empty)
                {
                    building.texture = new Lazy<Texture2D>(() => Helper.Content.Load<Texture2D>("assets/Coop2_emptyWaterTrough.png", ContentSource.ModFolder));
                }
                else
                {
                    building.texture = new Lazy<Texture2D>(() => Helper.Content.Load<Texture2D>("assets/Coop2_fullWaterTrough.png", ContentSource.ModFolder));
                }
            }
        }

        public string NextSeason(string season)
        {
            string newSeason = "";
            if (season == "spring") newSeason = "summer";
            if (season == "summer") newSeason = "fall";
            if (season == "fall") newSeason = "winter";
            if (season == "winter") newSeason = "spring";

            return newSeason;
        }

        public class AnimalLeftThirsty
        {
            public AnimalLeftThirsty(string displayName, string gender)
            {
                this.DisplayName = displayName;
                this.Gender = gender;
            }

            public string DisplayName { get; set; }
            public string Gender { get; set; }
        }
        #endregion
    }
}
