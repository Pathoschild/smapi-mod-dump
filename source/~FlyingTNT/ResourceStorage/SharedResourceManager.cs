/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/FlyingTNT/StardewValleyMods
**
*************************************************/

using Common.Utilities;
using Newtonsoft.Json;
using ResourceStorage.BetterCrafting;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using System;
using System.Collections.Generic;

namespace ResourceStorage
{
    internal class SharedResourceManager
    {
        public static IManifest Manifest;
        public static IMonitor SMonitor;
        public static IModHelper SHelper;
        public static ModConfig Config;

        /// <summary> Whether or not this player should use the shared resource dictionary. </summary>
        public static readonly PerScreen<bool> UseSharedResources = new PerScreen<bool>();

        /// <summary> The key for UseSharedResources in the players' mod data </summary>
        const string useSharedResourcesKey = "FlyingTNT.ResourceStorage/use-shared-resources";

        /// <summary> The key of the resource dictionary in the save data </summary>
        const string saveDataKey = "shared-resource-dictionary";

        /// <summary> The mod message type for a dictionary request (requesting the dictionary from the host). </summary>
        const string DictionaryRequest = "DictionaryRequest";

        /// <summary> The mod message type for a dictionary response (the host sending the dictionary to a player). </summary>
        const string DictionaryResponse = "DictionaryResponse";

        /// <summary> The mod message type for a resource modification request (changing the amount of a specific resource). </summary>
        const string ResourceModification = "ResourceModification";

        /// <summary> The dictionary for shared multiplayer resources. </summary>
        private static readonly PerScreen<Dictionary<string, long>> sharedResourceDictionary = new PerScreen<Dictionary<string, long>>(() => new()); // Initialized to an empty dict to avoid null exceptions; this will be overwritten

        /// <summary> The number of the last update to the dictionary this instance recieved from the host. If this is the host instance, it is simply the number of updates that have been sent. </summary>
        private static readonly PerScreen<int> lastUpdateFromHost = new PerScreen<int>(() => 0);

        public static void Initialize(IMonitor monitor, IModHelper helper, ModConfig config, IManifest manifest)
        {
            SMonitor = monitor;
            SHelper = helper;
            Config = config;
            Manifest = manifest;

            SHelper.Events.GameLoop.SaveLoaded += GameLoop_SaveLoaded;
            SHelper.Events.GameLoop.Saving += GameLoop_Saving;
            SHelper.Events.Multiplayer.ModMessageReceived += Multiplayer_ModMessageRecieved;
        }

        public static void GameLoop_SaveLoaded(object sender, SaveLoadedEventArgs args)
        {
            InitializeShouldUseSharedResources();
            IntitializeDictionary();
        }

        /// <summary>
        /// Load the value of UseSharedResources from the player's mod data.
        /// </summary>
        public static void InitializeShouldUseSharedResources()
        {
            UseSharedResources.Value = PerPlayerConfig.LoadConfigOption(Game1.player, useSharedResourcesKey, defaultValue: false);
        }

        /// <summary>
        /// Load the resource dictionary from the save game data if the host, or request the dictionary from the host otherwise. 
        /// </summary>
        public static void IntitializeDictionary()
        {
            // If the player is not the host, request the dictionry from the host.
            if (!Context.IsMainPlayer)
            {
                RequestDictionaryFromHost();
                return;
            }

            // If the player is the host, read the dictionary from the save data.
            if (PerSaveConfig.TryLoadConfigOption(SHelper, saveDataKey, out Dictionary<string, long> dictionary))
            {
                SMonitor.Log("Loaded Dictionary!");
                sharedResourceDictionary.Value = dictionary;
            }
            else
            {
                SMonitor.Log("No mod data found; making a new dictionary.");
                sharedResourceDictionary.Value = new();
            }
        }

        public static void GameLoop_Saving(object sender, SavingEventArgs args)
        {
            if (!Context.IsMainPlayer)
                return;

            SMonitor.Log("Saving the shared resource dictionary.");
            PerSaveConfig.SaveConfigOption(SHelper, saveDataKey, sharedResourceDictionary.Value);
        }

        public static void Multiplayer_ModMessageRecieved(object sender, ModMessageReceivedEventArgs args)
        {
            try
            {
                if (args.FromModID != Manifest.UniqueID)
                {
                    return;
                }

                SMonitor.Log($"Recieved message {args.Type} from {args.FromPlayerID}");

                if (args.Type == ResourceModification)
                {
                    HandleResourceModificationRequest(args);
                    return;
                }

                if (args.Type == DictionaryRequest)
                {
                    HandleResourceDictionaryRequest(args);
                    return;
                }

                if (args.Type == DictionaryResponse)
                {
                    HandleResourceDictionaryResponse(args);
                    return;
                }
            }
            catch (Exception ex)
            {
                SMonitor.Log($"Failed in {nameof(Multiplayer_ModMessageRecieved)} {ex}", LogLevel.Error);
            }
        }

        /// <summary>
        /// Handles a resource modification request.
        /// A resource modification request is a mod message informing the other players that the resource dictionary was changed.<br></br>
        /// Generally for each chnage to the dictionary, two modification requests are sent. First, an instance broadcasts the request to all instances, and they update their dictionaries accordingly.
        /// Once the host instance recieves the request, it will sent out another request confirming the change. If the other instances' dictionaries match what the host says they should be,
        /// nothing happens, but otherwise they update their dictionaries to match the host.<br></br>
        /// The contents of the request are stored in a <see cref="ResourceModificationRequest"/>.
        /// </summary>
        /// <param name="args"></param>
        static void HandleResourceModificationRequest(ModMessageReceivedEventArgs args)
        {
            if (args.Type != ResourceModification)
                return;

            ResourceModificationRequest request = args.ReadAs<ResourceModificationRequest>();

            if (!Context.IsMainPlayer)
            {
                SMonitor.Log($"Handling ResourceModificationRequest as non-main player ({request.ItemId} {request.Change} {request.NewAmount}) | {request.IsFromMainPlayer} {request.HostVersion}");

                // Ignore requests that were sent with with an older host version
                if (!request.IsFromMainPlayer && request.HostVersion < lastUpdateFromHost.Value)
                {
                    return;
                }

                // Update the latest host version
                if (request.IsFromMainPlayer)
                {
                    if (lastUpdateFromHost.Value > request.HostVersion)
                    {
                        SMonitor.Log("We recieved host updates out of order! Re-requesting the dictionary.");
                        RequestDictionaryFromHost();
                    }

                    lastUpdateFromHost.Value = request.HostVersion;
                }

                long currentAmount = GetResourceAmount(request.ItemId);
                long newAmount = Math.Max(currentAmount + request.Change, 0);

                // If the request is from the host and our calculated new amount doesn't align with what they say the amount should be, use their amount instead.
                if(request.IsFromMainPlayer && newAmount != request.NewAmount)
                {
                    newAmount = request.NewAmount;
                }

                BetterCraftingIntegration.NotifyResourceChange(request.ItemId, (int)(newAmount-currentAmount), Game1.player.UniqueMultiplayerID);

                // Update the dictionary
                ChangeResourceAmount(request.ItemId, newAmount);
            }
            else
            {
                SMonitor.Log($"Handling ResourceModificationRequest as main player ({request.ItemId} {request.Change} {request.NewAmount}) | {request.IsFromMainPlayer} {request.HostVersion}");

                // The host can ignore requests from themselves
                if(request.IsFromMainPlayer)
                {
                    // This should actually never be true; the sending player doesn't recieve their own mod messages. If this is hit, that means Context.IsMainPlayer doesn't mean what I think it does.
                    // I'm logging all of the similar values so that if this is hit, I have a good idea what to use instead.
                    SMonitor.Log($"Main player recieved a request from the main player?! {Context.IsMainPlayer} {Context.IsOnHostComputer} {Game1.MasterPlayer} {Game1.IsMasterGame}");
                    return;
                }

                long currentAmount = GetResourceAmount(request.ItemId);
                long newAmount = Math.Max(currentAmount + request.Change, 0);

                BetterCraftingIntegration.NotifyResourceChange(request.ItemId, (int)(newAmount - currentAmount), Game1.player.UniqueMultiplayerID);

                // Send the resource change to the other players
                ReportResourceChange(request.ItemId, newAmount - currentAmount);
            }
        }

        /// <summary>
        /// Handles a dictioanry request mod message.
        /// A dictionary request is a message from a connected instance to the host requesting that the host sends the entire resource dictionary to the instance.
        /// The host will respond with a resource dictionart response.
        /// </summary>
        /// <param name="args"></param>
        static void HandleResourceDictionaryRequest(ModMessageReceivedEventArgs args)
        {
            // Only the host should provide the resource dictionary
            if (!Context.IsMainPlayer)
            {
                return;
            }

            if(args.Type != DictionaryRequest)
            {
                return;
            }

            SMonitor.Log("Sending dictionary");

            // Send a tuple containing the current update version and the resource dictionary to the player who sent the request
            SHelper.Multiplayer.SendMessage((lastUpdateFromHost.Value, sharedResourceDictionary.Value), DictionaryResponse, new string[] {Manifest.UniqueID}, new long[] {args.FromPlayerID});
        }

        /// <summary>
        /// Handles a dictionary response mod message.
        /// A dictionary response is a message from the host to a connected instance which contains the entire resource dictionary and the current dictionary version number.
        /// It is sent in response to a dictionary request.
        /// </summary>
        /// <param name="args"></param>
        static void HandleResourceDictionaryResponse(ModMessageReceivedEventArgs args)
        {
            // The host should always be the supplier of the dictionary; they should not recieve it from any other player
            if (Context.IsMainPlayer)
            {
                return;
            }

            if(args.Type != DictionaryResponse)
            {
                return;
            }

            SMonitor.Log("Recieved dictionary!");

            // Read the contents of the message as an int, dictionary tuple
            // The dictionary is the resource dictionary and the int is the host's update number at the time of sending 
            var data = args.ReadAs<Tuple<int, Dictionary<string, long>>>();

            // Set the last update from the host to the provided version
            lastUpdateFromHost.Value = data.Item1;

            // Set the dictionary to the provided dictionary
            sharedResourceDictionary.Value = data.Item2;
        }

        /// <summary>
        /// Get the amount of the given resource in the shared resource dictionary.
        /// </summary>
        /// <param name="itemId">The qualified item id of the resource to look for.</param>
        /// <returns></returns>
        public static long GetResourceAmount(string itemId)
        {
            return sharedResourceDictionary.Value.TryGetValue(itemId, out long amount) ? amount : 0;
        }

        /// <summary>
        /// Changes the amount of the given resource in the new dictionary to the new amount.
        /// 
        /// If the new value is zero or less, just removes the resource from the dictionary.
        /// </summary>
        /// <param name="itemId">The qualified item id of the resource to change.</param>
        /// <param name="newAmount">The value to change the amount to.</param>
        private static void ChangeResourceAmount(string itemId, long newAmount)
        {
            SMonitor.Log($"Changing amount of {itemId} from {GetResourceAmount(itemId)} to {newAmount}");

            // Tell the resource menu that it needs to redraw the items (only has an effect if the menu is open)
            ResourceMenu.resourceListDirty.Value = true;

            if (newAmount <= 0)
            {
                sharedResourceDictionary.Value.Remove(itemId);
            }
            else
            {
                sharedResourceDictionary.Value[itemId] = newAmount;
            }
        }

        /// <summary>
        /// Gets the shared resource dictionary.
        /// 
        /// This should not be modified directly; use <see cref="ReportResourceChange(string, long)"/> to modify the dictionary.
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, long> GetDictionary()
        {
            return sharedResourceDictionary.Value;
        }

        /// <summary>
        /// Whether the given Farmer should use the shared resource dictionary.
        /// </summary>
        /// <param name="farmer"></param>
        /// <returns></returns>
        public static bool ShouldUseSharedResources(Farmer farmer)
        {
            // If the farmer is this instance's player, uses the value of UseSharedResources (it was loaded from the player's mod data, so it ends up being the same outcome without the need of parsing the data again.
            if(farmer == Game1.player)
            {
                return UseSharedResources.Value;
            }

            // Try to get the value from the farmer's mod data. If there is no mod data, returns false.
            return PerPlayerConfig.LoadConfigOption(farmer, useSharedResourcesKey, defaultValue: false);
        }

        /// <summary>
        /// Change the given resource by the given amount, and report the change to all other players.
        /// </summary>
        /// <param name="itemId">The item id of the resource to change. Does not have to be qualified (this method will qualify it using <see cref="ItemRegistry.QualifyItemId(string)"/>).</param>
        /// <param name="amountChange">The amount to change by.</param>
        public static void ReportResourceChange(string itemId, long amountChange)
        {
            // Make sure the item id is qualified
            itemId = ItemRegistry.QualifyItemId(itemId);

            // If the id can't be qualified, itemId will be null
            if(itemId is null)
            {
                SMonitor.Log("Invalid item id! Not sending a change rerequest.");
                return;
            }

            SMonitor.Log($"Sending recource change request for {itemId}: {amountChange}");

            // If this is the host, increments the number of updates sent
            if(Context.IsMainPlayer)
            {
                lastUpdateFromHost.Value++;
            }

            // Construct the message
            ResourceModificationRequest message = new ResourceModificationRequest(itemId, amountChange, GetResourceAmount(itemId) + amountChange, lastUpdateFromHost.Value, Context.IsMainPlayer);

            // Send the message to all instances of this mod
            SHelper.Multiplayer.SendMessage(message, ResourceModification, new string[] { Manifest.UniqueID });

            // Update the resource dictionary
            long newAmount = GetResourceAmount(itemId) + amountChange;
            ChangeResourceAmount(itemId, newAmount);
        }

        /// <summary>
        /// Method for changing the player's ShouldUseShared in the config.
        /// Changes it to the given value, moves the player's resources to the shared dictionary if necessary, and saves the new config to the player's mod data.
        /// </summary>
        /// <param name="newValue"></param>
        public static void ChangeShouldUseShared(bool newValue)
        {
            if (newValue)
            {
                // Move the player's resources from their individual storage to the shared storage.
                MoveIndividualResourcesToSharedDictionary();
            }

            PerPlayerConfig.SaveConfigOption(Game1.player, useSharedResourcesKey, newValue);
            UseSharedResources.Value = newValue;
        }

        /// <summary>
        /// Move all of the resources in the player's personal storage to the shared storage.
        /// </summary>
        public static void MoveIndividualResourcesToSharedDictionary()
        {
            Dictionary<string, long> currentDictionary = ModEntry.GetFarmerResources(Game1.player, allowShared: false);

            foreach(var kvp in currentDictionary)
            {
                ReportResourceChange(kvp.Key, kvp.Value);
            }

            currentDictionary.Clear();
            ModEntry.SaveResourceDictionary(Game1.player);
        }

        /// <summary>
        /// Send a dictionary request to the host instance.
        /// Really, sends the request to all instances, but only the host will respond.
        /// </summary>
        private static void RequestDictionaryFromHost()
        {
            SHelper.Multiplayer.SendMessage("", DictionaryRequest, new string[] { Manifest.UniqueID });
        }
    }

    /// <summary>
    /// Represents a modification to the shared resource dictionary. Used to communicate between game instances.
    /// </summary>
    struct ResourceModificationRequest
    {
        /// <summary> Whether this request is coming from the host instance. </summary>
        public bool IsFromMainPlayer;

        /// <summary> The number of the latest update the sending instance recieved from the host (or the numbe of this update, if the sending instance is the host). </summary>
        public int HostVersion;

        /// <summary> The qualified id of the item to modify the amount of. </summary>
        public string ItemId;

        /// <summary> The amount to chnage the item by. This is used over NewAmount if the sender is not the host. </summary>
        public long Change;

        /// <summary> The amount of the resource after the change. This is trusted over Change if the sender is the host. </summary>
        public long NewAmount;

        public ResourceModificationRequest()
        {
            ItemId = "";
            Change = 0;
            NewAmount = 0;
            HostVersion = 0;
            IsFromMainPlayer = false;
        }

        public ResourceModificationRequest(string itemId, long change, long newAmount, int hostVersion, bool isFromMainPlayer = false)
        {
            ItemId = itemId;
            Change = change;
            NewAmount = newAmount;
            IsFromMainPlayer = isFromMainPlayer;
            HostVersion = hostVersion;
        }
    }
}
