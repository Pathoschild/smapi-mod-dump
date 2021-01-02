/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MartyrPher/GetGlam
**
*************************************************/

using GetGlam.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using Harmony;
using Microsoft.Xna.Framework.Graphics;

namespace GetGlam
{
    public class ModEntry : Mod
    {
        /// TODO for bugfixes
        /// Skin Tone with Favorites - fixed
        /// Option to not include bases
        /// Favorites not saving on reload

        // The mods config
        private ModConfig Config;

        // Instance of ContentPackHelper
        private ContentPackHelper PackHelper;

        // Instance of DresserHandler
        private DresserHandler Dresser;

        // Instance of GlamMenu
        private GlamMenu Menu;

        // Instance of PlayerLoader
        private CharacterLoader PlayerLoader;

        // Instance of SaveLoadMenuPatcher
        private SaveLoadMenuPatcher MenuPatcher;

        // Instance of Harmony Helper
        public HarmonyHelper HarmonyHelper;

        // Whether SpaceCore is installed
        public bool IsSpaceCoreInstalled = false;

        // Whether Customize Anywhere is installed
        public bool IsCustomizeAnywhereInstalled = false;

        /// <summary>
        /// The mods entry point.
        /// </summary>>
        /// <param name="helper">SMAPI's mod helper</param>
        public override void Entry(IModHelper helper)
        {
            // Load the config
            Config = helper.ReadConfig<ModConfig>();

            // Initialize classes
            InitializeClasses();

            // Set up the events
            SetUpEvents();

            // Check for installed optional mods
            CheckIfOptionalModsAreInstalled();

            // If SpaceCore installed then register the extended tilesheets
            if (IsSpaceCoreInstalled)
                RegisterSpaceCoreSheets();

            // Initialize the LoadSaveMenu Patcher
            MenuPatcher.Init();

            // Initialize and Patch with Harmony
            HarmonyHelper.InitializeAndPatch();

            // Add the ContentLoader class to the AssetLoader List
            helper.Content.AssetLoaders.Add(new ContentLoader(this, PackHelper));
        }

        /// <summary>
        /// Initializes the needed classes for the mod.
        /// </summary>
        private void InitializeClasses()
        {
            PackHelper = new ContentPackHelper(this);
            Dresser = new DresserHandler(this, Config);
            PlayerLoader = new CharacterLoader(this, PackHelper, Dresser);
            MenuPatcher = new SaveLoadMenuPatcher(this, PlayerLoader);
            HarmonyHelper = new HarmonyHelper(this);
        }

        /// <summary>
        /// Sets up the events for the mod.
        /// </summary>
        private void SetUpEvents()
        {
            Helper.Events.Input.ButtonPressed += OnButtonPressed;
            Helper.Events.GameLoop.GameLaunched += OnGameLaunched;
            Helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
            Helper.Events.GameLoop.ReturnedToTitle += OnReturnedToTitle;
        }

        /// <summary>
        /// Checks if any of the optional mods are installed.
        /// </summary>
        private void CheckIfOptionalModsAreInstalled()
        {
            CheckIfSpaceCoreIsInstalled();

            CheckIfCustomizeAnywhereIsInstalled();
        }

        /// <summary>
        /// Check if the mod Space Core is currently installed.
        /// </summary>
        private void CheckIfSpaceCoreIsInstalled()
        {
            IsSpaceCoreInstalled = Helper.ModRegistry.IsLoaded("spacechase0.SpaceCore");
            Monitor.Log($"Space Core Installed: {IsSpaceCoreInstalled}", LogLevel.Trace);
        }

        /// <summary>
        /// Checks if the mod Customize Anywhere is currently installed.
        /// </summary>
        private void CheckIfCustomizeAnywhereIsInstalled()
        {
            IsCustomizeAnywhereInstalled = Helper.ModRegistry.IsLoaded("Cherry.CustomizeAnywhere");
            Monitor.Log($"Customize AnyWhere Installed: {IsCustomizeAnywhereInstalled}", LogLevel.Trace);
        }

        /// <summary>
        /// Registers SpaceCore for patching more hairs.
        /// </summary>
        private void RegisterSpaceCoreSheets()
        {
            Monitor.Log("Reflecting into SpaceCore", LogLevel.Trace);
            HarmonyHelper.SpaceCorePatchHairStyles();
        }

        /// <summary>
        /// Event that is called when a save is loaded.
        /// </summary> 
        /// <param name="sender"> The object</param>
        /// <param name="e"> The Save Loaded Event arguement</param>
        private void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            // Set up the Dresser
            SetUpDresser();

            //Create the Menu
            CreateAndAssignDresserMenu();

            //Load the character
            PlayerLoader.LoadCharacterLayout(Menu);
        }

        /// <summary>
        /// Sets up the dresser placement and texture.
        /// </summary>
        private void SetUpDresser()
        {
            Dresser.FarmHouse = Game1.getLocationFromName("FarmHouse");
            Dresser.SetDresserTexture();
            Dresser.PlaceDresser();
        }

        /// <summary>
        /// Creates the Glam Menu and Assigns it to the dresser.
        /// </summary>
        private void CreateAndAssignDresserMenu()
        {
            Menu = new GlamMenu(this, Config, PackHelper, Dresser, PlayerLoader);
            Dresser.Menu = Menu;
        }

        /// <summary>
        /// Event that is called when the game launches.
        /// </summary>
        /// <param name="sender"> The object</param>
        /// <param name="e"> The Game Launched Event argument</param>
        /// <remarks> This is used to load the content packs and dresser.png</remarks>
        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            // Add an AssetEditor to ignore hair for new hats
            if (Config.NewHatsIgnoreHair)
                Helper.Content.AssetEditors.Add(new HatEditor(this));

            // Load the dresser image and read the content packs
            Game1.content.Load<Texture2D>($"Mods/{this.ModManifest.UniqueID}/dresser.png");
            PackHelper.ReadContentPacks();
        }

        /// <summary>
        /// Event that is called when the game returns to the title.
        /// </summary>
        /// <param name="sender"> The object</param>
        /// <param name="e"> The Returned To Title Event argument</param>
        private void OnReturnedToTitle(object sender, ReturnedToTitleEventArgs e)
        {
            // Set the menu to null since it's a per save type of thing
            Menu = null;

            // Clear the favorites list as it's per save
            PlayerLoader.Favorites.Clear();
        }

        /// <summary>
        /// Event that is called when a button is pressed.
        /// </summary>
        /// <param name="sender"> The object</param>
        /// <param name="e"> The Button Pressed Event argument</param>
        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            // Check if the button is the Menu key and there is no menu
            if (e.Button.Equals(Config.OpenGlamMenuKey) && Game1.activeClickableMenu is null)
            {
                // Take a Snapshot
                Menu.TakeSnapshot();

                // Change player direction and open the menu
                ChangePlayerDirection();
            }

            // Check if the user clicked on the dresser
            Dresser.DresserInteractCheck(e.Button);
        }

        /// <summary>
        /// Chnages the player direction to face forward and opens the menu.
        /// </summary>
        private void ChangePlayerDirection()
        {
            Game1.player.faceDirection(2);
            Game1.player.FarmerSprite.StopAnimation();
            Game1.player.completelyStopAnimatingOrDoingAction();
            Game1.activeClickableMenu = Menu;
        }
    }
}
