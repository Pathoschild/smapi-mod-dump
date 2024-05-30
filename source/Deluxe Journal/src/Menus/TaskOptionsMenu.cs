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

using DeluxeJournal.Framework.Task;
using DeluxeJournal.Menus.Components;
using DeluxeJournal.Task;
using DeluxeJournal.Task.Tasks;

using Period = DeluxeJournal.Task.ITask.Period;
using static StardewValley.Menus.ClickableComponent;
using static DeluxeJournal.Task.TaskParameterAttribute;

namespace DeluxeJournal.Menus
{
    /// <summary>TasksPage child menu for editing task options.</summary>
    public class TaskOptionsMenu : IClickableMenu
    {
        private const int LabelWidth = 256;
        private const int VerticalSpacing = 64;
        private const int BottomGap = 32;

        private const int ParameterTextBoxId = 1000;
        private const int ParameterDropDownId = 1100;
        private const int ParameterIconId = 900;

        public readonly ClickableTextureComponent backButton;
        public readonly ClickableTextureComponent cancelButton;
        public readonly ClickableTextureComponent okButton;

        public readonly ClickableComponent nameTextBoxCC;
        public readonly ClickableComponent customRenewTextBoxCC;
        public readonly DropDownComponent renewPeriodDropDown;
        public readonly DropDownComponent weekdaysDropDown;
        public readonly DropDownComponent seasonsDropDown;
        public readonly DropDownComponent daysDropDown;
        public readonly List<ClickableComponent> parameterTextBoxCCs;
        public readonly List<ClickableComponent> typeIcons;

        private readonly SideScrollingTextBox _nameTextBox;
        private readonly SideScrollingTextBox _customRenewTextBox;
        private readonly IList<TaskParameterTextBox> _parameterTextBoxes;
        private readonly IDictionary<int, TaskParameterDropDown> _parameterDropDowns;
        private readonly IDictionary<int, SmartIconComponent> _parameterIcons;

        private readonly ITranslationHelper _translation;
        private readonly Texture2D _textBoxTexture;
        private readonly Rectangle _fixedContentBounds;
        private readonly ITask? _task;

        private Task.TaskFactory? _taskFactory;
        private DropDownComponent? _activeDropDown;
        private string _selectedTaskID;
        private string _hoverText;

        public TaskOptionsMenu(ITask task, ITranslationHelper translation) : this(translation)
        {
            _task = task;
            _selectedTaskID = task.ID;
            _taskFactory = TaskRegistry.CreateFactoryInstance(task.ID);
            _taskFactory.Initialize(task, translation);

            _nameTextBox.Text = task.Name;
            renewPeriodDropDown.SelectedOption = (int)_task.RenewPeriod;
            
            if (_task.RenewPeriod == Period.Custom)
            {
                _customRenewTextBox.Text = _task.RenewCustomInterval.ToString();
            }
            else if (_task.RenewPeriod != Period.Never)
            {
                weekdaysDropDown.SelectedOption = (_task.RenewDate.DayOfMonth - 1) % 7;

                if (_task.RenewPeriod != Period.Weekly)
                {
                    daysDropDown.SelectedOption = _task.RenewDate.DayOfMonth - 1;
                    seasonsDropDown.SelectedOption = _task.RenewDate.SeasonIndex;
                }
            }

            SetupParameters();
        }

        public TaskOptionsMenu(string taskName, TaskParser taskParser, ITranslationHelper translation)
            : this(translation)
        {
            _selectedTaskID = taskParser.ID;
            _taskFactory = taskParser.Factory;
            _nameTextBox.Text = taskName;

            SetupParameters();
        }

        private TaskOptionsMenu(ITranslationHelper translation) : base(0, 0, 928, 576)
        {
            xPositionOnScreen = (Game1.uiViewport.Width / 2) - (width / 2);
            yPositionOnScreen = (Game1.uiViewport.Height / 2) - (height / 2);

            _translation = translation;
            _textBoxTexture = Game1.content.Load<Texture2D>("LooseSprites\\textBox");
            _activeDropDown = null;
            _taskFactory = null;
            _task = null;
            _selectedTaskID = TaskTypes.Basic;
            _hoverText = string.Empty;

            _fixedContentBounds = default;
            _fixedContentBounds.X = xPositionOnScreen + spaceToClearSideBorder + 28;
            _fixedContentBounds.Y = yPositionOnScreen + spaceToClearTopBorder + 16;
            _fixedContentBounds.Width = width - (_fixedContentBounds.X - xPositionOnScreen) * 2;

            _parameterIcons = new Dictionary<int, SmartIconComponent>();
            _parameterDropDowns = new Dictionary<int, TaskParameterDropDown>();
            _parameterTextBoxes = new List<TaskParameterTextBox>();
            parameterTextBoxCCs = new List<ClickableComponent>();
            typeIcons = new List<ClickableComponent>();

            _nameTextBox = new SideScrollingTextBox(_textBoxTexture, null, Game1.smallFont, Game1.textColor)
            {
                X = _fixedContentBounds.X + LabelWidth,
                Y = _fixedContentBounds.Y,
                Width = _fixedContentBounds.Width - LabelWidth
            };

            nameTextBoxCC = new ClickableComponent(_nameTextBox.Bounds, string.Empty)
            {
                myID = 100,
                downNeighborID = 101,
                rightNeighborID = SNAP_AUTOMATIC,
                leftNeighborID = SNAP_AUTOMATIC,
                fullyImmutable = true
            };

            string[] weekdayNames = new[]
            {
                Game1.content.LoadString("Strings\\StringsFromCSFiles:Game1.cs.3043"),
                Game1.content.LoadString("Strings\\StringsFromCSFiles:Game1.cs.3044"),
                Game1.content.LoadString("Strings\\StringsFromCSFiles:Game1.cs.3045"),
                Game1.content.LoadString("Strings\\StringsFromCSFiles:Game1.cs.3046"),
                Game1.content.LoadString("Strings\\StringsFromCSFiles:Game1.cs.3047"),
                Game1.content.LoadString("Strings\\StringsFromCSFiles:Game1.cs.3048"),
                Game1.content.LoadString("Strings\\StringsFromCSFiles:Game1.cs.3042")
            };

            string[] seasonNames = new[]
            {
                Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5680"),
                Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5681"),
                Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5682"),
                Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5683")
            };

            string[] seasonKeys = new[]
            {
                nameof(Season.Spring),
                nameof(Season.Summer),
                nameof(Season.Fall),
                nameof(Season.Winter)
            };

            IEnumerable<string> renewOptions = Enum.GetNames<Period>()
                .Select(period => translation.Get("ui.tasks.options.renew." + period).ToString());

            renewPeriodDropDown = new DropDownComponent(renewOptions,
                new(_nameTextBox.X + 8, _nameTextBox.Y + VerticalSpacing, 0, 44),
                string.Empty)
            {
                myID = 101,
                upNeighborID = SNAP_AUTOMATIC,
                downNeighborID = CUSTOM_SNAP_BEHAVIOR,
                rightNeighborID = SNAP_AUTOMATIC,
                leftNeighborID = SNAP_AUTOMATIC
            };

            weekdaysDropDown = new DropDownComponent(weekdayNames,
                new(renewPeriodDropDown.bounds.Right + 8, renewPeriodDropDown.bounds.Y, 0, renewPeriodDropDown.bounds.Height),
                string.Empty)
            {
                myID = 102,
                upNeighborID = SNAP_AUTOMATIC,
                downNeighborID = CUSTOM_SNAP_BEHAVIOR,
                rightNeighborID = SNAP_AUTOMATIC,
                leftNeighborID = SNAP_AUTOMATIC
            };
            
            seasonsDropDown = new DropDownComponent(seasonKeys, seasonNames,
                new(renewPeriodDropDown.bounds.Right + 8, renewPeriodDropDown.bounds.Y, 0, renewPeriodDropDown.bounds.Height),
                string.Empty)
            {
                myID = 103,
                upNeighborID = SNAP_AUTOMATIC,
                downNeighborID = CUSTOM_SNAP_BEHAVIOR,
                rightNeighborID = SNAP_AUTOMATIC,
                leftNeighborID = SNAP_AUTOMATIC
            };
            
            daysDropDown = new DropDownComponent(Enumerable.Range(1, 28).Select(i => i.ToString()),
                new(seasonsDropDown.bounds.Right + 8, seasonsDropDown.bounds.Y, 112, seasonsDropDown.bounds.Height),
                string.Empty,
                DropDownComponent.WrapStyle.Horizontal,
                wrapLimit: 7,
                fixedWidth: true)
            {
                myID = 104,
                upNeighborID = SNAP_AUTOMATIC,
                downNeighborID = CUSTOM_SNAP_BEHAVIOR,
                rightNeighborID = SNAP_AUTOMATIC,
                leftNeighborID = SNAP_AUTOMATIC
            };

            _customRenewTextBox = new SideScrollingTextBox(_textBoxTexture, null, Game1.smallFont, Game1.textColor)
            {
                X = weekdaysDropDown.bounds.X,
                Y = weekdaysDropDown.bounds.Y,
                Width = _fixedContentBounds.Width + _fixedContentBounds.X - weekdaysDropDown.bounds.X,
                numbersOnly = true
            };

            customRenewTextBoxCC = new ClickableComponent(_customRenewTextBox.Bounds, string.Empty)
            {
                myID = 105,
                upNeighborID = SNAP_AUTOMATIC,
                downNeighborID = CUSTOM_SNAP_BEHAVIOR,
                rightNeighborID = SNAP_AUTOMATIC,
                leftNeighborID = SNAP_AUTOMATIC
            };

            Rectangle iconBounds = new Rectangle(0, 0, 56, 56);
            int offset = 0;
            int snapId = 0;

            foreach (string id in TaskRegistry.Keys)
            {
                iconBounds.X = _fixedContentBounds.X + LabelWidth + 12 + (offset % 576);
                iconBounds.Y = _fixedContentBounds.Y + 20 + VerticalSpacing * (2 + (offset / 576));

                typeIcons.Add(new ClickableComponent(iconBounds, id)
                {
                    myID = snapId,
                    upNeighborID = SNAP_AUTOMATIC,
                    downNeighborID = CUSTOM_SNAP_BEHAVIOR,
                    rightNeighborID = snapId == TaskRegistry.Count - 1 ? 106 : snapId + 1,
                    leftNeighborID = snapId == 0 ? 105 : snapId - 1,
                    fullyImmutable = true
                });

                offset += 64;
                snapId++;
            }

            _fixedContentBounds.Height = typeIcons.Last().bounds.Y + VerticalSpacing + 4 - _fixedContentBounds.Y;
            height = _fixedContentBounds.Bottom + BottomGap - xPositionOnScreen;

            backButton = new ClickableTextureComponent(
                new Rectangle(xPositionOnScreen - 64, _fixedContentBounds.Y + 12, 64, 32),
                Game1.mouseCursors,
                new Rectangle(352, 495, 12, 11),
                4f)
            {
                myID = 105,
                downNeighborID = 0,
                rightNeighborID = nameTextBoxCC.myID,
                fullyImmutable = true
            };

            cancelButton = new ClickableTextureComponent(
                new Rectangle(xPositionOnScreen + width - 12, _fixedContentBounds.Y, 64, 64),
                DeluxeJournalMod.UiTexture,
                new Rectangle(0, 80, 16, 16),
                4f)
            {
                myID = 106,
                downNeighborID = 107,
                leftNeighborID = nameTextBoxCC.myID,
                fullyImmutable = true
            };

            okButton = new ClickableTextureComponent(
                new Rectangle(xPositionOnScreen + width - 100, yPositionOnScreen + height - 4, 64, 64),
                DeluxeJournalMod.UiTexture,
                new Rectangle(32, 80, 16, 16),
                4f)
            {
                myID = 107,
                upNeighborID = CUSTOM_SNAP_BEHAVIOR,
                rightNeighborID = cancelButton.myID,
                fullyImmutable = true
            };

            Game1.playSound("shwip");

            exitFunction = OnExit;
        }

        public void RecalculateBounds()
        {
            height = _fixedContentBounds.Bottom + BottomGap - yPositionOnScreen;

            if (_taskFactory != null)
            {
                foreach (TaskParameter parameter in _taskFactory.GetParameters())
                {
                    if (!parameter.Attribute.Hidden && string.IsNullOrEmpty(parameter.Attribute.Parent))
                    {
                        height += VerticalSpacing;
                    }
                }
            }

            okButton.bounds.Y = yPositionOnScreen + height - 4;
        }

        public bool CanApplyChanges()
        {
            return _nameTextBox.Text.Trim().Length > 0 && _taskFactory?.IsReady() == true;
        }

        private void ApplyChanges()
        {
            string name = _nameTextBox.Text.Trim();
            string season = seasonsDropDown.Options[seasonsDropDown.SelectedOption];
            ITask task = _taskFactory?.Create(name) ?? new BasicTask(name);
            TasksPage? tasksPage = null;

            task.RenewPeriod = (Period)renewPeriodDropDown.SelectedOption;
            task.RenewCustomInterval = task.RenewPeriod == Period.Custom && int.TryParse(_customRenewTextBox.Text, out int days) ? days : 1;
            task.RenewDate = new WorldDate(1, season, task.RenewPeriod switch
            {
                Period.Custom => 1,
                Period.Weekly => weekdaysDropDown.SelectedOption + 1,
                _ => daysDropDown.SelectedOption + 1
            });

            for (IClickableMenu parent = GetParentMenu(); parent != null; parent = parent.GetParentMenu())
            {
                if (parent is TasksPage)
                {
                    tasksPage = parent as TasksPage;
                    break;
                }
            }

            if (_task != null)
            {
                if (DeluxeJournalMod.Instance?.TaskManager is TaskManager taskManager)
                {
                    IList<ITask> tasks = taskManager.Tasks;
                    int index = tasks.IndexOf(_task);

                    task.Active = _task.Active;
                    task.Complete = _task.Complete;
                    task.Count = _task.Count;
                    task.MarkAsViewed();
                    task.SetSortingIndex(_task.GetSortingIndex());

                    if (task.Active && task.Count >= task.MaxCount)
                    {
                        task.Complete = true;
                    }

                    task.Validate();
                    tasks.RemoveAt(index);
                    tasks.Insert(index, task);
                    tasksPage?.OnTasksUpdated();
                }
            }
            else
            {
                tasksPage?.AddTask(task);
            }
        }

        public void SelectTask(string id)
        {
            _selectedTaskID = id;
            _taskFactory = TaskRegistry.CreateFactoryInstance(id);

            SetupParameters();
        }

        private void SetupParameters()
        {
            _parameterIcons.Clear();
            _parameterTextBoxes.Clear();
            _parameterDropDowns.Clear();
            parameterTextBoxCCs.Clear();

            if (_taskFactory != null)
            {
                IReadOnlyList<TaskParameter> parameters = _taskFactory.GetParameters();

                for (int i = 0, j = 0; i < parameters.Count; i++)
                {
                    TaskParameter parameter = parameters[i];

                    if (parameter.Attribute.Hidden)
                    {
                        continue;
                    }
                    else if (parameter.Attribute.Parent is string parentName)
                    {
                        for (int k = 0; k < i; k++)
                        {
                            TaskParameter parentParameter = parameters[k];

                            if (parentParameter.Attribute.Name == parentName)
                            {
                                TaskParameterTextBox parentTextBox = _parameterTextBoxes[k];
                                ClickableComponent parentTextBoxCC = parameterTextBoxCCs[k];
                                Rectangle dropDownBounds = new Rectangle(parentTextBox.X + parentTextBox.Width - 120, parentTextBox.Y, 120, 44);

                                parentTextBox.Width = _fixedContentBounds.Width - LabelWidth - dropDownBounds.Width + 4;
                                parentTextBoxCC.bounds.Width = parentTextBox.Width;

                                if (parameter.Attribute.Tag == TaskParameterTag.Quality && DeluxeJournalMod.UiTexture is Texture2D uiTexture)
                                {
                                    KeyValuePair<Texture2D, Rectangle>[] options = new KeyValuePair<Texture2D, Rectangle>[]
                                    {
                                        new(uiTexture, new Rectangle(96, 80, 8, 8)),
                                        new(Game1.mouseCursors, new Rectangle(338, 400, 8, 8)),
                                        new(Game1.mouseCursors, new Rectangle(346, 400, 8, 8)),
                                        new(Game1.mouseCursors, new Rectangle(346, 392, 8, 8))
                                    };

                                    _parameterDropDowns.Add(k, new TaskParameterDropDown(parameter, options, dropDownBounds)
                                    {
                                        myID = ParameterDropDownId + k,
                                        upNeighborID = parentTextBoxCC.upNeighborID,
                                        downNeighborID = parentTextBoxCC.downNeighborID,
                                        rightNeighborID = parentTextBoxCC.rightNeighborID,
                                        leftNeighborID = parentTextBoxCC.myID,
                                        fullyImmutable = true
                                    });

                                    parentTextBoxCC.rightNeighborID = ParameterDropDownId + k;
                                }
                                break;
                            }
                        }
                        continue;
                    }

                    Rectangle textBoxBounds = new Rectangle(
                        _fixedContentBounds.X + LabelWidth,
                        _fixedContentBounds.Bottom + VerticalSpacing * j,
                        _fixedContentBounds.Width - LabelWidth,
                        _textBoxTexture.Height);
                    Rectangle iconBounds = new Rectangle(textBoxBounds.X - 60, textBoxBounds.Y - 4, 56, 56);

                    TaskParameterTextBox textBox = new TaskParameterTextBox(parameter, _taskFactory, _textBoxTexture, null, Game1.smallFont, Game1.textColor, _translation)
                    {
                        X = textBoxBounds.X,
                        Y = textBoxBounds.Y,
                        Width = textBoxBounds.Width,
                        Label = _translation.Get("ui.tasks.parameter." + parameter.Attribute.Name).Default(parameter.Attribute.Name),
                        numbersOnly = parameter.Type == typeof(int)
                    };

                    _parameterTextBoxes.Add(textBox);
                    parameterTextBoxCCs.Add(new ClickableComponent(textBoxBounds, "")
                    {
                        myID = ParameterTextBoxId + j,
                        upNeighborID = j == 0 ? CUSTOM_SNAP_BEHAVIOR : ParameterTextBoxId + j - 1,
                        downNeighborID = ParameterTextBoxId + j + 1,
                        rightNeighborID = cancelButton.myID,
                        leftNeighborID = CUSTOM_SNAP_BEHAVIOR,
                        fullyImmutable = true
                    });

                    SmartIconFlags mask = _taskFactory.EnabledSmartIcons & parameter.Attribute.Tag switch
                    {
                        TaskParameterTag.ItemList => SmartIconFlags.Item,
                        TaskParameterTag.Building => SmartIconFlags.Building,
                        TaskParameterTag.FarmAnimalList => SmartIconFlags.Animal,
                        TaskParameterTag.NpcName => SmartIconFlags.Npc,
                        _ => SmartIconFlags.None
                    };

                    if (mask != SmartIconFlags.None)
                    {
                        _parameterIcons.Add(j, new SmartIconComponent(iconBounds, textBox.TaskParser, ParameterIconId + j, mask, 1, false));
                    }

                    j++;
                }

                if (parameterTextBoxCCs.LastOrDefault() is ClickableComponent parameterTextBoxCC)
                {
                    parameterTextBoxCC.downNeighborID = okButton.myID;
                }
            }

            RecalculateBounds();
            populateClickableComponentList();
        }

        private void OnExit()
        {
            Game1.keyboardDispatcher.Subscriber = null;

            if (Game1.options.SnappyMenus)
            {
                Game1.activeClickableMenu?.snapToDefaultClickableComponent();
            }
        }

        protected override void customSnapBehavior(int direction, int oldRegion, int oldID)
        {
            switch (direction)
            {
                case Game1.up:
                    if (oldID == okButton.myID && parameterTextBoxCCs.Count > 0)
                    {
                        currentlySnappedComponent = parameterTextBoxCCs.Last();
                    }
                    else
                    {
                        currentlySnappedComponent = GetSelectedTypeIcon(_selectedTaskID);
                    }
                    break;
                case Game1.down:
                    if (oldID >= 0 && oldID < typeIcons.Count)
                    {
                        currentlySnappedComponent = parameterTextBoxCCs.Count > 0 ? parameterTextBoxCCs.First() : okButton;
                    }
                    else
                    {
                        currentlySnappedComponent = GetSelectedTypeIcon(_selectedTaskID);
                    }
                    break;
                case Game1.left:
                    if (oldID >= ParameterTextBoxId && oldID < ParameterTextBoxId + _parameterTextBoxes.Count)
                    {
                        if (getComponentWithID(oldID - (ParameterTextBoxId - ParameterIconId)) is ClickableComponent icon)
                        {
                            currentlySnappedComponent = icon;
                        }
                        else
                        {
                            currentlySnappedComponent = backButton;
                        }
                    }
                    break;
            }

            snapCursorToCurrentSnappedComponent();
        }

        public ClickableComponent? GetSelectedTypeIcon(string name)
        {
            foreach (ClickableComponent component in typeIcons)
            {
                if (component.name == name)
                {
                    return component;
                }
            }

            return null;
        }

        public override void populateClickableComponentList()
        {
            base.populateClickableComponentList();

            foreach (var icon in _parameterIcons.Values)
            {
                allClickableComponents.AddRange(icon.GetClickableComponents());
            }

            foreach (var dropDown in _parameterDropDowns.Values)
            {
                allClickableComponents.Add(dropDown);
            }
        }

        public override void snapCursorToCurrentSnappedComponent()
        {
            base.snapCursorToCurrentSnappedComponent();

            if (currentlySnappedComponent is ClickableComponent cc
                && cc.myID >= ParameterTextBoxId
                && cc.myID < ParameterTextBoxId + _parameterTextBoxes.Count)
            {
                _parameterTextBoxes[cc.myID - ParameterTextBoxId].FillWithParsedText();
            }
        }

        public override void snapToDefaultClickableComponent()
        {
            currentlySnappedComponent = backButton;
            snapCursorToCurrentSnappedComponent();
        }

        public override void receiveGamePadButton(Buttons b)
        {
            switch (b)
            {
                case Buttons.B:
                    currentlySnappedComponent = cancelButton;
                    snapCursorToCurrentSnappedComponent();
                    break;
            }
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            if (Game1.keyboardDispatcher.Subscriber is IKeyboardSubscriber keyboard)
            {
                if (keyboard is TaskParameterTextBox activeTextBox)
                {
                    activeTextBox.FillWithParsedText();
                }

                keyboard.Selected = false;
            }

            if (backButton.containsPoint(x, y))
            {
                exitThisMenu(playSound);
            }
            else if (cancelButton.containsPoint(x, y))
            {
                ExitAllChildMenus(playSound);
            }
            else if (okButton.containsPoint(x, y) && CanApplyChanges())
            {
                ApplyChangesAndExit(playSound);
            }
            else if (nameTextBoxCC.containsPoint(x, y))
            {
                _nameTextBox.SelectMe();
                _nameTextBox.ForceUpdate();
            }
            else if (renewPeriodDropDown.containsPoint(x, y))
            {
                renewPeriodDropDown.ReceiveLeftClick(x, y);
                _activeDropDown = renewPeriodDropDown;
            }
            else if (weekdaysDropDown.containsPoint(x, y))
            {
                weekdaysDropDown.ReceiveLeftClick(x, y);
                _activeDropDown = weekdaysDropDown;
            }
            else if (seasonsDropDown.containsPoint(x, y))
            {
                seasonsDropDown.ReceiveLeftClick(x, y);
                _activeDropDown = seasonsDropDown;
            }
            else if (daysDropDown.containsPoint(x, y))
            {
                daysDropDown.ReceiveLeftClick(x, y);
                _activeDropDown = daysDropDown;
            }
            else if (customRenewTextBoxCC.containsPoint(x, y))
            {
                _customRenewTextBox.SelectMe();
                _customRenewTextBox.ForceUpdate();
            }
            else
            {
                for (int i = 0; i < typeIcons.Count; i++)
                {
                    if (typeIcons[i].containsPoint(x, y))
                    {
                        SelectTask(typeIcons[i].name);
                        return;
                    }
                }

                for (int i = 0; i < _parameterTextBoxes.Count; i++)
                {
                    TaskParameterTextBox textBox = _parameterTextBoxes[i];

                    if (textBox.ContainsPoint(x, y))
                    {
                        textBox.SelectMe();
                        textBox.ForceUpdate();
                        return;
                    }
                    else if (_parameterDropDowns.TryGetValue(i, out var dropDown) && dropDown.containsPoint(x, y))
                    {
                        dropDown.ReceiveLeftClick(x, y);
                        _activeDropDown = dropDown;
                        return;
                    }
                }
            }
        }

        public override void leftClickHeld(int x, int y)
        {
            if (_activeDropDown != null)
            {
                _activeDropDown.LeftClickHeld(x, y);
            }
        }

        public override void releaseLeftClick(int x, int y)
        {
            if (_activeDropDown != null)
            {
                _activeDropDown.LeftClickReleased(x, y);
                _activeDropDown = null;
            }
        }

        public override void receiveKeyPress(Keys key)
        {
            if (Game1.options.SnappyMenus)
            {
                applyMovementKey(key);
            }
            else if (Game1.keyboardDispatcher.Subscriber == null)
            {
                base.receiveKeyPress(key);
            }

            if (_activeDropDown != null)
            {
                _activeDropDown.ReceiveKeyPress(key);
            }

            switch (key)
            {
                case Keys.Escape:
                    exitThisMenu();
                    break;
                case Keys.Tab:
                    CycleTextBoxes();
                    break;
                case Keys.Enter:
                    if (CycleTextBoxes() && CanApplyChanges())
                    {
                        ApplyChangesAndExit();
                    }
                    break;
            }
        }

        /// <summary>Cycle through text boxes.</summary>
        /// <param name="completeParameterText">Complete the parameter text before attempting to cycle.</param>
        /// <returns><c>true</c> if the next text box was selected and <c>false</c> otherwise.</returns>
        private bool CycleTextBoxes(bool completeParameterText = true)
        {
            if (_activeDropDown != null || Game1.options.SnappyMenus)
            {
                return false;
            }

            TextBox selectTextBox = _nameTextBox;

            if (_nameTextBox.Selected)
            {
                if (customRenewTextBoxCC.visible)
                {
                    selectTextBox = _customRenewTextBox;
                }
                else if (_parameterTextBoxes.Count > 0)
                {
                    selectTextBox = _parameterTextBoxes.First();
                }
            }
            else if (_customRenewTextBox.Selected)
            {
                selectTextBox = _parameterTextBoxes.Count > 0 ? _parameterTextBoxes.First() : _nameTextBox;
            }
            else if (_parameterTextBoxes.Count > 0)
            {
                for (int i = 0; i < _parameterTextBoxes.Count; i++)
                {
                    if (_parameterTextBoxes[i].Selected)
                    {
                        if (completeParameterText && _parameterTextBoxes[i].FillWithParsedText())
                        {
                            return false;
                        }
                        else if (i + 1 < _parameterTextBoxes.Count)
                        {
                            selectTextBox = _parameterTextBoxes[i + 1];
                        }

                        break;
                    }
                }
            }

            if (!selectTextBox.Selected)
            {
                selectTextBox.SelectMe();
            }

            return true;
        }

        public override void applyMovementKey(int direction)
        {
            if (_activeDropDown == null)
            {
                base.applyMovementKey(direction);
            }
        }

        public override void performHoverAction(int x, int y)
        {
            _hoverText = string.Empty;

            if (cancelButton.containsPoint(x, y))
            {
                _hoverText = _translation.Get("ui.tasks.new.cancelbutton.hover");
            }
            else
            {
                for (int i = 0; i < typeIcons.Count; i++)
                {
                    if (typeIcons[i].containsPoint(x, y))
                    {
                        _hoverText = _translation.Get("task." + typeIcons[i].name);
                        break;
                    }
                }

                if (_taskFactory != null && string.IsNullOrEmpty(_hoverText))
                {
                    for (int i = 0; i < _parameterTextBoxes.Count; i++)
                    {
                        if (_parameterIcons.TryGetValue(i, out var icon) && icon.TryGetHoverText(x, y, _translation, out string hoverText))
                        {
                            _hoverText = hoverText;
                            break;
                        }
                        else if (_parameterDropDowns.TryGetValue(i, out var dropDown) && dropDown.containsPoint(x, y))
                        {
                            _hoverText = _translation.Get("ui.tasks.parameter." + dropDown.TaskParameter.Attribute.Name);
                            break;
                        }
                    }
                }
            }

            backButton.tryHover(x, y, 0.2f);
            cancelButton.tryHover(x, y, 0.2f);
            okButton.tryHover(x, y);
        }

        public override void draw(SpriteBatch b)
        {
            b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.75f);
            SpriteText.drawStringWithScrollCenteredAt(b, _translation.Get("ui.tasks.options"), xPositionOnScreen + width / 2, yPositionOnScreen + 16);

            Game1.drawDialogueBox(xPositionOnScreen, yPositionOnScreen, width, height, false, true);
            drawHorizontalPartition(b, yPositionOnScreen + 212);

            DrawLabel(b, _translation.Get("ui.tasks.options.type"), _fixedContentBounds.Y + (VerticalSpacing * 2) + 24, Game1.textColor);

            for (int i = 0; i < typeIcons.Count; i++)
            {
                TaskRegistry.GetTaskIcon(typeIcons[i].name).DrawIcon(b,
                    typeIcons[i].bounds,
                    typeIcons[i].name == _selectedTaskID ? Color.White : Color.DimGray,
                    drawShadow: true);
            }

            backButton.draw(b);
            cancelButton.draw(b);
            okButton.draw(b, CanApplyChanges() ? Color.White : Color.Gray * 0.8f, 0.88f);

            if (_taskFactory != null)
            {
                for (int i = _parameterTextBoxes.Count - 1; i >= 0; i--)
                {
                    TaskParameterTextBox parameterTextBox = _parameterTextBoxes[i];
                    TaskParameter parameter = parameterTextBox.TaskParameter;
                    int quality = 0;

                    DrawLabel(b, parameterTextBox.Label, parameterTextBox.Y, parameter.IsValid() ? Game1.textColor : Color.DarkRed);

                    if (_parameterDropDowns.TryGetValue(i, out var dropDown))
                    {
                        dropDown.Draw(b);

                        if (dropDown.TaskParameter.Value is int value)
                        {
                            quality = value;
                        }
                    }

                    if (_parameterIcons.TryGetValue(i, out var icon))
                    {
                        icon.Draw(b, parameter.IsValid() ? Color.White : Color.Gray * 0.8f, quality, false, true);
                    }

                    parameterTextBox.Draw(b);
                }
            }

            DrawLabel(b, _translation.Get("ui.tasks.options.name"), _nameTextBox.Y, _nameTextBox.Text.Trim().Length > 0 ? Game1.textColor : Color.DarkRed);
            _nameTextBox.Draw(b);

            Period renewPeriod = (Period)renewPeriodDropDown.SelectedOption;
            weekdaysDropDown.visible = renewPeriod == Period.Weekly;
            seasonsDropDown.visible = renewPeriod == Period.Annually;
            daysDropDown.visible = renewPeriod == Period.Monthly || renewPeriod == Period.Annually;
            customRenewTextBoxCC.visible = renewPeriod == Period.Custom;

            if (daysDropDown.visible)
            {
                daysDropDown.bounds.X = (renewPeriod == Period.Annually ? seasonsDropDown.bounds.Right : renewPeriodDropDown.bounds.Right) + 8;
                daysDropDown.RecalculateBounds();
            }

            DrawLabel(b, _translation.Get("ui.tasks.options.renew"), renewPeriodDropDown.bounds.Y, Game1.textColor);
            renewPeriodDropDown.Draw(b);
            weekdaysDropDown.Draw(b);
            seasonsDropDown.Draw(b);
            daysDropDown.Draw(b);

            if (customRenewTextBoxCC.visible)
            {
                _customRenewTextBox.Draw(b);

                if (string.IsNullOrEmpty(_customRenewTextBox.Text))
                {
                    Utility.drawTextWithShadow(b,
                        _translation.Get("ui.tasks.renew.days", new { count = "#" }),
                        Game1.smallFont,
                        new(_customRenewTextBox.X + 28, _customRenewTextBox.Y + 12),
                        Game1.unselectedOptionColor);
                }
            }

            if (_hoverText.Length > 0)
            {
                drawHoverText(b, _hoverText, Game1.dialogueFont);
            }
        }

        private void DrawLabel(SpriteBatch b, string name, int yPos, Color color)
        {
            Utility.drawTextWithShadow(b, name, Game1.dialogueFont, new Vector2(_fixedContentBounds.X, yPos), color);
        }

        private void ApplyChangesAndExit(bool playSound = true)
        {
            ApplyChanges();

            if (playSound)
            {
                Game1.playSound("bigSelect");
            }

            ExitAllChildMenus(false);
        }

        private void ExitAllChildMenus(bool playSound = true)
        {
            if (playSound)
            {
                Game1.playSound("bigDeSelect");
            }

            for (IClickableMenu parent = GetParentMenu(); parent != null; parent = parent.GetParentMenu())
            {
                parent.GetChildMenu().exitThisMenuNoSound();
            }
        }
    }
}
