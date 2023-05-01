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
using StardewValley;
using StardewValley.Menus;

namespace StardewArchipelago.GameModifications
{
    public class InformationDialog : IClickableMenu
    {
        public const int region_okButton = 101;
        protected string message;
        public ClickableTextureComponent okButton;
        protected ConfirmationDialog.behavior onClickOk;
        protected ConfirmationDialog.behavior onClose;
        private bool active = true;

        public InformationDialog(
          string message,
          ConfirmationDialog.behavior onClickOkBehavior = null,
          ConfirmationDialog.behavior onCloseBehavior = null)
          : base(Game1.uiViewport.Width / 2 - (int)Game1.dialogueFont.MeasureString(message).X / 2 - borderWidth, Game1.uiViewport.Height / 2 - (int)Game1.dialogueFont.MeasureString(message).Y / 2, (int)Game1.dialogueFont.MeasureString(message).X + borderWidth * 2, (int)Game1.dialogueFont.MeasureString(message).Y + borderWidth * 2 + 160)
        {
            onClickOk = onClickOkBehavior ?? CloseDialog;
            onClose = onCloseBehavior;
            var titleSafeArea = Game1.graphics.GraphicsDevice.Viewport.GetTitleSafeArea();
            this.message = Game1.parseText(message, Game1.dialogueFont, Math.Min(titleSafeArea.Width - 64, width));
            okButton = new ClickableTextureComponent("OK", new Rectangle(xPositionOnScreen + width - borderWidth - spaceToClearSideBorder - 128 - 4, yPositionOnScreen + height - borderWidth - spaceToClearTopBorder + 21, 64, 64), null, null, Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 46), 1f);
            okButton.myID = 101;
            okButton.rightNeighborID = 102;
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
            okButton.setPosition(xPositionOnScreen + width - borderWidth - spaceToClearSideBorder - 128 - 4, yPositionOnScreen + height - borderWidth - spaceToClearTopBorder + 21);
        }

        public override void receiveKeyPress(Keys key)
        {
            if (key == Keys.None)
                return;
            if (Game1.options.doesInputListContain(Game1.options.menuButton, key) && this.readyToClose())
            {
                CloseDialog(Game1.player);
                return;
            }

            base.receiveKeyPress(key);
        }

        public virtual void CloseDialog(Farmer who)
        {
            if (onClose != null)
            {
                onClose(who);
                return;
            }

            if (Game1.activeClickableMenu is TitleMenu titleMenu)
            {
                titleMenu.backButtonPressed();
            }
            else
            {
                Game1.exitActiveMenu();
            }
        }

        public override void snapToDefaultClickableComponent()
        {
            currentlySnappedComponent = getComponentWithID(102);
            snapCursorToCurrentSnappedComponent();
        }

        public void Confirm()
        {
            if (onClickOk != null)
                onClickOk(Game1.player);
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
            if (okButton.containsPoint(x, y))
            {
                Confirm();
            }
        }

        public override void performHoverAction(int x, int y)
        {
            if (okButton.containsPoint(x, y))
                okButton.scale = Math.Min(okButton.scale + 0.02f, okButton.baseScale + 0.2f);
            else
                okButton.scale = Math.Max(okButton.scale - 0.02f, okButton.baseScale);
        }

        public override void draw(SpriteBatch b)
        {
            if (!active)
                return;
            b.Draw(Game1.fadeToBlackRect, new Rectangle(0, 0, Game1.uiViewport.Width, Game1.uiViewport.Height), Color.Black * 0.5f);
            Game1.drawDialogueBox(xPositionOnScreen, yPositionOnScreen, width, height, false, true);
            b.DrawString(Game1.dialogueFont, message, new Vector2(xPositionOnScreen + borderWidth, yPositionOnScreen + spaceToClearTopBorder + borderWidth / 2), Game1.textColor);
            okButton.draw(b);
            drawMouse(b);
        }

        public override void receiveRightClick(int x, int y, bool playSound = true)
        {
        }

        public delegate void behavior(Farmer who);
    }
}
