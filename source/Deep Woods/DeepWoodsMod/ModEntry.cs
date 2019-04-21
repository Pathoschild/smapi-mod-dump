using System;
using System.Collections.Generic;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Locations;
using xTile.Layers;
using xTile.Tiles;
using static DeepWoodsMod.DeepWoodsSettings;
using static DeepWoodsMod.DeepWoodsGlobals;
using System.Collections.Concurrent;
using Microsoft.Xna.Framework;
using System.Linq;
using Omegasis.SaveAnywhere.API;
using DeepWoodsMod.API.Impl;
using DeepWoodsMod.Framework.Messages;

namespace DeepWoodsMod
{
    public class ModEntry : Mod, IAssetEditor, IAssetLoader
    {
        private static DeepWoodsAPI api = new DeepWoodsAPI();
        private static ModEntry mod;
        private static Multiplayer multiplayer;

        private bool isDeepWoodsGameRunning = false;
        private Dictionary<long, GameLocation> playerLocations = new Dictionary<long, GameLocation>();

        private static ConcurrentQueue<string> queuedErrorMessages = new ConcurrentQueue<string>();

        private static void WorkErrorMessageQueue()
        {
            string msg;
            while (queuedErrorMessages.TryDequeue(out msg))
            {
                Log(msg, LogLevel.Error);
            }
        }

        public static void Log(string message, LogLevel level = LogLevel.Trace)
        {
            ModEntry.mod?.Monitor?.Log(message, level);
        }

        public static void SendMessage<T>(T message, string messageType, long playerID)
        {
            ModEntry.mod.Helper.Multiplayer.SendMessage(message, messageType, modIDs: new [] { ModEntry.mod.ModManifest.UniqueID }, playerIDs: new [] { playerID });
        }

        public static void SendMessage(string messageType, long playerID)
        {
            ModEntry.SendMessage(true, messageType, playerID);
        }

        public static IReflectionHelper GetReflection()
        {
            return ModEntry.mod?.Helper?.Reflection;
        }

        public static IModHelper GetHelper()
        {
            return ModEntry.mod?.Helper;
        }

        public static Multiplayer GetMultiplayer()
        {
            return ModEntry.multiplayer;
        }

        public override void Entry(IModHelper helper)
        {
            ModEntry.mod = this;
            RegisterEvents(helper.Events);
        }

        public override object GetApi()
        {
            return ModEntry.GetAPI();
        }

        public static DeepWoodsAPI GetAPI()
        {
            return api;
        }

        private void RegisterEvents(IModEvents events)
        {
            events.GameLoop.Saving += this.OnSaving;
            events.GameLoop.Saved += this.OnSaved;
            events.GameLoop.SaveLoaded += this.OnSaveLoaded;
            events.GameLoop.ReturnedToTitle += this.OnReturnedToTitle;
            events.GameLoop.DayStarted += this.OnDayStarted;
            events.GameLoop.TimeChanged += this.OnTimeChanged;
            events.GameLoop.UpdateTicked += this.OnUpdateTicked;
            events.GameLoop.GameLaunched += this.OnGameLaunched;
            events.Display.Rendered += this.OnRendered;
            events.Multiplayer.ModMessageReceived += this.OnModMessageReceived;
        }

        private void OnGameLaunched(object sender, GameLaunchedEventArgs args)
        {
            ModEntry.multiplayer = Helper.Reflection.GetField<Multiplayer>(typeof(Game1), "multiplayer").GetValue();
            DeepWoodsSettings.Init(Helper.Translation);
            DeepWoodsTextures.Textures.LoadAll();
            if (Helper.ModRegistry.IsLoaded("Omegasis.SaveAnywhere"))
            {
                ISaveAnywhereAPI api = Helper.ModRegistry.GetApi<ISaveAnywhereAPI>("Omegasis.SaveAnywhere");
                if (api != null)
                {
                    api.BeforeSave += (s, e) => this.CleanupBeforeSave();
                    api.AfterSave += (s, e) => this.RestoreAfterSave();
                    api.AfterLoad += (s, e) => this.InitAfterLoad();
                }
            }
        }

        private int isBeforeSaveCount = 0;
        private void OnSaving(object sender, SavingEventArgs args)
        {
            this.CleanupBeforeSave();
        }

        private void CleanupBeforeSave()
        {
            ModEntry.Log("SaveEvents_BeforeSave", StardewModdingAPI.LogLevel.Trace);

            isBeforeSaveCount++;
            if (isBeforeSaveCount > 1)
            {
                ModEntry.Log("BeforeSave event was called twice in a row. Ignoring second call.", StardewModdingAPI.LogLevel.Warn);
                return;
            }

            DeepWoodsManager.Remove();
            EasterEggFunctions.RemoveAllEasterEggsFromGame();
            WoodsObelisk.RemoveAllFromGame();
            DeepWoodsSettings.DoSave();

            foreach (var who in Game1.getAllFarmers())
            {
                if (who.currentLocation is DeepWoods)
                {
                    who.currentLocation = Game1.getLocationFromName("Woods");
                    who.Position = new Vector2(WOODS_WARP_LOCATION.X * 64, WOODS_WARP_LOCATION.Y * 64);
                }
            }
        }

        private void OnSaved(object sender, SavedEventArgs args)
        {
            this.RestoreAfterSave();
        }

        private void RestoreAfterSave()
        {
            ModEntry.Log("SaveEvents_AfterSave", StardewModdingAPI.LogLevel.Trace);

            isBeforeSaveCount--;

            if (isBeforeSaveCount > 0)
            {
                ModEntry.Log("AfterSave event was called before save has finished. Ignoring.", StardewModdingAPI.LogLevel.Warn);
                return;
            }

            if (isBeforeSaveCount < 0)
            {
                ModEntry.Log("AfterSave event was called without previous BeforeSave call. Mod is now in unknown state, all hell might break lose.", StardewModdingAPI.LogLevel.Error);
                return;
            }

            DeepWoodsManager.Restore();
            EasterEggFunctions.RestoreAllEasterEggsInGame();
            WoodsObelisk.RestoreAllInGame();
        }

        private void OnReturnedToTitle(object sender, ReturnedToTitleEventArgs args)
        {
            ModEntry.Log("GameEvents_AfterReturnToTitle", StardewModdingAPI.LogLevel.Trace);

            isDeepWoodsGameRunning = false;
        }

        private void InitGameIfNecessary()
        {
            ModEntry.Log("InitGameIfNecessary(" + isDeepWoodsGameRunning + ")", StardewModdingAPI.LogLevel.Trace);

            if (isDeepWoodsGameRunning)
                return;

            if (Game1.IsMasterGame)
            {
                DeepWoodsSettings.DoLoad();
                DeepWoodsManager.Add();
                EasterEggFunctions.RestoreAllEasterEggsInGame();
                WoodsObelisk.RestoreAllInGame();
                isDeepWoodsGameRunning = true;
            }
            else
            {
                DeepWoodsManager.Remove();
                ModEntry.SendMessage(MessageId.RequestMetadata, Game1.MasterPlayer.UniqueMultiplayerID);
            }
        }

        private void OnSaveLoaded(object sender, SaveLoadedEventArgs args)
        {
            this.InitAfterLoad();
        }

        private void InitAfterLoad()
        {
            ModEntry.Log("SaveEvents_AfterLoad", StardewModdingAPI.LogLevel.Trace);

            isDeepWoodsGameRunning = false;
            InitGameIfNecessary();
        }

        public static void DeepWoodsInitServerAnswerReceived(string[] deepWoodsLevelNames)
        {
            if (Game1.IsMasterGame)
                return;

            ModEntry.Log("DeepWoodsInitServerAnswerReceived", StardewModdingAPI.LogLevel.Trace);

            DeepWoodsManager.AddAll(deepWoodsLevelNames);
            EasterEggFunctions.RestoreAllEasterEggsInGame();
            // WoodsObelisk.RestoreAllInGame(); <- Not needed, server already sends correct building
            mod.isDeepWoodsGameRunning = true;
        }

        private void OnDayStarted(object sender, DayStartedEventArgs args)
        {
            ModEntry.Log("TimeEvents_AfterDayStarted", StardewModdingAPI.LogLevel.Trace);

            InitGameIfNecessary();

            if (!isDeepWoodsGameRunning)
                return;

            DeepWoodsManager.LocalDayUpdate(Game1.dayOfMonth);
            EasterEggFunctions.InterceptIncubatorEggs();
        }

        private void OnTimeChanged(object sender, TimeChangedEventArgs args)
        {
            if (!isDeepWoodsGameRunning)
                return;

            DeepWoodsManager.LocalTimeUpdate(Game1.timeOfDay);
        }

        private void OnUpdateTicked(object sender, UpdateTickedEventArgs args)
        {
            if (!isDeepWoodsGameRunning)
                return;

            WorkErrorMessageQueue();

            Dictionary<long, GameLocation> newPlayerLocations = new Dictionary<long, GameLocation>();
            foreach (Farmer farmer in Game1.getOnlineFarmers())
            {
                newPlayerLocations.Add(farmer.UniqueMultiplayerID, farmer.currentLocation);
            }

            // Detect any farmer who left, joined or changed location.
            foreach (var playerLocation in playerLocations)
            {
                if (!newPlayerLocations.ContainsKey(playerLocation.Key))
                {
                    // player left
                    PlayerWarped(Game1.getFarmer(playerLocation.Key), playerLocation.Value, null);
                }
                else if (playerLocation.Value?.Name != newPlayerLocations[playerLocation.Key]?.Name)
                {
                    // player warped
                    PlayerWarped(Game1.getFarmer(playerLocation.Key), playerLocation.Value, newPlayerLocations[playerLocation.Key]);
                }
            }

            foreach (var newPlayerLocation in newPlayerLocations)
            {
                if (!playerLocations.ContainsKey(newPlayerLocation.Key))
                {
                    // player joined
                    PlayerWarped(Game1.getFarmer(newPlayerLocation.Key), null, newPlayerLocation.Value);
                }
            }

            // Update cache
            playerLocations = newPlayerLocations;

            // 
            DeepWoodsManager.LocalTick();

            // Fix lighting in Woods and DeepWoods
            DeepWoodsManager.FixLighting();

            // Add woods obelisk to wizard shop if possible and necessary,
            // intercept Building.obeliskWarpForReal() calls.
            WoodsObelisk.InjectWoodsObeliskIntoGame();
        }

        private void OnRendered(object sender, RenderedEventArgs e)
        {
            if (Game1.player.currentLocation is DeepWoods deepWoods)
                deepWoods.DrawLevelDisplay();
        }

        private void PlayerWarped(Farmer who, GameLocation prevLocation, GameLocation newLocation)
        {
            if (!isDeepWoodsGameRunning)
                return;

            if (prevLocation is DeepWoods dw1 && newLocation is DeepWoods dw2 && dw1.Name == dw2.Name)
                return;

            if (newLocation is Woods woods)
            {
                OpenPassageInSecretWoods(woods);
            }

            DeepWoodsManager.PlayerWarped(who, prevLocation, newLocation);

            if (newLocation is AnimalHouse animalHouse)
            {
                EasterEggFunctions.CheckEggHatched(who, animalHouse);
            }
        }

        private void OnModMessageReceived(object sender, ModMessageReceivedEventArgs e)
        {
            if (e.FromModID != this.ModManifest.UniqueID)
                return;

            ModEntry.Log($"[{(Context.IsMainPlayer ? "host" : "farmhand")}] Received {e.Type} from {e.FromPlayerID}.", LogLevel.Trace);

            switch (e.Type)
            {
                // farmhand requested metadata
                case MessageId.RequestMetadata:
                    if (Context.IsMainPlayer)
                    {
                        // client requests settings and state, send it:
                        InitResponseMessage response = new InitResponseMessage
                        {
                            Settings = DeepWoodsSettings.Settings,
                            State = DeepWoodsSettings.DeepWoodsState,
                            LevelNames = Game1.locations.OfType<DeepWoods>().Select(p => p.Name).ToArray()
                        };
                        ModEntry.SendMessage(response, MessageId.Metadata, e.FromPlayerID);
                    }
                    break;

                // host sent metadata
                case MessageId.Metadata:
                    if (!Context.IsMainPlayer)
                    {
                        InitResponseMessage response = e.ReadAs<InitResponseMessage>();
                        DeepWoodsSettings.Settings = response.Settings;
                        DeepWoodsSettings.DeepWoodsState = response.State;
                        ModEntry.DeepWoodsInitServerAnswerReceived(response.LevelNames);
                    }
                    break;

                // farmhand requested that we load and activate a DeepWoods level
                case MessageId.RequestWarp:
                    if (Context.IsMainPlayer)
                    {
                        // load level
                        int level = e.ReadAs<int>();
                        DeepWoods deepWoods = DeepWoodsManager.AddDeepWoodsFromObelisk(level);

                        // send response
                        WarpMessage response = new WarpMessage
                        {
                            Level = deepWoods.Level,
                            Name = deepWoods.Name,
                            EnterLocation = new Vector2(deepWoods.enterLocation.Value.X, deepWoods.enterLocation.Value.Y)
                        };
                        ModEntry.SendMessage(response, MessageId.Warp, e.FromPlayerID);
                    }
                    break;

                // host loaded area for warp
                case MessageId.Warp:
                    if (!Context.IsMainPlayer)
                    {
                        WarpMessage data = e.ReadAs<WarpMessage>();

                        DeepWoodsManager.AddBlankDeepWoodsToGameLocations(data.Name);
                        DeepWoodsManager.WarpFarmerIntoDeepWoodsFromServerObelisk(data.Name, data.EnterLocation);
                    }
                    break;

                // host sent 'lowest level reached' update
                case MessageId.SetLowestLevelReached:
                    if (!Context.IsMainPlayer)
                        DeepWoodsState.LowestLevelReached = e.ReadAs<int>();
                    break;

                // host sent 'received stardrop from unicorn' update
                case MessageId.SetUnicornStardropReceived:
                    if (Context.IsMainPlayer)
                        DeepWoodsState.PlayersWhoGotStardropFromUnicorn.Add(e.FromPlayerID);
                    break;

                // host added/removed location
                case MessageId.AddLocation:
                    if (!Context.IsMainPlayer)
                    {
                        string name = e.ReadAs<string>();
                        DeepWoodsManager.AddBlankDeepWoodsToGameLocations(name);
                    }
                    break;
                case MessageId.RemoveLocation:
                    if (!Context.IsMainPlayer)
                    {
                        string name = e.ReadAs<string>();
                        DeepWoodsManager.RemoveDeepWoodsFromGameLocations(name);
                    }
                    break;

                default:
                    ModEntry.Log("   ignored unknown type.", LogLevel.Trace);
                    break;
            }
        }

        public bool CanEdit<T>(IAssetInfo asset)
        {
            return asset.AssetNameEquals("Data/mail");
        }

        private void OpenPassageInSecretWoods(Woods woods)
        {
            // TODO: Configurable (and moddable!) locations to modify Woods

            // Game isn't running
            if (!isDeepWoodsGameRunning)
            {
                ModEntry.Log("OpenPassageInSecretWoods: Cancelled, mod not initialized.", LogLevel.Trace);
                return;
            }

            Layer buildingsLayer = woods.map.GetLayer("Buildings");

            // Just to be sure
            if (buildingsLayer == null)
            {
                ModEntry.Log("OpenPassageInSecretWoods: Cancelled, invalid map (buildingsLayer is null).", LogLevel.Trace);
                return;
            }

            // Already patched
            if (buildingsLayer.Tiles[29, 25] == null)
            {
                ModEntry.Log("OpenPassageInSecretWoods: Cancelled, map incompatible or already patched.", LogLevel.Trace);
                return;
            }

            ModEntry.Log("OpenPassageInSecretWoods", LogLevel.Trace);

            TileSheet borderTileSheet = buildingsLayer.Tiles[29, 25].TileSheet;
            int borderTileIndex = buildingsLayer.Tiles[29, 25].TileIndex;

            buildingsLayer.Tiles[29, 25] = null;
            buildingsLayer.Tiles[29, 26] = null;

            for (int x = 24; x < 29; x++)
            {
                buildingsLayer.Tiles[x, 24] = new StaticTile(buildingsLayer, borderTileSheet, BlendMode.Alpha, borderTileIndex);
                woods.warps.Add(new Warp(x, 32, "DeepWoods", Settings.Map.RootLevelEnterLocation.X, Settings.Map.RootLevelEnterLocation.Y + 1, false));
            }

            /*
            foreach (var location in DeleteBuildingTiles)
            {
            }

            foreach (var location in AddBuildingTiles)
            {
            }

            foreach (var location in WarpLocations)
            {
            }
            */
        }

        public void Edit<T>(IAssetData asset)
        {
            if (asset.AssetNameEquals("Data/mail"))
            {
                EditMail(asset.GetData<Dictionary<string, string>>());
                Game1.content.Load<Dictionary<string, string>>("Data\\mail");
            }
            else
            {
                throw new ArgumentException("Can't edit " + asset.AssetName);
            }
        }

        private void EditMail(Dictionary<string, string> mailData)
        {
            mailData.Add(WOODS_OBELISK_WIZARD_MAIL_ID, I18N.WoodsObeliskWizardMailMessage);
        }

        public bool CanLoad<T>(IAssetInfo asset)
        {
            return asset.AssetNameEquals($"Buildings\\{WOODS_OBELISK_BUILDING_NAME}")
                || asset.AssetNameEquals("Maps\\deepWoodsLakeTilesheet");
        }

        public T Load<T>(IAssetInfo asset)
        {
            if (asset.AssetNameEquals($"Buildings\\{WOODS_OBELISK_BUILDING_NAME}"))
            {
                return (T)(object)DeepWoodsTextures.Textures.WoodsObelisk;
            }
            else if (asset.AssetNameEquals("Maps\\deepWoodsLakeTilesheet"))
            {
                return (T)(object)DeepWoodsTextures.Textures.LakeTilesheet;
            }
            else
            {
                throw new ArgumentException("Can't load " + asset.AssetName);
            }
        }
    }
}
