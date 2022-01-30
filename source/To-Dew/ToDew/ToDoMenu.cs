/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/jltaylor-us/StardewToDew
**
*************************************************/

// Copyright 2020 Jamie Taylor
// Portions Copyright 2016–2019 Pathoschild and other contributors, see NOTICE for license
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;

namespace ToDew {
    /// <summary>
    /// Encapsulates the UI for the to-do list.
    /// </summary>
    public class ToDoMenu : IClickableMenu, IDisposable {
        /// <summary>
        /// Rendering for an individual row in the to-do list.
        /// </summary>
        private class MenuItem : ClickableComponent {
            const int MinMenuItemHeight = 40;
            const int rightMarginReserve = 90;
            const int borderWidth = 2;
            const int leftMarginReserve = 70;
            const int iconSpacing = 6;
            const int headerOutdent = 20;
            const int topPadding = 5;

            private readonly ToDoMenu menu;
            internal readonly ToDoList.ListItem todoItem;
            internal readonly int myIndex;
            internal readonly int totalItemCount;
            public MenuItem(ToDoMenu menu, ToDoList.ListItem todoItem, int index, int totalItemCount)
                : base(Rectangle.Empty, "ToDo: " + todoItem.Text) {
                this.menu = menu;
                this.todoItem = todoItem;
                this.myIndex = index;
                this.totalItemCount = totalItemCount;
            }

            public bool IsFirstItem { get => myIndex == 0; }
            public bool IsLastItem { get => myIndex == totalItemCount - 1; }

            // private static readonly Rectangle smallUpArrowSource = new Rectangle(420, 459, 12, 12);
            // private static readonly Rectangle smallDownArrowSource = new Rectangle(420, 472, 12, 12);
            // private static readonly Rectangle smallConfigButtonRect = new Rectangle(48, 208, 16, 16);
            private static readonly Rectangle configButtonSource = new Rectangle(154, 154, 20, 20);
            private static readonly Rectangle doneButtonSource = new Rectangle(341, 410, 23, 9);

            private Rectangle upArrowBounds;
            private Rectangle downArrowBounds;
            private Rectangle insideBorderArea;
            private Rectangle configControlBounds;
            // computes all of the bounds for this object and sub-objects that don't depend on Height
            private void ComputeBounds(int positionX, int positionY, int width) {
                this.bounds.X = positionX;
                this.bounds.Y = positionY;
                this.bounds.Width = width;
                this.insideBorderArea = new Rectangle(positionX + borderWidth * 2, positionY + borderWidth, width - borderWidth * 4, 0);
            }
            private int TopForCenteredVertically(int spriteHeight) {
                return this.bounds.Y + (this.bounds.Height - spriteHeight) / 2;
            }
            // Computes the remaining bounds that do depend on Height
            private void FinalizeBounds(int height) {
                this.bounds.Height = height;
                this.insideBorderArea.Height = this.bounds.Height - borderWidth * 2;
                int leftPx = this.bounds.X + this.bounds.Width - rightMarginReserve + iconSpacing;
                if (IsFirstItem) {
                    upArrowBounds = Rectangle.Empty;
                } else {
                    upArrowBounds = new Rectangle(
                        leftPx,
                        TopForCenteredVertically(CommonSprites.Icons.UpArrow.Height / 2),
                        CommonSprites.Icons.UpArrow.Width / 2,
                        CommonSprites.Icons.UpArrow.Height / 2);
                }
                leftPx += CommonSprites.Icons.UpArrow.Width / 2 + iconSpacing;
                if (IsLastItem) {
                    downArrowBounds = Rectangle.Empty;
                } else {
                    downArrowBounds = new Rectangle(
                        leftPx,
                        TopForCenteredVertically(CommonSprites.Icons.DownArrow.Height / 2),
                        CommonSprites.Icons.DownArrow.Width / 2,
                        CommonSprites.Icons.DownArrow.Height / 2);
                }
                leftPx += CommonSprites.Icons.DownArrow.Width / 2 + iconSpacing;
                configControlBounds = new Rectangle(
                    leftPx,
                    this.bounds.Y + (this.bounds.Height - configButtonSource.Height) / 2,
                    configButtonSource.Width,
                    configButtonSource.Height);
            }
            /// <summary>Draw the ToDo List item to the screen.</summary>
            /// <param name="spriteBatch">The sprite batch being drawn.</param>
            /// <param name="positionX">The X position at which to draw the item.</param>
            /// <param name="positionY">The Y position at which to draw the item.</param>
            /// <param name="width">The width to draw.</param>
            /// <param name="highlight">Whether to highlight the search result.</param>
            public Vector2 Draw(SpriteBatch spriteBatch, int positionX, int positionY, int width, int mouseX, int mouseY) {
                bool recomputedBounds = false;
                if (positionX != bounds.X || positionY != bounds.Y || width != bounds.Width) {
                    // update bounds
                    recomputedBounds = true;
                    ComputeBounds(positionX, positionY, width);
                }
                Color highlightBorderColor = Color.Black;
                bool mouseInButton = false;

                // draw
                var textSize = spriteBatch.DrawTextBlock(
                    Game1.smallFont,
                    todoItem.Text,
                    new Vector2(this.bounds.X, this.bounds.Y) + new Vector2(leftMarginReserve - (todoItem.IsHeader ? headerOutdent : 0), topPadding),
                    this.bounds.Width - leftMarginReserve - rightMarginReserve,
                    null, // color
                    todoItem.IsBold); // bold
                var maybeNewHeight = Math.Max(MinMenuItemHeight, topPadding + (int)textSize.Y);
                if (recomputedBounds || maybeNewHeight != bounds.Height) {
                    FinalizeBounds(maybeNewHeight);
                }
                spriteBatch.DrawLine(this.bounds.X, this.bounds.Y, new Vector2(this.bounds.Width, borderWidth), Color.Black); // border
                if (!IsFirstItem) {
                    bool highlight = upArrowBounds.Contains(mouseX, mouseY);
                    spriteBatch.DrawSprite(Game1.mouseCursors, CommonSprites.Icons.UpArrow, upArrowBounds.X, upArrowBounds.Y, null, highlight ? 0.6f : 0.5f);
                    mouseInButton |= highlight;
                }
                if (!IsLastItem) {
                    bool highlight = downArrowBounds.Contains(mouseX, mouseY);
                    spriteBatch.DrawSprite(Game1.mouseCursors, CommonSprites.Icons.DownArrow, downArrowBounds.X, downArrowBounds.Y, null, highlight ? 0.6f : 0.5f);
                    mouseInButton |= highlight;
                }
                if (/* !mouseInButton &&*/ this.containsPoint(mouseX, mouseY)) {
                    spriteBatch.DrawLine(this.bounds.X, this.bounds.Y + borderWidth, new Vector2(this.bounds.Width, borderWidth), highlightBorderColor);
                    spriteBatch.DrawLine(this.bounds.X, this.bounds.Y + this.bounds.Height - borderWidth, new Vector2(this.bounds.Width, borderWidth), highlightBorderColor);
                    spriteBatch.DrawLine(this.bounds.X, this.bounds.Y, new Vector2(borderWidth * 2, this.bounds.Height), highlightBorderColor);
                    spriteBatch.DrawLine(this.bounds.X + this.bounds.Width - borderWidth * 2, this.bounds.Y, new Vector2(borderWidth * 2, this.bounds.Height), highlightBorderColor);
                    if (IsLastItem) {
                        // there is no item below us to draw a border, so let's do it ourselves so the highlight box doesn't look weird
                        spriteBatch.DrawLine(this.bounds.X, this.bounds.Y + this.bounds.Height, new Vector2(this.bounds.Width, borderWidth), Color.Black); // border
                        this.bounds.Height += borderWidth;
                    }
                }
                spriteBatch.DrawSprite(Game1.mouseCursors2, configButtonSource, configControlBounds.X, configControlBounds.Y, null, configControlBounds.Contains(mouseX, mouseY) ? 1.2f : 1.0f);
                if (todoItem.IsDone) {
                    spriteBatch.DrawSprite(Game1.mouseCursors, doneButtonSource, bounds.X + leftMarginReserve / 2 - doneButtonSource.Width + 5, bounds.Y + bounds.Height / 2 - doneButtonSource.Height, null, 2.0f);
                }

                // return size
                return new Vector2(this.bounds.Width, this.bounds.Height);
            }

            // presumes we aren't sent this message unless the mouse is actually within our bounds
            public void receiveClick(int mouseX, int mouseY, ToDoList theList) {
                if (upArrowBounds.Contains(mouseX, mouseY)) {
                    theList.MoveItemUp(todoItem.Id);
                    Game1.playSound("shwip");
                    return;
                }
                if (downArrowBounds.Contains(mouseX, mouseY)) {
                    theList.MoveItemDown(todoItem.Id);
                    Game1.playSound("shwip");
                    return;
                }
                if (configControlBounds.Contains(mouseX, mouseY)) {
                    menu.currentItemEditor = new ItemConfigMenu(menu, todoItem, myIndex, totalItemCount);
                    menu.Textbox.Text = todoItem.Text;
                    return;
                }
                if (insideBorderArea.Contains(mouseX, mouseY) && !todoItem.IsHeader) {
                    theList.SetItemDone(todoItem, !todoItem.IsDone);
                    Game1.playSound("coin");
                    return;
                }
            }
        }

        private class ItemConfigMenu {
            private readonly ToDoMenu menu;
            internal readonly ToDoList.ListItem todoItem;
            internal readonly int myIndex;
            internal readonly int totalItemCount;
            public ItemConfigMenu(ToDoMenu menu, ToDoList.ListItem todoItem, int index, int totalItemCount) {
                this.menu = menu;
                this.todoItem = todoItem;
                this.myIndex = index;
                this.totalItemCount = totalItemCount;
            }

            public bool IsFirstItem { get => myIndex == 0; }
            public bool IsLastItem { get => myIndex == totalItemCount - 1; }

            private static readonly Rectangle okButtonSource = new Rectangle(128, 256, 64, 64);
            private static readonly Rectangle trashSource = new Rectangle(564, 102, 18, 26);
            private static readonly Rectangle trashLidSource = new Rectangle(564, 129, 18, 10);
            private static readonly ToDoList.DayVisibility[] daysOfWeek = {
                ToDoList.DayVisibility.Monday,
                ToDoList.DayVisibility.Tuesday,
                ToDoList.DayVisibility.Wednesday,
                ToDoList.DayVisibility.Thurdsay,
                ToDoList.DayVisibility.Friday,
                ToDoList.DayVisibility.Saturday,
                ToDoList.DayVisibility.Sunday,
            };
            private static readonly ToDoList.DayVisibility[] seasons = {
                ToDoList.DayVisibility.Spring,
                ToDoList.DayVisibility.Summer,
                ToDoList.DayVisibility.Fall,
                ToDoList.DayVisibility.Winter,
            };
            private static readonly ToDoList.DayVisibility[] weeks = {
                ToDoList.DayVisibility.Week1,
                ToDoList.DayVisibility.Week2,
                ToDoList.DayVisibility.Week3,
                ToDoList.DayVisibility.Week4,
            };


            private const int margin = 5;
            private const int leftPadding = 5;
            private Rectangle bounds = Rectangle.Empty;
            private Rectangle okButton = Rectangle.Empty;
            private Rectangle trashCan = Rectangle.Empty;
            private Rectangle doubleUp = Rectangle.Empty;
            private Rectangle doubleDown = Rectangle.Empty;
            private Rectangle repeatingCheckbox = Rectangle.Empty;
            private Rectangle headerCheckbox = Rectangle.Empty;
            private Rectangle boldCheckbox = Rectangle.Empty;
            private Rectangle hideInOverlayCheckbox = Rectangle.Empty;
            private Rectangle rainingCheckbox = Rectangle.Empty;
            private Rectangle notRainingCheckbox = Rectangle.Empty;
            private Rectangle islandRainingCheckbox = Rectangle.Empty;
            private Rectangle islandNotRainingCheckbox = Rectangle.Empty;
            private Rectangle[] daysOfWeekCheckboxes = new Rectangle[daysOfWeek.Length];
            private Rectangle[] seasonsCheckboxes = new Rectangle[seasons.Length];
            private Rectangle[] weeksCheckboxes = new Rectangle[weeks.Length];
            private string[] dayOfWeekNames = {
                I18n.Monday(),
                I18n.Tuesday(),
                I18n.Wednesday(),
                I18n.Thursday(),
                I18n.Friday(),
                I18n.Saturday(),
                I18n.Sunday(),
                I18n.Sunday(),
            };

            private const int trashScale = 3;
            private const int checkboxScale = 3;
            private const int checkboxLabelSpace = 5;
            private const int doubleArrowOffset = 10;

            private Rectangle MakeCheckboxRect(int x, int y) {
                return new Rectangle(
                    x + margin + leftPadding,
                    y + margin,
                    OptionsCheckbox.sourceRectChecked.Width * checkboxScale,
                    OptionsCheckbox.sourceRectChecked.Height * checkboxScale);
            }
            private void ComputeBounds(int positionX, int positionY, int width, int nonScrollingHeight) {
                bounds.X = positionX;
                bounds.Y = positionY;
                bounds.Width = width;
                bounds.Height = nonScrollingHeight;
                okButton = new Rectangle(bounds.Right - margin - okButtonSource.Width, bounds.Bottom - margin - okButtonSource.Height, okButtonSource.Width, okButtonSource.Height);
                trashCan = new Rectangle(okButton.X, okButton.Y - trashSource.Height * trashScale - margin * 2, trashSource.Width * trashScale, trashSource.Height * trashScale);
                doubleUp = new Rectangle(okButton.X, bounds.Y + margin, CommonSprites.Icons.UpArrow.Width, CommonSprites.Icons.UpArrow.Height + doubleArrowOffset);
                doubleDown = new Rectangle(okButton.X, doubleUp.Bottom + margin * 2, CommonSprites.Icons.DownArrow.Width, CommonSprites.Icons.DownArrow.Height + doubleArrowOffset);
                int leftPx = bounds.X + margin + leftPadding;
                int topPx = bounds.Y + margin;
                headerCheckbox = MakeCheckboxRect(leftPx, topPx);
                topPx += headerCheckbox.Height + margin;
                boldCheckbox = MakeCheckboxRect(leftPx, topPx);
                topPx += boldCheckbox.Height + margin;
                repeatingCheckbox = MakeCheckboxRect(leftPx, topPx);
                topPx += repeatingCheckbox.Height + margin;
                hideInOverlayCheckbox = MakeCheckboxRect(leftPx, topPx);
                topPx += hideInOverlayCheckbox.Height + margin;

                int lineHeight = (int)Game1.smallFont.MeasureString("ABC").Y;
                topPx += lineHeight;
                rainingCheckbox = MakeCheckboxRect(leftPx, topPx);
                topPx += rainingCheckbox.Height + margin;
                notRainingCheckbox = MakeCheckboxRect(leftPx, topPx);
                topPx += notRainingCheckbox.Height + margin;
                //topPx += lineHeight;
                islandRainingCheckbox = MakeCheckboxRect(leftPx, topPx);
                topPx += islandRainingCheckbox.Height + margin;
                islandNotRainingCheckbox = MakeCheckboxRect(leftPx, topPx);
                topPx += islandNotRainingCheckbox.Height + margin;

                int rightColTopPx = bounds.Y + margin + lineHeight;
                int rightColLeftPx = bounds.X + 375;

                int widestDay = 0;
                for (int day = 0; day < daysOfWeek.Length; day++) {
                    daysOfWeekCheckboxes[day] = MakeCheckboxRect(rightColLeftPx, rightColTopPx);
                    rightColTopPx += daysOfWeekCheckboxes[day].Height + margin;
                    widestDay = Math.Max(widestDay, (int)Game1.smallFont.MeasureString(dayOfWeekNames[day]).X);
                }

                int numWidth = (int)Game1.smallFont.MeasureString("4").X;
                int weekLeftPx = daysOfWeekCheckboxes[3].Right + margin + widestDay;
                int weekTopPx = daysOfWeekCheckboxes[1].Top;
                for (int week = 0; week < weeks.Length; week++) {
                    weeksCheckboxes[week] = MakeCheckboxRect(weekLeftPx, weekTopPx);
                    weekLeftPx += weeksCheckboxes[week].Width + margin + numWidth + numWidth;
                }

                topPx = Math.Max(topPx, rightColTopPx);
                leftPx += (int)Game1.smallFont.MeasureString(I18n.Menu_Edit_OnlySeason()).X;
                int spaceWidth = (int)Game1.smallFont.MeasureString(" ").X;
                for (int season = 0; season < seasons.Length; season++) {
                    seasonsCheckboxes[season] = MakeCheckboxRect(leftPx, topPx);
                    leftPx += seasonsCheckboxes[season].Width + margin + (int)Game1.smallFont.MeasureString(Utility.getSeasonNameFromNumber(season)).X + spaceWidth;
                }
                topPx += seasonsCheckboxes[0].Height;

                bounds.Height = Math.Max(nonScrollingHeight, topPx - bounds.Y);
            }
            private void DrawCheckbox(SpriteBatch spriteBatch, Rectangle rect, bool isChecked, string label) {
                spriteBatch.DrawSprite(Game1.mouseCursors, isChecked ? OptionsCheckbox.sourceRectChecked : OptionsCheckbox.sourceRectUnchecked, rect.X, rect.Y, null, checkboxScale);
                spriteBatch.DrawTextBlock(Game1.smallFont, label, new Vector2(rect.Right + checkboxLabelSpace, rect.Y), bounds.Width);
            }
            private int prevNonScrollingHeight = 0;
            public Vector2 Draw(SpriteBatch spriteBatch, int positionX, int positionY, int width, int nonScrollingHeight, int mouseX, int mouseY) {
                if (bounds.X != positionX || bounds.Y != positionY || bounds.Width != width || prevNonScrollingHeight != nonScrollingHeight) {
                    ComputeBounds(positionX, positionY, width, nonScrollingHeight);
                    prevNonScrollingHeight = nonScrollingHeight;
                }
                int topPx = bounds.Y + margin;

                // checkboxes
                DrawCheckbox(spriteBatch, headerCheckbox, todoItem.IsHeader, I18n.Menu_Edit_Header());
                DrawCheckbox(spriteBatch, boldCheckbox, todoItem.IsBold, I18n.Menu_Edit_Bold());
                DrawCheckbox(spriteBatch, repeatingCheckbox, todoItem.IsRepeating, I18n.Menu_Edit_Repeating());
                DrawCheckbox(spriteBatch, hideInOverlayCheckbox, todoItem.HideInOverlay, I18n.Menu_Edit_HideInOverlay());

                // weather
                spriteBatch.DrawString(Game1.smallFont, I18n.Menu_Edit_ShowWhen(), new Vector2(hideInOverlayCheckbox.X, hideInOverlayCheckbox.Bottom + margin * 2), Color.Black);
                DrawCheckbox(spriteBatch, rainingCheckbox, todoItem.FarmWeatherVisiblity.HasFlag(ToDoList.WeatherVisiblity.Raining), I18n.Menu_Edit_RainingOnFarm());
                DrawCheckbox(spriteBatch, notRainingCheckbox, todoItem.FarmWeatherVisiblity.HasFlag(ToDoList.WeatherVisiblity.NotRaining), I18n.Menu_Edit_NotRainingOnFarm());
                if (Game1.player.hasOrWillReceiveMail("Visited_Island")) {
                    //spriteBatch.DrawString(Game1.smallFont, "Island Weather", new Vector2(notRainingCheckbox.X, notRainingCheckbox.Bottom + margin * 2), Color.Black);
                    DrawCheckbox(spriteBatch, islandRainingCheckbox, todoItem.IslandWeatherVisiblity.HasFlag(ToDoList.WeatherVisiblity.Raining), I18n.Menu_Edit_RainingOnIsland());
                    DrawCheckbox(spriteBatch, islandNotRainingCheckbox, todoItem.IslandWeatherVisiblity.HasFlag(ToDoList.WeatherVisiblity.NotRaining), I18n.Menu_Edit_NotRainingOnIsland());
                }

                // days of week
                spriteBatch.DrawString(Game1.smallFont, I18n.Menu_Edit_OnlyDays(), new Vector2(daysOfWeekCheckboxes[0].X, bounds.Y + margin * 2), Color.Black);
                for (int day = 0; day < daysOfWeek.Length; day++) {
                    DrawCheckbox(spriteBatch, daysOfWeekCheckboxes[day], todoItem.DayOfWeekVisibility.HasFlag(daysOfWeek[day]), dayOfWeekNames[day]);
                }
                // weeks
                spriteBatch.DrawString(Game1.smallFont, I18n.Menu_Edit_Week(), new Vector2(weeksCheckboxes[0].X, daysOfWeekCheckboxes[0].Top), Color.Black);
                for (int week = 0; week < weeks.Length; week++) {
                    DrawCheckbox(spriteBatch, weeksCheckboxes[week], todoItem.DayOfWeekVisibility.HasFlag(weeks[week]), Convert.ToString(week + 1));
                }
                // seasons
                spriteBatch.DrawString(Game1.smallFont, I18n.Menu_Edit_OnlySeason(), new Vector2(bounds.X + margin + leftPadding, seasonsCheckboxes[0].Y), Color.Black);
                for (int season = 0; season < seasons.Length; season++) {
                    DrawCheckbox(spriteBatch, seasonsCheckboxes[season], todoItem.DayOfWeekVisibility.HasFlag(seasons[season]), Utility.getSeasonNameFromNumber(season));
                }



                //var boxSize = Game1.tinyFont.MeasureString("00 ");
                //for (int season = 0; season < 4; season++) {
                //    int date = 1;
                //    for (int week = 0; week < 4; week++) {
                //        for (int day = 0; day < 7; day++) {
                //            spriteBatch.DrawString(Game1.tinyFont, $"{date}", new Vector2(bounds.X + 350 + (int)boxSize.X * day, topPx + week * (int)(boxSize.Y) + season * ((int)boxSize.Y * 4 + 15)), Color.Black);
                //            date++;
                //        }
                //    }
                //}

                // double arrows
                if (!IsFirstItem) {
                    spriteBatch.DrawSprite(Game1.mouseCursors, CommonSprites.Icons.UpArrow, doubleUp.X, doubleUp.Y, null, doubleUp.Contains(mouseX, mouseY) ? 1.1f : 1.0f);
                    spriteBatch.DrawSprite(Game1.mouseCursors, CommonSprites.Icons.UpArrow, doubleUp.X, doubleUp.Y + doubleArrowOffset, null, doubleUp.Contains(mouseX, mouseY) ? 1.1f : 1.0f);
                }
                if (!IsLastItem) {
                    spriteBatch.DrawSprite(Game1.mouseCursors, CommonSprites.Icons.DownArrow, doubleDown.X, doubleDown.Y + doubleArrowOffset, null, doubleDown.Contains(mouseX, mouseY) ? 1.1f : 1.0f);
                    spriteBatch.DrawSprite(Game1.mouseCursors, CommonSprites.Icons.DownArrow, doubleDown.X, doubleDown.Y, null, doubleDown.Contains(mouseX, mouseY) ? 1.1f : 1.0f);
                }
                // trash and OK button
                float trashExtraScale = trashCan.Contains(mouseX, mouseY) ? 0.2f : 0.0f;
                spriteBatch.DrawSprite(Game1.mouseCursors, trashSource, trashCan.X, trashCan.Y, null, trashScale + trashExtraScale);
                spriteBatch.DrawSprite(Game1.mouseCursors, trashLidSource, trashCan.X - 1 * trashScale, trashCan.Y, null, trashScale + trashExtraScale);
                spriteBatch.DrawSprite(Game1.mouseCursors, okButtonSource, okButton.X, okButton.Y, null, okButton.Contains(mouseX, mouseY) ? 1.1f : 1.0f);

                bounds.Height = Math.Max(bounds.Height, topPx - bounds.Y);
                return new Vector2(bounds.Width, bounds.Height);
            }
            public void receiveClick(int mouseX, int mouseY, ToDoList theList) {
                if (okButton.Contains(mouseX, mouseY)) {
                    menu.exitItemConfig();
                    return;
                }
                if (headerCheckbox.Contains(mouseX, mouseY)) {
                    theList.SetItemHeader(todoItem, !todoItem.IsHeader);
                    Game1.playSound("drumkit6");
                    return;
                }
                if (boldCheckbox.Contains(mouseX, mouseY)) {
                    theList.SetItemBold(todoItem, !todoItem.IsBold);
                    Game1.playSound("drumkit6");
                    return;
                }
                if (repeatingCheckbox.Contains(mouseX, mouseY)) {
                    theList.SetItemRepeating(todoItem, !todoItem.IsRepeating);
                    Game1.playSound("drumkit6");
                    return;
                }
                if (hideInOverlayCheckbox.Contains(mouseX, mouseY)) {
                    theList.SetItemHiddenInOverlay(todoItem, !todoItem.HideInOverlay);
                    Game1.playSound("drumkit6");
                    return;
                }
                if (rainingCheckbox.Contains(mouseX, mouseY)) {
                    theList.SetItemWeatherVisibilityFlag(todoItem, false, ToDoList.WeatherVisiblity.Raining, !todoItem.FarmWeatherVisiblity.HasFlag(ToDoList.WeatherVisiblity.Raining));
                    Game1.playSound("drumkit6");
                    return;
                }
                if (notRainingCheckbox.Contains(mouseX, mouseY)) {
                    theList.SetItemWeatherVisibilityFlag(todoItem, false, ToDoList.WeatherVisiblity.NotRaining, !todoItem.FarmWeatherVisiblity.HasFlag(ToDoList.WeatherVisiblity.NotRaining));
                    Game1.playSound("drumkit6");
                    return;
                }
                if (islandRainingCheckbox.Contains(mouseX, mouseY)) {
                    theList.SetItemWeatherVisibilityFlag(todoItem, true, ToDoList.WeatherVisiblity.Raining, !todoItem.IslandWeatherVisiblity.HasFlag(ToDoList.WeatherVisiblity.Raining));
                    Game1.playSound("drumkit6");
                    return;
                }
                if (islandNotRainingCheckbox.Contains(mouseX, mouseY)) {
                    theList.SetItemWeatherVisibilityFlag(todoItem, true, ToDoList.WeatherVisiblity.NotRaining, !todoItem.IslandWeatherVisiblity.HasFlag(ToDoList.WeatherVisiblity.NotRaining));
                    Game1.playSound("drumkit6");
                    return;
                }
                for (int day = 0; day < daysOfWeek.Length; day++) {
                    if (daysOfWeekCheckboxes[day].Contains(mouseX, mouseY)) {
                        theList.SetItemDayVisibilityFlag(todoItem, daysOfWeek[day], !todoItem.DayOfWeekVisibility.HasFlag(daysOfWeek[day]));
                        Game1.playSound("drumkit6");
                        return;
                    }
                }
                for (int season = 0; season < seasons.Length; season++) {
                    if (seasonsCheckboxes[season].Contains(mouseX, mouseY)) {
                        theList.SetItemDayVisibilityFlag(todoItem, seasons[season], !todoItem.DayOfWeekVisibility.HasFlag(seasons[season]));
                        Game1.playSound("drumkit6");
                        return;
                    }
                }
                for (int week = 0; week < weeks.Length; week++) {
                    if (weeksCheckboxes[week].Contains(mouseX, mouseY)) {
                        theList.SetItemDayVisibilityFlag(todoItem, weeks[week], !todoItem.DayOfWeekVisibility.HasFlag(weeks[week]));
                        Game1.playSound("drumkit6");
                        return;
                    }
                }
                if (!IsFirstItem && doubleUp.Contains(mouseX, mouseY)) {
                    theList.MoveItemAtIndexToTop(myIndex);
                    Game1.playSound("shwip");
                    return;
                }
                if (!IsLastItem && doubleDown.Contains(mouseX, mouseY)) {
                    theList.MoveItemAtIndexToBottom(myIndex);
                    Game1.playSound("shwip");
                    return;
                }
                if (trashCan.Contains(mouseX, mouseY)) {
                    Game1.playSound("trashcan");
                    menu.currentItemEditor = null;
                    menu.Textbox.Text = "";
                    theList.DeleteItem(todoItem.Id);
                    return;
                }
            }

        }

        private readonly ModEntry theMod;
        private readonly ToDoList theList;
        private List<MenuItem> menuItemList;
        private ItemConfigMenu currentItemEditor;

        private readonly TextBox Textbox;
        /// <summary>The maximum pixels to scroll.</summary>
        private int MaxScroll;
        /// <summary>The number of pixels to scroll.</summary>
        private int CurrentScroll;
        /// <summary>Force the CurrentScroll to the bottom (MaxScroll) after rendering all items</summary>
        /// Set after adding an item because adding is asynchronous for farmhands.  Cleared on other actions.
        private bool forceScrollToBottom = false;

        /// <summary>The area where list items are rendered (used to filter mouse clicks)</summary>
        private Rectangle contentArea = Rectangle.Empty;

        /// <summary>spacing between the menu edge and content area</summary>
        private const int gutter = 15;
        /// <summary>width of the content area (not including gutter)</summary>
        private int contentWidth;
        /// <summary>height of the content area (not including gutter)</summary>
        private int contentHeight;

        private Rectangle scrollUpRect;
        private bool scrollUpVisible = false;
        private Rectangle scrollDownRect;
        private bool scrollDownVisible = false;

        public ToDoMenu(ModEntry theMod, ToDoList theList) {
            this.theMod = theMod;
            this.theList = theList;

            // update size
            this.width = Math.Min(Game1.tileSize * 14, Game1.viewport.Width);
            this.height = Math.Min((int)((float)Sprites.Letter.Sprite.Height / Sprites.Letter.Sprite.Width * this.width), Game1.viewport.Height);
            this.contentWidth = this.width - gutter * 2;
            this.contentHeight = this.height - gutter * 2;

            // update position
            Vector2 origin = Utility.getTopLeftPositionForCenteringOnScreen(this.width, this.height);
            this.xPositionOnScreen = (int)origin.X;
            this.yPositionOnScreen = (int)origin.Y;

            // initialize the scroll button location rectangles
            scrollDownRect = new Rectangle(xPositionOnScreen + gutter, yPositionOnScreen + contentHeight - CommonSprites.Icons.DownArrow.Height, CommonSprites.Icons.DownArrow.Width, CommonSprites.Icons.DownArrow.Height);
            scrollUpRect = new Rectangle(xPositionOnScreen + gutter, scrollDownRect.Top - gutter - CommonSprites.Icons.UpArrow.Height, CommonSprites.Icons.UpArrow.Width, CommonSprites.Icons.UpArrow.Height);

            // create the text box
            this.Textbox = new TextBox(Sprites.Textbox.Sheet, null, Game1.smallFont, Color.Black);
            this.Textbox.TitleText = I18n.Menu_Textbox_Title();
            this.Textbox.Selected = true;

            // initialize the list UI and callback
            theList.OnChanged += OnListChanged;
            syncMenuItemList();
        }

        private void syncMenuItemList() {
            var items = theList.Items;
            var itemCount = items.Count;
            menuItemList = new List<MenuItem>(itemCount);
            for (int i = 0; i < itemCount; i++) {
                menuItemList.Add(new MenuItem(this, items[i], i, itemCount));
            }
            syncCurrentlyItemEditor();
        }
        private void syncCurrentlyItemEditor() {
            if (currentItemEditor == null) return;
            foreach (var menuItem in menuItemList) {
                if (menuItem.todoItem.Id == currentItemEditor.todoItem.Id) {
                    currentItemEditor = new ItemConfigMenu(this, menuItem.todoItem, menuItem.myIndex, menuItem.totalItemCount);
                    return;
                }
            }
            theMod.Monitor.Log(I18n.Message_CurrentItemDeleted(itemId: currentItemEditor.todoItem.Id), LogLevel.Info);
            currentItemEditor = null;
            Textbox.Text = "";
        }

        public override void draw(SpriteBatch b) {
            int x = this.xPositionOnScreen;
            int y = this.yPositionOnScreen;
            float leftOffset = gutter;
            float topOffset = gutter;
            float bodyWidth = this.width - leftOffset - gutter; // same as contentWidth


            // get font
            SpriteFont font = Game1.smallFont;
            float spaceWidth = CommonHelper.GetSpaceWidth(font);

            // draw background and header
            // (This uses a separate sprite batch because it needs to be drawn before the
            // foreground batch, and we can't use the foreground batch because the background is
            // outside the clipping area.)
            using (SpriteBatch backgroundBatch = new SpriteBatch(Game1.graphics.GraphicsDevice)) {
                float scale = this.width / (float)Sprites.Letter.Sprite.Width;
                backgroundBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp, null, null);
                backgroundBatch.Draw(Sprites.Letter.Sheet, new Vector2(this.xPositionOnScreen, this.yPositionOnScreen),
                    Sprites.Letter.Sprite, Color.White, 0, Vector2.Zero, scale, SpriteEffects.None, 0);

                if (currentItemEditor == null) {
                    Vector2 titleSize = backgroundBatch.DrawTextBlock(font, I18n.Menu_List_TitleBoldPart(), new Vector2(x + leftOffset, y + topOffset), bodyWidth, bold: true);
                    Vector2 farmNameSize = backgroundBatch.DrawTextBlock(font, I18n.Menu_List_TitleRest(farmName: Game1.player.farmName.Value), new Vector2(x + leftOffset + titleSize.X + spaceWidth, y + topOffset), bodyWidth);
                    topOffset += Math.Max(titleSize.Y, farmNameSize.Y);
                } else {
                    Vector2 titleSize = backgroundBatch.DrawTextBlock(font, I18n.Menu_Edit_Title(), new Vector2(x + leftOffset, y + topOffset), bodyWidth, bold: true);
                    topOffset += titleSize.Y;
                }

                this.Textbox.X = x + (int)leftOffset;
                this.Textbox.Y = y + (int)topOffset;
                this.Textbox.Width = (int)bodyWidth;
                this.Textbox.Draw(backgroundBatch);
                topOffset += this.Textbox.Height;

                backgroundBatch.End();
            }

            topOffset += gutter;
            int headerHeight = (int)topOffset;

            // draw foreground
            // (This uses a separate sprite batch to set a clipping area for scrolling.)
            using (SpriteBatch contentBatch = new SpriteBatch(Game1.graphics.GraphicsDevice)) {
                GraphicsDevice device = Game1.graphics.GraphicsDevice;
                Rectangle prevScissorRectangle = device.ScissorRectangle;
                try {
                    // begin draw
                    device.ScissorRectangle = new Rectangle(x + gutter, y + headerHeight, contentWidth, contentHeight - headerHeight);
                    contentArea = device.ScissorRectangle;
                    contentBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp, null, new RasterizerState { ScissorTestEnable = true });

                    //// scroll view
                    this.CurrentScroll = Math.Max(0, this.CurrentScroll); // don't scroll past top
                    this.CurrentScroll = Math.Min(this.MaxScroll, this.CurrentScroll); // don't scroll past bottom
                    topOffset -= this.CurrentScroll; // scrolled down == move text up

                    int mouseX = Game1.getMouseX();
                    int mouseY = Game1.getMouseY();

                    // draw fields
                    {
                        if (currentItemEditor == null) {
                            foreach (MenuItem item in this.menuItemList) {
                                var objSize = item.Draw(contentBatch, x + (int)leftOffset, y + (int)topOffset, (int)bodyWidth, mouseX, mouseY);
                                topOffset += objSize.Y;
                            }
                        } else {
                            var objSize = currentItemEditor.Draw(contentBatch, x + (int)leftOffset, y + (int)topOffset, (int)bodyWidth, (int)contentHeight - headerHeight, mouseX, mouseY);
                            topOffset += objSize.Y;
                        }
                    }

                    // update max scroll
                    this.MaxScroll = Math.Max(0, (int)(topOffset - contentHeight + this.CurrentScroll));
                    if (forceScrollToBottom) {
                        this.CurrentScroll = this.MaxScroll;
                    }

                    // draw scroll icons
                    scrollUpVisible = this.MaxScroll > 0 && this.CurrentScroll > 0;
                    scrollDownVisible = this.MaxScroll > 0 && this.CurrentScroll < this.MaxScroll;
                    if (scrollUpVisible)
                        contentBatch.DrawSprite(CommonSprites.Icons.Sheet, CommonSprites.Icons.UpArrow, scrollUpRect.X, scrollUpRect.Y, null, scrollUpRect.Contains(mouseX, mouseY) ? 1.1f : 1.0f);
                    if (scrollDownVisible)
                        contentBatch.DrawSprite(CommonSprites.Icons.Sheet, CommonSprites.Icons.DownArrow, scrollDownRect.X, scrollDownRect.Y, null, scrollDownRect.Contains(mouseX, mouseY) ? 1.1f : 1.0f);

                    // end draw
                    contentBatch.End();
                } finally {
                    device.ScissorRectangle = prevScissorRectangle;
                }


                this.drawMouse(Game1.spriteBatch);
            }
        }

        private bool ignoreKeyFlag = true; // ignore the keypress that opens the menu if it also closes the menu

        /// <summary>The method invoked when the player presses an input button.</summary>
        /// <param name="key">The pressed input.</param>
        public override void receiveKeyPress(Keys key) {
            // deliberately avoid calling base, which may let another key close the menu
            if (ignoreKeyFlag && this.theMod.config.secondaryCloseButton.Equals(key.ToSButton())) {
                ignoreKeyFlag = false;
                return;
            }
            ignoreKeyFlag = false;
            if (currentItemEditor == null) {
                if (key.Equals(Keys.Escape) || this.theMod.config.secondaryCloseButton.Equals(key.ToSButton()))
                    this.exitThisMenu();

                if (key.Equals(Keys.Enter)) {
                    this.theList.AddItem(this.Textbox.Text);
                    this.Textbox.Text = "";
                    this.forceScrollToBottom = true;
                    //this.MaxScroll += MenuItem.MenuItemHeight;
                    //this.CurrentScroll = this.MaxScroll;
                    Game1.playSound("coin");
                }
            } else {
                if (key.Equals(Keys.Escape)
                    || this.theMod.config.secondaryCloseButton.Equals(key.ToSButton())
                    || key.Equals(Keys.Enter)) {
                    exitItemConfig();
                }
            }
        }
        private void exitItemConfig() {
            if (!currentItemEditor.todoItem.Text.Equals(Textbox.Text)) {
                theList.SetItemText(currentItemEditor.todoItem, Textbox.Text);
            }
            Game1.playSound("coin");
            currentItemEditor = null;
            Textbox.Text = "";
        }

        public override void receiveGamePadButton(Buttons b) {
            bool ignore = ignoreKeyFlag;
            ignoreKeyFlag = false;
            if (this.theMod.config.secondaryCloseButton.Equals(b.ToSButton())) {
                if (ignore) return;
                if (currentItemEditor == null) {
                    this.exitThisMenu();
                } else {
                    exitItemConfig();
                }
            } else {
                base.receiveGamePadButton(b);
            }
        }

        /// <summary>The method invoked when the player scrolls the mouse wheel on the lookup UI.</summary>
        /// <param name="direction">The scroll direction.</param>
        public override void receiveScrollWheelAction(int direction) {
            this.forceScrollToBottom = false;
            this.CurrentScroll -= direction; // down direction == increased scroll
        }

        /// <summary>The method invoked when the player left-clicks on the menu UI.</summary>
        /// <param name="x">The X-position of the cursor.</param>
        /// <param name="y">The Y-position of the cursor.</param>
        /// <param name="playSound">Whether to enable sound.</param>
        public override void receiveLeftClick(int x, int y, bool playSound = true) {
            const int scrollAmount = 120;
            this.forceScrollToBottom = false;
            if (!contentArea.Contains(x, y)) return;
            if (scrollUpRect.Contains(x, y)) {
                this.CurrentScroll -= scrollAmount;
                return;
            }
            if (scrollDownRect.Contains(x, y)) {
                this.CurrentScroll += scrollAmount;
                return;
            }
            if (currentItemEditor == null) {
                foreach (MenuItem match in this.menuItemList) {
                    if (match.containsPoint(x, y)) {
                        match.receiveClick(x, y, theList);
                        return;
                    }
                }
            } else {
                currentItemEditor.receiveClick(x, y, theList);
            }
        }

        /// <summary>The method invoked when the player right-clicks on the menu UI.</summary>
        /// <param name="x">The X-position of the cursor.</param>
        /// <param name="y">The Y-position of the cursor.</param>
        /// <param name="playSound">Whether to enable sound.</param>
        public override void receiveRightClick(int x, int y, bool playSound = true) {
            this.forceScrollToBottom = false;
            if (!contentArea.Contains(x, y)) return;
            if (currentItemEditor != null) return;
            foreach (MenuItem match in this.menuItemList) {
                if (match.containsPoint(x, y)) {
                    this.Textbox.Text = match.todoItem.Text;
                    return;
                }
            }
        }

        private void OnListChanged(object sender, List<ToDoList.ListItem> e) {
            syncMenuItemList();
        }

        public void Dispose() {
            this.theList.OnChanged -= OnListChanged;
        }
    }

    // Based on https://github.com/Pathoschild/StardewMods/blob/develop/LookupAnything/Components/Sprites.cs
    /// <summary>Simplifies access to the game's sprite sheets.</summary>
    /// <remarks>Each sprite is represented by a rectangle, which specifies the coordinates and dimensions of the image in the sprite sheet.</remarks>
    internal static class Sprites {
        /*********
        ** Accessors
        *********/
        /// <summary>Sprites used to draw a letter.</summary>
        public static class Letter {
            /// <summary>The sprite sheet containing the letter sprites.</summary>
            public static Texture2D Sheet => Game1.content.Load<Texture2D>("LooseSprites\\letterBG");

            /// <summary>The letter background (including edges and corners).</summary>
            public static readonly Rectangle Sprite = new Rectangle(0, 0, 320, 180);

            /// <summary>The notebook paper letter background (including edges and corners).</summary>
            public static readonly Rectangle NotebookSprite = new Rectangle(320, 0, 320, 180);
        }

        /// <summary>Sprites used to draw a textbox.</summary>
        public static class Textbox {
            /// <summary>The sprite sheet containing the textbox sprites.</summary>
            public static Texture2D Sheet => Game1.content.Load<Texture2D>("LooseSprites\\textBox");
        }
    }
}
