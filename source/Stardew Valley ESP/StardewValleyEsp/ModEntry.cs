using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValleyEsp.Config;
using StardewValleyEsp.Detectors;
using StardewValleyEsp.Labels;
using StardewValleyEsp.Menu;

namespace StardewValleyEsp
{
    /// <summary>The mod entry class.</summary>
    public class ModEntry : Mod
    {
        private static Detector detector;
        private static Settings settings;
        private static LabelDrawingManager drawingManager;
        private static ConfigMenu configMenu;

        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            helper.Events.GameLoop.GameLaunched += OnGameLaunched;
            helper.Events.Display.RenderingHud += OnRenderingHud;
            helper.Events.Player.Warped += OnWarped;
            helper.Events.Input.ButtonPressed += OnButtonPressed;
        }

        /// <summary>Raised after the game is launched, right before the first update tick. This happens once per game session (unrelated to loading saves). All mods are loaded and initialised at this point, so this is a good time to set up mod integrations.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            settings = new Settings(Helper);
            configMenu = new ConfigMenu(settings);
            drawingManager = new LabelDrawingManager(settings);
            detector = new Detector(settings);
            detector.AddDetector("NPC")
                .AddDetector("Object")
                .AddDetector("FarmAnimal")
                .AddDetector("WaterEntity");
        }


        /// <summary>Raised before drawing the HUD (item toolbar, clock, etc) to the screen. The vanilla HUD may be hidden at this point (e.g. because a menu is open).</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnRenderingHud(object sender, RenderingHudEventArgs e)
        {
            detector.Detect();
            drawingManager.LabelEntities(detector);
        }

        /// <summary>Raised after the player presses a button on the keyboard, controller, or mouse.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (e.Button == settings.LoadKey)
            {
                settings.LoadSettings();
                drawingManager.SendHudMessage("Loaded settings from file", 4);
            }
            else if (e.Button == settings.MenuKey)
                Game1.activeClickableMenu = configMenu;
        }

        /// <summary>Raised after a player warps to a new location.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnWarped(object sender, WarpedEventArgs e)
        {
            if (e.IsLocalPlayer)
                detector.SetLocation(e.NewLocation);
        }
    }
}
