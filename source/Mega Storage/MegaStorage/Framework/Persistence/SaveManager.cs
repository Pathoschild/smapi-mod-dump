using furyx639.Common;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Objects;
using System;
using System.Linq;
using Context = StardewModdingAPI.Context;

namespace MegaStorage.Framework.Persistence
{
    internal static class SaveManager
    {
        /*********
        ** Fields
        *********/
        public const string SaveDataKey = "NiceChest";

        /*********
        ** Public methods
        *********/
        public static void Start()
        {
            MegaStorageMod.ModHelper.Events.GameLoop.SaveLoaded += ReloadCustomChests;
            MegaStorageMod.ModHelper.Events.GameLoop.Saving += HideAndSaveCustomChests;
            MegaStorageMod.ModHelper.Events.GameLoop.ReturnedToTitle += HideAndSaveCustomChests;
            MegaStorageMod.ModHelper.Events.Multiplayer.PeerContextReceived += HideAndSaveCustomChests;
            MegaStorageMod.ModHelper.Events.GameLoop.Saved += ReAddCustomChests;
            StateManager.PlayerAdded += ReAddCustomChests;
            StateManager.PlayerRemoved += ReAddCustomChests;

            var saveAnywhereApi = MegaStorageMod.ModHelper.ModRegistry.GetApi<ISaveAnywhereApi>("Omegasis.SaveAnywhere");
            if (!(saveAnywhereApi is null))
            {
                saveAnywhereApi.AfterLoad += ReloadCustomChests;
                saveAnywhereApi.BeforeSave += HideAndSaveCustomChests;
                saveAnywhereApi.AfterSave += ReAddCustomChests;
            }

            ReloadCustomChests(null, null);
        }

        /*********
        ** Private methods
        *********/
        private static void ReloadCustomChests(object sender, EventArgs e)
        {
            if (!Context.IsWorldReady)
                return;

            MegaStorageMod.ModMonitor.VerboseLog("SaveManager: ReloadCustomChests");
            LegacyHelper.FixLegacyOptions();
            StateManager.PlacedChests.Clear();
            foreach (var location in CommonHelper.GetLocations())
            {
                var placedChests = location.Objects.Pairs
                    .Where(c => c.Value is Chest chest && CustomChestFactory.ShouldBeCustomChest(chest))
                    .ToDictionary(
                        c => c.Key,
                        c => c.Value.ToCustomChest(c.Key));

                foreach (var placedChest in placedChests)
                {
                    var pos = placedChest.Key;
                    var customChest = placedChest.Value;
                    MegaStorageMod.ModMonitor.VerboseLog($"Loading Chest at: {location.Name}: {customChest.Name} ({pos})");
                    location.objects[placedChest.Key] = customChest;
                    StateManager.PlacedChests.Add(new Tuple<GameLocation, Vector2>(location, pos), customChest);
                }
            }
        }

        private static void HideAndSaveCustomChests(object sender, EventArgs e)
        {
            if (!Context.IsWorldReady)
                return;

            MegaStorageMod.ModMonitor.VerboseLog("SaveManager: HideAndSaveCustomChests");
            foreach (var placedChest in StateManager.PlacedChests)
            {
                var location = placedChest.Key.Item1;
                var pos = placedChest.Key.Item2;
                var customChest = placedChest.Value;
                MegaStorageMod.ModMonitor.VerboseLog($"Hiding and Saving in {location.Name}: {customChest.Name} ({pos})");
                location.objects[pos] = customChest.ToChest();
                if (!Context.IsMainPlayer || !customChest.Equals(StateManager.MainChest))
                    continue;
                var deserializedChest = customChest.ToDeserializedChest(location.NameOrUniqueName);
                MegaStorageMod.ModHelper.Data.WriteSaveData(SaveDataKey, deserializedChest);
            }
        }

        private static void ReAddCustomChests(object sender, EventArgs e)
        {
            if (!Context.IsWorldReady)
                return;

            MegaStorageMod.ModMonitor.VerboseLog("SaveManager: ReAddCustomChests");
            LegacyHelper.FixLegacyOptions();
            foreach (var placedChest in StateManager.PlacedChests)
            {
                var location = placedChest.Key.Item1;
                var pos = placedChest.Key.Item2;
                var customChest = placedChest.Value;
                MegaStorageMod.ModMonitor.VerboseLog($"ReAddCustomChests in {location.Name}: {customChest.Name} ({pos})");
                location.objects[pos] = customChest;
            }
        }
    }
}
