/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Janglinator/Secretariat
**
*************************************************/

using System;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;

namespace Secretariat
{
    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod
    {
        private ModConfig _config;
        private int _addedSpeed = 0;

        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            _config = this.Helper.ReadConfig<ModConfig>();
            helper.Events.GameLoop.UpdateTicked += this.OnUpdateTicked;
        }


        /*********
        ** Private methods
        *********/
        /// <summary>Raised after the game state is updated (≈60 times per second).</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            if (!Context.IsPlayerFree || !Context.IsWorldReady || Game1.paused
                || Game1.activeClickableMenu != null)
                return;

            UpdateAddedSpeed();
        }

        private void UpdateAddedSpeed()
        {
            int existingBuff = 0;

            if (Game1.player.addedSpeed > _addedSpeed)
            {
                // There's an existing buff
                existingBuff = Game1.player.addedSpeed - _addedSpeed;
            }

            if (Game1.player.mount != null && _addedSpeed == 0)
            {
                _addedSpeed = _config.SpeedBoost;
            }
            else if (Game1.player.mount == null && _addedSpeed != 0)
            {
                _addedSpeed = 0;
            }

            Game1.player.addedSpeed = _addedSpeed + existingBuff;
        }
    }
}