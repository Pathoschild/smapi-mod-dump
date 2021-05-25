/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://sourceforge.net/projects/sdvmod-remind-to-exit/
**
*************************************************/

/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using StardewModdingAPI;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.BellsAndWhistles;
using Microsoft.Xna.Framework.Input;

namespace RemindToExit
    {
    class ReminderMessageUI : IClickableMenu
        {
        private readonly int XPos = (int)(Game1.viewport.Width * Game1.options.zoomLevel * (1 / Game1.options.uiScale)) / 2 - (632 + IClickableMenu.borderWidth * 2) / 2;
        private readonly int YPos = (int)(Game1.viewport.Height * Game1.options.zoomLevel * (1 / Game1.options.uiScale)) / 2 - (600 + IClickableMenu.borderWidth * 2) / 2 - Game1.tileSize;
        private readonly int UIWidth = 800 + IClickableMenu.borderWidth * 2;
        private readonly int UIHeight = 300 + IClickableMenu.borderWidth * 2 + Game1.tileSize;

        private readonly List<ClickableComponent> Labels = new List<ClickableComponent>();

        private TextBox ReminderTextBox;
        //private TextEntryMenu ReminderTextEntry;
        protected List<ClickableTextureComponent> Buttons = new List<ClickableTextureComponent>();
        protected ClickableTextureComponent OkButton;
        protected ClickableTextureComponent CancelButton;
        protected ClickableTextureComponent ClearButton;

        private readonly IModHelper Helper;
        private readonly Action<string, string> OnClosing;
        private readonly string InitialText;
        private readonly SButton MenuButton;

        private readonly string txtCustomReminderMsg;

        public ReminderMessageUI(IModHelper helper, string initial, SButton menuButton, Action<string, string> onCLosing) {
            Helper = helper;
            InitialText = initial ?? "";
            MenuButton = menuButton;
            OnClosing = onCLosing;

            txtCustomReminderMsg = Helper.Translation.Get("reminder-ui.custom-msg-label");

            base.initialize(XPos, YPos, UIWidth, UIHeight);
            SetupUI();
            }

        private void SetupUI() {
            Labels.Clear();

            OkButton = new ClickableTextureComponent("OK",
                new Rectangle(
                    xPositionOnScreen + width - IClickableMenu.borderWidth - IClickableMenu.spaceToClearSideBorder - 2 * Game1.tileSize,
                    yPositionOnScreen + height - IClickableMenu.borderWidth - IClickableMenu.spaceToClearTopBorder + Game1.tileSize / 4,
                    Game1.tileSize,
                    Game1.tileSize
                    ),
                "", "Ok & Activate", Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 46), 1f
                );
            Buttons.Add(OkButton);

            CancelButton = new ClickableTextureComponent("Cancel",
                new Rectangle(
                    xPositionOnScreen + width - IClickableMenu.borderWidth - IClickableMenu.spaceToClearSideBorder - ((7 * Game1.tileSize) >> 1),
                    yPositionOnScreen + height - IClickableMenu.borderWidth - IClickableMenu.spaceToClearTopBorder + Game1.tileSize / 4,
                    Game1.tileSize,
                    Game1.tileSize
                    ),
                "", "Cancel", Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 47), 1f
                );
            Buttons.Add(CancelButton);

            ClearButton = new ClickableTextureComponent("Clear",
                new Rectangle(
                    xPositionOnScreen + borderWidth + spaceToClearSideBorder + Game1.tileSize,
                    yPositionOnScreen + height - IClickableMenu.borderWidth - IClickableMenu.spaceToClearTopBorder + Game1.tileSize / 4,
                    Game1.tileSize,
                    Game1.tileSize
                    ),
                "", "Clear", Game1.mouseCursors2, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors2, 213, 16, 16), 4.0f
                );
            Buttons.Add(ClearButton);

            Labels.Add(new ClickableComponent(
                new Rectangle(
                    xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + Game1.tileSize * 1 + 4,
                    yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder - Game1.tileSize / 8 + 8,
                    1,
                    1),
                txtCustomReminderMsg
                ));

            ReminderTextBox = new TextBox(null, null, Game1.smallFont, Game1.textColor) {
                X = xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + Game1.tileSize * 1,
                Y = yPositionOnScreen + IClickableMenu.spaceToClearTopBorder + IClickableMenu.borderWidth - Game1.tileSize / 4 + Game1.tileSize / 2 + Game1.tileSize,
                Width = width - IClickableMenu.borderWidth - IClickableMenu.spaceToClearSideBorder - Game1.tileSize * 3 - Game1.tileSize / 4,
                Height = 180,
                Text = InitialText,
                textLimit = 999,
                limitWidth = false
                };
            Game1.keyboardDispatcher.Subscriber = ReminderTextBox;

            }

        /// <summary>Handle a button click.</summary>
        /// <param name="name">The button name that was clicked.</param>
        private void HandleButtonClick(string name) {
            switch (name) {
                case null:
                    break;
                case "Clear":
                    Game1.playSound("stoneStep");
                    ReminderTextBox.Text = "";
                    ReminderTextBox.Update();
                    ClearButton.scale = 4.0f;
                    break;
                case "Cancel":
                    CloseCancel();
                    break;
                case "OK":
                    CloseOK();
                    break;
                }
            }

        private void CloseCancel() {
            OnClosing("cancel", null);
            Game1.playSound("stoneStep");
            }

        private void CloseOK() {
            OnClosing("ok", ReminderTextBox.Text);
            Game1.playSound("coin");
            }

        /// <summary>The method invoked when the player left-clicks on the menu.</summary>
        /// <param name="x">The X-position of the cursor.</param>
        /// <param name="y">The Y-position of the cursor.</param>
        /// <param name="playSound">Whether to enable sound.</param>
        public override void receiveLeftClick(int x, int y, bool playSound = true) {
            ReminderTextBox.Update();
            foreach (var button in Buttons) if (button.containsPoint(x, y)) {
                    button.scale *= 0.75f;
                    button.scale = Math.Max(0.75f, button.scale);
                    HandleButtonClick(button.name);
                    }
            }

        public override void receiveKeyPress(Keys key) {
            Helper.Input.Suppress(MenuButton);
            base.receiveKeyPress(key);
            switch (key) {
                case Keys.Enter:
                    CloseOK();
                    break;
                case Keys.Escape:
                    CloseCancel();
                    break;
                }
            }

        /// <summary>Draw the menu to the screen.</summary>
        /// <param name="b">The sprite batch.</param>
        public override void draw(SpriteBatch b) {
            Helper.Input.Suppress(MenuButton);

            // screen fade
            b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.75f);

            // menu box
            Game1.drawDialogueBox(xPositionOnScreen, yPositionOnScreen, width, height, false, true);

            // draw title scroll
            SpriteText.drawStringWithScrollCenteredAt(
                b,
                "Remind to Exit",
                Game1.options.uiScale <= 1.25f ? XPos + (UIWidth / 2) : XPos - Game1.tileSize * 2 - 32,
                Game1.options.uiScale <= 1.25f ? YPos - (Game1.tileSize / 4) : YPos + Game1.tileSize * 3,
                "Remind to Exit"
            );

            // draw textbox
            ReminderTextBox.Draw(b, false);

            // draw labels
            foreach (ClickableComponent label in Labels) {
                if (label.name == txtCustomReminderMsg)
                    Utility.drawTextWithShadow(b, label.name, Game1.dialogueFont, new Vector2(label.bounds.X, label.bounds.Y), Color.Black);
                else
                    Utility.drawTextWithShadow(b, label.name, Game1.smallFont, new Vector2(label.bounds.X, label.bounds.Y), Color.Black);
                }

            // draw buttons
            foreach (var button in Buttons) button.draw(b);

            // draw cursor
            drawMouse(b);

            }

        }
    }
