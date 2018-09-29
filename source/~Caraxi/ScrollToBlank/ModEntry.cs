using System;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;

namespace ScrollToBlank
{
    public class ModEntry : Mod
    {
        public override void Entry(IModHelper helper)
        {
            GameEvents.UpdateTick += this.updateTick;
            ControlEvents.MouseChanged += this.mouseChanged;
        }

        private void updateTick(object sender, EventArgs e)
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

        private void mouseChanged(object sender, EventArgsMouseStateChanged e)
        {
            
            if (e.NewState.ScrollWheelValue != e.PriorState.ScrollWheelValue)
            {
                int currentToolIndex = Game1.player.CurrentToolIndex;
                this.newToolIndex = currentToolIndex;

                bool mouseWheelMovedDown = e.NewState.ScrollWheelValue < e.PriorState.ScrollWheelValue;
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
        }

        private bool updateToolIndex = false;

        private int newToolIndex = 0;
    }
}
