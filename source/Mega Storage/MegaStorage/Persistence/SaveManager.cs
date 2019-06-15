using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace MegaStorage.Persistence
{
    public class SaveManager
    {
        private readonly IModHelper _modHelper;
        private readonly IMonitor _monitor;
        private readonly ISaver[] _savers;

        public SaveManager(IModHelper modHelper, IMonitor monitor, ISaver[] savers)
        {
            _modHelper = modHelper;
            _monitor = monitor;
            _savers = savers;
        }

        public void Start()
        {
            _modHelper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
            _modHelper.Events.GameLoop.DayStarted += OnDayStarted;
            _modHelper.Events.GameLoop.DayEnding += OnDayEnding;
        }

        private void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            _monitor.VerboseLog("OnSaveLoaded");
            foreach (var saver in _savers)
            {
                saver.LoadNiceChests();
            }
        }

        private void OnDayStarted(object sender, DayStartedEventArgs e)
        {
            _monitor.VerboseLog("OnDayStarted");
            foreach (var saver in _savers)
            {
                saver.ReAddNiceChests();
            }
        }

        private void OnDayEnding(object sender, DayEndingEventArgs e)
        {
            _monitor.VerboseLog("OnDayEnding");
            foreach (var saver in _savers)
            {
                saver.HideAndSaveNiceChests();
            }
        }

    }
}
