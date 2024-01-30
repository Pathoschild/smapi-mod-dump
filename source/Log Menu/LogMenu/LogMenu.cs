/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/jaredtjahjadi/LogMenu
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Menus;
using System;
using System.Collections.Generic;

namespace LogMenu
{
    internal class LogMenu : IClickableMenu
    {
        private int linesPerPage = 4; // Number of dialogue lines visible in menu without having to scroll
        private List<DialogueElement> dialogueList = new(); // Full dialogue list
        private List<ClickableComponent> dialogueSlots = new(); // Visible dialogue lines on screen
        private int currentItemIndex; // Current position in log menu

        // Scrollbar elements
        public ClickableTextureComponent upArrow;
        public ClickableTextureComponent downArrow;
        private ClickableTextureComponent scrollBar;
        private Rectangle scrollBarRunner;

        public LogMenu(DialogueQueue<DialogueElement> dialogues, bool recentMessagesFirst, bool OldestToNewest)
        {
            foreach (DialogueElement dialogue in dialogues)
            {
                if (OldestToNewest) dialogueList.Add(dialogue);
                else dialogueList.Insert(0, dialogue);
            }
            // If more than 4 dialogue lines, sets item index to either the first or last dialogue line, depending on the recentMessagesFirst config option
            currentItemIndex = recentMessagesFirst ? (dialogueList.Count > linesPerPage ? dialogueList.Count - linesPerPage : 0) : 0;
            //currentItemIndex = dialogueList.Count > linesPerPage ? dialogueList.Count - linesPerPage : 0;
            SetPositions();
        }

        private void DownArrowPressed()
        {
            downArrow.scale = downArrow.baseScale;
            currentItemIndex++;
            SetScrollBarToCurrentIndex();
        }

        private void UpArrowPressed()
        {
            upArrow.scale = upArrow.baseScale;
            currentItemIndex--;
            SetScrollBarToCurrentIndex();
        }

        // Changes position of scrollbar
        private void SetScrollBarToCurrentIndex()
        {
            if (dialogueList.Count <= 0) return;
            scrollBar.bounds.Y = scrollBarRunner.Height / Math.Max(1, dialogueList.Count - linesPerPage + 1) * currentItemIndex + upArrow.bounds.Bottom + Game1.pixelZoom;
            if (currentItemIndex != dialogueList.Count - linesPerPage) return;
            scrollBar.bounds.Y = downArrow.bounds.Y - scrollBar.bounds.Height - Game1.pixelZoom;
        }

        public override void receiveScrollWheelAction(int direction)
        {
            if (GameMenu.forcePreventClose) return;
            base.receiveScrollWheelAction(direction);
            if (direction > 0 && currentItemIndex > 0) // Scroll upwards
            {
                UpArrowPressed();
                Game1.playSound("shiny4");
            }
            else // Scroll downwards
            {
                if (direction >= 0 || currentItemIndex >= Math.Max(0, dialogueList.Count - linesPerPage)) return;
                DownArrowPressed();
                Game1.playSound("shiny4");
            }
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            if (GameMenu.forcePreventClose) return;
            if (downArrow.containsPoint(x, y) && currentItemIndex < Math.Max(0, dialogueList.Count - linesPerPage))
            {
                DownArrowPressed();
                Game1.playSound("shwip");
            }
            else if(upArrow.containsPoint(x, y) && currentItemIndex > 0)
            {
                UpArrowPressed();
                Game1.playSound("shwip");
            }

        }

        public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds) { SetPositions(); }

        // Draw (display) menu elements to screen
        public override void draw(SpriteBatch b)
        {
            // Displays background or uses fade-to-black rectangle against gameplay, depending on in-game Menu Backgrounds option
            if (Game1.options.showMenuBackground) drawBackground(b);
            else b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.4f);

            // Menu box
            Game1.drawDialogueBox(xPositionOnScreen, yPositionOnScreen, width, height, false, true);

            // Dialogue text
            // If no dialogue lines have been logged as of yet, display a message at the center of the screen
            if(dialogueList.Count == 0) SpriteText.drawStringHorizontallyCenteredAt(b, "No Dialogue Lines", Game1.uiViewport.Width / 2, Game1.uiViewport.Height / 2);
            for(int i = 0; i < dialogueSlots.Count; ++i)
            {
                if (currentItemIndex >= 0 && currentItemIndex + i < dialogueList.Count)
                {
                    // Calls draw method of DialogueElement
                    dialogueList[currentItemIndex + i].draw(b, dialogueSlots[i].bounds.X, dialogueSlots[i].bounds.Y + Game1.tileSize / 4);

                    // Draws the character portrait aligned to the right of the menu
                    ClickableComponent button = dialogueSlots[i];
                    DialogueElement currElem = dialogueList[currentItemIndex + i];
                    if (currElem.charDiag is not null && Game1.options.showPortraits)
                    {
                        // Portrait background (from LooseSprites\Cursors)
                        b.Draw(Game1.mouseCursors,
                            new Vector2(
                                button.bounds.X + button.bounds.Width - 128 - borderWidth / 2 - Game1.pixelZoom * 2 + 1,
                                button.bounds.Y + button.bounds.Height / 2 - 64 + Game1.pixelZoom
                            ),
                            new Rectangle(603, 414, 74, 74), Color.White, 0f, Vector2.Zero,
                            height / button.bounds.Height / 2.13f, SpriteEffects.None, 0.88f
                        );

                        // Character portrait
                        Dialogue currCharDiag = currElem.charDiag;
                        Texture2D portraitTexture = currCharDiag.overridePortrait ?? currCharDiag.speaker.Portrait;
                        Rectangle portraitSource = Game1.getSourceRectForStandardTileSheet(portraitTexture, currElem.portraitIndex, 64, 64);
                        if (!portraitTexture.Bounds.Contains(portraitSource)) portraitSource = new Rectangle(0, 0, 64, 64);
                        b.Draw(
                            portraitTexture,
                            new Vector2(
                                button.bounds.X + button.bounds.Width - 128 - borderWidth / 2 + 2,
                                button.bounds.Y + button.bounds.Height / 2 - 64 + Game1.pixelZoom * 3 + 1
                            ),
                            portraitSource, Color.White, 0f, Vector2.Zero, height / button.bounds.Height / 2.12f, SpriteEffects.None, 0.88f);
                    }
                }
            }

            // Scrollbar
            if(!GameMenu.forcePreventClose)
            {
                upArrow.draw(b);
                downArrow.draw(b);
                if (dialogueList.Count > linesPerPage)
                {
                    drawTextureBox(
                        b, Game1.mouseCursors, new Rectangle(403, 383, 6, 6),
                        scrollBarRunner.X, scrollBarRunner.Y, scrollBarRunner.Width, scrollBarRunner.Height,
                        Color.White, Game1.pixelZoom, false
                    );
                    scrollBar.draw(b);
                }
            }

            drawMouse(b);
        }

        private void SetPositions()
        {
            // Menu dimensions (1000x600, + border width)
            width = 1000 + borderWidth * 2;
            height = 600 + borderWidth * 2;

            // Starting position of menu (top-left x/y coords)
            xPositionOnScreen = (Game1.uiViewport.Width - width) / 2;
            yPositionOnScreen = (Game1.uiViewport.Height - height) / 2;

            dialogueSlots.Clear();
            for (int i = 0; i < linesPerPage; ++i)
            {
                dialogueSlots.Add(new ClickableComponent(
                    new Rectangle(
                        xPositionOnScreen + Game1.tileSize / 4,
                        yPositionOnScreen + Game1.tileSize * 5 / 4 + Game1.pixelZoom + i * (height - Game1.tileSize * 2) / linesPerPage,
                        width - Game1.tileSize / 2,
                        (height - Game1.tileSize * 2) / linesPerPage + Game1.pixelZoom
                    ),
                    i.ToString()
                ));
            }

            // Scrollbar components
            int spaceBtwnMenuAndArrows = xPositionOnScreen + width + Game1.tileSize / 4;
            // Grabs up arrow, down arrow, and scrollbar sprites from Game1.mouseCursors (<Stardew Valley direectory>\Content\LooseSprites\Cursors)
            upArrow = new ClickableTextureComponent(
                new Rectangle(
                    spaceBtwnMenuAndArrows, yPositionOnScreen + Game1.tileSize,
                    11 * Game1.pixelZoom, 12 * Game1.pixelZoom
                ),
                Game1.mouseCursors, new Rectangle(421, 459, 11, 12), Game1.pixelZoom);
            downArrow = new ClickableTextureComponent(
                new Rectangle(
                    spaceBtwnMenuAndArrows, yPositionOnScreen + height - Game1.tileSize,
                    11 * Game1.pixelZoom, 12 * Game1.pixelZoom
                ),
                Game1.mouseCursors,
                new Rectangle(421, 472, 11, 12),
                Game1.pixelZoom);
            scrollBar = new ClickableTextureComponent(
                new Rectangle(
                    upArrow.bounds.X + Game1.pixelZoom * 3, upArrow.bounds.Y + upArrow.bounds.Height + Game1.pixelZoom,
                    6 * Game1.pixelZoom, 10 * Game1.pixelZoom
                ),
                Game1.mouseCursors,
                new Rectangle(435, 463, 6, 10),
                Game1.pixelZoom
            );
            scrollBarRunner = new Rectangle(
                scrollBar.bounds.X,
                upArrow.bounds.Y + upArrow.bounds.Height + Game1.pixelZoom,
                scrollBar.bounds.Width,
                height - Game1.tileSize * 2 - upArrow.bounds.Height - Game1.pixelZoom * 2
            );
            SetScrollBarToCurrentIndex();
        }
    }
}