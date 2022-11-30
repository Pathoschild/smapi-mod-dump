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
using System.Collections.Generic;
using System.Linq;

namespace StardewRoguelike.UI
{
    public class DisclaimerMenu : IClickableMenu
    {
        private readonly float percentOfViewportWide = 0.5f;
        private readonly float percentOfViewportTall = 0.66f;

        private readonly int borderSize = 20;

        private readonly int modsToShow = 5;

        private float understandScale = 1f;
        private float understandBaseScale = 1f;

        private Vector2 textSize;

        private Color understandButtonColor = Game1.textColor;
        private ClickableComponent understandButton;

        private List<IModInfo> invalidMods = ModEntry.GetInvalidMods();

        private int currentDrawHeight;

        public DisclaimerMenu()
        {
            string understandText = I18n.UI_DisclaimerMenu_Understand();
            textSize = Game1.smallFont.MeasureString(understandText);
            understandButton = new(new(0, 0, (int)textSize.X, (int)textSize.Y), "understandButton", understandText)
            {
                myID = 101
            };

            CalculatePosition();

            populateClickableComponentList();
        }

        private void CalculatePosition()
        {
            width = (int)(Game1.uiViewport.Width * percentOfViewportWide);
            height = (int)(Game1.uiViewport.Height * percentOfViewportTall);

            xPositionOnScreen = Game1.uiViewport.Width / 2 - width / 2;
            yPositionOnScreen = 100.ToUIScale();

            understandButton.bounds = new(0, 0, (int)textSize.X, (int)textSize.Y);
        }

        public override void snapToDefaultClickableComponent()
        {
            currentlySnappedComponent = understandButton;
            snapCursorToCurrentSnappedComponent();
        }

        public override void update(GameTime time)
        {
            base.update(time);

            if (Game1.options.gamepadControls)
                snapToDefaultClickableComponent();
        }

        public override void performHoverAction(int x, int y)
        {
            if (understandButton.containsPoint(x, y))
            {
                understandScale = Math.Min(understandScale + 0.04f, understandBaseScale + 0.25f);
                understandButtonColor = Game1.unselectedOptionColor;
            }
            else
            {
                understandScale = Math.Max(understandScale - 0.04f, understandBaseScale);
                understandButtonColor = Game1.textColor;
            }
        }

        public override bool readyToClose()
        {
            return false;
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            base.receiveLeftClick(x, y, playSound);

            if (understandButton is not null && understandButton.containsPoint(x, y))
            {
                Game1.playSound("bigSelect");
                exitThisMenu(playSound: false);
            }
        }

        public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
        {
            CalculatePosition();
        }

        public void DrawTitle(SpriteBatch spriteBatch)
        {
            Vector2 textSize = Game1.dialogueFont.MeasureString(I18n.UI_DisclaimerMenu_Title());

            Utility.drawTextWithShadow(
                spriteBatch,
                I18n.UI_DisclaimerMenu_Title(),
                Game1.dialogueFont,
                new Vector2(
                    xPositionOnScreen + (width / 2) - (textSize.X / 2),
                    currentDrawHeight + borderSize
                ),
                Color.Black
            );

            currentDrawHeight += (int)textSize.Y;
        }

        public void DrawWarning(SpriteBatch spriteBatch)
        {
            string parsedText = Game1.parseText(I18n.UI_DisclaimerMenu_Explanation(), Game1.smallFont, width - (borderSize * 2));
            Vector2 textSize = Game1.smallFont.MeasureString(parsedText);

            Utility.drawTextWithShadow(
                spriteBatch,
                parsedText,
                Game1.smallFont,
                new Vector2(
                    xPositionOnScreen + borderSize,
                    currentDrawHeight + borderSize
                ),
                Color.Black
            );

            currentDrawHeight += (int)textSize.Y + 16;

            string illegalModsString;
            string mods = string.Join(", ", invalidMods.Select(m => m.Manifest.Name).Take(modsToShow));
            if (invalidMods.Count - modsToShow > 0)
                illegalModsString = I18n.UI_DisclaimerMenu_IllegalModsSample(modsSample: mods, extraCount: invalidMods.Count - modsToShow);
            else
                illegalModsString = I18n.UI_DisclaimerMenu_IllegalModsAll(mods: mods);

            string parsedMods = Game1.parseText(illegalModsString, Game1.smallFont, width - (borderSize * 2));
            textSize = Game1.smallFont.MeasureString(parsedMods);

            Utility.drawTextWithShadow(
                spriteBatch,
                parsedMods,
                Game1.smallFont,
                new Vector2(
                    xPositionOnScreen + borderSize,
                    currentDrawHeight + borderSize
                ),
                Color.Black
            );

            currentDrawHeight += (int)textSize.Y;
        }

        public void DrawUnderstandButton(SpriteBatch spriteBatch)
        {
            understandButton.bounds.X = xPositionOnScreen + (width / 2) - (understandButton.bounds.Width / 2);
            understandButton.bounds.Y = currentDrawHeight + understandButton.bounds.Height + 16;

            drawTextureBox(
                spriteBatch,
                Game1.menuTexture,
                new Rectangle(0, 256, 60, 60),
                xPositionOnScreen + (width / 2) - (understandButton.bounds.Width / 2) - 16,
                currentDrawHeight + understandButton.bounds.Height,
                understandButton.bounds.Width + 32,
                understandButton.bounds.Height + 24,
                Color.White,
                drawShadow: false,
                scale: understandScale
            );

            Utility.drawBoldText(
                spriteBatch,
                understandButton.label,
                Game1.smallFont,
                new Vector2(
                    xPositionOnScreen + (width / 2) - (understandButton.bounds.Width / 2),
                    currentDrawHeight + understandButton.bounds.Height + 16
                ),
                understandButtonColor
            );

            currentDrawHeight += understandButton.bounds.Height;
        }

        public override void draw(SpriteBatch spriteBatch)
        {
            currentDrawHeight = yPositionOnScreen;

            drawTextureBox(
                spriteBatch,
                Game1.menuTexture,
                new Rectangle(0, 256, 60, 60),
                xPositionOnScreen,
                yPositionOnScreen,
                width,
                height,
                Color.White,
                drawShadow: true
            );

            DrawTitle(spriteBatch);
            DrawWarning(spriteBatch);
            DrawUnderstandButton(spriteBatch);
            height = currentDrawHeight;

            drawMouse(spriteBatch);
        }
    }
}
