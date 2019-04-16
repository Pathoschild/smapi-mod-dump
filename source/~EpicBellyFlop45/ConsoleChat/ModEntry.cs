using Harmony;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleChat
{
    class ModEntry : Mod
    {
        static IMonitor ModMonitor;

        public override void Entry(IModHelper helper)
        {
            // Create a new Harmony instance for patching source code
            HarmonyInstance harmony = HarmonyInstance.Create(this.ModManifest.UniqueID);

            // Get the method we want to patch
            MethodInfo targetMethod = AccessTools.Method(typeof(ChatBox), nameof(ChatBox.receiveChatMessage));

            // Get the patch that was created
            MethodInfo postfix = AccessTools.Method(typeof(ModEntry), nameof(ModEntry.Postfix));

            // Apply the patch
            harmony.Patch(targetMethod, postfix: new HarmonyMethod(postfix));

            ModMonitor = this.Monitor;

            // Add a console command for the 'say' command to talk from the console
            this.Helper.ConsoleCommands.Add("say", "Writes text to chatbox from console.\n\nUsage: say <message>\n- message: The message to post in chatbox.", Say);
        }

        private static void Postfix(long sourceFarmer, int chatKind, LocalizedContentManager.LanguageCode language, string message)
        {
            string playerName = "";

            // Find the player's nanme from the id
            if (sourceFarmer == Game1.player.UniqueMultiplayerID)
            {
                playerName = ChatBox.formattedUserName(Game1.player);
            }
            else if (Game1.otherFarmers.ContainsKey(sourceFarmer))
            {
                playerName = ChatBox.formattedUserName(Game1.otherFarmers[sourceFarmer]);
            }

            // Print to console the message
            ModMonitor.Log($"{playerName}: {message}", LogLevel.Info);
        }

        private void Say(string command, string[] args)
        {
            //string message = args[0];

            //ChatBox chatBox = new ChatBox();
            //chatBox.receiveChatMessage(Game1.player.UniqueMultiplayerID, 0, LocalizedContentManager.CurrentLanguageCode, message);
        }
    }
}
