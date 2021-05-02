/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/janavarro95/Stardew_Valley_Mods
**
*************************************************/

using System.Collections.Generic;
using System.Linq;
using Omegasis.AutoSpeed.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace Omegasis.AutoSpeed
{
    /// <summary>The mod entry point.</summary>
    public class AutoSpeed : Mod
    {
        /*********
        **Static Fields
        *********/
        /// <summary>
        /// All of the speed that is added together for auto speed. This is used for mod authors to hook in their speed boosts before auto speed applies the default speed boost.
        /// </summary>
        public Dictionary<string, int> combinedAddedSpeed;

        /*********
        ** Fields
        *********/
        /// <summary>The mod configuration.</summary>
        private ModConfig Config;

        /// <summary>
        /// A static reference to expose public fields.
        /// </summary>
        public static AutoSpeed Instance;

        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            helper.Events.GameLoop.UpdateTicked += this.OnUpdateTicked;
            this.Config = helper.ReadConfig<ModConfig>();
            this.combinedAddedSpeed = new Dictionary<string, int>();
            Instance = this;
        }

        /// <summary>
        /// Returns a copy of the mods' api.
        /// </summary>
        /// <returns></returns>
        public override object GetApi()
        {
            return new AutoSpeedAPI();
        }


        /*********
        ** Private methods
        *********/
        /// <summary>Raised after the game state is updated (≈60 times per second).</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            if (Context.IsPlayerFree)
            {
                int addedSpeed = this.combinedAddedSpeed.Values.Sum();
                Game1.player.addedSpeed = this.Config.Speed+addedSpeed;
            }
        }
    }
}
