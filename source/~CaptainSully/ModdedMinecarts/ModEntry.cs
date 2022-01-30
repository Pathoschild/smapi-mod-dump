/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/CaptainSully/StardewMods
**
*************************************************/


namespace ModdedMinecarts
{
    // <summary>The mod entry point.</summary>
    public class ModEntry : Mod 
    {
        /*********
        ** Fields
        *********/
        /// <summary>Static reference to the mod instance for logging in other classes.</summary>
        internal static ModEntry Instance { get; set; }

        /// <summary>Logging tool.</summary>
        internal Log log;

        /// <summary>The mod configuration.</summary>
        private ModConfig Config;

        /// <summary>The mods unique ID.</summary>
        internal static string UID { get; set; }


        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides methods for interacting with the mod directory, such as read/writing a config file or custom JSON files.</param>
        public override void Entry(IModHelper helper)
        {
            Instance = this;
            log = new(this);
            UID = ModManifest.UniqueID;

            //UpdateConfig();

            helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;
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
            log.T("Initialising mod data.");

            // Content
            //Translations.Initialise();

            // Patches
            Patcher.PatchAll();

            //ModConfig.SetUpModConfigMenu(Config, this);
        }

        /// <summary>Update the mod configuration.</summary>
        private void UpdateConfig()
        {
            Config = Helper.ReadConfig<ModConfig>();
            ModConfig.VerifyConfigValues(Config, this);
        }
    }
}