/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/jamespfluger/Stardew-EqualMoneySplit
**
*************************************************/

using EqualMoneySplit.MoneyNetwork;
using EqualMoneySplit.Networking;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Linq;

namespace EqualMoneySplit.Events
{
    public class EventSubscriber

    {
        /// <summary>
        /// Global instance of EventSubscriber
        /// </summary>
        public static EventSubscriber Instance { get { return lazyEventSubscriber.Value; } }

        /// <summary>
        /// Local instance of the EventSubscriber class loaded lazily
        /// </summary>
        private static readonly Lazy<EventSubscriber> lazyEventSubscriber = new Lazy<EventSubscriber>(() => new EventSubscriber());

        // Initialize all event handler classes
        private readonly MultiplayerEventHandlers multiplayerEventHandlers;
        private readonly InventoryEventHandlers inventoryChangedHandler;
        private readonly GameLoopEventHandlers gameLoopHandler;
        private readonly SaveEventHandlers saveEventHandler;

        // Whether or not the subscriptions have been added
        private bool hasAddedSubscriptions = false;

        /// <summary>
        /// Handles the subscriptions and unsubscriptions of events after criteria are checked
        /// </summary>
        public EventSubscriber()
        {
            multiplayerEventHandlers = new MultiplayerEventHandlers();
            inventoryChangedHandler = new InventoryEventHandlers();
            gameLoopHandler  = new GameLoopEventHandlers();
            saveEventHandler = new SaveEventHandlers();
        }

        /// <summary>
        /// Subscribe to the events that we always need to be listening for to set the other event subscriptions
        /// </summary>
        public void AddRequiredSubscriptions()
        {
            // Instantiate all events needed
            EqualMoneyMod.SMAPI.Events.GameLoop.DayStarted += gameLoopHandler.OnDayStartedHandler;
            EqualMoneyMod.SMAPI.Events.Multiplayer.PeerConnected += multiplayerEventHandlers.OnPeerConnected;
        }

        /// <summary>
        /// Tries to sets the valid event subscriptions based on the current state of the game
        /// </summary>
        /// <returns>True if SMAPI contains the valid subscriptions, otherwise false</returns>
        public bool TrySetEventSubscriptions()
        {
            if (!CheckForValidConditions())
            {
                // If the conditions for the mod to execute are not valid, remove the subscriptions
                RemoveSubscriptions();

                // Further, if the specific condition is the players not using separate wallets, tell them the mod won't work
                if (Context.IsMultiplayer && EqualMoneyMod.SMAPI.Multiplayer.GetConnectedPlayers().Any() && !Game1.player.useSeparateWallets)
                {
                    EqualMoneyMod.Logger.Log("EqualMoneySplit cannot be run unless individual wallets are set up! You must either disable the mod or set up individual wallets!", StardewModdingAPI.LogLevel.Warn);
                    Game1.chatBox.addErrorMessage("ERROR: EqualMoneySplit cannot be run unless individual wallets are set up!");
                }

                return false;
            }
            else if (!hasAddedSubscriptions)
            {
                // This adds any subscriptions that haven't already been added
                AddSubscriptions();
            }

            return true;
        }

        /// <summary>
        /// Subscribe to the events that implement money split functionality
        /// </summary>
        private void AddSubscriptions()
        {
            // Start any mod message listeners we need
            StartNetworkListeners();

            // Instantiate events only required when mod is enabled
            EqualMoneyMod.SMAPI.Events.Multiplayer.PeerDisconnected += multiplayerEventHandlers.OnPeerDisconnected;
            EqualMoneyMod.SMAPI.Events.Player.InventoryChanged += inventoryChangedHandler.OnInventoryChanged;
            EqualMoneyMod.SMAPI.Events.GameLoop.UpdateTicking += gameLoopHandler.OnUpdateTicking;
            EqualMoneyMod.SMAPI.Events.GameLoop.DayEnding += gameLoopHandler.OnDayEndingHandler;
            EqualMoneyMod.SMAPI.Events.GameLoop.Saving += saveEventHandler.OnSavingHandler;

            EqualMoneyMod.SMAPI.Events.Multiplayer.ModMessageReceived += Network.Instance.OnModMessageReceived;

            hasAddedSubscriptions = true;
            EqualMoneyMod.Logger.Log("Events have been subscribed to");
        }

        /// <summary>
        /// Removes the event listeners from the core SMAPI API
        /// </summary>
        private void RemoveSubscriptions()
        {
            // Remove events that shouldn't be triggering
            EqualMoneyMod.SMAPI.Events.Multiplayer.PeerDisconnected -= multiplayerEventHandlers.OnPeerDisconnected;
            EqualMoneyMod.SMAPI.Events.Player.InventoryChanged -= inventoryChangedHandler.OnInventoryChanged;
            EqualMoneyMod.SMAPI.Events.GameLoop.UpdateTicking -= gameLoopHandler.OnUpdateTicking;
            EqualMoneyMod.SMAPI.Events.GameLoop.DayEnding -= gameLoopHandler.OnDayEndingHandler;
            EqualMoneyMod.SMAPI.Events.GameLoop.Saving -= saveEventHandler.OnSavingHandler;

            EqualMoneyMod.SMAPI.Events.Multiplayer.ModMessageReceived -= Network.Instance.OnModMessageReceived;

            // Stop any mod message listeners we started
            StopNetworkListeners();

            hasAddedSubscriptions = false;
            EqualMoneyMod.Logger.Log("Events have been unsubscribed from");
        }

        /// <summary>
        /// Determines whether or not the mod can be enabled.
        /// </summary>
        /// <returns>True if the mod can be enabled; otherwise false</returns>
        private bool CheckForValidConditions()
        {
            if (!Context.IsMultiplayer)
                return false;
            else if (!EqualMoneyMod.SMAPI.Multiplayer.GetConnectedPlayers().Any())
                return false;
            else if (!Game1.player.useSeparateWallets)
                return false;
            else
                return true;
        }

        /// <summary>
        /// Start any network listeners needed
        /// </summary>
        private void StartNetworkListeners()
        {
            MoneyListener.Instance.Start();
        }

        /// <summary>
        /// Stop the listeners we started earlier
        /// </summary>
        private void StopNetworkListeners()
        {
            MoneyListener.Instance.Stop();
        }
    }
}
