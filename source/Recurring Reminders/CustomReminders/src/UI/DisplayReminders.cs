/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/dem1se/SDVMods
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.IO;

namespace Dem1se.CustomReminders.UI
{
    /// <summary> UI to display the currently set reminders </summary>
    public class DisplayReminders : IClickableMenu
    {
        public readonly int XPos = (int)(Game1.viewport.Width * Game1.options.zoomLevel * (1 / Game1.options.uiScale)) / 2 - (632 + borderWidth * 2) / 2;
        public readonly int YPos = (int)(Game1.viewport.Height * Game1.options.zoomLevel * (1 / Game1.options.uiScale)) / 2 - (600 + borderWidth * 2) / 2 - Game1.tileSize;
        public readonly int UIWidth = 632 + borderWidth * 2;
        public readonly int UIHeight = 600 + borderWidth * 2 + Game1.tileSize;
        int PageIndex = 0;

        readonly List<ClickableTextureComponent> DeleteButtons = new();
        readonly List<ClickableTextureComponent> RecurringIcons = new();
        readonly List<ClickableTextureComponent> Boxes = new();
        readonly List<ClickableComponent> ReminderMessages = new();
        readonly List<ReminderModel> Reminders = new();

        ClickableTextureComponent NextPageButton, PrevPageButton, NewReminderButton;
        ClickableComponent NoRemindersWarning;

        /// <summary> This is required for switching to New Reminders menu (As its constructor requires this call back function)</summary>
        readonly Action<string, string, int, bool> Page1OnChangeBehaviour;

        /// <param name="page1OnChangeBehaviour">Required to switch to the New Reminder menu (as its constructor requires this callback function)</param>
        public DisplayReminders(Action<string, string, int, bool> page1OnChangeBehaviour)
        {
            initialize(XPos, YPos, UIWidth, UIHeight);
            Page1OnChangeBehaviour = page1OnChangeBehaviour;

            SetUpUI();
        }
            
        public void SetUpUI()
        {
            NextPageButton = new ClickableTextureComponent(new Rectangle(xPositionOnScreen + width + Game1.tileSize - Game1.tileSize / 2, yPositionOnScreen + height - Game1.tileSize, Game1.tileSize, Game1.tileSize), Utilities.Globals.Helper.ModContent.Load<Texture2D>("assets/rightArrow.png"), new Rectangle(), 1.5f);
            PrevPageButton = new ClickableTextureComponent(new Rectangle(xPositionOnScreen - Game1.tileSize, yPositionOnScreen + height - Game1.tileSize, Game1.tileSize, Game1.tileSize), Utilities.Globals.Helper.ModContent.Load<Texture2D>("assets/leftArrow.png"), new Rectangle(), 1.5f);
            NewReminderButton = new ClickableTextureComponent(new Rectangle(xPositionOnScreen - Game1.tileSize * 5 - spaceToClearSideBorder * 2, yPositionOnScreen + spaceToClearTopBorder, Game1.tileSize * 5 + Game1.tileSize / 4 + Game1.tileSize / 8, Game1.tileSize + Game1.tileSize / 8), Utilities.Globals.Helper.ModContent.Load<Texture2D>("assets/NewReminder.png"), new Rectangle(), 1.5f);
            NoRemindersWarning = new ClickableComponent(new Rectangle(xPositionOnScreen + width / 2 - width / 4 + Game1.tileSize / 2, yPositionOnScreen + height / 2, width, Game1.tileSize), Utilities.Globals.Helper.Translation.Get("display-reminder.zero-reminders"));
            
            Reminders.Clear();
            Boxes.Clear();
            ReminderMessages.Clear();
            DeleteButtons.Clear();
            RecurringIcons.Clear();

            PopulateRemindersList();
            // Setup the boxes for pages that are not the last
            if (Reminders.Count - (PageIndex * 5) >= 5)
            {
                for (int i = 0; i < 5; i++)
                {
                    ReminderMessages.Add(new ClickableComponent(new Rectangle(xPositionOnScreen + spaceToClearSideBorder + borderWidth + Game1.tileSize - Game1.tileSize / 16, yPositionOnScreen + borderWidth + spaceToClearTopBorder - Game1.tileSize / 2 + Game1.tileSize / 16 + Game1.tileSize * (i * 2) - Game1.tileSize / 2 - (i * 8) + Game1.tileSize, 1, 1), Reminders[i + (PageIndex * 5)].ReminderMessage));
                    Boxes.Add(new ClickableTextureComponent(new Rectangle(xPositionOnScreen + spaceToClearSideBorder + 16, yPositionOnScreen + spaceToClearTopBorder + Game1.tileSize * (1 + (i * 2)) - Game1.tileSize - (8 * i), width - spaceToClearSideBorder * 2 - 32, Game1.tileSize * 2 - 16), Utilities.Globals.Helper.ModContent.Load<Texture2D>("assets/reminderBox.png"), new Rectangle(), Game1.pixelZoom));
                    DeleteButtons.Add(new ClickableTextureComponent(new Rectangle(xPositionOnScreen - spaceToClearSideBorder - borderWidth + width - Game1.tileSize * 1, yPositionOnScreen + borderWidth + spaceToClearTopBorder - Game1.tileSize / 2 + Game1.tileSize / 16 + Game1.tileSize * (i * 2) - Game1.tileSize / 2 - (i * 8) + Game1.tileSize, Game1.tileSize, Game1.tileSize), Utilities.Globals.Helper.ModContent.Load<Texture2D>("assets/deleteButton.png"), new Rectangle(), Game1.pixelZoom));
                    
                    if (Reminders[i + (PageIndex * 5)].Interval != -1)
                    {
                        string second_phrase;
                        if (Reminders[i + (PageIndex * 5)].Interval == 1)
                            second_phrase = "everyday";
                        else
                            second_phrase = $"every {Reminders[i + (PageIndex * 5)].Interval - 1} days";

                        Boxes[i].hoverText = $"{Utilities.Convert.ToPrettyTime(Reminders[i + (PageIndex * 5)].Time)} {second_phrase}, since {Utilities.Convert.ToPrettyDate(Reminders[i + (PageIndex * 5)].DaysSinceStart)}";
                        RecurringIcons.Add(new ClickableTextureComponent(new Rectangle(xPositionOnScreen - spaceToClearSideBorder - borderWidth + width - Game1.tileSize * 1 - 10*Game1.pixelZoom, yPositionOnScreen + borderWidth + spaceToClearTopBorder - Game1.tileSize / 2 + Game1.tileSize / 16 + Game1.tileSize * (i * 2) - Game1.tileSize / 2 - (i * 8) + Game1.tileSize, Game1.tileSize, Game1.tileSize), Utilities.Globals.Helper.ModContent.Load<Texture2D>("assets/greenLoop.png"), new Rectangle(), Game1.pixelZoom));
                    }
                    else
                    {
                        Boxes[i].hoverText = Utilities.Convert.ToPrettyTime(Reminders[i + (PageIndex * 5)].Time) + ", " + Utilities.Convert.ToPrettyDate(Reminders[i + (PageIndex * 5)].DaysSinceStart);
                    }
                }
            }
            // Setup the boxes for the last page
            else
            {
                for (int i = 0; i < Reminders.Count - (PageIndex * 5); i++)
                {
                    ReminderMessages.Add(new ClickableComponent(new Rectangle(xPositionOnScreen + spaceToClearSideBorder + borderWidth + Game1.tileSize - Game1.tileSize / 16, yPositionOnScreen + borderWidth + spaceToClearTopBorder - Game1.tileSize / 2 + Game1.tileSize / 16 + Game1.tileSize * (i * 2) - Game1.tileSize / 2 - (i * 8) + Game1.tileSize, 1, 1), Reminders[i + (PageIndex * 5)].ReminderMessage));
                    Boxes.Add(new ClickableTextureComponent(new Rectangle(xPositionOnScreen + spaceToClearSideBorder + 16, yPositionOnScreen + spaceToClearTopBorder + Game1.tileSize * (1 + (i * 2)) - Game1.tileSize - (8 * i), width - spaceToClearSideBorder * 2 - 16, Game1.tileSize * 2 - 16), Utilities.Globals.Helper.ModContent.Load<Texture2D>("assets/reminderBox.png"), new Rectangle(), Game1.pixelZoom));
                    Boxes[i].hoverText = Utilities.Convert.ToPrettyTime(Reminders[i + (PageIndex * 5)].Time) + ", " + Utilities.Convert.ToPrettyDate(Reminders[i + (PageIndex * 5)].DaysSinceStart);
                    DeleteButtons.Add(new ClickableTextureComponent(new Rectangle(xPositionOnScreen - spaceToClearSideBorder - borderWidth + width - Game1.tileSize * 1, yPositionOnScreen + borderWidth + spaceToClearTopBorder - Game1.tileSize / 2 + Game1.tileSize / 16 + Game1.tileSize * (i * 2) - Game1.tileSize / 2 - (i * 8) + Game1.tileSize, Game1.tileSize, Game1.tileSize), Utilities.Globals.Helper.ModContent.Load<Texture2D>("assets/deleteButton.png"), new Rectangle(), Game1.pixelZoom));
                }
            }
        }

        /// <summary>This fills the Reminders list by reading all the reminder files</summary>
        private void PopulateRemindersList()
        {
            SDate now = SDate.Now();
            foreach (string absoulutePath in Directory.GetFiles(Path.Combine(Utilities.Globals.Helper.DirectoryPath, "data", Utilities.Globals.SaveFolderName)))
            {
                string relativePath = Utilities.Extras.MakeRelativePath(absoulutePath);
                
                // make sure the file is a Json. Fix for Vortex generated files causing issues.
                if (!relativePath.ToLower().EndsWith(".json"))
                    continue;

                ReminderModel Reminder = Utilities.Globals.Helper.Data.ReadJsonFile<ReminderModel>(relativePath);
                //Json-x-ly Notes: Threw this check in since now there are entries that are spent, but still awaiting cleanup. Implies to the user that the Reminder is gone.
                // -- Changing the "Reminder.Time < Game1.timeOfDay" to "Reminder.Time <= Game1.timeOfDay" determines if the entry is left in the list for the actual moment in time it's triggered.
                // If Reminder is today or earlier and Reminders Time is earlier then now, omit the entry.
                if (Reminder.DaysSinceStart <= now.DaysSinceStart && Reminder.Time < Game1.timeOfDay && Reminder.Interval == -1) 
                    continue;

                Reminders.Add(Reminder);
            }
        }

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
                Game1.activeClickableMenu = new NewReminderDatePage(Page1OnChangeBehaviour);

            // clicked delete button
            int reminderindex = 0;
            foreach (ClickableTextureComponent deleteButton in DeleteButtons)
            {
                reminderindex++;
                if (deleteButton.containsPoint(x, y))
                {
                    int reminderIndex = (PageIndex * 5) + reminderindex;
                    Utilities.File.DeleteReminder(reminderIndex);
                    SetUpUI();
                    break;
                }
            }
        }

        public override void performHoverAction(int x, int y)
        {
            NewReminderButton.scale = NewReminderButton.containsPoint(x, y)
                ? Math.Min(NewReminderButton.scale + 0.02f, NewReminderButton.baseScale + 0.1f)
                : Math.Max(NewReminderButton.scale - 0.02f, NewReminderButton.baseScale);

            // The hover text boxes are handled in the draw() method, as it's just simpler to just directly
            // do the drawing than to split up the creation of hover text and the drawing of it.
        }

        public override void draw(SpriteBatch b)
        {
            // suppress the Menu button
            Utilities.Globals.Helper.Input.Suppress(Utilities.Globals.MenuButton);

            // draw screen fade
            b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.75f);

            // draw menu box
            Game1.drawDialogueBox(xPositionOnScreen, yPositionOnScreen, width, height - 12, false, true);

            // draw title scroll
            SpriteText.drawStringWithScrollCenteredAt(b, Utilities.Globals.Helper.Translation.Get("display-reminder.title"), Game1.options.uiScale <= 1.25f ? XPos + (UIWidth / 2) : XPos - Game1.tileSize * 2 - 32, Game1.options.uiScale <= 1.25f ? YPos - (Game1.tileSize / 4) : YPos + Game1.tileSize * 3, Utilities.Globals.Helper.Translation.Get("display-reminder.title"));

            // draw boxes
            foreach (ClickableTextureComponent box in Boxes)
                box.draw(b);

            // draw labels
            foreach (ClickableComponent label in ReminderMessages)
            {
                string text = "";
                Color color = Game1.textColor;
                Utility.drawTextWithShadow(
                    b,
                    label.name,
                    Game1.smallFont,
                    new Vector2(label.bounds.X, label.bounds.Y),
                    color
                );
                if (text.Length > 0)
                    Utility.drawTextWithShadow(
                        b,
                        text,
                        Game1.smallFont,
                        new Vector2((label.bounds.X + Game1.tileSize / 3) - Game1.smallFont.MeasureString(text).X / 2f,
                        (label.bounds.Y + Game1.tileSize / 2)),
                        color
                    );
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

            foreach (ClickableTextureComponent icon in RecurringIcons)
            {
                icon.draw(b);
            }

            // draw the warning for no reminder
            if (Reminders.Count <= 0)
                Utility.drawTextWithShadow(b, NoRemindersWarning.name, Game1.smallFont, new Vector2(NoRemindersWarning.bounds.X, NoRemindersWarning.bounds.Y), Game1.textColor);

            // draw new reminders button
            NewReminderButton.draw(b);

            // draw the boxes hover text
            foreach (ClickableTextureComponent box in Boxes)
            {
                if (box.containsPoint(Game1.getMousePosition(true).X, Game1.getMousePosition(true).Y))
                {
                    if (box.hoverText != null)
                    {
                        int height;
                        int x = Game1.getMousePosition(true).X + 32;
                        int y = Game1.getMousePosition(true).Y + 32 + 16;
                        if (Game1.viewport.Width - (x + Utilities.Extras.EstimateStringDimension(box.hoverText)) < 0)
                            height = Game1.tileSize * 2 + 16;
                        else
                            height = Game1.tileSize + 16;
                        drawTextureBox(b, Game1.menuTexture, new Rectangle(0, 256, 60, 60), x, y - 16, Utilities.Extras.EstimateStringDimension(box.hoverText) + 8, height, Color.White, 1f, true);
                        SpriteText.drawString(b, box.hoverText, x + 32, y, 999, -1, 99, 1f, 0.88f, false, -1, "", 8, SpriteText.ScrollTextAlignment.Left);
                    }
                }
            }

            // draw cursor at last
            drawMouse(b);
        }
    }
}
