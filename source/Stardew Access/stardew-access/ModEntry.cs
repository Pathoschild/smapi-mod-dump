/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/stardew-access/stardew-access
**
*************************************************/

using stardew_access.Features;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using HarmonyLib;
using stardew_access.Patches;
using stardew_access.ScreenReader;
using Microsoft.Xna.Framework;

namespace stardew_access
{
    public class MainClass : Mod
    {
        private Harmony? harmony;
        public static bool readTile = true;
        public static bool snapMouse = true;
        public static bool isNarratingHudMessage = false;
        public static bool radar = false;
        public static bool radarDebug = false;
        public static bool radarStereoSound = true;
        private static IMonitor monitor;
        public static string hudMessageQueryKey = "";
        private static Radar radarFeature;
        private static IScreenReader? screenReader;
        private static IModHelper modHelper;

        public static IModHelper ModHelper { get => modHelper; }
        public static Radar RadarFeature { get => radarFeature; set => radarFeature = value; }

        public static IScreenReader GetScreenReader()
        {
            if (screenReader == null)
                screenReader = new ScreenReaderController().Initialize();
            return screenReader;
        }

        public static void SetScreenReader(IScreenReader value)
        {
            screenReader = value;
        }

        public static IMonitor GetMonitor()
        {
            return monitor;
        }

        public static void SetMonitor(IMonitor value)
        {
            monitor = value;
        }

        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            #region Initializations

            SetMonitor(base.Monitor); // Inititalize monitor
            modHelper = helper;

            Game1.options.setGamepadMode("force_on");

            SetScreenReader(new ScreenReaderController().Initialize());

            CustomSoundEffects.Initialize();

            CustomCommands.Initialize();

            RadarFeature = new Radar();

            harmony = new Harmony(ModManifest.UniqueID);
            HarmonyPatches.Initialize(harmony);

            //Initialize marked locations
            for (int i = 0; i < BuildingNAnimalMenuPatches.marked.Length; i++)
            {
                BuildingNAnimalMenuPatches.marked[i] = Vector2.Zero;
            }

            for (int i = 0; i < BuildingNAnimalMenuPatches.availableBuildings.Length; i++)
            {
                BuildingNAnimalMenuPatches.availableBuildings[i] = null;
            }
            #endregion

            helper.Events.Input.ButtonPressed += this.OnButtonPressed;
            helper.Events.GameLoop.UpdateTicked += this.onUpdateTicked;
            AppDomain.CurrentDomain.DomainUnload += OnExit;
            AppDomain.CurrentDomain.ProcessExit += OnExit;
        }

        public void OnExit(object? sender, EventArgs? e)
        {
            // Don't if this ever gets called or not but, just in case if it does.
            if (GetScreenReader() != null)
                GetScreenReader().CloseScreenReader();
        }

        /// <summary>Returns the Screen Reader class for other mods to use.</summary>
        public override object GetApi()
        {
            return new API();
        }

        private void onUpdateTicked(object? sender, UpdateTickedEventArgs? e)
        {
            if (!Context.IsPlayerFree)
                return;

            // Reset variables
            MenuPatches.resetGlobalVars();
            QuestPatches.resetGlobalVars();

            Other.narrateCurrentSlot();

            Other.narrateCurrentLocation();

            if (snapMouse)
                Other.SnapMouseToPlayer();

            if (!ReadTile.isReadingTile && readTile)
                ReadTile.run();

            if (!RadarFeature.isRunning && radar)
                RadarFeature.Run();

            if (!isNarratingHudMessage)
            {
                Other.narrateHudMessages();
            }
        }

        private void OnButtonPressed(object? sender, ButtonPressedEventArgs? e)
        {
            if (e == null)
                return;

            if (Game1.activeClickableMenu != null)
            {
                bool isLeftShiftPressed = Game1.input.GetKeyboardState().IsKeyDown(Microsoft.Xna.Framework.Input.Keys.LeftShift);
                bool isLeftControlPressed = Game1.input.GetKeyboardState().IsKeyDown(Microsoft.Xna.Framework.Input.Keys.LeftControl);

                // Perform Left Click
                if (Equals(e.Button, SButton.OemOpenBrackets))
                {
                    Game1.activeClickableMenu.receiveLeftClick(Game1.getMouseX(true), Game1.getMouseY(true));
                }
                if (isLeftControlPressed && Equals(e.Button, SButton.Enter))
                {
                    Game1.activeClickableMenu.receiveLeftClick(Game1.getMouseX(true), Game1.getMouseY(true));
                }

                // Perform Right CLick
                if (Equals(e.Button, SButton.OemCloseBrackets))
                {
                    Game1.activeClickableMenu.receiveRightClick(Game1.getMouseX(true), Game1.getMouseY(true));
                }
                if (isLeftShiftPressed && Equals(e.Button, SButton.Enter))
                {
                    Game1.activeClickableMenu.receiveRightClick(Game1.getMouseX(true), Game1.getMouseY(true));
                }
            }

            if (!Context.IsPlayerFree)
                return;

            // Narrate health and stamina
            if (Equals(e.Button, SButton.H))
            {
                string toSpeak = $"Health is {CurrentPlayer.getHealth()} and Stamina is {CurrentPlayer.getStamina()}";
                MainClass.GetScreenReader().Say(toSpeak, true);
            }

            // Narrate Position
            if (Equals(e.Button, SButton.K))
            {
                string toSpeak = $"X: {CurrentPlayer.getPositionX()} , Y: {CurrentPlayer.getPositionY()}";
                MainClass.GetScreenReader().Say(toSpeak, true);
            }

            // Narrate money at hand
            if (Equals(e.Button, SButton.R))
            {
                string toSpeak = $"You have {CurrentPlayer.getMoney()}g";
                MainClass.GetScreenReader().Say(toSpeak, true);
            }

            // Narrate time and season
            if (Equals(e.Button, SButton.Q))
            {
                string toSpeak = $"Time is {CurrentPlayer.getTimeOfDay()} and it is {CurrentPlayer.getDay()} {CurrentPlayer.getDate()} of {CurrentPlayer.getSeason()}";
                MainClass.GetScreenReader().Say(toSpeak, true);
            }

            // Manual read tile
            if (Equals(e.Button, SButton.J))
            {
                ReadTile.run(manuallyTriggered: true);
            }

            /*if (Equals(e.Button, SButton.B))
            {
                Game1.player.controller = new PathFindController(Game1.player, Game1.currentLocation, new Point(49,13), 2);
                monitor.Log($"{Game1.player.controller.pathToEndPoint==null}", LogLevel.Debug); // true if path not found
            }*/
        }
    }
}