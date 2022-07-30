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
using StardewValley.Tools;
using StardewValley.Objects;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using SObject = StardewValley.Object;
using ItemPipes.Framework;
using ItemPipes.Framework.Util;
using ItemPipes.Framework.Model;
using ItemPipes.Framework.Patches;
using ItemPipes.Framework.Factories;
using ItemPipes.Framework.Items;
using ItemPipes.Framework.Nodes;
using ItemPipes.Framework.Data;
using ItemPipes.Framework.Items.Tools;
using HarmonyLib;
using System.Diagnostics;
using System.Threading;
using ItemPipes.Framework.APIs;

namespace ItemPipes
{
    class ModEntry : Mod
    {
        public static IModHelper helper;
        public static ModConfig config;
        public DataAccess DataAccess { get; set; }
        public bool NetworksInitialized { get; set; }

        public override void Entry(IModHelper helper)
        {
            ModEntry.helper = helper;
            Printer.SetMonitor(this.Monitor);
            ModHelper.SetHelper(helper.ModContent);
            DataAccess = DataAccess.GetDataAccess();
            config = DataAccess.LoadConfig();

            ApplyPatches();
            CheckImcompatibilities();

            NetworksInitialized = false;

            helper.Events.GameLoop.GameLaunched += OnGameLaunched;
            helper.Events.GameLoop.SaveLoaded += this.OnSaveLoaded;
            helper.Events.GameLoop.Saving += this.OnSaving;
            helper.Events.GameLoop.Saved += this.OnSaved;
            helper.Events.Content.AssetRequested += this.OnAssetRequested;
            helper.Events.GameLoop.DayStarted += this.OnDayStarted;
            helper.Events.World.ObjectListChanged += this.OnObjectListChanged;
            helper.Events.GameLoop.UpdateTicked += this.OnUpdateTicked;
            helper.Events.World.BuildingListChanged += this.OnBuildingListChanged;
            helper.Events.Input.ButtonPressed += this.OnButtonPressed;
            helper.Events.GameLoop.TimeChanged += this.OnTimeChanged;

        }

        private void ApplyPatches()
        {
            var harmony = new Harmony(ModManifest.UniqueID);
            try
            {
                CraftingPatcher.Apply(harmony);
                LetterPatcher.Apply(harmony);
                if (this.Helper.ModRegistry.IsLoaded("CJBok.ItemSpawner"))
                {
                    IModInfo itemSpawner = helper.ModRegistry.Get("CJBok.ItemSpawner");
                    if(itemSpawner != null)
                    {
                        Printer.Debug("CJB Item Spawner loaded");
                        Printer.Debug($"Applying CJBItemSpawner integration patches...");
                        CJBItemSpawnerIntegration.Apply(harmony);
                    }
                }
            }
            catch(Exception e)
            {
                Printer.Error("Error while applying harmony patches");
                Printer.Error(e.Message);
            }
        }

        private void CheckImcompatibilities()
        {
            if (this.Helper.ModRegistry.IsLoaded("Aredjay.SaveAnywhere1.5"))
            {
                IModInfo itemSpawner = helper.ModRegistry.Get("Aredjay.SaveAnywhere1.5");
                if (itemSpawner != null)
                {
                    Printer.Error("Found SaveAnywhere mod in your folder.");
                    Printer.Error("SaveAnywhere is imcompatible with ItemPipes. You must not save the game using it or the game will crash!");
                }
            }
            if (this.Helper.ModRegistry.IsLoaded("Omegasis.SaveAnywhere"))
            {
                IModInfo itemSpawner = helper.ModRegistry.Get("Omegasis.SaveAnywhere");
                if (itemSpawner != null)
                {
                    Printer.Error("Found SaveAnywhere mod in your folder.");
                    Printer.Error("SaveAnywhere is imcompatible with ItemPipes. You must not save the game using it or the game will crash!");
                }
            }
        }
        private void OnBuildingListChanged(object sender, BuildingListChangedEventArgs e)
        {
            NetworkBuilder.BuildLocationNetworksTEMP(e.Location);
        }

        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            config.RegisterModConfigMenu(helper, this.ModManifest);
        }

        private void OnAssetRequested(object sender, AssetRequestedEventArgs e)
        {
            if (e.NameWithoutLocale.IsEquivalentTo("Data/CraftingRecipes"))
            {
                e.Edit(asset =>
                {
                    IDictionary<string, string> data = asset.AsDictionary<string, string>().Data;
                    foreach (KeyValuePair<string, string> pair in DataAccess.FakeRecipes)
                    {
                        if (!data.ContainsKey(pair.Key))
                        {
                            data.Add(pair.Key, pair.Value);
                        }
                        else
                        {
                            data[pair.Key] = pair.Value;
                        }
                    }
                });
            }
            if (e.NameWithoutLocale.IsEquivalentTo("Data\\mail"))
            {
                e.Edit(asset =>
                {
                    IDictionary<string, string> data = asset.AsDictionary<string, string>().Data;
                    foreach (KeyValuePair<string, string> pair in DataAccess.Letters)
                    {
                        if (!data.ContainsKey(pair.Key))
                        {
                            data.Add(pair.Key, pair.Value);
                        }
                        else
                        {
                            data[pair.Key] = pair.Value;
                        }
                    }
                });
            }
        }

        private void OnSaving(object sender, SavingEventArgs e)
        {
            if (Context.IsMainPlayer)
            {
                //DataAccess.LostItems.Clear();
                if (ModEntry.config.DebugMode) { Printer.Debug("Waiting for all items to arrive at inputs..."); }
                foreach (GameLocation location in Game1.locations)
                {
                    foreach(KeyValuePair<Vector2, SObject> pair in location.objects.Pairs)
                    {
                        if(pair.Value is PipeItem)
                        {
                            DataAccess DataAccess = DataAccess.GetDataAccess();
                            List<Node> nodes = DataAccess.LocationNodes[Game1.currentLocation];
                            Node node = nodes.Find(n => n.Position.Equals(pair.Value.TileLocation));
                            if (node != null && node is PipeNode)
                            {
                                PipeNode pipe = (PipeNode)node;
                                pipe.FlushPipe();
                            }
                        }
                    }
                }
                if (ModEntry.config.DebugMode) { Printer.Debug("Saving modded items...!"); }
                ConvertToVanillaMap();
                ConvertToVanillaPlayer();
                if (ModEntry.config.DebugMode) { Printer.Debug("All modded items saved!"); }
            }
        }

        public void OnSaved(object sender, SavedEventArgs args)
        {
            if (Context.IsMainPlayer) 
            {
                DataAccess.Reset();
                foreach (GameLocation location in Game1.locations)
                {
                    DataAccess.LocationNetworks.Add(location, new List<Network>());
                    DataAccess.LocationNodes.Add(location, new List<Node>());
                    DataAccess.UsedNetworkIDs.Add(location, new List<long>());
                    NetworkBuilder.BuildLocationNetworksTEMP(location);
                    NetworkManager.UpdateLocationNetworks(location);
                }

                ConvertFromVanillaMap();
                ConvertFromVanillaPlayer();

                if (ModEntry.config.DebugMode) { Printer.Debug("Location networks loaded!"); }
            }
        }

        private void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            if (Context.IsMainPlayer)
            {
                DataAccess.Reset();
                DataAccess.LoadAssets();
                Helper.GameContent.InvalidateCache("Data/CraftingRecipes");
                Helper.GameContent.InvalidateCache($"Data/CraftingRecipes.{this.Helper.Translation.Locale}");

                foreach (GameLocation location in Game1.locations)
                {
                    DataAccess.LocationNetworks.Add(location, new List<Network>());
                    DataAccess.LocationNodes.Add(location, new List<Node>());
                    DataAccess.UsedNetworkIDs.Add(location, new List<long>());
                    NetworkBuilder.BuildLocationNetworksTEMP(location);
                    NetworkManager.UpdateLocationNetworks(location);
                }

                ConvertFromVanillaMap();
                ConvertFromVanillaPlayer();
            }
            if (ModEntry.config.DebugMode) { Printer.Debug("Location networks loaded!"); }
        }

        private void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            if (ModEntry.config.ItemSending)
            {
                if (Context.IsWorldReady)
                {
                    //Tier 1 Extractors
                    if (e.IsMultipleOf(60))
                    {
                        foreach (GameLocation location in Game1.locations)
                        {
                            List<Network> networks = DataAccess.LocationNetworks[location];
                            if (networks.Count > 0)
                            {
                                foreach (Network network in networks)
                                {
                                    if (network != null && network.Outputs.Count > 0)
                                    {
                                        network.ProcessExchanges(1);
                                        
                                    }
                                }
                            }
                        }
                    }
                    //Tier 2 Extractors
                    if (e.IsMultipleOf(30))
                    {
                        foreach (GameLocation location in Game1.locations)
                        {
                            List<Network> networks = DataAccess.LocationNetworks[location];
                            if (networks.Count > 0)
                            {
                                foreach (Network network in networks)
                                {
                                    if (network != null && network.Outputs.Count > 0)
                                    {
                                        network.ProcessExchanges(2);
                                    }

                                }
                            }
                        }
                    }
                    //Tier 3 Extractors
                    if (e.IsMultipleOf(15))
                    {
                        foreach (GameLocation location in Game1.locations)
                        {
                            List<Network> networks = DataAccess.LocationNetworks[location];
                            if (networks.Count > 0)
                            {
                                foreach (Network network in networks)
                                {
                                    if (network != null && network.Outputs.Count > 0)
                                    {
                                        network.ProcessExchanges(3);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private void OnTimeChanged(object sender, TimeChangedEventArgs e)
        {

        }

        private void OnObjectListChanged(object sender, ObjectListChangedEventArgs e)
        {
            List<KeyValuePair<Vector2, StardewValley.Object>> addedObjects = e.Added.ToList();
            foreach (KeyValuePair<Vector2, StardewValley.Object> obj in addedObjects)
            {
                if(obj.Value is CustomObjectItem || obj.Value is Chest)
                {
                    NetworkManager.AddObject(obj, e.Location);
                    NetworkManager.UpdateLocationNetworks(Game1.currentLocation);
                }
            }

            List<KeyValuePair<Vector2, StardewValley.Object>> removedObjects = e.Removed.ToList();
            foreach (KeyValuePair<Vector2, StardewValley.Object> obj in removedObjects)
            {

                if (obj.Value is CustomObjectItem || obj.Value is Chest)
                {
                    NetworkManager.RemoveObject(obj, e.Location);
                    NetworkManager.UpdateLocationNetworks(Game1.currentLocation);
                }
            }
            
            foreach (GameLocation location in Game1.locations)
            {
                NetworkManager.UpdateLocationNetworks(location);
            }
        }

        private void OnDayStarted(object sender, DayStartedEventArgs e)
        {
            if (Game1.player.craftingRecipes.ContainsKey("IronPipe") && Game1.player.craftingRecipes["IronPipe"] > 0 && !Game1.player.mailReceived.Contains("ItemPipes_SendWrench"))
            {
                Game1.player.mailbox.Add("itempipes_sendwrench");
            }
        }

        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            SButton graphKey = SButton.L;
            if (e.Button == graphKey)
            {
                foreach (Network network in DataAccess.LocationNetworks[Game1.currentLocation])
                {
                    Printer.Info(network.PrintGraph());
                }
            }
        }

        private void ConvertToVanillaMap()
        {
            foreach (GameLocation location in Game1.locations)
            {
                foreach (KeyValuePair<Vector2, SObject> obj in location.Objects.Pairs.ToList())
                {
                    if (obj.Value is CustomObjectItem)
                    {
                        CustomObjectItem customObj = (CustomObjectItem)obj.Value;
                        SObject tempObj = customObj.Save();
                        location.objects.Remove(obj.Key);
                        location.objects.Add(obj.Key, tempObj);
                    }
                    if (obj.Value is Chest && (obj.Value as Chest).items.Any(i => i is CustomObjectItem || i is CustomToolItem))
                    {
                        for (int i = 0; i < (obj.Value as Chest).items.Count; i++)
                        {
                            if ((obj.Value as Chest).items[i] is CustomObjectItem)
                            {
                                CustomObjectItem customObj = (CustomObjectItem)(obj.Value as Chest).items[i];
                                SObject tempObj = customObj.Save();
                                (obj.Value as Chest).items.RemoveAt(i);
                                (obj.Value as Chest).items.Insert(i, tempObj);
                            }
                            else if ((obj.Value as Chest).items[i] is CustomToolItem)
                            {
                                CustomToolItem customTool = (CustomToolItem)(obj.Value as Chest).items[i];
                                Tool tempTool = customTool.Save();
                                (obj.Value as Chest).items.RemoveAt(i);
                                (obj.Value as Chest).items.Insert(i, tempTool);
                            }
                        }
                    }
                }
            }
        }
        private void ConvertToVanillaPlayer()
        {
            if (Game1.player.Items.Any(i => i is CustomObjectItem || i is CustomToolItem))
            {
                for (int i = 0; i < Game1.player.Items.Count; i++)
                {
                    if (Game1.player.Items[i] is CustomObjectItem)
                    {
                        CustomObjectItem customObj = (CustomObjectItem)Game1.player.Items[i];
                        SObject tempObj = customObj.Save();
                        Game1.player.Items.RemoveAt(i);
                        Game1.player.Items.Insert(i, tempObj);
                    }
                    else if (Game1.player.Items[i] is CustomToolItem)
                    {
                        CustomToolItem customTool = (CustomToolItem)Game1.player.Items[i];
                        Tool tempTool = customTool.Save();
                        Game1.player.Items.RemoveAt(i);
                        Game1.player.Items.Insert(i, tempTool);
                    }
                }
            }
        }

        private void ConvertFromVanillaMap()
        {
            foreach (GameLocation location in Game1.locations)
            {
                foreach (KeyValuePair<Vector2, SObject> obj in location.Objects.Pairs.ToList())
                {
                    if (obj.Value is Fence && obj.Value.modData.ContainsKey("ItemPipes"))
                    {
                        if (obj.Value.modData["Type"] != null)
                        {
                            CustomObjectItem customObj = ItemFactory.CreateObject(obj.Key, obj.Value.modData["Type"]);
                            customObj.Load(obj.Value.modData);
                            location.objects.Remove(obj.Key);
                            location.objects.Add(obj.Key, customObj);
                        }
                    }
                    if (obj.Value is Chest && (obj.Value as Chest).items.Any(i => i is Fence))
                    {
                        for (int i = 0; i < (obj.Value as Chest).items.Count; i++)
                        {
                            if ((obj.Value as Chest).items[i] is Fence && (obj.Value as Chest).items[i].modData.ContainsKey("ItemPipes"))
                            {
                                Fence tempObj = (Fence)(obj.Value as Chest).items[i];
                                CustomObjectItem customObj = ItemFactory.CreateItem(tempObj.modData["Type"]);
                                (obj.Value as Chest).items.RemoveAt(i);
                                (obj.Value as Chest).items.Insert(i, customObj);
                            }
                            else if ((obj.Value as Chest).items[i] is Axe && (obj.Value as Chest).items[i].modData.ContainsKey("ItemPipes"))
                            {
                                Axe tempTool = (Axe)(obj.Value as Chest).items[i];
                                CustomToolItem customObj = ItemFactory.CreateTool(tempTool.modData["Type"]);
                                (obj.Value as Chest).items.RemoveAt(i);
                                (obj.Value as Chest).items.Insert(i, customObj);
                            }
                        }
                    }
                    if (obj.Value is Chest)
                    {
                        NetworkManager.AddObject(obj, location);
                    }
                }
            }
        }

        private void ConvertFromVanillaPlayer()
        {
            if (Game1.player.Items.Any(i =>
                (i is Fence && (i as Fence).modData.ContainsKey("ItemPipes"))
                || (i is Axe && (i as Axe).modData.ContainsKey("ItemPipes"))))
            {
                for (int i = 0; i < Game1.player.Items.Count; i++)
                {
                    if (Game1.player.Items[i] is Fence && Game1.player.Items[i].modData.ContainsKey("ItemPipes"))
                    {
                        CustomObjectItem customObj = ItemFactory.CreateItem(Game1.player.Items[i].modData["Type"]);
                        customObj.Load(Game1.player.Items[i].modData);
                        Game1.player.Items.RemoveAt(i);
                        Game1.player.Items.Insert(i, customObj);
                    }
                    else if (Game1.player.Items[i] is Axe && Game1.player.Items[i].modData.ContainsKey("ItemPipes"))
                    {
                        CustomToolItem customTool = ItemFactory.CreateTool(Game1.player.Items[i].modData["Type"]);
                        customTool.Load(Game1.player.Items[i].modData);
                        Game1.player.Items.RemoveAt(i);
                        Game1.player.Items.Insert(i, customTool);
                    }
                }
            }
        }
    }
}
