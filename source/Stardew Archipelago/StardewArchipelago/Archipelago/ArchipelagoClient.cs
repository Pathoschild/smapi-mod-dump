/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/agilbert1412/StardewArchipelago
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using Archipelago.MultiClient.Net;
using Archipelago.MultiClient.Net.BounceFeatures.DeathLink;
using Archipelago.MultiClient.Net.Enums;
using Archipelago.MultiClient.Net.Helpers;
using Archipelago.MultiClient.Net.MessageLog.Messages;
using Archipelago.MultiClient.Net.Models;
using Archipelago.MultiClient.Net.Packets;
using HarmonyLib;
using StardewArchipelago.Extensions;
using StardewModdingAPI;
using StardewValley;
using Color = Microsoft.Xna.Framework.Color;

namespace StardewArchipelago.Archipelago
{
    public class ArchipelagoClient
    {
        private const string GAME_NAME = "Stardew Valley";
        private IMonitor _console;
        private IModHelper _modHelper;
        private ArchipelagoSession _session;
        private DeathLinkService _deathLinkService;
        private Harmony _harmony;
        private DeathManager _deathManager;
        private ArchipelagoConnectionInfo _connectionInfo;
        private IManifest _modManifest;

        private Action _itemReceivedFunction;

        public bool IsConnected { get; private set; }
        public SlotData SlotData { get; private set; }
        public Dictionary<string, ScoutedLocation> ScoutedLocations { get; set; }
        public bool DeathLink => _connectionInfo?.DeathLink == true;

        private DataPackageCache _localDataPackage;
        // public Random MultiRandom { get; set; }

        public ArchipelagoClient(IMonitor console, IModHelper modHelper, Harmony harmony, Action itemReceivedFunction, IManifest manifest)
        {
            _console = console;
            _modHelper = modHelper;
            _harmony = harmony;
            _itemReceivedFunction = itemReceivedFunction;
            IsConnected = false;
            _modManifest = manifest;
            ScoutedLocations = new Dictionary<string, ScoutedLocation>();
            _localDataPackage = new DataPackageCache(modHelper);
        }

        public void Connect(ArchipelagoConnectionInfo connectionInfo, out string errorMessage)
        {
            DisconnectPermanently();
            var success = TryConnect(connectionInfo, out errorMessage);
            if (!success)
            {
                DisconnectPermanently();
                return;
            }

            if (!IsMultiworldVersionSupported())
            {
                errorMessage = $"This Multiworld has been created for StardewArchipelago version {SlotData.MultiworldVersion},\nbut this is StardewArchipelago version {_modManifest.Version}.\nPlease update to a compatible mod version.";
                DisconnectPermanently();
                return;
            }

            if (!SlotData.Mods.IsModStateCorrect(_modHelper, out errorMessage))
            {
                DisconnectPermanently();
                return;
            }
        }

        private bool TryConnect(ArchipelagoConnectionInfo connectionInfo, out string errorMessage)
        {
            LoginResult result;
            try
            {
                InitSession(connectionInfo);
                var itemsHandling = ItemsHandlingFlags.AllItems;
                var minimumVersion = new Version(0, 4, 0);
                var tags = connectionInfo.DeathLink == true ? new[] { "AP", "DeathLink" } : new[] { "AP" };
                result = _session.TryConnectAndLogin(GAME_NAME, _connectionInfo.SlotName, itemsHandling, minimumVersion, tags, null, _connectionInfo.Password);
            }
            catch (Exception e)
            {
                result = new LoginFailure(e.GetBaseException().Message);
            }

            if (!result.Successful)
            {
                var failure = (LoginFailure)result;
                errorMessage = $"Failed to Connect to {_connectionInfo.HostUrl}:{_connectionInfo.Port} as {_connectionInfo.SlotName}:";
                foreach (var error in failure.Errors)
                {
                    errorMessage += $"\n    {error}";
                }

                var detailedErrorMessage = errorMessage;
                foreach (var error in failure.ErrorCodes)
                {
                    detailedErrorMessage += $"\n    {error}";
                }

                _console.Log(detailedErrorMessage, LogLevel.Error);
                DisconnectAndCleanup();
                return false; // Did not connect, show the user the contents of `errorMessage`
            }

            errorMessage = "";

            // Successfully connected, `ArchipelagoSession` (assume statically defined as `session` from now on) can now be used to interact with the server and the returned `LoginSuccessful` contains some useful information about the initial connection (e.g. a copy of the slot data as `loginSuccess.SlotData`)
            var loginSuccess = (LoginSuccessful)result;
            var loginMessage = $"Connected to Archipelago server as {connectionInfo.SlotName} (Team {loginSuccess.Team}).";
            _console.Log(loginMessage, LogLevel.Info);

            // Must go AFTER a successful connection attempt
            InitializeSlotData(connectionInfo.SlotName, loginSuccess.SlotData);
            if (connectionInfo.DeathLink == null)
            {
                connectionInfo.DeathLink = SlotData.DeathLink;
            }
            InitializeAfterConnection();
            return true;
        }

        private void InitializeAfterConnection()
        {
            IsConnected = true;
			
            _session.Items.ItemReceived += OnItemReceived;
            _session.MessageLog.OnMessageReceived += OnMessageReceived;
            _session.Socket.ErrorReceived += SessionErrorReceived;
            _session.Socket.SocketClosed += SessionSocketClosed;

            InitializeDeathLink();
            // MultiRandom = new Random(SlotData.Seed);
        }

        public void Sync()
        {
            if (!MakeSureConnected(0))
            {
                return;
            }

            _session.Socket.SendPacket(new SyncPacket());
        }

        private void InitializeDeathLink()
        {
            if (_deathManager == null)
            {
                _deathManager = new DeathManager(_console, _modHelper, _harmony, this);
                _deathManager.HookIntoDeathlinkEvents();
            }

            _deathLinkService = _session.CreateDeathLinkService();
            _deathLinkService.OnDeathLinkReceived += ReceiveDeathLink;
            if (_connectionInfo.DeathLink == true)
            {
                _deathLinkService.EnableDeathLink();
            }
            else
            {
                _deathLinkService.DisableDeathLink();
            }
        }

        public void ToggleDeathlink()
        {
            if (_connectionInfo.DeathLink == true)
            {
                _deathLinkService.DisableDeathLink();
                _connectionInfo.DeathLink = false;
            }
            else
            {
                _deathLinkService.EnableDeathLink();
                _connectionInfo.DeathLink = true;
            }
        }

        private void InitializeSlotData(string slotName, Dictionary<string, object> slotDataFields)
        {
            SlotData = new SlotData(slotName, slotDataFields, _console);
        }

        private void InitSession(ArchipelagoConnectionInfo connectionInfo)
        {
            _session = ArchipelagoSessionFactory.CreateSession(connectionInfo.HostUrl,
                connectionInfo.Port);
            _connectionInfo = connectionInfo;
        }

        private void OnMessageReceived(LogMessage message)
        {
            var fullMessage = string.Join(" ", message.Parts.Select(str => str.Text));
            _console.Log(fullMessage, LogLevel.Info);

            fullMessage = fullMessage.Replace("<3", "<");

            switch (message)
            {
                case ChatLogMessage chatMessage:
                {
                    var color = chatMessage.Player.Name.GetAsBrightColor();
                    Game1.chatBox?.addMessage(fullMessage, color);
                    return;
                }
                case ItemSendLogMessage itemSendLogMessage:
                {
                    if (!itemSendLogMessage.IsRelatedToActivePlayer)
                    {
                        return;
                    }
                    var color = Color.Gold;
                    Game1.chatBox?.addMessage(fullMessage, color);
                    return;
                }
            }
        }

        public void SendMessage(string text)
        {
            if (!MakeSureConnected())
            {
                return;
            }

            var packet = new SayPacket()
            {
                Text = text
            };

            _session.Socket.SendPacket(packet);
        }

        private void OnItemReceived(ReceivedItemsHelper receivedItemsHelper)
        {
            if (!MakeSureConnected())
            {
                return;
            }
            
            _itemReceivedFunction();
        }

        public void ReportCheckedLocations(long[] locationIds)
        {
            if (!MakeSureConnected())
            {
                return;
            }

            _session.Locations.CompleteLocationChecks(locationIds);
        }

        public string GetPlayerName(int playerId)
        {
            return _session.Players.GetPlayerName(playerId) ?? "Archipelago";
        }

        public string GetPlayerAlias(string playerName)
        {
            var player = _session.Players.AllPlayers.FirstOrDefault(x => x.Name == playerName);
            if (player == null)
            {
                return null;
            }

            return player.Alias;
        }

        public bool IsStardewValleyPlayer(string playerName)
        {
            var player = _session.Players.AllPlayers.FirstOrDefault(x => x.Name == playerName);
            if (player == null)
            {
                player = _session.Players.AllPlayers.FirstOrDefault(x => x.Alias == playerName);
            }
            
            return player != null && player.Game == GAME_NAME;
        }

        public const string STRING_DATA_STORAGE_DELIMITER = "|||";
        public void AddToStringDataStorage(string key, string value)
        {
            if (!MakeSureConnected())
            {
                return;
            }

            var existingValue = ReadStringFromDataStorage(key);
            if (string.IsNullOrWhiteSpace(existingValue))
            {
                _session.DataStorage[Scope.Game, key] = value;
            }
            else
            {
                _session.DataStorage[Scope.Game, key] = existingValue + STRING_DATA_STORAGE_DELIMITER + value;
            }
        }

        public void SetStringDataStorage(string key, string value)
        {
            if (!MakeSureConnected())
            {
                return;
            }

            _session.DataStorage[Scope.Game, key] = value;
        }

        public bool StringExistsInDataStorage(string key)
        {
            if (!MakeSureConnected())
            {
                return false;
            }

            var value = _session.DataStorage[Scope.Game, key];
            return !string.IsNullOrWhiteSpace(value.To<string>());
        }

        public string ReadStringFromDataStorage(string key)
        {
            if (!MakeSureConnected())
            {
                return null;
            }

            var value = _session.DataStorage[Scope.Game, key];
            var stringValue = value.To<string>();
            if (string.IsNullOrWhiteSpace(stringValue))
            {
                return null;
            }

            return stringValue;
        }

        public void RemoveStringFromDataStorage(string key)
        {
            if (!MakeSureConnected())
            {
                return;
            }

            _session.DataStorage[Scope.Game, key] = "";
        }

        public Dictionary<string, long> GetAllCheckedLocations()
        {
            if (!MakeSureConnected())
            {
                return new Dictionary<string, long>();
            }

            var allLocationsCheckedIds = _session.Locations.AllLocationsChecked;
            var allLocationsChecked = allLocationsCheckedIds.ToDictionary(GetLocationName, x => x);
            return allLocationsChecked;
        }

        public List<ReceivedItem> GetAllReceivedItems()
        {
            if (!MakeSureConnected())
            {
                return new List<ReceivedItem>();
            }

            var allReceivedItems = new List<ReceivedItem>();
            var apItems = _session.Items.AllItemsReceived.ToArray();
            for (var itemIndex = 0; itemIndex < apItems.Length; itemIndex++)
            {
                var apItem = apItems[itemIndex];
                var itemName = GetItemName(apItem.Item);
                var playerName = GetPlayerName(apItem.Player);
                var locationName = GetLocationName(apItem.Location) ?? "Thin air";

                var receivedItem = new ReceivedItem(locationName, itemName, playerName, apItem.Location, apItem.Item,
                    apItem.Player, itemIndex);

                allReceivedItems.Add(receivedItem);
            }

            return allReceivedItems;
        }

        public Dictionary<string, int> GetAllReceivedItemNamesAndCounts()
        {
            if (!MakeSureConnected())
            {
                return new Dictionary<string, int>();
            }

            var receivedItemsGrouped = _session.Items.AllItemsReceived.GroupBy(x => x.Item);
            var receivedItemsWithCount = receivedItemsGrouped.ToDictionary(x => GetItemName(x.First().Item), x => x.Count());
            return receivedItemsWithCount;
        }

        public bool HasReceivedItem(string itemName)
        {
            return HasReceivedItem(itemName, out _);
        }

        public bool HasReceivedItem(string itemName, out string sendingPlayer)
        {
            sendingPlayer = "";
            if (!MakeSureConnected())
            {
                return false;
            }

            foreach (var receivedItem in _session.Items.AllItemsReceived)
            {
                if (GetItemName(receivedItem.Item) != itemName)
                {
                    continue;
                }

                sendingPlayer = _session.Players.GetPlayerName(receivedItem.Player);
                return true;
            }

            return false;
        }

        public int GetReceivedItemCount(string itemName)
        {
            if (!MakeSureConnected())
            {
                return 0;
            }

            return _session.Items.AllItemsReceived.Count(x => GetItemName(x.Item) == itemName);
        }

        public Hint[] GetHints()
        {
            if (!MakeSureConnected())
            {
                return Array.Empty<Hint>();
            }

            var hintTask = _session.DataStorage.GetHintsAsync();
            hintTask.Wait();
            return hintTask.Result;
        }

        public void ReportGoalCompletion()
        {
            if (!MakeSureConnected())
            {
                return;
            }

            var statusUpdatePacket = new StatusUpdatePacket();
            statusUpdatePacket.Status = ArchipelagoClientState.ClientGoal;
            _session.Socket.SendPacket(statusUpdatePacket);
        }

        public string GetLocationName(long locationId)
        {
            if (!MakeSureConnected())
            {
                return "";
            }
            
            var locationName = _session.Locations.GetLocationNameFromId(locationId);
            if (string.IsNullOrWhiteSpace(locationName))
            {
                locationName = _localDataPackage.GetLocalLocationName(locationId);
            }

            return locationName;
        }

        public bool LocationExists(string locationName)
        {
            var id = GetLocationId(locationName);
            return _session.Locations.AllLocations.Contains(id);
        }

        public long GetLocationId(string locationName, string gameName = GAME_NAME)
        {
            if (!MakeSureConnected())
            {
                return -1;
            }

            var locationId = _session.Locations.GetLocationIdFromName(gameName, locationName);
            if (locationId <= 0)
            {
                locationId = _localDataPackage.GetLocalLocationId(locationName);
            }

            return locationId;
        }

        public string GetItemName(long itemId)
        {
            if (!MakeSureConnected())
            {
                return "";
            }

            var itemName =  _session.Items.GetItemName(itemId);
            if (string.IsNullOrWhiteSpace(itemName))
            {
                itemName = _localDataPackage.GetLocalItemName(itemId);
            }

            return itemName;
        }

        public void SendDeathLink(string player, string reason = "Unknown cause")
        {
            if (!MakeSureConnected())
            {
                return;
            }

            _deathLinkService.SendDeathLink(new DeathLink(player, reason));
        }

        private void ReceiveDeathLink(DeathLink deathlink)
        {
            if (_connectionInfo.DeathLink != true)
            {
                return;
            }

            DeathManager.ReceiveDeathLink();
            var deathLinkMessage = $"You have been killed by {deathlink.Source} ({deathlink.Cause})";
            _console.Log(deathLinkMessage, LogLevel.Info);
            Game1.chatBox?.addInfoMessage(deathLinkMessage);
        }

        public ScoutedLocation ScoutSingleLocation(string locationName, bool createAsHint = false)
        {
            if (ScoutedLocations.ContainsKey(locationName))
            {
                return ScoutedLocations[locationName];
            }

            if (!MakeSureConnected())
            {
                _console.Log($"Could not find the id for location \"{locationName}\".");
                return null;
            }

            try
            {
                var locationId = GetLocationId(locationName);
                if (locationId == -1)
                {
                    _console.Log($"Could not find the id for location \"{locationName}\".");
                    return null;
                }

                var locationInfo = ScoutLocation(locationId, createAsHint);
                if (locationInfo.Locations.Length < 1)
                {
                    _console.Log($"Could not scout location \"{locationName}\".");
                    return null;
                }

                var firstLocationInfo = locationInfo.Locations[0];
                var itemName = GetItemName(firstLocationInfo.Item);
                var playerSlotName = _session.Players.GetPlayerName(firstLocationInfo.Player);

                var scoutedLocation = new ScoutedLocation(locationName, itemName, playerSlotName, locationId,
                    firstLocationInfo.Item, firstLocationInfo.Player);

                ScoutedLocations.Add(locationName, scoutedLocation);
                return scoutedLocation;
            }
            catch (Exception e)
            {
                _console.Log($"Could not scout location \"{locationName}\". Message: {e.Message}");
                return null;
            }
        }

        private LocationInfoPacket ScoutLocation(long locationId, bool createAsHint)
        {
            var scoutTask = _session.Locations.ScoutLocationsAsync(createAsHint, locationId);
            scoutTask.Wait();
            return scoutTask.Result;
        }

        private void SessionErrorReceived(Exception e, string message)
        {
            _console.Log(message, LogLevel.Error);
            Game1.chatBox?.addMessage("Connection to Archipelago lost. The game will try reconnecting later. Check the SMAPI console for details", Color.Red);
            _lastConnectFailure = DateTime.Now;
            DisconnectAndCleanup();
        }

        private void SessionSocketClosed(string reason)
        {
            _console.Log($"Connection to Archipelago lost: {reason}", LogLevel.Error);
            Game1.chatBox?.addMessage("Connection to Archipelago lost. The game will try reconnecting later. Check the SMAPI console for details", Color.Red);
            _lastConnectFailure = DateTime.Now;
            DisconnectAndCleanup();
        }

        public void DisconnectAndCleanup()
        {
            if (!IsConnected)
            {
                return;
            }

            if (_session != null)
            {
                _session.Items.ItemReceived -= OnItemReceived;
                _session.MessageLog.OnMessageReceived -= OnMessageReceived;
                _session.Socket.ErrorReceived -= SessionErrorReceived;
                _session.Socket.SocketClosed -= SessionSocketClosed;
                _session.Socket.DisconnectAsync();
            }
            _session = null;
            IsConnected = false;
        }

        public void DisconnectPermanently()
        {
            DisconnectAndCleanup();
            _connectionInfo = null;
        }

        private DateTime _lastConnectFailure;
        private const int THRESHOLD_TO_RETRY_CONNECTION_IN_SECONDS = 15;
        public bool MakeSureConnected(int threshold = THRESHOLD_TO_RETRY_CONNECTION_IN_SECONDS)
        {
            if (IsConnected)
            {
                return true;
            }

            if (_connectionInfo == null)
            {
                return false;
            }

            var now = DateTime.Now;
            var timeSinceLastFailure = now - _lastConnectFailure;
            if (timeSinceLastFailure.TotalSeconds < threshold)
            {
                return false;
            }

            TryConnect(_connectionInfo, out _);
            if (!IsConnected)
            {
                Game1.chatBox?.addMessage("Reconnection attempt failed", Color.Red);
                _lastConnectFailure = DateTime.Now;
                return false;
            }

            Game1.chatBox?.addMessage("Reconnection attempt successful!", Color.Green);
            return IsConnected;
        }
        
        public void APUpdate()
        {
            MakeSureConnected(60);
        }

        private bool IsMultiworldVersionSupported()
        {
            var majorVersion = _modManifest.Version.MajorVersion.ToString();
            var multiworldVersionParts = SlotData.MultiworldVersion.Split(".");
            if (multiworldVersionParts.Length < 3)
            {
                return false;
            }

            var multiworldMajor = multiworldVersionParts[0];
            var multiworldMinor = multiworldVersionParts[1];
            var multiworldFix = multiworldVersionParts[2];
            return majorVersion == multiworldMajor;
        }
    }
}
