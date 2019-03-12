using DailyTasksReport.Tasks;
using DailyTasksReport.UI;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using System;
using System.Collections.Generic;

namespace DailyTasksReport
{
    /// <inheritdoc />
    /// <summary>The mod entry point.</summary>
    // ReSharper disable once ClassNeverInstantiated.Global
    public class ModEntry : Mod
    {
        internal static IReflectionHelper ReflectionHelper;
        internal static IModEvents EventsHelper;
        internal static IInputHelper InputHelper;
        private readonly List<Task> _tasks = new List<Task>();

        private bool _firstRun = true;
        private bool _refreshReport;

        internal ModConfig Config;

        /*********
        ** Public methods
        *********/

        /// <inheritdoc />
        public override void Entry(IModHelper helper)
        {
            ReflectionHelper = helper.Reflection;
            EventsHelper = helper.Events;
            InputHelper = helper.Input;

            Config = helper.ReadConfig<ModConfig>();
            if (Config.Check(Monitor))
                helper.WriteConfig(Config);

            SetupTasks();

            helper.Events.GameLoop.ReturnedToTitle += GameLoop_ReturnedToTitle;
            helper.Events.GameLoop.SaveLoaded += GameLoop_SaveLoaded;
            helper.Events.GameLoop.DayStarted += GameLoop_DayStarted;

            // In-game Events
            helper.Events.Input.ButtonPressed += Input_ButtonPressed;
            helper.Events.Display.MenuChanged += Display_MenuChanged;
            SettingsMenu.ReportConfigChanged += SettingsMenu_ReportConfigChanged;

            // Draw Events
            helper.Events.Display.RenderingHud += Display_RenderingHud;
        }

        private void GameLoop_ReturnedToTitle(object sender, ReturnedToTitleEventArgs e)
        {
            _tasks.ForEach(t => t.Clear());
        }

        private void GameLoop_SaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            // If inserted last and player has no quest, DayTimeMoneyBox will receive left click
            Game1.onScreenMenus.Insert(0, new ReportButton(this, OpenReport));
        }

        // Setup events

        private void SetupTasks()
        {
            _tasks.Add(new MiscTask(Config));
            _tasks.Add(new CropsTask(Config, CropsTaskId.UnwateredCropFarm));
            _tasks.Add(new CropsTask(Config, CropsTaskId.UnwateredCropGreenhouse));
            _tasks.Add(new CropsTask(Config, CropsTaskId.UnharvestedCropFarm));
            _tasks.Add(new CropsTask(Config, CropsTaskId.UnharvestedCropGreenhouse));
            _tasks.Add(new CropsTask(Config, CropsTaskId.DeadCropFarm));
            _tasks.Add(new CropsTask(Config, CropsTaskId.DeadCropGreenhouse));
            _tasks.Add(new CropsTask(Config, CropsTaskId.FruitTreesFarm));
            _tasks.Add(new CropsTask(Config, CropsTaskId.FruitTreesGreenhouse));
            _tasks.Add(new PetTask(Config));
            _tasks.Add(new AnimalsTask(Config, AnimalsTaskId.UnpettedAnimals));
            _tasks.Add(new AnimalsTask(Config, AnimalsTaskId.AnimalProducts));
            _tasks.Add(new AnimalsTask(Config, AnimalsTaskId.MissingHay));
            _tasks.Add(new FarmCaveTask(Config));
            _tasks.Add(new ObjectsTask(Config, ObjectsTaskId.UncollectedCrabpots));
            _tasks.Add(new ObjectsTask(Config, ObjectsTaskId.NotBaitedCrabpots));
            _tasks.Add(new ObjectsTask(Config, ObjectsTaskId.UncollectedMachines));
        }

        private void GameLoop_DayStarted(object sender, DayStartedEventArgs e)
        {
            _tasks.ForEach(t => t.OnDayStarted());

            if (!_firstRun) return;

            Report.GetReportText(_tasks, true); // Let JIT do it's thing
            _firstRun = false;
        }

        // In-game events

        private void Display_MenuChanged(object sender, MenuChangedEventArgs e)
        {
            if (_refreshReport && e.OldMenu is SettingsMenu && e.NewMenu is ReportMenu)
                OpenReport(true);
            _refreshReport = false;
        }

        private void Input_ButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (!Context.IsWorldReady || !Context.IsPlayerFree || e.Button == SButton.None)
                return;

            if (e.Button == Config.OpenReportKey)
            {
                OpenReport();
            }
            else if (e.Button == Config.OpenSettings)
            {
                SettingsMenu.OpenMenu(this);
            }
            else if (e.Button == Config.ToggleBubbles)
            {
                Config.DisplayBubbles = !Config.DisplayBubbles;
                Helper.WriteConfig(Config);
            }
        }

        private void SettingsMenu_ReportConfigChanged(object sender, EventArgs e)
        {
            _refreshReport = true;
        }

        private void OpenReport(bool skipAnimation = false)
        {
            if (Game1.activeClickableMenu != null)
                Game1.exitActiveMenu();
            Game1.activeClickableMenu =
                new ReportMenu(this, Report.GetReportText(_tasks, Config.ShowDetailedInfo), skipAnimation);
        }

        // Draw Events

        private void Display_RenderingHud(object sender, RenderingHudEventArgs e)
        {
            if (!Config.DisplayBubbles || !Context.IsWorldReady || Game1.currentMinigame != null ||
                Game1.showingEndOfNightStuff || Game1.CurrentEvent != null) return;
            _tasks.ForEach(t => t.Draw(Game1.spriteBatch));
        }
    }
}