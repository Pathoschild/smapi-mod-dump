
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omegasis.HappyBirthday.Framework
{
    public class MultiplayerSupport
    {
        /*
        public static string FSTRING_SendBirthdayMessageToOthers = "Omegasis.HappyBirthday.Framework.Messages.SendBirthdayMessageToOtherPlayers";

        public static void initializeMultiplayerSupport()
        {
            ModdedUtilitiesNetworking.ModCore.possibleVoidFunctions.Add(FSTRING_SendBirthdayMessageToOthers,new voidFunc(ShowStarMessage));
        }


        public static void ShowStarMessage(object obj)
        {
            //IEnumerable<Farmer> players= Game1.getAllFarmers();

            DataInfo info = (DataInfo)obj;
            HUDMessage message = (HUDMessage)info.data;
            Game1.addHUDMessage(message);



            if (!message.message.Contains("Inventory"))
            {
                Game1.playSound("cancel");
                return;
            }
            if (!Game1.player.mailReceived.Contains("BackpackTip"))
            {
                Game1.player.mailReceived.Add("BackpackTip");
                Game1.addMailForTomorrow("pierreBackpack", false, false);
            }
        }

        public static void SendBirthdayMessageToOtherPlayers()
        {
            HUDMessage message = new HUDMessage("It's " + Game1.player.Name + "'s birthday! Happy birthday to them!", 1);

            List<Farmer> farmers =ModdedUtilitiesNetworking.Framework.CustomMultiplayer.getAllFarmersExceptThisOne();
            foreach (Farmer f in farmers)
            {
                ModdedUtilitiesNetworking.ModCore.multiplayer.sendMessage(FSTRING_SendBirthdayMessageToOthers, ModdedUtilitiesNetworking.Framework.Extentions.StrardewValleyExtentions.MessagesExtentions.HUDMessageIconIdentifier, message, ModdedUtilitiesNetworking.Framework.Enums.MessageTypes.messageTypes.SendToSpecific, f);
            }
        }
        */
    }
}
