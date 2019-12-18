using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.Text;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using System.IO;

namespace DailyFarmPhoto
{
    class ModEntry : Mod
    {
        private bool screenshotTaken = false;
        private bool takeShot = false;
        private string fileDumpPath = "";
        private FileInfo mapFolder;

        private ModConfig config;

        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            fileDumpPath = Path.Combine(Path.Combine(Path.Combine(
                    Environment.GetFolderPath((Environment.SpecialFolder)(Environment.OSVersion.Platform != PlatformID.Unix ? 26 : 28)), //appdata
                    "StardewValley"),
                    "Screenshots"));

            this.config = helper.ReadConfig<ModConfig>();

            helper.Events.GameLoop.SaveLoaded += this.OnSaveLoaded;
            helper.Events.Player.Warped += this.OnWarped;
            helper.Events.GameLoop.DayEnding += this.OnDayEnding;
            helper.Events.GameLoop.DayStarted += this.OnDayStarted;
            helper.Events.GameLoop.OneSecondUpdateTicked += OnOneSecondUpdateTicked;

            CreateFileSystemWatcher();
        }

        private void OnWarped(object sender, WarpedEventArgs e)
        {
            if (!screenshotTaken && e.Player.IsMainPlayer && e.NewLocation.isOutdoors.Value && e.NewLocation.isFarm.Value)
            {
                takeShot = true;
            }
            else
            {
                takeShot = false;
            }
        }

        private void OnOneSecondUpdateTicked(object sender, OneSecondUpdateTickedEventArgs e)
        {
            if (!screenshotTaken && takeShot && Game1.timeOfDay >= config.TakeScreenshotAfter) {
                takeShot = false;
                
                var filename = GetFarmName() + "_Year" + Game1.Date.Year + "_Season" + Game1.Date.SeasonIndex + "_Day" + Game1.Date.DayOfMonth + "_Time" + Game1.timeOfDay;
                
                Game1.game1.takeMapScreenshot(config.Resolution / 100f, filename);
                screenshotTaken = true;
            }
        }

        private void OnDayEnding(object sender, DayEndingEventArgs e)
        {
            screenshotTaken = false;
            takeShot = false;
        }

        private void OnDayStarted(object sender, DayStartedEventArgs e)
        {
            screenshotTaken = false;
            takeShot = false;
        }

        private void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            mapFolder = new FileInfo(Path.Combine(Path.Combine(fileDumpPath, GetFarmName()), "asdfasdf"));
            if (!Directory.Exists(mapFolder.FullName))
            {
                mapFolder.Directory.Create();
            }
        }

        // <summary>Creates a new FileSystemWatcher and set its properties.</summary>
        private void CreateFileSystemWatcher()
        {
            FileSystemWatcher watcher = new FileSystemWatcher
            {
                Path = fileDumpPath,
                NotifyFilter = NotifyFilters.LastWrite,
                Filter = $"*.png"
            };

            watcher.Changed += OnFileChanged;

            // Begin watching.
            watcher.EnableRaisingEvents = true;
        }

        private void OnFileChanged(object sender, FileSystemEventArgs e)
        {

            if (File.Exists(e.FullPath))
            {
                try
                {
                    FileInfo dumpedScreenshot = new FileInfo(e.FullPath);
                    FileInfo destinationScreenshot = new FileInfo(Path.Combine(mapFolder.DirectoryName, e.Name));

                    if (!destinationScreenshot.Exists && mapFolder.Directory.Exists)
                    {
                        dumpedScreenshot.MoveTo(destinationScreenshot.FullName);
                    }
                }
                catch (IOException) { }
            }
        }

        private string GetFarmName()
        {
            var farmName = Game1.player.farmName.Value;

            //if farm name is just spaces, we can't create a folder named that, so give it a different name
            if (farmName.Trim().Length == 0)
                return farmName.Replace(" ", "_");

            return farmName;
        }
    }
}
