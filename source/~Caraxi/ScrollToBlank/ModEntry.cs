using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;

namespace ScrollToBlank
{
    public class ModEntry : Mod
    {
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            helper.Events.GameLoop.UpdateTicked += this.onUpdateTicked;
            helper.Events.Input.MouseWheelScrolled += this.onMouseWheelScrolled;
        }

        /// <summary>Raised after the game state is updated (≈60 times per second).</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void onUpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            if (this.updateToolIndex)
            {
                this.updateToolIndex = false;

                bool blockUpdate = false;
                blockUpdate = blockUpdate || Game1.player.UsingTool;
                blockUpdate = blockUpdate || (Game1.activeClickableMenu is GameMenu menu);

                if (Game1.menuUp)
                {
                    this.Monitor.Log("Menu Up");
                }

                if (!blockUpdate)
                {
                    Game1.player.CurrentToolIndex = this.newToolIndex;
                }
            }
        }

        /// <summary>Raised after the player scrolls the mouse wheel.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void onMouseWheelScrolled(object sender, MouseWheelScrolledEventArgs e)
        {
            int currentToolIndex = Game1.player.CurrentToolIndex;
            this.newToolIndex = currentToolIndex;

            bool mouseWheelMovedDown = e.Delta < 0;
            bool invertScrollDirection = Game1.options.invertScrollDirection;

            if (mouseWheelMovedDown && !invertScrollDirection)
            {
                this.newToolIndex++;
            }
            else
            {
                this.newToolIndex--;
            }

            // Wrap Around
            if (this.newToolIndex < 0)
            {
                this.newToolIndex = 11;
            }

            if (this.newToolIndex >= 12)
            {
                this.newToolIndex = 0;
            }

            this.updateToolIndex = true;
        }

        private bool updateToolIndex = false;

        private int newToolIndex = 0;
    }
}
