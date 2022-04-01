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
using DeluxeJournal.Framework.Tasks;
using DeluxeJournal.Menus.Components;
using DeluxeJournal.Tasks;

using Period = DeluxeJournal.Tasks.ITask.Period;
using static StardewValley.Menus.ClickableComponent;

namespace DeluxeJournal.Menus
{
    /// <summary>TasksPage child menu for editing task options.</summary>
    public class TaskOptionsMenu : IClickableMenu
    {
        private const int LabelWidth = 256;
        private const int VerticalSpacing = 64;
        private const int BottomGap = 32;

        public readonly ClickableTextureComponent backButton;
        public readonly ClickableTextureComponent cancelButton;
        public readonly ClickableTextureComponent okButton;

        public readonly ClickableComponent nameTextBoxCC;
        public readonly ClickableComponent renewPeriodDropDownCC;
        public readonly ClickableComponent weekdaysDropDownCC;
        public readonly ClickableComponent daysDropDownCC;
        public readonly ClickableComponent seasonsDropDownCC;
        public readonly List<ClickableComponent> parameterTextBoxCCs;
        public readonly List<ClickableComponent> typeIcons;

        private readonly SideScrollingTextBox _nameTextBox;
        private readonly OptionsDropDown _renewPeriodDropDown;
        private readonly OptionsDropDown _weekdaysDropDown;
        private readonly OptionsDropDown _daysDropDown;
        private readonly OptionsDropDown _seasonsDropDown;
        private readonly IDictionary<int, ClickableTextureComponent> _parameterIcons;
        private readonly IDictionary<int, TaskParameterTextBox> _parameterTextBoxes;

        private readonly ITranslationHelper _translation;
        private readonly Texture2D _textBoxTexture;
        private readonly Rectangle _fixedContentBounds;
        private readonly ITask? _task;
        private Tasks.TaskFactory? _taskFactory;
        private OptionsElement? _optionHeld;
        private string _selectedTaskID;
        private bool _restoreChildMenu;
        private string _hoverText;

        public TaskOptionsMenu(ITask task, ITranslationHelper translation) : this(translation)
        {
            _task = task;
            _selectedTaskID = task.ID;
            _taskFactory = TaskRegistry.CreateFactoryInstance(task.ID);
            _taskFactory.Initialize(task, translation);

            _nameTextBox.Text = task.Name;
            _renewPeriodDropDown.selectedOption = (int)_task.RenewPeriod;
            
            if (_task.RenewPeriod == Period.Weekly)
            {
                _weekdaysDropDown.selectedOption = (_task.RenewDate.DayOfMonth - 1) % 7;
            }
            else if (_task.RenewPeriod != Period.Never)
            {
                _daysDropDown.selectedOption = _task.RenewDate.DayOfMonth;
                _seasonsDropDown.selectedOption = _task.RenewDate.SeasonIndex;
            }

            SetupParameters();
        }

        public TaskOptionsMenu(string taskName, TaskParser taskParser, ITranslationHelper translation) :
            this(translation)
        {
            _selectedTaskID = taskParser.ID;
            _taskFactory = taskParser.GenerateFactory();
            _nameTextBox.Text = taskName;

            SetupParameters();
        }

        public TaskOptionsMenu(ITranslationHelper translation) : base(0, 0, 864, 576)
        {
            xPositionOnScreen = (Game1.uiViewport.Width / 2) - (width / 2);
            yPositionOnScreen = (Game1.uiViewport.Height / 2) - (height / 2);

            _translation = translation;
            _textBoxTexture = Game1.content.Load<Texture2D>("LooseSprites\\textBox");
            _optionHeld = null;
            _taskFactory = null;
            _task = null;
            _selectedTaskID = TaskTypes.Basic;
            _restoreChildMenu = false;
            _hoverText = "";

            _fixedContentBounds = default;
            _fixedContentBounds.X = xPositionOnScreen + spaceToClearSideBorder + 28;
            _fixedContentBounds.Y = yPositionOnScreen + spaceToClearTopBorder + 16;
            _fixedContentBounds.Width = width - (_fixedContentBounds.X - xPositionOnScreen) * 2;

            _parameterIcons = new Dictionary<int, ClickableTextureComponent>();
            _parameterTextBoxes = new Dictionary<int, TaskParameterTextBox>();
            parameterTextBoxCCs = new List<ClickableComponent>();
            typeIcons = new List<ClickableComponent>();

            _nameTextBox = new SideScrollingTextBox(_textBoxTexture, null, Game1.smallFont, Game1.textColor)
            {
                X = _fixedContentBounds.X + LabelWidth,
                Y = _fixedContentBounds.Y,
                Width = _fixedContentBounds.Width - LabelWidth
            };

            _renewPeriodDropDown = CreateTranslatedDropDown("ui.tasks.options.renew", 0, VerticalSpacing, true);
            _weekdaysDropDown = CreateTranslatedDropDown("weekdays", _renewPeriodDropDown.bounds.Width + 8, VerticalSpacing, false);
            _seasonsDropDown = CreateTranslatedDropDown("seasons", _renewPeriodDropDown.bounds.Width + 8, VerticalSpacing, false);
            _daysDropDown = new OptionsNumericDropDown(string.Empty, 1, 28, OptionsNumericDropDown.WrapStyle.Horizontal, 7, _renewPeriodDropDown.bounds.Right + 8, _renewPeriodDropDown.bounds.Y);

            nameTextBoxCC = new ClickableComponent(new Rectangle(_nameTextBox.X, _nameTextBox.Y, _nameTextBox.Width, _nameTextBox.Height), "")
            {
                myID = 100,
                downNeighborID = 101,
                rightNeighborID = SNAP_AUTOMATIC,
                leftNeighborID = SNAP_AUTOMATIC
            };
            
            renewPeriodDropDownCC = new ClickableComponent(_renewPeriodDropDown.bounds, "")
            {
                myID = 101,
                upNeighborID = SNAP_AUTOMATIC,
                downNeighborID = CUSTOM_SNAP_BEHAVIOR,
                rightNeighborID = SNAP_AUTOMATIC,
                leftNeighborID = SNAP_AUTOMATIC
            };
            
            weekdaysDropDownCC = new ClickableComponent(_weekdaysDropDown.bounds, "")
            {
                myID = 102,
                upNeighborID = SNAP_AUTOMATIC,
                downNeighborID = CUSTOM_SNAP_BEHAVIOR,
                rightNeighborID = SNAP_AUTOMATIC,
                leftNeighborID = SNAP_AUTOMATIC
            };
            
            daysDropDownCC = new ClickableComponent(_daysDropDown.bounds, "")
            {
                myID = 103,
                upNeighborID = SNAP_AUTOMATIC,
                downNeighborID = CUSTOM_SNAP_BEHAVIOR,
                rightNeighborID = SNAP_AUTOMATIC,
                leftNeighborID = SNAP_AUTOMATIC
            };
            
            seasonsDropDownCC = new ClickableComponent(_seasonsDropDown.bounds, "")
            {
                myID = 104,
                upNeighborID = SNAP_AUTOMATIC,
                downNeighborID = CUSTOM_SNAP_BEHAVIOR,
                rightNeighborID = SNAP_AUTOMATIC,
                leftNeighborID = SNAP_AUTOMATIC
            };

            Rectangle iconBounds = new Rectangle(0, 0, 56, 56);
            int offset = 0;
            int i = 0;

            foreach (string id in TaskRegistry.Keys)
            {
                iconBounds.X = _fixedContentBounds.X + LabelWidth + 12 + (offset % 512);
                iconBounds.Y = _fixedContentBounds.Y + 20 + VerticalSpacing * (2 + (offset / 512));

                typeIcons.Add(new ClickableComponent(iconBounds, id)
                {
                    myID = i,
                    upNeighborID = SNAP_AUTOMATIC,
                    downNeighborID = SNAP_AUTOMATIC,
                    rightNeighborID = SNAP_AUTOMATIC,
                    leftNeighborID = SNAP_AUTOMATIC
                });

                offset += 64;
                i++;
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
                rightNeighborID = SNAP_AUTOMATIC
            };

            cancelButton = new ClickableTextureComponent(
                new Rectangle(xPositionOnScreen + width - 12, _fixedContentBounds.Y, 64, 64),
                DeluxeJournalMod.UiTexture,
                new Rectangle(0, 80, 16, 16),
                4f)
            {
                myID = 106,
                leftNeighborID = SNAP_AUTOMATIC
            };

            okButton = new ClickableTextureComponent(
                new Rectangle(xPositionOnScreen + width - 100, yPositionOnScreen + height - 4, 64, 64),
                DeluxeJournalMod.UiTexture,
                new Rectangle(32, 80, 16, 16),
                4f)
            {
                myID = 107,
                upNeighborID = CUSTOM_SNAP_BEHAVIOR,
                rightNeighborID = SNAP_AUTOMATIC
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
                    if (!parameter.Attribute.Hidden)
                    {
                        height += VerticalSpacing;
                    }
                }
            }

            okButton.bounds.Y = yPositionOnScreen + height - 4;
        }

        public bool CanApplyChanges()
        {
            return _nameTextBox.Text.Trim().Length > 0 && (_taskFactory == null || _taskFactory.IsReady());
        }

        private void ApplyChanges()
        {
            string name = _nameTextBox.Text.Trim();
            string season = _seasonsDropDown.dropDownOptions[_seasonsDropDown.selectedOption];
            ITask task = _taskFactory?.Create(name) ?? new BasicTask(name);

            task.RenewPeriod = (Period)_renewPeriodDropDown.selectedOption;
            task.RenewDate = new WorldDate(1, season, ((task.RenewPeriod == Period.Weekly) ? _weekdaysDropDown.selectedOption : _daysDropDown.selectedOption) + 1);

            if (_task != null)
            {
                if (DeluxeJournalMod.Instance?.TaskManager is TaskManager taskManager)
                {
                    task.Active = _task.Active;
                    task.Complete = _task.Complete;
                    task.Count = _task.Count;
                    task.MarkAsViewed();
                    task.SetSortingIndex(_task.GetSortingIndex());

                    if (task.Active && task.Count >= task.MaxCount)
                    {
                        task.Complete = true;
                    }

                    taskManager.ReplaceTask(_task, task);
                }
            }
            else if (GetParentMenu() is TasksPage tasksPage)
            {
                tasksPage.AddTask(task);
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
            parameterTextBoxCCs.Clear();

            if (_taskFactory != null)
            {
                IReadOnlyList<TaskParameter> parameters = _taskFactory.GetParameters();
                TaskParameterTextBox textBox;
                TaskParameter parameter;
                Rectangle iconBounds;
                object? value;
                string name;

                for (int i = 0; i < parameters.Count; i++)
                {
                    parameter = parameters[i];
                    value = parameter.Value;
                    iconBounds = new Rectangle(_fixedContentBounds.X + LabelWidth - 60, _fixedContentBounds.Bottom - 4 + VerticalSpacing * i, 56, 56);

                    if (parameter.Attribute.Hidden)
                    {
                        continue;
                    }

                    if (parameter.Type == typeof(Item))
                    {
                        name = value != null ? ((Item)value).DisplayName : string.Empty;

                        _parameterIcons.Add(_parameterTextBoxes.Count, new ClickableTextureComponent(
                            iconBounds,
                            DeluxeJournalMod.UiTexture,
                            new Rectangle(14, 110, 14, 14),
                            4f,
                            drawShadow: true));
                    }
                    else if (parameter.Type == typeof(NPC))
                    {
                        name = value != null ? ((NPC)value).displayName : string.Empty;

                        _parameterIcons.Add(_parameterTextBoxes.Count, new ClickableTextureComponent(
                            iconBounds,
                            DeluxeJournalMod.UiTexture,
                            new Rectangle(0, 110, 14, 14),
                            4f,
                            drawShadow: true));
                    }
                    else
                    {
                        name = value?.ToString() ?? string.Empty;

                        if (parameter.Attribute.Tag == "building")
                        {
                            name = Game1.content.Load<Dictionary<string, string?>>("Data\\Blueprints").GetValueOrDefault(name, null)?.Split('/')[8] ?? string.Empty;
                        }
                        else if (parameter.Attribute.Tag == "monster")
                        {
                            name = Game1.content.Load<Dictionary<string, string?>>("Data\\Monsters").GetValueOrDefault(name, null)?.Split('/')[14] ?? string.Empty;
                        }
                    }

                    textBox = new TaskParameterTextBox(parameter, _textBoxTexture, null, Game1.smallFont, Game1.textColor, _translation)
                    {
                        X = _fixedContentBounds.X + LabelWidth,
                        Y = _fixedContentBounds.Bottom + VerticalSpacing * i,
                        Width = _fixedContentBounds.Width - LabelWidth,
                        Label = _translation.Get("ui.tasks.parameter." + parameter.Attribute.Name).Default(parameter.Attribute.Name),
                        TextWithParse = name,
                        numbersOnly = parameter.Type == typeof(int)
                    };

                    _parameterTextBoxes.Add(i, textBox);
                    parameterTextBoxCCs.Add(new ClickableComponent(new Rectangle(textBox.X, textBox.Y, textBox.Width, textBox.Height), "")
                    {
                        myID = 1000 + i,
                        upNeighborID = (i == 0) ? CUSTOM_SNAP_BEHAVIOR : 999 + i,
                        downNeighborID = SNAP_AUTOMATIC,
                        rightNeighborID = SNAP_AUTOMATIC,
                        leftNeighborID = SNAP_AUTOMATIC
                    });
                }
            }

            RecalculateBounds();
            populateClickableComponentList();
        }

        private OptionsDropDown CreateTranslatedDropDown(string prefix, int xOffset, int yOffset, bool setLabel)
        {
            string label = setLabel ? _translation.Get(prefix) : string.Empty;
            int prefixLength = prefix.Length + 1;
            OptionsDropDown dropDown = new OptionsDropDown(label, 0, _fixedContentBounds.X + LabelWidth + xOffset, _fixedContentBounds.Y + yOffset);
            
            foreach (Translation translation in _translation.GetTranslations())
            {
                if (translation.Key.StartsWith(prefix + '.'))
                {
                    dropDown.dropDownOptions.Add(translation.Key[prefixLength..]);
                    dropDown.dropDownDisplayOptions.Add(translation);
                }
            }

            dropDown.bounds.Width = 0;
            dropDown.RecalculateBounds();
            dropDown.labelOffset.X = -(dropDown.bounds.Width + LabelWidth + 8);

            return dropDown;
        }

        private void OnExit()
        {
            Game1.keyboardDispatcher.Subscriber = null;

            if (Game1.options.SnappyMenus)
            {
                Game1.activeClickableMenu?.snapToDefaultClickableComponent();
            }
        }

        protected override void cleanupBeforeExit()
        {
            if (_restoreChildMenu)
            {
                _parentMenu?.SetChildMenu(_childMenu);
                _parentMenu = null;
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
                    currentlySnappedComponent = GetSelectedTypeIcon(_selectedTaskID);
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
            Game1.keyboardDispatcher.Subscriber = null;

            if (backButton.containsPoint(x, y))
            {
                _restoreChildMenu = true;
                exitThisMenu(playSound);
            }
            else if (cancelButton.containsPoint(x, y))
            {
                exitThisMenu(playSound);
            }
            else if (okButton.containsPoint(x, y) && CanApplyChanges())
            {
                ApplyChanges();

                Game1.playSound("bigSelect");
                exitThisMenuNoSound();
            }
            else if (nameTextBoxCC.containsPoint(x, y))
            {
                _nameTextBox.SelectMe();
                _nameTextBox.Update();
            }
            else if (renewPeriodDropDownCC.containsPoint(x, y))
            {
                _renewPeriodDropDown.receiveLeftClick(x, y);
                _optionHeld = _renewPeriodDropDown;
            }
            else if (weekdaysDropDownCC.containsPoint(x, y))
            {
                _weekdaysDropDown.receiveLeftClick(x, y);
                _optionHeld = _weekdaysDropDown;
            }
            else if (seasonsDropDownCC.containsPoint(x, y))
            {
                _seasonsDropDown.receiveLeftClick(x, y);
                _optionHeld = _seasonsDropDown;
            }
            else if (daysDropDownCC.containsPoint(x, y))
            {
                _daysDropDown.receiveLeftClick(x, y);
                _optionHeld = _daysDropDown;
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

                foreach (TaskParameterTextBox textBox in _parameterTextBoxes.Values)
                {
                    if (textBox.ContainsPoint(x, y))
                    {
                        textBox.SelectMe();
                        textBox.Update();
                        return;
                    }
                }
            }
        }

        public override void leftClickHeld(int x, int y)
        {
            if (_optionHeld != null)
            {
                _optionHeld.leftClickHeld(x, y);
            }
        }

        public override void releaseLeftClick(int x, int y)
        {
            if (_optionHeld != null)
            {
                _optionHeld.leftClickReleased(x, y);
            }

            _optionHeld = null;
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

            if (_optionHeld != null)
            {
                _optionHeld.receiveKeyPress(key);
            }

            switch (key)
            {
                case Keys.Escape:
                    exitThisMenu();
                    break;
            }
        }

        public override void applyMovementKey(int direction)
        {
            if (_optionHeld == null)
            {
                base.applyMovementKey(direction);
            }
        }

        public override void performHoverAction(int x, int y)
        {
            _hoverText = "";

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
            }

            backButton.tryHover(x, y, 0.2f);
            cancelButton.tryHover(x, y, 0.2f);
            okButton.tryHover(x, y);
        }

        public override void draw(SpriteBatch b)
        {
            string title = _translation.Get("ui.tasks.options");
            Period renewPeriod = (Period)_renewPeriodDropDown.selectedOption;
            int typeSectionY = _fixedContentBounds.Y + (VerticalSpacing * 2) + 24;

            b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.75f);
            SpriteText.drawStringWithScrollCenteredAt(b, title, xPositionOnScreen + width / 2, yPositionOnScreen + 16);

            Game1.drawDialogueBox(xPositionOnScreen, yPositionOnScreen, width, height, false, true);
            drawHorizontalPartition(b, yPositionOnScreen + 212);

            DrawLabel(b, _translation.Get("ui.tasks.options.type"), typeSectionY, Game1.textColor);

            for (int i = 0; i < typeIcons.Count; i++)
            {
                TaskRegistry.GetTaskIcon(typeIcons[i].name).DrawIcon(b,
                    typeIcons[i].bounds,
                    typeIcons[i].name == _selectedTaskID ? Color.White : Color.DimGray,
                    drawShadow: true);
            }

            if (_taskFactory != null)
            {
                IReadOnlyList<TaskParameter> parameters = _taskFactory.GetParameters();
                TaskParameter parameter;
                TaskParameterTextBox parameterTextBox;

                for (int i = 0; i < parameters.Count; i++)
                {
                    if (!_parameterTextBoxes.ContainsKey(i))
                    {
                        continue;
                    }

                    parameter = parameters[i];
                    parameterTextBox = _parameterTextBoxes[i];

                    DrawLabel(b, parameterTextBox.Label, parameterTextBox.Y, parameter.IsValid() ? Game1.textColor : Color.DarkRed);
                    parameterTextBox.Draw(b);

                    if (_parameterIcons.ContainsKey(i))
                    {
                        ClickableTextureComponent icon = _parameterIcons[i];

                        if (parameter.Type == typeof(Item) && parameterTextBox.TaskParser.Item != null)
                        {
                            icon.draw(b);
                            parameterTextBox.TaskParser.Item?.drawInMenu(b,
                                new Vector2(icon.bounds.X - (parameterTextBox.TaskParser.Item.ParentSheetIndex == SObject.wood ? 6 : 2), icon.bounds.Y - 2),
                                0.75f, 1.0f, 0.9f,
                                StackDrawType.Draw,
                                Color.White,
                                false);
                        }
                        else if (parameter.Type == typeof(NPC) && parameterTextBox.TaskParser.NPC != null)
                        {
                            icon.draw(b);
                            CharacterIcon.DrawIcon(b,
                                parameterTextBox.TaskParser.NPC?.Name ?? "?",
                                new Rectangle(icon.bounds.X + 8, icon.bounds.Y + 8, 40, 40));
                        }
                    }
                }
            }

            DrawLabel(b, _translation.Get("ui.tasks.options.name"), _nameTextBox.Y, _nameTextBox.Text.Trim().Length > 0 ? Game1.textColor : Color.DarkRed);
            _nameTextBox.Draw(b);

            backButton.draw(b);
            cancelButton.draw(b);
            okButton.draw(b, CanApplyChanges() ? Color.White : Color.Gray * 0.8f, 0.88f);

            weekdaysDropDownCC.visible = renewPeriod == Period.Weekly;
            seasonsDropDownCC.visible = renewPeriod == Period.Annually;
            daysDropDownCC.visible = renewPeriod == Period.Monthly || renewPeriod == Period.Annually;
            daysDropDownCC.bounds.X = (renewPeriod == Period.Annually ? _seasonsDropDown.bounds.Right : _renewPeriodDropDown.bounds.Right) + 8;
            _daysDropDown.bounds.X = daysDropDownCC.bounds.X;
            _daysDropDown.dropDownBounds.X = daysDropDownCC.bounds.X;

            _renewPeriodDropDown.draw(b, 0, 0);

            if (weekdaysDropDownCC.visible)
            {
                _weekdaysDropDown.draw(b, 0, 0);
            }
            if (seasonsDropDownCC.visible)
            {
                _seasonsDropDown.draw(b, 0, 0);
            }
            if (daysDropDownCC.visible)
            {
                _daysDropDown.draw(b, 0, 0);
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
    }
}
