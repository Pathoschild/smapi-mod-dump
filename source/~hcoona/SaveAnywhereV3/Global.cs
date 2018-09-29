using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using SaveAnywhereV3.Service;
using StardewModdingAPI;
using StardewValley;

namespace SaveAnywhereV3
{
    internal class Global
    {
        public static Config Config { get; internal set; }

        public static IModHelper Helper { get; internal set; }

        public static IMonitor Monitor { get; internal set; }

        private static Lazy<IList<ISaveLoadService>> saveLoadServices =
            new Lazy<IList<ISaveLoadService>>(() =>
            {
                //return Assembly.GetAssembly(typeof(ISaveLoadService))
                //    .GetTypes()
                //    .Where(t => t.IsClass && !t.IsAbstract && typeof(ISaveLoadService).IsAssignableFrom(t))
                //    .Select(t => (ISaveLoadService)Activator.CreateInstance(t))
                //    .ToArray();

                // Global must before npc loading.
                return new ISaveLoadService[]
                {
                    new GlobalService(),
                    new ShippingBinService(),
                    new UniqueNpcPositionService(),
                    new PlayerPositionService()
                };
            });
        public static IList<ISaveLoadService> SaveLoadServices => saveLoadServices.Value;

        public static string GetSaveFilePath(string filename)
        {
            return Path.Combine(Helper.DirectoryPath, Game1.player.name, filename);
        }
    }
}
