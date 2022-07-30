/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Yariazen/BuildableGreenhouse
**
*************************************************/

using StardewModdingAPI;
using StardewModdingAPI.Events;
using static BuildableGreenhouse.Migrations.ModMigrations;

namespace BuildableGreenhouse.ModExtension
{
    public static partial class ModExtension
    {
        private static IMonitor Monitor;
        private static IModHelper Helper;
        private static IManifest Manifest;
        private static ModConfig Config;

        private static partial void SolidFoundationsExtension();
        private static partial void GenericModConfigMenuExtention();

        public static void InitializeExtensions(IModHelper helper, IMonitor monitor, IManifest manifest)
        {
            Monitor = monitor;
            Helper = helper;
            Manifest = manifest;
            Config = helper.ReadConfig<ModConfig>();

            Helper.Events.GameLoop.GameLaunched += initializeAPIs;

            SolidFoundationsExtension();
            GenericModConfigMenuExtention();
        }

        private static void initializeAPIs(object sender, GameLaunchedEventArgs e)
        {
            Monitor.Log($"{Manifest.UniqueID} hooking into Apis", LogLevel.Trace);
            if (Helper.ModRegistry.IsLoaded("PeacefulEnd.SolidFoundations"))
            {
                Monitor.Log($"{Manifest.UniqueID} hooking into SolidFoundations Api", LogLevel.Trace);
                SolidFoundationsApi = Helper.ModRegistry.GetApi<ISolidFoundationsApi>("PeacefulEnd.SolidFoundations");
            }
            if (Helper.ModRegistry.IsLoaded("spacechase0.GenericModConfigMenu"))
            {
                Monitor.Log($"{Manifest.UniqueID} hooking into GMCM Api", LogLevel.Trace);
                GenericModConfigMenuApi = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            }
            InitializeMigrations(Helper, Monitor, Manifest);
            ApplyMigration1(SolidFoundationsApi);
        }
    }
}
