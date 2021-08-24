/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/popska/UsefulHats
**
*************************************************/

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Objects;

namespace HatBuffs
{
    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod
    {
        Buff currentBuff;
        Dictionary<string, Buff> buffs = new Dictionary<string, Buff>();


        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            helper.Events.GameLoop.UpdateTicked+= this.OnUpdateTicked;
            helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;
            helper.Events.GameLoop.DayStarted += this.OnDayStarted;
        }



        /*********
        ** Private methods
        *********/
        /// <summary>Raised after the player presses a button on the keyboard, controller, or mouse.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            // public Buff(int farming, int fishing, int mining, int digging, int luck, int foraging, int crafting, int maxStamina, int magneticRadius, int speed, int defense, int attack, int minutesDuration, string source, string displaySource)

            buffs.Add("Sailor's Cap", new Buff(0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, "UsefulHats", "Sailor's Cap"));
            buffs.Add("Bowler Hat", new Buff(0, 0, 0, 0, 0, 0, 0, 30, 0, 0, 0, 0, 1, "UsefulHats", "Bowler Hat"));
            buffs.Add("Straw Hat", new Buff(1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, "UsefulHats", "Straw Hat"));
            buffs.Add("Lucky Bow", new Buff(0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 1, "UsefulHats", "Lucky Bow"));
            buffs.Add("Eye Patch", new Buff(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, "UsefulHats", "Eye Patch"));
            buffs.Add("Hard Hat", new Buff(0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 1, 0, 1, "UsefulHats", "Hard Hat"));
            buffs.Add("Mushroom Cap", new Buff(0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 1, "UsefulHats", "Mushroom Cap"));
            buffs.Add("Dinosaur Hat", new Buff(0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 1, "UsefulHats", "Dinosaur Hat"));
            buffs.Add("Knight's Helmet", new Buff(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 1, "UsefulHats", "Knight's Helmet"));
            buffs.Add("Squire's Helmet", new Buff(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 1, "UsefulHats", "Squire's Helmet"));
            buffs.Add("Fishing Hat", new Buff(0, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, "UsefulHats", "Fishing Hat"));
            buffs.Add("Blobfish Mask", new Buff(0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, "UsefulHats", "Blobfish Mask"));
            buffs.Add("Party Hat", new Buff(0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 1, "UsefulHats", "Party Hat"));
            buffs.Add("Pirate Hat", new Buff(0, 1, 0, 0, 1, 0, 0, 0, 0, 0, 0, 1, 1, "UsefulHats", "Pirate Hat"));
            buffs.Add("Deluxe Pirate Hat", new Buff(0, 1, 0, 0, 2, 0, 0, 0, 0, 0, 0, 2, 1, "UsefulHats", "Deluxe Pirate Hat"));
            buffs.Add("Copper Pan", new Buff(0, 0, 0, 0, 0, 0, 0, 0, 32, 0, 0, 0, 1, "UsefulHats", "Copper Pan"));
            buffs.Add("Small Cap", new Buff(0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 1, "UsefulHats", "Small Cap"));
            buffs.Add("Forager's Hat", new Buff(0, 0, 0, 0, 0, 2, 0, 0, 0, 0, 0, 0, 1, "UsefulHats", "Forager's Hat"));
            buffs.Add("Warrior Helmet", new Buff(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2, 1, "UsefulHats", "Warrior Helmet"));
            buffs.Add("Emily's Magic Hat", new Buff(0, 0, 0, 0, 1, 0, 0, 0, 32, 0, 0, 0, 1, "UsefulHats", "Emily's Magic Hat"));
            buffs.Add("Good Ol' Cap", new Buff(1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, "UsefulHats", "Good Ol' Cap"));
            buffs.Add("Cat Ears", new Buff(0, 0, 0, 0, 0, 0, 0, 0, 0, 2, 0, 0, 1, "UsefulHats", "Cat Ears"));
            buffs.Add("Wearable Dwarf Helm", new Buff(0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, "UsefulHats", "Wearable Dwarf Helm"));
            buffs.Add("Chicken Mask", new Buff(0, 0, 0, 0, 0, 0, 0, 50, 0, 0, 0, 0, 1, "UsefulHats", "Chicken Mask"));
            buffs.Add("Totem Mask", new Buff(0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 1, "UsefulHats", "Totem Mask"));
            buffs.Add("Golden Mask", new Buff(0, 0, 0, 0, 0, 0, 0, 0, 32, 0, 0, 0, 1, "UsefulHats", "Golden Mask"));

            buffs.Add("Fedora", new Buff(0, 0, 0, 0, 0, 0, 0, 0, 32, 0, 0, 0, 1, "UsefulHats", "Fedora"));
            buffs.Add("Top Hat", new Buff(0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 1, "UsefulHats", "Top Hat"));
            buffs.Add("Tropiclip", new Buff(0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, "UsefulHats", "Tropiclip"));
            buffs.Add("Archer's Cap", new Buff(0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 1, "UsefulHats", "Archer's Cap"));
            buffs.Add("Tiara", new Buff(0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 1, "UsefulHats", "Tiara"));
        }


        // refreshes buffs in the morning
        private void OnDayStarted(object sender, EventArgs e)
        {
            currentBuff = null;
        }



        private void OnUpdateTicked(object sender, EventArgs e)
        {
            // ignore if player hasn't loaded a save yet
            if (!Context.IsWorldReady)
               return;
               
            Hat hat = Game1.player.hat;

            if (hat == null || !buffs.ContainsKey(hat.displayName))
            {
                if (currentBuff != null)
                {
                    Game1.buffsDisplay.removeOtherBuff(currentBuff.which);
                    Monitor.Log("Removing hat buff from " + currentBuff.displaySource, LogLevel.Trace);
                    currentBuff = null;
                }
            }
            else
            {
                if (buffs[hat.displayName] != currentBuff)
                {
                    if (currentBuff != null)
                    {
                        Game1.buffsDisplay.removeOtherBuff(currentBuff.which);
                        Monitor.Log("Removing hat buff from " + currentBuff.displaySource, LogLevel.Trace);
                    }
                    currentBuff = buffs[hat.displayName];
                    Game1.buffsDisplay.addOtherBuff(currentBuff);
                    Monitor.Log("Applying hat buff from " + currentBuff.displaySource, LogLevel.Trace);
                }
                currentBuff.millisecondsDuration = 50;
            }
            return;
        }
    }
}
