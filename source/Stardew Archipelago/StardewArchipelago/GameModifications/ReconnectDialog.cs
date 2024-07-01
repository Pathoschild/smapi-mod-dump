/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/agilbert1412/StardewArchipelago
**
*************************************************/

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewArchipelago.Archipelago;
using StardewValley;
using StardewValley.Menus;

namespace StardewArchipelago.GameModifications
{
    public class ReconnectDialog : IClickableMenu
    {
        protected string message;
        protected OptionsTextEntry hostField;
        public ClickableTextureComponent cancelButton;
        public ClickableTextureComponent retryButton;
        protected behavior onClickRetry;
        protected behavior onClose;
        private bool active = true;
        private int y;

        public ReconnectDialog(
            string message,
            ArchipelagoConnectionInfo connectionInfo,
            behavior onClickRetryBehavior,
            behavior onCloseBehavior)
            : base(Game1.uiViewport.Width / 2 - (int)Game1.dialogueFont.MeasureString(message).X / 2 - borderWidth, Game1.uiViewport.Height / 2 - (int)Game1.dialogueFont.MeasureString(message).Y / 2, (int)Game1.dialogueFont.MeasureString(message).X + borderWidth * 2, (int)Game1.dialogueFont.MeasureString(message).Y + borderWidth * 2 + 160)
        {
            onClickRetry = onClickRetryBehavior;
            onClose = onCloseBehavior;
            var titleSafeArea = Game1.graphics.GraphicsDevice.Viewport.TitleSafeArea;
            this.message = Game1.parseText(message, Game1.dialogueFont, Math.Min(titleSafeArea.Width - 64, width));
            y = yPositionOnScreen + height - borderWidth - spaceToClearTopBorder + 31;
            hostField = new OptionsTextEntry("", -999, xPositionOnScreen + borderWidth + spaceToClearSideBorder + 250, y);
            if (connectionInfo != null)
            {
                hostField.textBox.Text = $"{connectionInfo.HostUrl}:{connectionInfo.Port}";
            }
            retryButton = new ClickableTextureComponent("Retry", new Rectangle(xPositionOnScreen + width - borderWidth - spaceToClearSideBorder - 128 - 4, y, 64, 64), null, null, Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 46), 1f);
            retryButton.myID = 105;
            retryButton.rightNeighborID = 101;
            cancelButton = new ClickableTextureComponent("Return", new Rectangle(xPositionOnScreen + width - borderWidth - spaceToClearSideBorder - 48 - 4, y, 64, 64), null, null, Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 47), 1f);
            cancelButton.myID = 101;
            cancelButton.rightNeighborID = 102;
            if (!Game1.options.SnappyMenus)
            {
                return;
            }
            populateClickableComponentList();
            snapToDefaultClickableComponent();
        }

        public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
        {
            base.gameWindowSizeChanged(oldBounds, newBounds);
            y = yPositionOnScreen + height - borderWidth - spaceToClearTopBorder + 31;
            hostField.bounds.X = xPositionOnScreen + borderWidth + spaceToClearSideBorder + 250;
            hostField.bounds.Y = y;
            retryButton.setPosition(xPositionOnScreen + width - borderWidth - spaceToClearSideBorder - 128 - 4, y);
            cancelButton.setPosition(xPositionOnScreen + width - borderWidth - spaceToClearSideBorder - 48 - 4, y);
        }

        public override void receiveKeyPress(Keys key)
        {
            if (key == Keys.None)
                return;
            if (Game1.options.doesInputListContain(Game1.options.menuButton, key) && readyToClose())
            {
                return;
            }

            base.receiveKeyPress(key);
        }

        public override void snapToDefaultClickableComponent()
        {
            currentlySnappedComponent = getComponentWithID(105);
            snapCursorToCurrentSnappedComponent();
        }

        public void Return()
        {
            onClose("");
            if (active)
                Game1.playSound("smallSelect");
            active = false;
        }

        public void Retry()
        {
            Game1.activeClickableMenu = null;
            onClickRetry(this.hostField.textBox.Text);
            if (active)
                Game1.playSound("smallSelect");
            active = false;
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            if (!active)
            {
                return;
            }
            if (cancelButton.containsPoint(x, y))
            {
                Return();
            }
            if (retryButton.containsPoint(x, y))
            {
                Retry();
            }
            if (hostField.bounds.Contains(x, y))
                hostField.receiveLeftClick(x, y);
        }

        public override void performHoverAction(int x, int y)
        {
            if (retryButton.containsPoint(x, y))
                retryButton.scale = Math.Min(retryButton.scale + 0.02f, retryButton.baseScale + 0.2f);
            else
                retryButton.scale = Math.Max(retryButton.scale - 0.02f, retryButton.baseScale);
            
            if (cancelButton.containsPoint(x, y))
                cancelButton.scale = Math.Min(cancelButton.scale + 0.02f, cancelButton.baseScale + 0.2f);
            else
                cancelButton.scale = Math.Max(cancelButton.scale - 0.02f, cancelButton.baseScale);
        }

        public override void draw(SpriteBatch b)
        {
            if (!active)
                return;
            b.Draw(Game1.fadeToBlackRect, new Rectangle(0, 0, Game1.uiViewport.Width, Game1.uiViewport.Height), Color.Black * 0.5f);
            Game1.drawDialogueBox(xPositionOnScreen, yPositionOnScreen, width, height, false, true);
            b.DrawString(Game1.dialogueFont, message, new Vector2(xPositionOnScreen + borderWidth, yPositionOnScreen + spaceToClearTopBorder + borderWidth / 2), Game1.textColor);
            
            b.DrawString(Game1.dialogueFont, "New address?", new Vector2(xPositionOnScreen + borderWidth, y), Game1.textColor);
            hostField.draw(b, 0, 0);
            retryButton.draw(b);
            cancelButton.draw(b);
            drawMouse(b);
        }

        public override void receiveRightClick(int x, int y, bool playSound = true)
        {
        }

        public delegate void behavior(string value);
    }
}
