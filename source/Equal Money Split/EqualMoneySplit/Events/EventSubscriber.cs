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
using System;

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
        /// Subscribe to the events that we will always be listening to
        /// </summary>
        public void AddSubscriptions()
        {
            // Start any mod message listeners we need
            StartNetworkListeners();

            // Instantiate all events needed
            EqualMoneyMod.SMAPI.Events.Multiplayer.PeerContextReceived += multiplayerEventHandlers.OnPeerContextReceived;
            EqualMoneyMod.SMAPI.Events.Player.InventoryChanged += inventoryChangedHandler.OnInventoryChanged;
            EqualMoneyMod.SMAPI.Events.GameLoop.UpdateTicking += gameLoopHandler.OnUpdateTicking;
            EqualMoneyMod.SMAPI.Events.GameLoop.DayStarted += gameLoopHandler.OnDayStartedHandler;
            EqualMoneyMod.SMAPI.Events.GameLoop.DayEnding += gameLoopHandler.OnDayEndingHandler;
            EqualMoneyMod.SMAPI.Events.GameLoop.Saving += saveEventHandler.OnSavingHandler;
            EqualMoneyMod.SMAPI.Events.GameLoop.Saved += saveEventHandler.OnSavedHandler;

            EqualMoneyMod.SMAPI.Events.Multiplayer.ModMessageReceived += Network.Instance.OnModMessageReceived;
        }

        /// <summary>
        /// Removes the event listeners from the core SMAPI API
        /// </summary>
        public void RemoveSubscriptions()
        {
            // Remove events that shouldn't be triggering
            EqualMoneyMod.SMAPI.Events.Multiplayer.PeerContextReceived -= multiplayerEventHandlers.OnPeerContextReceived;
            EqualMoneyMod.SMAPI.Events.Player.InventoryChanged -= inventoryChangedHandler.OnInventoryChanged;
            EqualMoneyMod.SMAPI.Events.GameLoop.UpdateTicking -= gameLoopHandler.OnUpdateTicking;
            EqualMoneyMod.SMAPI.Events.GameLoop.DayStarted -= gameLoopHandler.OnDayStartedHandler;
            EqualMoneyMod.SMAPI.Events.GameLoop.DayEnding -= gameLoopHandler.OnDayEndingHandler;
            EqualMoneyMod.SMAPI.Events.GameLoop.Saving -= saveEventHandler.OnSavingHandler;
            EqualMoneyMod.SMAPI.Events.GameLoop.Saved -= saveEventHandler.OnSavedHandler;

            EqualMoneyMod.SMAPI.Events.Multiplayer.ModMessageReceived -= Network.Instance.OnModMessageReceived;

            // Stop any mod message listeners we started
            StopNetworkListeners();
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
