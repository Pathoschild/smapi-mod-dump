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
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Menus;
using DeluxeJournal.Framework;
using DeluxeJournal.Framework.Task;
using DeluxeJournal.Menus.Components;
using DeluxeJournal.Task;

using static StardewValley.Menus.ClickableComponent;

namespace DeluxeJournal.Menus
{
    /// <summary>Tasks page.</summary>
    public class TasksPage : PageBase
    {
        /// <summary>Maximum viewable entry slots.</summary>
        private const int MaxEntries = 8;

        private const int FilterTabId = 300;

        public static readonly Color EmptyListTextColor = new Color(86, 22, 12);

        private static readonly Rectangle LightFilterTabSource = new(96, 0, 16, 16);
        private static readonly Rectangle DarkFilterTabSource = new(112, 0, 16, 16);
        private static readonly PerScreen<int> ScrollAmountPerScreen = new();

        public readonly List<TaskEntryComponent> taskEntries;
        public readonly List<ClickableTextureComponent> filterTabs;
        public readonly ClickableTextureComponent addTaskButton;
        public readonly ClickableTextureComponent moneyButton;
        public readonly ClickableComponent moneyBox;
        public readonly MoneyDial moneyDial;
        public readonly ScrollComponent scrollComponent;

        private readonly ITranslationHelper _translation;
        private readonly Config _config;
        private readonly TaskManager _taskManager;
        private readonly Rectangle _boundsWithScrollBar;
        private readonly double _dragScrollInterval;
        private double _dragScrollStartTime;
        private int _currentlySnappedEntryId;
        private int _selectedTaskIndex;
        private bool _filterTabsVisible;
        private bool _moneyButtonVisible;
        private bool _dragging;

        /// <summary>The currently selected task filter tab index.</summary>
        public static int SelectedFilterTab { get; private set; } = 0;

        /// <summary>A list of tasks filtered by the selected filter tab rules.</summary>
        public IList<ITask> FilteredTasks { get; private set; }

        public TasksPage(string name, Rectangle bounds, Texture2D tabTexture, ITranslationHelper translation)
            : this(name, translation.Get("ui.tab.tasks"), bounds.X, bounds.Y, bounds.Width, bounds.Height, tabTexture, new Rectangle(16, 0, 16, 16), translation)
        {
        }

        public TasksPage(string name, string title, int x, int y, int width, int height, Texture2D tabTexture, Rectangle tabSourceRect, ITranslationHelper translation)
            : base(name, title, x, y, width, height, tabTexture, tabSourceRect)
        {
            if (DeluxeJournalMod.Config is not Config config)
            {
                throw new InvalidOperationException($"{nameof(TasksPage)} created before mod entry.");
            }

            if (DeluxeJournalMod.TaskManager is not TaskManager taskManager)
            {
                throw new InvalidOperationException($"{nameof(TasksPage)} created before instantiation of {nameof(TaskManager)}");
            }
            
            _translation = translation;
            _config = config;
            _taskManager = taskManager;
            _dragScrollInterval = 0.16;
            _dragScrollStartTime = 0;
            _currentlySnappedEntryId = 0;
            _selectedTaskIndex = -1;
            _filterTabsVisible = false;
            _dragging = false;
            taskEntries = [];
            filterTabs = [];
            moneyDial = new MoneyDial(8, false);
            FilteredTasks = Array.Empty<ITask>();

            Rectangle entryBounds = new(x + 16, y + 16, width - 32, (height - 32) / MaxEntries);

            for (int i = 0; i < MaxEntries; i++)
            {
                entryBounds.Y = y + 20 + i * entryBounds.Height;
                taskEntries.Add(new TaskEntryComponent(entryBounds, i.ToString(), _translation)
                {
                    myID = i,
                    upNeighborID = CUSTOM_SNAP_BEHAVIOR,
                    downNeighborID = CUSTOM_SNAP_BEHAVIOR,
                    rightNeighborID = CUSTOM_SNAP_BEHAVIOR,
                    leftNeighborID = CUSTOM_SNAP_BEHAVIOR,
                    fullyImmutable = true
                });
            }

            foreach (ITask.Period period in Enum.GetValues<ITask.Period>())
            {
                filterTabs.Add(new ClickableTextureComponent(
                    new(x + 16 + (int)period * 64, y + height, 64, 64),
                    DeluxeJournalMod.UiTexture,
                    DarkFilterTabSource,
                    4f)
                {
                    myID = FilterTabId + (int)period,
                    upNeighborID = CUSTOM_SNAP_BEHAVIOR,
                    fullyImmutable = true,
                    hoverText = translation.Get("ui.tasks.filter", new
                    {
                        category = translation.Get(period == ITask.Period.Never
                            ? "ui.tasks.filter.all"
                            : "ui.tasks.options.renew." + Enum.GetName(period))
                    })
                });
            }

            addTaskButton = new ClickableTextureComponent(
                new(x + width - 336, y + height, 60, 68),
                DeluxeJournalMod.UiTexture,
                new(0, 32, 15, 17),
                4f)
            {
                myID = 1000,
                upNeighborID = CUSTOM_SNAP_BEHAVIOR,
                leftNeighborID = CUSTOM_SNAP_BEHAVIOR,
                rightNeighborID = 1001,
                fullyImmutable = true
            };

            moneyButton = new ClickableTextureComponent(
                new(x + width - 260, y + height + 20, 24, 36),
                DeluxeJournalMod.UiTexture,
                new(85, 37, 6, 9),
                4f)
            {
                myID = 1001,
                upNeighborID = CUSTOM_SNAP_BEHAVIOR,
                rightNeighborID = 1002,
                leftNeighborID = 1000,
                fullyImmutable = true
            };

            moneyBox = new ClickableComponent(new(x + width - 236, y + height, 236, 68), "moneyBox")
            {
                myID = 1002,
                upNeighborID = CUSTOM_SNAP_BEHAVIOR,
                leftNeighborID = 1001,
                fullyImmutable = true
            };

            Rectangle scrollBarBounds = new(x + width + 16, y + 148, 24, height - 216);
            Rectangle scrollContentBounds = new(x, y + 16, width, height - 32);

            scrollComponent = new ScrollComponent(scrollBarBounds, scrollContentBounds, (height - 32) / 8, true);
            scrollComponent.ContentHeight = _taskManager.Tasks.Count * scrollComponent.ScrollDistance;
            scrollComponent.ScrollAmount = ScrollAmountPerScreen.Value;
            scrollComponent.OnScroll += (self) => ScrollAmountPerScreen.Value = self.ScrollAmount;

            _boundsWithScrollBar = new(x, y, scrollBarBounds.Right - x + 16, height);

            SelectFilterTab(SelectedFilterTab, false, false);
            ReloadFilteredTasks();
        }

        public void OpenAddTaskMenu()
        {
            SetSnappyChildMenu(new AddTaskMenu(_translation));
        }

        public void OpenTaskOptionsMenu(ITask task)
        {
            SetSnappyChildMenu(new TaskOptionsMenu(task, _translation));
        }

        private void OpenTaskEntryMenu(TaskEntryComponent entry, ITask task)
        {
            SetSnappyChildMenu(new TaskEntryMenu(entry, task, _translation));
        }

        public void OnTasksUpdated()
        {
            ReloadFilteredTasks();
            UpdateFilterTabs();
        }

        public void AddTask(ITask task)
        {
            int i = 0;

            for (; i < _taskManager.Tasks.Count; i++)
            {
                if (!_taskManager.Tasks[i].IsHeader)
                {
                    break;
                }
            }

            if (task.IsHeader || (int)task.RenewPeriod != SelectedFilterTab)
            {
                SelectFilterTab(0, false);
            }

            _taskManager.Tasks.Insert(i, task);
            _config.ShowAddTaskHelpMessage = false;
            scrollComponent.ContentHeight += scrollComponent.ScrollDistance;
            OnTasksUpdated();
        }

        public void RemoveTask(ITask task)
        {
            _taskManager.Tasks.Remove(task);
            scrollComponent.ContentHeight -= scrollComponent.ScrollDistance;
            scrollComponent.ScrollAmount -= scrollComponent.ScrollDistance;
            OnTasksUpdated();
        }

        public void SortTasks()
        {
            _taskManager.SortTasks();
            ReloadFilteredTasks();
        }

        public void ReloadFilteredTasks(bool resetScroll = false)
        {
            bool skipHeader = true;

            FilteredTasks = SelectedFilterTab == 0
                ? _taskManager.Tasks
                : _taskManager.Tasks.Reverse().Where(reverseFilterPredicate).Reverse().ToList();
            
            scrollComponent.ContentHeight = FilteredTasks.Count * scrollComponent.ScrollDistance;

            if (resetScroll)
            {
                scrollComponent.ScrollAmount = 0;
            }

            _taskManager.RefreshGroups();

            bool reverseFilterPredicate(ITask task)
            {
                if (task.IsHeader)
                {
                    if (!skipHeader)
                    {
                        skipHeader = true;
                        return true;
                    }
                }
                else if ((int)task.RenewPeriod == SelectedFilterTab)
                {
                    skipHeader = false;
                    return true;
                }

                return false;
            }
        }

        public override void OnHidden()
        {
            foreach (ITask task in FilteredTasks)
            {
                task.MarkAsViewed();
            }

            SortTasks();
        }

        public override void OnVisible()
        {
            _selectedTaskIndex = -1;
            allClickableComponents.AddRange(taskEntries);
            SortTasks();
            RefreshMoneyDial();
            UpdateFilterTabs();
        }

        public override bool readyToClose()
        {
            return base.readyToClose() && (_childMenu == null || _childMenu.readyToClose());
        }

        protected override void customSnapBehavior(int direction, int oldRegion, int oldID)
        {
            int filledEntries = Math.Min(MaxEntries, FilteredTasks.Count);

            switch (direction)
            {
                case Game1.up:
                    if (filledEntries > 0)
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
                    if (oldID == addTaskButton.myID && _filterTabsVisible)
                    {
                        for (int i = filterTabs.Count - 1; i >= 0; i--)
                        {
                            if (filterTabs[i].visible)
                            {
                                currentlySnappedComponent = filterTabs[i];
                                snapCursorToCurrentSnappedComponent();
                                return;
                            }
                        }
                    }

                    SnapToActiveTabComponent();
                    return;
            }

            if (oldID >= 0 && oldID < filledEntries)
            {
                _currentlySnappedEntryId = currentlySnappedComponent.myID;
            }
        }

        public override void snapToDefaultClickableComponent()
        {
            currentlySnappedComponent = (FilteredTasks.Count > 0) ? getComponentWithID(_currentlySnappedEntryId) : addTaskButton;
            snapCursorToCurrentSnappedComponent();
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
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
            else if (moneyBox.containsPoint(x, y))
            {
                RefreshMoneyDial();
            }
            else
            {
                int scrollOffset = scrollComponent.GetScrollOffset();

                for (int i = 0; i < taskEntries.Count && i + scrollOffset < FilteredTasks.Count; i++)
                {
                    TaskEntryComponent entry = taskEntries[i];

                    if (entry.containsPoint(x, y))
                    {
                        ITask task = FilteredTasks[i + scrollOffset];

                        if (entry.checkbox.containsPoint(x, y))
                        {
                            if (task.Active)
                            {
                                task.Complete = !task.Complete;
                                task.MarkAsViewed();
                                Game1.playSound("tinyWhip", task.Complete ? 2000 : 1000);
                            }
                            else
                            {
                                task.Active = true;
                                Game1.playSound("newRecipe");
                            }
                        }
                        else if (entry.removeButton.containsPoint(x, y))
                        {
                            RemoveTask(task);
                            Game1.playSound("woodyStep");
                        }
                        else
                        {
                            _selectedTaskIndex = i + scrollOffset;
                        }

                        return;
                    }
                }

                for (int i = 0; i < filterTabs.Count; i++)
                {
                    if (filterTabs[i].containsPoint(x, y))
                    {
                        SelectFilterTab(i, playSound);
                        return;
                    }
                }

                if (!scrollComponent.CanScroll || !_boundsWithScrollBar.Contains(x, y))
                {
                    ExitJournalMenu(playSound);
                }
            }

            scrollComponent.ReceiveLeftClick(x, y, playSound);
        }

        public override void leftClickHeld(int x, int y)
        {
            if (!GameMenu.forcePreventClose)
            {
                if (_selectedTaskIndex >= 0 && _selectedTaskIndex < FilteredTasks.Count)
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

                    IList<ITask> tasks = _taskManager.Tasks;
                    ITask task = FilteredTasks[_selectedTaskIndex];
                    int taskY = scrollComponent.ContentBounds.Y + (_selectedTaskIndex * scrollComponent.ScrollDistance - scrollComponent.ScrollAmount);
                    int scrollOffset = scrollComponent.GetScrollOffset();
                    int nextTaskIndex;

                    if (y < taskY)
                    {
                        if (_selectedTaskIndex > scrollOffset)
                        {
                            nextTaskIndex = tasks.IndexOf(FilteredTasks[--_selectedTaskIndex]);
                            tasks.Remove(task);
                            tasks.Insert(nextTaskIndex, task);

                            if (SelectedFilterTab > 0)
                            {
                                FilteredTasks.RemoveAt(_selectedTaskIndex + 1);
                                FilteredTasks.Insert(_selectedTaskIndex, task);
                            }

                            _taskManager.RefreshGroups();

                            if (_dragging)
                            {
                                Game1.playSound("Cowboy_gunshot");
                            }
                        }

                        _dragging = true;
                    }
                    else if (y > taskY + scrollComponent.ScrollDistance)
                    {
                        if (_selectedTaskIndex < Math.Min(FilteredTasks.Count, scrollOffset + taskEntries.Count) - 1)
                        {
                            nextTaskIndex = tasks.IndexOf(FilteredTasks[++_selectedTaskIndex]);
                            tasks.Remove(task);
                            tasks.Insert(nextTaskIndex, task);

                            if (SelectedFilterTab > 0)
                            {
                                FilteredTasks.RemoveAt(_selectedTaskIndex - 1);
                                FilteredTasks.Insert(_selectedTaskIndex, task);
                            }

                            _taskManager.RefreshGroups();

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
                if (_selectedTaskIndex >= 0 && _selectedTaskIndex < FilteredTasks.Count && !_dragging)
                {
                    ITask task = FilteredTasks[_selectedTaskIndex];
                    int entryIndex = _selectedTaskIndex - scrollComponent.GetScrollOffset();

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
            base.receiveKeyPress(key);

            if (GetChildMenu() == null)
            {
                if (Game1.options.SnappyMenus && Game1.options.doesInputListContain(Game1.options.useToolButton, key))
                {
                    currentlySnappedComponent = addTaskButton;
                    snapCursorToCurrentSnappedComponent();
                }

                switch (key)
                {
                    case Keys.Space:
                        OpenAddTaskMenu();
                        break;
                }
            }
        }

        public override void performHoverAction(int x, int y)
        {
            base.performHoverAction(x, y);

            _moneyButtonVisible = moneyButton.containsPoint(x, y);

            if (_moneyButtonVisible)
            {
                HoverText = _translation.Get("ui.tasks.moneybutton.hover");
            }
            else if (moneyBox.containsPoint(x, y))
            {
                HoverText = _translation.Get("ui.tasks.moneybox." + (_config.MoneyViewNetWealth ? "hover1" : "hover0"));
            }
            else if (addTaskButton.containsPoint(x, y))
            {
                HoverText = _translation.Get("ui.tasks.addbutton.hover");
            }

            int scrollOffset = scrollComponent.GetScrollOffset();

            for (int i = 0; i < taskEntries.Count; i++)
            {
                TaskEntryComponent entry = taskEntries[i];

                if (entry.TryHover(x, y) && i + scrollOffset < FilteredTasks.Count && !_dragging)
                {
                    ITask task = FilteredTasks[i + scrollOffset];
                    task.MarkAsViewed();

                    if (!entry.containsPoint(Game1.getOldMouseX(), Game1.getOldMouseY()))
                    {
                        Game1.playSound("Cowboy_gunshot");
                    }

                    if (HoverText.Length == 0)
                    {
                        if (entry.checkbox.containsPoint(x, y) && !task.Active)
                        {
                            HoverText = _translation.Get("ui.tasks.renewbutton.hover");
                        }
                        else if (entry.removeButton.containsPoint(x, y))
                        {
                            HoverText = _translation.Get("ui.tasks.removebutton.hover");
                        }
                        else if (entry.IsNameTruncated && entry.TimeHovering() > 1.0)
                        {
                            HoverText = task.Name;
                        }
                    }
                }
            }

            moneyButton.tryHover(x, y);
            addTaskButton.tryHover(x, y);

            if (HoverText.Length == 0)
            {
                foreach (var filterTab in filterTabs)
                {
                    if (filterTab.containsPoint(x, y))
                    {
                        HoverText = filterTab.hoverText;
                        return;
                    }
                }
            }
        }

        public override void draw(SpriteBatch b)
        {
            Rectangle moneyBoxDrawBounds = new(moneyBox.bounds.X - 36, moneyBox.bounds.Y, moneyBox.bounds.Width + 36, moneyBox.bounds.Height);

            scrollComponent.BeginScissorTest(b);

            if (FilteredTasks.Count > 0)
            {
                int scrollOffset = scrollComponent.GetScrollOffset();

                for (int i = 0; i < MaxEntries && i + scrollOffset < FilteredTasks.Count; i++)
                {
                    ITask task = FilteredTasks[i + scrollOffset];
                    int colorIndex = task.DisplayColorIndex;

                    taskEntries[i].Draw(b, task, DeluxeJournalMod.ColorSchemas[colorIndex < DeluxeJournalMod.ColorSchemas.Count ? colorIndex : 0]);
                }
            }
            else if (_config.ShowAddTaskHelpMessage)
            {
                string helpText = _translation.Get("ui.tasks.help");
                Vector2 helpTextSize = Game1.dialogueFont.MeasureString(helpText);
                Vector2 textPosition = new(xPositionOnScreen + (width / 2) - (helpTextSize.X / 2), yPositionOnScreen + (height / 2) - helpTextSize.Y);
                textPosition = new(textPosition.X - (textPosition.X % 4), textPosition.Y - (textPosition.Y % 4));

                Utility.drawTextWithShadow(b, helpText, Game1.dialogueFont, textPosition, EmptyListTextColor);
            }

            scrollComponent.EndScissorTest(b);
            scrollComponent.DrawScrollBar(b);

            if (_filterTabsVisible)
            {
                for (int i = 0; i < filterTabs.Count; i++)
                {
                    var filterTab = filterTabs[i];

                    if (filterTab.visible)
                    {
                        filterTab.draw(b);
                        b.Draw(DeluxeJournalMod.UiTexture,
                            new Rectangle(filterTab.bounds.X + 16, filterTab.bounds.Y + 6, 32, 32),
                            new(96 + 16 * (i % 2), 16 + 16 * (i / 2), 16, 16),
                            Color.White);
                    }
                }
            }

            addTaskButton.draw(b);
            b.Draw(DeluxeJournalMod.UiTexture,
                new Rectangle(addTaskButton.bounds.X + 16, addTaskButton.bounds.Y + 24, 28, 28),
                new(0, 49, 7, 7),
                Color.White);

            b.Draw(DeluxeJournalMod.UiTexture,
                moneyBoxDrawBounds,
                new(16, 32, 68, 17),
                Color.White);

            if (_moneyButtonVisible)
            {
                moneyButton.draw(b);
            }

            int money = _config.MoneyViewNetWealth ? Game1.player.Money : 0;

            foreach (ITask task in FilteredTasks)
            {
                if (!task.Complete)
                {
                    money -= task.GetPrice();
                }
            }

            moneyDial.draw(b, new Vector2(moneyBoxDrawBounds.X + 68, moneyBoxDrawBounds.Y + 24), Math.Abs(money));

            if (money < 0)
            {
                int signOffset = (int)Math.Log10(moneyDial.currentValue) * 24;
                b.Draw(DeluxeJournalMod.UiTexture,
                    new Rectangle(moneyBoxDrawBounds.Right - signOffset - 60, moneyBoxDrawBounds.Y + 24, 20, 32),
                    new(91, 37, 5, 8),
                    Color.Maroon);
            }
        }

        private void RefreshMoneyDial()
        {
            int money = _config.MoneyViewNetWealth ? Game1.player.Money : 0;

            foreach (ITask task in FilteredTasks)
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

        private void SelectFilterTab(int index, bool playSound = true, bool deselectPrevious = true)
        {
            if (index < 0 || index >= filterTabs.Count)
            {
                return;
            }

            if (playSound)
            {
                Game1.playSound("smallSelect");
            }

            if (deselectPrevious)
            {
                filterTabs[SelectedFilterTab].bounds.Y += DeluxeJournalMenu.ActiveTabOffset;
                filterTabs[SelectedFilterTab].sourceRect = DarkFilterTabSource;
            }

            filterTabs[index].bounds.Y -= DeluxeJournalMenu.ActiveTabOffset;
            filterTabs[index].sourceRect = LightFilterTabSource;

            if (SelectedFilterTab != (SelectedFilterTab = index))
            {
                ReloadFilteredTasks(true);
            }
        }

        private void UpdateFilterTabs()
        {
            _filterTabsVisible = false;

            for (int i = 1; i < filterTabs.Count; i++)
            {
                filterTabs[i].visible = false;
            }

            foreach (ITask task in _taskManager.Tasks)
            {
                if (task.RenewPeriod == ITask.Period.Never)
                {
                    continue;
                }

                int index = (int)task.RenewPeriod;
                var tab = filterTabs[index];

                if (!tab.visible)
                {
                    tab.sourceRect = index == SelectedFilterTab ? LightFilterTabSource : DarkFilterTabSource;
                    tab.visible = true;
                    _filterTabsVisible = true;
                }
            }

            if (filterTabs[0].visible = _filterTabsVisible)
            {
                var visibleTabs = filterTabs.Where(tab => tab.visible).ToList();
                ChainNeighborsLeftRight(visibleTabs);
                visibleTabs.First().leftNeighborID = CUSTOM_SNAP_BEHAVIOR;
                visibleTabs.Last().rightNeighborID = addTaskButton.myID;
            }

            for (int i = 1, j = 1; i < filterTabs.Count; i++)
            {
                var tab = filterTabs[i];

                if (tab.visible)
                {
                    tab.bounds.X = xPositionOnScreen + 16 + tab.bounds.Width * j++;
                }
            }

            if (SelectedFilterTab > 0 && !filterTabs[SelectedFilterTab].visible)
            {
                SelectFilterTab(0, false);
            }
        }
    }
}
