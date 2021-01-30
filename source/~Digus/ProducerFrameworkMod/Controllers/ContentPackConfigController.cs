/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Digus/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProducerFrameworkMod.ContentPack;
using StardewModdingAPI;

namespace ProducerFrameworkMod.Controllers
{
    public class ContentPackConfigController
    {
        private static readonly Dictionary<string, ContentPackConfig> Repository = new Dictionary<string, ContentPackConfig>();

        /// <summary>
        /// Adds or replace the content pack global config.
        /// </summary>
        /// <param name="config">The config to be added or replaced.</param>
        /// <param name="modUniqueId">The mod unique id.</param>
        /// 
        public static void AddConfig(ContentPackConfig config, string modUniqueId)
        {
            Repository[modUniqueId] = config;
        }

        /// <summary>
        /// Get the content pack global config from the mod unique id.
        /// </summary>
        /// <param name="modUniqueId">The mod unique id</param>
        /// <returns>The config</returns>
        public static ContentPackConfig GetConfig(string modUniqueId)
        {
            Repository.TryGetValue(modUniqueId, out ContentPackConfig producerConfig);
            return producerConfig;
        }

        internal static LogLevel GetDefaultWarningLogLevel(string modUniqueId)
        {
            return GetConfig(modUniqueId)?.DefaultWarningsLogLevel ?? LogLevel.Warn;
        }
    }
}
