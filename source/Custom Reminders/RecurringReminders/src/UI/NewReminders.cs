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
using StardewModdingAPI;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Menus;
using System;
using System.Collections.Generic;

namespace Dem1se.RecurringReminders.UI
{
    /// <summary>The menu which lets the set new reminders.</summary>
    internal class NewReminder_Page1 : IClickableMenu
    {
        private readonly List<ClickableComponent> Labels = new List<ClickableComponent>();
        private ClickableTextureComponent DisplayRemindersButton, OkButton;
        private TextBox ReminderMessageTextBox, ReminderIntervalTextBox;

        private string ReminderMessage;
        private int ReminderInterval;

        /// <summary>The callback to invoke when the ok button is pressed</summary>
        private readonly Action<string, int> OnChanged;

        /// <summary>Construct an instance of the first page.</summary>
        /// <param name="onChange"></param>
        public NewReminder_Page1(Action<string, int> onChange)
            : base(Game1.viewport.Width / 2 - (632 + IClickableMenu.borderWidth * 2) / 2, Game1.viewport.Height / 2 - (600 + IClickableMenu.borderWidth * 2) / 2 - Game1.tileSize, 632 + IClickableMenu.borderWidth * 2, 600 + IClickableMenu.borderWidth * 2 + Game1.tileSize)
        {
            OnChanged = onChange;
            SetUpPositions();
        }

        /// <summary>The method called when the game window changes size.</summary>
        /// <param name="oldBounds">The former viewport.</param>
        /// <param name="newBounds">The new viewport.</param>
        public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
        {
            base.gameWindowSizeChanged(oldBounds, newBounds);
            xPositionOnScreen = Game1.viewport.Width / 2 - (632 + IClickableMenu.borderWidth * 2) / 2;
            yPositionOnScreen = Game1.viewport.Height / 2 - (600 + IClickableMenu.borderWidth * 2) / 2 - Game1.tileSize;
            SetUpPositions();
        }


        /// <summary>Regenerate the UI.</summary>
        private void SetUpPositions()
        {
            Labels.Clear();

            OkButton = new ClickableTextureComponent("OK", new Rectangle(xPositionOnScreen + width - IClickableMenu.borderWidth - IClickableMenu.spaceToClearSideBorder - Game1.tileSize, yPositionOnScreen + height - IClickableMenu.borderWidth - IClickableMenu.spaceToClearTopBorder + Game1.tileSize / 4, Game1.tileSize, Game1.tileSize), "", null, Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 46), 1f);
            DisplayRemindersButton = new ClickableTextureComponent(new Rectangle(xPositionOnScreen - Game1.tileSize * 5 - IClickableMenu.spaceToClearSideBorder * 2, yPositionOnScreen + IClickableMenu.spaceToClearTopBorder, Game1.tileSize * 5 + Game1.tileSize / 4 + Game1.tileSize / 8, Game1.tileSize + Game1.tileSize / 8), Utilities.Data.Helper.Content.Load<Texture2D>("assets/DisplayReminders.png", ContentSource.ModFolder), new Rectangle(), 1.5f);
            Labels.Add(new ClickableComponent(new Rectangle(xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + Game1.tileSize * 2 + 4, yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder - Game1.tileSize / 8 + 8, 1, 1), "Reminder Message: "));
            Labels.Add(new ClickableComponent(new Rectangle(xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + Game1.tileSize * 2 + 4 + Game1.tileSize / 2, yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder - Game1.tileSize / 8 + 8 + Game1.tileSize * 3, 1, 1), "Reminder Interval: "));
            Labels.Add(new ClickableComponent(new Rectangle(xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + 4, yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder - Game1.tileSize / 8 + 8 + Game1.tileSize * 6, 1, 1), "Interval 1: everyday, 2: every other day, and so on..."));
            
            ReminderMessageTextBox = new TextBox(null, null, Game1.smallFont, Game1.textColor)
            {
                X = xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + Game1.tileSize * 1,
                Y = yPositionOnScreen + IClickableMenu.spaceToClearTopBorder + IClickableMenu.borderWidth - Game1.tileSize / 4 + Game1.tileSize / 2 + Game1.tileSize,
                Width = width - IClickableMenu.borderWidth - IClickableMenu.spaceToClearSideBorder - Game1.tileSize * 3 - Game1.tileSize / 4,
                Height = 180
            };
            Game1.keyboardDispatcher.Subscriber = ReminderMessageTextBox;

            ReminderIntervalTextBox = new TextBox(null, null, Game1.smallFont, Game1.textColor)
            {
                X = xPositionOnScreen + (this.width / 2) - Game1.tileSize,
                Y = yPositionOnScreen + IClickableMenu.spaceToClearTopBorder + IClickableMenu.borderWidth - Game1.tileSize / 4 + Game1.tileSize / 2 + Game1.tileSize * 4,
                Width = Game1.tileSize * 2,
                Height = 180,
                textLimit = 3,
                numbersOnly = true
            };
        }

        /// <summary>Handle a button click.</summary>
        /// <param name="name">The button name that was clicked.</param>
        private void HandleButtonClick(string name)
        {
            if (name == null)
                return;

            switch (name)
            {
                // OK button
                case "OK":
                    if (IsOkButtonReady())
                    {
                        ReminderMessage = ReminderMessageTextBox.Text;
                        ReminderInterval = Convert.ToInt32(ReminderIntervalTextBox.Text);
                        OnChanged(ReminderMessage, ReminderInterval);
                    }
                    break;
            }
            Game1.playSound("coin");
        }

        /// <summary>
        /// Checks if the page1 inputs are all valid
        /// </summary>
        /// <returns>True if ok button is all the conditions are met, False if not. See method definition for conditions.</returns>
        private bool IsOkButtonReady()
        {
            if (string.IsNullOrEmpty(ReminderIntervalTextBox.Text)) return false;
            if (!string.IsNullOrEmpty(ReminderMessageTextBox.Text) && Convert.ToInt32(ReminderIntervalTextBox.Text) > 0) return true;
            return false;
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {

            ReminderMessageTextBox.Update();
            ReminderIntervalTextBox.Update();

            if (OkButton.containsPoint(x, y) && IsOkButtonReady())
            {
                HandleButtonClick(OkButton.name);
                OkButton.scale -= 0.25f;
                OkButton.scale = Math.Max(0.75f, OkButton.scale);
            }

            if (DisplayRemindersButton.containsPoint(x, y))
                Game1.activeClickableMenu = new DisplayReminders(OnChanged);

            ReminderMessageTextBox.Update();
            ReminderIntervalTextBox.Update();
        }


        /// <summary>The method invoked when the player hovers the cursor over the menu.</summary>
        /// <param name="x">The X-position of the cursor.</param>
        /// <param name="y">The Y-position of the cursor.</param>
        public override void performHoverAction(int x, int y)
        {
            OkButton.scale = OkButton.containsPoint(x, y) && IsOkButtonReady()
                ? Math.Min(OkButton.scale + 0.02f, OkButton.baseScale + 0.1f)
                : Math.Max(OkButton.scale - 0.02f, OkButton.baseScale);

            DisplayRemindersButton.scale = DisplayRemindersButton.containsPoint(x, y)
                ? Math.Min(DisplayRemindersButton.scale + 0.02f, DisplayRemindersButton.baseScale + 0.1f)
                : Math.Max(DisplayRemindersButton.scale - 0.02f, DisplayRemindersButton.baseScale);
        }

        /// <summary>Draw the menu to the screen.</summary>
        /// <param name="b">The sprite batch.</param>
        public override void draw(SpriteBatch b)
        {
            Utilities.Data.Helper.Input.Suppress(Utilities.Data.MenuButton);

            //draw screen fade
            b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.75f);

            // draw menu box
            Game1.drawDialogueBox(xPositionOnScreen, yPositionOnScreen, width, height, false, true);

            // draw title scroll
            SpriteText.drawStringWithScrollCenteredAt(b, "New Recurring Reminder", Game1.viewport.Width / 2, yPositionOnScreen, "New Recurring Reminder");

            // draw textboxes
            ReminderMessageTextBox.Draw(b, false);
            ReminderIntervalTextBox.Draw(b, false);

            // draw labels
            foreach (ClickableComponent label in Labels)
            {
                if (label.name == "Reminder Message: " || label.name == "Reminder Interval: ")
                    Utility.drawTextWithShadow(b, label.name, Game1.dialogueFont, new Vector2(label.bounds.X, label.bounds.Y), Color.Black);
                else
                    Utility.drawTextWithShadow(b, label.name, Game1.smallFont, new Vector2(label.bounds.X, label.bounds.Y), Color.Black);
            }

            // draw OK button
            if (IsOkButtonReady())
                OkButton.draw(b);
            else
            {
                OkButton.draw(b);
                OkButton.draw(b, Color.Black * 0.5f, 0.97f);
            }

            // draw displayreminder button
            DisplayRemindersButton.draw(b);

            // draw cursor
            drawMouse(b);

        }
    }

    /// <summary>
    /// Second page of the menu that sets the reminder's time
    /// </summary>
    internal class NewReminder_Page2 : IClickableMenu
    {
        // title and the error message
        private readonly List<ClickableComponent> Labels = new List<ClickableComponent>();

        private readonly List<ClickableTextureComponent> MinutesAndMeridiemList = new List<ClickableTextureComponent>();
        private readonly List<ClickableTextureComponent> HoursButtons = new List<ClickableTextureComponent>();
        private ClickableComponent CurrentChoiceDisplay;
        private ClickableTextureComponent OkButton;

        /// <summary>Field that contains the Time inputed</summary>
        private int Hours = 0;
        private string Minutes;
        private string Meridiem;

        /// <summary>The callback to invoke when the birthday value changes.</summary>
        private readonly Action<int> OnChanged;

        /// <summary>The callback function that gets called when ok buttong is pressed</summary>
        public NewReminder_Page2(Action<int> onChange)
            : base(Game1.viewport.Width / 2 - (632 + IClickableMenu.borderWidth * 2) / 2, Game1.viewport.Height / 2 - (600 + IClickableMenu.borderWidth * 2) / 2 - Game1.tileSize, 632 + IClickableMenu.borderWidth * 2, 600 + IClickableMenu.borderWidth * 2 + Game1.tileSize)
        {
            OnChanged = onChange;
            SetupPositions();
        }

        /// <summary>Generates the UI</summary>
        private void SetupPositions()
        {
            // Ok button
            OkButton = new ClickableTextureComponent("OK", new Rectangle(xPositionOnScreen + width - IClickableMenu.borderWidth - IClickableMenu.spaceToClearSideBorder - Game1.tileSize, yPositionOnScreen + height - IClickableMenu.borderWidth - IClickableMenu.spaceToClearTopBorder + Game1.tileSize / 4, Game1.tileSize, Game1.tileSize), "", null, Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 46), 1f);

            // Titles
            Labels.Add(new ClickableComponent(new Rectangle(xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + Game1.tileSize * 1 + 4, yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder - Game1.tileSize / 8 + 8, 1, 1), "Reminder Time: "));
            Labels.Add(new ClickableComponent(new Rectangle(xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + Game1.tileSize * 1 + 4, yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + Game1.tileSize * 2 - Game1.tileSize / 8 + 8, 1, 1), "", "error"));

            // Current Choice
            CurrentChoiceDisplay = new ClickableComponent(new Rectangle(xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + Game1.tileSize * 1 + 4, yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + Game1.tileSize - Game1.tileSize / 8 + 8, 1, 1), "CurrentTimeDisplay");

            // AM or PM
            MinutesAndMeridiemList.Add(new ClickableTextureComponent("00", new Rectangle(xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + Game1.tileSize + 4, yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder - Game1.tileSize / 8 + Game1.tileSize * 6, Game1.tileSize * 2 + 16, Game1.tileSize + 8), "", "", Utilities.Data.Helper.Content.Load<Texture2D>("assets/00.png", ContentSource.ModFolder), new Rectangle(), (int)(Game1.pixelZoom * 0.75f)));
            MinutesAndMeridiemList.Add(new ClickableTextureComponent("30", new Rectangle(xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + Game1.tileSize + 4, yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder - Game1.tileSize / 8 + Game1.tileSize * 7 + 8, Game1.tileSize * 2 + 16, Game1.tileSize + 8), "", "", Utilities.Data.Helper.Content.Load<Texture2D>("assets/30.png", ContentSource.ModFolder), new Rectangle(), (int)(Game1.pixelZoom * 0.75f)));

            // Minutes
            MinutesAndMeridiemList.Add(new ClickableTextureComponent("AM", new Rectangle(xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + Game1.tileSize * 4 + 4, yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder - Game1.tileSize / 8 + Game1.tileSize * 6, Game1.tileSize * 2 + 16, Game1.tileSize + 8), "", "", Utilities.Data.Helper.Content.Load<Texture2D>("assets/AM.png", ContentSource.ModFolder), new Rectangle(), (int)(Game1.pixelZoom * 0.75f)));
            MinutesAndMeridiemList.Add(new ClickableTextureComponent("PM", new Rectangle(xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + Game1.tileSize * 4 + 4, yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder - Game1.tileSize / 8 + Game1.tileSize * 7 + 8, Game1.tileSize * 2 + 16, Game1.tileSize + 8), "", "", Utilities.Data.Helper.Content.Load<Texture2D>("assets/PM.png", ContentSource.ModFolder), new Rectangle(), (int)(Game1.pixelZoom * 0.75f)));

            // Hour
            // first row: 1-6
            for (int i = 1; i <= 6; i++)
            {
                HoursButtons.Add(new ClickableTextureComponent($"{i}", new Rectangle(xPositionOnScreen + width / 2 - (int)(Game1.tileSize * 5.0f) + (int)(Game1.tileSize * (i * 1.25f)), yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder - Game1.tileSize / 8 + Game1.tileSize * 3, Game1.tileSize + 16, Game1.tileSize + 16), "", "", Utilities.Data.Helper.Content.Load<Texture2D>($"assets/hourButtons/{i}HourButton.png", ContentSource.ModFolder), new Rectangle(), Game1.pixelZoom));
            }
            // second raw: 7-12
            for (int i = 7; i <= 12; i++)
            {
                HoursButtons.Add(new ClickableTextureComponent($"{i}", new Rectangle(xPositionOnScreen + width / 2 - (int)(Game1.tileSize * 5.0f) + (int)(Game1.tileSize * ((i - 6) * 1.25f)), yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder - Game1.tileSize / 8 + Game1.tileSize * 4 + Game1.tileSize / 4, Game1.tileSize + 16, Game1.tileSize + 16), "", "", Utilities.Data.Helper.Content.Load<Texture2D>($"assets/hourButtons/{i}HourButton.png", ContentSource.ModFolder), new Rectangle(), Game1.pixelZoom));
            }
        }

        /// <summary>Handles the left clicks on the menu</summary>
        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            // The 12, hour buttons
            foreach (ClickableTextureComponent hourButton in HoursButtons)
            {
                if (hourButton.containsPoint(x, y))
                {
                    Game1.playSound("coin");
                    hourButton.scale -= 0.25f;
                    Hours = Convert.ToInt32(hourButton.name);
                    hourButton.scale = Math.Max(0.75f, Game1.pixelZoom);
                }
            }

            // AM/PM and 00/30 buttons
            foreach (ClickableTextureComponent button in MinutesAndMeridiemList)
            {
                if (button.containsPoint(x, y))
                {
                    Game1.playSound("coin");
                    button.scale -= 0.25f;
                    if (button.name.EndsWith("M"))
                    {
                        Meridiem = button.name;
                    }
                    else if (button.name.EndsWith("0"))
                    {
                        Minutes = button.name;
                    }
                    button.scale = Math.Max(0.75f, Game1.pixelZoom * 0.75f);
                }
            }

            // ok button
            if (OkButton.containsPoint(x, y))
            {
                if (IsOkButtonReady())
                {
                    Game1.playSound("coin");
                    OkButton.scale -= 0.25f;
                    int reminderTime = Convert.ToInt32($"{Hours}{Minutes}");
                    if (Meridiem == "PM")
                        reminderTime += 1200;
                    OnChanged(reminderTime);
                    OkButton.scale = Math.Max(0.75f, OkButton.scale);
                    Game1.exitActiveMenu();
                }
            }

            // The error message label to say invalid time is chosen
            if (Hours != 0 && !string.IsNullOrEmpty(Minutes) && !string.IsNullOrEmpty(Meridiem))
            {
                if (!IsValidTime())
                {
                    Labels[1].name = "The entered time is invalid";
                }
                else
                {
                    Labels[1].name = "";
                }
            }
        }

        private bool IsValidTime()
        {
            int reminderTime = Convert.ToInt32($"{Hours}{Minutes}");
            if (Meridiem == "PM")
                reminderTime += 1200;
            if (reminderTime <= 2600 && reminderTime >= 600)
                return true;
            else
                return false;
        }

        /// <summary>Defines what to do when hovering over UI elements</summary>
        public override void performHoverAction(int x, int y)
        {
            OkButton.scale = OkButton.containsPoint(x, y) && IsOkButtonReady()
                ? Math.Min(OkButton.scale + 0.02f, OkButton.baseScale + 0.1f)
                : Math.Max(OkButton.scale - 0.02f, OkButton.baseScale);

            foreach (ClickableTextureComponent button in HoursButtons)
            {
                button.scale = button.containsPoint(x, y)
                    ? Math.Min(button.scale + 0.02f, button.baseScale + 0.1f)
                    : Math.Max(button.scale - 0.02f, button.baseScale);
            }

            foreach (ClickableTextureComponent button in MinutesAndMeridiemList)
            {
                button.scale = button.containsPoint(x, y)
                    ? Math.Min(button.scale + 0.02f, button.baseScale + 0.1f)
                    : Math.Max(button.scale - 0.02f, button.baseScale);
            }
        }

        /// <summary>
        /// Checks if the page1 inputs are all valid
        /// </summary>
        /// <returns>True if ok button is ready, False if not</returns>
        private bool IsOkButtonReady()
        {
            if (Hours != -1 && Meridiem != null && Minutes != null)
            {
                int reminderTime = Convert.ToInt32($"{Hours}{Minutes}");
                if (Meridiem == "PM")
                    reminderTime += 1200;
                if (reminderTime <= 2600 && reminderTime >= 600)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        /// <summary> Draws the UI</summary>
        public override void draw(SpriteBatch b)
        {
            // supress the Menu button
            Utilities.Data.Helper.Input.Suppress(Utilities.Data.MenuButton);

            // draw screen fade
            b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.75f);

            // draw menu box
            Game1.drawDialogueBox(xPositionOnScreen, yPositionOnScreen, width, height, false, true);

            // draw CurrentChoiceDisplay
            if (Hours == 0 && string.IsNullOrEmpty(Minutes) && string.IsNullOrEmpty(Meridiem))
            {
                CurrentChoiceDisplay.name = "Choose the Hour, Minutes and Period";
            }
            else
            {
                CurrentChoiceDisplay.name = $"{Hours}:{Minutes} {Meridiem}";
            }
            Utility.drawTextWithShadow(b, CurrentChoiceDisplay.name, Game1.smallFont, new Vector2(CurrentChoiceDisplay.bounds.X, CurrentChoiceDisplay.bounds.Y), Color.Black);

            // draw the 12 hour buttons
            foreach (ClickableTextureComponent button in HoursButtons)
            {
                button.draw(b);
            }

            // draw title and error message labels
            foreach (ClickableComponent label in Labels)
            {
                if (label.label == "error")
                    Utility.drawTextWithShadow(b, label.name, Game1.dialogueFont, new Vector2(label.bounds.X, label.bounds.Y), Color.Red);
                else
                    Utility.drawTextWithShadow(b, label.name, Game1.dialogueFont, new Vector2(label.bounds.X, label.bounds.Y), Color.Black);
            }

            // draw AM/PM and Minutes buttons
            foreach (ClickableTextureComponent button in MinutesAndMeridiemList)
            {
                button.draw(b);
            }

            // draw OK button
            if (IsOkButtonReady())
                OkButton.draw(b);
            else
            {
                OkButton.draw(b);
                OkButton.draw(b, Color.Black * 0.5f, 0.97f);
            }

            // draw cursor
            drawMouse(b);
        }
    }
}