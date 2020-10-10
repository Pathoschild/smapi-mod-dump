/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/minervamaga/StandardizedKrobus
**
*************************************************/

using System;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Characters;

namespace StandardizedKrobus
{

    public class ModEntry : Mod
    {
        /*********
        ** Fields
        *********/

        //setting up for a config, the SMAPI helper, and other uses later
        internal static ModConfig Config;
        internal static IModHelper Helper;


        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            Helper = helper;

            //Checks for the save being loaded and when the player warps to a new location
            Helper.Events.Player.Warped += OnWarp;
            Helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;

            Config = Helper.ReadConfig<ModConfig>();

        }

        /*********
        ** Private methods
        *********/
        /// <summary>Raised after the player presses a button on the keyboard, controller, or mouse.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>

        private void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            //calling something we do below, so we don't have to type it out a million times; limits it to after the tick so it doesn't run before it can actually work
            Helper.Events.GameLoop.UpdateTicked += this.ResizeKrobus;
        }


        private void OnWarp(object sender, WarpedEventArgs e)
        {
            //same thing again, calling the method below so we dont' have to type it out
            Helper.Events.GameLoop.UpdateTicked += ResizeKrobus;
        }

        /*********
        ** Public methods
        *********/
        public void ResizeKrobus(object sender, UpdateTickedEventArgs e)
        {
            //if not ready and free, keep going

            if (!Context.IsWorldReady && !Context.IsPlayerFree)
            {
                if (ModEntry.Config.VerboseLogging)
                {
                    //If the game isn't fully loaded in and it tries to run for some reason, throw a log and move on.  No crashes here!
                    this.Monitor.Log($"World is not loaded, unable to patch!", LogLevel.Debug);
                }
                return;
            }
            else
            {
                //proceed if world is ready.  Grab Krobus's info from the name first
                NPC krobus = Game1.getCharacterFromName("Krobus", false);
                if (ModEntry.Config.VerboseLogging)
                {
                    //Add a log message that we got this far
                    Monitor.Log($"Found Krobus", LogLevel.Info);
                }
                //set the sprite height and width.  Width doesn't change, but we want to be sure
                krobus.Sprite.SpriteHeight = 32;
                krobus.Sprite.SpriteWidth = 16;
                //and then tell the game to update its info
                krobus.Sprite.UpdateSourceRect();
                if (ModEntry.Config.VerboseLogging)
                {
                    //Confirms in the log that the updates completed successfully
                    this.Monitor.Log($"Set Krobus's sprite to standard size!", LogLevel.Info);
                }
                //Do the resize *after* the update tick completes.  Helps prevent null check issues
                Helper.Events.GameLoop.UpdateTicked -= this.ResizeKrobus;
            }

        }

    }
}
