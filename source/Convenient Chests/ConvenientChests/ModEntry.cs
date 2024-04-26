/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aEnigmatic/StardewValley
**
*************************************************/

using ConvenientChests.CategorizeChests;
using ConvenientChests.CraftFromChests;
using ConvenientChests.StashToChests;
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
        public static StashFromAnywhereModule   StashFromAnywhere;

        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper) {
            Config = helper.ReadConfig<Config>();
            StaticMonitor = Monitor;
            StaticHelper  = Helper;

            helper.Events.GameLoop.GameLaunched    += (_, _) => RegisterSettings();
            helper.Events.GameLoop.SaveLoaded      += (_, _) => LoadModules();
            helper.Events.GameLoop.ReturnedToTitle += (_, _) => UnloadModules();
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
            
            StashFromAnywhere = new StashFromAnywhereModule(this);
            if (Config.StashAnywhere)
                StashFromAnywhere.Activate();
        }

        private void UnloadModules() {
            StashNearby.Deactivate();
            StashNearby = null;

            CategorizeChests.Deactivate();
            CategorizeChests = null;

            CraftFromChests.Deactivate();
            CraftFromChests = null;
            
            StashFromAnywhere.Deactivate();
            StashFromAnywhere = null;
        }

        public override object GetApi() {
            return new ModAPI();
        }

        private void RegisterSettings() {
            {
                // get Generic Mod Config Menu's API (if it's installed)
                var configMenu = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
                if (configMenu is null)
                    return;

                // register mod
                var options = new ModConfigOptions(configMenu, ModManifest);
                options.Register(
                                 reset: () => Config = new Config(),
                                 save: () => Helper.WriteConfig(Config)
                                );

                options.AddSection("Categorize chests");
                options.Add(() => Config.CategorizeChests,
                            value => Config.CategorizeChests = value,
                            "Active");

                options.AddSection("Craft from chests");
                options.Add(() => Config.CraftFromChests,
                            value => Config.CraftFromChests = value,
                            "Active");
                options.Add(() => Config.CraftRadius,
                            value => Config.CraftRadius = value,
                            "Radius");

                options.AddSection("Stash to nearby", "Allows for items to be stashed to chests in the player's vicinity.");
                options.Add(() => Config.StashToNearbyChests,
                            value => Config.StashToNearbyChests = value,
                            "Active");
                options.Add(() => Config.StashRadius,
                            value => Config.StashRadius = value,
                            "Radius");
                options.Add(() => Config.StashKey,
                            value => Config.StashKey = value,
                            "Stash to nearby key");

                options.AddSection("Stash from anywhere", "Allows for items to be stashed to any chest accessible to the player.");
                options.Add(() => Config.StashAnywhere,
                            value => Config.StashAnywhere = value,
                            "Active");
                options.Add(() => Config.StashAnywhereToFridge,
                            value => Config.StashAnywhereToFridge = value,
                            "Stash to fridge first?");
                options.Add(() => Config.StashToExistingStacks,
                            value => Config.StashToExistingStacks = value,
                            "Stash to existing stacks?");
                options.Add(() => Config.StashAnywhereKey,
                            value => Config.StashAnywhereKey = value,
                            "Stash from anywhere key");
            }
        }
    }
}