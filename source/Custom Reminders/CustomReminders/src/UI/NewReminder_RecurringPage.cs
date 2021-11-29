/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Dem1se/CustomReminders
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;

namespace Dem1se.CustomReminders.UI
{
    public class NewReminder_RecurringPage : IClickableMenu
    {
        readonly int XPos = (int)(Game1.viewport.Width * Game1.options.zoomLevel * (1 / Game1.options.uiScale)) / 2 - (632 + IClickableMenu.borderWidth * 2) / 2;
        readonly int YPos = (int)(Game1.viewport.Height * Game1.options.zoomLevel * (1 / Game1.options.uiScale)) / 2 - (600 + IClickableMenu.borderWidth * 2) / 2 - Game1.tileSize;
        readonly int UIWidth = 632 + IClickableMenu.borderWidth * 2;
        readonly int UIHeight = 600 + IClickableMenu.borderWidth * 2 + Game1.tileSize;

        ClickableTextureComponent OkButton;
        TextBox IntervalTextBox;
        List<ClickableComponent> Labels = new();

        Action<int> OnChanged;

        public NewReminder_RecurringPage(Action<int> onChanged)
        {
            base.initialize(XPos, YPos, UIWidth, UIHeight);
            OnChanged = onChanged;
            SetUpUI();
        }

        public void SetUpUI()
        {
            // Ok button
            OkButton = new ClickableTextureComponent("OK", new Rectangle(xPositionOnScreen + width - IClickableMenu.borderWidth - IClickableMenu.spaceToClearSideBorder - Game1.tileSize, yPositionOnScreen + height - IClickableMenu.borderWidth - IClickableMenu.spaceToClearTopBorder + Game1.tileSize / 4, Game1.tileSize, Game1.tileSize), "", null, Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 46), 1f);

            // TextBox
            IntervalTextBox = new TextBox(null, null, Game1.smallFont, Game1.textColor)
            {
                X = XPos + Game1.tileSize*5,
                Y = YPos + Game1.tileSize*4,
                Width = Game1.tileSize*2,
                Height = 180,
                numbersOnly = true
            };
            Game1.keyboardDispatcher.Subscriber = IntervalTextBox;


            // Interval Label
            Labels.Add(new ClickableComponent(
                new Rectangle(xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + Game1.tileSize * 1 + 4, 
                    yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder - Game1.tileSize / 8 + 8 + Game1.tileSize*2, 
                    1, 
                    1), 
                Utilities.Globals.Helper.Translation.Get("new-reminder.interval-label")
                )
            );

            // Explaination Label
            Labels.Add(new ClickableComponent(new Rectangle(xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + 4, yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder - Game1.tileSize / 8 + 8 + Game1.tileSize * 5, 1, 1), Utilities.Globals.Helper.Translation.Get("new-reminder.interval-explaination")));
        }

        private bool IsOkButtonReady()
        {
            if (string.IsNullOrEmpty(IntervalTextBox.Text))
                return false;

            if (Convert.ToInt32(IntervalTextBox.Text) <= 0)
                return false;

            return true;
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            IntervalTextBox.Update();

            if (OkButton.containsPoint(x, y) && IsOkButtonReady())
            {
                OnChanged(Convert.ToInt32(IntervalTextBox.Text));
                OkButton.scale -= 0.25f;
                OkButton.scale = Math.Max(0.75f, OkButton.scale);
            }
            
            IntervalTextBox.Update();
        }

        public override void performHoverAction(int x, int y)
        {
            OkButton.scale = OkButton.containsPoint(x, y) && IsOkButtonReady()
                ? Math.Min(OkButton.scale + 0.02f, OkButton.baseScale + 0.1f)
                : Math.Max(OkButton.scale - 0.02f, OkButton.baseScale);
        }

        public override void draw(SpriteBatch b)
        {
            // supress the Menu button
            Utilities.Globals.Helper.Input.Suppress(Utilities.Globals.MenuButton);

            // draw screen fade
            b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.75f);

            // draw menu box
            Game1.drawDialogueBox(xPositionOnScreen, yPositionOnScreen, width, height, false, true);

            // draw textbox
            IntervalTextBox.Draw(b, false);

            // draw OK button
            if (IsOkButtonReady())
                OkButton.draw(b);
            else
            {
                OkButton.draw(b);
                OkButton.draw(b, Color.Black * 0.5f, 0.97f);
            }

            foreach (ClickableComponent label in Labels)
            {
                if (label.name == Utilities.Globals.Helper.Translation.Get("new-reminder.interval-label"))
                    Utility.drawTextWithShadow(b, label.name, Game1.dialogueFont, new Vector2(label.bounds.X, label.bounds.Y), Color.Black);
                else
                    Utility.drawTextWithShadow(b, label.name, Game1.smallFont, new Vector2(label.bounds.X, label.bounds.Y), Color.Black);
            }

            // draw cursor
            drawMouse(b);
        }
    }
}