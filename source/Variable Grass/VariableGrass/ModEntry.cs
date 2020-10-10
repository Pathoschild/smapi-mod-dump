/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/dantheman999301/StardewMods
**
*************************************************/

using System;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using VariableGrass.Framework;

namespace VariableGrass
{
    /// <summary>The mod entry class.</summary>
    public class ModEntry : Mod
    {
        /*********
        ** Properties
        *********/
        /// <summary>The minimum number of iterations.</summary>
        private int MinIterations;

        /// <summary>The maximum number of iterations.</summary>
        private int MaxIterations;

        /// <summary>The random number generator.</summary>
        private readonly Random Random = new Random();


        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            // get settings
            var config = helper.ReadConfig<ModConfig>();
            this.MinIterations = config.MinIterations;
            this.MaxIterations = config.MaxIterations;

            // register events
            helper.Events.GameLoop.DayStarted += this.OnDayStarted;
        }


        /*********
        ** Private methods
        *********/
        /// <summary>Raised after the game begins a new day (including when the player loads a save).</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnDayStarted(object sender, DayStartedEventArgs e)
        {
            int iterations = this.Random.Next(this.MinIterations, this.MaxIterations);
            this.Monitor.Log($"Growing grass ({iterations} iterations)...");
            Game1.getFarm().growWeedGrass(iterations);
        }
    }
}
