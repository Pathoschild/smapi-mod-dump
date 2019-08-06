using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;

namespace EnergyCount
{
    /// <summary>The mod entry point.</summary>
    internal class ModEntry : Mod
    {
        /*********
        ** Properties
        *********/
        /// <summary>The player's last stamina value.</summary>
        private int LastStamina;


        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            helper.Events.GameLoop.SaveLoaded += this.SaveEvents_AfterLoad;
            helper.Events.GameLoop.UpdateTicked += this.GameEvents_UpdateTick;
            helper.Events.Display.RenderingHud += this.RenderingHud;
        }


        /*********
        ** Private methods
        *********/
        /// <summary>The method invoked when the player loads a save.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void SaveEvents_AfterLoad(object sender, EventArgs e)
        {
            this.LastStamina = (int)Game1.player.Stamina;
        }

        /// <summary>The method invoked after the game updates (roughly 60 times per second).</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void GameEvents_UpdateTick(object sender, EventArgs e)
        {
            // skip if save not loaded yet
            if (!Context.IsWorldReady)
                return;

            // skip if stamina not changed
            int currentStamina = (int)Game1.player.Stamina;
            if (currentStamina == this.LastStamina)
                return;

            // print message & update stamina
            // this.Monitor.Log($"Player stamina changed from {currentStamina} to {this.LastStamina}");
            this.LastStamina = currentStamina;
        }

        private void RenderingHud(object sender, RenderingHudEventArgs e)
        {
            int rightPosition = 0;
            if (Game1.showingHealth)
            {
                rightPosition = 265;
            }
            else
            {
                rightPosition = 215;
            }

            if(!Game1.eventUp)
                Game1.spriteBatch.DrawString(Game1.dialogueFont, $"{this.LastStamina}/{Game1.player.maxStamina}", new Vector2(Game1.graphics.GraphicsDevice.Viewport.GetTitleSafeArea().Right - rightPosition, Game1.graphics.GraphicsDevice.Viewport.GetTitleSafeArea().Bottom - 60), Color.White);
        }
    }
}