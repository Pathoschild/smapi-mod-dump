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
        
        public static string FSTRING_SendBirthdayMessageToOthers = "Omegasis.HappyBirthday.Framework.Messages.SendBirthdayMessageToOtherPlayers";
        public static string FSTRING_SendBirthdayInfoToOthers = "Omegasis.HappyBirthday.Framework.Messages.SendBirthdayInfoToOtherPlayers";


        public static void SendBirthdayMessageToOtherPlayers()
        {
            HUDMessage message = new HUDMessage("It's " + Game1.player.Name + "'s birthday! Happy birthday to them!", 1);

            
            foreach (KeyValuePair<long,Farmer> f in Game1.otherFarmers)
            {
                HappyBirthday.ModHelper.Multiplayer.SendMessage<string>(message.message, FSTRING_SendBirthdayMessageToOthers,new string[] { HappyBirthday.ModHelper.Multiplayer.ModID }, new long[] { f.Key });
                //ModdedUtilitiesNetworking.ModCore.multiplayer.sendMessage(FSTRING_SendBirthdayMessageToOthers, ModdedUtilitiesNetworking.Framework.Extentions.StrardewValleyExtentions.MessagesExtentions.HUDMessageIconIdentifier, message, ModdedUtilitiesNetworking.Framework.Enums.MessageTypes.messageTypes.SendToSpecific, f);
            }
        }

        public static void SendBirthdayInfoToOtherPlayers()
        {
            foreach (KeyValuePair<long, Farmer> f in Game1.otherFarmers)
            {
                HappyBirthday.ModHelper.Multiplayer.SendMessage<KeyValuePair<long,PlayerData>>(new KeyValuePair<long, PlayerData>(Game1.player.uniqueMultiplayerID, HappyBirthday.PlayerBirthdayData), FSTRING_SendBirthdayInfoToOthers, new string[] { HappyBirthday.ModHelper.Multiplayer.ModID }, new long[] { f.Key });
                //ModdedUtilitiesNetworking.ModCore.multiplayer.sendMessage(FSTRING_SendBirthdayMessageToOthers, ModdedUtilitiesNetworking.Framework.Extentions.StrardewValleyExtentions.MessagesExtentions.HUDMessageIconIdentifier, message, ModdedUtilitiesNetworking.Framework.Enums.MessageTypes.messageTypes.SendToSpecific, f);
            }

        }

        public static void SendBirthdayInfoToConnectingPlayer(long id)
        {
            HappyBirthday.ModHelper.Multiplayer.SendMessage<KeyValuePair<long, PlayerData>>(new KeyValuePair<long, PlayerData>(Game1.player.uniqueMultiplayerID, HappyBirthday.PlayerBirthdayData), FSTRING_SendBirthdayInfoToOthers, new string[] { HappyBirthday.ModHelper.Multiplayer.ModID }, new long[] { id });
            //ModdedUtilitiesNetworking.ModCore.multiplayer.sendMessage(FSTRING_SendBirthdayMessageToOthers, ModdedUtilitiesNetworking.Framework.Extentions.StrardewValleyExtentions.MessagesExtentions.HUDMessageIconIdentifier, message, ModdedUtilitiesNetworking.Framework.Enums.MessageTypes.messageTypes.SendToSpecific, f);

        }

    }
}
