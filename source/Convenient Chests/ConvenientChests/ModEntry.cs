using ConvenientChests.CategorizeChests;
using ConvenientChests.CraftFromChests;
using ConvenientChests.StackToNearbyChests;
using StardewModdingAPI;

namespace ConvenientChests {
    /// <summary>The mod entry class loaded by SMAPI.</summary>
    public class ModEntry : Mod {
        public static   Config     Config        { get; private set; }
        internal static IModHelper StaticHelper  { get; private set; }
        internal static IMonitor   StaticMonitor { get; private set; }

        internal static void Log(string s, LogLevel l = LogLevel.Trace) => StaticMonitor.Log(s, l);

        public static StashToNearbyChestsModule StashNearby;
        public static CategorizeChestsModule    CategorizeChests;
        public static CraftFromChestsModule     CraftFromChests;

        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper) {
            Config = helper.ReadConfig<Config>();
            StaticMonitor = Monitor;
            StaticHelper  = Helper;

            helper.Events.GameLoop.SaveLoaded      += (sender, e) => LoadModules();
            helper.Events.GameLoop.ReturnedToTitle += (sender, e) => UnloadModules();
        }

        private void LoadModules() {
            StashNearby = new StashToNearbyChestsModule(this);
            if (Config.StashToNearbyChests)
                StashNearby.Activate();

            CategorizeChests = new CategorizeChestsModule(this);
            if (Config.CategorizeChests)
                CategorizeChests.Activate();

            CraftFromChests = new CraftFromChestsModule(this);
            if (Config.CraftFromChests)
                CraftFromChests.Activate();
        }

        private void UnloadModules() {
            StashNearby.Deactivate();
            StashNearby = null;
            
            CategorizeChests.Deactivate();
            CategorizeChests = null;
            
            CraftFromChests.Deactivate();
            CraftFromChests = null;
        }
    }
}