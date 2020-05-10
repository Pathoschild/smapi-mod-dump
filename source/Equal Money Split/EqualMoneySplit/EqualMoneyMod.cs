using EqualMoneySplit.Events;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace EqualMoneySplit
{
    /// <summary>
    /// Mod used to evenly split money earned from selling items between farmers
    /// </summary>
    public class EqualMoneyMod : Mod
    {
        /// <summary>
        /// The SMAPI API used for monitoring and logging
        /// </summary>
        public static IMonitor Logger { get; private set; }
        /// <summary>
        /// The SMAPI API used to integrate mods with the base Stardew Valley game
        /// </summary>
        public static IModHelper SMAPI { get; private set; }
        
        /// <summary>
        /// Checks if this is the first day the user is connecting for
        /// </summary>
        private bool isFirstDay = true;

        /// <summary>
        /// Entry point of EqualMoneyMod
        /// </summary>
        /// <param name="helper">SMAPI provided API for writing mods</param>
        public override void Entry(IModHelper helper)
        {
            Logger = base.Monitor;
            SMAPI = base.Helper;

            SMAPI.Events.GameLoop.DayStarted += FirstDayEventSubscriptions;
        }

        /// <summary>
        /// Subscribes to the events on the first day of any game session
        /// </summary>
        private void FirstDayEventSubscriptions(object sender, DayStartedEventArgs args)
        {
            if (!Context.IsMultiplayer)
            {
                Logger.Log("Multiplayer is not being used, but the mod is enabled.");
            }
            else if (!Game1.player.useSeparateWallets)
            {
                Logger.Log("WARNING: EqualMoneySplit cannot be run unless individual wallets are set up! You must either disable the mod or set up individual wallets!");
                Game1.chatBox.addErrorMessage("WARNING: EqualMoneySplit cannot be run unless individual wallets are set up!");
                EventSubscriber.Instance.RemoveSubscriptions();
            }
            else if (isFirstDay)
            {
                EventSubscriber.Instance.AddSubscriptions();

                // Start subscribing to the event of returning to the title
                SMAPI.Events.GameLoop.ReturnedToTitle += ReturnToTitleEventUnsubcriptions;
                SMAPI.Events.GameLoop.DayStarted -= FirstDayEventSubscriptions;

                // After our first day has started, it is no longer the start of the first day
                isFirstDay = false;
            }
        }

        /// <summary>
        /// Removes event subscriptions when the player returns to the title
        /// </summary>
        private void ReturnToTitleEventUnsubcriptions(object sender, ReturnedToTitleEventArgs args)
        {
            EventSubscriber.Instance.RemoveSubscriptions();

            // Re-add the first day event subscriptions
            SMAPI.Events.GameLoop.DayStarted += FirstDayEventSubscriptions;
            SMAPI.Events.GameLoop.ReturnedToTitle -= ReturnToTitleEventUnsubcriptions;

            // If we exit to the menu, then we will need a new first day setup
            isFirstDay = true;
        }
    }
}
