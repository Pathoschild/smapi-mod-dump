using System.Linq;
using ConvenientChests.CategorizeChests;
using ConvenientChests.CraftFromChests;
using ConvenientChests.StackToNearbyChests;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Locations;
using StardewValley.TerrainFeatures;

namespace ConvenientChests {
    public class ModEntry : Mod {
        public static   Config     Config        { get; private set; }
        internal static IModHelper StaticHelper  { get; private set; }
        internal static IMonitor   StaticMonitor { get; private set; }

        internal static void Log(string s, LogLevel l = LogLevel.Trace) => StaticMonitor.Log(s, l);

        public static StashToNearbyChestsModule StashNearby;
        public static CategorizeChestsModule    CategorizeChests;
        public static CraftFromChestsModule     CraftFromChests;

        public override void Entry(IModHelper helper) {
            Config = helper.ReadConfig<Config>();
            StaticMonitor = Monitor;
            StaticHelper  = Helper;

            SaveEvents.AfterLoad          += (sender, e) => LoadModules();
            SaveEvents.AfterReturnToTitle += (sender, e) => UnloadModules();
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