/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/elizabethcd/MoreConversationTopics
**
*************************************************/

using System;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace MoreConversationTopics
{
    public class MCTHelperFunctions
    {
        private static IMonitor Monitor;
        private static ModConfig Config;

        // Initialize Monitor and Config for this class and all other classes
        public static void Initialize(IMonitor monitor, ModConfig config)
        {
            Monitor = monitor;
            Config = config;

            RepeatPatcher.Initialize(Monitor, Config);
            WeddingPatcher.Initialize(Monitor, Config);
            BirthPatcher.Initialize(Monitor, Config);
            DivorcePatcher.Initialize(Monitor, Config);
            LuauPatcher.Initialize(Monitor, Config);
            WorldChangePatcher.Initialize(Monitor, Config);
            NightEventPatcher.Initialize(Monitor, Config);
            JojaEventAssetEditor.Initialize(Monitor, Config);
            IslandPatcher.Initialize(Monitor, Config);
        }

        // Helper function to check if a string is on the list of repeatable CTs added by this mod
        public static Boolean isRepeatableCTAddedByMod(string topic)
        {
            string[] modRepeatableConversationTopics = new string[] {
                "wedding", "divorce", "babyBoy", "babyGirl", // repeatable social CTs
                "luauBest", "luauShorts", "luauPoisoned", // repeatable luau CTs
                "meteoriteLandedOnFarm", "owlStatueLandedOnFarm", // repeatable night event CTs
                "fairyFarmVisit", "witchSlimeHutVisit", "goldenWitchCoopVisit", "witchCoopVisit" // repeatable magical farm event CTs
            };
            foreach (string s in modRepeatableConversationTopics)
            {
                if (s == topic)
                {
                    return true;
                }
            }
            return false;
        }

        // Function to add conversation topics that may already exist to the current player
        public static bool AddMaybePreExistingCT(string conversationTopic, int duration)
        {
            // Check if the conversation topic has already been added
            if (Game1.player.activeDialogueEvents.ContainsKey(conversationTopic))
            {
                Monitor.Log($"Not adding conversation topic {conversationTopic} because it's already there.", LogLevel.Warn);
                return false;
            }

            // If not, then add the conversation topic to the desired player
            Game1.player.activeDialogueEvents.Add(conversationTopic, duration);
            return true;
        }

        // Function to add conversation topics that may already exist to any player
        public static bool AddMaybePreExistingCT(Farmer playerToAddTo, string conversationTopic, int duration)
        {
            // Check if the conversation topic has already been added
            if (Game1.player.activeDialogueEvents.ContainsKey(conversationTopic))
            {
                Monitor.Log($"Not adding conversation topic {conversationTopic} because it's already there.", LogLevel.Warn);
                return false;
            }

            // If not, then add the conversation topic to the desired player
            playerToAddTo.activeDialogueEvents.Add(conversationTopic, duration);
            return true;
        }

        // Prints out all current conversation topics for console command
        public static void console_GetCurrentCTs(string command, string[] args)
        {
            if (!Context.IsWorldReady)
                return;

            try
            {
                Monitor.Log(string.Join(", ", Game1.player.activeDialogueEvents.Keys), LogLevel.Debug);
            }
            catch (Exception ex)
            {
                Monitor.Log($"Failed to print conversation topics with exception: {ex}", LogLevel.Error);
            }
        }

        // Checks mail flags for console command
        public static void console_HasMailFlag(string command, string[] args)
        {
            if (!Context.IsWorldReady)
                return;

            try
            {
                if (Game1.player.mailReceived.Contains(args[0]))
                {
                    Monitor.Log($"Yes, you have this mail flag", LogLevel.Debug);
                }
                else
                {
                    Monitor.Log($"No, you don't have this mail flag", LogLevel.Debug);
                }
            }
            catch (Exception ex)
            {
                Monitor.Log($"Bad or missing argument with exception: {ex}", LogLevel.Error);
            }
        }

        // Add conversation topic for console command
        public static void console_AddConversationTopic(string command, string[] args)
        {
            if (!Context.IsWorldReady)
                return;

            // Try to get the duration from the input arguments, default to 1 if cannot be parsed
            int duration = 1;
            try
            {
                duration = Int32.Parse(args[1]);
            }
            catch
            {
                Monitor.Log($"Couldn't parse duration as an integer, defaulting to 1 day", LogLevel.Warn);
            }

            // Add the conversation topic to the current player
            try
            {
                bool success = AddMaybePreExistingCT(args[0], duration);
                if (success)
                {
                    Monitor.Log($"Added conversation topic {args[0]} with duration {duration}", LogLevel.Debug);
                }
            }
            catch (Exception ex)
            {
                Monitor.Log($"Couldn't add conversation topic with exception: {ex}", LogLevel.Error);
            }
        }

        // Remove conversation topic for console command
        public static void console_RemoveConversationTopic(string command, string[] args)
        {
            if (!Context.IsWorldReady)
                return;

            try
            {
                Game1.player.activeDialogueEvents.Remove(args[0]);
                Monitor.Log("Removed conversation topic", LogLevel.Debug);
            }
            catch (Exception ex)
            {
                Monitor.Log($"Bad or missing argument with exception: {ex}", LogLevel.Error);
            }
        }
    }
}
