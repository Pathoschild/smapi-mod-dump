/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MindMeltMax/Stardew-Valley-Mods
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SObject = StardewValley.Object;

namespace Trading.Utilities
{
    internal static class Utilites
    {
        public const string MSG_RequestTrade = "Trading.RequestTrade";
        public const string MSG_TradeRequestAccept = "Trading.AcceptRequest";
        public const string MSG_TradeRequestDecline = "Trading.DeclineRequest";
        public const string MSG_UpdateTradeInventory = "Trading.UpdateView";
        public const string MSG_SendTradeOffer = "Trading.SendOffer";
        public const string MSG_DeclineOffer = "Trading.DeclineOffer";
        public const string MSG_ConfirmTrade = "Trading.ConfirmTrade";
        public const string MSG_ExitTrade = "Trading.Exited";

        public static bool InRadiusOff(Vector2 pointA, Vector2 pointB, int radius = 1)
        {
            return pointA.X >= pointB.X - radius && 
                   pointA.X <= pointB.X + radius && 
                   pointA.Y >= pointB.Y - radius && 
                   pointA.Y <= pointB.Y + radius;
        }

        /// <summary>
        /// https://stackoverflow.com/a/19308859
        /// </summary>
        public static string WrapText(SpriteFont font, string text, float maxWidth)
        {
            string[] words = text.Split(' ');
            float spaceWidth = font.MeasureString(" ").X;
            float remainingSpace = maxWidth;
            float wordWidth;

            StringBuilder sb = new StringBuilder();

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

        public static List<TemporaryNetworkObject> ParseItems(List<SObject> items)
        {
            List<TemporaryNetworkObject> res = new List<TemporaryNetworkObject>();
            foreach (var item in items)
                res.Add((TemporaryNetworkObject)item);
            return res;
        }

        public static List<SObject> ParseItems(List<TemporaryNetworkObject> items)
        {
            List<SObject> res = new List<SObject>();
            foreach (var item in items)
                res.Add((SObject)item);
            return res;
        }
    }
}
