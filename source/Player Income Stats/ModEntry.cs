/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ha1fdaew/PlayerIncomeStats
**
*************************************************/

using PlayerIncomeStats.Client.UI;
using PlayerIncomeStats.Core;
using PlayerIncomeStats.Harmony;
using StardewModdingAPI;
using StardewValley;

namespace PlayerIncomeStats
{
    public class ModEntry : Mod
    {
        public static ModEntry instance;
        public NetworkManager networkManager;
        public UIManager userInterfaceManager;
        public bool flag;
        private int lastShippedItemPrice;

        public ModEntry() => instance = this;

        public static void LogDebug(string msg) => instance.Monitor.Log(msg, LogLevel.Debug);

        public override void Entry(IModHelper helper)
        {
            HPatch.Patch();
            userInterfaceManager = new UIManager();
            networkManager = new NetworkManager();
            SubscribeEvents(helper);
        }

        public void OnItemShipped(Item i, Farmer who)
        {
            lastShippedItemPrice = Utility.getSellToStorePriceOfItem(i);
            networkManager.OnItemShipped(lastShippedItemPrice, who);
            flag = true;
        }

        public void SubscribeEvents(IModHelper helper)
        {
            helper.Events.Multiplayer.ModMessageReceived += networkManager.OnModMessageReceived;
            helper.Events.Multiplayer.PeerConnected += networkManager.OnPeerConnected;
            helper.Events.GameLoop.SaveLoaded += networkManager.OnSaveLoaded;
            helper.Events.GameLoop.Saving += networkManager.OnSaving;
            helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;
        }

        private void OnUpdateTicked(object sender, StardewModdingAPI.Events.UpdateTickedEventArgs e)
        {
            if (!Context.IsWorldReady)
                return;

            if (Game1.getFarm().lastItemShipped == null && flag)
            {
                networkManager.OnItemShipped(lastShippedItemPrice, Game1.player, false);
                flag = false;
            }
        }
    }
}