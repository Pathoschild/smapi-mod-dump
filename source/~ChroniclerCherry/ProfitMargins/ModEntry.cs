/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ChroniclerCherry/stardew-valley-mods
**
*************************************************/

using ProfitMargins.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace ProfitMargins
{
    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod
    {
        /*********
        ** Properties
        *********/
        /// <summary>The mod configuration from the player.</summary>
        private ModConfig config;
        private float originalDifficulty;

        /*********
        ** Public methods
        *********/

        public override void Entry(IModHelper helper)
        {
            this.config = helper.ReadConfig<ModConfig>();
            helper.Events.GameLoop.DayStarted += this.DayStarted;
            helper.Events.GameLoop.Saving += this.OnSaving;
            return;

        }

        /*********
        ** Private methods
        *********/

        private void DayStarted(object sender, DayStartedEventArgs args)
        {
            if (this.checkContext())
            {
                this.originalDifficulty = Game1.player.difficultyModifier;
                Game1.player.difficultyModifier = this.config.ProfitMargin;
            }
        }

        private void OnSaving(object sender, SavingEventArgs args)
        {
            if (this.checkContext())
            {
                Game1.player.difficultyModifier = this.originalDifficulty;
                this.Monitor.Log("During save, DL:" + Game1.player.difficultyModifier.ToString(), LogLevel.Debug);
            }
        }

        private bool checkContext()
        {
            if (!Context.IsMainPlayer)
            {
                return false;
            }
            else if (Context.IsMultiplayer && !this.config.EnableInMultiplayer)
            {
                return false;
            }
            return true;
        }
    }
}
