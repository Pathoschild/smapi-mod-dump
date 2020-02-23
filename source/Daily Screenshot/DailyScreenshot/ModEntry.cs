using Microsoft.Xna.Framework.Input;
using Netcode;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace DailyScreenshot
{
    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod
    {
#region Constants 
        /// <summary>
        /// Maximum attempts to move the file
        /// </summary>
        private const int MAX_ATTEMPTS_TO_MOVE = 10000;

        /// <summary>
        /// Sharing violation code
        /// </summary>
        private const int SHARING_VIOLATION = 32;

        /// <summary>
        /// Tick countdown
        /// </summary>
        private const int MAX_COUNTDOWN_IN_TICKS = 60;

        /// <summary>
        /// Time to sleep between move attempts
        /// </summary>
        private const int MILLISECONDS_TIMEOUT = 10;

#endregion

        /// <summary>The mod configuration from the player.</summary>
        private ModConfig Config;

        private bool screenshotTakenToday = false;

        int countdownInTicks = MAX_COUNTDOWN_IN_TICKS;

        public string defaultStardewValleyScreenshotsDirectory { get; private set; }

        /// <summary>
        /// Check that a directory contains no files or directories
        /// </summary>
        /// <param name="path">Directory to check</param>
        /// <returns>true if the directory is empty</returns>
        private bool DirectoryIsEmpty(string path) => Directory.GetDirectories(path).Length == 0 && Directory.GetFiles(path).Length == 0;
#region Logging
        /// <summary>
        /// Sends messages to the SMAPI console
        /// </summary>
        /// <param name="message">text to send</param>
        /// <param name="level">type of message</param>
        // Private copy so there's one place to alter all log messages if needed
#if DEBUG
        private void LogMessageToConsole(string message, LogLevel level) => Monitor.Log(message, level);
#else
        private void LogMessageToConsole(string message, LogLevel level) => Monitor.VerboseLog(message, level);
#endif


        /// <summary>
        /// Helper function for sending trace messages
        /// </summary>
        /// <param name="message">text to send</param>
        private void MTrace(string message) => LogMessageToConsole(message, LogLevel.Trace);


        /// <summary>
        /// Helper function for sending trace messages
        /// </summary>
        /// <param name="message">text to send</param>
        private void MDebug(string message) => LogMessageToConsole(message, LogLevel.Debug);

        /// <summary>
        /// Helper function for sending trace messages
        /// </summary>
        /// <param name="message">text to send</param>
        private void MInfo(string message) => LogMessageToConsole(message, LogLevel.Info);

        /// <summary>
        /// Helper function for sending trace messages
        /// </summary>
        /// <param name="message">text to send</param>
        private void MAlert(string message) => LogMessageToConsole(message, LogLevel.Alert);

        /// <summary>
        /// Helper function for sending warning messages
        /// </summary>
        /// <param name="message">text to send</param>
        private void MWarn(string message) => LogMessageToConsole(message, LogLevel.Warn);

        /// <summary>
        /// Helper function for sending error messages
        /// Always display even if verbose logging is off
        /// </summary>
        /// <param name="message">text to send</param>
        private void MError(string message) => Monitor.Log(message, LogLevel.Error);
#endregion

        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            Config = Helper.ReadConfig<ModConfig>();
            int num11 = Environment.OSVersion.Platform != PlatformID.Unix ? 26 : 28;
            var path = Environment.GetFolderPath((Environment.SpecialFolder)num11);

            // path is combined with StardewValley and then Screenshots
            defaultStardewValleyScreenshotsDirectory = Path.Combine(path, "StardewValley", "Screenshots");
            Helper.Events.GameLoop.GameLaunched += OnGameLaunched;
        }

        /// <summary>Raised after the save file is loaded.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            Helper.Events.Player.Warped += OnWarped;
            Helper.Events.GameLoop.DayStarted += OnDayStarted;
            Helper.Events.GameLoop.ReturnedToTitle += OnReturnedToTitle;
            Helper.Events.Input.ButtonPressed += OnButtonPressed;
        }

        /// <summary>Raised after a button is pressed.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (e.Button.TryGetKeyboard(out Keys _))
            {
                if (e.Button == Config.TakeScreenshotKey)
                {
                    TakeScreenshotViaKeypress();
                }
            }
        }

        /// <summary>Raised after day has started.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnDayStarted(object sender, DayStartedEventArgs e)
        {
            Helper.Events.GameLoop.UpdateTicked -= OnUpdateTicked;
            screenshotTakenToday = false;
            countdownInTicks = MAX_COUNTDOWN_IN_TICKS;

            EnqueueAction(() =>
            {
                CheckScreenshotAction();
            });
        }

        /// <summary>Raised after the player enters a new location.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnWarped(object sender, WarpedEventArgs e)
        {
            if (e.NewLocation is Farm && !screenshotTakenToday && Game1.timeOfDay >= Config.TimeScreenshotGetsTakenAfter)
            {
                Helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;
            }
        }

        /// <summary>Raised after game state is updated.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            countdownInTicks--;

            if (countdownInTicks == 0)
            {
                while (_actions.Count > 0)
                    _actions.Dequeue().Invoke();
            }
        }

        /// <summary>Checks whether it is the appropriate day to take a screenshot of the entire farm.</summary>
        private void CheckScreenshotAction()
        {
            if (Config.TakeScreenshotOnRainyDays == false && (Game1.isRaining == true || Game1.isLightning == true))
            {
                return;
            }

            if (Config.HowOftenToTakeScreenshot["Daily"] == true)
            {
                AutoScreenshot();
            }
            else if (Config.HowOftenToTakeScreenshot[Game1.Date.DayOfWeek + "s"] == true)
            {
                AutoScreenshot();
            }
            else if (Config.HowOftenToTakeScreenshot["First Day of Month"] == true && Game1.Date.DayOfMonth.ToString() == "1")
            {
                AutoScreenshot();
            }
            else if (Config.HowOftenToTakeScreenshot["Last Day of Month"] == true && Game1.Date.DayOfMonth.ToString() == "28")
            {
                AutoScreenshot();
            }
        }

        /// <summary>Takes a screenshot of the entire farm.</summary>
        private void AutoScreenshot()
        {
            // path must be relative
            string autoSSDirectory = Game1.player.farmName + "-Farm-Screenshots-" + Game1.uniqueIDForThisGame;
            string screenshotPath = Path.Combine(autoSSDirectory, GameDateToName());
            // yes, we're going to create the directory even if we move out of it and delete it
            // this is for two reasons.
            // 1. Ensures we're not stomping over another file unless it was made by this mod
            // 2. Gives a good fallback location if the move fails for some reason
            if (!Directory.Exists(Path.Combine(defaultStardewValleyScreenshotsDirectory, autoSSDirectory)))
            {
                Directory.CreateDirectory(Path.Combine(defaultStardewValleyScreenshotsDirectory, autoSSDirectory));
            }
            string mapScreenshot = Game1.game1.takeMapScreenshot(Config.TakeScreenshotZoomLevel, screenshotPath);
            MTrace($"Snapshot saved to {mapScreenshot}");
            Game1.addHUDMessage(new HUDMessage(Path.GetFileName(mapScreenshot), HUDMessage.screenshot_type));
            Game1.playSound("cameraNoise");
            screenshotTakenToday = true;
            if(ModConfig.DEFAULT_FOLDER != Config.FolderDestinationForDailyScreenshots)
            {
                MoveScreenshotToCorrectFolder(mapScreenshot);
                if (DirectoryIsEmpty(Path.Combine(defaultStardewValleyScreenshotsDirectory, autoSSDirectory)))
                {
                    Directory.Delete(Path.Combine(defaultStardewValleyScreenshotsDirectory, autoSSDirectory));
                }
            }
        }

        /// <summary>Takes a screenshot of the entire map, activated via keypress.</summary>
        private void TakeScreenshotViaKeypress()
        {
            string mapScreenshot = Game1.game1.takeMapScreenshot(Config.TakeScreenshotKeyZoomLevel);
            Game1.addHUDMessage(new HUDMessage(mapScreenshot, 6));
            Game1.playSound("cameraNoise");
            MTrace($"Snapshot saved to {mapScreenshot}");

            if (ModConfig.DEFAULT_FOLDER != Config.FolderDestinationForKeypressScreenshots)
            {
                MoveScreenshotToCorrectFolder(mapScreenshot, true);
            }
        }

        private Queue<Action> _actions = new Queue<Action>();

        /// <summary>Allows ability to enqueue actions to the queue.</summary>
        /// <param name="action">The action.</param>
        public void EnqueueAction(Action action)
        {
            if (null == action) return;
            _actions.Enqueue(action);
        }

        /// <summary>
        /// Turns the current game date into a constant filename for the day
        /// Setup so OS naturally keeps the files in order
        /// </summary>
        /// <returns>01-02-03 for year 1, summer, day 3</returns>
        private string GameDateToName()
        {
            return string.Format("{0:D2}-{1:D2}-{2:D2}",Game1.Date.Year, Game1.Date.SeasonIndex+1, Game1.Date.DayOfMonth);
        }

        /// <summary>Moves screenshot into StardewValley/Screenshots directory, in the save file folder.</summary>
        /// <param name="screenshotPath">The name of the screenshot file.</param>
        /// <param name="keypress">true if the user pressed the key</param>
        private void MoveScreenshotToCorrectFolder(string screenshotPath, bool keypress = false)
        {
            // special folder path

            // path for original screenshot location and new screenshot location
            string sourceFile = Path.Combine(defaultStardewValleyScreenshotsDirectory, screenshotPath);
            string destinationFile;
            if(keypress)
            {
                destinationFile = Path.Combine(Config.FolderDestinationForKeypressScreenshots, screenshotPath);
            }
            else
            {
                destinationFile = Path.Combine(Config.FolderDestinationForDailyScreenshots, screenshotPath);
            }
            MTrace($"Snapshot moving from {sourceFile} to {destinationFile}");


            // create save directory if it doesn't already exist
            if (!Directory.Exists(Path.GetDirectoryName(destinationFile)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(destinationFile));
            }

            // wait for screenshot to finish
            while (Game1.game1.takingMapScreenshot)
            {
                MTrace("Sleeping while takingMapScreenshot");
                Thread.Sleep(MILLISECONDS_TIMEOUT);
            }
            int attemptCount = 0;
            while (File.Exists(sourceFile) && attemptCount < MAX_ATTEMPTS_TO_MOVE)
            {
                try
                {
                    attemptCount++;
                    using (FileStream lockFile = new FileStream(
                        sourceFile,
                        FileMode.Open,
                        FileAccess.ReadWrite,
                        FileShare.Read | FileShare.Delete
                    ))
                    {
                        // delete old version of screenshot if one exists
                        if (File.Exists(destinationFile))
                        {
                            File.Delete(destinationFile);
                        }
                        File.Move(sourceFile, destinationFile);
                    }
                }
                catch (IOException ex)
                {
                    int HResult = System.Runtime.InteropServices.Marshal.GetHRForException(ex);
                    if (SHARING_VIOLATION == (HResult & 0xFFFF))
                    {
                        // Hiding the warning as it isn't useful to other mod devs
                        //MWarn($"File may be in use, retrying in {MILLISECONDS_TIMEOUT} milliseconds, attempt {attemptCount} of {MAX_ATTEMPTS_TO_MOVE}");
                        Thread.Sleep(MILLISECONDS_TIMEOUT);
                    }
                    else
                    {
                        MError($"Error moving file '{screenshotPath}' to {destinationFile}. Technical details:\n{ex}");
                        attemptCount = MAX_ATTEMPTS_TO_MOVE;
                    }
                }
                catch (Exception ex)
                {
                    MError($"Error moving file '{screenshotPath}' to {destinationFile} folder. Technical details:\n{ex}");
                    attemptCount = MAX_ATTEMPTS_TO_MOVE;
                }
            }
        }

        /// <summary>Raised after the player returns to the title screen.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnReturnedToTitle(object sender, ReturnedToTitleEventArgs e)
        {
            screenshotTakenToday = false;
            countdownInTicks = MAX_COUNTDOWN_IN_TICKS;
        }
    }
}