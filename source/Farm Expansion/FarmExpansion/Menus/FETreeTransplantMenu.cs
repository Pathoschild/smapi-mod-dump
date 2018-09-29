using FarmExpansion.Framework;
using StardewValley;
using StardewValley.Menus;
using System;
using Rectangle = Microsoft.Xna.Framework.Rectangle;
using xTile.Dimensions;
using Microsoft.Xna.Framework.Graphics;

namespace FarmExpansion.Menus
{
    internal class FETreeTransplantMenu : IClickableMenu
    {
        private FEFramework framework;
        private IClickableMenu TreeTransplantMenu;
        private ClickableTextureComponent swapFarmButton;
        private Rectangle previousClientBounds;

        public FETreeTransplantMenu(FEFramework framework, IClickableMenu menu)
        {
            this.framework = framework;
            this.TreeTransplantMenu = menu;

            this.assignClientBounds();

            this.swapFarmButton = new ClickableTextureComponent(new Rectangle(0, 0, Game1.tileSize, Game1.tileSize), framework.TreeTransplantFarmIcon, new Rectangle(0, 0, 64, 64), 1.0f);

            resetBounds();
        }

        private void resetBounds()
        {
            swapFarmButton.bounds.X = (Game1.viewport.Width - Game1.tileSize * 2) - (int)(Game1.tileSize / 0.75) - (int)(Game1.tileSize / 0.75);
            swapFarmButton.bounds.Y = (Game1.viewport.Height - Game1.tileSize * 2);
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            if (Game1.globalFade)
                return;
            if (swapFarmButton.containsPoint(x, y))
            {
                handleSwapFarmAction();
                return;
            }
        }

        public override void receiveRightClick(int x, int y, bool playSound = true)
        {
        }

        public override void performHoverAction(int x, int y)
        {
            swapFarmButton.tryHover(x, y);
        }

        public override void draw(SpriteBatch b)
        {
            if (Game1.globalFade)
                return;

            if (Game1.game1.Window.ClientBounds.X != previousClientBounds.X || Game1.game1.Window.ClientBounds.Y != previousClientBounds.Y || Game1.game1.Window.ClientBounds.Width != previousClientBounds.Width || Game1.game1.Window.ClientBounds.Height != previousClientBounds.Height)
            {
                assignClientBounds();
                resetBounds();
            }

            swapFarmButton.draw(b);
        }

        private void assignClientBounds()
        {
            Rectangle newClientBounds = Game1.game1.Window.ClientBounds;
            previousClientBounds = new Rectangle(newClientBounds.X, newClientBounds.Y, newClientBounds.Width, newClientBounds.Height);
        }

        private void handleSwapFarmAction()
        {
            if (TreeTransplantMenu.readyToClose())
                Game1.globalFadeToBlack(new Game1.afterFadeFunction(swapFarm));
            else
                framework.helper.Reflection.GetMethod(TreeTransplantMenu, "handleCancelAction").Invoke();
        }

        private void swapFarm()
        {
            // clean up before leaving the area
            Game1.currentLocation.cleanupBeforePlayerExit();
            // move to the opposite farm
            Game1.currentLocation = Game1.currentLocation.GetType().FullName.Contains("Expansion") ? Game1.getFarm() : Game1.getLocationFromName("FarmExpansion");
            // reset the location for our entry
            Game1.currentLocation.resetForPlayerEntry();
            // ensure the TreeTransplant menu is checking the right farm when determining whether a tree can be placed
            Farm farm = framework.helper.Reflection.GetField<Farm>(TreeTransplantMenu, "farm").GetValue();
            framework.helper.Reflection.GetField<Farm>(TreeTransplantMenu, "farm").SetValue(framework.swapFarm(farm));
            // set the new viewport
            Game1.viewport.Location = new Location(49 * Game1.tileSize, 5 * Game1.tileSize);
            // pan the screen
            Game1.panScreen(0, 0);
            // fade the screen in with no callback
            Game1.globalFadeToClear();
        }
    }
}
