using System.Threading.Tasks;
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
            _modHelper.Events.GameLoop.SaveLoaded += (sender, args) => LoadCustomChests();
            _modHelper.Events.GameLoop.Saving += (sender, args) => HideAndSaveCustomChests();
            _modHelper.Events.GameLoop.Saved += (sender, args) => ReAddCustomChests();
            _modHelper.Events.GameLoop.ReturnedToTitle += (sender, args) => HideAndSaveCustomChests();

            _modHelper.Events.Multiplayer.PeerContextReceived += OnPeerContextReceived;
            _modHelper.Events.Multiplayer.PeerDisconnected += OnPeerDisconnected;
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

        private async void OnPeerContextReceived(object sender, PeerContextReceivedEventArgs e)
        {
            _monitor.VerboseLog("OnPeerContextReceived");
            HideAndSaveCustomChests();
            await Task.Delay(1000); // hack :-(
            ReAddCustomChests();
        }

        private async void OnPeerDisconnected(object sender, PeerDisconnectedEventArgs e)
        {
            _monitor.VerboseLog("OnPeerDisconnected");
            await Task.Delay(1000); // hack :-(
            ReAddCustomChests();
        }

    }
}
