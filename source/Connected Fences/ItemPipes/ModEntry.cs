/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/sergiomadd/StardewValleyMods
**
*************************************************/

using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using SVObject = StardewValley.Objects;
using ItemPipes.Framework;
using ItemPipes.Framework.Util;
using ItemPipes.Framework.Model;
using ItemPipes.Framework.Patches;
using ItemPipes.Framework.API;
using ItemPipes.Framework.ContentPackUtil;
using HarmonyLib;

namespace ItemPipes
{
    public interface IJsonAssetsApi
    {
        int GetObjectId(string name);
        void LoadAssets(string path);
    }

    class ModEntry : Mod
    {
        public Dictionary<string, int> LogisticItemIds;
        public DataAccess DataAccess { get; set; }

        internal static readonly string ContentPackPath = Path.Combine("assets", "DGAItemLogistics");

        private IJsonAssetsApi JsonAssets;

        public override void Entry(IModHelper helper)
        {
            Printer.SetMonitor(this.Monitor);
            Framework.Helper.SetHelper(helper);
            LogisticItemIds = new Dictionary<string, int>();
            DataAccess = DataAccess.GetDataAccess();

            string dataPath = "assets/data.json";
            DataModel data = null;
            ModConfig config = null;
            try
            {
                data = this.Helper.Data.ReadJsonFile<DataModel>(dataPath);
                if (data == null)
                {
                    this.Monitor.Log($"The {dataPath} file seems to be missing or invalid.", LogLevel.Error);
                }
                config = this.Helper.ReadConfig<ModConfig>();
                if (config == null)
                {
                    this.Monitor.Log($"The config file seems to be missing or invalid.", LogLevel.Error);
                }
            }
            catch (Exception ex)
            {
                this.Monitor.Log($"The {dataPath} file seems to be invalid.\n{ex}", LogLevel.Error);
            }

            DataAccess.ModItems = data.ModItems;
            DataAccess.NetworkItems = data.NetworkItems;
            DataAccess.PipeNames = data.PipeNames;
            DataAccess.IOPipeNames = data.IOPipeNames;
            DataAccess.ExtraNames = data.ExtraNames;
            DataAccess.Buildings = data.Buildings;
            DataAccess.Locations = data.Locations;

            if(config.DebugMode)
            {
                Globals.Debug = true;
            }
            else
            {
                Globals.Debug = false;
            }
            if (!config.DisableItemSending)
            {
                Globals.DisableItemSending = true;
            }
            else
            {
                Globals.DisableItemSending = false;
            }
            //REMOVE
            Globals.Debug = true;

            var harmony = new Harmony(this.ModManifest.UniqueID);
            FencePatcher.Apply(harmony);
            ChestPatcher.Apply(harmony);


            helper.Events.GameLoop.GameLaunched += OnGameLaunched;
            helper.Events.GameLoop.SaveLoaded += this.OnSaveLoaded;
            helper.Events.GameLoop.DayStarted += this.OnDayStarted;
            helper.Events.World.ObjectListChanged += this.OnObjectListChanged;
            helper.Events.GameLoop.OneSecondUpdateTicked += this.OnOneSecondUpdateTicked;

        }

        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            JsonAssets = Helper.ModRegistry.GetApi<IJsonAssetsApi>("spacechase0.JsonAssets");
            if (JsonAssets == null)
            {
                Monitor.Log("Can't load Json Assets API, which is needed for Home Sewing Kit to function", LogLevel.Error);
            }
            else
            {
                JsonAssets.LoadAssets(Path.Combine(Helper.DirectoryPath, "assets"));
            }

            /*if (Helper.ModRegistry.IsLoaded("spacechase0.JsonAssets"))
            {
                Printer.Info("Attempting to hook into spacechase0.JsonAssets.");
                ApiManager.HookIntoJsonAssets();
            }*/

            //For when migrating to DGA
            // Check if spacechase0's DynamicGameAssets is in the current mod list
            /*if (Helper.ModRegistry.IsLoaded("spacechase0.DynamicGameAssets"))
            {
                Printer.Info("Attempting to hook into spacechase0.DynamicGameAssets.");
                ApiManager.HookIntoDynamicGameAssets();

                Manifest manifest = new Manifest();
                manifest = Helper.Data.ReadJsonFile<Manifest>("manifest.json");
                //It doesn't read the extra fields, don't know why
                manifest.ExtraFields = new Dictionary<string, object>();
                manifest.ExtraFields.Add("DGA.FormatVersion", 2);
                manifest.ExtraFields.Add("DGA.ConditionsFormatVersion", "1.24.2");
                var contentPackManifest = new CPManifest(manifest);
                ApiManager.GetDynamicGameAssetsInterface().AddEmbeddedPack(contentPackManifest, Path.Combine(Helper.DirectoryPath, ContentPackPath));
            }
            */
        }



        private void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            Reload();
            
            foreach (GameLocation location in Game1.locations)
            {
                //Monitor.Log("LOADING " + location.Name, LogLevel.Info);
                DataAccess.LocationNetworks.Add(location, new List<Network>());
                DataAccess.LocationNodes.Add(location, new List<Node>());
                NetworkBuilder.BuildLocationNetworks(location);
                NetworkManager.UpdateLocationNetworks(location);
                //Monitor.Log(location.Name + " LOADED!", LogLevel.Info);
                if (Globals.Debug)
                {
                    NetworkManager.PrintLocationNetworks(location);
                }
                /*
                if (DataAccess.Locations.Contains(location.Name))
                {
                    Monitor.Log("LOADING " + location.Name, LogLevel.Info);
                    DataAccess.LocationNetworks.Add(location, new List<Network>());
                    DataAccess.LocationMatrix.Add(location, new Node[location.map.DisplayWidth, location.map.DisplayHeight]);
                    NetworkBuilder.BuildLocationNetworks(location);
                    NetworkManager.UpdateLocationNetworks(location);
                    Monitor.Log(location.Name + " LOADED!", LogLevel.Info);
                    if(Globals.Debug)
                    {
                        NetworkManager.PrintLocationNetworks(location);
                    }
                }*/
            }
        }

        private void Reload()
        {
            DataAccess.LocationNodes.Clear();
            DataAccess.LocationNetworks.Clear();
            DataAccess.UsedNetworkIDs.Clear();
        }

        private void OnOneSecondUpdateTicked(object sender, OneSecondUpdateTickedEventArgs e)
        {
            if(Globals.DisableItemSending)
            {
                if (Context.IsWorldReady)
                {
                    /*if (e.IsMultipleOf(30))
                    {
                        if (Globals.Debug) { Printer.Info($"[X] UPDATETICKET"); }
                        Animator.updated = true;
                    }*/
                    if (e.IsMultipleOf(120))
                    {
                        DataAccess DataAccess = DataAccess.GetDataAccess();
                        List<Network> networks;
                        foreach (GameLocation location in Game1.locations)
                        {
                            if (DataAccess.LocationNetworks.TryGetValue(location, out networks))
                            {
                                if(networks.Count > 0)
                                {
                                    //if (Globals.Debug) { Printer.Info("Network amount: " + networks.Count.ToString()); }
                                    foreach (Network network in networks)
                                    {
                                        //network.ProcessExchanges();
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private void OnObjectListChanged(object sender, ObjectListChangedEventArgs e)
        {
            List<KeyValuePair<Vector2, StardewValley.Object>> addedObjects = e.Added.ToList();
            foreach (KeyValuePair<Vector2, StardewValley.Object> obj in addedObjects)
            {
                NetworkManager.AddObject(obj);
                NetworkManager.UpdateLocationNetworks(Game1.currentLocation);
            }

            List<KeyValuePair<Vector2, StardewValley.Object>> removedObjects = e.Removed.ToList();
            foreach (KeyValuePair<Vector2, StardewValley.Object> obj in removedObjects)
            {
                NetworkManager.RemoveObject(obj);
                NetworkManager.UpdateLocationNetworks(Game1.currentLocation);
            }
        }

        private void OnDayStarted(object sender, DayStartedEventArgs e)
        {
            RepairPipes();
        }

        private void RepairPipes()
        {
            foreach (GameLocation location in Game1.locations)
            {
                foreach (Fence fence in location.Objects.Values.OfType<Fence>())
                {
                    if (DataAccess.PipeNames.Contains(fence.name))
                    {
                        fence.health.Value = 100f;
                        fence.maxHealth.Value = fence.health.Value;
                    }
                }
            }
        }
    }
}
