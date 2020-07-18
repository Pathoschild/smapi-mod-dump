using System;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using Harmony;
using Netcode;
using StardewValley.Locations;
using System.Collections.Generic;
using System.Linq;

namespace AngryGrandpa
{
    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod
    {
        /*********
        ** Fields
        *********/
        internal static ModEntry Instance { get; private set; }
        internal HarmonyInstance Harmony { get; private set; }

        /// <summary>Whether the next tick is the first one.</summary>
        private bool IsFirstTick = true;
        /// <summary>Monitor in-game year for changes.</summary>
        private int CurrentYear;


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
            EventPatches.Apply();
            FarmPatches.Apply();
            ItemGrabMenuPatches.Apply();
            ObjectPatches.Apply();
            UtilityPatches.Apply();

            // Add console commands.
            ConsoleCommands.Apply();

            // Listen for game events.
            helper.Events.GameLoop.GameLaunched += this.onGameLaunched;
            helper.Events.GameLoop.UpdateTicked += this.onUpdateTicked;
            helper.Events.GameLoop.SaveLoaded += this.onSaveLoaded;
            helper.Events.GameLoop.DayStarted += this.onDayStarted;
            helper.Events.Player.Warped += this.onWarped;

            // Set up portrait asset editor. This one is added early since it never changes.
            helper.Content.AssetEditors.Add(new PortraitEditor());
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

                // Set up asset loaders/editors. PortraitEditor is added in the Entry method instead.
                Instance.Helper.Content.AssetEditors.Add(new GrandpaNoteEditor());
                Instance.Helper.Content.AssetEditors.Add(new EventEditor());
                Instance.Helper.Content.AssetEditors.Add(new EvaluationEditor());
            }
        }

        /// <summary>
        /// Checks to place the grandpa note mail in collections if needed; Initialises tracking current game year.
        /// Called after loading a save or connecting to a multiplayer world. 
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void onSaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            if (Game1.getFarm().hasSeenGrandpaNote
                && Game1.player.mailReceived[0] != "6324grandpaNoteMail" ) // Missing (or misplaced) AG mail flag?
            {
                Game1.player.mailReceived.Remove("6324grandpaNoteMail");
                Game1.player.mailReceived.Insert(0, "6324grandpaNoteMail"); // Insert grandpa note first on mail tab
            }
            CurrentYear = Game1.year; // Track year for updating cached Events
        }

        /// <summary>
        /// Resets cache for affected Event assets if needed.
        /// Raised after a new in-game day starts.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void onDayStarted(object sender, DayStartedEventArgs e)
        {
            ResetEventsCacheIfYearChanged();
        }

        /// <summary>
        /// Resets cache for affected Event assets if needed.
        /// Raised after the current player moves to a new location.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void onWarped(object sender, WarpedEventArgs e)
        {
            ResetEventsCacheIfYearChanged();
        }

        /****
        ** Helper methods
        ****/
        /// <summary>
        /// If the in-game year has changed, resets the cache for affected Event assets so they can be reloaded.
        /// Called by onDayStarted and onWarped.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void ResetEventsCacheIfYearChanged()
        {
            if (Game1.year != CurrentYear) // Need to reload assets that contain references to years passed
            {
                CurrentYear = Game1.year; // Update tracked value
                Helper.Content.InvalidateCache(asset // Trigger changed assets to reload on next use.
                => asset.AssetNameEquals("Data\\Events\\Farmhouse")
                || asset.AssetNameEquals("Data\\Events\\Farm"));
            }
        }
    }
}