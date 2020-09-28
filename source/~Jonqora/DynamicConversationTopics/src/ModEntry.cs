using System;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using Harmony;
using Netcode;
using StardewValley.Locations;
using System.Collections.Generic;
using System.Linq;
using System.Data.Common;

namespace DynamicConversationTopics
{
    public class ModEntry : Mod
    {
        /*********
        ** Fields
        *********/
        internal static ModEntry Instance { get; private set; }
        internal HarmonyInstance Harmony { get; private set; }

        /// <summary>Whether the next tick is the first one.</summary>
        private bool IsFirstTick = true;

        /// <summary>List of topics added by test commands for debugging.</summary>
        public List<string> TestTopics = new List<string>();

        //TODO: reset RecentTopicSpeakers every new day, and check for removals when game time changes.
        /// <summary>Stores NPC Names and integer gametime of their last time responding with conversation topic dialogue.</summary>
        public Dictionary<string, int> RecentTopicSpeakers = new Dictionary<string, int>();

        const int HOURS_CONSIDERED_RECENT = 1;


        /*********
        ** Accessors
        *********/
        internal protected static ModConfig Config => ModConfig.Instance;


        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            // Make resources available.
            Instance = this;
            ModConfig.Load();

            // Apply Harmony patches.
            Harmony = HarmonyInstance.Create(ModManifest.UniqueID);
            DialoguePatches.Apply();

            // Add console commands.
            ConsoleCommands.Apply();

            // Listen for game events.
            helper.Events.GameLoop.GameLaunched += this.onGameLaunched;
            helper.Events.GameLoop.UpdateTicked += this.onUpdateTicked;
            helper.Events.GameLoop.SaveLoaded += this.onSaveLoaded;
            helper.Events.GameLoop.TimeChanged += this.onTimeChanged;

            helper.Events.GameLoop.DayStarted += this.ClearRecentTopicSpeakers;
            helper.Events.GameLoop.ReturnedToTitle += this.ClearRecentTopicSpeakers;

            // Set up early dialogue asset editor - allows content patcher packs to easily overwrite.
            helper.Content.AssetEditors.Add(new DialogueEditor());
        }


        /*********
        ** Private methods
        *********/
        /****
        ** Event handlers
        ****/
        /// <summary>
        /// Sets up the Generic Mod Config Menu integration if available.
        /// Called after the game is launched, before the first update tick.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void onGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            ModConfig.SetUpMenu();
        }

        /// <summary>
        /// Loads asset editors on the second update tick, after Content Patcher has loaded most others.
        /// Raised after the game state is updated. Only runs twice then unhooks itself.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void onUpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            if (this.IsFirstTick)
            {
                this.IsFirstTick = false;
            }
            else // Is second update tick
            {
                Instance.Helper.Events.GameLoop.UpdateTicked -= this.onUpdateTicked; // Don't check again

                // Set up late asset loaders/editors. DialogueEditor is added in the Entry method instead.
                //Instance.Helper.Content.AssetEditors.Add(new GrandpaNoteEditor());
            }
        }

        /// <summary>
        /// Sets up test topics and invalidates cached assets used by this mod.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void onSaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            if (Config.DebugMode)
            {
                TestTopics = new List<string>() {
                    "DCT.Test0", "DCT.Test1", "DCT.Test2", "DCT.Test3", "DCT.Test4", "DCT.Test5", "DCT.Test6", "DCT.Test7", "DCT.Test8", "DCT.Test9"
                };
            }
            else TestTopics = new List<string>();

            //TODO: invalidate cached assets
        }

        /// <summary>
        /// Update table of NPCs who have "recently" given conversation topic dialogue.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void onTimeChanged(object sender, TimeChangedEventArgs e)
        {
            int currentTime = e.NewTime;
            int intTimeLimit = HOURS_CONSIDERED_RECENT * 100;
            List<string> toDelete = new List<string>();

            foreach (KeyValuePair<string, int> speaker in RecentTopicSpeakers)
            {
                if (Math.Abs(currentTime - speaker.Value) >= intTimeLimit)
                {
                    toDelete.Add(speaker.Key);
                }
            }

            int count = 0;
            foreach (string npc in toDelete)
            {
                RecentTopicSpeakers.Remove(npc);
                count++;
            }
            if (Config.DebugMode) Monitor.Log($"Cleared {count} NPCs from RecentTopicSpeakers.", LogLevel.Debug);
        }

        /// <summary>
        /// Clear table of NPCs who have "recently" given conversation topic dialogue.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void ClearRecentTopicSpeakers(object sender, EventArgs e)
        {
            int count = RecentTopicSpeakers.Count();
            RecentTopicSpeakers.Clear();
            if (Config.DebugMode) Monitor.Log($"Cleared {count} NPCs from RecentTopicSpeakers.", LogLevel.Debug);
        }
    }
}
