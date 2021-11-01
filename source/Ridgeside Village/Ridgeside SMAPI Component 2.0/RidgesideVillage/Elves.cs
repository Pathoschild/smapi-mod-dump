/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Rafseazz/Ridgeside-Village-Mod
**
*************************************************/

using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley;
using StardewModdingAPI.Events;
using Microsoft.Xna.Framework;
using StardewValley.Menus;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI.Utilities;

namespace RidgesideVillage
{
    internal static class Elves
    {
        static IModHelper Helper;
        static IMonitor Monitor;
        const string STAYHOME = "RSV.UndreyaStayHome";
        internal static void Initialize(IMod ModInstance)
        {
            Helper = ModInstance.Helper;
            Monitor = ModInstance.Monitor;
            TileActionHandler.RegisterTileAction("RSVUndreyaSched", RSVUndreyaSched);
        }

        private static void RSVUndreyaSched(string tileActionString, Vector2 position)
        {
            if (!Game1.player.IsMainPlayer)
            {
                Game1.addHUDMessage(new HUDMessage(Helper.Translation.Get("RSV.UndreyaError"), HUDMessage.error_type));
            }
            else
            {
                if (!Game1.player.mailReceived.Contains(STAYHOME))
                {
                        var responses = new List<Response>
                    {
                        new Response("yes", Helper.Translation.Get("Offer.Yes")),
                        new Response("no", Helper.Translation.Get("Offer.No")),
                    };
                        var responseActions = new List<Action>
                    {
                        delegate
                        {
                            Game1.player.mailReceived.Add(STAYHOME);
                            Game1.activeClickableMenu = new DialogueBox(Helper.Translation.Get("RSV.UndreyaWontPlay"));
                        },
                        delegate{}
                    };
                        Game1.activeClickableMenu = new DialogueBoxWithActions(Helper.Translation.Get("RSV.UndreyaSchedStop"), responses, responseActions);
                }
                else if (Game1.player.mailReceived.Contains(STAYHOME))
                {
                    var responses = new List<Response>
                    {
                        new Response("yes", Helper.Translation.Get("Offer.Yes")),
                        new Response("no", Helper.Translation.Get("Offer.No")),
                    };
                    var responseActions = new List<Action>
                    {
                        delegate
                        {
                            Game1.player.mailReceived.Remove(STAYHOME);
                            Game1.activeClickableMenu = new DialogueBox(Helper.Translation.Get("RSV.UndreyaWillPlay"));
                        },
                        delegate{}
                    };
                    Game1.activeClickableMenu = new DialogueBoxWithActions(Helper.Translation.Get("RSV.UndreyaSchedStart"), responses, responseActions);
                }
            }
        }


    }
}

