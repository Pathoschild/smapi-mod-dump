/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MolsonCAD/DeluxeJournal
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Menus;
using DeluxeJournal.Framework;
using DeluxeJournal.Framework.Tasks;
using DeluxeJournal.Menus.Components;
using DeluxeJournal.Tasks;

using static StardewValley.Menus.ClickableComponent;

namespace DeluxeJournal.Menus
{
    /// <summary>Tasks page.</summary>
    public class TasksPage : PageBase
    {
        private const int maxEntries = 8;

        public static readonly Color EmptyListTextColor = new Color(86, 22, 12);

        public readonly List<TaskEntryComponent> taskEntries;
        public readonly ClickableTextureComponent addTaskButton;
        public readonly ClickableTextureComponent moneyButton;
        public readonly ClickableComponent moneyBox;
        public readonly MoneyDial moneyDial;
        public readonly ScrollComponent scrollComponent;

        private readonly Config _config;
        private readonly TaskManager _taskManager;
        private readonly Rectangle _boundsWithScrollBar;
        private readonly double _dragScrollInterval;
        private double _dragScrollStartTime;
        private int _currentlySnappedEntry;
        private int _selectedTaskIndex;
        private bool _moneyButtonVisible;
        private bool _dragging;

        public TasksPage(Rectangle bounds, Texture2D tabTexture, ITranslationHelper translation) :
            this("tasks", translation.Get("ui.tab.tasks"), bounds.X, bounds.Y, bounds.Width, bounds.Height, tabTexture, new Rectangle(16, 0, 16, 16), translation)
        {
        }

        public TasksPage(string name, string title, int x, int y, int width, int height, Texture2D tabTexture, Rectangle tabSourceRect, ITranslationHelper translation) :
            base(name, title, x, y, width, height, tabTexture, tabSourceRect, translation)
        {
            if (DeluxeJournalMod.Instance?.Config is not Config config)
            {
                throw new InvalidOperationException("TasksPage created before mod entry.");
            }

            if (DeluxeJournalMod.Instance?.TaskManager is not TaskManager taskManager)
            {
                throw new InvalidOperationException("TasksPage created before instantiation of TaskManager");
            }

            _config = config;
            _taskManager = taskManager;
            _dragScrollInterval = 0.16;
            _dragScrollStartTime = 0;
            _currentlySnappedEntry = 0;
            _selectedTaskIndex = -1;
            _dragging = false;
            taskEntries = new List<TaskEntryComponent>();
            moneyDial = new MoneyDial(8, false);

            Rectangle bounds = new Rectangle(x + 16, y + 16, width - 32, (height - 32) / maxEntries);

            for (int i = 0; i < maxEntries; i++)
            {
                bounds.Y = y + 20 + i * bounds.Height;
                taskEntries.Add(new TaskEntryComponent(bounds, i.ToString(), Translation)
                {
                    myID = i,
                    upNeighborID = CUSTOM_SNAP_BEHAVIOR,
                    downNeighborID = CUSTOM_SNAP_BEHAVIOR,
                    rightNeighborID = CUSTOM_SNAP_BEHAVIOR,
                    leftNeighborID = CUSTOM_SNAP_BEHAVIOR,
                    fullyImmutable = true
                });
            }

            addTaskButton = new ClickableTextureComponent(
                new Rectangle(x + width - 336, y + height, 60, 60),
                DeluxeJournalMod.UiTexture,
                new Rectangle(0, 32, 15, 17),
                4f)
            {
                myID = 1000,
                upNeighborID = CUSTOM_SNAP_BEHAVIOR,
                leftNeighborID = CUSTOM_SNAP_BEHAVIOR,
                rightNeighborID = 1001,
                rightNeighborImmutable = true
            };

            moneyButton = new ClickableTextureComponent(
                new Rectangle(x + width - 260, y + height + 20, 24, 36),
                DeluxeJournalMod.UiTexture,
                new Rectangle(85, 37, 6, 9),
                4f)
            {
                myID = 1001,
                upNeighborID = CUSTOM_SNAP_BEHAVIOR,
                rightNeighborID = 1002,
                leftNeighborID = 1000,
                fullyImmutable = true
            };

            moneyBox = new ClickableComponent(new Rectangle(x + width - 236, y + height, 236, 68), "moneyBox")
            {
                myID = 1002,
                upNeighborID = CUSTOM_SNAP_BEHAVIOR,
                leftNeighborID = 1001,
                fullyImmutable = true
            };

            Rectangle scrollBarBounds = new Rectangle(x + width + 16, y + 148, 24, height - 216);
            Rectangle scrollContentBounds = new Rectangle(x, y + 16, width, height - 32);
            scrollComponent = new ScrollComponent(scrollBarBounds, scrollContentBounds, (height - 32) / 8, true);
            scrollComponent.ContentHeight = _taskManager.Tasks.Count * scrollComponent.ScrollDistance;

            _boundsWithScrollBar = new Rectangle(x, y, scrollBarBounds.Right - x + 16, height);

            RefreshMoneyDial();
        }

        public void OpenAddTaskMenu()
        {
            SetSnappyChildMenu(new AddTaskMenu(Translation));
        }

        public void OpenTaskOptionsMenu(ITask task)
        {
            SetSnappyChildMenu(new TaskOptionsMenu(task, Translation));
        }

        private void OpenTaskEntryMenu(TaskEntryComponent entry, ITask task)
        {
            SetSnappyChildMenu(new TaskEntryMenu(entry, task, Translation));
        }

        public void AddTask(ITask task)
        {
            _taskManager.AddTask(task);
            scrollComponent.ContentHeight += scrollComponent.ScrollDistance;
            scrollComponent.Refresh();
        }

        public void RemoveTask(ITask task)
        {
            _taskManager.RemoveTask(task);
            scrollComponent.ContentHeight -= scrollComponent.ScrollDistance;
            scrollComponent.ScrollAmount -= scrollComponent.ScrollDistance;
        }

        private void RemoveTaskAt(int i)
        {
            _taskManager.RemoveTaskAt(i);
            scrollComponent.ContentHeight -= scrollComponent.ScrollDistance;
            scrollComponent.ScrollAmount -= scrollComponent.ScrollDistance;
        }

        public void SortTasks()
        {
            _taskManager.SortTasks();
        }

        public override void OnHidden()
        {
            foreach (ITask task in _taskManager.Tasks)
            {
                task.MarkAsViewed();
            }
        }

        public override void OnVisible()
        {
            _selectedTaskIndex = -1;
            allClickableComponents.AddRange(taskEntries);
            SortTasks();
        }

        public override bool readyToClose()
        {
            return base.readyToClose() && (_childMenu == null || _childMenu.readyToClose());
        }

        protected override void customSnapBehavior(int direction, int oldRegion, int oldID)
        {
            int filledEntries = Math.Min(maxEntries, _taskManager.Tasks.Count);

            switch (direction)
            {
                case Game1.up:
                    if (_taskManager.Tasks.Count > 0)
                    {
                        if (oldID > 0 && oldID < filledEntries)
                        {
                            currentlySnappedComponent = getComponentWithID(oldID - 1);
                        }
                        else if (oldID == 0)
                        {
                            scrollComponent.Scroll(1, false);
                        }
                        else
                        {
                            currentlySnappedComponent = getComponentWithID(filledEntries - 1);
                        }
                    }
                    else
                    {
                        SnapToActiveTabComponent();
                        return;
                    }
                    break;
                case Game1.down:
                    if (oldID >= 0 && oldID < filledEntries)
                    {
                        if (oldID < filledEntries - 1)
                        {
                            currentlySnappedComponent = getComponentWithID(oldID + 1);
                        }
                        else if (scrollComponent.GetPercentScrolled() == 1f)
                        {
                            currentlySnappedComponent = addTaskButton;
                        }
                        else
                        {
                            scrollComponent.Scroll(-1, false);
                        }
                    }
                    break;
                case Game1.right:
                    currentlySnappedComponent = addTaskButton;
                    break;
                case Game1.left:
                    SnapToActiveTabComponent();
                    return;
            }

            if (oldID >= 0 && oldID < filledEntries)
            {
                _currentlySnappedEntry = currentlySnappedComponent.myID;
            }

            snapCursorToCurrentSnappedComponent();
        }

        public override void snapToDefaultClickableComponent()
        {
            if (ChildHasFocus())
            {
                _childMenu.snapToDefaultClickableComponent();
            }
            else
            {
                currentlySnappedComponent = (_taskManager.Tasks.Count > 0) ? getComponentWithID(_currentlySnappedEntry) : addTaskButton;
                snapCursorToCurrentSnappedComponent();
            }
        }

        public override void snapCursorToCurrentSnappedComponent()
        {
            if (ChildHasFocus())
            {
                _childMenu.snapCursorToCurrentSnappedComponent();
            }
            else
            {
                base.snapCursorToCurrentSnappedComponent();
            }
        }

        public override void receiveGamePadButton(Buttons b)
        {
            if (ChildHasFocus())
            {
                _childMenu.receiveGamePadButton(b);
            }
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            if (ChildHasFocus())
            {
                _childMenu.receiveLeftClick(x, y, playSound);
                return;
            }

            if (_dragging)
            {
                return;
            }
            else if (addTaskButton.containsPoint(x, y))
            {
                OpenAddTaskMenu();
                return;
            }
            else if (moneyButton.containsPoint(x, y))
            {
                _config.MoneyViewNetWealth = !_config.MoneyViewNetWealth;
                RefreshMoneyDial();
            }
            else
            {
                IReadOnlyList<ITask> tasks = _taskManager.Tasks;
                int scrollOffset = scrollComponent.ScrollAmount / scrollComponent.ScrollDistance;

                for (int i = 0; i < taskEntries.Count && i + scrollOffset < tasks.Count; i++)
                {
                    TaskEntryComponent entry = taskEntries[i];
                    ITask task = tasks[i + scrollOffset];

                    if (entry.containsPoint(x, y))
                    {
                        if (entry.checkbox.containsPoint(x, y))
                        {
                            if (task.Active)
                            {
                                task.Complete = !task.Complete;
                                task.MarkAsViewed();
                                Game1.playSoundPitched("tinyWhip", task.Complete ? 2000 : 1000);
                            }
                            else
                            {
                                task.Active = true;
                                Game1.playSound("newRecipe");
                            }
                        }
                        else if (entry.removeButton.containsPoint(x, y))
                        {
                            RemoveTaskAt(i + scrollOffset);
                            Game1.playSound("trashcan");
                        }
                        else
                        {
                            _selectedTaskIndex = i + scrollOffset;
                        }

                        return;
                    }
                }

                if (!scrollComponent.CanScroll() || !_boundsWithScrollBar.Contains(x, y))
                {
                    Game1.activeClickableMenu?.exitThisMenu();
                }
            }

            scrollComponent.ReceiveLeftClick(x, y, playSound);
        }

        public override void leftClickHeld(int x, int y)
        {
            if (!GameMenu.forcePreventClose)
            {
                if (ChildHasFocus())
                {
                    _childMenu.leftClickHeld(x, y);
                    return;
                }

                if (_selectedTaskIndex != -1)
                {
                    double currentTime = Game1.currentGameTime.TotalGameTime.TotalSeconds;

                    if (currentTime - _dragScrollStartTime > _dragScrollInterval)
                    {
                        if (y < scrollComponent.ContentBounds.Y)
                        {
                            _dragScrollStartTime = currentTime;
                            scrollComponent.Scroll(1, false);
                        }
                        else if (y > scrollComponent.ContentBounds.Bottom)
                        {
                            _dragScrollStartTime = currentTime;
                            scrollComponent.Scroll(-1, false);
                        }
                    }

                    ITask task = _taskManager.Tasks[_selectedTaskIndex];
                    int taskY = scrollComponent.ContentBounds.Y + (_selectedTaskIndex * scrollComponent.ScrollDistance - scrollComponent.ScrollAmount);
                    int scrollOffset = scrollComponent.ScrollAmount / scrollComponent.ScrollDistance;

                    if (y < taskY)
                    {
                        if (_selectedTaskIndex > scrollOffset)
                        {
                            _taskManager.RemoveTaskAt(_selectedTaskIndex);
                            _taskManager.InsertTask(--_selectedTaskIndex, task);

                            if (_dragging)
                            {
                                Game1.playSound("Cowboy_gunshot");
                            }
                        }

                        _dragging = true;
                    }
                    else if (y > taskY + scrollComponent.ScrollDistance)
                    {
                        if (_selectedTaskIndex < Math.Min(_taskManager.Tasks.Count - 1, scrollOffset + taskEntries.Count - 1))
                        {
                            _taskManager.RemoveTaskAt(_selectedTaskIndex);
                            _taskManager.InsertTask(++_selectedTaskIndex, task);

                            if (_dragging)
                            {
                                Game1.playSound("Cowboy_gunshot");
                            }
                        }

                        _dragging = true;
                    }
                }

                scrollComponent.LeftClickHeld(x, y);
            }
        }

        public override void releaseLeftClick(int x, int y)
        {
            if (!GameMenu.forcePreventClose)
            {
                if (ChildHasFocus())
                {
                    _childMenu.releaseLeftClick(x, y);
                    return;
                }

                if (_selectedTaskIndex != -1 && !_dragging)
                {
                    ITask task = _taskManager.Tasks[_selectedTaskIndex];
                    int entryIndex = _selectedTaskIndex - scrollComponent.ScrollAmount / scrollComponent.ScrollDistance;

                    if (Game1.options.SnappyMenus)
                    {
                        OpenTaskEntryMenu(taskEntries[entryIndex], task);
                    }
                    else
                    {
                        OpenTaskOptionsMenu(task);
                    }
                }

                scrollComponent.ReleaseLeftClick(x, y);
            }

            _selectedTaskIndex = -1;
            _dragging = false;
        }

        public override void receiveScrollWheelAction(int direction)
        {
            scrollComponent.Scroll(direction);
        }

        public override void receiveKeyPress(Keys key)
        {
            if (ChildHasFocus())
            {
                _childMenu.receiveKeyPress(key);
                return;
            }

            base.receiveKeyPress(key);
        }

        public override void applyMovementKey(int direction)
        {
            if (!ChildHasFocus())
            {
                base.applyMovementKey(direction);
            }
        }

        public override void performHoverAction(int x, int y)
        {
            base.performHoverAction(x, y);

            if (ChildHasFocus())
            {
                _childMenu.performHoverAction(x, y);
                return;
            }

            _moneyButtonVisible = moneyButton.containsPoint(x, y);

            if (_moneyButtonVisible)
            {
                HoverText = Translation.Get("ui.tasks.moneybutton.hover");
            }
            else if (moneyBox.containsPoint(x, y))
            {
                HoverText = Translation.Get("ui.tasks.moneybox." + (_config.MoneyViewNetWealth ? "hover1" : "hover0"));
            }
            else if (addTaskButton.containsPoint(x, y))
            {
                HoverText = Translation.Get("ui.tasks.addbutton.hover");
            }

            IReadOnlyList<ITask> tasks = _taskManager.Tasks;
            int scrollOffset = scrollComponent.ScrollAmount / scrollComponent.ScrollDistance;

            for (int i = 0; i < taskEntries.Count; i++)
            {
                TaskEntryComponent entry = taskEntries[i];

                if (entry.TryHover(x, y) && i + scrollOffset < tasks.Count && !_dragging)
                {
                    tasks[i + scrollOffset].MarkAsViewed();

                    if (!entry.containsPoint(Game1.getOldMouseX(), Game1.getOldMouseY()))
                    {
                        Game1.playSound("Cowboy_gunshot");
                    }

                    if (HoverText.Length == 0)
                    {
                        if (entry.checkbox.containsPoint(x, y) && !tasks[i + scrollOffset].Active)
                        {
                            HoverText = Translation.Get("ui.tasks.renewbutton.hover");
                        }
                        else if (entry.removeButton.containsPoint(x, y))
                        {
                            HoverText = Translation.Get("ui.tasks.removebutton.hover");
                        }
                        else if (entry.IsNameTruncated() && entry.TimeHovering() > 1.0)
                        {
                            HoverText = tasks[i + scrollOffset].Name;
                        }
                    }
                }
            }

            moneyButton.tryHover(x, y);
            addTaskButton.tryHover(x, y);
        }

        public override void update(GameTime time)
        {
            if (ChildHasFocus())
            {
                _childMenu.update(time);
            }
        }

        public override void draw(SpriteBatch b)
        {
            IReadOnlyList<ITask> tasks = _taskManager.Tasks;
            Vector2 moneyBoxPosition = new Vector2(moneyBox.bounds.X - 36, moneyBox.bounds.Y);

            scrollComponent.BeginScissorTest(b);

            if (tasks.Count > 0)
            {
                int scrollOffset = scrollComponent.ScrollAmount / scrollComponent.ScrollDistance;

                for (int i = 0; i < maxEntries && i + scrollOffset < tasks.Count; i++)
                {
                    taskEntries[i].Draw(b, tasks[i + scrollOffset]);
                }
            }
            else
            {
                string helpText = Translation.Get("ui.tasks.help");
                Vector2 helpTextSize = Game1.dialogueFont.MeasureString(helpText);
                Vector2 textPosition = new Vector2(xPositionOnScreen + (width / 2) - (helpTextSize.X / 2), yPositionOnScreen + (height / 2) - helpTextSize.Y);
                textPosition = new Vector2(textPosition.X - (textPosition.X % 4), textPosition.Y - (textPosition.Y % 4));

                Utility.drawTextWithShadow(b, helpText, Game1.dialogueFont, textPosition, EmptyListTextColor);
            }

            scrollComponent.EndScissorTest(b);
            scrollComponent.DrawScrollBar(b);

            addTaskButton.draw(b);
            b.Draw(DeluxeJournalMod.UiTexture,
                moneyBoxPosition,
                new Rectangle(16, 32, 68, 17),
                Color.White,
                0f, Vector2.Zero, 4f,
                SpriteEffects.None, 0.9f);

            if (_moneyButtonVisible)
            {
                moneyButton.draw(b);
            }

            int money = _config.MoneyViewNetWealth ? Game1.player.Money : 0;

            foreach (ITask task in tasks)
            {
                if (!task.Complete)
                {
                    money -= task.GetPrice();
                }
            }

            if (money < 0)
            {
                moneyDial.draw(b, moneyBoxPosition + new Vector2(68, 24), Math.Abs(money));

                int signOffset = (int)Math.Log10(moneyDial.currentValue) * 24;
                b.Draw(DeluxeJournalMod.UiTexture,
                    moneyBoxPosition + new Vector2(212 - signOffset, 24),
                    new Rectangle(91, 37, 5, 8),
                    Color.Maroon,
                    0f, Vector2.Zero, 4f,
                    SpriteEffects.None, 0.9f);
            }
            else
            {
                moneyDial.draw(b, moneyBoxPosition + new Vector2(68, 24), money);
            }

            if (ChildHasFocus())
            {
                _childMenu.draw(b);
            }
        }

        private void RefreshMoneyDial()
        {
            int money = _config.MoneyViewNetWealth ? Game1.player.Money : 0;

            foreach (ITask task in _taskManager.Tasks)
            {
                if (!task.Complete)
                {
                    money -= task.GetPrice();
                }
            }

            money = Math.Abs(money);
            moneyDial.currentValue = money;
            moneyDial.previousTargetValue = money;
        }
    }
}
