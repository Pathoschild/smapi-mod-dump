using System;
using System.IO;
using Pathoschild.UI;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using TwilightShards.Stardew.Common;

namespace TwilightShards.ClimatesOfFerngillV2
{
    public class CoFCore : Mod
    {
        /// <summary> The mod configuration </summary>
        internal ClimatesConfig ModConfig;

        /// <summary> The weather tracker </summary>
        internal FerngillConditions WeatherTracker;

        /// <summary> This is used to allow the menu to revert back to a previous menu </summary>
        private IClickableMenu _previousMenu;

        /// <summary>  The entry point.  </summary>
        /// <param name="helper">The helper function, by SMAPI</param>
        public override void Entry(IModHelper helper)
        {
            //create global objects
            ModConfig = Helper.ReadConfig<ClimatesConfig>();
            WeatherTracker = new FerngillConditions();
            
            //read climate file
            var path = Path.Combine("assets", "climates", ModConfig.Climate + ".json");
            var modClimate = helper.Data.ReadJsonFile<ClimateFile>(path);
            if (modClimate is null)
            {
                Monitor.Log($"The selected climate file {ModConfig.Climate} is null. Attempting fallback to default file", LogLevel.Error);
                path = Path.Combine("assets", "climates", "normal.json");
                modClimate = helper.Data.ReadJsonFile<ClimateFile>(path);
                if (modClimate is null)
                {
                    Monitor.Log("The default climate file is not found! Please doublecheck your setting in the mod file!!!", LogLevel.Error);
                    return;
                }
            }

            //process the climate file



            //add event listeners
            //save events
            Helper.Events.GameLoop.SaveLoaded += OnSaveLoad;
            Helper.Events.GameLoop.Saving += OnGameSave;

            //normal events
            Helper.Events.GameLoop.DayStarted += OnNewDay;
            Helper.Events.Multiplayer.ModMessageReceived += SyncMP;
            Helper.Events.GameLoop.ReturnedToTitle += GameReturnToTitle;
            Helper.Events.GameLoop.TimeChanged += TenMinuteUpdate;
            Helper.Events.Player.Warped += PlayerMovingLocations;

            //menu events
            Helper.Events.Input.ButtonPressed += Input_ButtonPressed;
            Helper.Events.Display.MenuChanged += Display_MenuChanged;

            //pikachu events - wait...
        }

        private void OnGameSave(object sender, SavingEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void OnSaveLoad(object sender, SaveLoadedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void OnNewDay(object sender, DayStartedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void TenMinuteUpdate(object sender, TimeChangedEventArgs e)
        {
        }

        private void PlayerMovingLocations(object sender, WarpedEventArgs e)
        {
        }

        /// <summary> Function handling a new menu or menu change </summary>
        private void Display_MenuChanged(object sender, MenuChangedEventArgs e)
        {
            if (!Context.IsMainPlayer)
               return;

            // restore previous menu on close
            if (e.OldMenu is WeatherMenu && this._previousMenu != null)
            {
                Game1.activeClickableMenu = this._previousMenu;
                this._previousMenu = null;
            }
        }

        /// <summary>  Function handling returning to title. </summary>
        private void GameReturnToTitle(object sender, StardewModdingAPI.Events.ReturnedToTitleEventArgs e)
        {
            WeatherTracker.ReturnToTitle();
        }

        private void SyncMP(object sender, StardewModdingAPI.Events.ModMessageReceivedEventArgs e)
        {
            if (Context.IsMainPlayer) return;
        }

        #region Menu
        /// <summary>Raised after the player presses a button on the keyboard, controller, or mouse.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void Input_ButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (ModConfig.WeatherMenuToggle != e.Button)  //sanity force this to exit!
                return;

            if (!Game1.hasLoadedGame)
                return;

            // perform bound action ONLY if there is no menu OR if the menu is a WeatherMenu
            if (Game1.activeClickableMenu == null || Game1.activeClickableMenu is WeatherMenu)
            {
                this.ToggleMenu();
            }
        }

        /// <summary>  Toggle the menu visibility  </summary>
        private void ToggleMenu()
        {
            if (Game1.activeClickableMenu is WeatherMenu)
                this.HideMenu();
            else
                this.ShowMenu();
        }

        /// <summary> Show the menu </summary>
        private void ShowMenu()
        {
            // show menu
            this._previousMenu = Game1.activeClickableMenu;
            string text = "I am a test sentence";
            Game1.activeClickableMenu = new WeatherMenu(Monitor,Helper.Reflection,ModConfig.ScrollAmount,text);
        }

        /// <summary> Hide the menu. </summary>
        private void HideMenu()
        {
            if (Game1.activeClickableMenu is WeatherMenu)
            {
                Game1.playSound("bigDeSelect"); // match default behaviour when closing a menu
                Game1.activeClickableMenu = null;
            }
        }
        #endregion

    }
}
