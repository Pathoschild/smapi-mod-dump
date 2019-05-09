using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;

namespace SadisticBundles
{
    public class ModEntry : Mod
    {
        static IModHelper hlp;
        private BundleInjector bundler;

        public override void Entry(IModHelper helper)
        {
            hlp = helper;
            bundler = new BundleInjector(helper, Monitor);
            var ccMan = new CommunityCenterManager(helper, Monitor, bundler);
            var stringer = new StringInjector(helper, Monitor);
            var cheats = new CheatManager(helper, Monitor);
            var achievs = new Acheivements(helper, Monitor);

            helper.Events.GameLoop.SaveLoaded += SaveLoaded;
            helper.Events.GameLoop.Saving += Saving;
            helper.Events.GameLoop.ReturnedToTitle += TitleReturn;

            helper.Content.AssetEditors.Add(bundler);
            helper.Content.AssetEditors.Add(stringer);
            helper.Content.AssetEditors.Add(cheats);

            helper.ConsoleCommands.Add("bundle", "Give all items for bundle.\n\nUsage: bundle <value>\n- value: the integer id of bundle in question.", this.GiveItems);
        }

        private void GiveItems(string command, string[] args)
        {
            var id = int.Parse(args[0]);
            var items = bundler.GetItems(id);
            foreach (var i in items)
            {
                Game1.player.addItemToInventory(i);
            }
        }

        const string saveKey = "sadistic-bundles";

        private void SaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            GameState.Current = Helper.Data.ReadSaveData<GameState>(saveKey);
            if (GameState.Current == null)
            {
                GameState.Current = new GameState();
            }
            InvalidateCache();
        }

        private void Saving(object sender, SavingEventArgs e)
        {
            Helper.Data.WriteSaveData(saveKey, GameState.Current);
        }

        private void TitleReturn(object sender, ReturnedToTitleEventArgs e)
        {
            GameState.Current = null;
            InvalidateCache();
        }

        public static void InvalidateCache()
        {
            hlp.Content.InvalidateCache("Data/Bundles");
            hlp.Content.InvalidateCache("Strings/UI");
            foreach(var ass in CheatManager.shopOpenAssets)
            {
                hlp.Content.InvalidateCache(ass);
            }
        }


    }

}
