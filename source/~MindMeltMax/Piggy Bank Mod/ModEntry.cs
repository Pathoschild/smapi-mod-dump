/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MindMeltMax/Stardew-Valley-Mods
**
*************************************************/

using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Framework;

using Piggy_Bank_Mod.Data;

using StardewValley;
using StardewValley.Menus;

using Microsoft.Xna.Framework;

using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Network;
using System.Linq;
using System.Runtime.CompilerServices;
using StardewValley.Buildings;
using System;

namespace Piggy_Bank_Mod
{
    public class ModEntry : Mod
    {
        private allGold allGold;
        private Config Config;
        private List<Response> responses;
        private List<long> playerIds;

        private ITranslationHelper i18n => Helper.Translation;

        private int upgradeLevel = -1;
        public int tempGlobalId = -1;
        private long hostId;
        public static IJsonAssetsApi JA;
        public static bool hasExtendedReach;
        public static bool hasJA;
        private static bool onlyOwner;
        private bool DisplayMoney;
        private bool showHoverText;
        private bool showNow;
        private Vector2 currentCursorTile;

        public override void Entry(IModHelper helper)
        {
            helper.Events.GameLoop.Saving += GameLoop_Saving;
            helper.Events.GameLoop.SaveLoaded += GameLoop_SaveLoaded;
            helper.Events.GameLoop.GameLaunched += GameLoop_GameLaunched;
            helper.Events.GameLoop.DayStarted += GameLoop_DayStarted;
            helper.Events.GameLoop.DayEnding += GameLoop_DayEnding;

            helper.Events.Input.ButtonPressed += Input_ButtonPressed;
            helper.Events.Input.CursorMoved += Input_CursorMoved;

            helper.Events.Display.RenderingHud += Display_RenderingHud;

            helper.Events.World.ObjectListChanged += World_ObjectListChanged;

            helper.Events.Multiplayer.PeerConnected += Multiplayer_PeerConnected;
            helper.Events.Multiplayer.ModMessageReceived += Multiplayer_ModMessageReceived;

            playerIds = new List<long>();

            Config = Helper.ReadConfig<Config>();
            showHoverText = Config.DisplayHoverText;
            DisplayMoney = Config.DisplayMoneyInTextBox;

            showNow = false;
            currentCursorTile = Vector2.Zero;
        }

        private void GameLoop_DayEnding(object sender, DayEndingEventArgs e)
        {
            upgradeLevel = Utility.getHomeOfFarmer(Game1.player).upgradeLevel;
        }

        private void GameLoop_DayStarted(object sender, DayStartedEventArgs e)
        {
            if (upgradeLevel == -1) return;
            if (Utility.getHomeOfFarmer(Game1.player).upgradeLevel != upgradeLevel)
            {
                ChangeBankTileOnHouseUpgrade(Game1.player.houseUpgradeLevel);
            }
        }

        private void Input_CursorMoved(object sender, CursorMovedEventArgs e)
        {
            if (!Context.IsPlayerFree) return;

            var oAtT = Game1.player.currentLocation.getObjectAtTile((int)e.NewPosition.Tile.X, (int)e.NewPosition.Tile.Y);

            if (oAtT != null && oAtT.Name == "Piggy Bank")
            {
                showNow = true;
                currentCursorTile = new Vector2((int)e.NewPosition.Tile.X, (int)e.NewPosition.Tile.Y);
            }
            else showNow = false;
        }

        private void Display_RenderingHud(object sender, RenderingHudEventArgs e)
        {
            if (!showHoverText || !showNow || !Context.IsPlayerFree)
                return;
            SpriteBatch sb = e.SpriteBatch;
            foreach(var o_key in Game1.currentLocation.netObjects.Keys)
            {
                if(o_key == currentCursorTile)
                {
                    var o_val = Game1.currentLocation.netObjects[o_key];
                    string pb_label = "";
                    int pb_current_gold = 0;
                    if (o_val.Name == "Piggy Bank")
                    {
                        foreach (PiggyBankGold g in allGold.goldList)
                        {
                            if (g.BankTile == o_key && g.BankLocationName == Game1.currentLocation.Name)
                            {
                                pb_label = g.Label;
                                pb_current_gold = (int)g.StoredGold;
                                break;
                            }
                        }
                        if (!string.IsNullOrEmpty(pb_label))
                            IClickableMenu.drawHoverText(sb, pb_label, Game1.smallFont, 0, 0, moneyAmountToDisplayAtBottom: DisplayMoney ? pb_current_gold : -1);

                        break;
                    }
                }
            }
        }

        private void Multiplayer_ModMessageReceived(object sender, ModMessageReceivedEventArgs e)
        {
            if (e.FromModID == ModManifest.UniqueID && e.Type == "PBData")
            {
                allGold message = e.ReadAs<allGold>();
                allGold = message;
            }
            else if(e.FromModID == ModManifest.UniqueID && e.Type == "PBAdditional")
            {
                DataMessage info = e.ReadAs<DataMessage>();
                hostId = info.HostId;
                onlyOwner = info.OnlyOwner;
            }

            if (allGold != null)
            {
                if (allGold.goldList != null)
                    return;
                else
                {
                    allGold.goldList = new List<PiggyBankGold>();
                    Monitor.Log($"gold data list was null, creating new to prevent crash. Exit code for developer : PB-X1", LogLevel.Warn);
                    return;
                }
            }
            else
            {
                allGold = new allGold(new List<PiggyBankGold>());
                Monitor.Log($"gold data was null, new data set created to prevent crash. Exit code for developer : PB-X2", LogLevel.Error);
                return;
            }
        }

        private void Multiplayer_PeerConnected(object sender, PeerConnectedEventArgs e)
        {
            if (!Game1.IsMasterGame)
                return;
            playerIds.Add(e.Peer.PlayerID);
            allGold message = allGold;
            DataMessage additionalInfo = new DataMessage();
            additionalInfo.HostId = hostId;
            additionalInfo.OnlyOwner = onlyOwner;
            Helper.Multiplayer.SendMessage<DataMessage>(additionalInfo, "PBAdditional", new[] { ModManifest.UniqueID }, new[] { e.Peer.PlayerID });
            Helper.Multiplayer.SendMessage<allGold>(message, "PBData", new[] { ModManifest.UniqueID }, new[] { e.Peer.PlayerID });
        }

        private void World_ObjectListChanged(object sender, ObjectListChangedEventArgs e)
        {
            int id = 0;
            for (int i = 0; i < allGold.goldList.Count; i++)
                id++;

            foreach(var i in e.Added)
            {
                if (i.Value.Name == "Piggy Bank")
                {
                    i.Value.owner.Value = Game1.player.UniqueMultiplayerID;
                    PiggyBankGold g = new PiggyBankGold("Piggy Bank", 0, new Vector2(i.Key.X, i.Key.Y), id, i.Value.owner.Value, Game1.currentLocation.Name);
                    allGold.goldList.Add(g);
                }
            }

            foreach(var i in e.Removed)
            {
                if(i.Value.Name == "Piggy Bank")
                {
                    PiggyBankGold removable = null;
                    foreach(PiggyBankGold g in allGold.goldList)
                    {
                        if (g.BankTile == i.Key && g.BankLocationName == Game1.currentLocation.Name)
                        {
                            removable = g;
                            break;
                        }
                    }
                    if(removable != null)
                        allGold.goldList.Remove(removable);
                }
            }

            allGold message = allGold;
            Helper.Multiplayer.SendMessage<allGold>(message, "PBData", new[] { ModManifest.UniqueID });
        }

        private void GameLoop_GameLaunched(object sender, GameLaunchedEventArgs e) //Get JsonAssets Api and directory on Game launch
        {
            JA = Helper.ModRegistry.GetApi<IJsonAssetsApi>("spacechase0.JsonAssets");
            JA.LoadAssets(Path.Combine(Helper.DirectoryPath, "assets"));

            hasExtendedReach = Helper.ModRegistry.IsLoaded("spacechase0.ExtendedReach");
            hasJA = Helper.ModRegistry.IsLoaded("spacechase0.JsonAssets");

            if (!hasJA || JA == null)
            {
                Monitor.Log($"JsonAssets was not loaded, the mod will force itself to stop to avoid crashes", LogLevel.Error);
                Helper.Events.GameLoop.Saving -= GameLoop_Saving;
                Helper.Events.GameLoop.SaveLoaded -= GameLoop_SaveLoaded;
                Helper.Events.GameLoop.GameLaunched -= GameLoop_GameLaunched;

                Helper.Events.Input.ButtonPressed -= Input_ButtonPressed;
                Helper.Events.Input.CursorMoved -= Input_CursorMoved;

                Helper.Events.Display.RenderingHud -= Display_RenderingHud;

                Helper.Events.World.ObjectListChanged -= World_ObjectListChanged;

                Helper.Events.Multiplayer.PeerConnected -= Multiplayer_PeerConnected;
                Helper.Events.Multiplayer.ModMessageReceived -= Multiplayer_ModMessageReceived;
            }
            else return;
        }

        private void GameLoop_Saving(object sender, SavingEventArgs e)
        {
            if (!Context.IsMainPlayer)
                return;

            for(int i=0; i<allGold.goldList.Count; i++)
            {
                Helper.Data.WriteSaveData("MindMeltMax.PiggyBank-" + i.ToString(), allGold.goldList[i]);
                Monitor.Log($"Saved gold with id : {allGold.goldList[i].Id} - stored gold : {allGold.goldList[i].StoredGold}", LogLevel.Trace);
            }
            allGold message = allGold;
            Helper.Multiplayer.SendMessage<allGold>(message, "PBData", new[] { ModManifest.UniqueID });
        }

        private void GameLoop_SaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            if (JA != null)
            {
                var piggyBankID = JA.GetBigCraftableId("Piggy Bank");
            }

            responses = new List<Response>();
            responses.Add(new Response("Deposit", i18n.Get("Deposit")));
            responses.Add(new Response("Withdraw", i18n.Get("Withdraw")));
            responses.Add(new Response("Label", i18n.Get("Label")));
            responses.Add(new Response("Close", i18n.Get("Close")));

            if (!Context.IsMainPlayer)
                return;

            onlyOwner = Config.OwnerOnly;
            hostId = Game1.MasterPlayer.UniqueMultiplayerID;

            if (allGold == null)
            {
                allGold = new allGold();
                allGold.goldList = new List<PiggyBankGold>();
                for(int i=0; i<250; i++)
                {
                    var temp = Helper.Data.ReadSaveData<PiggyBankGold>("MindMeltMax.PiggyBank-" + i.ToString());
                    if (temp != null)
                        allGold.goldList.Add(temp);
                    else
                        break;
                }
            }
        }

        private void Input_ButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (!Context.CanPlayerMove)
                return;

            SButton ActionButton1 = Game1.options.actionButton.First().ToSButton();
            SButton ActionButton2 = Game1.options.actionButton.Last().ToSButton();

            if (e.Button != ActionButton1 && e.Button != ActionButton2)
                return;
            Vector2 tile;
            if (hasExtendedReach)
                tile = e.Cursor.Tile;
            else
                tile = e.Cursor.GrabTile;
            var location = Game1.currentLocation;
            var clickedObject = location.getObjectAtTile((int)tile.X, (int)tile.Y);

            if (clickedObject == null)
                return;

            else if (clickedObject.Name == "Piggy Bank")
            {
                if(tile != null)
                {
                    foreach (PiggyBankGold gold in allGold.goldList)
                    {
                        if (gold.BankTile == tile && gold.BankLocationName == Game1.currentLocation.Name)
                        {
                            Monitor.Log($"Found piggybank with id : {gold.Id} at {tile}", LogLevel.Trace);
                            openPiggy(gold.Id);
                            break;
                        }
                        else continue;
                    }
                }
            }
        }

        private bool openPiggy(int id)
        {
            foreach (PiggyBankGold gold in allGold.goldList)
            {
                if (id == gold.Id)
                {
                    Monitor.Log($"attempting to open piggybank with id : {gold.Id} - stored gold : {gold.StoredGold}", LogLevel.Trace);
                    string text = i18n.Get("Stored") + gold.StoredGold.ToString() + "g.";
                    tempGlobalId = id;
                    Game1.currentLocation.createQuestionDialogue(text, responses.ToArray(), piggyBankMenu);
                    return true;
                }
                else continue;
            }
            return false;
        }

        private void piggyBankMenu(Farmer who, string key)
        {
            if (key == "Close")
            {
                tempGlobalId = -1;
                return;
            }
                
            else if (key == "Label")
            {
                foreach (PiggyBankGold gold in allGold.goldList)
                {
                    if (tempGlobalId == gold.Id)
                    {
                        if (who.UniqueMultiplayerID == gold.OwnerID)
                        {
                            Game1.activeClickableMenu = new NamingMenu(name => NamePiggyBank(name), i18n.Get($"Label"), gold.Label);
                            return;
                        }
                        else
                        {
                            Game1.activeClickableMenu = new DialogueBox($"Can't edit the label of {Game1.getFarmer(gold.OwnerID).Name}'s piggy bank");
                            tempGlobalId = -1;
                            return;
                        }
                    }
                }
            }

            foreach(PiggyBankGold gold in allGold.goldList)
            {
                if (gold.Id == tempGlobalId)
                {
                    if (!onlyOwner)
                    {
                        string txt = responses.Find(k => k.responseKey == key).responseText;
                        Game1.activeClickableMenu = new NumberSelectionMenu(txt, (nr, cost, farmer) => processRequest(nr, cost, farmer, key), -1, 0, (key != "Withdraw") ? (int)who.Money : (int)gold.StoredGold);
                        return;
                    }
                    else if (onlyOwner && who.UniqueMultiplayerID == gold.OwnerID)
                    {
                        string txt = responses.Find(k => k.responseKey == key).responseText;
                        Game1.activeClickableMenu = new NumberSelectionMenu(txt, (nr, cost, farmer) => processRequest(nr, cost, farmer, key), -1, 0, (key != "Withdraw") ? (int)who.Money : (int)gold.StoredGold);
                        return;
                    }
                    else
                    {
                        Game1.activeClickableMenu = new DialogueBox($"The host ({Game1.getFarmer(hostId).Name}) has disabled shared piggy banks");
                        return;
                    }
                }
                else continue;
            }
        }

        private void processRequest(int number, int cost, Farmer who, string key)
        {
            foreach(PiggyBankGold gold in allGold.goldList)
            {
                if(gold.Id == tempGlobalId)
                {
                    if (key == "Deposit")
                    {
                        who.Money -= number;
                        gold.StoredGold += number;
                    }
                    if (key == "Withdraw")
                    {
                        who.totalMoneyEarned -= (uint)number;
                        who.Money += number;
                        gold.StoredGold -= number;
                    }
                    Game1.exitActiveMenu();
                    tempGlobalId = -1;
                }
            }

            allGold message = allGold;
            Helper.Multiplayer.SendMessage<allGold>(message, "PBData", new[] { ModManifest.UniqueID });
        }

        private void NamePiggyBank(string name)
        {
            string origName = "";
            foreach(PiggyBankGold gold in allGold.goldList)
            {
                if(gold.Id == tempGlobalId)
                {
                    origName = gold.Label;
                    gold.Label = name;
                    tempGlobalId = -1;
                    break;
                }
            }
            Game1.activeClickableMenu = new DialogueBox($"Renamed {origName} to {name}");
            allGold message = allGold;
            Helper.Multiplayer.SendMessage<allGold>(message, "PBData", new[] { ModManifest.UniqueID });
            return;
        }

        private void ChangeBankTileOnHouseUpgrade(int whichUpgrade)
        {
            var upgrade = Utility.getHomeOfFarmer(Game1.player).upgradeLevel - 1;
            switch (whichUpgrade)
            {
                case 0:
                    if (upgrade != 1)
                        break;
                    ChangeBankTiles(-6, 0);
                    break;
                case 1:
                    if (upgrade == 0)
                        ChangeBankTiles(6, 0);
                    if (upgrade != 2)
                        break;
                    ChangeBankTiles(-3, 0);
                    break;
                case 2:
                case 3:
                    if (upgrade == 1)
                    {
                        ChangeBankTiles(3, 9);
                    }
                    if (upgrade != 0)
                        break;
                    ChangeBankTiles(9, 9);
                    break;
            }
        }

        private void ChangeBankTiles(int newX, int newY)
        {
            var banks = allGold.goldList.Where(x => x.BankLocationName == Utility.getHomeOfFarmer(Game1.player).Name);
            foreach(var bank in banks)
            {
                bank.BankTile = new Vector2(bank.BankTile.X + newX, bank.BankTile.Y + newY);
            }
        }
    }

    public class DataMessage
    {
        public bool OnlyOwner { get; set; }
        public long HostId { get; set; }
    }

    public interface IJsonAssetsApi //Get The JsonAssets Api functions
    {
        int GetBigCraftableId(string name);
        void LoadAssets(string path);
    }
}
