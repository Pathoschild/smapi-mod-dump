using SimpleSoundManager.Framework;
using SimpleSoundManager.Framework.Music;
using StardewModdingAPI;

namespace SimpleSoundManager
{
    /// <summary>
    /// Mod core.
    ///
    /// Needs testing.
    ///
    /// Seems like the current structure will require both content packs and a programmed mod to request when to play specific sounds. Interesting.
    /// </summary>
    public class ModCore : Mod
    {
        internal static IModHelper ModHelper;
        internal static IMonitor ModMonitor;
        internal static Config Config;
        public static MusicManager MusicManager;

        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            ModHelper = helper;
            ModMonitor = this.Monitor;
            Config = helper.ReadConfig<Config>();
            this.loadContentPacks();
            this.Helper.Events.GameLoop.OneSecondUpdateTicked += this.GameLoop_OneSecondUpdateTicked;

            this.Helper.Events.GameLoop.SaveLoaded += this.GameLoop_SaveLoaded;
        }

        private void GameLoop_SaveLoaded(object sender, StardewModdingAPI.Events.SaveLoadedEventArgs e)
        {
            
            //MusicManager.MusicPacks["Your Project Name"].PlaySound("toby fox - UNDERTALE Soundtrack - 01 Once Upon a Time");
            //DebugLog("PLAY SOME SOUNDS");
        }

        /// <summary>
        /// Update all music packs every second.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GameLoop_OneSecondUpdateTicked(object sender, StardewModdingAPI.Events.OneSecondUpdateTickedEventArgs e)
        {
            foreach(MusicPack pack in MusicManager.MusicPacks.Values)
            {
                pack.update();
            }
        }

        /// <summary>
        /// Loads all content packs for SimpleSoundManager
        /// </summary>
        private void loadContentPacks()
        {
            MusicManager = new MusicManager();
            foreach (IContentPack contentPack in this.Helper.ContentPacks.GetOwned())
            {
                this.Monitor.Log($"Reading content pack: {contentPack.Manifest.Name} {contentPack.Manifest.Version} from {contentPack.DirectoryPath}");
                MusicPack musicPack = new MusicPack(contentPack);
                MusicManager.addMusicPack(musicPack, true, true);
            }

            
        }

        /// <summary>
        /// Easy way to display debug logs when allowing for a check to see if they are enabled.
        /// </summary>
        /// <param name="s">The message to display.</param>
        public static void DebugLog(string s)
        {
            if (Config.EnableDebugLog)
                ModMonitor.Log(s);
        }
    }
}
