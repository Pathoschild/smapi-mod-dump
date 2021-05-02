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
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModdedUtilitiesNetworking.Framework.Extentions.StrardewValleyExtentions
{
    public static class MessagesExtentions
    {

        public static string  HUDMessageIconIdentifier= typeof(HUDMessage).ToString()+".MessageWithIcon";

        public static HUDMessage ReadHUDMessageWithIcon(this BinaryReader reader)
        {
            string message = reader.ReadString();
            int messageType = reader.ReadInt32();

            return new HUDMessage(message,messageType);
        }


        public static void WriteHUDMessageWithIcon(this BinaryWriter writer, object obj)
        {
            HUDMessage message =(HUDMessage) obj;
            writer.WriteString(message.message);
            writer.Write(message.whatType);   
        }



    }
}
