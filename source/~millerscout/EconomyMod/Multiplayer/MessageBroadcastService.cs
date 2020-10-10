using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EconomyMod.Multiplayer.Messages;
using StardewModdingAPI;
using StardewValley;

namespace EconomyMod.Multiplayer
{
    public class MessageBroadcastService
    {
        private TaxationService taxation;
        private Dictionary<BroadcastType, Action<BroadcastMessage>> broadcastActions;

        public MessageBroadcastService(TaxationService taxation)
        {
            this.taxation = taxation;

            Util.Helper.Events.Multiplayer.ModMessageReceived += Multiplayer_ModMessageReceived;

            this.taxation.OnPayTaxesCompleted += Taxation_OnPayTaxesCompleted;
            this.taxation.OnPostPoneTaxesCompleted += Taxation_OnPostPoneTaxesCompleted;


            this.broadcastActions = new Dictionary<BroadcastType, Action<BroadcastMessage>>() {
                {  BroadcastType.Taxation_Paid, PaidTaxes},
                {  BroadcastType.Taxation_Postpone, PostPone},
            };

        }

        private void Taxation_OnPostPoneTaxesCompleted(object sender, EventHandlerMessage emessage)
        {
            if (Util.IsValidMultiplayer)
            {
                BroadcastMessage message = new BroadcastMessage()
                {
                    Tax = emessage.tax,
                    IsMale = emessage.isMale,
                    DisplayName = Game1.player.displayName
                };
                Util.Helper.Multiplayer.SendMessage(message, $"{BroadcastType.Taxation_Postpone}", modIDs: new[] { Util.ModManifest.UniqueID });
            }
        }

        private void Taxation_OnPayTaxesCompleted(object sender, EventHandlerMessage emessage)
        {
            if (Util.IsValidMultiplayer)
            {
                BroadcastMessage message = new BroadcastMessage()
                {
                    Tax = emessage.tax,
                    IsMale = emessage.isMale,
                    DisplayName = Game1.player.displayName
                };
                Util.Helper.Multiplayer.SendMessage(message, $"{BroadcastType.Taxation_Paid}", modIDs: new[] { Util.ModManifest.UniqueID });
            }
        }

        private void Multiplayer_ModMessageReceived(object sender, StardewModdingAPI.Events.ModMessageReceivedEventArgs e)
        {

            if (Util.IsValidMultiplayer)
            {
                if (e.FromModID == Util.ModManifest.UniqueID)
                {
                    var message = e.ReadAs<Messages.BroadcastMessage>();
                    broadcastActions[message.Type](message);

                }
            }
        }

        private void PaidTaxes(BroadcastMessage message)
        {
            Game1.chatBox.addInfoMessage(message.IsMale
                ? Util.Helper.Translation.Get("playerPaidTaxesMaleText").ToString().Replace("#playerName#", message.DisplayName)
                : Util.Helper.Translation.Get("playerPaidTaxesFemaleText").ToString().Replace("#playerName#", message.DisplayName));
        }
        private void PostPone(BroadcastMessage message)
        {
            Game1.chatBox.addInfoMessage(Util.Helper.Translation.Get("PostponeChatText").ToString().Replace("#playerName#", message.DisplayName).Replace("#Tax#", $"{message.Tax}"));
        }
    }
}
