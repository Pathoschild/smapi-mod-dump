using MegaStorage.Models;
using MegaStorage.Persistence;
using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace MegaStorage
{
    public class MegaStorageMod : Mod
    {
        public static MegaStorageMod Instance;

        public override void Entry(IModHelper modHelper)
        {
            Monitor.VerboseLog("Entry of MegaStorageMod");
            Instance = this;
            Helper.Events.GameLoop.GameLaunched += OnGameLaunched;
        }

        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            var convenientChestsApi = Helper.ModRegistry.GetApi<IConvenientChestsApi>("aEnigma.ConvenientChests");

            var spritePatcher = new SpritePatcher(Helper, Monitor);
            var itemPatcher = new ItemPatcher(Helper, Monitor);
            var menuChanger = new MenuChanger(Helper, Monitor);
            var saveManager = new SaveManager(Helper, Monitor,
                new FarmhandMonitor(Helper, Monitor),
                new InventorySaver(Helper, Monitor, convenientChestsApi),
                new FarmhandInventorySaver(Helper, Monitor, convenientChestsApi),
                new LocationSaver(Helper, Monitor, convenientChestsApi),
                new LocationInventorySaver(Helper, Monitor, convenientChestsApi));

            Helper.ReadConfig<ModConfig>();
            Helper.Content.AssetEditors.Add(spritePatcher);
            itemPatcher.Start();
            saveManager.Start();
            menuChanger.Start();
        }

    }
}
