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
            //EventPatches.Apply();
            //FarmPatches.Apply();

            // Add console commands.
            ConsoleCommands.Apply();

            // Listen for game events.
            helper.Events.GameLoop.GameLaunched += this.onGameLaunched;
            helper.Events.GameLoop.UpdateTicked += this.onUpdateTicked;

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
    }
}
