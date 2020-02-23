using GetGlam.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using Harmony;
using GetGlam.Framework.Patches;
using Microsoft.Xna.Framework.Graphics;
using System.Reflection;
using Microsoft.Xna.Framework;
using StardewValley.Menus;
using System;

namespace GetGlam
{
    public class ModEntry : Mod
    {
        //The mods config
        private ModConfig Config;

        //Instance of ContentPackHelper
        private ContentPackHelper PackHelper;

        //Instance of DresserHandler
        private DresserHandler Dresser;

        //Harmony instance, used to patch accessory length
        private HarmonyInstance Harmony;

        //Instance of GlamMenu
        private GlamMenu Menu;

        //Instance of PlayerLoader
        private CharacterLoader PlayerLoader;

        //Instance of SaveLoadMenuPatcher
        private SaveLoadMenuPatcher MenuPatcher;

        //Whether SpaceCore is installed
        public bool IsSpaceCoreInstalled = false;

        //Whether Customize Anywhere is installed
        public bool IsCustomizeAnywhereInstalled = false;

        /// <summary>The mods entry point.</summary>>
        /// <param name="helper">SMAPI's mod helper</param>
        public override void Entry(IModHelper helper)
        {
            //Load the config
            Config = helper.ReadConfig<ModConfig>();

            //Initialize classes
            PackHelper = new ContentPackHelper(this);
            Dresser = new DresserHandler(this, Config);
            PlayerLoader = new CharacterLoader(this, PackHelper, Dresser);
            MenuPatcher = new SaveLoadMenuPatcher(this, PlayerLoader);
            
            //Set up the events
            helper.Events.Input.ButtonPressed += OnButtonPressed;
            helper.Events.GameLoop.GameLaunched += OnGameLaunched;
            helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
            helper.Events.GameLoop.ReturnedToTitle += OnReturnedToTitle;

            //Check if SpaceCore is Installed
            IsSpaceCoreInstalled = Helper.ModRegistry.IsLoaded("spacechase0.SpaceCore");
            Monitor.Log($"Space Core Installed: {IsSpaceCoreInstalled}", LogLevel.Trace);

            //Check if Customize Anywhere is installed
            IsCustomizeAnywhereInstalled = Helper.ModRegistry.IsLoaded("Cherry.CustomizeAnywhere");
            Monitor.Log($"Customize AnyWhere Installed: {IsCustomizeAnywhereInstalled}", LogLevel.Trace);

            //if it's installed then register the extended tilesheets
            if (IsSpaceCoreInstalled)
            {
                Monitor.Log("Reflecting into SpaceCore", LogLevel.Trace);
                SpaceCoreHackery();
            }

            //Initialized the LoadSaveMenu Patcher
            MenuPatcher.Init();

            //Conduct the Harmony Patch
            Harmony = HarmonyInstance.Create(this.ModManifest.UniqueID);
            CommenceHarmonyPatch();

            //Add the ContentLoader class to the AssetLoader List
            helper.Content.AssetLoaders.Add(new ContentLoader(this, PackHelper));
        }

        private void SpaceCoreHackery()
        {
            var modData = Helper.ModRegistry.Get("spacechase0.SpaceCore");
            var spaceCoreInstance = modData.GetType().GetProperty("Mod", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public).GetValue(modData);
            var spaceCoreAssembly = spaceCoreInstance.GetType().Assembly;
            var registerTileSheet = spaceCoreAssembly.GetType("SpaceCore.TileSheetExtensions").GetMethod("RegisterExtendedTileSheet");
            registerTileSheet.Invoke(null, new object[] { "Characters\\Farmer\\hairstyles", 96 });
        }

        public void SpaceCorePatchExtendedTileSheet(IAssetDataForImage asset, Texture2D sourceTexture, Rectangle sourceRect, Rectangle targetRect)
        {
            var modData = Helper.ModRegistry.Get("spacechase0.SpaceCore");
            var spaceCoreInstance = modData.GetType().GetProperty("Mod", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public).GetValue(modData);
            var spaceCoreAssembly = spaceCoreInstance.GetType().Assembly;
            var patchExtendedTileSheet = spaceCoreAssembly.GetType("SpaceCore.TileSheetExtensions").GetMethod("PatchExtendedTileSheet");
            patchExtendedTileSheet.Invoke(null, new object[] { asset, sourceTexture, sourceRect, targetRect, PatchMode.Replace});
        }

        public void CustomizeAnywhereClothingMenu()
        {
            var modData = Helper.ModRegistry.Get("Cherry.CustomizeAnywhere");
            var customizeInstance = modData.GetType().GetProperty("Mod", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public).GetValue(modData);
            var customizeAssembly = customizeInstance.GetType().Assembly;
            var dresserMenu = customizeAssembly.GetType("CustomizeAnywhere.DresserMenu");
            Game1.activeClickableMenu = (IClickableMenu)Activator.CreateInstance(dresserMenu);
        }

        /// <summary>Event that is called when a save is loaded.</summary> 
        /// <param name="sender">The object</param>
        /// <param name="e">The Save Loaded Event arguement</param>
        private void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            //Set up the Dresser and Place it
            Dresser.FarmHouse = Game1.getLocationFromName("FarmHouse");
            Dresser.SetDresserTexture();
            Dresser.PlaceDresser();

            //Create the Menus and set the var in DresserHandler
            Menu = new GlamMenu(this, Config, PackHelper, Dresser, PlayerLoader);
            Dresser.Menu = Menu;

            //Load the character
            PlayerLoader.LoadCharacterLayout(Menu);
        }

        /// <summary>Event that is called when the game launches.</summary>
        /// <param name="sender">The object</param>
        /// <param name="e">The Game Launched Event argument</param>
        /// <remarks>This is used to load the content packs and dresser.png</remarks>
        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            //Add an AssetEditor to ignore hair for new hats
            if (Config.NewHatsIgnoreHair)
                Helper.Content.AssetEditors.Add(new HatEditor(this));

            //Load the dresser image and read the content packs
            Game1.content.Load<Texture2D>($"Mods/{this.ModManifest.UniqueID}/dresser.png");
            PackHelper.ReadContentPacks();
        }

        /// <summary>Event that is called when the game returns to the title.</summary>
        /// <param name="sender">The object</param>
        /// <param name="e">The Returned To Titles Event argument</param>
        private void OnReturnedToTitle(object sender, ReturnedToTitleEventArgs e)
        {
            //Set the menu to null since it's a per save type of thing
            Menu = null;

            //Clear the favorites list as it's per save
            PlayerLoader.Favorites.Clear();
        }

        /// <summary>Event that is called when a button is pressed.</summary>
        /// <param name="sender">The object</param>
        /// <param name="e">The Button Pressed Event argument</param>
        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            //Check if the button is the Menu key and their is no menu
            if (e.Button.Equals(Config.OpenGlamMenuKey) && Game1.activeClickableMenu is null)
            {
                //Take a Snapshot
                Menu.TakeSnapshot();

                //Change player direction and open the menu
                Game1.player.faceDirection(2);
                Game1.player.FarmerSprite.StopAnimation();
                Game1.player.completelyStopAnimatingOrDoingAction();
                Game1.activeClickableMenu = Menu;
            }

            //Check if the user clicked on the dresser
            Dresser.DresserInteractCheck(e.Button);
        }

        /// <summary>Harmony patch to patch the accessory length and skin color length</summary>
        private void CommenceHarmonyPatch()
        {
            //Create a new instance of the patches
            AccessoryPatch accessoryPatch = new AccessoryPatch(this);
            SkinColorPatch skinColorPatch = new SkinColorPatch(this);

            //Tell the log I'm patching it, then patch it.
            Monitor.Log("Patching changeAccessory()", LogLevel.Trace);
            Harmony.Patch(
                original: AccessTools.Method(typeof(Farmer), nameof(Farmer.changeAccessory)),
                transpiler: new HarmonyMethod(accessoryPatch.GetType(), nameof(AccessoryPatch.ChangeAccessoryTranspiler))
            );

            //Patch the skin color length
            Monitor.Log("Patching changeSkinColor()", LogLevel.Trace);
            Harmony.Patch(
                original: AccessTools.Method(typeof(Farmer), nameof(Farmer.changeSkinColor)),
                transpiler: new HarmonyMethod(skinColorPatch.GetType(), nameof(SkinColorPatch.ChangeSkinColorTranspiler))
            );
        }
    }
}
