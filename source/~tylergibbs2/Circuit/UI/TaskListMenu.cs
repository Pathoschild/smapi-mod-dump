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
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;


namespace Circuit.UI
{
    internal class TaskListMenu : IClickableMenu
    {
        private int CurrentDrawHeight;

        private int ScrollOffset = 0;

        private int TitleDrawHeight;

        private int HighestDrawHeight;

        private int TasksOnScreen;

        private ClickableTextureComponent UpArrow = null!;

        private ClickableTextureComponent DownArrow = null!;

        private Rectangle UncheckedBoxSourceRect = new(227, 425, 9, 9);

        private Rectangle CheckedBoxSourceRect = new(236, 425, 9, 9);

        private static readonly Rectangle HorizontalLineRectangle = new(0, 256, 60, 60);

        private int TasksHiddenAboveScroll;

        public TaskListMenu() : base(0, 0, 0, 0)
        {
            CalculatePositions();
        }

        public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
        {
            CalculatePositions();
        }

        private void CalculatePositions()
        {
            width = (int)(Game1.uiViewport.Width * 0.66f);

            xPositionOnScreen = Game1.uiViewport.Width / 2 - width / 2;
            yPositionOnScreen = 100;

            height = 808;
            while (height + yPositionOnScreen > Game1.uiViewport.Height)
                height -= 100;

            UpArrow = new(new Rectangle(xPositionOnScreen + width - 64, yPositionOnScreen + 125, 44, 48), Game1.mouseCursors, new Rectangle(421, 459, 11, 12), 4f);
            DownArrow = new(new Rectangle(xPositionOnScreen + width - 64, yPositionOnScreen + height - 64, 44, 48), Game1.mouseCursors, new Rectangle(421, 472, 11, 12), 4f);
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
            return TasksHiddenAboveScroll > 0;
        }

        private bool CanScrollDown()
        {
            if (ModEntry.Instance.TaskManager is null)
                return false;

            TaskManager manager = ModEntry.Instance.TaskManager;
            return TasksHiddenAboveScroll + TasksOnScreen < manager.OriginalTaskOrder.Count;
        }

        public override void receiveScrollWheelAction(int direction)
        {
            int lineHeight = (int)Game1.smallFont.MeasureString("F").Y;

            if (direction > 0 && CanScrollUp())
            {
                ScrollOffset += lineHeight * 3;
                Game1.playSound("shiny4");
            }
            else if (direction < 0 && CanScrollDown())
            {
                ScrollOffset -= lineHeight * 3;
                Game1.playSound("shiny4");
            }
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            base.receiveLeftClick(x, y, playSound);
            if (UpArrow.containsPoint(x, y) && CanScrollUp())
            {
                ScrollOffset += 100;
                Game1.playSound("shiny4");
                UpArrow.scale = UpArrow.baseScale;
            }
            else if (DownArrow.containsPoint(x, y) && CanScrollDown())
            {
                ScrollOffset -= 100;
                Game1.playSound("shiny4");
                DownArrow.scale = DownArrow.baseScale;
            }
        }

        public override void performHoverAction(int x, int y)
        {
            base.performHoverAction(x, y);
            UpArrow.tryHover(x, y);
            DownArrow.tryHover(x, y);
        }


        public void DrawTitle(SpriteBatch b)
        {
            Vector2 textSize = Game1.dialogueFont.MeasureString("Task List");

            Utility.drawTextWithShadow(
                b,
                "Task List",
                Game1.dialogueFont,
                new Vector2(
                    xPositionOnScreen + (width / 2) - (textSize.X / 2),
                    CurrentDrawHeight + spaceToClearSideBorder * 2
                ),
                Game1.textColor
            );

            Vector2 subtitleTextSize = Vector2.Zero;
            if (EventManager.AnyEventIsActive())
            {
                EventBase evt = EventManager.GetCurrentEvent()!;
                string subtitle = Game1.parseText($"{evt.GetDisplayName()}: {evt.GetDescription()}", Game1.smallFont, width - spaceToClearSideBorder * 2);
                subtitleTextSize = Game1.smallFont.MeasureString(subtitle);

                Utility.drawTextWithShadow(
                    b,
                    subtitle,
                    Game1.smallFont,
                    new Vector2(
                        xPositionOnScreen + (width / 2) - (subtitleTextSize.X / 2),
                        CurrentDrawHeight + spaceToClearSideBorder * 2 + textSize.Y
                    ),
                    Game1.textColor
                );
            }

            DrawHorizontalLine(b, CurrentDrawHeight + (int)subtitleTextSize.Y);

            CurrentDrawHeight += (int)textSize.Y + (int)subtitleTextSize.Y + 16 + spaceToClearSideBorder * 2;
            TitleDrawHeight = CurrentDrawHeight;
        }

        public void DrawHorizontalLine(SpriteBatch b, int y)
        {
            drawTextureBox(
                b,
                Game1.menuTexture,
                HorizontalLineRectangle,
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
            return yLevel >= TitleDrawHeight && yLevel < height + yPositionOnScreen - 65;
        }

        public void DrawScrollArrows(SpriteBatch b)
        {
            if (CanScrollUp())
                UpArrow.draw(b);
            if (CanScrollDown())
                DownArrow.draw(b);
        }

        public void DrawTask(SpriteBatch b, CircuitTask task)
        {
            Color difficultyColor = CircuitTasks.GetTaskColor(task);
            string difficultyText = $"+{CircuitTasks.GetTaskPoints(task)}";

            Vector2 difficultyTextSize = Game1.smallFont.MeasureString(difficultyText);

            string displayText = CircuitTasks.GetTaskDisplayText(task);
            displayText = Game1.parseText(displayText, Game1.smallFont, width - spaceToClearSideBorder - 70 - (int)difficultyTextSize.X);
            Vector2 textSize = Game1.smallFont.MeasureString(displayText);

            bool completed = ModEntry.Instance.TaskManager!.CompleteTasks.Contains(task);

            int drawHeight = CurrentDrawHeight + ScrollOffset;
            if (ShouldDraw(drawHeight))
            {
                TasksOnScreen++;

                b.Draw(
                    Game1.mouseCursors,
                    new Vector2(xPositionOnScreen + spaceToClearSideBorder + 8, drawHeight + 9),
                    completed ? CheckedBoxSourceRect : UncheckedBoxSourceRect,
                    Color.White,
                    0f,
                    Vector2.Zero,
                    3f,
                    SpriteEffects.None,
                    1f
                );

                b.DrawString(
                    Game1.smallFont,
                    displayText,
                    new Vector2(xPositionOnScreen + 46 + spaceToClearSideBorder, drawHeight + 8),
                    Game1.textColor
                );

                b.DrawString(
                    Game1.smallFont,
                    difficultyText,
                    new Vector2(xPositionOnScreen + 46 + spaceToClearSideBorder + (int)textSize.X, drawHeight + 8),
                    difficultyColor
                );
            }
            else if (drawHeight < TitleDrawHeight)
                TasksHiddenAboveScroll++;

            CurrentDrawHeight += (int)textSize.Y;

            if (CurrentDrawHeight > HighestDrawHeight)
                HighestDrawHeight = CurrentDrawHeight;
        }

        public override void draw(SpriteBatch b)
        {
            if (ModEntry.Instance.TaskManager is null)
                return;

            CurrentDrawHeight = yPositionOnScreen;
            TasksOnScreen = 0;
            TasksHiddenAboveScroll = 0;

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

            foreach (CircuitTask task in ModEntry.Instance.TaskManager.OriginalTaskOrder)
                DrawTask(b, task);

            UpArrow.bounds.Y = TitleDrawHeight + 8;
            DrawScrollArrows(b);

            drawMouse(b);
        }
    }
}
