using EventSystem.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace EventSystem
{
    // TODO: Make Bed/Sleep Event. 
    public class EventSystem : Mod
    {
        public static IModHelper ModHelper;
        public static IMonitor ModMonitor;

        public static EventManager eventManager;

        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            ModHelper = this.Helper;
            ModMonitor = this.Monitor;
            helper.Events.GameLoop.UpdateTicked += this.OnUpdateTicked;
            helper.Events.GameLoop.SaveLoaded += this.OnSaveLoaded;
        }

        /// <summary>Raised after the player loads a save slot and the world is initialised.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            eventManager = new EventManager();
        }

        /// <summary>Raised after the game state is updated (≈60 times per second).</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            eventManager?.update();
        }
    }
}
