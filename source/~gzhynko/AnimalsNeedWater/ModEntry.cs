/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/gzhynko/StardewMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using AnimalsNeedWater.Patching;
using AnimalsNeedWater.Types;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Buildings;
using xTile.Dimensions;
using xTile.Layers;
using xTile.Tiles;

namespace AnimalsNeedWater
{
    /// <summary> The mod entry class loaded by SMAPI. </summary>
    public class ModEntry : Mod
    {
        #region Variables

        public static IMonitor ModMonitor;
        public static IModHelper ModHelper;
        public static ModEntry Instance;
        public ModConfig Config;
        public Profile CurrentTroughPlacementProfile;
        public List<FarmAnimal> AnimalsLeftThirstyYesterday;
        
        #endregion
        #region Public methods

        /// <summary> The mod entry point, called after the mod is first loaded. </summary>
        /// <param name="helper"> Provides simplified APIs for writing mods. </param>
        public override void Entry(IModHelper helper)
        {
            ModHelper = helper;
            ModMonitor = Monitor;
            Instance = this;

            AnimalsLeftThirstyYesterday = new List<FarmAnimal>();

            Config = Helper.ReadConfig<ModConfig>();
            helper.Events.GameLoop.GameLaunched += OnGameLaunched;
            helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
            helper.Events.GameLoop.Saved += HandleDayUpdate;
            helper.Events.GameLoop.DayEnding += OnDayEnding;
            helper.Events.Multiplayer.ModMessageReceived += OnModMessageReceived;

            DetermineTroughPlacementProfile();
        }
        
        public void SaveConfig(ModConfig newConfig)
        {
            Config = newConfig;
            Helper.WriteConfig(newConfig);
        }

        /// <summary> Get ANW's API </summary>
        /// <returns> API instance </returns>
        public override object GetApi()
        {
            return new API();
        }
        
        public void SendTroughWateredMessage(Type buildingType, string building)
        {
            SendMessageToSelf(new TroughWateredMessage(buildingType, building), "TroughWateredMessage");
        }
        
        #endregion
        #region Private methods
        
        /// <summary> Look for known mods that modify coop/barn interiors and load corresponding profiles. </summary>
        private void DetermineTroughPlacementProfile()
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
                Monitor.Log("No known mods that affect trough placement in Barns and Coops found loaded. Loading the default trough placement profile.");
            }
        }

        /// <summary> Empty water troughs in animal houses. </summary>
        private void EmptyWaterTroughs()
        {
            ModData.BarnsWithWateredTrough = new List<string>();
            ModData.CoopsWithWateredTrough = new List<string>();

            foreach (Building building in Game1.getFarm().buildings)
            {
                // If the building is a deluxe one and the corresponding config option is set to true, 
                // avoid emptying troughs and mark it as watered.
                if(building.nameOfIndoorsWithoutUnique.ToLower().Contains("3") && Config.WateringSystemInDeluxeBuildings)
                {
                    switch (building.nameOfIndoorsWithoutUnique.ToLower())
                    {
                        case "barn3":
                        {
                            if (!ModData.BarnsWithWateredTrough.Contains(building.nameOfIndoors.ToLower()))
                                ModData.BarnsWithWateredTrough.Add(building.nameOfIndoors.ToLower());
                            break;
                        }
                        case "coop3":
                        {
                            if (!ModData.CoopsWithWateredTrough.Contains(building.nameOfIndoors.ToLower()))
                                ModData.CoopsWithWateredTrough.Add(building.nameOfIndoors.ToLower());
                            break;
                        }
                    }

                    continue;
                }
                
                EmptyWaterTroughsInBuilding(building);
            }
        }
        
        /// <summary>
        /// Empty water troughs in the specified animal house.
        /// </summary>
        /// <param name="building"></param>
        private void EmptyWaterTroughsInBuilding(Building building)
        {
            int animalCount = 0;
            GameLocation gameLocation = building.indoors.Value;

            foreach (FarmAnimal animal in Game1.getFarm().getAllFarmAnimals())
            {
                if (animal.home.nameOfIndoors.ToLower().Equals(building.nameOfIndoors.ToLower())) animalCount++;
            }

            if (building is Coop && animalCount > 0)
            {
                switch (building.nameOfIndoorsWithoutUnique.ToLower())
                {
                    case "coop":
                    {
                        ChangeCoopTexture(building, true);

                        foreach (TroughTile tile in CurrentTroughPlacementProfile.coopTroughTiles)
                        {
                            gameLocation.removeTile(tile.TileX, tile.TileY, tile.Layer);
                        }

                        Layer buildingsLayer = gameLocation.Map.GetLayer("Buildings");
                        Layer frontLayer = gameLocation.Map.GetLayer("Front");
                        TileSheet tilesheet = gameLocation.Map.GetTileSheet("z_waterTroughTilesheet");

                        foreach (TroughTile tile in CurrentTroughPlacementProfile.coopTroughTiles)
                        {
                            if (tile.Layer.Equals("Buildings"))
                                buildingsLayer.Tiles[tile.TileX, tile.TileY] = new StaticTile(buildingsLayer, tilesheet, BlendMode.Alpha, tileIndex: tile.EmptyTroughTilesheetIndex);
                            else if (tile.Layer.Equals("Front"))
                                frontLayer.Tiles[tile.TileX, tile.TileY] = new StaticTile(frontLayer, tilesheet, BlendMode.Alpha, tileIndex: tile.EmptyTroughTilesheetIndex);
                        }

                        break;
                    }
                    case "coop2":
                    {
                        ChangeBigCoopTexture(building, true);

                        foreach (TroughTile tile in CurrentTroughPlacementProfile.coop2TroughTiles)
                        {
                            gameLocation.removeTile(tile.TileX, tile.TileY, tile.Layer);
                        }

                        Layer buildingsLayer = gameLocation.Map.GetLayer("Buildings");
                        Layer frontLayer = gameLocation.Map.GetLayer("Front");
                        TileSheet tilesheet = gameLocation.Map.GetTileSheet("z_waterTroughTilesheet");

                        foreach (TroughTile tile in CurrentTroughPlacementProfile.coop2TroughTiles)
                        {
                            if (tile.Layer.Equals("Buildings"))
                                buildingsLayer.Tiles[tile.TileX, tile.TileY] = new StaticTile(buildingsLayer, tilesheet, BlendMode.Alpha, tileIndex: tile.EmptyTroughTilesheetIndex);
                            else if (tile.Layer.Equals("Front"))
                                frontLayer.Tiles[tile.TileX, tile.TileY] = new StaticTile(frontLayer, tilesheet, BlendMode.Alpha, tileIndex: tile.EmptyTroughTilesheetIndex);
                        }

                        break;
                    }
                    case "coop3":
                    {
                        foreach (TroughTile tile in CurrentTroughPlacementProfile.coop3TroughTiles)
                        {
                            gameLocation.removeTile(tile.TileX, tile.TileY, tile.Layer);
                        }

                        Layer buildingsLayer = gameLocation.Map.GetLayer("Buildings");
                        Layer frontLayer = gameLocation.Map.GetLayer("Front");
                        TileSheet tilesheet = gameLocation.Map.GetTileSheet("z_waterTroughTilesheet");

                        foreach (TroughTile tile in CurrentTroughPlacementProfile.coop3TroughTiles)
                        {
                            if (tile.Layer.Equals("Buildings"))
                                buildingsLayer.Tiles[tile.TileX, tile.TileY] = new StaticTile(buildingsLayer, tilesheet, BlendMode.Alpha, tileIndex: tile.EmptyTroughTilesheetIndex);
                            else if (tile.Layer.Equals("Front"))
                                frontLayer.Tiles[tile.TileX, tile.TileY] = new StaticTile(frontLayer, tilesheet, BlendMode.Alpha, tileIndex: tile.EmptyTroughTilesheetIndex);
                        }

                        break;
                    }
                }
            }
            else if (building is Barn && animalCount > 0)
            {
                switch (building.nameOfIndoorsWithoutUnique.ToLower())
                {
                    case "barn":
                    {
                        foreach (TroughTile tile in CurrentTroughPlacementProfile.barnTroughTiles)
                        {
                            gameLocation.removeTile(tile.TileX, tile.TileY, tile.Layer);
                        }

                        Layer buildingsLayer = gameLocation.Map.GetLayer("Buildings");
                        Layer frontLayer = gameLocation.Map.GetLayer("Front");
                        TileSheet tilesheet = gameLocation.Map.GetTileSheet("z_waterTroughTilesheet");

                        foreach (TroughTile tile in CurrentTroughPlacementProfile.barnTroughTiles)
                        {
                            if (tile.Layer.Equals("Buildings"))
                                buildingsLayer.Tiles[tile.TileX, tile.TileY] = new StaticTile(buildingsLayer, tilesheet, BlendMode.Alpha, tileIndex: tile.EmptyTroughTilesheetIndex);
                            else if (tile.Layer.Equals("Front"))
                                frontLayer.Tiles[tile.TileX, tile.TileY] = new StaticTile(frontLayer, tilesheet, BlendMode.Alpha, tileIndex: tile.EmptyTroughTilesheetIndex);
                        }

                        break;
                    }
                    case "barn2":
                    {
                        foreach (TroughTile tile in CurrentTroughPlacementProfile.barn2TroughTiles)
                        {
                            gameLocation.removeTile(tile.TileX, tile.TileY, tile.Layer);
                        }

                        Layer buildingsLayer = gameLocation.Map.GetLayer("Buildings");
                        Layer frontLayer = gameLocation.Map.GetLayer("Front");
                        TileSheet tilesheet = gameLocation.Map.GetTileSheet("z_waterTroughTilesheet");

                        foreach (TroughTile tile in CurrentTroughPlacementProfile.barn2TroughTiles)
                        {
                            if (tile.Layer.Equals("Buildings"))
                                buildingsLayer.Tiles[tile.TileX, tile.TileY] = new StaticTile(buildingsLayer, tilesheet, BlendMode.Alpha, tileIndex: tile.EmptyTroughTilesheetIndex);
                            else if (tile.Layer.Equals("Front"))
                                frontLayer.Tiles[tile.TileX, tile.TileY] = new StaticTile(frontLayer, tilesheet, BlendMode.Alpha, tileIndex: tile.EmptyTroughTilesheetIndex);
                        }

                        break;
                    }
                    case "barn3":
                    {
                        foreach (TroughTile tile in CurrentTroughPlacementProfile.barn3TroughTiles)
                        {
                            gameLocation.removeTile(tile.TileX, tile.TileY, tile.Layer);
                        }

                        Layer buildingsLayer = gameLocation.Map.GetLayer("Buildings");
                        Layer frontLayer = gameLocation.Map.GetLayer("Front");
                        TileSheet tilesheet = gameLocation.Map.GetTileSheet("z_waterTroughTilesheet");

                        foreach (TroughTile tile in CurrentTroughPlacementProfile.barn3TroughTiles)
                        {
                            if (tile.Layer.Equals("Buildings"))
                                buildingsLayer.Tiles[tile.TileX, tile.TileY] = new StaticTile(buildingsLayer, tilesheet, BlendMode.Alpha, tileIndex: tile.EmptyTroughTilesheetIndex);
                            else if (tile.Layer.Equals("Front"))
                                frontLayer.Tiles[tile.TileX, tile.TileY] = new StaticTile(frontLayer, tilesheet, BlendMode.Alpha, tileIndex: tile.EmptyTroughTilesheetIndex);
                        }

                        break;
                    }
                }
            }
            else if (animalCount == 0)
            {
                if (building is Coop)
                {
                    ModData.CoopsWithWateredTrough.Add(building.nameOfIndoors.ToLower());
                }
                else if (building is Barn)
                {
                    ModData.BarnsWithWateredTrough.Add(building.nameOfIndoors.ToLower());
                }
            }
        }

        private List<FarmAnimal> FindThirstyAnimals()
        { 
            List<FarmAnimal> animalsLeftThirsty = new List<FarmAnimal>();
            
            // Look for all animals inside buildings and check whether their troughs are watered.
            foreach (Building building in Game1.getFarm().buildings)
            {
                if (building.nameOfIndoors.ToLower().Contains("coop"))
                {
                    foreach (var animal in ((AnimalHouse) building.indoors.Value).animals.Values
                        .Where(animal =>
                            ModData.CoopsWithWateredTrough.Contains(animal.home.nameOfIndoors.ToLower()) == false &&
                            ModData.FullAnimals.Contains(animal) == false).Where(animal =>
                            !animalsLeftThirsty.Contains(animal)))
                    {
                        animal.friendshipTowardFarmer.Value -= Math.Abs(Config.NegativeFriendshipPointsForNotWateredTrough);
                        animalsLeftThirsty.Add(animal);
                    }
                } 
                else if (building.nameOfIndoors.ToLower().Contains("barn"))
                {
                    foreach (var animal in ((AnimalHouse) building.indoors.Value).animals.Values
                        .Where(animal =>
                            ModData.BarnsWithWateredTrough.Contains(animal.home.nameOfIndoors.ToLower()) == false &&
                            ModData.FullAnimals.Contains(animal) == false).Where(animal =>
                            !animalsLeftThirsty.Contains(animal)))
                    {
                        animal.friendshipTowardFarmer.Value -= Math.Abs(Config.NegativeFriendshipPointsForNotWateredTrough);
                        animalsLeftThirsty.Add(animal);
                    }
                }
            }

            // Check for animals outside their buildings as well.
            foreach (var animal in Game1.getFarm().animals.Values)
            {
                if (animal.home is Coop)
                {
                    if ((ModData.CoopsWithWateredTrough.Contains(animal.home.nameOfIndoors.ToLower()) ||
                         ModData.FullAnimals.Contains(animal)) &&
                        animal.home.animalDoorOpen.Value) continue;
                    
                    if (animalsLeftThirsty.Contains(animal)) continue;
                        
                    animal.friendshipTowardFarmer.Value -= Math.Abs(Config.NegativeFriendshipPointsForNotWateredTrough);
                    animalsLeftThirsty.Add(animal);
                } 
                else if(animal.home is Barn)
                {
                    if ((ModData.BarnsWithWateredTrough.Contains(animal.home.nameOfIndoors.ToLower()) ||
                         ModData.FullAnimals.Contains(animal)) &&
                        animal.home.animalDoorOpen.Value) continue;
                    
                    if (animalsLeftThirsty.Contains(animal)) continue;
                    
                    animal.friendshipTowardFarmer.Value -= Math.Abs(Config.NegativeFriendshipPointsForNotWateredTrough);
                    animalsLeftThirsty.Add(animal);
                }
            }

            return animalsLeftThirsty;
        }

        /// <summary> Looks for animals left thirsty, notifies player of them and loads new tilesheets if needed. </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void HandleDayUpdate(object sender, SavedEventArgs e)
        {
            LoadNewTileSheets();
            
            // Notify player of animals left thirsty, if any.
            if (AnimalsLeftThirstyYesterday.Any())
            {
                switch (AnimalsLeftThirstyYesterday.Count)
                {
                    case 1 when Helper.ModRegistry.IsLoaded("Paritee.GenderNeutralFarmAnimals"):
                        Game1.showGlobalMessage(ModHelper.Translation.Get(
                            "AnimalsLeftWithoutWaterYesterday.globalMessage.oneAnimal_UnknownGender",
                            new { firstAnimalName = AnimalsLeftThirstyYesterday[0].displayName }));
                        break;
                    case 1 when AnimalsLeftThirstyYesterday[0].isMale():
                        Game1.showGlobalMessage(ModHelper.Translation.Get(
                            "AnimalsLeftWithoutWaterYesterday.globalMessage.oneAnimal_Male",
                            new { firstAnimalName = AnimalsLeftThirstyYesterday[0].displayName }));
                        break;
                    case 1 when !AnimalsLeftThirstyYesterday[0].isMale():
                        Game1.showGlobalMessage(ModHelper.Translation.Get(
                            "AnimalsLeftWithoutWaterYesterday.globalMessage.oneAnimal_Female",
                            new { firstAnimalName = AnimalsLeftThirstyYesterday[0].displayName }));
                        break;
                    case 2:
                        Game1.showGlobalMessage(ModHelper.Translation.Get(
                            "AnimalsLeftWithoutWaterYesterday.globalMessage.twoAnimals",
                            new
                            {
                                firstAnimalName = AnimalsLeftThirstyYesterday[0].displayName,
                                secondAnimalName = AnimalsLeftThirstyYesterday[1].displayName
                            }));
                        break;
                    case 3:
                        Game1.showGlobalMessage(ModHelper.Translation.Get(
                            "AnimalsLeftWithoutWaterYesterday.globalMessage.threeAnimals",
                            new
                            {
                                firstAnimalName = AnimalsLeftThirstyYesterday[0].displayName,
                                secondAnimalName = AnimalsLeftThirstyYesterday[1].displayName,
                                thirdAnimalName = AnimalsLeftThirstyYesterday[2].displayName
                            }));
                        break;
                    default:
                        Game1.showGlobalMessage(ModHelper.Translation.Get(
                            "AnimalsLeftWithoutWaterYesterday.globalMessage.multipleAnimals",
                            new
                            {
                                firstAnimalName = AnimalsLeftThirstyYesterday[0].displayName,
                                secondAnimalName = AnimalsLeftThirstyYesterday[1].displayName,
                                thirdAnimalName = AnimalsLeftThirstyYesterday[2].displayName,
                                totalAmountExcludingFirstThree = AnimalsLeftThirstyYesterday.Count - 3
                            }));
                        break;
                }
            }
            
            ModData.FullAnimals = new List<FarmAnimal>();
            
            // Check whether there is a festival today. If not, empty the troughs.
            if (!Utility.isFestivalDay(Game1.dayOfMonth, Game1.currentSeason))
            {
                EmptyWaterTroughs();
            }
            else
            {
                foreach (Building building in Game1.getFarm().buildings)
                {
                    if (building is Coop)
                    {
                        ModData.CoopsWithWateredTrough.Add(building.nameOfIndoors.ToLower());

                        switch (building.nameOfIndoorsWithoutUnique.ToLower())
                        {
                            case "coop":
                                ChangeCoopTexture(building, false);
                                break;
                            
                            case "coop2":
                                ChangeBigCoopTexture(building, false);
                                break;
                        }
                    }
                    else if (building is Barn)
                    {
                        ModData.BarnsWithWateredTrough.Add(building.nameOfIndoors.ToLower());
                    }
                }
            }
            
            PlaceWateringSystems();
        }

        /// <summary> Raised after the game is launched, right before the first update tick. This happens once per game session (unrelated to loading saves). All mods are loaded and initialised at this point, so this is a good time to set up mod integrations. </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            var harmony = new Harmony(ModManifest.UniqueID);
            
            ModMonitor.VerboseLog("Patching AnimalHouse.performToolAction.");
            harmony.Patch(
                AccessTools.Method(typeof(AnimalHouse), nameof(AnimalHouse.performToolAction)),
                new HarmonyMethod(typeof(HarmonyPatches), nameof(HarmonyPatches.AnimalHouseToolAction))
            );
            
            ModMonitor.VerboseLog("Patching FarmAnimal.dayUpdate.");
            harmony.Patch(
                AccessTools.Method(typeof(FarmAnimal), nameof(FarmAnimal.dayUpdate)),
                new HarmonyMethod(typeof(HarmonyPatches), nameof(HarmonyPatches.AnimalDayUpdate))
            );

            ModMonitor.VerboseLog("Patching FarmAnimal.behaviors.");
            harmony.Patch(
                AccessTools.Method(typeof(FarmAnimal), "behaviors", new[] {
                    typeof(GameTime),
                    typeof(GameLocation)
                }),
                new HarmonyMethod(typeof(HarmonyPatches), nameof(HarmonyPatches.AnimalBehaviors))
            );

            ModMonitor.VerboseLog("Patching Game1.warpFarmer.");
            harmony.Patch(
                AccessTools.Method(typeof(Game1), nameof(Game1.warpFarmer), new[] {
                    typeof(string),
                    typeof(int),
                    typeof(int),
                    typeof(int),
                    typeof(bool)
                }),
                new HarmonyMethod(typeof(HarmonyPatches), nameof(HarmonyPatches.WarpFarmer))
            );
            
            ModMonitor.VerboseLog("Done patching.");
            
            ModConfig.SetUpModConfigMenu(Config, this);
        }

        private void OnModMessageReceived(object sender, ModMessageReceivedEventArgs e)
        {
            if (e.FromModID != ModManifest.UniqueID || e.Type != "TroughWateredMessage") return;
            
            TroughWateredMessage message = e.ReadAs<TroughWateredMessage>();

            if (message.BuildingType == typeof(Coop))
            {
                ModData.CoopsWithWateredTrough.Add(message.BuildingUniqueName);
            }
            else
            {
                ModData.BarnsWithWateredTrough.Add(message.BuildingUniqueName);
            }
            
            string locationName = message.BuildingUniqueName;
            string locationNameWithoutUnique = Game1.getLocationFromName(locationName).Name;
            Building building = ((AnimalHouse)Game1.getLocationFromName(locationName)).getBuilding();

            switch (building.nameOfIndoorsWithoutUnique.ToLower())
            {
                case "coop":
                    ChangeCoopTexture(building, false);
                    break;
                case "coop2":
                    ChangeBigCoopTexture(building, false);
                    break;
            }
                
            if (string.Equals(Game1.currentLocation.NameOrUniqueName, message.BuildingUniqueName,
                StringComparison.CurrentCultureIgnoreCase))
            {
                HarmonyPatchExecutors.CheckForWateredTroughs(building, locationName, locationNameWithoutUnique);
            }
        }

        /// <summary> Raised after the save is loaded. </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            CheckHomeStatus();
            LoadNewTileSheets();
            PlaceWateringSystems();

            HandleDayStart();
        }
        
        private void OnDayEnding(object sender, DayEndingEventArgs e)
        {
            AnimalsLeftThirstyYesterday = FindThirstyAnimals();
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
                    if (building is Coop)
                    {
                        ModData.CoopsWithWateredTrough.Add(building.nameOfIndoors.ToLower());

                        if (building.nameOfIndoorsWithoutUnique.ToLower().Equals("coop"))
                        {
                            ChangeCoopTexture(building, false);
                        }
                        else if (building.nameOfIndoorsWithoutUnique.ToLower().Equals("coop2"))
                        {
                            ChangeBigCoopTexture(building, false);
                        }
                    }
                    else if (building is Barn)
                    {
                        ModData.BarnsWithWateredTrough.Add(building.nameOfIndoors.ToLower());
                    }
                }
            }
        }
        
        private void CheckHomeStatus()
        {
            if (Game1.getFarm().getAllFarmAnimals().Any(animal => animal.home == null))
            {
                Utility.fixAllAnimals();
            }
        }

        #endregion
        #region Utils
        
        public void ChangeBigCoopTexture(Building building, bool empty)
        {
            if (!Config.ReplaceCoopTextureIfTroughIsEmpty) return;

            building.texture = empty ? 
                new Lazy<Texture2D>(() => Helper.Content.Load<Texture2D>("assets/Coop2_emptyWaterTrough.png"))
                : new Lazy<Texture2D>(() => Helper.Content.Load<Texture2D>("assets/Coop2_fullWaterTrough.png"));
        }
        
        public void ChangeCoopTexture(Building building, bool empty)
        {
            if (!Config.ReplaceCoopTextureIfTroughIsEmpty) return;

            building.texture = empty ? 
                new Lazy<Texture2D>(() => Helper.Content.Load<Texture2D>("assets/Coop_emptyWaterTrough.png"))
                : new Lazy<Texture2D>(() => Helper.Content.Load<Texture2D>("assets/Coop_fullWaterTrough.png"));
        }

        private void LoadNewTileSheets()
        {
            foreach (Building building in Game1.getFarm().buildings)
            {
                if (building is Coop)
                {
                    var coopMap = building.indoors.Value.Map;
                    
                    if (coopMap.TileSheets.All(ts => !ts.Id.Equals("z_waterTroughTilesheet")))
                    {
                        var tileSheet = new TileSheet(
                            "z_waterTroughTilesheet",
                            coopMap,
                            Instance.Helper.Content.GetActualAssetKey($"assets/waterTroughTilesheet{ (Config.CleanerTroughs ? "_clean" : "") }.xnb"),
                            new Size(160, 16),
                            new Size(16, 16)
                        );

                        coopMap.AddTileSheet(tileSheet);
                        coopMap.LoadTileSheets(Game1.mapDisplayDevice);
                    }
                    
                    if (building.nameOfIndoorsWithoutUnique.ToLower() != "coop3" ||
                        !Config.WateringSystemInDeluxeBuildings ||
                        coopMap.TileSheets.Any(ts => ts.Id.Equals("z_wateringSystemTilesheet"))) continue;

                    var coop3Map = building.indoors.Value.Map;

                    var tileSheet3 = new TileSheet(
                        "z_wateringSystemTilesheet",
                        coop3Map,
                        Instance.Helper.Content.GetActualAssetKey("assets/wateringSystemTilesheet.xnb"),
                        new Size(48, 16),
                        new Size(16, 16)
                    );

                    coop3Map.AddTileSheet(tileSheet3);
                    coop3Map.LoadTileSheets(Game1.mapDisplayDevice);
                }
                else if (building is Barn)
                {
                    var barnMap = building.indoors.Value.Map;

                    if (barnMap.TileSheets.All(ts => !ts.Id.Equals("z_waterTroughTilesheet")))
                    {
                        var tileSheet = new TileSheet(
                            "z_waterTroughTilesheet",
                            barnMap,
                            Instance.Helper.Content.GetActualAssetKey($"assets/waterTroughTilesheet{ (Config.CleanerTroughs ? "_clean" : "") }.xnb"),
                            new Size(160, 16),
                            new Size(16, 16)
                        );

                        barnMap.AddTileSheet(tileSheet);
                        barnMap.LoadTileSheets(Game1.mapDisplayDevice);
                    }

                    if (building.nameOfIndoorsWithoutUnique.ToLower() != "barn3" ||
                        !Config.WateringSystemInDeluxeBuildings || 
                        barnMap.TileSheets.Any(ts => ts.Id.Equals("z_wateringSystemTilesheet"))) continue;
                    
                    var barn3Map = building.indoors.Value.Map;

                    var tileSheet3 = new TileSheet(
                        "z_wateringSystemTilesheet",
                        barn3Map,
                        Instance.Helper.Content.GetActualAssetKey("assets/wateringSystemTilesheet.xnb"),
                        new Size(48, 16),
                        new Size(16, 16)
                    );

                    barn3Map.AddTileSheet(tileSheet3);
                    barn3Map.LoadTileSheets(Game1.mapDisplayDevice);
                }
            }
        }

        private void PlaceWateringSystems()
        {
            if (!Config.WateringSystemInDeluxeBuildings) return;
            
            foreach (Building building in Game1.getFarm().buildings)
            {
                switch (building.nameOfIndoorsWithoutUnique.ToLower())
                {
                    case "coop3":
                    {
                        var gameLocation = building.indoors.Value;

                        foreach (SimplifiedTile tile in CurrentTroughPlacementProfile.coop3WateringSystem.TilesToRemove)
                        {
                            gameLocation.removeTile(tile.TileX, tile.TileY, tile.Layer);
                        }

                        Layer buildingsLayer = gameLocation.Map.GetLayer("Buildings");
                        Layer frontLayer = gameLocation.Map.GetLayer("Front");
                        TileSheet tilesheet = gameLocation.Map.GetTileSheet("z_wateringSystemTilesheet");

                        if (CurrentTroughPlacementProfile.coop3WateringSystem.Layer.Equals("Buildings"))
                            buildingsLayer.Tiles[CurrentTroughPlacementProfile.coop3WateringSystem.TileX, CurrentTroughPlacementProfile.coop3WateringSystem.TileY] = new StaticTile(buildingsLayer, tilesheet, BlendMode.Alpha, tileIndex: CurrentTroughPlacementProfile.coop3WateringSystem.SystemTilesheetIndex);
                        else if (CurrentTroughPlacementProfile.coop3WateringSystem.Layer.Equals("Front"))
                            frontLayer.Tiles[CurrentTroughPlacementProfile.coop3WateringSystem.TileX, CurrentTroughPlacementProfile.coop3WateringSystem.TileY] = new StaticTile(frontLayer, tilesheet, BlendMode.Alpha, tileIndex: CurrentTroughPlacementProfile.coop3WateringSystem.SystemTilesheetIndex);
                        break;
                    }
                    case "barn3":
                    {
                        var gameLocation = building.indoors.Value;

                        foreach (SimplifiedTile tile in CurrentTroughPlacementProfile.barn3WateringSystem.TilesToRemove)
                        {
                            gameLocation.removeTile(tile.TileX, tile.TileY, tile.Layer);
                        }

                        Layer buildingsLayer = gameLocation.Map.GetLayer("Buildings");
                        Layer frontLayer = gameLocation.Map.GetLayer("Front");
                        TileSheet tilesheet = gameLocation.Map.GetTileSheet("z_wateringSystemTilesheet");

                        if (CurrentTroughPlacementProfile.barn3WateringSystem.Layer.Equals("Buildings"))
                            buildingsLayer.Tiles[CurrentTroughPlacementProfile.barn3WateringSystem.TileX, CurrentTroughPlacementProfile.barn3WateringSystem.TileY] = new StaticTile(buildingsLayer, tilesheet, BlendMode.Alpha, tileIndex: CurrentTroughPlacementProfile.barn3WateringSystem.SystemTilesheetIndex);
                        else if (CurrentTroughPlacementProfile.barn3WateringSystem.Layer.Equals("Front"))
                            frontLayer.Tiles[CurrentTroughPlacementProfile.barn3WateringSystem.TileX, CurrentTroughPlacementProfile.barn3WateringSystem.TileY] = new StaticTile(frontLayer, tilesheet, BlendMode.Alpha, tileIndex: CurrentTroughPlacementProfile.barn3WateringSystem.SystemTilesheetIndex);
                        break;
                    }
                }
            }
        }

        private void SendMessageToSelf(object message, string messageType)
        {
            Helper.Multiplayer.SendMessage(message, messageType, new[] { ModManifest.UniqueID });
        }

        private string NextSeason(string season)
        {
            var newSeason = "";
            
            switch (season)
            {
                case "spring":
                    newSeason = "summer";
                    break;
                case "summer":
                    newSeason = "fall";
                    break;
                case "fall":
                    newSeason = "winter";
                    break;
                case "winter":
                    newSeason = "spring";
                    break;
            }

            return newSeason;
        }

        #endregion
    }
}
