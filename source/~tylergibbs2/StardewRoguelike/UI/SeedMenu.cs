/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tylergibbs2/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Menus;
using System;

namespace StardewRoguelike.UI
{
    internal class SeedInputBox : TextBox
    {
        public SeedInputBox(SpriteFont font, Color textColor) : base(null, null, font, textColor) { }

        public override void RecieveTextInput(char inputChar)
        {
            if (Text.Length == 0 && inputChar == '-')
            {
                Text += inputChar;
                return;
            }

            if (!Selected || !char.IsDigit(inputChar))
                return;

            Text += inputChar;
        }
    }

    internal class SeedMenu : IClickableMenu
    {
        private SeedInputBox textBox;

        private ClickableComponent textBoxComponent;

        private ClickableTextureComponent randomButton;

        private ClickableTextureComponent doneButton;

        private ClickableTextureComponent doRerollButton;

        private bool rerollTarget;

        public SeedMenu()
        {
            rerollTarget = Roguelike.RerollRandomEveryRun;

            textBox = new(Game1.smallFont, Game1.textColor);
            textBox.OnEnterPressed += textBoxEnter;
            Game1.keyboardDispatcher.Subscriber = textBox;
            textBox.Selected = true;

            textBoxComponent = new(Rectangle.Empty, "")
            {
                myID = 1001,
                rightNeighborID = 1002,
                downNeighborID = 1004
            };
            randomButton = new(Rectangle.Empty, Game1.mouseCursors, new Rectangle(381, 361, 10, 10), 4f)
            {
                myID = 1002,
                leftNeighborID = 1001,
                rightNeighborID = 1003
            };
            doneButton = new(Rectangle.Empty, Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 46), 1f)
            {
                myID = 1003,
                leftNeighborID = 1002
            };
            doRerollButton = new ClickableTextureComponent("Reroll", Rectangle.Empty, null, "Reroll Every Run", Game1.mouseCursors, new Rectangle(227, 425, 9, 9), 4f)
            {
                myID = 1004,
                upNeighborID = 1001
            };

            CalculatePositions();
            textBox.Text = Roguelike.FloorRngSeed.ToString();

            allClickableComponents ??= new();

            allClickableComponents.Add(textBoxComponent);
            allClickableComponents.Add(randomButton);
            allClickableComponents.Add(doneButton);
            allClickableComponents.Add(doRerollButton);

            if (Game1.options.gamepadControls)
                snapToDefaultClickableComponent();
        }

        private void CalculatePositions()
        {
            width = 600;
            height = 190;

            xPositionOnScreen = Game1.uiViewport.Width / 2 - (width / 2);
            yPositionOnScreen = Game1.uiViewport.Height / 2 - (height / 2) - 100;

            textBox.X = xPositionOnScreen + 32;
            textBox.Y = yPositionOnScreen + 54;
            textBox.Width = 370;
            textBox.Height = 186;

            textBoxComponent.bounds = new(textBox.X - 64, textBox.Y - 16, 400, 75);

            randomButton.bounds = new(xPositionOnScreen + width - 150, yPositionOnScreen + 54, 64, 64);
            doneButton.bounds = new(xPositionOnScreen + width - 90, yPositionOnScreen + 42, 64, 64);
            doRerollButton.bounds = new(xPositionOnScreen + 32, yPositionOnScreen + textBoxComponent.bounds.Height + 56, 36, 36);
            doRerollButton.sourceRect.X = rerollTarget ? 236 : 227;
        }

        public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
        {
            base.gameWindowSizeChanged(oldBounds, newBounds);
            CalculatePositions();
        }

        public override void snapToDefaultClickableComponent()
        {
            currentlySnappedComponent = getComponentWithID(1001);
            snapCursorToCurrentSnappedComponent();
        }

        public override void performHoverAction(int x, int y)
        {
            base.performHoverAction(x, y);
            doneButton.tryHover(x, y);
            randomButton.tryHover(x, y);
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            base.receiveLeftClick(x, y, playSound);
            textBox.Update();
            if (doneButton.containsPoint(x, y))
            {
                textBoxEnter(textBox);
                Game1.playSound("smallSelect");
            }
            else if (randomButton.containsPoint(x, y))
            {
                textBox.Text = Guid.NewGuid().GetHashCode().ToString();
                Game1.playSound("drumkit6");
            }
            else if (doRerollButton.containsPoint(x, y))
            {
                Game1.playSound("drumkit6");
                rerollTarget = !rerollTarget;
                doRerollButton.sourceRect.X = rerollTarget ? 236 : 227;
            }
        }

        private void textBoxEnter(TextBox sender)
        {
            if (sender.Text.Length >= 1)
            {
                Roguelike.FloorRngSeed = int.Parse(sender.Text);
                Roguelike.FloorRng = new(Roguelike.FloorRngSeed);
                Roguelike.RerollRandomEveryRun = rerollTarget;

                Game1.exitActiveMenu();
            }
        }

        public override void draw(SpriteBatch b)
        {
            b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.75f);

            SpriteText.drawStringWithScrollCenteredAt(b, I18n.UI_SeedMenu_Title(), Game1.uiViewport.Width / 2, yPositionOnScreen - 70, I18n.UI_SeedMenu_Title());

            drawTextureBox(
                b,
                Game1.menuTexture,
                new Rectangle(0, 256, 60, 60),
                xPositionOnScreen,
                yPositionOnScreen,
                width,
                height,
                Color.White,
                drawShadow: true
            );

            Utility.drawTextWithShadow(
                b,
                I18n.UI_SeedMenu_RerollEveryRun(),
                Game1.smallFont,
                new(
                    doRerollButton.bounds.X + doRerollButton.bounds.Width + 8,
                    doRerollButton.bounds.Y + 4
                ),
                Game1.textColor
            );

            textBox.Draw(b);
            doneButton.draw(b);
            randomButton.draw(b);
            doRerollButton.draw(b);

            drawMouse(b);
        }
    }
}
