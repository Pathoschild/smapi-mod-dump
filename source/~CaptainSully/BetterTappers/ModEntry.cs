/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/CaptainSully/StardewMods
**
*************************************************/


namespace BetterTappers
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
        internal static ModConfig Config { get; set; }
        /// <summary>The mods unique id.</summary>
        internal static string UID { get; set; }


        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides methods for interacting with the mod directory, such as read/writing a config file or custom JSON files.</param>
        public override void Entry(IModHelper helper)
        {
            // initialize fields
            Instance = this;
            log = new(this);
            UID = ModManifest.UniqueID;

            // load config
            Config = Helper.ReadConfig<ModConfig>();
            ModConfig.VerifyConfigValues(Config, Instance);

            // hook events
            Helper.Events.GameLoop.GameLaunched += OnGameLaunched;
            Helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
            Helper.Events.GameLoop.DayStarted += delegate { CoreLogic.IncreaseTreeAges(); };
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

            // Load APIs
            LoadAPIs();
        }

        /// <summary>Load other mod APIs.</summary>
        private void LoadAPIs()
        {
            log.T("Loading mod-provided APIs.");

            // setup GMCM
            ModConfig.SetUpModConfigMenu(Config, this);

            // setup spacecore
            ISpaceCoreAPI spacecoreAPI = Helper.ModRegistry.GetApi<ISpaceCoreAPI>("spacechase0.SpaceCore");
            if (spacecoreAPI is null)
            {
                // Skip patcher mod behaviours if we fail to load the objects
                log.E($"Couldn't access mod-provided API for SpaceCore. " + UID + " will not be available, and no changes will be made.");
            }
            spacecoreAPI.RegisterSerializerType(typeof(Tapper));
        }

        private void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            if (!Context.IsMainPlayer) return;

            ModData data = CoreLogic.GetData() ?? new();

            if (!data.VanillaTappersConverted)
            {
                int s = 0;
                int f = 0;
                foreach (GameLocation location in Game1.locations)
                {
                    foreach (SObject o in location.Objects.Values.ToList())//foreach (KeyValuePair<Vector2, SObject> entry in location.Objects)
                    {
                        if (o is Tapper)
                        {
                            if (CoreLogic.ConvertToNormalTappers(o, location) is not Tapper) s++;
                            else f++;
                        }
                    }
                }
                log.W("Successful conversions: " + s + "    failures: " + f);

                if (f == 0) data.VanillaTappersConverted = true;
                CoreLogic.SaveData(data);
            }
        }
    }
}