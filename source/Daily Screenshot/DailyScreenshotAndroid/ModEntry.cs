using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DailyScreenshot
{
    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod
    {
        private string stardewValleyYear, stardewValleySeason, stardewValleyDayOfMonth;
        private bool screenshotTakenToday = false;
        private string displayScreenshotFileName = "null";
        private int countdownInSeconds = 60;
        private ulong saveFileCode;

        /// <summary>
        /// Default screenshot directory set in the entry
        /// </summary>
        /// <value>Path to the screenshot directory for this platform</value>
        public DirectoryInfo DefaultScreenshotDirectory { get; private set; }

        /// <summary>
        /// Default screenshot directory set in the entry
        /// </summary>
        /// <value>Path to the screenshot directory for this platform</value>
        public DirectoryInfo DefaultScreenshotSubdirectory { get; private set; }

        /// <summary>
        /// Helper function for sending trace messages
        /// </summary>
        /// <param name="message">text to send</param>
        internal void MTrace(string message) => Monitor.Log(message, LogLevel.Trace);

        /// <summary>
        /// Helper function for sending error messages
        /// Always display even if verbose logging is off
        /// </summary>
        /// <param name="message">text to send</param>
        internal void MError(string message) => Monitor.Log(message, LogLevel.Error);

        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            var stardewValleyRootDirectory = new DirectoryInfo(Constants.ExecutionPath);
            DefaultScreenshotDirectory = stardewValleyRootDirectory.EnumerateDirectories("MapExport").FirstOrDefault();
            if (DefaultScreenshotDirectory == null)
            {
                DefaultScreenshotDirectory = stardewValleyRootDirectory.CreateSubdirectory("MapExport");
            }

            Helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
        }

        /// <summary>Raised after the save file is loaded.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            saveFileCode = Game1.uniqueIDForThisGame;
            var directoryName = Game1.player.farmName + "-Farm-Screenshots-" + saveFileCode;
            DefaultScreenshotSubdirectory = DefaultScreenshotDirectory.CreateSubdirectory(directoryName);

            Helper.Events.Player.Warped += OnWarped;
            Helper.Events.GameLoop.DayStarted += OnDayStarted;
            Helper.Events.GameLoop.ReturnedToTitle += OnReturnedToTitle;
        }

        /// <summary>Raised after day has started.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnDayStarted(object sender, DayStartedEventArgs e)
        {
            ConvertInGameDateToNumericFormat();
            displayScreenshotFileName = $"{stardewValleyYear}-{stardewValleySeason}-{stardewValleyDayOfMonth}.png";

            Helper.Events.GameLoop.UpdateTicked -= OnUpdateTicked;
            screenshotTakenToday = false;
            countdownInSeconds = 60;

            EnqueueAction(() => {
                TakeScreenshot();
            });
        }

        /// <summary>Raised after the player enters a new location.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnWarped(object sender, WarpedEventArgs e)
        {
            if (e.NewLocation is Farm && !screenshotTakenToday)
            {
                Helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;
            }
        }

        /// <summary>Raised after game state is updated.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            countdownInSeconds--;

            if (countdownInSeconds == 0)
            {
                while (_actions.Count > 0)
                    _actions.Dequeue().Invoke();
            }
            if (countdownInSeconds == -1)
            {
                string mapScreenshotPath = "Farm.png"; // screenshot name
                FileInfo mapScreenshot = new FileInfo(Path.Combine(DefaultScreenshotDirectory.FullName, mapScreenshotPath));
                string newScreenshotNameWithExtension = $"{stardewValleyYear}-{stardewValleySeason}-{stardewValleyDayOfMonth}.png";
                MoveScreenshotToCorrectFolder(mapScreenshot, new FileInfo(Path.Combine("//storage//emulated//0//StardewValley//smapi-internal//MapExport//", Game1.player.farmName + "-Farm-Screenshots-" + saveFileCode, newScreenshotNameWithExtension)));
                // was: Path.Combine(DefaultScreenshotDirectory.FullName, DefaultScreenshotSubdirectory.FullName, newScreenshotNameWithExtension))
                Helper.Events.GameLoop.UpdateTicked -= OnUpdateTicked;
            }
        }

        /// <summary>Takes a screenshot of the entire farm.</summary>
        private void TakeScreenshot()
        {
            Helper.ConsoleCommands.Trigger("export", new[] { "Farm", "all" });
            Game1.addHUDMessage(new HUDMessage(displayScreenshotFileName, 6));
            Game1.playSound("cameraNoise");
            screenshotTakenToday = true;
        }

        private Queue<Action> _actions = new Queue<Action>();

        /// <summary>Allows ability to enqueue actions to the queue.</summary>
        /// <param name="action">The action.</param>
        public void EnqueueAction(Action action)
        {
            if (action == null) return;
            _actions.Enqueue(action);
        }

        /// <summary>Fixes the screenshot name to be in the proper format.</summary>
        private void ConvertInGameDateToNumericFormat()
        {
            stardewValleyYear = Game1.Date.Year.ToString();
            stardewValleySeason = Game1.Date.Season.ToString();
            stardewValleyDayOfMonth = Game1.Date.DayOfMonth.ToString();

            // fix year and month to be in numeric format
            if (int.Parse(stardewValleyYear) < 10)
            {
                stardewValleyYear = "0" + stardewValleyYear;
            }
            if (int.Parse(stardewValleyDayOfMonth) < 10)
            {
                stardewValleyDayOfMonth = "0" + stardewValleyDayOfMonth;
            }

            // fix season to be in numeric format
            switch (Game1.Date.Season)
            {
                case "spring":
                    stardewValleySeason = "01";
                    break;
                case "summer":
                    stardewValleySeason = "02";
                    break;
                case "fall":
                    stardewValleySeason = "03";
                    break;
                case "winter":
                    stardewValleySeason = "04";
                    break;
            }
        }

        /// <summary>Moves screenshot into StardewValley/Screenshots directory, in the save file folder.</summary>
        /// <param name="sourceFile">File to move</param>
        /// <param name="destinationFile">Where to move the file</param>
        private void MoveScreenshotToCorrectFolder(FileInfo sourceFile, FileInfo destinationFile)
        {
            sourceFile = new FileInfo("/storage/emulated/0/StardewValley/smapi-internal/MapExport/Farm.png");
            MTrace($"Snapshot moving from {sourceFile} to {destinationFile}");

            // create save directory if it doesn't already exist
            if (!Directory.Exists(destinationFile.DirectoryName))
            {
                Directory.CreateDirectory(destinationFile.DirectoryName);
            }

            // delete old version of screenshot if one exists
            if (destinationFile.Exists)
            {
                destinationFile.Delete();
            }

            try
            {
                sourceFile.MoveTo(destinationFile.FullName);
            }
            catch (Exception ex)
            {
                MError($"Error moving file '{sourceFile.FullName}' to {destinationFile.FullName}. Technical details:\n{ex}");
            }
        }

        /// <summary>Raised after the player returns to the title screen.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnReturnedToTitle(object sender, ReturnedToTitleEventArgs e)
        {
            screenshotTakenToday = false;
            countdownInSeconds = 60;
        }
    }
}