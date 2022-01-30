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
using ItemPipes.Framework.Nodes;
using ItemPipes.Framework.Items;
using ItemPipes.Framework.Items.Recipes;
using HarmonyLib;
using SpaceCore;

namespace ItemPipes
{
    public interface ISpaceCoreApi
    {
        void RegisterSerializerType(Type type);
    }
    class ModEntry : Mod, IAssetEditor
    {
        public static IModHelper helper;
        public Dictionary<string, int> LogisticItemIds;
        public DataAccess DataAccess { get; set; }

        internal static readonly string ContentPackPath = Path.Combine("assets", "DGAItemLogistics");

        public override void Entry(IModHelper helper)
        {
            ModEntry.helper = helper;
            Printer.SetMonitor(this.Monitor);
            Framework.Util.Helper.SetHelper(helper);
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

            //Normal debug = only errors
            if(config.DebugMode)
            {
                Globals.Debug = true;
                if (Globals.Debug) { Printer.Info("Debug mode ENABLED"); }
            }
            else
            {
                Globals.Debug = false;
                if (Globals.Debug) { Printer.Info("Debug mode DISABLED"); }
            }
            //Ultra debug = all the prints like step by step
            if (config.UltraDebugMode)
            {
                Globals.UltraDebug = true;
                if (Globals.Debug) { Printer.Info("UltraDebug mode ENABLED"); }
            }
            else
            {
                Globals.UltraDebug = false;
                if (Globals.Debug) { Printer.Info("UltraDebug mode DISABLED"); }
            }
            if (!config.DisableItemSending)
            {
                Globals.DisableItemSending = true;
                if (Globals.Debug) { Printer.Info("Item sending ENABLED"); }
            }
            else
            {
                Globals.DisableItemSending = false;
                if (Globals.Debug) { Printer.Info("Item sending DISABLED"); }
            }

            var harmony = new Harmony(this.ModManifest.UniqueID);
            //FencePatcher.Apply(harmony);
            ChestPatcher.Apply(harmony);
            

            helper.Events.GameLoop.GameLaunched += OnGameLaunched;
            helper.Events.GameLoop.SaveLoaded += this.OnSaveLoaded;
            helper.Events.GameLoop.DayStarted += this.OnDayStarted;
            helper.Events.World.ObjectListChanged += this.OnObjectListChanged;
            helper.Events.GameLoop.OneSecondUpdateTicked += this.OnOneSecondUpdateTicked;

        }

        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            Helper.Content.AssetEditors.Add(this);
            var spaceCore = this.Helper.ModRegistry.GetApi<ISpaceCoreApi>("spacechase0.SpaceCore");
            spaceCore.RegisterSerializerType(typeof(IronPipeItem));
            spaceCore.RegisterSerializerType(typeof(GoldPipeItem));

            spaceCore.RegisterSerializerType(typeof(ExtractorPipeItem));
            spaceCore.RegisterSerializerType(typeof(InserterPipeItem));
            spaceCore.RegisterSerializerType(typeof(PolymorphicPipeItem));
            spaceCore.RegisterSerializerType(typeof(FilterPipeItem));

            spaceCore.RegisterSerializerType(typeof(WrenchItem));
            //spaceCore.RegisterSerializerType(typeof(IronPipeRecipe));
            CustomCraftingRecipe.CraftingRecipes.Add("Iron Pipe", new IronPipeRecipe());
        }
        public bool CanLoad<T>(IAssetInfo asset)
        {
            if (asset.AssetNameEquals("Data/ObjectInformation"))
            {
                return true;
            }

            return false;
        }

        public bool CanEdit<T>(IAssetInfo asset)
        {
            if (asset.AssetNameEquals("Data/CraftingRecipes"))
            {
                return true;
            }

            return false;
        }

        public void Edit<T>(IAssetData asset)
        {
            if (asset.AssetNameEquals("Data/CraftingRecipes"))
            {
                IDictionary<string, string> data = asset.AsDictionary<string, string>().Data;
                string hardwoodFenceTest = "709 1/Field/298/false/Mining 3";
                string ironPipe = "388 20 709 1/Home/0 1/false/Mining 3";
                string fakeRecipe = "0 1//0 1/false//Test Recipe";
                if (!data.ContainsKey("Iron Pipe"))
                {
                    data.Add("Iron Pipe", fakeRecipe);
                }
                else
                {
                    data["Iron Pipe"] = fakeRecipe;
                }
            }
        }

        private void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            Reset();
            foreach (GameLocation location in Game1.locations)
            {
                DataAccess.LocationNetworks.Add(location, new List<Network>());
                DataAccess.LocationNodes.Add(location, new List<Node>());
                NetworkBuilder.BuildLocationNetworks(location);
                NetworkManager.UpdateLocationNetworks(location);
            }
            if (Globals.UltraDebug) { Printer.Info("Location networks loaded!"); }
        }

        private void Reset()
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
                    //Tier 1 Extractors
                    if (e.IsMultipleOf(120))
                    {
                        DataAccess DataAccess = DataAccess.GetDataAccess();
                        foreach (GameLocation location in Game1.locations)
                        {
                            List<Network> networks = DataAccess.LocationNetworks[location];
                            if (networks.Count > 0)
                            {
                                //if (Globals.UltraDebug) { Printer.Info("Network amount: " + networks.Count.ToString()); }
                                foreach (Network network in networks)
                                {
                                    //Printer.Info(network.Print());
                                    network.ProcessExchanges(1);
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
            for(int i=0;i<15;i++)
            {
                //Game1.player.addItemToInventory(new Test());
                Game1.player.addItemToInventory(new IronPipeItem());
                Game1.player.addItemToInventory(new GoldPipeItem());
                Game1.player.addItemToInventory(new ExtractorPipeItem());
                Game1.player.addItemToInventory(new InserterPipeItem());
                Game1.player.addItemToInventory(new PolymorphicPipeItem());
                Game1.player.addItemToInventory(new FilterPipeItem());
                //Game1.player.addItemToInventory(new Pipe());
            }
            if(!Game1.player.hasItemInInventoryNamed("Wrench"))
            {
                Game1.player.addItemToInventory(new WrenchItem());
            }
        }
    }
}
