using System;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using static StardewValley.Minigames.MineCart;

namespace ConfigurableJunimoKart
{
    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod
    {
        private ModConfig config_;

        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            this.config_ = helper.ReadConfig<ModConfig>();
            helper.Events.Input.ButtonPressed += this.OnButtonPressed;
            helper.Events.GameLoop.UpdateTicked += this.eventUpdateTicks;
        }


        /*********
        ** Private methods
        *********/
        /// <summary>Raised after the player presses a button on the keyboard, controller, or mouse.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            // Ignore if player hasn't loaded a save yet
            if (!Context.IsWorldReady)
                return;
            // Ignore if player isn't playing Junimo Kart 
            if (Game1.currentMinigame == null || !"MineCart".Equals(Game1.currentMinigame.GetType().Name))
            {
                return;
            }

            // If inifinite jumps is enabled and space is pressed then force a jump
            if (this.config_.infinite_jumps && SButton.Space == e.Button)
            {
                // Get Junimo Kart player object
                MineCartCharacter player = this.Helper.Reflection.GetField<MineCartCharacter>(Game1.currentMinigame, "player").GetValue();
                // Force jump with config value jump strength
                player.velocity.Y = 0f - this.config_.jump_strength;
                // Reinsert Junimo Kart player object with new values
                this.Helper.Reflection.GetField<MineCartCharacter>(Game1.currentMinigame, "player").SetValue(player);
            }
        }

        /// <summary>Raised on every game tick.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void eventUpdateTicks(object sender, EventArgs e)
        {
            // Ignore if player hasn't loaded a save yet
            if (!Context.IsWorldReady)
                return;

            // Ignore if player isn't playing Junimo Kart
            if (Game1.currentMinigame == null || !"MineCart".Equals(Game1.currentMinigame.GetType().Name))
            {
                return;
            }

            // If infinite lives is enabled then force lives to 3 every tick
            if (this.config_.infinite_lives)
            {
                this.Helper.Reflection.GetField<int>(Game1.currentMinigame, "livesLeft").SetValue(3);
            }

            // Get Junimo Kart player object
            MineCartCharacter player = this.Helper.Reflection.GetField<MineCartCharacter>(Game1.currentMinigame, "player").GetValue();

            // Force gravity to constant fall gravity in config
            player.gravity = this.config_.gravity;

            // Force player private speed multiplier to speed multiplier in config
            this.Helper.Reflection.GetField<float>(player,"_speedMultiplier").SetValue(this.config_.speed_multiplier);

            // Reinsert Junimo Kart player object with new values
            this.Helper.Reflection.GetField<MineCartCharacter>(Game1.currentMinigame, "player").SetValue(player);
        }

    }
}
