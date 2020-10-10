/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/minervamaga/NoMineFlyersRedux
**
*************************************************/

using System;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Monsters;
using System.Linq;

namespace NoMineFlyersRedux
{
    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod
    {
        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            //Checks list of locations when a new one is added to the game.  Gives that event a nickname for later
            helper.Events.World.LocationListChanged += this.OnLocationChanged;
            //Checks list of NPCs when updated.  Gives that event a nickname too
            helper.Events.World.NpcListChanged += this.OnNpcListChanged;
        }


        /*********
        ** Private methods
        *********/
        /// <summary>Raised after the player presses a button on the keyboard, controller, or mouse.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void RemoveIfApplicable(GameLocation location, Monster monster)
            //We're gonna kill the bugs
            //looks at the location in GameLocations and the monster in Monsters
        {
            if (monster is Fly)
                //If our monster is the Fly type
            location.characters.Remove(monster);
                //Kill it!

            // Log to console when it does the thing
            this.Monitor.Log($"Flyers removed!", LogLevel.Trace);
        }

        private void OnLocationChanged(object sender, LocationListChangedEventArgs e)
        {
            // ignore if player hasn't loaded a save yet
            if (!Context.IsWorldReady)
                return;
            
            //Checks each game location as it's added to the list
            foreach (GameLocation location in e.Added)
                //Then checks each monter in the list and adds them to a read-only list; prevents crashes when removing characters
                foreach (Monster monster in location.characters.OfType<Monster>().ToArray())
                {
                    //executes the RemoveIfApplicable to the specified location and monster
                    this.RemoveIfApplicable(location, monster);
                }
        }

        private void OnNpcListChanged(object sender, NpcListChangedEventArgs e)
            //Here's where we check the NPC list and use the SMAPI event
        {
            foreach (Monster monster in e.Added.OfType<Monster>())
                //Every monster in the NPC group that is added gets checked; specify the monsters only
                this.RemoveIfApplicable(e.Location, monster);
            //Do the thing if it matches our specified monster

        }

    }
}