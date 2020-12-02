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

using Piggy_Bank_Mod.Data;

using StardewValley;
using StardewValley.Menus;

using Microsoft.Xna.Framework;

using System.Collections.Generic;
using System.IO;
using xTile.Dimensions;
using System.Linq;
using System.Net.Configuration;
using System.Diagnostics.Contracts;

namespace Piggy_Bank_Mod
{
    public class ModEntry : Mod
    {
        private allGold allGold;
        private List<Response> responses;
        private List<long> playerIds;
        private ITranslationHelper i18n => Helper.Translation;

        public int tempGlobalId = -1;
        public static IJsonAssetsApi JA;
        public static bool hasExtendedReach;
        public static bool hasJA;

        public override void Entry(IModHelper helper)
        {
            helper.Events.GameLoop.Saving += GameLoop_Saving;
            helper.Events.GameLoop.SaveLoaded += GameLoop_SaveLoaded;
            helper.Events.GameLoop.GameLaunched += GameLoop_GameLaunched;

            helper.Events.Input.ButtonPressed += Input_ButtonPressed;

            helper.Events.World.ObjectListChanged += World_ObjectListChanged;

            helper.Events.Multiplayer.PeerConnected += Multiplayer_PeerConnected;
            helper.Events.Multiplayer.ModMessageReceived += Multiplayer_ModMessageReceived;

            playerIds = new List<long>();
        }

        private void Multiplayer_ModMessageReceived(object sender, ModMessageReceivedEventArgs e)
        {
            if (e.FromModID == ModManifest.UniqueID && e.Type == "PBData")
            {
                allGold message = e.ReadAs<allGold>();
                allGold = message;
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
            Helper.Multiplayer.SendMessage<allGold>(message, "PBData", new[] { ModManifest.UniqueID });
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
                    PiggyBankGold g = new PiggyBankGold("Basic Label", 0, new Vector2(i.Key.X, i.Key.Y), id, i.Value.owner);
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
                        if (g.BankTile == i.Key)
                        {
                            removable = g;
                            break;
                        }
                    }
                    if(removable != null)
                        allGold.goldList.Remove(removable);
                }
            }
        }

        private void GameLoop_GameLaunched(object sender, GameLaunchedEventArgs e) //Get JsonAssets Api and directory on Game launch
        {
            JA = Helper.ModRegistry.GetApi<IJsonAssetsApi>("spacechase0.JsonAssets");
            JA.LoadAssets(Path.Combine(Helper.DirectoryPath, "assets"));

            hasExtendedReach = Helper.ModRegistry.IsLoaded("spacechase0.ExtendedReach");
            hasJA = Helper.ModRegistry.IsLoaded("spacechase0.JsonAssets");

            if (!hasJA)
            {
                Monitor.Log($"JsonAssets was not loaded, the mod will force itself to stop to avoid crashes", LogLevel.Error);
                Helper.Events.GameLoop.Saving -= GameLoop_Saving;
                Helper.Events.GameLoop.SaveLoaded -= GameLoop_SaveLoaded;
                Helper.Events.GameLoop.GameLaunched -= GameLoop_GameLaunched;

                Helper.Events.Input.ButtonPressed -= Input_ButtonPressed;

                Helper.Events.World.ObjectListChanged -= World_ObjectListChanged;

                Helper.Events.Multiplayer.PeerConnected -= Multiplayer_PeerConnected;
                Helper.Events.Multiplayer.ModMessageReceived -= Multiplayer_ModMessageReceived;
            }
            else return;
        }

        private void GameLoop_Saving(object sender, SavingEventArgs e)
        {
            if (!Game1.IsMasterGame)
                return;

            for(int i=0; i<allGold.goldList.Count; i++)
            {
                Helper.Data.WriteSaveData("MindMeltMax.PiggyBank-" + i.ToString(), allGold.goldList[i]);
            }
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
            responses.Add(new Response("Close", i18n.Get("Close")));

            if (!Game1.IsMasterGame)
                return;

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
                if (allGold == null)
                    allGold.goldList = new List<PiggyBankGold>();
            }
        }

        private void Input_ButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (!Context.CanPlayerMove)
                return;

            if (e.Button != SButton.MouseRight)
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
                        if (gold.BankTile == tile)
                            openPiggy(gold.Id);
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
                return;

            foreach(PiggyBankGold gold in allGold.goldList)
            {
                if (gold.Id == tempGlobalId)
                {
                    string txt = responses.Find(k => k.responseKey == key).responseText;
                    Game1.activeClickableMenu = new NumberSelectionMenu(txt, (nr, cost, farmer) => processRequest(nr, cost, farmer, key), -1, 0, (key != "Withdraw") ? (int)who.Money : (int)gold.StoredGold);
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
                    Game1.activeClickableMenu = null;
                    tempGlobalId = -1;
                }
            }
        }
    }
    public interface IJsonAssetsApi //Get The JsonAssets Api functions
    {
        int GetBigCraftableId(string name);
        void LoadAssets(string path);
    }
}
