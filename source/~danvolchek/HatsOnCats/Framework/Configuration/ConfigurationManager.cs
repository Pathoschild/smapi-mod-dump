/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/danvolchek/StardewMods
**
*************************************************/

using System.Collections.Generic;
using Microsoft.Xna.Framework;
using StardewModdingAPI;

namespace HatsOnCats.Framework.Configuration
{
    internal class ConfigurationManager
    {
        private readonly IEnumerable<IConfigurable> configurables;
        private readonly IModHelper helper;

        public ConfigurationManager(IModHelper helper, IEnumerable<IConfigurable> configurables)
        {
            this.helper = helper;
            this.configurables = configurables;
        }


        public void UpdateConfig()
        {
            ModConfig config = this.helper.ReadConfig<ModConfig>() ?? new ModConfig();
            config.Offsets = this.UpdateConfig(config.Offsets);
            this.helper.WriteConfig(config);
        }

        private IDictionary<string, IDictionary<Frame, Offset>> UpdateConfig(IDictionary<string, IDictionary<Frame, Offset>> offsetConfigurations)
        {
            IDictionary<string, IDictionary<Frame, Offset>> newConfiguration = new Dictionary<string, IDictionary<Frame, Offset>>();

            foreach (IConfigurable configurable in this.configurables)
            {
                if (offsetConfigurations.TryGetValue(configurable.Name, out IDictionary<Frame, Offset> configuration))
                {
                    configurable.Configuration = configuration;
                }

                newConfiguration[configurable.Name] = configurable.Configuration;
            }

            return newConfiguration;
        }
    }
}
