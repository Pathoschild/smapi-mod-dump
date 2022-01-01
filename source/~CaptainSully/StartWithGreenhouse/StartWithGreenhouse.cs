/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/CaptainSully/StardewMods
**
*************************************************/

using StardewModdingAPI;
using StardewValley;
using StardewModdingAPI.Events;
using SullySDVcore;

namespace StartWithGreenhouse
{
    public class StartWithGreenhouse : Mod
    {
        internal static StartWithGreenhouse Instance { get; set; }
        internal Log log;
        internal static Config Config { get; set; }


        public override void Entry(IModHelper helper)
        {
            Instance = this;
            log = new(this);
            Config = helper.ReadConfig<Config>();
            Config.SetUpModConfigMenu(Config, this);
            Helper.Events.GameLoop.SaveLoaded += SaveLoaded;
            Helper.Events.GameLoop.GameLaunched += GameLoop_GameLaunched;
        }

        private void GameLoop_GameLaunched(object sender, GameLaunchedEventArgs e)
        {
            this.Helper.Events.GameLoop.OneSecondUpdateTicked += this.Event_LoadLate;
        }
        private void Event_LoadLate(object sender, OneSecondUpdateTickedEventArgs e)
        {
            this.Helper.Events.GameLoop.OneSecondUpdateTicked -= this.Event_LoadLate;
            Config.SetUpModConfigMenu(Config, this);
        }

        private void SetPantryFlag()
        {
                Game1.player.mailReceived.Add("ccPantry");
                log.T("Pantry flag set (Greenhouse unlocked).");
        }
        
        private void SaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            if (!Context.IsWorldReady)
                return;
            if (!Config.DisableAllModEffects && !Game1.player.mailReceived.Contains("ccPantry"))
                SetPantryFlag();
        }
    }
}