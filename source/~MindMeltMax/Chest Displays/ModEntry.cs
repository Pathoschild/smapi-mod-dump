/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MindMeltMax/Stardew-Valley-Mods
**
*************************************************/

using Chest_Displays.Harmony;
using Chest_Displays.Utility;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Objects;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;

using SObject = StardewValley.Object;
using SUtils = StardewValley.Utility;

namespace Chest_Displays
{
    public class ModEntry : Mod
    {
        public static IModHelper RequestableHelper;
        public static IMonitor RequestableMonitor;
        public static Config RequestableConfig;

        public bool host = false;

        public static List<SaveData> SavedData = new List<SaveData>();

        public override void Entry(IModHelper helper)
        {
            RequestableHelper = Helper;
            RequestableMonitor = Monitor;

            helper.Events.Input.ButtonPressed += Input_ButtonPressed;

            helper.Events.World.ObjectListChanged += World_ObjectListChanged;
            helper.Events.World.ChestInventoryChanged += World_ChestInventoryChanged;

            helper.Events.GameLoop.Saving += GameLoop_Saving;
            helper.Events.GameLoop.SaveLoaded += GameLoop_SaveLoaded;
            helper.Events.GameLoop.GameLaunched += GameLoop_GameLaunched;

            helper.Events.Multiplayer.ModMessageReceived += Multiplayer_ModMessageReceived;
            helper.Events.Multiplayer.PeerConnected += Multiplayer_PeerConnected;
            CheckConfig();
        }


        private void CheckConfig()
        {
            RequestableConfig = Helper.ReadConfig<Config>();
            if (!(RequestableConfig.ChangeItemKey == "Quotes" || RequestableConfig.ChangeItemKey == "Quote")) return;
            RequestableConfig.ChangeItemKey = "OemQuotes";
            Helper.WriteConfig(RequestableConfig);
        }

        private void Multiplayer_PeerConnected(object sender, PeerConnectedEventArgs e)
        {
            if (!host) Helper.Multiplayer.SendMessage(SavedData, Helper.ModRegistry.ModID, new[] { Helper.ModRegistry.ModID }, new[] { e.Peer.PlayerID });
        }

        private void Multiplayer_ModMessageReceived(object sender, ModMessageReceivedEventArgs e)
        {
            if(e.FromModID == Helper.ModRegistry.ModID)
                SavedData = e.ReadAs<List<SaveData>>();
        }

        private void GameLoop_Saving(object sender, SavingEventArgs e)
        {
            if (!host) return;

            Helper.Data.WriteSaveData("MindMeltMax.ChestDisplay", SavedData);
            if (Game1.IsMultiplayer)
                Helper.Multiplayer.SendMessage(SavedData, Helper.ModRegistry.ModID, new[] { Helper.ModRegistry.ModID });
        }

        private void GameLoop_SaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            if (Game1.IsMasterGame || (Game1.IsMultiplayer && Game1.MasterPlayer == Game1.player)) host = true;
            if (!host) return;

            var data = Helper.Data.ReadSaveData<List<SaveData>>($"MindMeltMax.ChestDisplay");
            if (data == null) return;
            SavedData = data;
        }

        private void GameLoop_GameLaunched(object sender, GameLaunchedEventArgs e) => Patcher.Init(Helper);

        private void Input_ButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (!Context.CanPlayerMove)
                return;

            if(Utils.IsChangeItemKey(e.Button))
            {
                var OatT = Game1.player.currentLocation.getObjectAtTile((int)e.Cursor.Tile.X, (int)e.Cursor.Tile.Y);
                if (OatT != null && OatT is Chest)
                {
                    var chest = OatT as Chest;
                    if (Utils.nullChest(chest))
                        return;
                    if (Game1.player.CurrentItem != null && !(Game1.player.CurrentItem is Tool))
                    {
                        foreach(SaveData sd in SavedData)
                        {
                            if (sd.Item == Game1.player.CurrentItem.Name && ((Utils.isZeroType(Game1.player.CurrentItem) && sd.ItemQuality == 0) || sd.ItemQuality == (Game1.player.CurrentItem as SObject).Quality) && sd.Location == Game1.player.currentLocation.Name && sd.X == e.Cursor.Tile.X && sd.Y == e.Cursor.Tile.Y) return;
                            else if (sd.Location == Game1.player.currentLocation.Name && sd.X == e.Cursor.Tile.X && sd.Y == e.Cursor.Tile.Y)
                            {
                                Chest c = OatT as Chest;
                                c.addItem(Game1.player.CurrentItem.getOne());
                                sd.Item = Game1.player.CurrentItem.Name;
                                sd.ItemDescription = SUtils.getStandardDescriptionFromItem(Utils.getItemFromName(sd.Item, c), 1);
                                sd.ItemQuality = Utils.isZeroType(Game1.player.CurrentItem) ? 0 : (Game1.player.CurrentItem as SObject).Quality;
                                Game1.player.CurrentItem.Stack--;
                                Item held = Game1.player.CurrentItem;
                                if (held.Stack <= 0 || Utils.isZeroType(held)) Game1.player.removeItemFromInventory(held);
                                Helper.Multiplayer.SendMessage(SavedData, Helper.ModRegistry.ModID, new[] { Helper.ModRegistry.ModID });
                                return;
                            }
                        }
                        var quality = Utils.isZeroType(Game1.player.CurrentItem) ? 0 : (Game1.player.CurrentItem as SObject).Quality;
                        SavedData.Add(new SaveData(Game1.player.CurrentItem.Name, SUtils.getStandardDescriptionFromItem(Game1.player.CurrentItem, Game1.player.CurrentItem.Stack), quality, (int)e.Cursor.Tile.X, (int)e.Cursor.Tile.Y, Game1.player.currentLocation.Name));
                        Chest tmp = OatT as Chest;
                        tmp.addItem(Game1.player.CurrentItem.getOne());
                        Game1.player.CurrentItem.Stack--;
                        var currentItem = Game1.player.CurrentItem;
                        if (currentItem.Stack <= 0 || Utils.isZeroType(currentItem)) Game1.player.removeItemFromInventory(currentItem);
                        Helper.Multiplayer.SendMessage(SavedData, Helper.ModRegistry.ModID, new[] { Helper.ModRegistry.ModID });
                    }
                }
            }
        }

        private void World_ObjectListChanged(object sender, ObjectListChangedEventArgs e)
        {
            if(e.Removed != null)
            {
                List<SaveData> tmp = new List<SaveData>();
                foreach (var obj in e.Removed)
                {
                    if(obj.Value is Chest)
                    {
                        foreach(SaveData sd in SavedData)
                        {
                            if (obj.Key == new Vector2(sd.X, sd.Y) && e.Location == Game1.getLocationFromName(sd.Location))
                                tmp.Add(sd);
                        }
                    }
                }
                foreach (SaveData sd in tmp)
                    SavedData.Remove(sd);
            }
        }

        private void World_ChestInventoryChanged(object sender, ChestInventoryChangedEventArgs e)
        {
            List<SaveData> tmp = new List<SaveData>();
            if (e.Removed != null)
            {
                foreach (SaveData sd in SavedData)
                {
                    foreach (Item i in e.Removed)
                        if (i.Name == sd.Item && e.Location == Game1.getLocationFromName(sd.Location) && e.Chest == e.Location.getObjectAtTile(sd.X, sd.Y) && !RequestableConfig.RetainItem)
                            tmp.Add(sd);
                }

                foreach (SaveData sd in tmp)
                    SavedData.Remove(sd);
            }
        }
    }
}
