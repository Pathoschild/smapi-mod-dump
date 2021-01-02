/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Underscore76/SDVPracticeMod
**
*************************************************/

using System;
using Microsoft.Xna.Framework;
using SpeedrunPractice.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;

namespace SpeedrunPractice
{
    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod
    {

        private AnimationCancelHelper animationCancelHelper;
        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            animationCancelHelper = new AnimationCancelHelper();
            helper.Events.GameLoop.UpdateTicked += GameLoop_UpdateTicked;
            helper.Events.Display.RenderedWorld += Display_RenderedWorld;
        }

        

        /*********
        ** Private methods
        *********/
        /// <summary>Raised after the frame has been update ticked, before draw.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void GameLoop_UpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            // ignore if player hasn't loaded a save yet
            if (!Context.IsWorldReady)
                return;

            this.animationCancelHelper?.Update(this.Monitor, this.Helper);
        }

        /// <summary>Raised after the base world has been drawn before HUD elements.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void Display_RenderedWorld(object sender, RenderedWorldEventArgs e)
        {
            // ignore if player hasn't loaded a save yet
            if (!Context.IsWorldReady)
                return;

            animationCancelHelper?.Draw(e.SpriteBatch);
        }
    }
}