/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MindMeltMax/Stardew-Valley-Mods
**
*************************************************/

global using Object = StardewValley.Object;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using PiggyBank.Data;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.GameData.BigCraftables;
using StardewValley.GameData.Shops;
using StardewValley.Menus;
using System.Collections.Generic;
using System.Linq;

namespace PiggyBank
{
    internal class ModEntry : Mod
    {
        internal static IMonitor IMonitor;
        internal static IModHelper IHelper;
        internal static Config IConfig;
        internal static ObjectInformation ObjectInfo;

        private ITranslationHelper i18n => Helper.Translation;
        private Config config;
        private List<Response> responses;
        private List<PiggyBankGold> oldData = new();

        private bool onlyOwner;
        internal static bool hasExtendedReach;

        public override void Entry(IModHelper helper)
        {
            IMonitor = Monitor;
            IHelper = Helper;

            IConfig = config = Helper.ReadConfig<Config>();
            ObjectInfo = Helper.Data.ReadJsonFile<ObjectInformation>("assets/data.json");

            Helper.Events.Content.AssetRequested += onAssetRequested;
            Helper.Events.Input.ButtonPressed += onButtonDown;
            Helper.Events.GameLoop.SaveLoaded += onSaveLoad;
            Helper.Events.GameLoop.Saving += onSaving;
            Helper.Events.Multiplayer.PeerConnected += onPlayerJoin;
            Helper.Events.Multiplayer.ModMessageReceived += onModMessageReceived;
            Helper.Events.GameLoop.GameLaunched += onGameLaunch;
        }

        private void onGameLaunch(object sender, GameLaunchedEventArgs e) => hasExtendedReach = Helper.ModRegistry.IsLoaded("spacechase0.ExtendedReach");

        private void onModMessageReceived(object sender, ModMessageReceivedEventArgs e)
        {
            if (e.FromModID != Helper.ModRegistry.ModID)
                return;

            if (e.Type == "PiggyBank.HostOnlyOwner")
                onlyOwner = e.ReadAs<bool>();
            else if (e.Type == "PiggyBank.OldData")
            {
                var forPlayer = e.ReadAs<List<PiggyBankGold>>();
                foreach (var item in forPlayer)
                {
                    Item i = ItemRegistry.Create($"(BC){ObjectInfo.Id}");
                    if (!Game1.player.addItemToInventoryBool(i))
                        Game1.createItemDebris(i, Game1.player.getStandingPosition(), Game1.player.FacingDirection, Game1.player.currentLocation);
                    Game1.player.addUnearnedMoney((int)item.StoredGold);
                    oldData.Remove(item);
                }
            }
        }

        private void onPlayerJoin(object sender, PeerConnectedEventArgs e)
        {
            if (!Game1.IsMasterGame)
                return;
            Helper.Multiplayer.SendMessage(config.OwnerOnly, "PiggyBank.HostOnlyOwner", new[] { Helper.ModRegistry.ModID }, new[] { e.Peer.PlayerID });
            var forPlayer = oldData.FindAll(x => x.Id == e.Peer.PlayerID);
            if (!forPlayer.Any())
                return;
            Helper.Multiplayer.SendMessage(forPlayer, "PiggyBank.OldData", new[] { Helper.ModRegistry.ModID }, new[] { e.Peer.PlayerID });
            oldData.RemoveAll(x => x.Id == e.Peer.PlayerID);
        }

        private void onSaving(object sender, SavingEventArgs e)
        {
            if (!Context.IsMainPlayer || !oldData.Any())
                return;
            for (int i = 0; i < oldData.Count; i++)
                Helper.Data.WriteSaveData($"MindMeltMax.PiggyBank-{i}", oldData[i]);
        }

        private void onSaveLoad(object sender, SaveLoadedEventArgs e)
        {
            responses = [
                new("Deposit", i18n.Get("Deposit")),
                new("Withdraw", i18n.Get("Withdraw")),
                new("Label", i18n.Get("Label")),
                new("Close", i18n.Get("Close"))
            ];

            if (!Context.IsMainPlayer)
                return;

            #region BackwardsCompatibility
            int counter = 0;

            while (true)
            {
                var data = Helper.Data.ReadSaveData<PiggyBankGold>($"MindMeltMax.PiggyBank-{counter}"); //I hate how I saved this
                if (data is null)
                    break;
                oldData.Add(data);
                counter++;
            }

            if (!oldData.Any())
                return;

            List<PiggyBankGold> forPlayer = oldData.FindAll(x => x.Id == Game1.player.UniqueMultiplayerID);
            foreach (var item in forPlayer)
            {
                Item i = ItemRegistry.Create($"(BC){ObjectInfo.Id}");
                if (!Game1.player.addItemToInventoryBool(i))
                    Game1.createItemDebris(i, Game1.player.getStandingPosition(), Game1.player.FacingDirection, Game1.player.currentLocation);
                Game1.player.addUnearnedMoney((int)item.StoredGold);
                oldData.Remove(item);
            }

            Game1.addHUDMessage(new("Piggybank mod updated, all old items have been added back to your balance / inventory"));
            #endregion
        }

        private void onButtonDown(object sender, ButtonPressedEventArgs e)
        {
            if (!Context.CanPlayerMove)
                return;

            IEnumerable<SButton> ActionButtons = Game1.options.actionButton.Select(x => x.ToSButton());
            if (Game1.options.gamepadControls)
                ActionButtons = new[] { SButton.ControllerA };

            if (!ActionButtons.Contains(e.Button))
                return;
            Vector2 tile = hasExtendedReach ? e.Cursor.Tile : e.Cursor.GrabTile;
            var obj = Game1.currentLocation.getObjectAtTile((int)tile.X, (int)tile.Y);

            if (obj is null || obj.ItemId != ObjectInfo.Id)
                return;
            openPiggy(obj);
        }

        private void onAssetRequested(object sender, AssetRequestedEventArgs e)
        {
            if (e.NameWithoutLocale.IsEquivalentTo("Data/BigCraftables"))
            {
                e.Edit(asset =>
                {
                    var data = asset.AsDictionary<string, BigCraftableData>().Data;
                    Monitor.LogOnce($"Loaded Piggy Bank with id : {ObjectInfo.Id}");
                    ObjectInfo.Object.DisplayName = i18n.Get("Name");
                    ObjectInfo.Object.Description = i18n.Get("Description");
                    data[ObjectInfo.Id] = ObjectInfo.Object;
                });
            }

            if (e.NameWithoutLocale.IsEquivalentTo("Data/CraftingRecipes"))
            {
                e.Edit(asset =>
                {
                    var data = asset.AsDictionary<string, string>().Data;
                    data["Piggy Bank"] = string.Format(ObjectInfo.Recipe, ObjectInfo.Id, i18n.Get("Name"));
                });
            }

            if (e.NameWithoutLocale.IsEquivalentTo("Data/Shops"))
            {
                e.Edit(asset =>
                {
                    var data = asset.AsDictionary<string, ShopData>().Data;
                    if (!data.TryGetValue("Carpenter", out ShopData shopData))
                    {
                        Monitor.Log("Could not add piggy bank recipe to shop data, carpenter shop entry missing", LogLevel.Warn);
                        return;
                    }
                    ObjectInfo.ShopItem.Id = string.Format(ObjectInfo.ShopItem.Id, ObjectInfo.Id);
                    ObjectInfo.ShopItem.ItemId = string.Format("(BC){0}", ObjectInfo.Id);
                    shopData.Items.Add(ObjectInfo.ShopItem);
                });
            }

            if (e.NameWithoutLocale.IsEquivalentTo("PiggyBank/PiggyBank"))
                e.LoadFromModFile<Texture2D>("assets/piggyBank.png", AssetLoadPriority.Exclusive);
        }

        private void openPiggy(Object o)
        {
            if (!o.modData.ContainsKey(Helper.ModRegistry.ModID))
                o.modData.Add(Helper.ModRegistry.ModID, JsonConvert.SerializeObject(new PiggyBankItem() { Owner = Game1.player.UniqueMultiplayerID }));
            var data = readData(o);
            string text = $"{(!string.IsNullOrWhiteSpace(data.Label) ? $"{data.Label} - " : "")}{i18n.Get("Stored")}{data.Gold}g.";
            Game1.currentLocation.createQuestionDialogue(text, [.. responses], (farmer, key) => piggyBankMenu(farmer, key, o));
        }

        private void piggyBankMenu(Farmer who, string key, Object o)
        {
            if (key == "Close")
                return;

            var data = readData(o);

            if (key == "Label")
            {
                if (who.UniqueMultiplayerID == data.Owner)
                    Game1.activeClickableMenu = new NamingMenu(name => namePiggyBank(name, o), i18n.Get("Label"), data.Label);
                else
                    Game1.activeClickableMenu = new DialogueBox(string.Format(i18n.Get("Label_No_Permission"), Game1.getFarmer(data.Owner).Name));
                return;
            }

            if (!onlyOwner || (onlyOwner && who.UniqueMultiplayerID == data.Owner))
            {
                string txt = responses.Find(k => k.responseKey == key).responseText;
                Game1.activeClickableMenu = new NumberSelectionMenu(txt, (nr, _, farmer) => processRequest(nr, farmer, key, o), -1, 0, key != "Withdraw" ? who.Money : (int)data.Gold);
                return;
            }
            Game1.activeClickableMenu = new DialogueBox(string.Format(i18n.Get("Withdraw_Deposit_No_Permission"), Game1.getFarmer(data.Owner).Name));
        }

        private void processRequest(int number, Farmer who, string key, Object o)
        {
            var data = readData(o);

            if (key == "Deposit")
            {
                who.Money -= number;
                data.Gold += number;
            }
            else if (key == "Withdraw")
            {
                who.addUnearnedMoney(number);
                data.Gold -= number;
            }
            writeData(o, data);
            Game1.exitActiveMenu();
        }

        private void namePiggyBank(string name, Object o)
        {
            var data = readData(o);
            string origName = data.Label;
            data.Label = name;
            Monitor.Log($"Renamed {origName} to {name}");
            writeData(o, data);
            Game1.activeClickableMenu = null;
            Game1.player.canMove = true; //I don't understand why, but if I don't force this, player just freezes after naming
        }

        internal static PiggyBankItem? readData(Object o) => JsonConvert.DeserializeObject<PiggyBankItem>(o.modData[IHelper.ModRegistry.ModID]);

        internal static void writeData(Object o, PiggyBankItem data) => o.modData[IHelper.ModRegistry.ModID] = JsonConvert.SerializeObject(data);
    }
}
