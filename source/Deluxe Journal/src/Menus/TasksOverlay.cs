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
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using DeluxeJournal.Events;
using DeluxeJournal.Framework.Events;
using DeluxeJournal.Framework.Task;
using DeluxeJournal.Task;
using DeluxeJournal.Task.Tasks;
using DeluxeJournal.Util;

namespace DeluxeJournal.Menus
{
    public class TasksOverlay : IOverlay
    {
        private readonly struct PrecomputedTaskInfo(ITask task, string name, Vector2 size)
        {
            /// <summary><see cref="ITask"/> reference.</summary>
            public readonly ITask Task { get; } = task;

            /// <summary><see cref="ITask.Name"/> truncated to fit the overlay bounds.</summary>
            public readonly string Name { get; } = name;

            /// <summary>Size of the <see cref="Name"/> string.</summary>
            public readonly Vector2 Size { get; } = size;

            /// <summary>Create a string representation of the current status of an <see cref="ITask"/>.</summary>
            /// <param name="task">Task whose status is to be stringified.</param>
            public static string CreateStatusString(ITask task)
            {
                if (!task.ShouldShowProgress() || task.ShouldShowCustomStatus())
                {
                    return string.Empty;
                }

                return $"({task.Count}/{task.MaxCount})";
            }
        }

        private const int CheckBoxSpacing = 28;
        private const int CollapseButtonSpacing = 24;

        private readonly IInputHelper _input;
        private readonly EventManager _events;
        private readonly TaskManager _taskManager;
        private readonly List<PrecomputedTaskInfo> _tasks;
        private readonly SpriteFontTools _fontTools;

        private Rectangle _contentBounds;
        private int _capacity;
        private bool _truncated;

        public override bool IsColorOptional => true;

        private int LineSpacing { get; }

        public TasksOverlay(Rectangle bounds, IInputHelper input) : base(bounds)
        {
            if (DeluxeJournalMod.EventManager is not EventManager events)
            {
                throw new InvalidOperationException($"{nameof(TasksOverlay)} created before instantiation of {nameof(EventManager)}");
            }

            if (DeluxeJournalMod.TaskManager is not TaskManager taskManager)
            {
                throw new InvalidOperationException($"{nameof(TasksOverlay)} created before instantiation of {nameof(TaskManager)}");
            }

            _input = input;
            _events = events;
            _taskManager = taskManager;
            _fontTools = new(Game1.smallFont);
            LineSpacing = (int)(_fontTools.LineSpacing * 1.5f);
            _capacity = (bounds.Height - 12) / LineSpacing;
            _tasks = new(_capacity);

            events.TaskListChanged.Add(OnTaskListChanged);
            events.TaskStatusChanged.Add(OnTaskStatusChanged);
            events.ModEvents.Input.ButtonPressed += OnButtonPressed;

            CalculateEdgeSnappedBounds();
            ReloadTasks();
        }

        ~TasksOverlay()
        {
            Dispose(false);
        }

        public void ReloadTasks()
        {
            int count = _taskManager.Tasks.Count;
            bool collapsed = false;
            bool hasGroupMembers = false;
            float maxNameWidth = 0f;
            float prevMaxNameWidth = 0f;
            float nameWidth;

            _tasks.Clear();
            _truncated = false;
            _contentBounds = EdgeSnappedBounds;
            _contentBounds.Height = 16;

            for (int i = 0; i < count; i++)
            {
                ITask task = _taskManager.Tasks[i];

                if (task.Active && !task.Complete)
                {
                    if (task.IsHeader)
                    {
                        if (!hasGroupMembers)
                        {
                            PopHeader();
                        }

                        if (i == count - 1)
                        {
                            break;
                        }

                        collapsed = task is HeaderTask header && header.IsCollapsed;
                    }
                    else if (collapsed)
                    {
                        hasGroupMembers = true;
                        continue;
                    }

                    string status = PrecomputedTaskInfo.CreateStatusString(task);
                    float xOffset = 16f + (task.IsHeader ? CollapseButtonSpacing : CheckBoxSpacing);

                    if (!string.IsNullOrEmpty(status))
                    {
                        status = " " + status;
                        xOffset += _fontTools.Font.MeasureString(status).X;
                    }

                    _truncated |= _fontTools.Truncate(task.Name, Size.X - xOffset, out string name);
                    _tasks.Add(new(task, name + status, _fontTools.Font.MeasureString(name + status)));
                    _contentBounds.Height += LineSpacing;
                    hasGroupMembers = !task.IsHeader;
                    prevMaxNameWidth = maxNameWidth;

                    if ((nameWidth = _fontTools.Font.MeasureString(name).X + xOffset) > maxNameWidth)
                    {
                        maxNameWidth = nameWidth;
                    }

                    if (_tasks.Count == _capacity)
                    {
                        break;
                    }
                }
                else if (i == count - 1 && (!collapsed || !hasGroupMembers))
                {
                    PopHeader();
                }
            }

            int contentWidth = (int)Math.Ceiling(maxNameWidth / 4f) * 4;
            int widthDelta = _contentBounds.Width - contentWidth;

            if (widthDelta > 8)
            {
                if (_contentBounds.X + _contentBounds.Width / 2 > Game1.uiViewport.Width / 2)
                {
                    _contentBounds.X += widthDelta;
                }

                _contentBounds.Width = contentWidth;
            }

            void PopHeader()
            {
                if (_tasks.Count > 0 && _tasks[^1].Task.IsHeader)
                {
                    maxNameWidth = prevMaxNameWidth;
                    _contentBounds.Height -= LineSpacing;
                    _tasks.RemoveAt(_tasks.Count - 1);
                }
            }
        }

        public override void Resize(int width, int height)
        {
            int oldEdgeSnappedWidth = EdgeSnappedBounds.Width;
            int capacity;

            base.Resize(width, height);
            capacity = Math.Max((Math.Min(height, EdgeSnappedBounds.Height) - 12) / LineSpacing, 1);

            // Resize shouldn't be called from the wrong game instance, but check just in case.
            if (ScreenId != Context.ScreenId)
            {
                return;
            }
            else if (capacity != _capacity && capacity <= _taskManager.Tasks.Count)
            {
                _capacity = capacity;
                ReloadTasks();
            }
            else if (oldEdgeSnappedWidth > EdgeSnappedBounds.Width || (_truncated && oldEdgeSnappedWidth < EdgeSnappedBounds.Width))
            {
                ReloadTasks();
            }
        }

        public override void DrawContents(SpriteBatch b)
        {
            if (_tasks.Count == 0)
            {
                return;
            }

            drawTextureBox(b,
                DeluxeJournalMod.UiTexture,
                BackgroundSource,
                _contentBounds.X,
                _contentBounds.Y,
                _contentBounds.Width,
                _contentBounds.Height,
                BackgroundColor,
                4f,
                false);

            DrawTaskNames(b);
        }

        public override void DrawInEditMode(SpriteBatch b)
        {
            drawTextureBox(b,
                DeluxeJournalMod.UiTexture,
                BackgroundSource,
                EdgeSnappedBounds.X,
                EdgeSnappedBounds.Y,
                EdgeSnappedBounds.Width,
                EdgeSnappedBounds.Height,
                BackgroundColor,
                4f,
                false);

            DrawTaskNames(b);

            drawTextureBox(b,
                DeluxeJournalMod.UiTexture,
                OutlineSource,
                _contentBounds.X,
                _contentBounds.Y,
                _contentBounds.Width,
                _contentBounds.Height,
                Color.Blue,
                2f,
                false);
        }

        private void DrawTaskNames(SpriteBatch b)
        {
            Vector2 mousePosition = new(Game1.getOldMouseX(), Game1.getOldMouseY());
            Vector2 namePosition = new(_contentBounds.X + 8, _contentBounds.Y + 8);
            float checkBoxOffsetY = Math.Max(_fontTools.LineSpacing - 22, 0);

            foreach (var taskInfo in _tasks)
            {
                ITask task = taskInfo.Task;
                string name = taskInfo.Name;
                int colorIndex = task.DisplayColorIndex;
                Color shadowColor = BackgroundColor;
                Color textColor;

                if (IsColorSelected)
                {
                    textColor = CustomColor;
                }
                else
                {
                    ColorSchema colorSchema = DeluxeJournalMod.ColorSchemas[colorIndex < DeluxeJournalMod.ColorSchemas.Count ? colorIndex : 0];
                    textColor = task.IsHeader ? colorSchema.Hover : colorSchema.Header;
                }

                if (task.IsHeader)
                {
                    bool hovering = new Rectangle(namePosition.ToPoint(), new((int)taskInfo.Size.X + CollapseButtonSpacing + 8, (int)taskInfo.Size.Y)).Contains(mousePosition);

                    b.DrawString(_fontTools.Font, name, new(namePosition.X - 2f, namePosition.Y + 3f), shadowColor);
                    b.DrawString(_fontTools.Font, name, new(namePosition.X, namePosition.Y + 3f), shadowColor);
                    Utility.drawBoldText(b, name, _fontTools.Font, namePosition, textColor);

                    if (task is HeaderTask header && ((IsContentInteractable && hovering) || header.IsCollapsed))
                    {
                        Utility.drawWithShadow(b,
                            DeluxeJournalMod.UiTexture,
                            new Vector2(namePosition.X + taskInfo.Size.X + (CollapseButtonSpacing + 8f) / 2f, namePosition.Y + taskInfo.Size.Y / 2f),
                            new(40, 50, 8, 5),
                            textColor,
                            header.IsCollapsed ? (float)Math.PI * 0.5f : 0f,
                            new(4f, 2.5f),
                            2f,
                            horizontalShadowOffset: -2,
                            verticalShadowOffset: 2,
                            shadowIntensity: BackgroundOpacity);
                    }
                }
                else
                {
                    Utility.drawTextWithColoredShadow(b, name, _fontTools.Font, new(namePosition.X + CheckBoxSpacing, namePosition.Y), textColor, shadowColor);

                    if (IsContentInteractable && new Rectangle((int)namePosition.X, (int)namePosition.Y, _contentBounds.Width - 16, LineSpacing).Contains(mousePosition))
                    {
                        Utility.drawWithShadow(b,
                            DeluxeJournalMod.UiTexture,
                            new Vector2(namePosition.X + 2f, namePosition.Y + checkBoxOffsetY),
                            new(39, 55, 9, 9),
                            textColor,
                            0f,
                            Vector2.Zero,
                            2f,
                            horizontalShadowOffset: -2,
                            verticalShadowOffset: 2,
                            shadowIntensity: BackgroundOpacity);

                        if (mousePosition.X >= namePosition.X && mousePosition.X < namePosition.X + CheckBoxSpacing)
                        {
                            b.Draw(DeluxeJournalMod.UiTexture,
                                new Vector2(namePosition.X + 12f, namePosition.Y + checkBoxOffsetY + 10f),
                                new(26, 25, 7, 7),
                                Color.White,
                                0f,
                                new(4f),
                                2f + Game1.dialogueButtonScale * 0.05f,
                                SpriteEffects.None,
                                0.8f);
                        }
                    }
                }

                namePosition.Y += LineSpacing;
            }
        }

        protected override void CalculateEdgeSnappedBounds()
        {
            base.CalculateEdgeSnappedBounds();
            _contentBounds.X = EdgeSnappedBounds.X;
            _contentBounds.Y = EdgeSnappedBounds.Y;

            if (EdgeSnappedBounds.X + EdgeSnappedBounds.Width / 2 > Game1.uiViewport.Width / 2)
            {
                _contentBounds.X += EdgeSnappedBounds.Width - _contentBounds.Width;
            }
        }

        protected override void cleanupBeforeExit()
        {
            _events.TaskListChanged.Remove(OnTaskListChanged);
            _events.TaskStatusChanged.Remove(OnTaskStatusChanged);
            _events.ModEvents.Input.ButtonPressed -= OnButtonPressed;
        }

        protected override void Dispose(bool disposing)
        {
            cleanupBeforeExit();
        }

        private void OnTaskListChanged(object? sender, TaskListChangedArgs e)
        {
            if (ScreenId == Context.ScreenId)
            {
                ReloadTasks();
            }
        }

        private void OnTaskStatusChanged(object? sender, TaskStatusChangedArgs e)
        {
            if (ScreenId != Context.ScreenId)
            {
                return;
            }

            if (e.OldActive != e.NewActive || e.OldComplete != e.NewComplete)
            {
                ReloadTasks();
            }
            else if (e.OldCount != e.NewCount)
            {
                for (int i = 0; i < _tasks.Count; i++)
                {
                    ITask task = _tasks[i].Task;

                    if (task != e.Task)
                    {
                        continue;
                    }

                    string status = PrecomputedTaskInfo.CreateStatusString(task);
                    float statusWidth = 0;

                    if (!string.IsNullOrEmpty(status))
                    {
                        status = " " + status;
                        statusWidth = _fontTools.Font.MeasureString(status).X;
                    }

                    _fontTools.Truncate(task.Name, Size.X - statusWidth - CheckBoxSpacing - 16f, out string name);
                    _tasks[i] = new(task, name + status, _fontTools.Font.MeasureString(name + status));
                    return;
                }
            }
        }

        private void OnButtonPressed(object? sender, ButtonPressedEventArgs e)
        {
            Vector2 mousePosition = e.Cursor.ScreenPixels;

            if (IsContentInteractable && e.Button == SButton.MouseLeft && _contentBounds.Contains(mousePosition))
            {
                int taskIndex = Math.Max((int)mousePosition.Y - _contentBounds.Y - 8, 0) / LineSpacing;

                if (taskIndex < _tasks.Count)
                {
                    ITask task = _tasks[taskIndex].Task;
                    Point taskPosition = new(_contentBounds.X + 8, _contentBounds.Y + taskIndex * LineSpacing + 8);
                    int taskWidth = (int)_tasks[taskIndex].Size.X;

                    if (task is HeaderTask header && new Rectangle(taskPosition.X + taskWidth + 8, taskPosition.Y, CollapseButtonSpacing, LineSpacing).Contains(mousePosition))
                    {
                        header.IsCollapsed = !header.IsCollapsed;
                        ReloadTasks();
                        Game1.playSound("smallSelect");
                    }
                    else if (!task.IsHeader && !task.Complete && new Rectangle(taskPosition.X, taskPosition.Y, CheckBoxSpacing, LineSpacing).Contains(mousePosition))
                    {
                        task.Complete = true;
                        task.MarkAsViewed();
                        Game1.playSound("tinyWhip", 2000);
                    }
                    else
                    {
                        return;
                    }

                    _input.Suppress(SButton.MouseLeft);
                }
            }
        }
    }
}
