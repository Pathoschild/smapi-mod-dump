/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/janavarro95/Stardew_Valley_Mods
**
*************************************************/

using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModdedUtilitiesNetworking.Framework.Features.Stardew
{
    public class MessageFeatures
    {

        public static string FSTRING_SendHUDMessageWithIcon = "Omegasis.ModdedUtilitiesNetworking.Framework.Features.Stardew.MessageFeatures.SendHUDMessageWithIcon";

        public static string FSTRING_DisplayConsoleMessageString = "Omegasis.ModdedUtilitiesNetworking.Framework.Features.Stardew.MessageFeatures.DisplayConsoleMessage";


        /// <summary>
        /// Displays a HUD message to all clients and server containing this information.
        /// </summary>
        /// <param name="data"></param>
        public static void SendHUDMessageWithIcon(object data)
        {
            DataInfo obj = (DataInfo)data;
            string type = obj.type;
            HUDMessage message =(HUDMessage)obj.data;
            Game1.addHUDMessage(message);
        }


        /// <summary>
        /// Static Debug function.
        /// </summary>
        /// <param name="param"></param>
        public static void DisplayConsoleMessage(object param)
        {
            DataInfo dataInfo = (DataInfo)param;
            string s = (string)dataInfo.data;
            ModCore.monitor.Log(s);
        }
    }
}
