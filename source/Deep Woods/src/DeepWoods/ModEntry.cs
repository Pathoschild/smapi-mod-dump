/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/maxvollmer/DeepWoodsMod
**
*************************************************/

using System.Collections.Generic;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using static DeepWoodsMod.DeepWoodsSettings;
using static DeepWoodsMod.DeepWoodsGlobals;
using Microsoft.Xna.Framework;
using System.Linq;
using Omegasis.SaveAnywhere.API;
using DeepWoodsMod.API.Impl;
using DeepWoodsMod.Framework.Messages;
using DeepWoodsMod.Helpers;
using DeepWoodsMod.Stuff;
using StardewModdingAPI.Utilities;

namespace DeepWoodsMod
{
    public class ModEntry : Mod
    {
        private class PerScreenStuff
        {
            public bool isDeepWoodsGameRunning = false;
            public Dictionary<long, GameLocation> playerLocations = new Dictionary<long, GameLocation>();
            public int beforeSaveCount = 0;
        }

        private static DeepWoodsAPI api = new DeepWoodsAPI();
        private static ModEntry mod;
        private static Multiplayer multiplayer;

        private static readonly PerScreen<PerScreenStuff> _perScreenStuff = new(() => new PerScreenStuff());

        public static bool IsDeepWoodsGameRunning
        {
            get => _perScreenStuff.Value.isDeepWoodsGameRunning;
            private set => _perScreenStuff.Value.isDeepWoodsGameRunning = value;
        }
        private static Dictionary<long, GameLocation> PlayerLocations
        {
            get => _perScreenStuff.Value.playerLocations;
            set => _perScreenStuff.Value.playerLocations = value;
        }
        private static int BeforeSaveCount
        {
            get => _perScreenStuff.Value.beforeSaveCount;
            set => _perScreenStuff.Value.beforeSaveCount = value;
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
            //DeepWoodsDebugInjector.Patch(this.ModManifest.UniqueID);

            ModEntry.mod = this;
            I18N.Init(helper.Translation);
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
            events.GameLoop.SaveCreating += this.OnSaveCreating;
            events.GameLoop.SaveCreated += this.OnSaveCreated;
            events.GameLoop.SaveLoaded += this.OnSaveLoaded;
            events.GameLoop.ReturnedToTitle += this.OnReturnedToTitle;
            events.GameLoop.DayStarted += this.OnDayStarted;
            events.GameLoop.TimeChanged += this.OnTimeChanged;
            events.GameLoop.UpdateTicked += this.OnUpdateTicked;
            events.GameLoop.GameLaunched += this.OnGameLaunched;
            events.Display.Rendered += this.OnRendered;
            events.Multiplayer.ModMessageReceived += this.OnModMessageReceived;
            events.Content.AssetRequested += this.OnAssetRequested;
            events.Specialized.LoadStageChanged += this.OnLoadStageChanged;
        }

        private void OnGameLaunched(object sender, GameLaunchedEventArgs args)
        {
            ModEntry.multiplayer = Helper.Reflection.GetField<Multiplayer>(typeof(Game1), "multiplayer").GetValue();
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

        private void OnSaving(object sender, SavingEventArgs args)
        {
            this.CleanupBeforeSave();
        }

        private void OnSaveCreating(object sender, SaveCreatingEventArgs args)
        {
            this.CleanupBeforeSave();
        }

        private void OnSaved(object sender, SavedEventArgs args)
        {
            this.RestoreAfterSave();
        }

        private void OnSaveCreated(object sender, SaveCreatedEventArgs args)
        {
            this.RestoreAfterSave();
        }

        private void CleanupBeforeSave()
        {
            ModEntry.Log("SaveEvents_BeforeSave", StardewModdingAPI.LogLevel.Trace);

            BeforeSaveCount++;
            if (BeforeSaveCount > 1)
            {
                ModEntry.Log("BeforeSave event was called twice in a row. Ignoring second call.", StardewModdingAPI.LogLevel.Warn);
                return;
            }

            foreach (var who in Game1.getAllFarmers())
            {
                if (who.currentLocation is DeepWoods)
                {
                    who.currentLocation = Game1.getLocationFromName("Woods");
                    who.Position = new Vector2(DeepWoodsSettings.Settings.WoodsPassage.WoodsWarpLocation.X * 64, DeepWoodsSettings.Settings.WoodsPassage.WoodsWarpLocation.Y * 64);
                }
            }

            DeepWoodsManager.Remove();
            WoodsObelisk.RemoveAllFromGame();
            DeepWoodsSettings.DoSave();
        }

        private void RestoreAfterSave()
        {
            ModEntry.Log("SaveEvents_AfterSave", StardewModdingAPI.LogLevel.Trace);

            BeforeSaveCount--;

            if (BeforeSaveCount > 0)
            {
                ModEntry.Log("AfterSave event was called before save has finished. Ignoring.", StardewModdingAPI.LogLevel.Warn);
                return;
            }

            if (BeforeSaveCount < 0)
            {
                ModEntry.Log("AfterSave event was called without previous BeforeSave call. Mod is now in unknown state, all hell might break lose.", StardewModdingAPI.LogLevel.Error);
                return;
            }

            DeepWoodsManager.Restore();
            WoodsObelisk.RestoreAllInGame();
        }

        private void OnReturnedToTitle(object sender, ReturnedToTitleEventArgs args)
        {
            ModEntry.Log("GameEvents_AfterReturnToTitle", StardewModdingAPI.LogLevel.Trace);

            IsDeepWoodsGameRunning = false;
        }

        private void InitGameIfNecessary()
        {
            ModEntry.Log("InitGameIfNecessary(" + IsDeepWoodsGameRunning + ")", StardewModdingAPI.LogLevel.Trace);

            DeepWoodsManager.AddMaxHut();

            if (IsDeepWoodsGameRunning)
                return;

            if (Game1.IsMasterGame)
            {
                DeepWoodsSettings.DoLoad();
                DeepWoodsManager.Add();
                WoodsObelisk.RestoreAllInGame();
                IsDeepWoodsGameRunning = true;
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

            IsDeepWoodsGameRunning = false;
            InitGameIfNecessary();
        }

        public static void DeepWoodsInitServerAnswerReceived(string[] deepWoodsLevelNames)
        {
            if (Game1.IsMasterGame)
                return;

            ModEntry.Log("DeepWoodsInitServerAnswerReceived", StardewModdingAPI.LogLevel.Trace);

            DeepWoodsManager.AddAll(deepWoodsLevelNames);
            // WoodsObelisk.RestoreAllInGame(); <- Not needed, server already sends correct building
            IsDeepWoodsGameRunning = true;
        }

        private void OnDayStarted(object sender, DayStartedEventArgs args)
        {
            ModEntry.Log("TimeEvents_AfterDayStarted", StardewModdingAPI.LogLevel.Trace);

            InitGameIfNecessary();

            if (!IsDeepWoodsGameRunning)
                return;

            DeepWoodsManager.LocalDayUpdate(Game1.dayOfMonth);
        }

        private void OnTimeChanged(object sender, TimeChangedEventArgs args)
        {
            if (!IsDeepWoodsGameRunning)
                return;

            DeepWoodsManager.LocalTimeUpdate(Game1.timeOfDay);
        }

        private void OnUpdateTicked(object sender, UpdateTickedEventArgs args)
        {
            if (!IsDeepWoodsGameRunning)
                return;

            Dictionary<long, GameLocation> newPlayerLocations = new Dictionary<long, GameLocation>();
            foreach (Farmer farmer in Game1.getAllFarmers())
            {
                newPlayerLocations.Add(farmer.UniqueMultiplayerID, farmer.currentLocation);
            }

            // Detect any farmer who left, joined or changed location.
            foreach (var playerLocation in PlayerLocations)
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
                if (!PlayerLocations.ContainsKey(newPlayerLocation.Key))
                {
                    // player joined
                    PlayerWarped(Game1.getFarmer(newPlayerLocation.Key), null, newPlayerLocation.Value);
                }
            }

            // Update cache
            PlayerLocations = newPlayerLocations;

            // 
            DeepWoodsManager.LocalTick();

            // Fix lighting in Woods and DeepWoods
            DeepWoodsManager.FixLighting();

            // Add woods obelisk to wizard shop if possible and necessary,
            // intercept Building.obeliskWarpForReal() calls.
            WoodsObelisk.InjectWoodsObeliskIntoGame();

            // Add DeepWoods Minecart if possible and necessary
            DeepWoodsMineCart.InjectDeepWoodsMineCartIntoGame();
        }

        private void OnRendered(object sender, RenderedEventArgs e)
        {
            if (Game1.player.currentLocation is DeepWoods deepWoods)
                deepWoods.DrawLevelDisplay();
        }

        private void PlayerWarped(Farmer who, GameLocation prevLocation, GameLocation newLocation)
        {
            if (!IsDeepWoodsGameRunning)
                return;

            if (prevLocation is DeepWoods dw1 && newLocation is DeepWoods dw2 && dw1.Name == dw2.Name)
                return;

            DeepWoodsManager.PlayerWarped(who, prevLocation, newLocation);
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

                // host sent 'orb stones saved' update
                case MessageId.SetOrbStonesSaved:
                    if (!Context.IsMainPlayer)
                        DeepWoodsState.OrbStonesSaved = e.ReadAs<int>();
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

                case MessageId.DeInfest:
                    if (!Context.IsMainPlayer)
                    {
                        string name = e.ReadAs<string>();
                        DeepWoodsManager.DeInfestDeepWoods(name);
                    }
                    break;

                default:
                    ModEntry.Log("   ignored unknown type.", LogLevel.Trace);
                    break;
            }
        }

        private void OnAssetRequested(object sender, AssetRequestedEventArgs e)
        {
            DeepWoodsContentAdder.OnAssetRequested(sender, e);
        }

        private void OnLoadStageChanged(object sender, LoadStageChangedEventArgs e)
        {
            DeepWoodsContentAdder.OnLoadStageChanged(sender, e);
        }

    }
}
