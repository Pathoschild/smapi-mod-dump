/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MindMeltMax/Stardew-Valley-Mods
**
*************************************************/

using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using System.Collections.Generic;
using System.Text;

namespace Trading.Utilities
{
    internal static class Messages
    {
        public const string MSG_RequestTrade = "Trading.RequestTrade";
        public const string MSG_TradeRequestAccept = "Trading.AcceptRequest";
        public const string MSG_TradeRequestDecline = "Trading.DeclineRequest";
        public const string MSG_UpdateTradeInventory = "Trading.UpdateView";
        public const string MSG_SendTradeOffer = "Trading.SendOffer";
        public const string MSG_DeclineOffer = "Trading.DeclineOffer";
        public const string MSG_ConfirmTrade = "Trading.ConfirmTrade";
        public const string MSG_ExitTrade = "Trading.Exited";
        public const string MSG_PollStatus = "Trading.RequestStatus";
        public const string MSG_RespondStatus = "Trading.RespondStatus";

        public const string MSG_Available = "Trading.Available";
        public const string MSG_Busy = "Trading.Busy";
    }

    internal static class Utilites
    {
        /// <summary>
        /// https://stackoverflow.com/a/19308859
        /// </summary>
        public static string WrapText(SpriteFont font, string text, float maxWidth)
        {
            string[] words = text.Split(' ');
            float spaceWidth = font.MeasureString(" ").X;
            float remainingSpace = maxWidth;
            float wordWidth;

            StringBuilder sb = new();

            for (int i = 0; i < words.Length; i++) 
            {
                wordWidth = font.MeasureString(words[i]).X;
                if (wordWidth + spaceWidth > remainingSpace)
                {
                    sb.AppendLine();
                    remainingSpace = maxWidth - wordWidth;
                }
                else remainingSpace -= wordWidth + spaceWidth;
                sb.Append(words[i] + " ");
            }

            return sb.ToString();
        }

        public static List<NetworkObject> ParseItems(List<Item> items)
        {
            List<NetworkObject> res = new();
            foreach (var item in items)
                res.Add((NetworkObject)item);
            return res;
        }

        public static List<Item> ParseItems(List<NetworkObject> items)
        {
            List<Item> res = new();
            foreach (var item in items)
                res.Add((Item)item!);
            return res;
        }
    }
}
