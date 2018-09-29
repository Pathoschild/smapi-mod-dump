using StardewModdingAPI;
using System;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using System.Collections.Generic;
using System.IO;

namespace AutoLoadGame
{
    class ModEntry : Mod
    {
        
        private int wait = 5;

        private ModConfig Config;
        public override void Entry(IModHelper helper)
        {

            this.Config = Helper.ReadConfig<ModConfig>();
            if (this.Config.LastFileLoaded != null) GameEvents.SecondUpdateTick += waitUntilReady;
            SaveEvents.AfterLoad += setLastSave;
            SaveEvents.AfterSave += setLastSave;
            if (this.Config.ForgetLastFileOnTitle) SaveEvents.AfterReturnToTitle += clearLastSave;
        }

        public void setLastSave(object s, EventArgs e)
        {
            this.Config.LastFileLoaded = Constants.SaveFolderName;
            Helper.WriteConfig<ModConfig>(this.Config);
        }

        public void clearLastSave(object s, EventArgs e)
        {
            this.Config.LastFileLoaded = null;
            Helper.WriteConfig<ModConfig>(this.Config);
        }

        public void waitUntilReady(object s, EventArgs e) {
            if (Game1.activeClickableMenu is TitleMenu t)
            {
                // Need to delay so options can load correctly... 
                if (wait <= 0)
                {
                    wait = 5;
                    GameEvents.SecondUpdateTick -= waitUntilReady;
                    GameEvents.SecondUpdateTick += doLoad;
                } else
                {
                    wait -= 1;
                }
                
            } else
            {
                wait = 5;
            }
        }

        public void doLoad(object s, EventArgs e)
        {
            GameEvents.SecondUpdateTick -= doLoad;

            String file = this.Config.LastFileLoaded;
            clearLastSave(s, e); // Don't try loading again if it crashes...

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
