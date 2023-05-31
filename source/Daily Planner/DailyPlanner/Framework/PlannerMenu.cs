/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/BuildABuddha/StardewDailyPlanner
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using DailyPlanner.Framework.Constants;

namespace DailyPlanner.Framework
{
    class PlannerMenu : IClickableMenu
    {
        /*********
        ** Fields
        *********/
        /// <summary>The mod settings.</summary>
        private readonly ModConfig Config;

        /// <summary>The planner helper.</summary>
        private readonly Planner Planner;

        /// <summary>The checklist helper.</summary>
        private readonly CheckList CheckList;

        /// <summary>Provides translations for the mod.</summary>
        private readonly ITranslationHelper TranslationHelper;

        private readonly List<ClickableComponent> OptionSlots = new();
        private readonly List<OptionsElement> Options = new();
        private readonly ClickableTextureComponent UpArrow;
        private readonly ClickableTextureComponent DownArrow;
        private readonly ClickableTextureComponent Scrollbar;
        private readonly List<ClickableComponent> Tabs = new();
        private readonly ClickableComponent Title;
        private const int ItemsPerPage = 10;

        private string HoverText = "";
        private int OptionsSlotHeld = -1;
        private int CurrentItemIndex;
        private bool IsScrolling;
        private readonly Rectangle ScrollbarRunner; 
        private bool CanClose;
        public TextBoxComponent SelectedTextBox;

        public bool HasSelectedTextbox;
        public readonly MenuTab CurrentTab;

        public IMonitor Monitor;

        private readonly List<string> WeekdayList;
        private readonly List<string> SeasonList;
        private readonly List<string> SeasonListWithAllYear = new() { "All Year", "Spring", "Summer", "Fall", "Winter" };

        private readonly PlusMinusComponent OnDateSeasonPlusMinus;
        private readonly PlusMinusComponent OnDateDayPlusMinus;
        private readonly PlusMinusComponent DailySeasonPlusMinus;
        private readonly PlusMinusComponent WeeklySeasonPlusMinus;
        private readonly PlusMinusComponent WeeklyDayOfWeekPlusMinus;
        private readonly PlusMinusComponent RemoveTaskSeasonPlusMinus;
        private readonly PlusMinusComponent RemoveTaskDayPlusMinus;

        /*********
        ** Public methods
        *********/
        public PlannerMenu(MenuTab tabIndex, ModConfig config, Planner planner, CheckList checklist, ITranslationHelper i18n, IMonitor monitor)
          : base(Game1.viewport.Width / 2 - (600 + IClickableMenu.borderWidth * 2) / 2, Game1.viewport.Height / 2 - (600 + IClickableMenu.borderWidth * 2) / 2, 800 + IClickableMenu.borderWidth * 2, 600 + IClickableMenu.borderWidth * 2)
        {
            this.Config = config;
            this.Planner = planner;
            this.TranslationHelper = i18n;
            this.CheckList = checklist;
            this.CanClose = false;

            this.WeekdayList = new()
            {
                i18n.Get("week.monday"),
                i18n.Get("week.tuesday"),
                i18n.Get("week.wednesday"),
                i18n.Get("week.thursday"),
                i18n.Get("week.friday"),
                i18n.Get("week.saturday"),
                i18n.Get("week.sunday")
            };

            this.SeasonList = new()
            {
                i18n.Get("seasons.spring"),
                i18n.Get("seasons.summer"),
                i18n.Get("seasons.fall"),
                i18n.Get("seasons.winter"),
            };

            this.SeasonListWithAllYear = new()
            {
                i18n.Get("seasons.all_year"),
                i18n.Get("seasons.spring"),
                i18n.Get("seasons.summer"),
                i18n.Get("seasons.fall"),
                i18n.Get("seasons.winter"),
            };

            this.Title = new ClickableComponent(new Rectangle(this.xPositionOnScreen + this.width / 2, this.yPositionOnScreen, Game1.tileSize * 4, Game1.tileSize), i18n.Get("title"));
            this.CurrentTab = tabIndex;

            this.Monitor = monitor;

            this.HasSelectedTextbox = false;

            this.OnDateSeasonPlusMinus = new(SeasonList, i18n.Get("slider.season"), this);
            this.OnDateDayPlusMinus = new(1, 28, i18n.Get("slider.day"), this);
            this.DailySeasonPlusMinus = new(SeasonListWithAllYear, i18n.Get("slider.season"), this);
            this.WeeklySeasonPlusMinus = new(SeasonListWithAllYear, i18n.Get("slider.season"), this);
            this.WeeklyDayOfWeekPlusMinus = new(WeekdayList, i18n.Get("slider.week"), this);
            this.RemoveTaskSeasonPlusMinus = new(SeasonList, i18n.Get("slider.season"), this);
            this.RemoveTaskDayPlusMinus = new(1, 28, i18n.Get("slider.day"), this);

            {
                int i = 0;
                int labelX = (int)(this.xPositionOnScreen - Game1.tileSize * 4.8f);
                int labelY = (int)(this.yPositionOnScreen + Game1.tileSize * 1.5f);
                int labelHeight = (int)(Game1.tileSize * 0.9F);

                this.Tabs.Add(new ClickableComponent(
                    new Rectangle(labelX, labelY + labelHeight * i++, Game1.tileSize * 5, Game1.tileSize), MenuTab.Daily.ToString(), i18n.Get("tabs.daily")));
                this.Tabs.Add(new ClickableComponent(
                    new Rectangle(labelX, labelY + labelHeight * i++, Game1.tileSize * 5, Game1.tileSize), MenuTab.Checklist.ToString(), i18n.Get("tabs.checklist")));
                this.Tabs.Add(new ClickableComponent(
                    new Rectangle(labelX, labelY + labelHeight * i++, Game1.tileSize * 5, Game1.tileSize), MenuTab.Weekly.ToString(), i18n.Get("tabs.weekly")));
                this.Tabs.Add(new ClickableComponent(
                    new Rectangle(labelX, labelY + labelHeight * i++, Game1.tileSize * 5, Game1.tileSize), MenuTab.Monthly.ToString(), i18n.Get("tabs.monthly")));
                this.Tabs.Add(new ClickableComponent(
                    new Rectangle(labelX, labelY + labelHeight * i++, Game1.tileSize * 5, Game1.tileSize), MenuTab.Add.ToString(), i18n.Get("tabs.add")));
                this.Tabs.Add(new ClickableComponent(
                    new Rectangle(labelX, labelY + labelHeight * i++, Game1.tileSize * 5, Game1.tileSize), MenuTab.Remove.ToString(), i18n.Get("tabs.remove")));
            }

            this.UpArrow = new ClickableTextureComponent(
                "up-arrow",
                new Rectangle(this.xPositionOnScreen + this.width + Game1.tileSize / 4, this.yPositionOnScreen + Game1.tileSize, 11 * Game1.pixelZoom, 12 * Game1.pixelZoom),
                "",
                "",
                Game1.mouseCursors,
                new Rectangle(421, 459, 11, 12),
                Game1.pixelZoom
                );
            this.DownArrow = new ClickableTextureComponent(
                "down-arrow",
                new Rectangle(this.xPositionOnScreen + this.width + Game1.tileSize / 4, this.yPositionOnScreen + this.height - Game1.tileSize, 11 * Game1.pixelZoom, 12 * Game1.pixelZoom),
                "",
                "",
                Game1.mouseCursors,
                new Rectangle(421, 472, 11, 12),
                Game1.pixelZoom
                );
            this.Scrollbar = new ClickableTextureComponent(
                "scrollbar",
                new Rectangle(this.UpArrow.bounds.X + Game1.pixelZoom * 3, this.UpArrow.bounds.Y + this.UpArrow.bounds.Height + Game1.pixelZoom, 6 * Game1.pixelZoom, 10 * Game1.pixelZoom),
                "",
                "",
                Game1.mouseCursors,
                new Rectangle(435, 463, 6, 10),
                Game1.pixelZoom
                );
            this.ScrollbarRunner = new Rectangle(
                this.Scrollbar.bounds.X,
                this.UpArrow.bounds.Y + this.UpArrow.bounds.Height + Game1.pixelZoom,
                this.Scrollbar.bounds.Width,
                this.height - Game1.tileSize * 2 - this.UpArrow.bounds.Height - Game1.pixelZoom * 2
                );
            for (int i = 0; i < ItemsPerPage; i++)
                this.OptionSlots.Add(new ClickableComponent(
                    new Rectangle(
                        this.xPositionOnScreen + Game1.tileSize / 4,
                        this.yPositionOnScreen + Game1.tileSize * 5 / 4 + Game1.pixelZoom + i * ((this.height - Game1.tileSize * 2) / PlannerMenu.ItemsPerPage),
                        this.width - Game1.tileSize / 2,
                        (this.height - Game1.tileSize * 2) / PlannerMenu.ItemsPerPage + Game1.pixelZoom
                        ),
                    string.Concat(i)
                    ));

            int slotWidth = this.OptionSlots[0].bounds.Width;
            switch (this.CurrentTab)
            {
                case MenuTab.Daily:
                    string label = this.Planner.DayToString(StardewModdingAPI.Utilities.SDate.Now().SeasonIndex + 1, StardewModdingAPI.Utilities.SDate.Now().Day) + ":";
                    this.Options.Add(new OptionsElement(label));
                    foreach (string task in this.Planner?.GetDailyPlan())
                    {
                        this.Options.Add(new PlannerListComponent(task, slotWidth, this.Planner, this));
                    }
                    break;

                case MenuTab.Checklist:
                    this.Options.Add(new TextBoxComponent(TaskType.Checklist, slotWidth, this));
                    foreach (string task in this.CheckList?.GetCheckListItems())
                    {
                        this.Options.Add(new PlannerListComponent(task, slotWidth, this.CheckList, this));
                    }
                    break;

                case MenuTab.Weekly:
                    foreach (string line in this.Planner.CreateWeekList())
                    {
                        if (line.Last() == ':') this.Options.Add(new OptionsElement(line));
                        else this.Options.Add(new PlannerListComponent(line, slotWidth, planner, this, false));
                    }
                    break;

                case MenuTab.Monthly:
                    foreach (string line in this.Planner.CreateMonthList())
                    {
                        if (line.Last() == ':')this.Options.Add(new OptionsElement(line));
                        else this.Options.Add(new PlannerListComponent(line, slotWidth, planner, this, false));
                    }
                    break;

                case MenuTab.Add:
                    this.Options.Add(new OptionsElement(i18n.Get("instructions.add_one_day")));
                    this.Options.Add(OnDateSeasonPlusMinus);
                    this.Options.Add(OnDateDayPlusMinus);
                    this.Options.Add(new TextBoxComponent(TaskType.OnDate, slotWidth, this));

                    this.Options.Add(new OptionsElement(""));
                    this.Options.Add(new OptionsElement(i18n.Get("instructions.add_daily")));
                    this.Options.Add(DailySeasonPlusMinus);
                    this.Options.Add(new TextBoxComponent(TaskType.Daily, slotWidth, this));

                    this.Options.Add(new OptionsElement(""));
                    this.Options.Add(new OptionsElement(i18n.Get("instructions.add_weekly")));
                    this.Options.Add(WeeklySeasonPlusMinus);
                    this.Options.Add(WeeklyDayOfWeekPlusMinus);
                    this.Options.Add(new TextBoxComponent(TaskType.Weekly, slotWidth, this));
                    break;
                case MenuTab.Remove:
                    this.Options.Add(RemoveTaskSeasonPlusMinus);
                    this.Options.Add(RemoveTaskDayPlusMinus);
                    this.RefreshRemoveTaskTab();
                    break;
            }
            this.SetScrollBarToCurrentIndex();
        }

        /// <summary>
        /// Constructor to create a new planner menu using the variables of an old one. Used to refresh the view.
        /// </summary>
        /// <param name="oldMenu"></param>
        public PlannerMenu(PlannerMenu oldMenu) : this(oldMenu.CurrentTab, oldMenu.Config, oldMenu.Planner, oldMenu.CheckList, oldMenu.TranslationHelper, oldMenu.Monitor)
        {
            this.CurrentItemIndex = oldMenu.CurrentItemIndex;
            this.SetScrollBarToCurrentIndex();
            this.CanClose = true;
        }

        /// <summary>
        /// Constructor to create a new planner menu using the variables of the old one, but switching tabs.
        /// </summary>
        /// <param name="oldMenu"></param>
        /// <param name="newTab"></param>
        public PlannerMenu(PlannerMenu oldMenu, MenuTab newTab) : this(newTab, oldMenu.Config, oldMenu.Planner, oldMenu.CheckList, oldMenu.TranslationHelper, oldMenu.Monitor)
        {
            this.CurrentItemIndex = 0;
            this.SetScrollBarToCurrentIndex();
            this.CanClose = true;
        }

        /// <summary>
        /// This should get called when loading the 'remove task' tab, as well as clicking on a task on that tab.
        /// </summary>
        public void RefreshRemoveTaskTab()
        {
            if (this.CurrentTab == MenuTab.Remove)
            {
                if (this.Options.Count > 2) this.Options.RemoveRange(2, this.Options.Count - 2);
                int slotWidth = this.OptionSlots[0].bounds.Width;
                int season = this.RemoveTaskSeasonPlusMinus.GetOutputInt() + 1;
                int day = this.RemoveTaskDayPlusMinus.GetOutputInt();
                
                // All year - daily tasks
                foreach (string line in this.Planner.GetTasksBySeasonTypeAndDate(0, TaskType.Daily, day))
                {
                    this.Options.Add(new RemoveTaskComponent(TaskType.Daily, day, line, slotWidth, this.Planner, this));
                }
                // All year - weekly tasks
                foreach (string line in this.Planner.GetTasksBySeasonTypeAndDate(0, TaskType.Weekly, day))
                {
                    this.Options.Add(new RemoveTaskComponent(TaskType.Weekly, day, line, slotWidth, this.Planner, this));
                }
                // Current season - daily tasks
                foreach (string line in this.Planner.GetTasksBySeasonTypeAndDate(season, TaskType.Daily, day))
                {
                    this.Options.Add(new RemoveTaskComponent(TaskType.Daily, season, day, line, slotWidth, this.Planner, this));
                }
                // Current season - weekly tasks
                foreach (string line in this.Planner.GetTasksBySeasonTypeAndDate(season, TaskType.Weekly, day))
                {
                    this.Options.Add(new RemoveTaskComponent(TaskType.Weekly, season, day, line, slotWidth, this.Planner, this));
                }
                // Current season - on date tasks
                foreach (string line in this.Planner.GetTasksBySeasonTypeAndDate(season, TaskType.OnDate, day))
                {
                    this.Options.Add(new RemoveTaskComponent(TaskType.OnDate, season, day, line, slotWidth, this.Planner, this));
                }
            }
        }

        public override void leftClickHeld(int x, int y)
        {
            if (GameMenu.forcePreventClose)
                return;
            base.leftClickHeld(x, y);
            if (this.IsScrolling)
            {
                int num = this.Scrollbar.bounds.Y;
                this.Scrollbar.bounds.Y = Math.Min(this.yPositionOnScreen + this.height - Game1.tileSize - Game1.pixelZoom * 3 - this.Scrollbar.bounds.Height, Math.Max(
                    y, this.yPositionOnScreen + this.UpArrow.bounds.Height + Game1.pixelZoom * 5
                    ));
                this.CurrentItemIndex = Math.Min(this.Options.Count - PlannerMenu.ItemsPerPage, Math.Max(0, (int)(this.Options.Count * (double)((y - this.ScrollbarRunner.Y) / (float)this.ScrollbarRunner.Height))));
                this.SetScrollBarToCurrentIndex();
                if (num == this.Scrollbar.bounds.Y)
                    return;
                Game1.soundBank.PlayCue("shiny4");
            }
            else
            {
                if (this.OptionsSlotHeld == -1 || this.OptionsSlotHeld + this.CurrentItemIndex >= this.Options.Count)
                    return;
                this.Options[this.CurrentItemIndex + this.OptionsSlotHeld].leftClickHeld(x - this.OptionSlots[this.OptionsSlotHeld].bounds.X, y - this.OptionSlots[this.OptionsSlotHeld].bounds.Y);
            }
        }

        public override void receiveKeyPress(Keys key)
        {
            bool isExitKey = Game1.options.menuButton.Contains(new InputButton(key)) || (this.Config.OpenMenuKey.TryGetKeyboard(out Keys exitKey) && key == exitKey);

            if (isExitKey && this.readyToClose() && this.CanClose && !this.HasSelectedTextbox)
            {
                Game1.exitActiveMenu();
                Game1.soundBank.PlayCue("bigDeSelect");
                return;
            }

            // If this is set to true in the constructor, the menu just closes immediately upon opening. It needs to be set to true here instead.
            this.CanClose = true;

            if ((key == Keys.Enter || key == Keys.Escape) && this.HasSelectedTextbox)
            {
                this.SelectedTextBox?.DeselectInputBox();
                return;
            }

            if (this.OptionsSlotHeld == -1 || this.OptionsSlotHeld + this.CurrentItemIndex >= this.Options.Count)
                return;
            this.Options[this.CurrentItemIndex + this.OptionsSlotHeld].receiveKeyPress(key);
        }

        /// <summary>Exit the menu if that's allowed for the current state.</summary>
        public void ExitIfValid()
        {
            if (this.readyToClose() && !GameMenu.forcePreventClose)
            {
                Game1.exitActiveMenu();
                Game1.playSound("bigDeSelect");
            }
        }

        public override void receiveGamePadButton(Buttons key)
        {
            if (key == Buttons.LeftShoulder || key == Buttons.RightShoulder)
            {
                // rotate tab index
                int index = this.Tabs.FindIndex(p => p.name == this.CurrentTab.ToString());
                if (key == Buttons.LeftShoulder)
                    index--;
                if (key == Buttons.RightShoulder)
                    index++;

                if (index >= this.Tabs.Count)
                    index = 0;
                if (index < 0)
                    index = this.Tabs.Count - 1;

                // open menu with new index
                MenuTab tabID = GetTabID(this.Tabs[index]);
                Game1.activeClickableMenu = new PlannerMenu(this, tabID);
            }
        }

        public override void receiveScrollWheelAction(int direction)
        {
            if (GameMenu.forcePreventClose)
                return;
            base.receiveScrollWheelAction(direction);
            if (direction > 0 && this.CurrentItemIndex > 0)
                this.UpArrowPressed();
            else
            {
                if (direction >= 0 || this.CurrentItemIndex >= Math.Max(0, this.Options.Count - PlannerMenu.ItemsPerPage))
                    return;
                this.DownArrowPressed();
            }
        }

        public override void releaseLeftClick(int x, int y)
        {
            if (GameMenu.forcePreventClose)
                return;
            base.releaseLeftClick(x, y);
            if (this.OptionsSlotHeld != -1 && this.OptionsSlotHeld + this.CurrentItemIndex < this.Options.Count)
                this.Options[this.CurrentItemIndex + this.OptionsSlotHeld].leftClickReleased(x - this.OptionSlots[this.OptionsSlotHeld].bounds.X, y - this.OptionSlots[this.OptionsSlotHeld].bounds.Y);
            this.OptionsSlotHeld = -1;
            this.IsScrolling = false;
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            if (GameMenu.forcePreventClose)
                return;
            if (this.DownArrow.containsPoint(x, y) && this.CurrentItemIndex < Math.Max(0, this.Options.Count - PlannerMenu.ItemsPerPage))
            {
                this.DownArrowPressed();
                Game1.soundBank.PlayCue("shwip");
            }
            else if (this.UpArrow.containsPoint(x, y) && this.CurrentItemIndex > 0)
            {
                this.UpArrowPressed();
                Game1.soundBank.PlayCue("shwip");
            }
            else if (this.Scrollbar.containsPoint(x, y))
                this.IsScrolling = true;
            else if (!this.DownArrow.containsPoint(x, y) && x > this.xPositionOnScreen + this.width && (x < this.xPositionOnScreen + this.width + Game1.tileSize * 2 && y > this.yPositionOnScreen) && y < this.yPositionOnScreen + this.height)
            {
                this.IsScrolling = true;
                this.leftClickHeld(x, y);
                this.releaseLeftClick(x, y);
            }
            this.CurrentItemIndex = Math.Max(0, Math.Min(this.Options.Count - PlannerMenu.ItemsPerPage, this.CurrentItemIndex));
            for (int index = 0; index < this.OptionSlots.Count; ++index)
            {
                if (this.OptionSlots[index].bounds.Contains(x, y) && this.CurrentItemIndex + index < this.Options.Count && this.Options[this.CurrentItemIndex + index].bounds.Contains(x - this.OptionSlots[index].bounds.X, y - this.OptionSlots[index].bounds.Y - 5))
                {
                    this.Options[this.CurrentItemIndex + index].receiveLeftClick(x - this.OptionSlots[index].bounds.X, y - this.OptionSlots[index].bounds.Y + 5);
                    this.OptionsSlotHeld = index;
                    break;
                }
            }

            foreach (var tab in this.Tabs)
            {
                if (tab.bounds.Contains(x, y))
                {
                    MenuTab tabID = GetTabID(tab);
                    Game1.activeClickableMenu = new PlannerMenu(this, tabID);
                    break;
                }
            }
        }

        public override void receiveRightClick(int x, int y, bool playSound = true) { }

        public override void performHoverAction(int x, int y)
        {
            if (GameMenu.forcePreventClose)
                return;
            this.HoverText = "";
            this.UpArrow.tryHover(x, y);
            this.DownArrow.tryHover(x, y);
            this.Scrollbar.tryHover(x, y);
        }

        public void OnTextBoxButtonPressed(TaskType buttonType, string input)
        {
            switch(buttonType)
            {
                case TaskType.OnDate:
                    this.Planner.AddTask(this.OnDateSeasonPlusMinus.GetOutputInt() + 1, buttonType, this.OnDateDayPlusMinus.GetOutputInt(), input);
                    break;
                case TaskType.Daily:
                    this.Planner.AddTask(this.DailySeasonPlusMinus.GetOutputInt(), buttonType, 0, input);
                    break;
                case TaskType.Weekly:
                    this.Planner.AddTask(this.WeeklySeasonPlusMinus.GetOutputInt(), buttonType, this.WeeklyDayOfWeekPlusMinus.GetOutputInt(), input);
                    break;
                case TaskType.Checklist:
                    this.CheckList.AddTask(input); 
                    Game1.activeClickableMenu = new PlannerMenu(this);
                    break;
                default:
                    break;
            }
        }

        public override void draw(SpriteBatch spriteBatch)
        {
            // Darken background while menu is open
            if (!Game1.options.showMenuBackground)
                spriteBatch.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.4f);
            
            // Draw dialogue box
            Game1.drawDialogueBox(this.xPositionOnScreen, this.yPositionOnScreen, this.width, this.height, false, true);
            
            // Draw title box
            DrawTextBox(this.Title.bounds.X, this.Title.bounds.Y, Game1.dialogueFont, this.Title.name, 1);
            spriteBatch.End();

            // Draw option slots
            spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.NonPremultiplied, SamplerState.PointClamp, null, null);
            for (int index = 0; index < this.OptionSlots.Count; ++index)
            {
                if (this.CurrentItemIndex >= 0 && this.CurrentItemIndex + index < this.Options.Count)
                    this.Options[this.CurrentItemIndex + index].draw(spriteBatch, this.OptionSlots[index].bounds.X, this.OptionSlots[index].bounds.Y + 5);
            }
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null);
            if (!GameMenu.forcePreventClose)
            {
                // Draw tabs on left side of screen
                foreach (ClickableComponent tab in this.Tabs)
                {
                    MenuTab tabID = GetTabID(tab);
                    DrawTextBox(tab.bounds.X + tab.bounds.Width, tab.bounds.Y, Game1.smallFont, tab.label, 2, this.CurrentTab == tabID ? 1F : 0.7F);
                }
                
                // Draw scroll wheel and arrows if needed
                if (this.Options.Count > PlannerMenu.ItemsPerPage)
                {
                    this.UpArrow.draw(spriteBatch);
                    this.DownArrow.draw(spriteBatch);
                    IClickableMenu.drawTextureBox(
                        spriteBatch,
                        Game1.mouseCursors,
                        new Rectangle(403, 383, 6, 6),
                        this.ScrollbarRunner.X,
                        this.ScrollbarRunner.Y,
                        this.ScrollbarRunner.Width,
                        this.ScrollbarRunner.Height,
                        Color.White,
                        Game1.pixelZoom,
                        false);
                    this.Scrollbar.draw(spriteBatch);
                }

            }

            // Draw hovertext if any
            if (this.HoverText != "")
                IClickableMenu.drawHoverText(spriteBatch, this.HoverText, Game1.smallFont);

            // Draw cursor
            if (!Game1.options.hardwareCursor)
                spriteBatch.Draw(
                    Game1.mouseCursors,
                    new Vector2(Game1.getOldMouseX(), Game1.getOldMouseY()),
                    Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, Game1.options.gamepadControls ? 44 : 0, 16, 16),
                    Color.White,
                    0f,
                    Vector2.Zero,
                    Game1.pixelZoom + Game1.dialogueButtonScale / 150f,
                    SpriteEffects.None,
                    1f);
        }

        /*********
        ** Private methods
        *********/
        
        /// <summary>
        /// Sets the scrollbar's postion based on current index. 
        /// </summary>
        private void SetScrollBarToCurrentIndex()
        {
            if (!this.Options.Any())
                return;
            this.Scrollbar.bounds.Y = this.ScrollbarRunner.Height / Math.Max(1, this.Options.Count - PlannerMenu.ItemsPerPage + 1) * this.CurrentItemIndex + this.UpArrow.bounds.Bottom
                + Game1.pixelZoom;
            if (this.CurrentItemIndex != this.Options.Count - PlannerMenu.ItemsPerPage)
                return;
            this.Scrollbar.bounds.Y = this.DownArrow.bounds.Y - this.Scrollbar.bounds.Height - Game1.pixelZoom;
        }

        private void DownArrowPressed()
        {
            this.DownArrow.scale = this.DownArrow.baseScale;
            ++this.CurrentItemIndex;
            this.SetScrollBarToCurrentIndex();
        }

        private void UpArrowPressed()
        {
            this.UpArrow.scale = this.UpArrow.baseScale;
            --this.CurrentItemIndex;
            this.SetScrollBarToCurrentIndex();
        }

        /// <summary>Get the tab constant represented by a tab component.</summary>
        /// <param name="tab">The component to check.</param>
        private static MenuTab GetTabID(ClickableComponent tab)
        {
            if (!Enum.TryParse(tab.name, out MenuTab tabID))
                throw new InvalidOperationException($"Couldn't parse tab name '{tab.name}'.");
            return tabID;
        }
        public override bool overrideSnappyMenuCursorMovementBan()
        {
            return true;
        }

        public static void DrawTextBox(int x, int y, SpriteFont font, string message, int align = 0, float colorIntensity = 1F)
        {
            SpriteBatch spriteBatch = Game1.spriteBatch;

            Vector2 bounds = font.MeasureString(message);
            int width = (int)bounds.X + Game1.tileSize / 2;
            int height = (int)font.MeasureString(message).Y + Game1.tileSize / 3;
            switch (align)
            {
                case 0:
                    IClickableMenu.drawTextureBox(spriteBatch, Game1.menuTexture, new Rectangle(0, 256, 60, 60), x, y, width, height + Game1.tileSize / 16, Color.White * colorIntensity);
                    Utility.drawTextWithShadow(spriteBatch, message, font, new Vector2(x + Game1.tileSize / 4, y + Game1.tileSize / 4), Game1.textColor);
                    break;
                case 1:
                    IClickableMenu.drawTextureBox(spriteBatch, Game1.menuTexture, new Rectangle(0, 256, 60, 60), x - width / 2, y, width, height + Game1.tileSize / 16, Color.White * colorIntensity);
                    Utility.drawTextWithShadow(spriteBatch, message, font, new Vector2(x + Game1.tileSize / 4 - width / 2, y + Game1.tileSize / 4), Game1.textColor);
                    break;
                case 2:
                    IClickableMenu.drawTextureBox(spriteBatch, Game1.menuTexture, new Rectangle(0, 256, 60, 60), x - width, y, width, height + Game1.tileSize / 16, Color.White * colorIntensity);
                    Utility.drawTextWithShadow(spriteBatch, message, font, new Vector2(x + Game1.tileSize / 4 - width, y + Game1.tileSize / 4), Game1.textColor);
                    break;
            }
        }
    }
}
