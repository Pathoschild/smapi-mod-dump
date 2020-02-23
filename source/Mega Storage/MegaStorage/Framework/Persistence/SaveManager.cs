namespace MegaStorage.Framework.Persistence
{
    public class SaveManager
    {
        private readonly ISaver[] _savers;
        private readonly FarmhandMonitor _farmhandMonitor;

        public SaveManager(FarmhandMonitor farmhandMonitor, params ISaver[] savers)
        {
            _savers = savers;
            _farmhandMonitor = farmhandMonitor;
        }

        public void Start()
        {
            var saveAnywhereApi = MegaStorageMod.ModHelper.ModRegistry.GetApi<ISaveAnywhereApi>("Omegasis.SaveAnywhere");

            if (!(saveAnywhereApi is null))
            {
                saveAnywhereApi.BeforeSave += (sender, args) => HideAndSaveCustomChests();
                saveAnywhereApi.AfterSave += (sender, args) => ReAddCustomChests();
                saveAnywhereApi.AfterLoad += (sender, args) => LoadCustomChests();
            }

            MegaStorageMod.ModHelper.Events.GameLoop.Saving += (sender, args) => HideAndSaveCustomChests();
            MegaStorageMod.ModHelper.Events.GameLoop.Saved += (sender, args) => ReAddCustomChests();
            MegaStorageMod.ModHelper.Events.GameLoop.ReturnedToTitle += (sender, args) => HideAndSaveCustomChests();
            MegaStorageMod.ModHelper.Events.Multiplayer.PeerContextReceived += (sender, args) => HideAndSaveCustomChests();

            _farmhandMonitor.Start();
            _farmhandMonitor.OnPlayerAdded += ReAddCustomChests;
            _farmhandMonitor.OnPlayerRemoved += ReAddCustomChests;

            LoadCustomChests();
        }

        private void LoadCustomChests()
        {
            MegaStorageMod.ModMonitor.VerboseLog("LoadCustomChests");
            foreach (var saver in _savers)
            {
                saver.LoadCustomChests();
            }
        }

        private void ReAddCustomChests()
        {
            MegaStorageMod.ModMonitor.VerboseLog("ReAddCustomChests");
            foreach (var saver in _savers)
            {
                saver.ReAddCustomChests();
            }
        }

        private void HideAndSaveCustomChests()
        {
            MegaStorageMod.ModMonitor.VerboseLog("HideAndSaveCustomChests");
            foreach (var saver in _savers)
            {
                saver.HideAndSaveCustomChests();
            }
        }

    }
}
