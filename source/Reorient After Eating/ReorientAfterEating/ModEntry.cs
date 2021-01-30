/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/amconners/ReorientAfterEating
**
*************************************************/

using System;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;

namespace ReorientAfterEating
{
    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod
    {
        /*********
        ** Fields
        *********/
        /// <summary>
        /// If the farmer was eating since direction was last saved.
        /// </summary>
        private readonly PerScreen<bool> wasEating = new PerScreen<bool>();

        /// <summary>
        /// Remembers the direction the farmer was facing before eating.
        /// </summary>
        private readonly PerScreen<int> oldDirection = new PerScreen<int>();

        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            helper.Events.GameLoop.UpdateTicked += this.OnUpdateTicked;
            helper.Events.Input.ButtonPressed += this.OnButtonPressed;
        }

        /*********
        ** Private methods
        *********/
        /// <summary>Raised after the game state is updated</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnUpdateTicked(object sender, EventArgs e)
        {
            if (Game1.player.isEating && !this.wasEating.Value)
            {
                this.wasEating.Value = true;
            }
            else if (!Game1.player.isEating && this.wasEating.Value)
            {
                Game1.player.faceDirection(oldDirection.Value);
                this.wasEating.Value = false;
            }
        }

        /// <summary>Raised after the player pressed a keyboard, mouse, or controller button.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (Game1.activeClickableMenu == null)
            {
                oldDirection.Value = Game1.player.FacingDirection;
            }
        }
    }
}