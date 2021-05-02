/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/jltaylor-us/StardewToDew
**
*************************************************/

// Copyright 2020 Jamie Taylor
using System;
using System.Collections.Generic;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace ToDew {
    /// <summary>
    /// Encapsulates the backing data model for the to-do list (including serialization and multiplayer support).
    /// </summary>
    public class ToDoList {
        /// <summary>The key used for this data in the game save file.</summary>
        private const string SaveKey = "todew-list";

        /// <summary>The version of the data format.</summary>
        /// The data format comprises the ListData object (and any objects reachable therefrom).
        public static readonly ISemanticVersion CurrentDataFormatVersion = new SemanticVersion(1, 1, 0);

        [Flags]
        public enum WeatherVisiblity {
            None = 0,
            Raining = 0b0001,
            NotRaining = 0b0010,
            All = -1
        };

        [Flags]
        public enum DayVisibility {
            None = 0,
            Sunday    = 0b00000001,
            Monday    = 0b00000010,
            Tuesday   = 0b00000100,
            Wednesday = 0b00001000,
            Thurdsay  = 0b00010000,
            Friday    = 0b00100000,
            Saturday  = 0b01000000,
            Spring    = 0b0001_00000000,
            Summer    = 0b0010_00000000,
            Fall      = 0b0100_00000000,
            Winter    = 0b1000_00000000,
            Week1     = 0b0001_0000_00000000,
            Week2     = 0b0010_0000_00000000,
            Week3     = 0b0100_0000_00000000,
            Week4     = 0b1000_0000_00000000,
            All = -1
        }

        /// <summary>
        /// The data model for an item on the to-do list.
        /// </summary>
        public class ListItem {
            public long Id { get; set; }
            public string Text { get; set; }
            public bool IsBold { get; set; }
            public bool IsDone { get; set; }
            public bool IsHeader { get; set; }
            public bool IsRepeating { get; set; }
            public bool HideInOverlay { get; set; }
            public WeatherVisiblity FarmWeatherVisiblity { get; set; }
            public WeatherVisiblity IslandWeatherVisiblity { get; set; }
            public DayVisibility DayOfWeekVisibility { get; set; }

            // transient; reset at beginning of day or when flags change
            public bool IsVisibleToday { get; set; }

            public ListItem() {
                // these provide the default values for items read from a serialized form
                // that doesn't include these properties
                FarmWeatherVisiblity = WeatherVisiblity.All;
                DayOfWeekVisibility = DayVisibility.All;
            }
            public ListItem(long id, string text) {
                this.Id = id;
                this.Text = text;
                // defaults for new items
                FarmWeatherVisiblity = WeatherVisiblity.All;
                DayOfWeekVisibility = DayVisibility.All;
                IsVisibleToday = true;
            }

            internal void RefreshVisibility(bool farmRaining, bool islandRaining) {
                DayVisibility dayOfWeek = (DayVisibility)(1 << (Game1.dayOfMonth % 7));
                DayVisibility season = (DayVisibility)((int)DayVisibility.Spring << Utility.getSeasonNumber(Game1.currentSeason));
                DayVisibility week = (DayVisibility)((int)DayVisibility.Week1 << ((Game1.dayOfMonth-1) / 7));
                bool dateVisibility = DayOfWeekVisibility.HasFlag(dayOfWeek | week | season);
                bool weatherVisibility = false;
                weatherVisibility |= FarmWeatherVisiblity.HasFlag(farmRaining ? WeatherVisiblity.Raining : WeatherVisiblity.NotRaining);
                weatherVisibility |= IslandWeatherVisiblity.HasFlag(islandRaining ? WeatherVisiblity.Raining : WeatherVisiblity.NotRaining);
                IsVisibleToday = weatherVisibility && dateVisibility;
            }

            internal void RefreshVisibility() {
                bool farmRaining = Game1.IsRainingHere(Game1.getFarm());
                bool islandRaining = Game1.IsRainingHere(Game1.getLocationFromName("IslandSouth"));
                RefreshVisibility(farmRaining, islandRaining);
            }
        }

        /// <summary>
        /// The data model for the entire to-do list.
        /// </summary>
        public class ListData {
            public ISemanticVersion DataFormatVersion { get; set; } = CurrentDataFormatVersion;
            public List<ListItem> Items { get; set; } = new List<ListItem>();
            public ListData() { }
            public ListData(NetworkListData data) {
                this.DataFormatVersion = new SemanticVersion(data.DataFormatVersion);
                this.Items = data.Items;
            }
        }
        /// <summary>
        /// The data model used to send the ListData between mod instances in multiplayer.
        /// </summary>
        /// Because SMAPI before 3.8.2 does not support deserializing an ISemanticVersion
        /// in the network code, even though it does in the save file code.
        /// (see https://github.com/Pathoschild/SMAPI/issues/745)
        public class NetworkListData {
            public string DataFormatVersion { get; set; } = CurrentDataFormatVersion.ToString();
            public List<ListItem> Items { get; set; } = new List<ListItem>();
            public NetworkListData() { }
            public NetworkListData(ListData data) {
                this.DataFormatVersion = data.DataFormatVersion.ToString();
                this.Items = data.Items;
            }
        }

        private readonly ModEntry theMod;
        private ListData theList;
        private bool _incompatibleMultiplayerHost = false;

        /// <summary>
        /// If true, then the current player is a farmhand and the host player does not have
        /// a version of To-Dew that we think we can talk to.
        /// </summary>
        public bool IncompatibleMultiplayerHost { get => _incompatibleMultiplayerHost; }
        /// <summary>The current to-do list.</summary>
        public List<ListItem> Items => theList.Items;

        /// <summary>
        /// Construct a new to-do list, restoring state as apropriate.
        /// </summary>
        /// If the current player is the host then read the to-do list from the current save file.
        /// If the current player is a farmhand, send a request to the host for the current list.
        /// Performs various checking for save format versions (as host) and mod versions (in
        /// multiplayer) and attempts to be smart (or at least not stupid) about issuing appropriate
        /// info or warning messages to the log.
        /// <param name="theMod">The To-Dew ModEntry object</param>
        public ToDoList(ModEntry theMod) {
            this.theMod = theMod;
            if (Context.IsMainPlayer) {
                this.theList = theMod.Helper.Data.ReadSaveData<ListData>(SaveKey) ?? new ListData();
                if (!theList.DataFormatVersion.Equals(CurrentDataFormatVersion)) {
                    if (theList.DataFormatVersion.IsNewerThan(CurrentDataFormatVersion)) {
                        theMod.Monitor.Log(I18n.Message_SaveVersionNewer(saveFormatVersion: theList.DataFormatVersion, modName: theMod.ModManifest.Name, modFormatVersion: CurrentDataFormatVersion), LogLevel.Warn);
                    } else {
                        theMod.Monitor.Log(I18n.Message_SaveVersionOlder(saveFormatVersion: theList.DataFormatVersion, modFormatVersion: CurrentDataFormatVersion), LogLevel.Info);
                    }
                    theList.DataFormatVersion = CurrentDataFormatVersion;
                }
            } else {
                // initialize with an empty list until we get the content from the host
                theList = new ListData();
                IMultiplayerPeer host = theMod.Helper.Multiplayer.GetConnectedPlayer(Game1.MasterPlayer.UniqueMultiplayerID);
                var hostMod = host.GetMod(theMod.ModManifest.UniqueID);
                if (hostMod == null) {
                    _incompatibleMultiplayerHost = true;
                    theMod.Monitor.Log(I18n.Message_HostNoMod(modName: theMod.ModManifest.Name), LogLevel.Warn);
                } else {
                    if (!hostMod.Version.Equals(theMod.ModManifest.Version)) {
                        theMod.Monitor.Log(I18n.Message_HostDifferentModVersion(hostVersion: hostMod.Version, modName: theMod.ModManifest.Name, myVersion: theMod.ModManifest.Version),
                            hostMod.Version.MajorVersion == theMod.ModManifest.Version.MajorVersion ? LogLevel.Info : LogLevel.Warn);
                    }
                    SendToHost(MessageType.RequestList, Game1.player.UniqueMultiplayerID);
                }
            }
        }

        /// <summary>The event raised when the list changes.</summary>
        public event EventHandler<List<ListItem>> OnChanged;

        private void DoAddItem(string text) {
            ListItem item = new ListItem(theMod.Helper.Multiplayer.GetNewID(), text);
            theList.Items.Add(item);
            Save();
        }

        /// <summary>
        /// Add an item to the to-do list.
        /// </summary>
        /// <param name="text"></param>
        public void AddItem(string text) {
            if (Context.IsMainPlayer) {
                DoAddItem(text);
            } else {
                SendToHost(MessageType.AddItem, text);
            }
        }

        /// <summary>
        /// Save the current list state and broadcast it to farmhands.
        /// </summary>
        /// Should only be called by the main player.
        private void Save() {
            theMod.Helper.Data.WriteSaveData<ListData>(SaveKey, theList);
            this.OnChanged?.Invoke(this, this.Items);
            if (Context.IsMultiplayer) {
                // It doesn't actually hurt to do this in single-player, but this avoids
                // adding extra debug output to the log.
                if (theMod.config.debug) {
                    theMod.Monitor.Log($"Broadcasting list state", LogLevel.Debug);
                }
                theMod.Helper.Multiplayer.SendMessage(new NetworkListData(this.theList), MessageType.ListData, new[] { theMod.ModManifest.UniqueID });
            }
        }

        private void DoDeleteItem(long id) {
            theList.Items.RemoveAll(li => li.Id == id);
            Save();
        }

        /// <summary>
        /// Remove an item from the to-do list.
        /// </summary>
        /// <param name="id">The ID of the item to remove</param>
        public void DeleteItem(long id) {
            if (Context.IsMainPlayer) {
                DoDeleteItem(id);
            } else {
                SendToHost(MessageType.DeleteItem, id);
            }
        }

        private void DoMoveItemAtIndexToTop(int idx) {
            var list = theList.Items;
            var movedItem = list[idx];
            for (int i = idx; i > 0; i--) {
                list[i] = list[i - 1];
            }
            list[0] = movedItem;
            Save();
        }

        private void DoMoveItemAtIndexToBottom(int idx) {
            var list = theList.Items;
            var movedItem = list[idx];
            for (int i = idx; i < list.Count - 1; i++) {
                list[i] = list[i + 1];
            }
            list[list.Count - 1] = movedItem;
            Save();
        }

        /// <summary>
        /// Move the item at the given index in the list to the top of the list
        /// </summary>
        /// <param name="idx">The index of the item to move</param>
        public void MoveItemAtIndexToTop(int idx) {
            if (Context.IsMainPlayer) {
                DoMoveItemAtIndexToTop(idx);
            } else {
                SendToHost(MessageType.MoveItemTop, theList.Items[idx].Id);
            }
        }

        /// <summary>
        /// Move the item at the given index in the list to the bottom of the list
        /// </summary>
        /// <param name="idx">The index of the item to move</param>
        public void MoveItemAtIndexToBottom(int idx) {
            if (Context.IsMainPlayer) {
                DoMoveItemAtIndexToBottom(idx);
            } else {
                SendToHost(MessageType.MoveItemBottom, theList.Items[idx].Id);
            }
        }

        private void DoSwapItemsAtIndex(int idx, int direction) {
            var list = theList.Items;
            int newIdx = idx + direction;
            if (newIdx >= 0 && newIdx < list.Count) {
                var tmp = list[newIdx];
                list[newIdx] = list[idx];
                list[idx] = tmp;
                Save();
            } else {
                if (theMod.config.debug) {
                    theMod.Monitor.Log($"DoMoveItem could not move item at index {idx} into position {newIdx}", LogLevel.Debug);
                }
            }
        }

        /// <summary>
        /// Move the given item up in the list
        /// </summary>
        /// <param name="id">The ID of the item to move</param>
        public void MoveItemUp(long id) {
            if (Context.IsMainPlayer) {
                CallWithItemIndex("MoveItemUp", (idx) => { DoSwapItemsAtIndex(idx, -1); }, id);
            } else {
                SendToHost(MessageType.MoveItemUp, id);
            }
        }

        /// <summary>
        /// Move the given item down in the list
        /// </summary>
        /// <param name="id">The ID of the item to move</param>
        public void MoveItemDown(long id) {
            if (Context.IsMainPlayer) {
                CallWithItemIndex("MoveItemDown", (idx) => { DoSwapItemsAtIndex(idx, 1); }, id);
            } else {
                SendToHost(MessageType.MoveItemDown, id);
            }
        }

        /// <summary>
        /// Mark an item done (or not done).
        /// </summary>
        /// <param name="id">The item to update</param>
        /// <param name="isDone">the new value of IsDone</param>
        public void SetItemDone(ListItem item, bool isDone) {
            if (Context.IsMainPlayer) {
                item.IsDone = isDone;
                Save();
            } else {
                SendToHost(MessageType.MarkItemDone, new Tuple<long, bool>(item.Id, isDone));
            }
        }

        /// <summary>
        /// Mark an item as having bold text (or not)
        /// </summary>
        /// <param name="id">The item to update</param>
        /// <param name="isBold">whether the text should be bold</param>
        public void SetItemBold(ListItem item, bool isBold) {
            if (Context.IsMainPlayer) {
                item.IsBold = isBold;
                Save();
            } else {
                SendToHost(MessageType.SetBoolProperty, new Tuple<long, string, bool>(item.Id, "IsBold", isBold));
            }
        }

        /// <summary>
        /// Mark an item as a header (or not).  Headers can not be marked done (in the UI).
        /// </summary>
        /// <param name="id">The item to update</param>
        /// <param name="isHeader">whether the item is a header</param>
        public void SetItemHeader(ListItem item, bool isHeader) {
            if (Context.IsMainPlayer) {
                item.IsHeader = isHeader;
                Save();
            } else {
                SendToHost(MessageType.SetBoolProperty, new Tuple<long, string, bool>(item.Id, "IsHeader", isHeader));
            }
        }

        /// <summary>
        /// Mark an item as repeating (or not).  Reapeating items that are marked done are
        /// reset at the end of the day rather than deleted.
        /// </summary>
        /// <param name="id">The item to update</param>
        /// <param name="isRepeating">whether the item is repeating</param>
        public void SetItemRepeating(ListItem item, bool isRepeating) {
            if (Context.IsMainPlayer) {
                item.IsRepeating = isRepeating;
                Save();
            } else {
                SendToHost(MessageType.SetBoolProperty, new Tuple<long, string, bool>(item.Id, "IsRepeating", isRepeating));
            }
        }

        /// <summary>
        /// Mark an item as hidden in the overlay (or not).
        /// </summary>
        /// <param name="id">The item to update</param>
        /// <param name="isHidden">whether the item is hidden in the overlay</param>
        public void SetItemHiddenInOverlay(ListItem item, bool isHidden) {
            if (Context.IsMainPlayer) {
                item.HideInOverlay = isHidden;
                Save();
            } else {
                SendToHost(MessageType.SetBoolProperty, new Tuple<long, string, bool>(item.Id, "HideInOverlay", isHidden));
            }
        }

        /// <summary>
        /// Set or clear flags in the weather visibility mask of an item.
        /// </summary>
        /// <param name="item">The item to update</param>
        /// <param name="forIsland">Whether this update is for the island (vs farm) weather</param>
        /// <param name="flag">The flag to update</param>
        /// <param name="value">whether the flag should be set (true) or cleared (false)</param>
        public void SetItemWeatherVisibilityFlag(ListItem item, bool forIsland, WeatherVisiblity flag, bool value) {
            if (Context.IsMainPlayer) {
                if (forIsland) {
                    if (value) {
                        item.IslandWeatherVisiblity |= flag;
                    } else {
                        item.IslandWeatherVisiblity &= ~flag;
                    }
                } else {
                    if (value) {
                        item.FarmWeatherVisiblity |= flag;
                    } else {
                        item.FarmWeatherVisiblity &= ~flag;
                    }
                }
                item.RefreshVisibility();
                Save();
            } else {
                SendToHost(MessageType.SetWeatherFlag, new Tuple<long, bool, WeatherVisiblity, bool>(item.Id, forIsland, flag, value));
            }
        }

        /// <summary>
        /// Set or clear flags in the day-of-week visibility mask of an item.
        /// </summary>
        /// <param name="item">The item to update</param>
        /// <param name="dayOfWeek">The flag to update</param>
        /// <param name="value">whether the flag should be set (true) or cleared (false)</param>
        public void SetItemDayVisibilityFlag(ListItem item, DayVisibility flag, bool value) {
            if (Context.IsMainPlayer) {
                if (value) {
                    item.DayOfWeekVisibility |= flag;
                } else {
                    item.DayOfWeekVisibility &= ~flag;
                }
                item.RefreshVisibility();
                Save();
            } else {
                SendToHost(MessageType.SetDayOfWeekFlag, new Tuple<long, DayVisibility, bool>(item.Id, flag, value));
            }
        }

        /// <summary>
        /// Set an item's text.
        /// </summary>
        /// <param name="id">The item to update</param>
        /// <param name="text">the new value of the item's text</param>
        public void SetItemText(ListItem item, string text) {
            if (Context.IsMainPlayer) {
                item.Text = text;
                Save();
            } else {
                SendToHost(MessageType.SetItemText, new Tuple<long, string>(item.Id, text));
            }
        }

        /// <summary>
        /// Removes completed items from the list before saving, resets repeating items.
        /// </summary>
        public void PreSaveCleanup() {
            if (Context.IsMainPlayer) {
                foreach (var item in theList.Items) {
                    if (item.IsRepeating) item.IsDone = false;
                    item.IsVisibleToday = false;
                }
                theList.Items.RemoveAll(li => li.IsDone);
                Save();
            }
        }

        public void RefreshVisibility() {
            if (Context.IsMainPlayer) {
                bool farmRaining = Game1.IsRainingHere(Game1.getFarm());
                bool islandRaining = Game1.IsRainingHere(Game1.getLocationFromName("IslandSouth"));
                foreach (var item in theList.Items) {
                    item.RefreshVisibility(farmRaining, islandRaining);
                }
                Save();
            }
        }

        private void SendToHost<T>(string messageType, T val) {
            if (theMod.config.debug) {
                theMod.Monitor.Log($"Sending message {messageType} to host {Game1.MasterPlayer.UniqueMultiplayerID}", LogLevel.Debug);
            }
            theMod.Helper.Multiplayer.SendMessage(val, messageType, new[] { theMod.ModManifest.UniqueID }, new[] { Game1.MasterPlayer.UniqueMultiplayerID });

        }

        /// <summary>
        /// Constants for the "type" of messages used to communicate with other instances of this mod in multiplayer.
        /// </summary>
        private static class MessageType {
            // messages sent to the host
            public const string RequestList = "RequestList";
            public const string AddItem = "AddItem";
            public const string DeleteItem = "DeleteItem";
            public const string MoveItemUp = "MoveItemUp";
            public const string MoveItemDown = "MoveItemDown";
            public const string MoveItemTop = "MoveItemTop";
            public const string MoveItemBottom = "MoveItemBottom";
            public const string MarkItemDone = "MarkItemDone";
            public const string SetItemText = "SetItemText";
            public const string SetBoolProperty = "SetBoolProperty";
            public const string SetWeatherFlag = "SetWeatherFlag";
            public const string SetDayOfWeekFlag = "SetDayOfWeekFlag"; // Deprecated
            public const string SetDayVisibilityFlag = "SetDayVisibilityFlag";

            // messages sent from the host
            public const string ListData = "ListData";
        }

        private void CallWithItemIndex(string who, Action<int> action, long itemId) {
            int idx = theList.Items.FindIndex(li => li.Id == itemId);
            if (idx < 0) {
                theMod.Monitor.Log($"{who} did not find item ID {itemId}", LogLevel.Debug);
            } else {
                action(idx);
            }
        }

        private void CallWithItem(string who, Action<ListItem> action, long itemId) {
            var item = theList.Items.Find(li => li.Id == itemId);
            if (item != null) {
                action(item);
            } else {
                theMod.Monitor.Log($"{who} did not find item {itemId}", LogLevel.Debug);
            }
        }
        private void CallWithItem<T>(string who, Action<ListItem, T> action, long itemId, T arg) {
            var item = theList.Items.Find(li => li.Id == itemId);
            if (item != null) {
                action(item, arg);
            } else {
                theMod.Monitor.Log($"{who} did not find item {itemId}", LogLevel.Debug);
            }
        }

        /// <summary>
        /// Handle a message from another instance of this mod in multiplayer.
        /// </summary>
        /// <param name="e">The message received event</param>
        public void ReceiveModMessage(ModMessageReceivedEventArgs e) {
            if (theMod.config.debug) {
                theMod.Monitor.Log($"Processing received message {e.Type}", LogLevel.Debug);
            }
            if (Context.IsMainPlayer) {
                switch (e.Type) {
                    case MessageType.RequestList:
                        theMod.Helper.Multiplayer.SendMessage(new NetworkListData(this.theList), MessageType.ListData, new[] { theMod.ModManifest.UniqueID }, new[] { e.ReadAs<long>() });
                        break;
                    case MessageType.AddItem:
                        DoAddItem(e.ReadAs<string>());
                        break;
                    case MessageType.DeleteItem:
                        DoDeleteItem(e.ReadAs<long>());
                        break;
                    case MessageType.MoveItemUp:
                        MoveItemUp(e.ReadAs<long>());
                        break;
                    case MessageType.MoveItemDown:
                        MoveItemDown(e.ReadAs<long>());
                        break;
                    case MessageType.MoveItemTop:
                        CallWithItemIndex("MoveItemTop", DoMoveItemAtIndexToTop, e.ReadAs<long>());
                        break;

                    case MessageType.MoveItemBottom:
                        CallWithItemIndex("MoveItemTop", DoMoveItemAtIndexToBottom, e.ReadAs<long>());
                        break;

                    case MessageType.MarkItemDone: {
                            var t = e.ReadAs<Tuple<long, bool>>();
                            CallWithItem("SetItemDone", SetItemDone, t.Item1, t.Item2);
                            break;
                        }
                    case MessageType.SetItemText: {
                            var t = e.ReadAs<Tuple<long, string>>();
                            CallWithItem("SetItemText", SetItemText, t.Item1, t.Item2);
                            break;
                        }
                    case MessageType.SetBoolProperty: {
                            var t = e.ReadAs<Tuple<long, string, bool>>();
                            long itemId = t.Item1;
                            string propName = t.Item2;
                            bool newVal = t.Item3;
                            CallWithItem("SetBoolProperty", (li) => {
                                IReflectedProperty<bool> prop = theMod.Helper.Reflection.GetProperty<bool>(li, propName, false);
                                if (prop != null) {
                                    prop.SetValue(newVal);
                                    Save();
                                } else {
                                    theMod.Monitor.Log($"Ignoring SetBoolProperty for unknown property {propName}", LogLevel.Debug);
                                }
                            }, itemId);
                            break;
                        }
                    case MessageType.SetWeatherFlag: {
                            var t = e.ReadAs<Tuple<long, bool, WeatherVisiblity, bool>>();
                            CallWithItem("SetWeatherFlag", (li) => {
                                SetItemWeatherVisibilityFlag(li, t.Item2, t.Item3, t.Item4);
                            }, t.Item1);
                            break;
                        }
                    case MessageType.SetDayOfWeekFlag:
                    case MessageType.SetDayVisibilityFlag: {
                            var t = e.ReadAs<Tuple<long, DayVisibility, bool>>();
                            CallWithItem("SetDayVisibilityFlag", (li) => {
                                SetItemDayVisibilityFlag(li, t.Item2, t.Item3);
                            }, t.Item1);
                            break;
                        }
                    default:
                        theMod.Monitor.Log(I18n.Message_IgnoringUnexpectedMessageType(messageType: e.Type, fromId: e.FromPlayerID, fromName: Game1.getFarmer(e.FromPlayerID)?.Name),
                            LogLevel.Warn);
                        break;
                }
            } else {
                if (e.Type == MessageType.ListData) {
                    ListData newList = new ListData(e.ReadAs<NetworkListData>());
                    if (!newList.DataFormatVersion.Equals(theList.DataFormatVersion)) {
                        if (newList.DataFormatVersion.IsNewerThan(theList.DataFormatVersion)) {
                            if (newList.DataFormatVersion.MajorVersion != theList.DataFormatVersion.MajorVersion) {
                                _incompatibleMultiplayerHost = true;
                                theMod.Monitor.Log(I18n.Message_HostNewerDataFormatMajor(hostVersion: newList.DataFormatVersion, modName: theMod.ModManifest.Name, myVersion: CurrentDataFormatVersion), LogLevel.Warn);
                            } else {
                                theMod.Monitor.Log(I18n.Message_HostNewerDataFormatMinor(hostVersion: newList.DataFormatVersion, modName: theMod.ModManifest.Name, myVersion: CurrentDataFormatVersion), LogLevel.Warn);
                            }
                        } else {
                            theMod.Monitor.Log(I18n.Message_HostOlderDataFormat(hostVersion: newList.DataFormatVersion, myVersion: CurrentDataFormatVersion), LogLevel.Info);
                        }
                    }
                    theList = newList;
                    this.OnChanged?.Invoke(this, this.Items);
                } else {
                    theMod.Monitor.Log(I18n.Message_IgnoringUnexpectedMessageType(messageType: e.Type, fromId: e.FromPlayerID, fromName: Game1.getFarmer(e.FromPlayerID)?.Name),
                        LogLevel.Warn);
                }
            }
        }
    }
}
