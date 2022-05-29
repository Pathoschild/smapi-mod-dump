/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/CustomCompanions
**
*************************************************/

using CustomCompanions.Framework.Assets;
using CustomCompanions.Framework.Companions;
using CustomCompanions.Framework.External.ContentPatcher;
using CustomCompanions.Framework.Interfaces;
using CustomCompanions.Framework.Interfaces.API;
using CustomCompanions.Framework.Managers;
using CustomCompanions.Framework.Models;
using CustomCompanions.Framework.Models.Companion;
using CustomCompanions.Framework.Patches;
using CustomCompanions.Framework.Utilities;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Locations;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CustomCompanions
{
    public class CustomCompanions : Mod
    {
        internal static IMonitor monitor;
        internal static IModHelper modHelper;

        internal const int PERIODIC_CHECK_INTERVAL = 300;
        internal const string COMPANION_KEY = "Companion";
        internal const string TOKEN_HEADER = "CustomCompanions/Companions/";

        private ISaveAnywhereApi _saveAnywhereApi;
        private IJsonAssetsApi _jsonAssetsApi;
        private IContentPatcherAPI _contentPatcherApi;

        private bool areAllModelsValidated;
        private int modelValidationIndex;
        private Dictionary<string, object> trackedModels = new Dictionary<string, object>();

        public override void Entry(IModHelper helper)
        {
            // Set up the monitor and helper
            monitor = Monitor;
            modHelper = Helper;

            // Set up the mod's resources
            this.Reset(true);

            // Load our Harmony patches
            try
            {
                var harmony = new Harmony(this.ModManifest.UniqueID);

                // Apply our patches
                new RingPatch(monitor).Apply(harmony);
                new UtilityPatch(monitor).Apply(harmony);
                new GameLocationPatch(monitor).Apply(harmony);
                new EventPatch(monitor).Apply(harmony);
            }
            catch (Exception e)
            {
                Monitor.Log($"Issue with Harmony patching: {e}", LogLevel.Error);
                return;
            }

            // Add in our debug commands
            helper.ConsoleCommands.Add("cc_spawn", "Spawns in a specific companion.\n\nUsage: cc_spawn [QUANTITY] UNIQUE_ID.COMPANION_NAME [X] [Y]", this.DebugSpawnCompanion);
            helper.ConsoleCommands.Add("cc_clear", "Removes all map-based custom companions at the current location.\n\nUsage: cc_clear", this.DebugClear);
            helper.ConsoleCommands.Add("cc_reload", "Reloads all custom companion content packs. Note: This will remove all spawned companions.\n\nUsage: cc_reload", this.DebugReload);

            // Hook into GameLoop events
            helper.Events.GameLoop.Saving += this.OnSaving;
            helper.Events.GameLoop.DayStarted += this.OnDayStarted;
            helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;
            helper.Events.GameLoop.ReturnedToTitle += this.OnReturnedToTitle;

            // Hook into Player events
            helper.Events.Player.Warped += this.OnWarped;

            // Hook into the Content events
            helper.Events.Content.AssetRequested += this.OnContentAssetRequested;
            helper.Events.Content.AssetsInvalidated += OnContentInvalidated;
        }

        private void OnContentInvalidated(object sender, AssetsInvalidatedEventArgs e)
        {
            foreach (var asset in e.Names.Where(a => trackedModels.ContainsKey(a.Name)))
            {
                var trackedModel = trackedModels[asset.Name];
                var tokenModel = Helper.GameContent.Load<TokenModel>(asset);

                var updatedModel = JsonParser.GetUpdatedModel(trackedModel, tokenModel.Companion);
                if (!JsonParser.CompareSerializedObjects(updatedModel, trackedModel))
                {
                    // Update the existing model object
                    if (CompanionManager.UpdateCompanionModel(JsonParser.Deserialize<CompanionModel>(updatedModel)))
                    {
                        trackedModels[asset.Name] = updatedModel;
                    }
                }
            }
        }

        private void OnContentAssetRequested(object sender, AssetRequestedEventArgs e)
        {
            if (e.DataType == typeof(TokenModel))
            {
                var companionModel = CompanionManager.companionModels.FirstOrDefault(c => e.Name.IsEquivalentTo($"{CustomCompanions.TOKEN_HEADER}{c.GetId()}"));
                if (companionModel is null)
                {
                    return;
                }

                e.LoadFrom(() => new TokenModel() { Companion = JsonParser.Serialize<object>(companionModel) }, AssetLoadPriority.Exclusive, companionModel.Owner);
            }
        }

        public override object GetApi()
        {
            return new Api(this);
        }

        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            // Hook into the APIs we utilize
            if (Helper.ModRegistry.IsLoaded("Omegasis.SaveAnywhere") && ApiManager.HookIntoSaveAnywhere(Helper))
            {
                _saveAnywhereApi = ApiManager.GetSaveAnywhereApi();

                // Hook into save related events
                _saveAnywhereApi.BeforeSave += this.OnSaving;
                _saveAnywhereApi.AfterSave += this.OnCustomLoad; // Save Anywhere's AfterSave event doesn't seem to be triggered, known issue
                _saveAnywhereApi.AfterLoad += this.OnCustomLoad;
            }

            if (Helper.ModRegistry.IsLoaded("bcmpinc.WearMoreRings") && ApiManager.HookIntoIWMR(Helper))
            {
                RingManager.wearMoreRingsApi = ApiManager.GetIWMRApi();
            }

            if (Helper.ModRegistry.IsLoaded("spacechase0.JsonAssets") && ApiManager.HookIntoJsonAssets(Helper))
            {
                _jsonAssetsApi = ApiManager.GetJsonAssetsApi();

                // Hook into IdsAssigned
                _jsonAssetsApi.IdsAssigned += this.IdsAssigned;
            }

            if (Helper.ModRegistry.IsLoaded("Pathoschild.ContentPatcher") && ApiManager.HookIntoContentPatcher(Helper))
            {
                _contentPatcherApi = ApiManager.GetContentPatcherInterface();
                _contentPatcherApi.RegisterToken(ModManifest, "Companions", new CompanionToken());
            }

            // Load any owned content packs
            this.LoadContentPacks();
        }

        private void IdsAssigned(object sender, EventArgs e)
        {
            // Get the ring IDs loaded in by JA from our owned content packs
            foreach (var ring in RingManager.rings)
            {
                int objectID = _jsonAssetsApi.GetObjectId(ring.Name);
                if (objectID == -1)
                {
                    continue;
                }

                ring.ObjectID = objectID;
            }
        }

        private void OnSaving(object sender, EventArgs e)
        {
            // Go through all game locations and purge any of custom creatures
            this.RemoveAllCompanions();
        }

        [EventPriority(EventPriority.Low)]
        private void OnDayStarted(object sender, EventArgs e)
        {
            RingManager.LoadWornRings();

            // Clear out the list of denied companions to respawn
            CompanionManager.denyRespawnCompanions = new List<SceneryCompanions>();

            // Spawn any required companions
            foreach (var location in Game1.locations)
            {
                this.SpawnSceneryCompanions(location, spawnOnlyRequiredCompanions: true);
            }
        }

        private void OnCustomLoad(object sender, EventArgs e)
        {
            this.OnDayStarted(sender, e);

            this.DebugReload(null, null);
        }

        private void OnReturnedToTitle(object sender, ReturnedToTitleEventArgs e)
        {
            this.Reset();
        }

        private void OnWarped(object sender, WarpedEventArgs e)
        {
            // Reset the tracked validation counter
            this.modelValidationIndex = 0;

            // Spawn any map-based companions that are located in this new area
            this.SpawnSceneryCompanions(e.NewLocation);

            // Remove companions that no longer have an existing map tile property
            this.RemoveOrphanCompanions(e.NewLocation);
        }

        private void RemoveOrphanCompanions(GameLocation location)
        {
            if (location.characters != null)
            {
                foreach (var creature in location.characters.Where(c => CompanionManager.IsOrphan(c, location)).ToList())
                {
                    Monitor.Log($"Removing orphan scenery companion {creature.Name} from {location.Name}", LogLevel.Trace);
                    location.characters.Remove(creature);
                }
            }

            if (location is BuildableGameLocation)
            {
                foreach (Building building in (location as BuildableGameLocation).buildings)
                {
                    GameLocation indoorLocation = building.indoors.Value;
                    if (indoorLocation is null)
                    {
                        continue;
                    }

                    if (indoorLocation.characters != null)
                    {
                        foreach (var creature in indoorLocation.characters.Where(c => CompanionManager.IsOrphan(c, location)).ToList())
                        {
                            indoorLocation.characters.Remove(creature);
                        }
                    }
                }
            }
        }

        private void LoadContentPacks(bool isReload = false)
        {
            this.Reset(false, isReload);

            // Load the owned content packs
            foreach (IContentPack contentPack in Helper.ContentPacks.GetOwned())
            {
                Monitor.Log($"Loading companions from pack: {contentPack.Manifest.Name} {contentPack.Manifest.Version} by {contentPack.Manifest.Author}", LogLevel.Debug);

                var companionFolders = new DirectoryInfo(Path.Combine(contentPack.DirectoryPath, "Companions")).GetDirectories();
                if (companionFolders.Count() == 0)
                {
                    Monitor.Log($"No sub-folders found under Companions for the content pack {contentPack.Manifest.Name}!", LogLevel.Warn);
                    continue;
                }

                // Load in the companions
                foreach (var companionFolder in companionFolders)
                {
                    if (!File.Exists(Path.Combine(companionFolder.FullName, "companion.json")))
                    {
                        Monitor.Log($"Content pack {contentPack.Manifest.Name} is missing a companion.json under {companionFolder.Name}!", LogLevel.Warn);
                        continue;
                    }

                    CompanionModel companion = contentPack.ReadJsonFile<CompanionModel>(Path.Combine(companionFolder.Parent.Name, companionFolder.Name, "companion.json"));
                    companion.Name = companion.Name.Replace(" ", "");
                    companion.Owner = contentPack.Manifest.UniqueID;

                    // Save the TileSheet, if one is given
                    if (String.IsNullOrEmpty(companion.TileSheetPath) && !File.Exists(Path.Combine(companionFolder.FullName, "companion.png")))
                    {
                        Monitor.Log($"Unable to add companion {companion.Name} from {contentPack.Manifest.Name}: No associated companion.png or TileSheetPath given", LogLevel.Warn);
                        continue;
                    }
                    else if (String.IsNullOrEmpty(companion.TileSheetPath))
                    {
                        companion.TileSheetPath = contentPack.ModContent.GetInternalAssetName(Path.Combine(companionFolder.Parent.Name, companionFolder.Name, "companion.png")).Name;
                    }

                    // Save the PortraitSheet, if one is given
                    if (companion.Portrait != null)
                    {
                        if (!File.Exists(Path.Combine(companionFolder.FullName, "portrait.png")))
                        {
                            Monitor.Log($"Warning for companion {companion.Name} from {contentPack.Manifest.Name}: Portrait property was given but no portrait.png was found", LogLevel.Warn);
                        }
                        else
                        {
                            companion.PortraitSheetPath = contentPack.ModContent.GetInternalAssetName(Path.Combine(companionFolder.Parent.Name, companionFolder.Name, "portrait.png")).Name;
                        }
                    }

                    if (contentPack.Translation != null)
                    {
                        companion.Translations = contentPack.Translation;
                    }
                    Monitor.Log(companion.ToString(), LogLevel.Trace);

                    // Add the companion to our cache
                    CompanionManager.companionModels.Add(companion);

                    // Cache the full name of the companion, so that it can be reference by a Content Patcher token
                    if (_contentPatcherApi != null)
                    {
                        var assetToken = $"{TOKEN_HEADER}{companion.GetId()}";
                        AssetManager.idToAssetToken[companion.GetId()] = assetToken;

                        if (!isReload)
                        {
                            var modelObject = Helper.GameContent.Load<TokenModel>(assetToken);
                            //var modelObject = AssetManager.GetCompanionModelObject(Helper.Content.Load<Dictionary<string, object>>(assetToken, ContentSource.GameContent));
                            trackedModels[$"{CustomCompanions.TOKEN_HEADER}{companion.GetId()}"] = modelObject.Companion;
                        }
                    }
                }

                if (_jsonAssetsApi != null && !isReload)
                {
                    // Load in the rings that will be paired to a companion
                    if (!Directory.Exists(Path.Combine(contentPack.DirectoryPath, "Objects")))
                    {
                        Monitor.Log($"No summoning rings available from {contentPack.Manifest.Name}, this may be intended", LogLevel.Trace);
                        continue;
                    }

                    foreach (var ringFolder in new DirectoryInfo(Path.Combine(contentPack.DirectoryPath, "Objects")).GetDirectories())
                    {
                        if (!File.Exists(Path.Combine(ringFolder.FullName, "object.json")))
                        {
                            Monitor.Log($"Content pack {contentPack.Manifest.Name} is missing a object.json under {ringFolder.Name}!", LogLevel.Warn);
                            continue;
                        }

                        RingModel ring = contentPack.ReadJsonFile<RingModel>(Path.Combine(ringFolder.Parent.Name, ringFolder.Name, "object.json"));
                        ring.Owner = contentPack.Manifest.UniqueID;

                        RingManager.rings.Add(ring);
                    }

                    // Generate content.json for Json Assets
                    contentPack.WriteJsonFile("content-pack.json", new ContentPackModel
                    {
                        Name = contentPack.Manifest.Name,
                        Author = contentPack.Manifest.Author,
                        Version = contentPack.Manifest.Version.ToString(),
                        Description = contentPack.Manifest.Description,
                        UniqueID = contentPack.Manifest.UniqueID,
                        UpdateKeys = contentPack.Manifest.UpdateKeys,
                    });

                    // Load in the associated rings objects (via JA)
                    _jsonAssetsApi.LoadAssets(contentPack.DirectoryPath);
                }
            }
        }

        private void Reset(bool isFirstRun = false, bool isReload = false)
        {
            if (isFirstRun)
            {
                // Set up the RingManager
                RingManager.rings = new List<RingModel>();
            }

            if (isFirstRun || isReload)
            {
                // Set up the companion models
                CompanionManager.companionModels = new List<CompanionModel>();

                // Set up the dictionary between content pack's manifest IDs to their asset names
                AssetManager.idToAssetToken = new Dictionary<string, string>();
            }

            // Reset the tracked validation counter
            this.modelValidationIndex = 0;

            // Set up the CompanionManager
            CompanionManager.activeCompanions = new List<BoundCompanions>();
            CompanionManager.sceneryCompanions = new List<SceneryCompanions>();
        }

        private void SpawnSceneryCompanions(GameLocation location, bool spawnOnlyRequiredCompanions = false)
        {
            try
            {
                var targetLayer = location.map.GetLayer("Back");
                if (location is FarmHouse)
                {
                    targetLayer = location.map.GetLayer("Front");
                }
                for (int x = 0; x < targetLayer.LayerWidth; x++)
                {
                    for (int y = 0; y < targetLayer.LayerHeight; y++)
                    {
                        var tile = targetLayer.Tiles[x, y];
                        if (tile is null)
                        {
                            continue;
                        }

                        if (tile.Properties.ContainsKey("CustomCompanions"))
                        {
                            if (String.IsNullOrEmpty(tile.Properties["CustomCompanions"]))
                            {
                                if (CompanionManager.sceneryCompanions.Any(s => s.Location == location && s.Tile == new Vector2(x, y)))
                                {
                                    Monitor.Log($"Removing cached SceneryCompanions on tile ({x}, {y}) for map {location.NameOrUniqueName}!", LogLevel.Trace);
                                    CompanionManager.RemoveSceneryCompanionsAtTile(location, new Vector2(x, y));
                                }
                                continue;
                            }

                            string command = tile.Properties["CustomCompanions"].ToString();
                            if (command.Split(' ')[0].ToUpper() != "SPAWN")
                            {
                                if (!String.IsNullOrEmpty(command))
                                {
                                    Monitor.Log($"Unknown CustomCompanions command ({command.Split(' ')[0]}) given on tile ({x}, {y}) for map {location.NameOrUniqueName}!", LogLevel.Warn);
                                }
                                continue;
                            }

                            string companionKey = command.Substring(command.IndexOf(' ') + 2).TrimStart();
                            if (!Int32.TryParse(command.Split(' ')[1], out int amountToSummon))
                            {
                                amountToSummon = 1;
                                companionKey = command.Substring(command.IndexOf(' ') + 1);
                            }

                            var companion = CompanionManager.companionModels.FirstOrDefault(c => String.Concat(c.Owner, ".", c.Name) == companionKey);
                            if (companion is null)
                            {
                                Monitor.Log($"Unable to find companion match for {companionKey} given on tile ({x}, {y}) for map {location.NameOrUniqueName}!", LogLevel.Warn);
                                continue;
                            }

                            if (spawnOnlyRequiredCompanions && !companion.EnableSpawnAtDayStart)
                            {
                                continue;
                            }

                            // Check if it is already spawned
                            if (location.characters.Any(c => CompanionManager.IsSceneryCompanion(c) && (c as MapCompanion).targetTile == new Vector2(x, y) * 64f && (c as MapCompanion).companionKey == companion.GetId()))
                            {
                                continue;
                            }

                            // Check if the companion is allowed to be spawned
                            if (CompanionManager.denyRespawnCompanions.Any(s => s.Location == location && s.Tile == new Vector2(x, y) && s.Companions.Any(c => c.companionKey == companion.GetId())))
                            {
                                continue;
                            }

                            Monitor.Log($"Spawning [{companionKey}] x{amountToSummon} on tile ({x}, {y}) for map {location.NameOrUniqueName}");
                            CompanionManager.SummonCompanions(companion, amountToSummon, new Vector2(x, y), location);
                        }
                    }
                }

                CompanionManager.RefreshLights(location);
            }
            catch (Exception ex)
            {
                Monitor.Log($"Unable to spawn companions on {location.NameOrUniqueName}, likely due to a bad map. See log for more details.", LogLevel.Warn);
                Monitor.Log(ex.ToString());
            }
        }

        private void RemoveAllCompanions(string owner = null, GameLocation targetLocation = null)
        {
            foreach (GameLocation location in Game1.locations.Where(l => l != null && (targetLocation is null || l == targetLocation)))
            {
                if (location.characters != null)
                {
                    foreach (var creature in location.characters.Where(c => CompanionManager.IsCustomCompanion(c) && (owner is null || (c as Companion).model.Owner.Equals(owner, StringComparison.OrdinalIgnoreCase))).ToList())
                    {
                        location.characters.Remove(creature);
                    }
                }


                if (location is BuildableGameLocation)
                {
                    foreach (Building building in (location as BuildableGameLocation).buildings)
                    {
                        GameLocation indoorLocation = building.indoors.Value;
                        if (indoorLocation is null)
                        {
                            continue;
                        }

                        if (indoorLocation.characters != null)
                        {
                            foreach (var creature in indoorLocation.characters.Where(c => CompanionManager.IsCustomCompanion(c) && (owner is null || (c as Companion).model.Owner.Equals(owner, StringComparison.OrdinalIgnoreCase))).ToList())
                            {
                                indoorLocation.characters.Remove(creature);
                            }
                        }
                    }
                }
            }

            this.Reset();
        }

        private void DebugSpawnCompanion(string command, string[] args)
        {
            if (args.Length == 0)
            {
                Monitor.Log($"Missing required arguments: [QUANTITY] UNIQUE_ID.COMPANION_NAME [X] [Y]", LogLevel.Warn);
                return;
            }

            int amountToSummon = 1;
            string companionKey = args[0];
            var targetTile = Game1.player.getTileLocation();
            if (args.Length > 1 && Int32.TryParse(args[0], out int parsedAmountToSummon))
            {
                amountToSummon = parsedAmountToSummon;
                companionKey = args[1];

                if (args.Length > 3 && Int32.TryParse(args[2], out int xTile) && Int32.TryParse(args[3], out int yTile))
                {
                    targetTile += new Vector2(xTile, yTile);
                }
            }
            else if (args.Length > 2 && Int32.TryParse(args[1], out int xTile) && Int32.TryParse(args[2], out int yTile))
            {
                targetTile += new Vector2(xTile, yTile);
            }

            if (!CompanionManager.companionModels.Any(c => String.Concat(c.Owner, ".", c.Name) == companionKey) && !CompanionManager.companionModels.Any(c => String.Concat(c.Name) == companionKey))
            {
                Monitor.Log($"No match found for the companion name {companionKey}.", LogLevel.Warn);
                return;
            }
            if (CompanionManager.companionModels.Where(c => String.Concat(c.Name) == companionKey).Count() > 1)
            {
                Monitor.Log($"There was more than one match to the companion name {companionKey}. Use exact name (UNIQUE_ID.COMPANION_NAME) to resolve this issue.", LogLevel.Warn);
                return;
            }

            var companion = CompanionManager.companionModels.Where(c => String.Concat(c.Name) == companionKey).Count() > 1 ? CompanionManager.companionModels.FirstOrDefault(c => String.Concat(c.Owner, ".", c.Name) == companionKey) : CompanionManager.companionModels.FirstOrDefault(c => String.Concat(c.Name) == companionKey);
            if (companion is null)
            {
                Monitor.Log($"An error has occured trying to spawn {companionKey}: Command failed!", LogLevel.Warn);
                return;
            }

            Monitor.Log($"Spawning {companionKey} x{amountToSummon} at {Game1.currentLocation.NameOrUniqueName} on tile {Game1.player.getTileLocation()}!", LogLevel.Debug);
            CompanionManager.SummonCompanions(companion, amountToSummon, targetTile, Game1.currentLocation);
        }

        private void DebugClear(string command, string[] args)
        {
            this.RemoveAllCompanions(targetLocation: Game1.player.currentLocation);
        }

        private void DebugReload(string command, string[] args)
        {
            this.RemoveAllCompanions();
            this.LoadContentPacks(true);

            // Respawn any previously active companions
            RingManager.LoadWornRings();

            this.SpawnSceneryCompanions(Game1.player.currentLocation);

            // Remove companions that no longer have an existing map tile property
            this.RemoveOrphanCompanions(Game1.player.currentLocation);
        }

        internal void ManualReload(string packUniqueId)
        {
            this.RemoveAllCompanions(owner: packUniqueId);

            // Reset the tracked validation counter
            this.modelValidationIndex = 0;

            // Set up the CompanionManager
            CompanionManager.activeCompanions = CompanionManager.activeCompanions.Where(c => !c.Companions.Any(m => m.model.Owner.Equals(packUniqueId, StringComparison.OrdinalIgnoreCase))).ToList();
            CompanionManager.sceneryCompanions = CompanionManager.sceneryCompanions.Where(c => !c.Companions.Any(m => m.model.Owner.Equals(packUniqueId, StringComparison.OrdinalIgnoreCase))).ToList();


            // Load the owned content packs
            foreach (IContentPack contentPack in Helper.ContentPacks.GetOwned().Where(c => c.Manifest.UniqueID.Equals(packUniqueId, StringComparison.OrdinalIgnoreCase)))
            {
                Monitor.Log($"Loading companions from pack: {contentPack.Manifest.Name} {contentPack.Manifest.Version} by {contentPack.Manifest.Author}", LogLevel.Debug);

                var companionFolders = new DirectoryInfo(Path.Combine(contentPack.DirectoryPath, "Companions")).GetDirectories();
                if (companionFolders.Count() == 0)
                {
                    Monitor.Log($"No sub-folders found under Companions for the content pack {contentPack.Manifest.Name}!", LogLevel.Warn);
                    continue;
                }

                // Load in the companions
                foreach (var companionFolder in companionFolders)
                {
                    if (!File.Exists(Path.Combine(companionFolder.FullName, "companion.json")))
                    {
                        Monitor.Log($"Content pack {contentPack.Manifest.Name} is missing a companion.json under {companionFolder.Name}!", LogLevel.Warn);
                        continue;
                    }

                    CompanionModel companion = contentPack.ReadJsonFile<CompanionModel>(Path.Combine(companionFolder.Parent.Name, companionFolder.Name, "companion.json"));
                    companion.Name = companion.Name.Replace(" ", "");
                    companion.Owner = contentPack.Manifest.UniqueID;
                    Monitor.Log(companion.ToString(), LogLevel.Trace);

                    // Save the TileSheet, if one is given
                    if (String.IsNullOrEmpty(companion.TileSheetPath) && !File.Exists(Path.Combine(companionFolder.FullName, "companion.png")))
                    {
                        Monitor.Log($"Unable to add companion {companion.Name} from {contentPack.Manifest.Name}: No associated companion.png or TileSheetPath given", LogLevel.Warn);
                        continue;
                    }
                    else if (String.IsNullOrEmpty(companion.TileSheetPath))
                    {
                        companion.TileSheetPath = contentPack.ModContent.GetInternalAssetName(Path.Combine(companionFolder.Parent.Name, companionFolder.Name, "companion.png")).Name;
                    }

                    // Save the PortraitSheet, if one is given
                    if (companion.Portrait != null)
                    {
                        if (!File.Exists(Path.Combine(companionFolder.FullName, "portrait.png")))
                        {
                            Monitor.Log($"Warning for companion {companion.Name} from {contentPack.Manifest.Name}: Portrait property was given but no portrait.png was found", LogLevel.Warn);
                        }
                        else
                        {
                            companion.PortraitSheetPath = contentPack.ModContent.GetInternalAssetName(Path.Combine(companionFolder.Parent.Name, companionFolder.Name, "portrait.png")).Name;
                        }
                    }

                    if (contentPack.Translation != null)
                    {
                        companion.Translations = contentPack.Translation;
                    }

                    // Cache the full name of the companion, so that it can be reference by a Content Patcher token
                    if (_contentPatcherApi != null)
                    {
                        var assetToken = $"{TOKEN_HEADER}{companion.GetId()}";
                        AssetManager.idToAssetToken[companion.GetId()] = assetToken;

                        var modelObject = Helper.GameContent.Load<TokenModel>(assetToken);
                        //var modelObject = AssetManager.GetCompanionModelObject(Helper.Content.Load<Dictionary<string, object>>(assetToken, ContentSource.GameContent));
                        trackedModels[$"{CustomCompanions.TOKEN_HEADER}{companion.GetId()}"] = modelObject.Companion;
                    }
                }
            }

            this.SpawnSceneryCompanions(Game1.player.currentLocation);

            // Remove companions that no longer have an existing map tile property
            this.RemoveOrphanCompanions(Game1.player.currentLocation);
        }

        internal static bool IsSoundValid(string soundName, bool logFailure = false)
        {
            try
            {
                Game1.soundBank.GetCue(soundName);
            }
            catch (Exception)
            {
                if (logFailure)
                {
                    monitor.Log($"Attempted to get a sound that doesn't exist: {soundName}", LogLevel.Debug);
                }

                return false;
            }

            return true;
        }

        internal static bool CompanionHasFullMovementSet(CompanionModel model)
        {
            if (model.UpAnimation is null)
            {
                return false;
            }
            if (model.RightAnimation is null)
            {
                return false;
            }
            if (model.DownAnimation is null)
            {
                return false;
            }
            if (model.LeftAnimation is null)
            {
                return false;
            }

            return true;
        }

        internal static Color GetColorFromArray(int[] colorArray)
        {
            if (colorArray.Length < 3)
            {
                return Color.White;
            }

            // Verify alpha is given
            int alpha = 255;
            if (colorArray.Length >= 4)
            {
                alpha = colorArray[3];
            }

            return new Color(colorArray[0], colorArray[1], colorArray[2], alpha);
        }
    }
}
