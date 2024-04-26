/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/SolidFoundations
**
*************************************************/

using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SolidFoundations.Framework.Extensions;
using SolidFoundations.Framework.External.ContentPatcher;
using SolidFoundations.Framework.Interfaces.Internal;
using SolidFoundations.Framework.Managers;
using SolidFoundations.Framework.Models.ContentPack;
using SolidFoundations.Framework.Models.ContentPack.Compatibility;
using SolidFoundations.Framework.Patches.Buildings;
using SolidFoundations.Framework.Patches.GameData;
using SolidFoundations.Framework.Utilities;
using SolidFoundations.Framework.Utilities.Extensions;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.GameData.Buildings;
using System;
using System.IO;
using System.Linq;
using xTile;

namespace SolidFoundations
{
    public class SolidFoundations : Mod
    {
        // Shared static helpers
        internal static Api api;
        internal static IMonitor monitor;
        internal static IModHelper modHelper;
        internal static Multiplayer multiplayer;

        // Managers
        internal static ApiManager apiManager;
        internal static BuildingManager buildingManager;
        internal static LightManager lightManager;

        public override void Entry(IModHelper helper)
        {
            // Validate that the game version is compatible
            if (IsGameVersionCompatible() is false)
            {
                Monitor.Log($"This version of Solid Foundations (v{ModManifest.Version}) is not compatible with Stardew Valley v{Game1.version}.\nSolid Foundations buildings will not be loaded.\nDownload the latest version of Solid Foundations to resolve this issue.", LogLevel.Error);
                return;
            }

            // Set up the monitor, helper and multiplayer
            api = new Api();
            monitor = Monitor;
            modHelper = helper;
            multiplayer = helper.Reflection.GetField<Multiplayer>(typeof(Game1), "multiplayer").GetValue();

            // Set up the managers
            apiManager = new ApiManager(monitor);
            buildingManager = new BuildingManager(monitor, helper);
            lightManager = new LightManager(monitor, helper);

            // Load our Harmony patches
            try
            {
                var harmony = new Harmony(this.ModManifest.UniqueID);

                // Apply building patches
                new BuildingPatch(monitor, helper).Apply(harmony);

                // Apply GameData patches
                new BuildingDataPatch(monitor, helper).Apply(harmony);

                // Apply language patches
                new LocalizedContentManagerPatch(monitor, helper).Apply(harmony);

                // Apply object patch
                new ChestPatch(monitor, helper).Apply(harmony);
            }
            catch (Exception e)
            {
                Monitor.Log($"Issue with Harmony patching: {e}", LogLevel.Error);
                return;
            }

            // Add in the debug commands
            helper.ConsoleCommands.Add("sf_reload", "Reloads all Solid Foundations content packs.\n\nUsage: sf_reload", delegate { this.LoadContentPacks(); this.RefreshAllCustomBuildings(); });
            helper.ConsoleCommands.Add("sf_place_building", "Adds a building in the current location at given tile.\n\nUsage: sf_place_building MODEL_ID TILE_X TILE_Y", this.PlaceBuildingAtTile);

            // Hook into the required events
            helper.Events.Content.AssetReady += OnAssetReady;
            helper.Events.Content.AssetRequested += OnAssetRequested;

            helper.Events.GameLoop.GameLaunched += OnGameLaunched;
            helper.Events.GameLoop.DayStarted += OnDayStarted;
            helper.Events.GameLoop.Saved += OnSaved;

            helper.Events.World.BuildingListChanged += OnBuildingListChanged;
        }

        private bool IsGameVersionCompatible()
        {
            var requiredMinimumVersion = new Version("1.6.0");
            var gameVersion = new Version(Game1.version);

            return gameVersion >= requiredMinimumVersion;
        }

        private void OnBuildingListChanged(object sender, BuildingListChangedEventArgs e)
        {
            foreach (var building in e.Added)
            {
                building.ResetLights(e.Location);
            }

            foreach (var building in e.Removed)
            {
                building.ClearLightSources(e.Location);
            }
        }

        private void OnSaved(object sender, SavedEventArgs e)
        {
            // Handle retiring any the obsolete SolidFoundations\building.json cache
            SaveMigration.RetireBuildingCache();
        }

        [EventPriority(EventPriority.High + 1)]
        private void OnDayStarted(object sender, DayStartedEventArgs e)
        {
            // Handle loading the obsolete SolidFoundations\building.json cache
            SaveMigration.LoadBuildingCache(monitor, buildingManager);
        }

        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            // Hook into the APIs we utilize
            if (Helper.ModRegistry.IsLoaded("Pathoschild.ContentPatcher") && apiManager.HookIntoContentPatcher(Helper))
            {
                apiManager.GetContentPatcherApi().RegisterToken(ModManifest, "IsBuildingHere", new IsBuildingHereToken());
                apiManager.GetContentPatcherApi().RegisterToken(ModManifest, "BuildingTexture", new BuildingTextureToken());
            }
            if (Helper.ModRegistry.IsLoaded("Cherry.ShopTileFramework") && apiManager.HookIntoShopTileFramework(Helper))
            {
                // Do nothing
            }
            if (Helper.ModRegistry.IsLoaded("Omegasis.SaveAnywhere") && apiManager.HookIntoSaveAnywhere(Helper))
            {
                var saveAnywhereApi = apiManager.GetSaveAnywhereApi();

                // Hook into save related events
                saveAnywhereApi.AfterLoad += delegate { SaveMigration.LoadBuildingCache(monitor, buildingManager); };
            }

            // Load any owned content packs
            LoadContentPacks();

            // Register our game state queries
            GameStateQueries.Register();
        }

        private void OnAssetReady(object sender, AssetReadyEventArgs e)
        {
            if (e.Name.IsEquivalentTo("Data/Buildings") is false)
            {
                return;
            }

            // Correct the DrawLayers.Texture and Skins.Texture ids
            foreach (ExtendedBuildingModel actualModel in buildingManager.GetAllBuildingModels())
            {
                if (actualModel.DrawLayers is not null)
                {
                    foreach (var layer in actualModel.DrawLayers.Where(t => String.IsNullOrEmpty(t.Texture) is false))
                    {
                        var spriteAsset = $"{actualModel.ID}_Sprites_{Path.GetFileNameWithoutExtension(layer.Texture)}";
                        if (buildingManager.GetTextureAsset(spriteAsset) is not null)
                        {
                            layer.Texture = spriteAsset;
                        }
                    }
                }

                if (actualModel.Skins is not null)
                {
                    foreach (var skin in actualModel.Skins.Where(t => String.IsNullOrEmpty(t.Texture) is false))
                    {
                        var skinAsset = $"{actualModel.ID}_Skins_{Path.GetFileNameWithoutExtension(skin.Texture)}";
                        if (buildingManager.GetTextureAsset(skinAsset) is not null)
                        {
                            skin.Texture = skinAsset;
                        }
                    }
                }

                buildingManager.AddBuilding(actualModel);
            }
        }

        private void OnAssetRequested(object sender, AssetRequestedEventArgs e)
        {
            if (e.NameWithoutLocale.IsEquivalentTo("Data/Buildings"))
            {
                e.Edit(asset =>
                {
                    foreach (var idToModel in buildingManager.GetIdToModels())
                    {
                        asset.AsDictionary<string, BuildingData>().Data[idToModel.Key] = idToModel.Value;
                    }
                });
            }
            else if (e.NameWithoutLocale.IsEquivalentTo("Data/PaintData"))
            {
                e.Edit(asset =>
                {
                    var data = asset.AsDictionary<string, string>().Data;

                    // Add any building masks
                    var models = buildingManager.GetAllBuildingModels();
                    foreach (var model in models.Where(m => m.PaintMasks is not null))
                    {
                        string parsedMaskText = null;
                        foreach (var mask in model.PaintMasks)
                        {
                            parsedMaskText += $"{mask.Name}/{mask.MinBrightness} {mask.MaxBrightness}/";
                        }

                        if (String.IsNullOrEmpty(parsedMaskText) is false)
                        {
                            data[model.ID] = parsedMaskText;
                        }
                    }

                    // Add any building skin masks
                    foreach (var skin in models.Where(m => m.Skins is not null).SelectMany(m => m.Skins))
                    {
                        if (skin.PaintMasks is null)
                        {
                            continue;
                        }

                        string parsedMaskText = null;
                        foreach (var mask in skin.PaintMasks)
                        {
                            parsedMaskText += $"{mask.Name}/{mask.MinBrightness} {mask.MaxBrightness}/";
                        }

                        if (String.IsNullOrEmpty(parsedMaskText) is false)
                        {
                            data[skin.Id] = parsedMaskText;
                        }
                    }
                });
            }
            else if (e.DataType == typeof(Texture2D))
            {
                var asset = e.Name;
                if (buildingManager.GetTextureAsset(asset.Name) is var texture && texture is not null)
                {
                    var clonedTexture = texture.CreateSelectiveCopy(Game1.graphics.GraphicsDevice, new Microsoft.Xna.Framework.Rectangle(0, 0, texture.Width, texture.Height));
                    e.LoadFrom(() => clonedTexture, AssetLoadPriority.Exclusive);
                }
                else if (buildingManager.GetTileSheetAsset(asset.Name) is var tileSheetPath && tileSheetPath is not null)
                {
                    e.LoadFrom(() => Helper.ModContent.Load<Texture2D>(tileSheetPath), AssetLoadPriority.Exclusive);
                }
            }
            else if (e.DataType == typeof(Map))
            {
                var asset = e.Name;
                if (buildingManager.GetMapAsset(asset.Name) is var mapPath && mapPath is not null)
                {
                    e.LoadFrom(() => Helper.GameContent.Load<Map>(mapPath), AssetLoadPriority.Exclusive);
                }
            }
        }

        public override object GetApi()
        {
            return api;
        }

        private void LoadContentPacks(bool silent = false)
        {
            // Clear the existing cache of custom buildings
            buildingManager.Reset();

            // Load owned content packs
            foreach (IContentPack contentPack in Helper.ContentPacks.GetOwned())
            {
                try
                {
                    Monitor.Log($"Loading data from pack: {contentPack.Manifest.Name} {contentPack.Manifest.Version} by {contentPack.Manifest.Author}", silent ? LogLevel.Trace : LogLevel.Debug);

                    // Load interiors
                    Monitor.Log($"Loading interiors from pack: {contentPack.Manifest.Name}", LogLevel.Trace);
                    LoadInteriors(contentPack, silent);

                    // Load the buildings
                    Monitor.Log($"Loading buildings from pack: {contentPack.Manifest.Name}", LogLevel.Trace);
                    LoadBuildings(contentPack, silent);
                }
                catch (Exception ex)
                {
                    Monitor.Log($"Failed to load the content pack {contentPack.Manifest.UniqueID}: {ex}", LogLevel.Warn);
                }
            }
        }

        private void LoadInteriors(IContentPack contentPack, bool silent)
        {
            try
            {
                var directoryPath = new DirectoryInfo(Path.Combine(contentPack.DirectoryPath, "Interiors"));
                if (!directoryPath.Exists)
                {
                    Monitor.Log($"No Interiors folder found for the content pack {contentPack.Manifest.Name}", LogLevel.Trace);
                    return;
                }

                var interiorFiles = directoryPath.GetFiles("*.tmx", SearchOption.AllDirectories);
                if (interiorFiles.Count() == 0)
                {
                    Monitor.Log($"No TMX files found under Interiors for the content pack {contentPack.Manifest.Name}", LogLevel.Warn);
                    return;
                }

                var parentFolderName = "Interiors";
                foreach (var interiorFile in interiorFiles)
                {
                    // Cache the interior map
                    if (interiorFile.Exists)
                    {
                        var mapPath = contentPack.ModContent.GetInternalAssetName(Path.Combine(parentFolderName, interiorFile.Name)).Name;
                        buildingManager.AddMapAsset(Path.GetFileNameWithoutExtension(interiorFile.Name), mapPath);

                        Monitor.Log($"Loaded the interior {mapPath}", LogLevel.Trace);
                    }
                    else
                    {
                        Monitor.Log($"Unable to add interior for {interiorFile.FullName} from {contentPack.Manifest.Name}!", LogLevel.Warn);
                        continue;
                    }
                }

                var tilesheetFiles = directoryPath.GetFiles("*.png", SearchOption.AllDirectories);
                if (tilesheetFiles.Count() == 0)
                {
                    Monitor.Log($"No tilesheets found under Interiors for the content pack {contentPack.Manifest.Name}", LogLevel.Trace);
                    return;
                }

                foreach (var tileSheetFile in tilesheetFiles)
                {
                    // Cache the interior map
                    if (tileSheetFile.Exists)
                    {
                        var tilesheetPath = contentPack.ModContent.GetInternalAssetName(tileSheetFile.FullName).Name;
                        var tilesheetPathWithoutExtension = tilesheetPath.Replace(".png", null);
                        buildingManager.AddTileSheetAsset(tilesheetPath, tileSheetFile.FullName);

                        if (tilesheetPath != tilesheetPathWithoutExtension)
                        {
                            buildingManager.AddTileSheetAsset(tilesheetPathWithoutExtension, tileSheetFile.FullName);
                        }

                        Monitor.Log($"Loaded the tilesheet {tilesheetPath} | {tilesheetPathWithoutExtension}", LogLevel.Trace);
                    }
                    else
                    {
                        Monitor.Log($"Unable to add tilesheet for {tileSheetFile.FullName} from {contentPack.Manifest.Name}!", LogLevel.Warn);
                        continue;
                    }
                }
            }
            catch (Exception ex)
            {
                Monitor.Log($"Error loading interiors / tilesheets from content pack {contentPack.Manifest.Name}: {ex}", LogLevel.Error);
            }
        }

        private void LoadBuildings(IContentPack contentPack, bool silent)
        {
            try
            {
                var directoryPath = new DirectoryInfo(Path.Combine(contentPack.DirectoryPath, "Buildings"));
                if (!directoryPath.Exists)
                {
                    Monitor.Log($"No Buildings folder found for the content pack {contentPack.Manifest.Name}", LogLevel.Trace);
                    return;
                }

                var buildingsFolder = directoryPath.GetDirectories("*", SearchOption.TopDirectoryOnly);
                if (buildingsFolder.Count() == 0)
                {
                    Monitor.Log($"No sub-folders found under Buildings for the content pack {contentPack.Manifest.Name}", LogLevel.Warn);
                    return;
                }

                // Load in the buildings
                int modelsWithCompatibilityIssues = 0;
                foreach (var folder in buildingsFolder)
                {
                    bool isUsingStringBasedRectangles = false;
                    if (!File.Exists(Path.Combine(folder.FullName, "building.json")))
                    {
                        if (folder.GetDirectories().Count() == 0)
                        {
                            Monitor.Log($"Content pack {contentPack.Manifest.Name} is missing a building.json under {folder.Name}", LogLevel.Warn);
                        }

                        continue;
                    }

                    var parentFolderName = folder.Parent.FullName.Replace(contentPack.DirectoryPath + Path.DirectorySeparatorChar, String.Empty);
                    var modelPath = Path.Combine(parentFolderName, folder.Name, "building.json");

                    // Parse the model and assign it the content pack's owner
                    ExtendedBuildingModel buildingModel = null;
                    try
                    {
                        buildingModel = contentPack.ReadJsonFile<ExtendedBuildingModel>(modelPath);
                    }
                    catch (Newtonsoft.Json.JsonReaderException)
                    {
                        // Alert that the pack is using string-based Rectangles
                        Monitor.Log($"Attempting to resolve ExtendedBuildingModel read issue by handling string-based Rectangles for {modelPath}", LogLevel.Trace);

                        // Attempt to handle string-based Rectangle classes (SourceRect / AnimalDoor)
                        buildingModel = contentPack.ReadJsonFile<OldExtendedBuildingModel>(modelPath);

                        isUsingStringBasedRectangles = true;
                        Monitor.Log($"{buildingModel.ID} is using the outdated string-based Rectangles for the \"SourceRect\" or \"AnimalDoor\" properties. Solid Foundations will handle this, though the value should be changed to use the recommended formatting for compatibility.", LogLevel.Trace);
                    }

                    // Verify the required Name property is set
                    if (String.IsNullOrEmpty(buildingModel.Name))
                    {
                        Monitor.Log($"Unable to add building from content pack {contentPack.Manifest.Name}: Missing the Name property", LogLevel.Warn);
                        continue;
                    }

                    if (buildingModel is null)
                    {
                        Monitor.Log($"Unable to add building from {modelPath}: The model is invalid", LogLevel.Warn);
                        continue;
                    }

                    // Set the PackName and Id
                    buildingModel.PackName = contentPack.Manifest.Name;
                    buildingModel.Owner = contentPack.Manifest.UniqueID;

                    if (String.IsNullOrEmpty(buildingModel.ID))
                    {
                        buildingModel.ID = String.Concat(buildingModel.Owner, "_", buildingModel.Name.Replace(" ", String.Empty));
                    }

                    // Verify that a building with the name doesn't exist in this pack
                    if (buildingManager.GetSpecificBuildingModel(buildingModel.ID) != null)
                    {
                        Monitor.Log($"Unable to add building from {contentPack.Manifest.Name}: This pack already contains a building with the name of {buildingModel.Name}", LogLevel.Warn);
                        continue;
                    }

                    // Verify we are given a texture and if so, track it
                    if (!File.Exists(Path.Combine(folder.FullName, "building.png")))
                    {
                        Monitor.Log($"Unable to add building for {buildingModel.Name} from {contentPack.Manifest.Name}: No associated building.png given", LogLevel.Warn);
                        continue;
                    }

                    // Load in the paint mask, if given
                    if (File.Exists(Path.Combine(folder.FullName, "paint_mask.png")))
                    {
                        var paintMaskPath = Path.Combine(parentFolderName, folder.Name, "paint_mask.png");
                        buildingModel.PaintMaskTexture = $"{buildingModel.ID}_BaseTexture_PaintMask";
                        buildingManager.AddTextureAsset(buildingModel.PaintMaskTexture, contentPack.ModContent.GetInternalAssetName(paintMaskPath).Name, contentPack);

                        Monitor.Log($"Loaded the building {buildingModel.ID} PaintMask texture: {buildingModel.PaintMaskTexture} | {paintMaskPath}", LogLevel.Trace);
                    }

                    // Load in any skins, if given
                    if (Directory.Exists(Path.Combine(folder.FullName, "Skins")))
                    {
                        foreach (var skinPath in Directory.GetFiles(Path.Combine(folder.FullName, "Skins"), "*.png").OrderBy(s => s))
                        {
                            var localSkinPath = Path.Combine(parentFolderName, folder.Name, "Skins", Path.GetFileName(skinPath));
                            buildingManager.AddTextureAsset($"{buildingModel.ID}_Skins_{Path.GetFileNameWithoutExtension(skinPath)}", contentPack.ModContent.GetInternalAssetName(localSkinPath).Name, contentPack);
                        }

                        if (buildingModel.Skins is not null)
                        {
                            foreach (var skin in buildingModel.Skins.Where(t => String.IsNullOrEmpty(t.Texture) is false))
                            {
                                var skinAsset = $"{buildingModel.ID}_Skins_{Path.GetFileNameWithoutExtension(skin.Texture)}";
                                if (buildingManager.GetTextureAsset(skinAsset) is null)
                                {
                                    Monitor.Log($"Unable to find the skin {skin.Texture} under Skins for {buildingModel.Name} from {contentPack.Manifest.Name}, skipping.", LogLevel.Trace);
                                    continue;
                                }

                                if (String.IsNullOrEmpty(skin.PaintMaskTexture) is false)
                                {
                                    var maskAsset = $"{buildingModel.ID}_Skins_{Path.GetFileNameWithoutExtension(skin.PaintMaskTexture)}";
                                    if (buildingManager.GetTextureAsset(maskAsset) is null)
                                    {
                                        Monitor.Log($"Unable to find the skin mask {skin.PaintMaskTexture} under Skins for {buildingModel.Name} from {contentPack.Manifest.Name}, skipping.", LogLevel.Trace);
                                    }
                                    else
                                    {
                                        skin.PaintMaskTexture = $"{skinAsset}_PaintMask";
                                        buildingManager.AddTextureAsset(skin.PaintMaskTexture, buildingManager.GetTextureAssetPath(maskAsset), contentPack);

                                        Monitor.Log($"Loaded the building {buildingModel.ID} skin {skin.Id} mask texture: {skin.PaintMaskTexture}", LogLevel.Trace);
                                    }
                                }
                                else if (File.Exists(Path.Combine(folder.FullName, "paint_mask.png")))
                                {
                                    skin.PaintMaskTexture = $"{skinAsset}_PaintMask";
                                    buildingManager.AddTextureAsset(skin.PaintMaskTexture, contentPack.ModContent.GetInternalAssetName(Path.Combine(parentFolderName, folder.Name, "paint_mask.png")).Name, contentPack);

                                    Monitor.Log($"Loaded the building {buildingModel.ID} skin {skin.Id} mask texture using the default paint mask", LogLevel.Trace);
                                }

                                skin.Texture = skinAsset;
                                Monitor.Log($"Loaded the building {buildingModel.ID} Skins texture: {skin.Texture} | {skinAsset}", LogLevel.Trace);
                            }
                        }
                    }

                    // Load in the sprites for DrawLayers, if given
                    if (Directory.Exists(Path.Combine(folder.FullName, "Sprites")))
                    {
                        foreach (var spritePath in Directory.GetFiles(Path.Combine(folder.FullName, "Sprites"), "*.png"))
                        {
                            var localSpritePath = Path.Combine(parentFolderName, folder.Name, "Sprites", Path.GetFileName(spritePath));
                            buildingManager.AddTextureAsset($"{buildingModel.ID}_Sprites_{Path.GetFileNameWithoutExtension(spritePath)}", contentPack.ModContent.GetInternalAssetName(localSpritePath).Name, contentPack);
                        }

                        if (buildingModel.DrawLayers is not null)
                        {
                            foreach (var layer in buildingModel.DrawLayers.Where(t => String.IsNullOrEmpty(t.Texture) is false))
                            {
                                var spriteAsset = $"{buildingModel.ID}_Sprites_{Path.GetFileNameWithoutExtension(layer.Texture)}";
                                if (buildingManager.GetTextureAsset(spriteAsset) is null)
                                {
                                    Monitor.Log($"Unable to find the texture {layer.Texture} under Sprites for {buildingModel.Name} from {contentPack.Manifest.Name}, assuming to be vanilla or a texture loaded via Content Patcher.", LogLevel.Trace);
                                    continue;
                                }

                                layer.Texture = spriteAsset;
                                Monitor.Log($"Loaded the building {buildingModel.ID} Sprites texture: {layer.Texture} | {spriteAsset}", LogLevel.Trace);
                            }
                        }
                    }

                    // Load in the texture
                    var texturePath = Path.Combine(parentFolderName, folder.Name, "building.png");
                    buildingModel.Texture = $"{buildingModel.ID}_BaseTexture";
                    buildingManager.AddTextureAsset(buildingModel.Texture, contentPack.ModContent.GetInternalAssetName(texturePath).Name, contentPack);

                    Monitor.Log($"Loaded the building texture {buildingModel.Texture} | {texturePath}", LogLevel.Trace);

                    // Handle setting HumanDoor, if AuxiliaryHumanDoors is populated but the former isn't
                    if (buildingModel.HumanDoor.X == -1 && buildingModel.HumanDoor.Y == -1 && buildingModel.AuxiliaryHumanDoors.Count > 0)
                    {
                        buildingModel.HumanDoor = buildingModel.AuxiliaryHumanDoors.First();
                    }

                    // Load in any provided translations
                    if (contentPack.Translation is not null)
                    {
                        buildingModel.Translations = contentPack.Translation;

                        // Preserve the translation keys for name / description
                        buildingModel.NameTranslationKey = buildingModel.Name;
                        buildingModel.DescriptionTranslationKey = buildingModel.Description;

                        buildingModel.Name = buildingModel.GetTranslation(buildingModel.NameTranslationKey);
                        buildingModel.Description = buildingModel.GetTranslation(buildingModel.DescriptionTranslationKey);
                    }

                    // Check for any compatibility issues
                    if (HandleCompatibilityIssues(buildingModel) || isUsingStringBasedRectangles)
                    {
                        modelsWithCompatibilityIssues += 1;
                    }

                    // Track the model
                    buildingManager.AddBuilding(buildingModel);

                    // Log it
                    Monitor.Log($"Loaded the building {buildingModel.ID}", LogLevel.Trace);
                }

                // Display a warning that the pack will work, but has potential issues if used without Solid Foundations
                if (modelsWithCompatibilityIssues > 0)
                {
                    Monitor.Log($"There were compatibility issues that were handled for {modelsWithCompatibilityIssues} {(modelsWithCompatibilityIssues > 1 ? "models" : "model")} within {contentPack.Manifest.Name}. See the log for details.", silent ? LogLevel.Trace : LogLevel.Warn);
                }
            }
            catch (Exception ex)
            {
                Monitor.Log($"Error loading buildings from content pack {contentPack.Manifest.Name}: {ex}", LogLevel.Error);
            }
        }

        private bool HandleCompatibilityIssues(ExtendedBuildingModel model)
        {
            if (model is null)
            {
                return false;
            }

            bool hasCompatibilityIssue = false;
            if (string.Equals(model.Builder, "Carpenter", StringComparison.OrdinalIgnoreCase))
            {
                Monitor.Log($"[COMPATIBILITY] {model.ID} is using the outdated value \"Carpenter\" for the \"Builder\" property. Solid Foundations will handle this, though the value should be changed to \"Robin\" for compatibility.", LogLevel.Trace);

                model.Builder = "Robin";
                hasCompatibilityIssue = true;
            }
            else if (model.MagicalConstruction is null && string.Equals(model.Builder, "Wizard", StringComparison.OrdinalIgnoreCase))
            {
                Monitor.Log($"[COMPATIBILITY] {model.ID} is using the value \"Wizard\" for the \"Builder\" property, but has not declared the \"MagicalConstruction\" property. Solid Foundations will infer \"MagicalConstruction\" as true, though for compatibility this should be set manually.", LogLevel.Trace);

                model.MagicalConstruction = true;
                hasCompatibilityIssue = true;
            }

            if (string.Equals(model.IndoorMapType, "StardewValley.Locations.BuildableGameLocation", StringComparison.OrdinalIgnoreCase))
            {
                Monitor.Log($"[COMPATIBILITY] {model.ID} is using an outdated value \"StardewValley.Locations.BuildableGameLocation\" for the \"IndoorMapType\" property. Solid Foundations will handle this, though the value should be changed to \"StardewValley.GameLocation\" and the map property \"CanBuildHere\" should be set for compatibility.", LogLevel.Trace);

                model.IndoorMapType = "StardewValley.GameLocation";

                model.ForceLocationToBeBuildable = true;
                hasCompatibilityIssue = true;
            }

            if (model.DrawLayers is not null && model.DrawLayers.Any(l => l.DrawBehindBase is true))
            {
                Monitor.Log($"[COMPATIBILITY] {model.ID} is using an outdated property \"DrawBehindBase\". Solid Foundations will handle this, though property should be changed to \"DrawInBackground\".", LogLevel.Trace);

                foreach (var layer in model.DrawLayers.Where(l => l.DrawBehindBase is true))
                {
                    layer.DrawInBackground = true;
                }
                hasCompatibilityIssue = true;
            }

            return hasCompatibilityIssue;
        }

        private void RefreshAllCustomBuildings()
        {
            foreach (GameLocation location in Game1.locations.Where(l => l.buildings is not null))
            {
                foreach (Building building in location.buildings.Where(b => buildingManager.DoesBuildingModelExist(b.buildingType.Value)))
                {
                    building.RefreshModel(buildingManager.GetSpecificBuildingModel(building.buildingType.Value));
                }
            }
        }

        private void PlaceBuildingAtTile(string command, string[] args)
        {
            if (args.Length < 3)
            {
                Monitor.Log($"Missing required arguments: X Y", LogLevel.Warn);
                return;
            }

            var targetTile = Game1.player.Tile;
            if (args.Length > 2 && Int32.TryParse(args[1], out int xTile) && Int32.TryParse(args[2], out int yTile))
            {
                targetTile = new Vector2(xTile, yTile);
            }

            monitor.Log(api.ConstructBuildingImmediately(args[0], Game1.currentLocation, targetTile).Value.ToString(), LogLevel.Debug);
        }
    }
}
