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
        public static readonly ISemanticVersion CurrentDataFormatVersion = new SemanticVersion(1, 0, 0);

        /// <summary>
        /// The data model for an item on the to-do list.
        /// </summary>
        public class ListItem {
            public long Id { get; set; }
            public string Text { get; set; }
            public ListItem() { }
            public ListItem(long id, string text) {
                this.Id = id;
                this.Text = text;
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
                        theMod.Monitor.Log($"List information in this save is in a newer format version ({theList.DataFormatVersion}) than this version of {theMod.ModManifest.Name} uses ({CurrentDataFormatVersion}).  The next save will use this older version, which will result in losing any attributes added in the newer version.", LogLevel.Warn);
                    } else {
                        theMod.Monitor.Log($"Read list information in format version {theList.DataFormatVersion}.  It will be updated to the current format version ({CurrentDataFormatVersion}) on next save.", LogLevel.Info);
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
                    theMod.Monitor.Log($"Host does not have {theMod.ModManifest.Name} installed; list disabled.", LogLevel.Warn);
                } else {
                    if (!hostMod.Version.Equals(theMod.ModManifest.Version)) {
                        theMod.Monitor.Log($"Host has version {hostMod.Version} of {theMod.ModManifest.Name} installed, but you have version {theMod.ModManifest.Version}.  Attempting to proceed anyway.",
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

        private void DoMoveItem(long id, int direction) {
            var list = theList.Items;
            int idx = list.FindIndex(li => li.Id == id);
            if (idx >= 0) {
                int newIdx = idx + direction;
                if (newIdx >= 0 && newIdx < list.Count) {
                    var tmp = list[newIdx];
                    list[newIdx] = list[idx];
                    list[idx] = tmp;
                    Save();
                } else {
                    if (theMod.config.debug) {
                        theMod.Monitor.Log($"DoMoveItem could not move item {id} into position {newIdx}", LogLevel.Debug);
                    }
                }
            } else {
                if (theMod.config.debug) {
                    theMod.Monitor.Log($"DoMoveItem did not find item {id}", LogLevel.Debug);
                }
            }
        }

        /// <summary>
        /// Move the given item up in the list
        /// </summary>
        /// <param name="id">The ID of the item to move</param>
        public void MoveItemUp(long id) {
            if (Context.IsMainPlayer) {
                DoMoveItem(id, -1);
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
                DoMoveItem(id, 1);
            } else {
                SendToHost(MessageType.MoveItemDown, id);
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

            // messages sent from the host
            public const string ListData = "ListData";
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
                        DoMoveItem(e.ReadAs<long>(), -1);
                        break;
                    case MessageType.MoveItemDown:
                        DoMoveItem(e.ReadAs<long>(), 1);
                        break;
                    default:
                        theMod.Monitor.Log($"Ignoring unexpected message type {e.Type} from player {e.FromPlayerID} ({Game1.getFarmer(e.FromPlayerID)?.Name})",
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
                                theMod.Monitor.Log($"Host is using a newer data format version ({newList.DataFormatVersion}) ({theList.DataFormatVersion}) than this version of {theMod.ModManifest.Name} uses ({CurrentDataFormatVersion}).  Since it has a different major version number, we're not going to try to handle it.", LogLevel.Warn);
                            } else {
                                theMod.Monitor.Log($"Host is using a newer data format version ({newList.DataFormatVersion}) ({theList.DataFormatVersion}) than this version of {theMod.ModManifest.Name} uses ({CurrentDataFormatVersion}), but we'll do our best to deal with it.", LogLevel.Warn);
                            }
                        } else {
                            theMod.Monitor.Log($"Host is using data format version {newList.DataFormatVersion}, which is older than the current format version ({CurrentDataFormatVersion}).  Some features may be unavailable.", LogLevel.Info);
                        }
                    }
                    theList = newList;
                    this.OnChanged?.Invoke(this, this.Items);
                } else {
                    theMod.Monitor.Log($"Ignoring unexpected message type {e.Type} from player {e.FromPlayerID} ({Game1.getFarmer(e.FromPlayerID)?.Name})",
                        LogLevel.Warn);
                }
            }
        }
    }
}
