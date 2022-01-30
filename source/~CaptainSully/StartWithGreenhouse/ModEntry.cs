/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/CaptainSully/StardewMods
**
*************************************************/


namespace StartWithGreenhouse
{
    // <summary>The mod entry point.</summary>
    internal class ModEntry : Mod
    {
        /*********
        ** Fields
        *********/
        /// <summary>Logging tool.</summary>
        private Log log;

        /// <summary>The mod configuration.</summary>
        private ModConfig Config { get; set; }


        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides methods for interacting with the mod directory, such as read/writing a config file or custom JSON files.</param>
        public override void Entry(IModHelper helper)
        {
            // initalize fields
            log = new(this);

            // load config
            UpdateConfig();

            // hook events
            Helper.Events.GameLoop.SaveLoaded += SaveLoaded;
            Helper.Events.GameLoop.GameLaunched += OnGameLaunched;
        }


        /*********
        ** Private methods
        *********/
        /****
        ** Event handlers
        ****/
        /// <inheritdoc cref="IGameLoopEvents.GameLaunched"/>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            ModConfig.SetUpModConfigMenu(Config, this);
        }

        /// <inheritdoc cref="IGameLoopEvents.SaveLoaded"/>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void SaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            if (!Context.IsWorldReady || !Context.IsMainPlayer)
                return;
            if (!Config.DisableAllModEffects && !Game1.player.mailReceived.Contains("ccPantry"))
                SetPantryFlag();
        }

        /// <summary>Give player the pantry mail flag, which unlocks the greenhouse.</summary>
        private void SetPantryFlag()
        {
            Game1.player.mailReceived.Add("ccPantry");
            log.T("Pantry flag set (Greenhouse unlocked).");
        }

        /// <summary>Update the mod configuration.</summary>
        private void UpdateConfig()
        {
            Config = Helper.ReadConfig<ModConfig>();
        }
    }
}