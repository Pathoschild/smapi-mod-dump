/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MindMeltMax/Stardew-Valley-Mods
**
*************************************************/

global using static Trading.Utilities.Messages;
global using Object = StardewValley.Object;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using System.Linq;
using Trading.Utilities;

namespace Trading
{
    internal class ModEntry : Mod
    {
        public static IModHelper IHelper;
        public static IMonitor IMonitor;
        public static TranslationHelper ITranslations;
        public static Config IConfig;

        private Response[] tradeResponses;

        internal static string[] ModId => new[] { IHelper.ModRegistry.ModID };

        public override void Entry(IModHelper helper)
        {
            Helper.Events.Input.ButtonPressed += onButtonDown;
            Helper.Events.Multiplayer.ModMessageReceived += onMultiplayerMessageReceived;
            Helper.Events.GameLoop.SaveLoaded += onSaveLoaded;
            Helper.Events.Display.WindowResized += onWindowSizeChanged;

            ITranslations = new TranslationHelper(helper);
            IHelper = Helper;
            IMonitor = Monitor;
            IConfig = Helper.ReadConfig<Config>();
        }

        private void onWindowSizeChanged(object? sender, WindowResizedEventArgs e)
        {
            if (Game1.activeClickableMenu is TradeMenu tm)
                Game1.activeClickableMenu = new TradeMenu(tm.Sender, tm.Receiver, tm.Pending, tm.ReceiverItems, tm.ReceiverGold, tm.SenderItems, tm.SenderGold);
            if (Game1.activeClickableMenu is PlayerSelectMenu psm)
                Game1.activeClickableMenu = new PlayerSelectMenu(psm.Sender, psm.NearbyFarmers);
        }

        private void onSaveLoaded(object? sender, SaveLoadedEventArgs e)
        {
            tradeResponses = new[]
            {
                new Response("Accept", ITranslations.TradeAccept),
                new Response("Decline", ITranslations.TradeDecline)
            };
        }

        private void onButtonDown(object? sender, ButtonPressedEventArgs e)
        {
            if (!Context.CanPlayerMove || !Context.IsMultiplayer || Game1.getOnlineFarmers().Count <= 1 || !IConfig.TradeMenuButton.IsDown()) 
                return;
            Game1.activeClickableMenu = new PlayerSelectMenu(Game1.player);
            Helper.Multiplayer.SendMessage("", MSG_PollStatus, ModId, (Game1.activeClickableMenu as PlayerSelectMenu)!.NearbyFarmers.Select(x => x.Key.UniqueMultiplayerID).ToArray());
        }

        private void onMultiplayerMessageReceived(object? sender, ModMessageReceivedEventArgs e)
        {
            if (e.FromModID != Helper.ModRegistry.ModID) 
                return;

            switch (e.Type)
            {
                case MSG_RequestTrade:
                    if (Game1.activeClickableMenu is not null || Game1.eventUp)
                    {
                        Helper.Multiplayer.SendMessage(MSG_Busy, MSG_RespondStatus, ModId, new[] { e.FromPlayerID });
                        return;
                    }
                    Game1.currentLocation.createQuestionDialogue(string.Format(ITranslations.TradeRequest, Game1.getFarmer(e.ReadAs<NetworkPlayer>().SenderId).Name), tradeResponses, (f, k) => 
                    {
                        if (k == "Accept")
                        {
                            Helper.Multiplayer.SendMessage((NetworkPlayer)Game1.player, MSG_TradeRequestAccept, ModId, new[] { e.ReadAs<NetworkPlayer>().SenderId });
                            Game1.activeClickableMenu = new TradeMenu(Game1.player, Game1.getFarmer(e.ReadAs<NetworkPlayer>().SenderId), false);
                        }
                        else
                            Helper.Multiplayer.SendMessage((NetworkPlayer)Game1.player, MSG_TradeRequestDecline, ModId, new[] { e.ReadAs<NetworkPlayer>().SenderId });
                    });
                    break;
                case MSG_TradeRequestAccept:
                    Game1.activeClickableMenu = new TradeMenu(Game1.player, Game1.getFarmer(e.ReadAs<NetworkPlayer>().SenderId), false);
                    break;
                case MSG_TradeRequestDecline:
                    Game1.activeClickableMenu.exitThisMenu();
                    Game1.drawDialogueBox(string.Format(ITranslations.TradeDeclined, Game1.getFarmer(e.ReadAs<NetworkPlayer>().SenderId).Name));
                    break;
                case MSG_UpdateTradeInventory:
                    if (Game1.activeClickableMenu is TradeMenu tm1)
                    {
                        var msg = e.ReadAs<NetworkInventory>();
                        tm1.ReceiverItems = Utilites.ParseItems(msg.Inventory);
                        tm1.ReceiverGold = msg.Gold;
                    }
                    break;
                case MSG_SendTradeOffer:
                    if (Game1.activeClickableMenu is TradeMenu tm2)
                    {
                        if (tm2.SentOffer || tm2.ReceivedOffer)
                            tm2.AcceptedOffer = true;
                        else
                            tm2.ReceivedOffer = true;
                    }
                    break;
                case MSG_DeclineOffer:
                    if (Game1.activeClickableMenu is TradeMenu tm3)
                    {
                        tm3.SentOffer = false;
                        tm3.ReceivedOffer = false;
                        tm3.AcceptedOffer = false;
                        tm3.ConfirmedOffer = false;
                    }
                    break;
                case MSG_ConfirmTrade:
                    if (Game1.activeClickableMenu is TradeMenu tm4)
                    {
                        if (!tm4.AcceptedOffer) break;
                        if (!tm4.ConfirmedOffer)
                        {
                            tm4.ReceivedConfirmation = true;
                            break;
                        }
                        tm4.SenderItems.Clear();
                        for (int i = 0; i < tm4.ReceiverItems.Count; i++)
                            if (!tm4.Sender.addItemToInventoryBool(tm4.ReceiverItems[i]))
                                Game1.createItemDebris(tm4.ReceiverItems[i], tm4.Sender.getStandingPosition(), tm4.Sender.FacingDirection, tm4.Sender.currentLocation);
                        tm4.Sender._money -= (int)tm4.SenderGold;
                        tm4.Sender._money += (int)tm4.ReceiverGold;
                        tm4.exit();
                        return;
                    }
                    break;
                case MSG_ExitTrade:
                    if (Game1.activeClickableMenu is TradeMenu tm5)
                        tm5.exit();
                    Game1.activeClickableMenu?.exitThisMenu();
                    break;
                case MSG_PollStatus:
                    if (Game1.activeClickableMenu is not null || Game1.eventUp)
                        Helper.Multiplayer.SendMessage(MSG_Busy, MSG_RespondStatus, ModId, new[] { e.FromPlayerID });
                    else
                        Helper.Multiplayer.SendMessage(MSG_Available, MSG_RespondStatus, ModId, new[] { e.FromPlayerID });
                    break;
                case MSG_RespondStatus:
                    if (Game1.activeClickableMenu is PlayerSelectMenu psm)
                        psm.NearbyFarmers[Game1.getFarmer(e.FromPlayerID)] = e.ReadAs<string>() == MSG_Available;
                    break;
            }
        }
    }
}
