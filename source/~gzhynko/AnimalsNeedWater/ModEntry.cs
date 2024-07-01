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
        public static IMonitor ModMonitor;
        public static IModHelper ModHelper;
        public static ModEntry Instance;
        public ModConfig Config;
        public TroughPlacementProfile CurrentTroughPlacementProfile;
        public List<FarmAnimal> AnimalsLeftThirstyYesterday;

        // Initialize a dictionary to group buildings by their parent location
        public List<Building> AnimalBuildings;
        public IEnumerable<IGrouping<GameLocation, Building>> AnimalBuildingGroups;

        /// <summary> The mod entry point, called after the mod is first loaded. </summary>
        /// <param name="helper"> Provides simplified APIs for writing mods. </param>
        public override void Entry(IModHelper helper)
        {
            ModHelper = helper;
            ModMonitor = Monitor;
            Instance = this;

            AnimalsLeftThirstyYesterday = new List<FarmAnimal>();
            AnimalBuildings = new List<Building>();

            Config = Helper.ReadConfig<ModConfig>();
            helper.Events.GameLoop.GameLaunched += OnGameLaunched;
            helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
            helper.Events.GameLoop.Saved += HandleDayUpdate;
            helper.Events.GameLoop.DayEnding += OnDayEnding;
            helper.Events.Multiplayer.ModMessageReceived += OnModMessageReceived;

            SetTroughPlacementProfile();
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
        
        public void SendTroughWateredMessage(string buildingType, string building)
        {
            SendMessageToSelf(new TroughWateredMessage(buildingType, building), "TroughWateredMessage");
        }
        
        /// <summary> Look for known mods that modify coop/barn interiors and load the matching profile (if any). </summary>
        private void SetTroughPlacementProfile()
        {
            try
            {
                TroughPlacementProfiles.LoadProfiles(Helper);
            }
            catch (Exception e)
            {
                Monitor.Log($"Error while loading trough placement profiles: {e}", LogLevel.Warn);
            }

            if (TroughPlacementProfiles.DefaultProfile == null)
            {
                Monitor.Log("The default trough placement profile was not loaded. Animals Need Water will not work correctly. Make sure all files in this mod's assets folder are in place.", LogLevel.Error);
                return;
            }
            
            foreach (var modInfo in Helper.ModRegistry.GetAll())
            {
                var profile = TroughPlacementProfiles.GetProfileByUniqueId(modInfo.Manifest.UniqueID);
                if (profile != null)
                {
                    CurrentTroughPlacementProfile = profile;
                }
            }

            CurrentTroughPlacementProfile ??= TroughPlacementProfiles.DefaultProfile;
        }

        /// <summary> Empty water troughs in animal houses. </summary>
        private void EmptyWaterTroughs()
        {
            ModData.BarnsWithWateredTrough = new List<string>();
            ModData.CoopsWithWateredTrough = new List<string>();

            foreach (Building building in AnimalBuildings)
            {
                if (!building.buildingType.Value.Contains("Barn") && !building.buildingType.Value.Contains("Coop"))
                    continue;
                
                // If the building is a deluxe one and the corresponding config option is set to true, 
                // avoid emptying troughs and mark it as watered.
                if(building.buildingType.Value.ToLower().Contains("deluxe") && Config.WateringSystemInDeluxeBuildings)
                {
                    switch (building.buildingType.Value.ToLower())
                    {
                        case "deluxe barn":
                        {
                            if (!ModData.BarnsWithWateredTrough.Contains(building.indoors.Value.NameOrUniqueName.ToLower()))
                                ModData.BarnsWithWateredTrough.Add(building.indoors.Value.NameOrUniqueName.ToLower());
                            break;
                        }
                        case "deluxe coop":
                        {
                            if (!ModData.CoopsWithWateredTrough.Contains(building.indoors.Value.NameOrUniqueName.ToLower()))
                                ModData.CoopsWithWateredTrough.Add(building.indoors.Value.NameOrUniqueName.ToLower());
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
            
            foreach (FarmAnimal animal in building.GetParentLocation().getAllFarmAnimals())
            {
                if (animal.home.indoors.Value.NameOrUniqueName.ToLower().Equals(gameLocation.NameOrUniqueName.ToLower())) animalCount++;
            }

            // if no animals live here, do not empty the water trough
            if (animalCount == 0)
            {
                if (IsCoop(building))
                {
                    ModData.CoopsWithWateredTrough.Add(building.indoors.Value.NameOrUniqueName.ToLower());
                }
                else if (IsBarn(building))
                {
                    ModData.BarnsWithWateredTrough.Add(building.indoors.Value.NameOrUniqueName.ToLower());
                }
                
                return;
            }
            
            switch (building.buildingType.Value.ToLower())
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
                            buildingsLayer.Tiles[tile.TileX, tile.TileY] = new StaticTile(buildingsLayer, tilesheet, BlendMode.Alpha, tileIndex: tile.EmptyIndex);
                        else if (tile.Layer.Equals("Front"))
                            frontLayer.Tiles[tile.TileX, tile.TileY] = new StaticTile(frontLayer, tilesheet, BlendMode.Alpha, tileIndex: tile.EmptyIndex);
                    }

                    break;
                }
                case "big coop":
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
                            buildingsLayer.Tiles[tile.TileX, tile.TileY] = new StaticTile(buildingsLayer, tilesheet, BlendMode.Alpha, tileIndex: tile.EmptyIndex);
                        else if (tile.Layer.Equals("Front"))
                            frontLayer.Tiles[tile.TileX, tile.TileY] = new StaticTile(frontLayer, tilesheet, BlendMode.Alpha, tileIndex: tile.EmptyIndex);
                    }

                    break;
                }
                case "deluxe coop":
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
                            buildingsLayer.Tiles[tile.TileX, tile.TileY] = new StaticTile(buildingsLayer, tilesheet, BlendMode.Alpha, tileIndex: tile.EmptyIndex);
                        else if (tile.Layer.Equals("Front"))
                            frontLayer.Tiles[tile.TileX, tile.TileY] = new StaticTile(frontLayer, tilesheet, BlendMode.Alpha, tileIndex: tile.EmptyIndex);
                    }

                    break;
                }
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
                            buildingsLayer.Tiles[tile.TileX, tile.TileY] = new StaticTile(buildingsLayer, tilesheet, BlendMode.Alpha, tileIndex: tile.EmptyIndex);
                        else if (tile.Layer.Equals("Front"))
                            frontLayer.Tiles[tile.TileX, tile.TileY] = new StaticTile(frontLayer, tilesheet, BlendMode.Alpha, tileIndex: tile.EmptyIndex);
                    }

                    break;
                }
                case "big barn":
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
                            buildingsLayer.Tiles[tile.TileX, tile.TileY] = new StaticTile(buildingsLayer, tilesheet, BlendMode.Alpha, tileIndex: tile.EmptyIndex);
                        else if (tile.Layer.Equals("Front"))
                            frontLayer.Tiles[tile.TileX, tile.TileY] = new StaticTile(frontLayer, tilesheet, BlendMode.Alpha, tileIndex: tile.EmptyIndex);
                    }

                    break;
                }
                case "deluxe barn":
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
                            buildingsLayer.Tiles[tile.TileX, tile.TileY] = new StaticTile(buildingsLayer, tilesheet, BlendMode.Alpha, tileIndex: tile.EmptyIndex);
                        else if (tile.Layer.Equals("Front"))
                            frontLayer.Tiles[tile.TileX, tile.TileY] = new StaticTile(frontLayer, tilesheet, BlendMode.Alpha, tileIndex: tile.EmptyIndex);
                    }

                    break;
                }
            }
        }

        private List<FarmAnimal> FindThirstyAnimals()
        { 
            List<FarmAnimal> animalsLeftThirsty = new List<FarmAnimal>();



            foreach (var locationGroup in AnimalBuildingGroups)
            {
                GameLocation parentLocation = locationGroup.Key;
                List<Building> buildingsInLocation = locationGroup.ToList();

                // Look for all animals inside buildings and check whether their troughs are watered.
                foreach (Building building in buildingsInLocation)
                {
                    if (!building.buildingType.Value.Contains("Barn") && !building.buildingType.Value.Contains("Coop"))
                        continue;
                
                    if (IsCoop(building))
                    {
                        foreach (var animal in ((AnimalHouse) building.indoors.Value).animals.Values
                            .Where(animal =>
                                ModData.CoopsWithWateredTrough.Contains(animal.home.indoors.Value.uniqueName.Value.ToLower()) == false &&
                                ModData.FullAnimals.Contains(animal) == false).Where(animal =>
                                !animalsLeftThirsty.Contains(animal)))
                        {
                            animal.friendshipTowardFarmer.Value -= Math.Abs(Config.NegativeFriendshipPointsForNotWateredTrough);
                            animalsLeftThirsty.Add(animal);
                        }
                    } 
                    else if (IsBarn(building))
                    {
                        foreach (var animal in ((AnimalHouse) building.indoors.Value).animals.Values
                            .Where(animal =>
                                ModData.BarnsWithWateredTrough.Contains(animal.home.indoors.Value.uniqueName.Value.ToLower()) == false &&
                                ModData.FullAnimals.Contains(animal) == false).Where(animal =>
                                !animalsLeftThirsty.Contains(animal)))
                        {
                            animal.friendshipTowardFarmer.Value -= Math.Abs(Config.NegativeFriendshipPointsForNotWateredTrough);
                            animalsLeftThirsty.Add(animal);
                        }
                    }
                }

                // Check for animals outside their buildings as well.
                foreach (var animal in parentLocation.animals.Values)
                {
                    if (IsCoop(animal.home))
                    {
                        if ((ModData.CoopsWithWateredTrough.Contains(animal.home.indoors.Value.uniqueName.Value.ToLower()) ||
                             ModData.FullAnimals.Contains(animal)) &&
                            animal.home.animalDoorOpen.Value) continue;
                    
                        if (animalsLeftThirsty.Contains(animal)) continue;
                        
                        animal.friendshipTowardFarmer.Value -= Math.Abs(Config.NegativeFriendshipPointsForNotWateredTrough);
                        animalsLeftThirsty.Add(animal);
                    } 
                    else if(IsBarn(animal.home))
                    {
                        if ((ModData.BarnsWithWateredTrough.Contains(animal.home.indoors.Value.uniqueName.Value.ToLower()) ||
                             ModData.FullAnimals.Contains(animal)) &&
                            animal.home.animalDoorOpen.Value) continue;
                    
                        if (animalsLeftThirsty.Contains(animal)) continue;
                    
                        animal.friendshipTowardFarmer.Value -= Math.Abs(Config.NegativeFriendshipPointsForNotWateredTrough);
                        animalsLeftThirsty.Add(animal);
                    }
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
            
            // If enabled in config: notify player of animals left thirsty, if any.
            if (AnimalsLeftThirstyYesterday.Any() && Config.ShowAnimalsLeftThirstyMessage)
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
            if (!Utility.isFestivalDay(Game1.dayOfMonth, Game1.season))
            {
                EmptyWaterTroughs();
            }
            else
            {
                foreach (Building building in AnimalBuildings)
                {
                    if (!building.buildingType.Contains("Barn") && !building.buildingType.Contains("Coop"))
                        continue;
                    
                    if (IsCoop(building))
                    {
                        ModData.CoopsWithWateredTrough.Add(building.indoors.Value.NameOrUniqueName.ToLower());

                        switch (building.GetIndoorsName().ToLower())
                        {
                            case "coop":
                                ChangeCoopTexture(building, false);
                                break;
                            
                            case "coop2":
                                ChangeBigCoopTexture(building, false);
                                break;
                        }
                    }
                    else if (IsBarn(building))
                    {
                        ModData.BarnsWithWateredTrough.Add(building.indoors.Value.NameOrUniqueName.ToLower());
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
            
            ModMonitor.VerboseLog("Patching GameLocation.performToolAction.");
            harmony.Patch(
                AccessTools.Method(typeof(GameLocation), nameof(GameLocation.performToolAction)),
                new HarmonyMethod(typeof(HarmonyPatches), nameof(HarmonyPatches.GameLocationToolAction))
            );
            
            ModMonitor.VerboseLog("Patching FarmAnimal.dayUpdate.");
            harmony.Patch(
                AccessTools.Method(typeof(FarmAnimal), nameof(FarmAnimal.dayUpdate)),
                new HarmonyMethod(typeof(HarmonyPatches), nameof(HarmonyPatches.AnimalDayUpdate))
            );

            ModMonitor.VerboseLog("Patching FarmAnimal.behaviors.");
            harmony.Patch(
                AccessTools.Method(typeof(FarmAnimal), nameof(FarmAnimal.behaviors)),
                new HarmonyMethod(typeof(HarmonyPatches), nameof(HarmonyPatches.AnimalBehaviors))
            );

            ModMonitor.VerboseLog("Patching Game1.OnLocationChanged.");
            harmony.Patch(
                AccessTools.Method(typeof(Game1), nameof(Game1.OnLocationChanged)),
                new HarmonyMethod(typeof(HarmonyPatches), nameof(HarmonyPatches.OnLocationChanged))
            );
            
            ModMonitor.VerboseLog("Done patching.");
            
            ModConfig.SetUpModConfigMenu(Config, this);
        }

        private void OnModMessageReceived(object sender, ModMessageReceivedEventArgs e)
        {
            if (e.FromModID != ModManifest.UniqueID || e.Type != "TroughWateredMessage") return;
            
            TroughWateredMessage message = e.ReadAs<TroughWateredMessage>();

            if (message.BuildingType == "coop")
            {
                ModData.CoopsWithWateredTrough.Add(message.BuildingUniqueName);
            }
            else if (message.BuildingType == "barn")
            {
                ModData.BarnsWithWateredTrough.Add(message.BuildingUniqueName);
            }
            
            string locationName = message.BuildingUniqueName;
            string locationNameWithoutUnique = Game1.getLocationFromName(locationName).Name;
            Building building = Game1.getLocationFromName(locationName).GetContainingBuilding();

            switch (building.buildingType.Value.ToLower())
            {
                case "coop":
                    ChangeCoopTexture(building, false);
                    break;
                case "big coop":
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
            GetAnimalBuildings();
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
            if (!Utility.isFestivalDay(Game1.dayOfMonth, Game1.season))
            {
                EmptyWaterTroughs();
            } 
            else
            {
                foreach (Building building in AnimalBuildings)
                {
                    if (!building.buildingType.Contains("Barn") && !building.buildingType.Contains("Coop"))
                        continue;
                    
                    if (IsCoop(building))
                    {
                        ModData.CoopsWithWateredTrough.Add(building.indoors.Value.NameOrUniqueName.ToLower());

                        if (building.buildingType.Value.ToLower().Equals("coop"))
                        {
                            ChangeCoopTexture(building, false);
                        }
                        else if (building.buildingType.Value.ToLower().Equals("big coop"))
                        {
                            ChangeBigCoopTexture(building, false);
                        }
                    }
                    else if (IsBarn(building))
                    {
                        ModData.BarnsWithWateredTrough.Add(building.indoors.Value.NameOrUniqueName.ToLower());
                    }
                }
            }
        }

        private void GetAnimalBuildings()
        {
            AnimalBuildings.Clear();
            foreach (GameLocation location in Game1.locations)
            {
                foreach (Building building in location.buildings)
                {
                    if (building.GetIndoors() is AnimalHouse)
                    {
                        AnimalBuildings.Add(building);
                    }
                }
            }

            AnimalBuildingGroups = AnimalBuildings.GroupBy(b => b.GetParentLocation());
        }

        private void CheckHomeStatus()
        {
            bool needToFixAnimals = false;

            foreach (var locationGroup in AnimalBuildingGroups)
            {
                GameLocation parentLocation = locationGroup.Key;

                if (parentLocation.getAllFarmAnimals().Any(animal => animal.home == null))
                {
                    needToFixAnimals = true;
                    break;
                }
            }

            if (needToFixAnimals)
            {
                Utility.fixAllAnimals();
            }
        }

        public void ChangeBigCoopTexture(Building building, bool empty)
        {
            if (!Config.ReplaceCoopTextureIfTroughIsEmpty) return;

            if (empty)
            {
                building.texture = new Lazy<Texture2D>(() =>
                    Helper.ModContent.Load<Texture2D>("assets/Coop2_emptyWaterTrough.png"));
            }
            else
            {
                building.resetTexture();
            }
        }
        
        public void ChangeCoopTexture(Building building, bool empty)
        {
            if (!Config.ReplaceCoopTextureIfTroughIsEmpty) return;
            
            if (empty)
            {
                building.texture = new Lazy<Texture2D>(() =>
                    Helper.ModContent.Load<Texture2D>("assets/Coop_emptyWaterTrough.png"));
            }
            else
            {
                building.resetTexture();
            }

        }

        private void LoadNewTileSheets()
        {
            foreach (Building building in AnimalBuildings)
            {
                if (IsCoop(building))
                {
                    var coopMap = building.indoors.Value.Map;
                    
                    if (coopMap.TileSheets.All(ts => !ts.Id.Equals("z_waterTroughTilesheet")))
                    {
                        var tileSheetImageSource = Instance.Helper.ModContent
                            .GetInternalAssetName(
                                $"assets/waterTroughTilesheet{(Config.CleanerTroughs ? "_clean" : "")}.png").Name;
                        var tileSheet = new TileSheet(
                            "z_waterTroughTilesheet",
                            coopMap,
                            tileSheetImageSource,
                            new Size(160, 16),
                            new Size(16, 16)
                        );

                        coopMap.AddTileSheet(tileSheet);
                        coopMap.LoadTileSheets(Game1.mapDisplayDevice);
                    }
                    
                    if (building.buildingType.Value.ToLower() != "deluxe coop" ||
                        !Config.WateringSystemInDeluxeBuildings ||
                        coopMap.TileSheets.Any(ts => ts.Id.Equals("z_wateringSystemTilesheet"))) continue;

                    var coop3Map = building.indoors.Value.Map;

                    var tileSheet3ImageSource = Instance.Helper.ModContent
                        .GetInternalAssetName("assets/wateringSystemTilesheet.png").Name;
                    var tileSheet3 = new TileSheet(
                        "z_wateringSystemTilesheet",
                        coop3Map,
                        tileSheet3ImageSource,
                        new Size(48, 16),
                        new Size(16, 16)
                    );

                    coop3Map.AddTileSheet(tileSheet3);
                    coop3Map.LoadTileSheets(Game1.mapDisplayDevice);
                }
                else if (IsBarn(building))
                {
                    var barnMap = building.indoors.Value.Map;

                    if (barnMap.TileSheets.All(ts => !ts.Id.Equals("z_waterTroughTilesheet")))
                    {
                        var tileSheetImageSource = Instance.Helper.ModContent
                            .GetInternalAssetName(
                                $"assets/waterTroughTilesheet{(Config.CleanerTroughs ? "_clean" : "")}.png").Name;
                        var tileSheet = new TileSheet(
                            "z_waterTroughTilesheet",
                            barnMap,
                            tileSheetImageSource,
                            new Size(160, 16),
                            new Size(16, 16)
                        );

                        barnMap.AddTileSheet(tileSheet);
                        barnMap.LoadTileSheets(Game1.mapDisplayDevice);
                    }

                    if (building.buildingType.Value.ToLower() != "deluxe barn" ||
                        !Config.WateringSystemInDeluxeBuildings || 
                        barnMap.TileSheets.Any(ts => ts.Id.Equals("z_wateringSystemTilesheet"))) continue;
                    
                    var barn3Map = building.indoors.Value.Map;

                    var tileSheet3ImageSource = Instance.Helper.ModContent
                        .GetInternalAssetName("assets/wateringSystemTilesheet.png").Name;
                    var tileSheet3 = new TileSheet(
                        "z_wateringSystemTilesheet",
                        barn3Map,
                        tileSheet3ImageSource,
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
            
            foreach (Building building in AnimalBuildings)
            {
                switch (building.buildingType.Value.ToLower())
                {
                    case "deluxe coop":
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
                            buildingsLayer.Tiles[CurrentTroughPlacementProfile.coop3WateringSystem.TileX, CurrentTroughPlacementProfile.coop3WateringSystem.TileY] = new StaticTile(buildingsLayer, tilesheet, BlendMode.Alpha, tileIndex: CurrentTroughPlacementProfile.coop3WateringSystem.SystemIndex);
                        else if (CurrentTroughPlacementProfile.coop3WateringSystem.Layer.Equals("Front"))
                            frontLayer.Tiles[CurrentTroughPlacementProfile.coop3WateringSystem.TileX, CurrentTroughPlacementProfile.coop3WateringSystem.TileY] = new StaticTile(frontLayer, tilesheet, BlendMode.Alpha, tileIndex: CurrentTroughPlacementProfile.coop3WateringSystem.SystemIndex);
                        break;
                    }
                    case "deluxe barn":
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
                            buildingsLayer.Tiles[CurrentTroughPlacementProfile.barn3WateringSystem.TileX, CurrentTroughPlacementProfile.barn3WateringSystem.TileY] = new StaticTile(buildingsLayer, tilesheet, BlendMode.Alpha, tileIndex: CurrentTroughPlacementProfile.barn3WateringSystem.SystemIndex);
                        else if (CurrentTroughPlacementProfile.barn3WateringSystem.Layer.Equals("Front"))
                            frontLayer.Tiles[CurrentTroughPlacementProfile.barn3WateringSystem.TileX, CurrentTroughPlacementProfile.barn3WateringSystem.TileY] = new StaticTile(frontLayer, tilesheet, BlendMode.Alpha, tileIndex: CurrentTroughPlacementProfile.barn3WateringSystem.SystemIndex);
                        break;
                    }
                }
            }
        }

        private void SendMessageToSelf(object message, string messageType)
        {
            Helper.Multiplayer.SendMessage(message, messageType, new[] { ModManifest.UniqueID });
        }
        
        private bool IsCoop(Building b)
        {
            return b.buildingType.Value.ToLower().Contains("coop");
        }
        private bool IsBarn(Building b)
        {
            return b.buildingType.Value.ToLower().Contains("barn");
        }
    }
}
