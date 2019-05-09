using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;

// To fix: 
// Readme
// Git

namespace ExpandedFridge
{
    /// The entry point of the mod handled by SMAPI.
    public class ModEntry : Mod
    {
        ExpandedFridgeHub expandedFridgeHub;
        public static StardewModdingAPI.SButton RemoteButton { get; private set; }
        public static Texture2D FridgeTexture { get; private set; }
        public static int cheatStorage { get; private set; }
        public static bool cheatUpgrades { get; private set; }
        public static IModHelper HelperInstance { get; private set; }
        public static IMonitor MonitorInstance { get; private set; }

        /// Called after mod is first loaded. We set up our event callbacks here.
        public override void Entry(IModHelper helper)
        {
            RemoteButton = SButton.R;
            FridgeTexture = helper.Content.Load<Texture2D>("Assets/fridge3.png", ContentSource.ModFolder);
            HelperInstance = Helper;
            MonitorInstance = Monitor;
            cheatStorage = ModEntry.HelperInstance.ReadConfig<ModConfig>().cheatStorage;
            cheatUpgrades = ModEntry.HelperInstance.ReadConfig<ModConfig>().cheatUpgrades;


            helper.Events.GameLoop.DayStarted += this.OnDayStarted;
            helper.Events.GameLoop.DayEnding += this.OnDayEnded;
        }



        /// *************************************************************************************************************************
        /// EVENT CALLBACK METHODS


        /// Callback function on day started. We hook in and modify the fridge here.
        private void OnDayStarted(object sender, DayStartedEventArgs e)
        {
            this.expandedFridgeHub = new ExpandedFridgeHub();
        }

        /// Callback function on day ended. We cleanup from the expanded fridge so when the game is saved all data is safe.
        private void OnDayEnded(object sender, DayEndingEventArgs e)
        {
            if (this.expandedFridgeHub != null)
            {
                this.expandedFridgeHub.CleanupForRelease();
                this.expandedFridgeHub = null;
            }
        }
    }
}
