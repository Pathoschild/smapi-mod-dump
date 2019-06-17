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
            _modHelper.Events.GameLoop.SaveLoaded += (sender, args) => LoadNiceChests();
            _modHelper.Events.GameLoop.DayEnding += (sender, args) => HideAndSaveNiceChests();
            _modHelper.Events.GameLoop.DayStarted += (sender, args) => ReAddNiceChests();
            _modHelper.Events.GameLoop.ReturnedToTitle += (sender, args) => HideAndSaveNiceChests();

            _modHelper.Events.Multiplayer.PeerContextReceived += (sender, args) => HideAndSaveNiceChests();
            _modHelper.Events.Multiplayer.PeerDisconnected += (sender, args) => OnPeerDisconnected();
            _modHelper.Events.Multiplayer.ModMessageReceived += OnModMessageReceived;
        }

        private void LoadNiceChests()
        {
            _monitor.VerboseLog("LoadNiceChests");
            foreach (var saver in _savers)
            {
                saver.LoadNiceChests();
            }
            ReportIsLoaded();
        }

        private void ReAddNiceChests()
        {
            _monitor.VerboseLog("ReAddNiceChests");
            foreach (var saver in _savers)
            {
                saver.ReAddNiceChests();
            }
        }

        private void HideAndSaveNiceChests()
        {
            _monitor.VerboseLog("HideAndSaveNiceChests");
            foreach (var saver in _savers)
            {
                saver.HideAndSaveNiceChests();
            }
        }

        private void ReportIsLoaded()
        {
            _modHelper.Multiplayer.SendMessage("PlayerLoaded", "PlayerLoaded", new[] { "Alek.MegaStorage" });
        }

        private async void OnPeerDisconnected()
        {
            _monitor.VerboseLog("OnPeerDisconnected");
            await Task.Delay(1000); // hack :-(
            ReAddNiceChests();
        }

        private void OnModMessageReceived(object sender, ModMessageReceivedEventArgs e)
        {
            _monitor.VerboseLog($"OnModMessageReceived from {e.FromModID}: {e.Type}");
            if (e.FromModID != "Alek.MegaStorage" || e.Type != "PlayerLoaded")
                return;
            ReAddNiceChests();
        }

    }
}
