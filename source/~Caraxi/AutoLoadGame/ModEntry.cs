using System;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using System.IO;

namespace AutoLoadGame
{
    class ModEntry : Mod
    {
        private int wait = 5;
        private ModConfig Config;

        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            this.Config = helper.ReadConfig<ModConfig>();
            if (this.Config.LastFileLoaded != null)
                helper.Events.GameLoop.UpdateTicked += waitUntilReady;

            helper.Events.GameLoop.SaveLoaded += onSaveLoaded;
            helper.Events.GameLoop.ReturnedToTitle += onReturnedToTitle;
        }

        /// <summary>Raised after the player loads a save slot and the world is initialised.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void onSaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            // set last save
            this.Config.LastFileLoaded = Constants.SaveFolderName;
            Helper.WriteConfig(this.Config);
        }

        /// <summary>Raised after the game returns to the title screen.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        public void onReturnedToTitle(object sender, ReturnedToTitleEventArgs e)
        {
            // clear last save
            if (this.Config.ForgetLastFileOnTitle)
                this.clearLastSave();
        }

        /// <summary>Raised after the game state is updated (≈60 times per second).</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        public void waitUntilReady(object sender, UpdateTickedEventArgs e)
        {
            if (Game1.activeClickableMenu is TitleMenu t)
            {
                // Need to delay so options can load correctly... 
                if (wait <= 0)
                {
                    wait = 5;
                    Helper.Events.GameLoop.UpdateTicked -= waitUntilReady;
                    doLoad();
                }
                else
                {
                    wait -= 1;
                }
            }
            else
            {
                wait = 5;
            }
        }

        private void clearLastSave()
        {
            this.Config.LastFileLoaded = null;
            Helper.WriteConfig(this.Config);
        }

        private void doLoad()
        {
            String file = this.Config.LastFileLoaded;
            clearLastSave(); // Don't try loading again if it crashes...

            String savePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "StardewValley", "Saves", file);

            if (!Directory.Exists(savePath))
            {
                Monitor.Log("Attempted to load a save that doesn't seem to exist.", LogLevel.Warn);
                return;
            }

            if (this.Config.LoadIntoMultiplayer)
            {
                Game1.multiplayerMode = 2;
            }
            try
            {
                this.Monitor.Log("Loading Save: " + file, LogLevel.Info);
                SaveGame.Load(file);
                if (Game1.activeClickableMenu is TitleMenu m) m.exitThisMenu(false);
            }
            catch (Exception ex)
            {
                this.Monitor.Log("Load Failed", LogLevel.Error);
                this.Monitor.Log(ex.Message);
            }
            
        }


    }
}
