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
using System.IO;

namespace Dem1se.RecurringReminders.UI
{
    /// <summary>
    /// UI to display the currently set reminders
    /// </summary>
    public class DisplayReminders : IClickableMenu
    {
        private readonly List<ClickableTextureComponent> DeleteButtons = new List<ClickableTextureComponent>();
        private readonly List<ClickableTextureComponent> Boxes = new List<ClickableTextureComponent>();
        private readonly List<ClickableComponent> ReminderMessages = new List<ClickableComponent>();
        private readonly List<RecurringReminderModel> Reminders = new List<RecurringReminderModel>();

        private readonly ClickableTextureComponent NextPageButton, PrevPageButton, NewReminderButton;
        private readonly ClickableComponent NoRemindersWarning;

        ///<summary>This is required for switching to New Reminders menu (for its constructor requires this call back function)</summary>
        private readonly Action<string, int> Page1OnChangeBehaviour;

        private ICursorPosition CursorPosition;
        private int PageIndex = 0;

        /// <summary>Construct an instance.</summary>
        /// <param name="page1OnChangeBehaviour">Required to switch to the New Reminder menu (as its constructor requires this callback function)</param>
        public DisplayReminders(Action<string, int> page1OnChangeBehaviour)
            : base(Game1.viewport.Width / 2 - (632 + IClickableMenu.borderWidth * 2) / 2, Game1.viewport.Height / 2 - (600 + IClickableMenu.borderWidth * 2) / 2 - Game1.tileSize, 632 + IClickableMenu.borderWidth * 2, 600 + IClickableMenu.borderWidth * 2 + Game1.tileSize)
        {
            //this.MenuButton = Utilities.Utilities.GetMenuButton();
            Page1OnChangeBehaviour = page1OnChangeBehaviour;

            SetUpUI();

            NextPageButton = new ClickableTextureComponent(new Rectangle(xPositionOnScreen + width + Game1.tileSize - Game1.tileSize / 2, yPositionOnScreen + height - Game1.tileSize, Game1.tileSize, Game1.tileSize), Utilities.Data.Helper.Content.Load<Texture2D>("assets/rightArrow.png", ContentSource.ModFolder), new Rectangle(), 1.5f);
            PrevPageButton = new ClickableTextureComponent(new Rectangle(xPositionOnScreen - Game1.tileSize, yPositionOnScreen + height - Game1.tileSize, Game1.tileSize, Game1.tileSize), Utilities.Data.Helper.Content.Load<Texture2D>("assets/leftArrow.png", ContentSource.ModFolder), new Rectangle(), 1.5f);

            NoRemindersWarning = new ClickableComponent(new Rectangle(xPositionOnScreen + width / 2 - width / 4 + Game1.tileSize / 2, yPositionOnScreen + height / 2, width, Game1.tileSize), "No reminders are set yet");
            NewReminderButton = new ClickableTextureComponent(new Rectangle(xPositionOnScreen - Game1.tileSize * 5 - IClickableMenu.spaceToClearSideBorder * 2, yPositionOnScreen + IClickableMenu.spaceToClearTopBorder, Game1.tileSize * 5 + Game1.tileSize / 4 + Game1.tileSize / 8, Game1.tileSize + Game1.tileSize / 8), Utilities.Data.Helper.Content.Load<Texture2D>("assets/NewReminder.png", ContentSource.ModFolder), new Rectangle(), 1.5f);
        }

        public void SetUpUI()
        {
            SetUpReminderMessages();
            SetUpBoxes();
            SetUpDeleteButtons();
        }

        /// <summary>Regenerates the reminder messages (for page switches and initializations)</summary>
        private void SetUpReminderMessages()
        {
            Reminders.Clear();
            PopulateRemindersList();
            ReminderMessages.Clear();
            // Setup the boxes for pages that are not the last
            if (Reminders.Count - (PageIndex * 5) >= 5)
            {
                for (int i = 0; i < 5; i++)
                    ReminderMessages.Add(new ClickableComponent(new Rectangle(xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + Game1.tileSize - Game1.tileSize / 16, yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder - Game1.tileSize / 2 + Game1.tileSize / 16 + Game1.tileSize * (i * 2) - Game1.tileSize / 2 - (i * 8) + Game1.tileSize, 1, 1), Reminders[i + (PageIndex * 5)].ReminderMessage));
            }
            // Setup the boxes for the last page
            else
            {
                for (int i = 0; i < Reminders.Count - (PageIndex * 5); i++)
                    ReminderMessages.Add(new ClickableComponent(new Rectangle(xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + Game1.tileSize - Game1.tileSize / 16, yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder - Game1.tileSize / 2 + Game1.tileSize / 16 + Game1.tileSize * (i * 2) - Game1.tileSize / 2 - (i * 8) + Game1.tileSize, 1, 1), Reminders[i + (PageIndex * 5)].ReminderMessage));
            }
        }

        /// <summary>Regenerates the boxes (for page switches nad initializations)</summary>
        private void SetUpBoxes()
        {
            Boxes.Clear();
            // Setup the boxes for pages that are not the last
            if (Reminders.Count - (PageIndex * 5) >= 5)
            {
                for (int i = 0; i < 5; i++)
                {
                    Boxes.Add(new ClickableTextureComponent(new Rectangle(xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + 16, yPositionOnScreen + IClickableMenu.spaceToClearTopBorder + Game1.tileSize * (1 + (i * 2)) - Game1.tileSize - (8 * i), width - IClickableMenu.spaceToClearSideBorder * 2 - 32, Game1.tileSize * 2 - 16), Utilities.Data.Helper.Content.Load<Texture2D>("assets/reminderBox.png", ContentSource.ModFolder), new Rectangle(), Game1.pixelZoom));
                    //Boxes[i].hoverText = Utilities.Converts.ConvertToPrettyTime(Reminders[i + (PageIndex * 5)].Time) + ", " + Utilities.Converts.ConvertToPrettyDate(Reminders[i + (PageIndex * 5)].DaysInterval);
                    Boxes[i].hoverText = $"Interval: {Reminders[i + (PageIndex * 5)].DaysInterval}, Time: {Utilities.Converts.ConvertToPrettyTime(Reminders[i + (PageIndex * 5)].Time)}";
                }
            }
            // Setup the boxes for the last page
            else
            {
                for (int i = 0; i < Reminders.Count - (PageIndex * 5); i++)
                {
                    Boxes.Add(new ClickableTextureComponent(new Rectangle(xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + 16, yPositionOnScreen + IClickableMenu.spaceToClearTopBorder + Game1.tileSize * (1 + (i * 2)) - Game1.tileSize - (8 * i), width - IClickableMenu.spaceToClearSideBorder * 2 - 16, Game1.tileSize * 2 - 16), Utilities.Data.Helper.Content.Load<Texture2D>("assets/reminderBox.png", ContentSource.ModFolder), new Rectangle(), Game1.pixelZoom));
                    //Boxes[i].hoverText = Utilities.Converts.ConvertToPrettyTime(Reminders[i + (PageIndex * 5)].Time) + ", " + Utilities.Converts.ConvertToPrettyDate(Reminders[i + (PageIndex * 5)].DaysInterval);
                    Boxes[i].hoverText = $"Interval: {Reminders[i + (PageIndex * 5)].DaysInterval}, Time: {Utilities.Converts.ConvertToPrettyTime(Reminders[i + (PageIndex * 5)].Time)}";
                }
            }
        }

        /// <summary>Regenerates the reminder messages (for page switches and initializations)</summary>
        private void SetUpDeleteButtons()
        {
            DeleteButtons.Clear();
            // Setup the delete buttons for pages that are not the last
            if (Reminders.Count - (PageIndex * 5) >= 5)
            {
                for (int i = 0; i < 5; i++)
                    DeleteButtons.Add(new ClickableTextureComponent(new Rectangle(xPositionOnScreen - IClickableMenu.spaceToClearSideBorder - IClickableMenu.borderWidth + width - Game1.tileSize * 1, yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder - Game1.tileSize / 2 + Game1.tileSize / 16 + Game1.tileSize * (i * 2) - Game1.tileSize / 2 - (i * 8) + Game1.tileSize, Game1.tileSize, Game1.tileSize), Utilities.Data.Helper.Content.Load<Texture2D>("assets/deleteButton.png", ContentSource.ModFolder), new Rectangle(), Game1.pixelZoom));
            }
            // Setup the delete buttons for the last page
            else
            {
                for (int i = 0; i < Reminders.Count - (PageIndex * 5); i++)
                    DeleteButtons.Add(new ClickableTextureComponent(new Rectangle(xPositionOnScreen - IClickableMenu.spaceToClearSideBorder - IClickableMenu.borderWidth + width - Game1.tileSize * 1, yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder - Game1.tileSize / 2 + Game1.tileSize / 16 + Game1.tileSize * (i * 2) - Game1.tileSize / 2 - (i * 8) + Game1.tileSize, Game1.tileSize, Game1.tileSize), Utilities.Data.Helper.Content.Load<Texture2D>("assets/deleteButton.png", ContentSource.ModFolder), new Rectangle(), Game1.pixelZoom));
            }
        }

        /// <summary>This fills the Reminders list by reading all the reminder files</summary>
        private void PopulateRemindersList()
        {
            foreach (string AbsoulutePath in Directory.GetFiles(Path.Combine(Utilities.Data.Helper.DirectoryPath, "data", Utilities.Data.SaveFolderName)))
            {
                string RelativePath = Utilities.Extras.MakeRelativePath(AbsoulutePath);
                Reminders.Add(Utilities.Data.Helper.Data.ReadJsonFile<RecurringReminderModel>(RelativePath));
            }
        }

        /// <summary>
        /// Handles the left click on the UI elements
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="playSound"></param>
        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            // clicked next page
            if (NextPageButton.containsPoint(x, y))
            {
                if (Reminders.Count > (PageIndex + 1) * 5)
                {
                    PageIndex += 1;
                    SetUpUI();
                }
            }
            // clicked previous page
            else if (PrevPageButton.containsPoint(x, y))
            {
                if (PageIndex != 0)
                {
                    PageIndex -= 1;
                    SetUpUI();
                }
            }

            // clicked new reminder
            if (NewReminderButton.containsPoint(x, y))
                Game1.activeClickableMenu = new NewReminder_Page1(Page1OnChangeBehaviour);

            // clicked delete button
            int reminderindex = 0;
            foreach (ClickableTextureComponent deleteButton in DeleteButtons)
            {
                reminderindex++;
                if (deleteButton.containsPoint(x, y))
                {
                    int reminderIndex = (PageIndex * 5) + reminderindex;
                    Utilities.Files.DeleteReminder(reminderIndex);
                    SetUpUI();
                    break;
                }
            }
        }

        /// <summary>Defines what to do when hovering over UI elements</summary>
        public override void performHoverAction(int x, int y)
        {
            NewReminderButton.scale = NewReminderButton.containsPoint(x, y)
                ? Math.Min(NewReminderButton.scale + 0.02f, NewReminderButton.baseScale + 0.1f)
                : Math.Max(NewReminderButton.scale - 0.02f, NewReminderButton.baseScale);
        }

        /// <summary>The draw calls for the UI elements</summary>
        public override void draw(SpriteBatch b)
        {
            CursorPosition = Utilities.Data.Helper.Input.GetCursorPosition();

            // suppress the Menu button
            Utilities.Data.Helper.Input.Suppress(Utilities.Data.MenuButton);

            // draw screen fade
            b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.75f);

            // draw menu box
            Game1.drawDialogueBox(xPositionOnScreen, yPositionOnScreen, width, height - 12, false, true);

            // draw title scroll
            SpriteText.drawStringWithScrollCenteredAt(b, "Display Recurring Reminders", Game1.viewport.Width / 2, yPositionOnScreen, "Display Recurring Reminders");

            // draw boxes
            foreach (ClickableTextureComponent box in Boxes)
                box.draw(b);

            // draw labels
            foreach (ClickableComponent label in ReminderMessages)
            {
                string text = "";
                Color color = Game1.textColor;
                Utility.drawTextWithShadow(b, label.name, Game1.smallFont, new Vector2(label.bounds.X, label.bounds.Y), color);
                if (text.Length > 0)
                    Utility.drawTextWithShadow(b, text, Game1.smallFont, new Vector2((label.bounds.X + Game1.tileSize / 3) - Game1.smallFont.MeasureString(text).X / 2f, (label.bounds.Y + Game1.tileSize / 2)), color);
            }
            if (Reminders.Count > (PageIndex + 1) * 5)
                NextPageButton.draw(b);
            if (PageIndex != 0)
                PrevPageButton.draw(b);

            // draw the delete buttons
            foreach (ClickableTextureComponent button in DeleteButtons)
            {
                button.draw(b);
            }

            // draw the warning for no reminder
            if (Reminders.Count <= 0)
                Utility.drawTextWithShadow(b, NoRemindersWarning.name, Game1.smallFont, new Vector2(NoRemindersWarning.bounds.X, NoRemindersWarning.bounds.Y), Game1.textColor);

            // draw new reminders button
            NewReminderButton.draw(b);

            // draw the boxes hover text
            foreach (ClickableTextureComponent box in Boxes)
            {
                if (box.containsPoint((int)CursorPosition.ScreenPixels.X, (int)CursorPosition.ScreenPixels.Y))
                {
                    if (box.hoverText != null)
                    {
                        int x = Game1.getMouseX() + 32;
                        int y = Game1.getMouseY() + 32 + 16;
                        IClickableMenu.drawTextureBox(b, Game1.menuTexture, new Rectangle(0, 256, 60, 60), x, y - 16, Utilities.Extras.EstimateStringDimension(box.hoverText) + 8, Game1.tileSize + 16, Color.White, 1f, true);
                        SpriteText.drawString(b, box.hoverText, x + 32, y, 999, -1, 99, 1f, 0.88f, false, -1, "", 8, SpriteText.ScrollTextAlignment.Left);
                    }
                }
            }

            // draw cursor
            drawMouse(b);
        }
    }
}
