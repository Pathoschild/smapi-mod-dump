using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Objects;

using DeliveryService.Framework;

using SObject = StardewValley.Object;
using Microsoft.Xna.Framework.Input;
using StardewValley.Menus;
using DeliveryService.Menus.Overlays;
using Pathoschild.Stardew.Common;

namespace DeliveryService
{
    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod
    {
        private ModConfig Config;

        /// <summary>The overlay for the current menu which which lets the player navigate and edit chests (or <c>null</c> if not applicable).</summary>
        private DeliveryOverlay CurrentOverlay;
        private Dictionary<Chest, DeliveryChest> DeliveryChests = new Dictionary<Chest, DeliveryChest>();
        private long HostID = 0;

        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            this.Config = this.Helper.ReadConfig<ModConfig>();
            helper.Events.GameLoop.DayEnding += this.OnDayEnding;
            helper.Events.Input.ButtonPressed += this.OnButtonPressed;
            //helper.Events.GameLoop.UpdateTicked += this.OnUpdateTicked;
            helper.Events.Display.MenuChanged += this.OnMenuChanged;
            helper.Events.GameLoop.Saving += this.Save;
            helper.Events.GameLoop.SaveLoaded += this.Load;
            helper.Events.Multiplayer.ModMessageReceived += this.OnModMessageReceived;
            helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;
            helper.ConsoleCommands.Add("player_classify_inventory", "List classification for each item in player's inventory.\n\nUsage: player_classify_inventory", this.ClassifyInventory);
        }
        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            if (Config.WaitForWizardShop)
                Helper.Content.AssetEditors.Add(new WizardMail());
        }

        private void OnMenuChanged(object sender, MenuChangedEventArgs e)
        {
            if (!DeliveryEnabled())
                return;
            // remove old overlay
            if (this.CurrentOverlay != null)
            {
                this.CurrentOverlay?.Dispose();
                this.CurrentOverlay = null;
            }
            this.Monitor.Log("New menu: " + e.NewMenu?.GetType(), LogLevel.Trace);
            if (e.NewMenu is ItemGrabMenu igm && igm.context is Chest container)
            {
                DeliveryChest chest;
                if (!this.DeliveryChests.TryGetValue(container, out chest))
                {
                    chest = new DeliveryChest(container);
                    if (chest.Location == null)
                    {
                        Monitor.Log($"Failed to find chest location.  Can't deliver to chest {container.Name}", LogLevel.Warn);
                        return;
                    }
                    DeliveryChests[container] = chest;
                    Monitor.Log($"Creating DeliveryChest {chest.Location}", LogLevel.Trace);
                }
                this.Monitor.Log($"Applying DeliveryOverlay to {chest.Location}", LogLevel.Trace);
                this.Monitor.Log($"Send: {string.Join(", ", chest.DeliveryOptions.Send)} Receive: {string.Join(", ", chest.DeliveryOptions.Receive)}", LogLevel.Trace);
                this.CurrentOverlay = new DeliveryOverlay(this.Monitor, igm, chest, this.Helper, this.ModManifest.UniqueID, this.HostID);
            }
        }
        /*********
        ** Private methods
        *********/
        /// <summary>Raised after the player presses a button on the keyboard, controller, or mouse.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnDayEnding(object sender, DayEndingEventArgs e)
        {
            if (!Context.IsWorldReady || !Context.IsMainPlayer)
                return;
            if (!DeliveryEnabled())
            {
                Monitor.Log("Delivery is not yet enabled", LogLevel.Info);
                return;
            }
            //this.Monitor.Log($"{Game1.player.Name} game-day ended {e}.", LogLevel.Debug);
            this.DoDelivery();
        }
        /// <summary>Raised after the player presses a button on the keyboard, controller, or mouse.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            // ignore if player hasn't loaded a save yet
            if (!Context.IsWorldReady || !Context.IsMainPlayer)
                return;
            // print button presses to the console window
            if (e.Button == Config.DeliverKey)
            {
                if (!DeliveryEnabled())
                {
                    Monitor.Log("Delivery is not yet enabled", LogLevel.Info);
                    return;
                }
                this.Monitor.Log($"--{Game1.player.Name} pressed {e.Button}.", LogLevel.Trace);
                this.DoDelivery();
            }
        }
        private void OnModMessageReceived(object sender, ModMessageReceivedEventArgs e)
        {
            //Monitor.Log($"Got message: {e.Type}", LogLevel.Debug);
            if (e.FromModID != this.ModManifest.UniqueID)
                return;
            if (e.Type == "UpdateDeliveryOptions")
            {
                SyncDataModel message = e.ReadAs<SyncDataModel>();
                DeliveryChest dchest = GetDeliveryChestFromMessage(message);
                if (dchest != null)
                {
                    //Monitor.Log($"Send:{string.Join(", ", message.DeliveryOptions.Send)}", LogLevel.Debug);
                    dchest.DeliveryOptions.Set(message.DeliveryOptions);
                    if (this.CurrentOverlay != null)
                        this.CurrentOverlay.ResetEdit();
                }
            }
            else if (e.Type == "RequestDeliveryOptions")
            {
                SerializableChestLocation message = e.ReadAs<SerializableChestLocation>();
                DeliveryChest dchest = GetDeliveryChestFromMessage(message);
                if (dchest != null)
                {
                    Helper.Multiplayer.SendMessage(new SyncDataModel(dchest), "UpdateDeliveryOptions", modIDs: new[] { e.FromModID }, playerIDs: new[] { e.FromPlayerID });
                }
            }
        }
        private void DoDelivery()
        {
            List<DeliveryChest> toChests = new List<DeliveryChest>();
            List<DeliveryChest> fromChests = new List<DeliveryChest>();
            // ignore if player hasn't loaded a save yet
            if (!Context.IsWorldReady)
                return;
            Chest[] containers = DeliveryChests.Keys.ToArray();
            foreach (Chest container in containers)
            {
                DeliveryChest chest = DeliveryChests[container];
                Monitor.Log($"chest:{chest} container:{container}, Chest:{chest.Chest} location:{chest.Location}", LogLevel.Trace);
                if (chest == null || !chest.Exists())
                {
                    this.Monitor.Log($"Chest {chest.Location} no longer exists", LogLevel.Trace);
                    DeliveryChests.Remove(container);
                    continue;
                }
                if (chest.DeliveryOptions.Send.Any(x => x > 0))
                {
                    fromChests.Add(chest);
                }
                if (chest.DeliveryOptions.Receive.Any(x => x > 0))
                {
                    toChests.Add(chest);
                }
            }
            foreach (DeliveryChest fromChest in fromChests)
            {
                bool match_color = fromChest.DeliveryOptions.MatchColor;
                foreach (DeliveryChest toChest in toChests)
                {
                    List<Tuple<DeliveryCategories, int>> categories = new List<Tuple<DeliveryCategories, int>>();
                    if (fromChest == toChest || (match_color && fromChest.Chest.playerChoiceColor != toChest.Chest.playerChoiceColor))
                    {
                        continue;
                    }
                    foreach (DeliveryCategories category in Enum.GetValues(typeof(DeliveryCategories)))
                    {
                        int mask = (fromChest.DeliveryOptions.Send[(int)category] & toChest.DeliveryOptions.Receive[(int)category]);
                        if ( mask > 0)
                        {
                            // There is overlap between these chests
                            categories.Add(new Tuple<DeliveryCategories, int>(category, mask));
                        }
                    }
                    if (categories.Count == 0 || (fromChest.DeliveryOptions.MatchColor && fromChest.Chest.playerChoiceColor != toChest.Chest.playerChoiceColor))
                    {
                        continue;
                    }
                    this.Monitor.Log($"Moving {string.Join(", ", categories)} items from {fromChest.Location} -> {toChest.Location}", LogLevel.Trace);
                    MoveItems(
                        from: fromChest,
                        to: toChest,
                        filter: categories.ToArray());
                }
            }
        }
        private void Save(object sender, SavingEventArgs e)
        {
            if (DeliveryEnabled() && Config.WaitForWizardShop)
            {
                Game1.addMailForTomorrow("DeliveryServiceWizardMail");
            }
            if (!Context.IsMainPlayer)
                return;
            List<SaveDataModel> save = new List<SaveDataModel>();
            foreach (DeliveryChest chest in this.DeliveryChests.Values)
            {
                if (chest == null || !chest.Exists())
                    continue;
                SaveDataModel data = new SaveDataModel(chest);
                if (data.Send.Count > 0 || data.Receive.Count > 0)
                    save.Add(data);
            }
            Helper.Data.WriteSaveData("delivery-service", save);
        }
        private void Load(object sender, SaveLoadedEventArgs e)
        {
            CurrentOverlay = null;
            DeliveryChests = new Dictionary<Chest, DeliveryChest>();
            HostID = 0;
            if (DeliveryEnabled() && Config.WaitForWizardShop)
            {
                Game1.addMailForTomorrow("DeliveryServiceWizardMail");
            }

            if (!Context.IsMainPlayer)
            {
                // Farmhands don't maintain state
                foreach (IMultiplayerPeer peer in this.Helper.Multiplayer.GetConnectedPlayers())
                {
                    if (peer.HasSmapi && peer.IsHost)
                    {
                        HostID = peer.PlayerID;
                        break;
                    }
                }
                return;
            }
            List<SaveDataModel> save = Helper.Data.ReadSaveData<List<SaveDataModel>>("delivery-service");
            if (save == null)
                return;
            foreach (SaveDataModel data in save)
            {
                DeliveryChest dchest = GetDeliveryChestFromMessage(data);
                if (dchest != null) {
                    int[] send = new int[Enum.GetValues(typeof(DeliveryCategories)).Length];
                    int[] receive = new int[Enum.GetValues(typeof(DeliveryCategories)).Length];
                    foreach (DeliveryCategories cat in Enum.GetValues(typeof(DeliveryCategories)))
                    {
                        send[(int)cat] = CheckCategoryEnabledInSave(cat, data.Send);
                        receive[(int)cat] = CheckCategoryEnabledInSave(cat, data.Receive);
                    }
                    dchest.DeliveryOptions.Set(send, receive, data.MatchColor);
                }
            }
        }
        private int CheckCategoryEnabledInSave(DeliveryCategories cat, List<string> match)
        {
            foreach (string item in match)
            {
                string[] items = item.Split(':');
                if (items[0] == cat.ToString())
                {
                    //Quality level 3 (==8) is unused
                    return items.Length > 1 ? Int32.Parse(items[1]) : (16 + 4 + 2 + 1);
                }
            }
            return 0;
        }
        private void MoveItems(DeliveryChest from, DeliveryChest to, Tuple<DeliveryCategories, int>[] filter)
        {
            // Store items because removing items aborts foreach()
            Item[] items = from.Chest.items.ToArray();
            foreach (Item item in items)
            {
                string type = "";
                int quality = 1;
                if (item is SObject obj)
                {
                    type = obj.Type;
                    quality = 1 << obj.Quality;
                }
                DeliveryCategories cat = item.getDeliveryCategory();
                this.Monitor.Log($"Found existing item {item.Name} Type: {type} Category: {item.getCategoryName()} cat: {cat.Name()}", LogLevel.Trace);
                if (!filter.Any(x => x.Item1 == cat && (x.Item2 & quality) > 0))
                    continue;
                Item chest_full = to.Chest.addItem(item);
                if (chest_full != null)
                {
                    this.Monitor.Log($"Item {item.Name} will not fit in chest at {to.Location}", LogLevel.Info);
                    continue;
                }
                //this.Monitor.Log($"Removing item", LogLevel.Trace);
                from.Chest.items.Remove(item);
            }
        }
        private DeliveryChest GetDeliveryChestFromMessage(SerializableChestLocation message)
        {
            foreach (GameLocation location in LocationHelper.GetAccessibleLocations())
            {
                if (location.NameOrUniqueName == message.Location)
                {
                    Item item;
                    if (message.isFridge)
                        if (location is FarmHouse house && Game1.player.HouseUpgradeLevel > 0)
                            item = house.fridge.Value;
                        else
                            break;
                    else
                        item = location.getObjectAtTile(message.X, message.Y);
                    if (item != null && item is Chest chest)
                    {
                        DeliveryChest dchest;
                        if (DeliveryChests.TryGetValue(chest, out dchest))
                        {
                            return dchest;
                        }
                        dchest = new DeliveryChest(chest);
                        DeliveryChests[chest] = dchest;
                        return dchest;
                    }
                }
            }
            return null;
        }
        private void ClassifyInventory(string command, string[] args)
        {
            foreach (Item item in Game1.player.Items)
            {
                if (item == null)
                    continue;
                string item_category = item.getCategoryName();
                string item_type = "None";
                string item_classification = item.getDeliveryCategory().Name();
                if (item_category == "")
                {
                    item_category = "None";
                }
                if (item is SObject obj)
                {
                    item_type = obj.Type;
                }
                Monitor.Log($"{item.Name}:  Category: {item_category}  Type: {item_type}  Classification: {item_classification}", LogLevel.Info);
            }
        }
        private bool DeliveryEnabled()
        {
            return (!Config.WaitForWizardShop || Game1.player.hasMagicInk);
        }
    }
}