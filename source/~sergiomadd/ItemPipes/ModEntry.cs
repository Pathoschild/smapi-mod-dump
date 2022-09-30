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
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewValley;
using StardewValley.Tools;
using StardewValley.Objects;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using SObject = StardewValley.Object;
using ItemPipes.Framework.Model;
using ItemPipes.Framework.Patches;
using ItemPipes.Framework.Factories;
using ItemPipes.Framework.Items;
using ItemPipes.Framework.Nodes;
using HarmonyLib;
using MaddUtil;
using StardewValley.Buildings;
using StardewValley.Locations;
using ItemPipes.Framework.APIs;

namespace ItemPipes.Framework
{
    class ModEntry : Mod
    {
        public static IModHelper helper;
        public static ModConfig config;
        public DataAccess DataAccess { get; set; }
        public static IChestPreviewAPI ChestPreviewAPI { get; set; }

        public override void Entry(IModHelper helper)
        {
            ModEntry.helper = helper;
            Printer.SetMonitor(this.Monitor);
            Helpers.SetModHelper(helper);
            Helpers.SetContentHelper(helper.Content);
            Helpers.SetModContentHelper(helper.ModContent);
            DataAccess = DataAccess.GetDataAccess();

            helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;
            
            helper.Events.GameLoop.SaveLoaded += this.OnSaveLoaded;
            helper.Events.GameLoop.Saving += this.OnSaving;
            helper.Events.GameLoop.Saved += this.OnSaved;
            helper.Events.Content.AssetRequested += this.OnAssetRequested;
            helper.Events.GameLoop.DayStarted += this.OnDayStarted;
            helper.Events.World.ObjectListChanged += this.OnObjectListChanged;
            helper.Events.GameLoop.UpdateTicked += this.OnUpdateTicked;
            helper.Events.World.BuildingListChanged += this.OnBuildingListChanged;
            helper.Events.Input.ButtonPressed += this.OnButtonPressed;
            helper.Events.World.LocationListChanged += this.OnLocationListChanged;


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

        private void CheckCompatibilities()
        {
            //Compatibilities
            //TODO
            /*
            if (this.Helper.ModRegistry.IsLoaded("furyx639.BetterChests"))
            {
                IModInfo betterChests = helper.ModRegistry.Get("furyx639.BetterChests");
                if (betterChests != null)
                {
                    Printer.Warn("Found BetterChests mod in your folder.");
                    Printer.Warn("BetterChests is compatible with ItemPipes. But the filter option wont work!");
                }
            }
            */
            //Incompatibilities
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
            if (this.Helper.ModRegistry.IsLoaded("sergiomadd.ChestPreview"))
            {
                ChestPreviewAPI = helper.ModRegistry.GetApi<IChestPreviewAPI>("sergiomadd.ChestPreview");
                if (ChestPreviewAPI != null)
                {
                    Printer.Info("chest preview api loadsed.");                    
                }
                else
                {
                    Printer.Error("chest preview api error.");
                }
            }
        }

        public void test(SpriteBatch spriteBatch, Vector2 location, float scaleSize, float transparency, float layerDepth, StackDrawType drawStackNumber, Color color, bool drawShadow)
        {

        }

        private void OnBuildingListChanged(object sender, BuildingListChangedEventArgs e)
        {
            foreach (Building building in e.Added)
            {
                foreach (GameLocation location in building.indoors)
                {
                    if (location is BuildableGameLocation)
                    {
                        DataAccess.TryRegisterLocation(location);
                    }
                }
            }
            foreach (Building building in e.Removed)
            {
                foreach (GameLocation location in building.indoors)
                {
                    if (location is BuildableGameLocation)
                    {
                        DataAccess.TryUnRegisterLocation(location);
                    }
                }
            }
        }

        private void OnLocationListChanged(object sender, LocationListChangedEventArgs e)
        {
            foreach(GameLocation location in e.Added)
            {
                DataAccess.TryRegisterLocation(location);
            }
            foreach (GameLocation location in e.Removed)
            {
                DataAccess.TryUnRegisterLocation(location);
            }
        }

        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            config = DataAccess.LoadConfig();
            config.RegisterModConfigMenu(helper, this.ModManifest);
            ApplyPatches();
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
                Printer.Debug("Waiting for all items to arrive at inputs...");
                foreach (GameLocation location in DataAccess.AllLocations)
                {
                    foreach(KeyValuePair<Vector2, SObject> pair in location.objects.Pairs)
                    {
                        if(pair.Value is PipeItem)
                        {
                            List<Node> nodes = DataAccess.LocationNodes[location];
                            Node node = nodes.Find(n => n.Position.Equals(pair.Value.TileLocation));
                            if (node != null && node is PipeNode)
                            {
                                PipeNode pipe = (PipeNode)node;
                                pipe.FlushPipe();
                            }
                        }
                    }
                }
                Printer.Debug("Saving pipes...");
                ConvertToVanillaMap();
                ConvertToVanillaPlayer();
            }
        }

        public void OnSaved(object sender, SavedEventArgs args)
        {
            if (Context.IsMainPlayer) 
            {
                DataAccess.Reset();
                DataAccess.InitSave();

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
                DataAccess.InitSave();
                Helper.GameContent.InvalidateCache("Data/CraftingRecipes");
                Helper.GameContent.InvalidateCache($"Data/CraftingRecipes.{this.Helper.Translation.Locale}");

                CheckCompatibilities();

                ConvertFromVanillaMap();
                ConvertFromVanillaPlayer();

            }
        }

        private void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            //this.Helper.Multiplayer.GetActiveLocations();
            if (ModEntry.config.ItemSending)
            {
                if (Context.IsWorldReady)
                {
                    //Tier 1 Extractors
                    if (e.IsMultipleOf(60))
                    {
                        foreach (GameLocation location in DataAccess.AllLocations)
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
                        foreach (GameLocation location in DataAccess.AllLocations)
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
                        foreach (GameLocation location in DataAccess.AllLocations)
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

        private void OnObjectListChanged(object sender, ObjectListChangedEventArgs e)
        {

            List<KeyValuePair<Vector2, StardewValley.Object>> removedObjects = e.Removed.ToList();
            foreach (KeyValuePair<Vector2, StardewValley.Object> obj in removedObjects)
            {

                if (obj.Value is CustomObjectItem || obj.Value is Chest)
                {
                    NetworkManager.RemoveObject(obj, e.Location);
                    NetworkManager.UpdateLocationNetworks(Game1.currentLocation);
                }
            }

            List<KeyValuePair<Vector2, StardewValley.Object>> addedObjects = e.Added.ToList();
            foreach (KeyValuePair<Vector2, StardewValley.Object> obj in addedObjects)
            {
                if(obj.Value is CustomObjectItem || (obj.Value is Chest && !obj.Value.modData.ContainsKey("ItemPipes")))
                {
                    NetworkManager.AddObject(obj, e.Location);
                    NetworkManager.UpdateLocationNetworks(Game1.currentLocation);
                }
            }

            foreach (GameLocation location in DataAccess.AllLocations)
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
            /*
            SButton graphKey = SButton.L;
            if (e.Button == graphKey)
            {
                Printer.Info($"Networks of {Game1.currentLocation.Name}:");
                foreach (Network network in DataAccess.LocationNetworks[Game1.currentLocation])
                {
                    Printer.Info(network.PrintGraph());
                }
                Printer.Info(Utilities.GetNetworkLegend());
            }
            */
        }

        private void ConvertToVanillaMap()
        {

            List<GameLocation> locations = Utilities.YieldAllLocations().ToList();
            foreach (GameLocation location in locations)
            {
                foreach (KeyValuePair<Vector2, SObject> obj in location.Objects.Pairs.ToList())
                {
                    if (obj.Value is CustomObjectItem)
                    {
                        CustomObjectItem customObj = (CustomObjectItem)obj.Value;
                        SObject tempObj = customObj.SaveObject();
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
                                Item tempObj = customObj.SaveItem();
                                (obj.Value as Chest).items.RemoveAt(i);
                                (obj.Value as Chest).items.Insert(i, tempObj);
                            }
                            else if ((obj.Value as Chest).items[i] is CustomToolItem)
                            {
                                CustomToolItem customTool = (CustomToolItem)(obj.Value as Chest).items[i];
                                Item tempTool = customTool.SaveItem();
                                (obj.Value as Chest).items.RemoveAt(i);
                                (obj.Value as Chest).items.Insert(i, tempTool);
                            }
                        }
                    }
                }
            }
        }
        //Multiplayer?
        private void ConvertToVanillaPlayer()
        {
            if(Game1.IsMultiplayer)
            {
                foreach (Farmer farmer in Game1.getOnlineFarmers())
                {
                    if (farmer.Items.Any(i => i is CustomObjectItem || i is CustomToolItem))
                    {
                        for (int i = 0; i < farmer.Items.Count; i++)
                        {
                            if (farmer.Items[i] is CustomObjectItem)
                            {
                                CustomObjectItem customObj = (CustomObjectItem)farmer.Items[i];
                                Item tempObj = customObj.SaveItem();
                                farmer.Items.RemoveAt(i);
                                farmer.Items.Insert(i, tempObj);
                            }
                            else if (farmer.Items[i] is CustomToolItem)
                            {
                                CustomToolItem customTool = (CustomToolItem)farmer.Items[i];
                                Item tempTool = customTool.SaveItem();
                                farmer.Items.RemoveAt(i);
                                farmer.Items.Insert(i, tempTool);
                            }
                        }
                    }
                }
            }
            else
            {
                if (Game1.player.Items.Any(i => i is CustomObjectItem || i is CustomToolItem))
                {
                    for (int i = 0; i < Game1.player.Items.Count; i++)
                    {
                        if (Game1.player.Items[i] is CustomObjectItem)
                        {
                            CustomObjectItem customObj = (CustomObjectItem)Game1.player.Items[i];
                            Item tempObj = customObj.SaveItem();
                            Game1.player.Items.RemoveAt(i);
                            Game1.player.Items.Insert(i, tempObj);
                        }
                        else if (Game1.player.Items[i] is CustomToolItem)
                        {
                            CustomToolItem customTool = (CustomToolItem)Game1.player.Items[i];
                            Item tempTool = customTool.SaveItem();
                            Game1.player.Items.RemoveAt(i);
                            Game1.player.Items.Insert(i, tempTool);
                        }
                    }
                }
            }
        }

        private void ConvertFromVanillaMap()
        {
            List<GameLocation> locations = Utilities.YieldAllLocations().ToList();
            foreach (GameLocation location in locations)
            {
                foreach (KeyValuePair<Vector2, SObject> obj in location.Objects.Pairs.ToList())
                {
                    if (obj.Value.modData.ContainsKey("ItemPipes"))
                    {
                        if (obj.Value.modData["Type"] != null)
                        {
                            CustomObjectItem customObj = ItemFactory.CreateObject(obj.Key, obj.Value.modData["Type"]);
                            customObj.LoadObject(obj.Value);
                            location.objects.Remove(obj.Key);
                            location.objects.Add(obj.Key, customObj);
                        }
                    }
                    if (!obj.Value.modData.ContainsKey("ItemPipes") && obj.Value is Chest && (obj.Value as Chest).items.Any(i => i!=null && i.modData.ContainsKey("ItemPipes")))
                    {
                        for (int i = 0; i < (obj.Value as Chest).items.Count; i++)
                        {
                            if((obj.Value as Chest).items[i] != null)
                            {
                                if ((obj.Value as Chest).items[i] is Tool && (obj.Value as Chest).items[i].modData.ContainsKey("ItemPipes"))
                                {
                                    CustomToolItem customObj = ItemFactory.CreateTool((obj.Value as Chest).items[i].modData["Type"]);
                                    (obj.Value as Chest).items.RemoveAt(i);
                                    (obj.Value as Chest).items.Insert(i, customObj);
                                }
                                else if ((obj.Value as Chest).items[i].modData.ContainsKey("ItemPipes"))
                                {
                                    CustomObjectItem customObj = ItemFactory.CreateItem((obj.Value as Chest).items[i].modData["Type"]);
                                    (obj.Value as Chest).items.RemoveAt(i);
                                    (obj.Value as Chest).items.Insert(i, customObj);
                                }
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
            if (Game1.player.Items.Any(i =>i!=null && i.modData.ContainsKey("ItemPipes")))
            {
                for (int i = 0; i < Game1.player.Items.Count; i++)
                {
                    if(Game1.player.Items[i] != null)
                    {
                        if (Game1.player.Items[i] is Axe && Game1.player.Items[i].modData.ContainsKey("ItemPipes"))
                        {
                            CustomToolItem customTool = ItemFactory.CreateTool(Game1.player.Items[i].modData["Type"]);
                            customTool.LoadItem(Game1.player.Items[i].modData);
                            Game1.player.Items.RemoveAt(i);
                            Game1.player.Items.Insert(i, customTool);
                        }
                        else if (Game1.player.Items[i].modData.ContainsKey("ItemPipes"))
                        {
                            CustomObjectItem customObj = ItemFactory.CreateItem(Game1.player.Items[i].modData["Type"]);
                            customObj.LoadItem(Game1.player.Items[i]);
                            Game1.player.Items.RemoveAt(i);
                            Game1.player.Items.Insert(i, customObj);
                        }
                    }
                }
            }
        }
    }
}
