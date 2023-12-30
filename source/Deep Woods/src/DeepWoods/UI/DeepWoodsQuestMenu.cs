/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/maxvollmer/DeepWoodsMod
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using static DeepWoodsMod.I18N;

namespace DeepWoodsMod.UI
{
    public class DeepWoodsQuestMenu : DialogueBox
    {
        /*
        < heart
        = purple star
        > right arrow
        @ left arrow
        ` up arrow
        $ coin
        + yellow face on skateboard??"
        */

        private static readonly string ModInfoText =
            "Hi, I am Max! < I make DeepWoods.^" +
            "^" +
            "If you love this mod, I invite you to join me on Discord and Youtube, where I post updates and engage with my community.^" +
            "^" +
            "Thank you! < ~Max";

        private static void ShowModInfo()
        {
            MakeTheThing(ModInfoText, new Response[3]
            {
                new Response("Youtube", "> youtube.com/MaxMakesMods"),
                new Response("Discord", "> discord.gg/jujwEGf62K"),
                new Response("No", I18N.MessageBoxClose).SetHotKey(Keys.Escape)
            }, ModInfoResponse);
        }



        public static void OpenQuestMenu(string text, ICollection<Response> responses, GameLocation.afterQuestionBehavior responseHandler = null)
        {
            MakeTheThing(text, responses, responseHandler ?? ModInfoResponse);
        }

        public static void OpenQuestMenuWithModInfo(string text, ICollection<Response> responses, GameLocation.afterQuestionBehavior responseHandler = null)
        {
            List<Response> responsesWithModInfo = new();
            responsesWithModInfo.AddRange(responses);
            responsesWithModInfo.Add(new Response("ModInfo", "Mod Info").SetHotKey(Keys.I));
            MakeTheThing(text, responsesWithModInfo, responseHandler ?? ModInfoResponse);
        }

        private static void MakeTheThing(string text, ICollection<Response> responses, GameLocation.afterQuestionBehavior responseHandler)
        {
            if (Game1.activeClickableMenu != null)
            {
                Game1.activeClickableMenu.emergencyShutDown();
            }

            Game1.activeClickableMenu = new DeepWoodsQuestMenu(text, responses.ToList(), responseHandler);
            Game1.player.CanMove = false;
            Game1.dialogueUp = true;

            Game1.currentLocation.afterQuestion = responseHandler;
        }

        private static void ModInfoResponse(Farmer who, string answer)
        {
            switch (answer)
            {
                case "No":
                    return;

                case "SearchThroughStuff":
                    searchThroughStuff();
                    return;

                case "Youtube":
                    // Links to my Youtube video "The future of DeepWoods"
                    OpenURL("https://www.youtube.com/watch?v=XxUz0jUaFOQ");
                    DeepWoodsQuestMenu.ShowModInfo();
                    return;

                case "Discord":
                    // Invite link for my Discord "Max Makes Mods"
                    OpenURL("https://discord.gg/jujwEGf62K");
                    DeepWoodsQuestMenu.ShowModInfo();
                    return;

                case "ModInfo":
                    DeepWoodsQuestMenu.ShowModInfo();
                    return;
            }
        }

        private static void searchThroughStuff()
        {
            // TODO: Random message + low chance to get one item per day
            Game1.showRedMessage(I18N.StuffNothing);
        }

        private static void OpenURL(string url)
        {
            try
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    try
                    {
                        Process.Start(url);
                    }
                    catch
                    {
                        Process.Start(new ProcessStartInfo("cmd", $"/c start {url}"));
                    }
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    Process.Start("xdg-open", url);
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    Process.Start("open", url);
                }
            }
            catch { }
        }

        GameLocation.afterQuestionBehavior responseHandler = null;

        private DeepWoodsQuestMenu(string text, List<Response> responses, GameLocation.afterQuestionBehavior responseHandler)
            : base(text, responses)
        {
            this.responseHandler = responseHandler;
        }

        public override void update(GameTime time)
        {
            if (Game1.currentLocation.afterQuestion != responseHandler)
            {
                Game1.currentLocation.afterQuestion = responseHandler;
            }

            base.update(time);
        }
    }
}
