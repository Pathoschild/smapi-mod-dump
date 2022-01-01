/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/CaptainSully/StardewMods
**
*************************************************/

using System;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using SullySDVcore;

namespace ModName   // **** Rename namespace, class name, and class extends Mod. Then good to go ****
{ /*
    public class ModName : Mod 
    {
        internal static ModName Instance { get; set; }
        internal Log log;
        internal static Config Config { get; set; }
        internal static string UID { get; set; }
        

        public override void Entry(IModHelper helper)
        {
            Instance = this;
            log = new(this);
            UID = ModManifest.UniqueID;
            Config = helper.ReadConfig<Config>();
            Config.VerifyConfigValues(Config, this);

            helper.Events.GameLoop.GameLaunched += this.GameLoop_GameLaunched;
        }
        private void GameLoop_GameLaunched(object sender, GameLaunchedEventArgs e)
        {
            this.Helper.Events.GameLoop.OneSecondUpdateTicked += this.Event_LoadLate;
        }
        private void Event_LoadLate(object sender, OneSecondUpdateTickedEventArgs e)
        {
            this.Helper.Events.GameLoop.OneSecondUpdateTicked -= this.Event_LoadLate;

            if (this.LoadAPIs())
            {
                this.Initialise();
            }
        }
        private void Initialise()
        {
            log.T("Initialising mod data.");

            // Content
            //Translations.Initialise();
            //Config.SetUpModConfigMenu(Config, this);

            // Patches
            //Patcher.PatchAll();

            // Events
            
        }
        private bool LoadAPIs()
        {
            
            return true;
        }
    }
    */
}