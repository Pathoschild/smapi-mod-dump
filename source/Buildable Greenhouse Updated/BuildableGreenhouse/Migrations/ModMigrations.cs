/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Yariazen/BuildableGreenhouse
**
*************************************************/

using BuildableGreenhouse.ModExtension;
using StardewModdingAPI;
using Migration1 = BuildableGreenhouse.Migrations.SpaceCore.MigrationExtension;

namespace BuildableGreenhouse.Migrations
{
    public static class ModMigrations
    {
        private static IMonitor Monitor;
        private static IModHelper Helper;
        private static IManifest Manifest;
        private static ModConfig Config;

        public static void InitializeMigrations(IModHelper helper, IMonitor monitor, IManifest manifest)
        {
            Monitor = monitor;
            Helper = helper;
            Manifest = manifest;
            Config = helper.ReadConfig<ModConfig>();
        }

        public static void ApplyMigration1(ISolidFoundationsApi SolidFoundationApi)
        {
            Migration1 migration1 = new Migration1(Monitor, SolidFoundationApi);
            Helper.Events.GameLoop.SaveLoaded += migration1.apply;
        }
    }
}