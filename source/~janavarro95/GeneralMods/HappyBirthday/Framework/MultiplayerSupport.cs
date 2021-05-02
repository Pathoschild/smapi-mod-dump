/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/janavarro95/Stardew_Valley_Mods
**
*************************************************/

using System.Collections.Generic;
using StardewValley;

namespace Omegasis.HappyBirthday.Framework
{
    public class MultiplayerSupport
    {
        public static string FSTRING_SendBirthdayMessageToOthers = "Omegasis.HappyBirthday.Framework.Messages.SendBirthdayMessageToOtherPlayers";
        public static string FSTRING_SendBirthdayInfoToOthers = "Omegasis.HappyBirthday.Framework.Messages.SendBirthdayInfoToOtherPlayers";
        public static string FSTRING_SendFarmhandBirthdayInfoToPlayer = "Omegasis.HappyBirthday.Framework.Messages.SendFarmhandBirthdayInfoToPlayer";
        public static string FSTRING_RequestBirthdayInfoFromServer = "Omegasis.HappyBirthday.Framework.Messages.RequestBirthdayInfoFromServer";

        public static void SendBirthdayMessageToOtherPlayers()
        {
            string str = BirthdayMessages.GetTranslatedString("Happy Birthday: Farmhand Birthday Message");
            str.Replace("@", Game1.player.Name);
            HUDMessage message = new HUDMessage(str, 1);

            

            foreach (KeyValuePair<long, Farmer> f in Game1.otherFarmers)
            {
                HappyBirthday.ModHelper.Multiplayer.SendMessage<string>(message.message, FSTRING_SendBirthdayMessageToOthers, new string[] { HappyBirthday.ModHelper.Multiplayer.ModID }, new long[] { f.Key });
            }
        }

        public static void SendBirthdayInfoToOtherPlayers()
        {
            foreach (KeyValuePair<long, Farmer> f in Game1.otherFarmers)
            {
                HappyBirthday.ModHelper.Multiplayer.SendMessage<KeyValuePair<long, PlayerData>>(new KeyValuePair<long, PlayerData>(Game1.player.UniqueMultiplayerID, HappyBirthday.PlayerBirthdayData), FSTRING_SendBirthdayInfoToOthers, new string[] { HappyBirthday.ModHelper.Multiplayer.ModID }, new long[] { f.Key });
            }
        }

        public static void SendBirthdayInfoToConnectingPlayer(long id)
        {
            HappyBirthday.ModHelper.Multiplayer.SendMessage<KeyValuePair<long, PlayerData>>(new KeyValuePair<long, PlayerData>(Game1.player.UniqueMultiplayerID, HappyBirthday.PlayerBirthdayData), FSTRING_SendBirthdayInfoToOthers, new string[] { HappyBirthday.ModHelper.Multiplayer.ModID }, new long[] { id });
        }

        public static void SendFarmandBirthdayInfoToPlayer(long id,PlayerData FarmhandBirthday)
        {
            HappyBirthday.ModHelper.Multiplayer.SendMessage<KeyValuePair<long, PlayerData>>(new KeyValuePair<long, PlayerData>(id, FarmhandBirthday), FSTRING_SendFarmhandBirthdayInfoToPlayer, new string[] { HappyBirthday.ModHelper.Multiplayer.ModID }, new long[] {id});
        }
        /// <summary>
        /// Requests the info from the server.
        /// </summary>
        /// <param name="id">The id of the connecting farmhand.</param>
        public static void RequestFarmandBirthdayInfoFromServer()
        {
            HappyBirthday.ModHelper.Multiplayer.SendMessage<KeyValuePair<long, string>>(new KeyValuePair<long, string>(Game1.player.UniqueMultiplayerID, ""), FSTRING_RequestBirthdayInfoFromServer, new string[] { HappyBirthday.ModHelper.Multiplayer.ModID }, new long[] { });
        }
    }
}
