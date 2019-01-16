using EventSystem.Framework.Events;
using EventSystem.Framework.FunctionEvents;
using EventSystem.Framework.Information;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
namespace SundropMapEvents
{
    public class Class1 : Mod
    {
        public static IModHelper ModHelper;
        public static IMonitor ModMonitor;

        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            ModHelper = this.Helper;
            ModMonitor = this.Monitor;
            helper.Events.GameLoop.SaveLoaded += this.OnSaveLoaded;
        }

        /// <summary>Raised after the player loads a save slot and the world is initialised.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            EventSystem.EventSystem.eventManager.addEvent(Game1.getLocationFromName("BusStop"), new WarpEvent("toRR", Game1.getLocationFromName("BusStop"), new Vector2(6, 11), new PlayerEvents(null, null), new WarpInformation("BusStop", 10, 12, 2, false)));
            EventSystem.EventSystem.eventManager.addEvent(Game1.getLocationFromName("BusStop"), new DialogueDisplayEvent("Hello.", Game1.getLocationFromName("BusStop"), new Vector2(10, 13), new MouseButtonEvents(null, null), new MouseEntryLeaveEvent(null, null), "Hello there!"));
        }
    }
}
