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
using Microsoft.Xna.Framework;
using HarmonyLib;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;

namespace MoreConversationTopics
{
    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod
    {

        // Properties
        private ModConfig Config;

        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            // Read in config file and create if needed
            this.Config = this.Helper.ReadConfig<ModConfig>();

            // Initialize the error logger in WeddingPatcher
            WeddingPatcher.Initialize(this.Monitor, this.Config);
            LuauPatcher.Initialize(this.Monitor, this.Config);
            BirthPatcher.Initialize(this.Monitor, this.Config);
            DivorcePatcher.Initialize(this.Monitor, this.Config);

            // Do the Harmony things
            var harmony = new Harmony(this.ModManifest.UniqueID);
            WeddingPatcher.Apply(harmony);
            LuauPatcher.Apply(harmony);
            BirthPatcher.Apply(harmony);
            DivorcePatcher.Apply(harmony);

            // Adds a command to check current active conversation topics
            helper.ConsoleCommands.Add("current_conversation_topics", "Dumps currently active dialogue events", (str, strs) =>
            {
                if (!Context.IsWorldReady)
                    return;

                // Add a test event to see if it's working
                //if (!Game1.player.activeDialogueEvents.ContainsKey("testDialogueEvent"))
                //{
                //    Game1.player.activeDialogueEvents.Add("testDialogueEvent", 1);
                //}

                Monitor.Log(string.Join(", ", Game1.player.activeDialogueEvents.Keys),LogLevel.Debug);
            });
        }
    }
}