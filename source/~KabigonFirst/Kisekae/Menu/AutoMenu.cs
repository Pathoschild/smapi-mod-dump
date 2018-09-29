using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;
using StardewModdingAPI;

namespace Kisekae.Menu {
    abstract class AutoMenu : IClickableMenu {
        public static int s_borderSize = IClickableMenu.borderWidth * 2 - Game1.tileSize / 2;
        /// <summary>Whether draw cusor.</summary>
        public bool m_drawCursor = true;
        /// <summary>Whether draw menu frame.</summary>
        public bool m_drawMenuFrame = true;
        /// <summary>Whether draw black background.</summary>
        public bool m_drawBlackFade = true;

        /// <summary>Global Mod Interface.</summary>
        protected readonly IMod m_env;
        /// <summary>The messages to display on the screen.</summary>
        protected readonly List<Alert> m_alerts = new List<Alert>();
        /// <summary>The components.</summary>
        protected readonly List<IAutoComponent> m_components = new List<IAutoComponent>();

        public AutoMenu(IMod env, int x, int y, int width, int height, bool showUpperRightCloseButton = false) : base(x, y, width, height, showUpperRightCloseButton) {
            m_env = env;
        }
        /// <summary>Update the menu state.</summary>
        /// <param name="time">The elapsed game time.</param>
        public override void update(GameTime time) {
            base.update(time);

            // update alert messages
            for (int i = this.m_alerts.Count - 1; i >= 0; i--) {
                if (this.m_alerts.ElementAt(i).Update(time))
                    this.m_alerts.RemoveAt(i);
            }
        }
        /// <summary>Draw the menu to the screen.</summary>
        /// <param name="spriteBatch">The sprite batch to which to draw.</param>
        public override void draw(SpriteBatch spriteBatch) {
            // black background
            if (m_drawBlackFade) {
                spriteBatch.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.4f);
            }

            // menu frame
            if (m_drawMenuFrame) {
                Game1.drawDialogueBox(xPositionOnScreen - Game1.tileSize / 4, yPositionOnScreen - Game1.tileSize - Game1.tileSize / 4, width + Game1.tileSize / 2, height + Game1.tileSize + Game1.tileSize / 2, false, true);
            }

            // components
            foreach (IAutoComponent cpt in m_components) {
                if (cpt.m_visible) {
                    cpt.draw(spriteBatch);
                }
            }

            // alerts
            foreach (Alert alert in m_alerts) {
                alert.Draw(spriteBatch, Game1.smallFont);
            }

            // cursor
            if (m_drawCursor) {
                spriteBatch.Draw(Game1.mouseCursors, new Vector2(Game1.getOldMouseX(), Game1.getOldMouseY()), Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 0, 16, 16), Color.White, 0f, Vector2.Zero, Game1.pixelZoom + Game1.dialogueButtonScale / 150f, SpriteEffects.None, 1f);
            }
        }
        /// <summary>The method invoked when the player presses the left mouse button.</summary>
        /// <param name="x">The X-position of the cursor.</param>
        /// <param name="y">The Y-position of the cursor.</param>
        /// <param name="playSound">Whether to enable sound.</param>
        public override void receiveLeftClick(int x, int y, bool playSound = true) {
            foreach (IAutoComponent cpt in m_components) {
                if (cpt.m_visible && cpt.containsPoint(x, y)) {
                    if (handleLeftClick(cpt)) {
                        return;
                    }
                }
            }
        }
        /// <summary>The method invoked when the cursor is over a given position.</summary>
        /// <param name="x">The X mouse position.</param>
        /// <param name="y">The Y mouse position.</param>
        public override void performHoverAction(int x, int y) {
            foreach (IAutoComponent cpt in m_components) {
                if (cpt is IHoverable h && h.m_hoverScale > 0) {
                    h.tryHover(x, y, h.m_hoverScale);
                }
            }
        }
        /// <summary>Perform the action associated with a component.</summary>
        /// <param name="cpt">The component.</param>
        /// <return>whether the component action is processed</return>
        public virtual bool handleLeftClick(IAutoComponent cpt) {
            return false;
        }
        /// <summary>The method called when the game window changes size.</summary>
        /// <param name="oldBounds">The former viewport.</param>
        /// <param name="newBounds">The new viewport.</param>
        public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds) {
            base.gameWindowSizeChanged(oldBounds, newBounds);
            this.updateLayout();
        }
        /// <summary>Update the menu layout for a change in the zoom level or viewport size.</summary>
        public virtual void updateLayout() {
        }
    }
}
