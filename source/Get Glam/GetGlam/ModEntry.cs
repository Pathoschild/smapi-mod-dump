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
using GetGlam.Framework.Menus;

namespace GetGlam
{
    public class ModEntry : Mod
    {
        // The mods config
        public ModConfig Config;

        // Instance of ContentPackHelper
        private ContentPackHelper PackHelper;

        // Instance of DresserHandler
        private DresserHandler Dresser;
        
        // Instance of DresserHandlerJson
        private DresserHandlerJson DresserJson;

        // Instance of GlamMenu
        private GlamMenu Menu;

        // Instance of PlayerLoader
        private CharacterLoader PlayerLoader;

        // Instance of SaveLoadMenuPatcher
        private SaveLoadMenuPatcher MenuPatcher;

        // Instance of PlayerChanger
        private PlayerChanger PlayerChanger;

        // Instance of Harmony Helper
        public HarmonyHelper HarmonyHelper;

        // Whether SpaceCore is installed
        public bool IsSpaceCoreInstalled = false;

        // Whether Customize Anywhere is installed
        public bool IsCustomizeAnywhereInstalled = false;

        // Whether Json Assets is installed
        public bool IsJsonAssetsInstalled = false;

        /// <summary>
        /// The mods entry point.
        /// </summary>>
        /// <param name="helper">SMAPI's mod helper</param>
        public override void Entry(IModHelper helper)
        {
            // Load the config
            Config = helper.ReadConfig<ModConfig>();

            InitializeClasses();
            SetUpEvents();
            CheckIfOptionalModsAreInstalled();

            // If SpaceCore installed then register the extended tilesheets
            if (IsSpaceCoreInstalled)
                RegisterSpaceCoreSheets();

            MenuPatcher.Initialize();
            HarmonyHelper.InitializeAndPatch();

            helper.Content.AssetLoaders.Add(new ContentLoader(this, PlayerChanger));
        }

        /// <summary>
        /// Initializes the needed classes for the mod.
        /// </summary>
        private void InitializeClasses()
        {
            PackHelper = new ContentPackHelper(this);
            PlayerChanger = new PlayerChanger(this, PackHelper);
            Dresser = new DresserHandler(this, Config, PackHelper);
            PlayerLoader = new CharacterLoader(this, PlayerChanger, Dresser);
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
            CheckIfJsonAssetsIsInstalled();
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
            Monitor.Log($"Customize Anywhere Installed: {IsCustomizeAnywhereInstalled}", LogLevel.Trace);
        }

        /// <summary>
        /// Checks if the mod Json Assets is currently installed.
        /// </summary>
        private void CheckIfJsonAssetsIsInstalled()
        {
            IsJsonAssetsInstalled= Helper.ModRegistry.IsLoaded("spacechase0.JsonAssets");
            Monitor.Log($"Json Assets Installed: {IsJsonAssetsInstalled}", LogLevel.Trace);
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
            SetUpDresser();
            CreateAndAssignDresserMenu();
            PlayerLoader.LoadCharacterLayout(Menu);
        }

        /// <summary>
        /// Sets up the dresser placement and texture.
        /// </summary>
        private void SetUpDresser()
        {
            Dresser.FarmHouse = Game1.getLocationFromName("FarmHouse");
            Dresser.PlaceDresser();
        }

        /// <summary>
        /// Creates the Glam Menu and Assigns it to the dresser.
        /// </summary>
        private void CreateAndAssignDresserMenu()
        {
            Menu = new GlamMenu(this, Config, PackHelper, Dresser, PlayerLoader, PlayerChanger);
            Dresser.Menu = Menu;

            if (IsJsonAssetsInstalled)
            {
                DresserJson.SetMenu(Menu);
                DresserJson.GetCraftableId();
            }
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

            if (IsJsonAssetsInstalled)
                DresserJson = new DresserHandlerJson(this);

            // Read The Content Packs
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
                Menu.TakeSnapshot();
                ChangePlayerDirection();
                OpenGlamMenu();
            }

            Dresser.DresserInteractCheck(e.Button);

            if (IsJsonAssetsInstalled)
                DresserJson.CheckJsonInput(sender, e);
        }

        /// <summary>
        /// Chnages the player direction to face forward and opens the menu.
        /// </summary>
        public void ChangePlayerDirection()
        {
            Game1.player.faceDirection(2);
            Game1.player.FarmerSprite.StopAnimation();
            Game1.player.completelyStopAnimatingOrDoingAction();
        }

        /// <summary>
        /// Opens the Glam Menu.
        /// </summary>
        public void OpenGlamMenu()
        {
            Game1.activeClickableMenu = Menu;
        }
    }
}
