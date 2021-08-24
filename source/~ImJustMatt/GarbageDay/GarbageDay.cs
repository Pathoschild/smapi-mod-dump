/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ImJustMatt/StardewMods
**
*************************************************/

using System.Collections.Generic;
using System.Linq;
using GarbageDay.Framework.Controllers;
using GarbageDay.Framework.Patches;
using HarmonyLib;
using Common.Integrations.GenericModConfigMenu;
using Common.Integrations.JsonAssets;
using XSAutomate.Common.Patches;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;

// ReSharper disable ClassNeverInstantiated.Global

namespace GarbageDay
{
    public class GarbageDay : Mod
    {
        internal static readonly IDictionary<string, GarbageCanController> GarbageCans = new Dictionary<string, GarbageCanController>();
        internal static int ObjectId;
        internal readonly Dictionary<string, Dictionary<string, double>> Loot = new();
        
        /// <summary>Handled content loaded by Expanded Storage.</summary>
        private AssetController _assetController;
        
        private IExpandedStorageAPI _expandedStorageAPI;
        
        /// <summary>Garbage Day API Instance</summary>
        private GarbageDayAPI _garbageDayAPI;
        
        private bool _objectsPlaced;
        internal ConfigController Config;
        
        /// <inheritdoc />
        public override object GetApi()
        {
            return _garbageDayAPI ??= new GarbageDayAPI(Loot);
        }
        
        /// <inheritdoc />
        public override void Entry(IModHelper helper)
        {
            Config = helper.ReadConfig<ConfigController>();
            _assetController = new AssetController(this);
            helper.Content.AssetLoaders.Add(_assetController);
            helper.Content.AssetEditors.Add(_assetController);
            
            // Initialize Global Loot from config
            Loot.Add("Global", Config.GlobalLoot);
            
            new Patcher(this).ApplyAll(
                typeof(ChestPatches)
            );
            
            // Console Commands
            foreach (var command in CommandsController.Commands)
            {
                helper.ConsoleCommands.Add(command.Name, command.Documentation, command.Callback);
            }
            
            // Events
            helper.Events.GameLoop.GameLaunched += OnGameLaunched;
            if (!Context.IsMainPlayer)
                return;
            helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
            helper.Events.GameLoop.ReturnedToTitle += OnReturnedToTitle;
            helper.Events.GameLoop.DayStarted += OnDayStarted;
        }
        
        /// <summary>Load Garbage Can</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            // Load Expanded Storage content
            _expandedStorageAPI = Helper.ModRegistry.GetApi<IExpandedStorageAPI>("furyx639.ExpandedStorage");
            _expandedStorageAPI.LoadContentPack(Helper.DirectoryPath);
            
            // Get ParentSheetIndex for object
            var jsonAssets = new JsonAssetsIntegration(Helper.ModRegistry);
            if (jsonAssets.IsLoaded)
                jsonAssets.API.IdsAssigned += delegate { ObjectId = jsonAssets.API.GetBigCraftableId("Garbage Can"); };
            
            var modConfigMenu = new GenericModConfigMenuIntegration(Helper.ModRegistry);
            if (!modConfigMenu.IsLoaded) return;
            Config.RegisterModConfig(Helper, ModManifest, modConfigMenu);
        }
        
        /// <summary>Initiate adding garbage can spots</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            if (_objectsPlaced) return;
            _objectsPlaced = true;
            Utility.ForAllLocations(delegate(GameLocation location)
            {
                var mapPath = PathUtilities.NormalizePath(location.mapPath.Value);
                foreach (var garbageCan in GarbageCans.Where(gc => gc.Value.MapName.Equals(mapPath)))
                {
                    if (!Loot.ContainsKey(garbageCan.Key)) Loot.Add(garbageCan.Key, new Dictionary<string, double>());
                    garbageCan.Value.Location = location;
                }
            });
            GarbageCans.Do(garbageCan => garbageCan.Value.AddToLocation());
            
            Monitor.Log(string.Join("\n",
                "Garbage Can Report",
                $"{"Name",-20} | {"Location",-30} | Coordinates",
                $"{new string('-', 21)}|{new string('-', 32)}|{new string('-', 15)}",
                string.Join("\n",
                    GarbageCans
                        .OrderBy(garbageCan => garbageCan.Key)
                        .Select(garbageCan => string.Join(" | ",
                            $"{garbageCan.Key,-20}",
                            $"{garbageCan.Value.Location.Name,-30}",
                            $"{garbageCan.Value.Tile.ToString()}")
                        ).ToList()
                )
            ), Config.LogLevelProperty);
        }
        
        /// <summary>Reset object id and tracked garbage cans</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnReturnedToTitle(object sender, ReturnedToTitleEventArgs e)
        {
            ObjectId = 0;
            _objectsPlaced = false;
        }
        
        /// <summary>Raised after a new in-game day starts, or after connecting to a multiplayer world.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private static void OnDayStarted(object sender, DayStartedEventArgs e)
        {
            GarbageCans.Do(garbageCan => garbageCan.Value.DayStart());
        }
    }
}