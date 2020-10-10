/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/EpicBellyFlop45/StardewMods
**
*************************************************/

using Harmony;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;

namespace ConsoleChat
{
    /// <summary>The mod entry point.</summary>
    class ModEntry : Mod
    {
        /// <summary>Provides method to log to the console.</summary>
        static IMonitor ModMonitor;

        /// <summary>The mod entry point</summary>
        /// <param name="helper">Provides methods for interacting with the mod directory as well as the modding api.</param>
        public override void Entry(IModHelper helper)
        {
            ApplyHarmonyPatches();

            ModMonitor = this.Monitor;

            // Add a console command for the 'say' command to talk from the console
            this.Helper.ConsoleCommands.Add("say", "Writes text to chatbox from console.\n\nUsage: say <message>\n- message: The message to post in chatbox.", Say);
        }

        private void ApplyHarmonyPatches()
        {
            // Create a new Harmony instance for patching source code
            HarmonyInstance harmony = HarmonyInstance.Create(this.ModManifest.UniqueID);

            // Apply patch
            harmony.Patch(
                original: AccessTools.Method(typeof(ChatBox), nameof(ChatBox.receiveChatMessage)),
                postfix: new HarmonyMethod(AccessTools.Method(typeof(ModEntry), nameof(ModEntry.Postfix)))
            );
        }

        /// <summary>This method get's ran whenever a player writes a message. This is used to capture the message sender and body to print into the console.</summary>
        /// <param name="sourceFarmer">The farmer that sent the message.</param>
        /// <param name="message">The message body.</param>
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

        /// <summary>This is a method that get's ran if the player using the 'say' command in the console, this will print that message to the ingame chatbox.</summary>
        /// <param name="command">The command the user used. (SMAPI handles validating so we have no need for this)</param>
        /// <param name="args">The message the user want's to pass to the chatbox. (Each word is a separate argument)</param>
        private void Say(string command, string[] args)
        {
            string message = "";

            foreach (var arg in args)
            {
                message += arg + " ";
            }

            // Ensure a save has been loaded and the user has inputted a message
            if (Game1.chatBox == null || string.IsNullOrEmpty(message))
            {
                return;
            }

            Game1.chatBox.addMessage($"Console: {message}", new Microsoft.Xna.Framework.Color(150, 150, 150));

            // Log console message to console
            ModMonitor.Log($"Console: {message}", LogLevel.Info);
        }
    }
}
