/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tylergibbs2/StardewValleyRoguelike
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
    public class StatsMenu : IClickableMenu
    {
        private readonly float percentOfViewportWide = 0.33f;
        private readonly float percentOfViewportTall = 0.66f;

        private readonly int borderSize = 60 / 3;

        private readonly int statLinePadding = 6;

        private readonly Rectangle horizontalLineRectangle = new(0, 256, 60, 60);

        private float uploadScale = 1f;
        private float uploadBaseScale = 1f;

        private Color uploadButtonColor = Game1.textColor;
        private ClickableComponent uploadButton;
        private Vector2 textSize;

        private bool AlreadyUploaded = false;

        private int currentDrawHeight;

        public StatsMenu()
        {
            string uploadText = "Upload";
            textSize = Game1.smallFont.MeasureString(uploadText);
            uploadButton = new(new(0, 0, (int)textSize.X, (int)textSize.Y), "uploadButton", uploadText)
            {
                myID = 101
            };

            upperRightCloseButton = new(new(xPositionOnScreen + width - 36, yPositionOnScreen - 8, 48, 48), Game1.mouseCursors, new Rectangle(337, 494, 12, 12), 4f)
            {
                myID = 102
            };

            CalculatePosition();

            populateClickableComponentList();

            if (Game1.options.gamepadControls)
                snapToDefaultClickableComponent();
        }

        public override void snapToDefaultClickableComponent()
        {
            currentlySnappedComponent = uploadButton;
            snapCursorToCurrentSnappedComponent();
        }

        public override void receiveKeyPress(Keys key)
        {
            Keys menuKey = Game1.options.getFirstKeyboardKeyFromInputButtonList(Game1.options.menuButton);
            Keys journalKey = Game1.options.getFirstKeyboardKeyFromInputButtonList(Game1.options.journalButton);
            if (key == menuKey || key == journalKey)
                Game1.exitActiveMenu();
        }

        public override void receiveGamePadButton(Buttons b)
        {
            base.receiveGamePadButton(b);

            if (currentlySnappedComponent is null)
                snapToDefaultClickableComponent();

            Keys key = Utility.mapGamePadButtonToKey(b);
            receiveKeyPress(key);
        }

        public static void Show()
        {
            Game1.activeClickableMenu = new StatsMenu();
        }

        private void CalculatePosition()
        {
            width = (int)(Game1.uiViewport.Width * percentOfViewportWide);
            height = (int)(Game1.uiViewport.Height * percentOfViewportTall);

            xPositionOnScreen = Game1.uiViewport.Width / 2 - width / 2;
            yPositionOnScreen = 100.ToUIScale();

            uploadButton.bounds = new(0, 0, (int)textSize.X, (int)textSize.Y);
            upperRightCloseButton.bounds = new(xPositionOnScreen + width - 36, yPositionOnScreen - 8, 48, 48);
        }

        public override void performHoverAction(int x, int y)
        {
            if (uploadButton.containsPoint(x, y))
            {
                uploadScale = Math.Min(uploadScale + 0.04f, uploadBaseScale + 0.25f);
                uploadButtonColor = Game1.unselectedOptionColor;
            }
            else
            {
                uploadScale = Math.Max(uploadScale - 0.04f, uploadBaseScale);
                uploadButtonColor = Game1.textColor;
            }
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            base.receiveLeftClick(x, y, playSound);

            if (uploadButton is not null && uploadButton.containsPoint(x, y) && ModEntry.Stats.EndTime is not null)
            {
                Game1.playSound("bigSelect");
                if (ModEntry.GetInvalidMods().Count > 0 || ModEntry.DisableUpload)
                {
                    Game1.addHUDMessage(new HUDMessage("Cannot upload due to mods.", 3));
                    return;
                }
                if (!Context.IsMainPlayer)
                {
                    Game1.addHUDMessage(new HUDMessage("Only the host can upload stats in multiplayer.", 3));
                    return;
                }
                else if (ModEntry.Stats.EndTime is null)
                {
                    Game1.addHUDMessage(new HUDMessage("Cannot upload mid-run.", 3));
                    return;
                }
                else if (AlreadyUploaded)
                {
                    Game1.addHUDMessage(new HUDMessage("Run has already been uploaded.", 3));
                    return;
                }
                else if (ModEntry.Stats.StartTime is null)
                {
                    Game1.addHUDMessage(new HUDMessage("Run has not started.", 3));
                    return;
                }

                bool result = ModEntry.Stats.Upload();
                if (result)
                {
                    AlreadyUploaded = true;
                    Game1.addHUDMessage(new HUDMessage("Run successfully uploaded.", 1));
                }
                else
                    Game1.addHUDMessage(new HUDMessage("There was an error uploading.", 3));
            }
        }

        public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
        {
            CalculatePosition();
        }

        public void DrawHorizontalLine(SpriteBatch spriteBatch, int y)
        {
            drawTextureBox(
                spriteBatch,
                Game1.menuTexture,
                horizontalLineRectangle,
                xPositionOnScreen,
                yPositionOnScreen - borderSize + y,
                width,
                borderSize,
                Color.White,
                drawShadow: false
            );

            currentDrawHeight += borderSize;
        }

        public void DrawTitle(SpriteBatch spriteBatch)
        {
            Vector2 textSize = Game1.dialogueFont.MeasureString("Statistics");

            Utility.drawTextWithShadow(
                spriteBatch,
                "Statistics",
                Game1.dialogueFont,
                new Vector2(
                    xPositionOnScreen + (width / 2) - (textSize.X / 2),
                    currentDrawHeight + borderSize
                ),
                Color.Black
            );

            currentDrawHeight += (int)textSize.Y;
        }

        public void DrawStats(SpriteBatch spriteBatch)
        {
            Vector2 textSize = Vector2.Zero;
            foreach (string line in ModEntry.Stats.GetLines())
            {
                textSize = Game1.smallFont.MeasureString(line);

                string label = line.Split(":")[0];
                string value = line.Split(":")[1];

                Utility.drawTextWithShadow(
                    spriteBatch,
                    label,
                    Game1.smallFont,
                    new Vector2(
                        xPositionOnScreen + borderSize,
                        currentDrawHeight + borderSize + statLinePadding
                    ),
                    Color.Black
                );

                int valueWidth = (int)Game1.smallFont.MeasureString(value).X;

                Utility.drawTextWithShadow(
                    spriteBatch,
                    value,
                    Game1.smallFont,
                    new Vector2(
                        xPositionOnScreen + width - (borderSize + valueWidth),
                        currentDrawHeight + borderSize + statLinePadding
                    ),
                    Color.Black
                );

                currentDrawHeight += (int)textSize.Y + statLinePadding;
            }

            currentDrawHeight -= (int)textSize.Y + statLinePadding;
        }

        public void DrawUploadButton(SpriteBatch spriteBatch)
        {
            uploadButton.bounds.X = xPositionOnScreen + (width / 2) - (uploadButton.bounds.Width / 2);
            uploadButton.bounds.Y = currentDrawHeight + uploadButton.bounds.Height + statLinePadding + borderSize + 16;

            drawTextureBox(
                spriteBatch,
                Game1.menuTexture,
                new Rectangle(0, 256, 60, 60),
                xPositionOnScreen + (width / 2) - (uploadButton.bounds.Width / 2) - 16,
                currentDrawHeight + uploadButton.bounds.Height + statLinePadding + borderSize,
                uploadButton.bounds.Width + 32,
                uploadButton.bounds.Height + 24,
                Color.White,
                drawShadow: false,
                scale: uploadScale
            );

            Utility.drawBoldText(
                spriteBatch,
                uploadButton.label,
                Game1.smallFont,
                new Vector2(
                    xPositionOnScreen + (width / 2) - (uploadButton.bounds.Width / 2),
                    currentDrawHeight + uploadButton.bounds.Height + statLinePadding + borderSize + 16
                ),
                uploadButtonColor
            );

            currentDrawHeight += uploadButton.bounds.Height + statLinePadding + borderSize;
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
            DrawStats(spriteBatch);

            if (ModEntry.Stats.EndTime is not null)
                DrawUploadButton(spriteBatch);

            height = currentDrawHeight;

            upperRightCloseButton.draw(spriteBatch);

            drawMouse(spriteBatch);
        }
    }
}
