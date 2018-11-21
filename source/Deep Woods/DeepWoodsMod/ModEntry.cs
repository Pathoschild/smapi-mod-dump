using System;
using System.Collections.Generic;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Locations;
using xTile;
using xTile.Layers;
using xTile.ObjectModel;
using xTile.Tiles;
using static DeepWoodsMod.DeepWoodsSettings;
using static DeepWoodsMod.DeepWoodsGlobals;
using System.Collections.Concurrent;
using StardewValley.Buildings;
using Microsoft.Xna.Framework;
using StardewValley.Objects;
using StardewValley.Tools;
using Microsoft.Xna.Framework.Audio;
using System.IO;
using Omegasis.SaveAnywhere.API;
using DeepWoodsMod.API;
using DeepWoodsMod.API.Impl;

namespace DeepWoodsMod
{
    public class ModEntry : Mod, IAssetEditor, IAssetLoader
    {
        private static DeepWoodsAPI api = new DeepWoodsAPI();
        private static ModEntry mod;

        private bool isDeepWoodsGameRunning = false;
        private bool hasRequestedInitMessageFromServer = false;
        private Dictionary<long, GameLocation> playerLocations = new Dictionary<long, GameLocation>();

        private static ConcurrentQueue<string> queuedErrorMessages = new ConcurrentQueue<string>();

        public static bool IsDeepWoodsGameRunning { get { return mod.isDeepWoodsGameRunning; } }
        public static bool HasRequestedInitMessageFromServer { get { return mod.hasRequestedInitMessageFromServer; } }

        private static void WorkErrorMessageQueue()
        {
            string msg;
            while (queuedErrorMessages.TryDequeue(out msg))
            {
                Log(msg, LogLevel.Error);
            }
        }

        public static void Log(string message, LogLevel level = LogLevel.Debug)
        {
            ModEntry.mod?.Monitor?.Log(message, level);
        }

        public static void QueueErrorMessage(string message)
        {
            queuedErrorMessages.Enqueue(message);
        }

        public static IReflectionHelper GetReflection()
        {
            return ModEntry.mod?.Helper?.Reflection;
        }

        public static IModHelper GetHelper()
        {
            return ModEntry.mod?.Helper;
        }

        public override void Entry(IModHelper helper)
        {
            ModEntry.mod = this;
            Game1MultiplayerAccessProvider.InterceptMultiplayerIfNecessary();
            Textures.LoadAll();
            RegisterEvents();
        }

        public override object GetApi()
        {
            return ModEntry.GetAPI();
        }

        public static DeepWoodsAPI GetAPI()
        {
            return api;
        }

        private void RegisterEvents()
        {
            SaveEvents.BeforeSave += this.SaveEvents_BeforeSave;
            SaveEvents.AfterSave += this.SaveEvents_AfterSave;
            SaveEvents.AfterLoad += this.SaveEvents_AfterLoad;
            SaveEvents.AfterReturnToTitle += this.SaveEvents_AfterReturnToTitle;
            TimeEvents.AfterDayStarted += this.TimeEvents_AfterDayStarted;
            TimeEvents.TimeOfDayChanged += this.TimeEvents_TimeOfDayChanged;
            GameEvents.UpdateTick += this.GameEvents_UpdateTick;
            GameEvents.FirstUpdateTick += this.GameEvents_FirstUpdateTick;
            GraphicsEvents.OnPostRenderEvent += this.GraphicsEvents_OnPostRenderEvent;
        }

        private void GameEvents_FirstUpdateTick(object sender, EventArgs args)
        {
            if (Helper.ModRegistry.IsLoaded("Omegasis.SaveAnywhere"))
            {
                ISaveAnywhereAPI api = Helper.ModRegistry.GetApi<ISaveAnywhereAPI>("Omegasis.SaveAnywhere");
                if (api != null)
                {
                    api.BeforeSave += this.SaveEvents_BeforeSave;
                    api.AfterSave += this.SaveEvents_AfterSave;
                    api.AfterLoad += this.SaveEvents_AfterLoad;
                }
            }
        }

        private int isBeforeSaveCount = 0;
        private void SaveEvents_BeforeSave(object sender, EventArgs args)
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

        private void SaveEvents_AfterSave(object sender, EventArgs args)
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

        private void SaveEvents_AfterReturnToTitle(object sender, EventArgs args)
        {
            ModEntry.Log("GameEvents_AfterReturnToTitle", StardewModdingAPI.LogLevel.Trace);

            isDeepWoodsGameRunning = false;
            hasRequestedInitMessageFromServer = false;
        }

        private void InitGameIfNecessary()
        {
            ModEntry.Log("InitGameIfNecessary(" + isDeepWoodsGameRunning + ")", StardewModdingAPI.LogLevel.Trace);

            // Make sure our interceptor is set.
            // E.g. MTN overrides Game1.multiplayer instead of wrapping.
            Game1MultiplayerAccessProvider.InterceptMultiplayerIfNecessary();

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
                hasRequestedInitMessageFromServer = true;
                Game1.MasterPlayer.queueMessage(Settings.Network.DeepWoodsMessageId, Game1.player, new object[] { NETWORK_MESSAGE_DEEPWOODS_INIT });
            }
        }

        private void SaveEvents_AfterLoad(object sender, EventArgs args)
        {
            ModEntry.Log("SaveEvents_AfterLoad", StardewModdingAPI.LogLevel.Trace);

            isDeepWoodsGameRunning = false;
            hasRequestedInitMessageFromServer = false;
            InitGameIfNecessary();
        }

        public static void DeepWoodsInitServerAnswerReceived(List<string> deepWoodsLevelNames)
        {
            if (Game1.IsMasterGame)
                return;

            ModEntry.Log("DeepWoodsInitServerAnswerReceived", StardewModdingAPI.LogLevel.Debug);

            DeepWoodsManager.AddAll(deepWoodsLevelNames);
            EasterEggFunctions.RestoreAllEasterEggsInGame();
            // WoodsObelisk.RestoreAllInGame(); <- Not needed, server already sends correct building
            mod.isDeepWoodsGameRunning = true;
        }

        private void TimeEvents_AfterDayStarted(object sender, EventArgs args)
        {
            ModEntry.Log("TimeEvents_AfterDayStarted", StardewModdingAPI.LogLevel.Trace);

            InitGameIfNecessary();

            if (!isDeepWoodsGameRunning)
                return;

            DeepWoodsManager.LocalDayUpdate(Game1.dayOfMonth);
            EasterEggFunctions.InterceptIncubatorEggs();
        }

        private void TimeEvents_TimeOfDayChanged(object sender, EventArgs args)
        {
            if (!isDeepWoodsGameRunning)
                return;

            DeepWoodsManager.LocalTimeUpdate(Game1.timeOfDay);
        }

        private void GameEvents_UpdateTick(object sender, EventArgs args)
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

        private void GraphicsEvents_OnPostRenderEvent(object sender, EventArgs e)
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

            DeepWoodsManager.PlayerWarped(who, prevLocation as DeepWoods, newLocation as DeepWoods, newLocation);

            if (newLocation is AnimalHouse animalHouse)
            {
                EasterEggFunctions.CheckEggHatched(who, animalHouse);
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
                return (T)(object)Textures.woodsObelisk;
            }
            else if (asset.AssetNameEquals("Maps\\deepWoodsLakeTilesheet"))
            {
                return (T)(object)Textures.lakeTilesheet;
            }
            else
            {
                throw new ArgumentException("Can't load " + asset.AssetName);
            }
        }
    }
}
