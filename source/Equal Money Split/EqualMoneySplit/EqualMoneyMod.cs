/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/jamespfluger/Stardew-EqualMoneySplit
**
*************************************************/

using EqualMoneySplit.Events;
using EqualMoneySplit.Models;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
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
        /// Contains information on each farmer's current and past money/inventory changes
        /// </summary>
        public static PerScreen<PersistentFarmerData> FarmerData { get; private set; } = new PerScreen<PersistentFarmerData>();

        /// <summary>
        /// Entry point of EqualMoneyMod
        /// </summary>
        /// <param name="helper">SMAPI provided API for writing mods</param>
        public override void Entry(IModHelper helper)
        {
            Logger = base.Monitor;
            SMAPI = base.Helper;
            FarmerData ??= new();
            FarmerData.Value ??= new();

            EqualMoneyMod.SMAPI.Events.GameLoop.GameLaunched += OnGameLaunched;
        }

        /// <summary>
        /// Performs initial mod registrations
        /// </summary>
        /// <param name="sender">The sender of the DayEndingEvent event</param>
        /// <param name="args">Event arguments for the DayEndingEvent event</param>
        public void OnGameLaunched(object sender, GameLaunchedEventArgs args)
        {
            EventSubscriber.Instance.AddRequiredSubscriptions();
        }
    }
}
