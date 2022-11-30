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
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewRoguelike.Extensions;
using StardewValley;
using StardewValley.Menus;
using System;

namespace StardewRoguelike.UI
{
    internal class PerkDisplayMenu : IClickableMenu
    {
        private int currentDrawHeight;

        private int scrollOffset = 0;

        private int titleDrawHeight;

        private int highestDrawHeight;

        private int perksOnScreen;

        private ClickableTextureComponent upArrow;

        private ClickableTextureComponent downArrow;

        private static readonly Rectangle horizontalLineRectangle = new(0, 256, 60, 60);

        public PerkDisplayMenu() : base(0, 0, 0, 0)
        {
            CalculatePosition();
        }

        public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
        {
            CalculatePosition();
        }

        public override void receiveKeyPress(Keys key)
        {
            if ((key == Keys.Escape || Game1.options.doesInputListContain(Game1.options.journalButton, key)) && readyToClose())
            {
                Game1.exitActiveMenu();
                Game1.playSound("bigDeSelect");
            }
        }

        private bool CanScrollUp()
        {
            return PerksHiddenAboveScroll() > 0;
        }

        private bool CanScrollDown()
        {
            return PerksHiddenAboveScroll() + perksOnScreen < Perks.GetActivePerks().Count;
        }

        private int PerksHiddenAboveScroll()
        {
            return Math.Abs(scrollOffset) / 100;
        }

        public override void receiveScrollWheelAction(int direction)
        {
            if (direction > 0 && CanScrollUp())
            {
                scrollOffset += 100;
                Game1.playSound("shiny4");
            }
            else if (direction < 0 && CanScrollDown())
            {
                scrollOffset -= 100;
                Game1.playSound("shiny4");
            }
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            base.receiveLeftClick(x, y, playSound);
            if (upArrow.containsPoint(x, y) && CanScrollUp())
            {
                scrollOffset += 100;
                Game1.playSound("shiny4");
                upArrow.scale = upArrow.baseScale;
            }
            else if (downArrow.containsPoint(x, y) && CanScrollDown())
            {
                scrollOffset -= 100;
                Game1.playSound("shiny4");
                downArrow.scale = downArrow.baseScale;
            }
        }

        public override void performHoverAction(int x, int y)
        {
            base.performHoverAction(x, y);
            upArrow.tryHover(x, y);
            downArrow.tryHover(x, y);
        }

        private void CalculatePosition()
        {
            width = (int)(Game1.uiViewport.Width * 0.66f);

            xPositionOnScreen = Game1.uiViewport.Width / 2 - width / 2;
            yPositionOnScreen = 100.ToUIScale();

            height = 808;
            while (height + yPositionOnScreen > Game1.uiViewport.Height)
                height -= 100;

            upArrow = new(new Rectangle(xPositionOnScreen + width - 64, yPositionOnScreen + 105, 44, 48), Game1.mouseCursors, new Rectangle(421, 459, 11, 12), 4f);
            downArrow = new(new Rectangle(xPositionOnScreen + width - 64, yPositionOnScreen + height - 64, 44, 48), Game1.mouseCursors, new Rectangle(421, 472, 11, 12), 4f);
        }

        public void DrawTitle(SpriteBatch b)
        {
            Vector2 textSize = Game1.dialogueFont.MeasureString(I18n.UI_PerkDisplayMenu_Title());

            Utility.drawTextWithShadow(
                b,
                I18n.UI_PerkDisplayMenu_Title(),
                Game1.dialogueFont,
                new Vector2(
                    xPositionOnScreen + (width / 2) - (textSize.X / 2),
                    currentDrawHeight + spaceToClearSideBorder * 2
                ),
                Color.Black
            );

            DrawHorizontalLine(b, currentDrawHeight + 24);

            currentDrawHeight += (int)textSize.Y + 32 + spaceToClearSideBorder * 2;
            titleDrawHeight = currentDrawHeight;
        }

        public void DrawNoPerksText(SpriteBatch b)
        {
            string noPerks = I18n.UI_PerkDisplayMenu_NoPerks();
            b.DrawString(Game1.smallFont, noPerks, new Vector2(xPositionOnScreen + width / 2 - Game1.smallFont.MeasureString(noPerks).X / 2f, yPositionOnScreen + 200 + spaceToClearTopBorder), Game1.textColor);
        }

        public void DrawHorizontalLine(SpriteBatch b, int y)
        {
            drawTextureBox(
                b,
                Game1.menuTexture,
                horizontalLineRectangle,
                xPositionOnScreen,
                yPositionOnScreen - spaceToClearSideBorder + y,
                width,
                spaceToClearSideBorder,
                Color.White,
                drawShadow: false
            );
        }

        private bool ShouldDraw(int yLevel)
        {
            return yLevel >= titleDrawHeight && yLevel < height;
        }

        public void DrawScrollArrows(SpriteBatch b)
        {
            if (CanScrollUp())
                upArrow.draw(b);
            if (CanScrollDown())
                downArrow.draw(b);
        }

        public void DrawPerk(SpriteBatch b, Perks.PerkType perkType)
        {
            Perks.PerkRarity rarity = Perks.GetPerkRarity(perkType);
            Color rarityColor = PerkMenu.GetRarityColor(rarity);
            Rectangle iconSourceRect = Perks.GetPerkSourceRect(perkType);

            string perkName = Perks.GetPerkDisplayName(perkType);
            string perkDescription = Perks.GetPerkDescription(perkType);
            string rarityText = PerkMenu.GetRarityText(rarity);

            Vector2 nameSize = Game1.dialogueFont.MeasureString(perkName);

            if (ShouldDraw(currentDrawHeight + scrollOffset))
            {
                perksOnScreen++;

                b.Draw(
                    Game1.mouseCursors,
                    new Vector2(xPositionOnScreen + spaceToClearSideBorder * 2, (currentDrawHeight + scrollOffset)),
                    iconSourceRect,
                    Color.White,
                    0f,
                    Vector2.Zero,
                    4f,
                    SpriteEffects.None,
                    1f
                );

                b.DrawString(
                    Game1.dialogueFont,
                    perkName,
                    new Vector2(xPositionOnScreen + 75 + spaceToClearSideBorder * 2, (currentDrawHeight + scrollOffset) - 8),
                    Game1.textColor
                );

                b.DrawString(
                    Game1.dialogueFont,
                    rarityText,
                    new Vector2(xPositionOnScreen + 100 + (int)nameSize.X + spaceToClearSideBorder * 2, (currentDrawHeight + scrollOffset) - 8),
                    rarityColor
                );

                b.DrawString(
                    Game1.smallFont,
                    Game1.parseText(perkDescription, Game1.smallFont, width - 48),
                    new Vector2(xPositionOnScreen + 75 + spaceToClearSideBorder * 2, (currentDrawHeight + scrollOffset) + 36),
                    Game1.textColor
                );

                bool isLastOnScreen = !ShouldDraw(currentDrawHeight + scrollOffset + 100);

                if (!isLastOnScreen)
                    DrawHorizontalLine(b, (currentDrawHeight + scrollOffset) + 12);
            }

            currentDrawHeight += 100;

            if (currentDrawHeight > highestDrawHeight)
                highestDrawHeight = currentDrawHeight;
        }

        public override void draw(SpriteBatch b)
        {
            currentDrawHeight = yPositionOnScreen;
            perksOnScreen = 0;

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

            DrawTitle(b);

            if (Perks.GetActivePerks().Count == 0)
                DrawNoPerksText(b);
            else
            {
                foreach (Perks.PerkType perkType in Perks.GetActivePerks())
                    DrawPerk(b, perkType);

                DrawScrollArrows(b);
            }

            drawMouse(b);
        }
    }


}
