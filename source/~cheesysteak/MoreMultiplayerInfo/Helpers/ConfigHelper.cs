/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/cheesysteak/stardew-steak
**
*************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewModdingAPI;

namespace MoreMultiplayerInfo.Helpers
{
    public class ConfigHelper
    {
        private static ModConfigOptions _configOptions;

        public static IModHelper Helper { get; set; }

        private static bool _configFileUpdated;

        public static ModConfigOptions GetOptions()
        {
            if (_configOptions == null || _configFileUpdated)
            {
                _configOptions = Helper.ReadConfig<ModConfigOptions>();
                _configFileUpdated = false;
            }

            return _configOptions;
        }

        public static void SaveOptions(object config)
        {
            Helper.WriteConfig(config);

            _configOptions = null;
        }

        static ConfigHelper()
        {
            var watcher = new FileSystemWatcher(StardewModdingAPI.Constants.DataPath, "config.json");

            watcher.Changed += (o, e) => _configFileUpdated = true;

        }
    }
}
