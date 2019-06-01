using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using DailyPlanner.Framework;

namespace DailyPlanner
{
    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod
    {
        /*********
        ** Fields
        *********/

        /// <summary>The mod settings.</summary>
        private ModConfig Config;

        private Planner Planner;



        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            this.Config = helper.ReadConfig<ModConfig>();
            this.Monitor.Log($"Started with menu key {this.Config.OpenMenuKey}.", LogLevel.Trace);

            helper.Events.Input.ButtonPressed += this.OnButtonPressed;
            helper.Events.GameLoop.DayStarted += this.OnDayStarted;
            //helper.Events.Display.Rendered += this.OnRendered;
            helper.Events.Display.MenuChanged += this.OnMenuChanged;
            //helper.Events.GameLoop.SaveLoaded += this.OnSaveLoaded;
            //helper.Events.GameLoop.UpdateTicked += this.OnUpdateTicked;
            //helper.Events.World.LocationListChanged += this.OnLocationListChanged;
        }


        /*********
        ** Private methods
        *********/
        /// <summary>Raised after the player presses a button on the keyboard, controller, or mouse.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            // ignore if player hasn't loaded a save yet
            if (!Context.IsPlayerFree)
                return;

            // Open window if button is tab button
            if ((e.Button == SButton.Tab) & (Context.IsWorldReady))
            {
                Game1.activeClickableMenu = new PlannerMenu(this.Config.DefaultTab, this.Config, this.Planner, this.Helper.Translation);
                Game1.soundBank.PlayCue("bigSelect");
            }
                
        }

        /// <summary>Raised after a game menu is opened, closed, or replaced.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnMenuChanged(object sender, MenuChangedEventArgs e)
        {
            // save config
            if (e.OldMenu is PlannerMenu)
            {
                this.Helper.WriteConfig(this.Config);
            }
        }

        private void OnDayStarted(object sender, DayStartedEventArgs e)
        {
            this.Planner = new Planner(Game1.year, this.Helper.DirectoryPath);
            this.Planner.CreateDailyPlan();
        }

    }
}
