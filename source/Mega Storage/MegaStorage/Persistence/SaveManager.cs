using StardewModdingAPI;

namespace MegaStorage.Persistence
{
    public class SaveManager
    {
        private readonly IModHelper _modHelper;
        private readonly IMonitor _monitor;
        private readonly ISaver[] _savers;
        private readonly FarmhandMonitor _farmhandMonitor;

        public SaveManager(IModHelper modHelper, IMonitor monitor, FarmhandMonitor farmhandMonitor, params ISaver[] savers)
        {
            _modHelper = modHelper;
            _monitor = monitor;
            _savers = savers;
            _farmhandMonitor = farmhandMonitor;
        }

        public void Start()
        {
            _modHelper.Events.GameLoop.SaveLoaded += (sender, args) => LoadCustomChests();
            _modHelper.Events.GameLoop.Saving += (sender, args) => HideAndSaveCustomChests();
            _modHelper.Events.GameLoop.Saved += (sender, args) => ReAddCustomChests();
            _modHelper.Events.GameLoop.ReturnedToTitle += (sender, args) => HideAndSaveCustomChests();
            _modHelper.Events.Multiplayer.PeerContextReceived += (sender, args) => HideAndSaveCustomChests();

            _farmhandMonitor.Start();
            _farmhandMonitor.OnPlayerAdded += ReAddCustomChests;
            _farmhandMonitor.OnPlayerRemoved += ReAddCustomChests;
        }

        private void LoadCustomChests()
        {
            _monitor.VerboseLog("LoadCustomChests");
            foreach (var saver in _savers)
            {
                saver.LoadCustomChests();
            }
        }

        private void ReAddCustomChests()
        {
            _monitor.VerboseLog("ReAddCustomChests");
            foreach (var saver in _savers)
            {
                saver.ReAddCustomChests();
            }
        }

        private void HideAndSaveCustomChests()
        {
            _monitor.VerboseLog("HideAndSaveCustomChests");
            foreach (var saver in _savers)
            {
                saver.HideAndSaveCustomChests();
            }
        }

    }
}
